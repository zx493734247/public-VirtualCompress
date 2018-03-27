using static VirtualCompress.Entity.E_Const;

namespace VirtualCompress.Entity
{
    /// <summary>
    /// 文件压缩模块的配置
    /// </summary>
    public class E_CompressConfig
    {
        /// <summary>
        /// CRC计算等级
        /// </summary>
        public EnumCRCLevel CRCLevel { get; set; } = EnumCRCLevel.Default;

        /// <summary>
        /// 保留内存的容量字节(CRC实时计算需要占用内存)默认1G
        /// </summary>
        public long ReserveMemory { get; set; } = 1 * 1024 * 1024 * 1024L;

        /// <summary>
        /// 错误日志保存路径(绝对地址)
        /// </summary>
        public string ErrorLogSavePath { get; set; }

    }
}
