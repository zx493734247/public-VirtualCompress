using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// Zip文件头类
    /// 作者:赵旭
    /// 时间:2016-8-9 10:30:48
    /// </summary>
    internal class E_LocalFileHeader
    {
        /// <summary>
        /// 类转字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetAllBytes()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(this.Signature);
            byteList.AddRange(this.MinExtractVersion);
            byteList.AddRange(this.PurposeFlag);
            byteList.AddRange(this.CmpMethod);
            byteList.AddRange(this.FileModifyTime);
            byteList.AddRange(this.FileModifyDate);
            byteList.AddRange(this.CRC32);
            byteList.AddRange(this.CmpSize);
            byteList.AddRange(this.UnCmpSize);
            byteList.AddRange(this.FileNameLength);
            byteList.AddRange(this.ExtraLength);
            byteList.AddRange(this.FileName);
            if (this.ExtraField != null)
            {
                BinaryReader r = new BinaryReader(this.ExtraField);
                r.BaseStream.Seek(0, SeekOrigin.Begin);
                byteList.AddRange(r.ReadBytes((int)r.BaseStream.Length));
            }
            return byteList.ToArray();
        }
        /// <summary>
        /// 头文件标识(4字节)
        /// </summary>
        public readonly byte[] Signature = new byte[4] { 0x50, 0x4B, 0x03, 0x04 };

        /// <summary>
        /// 解压文件所需最低版本(2字节)
        /// </summary>
        public byte[] MinExtractVersion = new byte[2] { 0x0a, 0x00 };

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
        /// 文件名(n字节)
        /// </summary>
        public byte[] FileName;

        /// <summary>
        /// 扩展区(数据区)
        /// </summary>
        public Stream ExtraField
        {
            get; set;
        }
    }
}