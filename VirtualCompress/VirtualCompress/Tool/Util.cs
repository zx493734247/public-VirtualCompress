using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace VirtualCompress.Tool
{
    /// <summary>
    /// 虚拟压缩包工具类
    /// 作者:赵旭
    /// 时间:2016-8-9 10:39:01
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Hex转String显示的Hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes)
        {
            string byteStr = string.Empty;
            if (bytes != null || bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    byteStr += string.Format("{0:X2}", bytes[i]);
                }
                //foreach (var item in bytes)
                //{
                //    byteStr += string.Format("{0:X2}", item);
                //}
            }
            return byteStr;
        }

        /// <summary>
        /// String转Hex
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToHex(this string str)
        {
            if (str.Length % 2 != 0) return null;
            int len = str.Length / 2;
            byte[] bytes = new byte[len];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new string(new char[] { str[j], str[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        /// <summary>
        /// string中的Hex转字节
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private static byte HexToByte(string hex)
        {
            byte tt = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return tt;
        }
        
        public static void SaveLog(string logPath,string msg)
        {
            File.AppendAllText(logPath, msg);
        }
    }
}