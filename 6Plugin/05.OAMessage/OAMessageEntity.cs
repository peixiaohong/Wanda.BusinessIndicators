using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;

namespace Plugin.OAMessage
{
    /// <summary>
    /// This object represents the properties and methods of a TemplateAttachment.
    /// </summary>
    [ORTableMapping("dbo.OAMessage")]
    public class OAMessageEntity : BaseModel
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        [ORFieldMapping("ResultData")]
        public string ResultData { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        [ORFieldMapping("FlowID")]
        public string FlowID { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [ORFieldMapping("FlowTitle")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 使用工作流名称
        /// </summary>
        [ORFieldMapping("WorkflowName")]
        public string WorkflowName { get; set; }
        /// <summary>
        /// 当前用户节点名称
        /// </summary>
        [ORFieldMapping("NodeName")]
        public string NodeName { get; set; }
        /// <summary>
        /// 电脑端审批地址
        /// </summary>
        [ORFieldMapping("PCUrl")]
        public string PCUrl { get; set; }
        /// <summary>
        /// 手机端审批地址
        /// </summary>
        [ORFieldMapping("AppUrl")]
        public string AppUrl { get; set; }
        /// <summary>
        /// 创建流程用户
        /// </summary>
        [ORFieldMapping("CreateFlowUser")]
        public string CreateFlowUser { get; set; }
        /// <summary>
        /// 创建流程时间
        /// </summary>
        [ORFieldMapping("CreateFlowTime")]
        public DateTime CreateFlowTime { get; set; }
        /// <summary>
        /// 接收流程用户
        /// </summary>
        [ORFieldMapping("ReceiverFlowUser")]
        public string ReceiverFlowUser { get; set; }
        /// <summary>
        /// 接收流程时间
        /// </summary>
        [ORFieldMapping("ReceiverFlowTime")]
        public DateTime ReceiverFlowTime { get; set; }
        /// <summary>
        /// 流程处理状态 0待办 2已办 4办结
        /// </summary>
        [ORFieldMapping("FlowType")]
        public int FlowType { get; set; }
        /// <summary>
        /// 流程查看状态 0未读 1已读
        /// </summary>
        [ORFieldMapping("ViewType")]
        public int ViewType { get; set; }


    }
}

