using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.DAL
{
    sealed class C_DocumentTreeAdapter : AppBaseAdapterT<C_DocumentTree>
    {
        public IList<C_DocumentTree> GetDocumentTreeList()
        {
            string sql = ORMapping.GetSelectSql<C_DocumentTree>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }


        /// <summary>
        /// 通过父ID 获取子级List
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public IList<C_DocumentTree> GetDocumentTreeList(Guid ParentID, Guid SysID)
        {
            string sql = ORMapping.GetSelectSql<C_DocumentTree>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND ParentID=@ParentID ";
            sql += " AND SystemID=@SysID ";
            SqlParameter _ParentID = CreateSqlParameter("@ParentID", System.Data.DbType.Guid, ParentID);
            SqlParameter _SystemID = CreateSqlParameter("@SysID", System.Data.DbType.Guid, SysID);
            return ExecuteQuery(sql, new SqlParameter[] { _ParentID, _SystemID });

        }


        /// <summary>
        /// 通过父ID 获取子级List
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public IList<C_DocumentTree> GetDocumentTreeListByID(Guid ParentID)
        {
            string sql = ORMapping.GetSelectSql<C_DocumentTree>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND ParentID=@ParentID ";
            SqlParameter _ParentID = CreateSqlParameter("@ParentID", System.Data.DbType.Guid, ParentID);
            return ExecuteQuery(sql, new SqlParameter[] { _ParentID });

        }

        /// <summary>
        /// 通过父ID和名称  获取List
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public IList<C_DocumentTree> GetDocumentTreeListByName(string ParentID, string TreeName)
        {
            string sql = ORMapping.GetSelectSql<C_DocumentTree>(TSqlBuilder.Instance);

            StringBuilder sqla = new StringBuilder(sql);
            sqla.Append(" WHERE" + base.NotDeleted);
            sqla.AppendFormat(" AND ParentID='{0}' AND TreeNodeName LIKE '%{1}%'", ParentID, TreeName);

            return ExecuteQuery(sqla.ToString());

        }
        /// <summary>
        /// 通过条件查早list
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public IList<C_DocumentTree> GetDocumentTreeListByValue(Guid TypeID, Guid CompanyID, string MinYear)
        {
            string sql = ORMapping.GetSelectSql<C_DocumentTree>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += " AND TypeID=@TypeID ";
            sql += " AND CompanyID=@CompanyID ";
            sql += " AND MinYear=@MinYear ";
            SqlParameter _TypeID = CreateSqlParameter("@TypeID", System.Data.DbType.Guid, TypeID);
            SqlParameter _CompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter _MinYear = CreateSqlParameter("@MinYear", System.Data.DbType.String, MinYear);

            return ExecuteQuery(sql, _TypeID, _CompanyID,_MinYear);

        }

    }
}
