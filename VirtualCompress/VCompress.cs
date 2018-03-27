using VirtualCompress.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using VirtualCompress.Tool;

namespace VirtualCompress
{
    /// <summary>
    /// 虚拟压缩包下载模块(VirtualCompressDownLoad)
    /// 作者:zx
    /// 时间:2016-8-9 10:58:21
    /// </summary>
    public class VCompress
    {

        private E_CompressConfig _config;

        private byte[] DefaultCRC = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 使用默认配置进行虚拟压缩
        /// </summary>
        public VCompress()
        {
            _config = new E_CompressConfig();
        }

        /// <summary>
        /// 根据配置进行虚拟压缩
        /// </summary>
        /// <param name="config"></param>
        public VCompress(E_CompressConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 上下文
        /// </summary>
        private HttpContext context= HttpContext.Current;

        /// <summary>
        /// 待下载的资料列表
        /// </summary>
        private IList<E_SourceFile> fileList { get; set; }


        private void HtmlWrite(string msg)
        {
            context.Response.Write(msg);
        }

        public void VirDownload(List<E_SourceFile> sourceList)
        {
            fileList = sourceList;
            //文件个数以及大小校验
            if (fileList.Count == 0)
            {
                ExitError("文件不存在"); return;
            }
            if (fileList.Sum(s => (long)s.FileSize) > int.MaxValue)
            {
                ExitError("文件过大,请减少文件个数后下载"); return;
            }
            try
            {
                ProcessSoftList();
                //根据资料集合构建
                E_VirtualZipStruct zipStruct = BuildZipStruct();
                //发送文件
                SendFile(zipStruct);
            }
            catch (OutOfMemoryException)
            {
                float a = GetFreeMemory(0);
                Util.SaveLog(_config.LogPath, $"当前内存不足,剩余内存为:{GetFreeMemory()}MB(排除了保留内存{_config.ReseverMemory / 1024 / 1024}GB)");
                ExitError("服务器忙...请稍后再试"); return;
            }
            catch (Exception ex)
            {
                Util.SaveLog(_config.LogPath, ex.StackTrace.ToString());
                ExitError("服务器异常,请稍后重试"); return;
            }
            

        }


        /// <summary>
        /// 处理资料列表
        /// </summary>
        private void ProcessSoftList()
        {
            foreach (var item in fileList)
            {
                FileInfo fi = new FileInfo(item.PhyFilePath);
                int SleepCnt = 10;//循环检测内存剩余计数器
                //判断是否还有剩余内存
                while (SleepCnt > 0 && GetFreeMemory(fi.Length) < 0)
                {
                    Thread.Sleep(1000);
                    SleepCnt--;
                    //超时抛异常
                    if (SleepCnt <= 0)
                    {                        
                        GC.Collect();
                        throw new OutOfMemoryException();
                    }
                }
                switch (_config.CRCLevel)
                {
                    case Const.EnumCRCLevel.Default:                        
                        break;
                    case Const.EnumCRCLevel.Auto:
                        if (item.CRC32 == DefaultCRC)
                        {
                            item.CRC32 = CRC.Crc32BytesForPath(item.PhyFilePath);
                        }
                        break;
                    case Const.EnumCRCLevel.RealTime:
                        item.CRC32 = CRC.Crc32BytesForPath(item.PhyFilePath);break;
                    default:
                        item.CRC32 = CRC.Crc32BytesForPath(item.PhyFilePath);break;
                }
                item.FileSize = (int)fi.Length;
            }
        }

        /// <summary>
        /// 获取剩余内存
        /// </summary>
        /// <param name="length">要减去的文件大小</param>
        /// <param name="reservedMem">保留内存</param>
        /// <returns>剩余内存量(MB)</returns>
        private float GetFreeMemory(long length = 0)
        {
            SystemInfo sysInfo = new SystemInfo();
            return (sysInfo.MemoryAvailable - length - _config.ReseverMemory) / 1024 / 1204;
        }

        /// <summary>
        /// 构建Zip结构
        /// </summary>
        /// <returns></returns>
        private E_VirtualZipStruct BuildZipStruct()
        {
            E_VirtualZipStruct zipStruct = new E_VirtualZipStruct();
            //定义头部和目录的集合(用于计算目录偏移量)
            List<E_FileAndDir> fdList = new List<E_FileAndDir>();
            //合计文件大小
            long totalFileSize = 0;
            //虚拟压缩包总大小(包含文件大小和头部,目录,尾部区域的字节)
            long virtualZipSize = 0;
            foreach (var file in fileList)
            {
                //1.构造头部区域
                E_LocalFileHeader header = FileHeaderInstance(file);
                zipStruct.Header.Add(new Tuple<byte[], string>(header.GetAllBytes(), file.PhyFilePath));
                virtualZipSize += header.GetAllBytes().Count();
                //2.构造目录部分
                E_CentralDirFileHeader dir = CentralDirInstance(file);
                //计算dir的偏移量
                if (fdList.Count > 0)
                {
                    dir.HeaderOffset = GetClassSize(fdList.Select(s => s.Header).ToList(), (int)totalFileSize);
                }
                zipStruct.Dir.AddRange(dir.GetAllBytes());
                //向头部和目录集合添加记录
                fdList.Add(new E_FileAndDir()
                {
                    Header = header,
                    Dir = dir
                });
                totalFileSize += file.FileSize;
            }
            E_EndCentralDirRecord endDir = EndDirInstance(fdList, (int)totalFileSize);
            zipStruct.End.AddRange(endDir.GetAllBytes());
            virtualZipSize += zipStruct.Dir.Count();
            virtualZipSize += zipStruct.End.Count();
            virtualZipSize += totalFileSize;
            zipStruct.ZipFileSize = (uint)virtualZipSize;
            return zipStruct;
        }

        #region 虚拟压缩包的读写,向客户端发送文件流的主要模块
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="zipStruct">Zip结构</param>
        private void SendFile(E_VirtualZipStruct zipStruct)
        {
            //设定默认字节流
            uint DefaultBuffer = 256 * 1024;
            //已经读取的文件流长度
            uint readOffset = 0;
            //缓冲区位移,用于判断是否该写入
            uint bfrOffset = 0;
            string saveFileName = string.Format("打包下载{0}等{1}个文件.zip", fileList[0].FileName, fileList.Count);
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachement;filename=" + HttpUtility.UrlEncode(saveFileName));
            context.Response.AddHeader("Content-Length", zipStruct.ZipFileSize.ToString());
            context.Response.AddHeader("Connection", "Keep-Alive");
            //重置写入文件的Buffer大小(用于重置小于1M的文件大小)
            byte[] bfrByte = ResetBuffer(ref readOffset, zipStruct.ZipFileSize, DefaultBuffer);
            #region 文件读取
            //1.读取zipXml中的data区域
            foreach (var item in zipStruct.Header)
            {
                //1-1.读取头部区域的头部字节部分
                {
                    //读取配置中的头部字节
                    byte[] headerByte = item.Item1;
                    //定义头部字节读取时的偏移量
                    ReadAndWrite(zipStruct.ZipFileSize, DefaultBuffer, ref readOffset, ref bfrOffset, ref bfrByte, headerByte);
                }
                //1-2.读取头部区域的文件流
                {
                    string path = item.Item2;
                    ReadAndWrite(zipStruct.ZipFileSize, DefaultBuffer, ref readOffset, ref bfrOffset, ref bfrByte, path);
                }
            }
            //2.读取zipXML中的DIR区域
            {
                byte[] DirByte = zipStruct.Dir.ToArray();
                ReadAndWrite(zipStruct.ZipFileSize, DefaultBuffer, ref readOffset, ref bfrOffset, ref bfrByte, DirByte);
            }
            //3.读取zipXML中的End区域
            {
                byte[] EndByte = zipStruct.End.ToArray();
                ReadAndWrite(zipStruct.ZipFileSize, DefaultBuffer, ref readOffset, ref bfrOffset, ref bfrByte, EndByte);
            }

            context.Response.Close();
            context.Response.End();
            #endregion
        }

        /// <summary>
        /// 读写
        /// </summary>
        /// <param name="ZipFileSize">Zip文件的大小</param>
        /// <param name="DefaultBuffer">默认的Buffer大小</param>
        /// <param name="used">已读取的文件长度</param>
        /// <param name="bfrOffset">缓冲区偏移量</param>
        /// <param name="bfrByte">缓冲器字节</param>
        /// <param name="filePath">要写入缓冲区的文件路径</param>
        private void ReadAndWrite(uint ZipFileSize, uint DefaultBuffer, ref uint used, ref uint bfrOffset, ref byte[] bfrByte, string filePath)
        {
            using (FileStream fsUnit = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                uint headerOffset = 0;
                while (context.Response.IsClientConnected)
                {
                    FillUnit(fsUnit, ref headerOffset, ref bfrOffset, ref bfrByte);
                    if (bfrOffset == 0)
                    {
                        Sending(bfrByte, 0, bfrByte.Length);
                        bfrByte = ResetBuffer(ref used, ZipFileSize, DefaultBuffer);
                    }
                    if (headerOffset == fsUnit.Length)
                    {
                        break;
                    }
                }
                fsUnit.Close();
            }
        }

        /// <summary>
        /// 读写
        /// </summary>
        /// <param name="ZipFileSize">Zip文件的大小</param>
        /// <param name="DefaultBuffer">默认的Buffer大小</param>
        /// <param name="used">已读取的文件长度</param>
        /// <param name="bfrOffset">缓冲区偏移量</param>
        /// <param name="bfrByte">缓冲器字节</param>
        /// <param name="unitByte">要写入的单元字节</param>
        private void ReadAndWrite(uint ZipFileSize, uint DefaultBuffer, ref uint used, ref uint bfrOffset, ref byte[] bfrByte, byte[] unitByte)
        {
            uint unitOffset = 0;
            while (context.Response.IsClientConnected)
            {
                //装填缓冲区,计算对应单元的偏移量,缓冲区偏移量和缓冲区字节
                FillUnit(unitByte, ref unitOffset, ref bfrOffset, ref bfrByte);
                //若缓冲区偏移量为0,则表示缓冲器已满,得写入文件
                if (bfrOffset == 0)
                {
                    //将缓冲区字节发送出去
                    Sending(bfrByte, 0, bfrByte.Length);
                    //重置缓冲区域的字节数大小
                    bfrByte = ResetBuffer(ref used, ZipFileSize, DefaultBuffer);
                }
                //若读取数和头部偏移相等,则表示已经读完头部,但是缓冲区未满
                if (unitOffset == unitByte.Count())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 向客户端发送文件流
        /// </summary>
        /// <param name="writeB">发送的缓冲区</param>
        /// <param name="offset">缓冲区偏移量</param>
        /// <param name="length">读取长度</param>
        private void Sending(byte[] writeB, uint offset, int length)
        {
            if (context.Response.IsClientConnected)
            {
                context.Response.OutputStream.Write(writeB, (int)offset, length);
                context.Response.Flush();
                context.Response.Clear();
            }
            else
            {
                context.Response.Write("下载失败");
                context.Response.End();
            }
        }

        /// <summary>
        /// 根据zip的文件大小和已分配过的情况确定缓冲区大小
        /// </summary>
        /// <param name="used">已读取的字节数</param>
        /// <param name="zipSize">zip文件的总字节数</param>
        /// <param name="defaultbuffer">默认的Buffer大小</param>
        /// <returns></returns>
        static byte[] ResetBuffer(ref uint used, uint zipSize, uint defaultbuffer = 1 * 1024 * 1024)
        {
            uint over = zipSize - used;
            byte[] b;
            if (over > defaultbuffer)
            {
                b = new byte[defaultbuffer];
                used += defaultbuffer;
            }
            else
            {
                b = new byte[over];
                used = zipSize;
            }
            return b;

        }

        /// <summary>
        /// 将byte[]装入到缓冲区,并计算重置下一个缓冲区大小
        /// </summary>
        /// <param name="fs">待读取的文件流</param>
        /// <param name="unitOffset">源文件的偏移量</param>
        /// <param name="bfrOffset">缓冲区的偏移量</param>
        /// <param name="writeB">缓冲区Byte[]</param>
        static void FillUnit(FileStream fs, ref uint unitOffset, ref uint bfrOffset, ref byte[] writeB)
        {
            uint fileSize = (uint)fs.Length;
            uint bufferOver = (uint)writeB.Length - bfrOffset;
            uint count = 0;
            uint fileOverSize = fileSize - unitOffset;
            uint _bfrOffsetOld = bfrOffset;
            uint _unitOffsetOld = unitOffset;
            //若buffer中剩余字节数大于文件的剩余读取长度,则读取文件长度,否则,读满buffer
            if (bufferOver > fileOverSize)
            {
                count = fileOverSize;
                bfrOffset += fileOverSize;
            }
            else
            {
                count = bufferOver;
                bfrOffset = 0;
            }
            unitOffset += count;
            byte[] fsTmp = new byte[count];
            fs.Seek(_unitOffsetOld, SeekOrigin.Begin);
            fs.Read(fsTmp, 0, (int)count);
            for (int i = 0; i < fsTmp.Length; i++)
            {
                writeB[_bfrOffsetOld + i] = fsTmp[i];
            }
        }

        /// <summary>
        /// 将byte[]装入到缓冲区,并计算重置下一个缓冲区大小
        /// </summary>
        /// <param name="srcByte">源文件字节</param>
        /// <param name="unitOffset">源文件的偏移量</param>
        /// <param name="bfrOffset">缓冲区的偏移量</param>
        /// <param name="writeB">缓冲区Byte[]</param>
        static void FillUnit(byte[] srcByte, ref uint unitOffset, ref uint bfrOffset, ref byte[] writeB)
        {
            uint srcSize = (uint)srcByte.Length;
            uint bufferOver = (uint)writeB.Length - bfrOffset;
            uint srcOverSize = srcSize - unitOffset;
            uint count = 0;
            uint _bfrOffsetOld = bfrOffset;
            uint _unitOffsetOld = unitOffset;
            //若buffer中剩余字节数大于源字节的剩余读取长度,则读取源字节长度,否则,读满buffer
            if (bufferOver > srcOverSize)
            {
                count = srcOverSize;
                bfrOffset += srcOverSize;
            }
            else
            {
                count = bufferOver;
                bfrOffset = 0;
            }
            unitOffset += count;
            byte[] srcTmp = new byte[count];
            for (int i = 0; i < count; i++)
            {
                srcTmp[i] = srcByte[_unitOffsetOld + i];
            }

            for (int i = 0; i < srcTmp.Length; i++)
            {
                writeB[_bfrOffsetOld + i] = srcTmp[i];
            }
        }
        #endregion

        #region Zip结构实例化
        /// <summary>
        /// zip文件头的实例化
        /// </summary>
        /// <param name="file">资料实体</param>
        /// <returns></returns>
        private E_LocalFileHeader FileHeaderInstance(E_SourceFile file)
        {
            E_LocalFileHeader l = new E_LocalFileHeader();
            l.MinExtractVersion = BitConverter.GetBytes((short)0x000a);
            l.PurposeFlag = BitConverter.GetBytes((short)0x0800);
            //压缩方法使用默认
            //文件最后修改时间和日期不用管
            l.CRC32 = file.CRC32;
            l.CmpSize = BitConverter.GetBytes((int)file.FileSize);
            l.UnCmpSize = BitConverter.GetBytes((int)file.FileSize);
            l.FileNameLength = BitConverter.GetBytes((short)Encoding.UTF8.GetBytes(file.FileName).Length);
            //文件扩展区长度默认为0即可
            l.FileName = Encoding.UTF8.GetBytes(file.FileName);
            //数据区填充null
            l.ExtraField = null;
            return l;
        }

        /// <summary>
        /// zip中央目录的实例化
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private E_CentralDirFileHeader CentralDirInstance(E_SourceFile file)
        {
            E_CentralDirFileHeader c = new E_CentralDirFileHeader();
            c.MadeVersion = BitConverter.GetBytes((short)0x003F);
            c.MinExtractVersion = BitConverter.GetBytes((short)0x000a);
            c.PurposeFlag = BitConverter.GetBytes((short)0x0800);
            //压缩方法默认0,文件修改时间和日期默认0
            c.CRC32 = file.CRC32;
            c.CmpSize = BitConverter.GetBytes((int)file.FileSize);
            c.UnCmpSize = BitConverter.GetBytes((int)file.FileSize);
            c.FileNameLength = BitConverter.GetBytes((short)Encoding.UTF8.GetBytes(file.FileName).Length);
            //扩展域长度,文件注释长度,开始位置的磁盘编号,为0即可
            c.ExternalFileAttr = BitConverter.GetBytes(0x00000020);
            //本地文件头的相对位移具体地方再做计算
            c.FileName = Encoding.UTF8.GetBytes(file.FileName);
            //扩展域和文件注释内容为null即可
            return c;
        }

        /// <summary>
        /// Zip尾部目录的实例化
        /// </summary>
        /// <param name="fdList"></param>
        /// <param name="totalFileSize"></param>
        /// <returns></returns>
        private E_EndCentralDirRecord EndDirInstance(List<E_FileAndDir> fdList, int totalFileSize)
        {
            E_EndCentralDirRecord e = new E_EndCentralDirRecord();
            //当前磁盘编号默认0
            //核心目录开始位置的磁盘编号也为0
            e.DirNum = BitConverter.GetBytes((short)fdList.Count);
            e.DirStructCnt = e.DirNum;
            e.DirSize = GetClassSize(fdList.Select(s => s.Dir).ToList());
            e.DirOffsefForArchive = GetClassSize(fdList.Select(s => s.Header).ToList(), totalFileSize);
            return e;
        }

        /// <summary>
        /// 异常退出
        /// </summary>
        /// <param name="msg">异常信息</param>
        private void ExitError(string msg)
        {
            context.Response.Write(msg);
            //context.Response.Close();
            context.Response.End();
        }
        #endregion

        #region 偏移量计算部分
        /// <summary>
        /// 获取目录的偏移地址(头部的长度集合)
        /// </summary>
        /// <param name="list">头部</param>
        /// <returns></returns>
        static byte[] GetClassSize(List<E_LocalFileHeader> list, int fileSize = 0)
        {
            if (list == null || list.Count == 0)
                return new byte[] { 0x00, 0x00, 0x00, 0x00 };
            else
            {
                int cnt = 0;
                foreach (var item in list)
                {
                    cnt += item.GetAllBytes().Count();
                }
                cnt += fileSize;
                return BitConverter.GetBytes(cnt);
            }
        }

        /// <summary>
        /// 获取目录的长度
        /// </summary>
        /// <param name="list">目录集合</param>
        /// <returns></returns>
        static byte[] GetClassSize(List<E_CentralDirFileHeader> list)
        {
            if (list == null)
                return new byte[] { 0x00, 0x00, 0x00, 0x00 };
            else
            {
                int cnt = 0;
                foreach (var item in list)
                {
                    cnt += item.GetAllBytes().Count();
                }
                return BitConverter.GetBytes(cnt);
            }
        }
        #endregion


        /// <summary>
        /// 计算文件的crc32值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetCRCBytesForFile(string filePath)
        {
            return CRC.Crc32BytesForPath(filePath);
        }
    }

}