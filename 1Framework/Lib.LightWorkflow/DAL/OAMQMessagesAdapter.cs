using System.Text;
using System.Collections.Generic;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;


namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class OAMQMessagesAdapter : LwfBaseAdapterT<OAMQMessages>, IOAMQMessagesAdapter
    {
        private static OAMQMessagesAdapter _instance = new OAMQMessagesAdapter();
        public static OAMQMessagesAdapter Instance { get { return _instance; } }

        public void Delete(int messageid)
        {
            base.Delete(new OAMQMessages { ID = messageid });
        }


        public OAMQMessages Load(int messageid)
        {
            return base.GetModelByID(messageid);
        }

        public List<OAMQMessages> LoadList(int count)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("Status", 0);

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT TOP {0} * FROM {1} WHERE {2}  ORDER BY Sendertime ASC",
                count,
                ORMapping.GetTableName(typeof(OAMQMessages)),
                where.ToSqlString(TSqlBuilder.Instance));
            return ExecuteQuery(sql.ToString());

        }
        ///<summary>
        /// 修改操作
        /// </summary>
        /// <param name="oAMQMessages"></param>
        public void Update(OAMQMessages oAMQMessages)
        {
            base.Update(oAMQMessages);
        }

        internal OAMQMessages LoadByProcessInstanceID(int processInstanceID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            //where.AppendItem("Status", 0);
            //where.AppendItem("Flowmess", 1);
            where.AppendItem("Viewtype", -2);
            where.AppendItem("FlowID", processInstanceID);
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT * FROM {0} WHERE {1}  ORDER BY Sendertime ASC",
                ORMapping.GetTableName(typeof(OAMQMessages)),
                where.ToSqlString(TSqlBuilder.Instance));
            List<OAMQMessages> result = ExecuteQuery(sql.ToString());
            return result.Count > 0 ? result[0] : null;
        }
    }
}

