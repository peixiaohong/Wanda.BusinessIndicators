using System.Collections;

namespace Lib.Core
{
    /// <summary>
    /// 职责链模式接口，该接口实现了职责链模式。
    /// </summary>
    /// <typeparam name="T">职责链中每一个节点的类型</typeparam>
    /// <remarks>
    /// 该接口提供对职责链的基本操作，包括添加一个节点，添加一组节点等等。   
    /// </remarks>
    internal interface IChain<T>
        where T : class
    {
        /// <summary>
        /// 职责链的第一个节点
        /// </summary>
        /// <remarks>
        /// 该属性是只读的
        /// </remarks>
        T Head { get;}

        /// <summary>
        /// 向职责链中添加一个节点
        /// </summary>
        /// <param name="item">需要向职责链中添加的节点</param>
        /// <remarks>
        /// 向职责链中添加一个节点
        /// </remarks>
        void Add(T item);

        /// <summary>
        /// 向职责链中添加一组节点。
        /// </summary>
        /// <param name="items">节点组</param>
        /// <remarks>
        /// 向职责链中添加一系列节点。
        /// </remarks>
        void AddRange(IEnumerable items);

        /// <summary>
        /// 在职责链中获得当前节点的下一个节点。
        /// </summary>
        /// <param name="current">当前的节点</param>
        /// <returns>获取职责链的指定节点的下一个节点，如果指定的节点为空或当前节点的下一个节点为空，则返回空。</returns>
        /// <remarks>
        /// 获得职责链中指定节点的下一个节点。
        /// </remarks>
        T GetNext(T current);
    }
}
