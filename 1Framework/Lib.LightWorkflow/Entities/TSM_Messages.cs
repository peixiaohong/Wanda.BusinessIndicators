using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{
    /// <summary>
    /// This object represents the properties and methods of a Messages.
    /// </summary>
    [ORTableMapping("dbo.TSM_Messages")]
    public class TMS_Messages : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("Target")]
        public string Target { get; set; }



        [ORFieldMapping("Title")]
        public string Title { get; set; }



        [ORFieldMapping("Content")]
        public string Content { get; set; }



        [ORFieldMapping("Priority")]
        public int Priority { get; set; }



        [ORFieldMapping("MessageType")]
        public int MessageType { get; set; }



        [ORFieldMapping("TargetTime")]
        public DateTime? TargetTime { get; set; }



        [ORFieldMapping("SendTime")]
        public DateTime SendTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("TryTimes")]
        public int TryTimes { get; set; }



        [ORFieldMapping("ErrorInfo")]
        public string ErrorInfo { get; set; }

        #endregion


    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        RTX = 1,
        SMS = 2,
        EMAIL = 3,
    }

    /// <summary>
    /// 发送状态
    /// </summary>
    public enum SendStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        NotSend = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 失败
        /// </summary>
        Failure = 2,

        /// <summary>
        /// 待重试
        /// </summary>
        TryAgain = 3,
    }

    //[Serializable]
    //public sealed class TMS_Messages
    //{
    //    public const string SourceTable = "TSM_Messages";

    //    /// <summary>
    //    ///消息ID
    //    /// </summary>
    //    [DataProperty(Field = "MessageID", IsKey = true, BindingFlag = BindingFlagType.Select | BindingFlagType.Where)]
    //    public Guid MessageID
    //    { get; set; }

    //    /// <summary>
    //    ///接受者
    //    /// </summary>
    //    [DataProperty(Field = "Target")]
    //    public string Target
    //    { get; set; }

    //    /// <summary>
    //    ///消息标题
    //    /// </summary>
    //    [DataProperty(Field = "Title")]
    //    public string Title
    //    { get; set; }

    //    /// <summary>
    //    ///消息内容
    //    /// </summary>
    //    [DataProperty(Field = "Content")]
    //    public string Content
    //    { get; set; }

    //    /// <summary>
    //    ///优先级
    //    /// </summary>
    //    [DataProperty(Field = "Priority")]
    //    public int Priority
    //    { get; set; }

    //    /// <summary>
    //    ///消息类型
    //    ///    1 RTX
    //    ///    2 SMS
    //    ///    3 OA
    //    ///    
    //    /// </summary>
    //    [DataProperty(Field = "MessageType")]
    //    public int MessageType
    //    { get; set; }

    //    /// <summary>
    //    ///建立时间
    //    /// </summary>
    //    [DataProperty(Field = "CreateTime")]
    //    public DateTime CreateTime
    //    { get; set; }

    //    /// <summary>
    //    ///调度时间
    //    /// </summary>
    //    [DataProperty(Field = "TargetTime")]
    //    public DateTime TargetTime
    //    { get; set; }

    //    /// <summary>
    //    ///发送时间
    //    /// </summary>
    //    [DataProperty(Field = "SendTime")]
    //    public DateTime SendTime
    //    { get; set; }

    //    /// <summary>
    //    ///发送状态
    //    ///    0 未处理
    //    ///    1 成功
    //    ///    2 失败
    //    ///    3 待重试
    //    /// </summary>
    //    [DataProperty(Field = "Status")]
    //    public int Status
    //    { get; set; }

    //    /// <summary>
    //    ///发送次数
    //    /// </summary>
    //    [DataProperty(Field = "TryTimes")]
    //    public int TryTimes
    //    { get; set; }

    //    /// <summary>
    //    ///错误信息
    //    /// </summary>
    //    [DataProperty(Field = "ErrorInfo")]
    //    public string ErrorInfo
    //    { get; set; }
    //}
}
