using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Plugin.OAMessage
{
    /// <summary>
    /// This object represents the properties and methods of a TemplateAttachment.
    /// </summary>
    public class OAMessageEntity
    {
        #region
        ///// <summary>
        ///// 返回结果
        ///// </summary>
        //public string ResultData { get; set; }
        ///// <summary>
        ///// 流程ID
        ///// </summary>
        //public string FlowID { get; set; }
        ///// <summary>
        ///// 流程标题
        ///// </summary>
        //public string FlowTitle { get; set; }
        ///// <summary>
        ///// 使用工作流名称
        ///// </summary>
        //public string WorkflowName { get; set; }
        ///// <summary>
        ///// 当前用户节点名称
        ///// </summary>
        //public string NodeName { get; set; }
        ///// <summary>
        ///// 电脑端审批地址
        ///// </summary>
        //public string PCUrl { get; set; }
        ///// <summary>
        ///// 手机端审批地址
        ///// </summary>
        //public string AppUrl { get; set; }
        ///// <summary>
        ///// 创建流程用户
        ///// </summary>
        //public string CreateFlowUser { get; set; }
        ///// <summary>
        ///// 创建流程时间
        ///// </summary>
        //public DateTime CreateFlowTime { get; set; }
        ///// <summary>
        ///// 接收流程用户
        ///// </summary>
        //public string ReceiverFlowUser { get; set; }
        ///// <summary>
        ///// 接收流程时间
        ///// </summary>
        //public DateTime ReceiverFlowTime { get; set; }
        ///// <summary>
        ///// 流程处理状态 0待办 2已办 4办结
        ///// </summary>
        //public int FlowType { get; set; }
        ///// <summary>
        ///// 流程查看状态 0未读 1已读
        ///// </summary>
        //public int ViewType { get; set; }
        #endregion
            
        /// <summary>
        /// 创建人
        /// </summary>
        public string LOGINID { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SYSSHORTNAME { get; set; }
        /// <summary>
        /// PC端连接地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// flowId
        /// </summary>
        public string REQUESTID { get; set; }
        /// <summary>
        /// 未知，值均为空
        /// </summary>
        public string REQUESTMARK { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public string CREATEDATE { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CREATETIME { get; set; }
        /// <summary>
        /// 未知。值为数字，猜测为创建人id
        /// </summary>
        private int CREATER { get; set; }
        /// <summary>
        /// 未知，值均为0
        /// </summary>
        public int CREATERTYPE { get; set; }
        /// <summary>
        /// 未知。不同流程存在值相同的情况
        /// </summary>
        public int WORKFLOWID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string REQUESTNAME { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string REQUESTNAMENEW { get; set; }
        /// <summary>
        /// 未知。字段均为空值
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 级别。默认为-1
        /// </summary>
        public int REQUESTLEVEL { get; set; }
        /// <summary>
        /// 当前结点id。默认为-1
        /// </summary>
        public int CURRENTNODEID { get; set; }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CURRENTNODENAME { get; set; }
        /// <summary>
        /// 流程查看状态。0：未读；1：已读；
        /// </summary>
        public int VIEWTYPE { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string JSRLOGINID { get; set; }
        /// <summary>
        /// 接受时间（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public string RECEIVE { get; set; }
        /// <summary>
        /// 接受日期
        /// </summary>
        public string RECEIVEDATE { get; set; }
        /// <summary>
        /// 接受时间
        /// </summary>
        public string RECEIVETIME { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string OPERATE { get; set; }
        /// <summary>
        /// 流程处理状态 0：待办；2：已办；4：办结
        /// </summary>
        public string ISREMARK { get; set; }
        /// <summary>
        /// 未知。值为0
        /// </summary>
        public string NODEID { get; set; }
        /// <summary>
        /// 未知。值为-1；
        /// </summary>
        public string AGENTORBYAGENTID { get; set; }
        /// <summary>
        /// 未知。值为0
        /// </summary>
        public string AGENTTYPE { get; set; }
        /// <summary>
        /// 未知。值为0
        /// </summary>
        public string ISPROCESSED { get; set; }
        /// <summary>
        /// 未知。值为1
        /// </summary>
        public string SYSTYPE { get; set; }
        /// <summary>
        /// 未知。各系统值不一样
        /// </summary>
        public string WORKFLOWTYPE { get; set; }
        /// <summary>
        /// 工作流名称
        /// </summary>
        public string WORKFLOWNAME { get; set; }
        /// <summary>
        /// 系统编码
        /// </summary>
        public string SYSCODE { get; set; }

    }
}

