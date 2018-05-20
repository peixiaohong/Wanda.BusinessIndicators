using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.DAL
{
    sealed class B_AttachmentAdapter : AppBaseAdapterT<B_Attachment>
    {

        public IList<B_Attachment> GetAttachmentList()
        {
            string sql = ORMapping.GetSelectSql<B_Attachment>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public IList<B_Attachment> GetAttachmentList(Guid businessID)
        {
            string sql = ORMapping.GetSelectSql<B_Attachment>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND businessID=@BusinessID ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pBusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, businessID);
            return ExecuteQuery(sql,pBusinessID);
        }
        public IList<B_Attachment> GetAttachmentList(string businessIDs, string businessType)
        {
            string sql = ORMapping.GetSelectSql<B_Attachment>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND businessID in (@BusinessID)  and BusinessType =@BusinessType";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pBusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.String, businessIDs);
            SqlParameter pBusinessType = CreateSqlParameter("@BusinessType", System.Data.DbType.String, businessType);
            return ExecuteQuery(sql, pBusinessID, pBusinessType);
        }

        public IList<B_Attachment> GetAttachmentList(Guid businessID , string businessType )
        {
            string sql = ORMapping.GetSelectSql<B_Attachment>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND businessID=@BusinessID  and BusinessType =@BusinessType";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pBusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, businessID);
            SqlParameter pBusinessType = CreateSqlParameter("@BusinessType", System.Data.DbType.String, businessType);

            return ExecuteQuery(sql, pBusinessID, pBusinessType);
        }




    }

}
