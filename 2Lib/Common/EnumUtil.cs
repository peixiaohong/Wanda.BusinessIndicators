using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Lib.Core;

namespace Wanda.BusinessIndicators.Common
{
    public static class EnumUtil
    {
        /// <summary> 
        /// 获得枚举类型数据项（不包括空项）
        /// </summary> 
        /// <param name="enumType">枚举类型</param> 
        /// <returns></returns> 
        public static IList<object> GetItems(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            IList<object> list = new List<object>();
            IList<AnonymousType> listAnonymoustype = new List<AnonymousType>();

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
                if (value > 0)
                {
                    string text = string.Empty;
                    object[] array = field.GetCustomAttributes(typeDescription, false);

                    if (array.Length > 0) text = ((DescriptionAttribute)array[0]).Description;
                    else text = field.Name; //没有描述，直接取值

                    //添加到列表
                    listAnonymoustype.Add(new AnonymousType(value, text, field.Name));
                }
            }

            AnonymousType temp;
            for (int m = 0; m < listAnonymoustype.Count - 1; m++)
            {
                for (int n = m; n < listAnonymoustype.Count; n++)
                {
                    if (listAnonymoustype[m].Value > listAnonymoustype[n].Value)
                    {
                        temp = listAnonymoustype[m];
                        listAnonymoustype[m] = listAnonymoustype[n];
                        listAnonymoustype[n] = temp;
                    }
                }
            }
            foreach (AnonymousType at in listAnonymoustype)
            {
                list.Add(new { Value = at.Value, Text = at.Text, FieldName = at.FieldName });
            }

            return list;
        }

        public static string GetEnumDescription(this Type tnum, object enumitem)
        {
            if (!tnum.IsEnum)
                throw new InvalidOperationException();

            FieldInfo field = tnum.GetField(tnum.GetEnumName(enumitem));

            object[] array = field.GetCustomAttributes(typeof(EnumItemDescriptionAttribute), false);

            if (array.Length > 0)
                return ((EnumItemDescriptionAttribute)array[0]).Description;

            return field.Name;
        }


        public static int GetEnueValue(this Type tnum, object enumDescription)
        {
            FieldInfo[] fields = tnum.GetFields();
            IList<object> list = new List<object>();

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)tnum.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
                if (value > 0)
                {
                    if (field.Name.ToString().ToLower() == enumDescription.ToString().ToLower())
                    {
                        return value;
                    }
                }
            }

            return 0;
        }
    }
}
