using System;
using System.Configuration;
using Lib.Core;
using System.Collections.Generic;
using Lib.Core.Properties;

namespace Lib.Config
{
    /// <summary>
    /// 以名字为键值的配置项
    /// </summary>
    public class NamedConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Uri的逻辑名称
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public virtual string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        [ConfigurationProperty("description", DefaultValue = "")]
        public virtual string Description
        {
            get
            {
                return (string)this["description"];
            }
        }

        /// <summary>
        /// Uri的地址字符串
        /// </summary>
        [ConfigurationProperty("uri")]
        private string Uri
        {
            get
            {
                return (string)this["uri"];
            }
        }
    }

    /// <summary>
    /// 以名字为键值的配置元素集合
    /// </summary>
    /// <typeparam name="T">配置元素类型</typeparam>
    public abstract class NamedConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : NamedConfigurationElement, new()
    {
        /// <summary>
        /// 按照序号获取指定的配置元素
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>配置元素</returns>
        public virtual T this[int index] { get { return (T)base.BaseGet(index); } }

        /// <summary>
        /// 按照名称获取指定的配置元素
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>配置元素</returns>
        public new T this[string name] { get { return (T)base.BaseGet(name); } }

        /// <summary>
        /// 是否包含指定的配置元素
        /// </summary>
        /// <param name="name">配置元素名称</param>
        /// <returns>是否包含</returns>
        public bool ContainsKey(string name) { return BaseGet(name) != null; }

        /// <summary>
        /// 得到元素的Key值
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>配置元素所对应的配置元素</returns>
        protected override object GetElementKey(ConfigurationElement element) { return ((T)element).Name; }

        /// <summary>
        /// 生成新的配置元素实例
        /// </summary>
        /// <returns>配置元素实例</returns>
        protected override ConfigurationElement CreateNewElement() { return new T(); }

        /// <summary>
        /// 通过name在字典内查找数据，如果name不存在，则抛出异常，本方法可重载
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>配置元素</returns>
        protected virtual T InnerGet(string name)
        {
            object element = base.BaseGet(name);
            ExceptionHelper.FalseThrow<KeyNotFoundException>(element != null, Resource.CanNotFindNamedConfigElement, name);
            return (T)element;
        }
    }
}
