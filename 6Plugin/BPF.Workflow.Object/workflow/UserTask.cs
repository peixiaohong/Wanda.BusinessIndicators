using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 待办和已办的实例对象
    /// </summary>
    public class UserTask : BaseProcessObject
    {

        /// <summary>
        /// 待办ID
        /// </summary>		
        public string TaskID
        {
            get
            {
                return _TaskID;
            }
            set
            {
                _TaskID = value;
            }
        }private string _TaskID = string.Empty;

        /// <summary>
        /// 任务标题
        /// </summary>		
        public string TaskTitle
        {
            get
            {
                return _TaskTitle;
            }
            set
            {
                _TaskTitle = value;
            }
        }private string _TaskTitle = string.Empty;

        /// <summary>
        /// 任务URL
        /// </summary>		
        public string TaskURL
        {
            get
            {
                return _TaskURL;
            }
            set
            {
                _TaskURL = value;
            }
        }private string _TaskURL = string.Empty;

        /// <summary>
        /// 任务移动端URL
        /// </summary>		
        public string TaskMobileURL
        {
            get
            {
                return _TaskMobileURL;
            }
            set
            {
                _TaskMobileURL = value;
            }
        }private string _TaskMobileURL = string.Empty;

        /// <summary>
        /// 任务类型，1：工作流任务，2：业务应用任务
        /// </summary>		
        public int TaskType
        {
            get
            {
                return _TaskType;
            }
            set
            {
                _TaskType = value;
            }
        }private int _TaskType;

        /// <summary>
        /// 任务动作，1：发起，2：审批，3：批注，4：确认
        /// </summary>
        public int TaskAction
        {
            get
            {
                return _TaskAction;
            }
            set
            {
                _TaskAction = value;
            }
        }private int _TaskAction;

        /// <summary>
        /// 节点ID
        /// 产生待办的节点ID，如果是转发、抄送待办，就用这个用户最后一个审判类节点的ID，如果没有，就为空字符串
        /// </summary>		
        public string NodeID
        {
            get
            {
                return _NodeID;
            }
            set
            {
                _NodeID = value;
            }
        }private string _NodeID = string.Empty;

        /// <summary>
        /// 待办用户
        /// </summary>		
        public UserInfo User
        {
            get
            {
                return _User;
            }
            set
            {
                _User = value;
            }
        }private UserInfo _User = null;

        /// <summary>
        /// 请求用户（创建待办的用户）
        /// </summary>		
        public UserInfo RequestUser
        {
            get
            {
                return _RequestUser;
            }
            set
            {
                _RequestUser = value;
            }
        }private UserInfo _RequestUser = null;

        /// <summary>
        /// 请求时间（待办创建时间）
        /// </summary>		
        public DateTime RequestDateTime
        {
            get
            {
                return _RequestDateTime;
            }
            set
            {
                _RequestDateTime = value;
            }
        }private DateTime _RequestDateTime = DateTime.MinValue;

        /// <summary>
        /// 待办办结时间
        /// </summary>		
        public DateTime FinishDateTime
        {
            get
            {
                return _FinishDateTime;
            }
            set
            {
                _FinishDateTime = value;
            }
        }private DateTime _FinishDateTime = DateTime.MinValue;

        /// <summary>
        /// 已读状态，0，未读；1，已读
        /// </summary>		
        public bool IsRead
        {
            get
            {
                return _IsRead;
            }
            set
            {
                _IsRead = value;
            }
        }private bool _IsRead = false;
        /// <summary>
        /// 读取时间
        /// </summary>		
        public DateTime ReadDatetime
        {
            get
            {
                return _ReadDatetime;
            }
            set
            {
                _ReadDatetime = value;
            }
        }private DateTime _ReadDatetime = DateTime.MinValue;
        /// <summary>
        /// 流程图Code
        /// </summary>		
        public string FlowCode
        {
            get
            {
                return _FlowCode;
            }
            set
            {
                _FlowCode = value;
            }
        }private string _FlowCode = string.Empty;

        /// <summary>
        /// 流程图名称
        /// </summary>		
        public string FlowName
        {
            get
            {
                return _FlowName;
            }
            set
            {
                _FlowName = value;
            }
        }private string _FlowName = string.Empty;
    }
}
