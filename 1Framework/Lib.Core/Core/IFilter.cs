
namespace Lib.Core
{
    /// <summary>
    /// 过滤器的接口，该接口实现了过滤器模式。
    /// </summary>
    /// <typeparam name="T">过滤器需要过滤的实例类型</typeparam>
    /// <remarks>例如：在过滤器中存放了很多的限制条件，若实例满足过滤器中的所有限制条件，则该实例需要进行某种处理，否则进行另一种处理。</remarks>
    public interface IFilter<T>
    {
        /// <summary>
        /// 判断实例是否满足过滤器中的所有限制条件。
        /// </summary>
        /// <param name="target">需要过滤的实例</param>
        /// <returns>布尔类型，若实例满足过滤器中的所有限制条件则返回true，否则返回false。</returns>
        /// <remarks>判断与给定的实例是否匹配,返回布尔值。
        /// </remarks>
        bool IsMatch(T target);

        /// <summary>
        /// 逻辑名称
        /// </summary>
        /// <remarks>在实现时，用户需要得到pipeline的逻辑名称，则可以从这里存取逻辑名称。该属性是可读可写的。</remarks>
        string Name { get; set;}
    }
}
