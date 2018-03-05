using System;
using System.Globalization;

namespace Lib.Globalization
{
    /// <summary>
    /// 默认的翻译器
    /// </summary>
    public sealed class DefaultTranslator : ITranslator
    {
        #region ITranslator Members

        /// <summary>
        /// 直接翻译，不做转换
        /// </summary>
        /// <param name="category"></param>
        /// <param name="sourceCulture"></param>
        /// <param name="sourceText"></param>
        /// <param name="targetCulture"></param>
        /// <param name="objParams"></param>
        /// <returns></returns>
        public string Translate(string category, CultureInfo sourceCulture, string sourceText, CultureInfo targetCulture, params object[] objParams)
        {
            return sourceText;
        }

        #endregion
    }
}
