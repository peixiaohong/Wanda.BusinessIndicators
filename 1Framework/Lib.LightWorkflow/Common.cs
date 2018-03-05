using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Lib.Core;

namespace Wanda.Lib.LightWorkflow
{
    [Serializable]
    public class CommonConsts
    {
        public const int LaunchNodeSeqNumber = 1;
        public const int CCNodeSeqNumber = 1000;
        public const int ArchiveNodeSeqNumber = 5000;

        /// <summary>
        ///  流程实例状态
        /// </summary>
        public enum ProcessInstanceStatus : int
        {
            /// <summary>
            /// 待上报
            /// </summary>
            [Description("待上报")]
            [EnumItemDescription("待上报")]
            Reporting = 1,
            /// <summary>
            /// 审批中（原：运行中） by zhengwei 2012-02-17
            /// </summary>
            [Description("审批中")]
            [EnumItemDescription("审批中")]
            Running = 2,
            /// <summary>
            /// 归档中（审批完成）
            /// </summary>
            [Description("归档中")]
            [EnumItemDescription("归档中")]
            Archiving = 4,
            /// <summary>
            /// 被退回(流程结束)
            /// </summary>
            [Description("被退回")]
            [EnumItemDescription("被退回")]
            Rejected = 8,
            /// <summary>
            /// 已取消
            /// </summary>
            [Description("已撤消")]
            [EnumItemDescription("已撤消")]
            Canceled = 16,
            /// <summary>
            /// 已办结（流程结束）
            /// </summary>
            [Description("已办结")]
            [EnumItemDescription("已办结")]
            Finished = 32,
            /// <summary>
            /// 已撤回
            /// </summary>
            [Description("已撤回")]
            [EnumItemDescription("已撤回")]
            WidthDrawed = 64,
        };

        /// <summary>
        /// 子流程类型
        /// </summary>
        public enum SubProcessType
        {
            /// <summary>
            /// 项目公司
            /// </summary>
            [EnumItemDescription("项目公司")]
            [Description("项目公司")]
            项目公司,

            /// <summary>
            /// 商管公司
            /// </summary>
            [EnumItemDescription("商管公司")]
            [Description("商管公司")]
            商管公司,

            /// <summary>
            /// 商业地产
            /// </summary>
            [EnumItemDescription("商业地产")]
            [Description("商业地产")]
            商业地产
        }

        /// <summary>
        /// 流程节点类型
        /// </summary>
        public enum NodeType : int
        {
            /// <summary>
            /// 发起，上报
            /// </summary>
            [Description("上报")]
            [EnumItemDescription("上报")]
            Launch = 1,

            /// <summary>
            /// 审批
            /// </summary>
            [Description("审批")]
            [EnumItemDescription("审批")]
            Approval = 2,

            /// <summary>
            /// 复核
            /// </summary>
            [Description("复核")]
            [EnumItemDescription("复核")]
            Review = 4,

            /// <summary>
            /// 归档
            /// </summary>
            [Description("归档")]
            [EnumItemDescription("归档")]
            Archive = 8,

            /// <summary>
            /// 抄送
            /// </summary>
            [Description("抄送")]
            [EnumItemDescription("抄送")]
            CC = 16,

            /// <summary>
            /// 转发
            /// </summary>
            [Description("转发")]
            [EnumItemDescription("转发")]
            Forward = 32,

            /// <summary>
            /// 委托代理
            /// </summary>
            [Description("委托")]
            [EnumItemDescription("委托")]
            Entrust = 64
        }

        /// <summary>
        /// 节点执行状态
        /// </summary>
        public enum NodeStatus : int
        {
            /// <summary>
            /// 执行中
            /// </summary>
            [Description("执行中")]
            [EnumItemDescription("执行中")]
            Executing = 1,

            /// <summary>
            /// 已执行
            /// </summary>
            [Description("已执行")]
            [EnumItemDescription("已执行")]
            Executed = 2,

            /// <summary>
            /// 未执行
            /// </summary>
            [Description("未执行")]
            [EnumItemDescription("未执行")]
            NotExecute = 4

        }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Serializable]
        public enum NodeOperationType : int
        {
            /// <summary>
            /// 上报
            /// </summary>
            [Description("上报")]
            [EnumItemDescription("上报")]
            Launch = 1,

            /// <summary>
            /// 批准
            /// </summary>
            [Description("批准")]
            [EnumItemDescription("批准")]
            Approved = 2,

            /// <summary>
            /// 退回
            /// </summary>
            [Description("退回")]
            [EnumItemDescription("退回")]
            Reject = 3,

            /// <summary>
            /// 撤销
            /// </summary>
            [Description("撤销")]
            [EnumItemDescription("撤销")]
            Cancel = 4,

            /// <summary>
            /// 归档
            /// </summary>
            [Description("归档")]
            [EnumItemDescription("归档")]
            Archive = 5,

            /// <summary>
            /// 批注:在页面上将批注按钮显示文字为提交
            /// </summary>
            [Description("提交")]
            [EnumItemDescription("提交")]
            Comment = 6,

            /// <summary>
            /// 委托entrust
            /// </summary>
            [Description("委托")]
            [EnumItemDescription("委托")]
            Entrust = 7,

            /// <summary>
            /// 转发(只有一个转发按钮时,不显示审批框,只能选择一个或多个用户转发)
            /// </summary>
            [Description("转发")]
            [EnumItemDescription("转发")]
            Forward = 8,

            /// <summary>
            /// 调整节点
            /// </summary>
            [Description("调整节点")]
            [EnumItemDescription("调整节点")]
            ChangeNode = 9,
            /// <summary>
            /// 撤回
            /// </summary>
            [Description("撤回")]
            [EnumItemDescription("撤回")]
            WidthDraw = 10
        }

        /// <summary>
        /// 待办的查看状态
        /// </summary>
        public enum TodoStatus : int
        {
            /// <summary>
            /// 未读
            /// </summary>
            [Description("未读")]
            [EnumItemDescription("未读")]
            Unread = 0,

            /// <summary>
            /// 已读
            /// </summary>
            [Description("已读")]
            [EnumItemDescription("已读")]
            Read = 1
        }

        /// <summary>
        /// 待办类型
        /// </summary>
        public enum TodoType : int
        {
            /// <summary>
            /// 待办
            /// </summary>
            [Description("待办")]
            [EnumItemDescription("待办")]
            TODO = 1,

            /// <summary>
            /// 通知
            /// </summary>
            [Description("通知")]
            [EnumItemDescription("通知")]
            Notification = 2
        }

        public enum MyFlowType : int
        {
            /// <summary>
            /// 我的未办流程
            /// </summary>
            [Description("未办")]
            [EnumItemDescription("未办")]
            TODO = 1,

            /// <summary>
            /// 我的已办流程
            /// </summary>
            [Description("已办")]
            [EnumItemDescription("已办")]
            Done = 2
        }
    }
}
