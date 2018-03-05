using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{
    [Serializable]
    [ORTableMapping("dbo.LWF_ProcessNodeInstance")]
    public class ProcessNodeInstance : BaseModel
    {

        #region Public Properties



        [NoMapping]
        public int NodeID { get; set; }



        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("ProcessInstanceID")]
        public int ProcessInstanceID { get; set; }



        [ORFieldMapping("PreviousNodeInstanceID")]
        public int PreviousNodeInstanceID { get; set; }



        [ORFieldMapping("NodeName")]
        public string NodeName { get; set; }



        [ORFieldMapping("NodeSeq")]
        public int NodeSeq { get; set; }



        [ORFieldMapping("Expression")]
        public string Expression { get; set; }



        [ORFieldMapping("NodeType")]
        public int NodeType { get; set; }



        [ORFieldMapping("IsHandSign")]
        public bool IsHandSign { get; set; }



        [ORFieldMapping("RoleID")]
        public int RoleID { get; set; }



        [ORFieldMapping("Description")]
        public string Description { get; set; }



        [ORFieldMapping("CreatedTime")]
        public DateTime CreatedTime { get; set; }



        [ORFieldMapping("LastUpdatedTime")]
        public DateTime LastUpdatedTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("UserID")]
        public int UserID { get; set; }



        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        [NoMapping]
        public string JobTitle { get; set; }

        [ORFieldMapping("OperationType")]
        public int OperationType { get; set; }



        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }



        [ORFieldMapping("ProcessType")]
        public string ProcessType { get; set; }


        #endregion

        // TODO: huwz
        [NoMapping]
        public int cUserID { get; set; }

        // TODO: huwz
        [NoMapping]
        public string UserCode { get; set; }

        // TODO: huwz
        [ORFieldMapping("NodeInstanceID")]
        public int NodeInstanceID { get; set; }
    } 
    ///// <summary>
    ///// 流程子项实例表
    ///// </summary>
    //[Serializable]
    //public partial class ProcessNodeInstance
    //{
    //    public const string SourceTable = "LWF_ProcessNodeInstance";

    //    public ProcessNodeInstance()
    //    {
    //    }

    //    /// <summary>
    //    /// NodeInstanceID
    //    /// </summary>		
    //    [DataProperty(Field = "NodeInstanceID", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where)]
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
    //    /// NodeID
    //    /// </summary>		
    //    [DataProperty(Field = "NID")]
    //    public int NID
    //    {
    //        get
    //        {
    //            return _NID;
    //        }
    //        set
    //        {
    //            _NID = value;
    //        }
    //    }private int _NID;

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
    //    /// 上一节点实例ID
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
    //    /// 节点顺序：0~N
    //    /// </summary>		
    //    [DataProperty(Field = "NodeSeq")]
    //    public int NodeSeq
    //    {
    //        get
    //        {
    //            return _NodeSeq;
    //        }
    //        set
    //        {
    //            _NodeSeq = value;
    //        }
    //    }private int _NodeSeq;

    //    /// <summary>
    //    /// 节点表达式：判断当前节点是否需要参与执行，如果表达式成立就进入，否则直接跳过该节点
    //    /// </summary>		
    //    [DataProperty(Field = "Expression")]
    //    public string Expression
    //    {
    //        get
    //        {
    //            return _Expression;
    //        }
    //        set
    //        {
    //            _Expression = value;
    //        }
    //    }private string _Expression = string.Empty;

    //    /// <summary>
    //    /// 动作类型 ：0撤销；1上报；2审批；4复核；8归档；16抄送；32转发；64委托
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
    //    /// 是否手签：该节点是否是手工签批，0：不是手签；1：手签。只要出现手工签批节点，流程自动跳过该节点。
    //    /// </summary>		
    //    [DataProperty(Field = "IsHandSign")]
    //    public bool IsHandSign
    //    {
    //        get
    //        {
    //            return _IsHandSign;
    //        }
    //        set
    //        {
    //            _IsHandSign = value;
    //        }
    //    }private bool _IsHandSign;

    //    /// <summary>
    //    /// 当前节点的角色
    //    /// </summary>		
    //    [DataProperty(Field = "Role")]
    //    public int Role
    //    {
    //        get
    //        {
    //            return _Role;
    //        }
    //        set
    //        {
    //            _Role = value;
    //        }
    //    }private int _Role;

    //    /// <summary>
    //    /// 节点描述
    //    /// </summary>		
    //    [DataProperty(Field = "Description")]
    //    public string Description
    //    {
    //        get
    //        {
    //            return _Description;
    //        }
    //        set
    //        {
    //            _Description = value;
    //        }
    //    }private string _Description = string.Empty;

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
    //    /// 最后更新时间
    //    /// </summary>		
    //    [DataProperty(Field = "LastUpdatedTime")]
    //    public DateTime LastUpdatedTime
    //    {
    //        get
    //        {
    //            return _LastUpdatedTime;
    //        }
    //        set
    //        {
    //            _LastUpdatedTime = value;
    //        }
    //    }private DateTime _LastUpdatedTime;

    //    /// <summary>
    //    /// 节点状态：1执行中；2已执行；4未执行
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
    //    /// <summary>
    //    /// 流程类型，用于支持一个流程Code配置多个类型的流程（流程组）
    //    /// </summary>		
    //    [DataProperty(Field = "ProcessType")]
    //    public string ProcessType
    //    {
    //        get
    //        {
    //            return _ProcessType;
    //        }
    //        set
    //        {
    //            _ProcessType = value;
    //        }
    //    }private string _ProcessType = string.Empty;
    //}
}
