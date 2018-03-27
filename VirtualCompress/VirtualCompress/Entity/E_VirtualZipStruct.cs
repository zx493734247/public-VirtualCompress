using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// 虚拟Zip结构
    /// 作者:赵旭
    /// 时间:2016-8-9 10:46:23
    /// </summary>
    internal class E_VirtualZipStruct
    {
        internal E_VirtualZipStruct()
        {
            Header = new List<Tuple<byte[], string>>();
            Dir = new List<byte>();
            End = new List<byte>();
        }
        /// <summary>
        /// 头部区域集合
        /// Tuple->Item1:头部字节,Tuple->Item2:文件路径
        /// </summary>
        public List<Tuple<byte[], string>> Header { get; set; }
        /// <summary>
        /// 目录字节集合
        /// </summary>
        public List<byte> Dir { get; set; }
        /// <summary>
        /// 目录结束字节集合
        /// </summary>
        public List<byte> End { get; set; }
        /// <summary>
        /// 虚拟压缩后的Zip文件的大小
        /// </summary>
        public uint ZipFileSize { get; set; }
    }
}