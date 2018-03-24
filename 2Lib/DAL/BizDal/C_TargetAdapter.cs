
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data;
using System.Linq;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Target对象的数据访问适配器
    /// </summary>
    sealed class C_TargetAdapter : AppBaseAdapterT<C_Target>
	{


        public IList<C_Target> GetTargetListByVersionTime(Guid _cTargetID, DateTime? _VersionTime)
        {
            string sql = ORMapping.GetSelectSql<C_Target>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID =@TargetID ";
            sql += " AND VersionStart <=  @VersionTime  and @VersionTime < VersionEnd ";
            sql += " ORDER BY Sequence ASC ";

            SqlParameter cTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, _cTargetID);
            SqlParameter VersionTime = CreateSqlParameter("@VersionTime", System.Data.DbType.DateTime, _VersionTime);

            return ExecuteQuery(sql, cTargetID, VersionTime);
        }

        public IList<C_Target> GetTargetList()
        {
            string sql = ORMapping.GetSelectSql<C_Target>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND  VersionStart <= GETDATE() AND GETDATE()< VersionEnd ";
            sql += " ORDER BY Sequence DESC";
            return ExecuteQuery(sql);
        }


        public IList<C_Target> GetTargetList(Guid SystemID, DateTime CurrentDate)
        {
            string sql = ORMapping.GetSelectSql<C_Target>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID ";
            sql += " AND  VersionStart <= @CurrentDate AND @CurrentDate < VersionEnd ";
            sql += " ORDER BY Sequence ASC";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, CurrentDate);
            List<C_Target> list = ExecuteQuery(sql, pSystemID, pCurrentDate);
            return (list != null && list.Count > 0) ? list : null;

        }

        public IList<string> GetTargetList (string SystemIDs, DateTime CurrentDate)
        {
            string sql = ORMapping.GetSelectSql<C_Target>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND  CAST(SystemID AS NVARCHAR(36)) in (" + SystemIDs + ")";
            sql += " AND  VersionStart <= @CurrentDate AND @CurrentDate < VersionEnd ";
            sql += " ORDER BY Sequence ASC";

            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, CurrentDate);
            DataSet ds = DbHelper.RunSqlReturnDS(sql.ToString(), ConnectionName , pCurrentDate);
            
           // List<C_Target> list = new List<C_Target>();

            List<string> list1 = new List<string>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                list1.Add(row["TargetName"].ToString());
                //C_Target item = new C_Target();
                //ORMapping.DataRowToObject(row, item);
                //list.Add(item);
            });
            
            return (list1 != null && list1.Count > 0) ? list1 : null;

        }






        public IList<C_Target> GetDetailTargetList(Guid SystemID)
        {
            string sql = ORMapping.GetSelectSql<C_Target>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID  AND NeedEvaluation=1";
            sql += " ORDER BY Sequence DESC";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);

            List<C_Target> list = ExecuteQuery(sql, pSystemID);
            return (list != null && list.Count > 0) ? list : null;

        }
        public int UpdateTargetVerSion(C_Target data,DateTime Verstart)
        {
            int i = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(ORMapping.GetUpdateSql<C_Target>(data, TSqlBuilder.Instance) + sqlSeperator);
            sql.AppendFormat(" AND VersionStart='{0}'",Verstart.ToString("yyyy-MM-dd HH:mm:ss:fff"));

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                i = ExecuteSql(sql.ToString());
                scope.Complete();
            }
            return i;
        }

        public int DeleteTargetListByID(Guid ID)
        {
            string sql = "UPDATE dbo.C_Target SET IsDeleted=1 WHERE ID=@ID";
            SqlParameter pTargetID = CreateSqlParameter("@ID", System.Data.DbType.Guid, ID);
            return ExecuteSql(sql, pTargetID);
        }
        /// <summary>
        /// 仅获取收入和支出类的指标
        /// </summary>
        /// <returns></returns>
        public IList<C_Target> GetTargetForType(Guid SystemID) 
        {
            string sql = "SELECT * FROM dbo.C_Target WHERE SystemID=@SystemID AND   (TargetType<3 OR TargetType=4)  AND IsDeleted=0 AND NeedEvaluation=1 ORDER BY Sequence";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            return ExecuteQuery(sql, pSystemID);

        }
	} 
}

