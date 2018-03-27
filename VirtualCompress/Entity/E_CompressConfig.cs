using static VirtualCompress.Entity.Const;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// 文件压缩模块的配置
    /// </summary>
    public class E_CompressConfig
    {
        /// <summary>
        /// 配置文件crc校验标准
        /// </summary>
        public EnumCRCLevel CRCLevel { get; set; } = EnumCRCLevel.Default;

        /// <summary>
        /// 配置保留内存(默认1G)
        /// 小文件的压缩一般不需要配置
        /// </summary>
        public long ReseverMemory { get; set; } = 1 * 1024 * 1024 * 1024L;

        /// <summary>
        /// 虚拟压缩包出错时日志存放路径(物理路径)
        /// </summary>
        public string LogPath { get; set; }
    }
}
