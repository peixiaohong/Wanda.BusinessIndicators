
namespace Lib.Core
{
    /// <summary>
    /// 策略模式，实现了IStrategy接口。
    /// </summary>
    /// <remarks>
    /// 定义抽象的Stragegy对象计算过程，实际开发中可以根据计算的要求借助 StrategyContextBase 对象来组织相关的计算内容。
    /// </remarks>
	/// <typeparam name="TData">计算对象类型</typeparam>
	/// <typeparam name="TResult">计算结果类型</typeparam>
    public interface IStrategy<TData, TResult>
    {
        /// <summary>
        /// 根据输入信息计算结果的算法部分
        /// </summary>
        /// <param name="data">输入数据</param>
        /// <returns>算法计算结果</returns>
		/// <remarks>根据输入信息计算结果的算法部分</remarks>
        TResult Calculate(TData data);
    }
}
