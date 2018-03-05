using System;

namespace Lib.Data
{
    /// <summary>
    /// 数据访问调用时机类型
    /// </summary>
    public enum DbEventType
    {
        /// <summary>
        /// 调用执行前
        /// </summary>
        BeforeExecution,
        /// <summary>
        /// 调用执行后
        /// </summary>
        AfterExecution,
        /// <summary>
        /// 调用异常阶段
        /// </summary>
        Exception
    }
}
