using System;
using System.Collections.Generic;
using System.Text;
namespace BPF.Workflow.Object
{
    /// <summary>
    /// 流程实例对象
    /// </summary>
    //WWF_Process的精简版本，部分用于流程计算的字段都没有包含，同时，所有的user相关字段被UserInfo对象重新封装
    public class Process : BaseProcessObject
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>		
        public string ProcessID
        {
            get
            {
                return _ProcessID;
            }
            set
            {
                _ProcessID = value;
            }
        }private string _ProcessID = string.Empty;
        /// <summary>
        /// 流程标题
        /// </summary>		
        public string ProcessTitle
        {
            get
            {
                return _ProcessTitle;
            }
            set
            {
                _ProcessTitle = value;
            }
        }private string _ProcessTitle = string.Empty;

        /// <summary>
        /// 流程的URL
        /// </summary>		
        public string ProcessURL
        {
            get
            {
                return _ProcessURL;
            }
            set
            {
                _ProcessURL = value;
            }
        }private string _ProcessURL = string.Empty;

        /// <summary>
        /// 流程的移动端URL
        /// </summary>		
        public string ProcessMobileURL
        {
            get
            {
                return _ProcessMobileURL;
            }
            set
            {
                _ProcessMobileURL = value;
            }
        }private string _ProcessMobileURL = string.Empty;

        /// <summary>
        /// 流程状态，0：未运行，草稿，1：运行中，2：退回，3：审批完成，4：归档，-1：作废，-2：驳回后归档
        /// </summary>		
        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }private int _Status = 0;

        /// <summary>
        /// 运行次数，发起人每发起一次加1
        /// </summary>		
        public int RunTimes
        {
            get
            {
                return _RunTimes;
            }
            set
            {
                _RunTimes = value;
            }
        }private int _RunTimes = 0;

        /// <summary>
        /// 流程创建时间
        /// </summary>		
        public DateTime CreateDateTime
        {
            get
            {
                return _CreateDateTime;
            }
            set
            {
                _CreateDateTime = value;
            }
        }private DateTime _CreateDateTime = DateTime.MinValue;

        /// <summary>
        /// 创建流程的用户对象
        /// </summary>		
        public UserInfo CreateUser
        {
            get
            {
                return _CreateUser;
            }
            set
            {
                _CreateUser = value;
            }
        }private UserInfo _CreateUser = null;

        /// <summary>
        /// 流程启动时间
        /// </summary>		
        public DateTime StartDateTime
        {
            get
            {
                return _StartDateTime;
            }
            set
            {
                _StartDateTime = value;
            }
        }private DateTime _StartDateTime = DateTime.MinValue;

        /// <summary>
        /// 流程启动用户对象
        /// </summary>		
        public UserInfo StartUser
        {
            get
            {
                return _StartUser;
            }
            set
            {
                _StartUser = value;
            }
        }private UserInfo _StartUser = null;

        /// <summary>
        /// 流程审批完成时间
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
        /// 流程审批完成用户对象
        /// </summary>		
        public UserInfo FinishUser
        {
            get
            {
                return _FinishUser;
            }
            set
            {
                _FinishUser = value;
            }
        }private UserInfo _FinishUser = null;


        /// <summary>
        /// 最后更新时间
        /// </summary>		
        public DateTime UpdateDateTime
        {
            get
            {
                return _UpdateDateTime;
            }
            set
            {
                _UpdateDateTime = value;
            }
        }private DateTime _UpdateDateTime = DateTime.MinValue;

        /// <summary>
        /// 最后更新用户对象
        /// </summary>		
        public UserInfo UpdateUser
        {
            get
            {
                return _UpdateUser;
            }
            set
            {
                _UpdateUser = value;
            }
        }private UserInfo _UpdateUser = null;
        /// <summary>
        /// 流程图ID
        /// </summary>		
        public string FlowID
        {
            get
            {
                return _FlowID;
            }
            set
            {
                _FlowID = value;
            }
        }private string _FlowID = string.Empty;

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

        /// <summary>
        /// 流程当前正在运行的节点ID，发起的流程，当前运行节点ID为发起节点ID，如果为空，表示流程已经执行完成。
        /// 注意：该节点并非当前用户所在的节点（WorkflowContext.CurrentUserNodeID）
        /// </summary>
        public string RunningNodeID
        {
            get { return _RunningNodeID; }
            set { _RunningNodeID = value; }
        }private string _RunningNodeID = string.Empty;

        ///////////////////////////////////////////////////////////////////////
        //                  以下字段在WWF_Process中不存在                    //
        ///////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// 流程发起节点ID
        /// </summary>
        public string StartNodeID
        {
            get { return _StartNodeID; }
            set { _StartNodeID = value; }
        }private string _StartNodeID = string.Empty;
    }
}
