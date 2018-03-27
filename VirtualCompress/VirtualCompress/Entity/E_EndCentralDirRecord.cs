using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// Zip目录结束标识
    /// 作者:赵旭
    /// 时间:2016-8-9 10:31:28
    /// </summary>
    internal class E_EndCentralDirRecord
    {
        /// <summary>
        /// 类转字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetAllBytes()
        {
            List<byte> blist = new List<byte>();
            blist.AddRange(this.Signature);
            blist.AddRange(this.DiskNum);
            blist.AddRange(this.DiskNumForDirStart);
            blist.AddRange(this.DirNum);
            blist.AddRange(this.DirStructCnt);
            blist.AddRange(this.DirSize);
            blist.AddRange(this.DirOffsefForArchive);
            blist.AddRange(this.NotesLength);
            return blist.ToArray();
        }

        /// <summary>
        /// 核心目录文件Header标识(4字节)
        /// </summary>
        public readonly byte[] Signature = new byte[] { 0x50, 0x4b, 0x05, 0x06 };

        /// <summary>
        /// 当前磁盘编号(2字节)
        /// </summary>
        public byte[] DiskNum = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 核心目录开始位置的磁盘编号(2字节)
        /// </summary>
        public byte[] DiskNumForDirStart = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 该磁盘上所记录的核心目录数量(2字节)
        /// </summary>
        public byte[] DirNum = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 核心目录结构总数(2字节)
        /// </summary>
        public byte[] DirStructCnt = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 核心目录的大小(4字节)注:网上2字节有误
        /// </summary>
        public byte[] DirSize = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 核心目录开始位置相对于archive开始的位移(4字节)
        /// </summary>
        public byte[] DirOffsefForArchive = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 注释长度(2字节)
        /// </summary>
        public byte[] NotesLength = new byte[2] { 0x00, 0x00 };

        /// <summary>
        /// 注释内容(由注释长度值决定)
        /// </summary>
        public string Notes { get; set; }
    }
}