using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// Zip头部和核心目录部分的合集
    /// 用于计算核心目录中的偏移量
    /// </summary>
    internal class E_FileAndDir
    {
        public E_LocalFileHeader Header { get; set; }
        public E_CentralDirFileHeader Dir { get; set; }
    }
}
