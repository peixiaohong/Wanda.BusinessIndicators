using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPF.OAMQMessages.Entities;

namespace BPF.OAMQMessages.DAL
{
    public class OAMQMessagesOracle : DALBase, IOAMQMessage
    {
        private static OAMQMessagesOracle _instance = null;
        public static OAMQMessagesOracle Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OAMQMessagesOracle();
                }
                return _instance;
            }
        }
        public List<Entities.OAMQMessage> LoadList(int count)
        {
            List<OAMQMessage> list = new List<OAMQMessage>();
            string senderCodeSQL = BPF.OAMQServices.OAMQBll.Instance.SendMode == 2 ? ",SenderCode" : "";
            string oracleQuery = @"SELECT MessageId,
       Sender,
       Sendertime,
       Flowtype,
       FlowID,
       Title,
       Nodename,
       PtpUrl,
       Userid,
       Creator,
       Createtime,
       Operatetime,
       Flowmess,
       Viewtype,
       Status,
       ErrorCount,
       MessageCreateTime,
       AllowMobile"
       + senderCodeSQL
       + @" FROM (SELECT MessageId,
               Sender,
               Sendertime,
               Flowtype,
               FlowID,
               Title,
               Nodename,
               PtpUrl,
               Userid,
               Creator,
               Createtime,
               Operatetime,
               Flowmess,
               Viewtype,
               Status,
               ErrorCount,
               MessageCreateTime,
               AllowMobile"
               + senderCodeSQL
               + @" FROM OAMQMessages
         WHERE Status = 0
         ORDER BY Sendertime ASC)
 WHERE ROWNUM <= " + count + @"
 ORDER BY ROWNUM ASC ;";
            DbCommand cmd = this.DbConnection.CreateCommand();
            cmd.CommandText = oracleQuery;
            DbDataAdapter dbDataAdapter = this.DbFactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;

            DataSet dataSet = new DataSet();

            if (dbDataAdapter.Fill(dataSet) > 0
                && dataSet.Tables != null
                && dataSet.Tables.Count > 0
                && dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    OAMQMessage message = null;
                    System.Data.DataRow row = dataSet.Tables[0].Rows[i];
                    foreach (object o in row.ItemArray)
                    {
                        message = new OAMQMessage()
                        {
                            MessageId = DataTypeHelper.GetString(row["MessageId"]),
                            Sender = DataTypeHelper.GetString(row["Sender"]),
                            Sendertime = DataTypeHelper.GetDateTime(row["Sendertime"]),
                            Flowtype = DataTypeHelper.GetString(row["Flowtype"]),
                            FlowID = DataTypeHelper.GetString(row["FlowID"]),
                            Title = DataTypeHelper.GetString(row["Title"]),
                            Nodename = DataTypeHelper.GetString(row["Nodename"]),
                            PtpUrl = DataTypeHelper.GetString(row["PtpUrl"]),
                            Userid = DataTypeHelper.GetString(row["Userid"]),
                            Creator = DataTypeHelper.GetString(row["Creator"]),
                            Createtime = DataTypeHelper.GetDateTime(row["Createtime"]),
                            Operatetime = DataTypeHelper.GetDateTime(row["Operatetime"]),
                            Flowmess = DataTypeHelper.GetInt(row["Flowmess"]),
                            Viewtype = DataTypeHelper.GetInt(row["Viewtype"]),
                            Status = DataTypeHelper.GetInt(row["Status"]),
                            ErrorCount = DataTypeHelper.GetInt(row["ErrorCount"]),
                            MessageCreateTime = DataTypeHelper.GetDateTime(row["MessageCreateTime"]),
                            AllowMobile = DataTypeHelper.GetInt(row["AllowMobile"])
                        };
                        if (BPF.OAMQServices.OAMQBll.Instance.SendMode == 2)
                            message.SenderCode = DataTypeHelper.GetString(row["SenderCode"]);
                    }
                    list.Add(message);
                }
            }
            return list;
        }

        public int Update(Entities.OAMQMessage message)
        {
            //string sql = @"update OAMQMessages set Status = :Status, ErrorCount = :ErrorCount WHERE MessageId = :MessageId ";
            //System.Data.Common.DbCommand cmd = this.DataSource.GetSqlStringCommand(sql);
            //string prefix = ":";

            //this.AddParameter(this.DataSource, cmd, System.Data.DbType.Int32, prefix + "Status", message.Status);
            //this.AddParameter(this.DataSource, cmd, System.Data.DbType.Int32, prefix + "ErrorCount", message.ErrorCount);
            //this.AddParameter(this.DataSource, cmd, System.Data.DbType.String, prefix + "MessageId", message.MessageId.ToString());

            string sql = @"update OAMQMessages set  
      Sendertime = :Sendertime
      ,Status = :Status
      ,ErrorCount = :ErrorCount
 WHERE MessageId = :MessageId ;";
            System.Data.Common.DbCommand cmd = this.DbConnection.CreateCommand();
            cmd.CommandText = sql;
            string prefix = string.Empty;
            this.AddParameter(cmd, System.Data.DbType.String, prefix + "Sendertime", message.Sendertime.ToString("yyyy-MM-dd HH:mm:ss"));
            this.AddParameter(cmd, System.Data.DbType.Int32, prefix + "Status", message.Status);
            this.AddParameter(cmd, System.Data.DbType.Int32, prefix + "ErrorCount", message.ErrorCount);
            this.AddParameter(cmd, System.Data.DbType.String, prefix + "MessageId", message.MessageId.ToString());

            return cmd.ExecuteNonQuery();
        }
    }
}
