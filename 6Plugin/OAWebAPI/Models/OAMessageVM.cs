using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plugin.OAMessage
{
    public class OAMessageVM
    {
        /// <summary>
        /// 异构系统标识
        /// </summary>
        public string syscode { get; set; }
        /// <summary>
        /// 流程任务id
        /// </summary>
        public string flowid { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string requestname { get; set; }
        /// <summary>
        /// 流程类型名称
        /// </summary>
        public string workflowname { get; set; }
        /// <summary>
        /// 步骤名称（节点名称）
        /// </summary>
        public string nodename { get; set; }
        /// <summary>
        /// PC地址
        /// </summary>
        public string pcurl { get; set; }
        /// <summary>
        /// APP地址
        /// </summary>
        public string appurl { get; set; }
        /// <summary>
        /// 流程处理状态 0：待办；2：已办；4：办结
        /// </summary>
        private string isremark { get; set; }
        /// <summary>
        /// 流程查看状态。0：未读；1：已读；
        /// </summary>
        private string viewtype { get; set; }
        /// <summary>
        /// 创建人（原值）
        /// </summary>
        public string creator { get; set; }
        public DateTime? createdatetime { get; set; }
        /// <summary>
        /// 接收人（原值）
        /// </summary>
        public string receiver { get; set; }
        public DateTime? receivedatetime { get; set; }
    }
}