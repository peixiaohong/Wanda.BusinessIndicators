using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wanda.BusinessIndicators
{
    public static class ExtensionMethod
    {
        public static Guid ToGuid(this string value)
        {
            Guid result = Guid.Empty;
            Guid.TryParse(value, out result);
            return result;
        }


        /// <summary>
        /// 扩展foreach方法
        /// </summary>
        /// <typeparam name="T">集合中元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="action">执行的方法</param>
        /// <returns>执行后的集合</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return list;
        }

    }
}
