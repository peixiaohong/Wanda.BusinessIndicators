using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Lib.Xml;

namespace Lib.Data
{
    /// <summary>
    /// 常用的值文对对象， Text 表示显示文本，Value表示值（泛型，可以是数值，字符串等）
    /// </summary>
    /// <remarks>readonly, 一旦创建不允许在应用中修改</remarks>
    /// <typeparam name="T">表示value的类型</typeparam>
    [Serializable]
    public class ValueText<T>
    {
        public string Text { get; internal set; }
        public T Value { get; internal set; }
    }

    [Serializable]
    public class ValueTwoTexts<T> : ValueText<T>
    {
        public string Text2 { get; internal set; }
    }


    [Serializable]
    public class ValueThreeTexts<T> : ValueTwoTexts<T>
    {
        public string Text3 { get; internal set; }
    }



    [Serializable]
    public class ValueFourTexts<T> : ValueThreeTexts<T>
    {
        public string Text4 { get; internal set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class ValueTextFactory
    {
        /// <summary>
        /// 创建一个ValueText对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">显示文字</param>
        /// <param name="value">实际值</param>
        /// <returns>ValueText对象</returns>
        public static ValueText<T> Create<T>(string text, T value)
        {
            return new ValueText<T> { Text = text, Value = value };
        }

        /// <summary>
        /// 创建一个ValueTwoTexts对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">显示文字</param>
        /// <param name="value">实际值</param>
        /// <returns>ValueText对象</returns>
        public static ValueTwoTexts<T> Create<T>(string text, string text2, T value)
        {
            return new ValueTwoTexts<T> { Text = text, Text2 = text2, Value = value };
        }


        /// <summary>
        /// 创建一个ValueThreeTexts对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">显示文字</param>
        /// <param name="value">实际值</param>
        /// <returns>ValueText对象</returns>
        public static ValueThreeTexts<T> Create<T>(string text, string text2, string text3, T value)
        {
            return new ValueThreeTexts<T> { Text = text, Text2 = text2, Text3 = text3, Value = value };
        }


        /// <summary>
        /// 创建一个ValueThreeTexts对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">显示文字</param>
        /// <param name="value">实际值</param>
        /// <returns>ValueText对象</returns>
        public static ValueFourTexts<T> Create<T>(string text, string text2, string text3, string text4, T value)
        {
            return new ValueFourTexts<T> { Text = text, Text2 = text2, Text3 = text3, Text4 = text4, Value = value };
        }
        /// <summary>
        /// 创建一个ValueText对象, 从指定的XElement对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <remarks>
        /// XElement 格式如<![CDATA[
        ///   <item text="" value=""></item>
        /// ]]>
        /// </remarks>
        public static ValueText<T> CreateValueText<T>(this XElement element)
        {
            if (element == null)
            {
                return null;
            }
            string text = element.GetAttributeValue("text", "");
            T value = element.GetAttributeValue("value", default(T));
            return Create<T>(text, value);
        }

        /// <summary>
        /// 创建一个ValueTwoTexts对象, 从指定的XElement对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <remarks>
        /// XElement 格式如<![CDATA[
        ///   <item text="" text2="" value=""></item>
        /// ]]>
        /// </remarks>
        public static ValueTwoTexts<T> CreateValueTwoTexts<T>(this XElement element)
        {
            if (element == null)
            {
                return null;
            }
            string text = element.GetAttributeValue("text", "");
            string text2 = element.GetAttributeValue("text2", "");
            T value = element.GetAttributeValue("value", default(T));
            return Create<T>(text, text2, value);
        }


        /// <summary>
        /// 创建一个ValueThreeTexts对象, 从指定的XElement对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <remarks>
        /// XElement 格式如<![CDATA[
        ///   <item text="" text2="" text3="" value=""></item>
        /// ]]>
        /// </remarks>
        public static ValueThreeTexts<T> CreateValueThreeTexts<T>(this XElement element)
        {
            if (element == null)
            {
                return null;
            }
            string text = element.GetAttributeValue("text", "");
            string text2 = element.GetAttributeValue("text2", "");
            string text3 = element.GetAttributeValue("text3", "");
            T value = element.GetAttributeValue("value", default(T));
            return Create<T>(text, text2, text3, value);
        }


        /// <summary>
        /// 创建一个ValueFourTexts对象, 从指定的XElement对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <remarks>
        /// XElement 格式如<![CDATA[
        ///   <item text="" text2="" text3="" text4="" value=""></item>
        /// ]]>
        /// </remarks>
        public static ValueFourTexts<T> CreateValueFourTexts<T>(this XElement element)
        {
            if (element == null)
            {
                return null;
            }
            string text = element.GetAttributeValue("text", "");
            string text2 = element.GetAttributeValue("text2", "");
            string text3 = element.GetAttributeValue("text3", "");
            string text4 = element.GetAttributeValue("text4", "");
            T value = element.GetAttributeValue("value", default(T));
            return Create<T>(text, text2, text3, text4, value);
        }

        /// <summary>
        /// 创建一个ValueText对象, 从指定的XElement对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ValueText<T> CreateValueText<T>(this XElement element, string textAttrName, string valueAttrName)
        {
            if (element == null)
            {
                return null;
            }

            string text = element.GetAttributeValue(textAttrName, "");
            T value = element.GetAttributeValue(valueAttrName, default(T));
            return Create<T>(text, value);
        }

        /// <summary>
        /// 创建一个ValueText对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static List<ValueText<T>> CreateValueTextList<T>(this XElement element, string childName = "item")
        {
            if (element == null)
            {
                return new List<ValueText<T>>();
            }
            var elements = element.Elements(childName);

            List<ValueText<T>> result = elements.Select(e => CreateValueText<T>(e)).ToList();
            return result;
        }

        /// <summary>
        /// 创建一个ValueTwoTexts对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static List<ValueTwoTexts<T>> CreateValueTwoTextsList<T>(this XElement element, string childName = "item")
        {
            if (element == null)
            {
                return new List<ValueTwoTexts<T>>();
            }
            var elements = element.Elements(childName);

            List<ValueTwoTexts<T>> result = elements.Select(e => CreateValueTwoTexts<T>(e)).ToList();
            return result;
        }
        /// <summary>
        /// 创建一个ValueThreeTexts对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static List<ValueThreeTexts<T>> CreateValueThreeTextsList<T>(this XElement element, string childName = "item")
        {
            if (element == null)
            {
                return new List<ValueThreeTexts<T>>();
            }
            var elements = element.Elements(childName);

            List<ValueThreeTexts<T>> result = elements.Select(e => CreateValueThreeTexts<T>(e)).ToList();
            return result;
        }

        /// <summary>
        /// 创建一个ValueFourTexts对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static List<ValueFourTexts<T>> CreateValueFourTextsList<T>(this XElement element, string childName = "item")
        {
            if (element == null)
            {
                return new List<ValueFourTexts<T>>();
            }
            var elements = element.Elements(childName);

            List<ValueFourTexts<T>> result = elements.Select(e => CreateValueFourTexts<T>(e)).ToList();
            return result;
        }
        /// <summary>
        /// 创建一个ValueText对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static List<ValueText<T>> CreateValueTextList<T>(this XElement element, string textAttrName, string valueAttrName, string childName = "item")
        {
            if (element == null)
            {
                return new List<ValueText<T>>();
            }
            var elements = element.Elements(childName);

            List<ValueText<T>> result = elements.Select(e => CreateValueText<T>(e, textAttrName, valueAttrName)).ToList();
            return result;
        }
    }


}
