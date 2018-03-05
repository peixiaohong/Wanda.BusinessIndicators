using System.Collections.Generic;

namespace Lib.Core
{
    /// <summary>
    /// 过滤器基类，该基类实现了IFilter接口。
    /// </summary>
    /// <typeparam name="TFilter">过滤器的类型</typeparam>
    /// <typeparam name="TTarget">需要过滤的实例的类型</typeparam>
    /// <remarks>例如：在过滤器中存放了很多的限制条件，若实例满足过滤器中的所有限制条件，则该实例需要进行某种处理，否则进行另一种处理。
    /// </remarks>
    public abstract class FilterPipelineBase<TFilter, TTarget> 
        where TFilter : IFilter<TTarget>
    {
        /// <summary>
        /// 过滤器队列对象
        /// </summary>
		/// <remarks>
		/// 过滤器队列对象，用于处理内部过滤器集合
		/// </remarks>
        protected List<TFilter> pipeline;

        /// <summary>
        /// 向过滤器中添加新的一个过滤项
        /// </summary>
        /// <param name="filter">过滤项</param>
        /// <remarks>
        /// 向过滤器中添加新的一个过滤项。
        /// </remarks>
        public abstract void Add(TFilter filter);

        /// <summary>
        /// 从过滤器中移除一个过滤项
        /// </summary>
        /// <param name="filter">过滤项</param>
        /// <remarks>从过滤器中移除一个过滤项
        /// </remarks>
		public abstract void Remove(TFilter filter);

        /// <summary>
        /// 判断实例是否满足过滤器中每一项的限制条件。
        /// </summary>
        /// <param name="target">需要进行过滤的实例</param>
        /// <returns>若实例满足过滤器中每一项的所有限制条件则返回真，否则返回假。</returns>
        /// <remarks>判断实例是否满足过滤器中每一项的限制条件，若满足则返回真，否则返回假。
        /// </remarks>
        public virtual bool IsMatch(TTarget target)
        {
			if (this.pipeline != null)
			{
				foreach (TFilter item in this.pipeline)
				{
					if (item.IsMatch(target))
						return true;
				}
			}
            return false;
        }
    }
}
