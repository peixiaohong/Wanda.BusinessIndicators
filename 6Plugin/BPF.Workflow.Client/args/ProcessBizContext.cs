using System;
using System.Collections.Generic;
using System.Text;
using Wanda.Workflow.Object;

namespace Wanda.Workflow.Client
{
    /// <summary>
    /// 流程上下文
    /// </summary>
    public class ProcessBizContext : Dictionary<string, object>
    {
        internal ProcessBizContext(BizContext processInfo)
        {
            this.ProcessContext = processInfo;
        }

        public BizContext ProcessContext { get; internal set; }
        public WorkflowContext WorkflowContext { get; internal set; }
    }
}
