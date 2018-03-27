using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualCompress.Entity
{
    public static class E_Const
    {
        /// <summary>
        /// CRC配置等级
        /// </summary>
        public enum EnumCRCLevel
        {
            /// <summary>
            /// 默认配置,如果传入值存在crc,则写入到zip中.没有则为空
            /// </summary>
            Default = 0,
            /// <summary>
            /// 实时计算,不管是否传入crc32,永远根据文件进行实时计算crc32
            /// </summary>
            RealTime=1,
        }

    }
}
