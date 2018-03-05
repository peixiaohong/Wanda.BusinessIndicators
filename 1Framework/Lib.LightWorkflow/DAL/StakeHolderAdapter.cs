using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Lib.Data;


namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class StakeHolderAdapter : LwfBaseAdapterT<StakeHolder>, IStakeHolderAdapter
    {

        private static StakeHolderAdapter _instance = new StakeHolderAdapter();
        public static StakeHolderAdapter Instance { get { return _instance; } }

        public void Delete(int shid)
        {
            base.Delete(new StakeHolder { ID = shid });
        }

        public StakeHolder Load(int piid, int userID)
        {
            string tableName = ORMapping.GetTableName(typeof(StakeHolder));
            WhereSqlClauseBuilder wc = new WhereSqlClauseBuilder();


            wc.AppendItem("ProcessInstanceID", piid);
            wc.AppendItem("UserID", userID);

            string sqlCommand = string.Format("SELECT TOP 1 * FROM {0} WHERE {1} AND {2} ",
                    tableName,
                    wc.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            List<StakeHolder> result = ExecuteQuery(sqlCommand);
            return result.Count > 0 ? result[0] : null;
        }

        public StakeHolder Load(int shid)
        {
            return base.GetModelByID(shid);
        }

        public List<StakeHolder> LoadList(int processInstanceId)
        {
            string tableName = ORMapping.GetTableName(typeof(StakeHolder));
            WhereSqlClauseBuilder wc = new WhereSqlClauseBuilder();

            wc.AppendItem("ProcessInstanceID", processInstanceId);

            string sqlCommand = string.Format("SELECT * FROM {0} WHERE {1} AND {2} ORDER BY CreatedTime",
                    tableName, wc.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            return ExecuteQuery(sqlCommand);
        }


        internal int DeleteStakeholder(int processInstanceID)
        {
            string tableName = ORMapping.GetTableName(typeof(StakeHolder));
            string sqlCommand = string.Format("UPDATE {0} SET ISDELETED=1 WHERE ProcessInstanceID={1} AND {2}",
                    tableName, processInstanceID, NotDeleted);
            return ExecuteSql(sqlCommand);
        }
    }
}
