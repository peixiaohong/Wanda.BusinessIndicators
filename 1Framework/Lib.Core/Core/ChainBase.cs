using System.Collections;
using System.Collections.Generic;

namespace Lib.Core
{
    /// <summary>
    /// 职责链模式的基类，实现了IChain(职责链Interface)接口。
    /// </summary>
    /// <typeparam name="T">职责链中每一个节点的类型</typeparam>
    /// <remarks>
    /// 该基类提供对职责链的基本操作实现，对于链对象的插入以及数据获取。
    /// </remarks>
    public abstract class ChainBase<T> : IChain<T>
        where T : class
    {
        /// <summary>
        /// 保存责任链中每一项的表。
        /// </summary>
		/// <remarks>保存责任链中每一项的表</remarks>
        protected List<T> chainItems;

        /// <summary>
        /// 不带参数的构造函数
        /// </summary>
        /// <remarks>不带参数的构造函数
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ChainBaseTest.cs" region="Test IChain and ChainBase" lang="cs" title="在DeluxeWorks里如何实现责任链模式" />
        /// </remarks>
        public ChainBase()
        {
			this.chainItems = new List<T>();
        }

        /// <summary>
        /// 职责链的第一个节点
        /// </summary>
        /// <remarks>
        /// 职责链的第一个节点，该属性是只读的。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ChainBaseTest.cs" region="Test IChain and ChainBase" lang="cs" title="在DeluxeWorks里如何实现责任链模式" />
        /// </remarks>
        public T Head
        {
            get
            {
				return this.chainItems.Count > 0 ? this.chainItems[0] : null;
            }
        }

        /// <summary>
        /// 向职责链中添加一个节点
        /// </summary>
        /// <param name="item">需要向职责链中添加的节点</param>
        /// <remarks>
        /// 向职责链中添加一个节点
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ChainBaseTest.cs" region="Test IChain and ChainBase" lang="cs" title="在DeluxeWorks里如何实现责任链模式"/>
        /// </remarks>
        public void Add(T item)
        {
			this.chainItems.Add(item);
        }

        /// <summary>
        /// 向职责链中添加一组节点。
        /// </summary>
		/// <param name="items">节点组</param>
        /// <remarks>
        /// 向职责链中添加一组节点。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ChainBaseTest.cs" region="Test IChain and ChainBase" lang="cs" title="在DeluxeWorks里如何实现责任链模式"/>
        /// </remarks>
        public void AddRange(IEnumerable items)
        {
			foreach (T item in items)
                Add(item);
        }

        /// <summary>
        /// 在职责链中获得当前节点的下一个节点。
        /// </summary>
        /// <param name="current">当前的节点</param>
        /// <returns>获取职责链的当前节点的下一个节点，如果指定的节点为空或当前节点的下一个节点为空，则返回空。
        /// </returns>
        /// <remarks>
        /// 获得职责链中指定节点的下一个节点
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ChainBaseTest.cs" region="Test IChain and ChainBase" lang="cs" title="在DeluxeWorks里如何实现责任链模式"/>
        /// </remarks>
        public T GetNext(T current)
        {
			for (int i = 0; i < this.chainItems.Count - 1; i++)
			{
				if (ReferenceEquals(current, this.chainItems[i]))
					return this.chainItems[i + 1];
			}

            return null;
        }

    }
}