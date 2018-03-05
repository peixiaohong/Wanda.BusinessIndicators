using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{
    [Serializable]
    [ORTableMapping("dbo.LWF_StakeHolder")]
    public class StakeHolder : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("ProcessInstanceID")]
        public int ProcessInstanceID { get; set; }



        [ORFieldMapping("UserID")]
        public int UserID { get; set; }



        [ORFieldMapping("UserName")]
        public string UserName { get; set; }



        [ORFieldMapping("NodeType")]
        public int NodeType { get; set; }



        [ORFieldMapping("CreatedTime")]
        public DateTime CreatedTime { get; set; }



        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }


        #endregion


        // TODO: huwz
        [NoMapping]
        public int cUserID { get; set; }

        // TODO: huwz
        [NoMapping]
        public string UserCode { get; set; }
    } 

    ///// <summary>
    ///// 流程干系人表
    ///// </summary>
    //[Serializable]
    //public partial class StakeHolder
    //{
    //    public const string SourceTable = "LWF_StakeHolder";

    //    public StakeHolder()
    //    {
    //    }

    //    /// <summary>
    //    /// SHID
    //    /// </summary>		
    //    [DataProperty(Field = "SHID", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where)]
    //    public int SHID
    //    {
    //        get
    //        {
    //            return _SHID;
    //        }
    //        set
    //        {
    //            _SHID = value;
    //        }
    //    }private int _SHID;

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
