using System.Collections.Generic;
using System.Text;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// Zip核心目录类
    /// 作者:赵旭
    /// 时间:2016-8-9 10:31:10
    /// </summary>
    internal class E_CentralDirFileHeader
    {
        /// <summary>
        /// 类转字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetAllBytes()
        {
            List<byte> blist = new List<byte>();
            blist.AddRange(this.Signature);
            blist.AddRange(this.MadeVersion);
            blist.AddRange(this.MinExtractVersion);
            blist.AddRange(this.PurposeFlag);
            blist.AddRange(this.CmpMethod);
            blist.AddRange(this.FileModifyTime);
            blist.AddRange(this.FileModifyDate);
            blist.AddRange(this.CRC32);
            blist.AddRange(this.CmpSize);
            blist.AddRange(this.UnCmpSize);
            blist.AddRange(this.FileNameLength);
            blist.AddRange(this.ExtraLength);
            blist.AddRange(this.FileNotesLength);
            blist.AddRange(this.DiskNumForStart);
            blist.AddRange(this.InternalFileAttr);
            blist.AddRange(this.ExternalFileAttr);
            blist.AddRange(this.HeaderOffset);
            blist.AddRange(this.FileName);
            if (this.ExtraField != null)
            {
                blist.AddRange(Encoding.Default.GetBytes(this.ExtraField));
            }
            return blist.ToArray();
        }

        /// <summary>
        /// 核心目录文件Header标识(4字节)
        /// </summary>
        public readonly byte[] Signature = new byte[4] { 0x50, 0x4B, 0x01, 0x02 };

        /// <summary>
        /// 压缩时所用的pkware版本(2字节)
        /// </summary>
        public byte[] MadeVersion = new byte[2] { 0x00, 0x00 };
        /// <summary>
        /// 解压文件所需最低版本(2字节)
        /// </summary>
        public byte[] MinExtractVersion = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 通用标记位(2字节)
        /// </summary>
        public byte[] PurposeFlag = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 压缩方法(2字节)
        /// </summary>
        public byte[] CmpMethod = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 文件最后修改时间(2字节)
        /// </summary>
        public byte[] FileModifyTime = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 文件最后修改日期(2字节)
        /// </summary>
        public byte[] FileModifyDate = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 文件crc32校验值(4字节)
        /// </summary>
        public byte[] CRC32 = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 压缩后文件大小(4字节)
        /// </summary>
        public byte[] CmpSize = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 未压缩时文件大小(4字节)
        /// </summary>
        public byte[] UnCmpSize = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 文件名长度(2字节)
        /// </summary>
        public byte[] FileNameLength = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 扩展区长度(2字节)
        /// </summary>
        public byte[] ExtraLength = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 文件注释长度(2字节)
        /// </summary>
        public byte[] FileNotesLength = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 文件开始位置的磁盘编号(2字节)
        /// </summary>
        public byte[] DiskNumForStart = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 内部文件属性(2字节)
        /// </summary>
        public byte[] InternalFileAttr = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 外部文件属性(4字节)
        /// </summary>
        public byte[] ExternalFileAttr = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 本地文件Header的相对位移(4字节)
        /// </summary>
        public byte[] HeaderOffset = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 文件名(n字节)
        /// </summary>
        public byte[] FileName = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 扩展域(由扩展长度决定)
        /// </summary>
        public string ExtraField = null;

        /// <summary>
        /// 文件注释内容(由文件注释长度决定)
        /// </summary>
        public string FileNotice = null;
    }
}