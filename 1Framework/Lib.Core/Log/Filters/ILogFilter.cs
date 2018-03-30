using System;
using Lib.Core;

namespace Lib.Log
{
    /// <summary>
    /// 接口，定义日志过滤器
    /// </summary>
    internal interface ILogFilter : IFilter<LogEntity>
    {
        /// <summary>
        /// 名称
        /// </summary>
        new string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 接口方法，实现日志过滤
        /// </summary>
        /// <param name="log">日志对象</param>
        /// <returns>布尔值，true：通过，false：不通过</returns>
        new bool IsMatch(LogEntity log);
    }
}
