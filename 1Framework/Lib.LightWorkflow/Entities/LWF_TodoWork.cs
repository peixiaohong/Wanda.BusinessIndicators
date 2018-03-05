using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{
    [Serializable]
    [ORTableMapping("dbo.LWF_TodoWork")]
    public class TodoWork : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("ProcessInstanceID")]
        public int ProcessInstanceID { get; set; }



        [ORFieldMapping("ProcessCode")]
        public string ProcessCode { get; set; }



        [ORFieldMapping("InstanceName")]
        public string InstanceName { get; set; }



        [ORFieldMapping("NodeName")]
        public string NodeName { get; set; }



        [ORFieldMapping("PreviousNodeInstanceID")]
        public int PreviousNodeInstanceID { get; set; }

        [ORFieldMapping("NodeInstanceID")]
        public int NodeInstanceID { get; set; }

        [ORFieldMapping("NodeType")]
        public int NodeType { get; set; }



        [ORFieldMapping("CreatedTime")]
        public DateTime CreatedTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("UserID")]
        public int UserID { get; set; }



        [ORFieldMapping("UserName")]
        public string UserName { get; set; }


        [ORFieldMapping("TodoType")]
        public int TodoType { get; set; }



        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }



        [ORFieldMapping("CreateProcessUserID")]
        public int CreateProcessUserID { get; set; }

        [ORFieldMapping("CreateProcessUserName")]
        public string CreateProcessUserName { get; set; }



        [ORFieldMapping("CreateProcessTime")]
        public DateTime CreateProcessTime { get; set; }



        [ORFieldMapping("ProjectID")]
        public int ProjectID { get; set; }


        #endregion

         [NoMapping]
        public string UserLoginName { get; set; }
         [NoMapping]
         public string CreateUserLoginName { get; set; }
        // TODO: huwz
        [NoMapping]
        public int cUserID { get; set; }

        // TODO: huwz
         

        // TODO: huwz
         [NoMapping]
        public string UserCode { get; set; }
    } 
    ///// <summary>
    ///// 待办工作表
    ///// </summary>
    //[Serializable]
    //public partial class TodoWork
    //{
    //    public const string SourceTable = "LWF_TodoWork";

    //    public TodoWork()
    //    {
    //    }

    //    /// <summary>
    //    /// todoWorkID
    //    /// </summary>		
    //    [DataProperty(Field = "todoWorkID", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where)]
    //    public int todoWorkID
    //    {
    //        get
    //        {
    //            return _todoWorkID;
    //        }
    //        set
    //        {
    //            _todoWorkID = value;
    //        }
    //    }private int _todoWorkID;

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
    //    /// 编码，不可更改
    //    /// </summary>		
    //    [DataProperty(Field = "ProcessCode")]
    //    public string ProcessCode
    //    {
    //        get
    //        {
    //            return _ProcessCode;
    //        }
    //        set
    //        {
    //            _ProcessCode = value;
    //        }
    //    }private string _ProcessCode = string.Empty;

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
    //    /// ModifyUserID
    //    /// </summary>		
    //    [DataProperty(Field = "ModifyUserID")]
    //    public int ModifyUserID
    //    {
    //        get
    //        {
    //            return _ModifyUserID;
    //        }
    //        set
    //        {
    //            _ModifyUserID = value;
    //        }
    //    }private int _ModifyUserID;

    //    /// <summary>
    //    /// 修改用户登录名
    //    /// </summary>		
    //    [DataProperty(Field = "ModifycUserID")]
    //    public string ModifycUserID
    //    {
    //        get
    //        {
    //            return _ModifycUserID;
    //        }
    //        set
    //        {
    //            _ModifycUserID = value;
    //        }
    //    }private string _ModifycUserID = string.Empty;

    //    /// <summary>
    //    /// 修改用户编码
    //    /// </summary>		
    //    [DataProperty(Field = "ModifyUserCode")]
    //    public string ModifyUserCode
    //    {
    //        get
    //        {
    //            return _ModifyUserCode;
    //        }
    //        set
    //        {
    //            _ModifyUserCode = value;
    //        }
    //    }private string _ModifyUserCode = string.Empty;

    //    /// <summary>
    //    /// 修改用户姓名
    //    /// </summary>		
    //    [DataProperty(Field = "ModifyUserName")]
    //    public string ModifyUserName
    //    {
    //        get
    //        {
    //            return _ModifyUserName;
    //        }
    //        set
    //        {
    //            _ModifyUserName = value;
    //        }
    //    }private string _ModifyUserName = string.Empty;

    //    /// <summary>
    //    /// 类型：1代办；2通知
    //    /// </summary>		
    //    [DataProperty(Field = "TodoType")]
    //    public int TodoType
    //    {
    //        get
    //        {
    //            return _TodoType;
    //        }
    //        set
    //        {
    //            _TodoType = value;
    //        }
    //    }private int _TodoType;
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
    //    /// 流程创建者UserID
    //    /// </summary>		
    //    [DataProperty(Field = "CreateProcessUserID")]
    //    public int CreateProcessUserID
    //    {
    //        get
    //        {
    //            return _CreateProcessUserID;
    //        }
    //        set
    //        {
    //            _CreateProcessUserID = value;
    //        }
    //    }private int _CreateProcessUserID;

    //    /// <summary>
    //    /// 流程创建者用户登录名
    //    /// </summary>		
    //    [DataProperty(Field = "CreateProcesscUserID")]
    //    public string CreateProcesscUserID
    //    {
    //        get
    //        {
    //            return _CreateProcesscUserID;
    //        }
    //        set
    //        {
    //            _CreateProcesscUserID = value;
    //        }
    //    }private string _CreateProcesscUserID = string.Empty;

    //    /// <summary>
    //    /// 流程创建者用户编码
    //    /// </summary>		
    //    [DataProperty(Field = "CreateProcessUserCode")]
    //    public string CreateProcessUserCode
    //    {
    //        get
    //        {
    //            return _CreateProcessUserCode;
    //        }
    //        set
    //        {
    //            _CreateProcessUserCode = value;
    //        }
    //    }private string _CreateProcessUserCode = string.Empty;

    //    /// <summary>
    //    /// 流程创建者用户姓名
    //    /// </summary>		
    //    [DataProperty(Field = "CreateProcessUserName")]
    //    public string CreateProcessUserName
    //    {
    //        get
    //        {
    //            return _CreateProcessUserName;
    //        }
    //        set
    //        {
    //            _CreateProcessUserName = value;
    //        }
    //    }private string _CreateProcessUserName = string.Empty;

    //    /// <summary>
    //    /// 最后修改时间
    //    /// </summary>		
    //    [DataProperty(Field = "ModifyTime")]
    //    public DateTime ModifyTime
    //    {
    //        get
    //        {
    //            return _ModifyTime;
    //        }
    //        set
    //        {
    //            _ModifyTime = value;
    //        }
    //    }private DateTime _ModifyTime;

    //    /// <summary>
    //    /// 流程创建时间
    //    /// </summary>		
    //    [DataProperty(Field = "CreateProcessTime")]
    //    public DateTime CreateProcessTime
    //    {
    //        get
    //        {
    //            return _CreateProcessTime;
    //        }
    //        set
    //        {
    //            _CreateProcessTime = value;
    //        }
    //    }private DateTime _CreateProcessTime;

    //    /// <summary>
    //    /// 项目ID
    //    /// </summary>
    //    [DataProperty(Field = "ProjectID")]
    //    public int ProjectID
    //    {
    //        get { return _ProjectID; }
    //        set { _ProjectID = value; }
    //    }private int _ProjectID;
    //}
}
