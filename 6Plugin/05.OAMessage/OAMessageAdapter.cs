using Lib.Data;
using LJTH.BusinessIndicators.DAL;
using System;
using System.Collections.Generic;

namespace Plugin.OAMessage
{
    /// <summary>
    /// AutoProcess对象的数据访问适配器
    /// </summary>
    public class OAMessageAdapter : AppBaseAdapterT<OAMessageEntity>
    {
        public PartlyCollection<OAMessageEntity> GetList(OAMessageEntityFilter filter)
        {
            var where = filter.ConvertToWhereBuilder();
            var tuple = GetPageSplitedCollection(filter.RowIndex, filter.PageSize, where.ToSqlString(TSqlBuilder.Instance));
            return tuple;
        }

        public OAMessageEntity LoadOAMessage(string flowId, string nodename, string receiver)
        {
            string sql = "SELECT TOP 1 * FROM dbo.OAMessage    WHERE FlowID=@FlowID AND NodeName=@NodeName AND ReceiverFlowUser=@ReceiverFlowUser ORDER BY CreateFlowTime DESC ";
            var result = ExecuteQuery(sql
                , CreateSqlParameter("@FlowID", System.Data.DbType.String, flowId)
                , CreateSqlParameter("@NodeName", System.Data.DbType.String, nodename)
                , CreateSqlParameter("@ReceiverFlowUser", System.Data.DbType.String, receiver)

                );
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        internal List<OAMessageEntity> GetList(int flowType, string user)
        {
            string sql = "SELECT * FROM dbo.OAMessage  WHERE FlowType=@FlowType AND ReceiverFlowUser=@UserName AND NOT EXISTS(SELECT 1 FROM dbo.OAMessage O WHERE O.FlowID=dbo.OAMessage.FlowID AND O.NodeName=dbo.OAMessage.NodeName AND O.ReceiverFlowUser=dbo.OAMessage.ReceiverFlowUser AND O.CreatorTime>dbo.OAMessage.CreatorTime)";
            var result = ExecuteQuery(sql
                , CreateSqlParameter("@FlowType", System.Data.DbType.Int32, flowType)
                , CreateSqlParameter("@UserName", System.Data.DbType.String, user)
                );

            return result;
        }
    }
}

