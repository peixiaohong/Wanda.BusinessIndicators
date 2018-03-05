using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{

    public interface IPartlyData<T>
    {
        int TotalCount { get; }
        List<T> SubCollection { get; }
    }

    public class PartlyData<T> :IPartlyData<T>{

        public int TotalCount
        {
            get;
            set;
        }

        public List<T> SubCollection
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 子集集合， 提供一个子集属性与全集数量属性
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    [Serializable]
    public class PartlyCollection<T> : ICollection<T>, IPartlyData<T>
    {

        #region 创建对象的工厂方法

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static PartlyCollection<T> Create(
            List<T> collection,
            int totalCount
            )
        {
            PartlyCollection<T> result = new PartlyCollection<T>();
            result.collection = collection;
            result.TotalCount = totalCount;
            return result;
        }


        public static PartlyCollection<T> Create(
            IEnumerable<T> items,
            int totalCount)
        {
            return Create(items.ToList(), totalCount);
        }
        #endregion


        #region 支持List与PartlyCollection的相互转换

        public static explicit operator PartlyCollection<T>(List<T> items)
        {
            return PartlyCollection<T>.Create(items, items.Count);
        }

        public static implicit operator List<T>(PartlyCollection<T> d)
        {
            return d.collection;  // implicit conversion
        }
        #endregion

        #region 属性

        private List<T> collection = new List<T>();

        public int TotalCount { get; private set; }

        public List<T> SubCollection { get { return collection; } }
        #endregion


        public void Add(T item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("不支持删除方法！");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // return ((IEnumerable)this).GetEnumerator(); //stack overflow
            return (IEnumerator)collection.GetEnumerator();
        }



    }
}
