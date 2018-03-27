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
        public long FileSize { get; set; }
        /// <summary>
        /// 文件物理路径
        /// </summary>
        public string PhyFilePath { get; set; }
        /// <summary>
        /// 文件的CRC32值
        /// </summary>
        public byte[] CRC32 { get; set; } = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
    }
}
