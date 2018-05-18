using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.DAL
{
    sealed class B_DocumentAttachmentsAdapter : AppBaseAdapterT<B_DocumentAttachments>
    {
        public IList<B_DocumentAttachments> GetDocumentAttachmentsList()
        {
            string sql = ORMapping.GetSelectSql<B_DocumentAttachments>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
      
   
        /// <summary>
        /// 通过文件名搜索文件
        /// </summary>
        /// <param name="TreeNodeID"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public IList<B_DocumentAttachments> GetDocumentAttachmentsListBySearch(Guid BusinessID, string FileName)
        {
            string sql = ORMapping.GetSelectSql<B_DocumentAttachments>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            //sql += " AND BusinessID=@BusinessID ";
            sql += " AND CHARINDEX(@FileName,FileName ) >0 ";
            sql += " ORDER BY CreateTime DESC";
            //SqlParameter _BusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, BusinessID);
            SqlParameter _FileName = CreateSqlParameter("@FileName", System.Data.DbType.String, FileName);
            return ExecuteQuery(sql, new SqlParameter[] {_FileName });

        }

        public IList<B_DocumentAttachments> GetDocumentAttachmentsListByName(string FileName)
        {
            string sql = ORMapping.GetSelectSql<B_DocumentAttachments>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            //sql += " AND BusinessID=@BusinessID ";
            sql += " AND  [FileName] LIKE '%@FileName%' ";
            sql += " ORDER BY CreateTime DESC";

            //SqlParameter _BusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, BusinessID);
            SqlParameter _FileName = CreateSqlParameter("@FileName", System.Data.DbType.String, FileName);
            return ExecuteQuery(sql, _FileName);

        }

        public IList<B_DocumentAttachments> GetListByBID(Guid BusinessID)
        {
            string sql =  ORMapping.GetSelectSql<B_DocumentAttachments>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;

            sql += " AND BusinessID=@BusinessID ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter _BusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, BusinessID);
            return ExecuteQuery(sql, new SqlParameter[] { _BusinessID });

        }

        public IList<B_DocumentAttachments> GetListByBID(Guid BusinessID,Guid SystemId,string Year="")
        {
            string sql = @";with cr as ( 
    select ID from C_DocumentTree  
	 where SystemID=@SystemID AND  id=@BusinessID AND IsDeleted=0
	UNION ALL
    SELECT e.ID from C_DocumentTree e 
	INNER JOIN cr 
        ON e.ParentID = cr.ID 
	 and e.SystemID=@SystemID AND e.IsDeleted=0) ";
             sql += ORMapping.GetSelectSql<B_DocumentAttachments>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            if (!string.IsNullOrEmpty(Year))
                sql += " AND FinYear=@Year ";
            sql += " AND BusinessID IN (SELECT ID FROM cr) ";

            sql += " ORDER BY CreateTime DESC";
            SqlParameter _SystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemId);
            SqlParameter _Year = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter _BusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, BusinessID);
            return ExecuteQuery(sql, new SqlParameter[] { _SystemID, _BusinessID, _Year });

        }
       

        public IList<B_DocumentAttachments> GetListByValueA(Guid ValueA)
        {
            string sql = ORMapping.GetSelectSql<B_DocumentAttachments>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ValueA=@ValueA";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter _BusinessID = CreateSqlParameter("@ValueA", System.Data.DbType.Guid, ValueA);

            return ExecuteQuery(sql, new SqlParameter[] { _BusinessID });

        }


    }
}
