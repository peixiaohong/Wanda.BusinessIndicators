
namespace Lib.Core
{
    /// <summary>
    /// 基于反射的创建者类接口，该接口实现了创建者模式。
    /// </summary>
    /// <typeparam name="T">实例的类型</typeparam>
    /// <remarks>基于反射的创建者类接口，该接口实现了创建者模式。
    /// </remarks>
    internal interface IBuilder<T>
    {
        /// <summary>
        /// 创建指定类型的实例。
        /// </summary>
        /// <param name="target">需要创建的实例</param>
        /// <returns>已创建的实例。</returns>
        /// <remarks>创建指定类型的实例。</remarks>
        T BuildUp(T target);

        /// <summary>
        /// 拆开指定类型的实例。
        /// </summary>
        /// <param name="target">需要拆开的实例</param>
        /// <returns>已拆开的实例</returns>
        /// <remarks>拆开指定类型的实例。</remarks>
        T TearDown(T target);
    }
}
