using System;
using System.Collections;
using System.Collections.Generic;

namespace Lib.Core
{
    /// <summary>
    /// 字典访问的辅助类
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// 读取泛型字典中的某一项，如果不存在此项，返回缺省值
        /// </summary>
        /// <typeparam name="TKey">字典Key的类型</typeparam>
        /// <typeparam name="TValue">字典项的类型</typeparam>
        /// <typeparam name="TReturnValue">返回值的类型</typeparam>
        /// <param name="dict">泛型字典对象</param>
        /// <param name="key">需要访问的key值</param>
        /// <param name="defaultValue">如果不存在key时，返回的缺省值</param>
        /// <returns></returns>
        public static TReturnValue GetValue<TKey, TValue, TReturnValue>(this IDictionary<TKey, TValue> dict, TKey key, TReturnValue defaultValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

            TReturnValue result = defaultValue;

            TValue oResult = default(TValue);

            if (dict.TryGetValue(key, out oResult))
                result = (TReturnValue)DataConverter.ChangeType(oResult, typeof(TReturnValue));

            return result;
        }

        /// <summary>
        /// 读取字典中的某一项，如果不存在此项，返回缺省值
        /// </summary>
        /// <typeparam name="TReturnValue">字典的返回类型</typeparam>
        /// <param name="dict">字典对象</param>
        /// <param name="key">需要访问的key值</param>
        /// <param name="defaultValue">如果不存在key时，返回的缺省值</param>
        /// <returns></returns>
        public static TReturnValue GetValue<TReturnValue>(this IDictionary dict, object key, TReturnValue defaultValue)
        {
            TReturnValue result = defaultValue;

            if (dict.Contains(key))
                result = (TReturnValue)DataConverter.ChangeType(dict[key], typeof(TReturnValue));

            return result;
        }

        /// <summary>
        /// 当添加的字典项不是该类型的缺省值时，则添加此项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns>是否添加了该项</returns>
        public static bool AddNonDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");
            bool added = false;

            if (data != null)
                if (data is string)
                {
                    if (data.ToString() != string.Empty)
                    {
                        dict.Add(key, data);
                        added = true;
                    }
                }
                else
                    if (data.Equals(default(TValue)) == false)
                    {
                        dict.Add(key, data);
                        added = true;
                    }

            return added;
        }

        /// <summary>
        /// 当添加的字典项不是指定的缺省值时，则添加此项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="defaultValue"></param>
        /// <returns>是否添加了该项</returns>
        public static bool AddNonDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data, TValue defaultValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");
            bool added = false;

            if (data != null)
            {
                if (data.Equals(defaultValue) == false)
                {
                    dict.Add(key, data);
                    added = true;
                }
            }
            else
                if (defaultValue != null)
                {
                    dict.Add(key, data);
                    added = true;
                }

            return added;
        }
    }
}
