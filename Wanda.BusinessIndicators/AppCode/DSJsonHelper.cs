using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanda.BusinessIndicators.Web.AppCode
{
   public class DSJsonHelper
    {
        /// <summary>
        /// 反序列化Json数据， 并对重要字段检查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPostData">json数据</param>
        /// <param name="check">检查， 防止重点数据未能反序列化后加载</param>
        /// <returns></returns>
        public static T DeserializeAndCheck<T>(string jsonPostData, Predicate<T> check)
        {
            if (string.IsNullOrEmpty(jsonPostData))
            {
                return default(T);
            }
            try
            {
                T result = JsonHelper.Deserialize<T>(jsonPostData);

                if (check != null && !check(result))
                {
                    throw new ArgumentException("数据反序列化异常：" + jsonPostData);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("数据反序列化异常：" + jsonPostData, ex);
            }
        }

        /// <summary>
        /// 反序列化Json对象数组， 并对重要字段检查
        /// </summary>
        /// <typeparam name="T1">集合</typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="jsonPostData">json数据</param>
        /// <param name="check">检查， 防止重点数据未能反序列化后加载； 如果为null或者Empty的话， 则不检查</param>
        /// <returns></returns>
        public static T1 DeserializeAndCheck<T1, T2>(string jsonPostData, Predicate<T2> check)
            where T1 : IEnumerable<T2>
        {
            if (string.IsNullOrEmpty(jsonPostData))
            {
                return default(T1);
            }
            try
            {
                T1 result = JsonHelper.Deserialize<T1>(jsonPostData);
                if (((IEnumerable<T2>)result).Count() == 0)
                {
                    return result;
                }
                T2 obj = ((IEnumerable<T2>)result).ToList().First();
                if (check != null && !check(obj))
                {
                    throw new ArgumentException("数据反序列化异常：" + jsonPostData);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("数据反序列化异常：" + jsonPostData, ex);
            }
        }
    }
}