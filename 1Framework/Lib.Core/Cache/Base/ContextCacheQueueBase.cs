using System;
using System.Collections.Generic;
using System.Collections;

namespace Lib.Cache
{
    /// <summary>
    /// 在调用上下文中缓存的Cache队列的基类，该Cache的生命期，仅仅在当前线程（WinForm）或一次Http（Web）)请求过程中
    /// </summary>
    public abstract class ContextCacheQueueBase
    {
    }

    /// <summary>
    /// 在调用上下文中缓存的泛型Cache队列的基类，该Cache的生命期，仅仅在当前线程（WinForm）或一次Http（Web）)请求过程中
    /// </summary>
    public abstract class ContextCacheQueueBase<TKey, TValue> : ContextCacheQueueBase
    {
        /// <summary>
        /// Cache项不存在时的委托定义
        /// </summary>
        /// <param name="cache">Cache对列</param>
        /// <param name="key">键值</param>
        /// <returns>新的Cache项</returns>
        public delegate TValue ContextCacheItemNotExistsAction(ContextCacheQueueBase<TKey, TValue> cache, TKey key);

        private Dictionary<TKey, TValue> innerDictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// 在Cach队列中添加一项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <returns>值</returns>
        public TValue Add(TKey key, TValue data)
        {
            this.innerDictionary[key] = data;
            return data;
        }

        /// <summary>
        /// 按照键值获取或设置Cache的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public TValue this[TKey key]
        {
            get
            {
                return this.innerDictionary[key];
            }
            set
            {
                this.innerDictionary[key] = value;
            }
        }

        /// <summary>
        /// 通过key，获取Cache项的value，如果相应的cache项存在的话
        /// 则将cache项的value作为输出参数，返回给客户端代码 
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue data)
        {
            return this.innerDictionary.TryGetValue(key, out data);
        }

        /// <summary>
        /// 在Cache中读取Cache项，如果不存在，则调用action
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="action">不存在时的回调</param>
        /// <returns>Cache项的值</returns>
        public TValue GetOrAddNewValue(TKey key, ContextCacheItemNotExistsAction action)
        {
            TValue result = default(TValue);

            if (TryGetValue(key, out result) == false)
                result = action(this, key);

            return result;
        }

        /// <summary>
        /// 按照键值，删除一项Cache项
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功删除</returns>
        public bool Remove(TKey key)
        {
            return this.innerDictionary.Remove(key);
        }

        /// <summary>
        /// Cache项的数量
        /// </summary>
        public int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }

        /// <summary>
        /// 清空所有Cache项
        /// </summary>
        public void Clear()
        {
            this.innerDictionary.Clear();
        }

        /// <summary>
        /// 是否包含某键值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否包含</returns>
        public bool ContainsKey(TKey key)
        {
            return this.innerDictionary.ContainsKey(key);
        }

        #region IEnumerable 成员

        /// <summary>
        /// 枚举Cache的每一项
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> kp in this.innerDictionary)
                yield return kp;
        }

        #endregion
    }
}
