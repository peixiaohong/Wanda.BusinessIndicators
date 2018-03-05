using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Wanda.Lib.LightWorkflow.Tools
{
    public class ObjectHelper
    {
        public static T Clone<T>(T originalObject)
        {
            if (originalObject != null)
            {
                if (originalObject is ICloneable)
                {
                    ICloneable obj = originalObject as ICloneable;
                    return (T)obj.Clone();
                }
                else
                {
                    MethodInfo method = originalObject.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        return (T)
                        method.Invoke(originalObject, null);
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        /// 获取一个枚举的所有属性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnumDescription(System.Enum value)
        {
            string result = string.Empty;

            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    result = attributes[0].Description;
                }
            }

            return result;
        }
    }
}
