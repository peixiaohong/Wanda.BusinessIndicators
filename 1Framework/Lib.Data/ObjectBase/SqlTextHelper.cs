using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{
    public static class SqlTextHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>方便在Adapter更新数据之前做检查， 做了一层封装</remarks>
        public static string SafeQuote(string data)
        {
            string result = data.Replace("'", "''");
            return result;
        }

        /// <summary>
        /// 将LIKE对应的查询子句转义。将语句中的%、[、_转义
        /// </summary>
        /// <param name="likeString"></param>
        /// <param name="omitStartAndEnd">true 表示忽略likeString首尾的特殊字符的转义</param>
        /// <returns></returns>
        public static string EscapeLikeString(string likeString, bool omitStartAndEnd = true)
        {
            string result = likeString;

            if (omitStartAndEnd)
            {
                char[] delimiters = "%_".ToCharArray();

                int startTrimedLength, endTrimedLength;
                result = result.TrimStart(delimiters);
                startTrimedLength = likeString.Length - result.Length;

                result = result.TrimEnd(delimiters);
                endTrimedLength = likeString.Length - result.Length - startTrimedLength;

                // 保留首尾的特殊字符
                result = likeString.Substring(0, startTrimedLength)
                    + InnerEscapeLikeString(result)
                    + likeString.Substring(likeString.Length - endTrimedLength, endTrimedLength);
            }

            else
            {
                result = InnerEscapeLikeString(result);
            }

            return result;
        }

        private static string InnerEscapeLikeString(string likeString)
        {
            string result = likeString;
            if (string.IsNullOrEmpty(result) == false)
            {
                result = result.Replace("[", "[[]");
                result = result.Replace("%", "[%]");
                result = result.Replace("_", "[_]");
            }

            return result;
        }
    }
}
