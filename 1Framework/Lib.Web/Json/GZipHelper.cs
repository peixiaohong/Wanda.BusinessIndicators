using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Web.Json
{
    public enum UseGZip
    {
        NO = 0,
        Both = 1,
        Param = 2,
        ReturnData = 3,
    }
    public class GZipHelper
    {
        /// <summary>  
        /// GZip压缩  
        /// </summary>  
        /// <param name="rawData"></param>  
        /// <returns></returns>  
        public static string Compress(string jsonData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    var vs = Encoding.UTF8.GetBytes(jsonData);
                    compressedzipStream.Write(vs, 0, vs.Length);
                    compressedzipStream.Close();
                    return Convert.ToBase64String( ms.ToArray());
                }
            }

        }
        /// <summary>  
        /// 将传入的二进制字符串资料以GZip算法解压缩  
        /// </summary>  
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>  
        /// <returns>原始未压缩字符串</returns>  
        public static string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
                return Encoding.UTF8.GetString(Decompress(zippedData));
            }
        }
        /// <summary>  
        /// ZIP解压  
        /// </summary>  
        /// <param name="zippedData"></param>  
        /// <returns></returns>  
        private static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }

    }
}
