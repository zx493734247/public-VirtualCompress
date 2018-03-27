using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualCompress.Entity;

namespace VirtualCompress
{
    internal class Test
    {
        public static IList<E_SourceFile> TestFileList()
        {
            IList<E_SourceFile> fileList = new List<E_SourceFile>();
            fileList.Add(new E_SourceFile()
            {
                FileName = "手写的从前.flac",
                PhyFilePath = "{%uploaddir%}1.flac",
                FileSize = 32299126,
            });
            fileList.Add(new E_SourceFile()
            {
                FileName = "美人鱼.flac",
                PhyFilePath = "{%uploaddir%}2.flac",
                FileSize = 25973000,
            });
            return fileList;
        }
    }
}
