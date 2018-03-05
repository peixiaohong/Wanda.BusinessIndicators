using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web.Script.Serialization;
using Lib.Core;
using System.Xml.Linq;
using System.Xml;

namespace Lib.Web.Json
{
    public class JsonHelper
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <returns>序列化结果</returns>
        /// <remarks>替代系统提供的序列化调用</remarks>
        public static string Serialize(object input)
        {
            return Serialize(input, null);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <param name="type">要序列化的类型，可以是input的基类或input实现的接口</param>
        /// <returns>序列化结果</returns>
        public static string Serialize(object input, Type type)
        {
            return Serialize(input, type, 0);
        }
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <param name="type">要序列化的类型，可以是input的基类或input实现的接口</param>
        /// <param name="resolverTypeLevel">要输出类型信息的级别深度</param>
        /// <returns>序列化结果</returns>
        /// <remarks>替代系统提供的序列化调用</remarks>
        public static string Serialize(object input, Type type, int resolverTypeLevel)
        {
            JavaScriptSerializer ser = resolverTypeLevel > 0 ?
                JsonSerializerFactory.GetJavaScriptSerializer(input.GetType()) :
               JsonSerializerFactory.GetJavaScriptSerializer();

            input = PreSerializeObject(input, type, resolverTypeLevel - 1);

            ser.MaxJsonLength = int.MaxValue;

            string result = ser.Serialize(input);

            return result;
        }

        /// <summary>
        /// 预处理序列化对象，通过调用此函数，可对序列化进行预处理
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <returns>处理结果</returns>
        /// <remarks>预处理序列化对象，通过调用此函数，可对序列化进行预处理</remarks>
        public static object PreSerializeObject(object input)
        {
            return PreSerializeObject(input, null, 0);
        }

        /// <summary>
        /// 预处理序列化对象，通过调用此函数，可对序列化进行预处理
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <param name="type">要序列化的类型，可以是input的基类或input实现的接口</param>
        /// <param name="resolverTypeLevel">要输出类型信息的级别深度</param>
        /// <returns>处理结果</returns>
        /// <remarks>预处理序列化对象，通过调用此函数，可对序列化进行预处理</remarks>
        public static object PreSerializeObject(object input, Type type, int resolverTypeLevel)
        {
            object result = input;
            if (input != null)
            {
                JavaScriptConverter converter = JsonSerializerFactory.GetJavaScriptConverter(input.GetType());
                if (converter != null)
                {
                    result = converter.Serialize(input, JsonSerializerFactory.GetJavaScriptSerializer());
                }
                else
                {
                    if (input is XElement || input is XmlElement)
                    {
                        result = null;
                    }
                    else if (input is DateTime)
                    {
                        result = input.ToString();
                    }
                    //当前判断条件，只适用经营指标系统，其他系统请注销此判断条件。
                    else if ( input is double )
                    {
                        result = ((double)input).ToString("F7");
                    }
                    else if (!(input is ValueType) && !(input is string) && !(input is IDictionary))
                    {
                        IEnumerable list = input as IEnumerable;
                        if (list != null)
                        {
                            ArrayList a = new ArrayList();
                            foreach (object o in list)
                            {
                                a.Add(PreSerializeObject(o, null, resolverTypeLevel - 1));
                            }
                            result = a;
                        }
                        else
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            Type inputType = input.GetType();
                            if (type != null)
                            {
                                ExceptionHelper.FalseThrow(type.IsAssignableFrom(inputType),
                                    string.Format("{0} 没有从类型{1}继承", inputType, type));
                            }
                            else
                                type = inputType;
                            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                            {
                                ScriptIgnoreAttribute ignoreAttr = (ScriptIgnoreAttribute)Attribute.GetCustomAttribute(pi, typeof(ScriptIgnoreAttribute), false);
                                if (ignoreAttr == null)
                                {
                                    MethodInfo mi = pi.GetGetMethod();
                                    if (mi != null && mi.GetParameters().Length <= 0)
                                    {
                                        object v = PreSerializeObject(mi.Invoke(input, null), null, resolverTypeLevel - 1);

                                        ClientPropertyNameAttribute nameAttr = (ClientPropertyNameAttribute)Attribute.GetCustomAttribute(pi, typeof(ClientPropertyNameAttribute), false);
                                        string name = nameAttr == null ? pi.Name : nameAttr.PropertyName;
                                        dict.Add(name, v);
                                    }
                                }
                            }
                            result = dict;
                        }
                    }
                }
            }

            if (resolverTypeLevel > 0 && result is Dictionary<string, object>)
            {
                Dictionary<string, object> resultDict = result as Dictionary<string, object>;
                if (!resultDict.ContainsKey("__type"))
                    resultDict["__type"] = input.GetType().AssemblyQualifiedName;
            }

            return result;
        }


        /// <summary>
        /// 将对象input转换成类别为T的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="input">要转换的对象</param>
        /// <returns>转换结果</returns>
        public static T Deserialize<T>(object input)
        {
            return (T)DeserializeObject(input, typeof(T));
        }

        /// <summary>
        /// 将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="type">要转换成的类型</param>
        /// <returns>转换结果</returns>
        /// <remarks>将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换</remarks>
        public static object DeserializeObject(object input, Type type)
        {
            //type = GetDeserializeObjectType(input, type);
            return DeserializeObject(input, type, 0);
        }

        /// <summary>
        /// 将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="type">要转换成的类型</param>
        /// <param name="level">调用级别</param>
        /// <returns>转换结果</returns>
        private static object DeserializeObject(object input, Type type, int level)
        {
            if (level > 3)
                throw new ApplicationException(string.Format("序列化层级超过{0}，退出", level));

            object result = null;

            if (input != null) //Added by shenzheng 2008-3-10
            {
                if (type.IsAssignableFrom(input.GetType()))
                {
                    result = input;
                }
                else if (type.GetInterface(typeof(IConvertible).AssemblyQualifiedName) != null)
                {
                    result = Convert.ChangeType(input, type, CultureInfo.InvariantCulture);
                }
                else
                {
                    if (input is object[])
                    {
                        object tempResult = DeserializeArrayObject((object[])input, type, level);

                        if (tempResult == null)
                            throw new ApplicationException(string.Format("无法将类型{0}转换成{1}", input.GetType().AssemblyQualifiedName, type.AssemblyQualifiedName));
                        else
                            result = tempResult;
                    }
                    else
                    {
                        Dictionary<string, object> dic = input as Dictionary<string, object>;
                        if (type != null && dic != null && !dic.ContainsKey("__type"))
                        {
                            dic["__type"] = type.AssemblyQualifiedName;
                        }
                        JavaScriptSerializer ser = JsonSerializerFactory.GetJavaScriptSerializer(type);

                        ser.MaxJsonLength = int.MaxValue;

                        string str = input is string ? (string)input : ser.Serialize(input);

                        result = ser.DeserializeObject(str);
                    }

                    result = DeserializeObject(result, type, ++level);
                }
            }

            return result;
        }

        private static object DeserializeArrayObject(object[] input, Type type, int level)
        {
            object tempResult = null;
            if (typeof(Array).IsAssignableFrom(type))
            {
                Type eltType = type.GetMethod("Get", new Type[1] { typeof(int) }).ReturnType;
                object[] objs = input;
                Array ins = Array.CreateInstance(eltType, objs.Length);

                for (int i = 0; i < objs.Length; i++)
                {
                    ins.SetValue(DeserializeObject(objs[i], eltType), i);
                }
                tempResult = ins;
            }
            else if (typeof(ICollection<object>).IsAssignableFrom(type))
            {
                object ins = TypeCreator.CreateInstance(type);
                ICollection<object> c = (ICollection<object>)ins;
                foreach (object o in input)
                    c.Add(DeserializeObject(o, type.GetGenericArguments()[0], level));
                tempResult = ins;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                object ins = TypeCreator.CreateInstance(type);
                IList l = (IList)ins;
                MethodInfo mi = type.GetMethod("get_Item", new Type[1] { typeof(int) });
                if (mi != null)
                {
                    Type eltType = mi.ReturnType;
                    foreach (object o in input)
                        l.Add(DeserializeObject(o, eltType, level));
                    tempResult = ins;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                object ins = TypeCreator.CreateInstance(type);
                MethodInfo mi = type.GetMethod("get_Item", new Type[1] { typeof(int) });
                if (mi != null)
                {
                    Type eltType = mi.ReturnType;
                    foreach (object o in input)
                        mi.Invoke(ins, new object[1] { DeserializeObject(o, eltType, level) });
                    tempResult = ins;
                }
            }

            return tempResult;
        }
    }
}
