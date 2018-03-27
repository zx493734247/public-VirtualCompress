namespace VirtualCompress.Entity
{
    /// <summary>
    /// 常量区
    /// </summary>
    public class Const
    {
        /// <summary>
        /// CRC32校验等级
        /// </summary>
        public enum EnumCRCLevel
        {
            /// <summary>
            /// 默认,从传入中读取crc32,若没有,则不计算填充crc32
            /// </summary>
            Default=0,
            /// <summary>
            /// 自动,默认从传入中获取,传入为空时,进行CRC32计算
            /// </summary>
            Auto=1,
            /// <summary>
            /// 实时计算,无论传入的参数中是否含有crc,都会强制校验一遍
            /// </summary>
            RealTime=2,
        }

        /// <summary>
        /// 保留内存(CRC32会占用大量内存),默认保留系统1G内存
        /// </summary>
        public long ReserverMemory { get; set; } = 1 * 1024 * 1024 * 1024L;

        /// <summary>
        /// 日志保存路径,用于记录压缩出错存放的位置
        /// </summary>
        public string LogPath { get; set; }
    }
}
