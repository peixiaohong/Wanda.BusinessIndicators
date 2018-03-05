using System;
using System.Configuration;
using Lib.Config;
using System.Globalization;

namespace Lib.Globalization
{
    /// <summary>
    /// 翻译器的配置节
    /// </summary>
    public sealed class TranslatorSettings : ConfigurationSection
    {
        /// <summary>
        /// 得到配置节
        /// </summary>
        /// <returns></returns>
        public static TranslatorSettings GetConfig()
        {
            TranslatorSettings result = (TranslatorSettings)ConfigurationBroker.GetSection("translatorSettings");

            if (result == null)
                result = new TranslatorSettings();

            return result;
        }

        /// <summary>
        /// 缺省的文种
        /// </summary>
        public CultureInfo DefaultCulture
        {
            get
            {
                CultureInfo result;

                if (CultureInfoContextCache.Instance.TryGetValue(DefaultCultureString, out result) == false)
                {
                    result = new CultureInfo(DefaultCultureString);

                    CultureInfoContextCache.Instance.Add(DefaultCultureString, result);
                }

                return result;
            }
        }

        /// <summary>
        /// 默认的Culture
        /// </summary>
        [ConfigurationProperty("defaultCulture", IsRequired = false, DefaultValue = "zh-CN")]
        private string DefaultCultureString
        {
            get
            {
                return (string)this["defaultCulture"];
            }
        }

        /// <summary>
        /// 翻译器
        /// </summary>
        public ITranslator Translator
        {
            get
            {
                ITranslator translator = null;

                if (TypeFactories.Count > 0)
                    translator = (ITranslator)TypeFactories[0].CreateInstance();
                else
                    translator = new DefaultTranslator();

                return translator;
            }
        }

        [ConfigurationProperty("typeFactories", IsRequired = false)]
        private TypeConfigurationCollection TypeFactories
        {
            get
            {
                return (TypeConfigurationCollection)this["typeFactories"];
            }
        }
    }
}
