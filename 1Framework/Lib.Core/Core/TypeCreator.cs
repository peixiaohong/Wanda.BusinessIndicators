using System;
using System.Reflection;
using System.Text;
using Lib.Core.Properties;

namespace Lib.Core
{
    /// <summary>
    /// 运用晚绑定方式动态生成实例
    /// </summary>
    public static class TypeCreator
    {
        private struct TypeInfo
        {
            public string AssemblyName;
            public string TypeName;

            public override string ToString()
            {
                return TypeName + ", " + AssemblyName;
            }
        }

        /// <summary>
        /// 运用后绑定方式动态的创建一个实例。
        /// </summary>
        /// <param name="typeDescription">创建实例的完整类型名称</param>
        /// <param name="constructorParams">创建实例的初始化参数</param>
        /// <returns>实例对象</returns>
        /// <remarks>运用晚绑定方式动态创建一个实例
        public static object CreateInstance(string typeDescription, params object[] constructorParams)
        {
            Type type = GetTypeInfo(typeDescription);

            ExceptionHelper.FalseThrow<TypeLoadException>(type != null, Resource.TypeLoadException, typeDescription);

            return CreateInstance(type, constructorParams);
        }

        /// <summary>
		/// 根据类型信息创建对象，该对象即使没有公有的构造方法，也可以创建实例
		/// </summary>
		/// <param name="type">创建类型时的类型信息</param>
		/// <param name="constructorParams">创建实例的初始化参数</param>
		/// <returns>实例对象</returns>
		/// <remarks>运用晚绑定方式动态创建一个实例</remarks>
        public static object CreateInstance(System.Type type, params object[] constructorParams)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");
            ExceptionHelper.FalseThrow<ArgumentNullException>(constructorParams != null, "constructorParams");

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            if (constructorParams.Length > 0)
            {
                Type[] types = new Type[constructorParams.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = constructorParams[i].GetType();

                }

                ConstructorInfo ci = type.GetConstructor(flags, null, CallingConventions.HasThis, types, null);
                if (ci != null)
                    return ci.Invoke(constructorParams);
            }
            else
            {
                return Activator.CreateInstance(type, true);
            }

            return null;
        }

        /// <summary>
        /// 得到某个数据类型的缺省值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>该类型的缺省值</returns>
        /// <remarks>如果该类型为引用类型，则返回null，否则返回值类型的缺省值。如Int32返回0，DateTime返回DateTime.MinValue</remarks>
        public static object GetTypeDefaultValue(System.Type type)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

            object result = null;

            if (type.IsValueType)
            {
            //    if (TypeDefaultValueCacheQueue.Instance.TryGetValue(type, out result) == false)
            //    {
                    result = TypeCreator.CreateInstance(type);

            //        TypeDefaultValueCacheQueue.Instance.Add(type, result);
            //    }
            }
            else
                result = null;

            return result;
        }

        public static Type GetTypeInfo(string typeDescription)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(typeDescription, "typeDescription");

            Type result = Type.GetType(typeDescription);

            if (result == null)
            {
                TypeInfo ti = GenerateTypeInfo(typeDescription);
            }

            return result;
        }

        private static TypeInfo GenerateTypeInfo(string typeDescription)
        {
            TypeInfo info = new TypeInfo();
            string[] typeParts = typeDescription.Split(',');

            if (typeParts.Length > 0)
                info.TypeName = typeParts[0].Trim();

            StringBuilder sbuilder = new StringBuilder(256);
            for (int i = 1; i < typeParts.Length; i++)
            {
                if (sbuilder.Length > 0)
                    sbuilder.Append(", ");

                sbuilder.Append(typeParts[i]);
            }
            info.AssemblyName = sbuilder.ToString().Trim();
            return info;
        }
    }
}
