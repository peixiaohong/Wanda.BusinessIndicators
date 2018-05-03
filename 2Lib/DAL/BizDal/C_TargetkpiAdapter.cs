
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Targetkpi对象的数据访问适配器
    /// </summary>
    sealed class C_TargetkpiAdapter : AppBaseAdapterT<C_TargetKpi>
	{

        public IList<C_TargetKpi> GetTargetKpiListByVersionTime(Guid _cTargetKpiID, DateTime? _VersionTime)
        {
            string sql = ORMapping.GetSelectSql<C_TargetKpi>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID =@TargetKpiID ";
            sql += " AND VersionStart <=  @VersionTime  and @VersionTime <= VersionEnd ";
            sql += " ORDER BY Sequence ASC ";

            SqlParameter TargetKpiID = CreateSqlParameter("@TargetKpiID", System.Data.DbType.Guid, _cTargetKpiID);
            SqlParameter VersionTime = CreateSqlParameter("@VersionTime", System.Data.DbType.Guid, _VersionTime);

            return ExecuteQuery(sql, TargetKpiID, VersionTime);
        }


        public IList<C_TargetKpi> GetTargetkpiList()
        {
            string sql = ORMapping.GetSelectSql<C_TargetKpi>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        
        
        /// <summary>
        /// 根据systemID得到年度指标
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public IList<C_TargetKpi> GetTargetkpiList(Guid systemID, int FinYear)
        {
            string sql = ORMapping.GetSelectSql<C_TargetKpi>(TSqlBuilder.Instance);
            
            sql += "WHERE " + base.NotDeleted;
            sql += " AND systemID=@systemID ";
            sql += " AND FinYear=@FinYear ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pSystemID = CreateSqlParameter("@systemID", System.Data.DbType.Guid, systemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int16, FinYear);
            return ExecuteQuery(sql, pSystemID, pFinYear);
        }
        /// <summary>
        /// 获取年度指标
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public IList<C_TargetKpi> GetTargetkpiList(int FinYear)
        {
            string sql = ORMapping.GetSelectSql<C_TargetKpi>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND FinYear=@FinYear ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int16, FinYear);
            return ExecuteQuery(sql, pFinYear);
        }

    } 
}

