using System;
using System.Collections.Generic;
using System.Text;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// BeforeExecute，SaveApplicationData，AfterExecute中传递的参数
    /// </summary>
    public class WFExecuteArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="param">流程执行的参数</param>
        public WFExecuteArgs(WFExecuteParameter param)
        {
            this.ExecuteParameter = param;
        }
        /// <summary>
        /// 流程执行的参数
        /// </summary>
        public WFExecuteParameter ExecuteParameter { get; private set; }
    }
}
