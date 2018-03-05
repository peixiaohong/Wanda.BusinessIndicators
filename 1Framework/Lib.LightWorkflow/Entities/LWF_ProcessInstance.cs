using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{

    /// <summary>
    /// This object represents the properties and methods of a ProcessInstance.
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.LWF_ProcessInstance")]
    public partial class ProcessInstance : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("ProcessID")]
        public int  ProcessID { get; set; }



        [ORFieldMapping("ProcessCode")]
        public string ProcessCode { get; set; }



        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }



        [ORFieldMapping("InstanceName")]
        public string InstanceName { get; set; }



        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }



        [ORFieldMapping("FinishTime")]
        public DateTime FinishTime { get; set; }



        [ORFieldMapping("LastUpdatedTime")]
        public DateTime LastUpdatedTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("UserID")]
        public int UserID { get; set; }



        [ORFieldMapping("UserName")]
        public string UserName { get; set; }



        [ORFieldMapping("BizProcessContext")]
        public string BizProcessContext { get; set; }


        #endregion


        // TODO: huwz 
        [NoMapping]
        public int cUserID { get; set; }
        // TODO: huwz 
        [NoMapping]
        public int ProjectID { get; set; }

        // TODO: huwz 
        [NoMapping]
        public string UserCode { get; set; }
    } 

    ///// <summary>
    ///// 流程实例表
    ///// </summary>
    //[Serializable]
    //public partial class ProcessInstance
    //{
    //    public const string SourceTable = "LWF_ProcessInstance";

    //    public ProcessInstance()
    //    {
    //    }

    //    /// <summary>
    //    /// ProcessInstanceID
    //    /// </summary>		
    //    [DataProperty(Field = "ProcessInstanceID", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where)]
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
    //    /// ProcessID
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
    //    /// 结束时间
    //    /// </summary>		
    //    [DataProperty(Field = "FinishedTime")]
    //    public DateTime FinishedTime
    //    {
    //        get
    //        {
    //            return _FinishedTime;
    //        }
    //        set
    //        {
    //            _FinishedTime = value;
    //        }
    //    }private DateTime _FinishedTime;

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
    //    /// 流程状态：2运行中；4审批完成，归档中；8被退回；流程结束；16流程已取消；32已归档
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
    //    /// 项目ID
    //    /// </summary>
    //    [DataProperty(Field = "ProjectID")]
    //    public int ProjectID
    //    {
    //        get { return _ProjectID; }
    //        set { _ProjectID = value; }
    //    }private int _ProjectID;

    //    /// <summary>
    //    /// 业务流程传过来的用于逻辑判断和后期使用的字段内容
    //    /// </summary>
    //    [DataProperty(Field="BizProcessContext")]
    //    public string BizProcessContext
    //    {
    //        get { return _BizProcessContext; }
    //        set { _BizProcessContext = value; }
    //    }private string _BizProcessContext;
    //}
}
