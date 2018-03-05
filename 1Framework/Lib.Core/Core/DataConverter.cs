using System;
using System.Xml.Linq;

namespace Lib.Core
{
    /// <summary>
    /// 提供字符串与枚举类型的转换，TimeSpan与整形的转换。
    /// </summary>
    /// <remarks>提供字符串和枚举、TimeSpan的转换
    /// </remarks>
    public static class DataConverter
    {
        /// <summary>
        /// 类型转换，提供字符串与枚举型、TimeSpan与整型之间的转换
        /// </summary>
        /// <typeparam name="TSource">源数据的类型</typeparam>
        /// <typeparam name="TResult">目标数据的类型</typeparam>
        /// <param name="srcValue">源数据的值</param>
        /// <returns>类型转换结果</returns>
        /// <remarks>
        /// 数据转换，主要调用系统Convert类的ChangeType方法，但是对于字符串与枚举，整型与TimeSpan类型之间的转换，进行了特殊处理。
        /// <seealso cref="MCS.Library.Core.XmlHelper"/>
        /// </remarks>
        public static TResult ChangeType<TSource, TResult>(TSource srcValue)
        {
            return (TResult)ChangeType(srcValue, typeof(TResult));
        }

        /// <summary>
        /// 字符串与枚举型、TimeSpan与整型之间转换的方法。
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <param name="srcValue">源数据的值</param>
        /// <param name="targetType">目标数据类型</param>
        /// <returns>类型转换后的结果</returns>
        /// <remarks>字符串与枚举型、TimeSpan与整型之间转换的方法。
        /// <seealso cref="MCS.Library.Core.XmlHelper"/>
        /// </remarks>
        public static object ChangeType<TSource>(TSource srcValue, System.Type targetType)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(targetType != null, "targetType");
            bool dealed = false;
            object result = null;
            System.Type srcType = typeof(TSource);

            if (srcType == typeof(object))
            {
                if (srcValue != null)
                    srcType = srcValue.GetType();
            }

            if (targetType.IsEnum)
            {
                if (srcType == typeof(string) || srcType == typeof(int))
                {
                    result = Enum.Parse(targetType, srcValue.ToString());
                    dealed = true;
                }
            }
            else if (targetType == typeof(XElement))
            {
                if (srcType == typeof(string))
                {
                    result = (srcValue == null) ? null : XElement.Parse(srcValue.ToString());
                    dealed = true;
                }
            }
            else if (targetType == typeof(TimeSpan))
            {
                if (srcType == typeof(TimeSpan))
                    result = srcValue;
                else
                    result = TimeSpan.FromSeconds((double)Convert.ChangeType(srcValue, typeof(double)));

                dealed = true;
            }
            else if (targetType == typeof(bool) && srcType == typeof(string))
            {
                result = StringToBool(srcValue.ToString(), out dealed);
            }
            else if (targetType == typeof(System.String) && srcType == typeof(System.Guid))
            {
                result = srcValue.ToString();
                dealed = true;
            }


            if (dealed == false)
            {
                if (targetType != typeof(object) && targetType.IsAssignableFrom(srcType))
                    result = srcValue;
                else
                    result = Convert.ChangeType(srcValue, targetType);
            }

            return result;
        }

        private static bool StringToBool(string srcValue, out bool dealed)
        {
            bool result = false;
            dealed = false;

            if (srcValue.Length > 0)
            {
                if (srcValue.Length == 1)
                {
                    result = ((string.Compare(srcValue, "0") != 0) && (string.Compare(srcValue, "n", true) != 0));

                    dealed = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="val"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double ChinaRound(double val, int decimals)
        {
            //if (val < 0)
            //{
            //    return Math.Round(val + 5 / Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero);
            //}
            //else
            //{
            //    return Math.Round(val, decimals, MidpointRounding.AwayFromZero);
            //}

            return Math.Round(val, decimals, MidpointRounding.AwayFromZero);
        }
    }
}
