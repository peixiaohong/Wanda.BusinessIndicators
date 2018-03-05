using System;

namespace Lib.Data
{
    /// <summary>
    /// 为操作绑定标记
    /// </summary>
    [Flags]
    public enum ClauseBindingFlags
    {
        /// <summary>
        /// 任何情况下都不出现
        /// </summary>
        None = 0,

        /// <summary>
        /// 表示属性会出现在Insert中
        /// </summary>
        Insert = 1,

        /// <summary>
        /// 表示属性会出现在Update中
        /// </summary>
        Update = 2,

        /// <summary>
        /// 表示属性会出现在Where语句部分
        /// </summary>
        Where = 4,

        /// <summary>
        /// 表示属性会出现在查询的返回字段中
        /// </summary>
        Select = 8,

        /// <summary>
        /// 在所有情况下都会出现
        /// </summary>
        All = 15,
    }
}
