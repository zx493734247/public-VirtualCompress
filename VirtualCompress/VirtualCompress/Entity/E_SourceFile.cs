using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// 待压缩的源文件
    /// </summary>
    public class E_SourceFile
    {
        /// <summary>
        /// 文件名(带扩展名)
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 文件物理路径
        /// </summary>
        public string PhyFilePath { get; set; }
        /// <summary>
        /// 文件的CRC32值
        /// </summary>
        public byte[] CRC32 { get; set; }
    }
}
