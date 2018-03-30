using System;

namespace Lib.Log
{
    /// <summary>
    /// 格式化日志记录接口
    /// </summary>
    /// <remarks>
    /// 将日志记录对象LogEntity对象格式化成字符串的格式化器，通过实现该接口来定制格式化器，如：文件，xml
    /// </remarks>
    public interface ILogFormatter
    {
        /// <summary>
        /// 日志名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 将对象格式化成字符串
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        string Format(LogEntity log);
    }
}
