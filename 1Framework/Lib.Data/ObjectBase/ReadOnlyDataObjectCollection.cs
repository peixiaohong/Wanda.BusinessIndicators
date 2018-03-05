using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 只读数据对象集合类的虚基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [ComVisible(true)]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class ReadOnlyDataObjectCollection<T> : CollectionBase, IEnumerable<T>
    {
        /// <summary>
        /// 迭代
        /// </summary>
        /// <param name="action"></param>
        public virtual void ForEach(Action<T> action)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

            foreach (T item in List)
                action(item);
        }

        /// <summary>
        /// 判断集合中是否存在某元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual bool Exists(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            bool result = false;

            foreach (T item in List)
            {
                if (match(item))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 判断集合中每个元素是否都满足某条件
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual bool MatchAll(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            bool result = true;

            foreach (T item in List)
            {
                if (match(item) == false)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 在集合中查找满足匹配条件的第一个元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual T Find(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            T result = default(T);

            foreach (T item in List)
            {
                if (match(item))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 从后向前查找，找到第一个匹配的元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual T FindLast(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            T result = default(T);

            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (match((T)List[i]))
                {
                    result = (T)List[i];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 找到满足匹配条件的所有元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual IList<T> FindAll(Predicate<T> match)
        {
            IList<T> result = new List<T>();

            foreach (T item in List)
            {
                if (match(item))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 是否包含某个元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Contains(T item)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

            return List.Contains(item);
        }

        /// <summary>
        /// 得到某个元素的位置
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int IndexOf(T item)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

            return List.IndexOf(item);
        }

        /// <summary>
        /// 复制到别的集合中
        /// </summary>
        /// <param name="collection"></param>
        public virtual void CopyTo(ICollection<T> collection)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(collection != null, "collection");

            this.ForEach(delegate(T item) { collection.Add(item); });
        }

        /// <summary>
        /// 转换到数组
        /// </summary>
        /// <returns></returns>
        public virtual T[] ToArray()
        {
            T[] result = new T[this.Count];

            for (int i = 0; i < this.Count; i++)
                result[i] = (T)List[i];

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void InnerAdd(T obj)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

            List.Add(obj);
        }

        #region IEnumerable<T> 成员

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<T> GetEnumerator()
        {
            foreach (T item in List)
                yield return item;
        }

        #endregion
    }
}
