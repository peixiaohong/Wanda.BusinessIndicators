using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{

    [Serializable]
    [ORTableMapping("dbo.LWF_ApprovalLog")]
    public class ApprovalLog : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("ProcessInstanceID")]
        public int ProcessInstanceID { get; set; }



        [ORFieldMapping("InstanceName")]
        public string InstanceName { get; set; }



        [ORFieldMapping("NodeName")]
        public string NodeName { get; set; }



        [ORFieldMapping("PreviousNodeInstanceID")]
        public int PreviousNodeInstanceID { get; set; }



        [ORFieldMapping("NodeType")]
        public int NodeType { get; set; }



        [ORFieldMapping("CreatedTime")]
        public DateTime CreatedTime { get; set; }



        [ORFieldMapping("CompletedTime")]
        public DateTime CompletedTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("UserID")]
        public int UserID { get; set; }



        [ORFieldMapping("UserName")]
        public string UserName { get; set; }



        [ORFieldMapping("ApprovalNote")]
        public string ApprovalNote { get; set; }



        [ORFieldMapping("OperationType")]
        public int OperationType { get; set; }

        [NoMapping]
        public string Operation { get; set; }

        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }


        #endregion


        //TODO:huwz
        [NoMapping]
        public int NodeInstanceID { get; set; }
    } 
    ///// <summary>
    ///// 审批日志表
    ///// </summary>
    //[Serializable]
    //public partial class ApprovalLog
    //{
    //    public const string SourceTable = "LWF_ApprovalLog";

    //    public ApprovalLog()
    //    {
    //    }

    //    /// <summary>
    //    /// APID
    //    /// </summary>		
    //    [DataProperty(Field = "APID", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where)]
    //    public int APID
    //    {
    //        get
    //        {
    //            return _APID;
    //        }
    //        set
    //        {
    //            _APID = value;
    //        }
    //    }private int _APID;

    //    /// <summary>
    //    /// 流程定义的ID
    //    /// </summary>		
    //    [DataProperty(Field = "ProcessID")]
    //    public int ProcessID
    //    {
    //        get
    //        {
    //            return _PID;
    //        }
    //        set
    //        {
    //            _PID = value;
    //        }
    //    }private int _PID;

    //    /// <summary>
    //    /// ProcessInstanceID
    //    /// </summary>		
    //    [DataProperty(Field = "ProcessInstanceID")]
    //    public int ProcessInstanceID
    //    {
    //        get
    //        {
    //            return _PIID;
    //        }
    //        set
    //        {
    //            _PIID = value;
    //        }
    //    }private int _PIID;

    //    /// <summary>
    //    /// 流程实例名称
    //    /// </summary>		
    //    [DataProperty(Field = "InstanceName")]
    //    public string InstanceName
    //    {
    //        get
    //        {
    //            return _InstanceName;
    //        }
    //        set
    //        {
    //            _InstanceName = value;
    //        }
    //    }private string _InstanceName = string.Empty;

    //    /// <summary>
    //    /// NodeInstanceID
    //    /// </summary>		
    //    [DataProperty(Field = "NodeInstanceID")]
    //    public int NodeInstanceID
    //    {
    //        get
    //        {
    //            return _NIID;
    //        }
    //        set
    //        {
    //            _NIID = value;
    //        }
    //    }private int _NIID;

    //    /// <summary>
    //    /// 节点名称
    //    /// </summary>		
    //    [DataProperty(Field = "NodeName")]
    //    public string NodeName
    //    {
    //        get
    //        {
    //            return _NodeName;
    //        }
    //        set
    //        {
    //            _NodeName = value;
    //        }
    //    }private string _NodeName = string.Empty;

    //    /// <summary>
    //    /// 上一阶段实例ID
    //    /// </summary>		
    //    [DataProperty(Field = "PreviousNodeInstanceID")]
    //    public int PreviousNodeInstanceID
    //    {
    //        get
    //        {
    //            return _PreviousNIID;
    //        }
    //        set
    //        {
    //            _PreviousNIID = value;
    //        }
    //    }private int _PreviousNIID;

    //    /// <summary>
    //    /// 动作类型 ：1上报；2审批；4复核；8归档；16抄送；32转发；64委托
    //    /// </summary>		
    //    [DataProperty(Field = "NodeType")]
    //    public int NodeType
    //    {
    //        get
    //        {
    //            return _NodeType;
    //        }
    //        set
    //        {
    //            _NodeType = value;
    //        }
    //    }private int _NodeType;

    //    /// <summary>
    //    /// 创建时间
    //    /// </summary>		
    //    [DataProperty(Field = "CreatedTime")]
    //    public DateTime CreatedTime
    //    {
    //        get
    //        {
    //            return _CreatedTime;
    //        }
    //        set
    //        {
    //            _CreatedTime = value;
    //        }
    //    }private DateTime _CreatedTime;

    //    /// <summary>
    //    /// 完成时间
    //    /// </summary>		
    //    [DataProperty(Field = "CompletedTime")]
    //    public DateTime CompletedTime
    //    {
    //        get
    //        {
    //            return _CompletedTime;
    //        }
    //        set
    //        {
    //            _CompletedTime = value;
    //        }
    //    }private DateTime _CompletedTime;

    //    /// <summary>
    //    /// 状态：0，刚接收；1，已读
    //    /// </summary>		
    //    [DataProperty(Field = "Status")]
    //    public int Status
    //    {
    //        get
    //        {
    //            return _Status;
    //        }
    //        set
    //        {
    //            _Status = value;
    //        }
    //    }private int _Status;

    //    /// <summary>
    //    /// UserID
    //    /// </summary>		
    //    [DataProperty(Field = "UserID")]
    //    public int UserID
    //    {
    //        get
    //        {
    //            return _UserID;
    //        }
    //        set
    //        {
    //            _UserID = value;
    //        }
    //    }private int _UserID;

    //    /// <summary>
    //    /// 用户登录名
    //    /// </summary>		
    //    [DataProperty(Field = "cUserID")]
    //    public string cUserID
    //    {
    //        get
    //        {
    //            return _cUserID;
    //        }
    //        set
    //        {
    //            _cUserID = value;
    //        }
    //    }private string _cUserID = string.Empty;

    //    /// <summary>
    //    /// 用户编码
    //    /// </summary>		
    //    [DataProperty(Field = "UserCode")]
    //    public string UserCode
    //    {
    //        get
    //        {
    //            return _UserCode;
    //        }
    //        set
    //        {
    //            _UserCode = value;
    //        }
    //    }private string _UserCode = string.Empty;

    //    /// <summary>
    //    /// 用户姓名
    //    /// </summary>		
    //    [DataProperty(Field = "UserName")]
    //    public string UserName
    //    {
    //        get
    //        {
    //            return _UserName;
    //        }
    //        set
    //        {
    //            _UserName = value;
    //        }
    //    }private string _UserName = string.Empty;

    //    /// <summary>
    //    /// 审批意见
    //    /// </summary>		
    //    [DataProperty(Field = "ApprovalNote")]
    //    public string ApprovalNote
    //    {
    //        get
    //        {
    //            return _ApprovalNote;
    //        }
    //        set
    //        {
    //            _ApprovalNote = value;
    //        }
    //    }private string _ApprovalNote = string.Empty;

    //    /// <summary>
    //    /// 操作类型：1，提交；2， 批准；3，退回；4，撤销；5，归档；6、批准；7、委托；8、转发
    //    /// </summary>		
    //    [DataProperty(Field = "OperationType")]
    //    public int OperationType
    //    {
    //        get
    //        {
    //            return _OperationType;
    //        }
    //        set
    //        {
    //            _OperationType = value;
    //        }
    //    }private int _OperationType;
    //    /// <summary>
    //    /// 业务流程ID
    //    /// </summary>		
    //    [DataProperty(Field = "BizProcessID")]
    //    public int BizProcessID
    //    {
    //        get
    //        {
    //            return _BizProcessID;
    //        }
    //        set
    //        {
    //            _BizProcessID = value;
    //        }
    //    }private int _BizProcessID;
    //}
}
