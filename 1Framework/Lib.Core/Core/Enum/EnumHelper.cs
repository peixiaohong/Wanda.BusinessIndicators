using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Core 
{
   public class EnumHelper
    {
        /// <summary>
        /// 根据描述找到枚举对象的值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static int GetEnumValue(Type enumType, string description)
        {
            EnumItemDescriptionList enumList = EnumItemDescriptionAttribute.GetDescriptionList(enumType);

            EnumItemDescription found = null;

            foreach (EnumItemDescription item in enumList)
            {
                if (item.Description == description)
                {
                    found = item;
                    break;
                }
            }

            if (found == null)
            {
                throw new ArgumentException(string.Format("无法在类型为{0}的枚举中找到描述为{1}的枚举对象", enumType.Name, description));
            }

            return found.EnumValue;
        }


        /// <summary>
        /// 根据值找到枚举对象的描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Type enumType, int value)
        {
            Enum item = (Enum)Enum.ToObject(enumType, value);
            return EnumItemDescriptionAttribute.GetDescription(item);
        }
    }
}
