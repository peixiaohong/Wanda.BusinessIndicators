using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using Lib.Core;

namespace Lib.Web.Json
{
    public class JsonSerializerFactory
    {
        /// <summary>
        /// 获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象
        /// </summary>
        /// <returns>JavaScriptSerializer对象</returns>
        /// <remarks>获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象</remarks>
        internal static JavaScriptSerializer GetJavaScriptSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            RegisterConverters(serializer);

            return serializer;
        }

        /// <summary>
        /// 获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象
        /// 并序列化器专门序列化resolverType类别的对象
        /// </summary>
        /// <param name="resolverType">要序列化数据的类型</param>
        /// <returns>JavaScriptSerializer对象</returns>
        /// <remarks>获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象
        /// 并序列化器专门序列化resolverType类别的对象</remarks>
        internal static JavaScriptSerializer GetJavaScriptSerializer(Type resolverType)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new DefaultTypeResolver(resolverType));

            RegisterConverters(serializer);

            return serializer;
        }

        /// <summary>
        /// 从配置文件中获取支持converterType类别的Converter转换器
        /// </summary>
        /// <param name="converterType">转换器匹配的类别</param>
        /// <returns>Converter转换器</returns>
        /// <remarks>从配置文件中获取支持converterType类别的Converter转换器</remarks>
        internal static JavaScriptConverter GetJavaScriptConverter(Type converterType)
        {
            return GetJavaScriptConverter(converterType, converterType, null);
        }

        private static JavaScriptConverter GetJavaScriptConverter(Type converterType, Type compareType, JavaScriptConverter[] converters)
        {
            JavaScriptConverter result = null;

            converters = converters ?? GetConverters();

            foreach (JavaScriptConverter c in converters)
            {
                foreach (Type t in c.SupportedTypes)
                {
                    if (t == compareType)
                    {
                        result = c;
                        break;
                    }
                }
            }

            return result;
        }

        private static void RegisterConverters(JavaScriptSerializer serializer)
        {
            serializer.RegisterConverters(GetConverters());
        }

        private static JavaScriptConverter[] GetConverters()
        {
            return CreateConfigConverters();
        }

        private static JavaScriptConverter[] CreateConfigConverters()
        {
            List<JavaScriptConverter> list = new List<JavaScriptConverter>();
            ScriptingJsonSerializationSection section = WebConfigFactory.GetJsonSerializationSection();
            if (section != null)
            {
                foreach (Converter converter in section.Converters)
                {
                    Type c = System.Web.Compilation.BuildManager.GetType(converter.Type, false);

                    if (c == null)
                    {
                        throw new ArgumentException("类型不能为空");
                    }
                    if (!typeof(JavaScriptConverter).IsAssignableFrom(c))
                    {
                        throw new ArgumentException("类型没有继承自JavaScriptConverter");
                    }

                    list.Add((JavaScriptConverter)TypeCreator.CreateInstance(c));
                }
            }
            return list.ToArray();
        }
    }
}
