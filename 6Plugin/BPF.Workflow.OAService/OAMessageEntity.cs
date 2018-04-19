using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPF.OAMQServices
{
    /// <summary>
    /// This object represents the properties and methods of a TemplateAttachment.
    /// </summary>
    public class OAMessageEntity 
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public string ResultData { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string FlowTitle { get; set; }
        /// <summary>
        /// 使用工作流名称
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// 当前用户节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 电脑端审批地址
        /// </summary>
        public string PCUrl { get; set; }
        /// <summary>
        /// 手机端审批地址
        /// </summary>
        public string AppUrl { get; set; }
        /// <summary>
        /// 创建流程用户
        /// </summary>
        public string CreateFlowUser { get; set; }
        /// <summary>
        /// 创建流程时间
        /// </summary>
        public DateTime CreateFlowTime { get; set; }
        /// <summary>
        /// 接收流程用户
        /// </summary>
        public string ReceiverFlowUser { get; set; }
        /// <summary>
        /// 接收流程时间
        /// </summary>
        public DateTime ReceiverFlowTime { get; set; }
        /// <summary>
        /// 流程处理状态 0待办 2已办 4办结
        /// </summary>
        public int FlowType { get; set; }
        /// <summary>
        /// 流程查看状态 0未读 1已读
        /// </summary>
        public int ViewType { get; set; }


    }
}

