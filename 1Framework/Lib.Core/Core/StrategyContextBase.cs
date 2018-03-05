

namespace Lib.Core
{
    /// <summary>
    /// 策略模式，实现了Strategy Context 。
    /// </summary>
    /// <remarks>
    /// 组合 IStragegy 对象计算过程。
    /// </remarks>
	/// <typeparam name="TStrategy">计算对象类型</typeparam>
	/// <typeparam name="TResult">计算结果类型</typeparam>
    public abstract class StrategyContextBase<TStrategy, TResult>
    {
        #region Private field
        /// <summary>
        /// Context 需要操作的算法对象
        /// </summary>
        protected TStrategy innerStrategy;
        #endregion

        /// <summary>
        /// 可以覆盖的抽象 IStragegy 类型各方法计算过程，作为抽象 Context 类型这里仅完成基本的 Calculate 方法调用。
        /// </summary>
        /// <returns>内部计算的结果值</returns>
		/// <remarks>
		/// 可以覆盖的抽象 IStragegy 类型各方法计算过程，作为抽象 Context 类型这里仅完成基本的 Calculate 方法调用。
		/// </remarks>
        public abstract TResult DoAction();

        /// <summary>
        /// 定义具体的算法类型
        /// </summary>
		/// <remarks>
		/// 定义具体的算法类型
		/// </remarks>
        public TStrategy Strategy
        {
			get 
			{
				return this.innerStrategy;
			}
            set 
			{
				this.innerStrategy = value; 
			}
        }
    }
}
