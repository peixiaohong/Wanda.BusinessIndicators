using System;
using System.Collections.Generic;
using System.Text;
using Wanda.Workflow.Object;

namespace Wanda.Workflow.Client
{
    /// <summary>
    /// 流程页面接口
    /// </summary>
    public interface IActivityHost
    {
        /// <summary>
        /// 返回流程上下文WorkflowContext信息
        /// </summary>
        /// <param name="workflowContext"></param>
        void Resident(WorkflowContext workflowContext);

        /// <summary>
        /// 是否自动创建Process
        /// </summary>
        bool IsAutoCreate { get; }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        string CurrentUser { get; }

        /// <summary>
        /// BusinessID
        /// </summary>
        string BusinessID { get; }
    }
}
