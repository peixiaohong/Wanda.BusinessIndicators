using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    public partial class ProcessLog : BaseProcessObject
    {
        /// <summary>
        /// 日志ID
        /// </summary>		
        public string LogID
        {
            get
            {
                return _LogID;
            }
            set
            {
                _LogID = value;
            }
        }private string _LogID = string.Empty;

        /// <summary>
        /// 流程ID
        /// </summary>		
        //public string ProcessID
        //{
        //    get
        //    {
        //        return _ProcessID;
        //    }
        //    set
        //    {
        //        _ProcessID = value;
        //    }
        //}private string _ProcessID = string.Empty;


        /// <summary>
        /// 节点ID
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
        /// 节点名称
        /// </summary>		
        public string NodeName
        {
            get
            {
                return _NodeName;
            }
            set
            {
                _NodeName = value;
            }
        }private string _NodeName = string.Empty;

        /// <summary>
        /// 节点类型
        /// </summary>		
        public int NodeType
        {
            get
            {
                return _NodeType;
            }
            set
            {
                _NodeType = value;
            }
        }private int _NodeType;

        /// <summary>
        /// 操作动作名称
        /// </summary>		
        public string OpertationName
        {
            get
            {
                return _OpertationName;
            }
            set
            {
                _OpertationName = value;
            }
        }private string _OpertationName;

        /// <summary>
        /// 用户信息
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
        /// 日志内容
        /// </summary>		
        public string LogContent
        {
            get
            {
                return _LogContent;
            }
            set
            {
                _LogContent = value;
            }
        }private string _LogContent = string.Empty;

        /// <summary>
        /// 日志类型，1：审批日志，2：普通运维日志，3：高级运维日志
        /// </summary>		
        public int LogType
        {
            get
            {
                return _LogType;
            }
            set
            {
                _LogType = value;
            }
        }private int _LogType;

        /// <summary>
        /// 接收时间
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
        /// 完成时间
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
    }
}
