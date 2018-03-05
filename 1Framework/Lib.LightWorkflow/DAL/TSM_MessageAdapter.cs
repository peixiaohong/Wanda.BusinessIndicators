using Lib.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Dal;
using Wanda.Lib.LightWorkflow.Entities;

namespace Wanda.Lib.LightWorkflow
{
    internal class TSM_MessageAdapter : LwfBaseAdapterT<TMS_Messages>, ITSM_MessageAdapter
    {
        private static TSM_MessageAdapter _Instance = new TSM_MessageAdapter();


        private TSM_MessageAdapter()
        {
        }

        /// <summary>
        /// 实例
        /// </summary>	
        public static TSM_MessageAdapter Instance
        {
            get
            {
                return _Instance;
            }
        }

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="receiver">接受者，手机号码或RTX号码。。。</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="messageType">消息类型，1:RTX，2:SMS，3:OA。。。</param>
        public int InsertMessage(string receiver, string title, string content, int messageType, DateTime? scheduleTime)
        {
            TMS_Messages tMessage = new TMS_Messages()
            {
               // ID=Guid.NewGuid().ToString(),
                Target = receiver,
                Title = title,
                Content = content,
                MessageType = messageType,
                TargetTime = scheduleTime,
                //CreateTime=DateTime.Now
            };

            return base.Insert(tMessage);
        }

        /// <summary>
        /// 新增RTX消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="content"></param>
        public int InsertRTXMessage(string receiver, string content)
        {
            //为RTX消息添加消息后缀
            string sufixStr = System.Configuration.ConfigurationManager.AppSettings["RTXMessageSufix"] ?? string.Empty;
            content += sufixStr;
            return this.InsertMessage(receiver, string.Empty, content, 1, null);
        }
        /// <summary>
        /// 新增RTX消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="content"></param>
        public void InsertRTXMessage(string[] receiver, string content)
        {
            //为RTX消息添加消息后缀
            string sufixStr = System.Configuration.ConfigurationManager.AppSettings["RTXMessageSufix"] ?? string.Empty;
            content += sufixStr;
            using (TransactionScope trans = TransactionScopeFactory.Create())
            {
                foreach (var item in receiver)
                {
                    this.InsertMessage(item, string.Empty, content, 1, null);
                }
                trans.Complete();
            }
        }
        /// <summary>
        /// 新增SMS消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="content"></param>
        public void InsertSMSMessage(string receiver, string content)
        {
            this.InsertMessage(receiver, string.Empty, content, 2, null);
        }

        /// <summary>
        /// 获取一部分队列消息(最多100条)。
        /// 当前仅处理MessageType为1(RTX)和2(SMS)的数据
        /// </summary>
        /// <returns></returns>
        public PartlyCollection<TMS_Messages> LoadMessageQueue(int count)
        {
            //筛选条件：
            //1.状态：待处理或待重试
            //2.调度时间：无调度时间或调度已到期
            //3.消息类型：RTX或SMS(在增加处理程序后，这个条件也要相应修改；要保证每一条读出的数据都能被处理)
            //4.排序：按已完成的重试次数排序，没有处理过的数据优先处理

            string TMS_MessagesTableName = ORMapping.GetTableName(typeof(Entities.TMS_Messages));
            //WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            //string sqlCommand = string.Format("SELECT top {0} * FROM {1} WHERE (Status in (1,2)) and (TargetTime is null or TargetTime <= '{2}') and (MessageType in (1,2)) ORDER BY TryTimes",
            //           count,TMS_MessagesTableName, DateTime.Now);
            string sqlCommand = string.Format(@"SELECT TOP {0} * FROM {1} t 
                                  INNER JOIN BUserinfo p ON T.[Target]=p.[LoginName] 
                                  WHERE (Status IN (0,3)) AND (TargetTime IS NULL OR TargetTime <= GETDATE()) AND (MessageType IN (1,2)) 
                                  AND  p.[IsForbidden]=0
                                  ORDER BY TryTimes", count, TMS_MessagesTableName);
            return ( PartlyCollection<TMS_Messages> )ExecuteQuery(sqlCommand);
        }
        /// <summary>
        /// 处理完成后更新状态
        /// </summary>
        /// <param name="message"></param>
        public void UploadMessageStatus(Entities.TMS_Messages message)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE TSM_Messages SET SendTime=@Now, Status=@Status, TryTimes=COALESCE(TryTimes,0)+1");
            if (!string.IsNullOrEmpty(message.ErrorInfo))
            {
                sql.Append(", ErrorInfo=@ErrorInfo");
            }
            if (message.Status == (int)MessageStatus.Retry && message.TargetTime > DateTime.MinValue)
            {
                sql.Append(", TargetTime=@TargetTime");
            }
            sql.Append(" WHERE MessageID=@MessageID");

            Database db = DatabaseFactory.Create(DbContext.GetContext(this.ConnectionName));

            DbCommand dbCommand = db.GetSqlStringCommand(sql.ToString());

            db.AddInParameter(dbCommand, "@Now", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dbCommand, "@MessageID", DbType.Guid, message.ID);
            db.AddInParameter(dbCommand, "@Status", DbType.Int32, message.Status);
            if (!string.IsNullOrEmpty(message.ErrorInfo))
            {
                db.AddInParameter(dbCommand, "@ErrorInfo", DbType.String, SqlTextHelper.SafeQuote(message.ErrorInfo));
            }
            if (message.Status == (int)MessageStatus.Retry && message.TargetTime > DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "@TargetTime", DbType.DateTime, message.TargetTime);
            }

            db.ExecuteNonQuery(dbCommand);
        }
    }

    /// <summary>
    /// 消息队列状态
    /// </summary>
    public enum MessageStatus
    {
        [Description("未处理")]
        Pending = 0,

        [Description("成功")]
        Successful = 1,

        [Description("失败")]
        Failed = 2,

        [Description("待重试")]
        Retry = 3
    }
}
