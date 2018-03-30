using System;
using Lib.Core;

namespace Lib.Log
{
    /// <summary>
    /// 日志优先级
    /// </summary>
    /// <remarks>
    /// 共分五级优先级
    /// </remarks>
    public enum LogPriority
    {
        /// <summary>
        /// 最低
        /// </summary>
        [EnumItemDescription("最低")]
        Lowest = 5,

        /// <summary>
        /// 低
        /// </summary>
        [EnumItemDescription("低")]
        BelowNormal = 4,

        /// <summary>
        /// 普通
        /// </summary>
        [EnumItemDescription("普通")]
        Normal = 3,

        /// <summary>
        /// 高
        /// </summary>
        [EnumItemDescription("高")]
        AboveNormal = 2,

        /// <summary>
        /// 最高
        /// </summary>
        [EnumItemDescription("最高")]
        Highest = 1
    }
}
