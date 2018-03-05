using System;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public enum LogicOperatorDefine
    {
        /// <summary>
        /// “与”操作
        /// </summary>
        [EnumItemDescription(Description = "“与”操作", ShortName = "AND")]
        And,

        /// <summary>
        /// “或”操作
        /// </summary>
        [EnumItemDescription(Description = "“或”操作", ShortName = "OR")]
        Or
    }
}
