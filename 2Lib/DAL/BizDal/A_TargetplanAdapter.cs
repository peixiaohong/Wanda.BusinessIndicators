
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;


namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Targetplan对象的数据访问适配器
    /// </summary>
    sealed class A_TargetplanAdapter : AppBaseAdapterT<A_TargetPlan>
    {

        public IList<A_TargetPlan> GetTargetplanList()
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public IList<A_TargetPlan> GetTargetplanList(Guid ID)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += "AND ID=@ID";

            SqlParameter pID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, ID);
            return ExecuteQuery(sql, pID);
        }

        public IList<A_TargetPlan> GetTargetplanList(Guid SystemID, int FinYear)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += "AND Systemid=@SystemID ";

            sql += "AND FinYear=@FinYear";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);

            return ExecuteQuery(sql, pSystemID, pFinYear);
        }


        /// <summary>
        /// 获取多版本混合指标计划
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetTargetplanListForMulitiVersion(Guid SystemID, int FinYear)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlan>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted;

            sql += " AND Systemid=@SystemID ";

            sql += " AND FinYear=@FinYear";

            sql += " AND a.VersionDefault=0";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);

            return ExecuteQuery(sql, pSystemID, pFinYear);
        }

        public IList<A_TargetPlan> GetTargetplanListByRecalculation(DateTime OperatorTime)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlan>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += "AND CreateTime >=@OperatorTime ";

            SqlParameter temp_OperatorTime = CreateSqlParameter("@OperatorTime", System.Data.DbType.DateTime, OperatorTime);

            return ExecuteQuery(sql, new SqlParameter[] { temp_OperatorTime });
        }

        /// <summary>
        /// 获取已有上传年份
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public List<A_TargetPlan> GetPlanYearList()
        {
            string sql = "select distinct FinYear from dbo.A_TargetPlan ";

            sql += "WHERE " + base.NotDeleted;
            sql += " ORDER BY FinYear DESC";

            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 获取已审批通过的分解指标版本类型集合
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetTargetVersionType(string systemID, int year, int month)
        {
            string sql = @"SELECT  distinct b.ID,isnull(b.VersionName,'') AS VersionName,b.VersionDefault,b.CreateTime  FROM A_TargetPlanDetail a INNER JOIN A_TargetPlan b ON a.TargetPlanID=b.ID AND a.IsDeleted=b.IsDeleted
            WHERE a.SystemID =@SystemID AND a.FinYear =@FinYear AND a.FinMonth =@FinMonth AND a.IsDeleted = 0 ";
            sql += " ORDER BY b.VersionDefault desc, b.CreateTime DESC";
            return ExecuteQuery(sql, CreateSqlParameter("@SystemID", System.Data.DbType.Guid, systemID.ToGuid()), CreateSqlParameter("@FinYear", System.Data.DbType.Int32, year), CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, month));
        }


         /// <summary>
        /// 获取已审批通过的分解指标版本类型集合
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetTargetVersionType(string systemID, int year)
        {
            string sql = @"SELECT  *  FROM A_TargetPlan 
            WHERE SystemID =@SystemID AND FinYear =@FinYear  AND IsDeleted = 0 ";
            sql += " ORDER BY VersionDefault desc, CreateTime DESC";
            return ExecuteQuery(sql, CreateSqlParameter("@SystemID", System.Data.DbType.Guid, systemID.ToGuid()), CreateSqlParameter("@FinYear", System.Data.DbType.Int32, year));
        }

        public int DeletePlan(Guid PlanID)
        {
            string sql = @"DELETE dbo.A_TargetPlan  WHERE ID=@PlanID;";
            return ExecuteSql(sql, CreateSqlParameter("@PlanID", System.Data.DbType.Guid, PlanID));
        }
    }
}

