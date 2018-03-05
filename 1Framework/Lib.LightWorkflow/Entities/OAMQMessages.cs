using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{
    [ORTableMapping("dbo.OAMQMessages")]
    public class OAMQMessages : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("Sender")]
        public string Sender { get; set; }



        [ORFieldMapping("Sendertime")]
        public DateTime Sendertime { get; set; }



        [ORFieldMapping("Flowtype")]
        public string Flowtype { get; set; }



        [ORFieldMapping("FlowID")]
        public string FlowID { get; set; }



        [ORFieldMapping("Title")]
        public string Title { get; set; }



        [ORFieldMapping("Nodename")]
        public string Nodename { get; set; }


        /// <summary>
        /// 业务系统生成的到业务系统相关流程的URL链接，如：http://rz.wanda.cn/openform.aspx?bpid=n
        /// </summary>
        [ORFieldMapping("PtpUrl")]
        public string PtpUrl { get; set; }



        [ORFieldMapping("Userid")]
        public string Userid { get; set; }


        [ORFieldMapping("Operatetime")]
        public DateTime Operatetime { get; set; }


        /// <summary>
        /// 1,待办事宜;2,已办事宜;9,作废(本系统不支持)
        /// </summary>
        [ORFieldMapping("Flowmess")]
        public int Flowmess { get; set; }



        [ORFieldMapping("Viewtype")]
        public int Viewtype { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("ErrorCount")]
        public int ErrorCount { get; set; }



        [ORFieldMapping("MessageCreateTime")]
        public DateTime MessageCreateTime { get; set; }

        [NoMapping]
        public int TodoType { get; set; }
        [NoMapping]
        public int TodoID { get; set; }
        [NoMapping]
        public int BizID { get; set; }
        #endregion


    } 
    ///// <summary>
    ///// 
    ///// </summary>
    //[Serializable]
    //public partial class OAMQMessages
    //{
    //    public const string SourceTable = "OAMQMessages";

    //    public OAMQMessages()
    //    {
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>		
    //    [DataProperty(Field = "MessageId", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where | BindingFlagType.Insert)]
    //    public Guid MessageId
    //    {
    //        get
    //        {
    //            return _MessageId;
    //        }
    //        set
    //        {
    //            _MessageId = value;
    //        }
    //    }private Guid _MessageId = Guid.Empty;

    //    /// <summary>
    //    /// 消息发送者，发现消息的系统名称，固定为“XX业务系统”
    //    /// </summary>		
    //    [DataProperty(Field = "Sender")]
    //    public string Sender
    //    {
    //        get
    //        {
    //            return _Sender;
    //        }
    //        set
    //        {
    //            _Sender = value;
    //        }
    //    }private string _Sender = string.Empty;

    //    /// <summary>
    //    /// 发送的时间，格式：yyyy-MM-dd hh:mm:ss
    //    /// </summary>		
    //    [DataProperty(Field = "Sendertime")]
    //    public DateTime Sendertime
    //    {
    //        get
    //        {
    //            return _Sendertime;
    //        }
    //        set
    //        {
    //            _Sendertime = value;
    //        }
    //    }private DateTime _Sendertime;

    //    /// <summary>
    //    /// 流程分类，就是工作流里面定义的流程名称
    //    /// </summary>		
    //    [DataProperty(Field = "Flowtype")]
    //    public string Flowtype
    //    {
    //        get
    //        {
    //            return _Flowtype;
    //        }
    //        set
    //        {
    //            _Flowtype = value;
    //        }
    //    }private string _Flowtype = string.Empty;

    //    /// <summary>
    //    /// 流程ID，系统内的流程标识，待办为BizProcessID_todoWorkID，已办为BizProcessID
    //    /// </summary>		
    //    [DataProperty(Field = "FlowID")]
    //    public string FlowID
    //    {
    //        get
    //        {
    //            return _FlowID;
    //        }
    //        set
    //        {
    //            _FlowID = value;
    //        }
    //    }private string _FlowID;

    //    /// <summary>
    //    /// 流程名称
    //    /// </summary>		
    //    [DataProperty(Field = "Title")]
    //    public string Title
    //    {
    //        get
    //        {
    //            return _Title;
    //        }
    //        set
    //        {
    //            _Title = value;
    //        }
    //    }private string _Title = string.Empty;

    //    /// <summary>
    //    /// 审批节点名称，如：项目财务副总审批
    //    /// </summary>		
    //    [DataProperty(Field = "Nodename")]
    //    public string Nodename
    //    {
    //        get
    //        {
    //            return _Nodename;
    //        }
    //        set
    //        {
    //            _Nodename = value;
    //        }
    //    }private string _Nodename = string.Empty;

    //    /// <summary>
    //    /// 业务系统生成的到业务系统相关流程的URL链接，如：http://rz.wanda.cn/openform.aspx?bpid=n
    //    /// </summary>		
    //    [DataProperty(Field = "PtpUrl")]
    //    public string PtpUrl
    //    {
    //        get
    //        {
    //            return _PtpUrl;
    //        }
    //        set
    //        {
    //            _PtpUrl = value;
    //        }
    //    }private string _PtpUrl = string.Empty;

    //    /// <summary>
    //    /// 当前处理人用户Id
    //    /// </summary>		
    //    [DataProperty(Field = "Userid")]
    //    public string Userid
    //    {
    //        get
    //        {
    //            return _Userid;
    //        }
    //        set
    //        {
    //            _Userid = value;
    //        }
    //    }private string _Userid = string.Empty;

    //    /// <summary>
    //    /// 流程创建人id
    //    /// </summary>		
    //    [DataProperty(Field = "Creator")]
    //    public string Creator
    //    {
    //        get
    //        {
    //            return _Creator;
    //        }
    //        set
    //        {
    //            _Creator = value;
    //        }
    //    }private string _Creator = string.Empty;

    //    /// <summary>
    //    /// 流程创建的时间
    //    /// </summary>		
    //    [DataProperty(Field = "Createtime")]
    //    public DateTime Createtime
    //    {
    //        get
    //        {
    //            return _Createtime;
    //        }
    //        set
    //        {
    //            _Createtime = value;
    //        }
    //    }private DateTime _Createtime;

    //    /// <summary>
    //    /// 创建待办时：待办创建的时间；删除待办时：待办办理的时间
    //    /// </summary>		
    //    [DataProperty(Field = "Operatetime")]
    //    public DateTime Operatetime
    //    {
    //        get
    //        {
    //            return _Operatetime;
    //        }
    //        set
    //        {
    //            _Operatetime = value;
    //        }
    //    }private DateTime _Operatetime;

    //    /// <summary>
    //    /// 1,待办事宜;2,已办事宜;9,作废(本系统不支持)
    //    /// </summary>		
    //    [DataProperty(Field = "Flowmess")]
    //    public int Flowmess
    //    {
    //        get
    //        {
    //            return _Flowmess;
    //        }
    //        set
    //        {
    //            _Flowmess = value;
    //        }
    //    }private int _Flowmess;

    //    /// <summary>
    //    /// 0：未读；-2：已读
    //    /// </summary>		
    //    [DataProperty(Field = "Viewtype")]
    //    public int Viewtype
    //    {
    //        get
    //        {
    //            return _Viewtype;
    //        }
    //        set
    //        {
    //            _Viewtype = value;
    //        }
    //    }private int _Viewtype;

    //    /// <summary>
    //    /// 0，待发送；1，已发送；2，发送失败
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
    //    /// 发送失败的次数，默认为0，每次失败后+1
    //    /// </summary>		
    //    [DataProperty(Field = "ErrorCount")]
    //    public int ErrorCount
    //    {
    //        get
    //        {
    //            return _ErrorCount;
    //        }
    //        set
    //        {
    //            _ErrorCount = value;
    //        }
    //    }private int _ErrorCount;

    //    /// <summary>
    //    /// 消息创建时间
    //    /// </summary>
    //    [DataProperty(Field = "MessageCreateTime")]
    //    public DateTime MessageCreateTime
    //    {
    //        get
    //        {
    //            return _MessageCreateTime;
    //        }
    //        set
    //        {
    //            _MessageCreateTime = value;
    //        }
    //    }private DateTime _MessageCreateTime;
    //}
}
