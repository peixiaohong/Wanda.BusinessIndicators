using System;
using System.Collections;
using System.Collections.Generic;
using Lib.Core.Properties;

namespace Lib.Core
{
    /// <summary>
    /// 运用最近最少用算法(Least Recently Used Algorithm)实现把最近最多使用的元素放在LruDictionary的前部，而最近最少使用的元素放在LruDictionary的后部
    /// </summary>
    /// <typeparam name="TKey">LruDictionary的键值Key的类型</typeparam>
    /// <typeparam name="TValue">LruDictionary的Value的类型</typeparam>
    /// <remarks>运用最近最少用算法(Least Recently Used Algorithm)实现LruDictionary，LruDictionary中最近最少使用的元素放在其后部，最近经常使用的元素放在其前部。
    /// LruDictionary中的每个元素由两部分组成—Key和Value，其中Key为LruDictionary的键值。
    /// </remarks>
    public sealed class LruDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private int maxLength = 100;
        private static object syncRoot = new object();//add by yuanyong 20071227
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> innerDictionary =
            new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();

        private LinkedList<KeyValuePair<TKey, TValue>> innerLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();

        #region 构造方法
        /// <summary>
        /// LruDictionary没有参数的构造函数。
        /// </summary>
        /// <remarks>该构造函数中LruDictionary的最大长度是100。此构造方法适用于构造不指定最大长度的LruDictionary。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="LruDictionaryTest" lang="cs" title="获取一个无参数构造的LruDictionary"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>    
        public LruDictionary()
        {
        }

        /// <summary>
        /// 定制最大长度为maxLength的LruDictionary。
        /// </summary>
        /// <param name="maxLruLength">需要设置的LruDictionary的最大长度。</param>
        /// <remarks>在默认情况下，LruDictionary的最大长度为100。此构造方法适用于构造指定最大长度的LruDictionary。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="LruDictionaryTest" lang="cs" title="获取最大长度为maxLength的LruDictionary"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public LruDictionary(int maxLruLength)
        {
            ExceptionHelper.TrueThrow<InvalidOperationException>(maxLruLength < 0, "maxLruLength must >= 0");

            this.maxLength = maxLruLength;
        }
        #endregion

        #region 公共属性
        /// <summary>
        /// 同步访问LruDictionary的属性
        /// </summary>
        /// <remarks>同步访问LruDictionary的属性，该属性是只读的。</remarks>
        public object SyncRoot
        {
            get
            {
                return ((ICollection)this.innerDictionary).SyncRoot;
            }
        }

        /// <summary>
        /// LruDictionary的最大长度
        /// </summary>
        /// <remarks>LruDictionary的最大长度，该属性是可读可写的。</remarks>
        public int MaxLength
        {
            get { return this.maxLength; }
            set { this.maxLength = value; }
        }

        /// <summary>
        /// LruDictionary的当前长度
        /// </summary>
        /// <remarks>当前LruDictionary的长度，该属性是只读的。</remarks>
        public int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 向LruDictionary中添加一个元素，用户需要把组成元素的Key和Value值传入。
        /// </summary>
        /// <param name="key">向LruDictionary中填入元素的Key值</param>
        /// <param name="data">向LruDictionary中填入元素的Value值</param>
        /// <remarks>向LruDictionary中添加一个元素时，该元素是由Key和Value值组成。按照LRU原则，经常使用的元素放在LruDictionary的前面。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="AddTest" lang="cs" title="向LruDictionary中添加一个元素"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        ///</remarks>
        public void Add(TKey key, TValue data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-Add-key");//add by yuanyong 20071227

            //如果已经存在，抛出异常
            ExceptionHelper.TrueThrow<ArgumentException>(this.innerDictionary.ContainsKey(key),
                Resource.DuplicateKey, key);

            LinkedListNode<KeyValuePair<TKey, TValue>> node =
                new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, data));

            lock (syncRoot)
            {
                this.innerDictionary.Add(key, node);
                this.innerLinkedList.AddFirst(node);

                if (this.innerLinkedList.Count >= MaxLength)
                {
                    for (int i = 0; i < this.innerLinkedList.Count - MaxLength + 1; i++)
                    {
                        LinkedListNode<KeyValuePair<TKey, TValue>> lastNode = this.innerLinkedList.Last;

                        if (this.innerDictionary.ContainsKey(lastNode.Value.Key))
                        {
                            this.innerDictionary.Remove(lastNode.Value.Key);
                            this.innerLinkedList.Remove(lastNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置LruDictionary中元素键值为key值的Value值
        /// </summary>
        /// <param name="key">要获得的元素的键值</param>
        /// <returns>LruDictionary中键值key的Value值</returns>
        /// <remarks>该属性是可读可写的。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" lang="cs" title="读取或设置LruDictionary中键值为Key值的Value值"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-get-key");//add by yuanyong 20071227

                LinkedListNode<KeyValuePair<TKey, TValue>> node = this.innerDictionary[key];
                //没有找到，会自动抛出异常
                lock (syncRoot)
                {
                    MoveNodeToFirst(node);
                }
                return node.Value.Value;
            }
            set
            {
                ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-set-key");//add by yuanyong 20071227

                LinkedListNode<KeyValuePair<TKey, TValue>> node;

                lock (syncRoot)
                {
                    if (this.innerDictionary.TryGetValue(key, out node))
                    {
                        MoveNodeToFirst(node);

                        node.Value = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else
                        Add(key, value);
                }
            }
        }

        /// <summary>
        /// 删除LruDictionary中键值为Key值的元素
        /// </summary>
        /// <param name="key">键值Key</param>
        /// <remarks>若LruDictionary中不包含键值为Key的元素，则系统自动的抛出异常。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="RemoveTest" lang="cs" title="从LruDictionary中删除键值为Key的元素"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public void Remove(TKey key)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-Remove-key");//add by yuanyong 20071227
            LinkedListNode<KeyValuePair<TKey, TValue>> node = null;

            lock (syncRoot)
            {
                if (this.innerDictionary.TryGetValue(key, out node))
                {
                    this.innerDictionary.Remove(key);
                    this.innerLinkedList.Remove(node);
                }
            }
        }

        /// <summary>
        /// 判断LruDictionary中是否包含键值为Key值的元素
        /// </summary>
        /// <param name="key">键值Key</param>
        /// <returns>若LruDictionary中包含键值为key值的元素,则返回true，否则返回false</returns>
        /// <remarks>若返回值为true，由于该Key值的元素刚使用过，则把该元素放在LruDictionary的最前面。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region ="ContainsKeyTest" lang="cs" title="判断LruDictionary中是否包含键值为key的元素"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public bool ContainsKey(TKey key)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-ContainsKey-key");//add by yuanyong 20071227

            return this.innerDictionary.ContainsKey(key);
        }

        /// <summary>
        /// 判断LruDictionary中是否包含键值为Key值的元素。若包含，则返回值是true，可以从data中取出该值，否则返回false。
        /// </summary>
        /// <param name="key">键值key</param>
        /// <param name="data">键值key的Value值</param>
        /// <returns>返回true或false</returns>
        /// <remarks>若返回值为true，由于该Key值的元素刚使用过，则把该元素放在LruDictionary的最前面。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="TryGetValueTest" lang="cs" title="试图从LruDictionary中取出键值为key的元素"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-TryGetValue-key");//add by yuanyong 20071227

            LinkedListNode<KeyValuePair<TKey, TValue>> node;
            data = default(TValue);

            lock (syncRoot)
            {
                bool result = this.innerDictionary.TryGetValue(key, out node);

                if (result)
                {
                    MoveNodeToFirst(node);

                    data = node.Value.Value;
                }

                return result;
            }
        }

        /// <summary>
        /// 清除LruDictionary内的所有值
        /// </summary>
        /// <remarks>此时LruDictionary中没有任何元素
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region ="ClearTest" lang="cs" title="清空LruDictionary中的所有元素"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public void Clear()
        {
            lock (syncRoot)
            {
                this.innerDictionary.Clear();
                this.innerLinkedList.Clear();
            }
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> 成员
        /// <summary>
        /// 获得LruDictionary中所有元素的枚举器
        /// </summary>
        /// <returns>LruDictionary中所有元素的枚举器</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="GetEnumeratorTest" lang="cs" title="获得LruDictionary中所有元素的枚举器"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        ///</remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return EnumItems();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return EnumItems();
        }

        #endregion

        private IEnumerator<KeyValuePair<TKey, TValue>> EnumItems()
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> node = this.innerLinkedList.First;

            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        private void MoveNodeToFirst(LinkedListNode<KeyValuePair<TKey, TValue>> node)
        {
            LinkedList<KeyValuePair<TKey, TValue>> list = node.List;

            list.Remove(node);
            list.AddFirst(node);
        }
    }
}
