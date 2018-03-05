using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.DAL
{
    sealed class C_SystemTreeAdapter : AppBaseAdapterT<C_SystemTree>
    {
        public IList<C_SystemTree> GetSystemTreeList()
        {
            string sql = ORMapping.GetSelectSql<C_SystemTree>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }


        /// <summary>
        /// 通过父ID 获取子级List
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public IList<C_SystemTree> GetSystemTreeList(Guid ParentID, Guid SysID)
        {
            string sql = ORMapping.GetSelectSql<C_SystemTree>(TSqlBuilder.Instance);

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
        public IList<C_SystemTree> GetSystemTreeListByID(Guid ParentID)
        {
            string sql = ORMapping.GetSelectSql<C_SystemTree>(TSqlBuilder.Instance);

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
        public IList<C_SystemTree> GetSystemTreeListByName(string ParentID, string TreeName)
        {
            string sql = ORMapping.GetSelectSql<C_SystemTree>(TSqlBuilder.Instance);

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
        public IList<C_SystemTree> GetSystemTreeListByValue(Guid TypeID, Guid CompanyID, string MinYear)
        {
            string sql = ORMapping.GetSelectSql<C_SystemTree>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += " AND TypeID=@TypeID ";
            sql += " AND CompanyID=@CompanyID ";
            sql += " AND MinYear=@MinYear ";
            SqlParameter _TypeID = CreateSqlParameter("@TypeID", System.Data.DbType.Guid, TypeID);
            SqlParameter _CompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter _MinYear = CreateSqlParameter("@MinYear", System.Data.DbType.String, MinYear);

            return ExecuteQuery(sql, _TypeID, _CompanyID,_MinYear);

        }


        /// <summary>
        /// 通过系统ID串，获取整棵树的数据 （子找父）
        /// </summary>
        /// <param name="SysIds"></param>
        /// <returns></returns>
        public List<C_SystemTree> GetSystemTreeData(string SysIds)
        {

            StringBuilder sql = new StringBuilder();

            sql.Append(" WITH    DataTree ");
            sql.Append("      AS(SELECT * ");
            sql.Append("           FROM     C_SystemTree ");
            sql.Append(" WHERE   IsDeleted =0 and  CAST(ID AS NVARCHAR(36)) IN(" + SysIds + ")");
            sql.Append(" UNION ALL ");
            sql.Append(" SELECT   a.* FROM C_SystemTree a  INNER JOIN DataTree ON a.ID = DataTree.ParentID ) ");
            sql.Append(" SELECT DISTINCT  * FROM    DataTree   where DataTree.IsDeleted =0 ORDER BY Sequence");

            DataSet ds = DbHelper.RunSqlReturnDS(sql.ToString(), ConnectionName);

            List<C_SystemTree> data = new List<C_SystemTree>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                C_SystemTree item = new C_SystemTree();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;
        }



        /// <summary>
        /// 通过系统ID串，获取整棵树的数据 （父找子）
        /// </summary>
        /// <param name="SysIds"></param>
        /// <returns></returns>
        public List<C_SystemTree> GetSystemTreeDataByParent (string SysIds)
        {

            StringBuilder sql = new StringBuilder();

            sql.Append(" WITH    DataTree ");
            sql.Append("      AS(SELECT * ");
            sql.Append("           FROM     C_SystemTree ");
            sql.Append(" WHERE  IsDeleted =0 and    CAST(ID AS NVARCHAR(36)) IN(" + SysIds + ")");
            sql.Append(" UNION ALL ");
            sql.Append(" SELECT   a.* FROM C_SystemTree a  INNER JOIN DataTree ON a.ParentID = DataTree.ID ) ");
            sql.Append(" SELECT DISTINCT  * FROM    DataTree where DataTree.IsDeleted =0 ORDER BY Sequence ");

            DataSet ds = DbHelper.RunSqlReturnDS(sql.ToString(), ConnectionName);

            List<C_SystemTree> data = new List<C_SystemTree>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                C_SystemTree item = new C_SystemTree();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;
        }
        /// <summary>
        /// 获取汇总分组数据
        /// </summary>
        /// <returns></returns>
        public List<C_SystemTree> GetSysTreeExcelGroup()
        {
            //string sql = "SELECT ID,TreeNodeName,ExcelGroup FROM C_SystemTree ";
            string sql = ORMapping.GetSelectSql<C_SystemTree>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ExcelGroup IS NOT Null ";
            sql += " ORDER BY ExcelGroup ";

            return ExecuteQuery(sql);
        }

    }
}
