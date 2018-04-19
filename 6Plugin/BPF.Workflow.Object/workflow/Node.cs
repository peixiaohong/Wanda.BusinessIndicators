using System;
using System.Collections.Generic;
using System.Text;
namespace BPF.Workflow.Object
{
    /// <summary>
    /// 节点实例对象
    /// </summary>
    ////WWF_Node的精简版本，部分用于流程计算的字段都没有包含，同时，所有的user相关字段被UserInfo对象重新封装
    public class Node : BaseProcessObject
    {
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
        /// 节点所属的流程ID
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
        }private int _NodeType = -1;

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
        /// 节点编码
        /// </summary>		
        public string NodeCode
        {
            get
            {
                return _NodeCode;
            }
            set
            {
                _NodeCode = value;
            }
        }private string _NodeCode = string.Empty;

        /// <summary>
        /// 待办标题
        /// </summary>		
        public string NodeTitle
        {
            get
            {
                return _NodeTitle;
            }
            set
            {
                _NodeTitle = value;
            }
        }private string _NodeTitle = string.Empty;

        /// <summary>
        /// 节点的 URL
        /// </summary>		
        public string NodeURL
        {
            get
            {
                return _NodeURL;
            }
            set
            {
                _NodeURL = value;
            }
        }private string _NodeURL = string.Empty;

        /// <summary>
        /// 节点的移动端URL
        /// </summary>		
        public string NodeMobileURL
        {
            get
            {
                return _NodeMobileURL;
            }
            set
            {
                _NodeMobileURL = value;
            }
        }private string _NodeMobileURL = string.Empty;

        /// <summary>
        /// 节点状态，0：未执行，1：执行中，2：已执行
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
        /// 节点用户对象
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
        /// 父节点ID，审批节点的父节点默认都是‘’，会签和通知节点子节点的PNID附是会签节点的ID
        /// </summary>		
        public string ParentNodeID
        {
            get
            {
                return _ParentNodeID;
            }
            set
            {
                _ParentNodeID = value;
            }
        }private string _ParentNodeID = string.Empty;

        /// <summary>
        /// 下一个节点的ID，最后一个节点的NID为‘’，会签和通知节点子节点的NID为‘’
        /// </summary>		
        public string NextNodeID
        {
            get
            {
                return _NextNodeID;
            }
            set
            {
                _NextNodeID = value;
            }
        }private string _NextNodeID = string.Empty;

        /// <summary>
        /// 上一个节点的ID，发起节点的PID为‘’，会签和通知节点子节点的PID为‘’
        /// </summary>		
        public string PrevNodeID
        {
            get
            {
                return _PrevNodeID;
            }
            set
            {
                _PrevNodeID = value;
            }
        }private string _PrevNodeID = string.Empty;
        /// <summary>
        /// 节点顺序，默认为0，会签和通知节点子节点的会有值
        /// </summary>
        public int NodeOrder
        {
            get { return _NodeOrder; }
            set { _NodeOrder = value; }
        }private int _NodeOrder = 0;

        /// <summary>
        /// 克隆节点ID，加签等节点才会有值
        /// </summary>		
        public string CloneNodeID
        {
            get
            {
                return _CloneNodeID;
            }
            set
            {
                _CloneNodeID = value;
            }
        }private string _CloneNodeID = string.Empty;

        /// <summary>
        /// 克隆节点的名称
        /// </summary>		
        public string CloneNodeName
        {
            get
            {
                return _CloneNodeName;
            }
            set
            {
                _CloneNodeName = value;
            }
        }private string _CloneNodeName = string.Empty;

        /// <summary>
        /// 克隆节点类型
        /// </summary>		
        public int CloneNodeType
        {
            get
            {
                return _CloneNodeType;
            }
            set
            {
                _CloneNodeType = value;
            }
        }private int _CloneNodeType = 0;

        /// <summary>
        /// 是否删除，0：未删除，1：已删除
        /// </summary>		
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
            }
        }private bool _IsDeleted = false;

        /// <summary>
        /// 节点实例的扩展属性
        /// </summary>		
        public Dictionary<string, string> ExtendProperties
        {
            get
            {
                return _ExtendProperties;
            }
            set
            {
                _ExtendProperties = value;
            }
        }private Dictionary<string, string> _ExtendProperties = null;


        /// <summary>
        /// 流程定义的节点扩展属性
        /// WWF_Node是存储在ActivityDefine中
        /// </summary>
        public List<ActivityProperty> ActivityProperties
        {
            get { return _ActivityProperties; }
            set { _ActivityProperties = value; }
        }private List<ActivityProperty> _ActivityProperties = null;

        /// <summary>
        /// 节点创建时间
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
        /// 节点完成时间
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
        /// 节点创建时间
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
        /// 创建节点的用户对象
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
        /// 最后更新用户
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
        /// 流程图中的活动ID
        /// </summary>		
        public string ActivityID
        {
            get
            {
                return _ActivityID;
            }
            set
            {
                _ActivityID = value;
            }
        }private string _ActivityID = string.Empty;

        /// <summary>
        /// 节点候选人列表
        /// </summary>
        public List<UserInfo> NomineeList
        {
            get { return _NomineeList; }
            set { _NomineeList = value; }
        }private List<UserInfo> _NomineeList = null;

        ///////////////////////////////////////////////////////////////////////
        //                  以下字段在WWF_Node中不存在                       //
        ///////////////////////////////////////////////////////////////////////
    }
}
