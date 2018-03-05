using Lib.Core;

namespace Lib.Core
{
    /// <summary>
    /// 基于反射的创建者基类，该基类继承自IBuilder接口。
    /// </summary>
    /// <typeparam name="T">实例的类型</typeparam>
    /// <remarks>基于反射的创建者基类，该基类继承自IBuilder接口。</remarks>
    public abstract class BuilderBase<T> : IBuilder<T>
    {
        /// <summary>
        /// 创建指定类型的实例。
        /// </summary>
        /// <param name="target">需要创建的实例</param>
        /// <returns>指定类型的实例</returns>
        /// <remarks>
		/// 创建指定类型的实例。
        /// </remarks>
		public abstract T BuildUp(T target);

        /// <summary>
        ///拆开指定类型的实例。
        /// </summary>
        /// <param name="target">需要拆开的实例</param>
        /// <returns>已拆开的实例</returns>
        /// <remarks>拆开指定类型的实例。</remarks>
		public abstract T TearDown(T target);

        /// <summary>
        /// 根据全类型名称动态的创建指定类型的实例。 
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="constructorParams">创建实例的参数</param>
        /// <returns>指定类型的实例对象</returns>
        /// <remarks>
		/// 根据全类型名称动态的创建指定类型的实例，该创建实例过程用到了反射机制。
        /// </remarks>
		protected object CreateInstance(string typeName, params object[] constructorParams)
        {
            return TypeCreator.CreateInstance(typeName, constructorParams);
        }
    }
}

