using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    public enum WFOperationType
    {
        /// <summary>
        /// 保存
        /// </summary>
        Save = 0,
        /// <summary>
        /// 流转
        /// </summary>
        MoveTo = 1,

        /// <summary>
        /// 转发
        /// </summary>
        Forward = 2,

        /// <summary>
        /// 前加签
        /// </summary>
        AddBefore = 3,

        /// <summary>
        /// 后加签
        /// </summary>
        AddAfter = 4,

        /// <summary>
        /// 会签
        /// </summary>
        Consign = 5,

        /// <summary>
        /// 退回
        /// </summary>
        Return = 6,

        /// <summary>
        /// 撤回
        /// </summary>
        Withdraw = 7,

        /// <summary>
        /// 抄送
        /// </summary>
        Cc = 8,

        /// <summary>
        /// 作废
        /// </summary>
        Cancel = 9,

        /// <summary>
        /// 传阅
        /// </summary>
        Circulate = 10,
    }

    public class WfOperationInfo
    {
        public int OperationType { get; set; }
    }
}