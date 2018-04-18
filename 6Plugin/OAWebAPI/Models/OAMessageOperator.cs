
using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Helper;

namespace Plugin.OAMessage
{
    /// <summary>
    /// TemplateAttachment对象的业务逻辑操作
    /// </summary>
    public class OAMessageOperator
    {
        private static string _select = "select * from ";
        
        public static List<OAMessageEntity> GetList(int flowType, string user,RequestType requestType)
        {
            return null;
        }
        public static OAMessageEntity LoadOAMessage(string flowId, string nodename, string receiver,string syscode,RequestType requestType)
        {
            string where = string.Format(" REQUESTID='{0}' AND JSRLOGINID='{1}' AND CURRENTNODENAME='{2}'", flowId, receiver, nodename);
            string sql = _select + getTableName(requestType) + " WHERE " + where;
            var data= OracleHelper.QueryData(sql);
            return data.ToEntity<OAMessageEntity>();
        }
        private static string getTableName(RequestType requestType)
        {
            switch (requestType)
            {
                case RequestType.Done:
                    return "WORKFLOW_DAIBAN";
                //case RequestType.ToDo:
                case RequestType.Over:
                    return "WORKFLOW_YIBAN";
                //return "WORKFLOW_BANJIE";
                case RequestType.DirectOver:
                    return "WORKFLOW_DAIBAN";
            }
            return "";
        }

    }
}

