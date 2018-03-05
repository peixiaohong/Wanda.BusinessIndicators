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
    sealed class R_MissTargetEvaluationScopeAdapter : AppBaseAdapterT<R_MissTargetEvaluationScope>
    {
        public IList<R_MissTargetEvaluationScope> GetTargetmappingList()
        {
            string sql = ORMapping.GetSelectSql<R_MissTargetEvaluationScope>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public IList<R_MissTargetEvaluationScope> GetTargetmappingList(Guid SystemID, Guid TargetID, int FinYear, int FinMonth)
        {
            string sql = ORMapping.GetSelectSql<R_MissTargetEvaluationScope>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID =@_SystemID  ";
            sql += " AND TargetID =@_TargetID  ";
            sql += " AND FinYear =@_FinYear ";
            sql += " AND FinMonth =@_FinMonth  ";

            SqlParameter pSystemID = CreateSqlParameter("@_SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pTargetID = CreateSqlParameter("@_TargetID", System.Data.DbType.Guid, TargetID);

            SqlParameter pFinYear = CreateSqlParameter("@_FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@_FinMonth", System.Data.DbType.Int32, FinMonth);

            return ExecuteQuery(sql, pSystemID, pTargetID, pFinYear, pFinMonth);

        }

        /// <summary>
        /// 根据type获取数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public IList<R_MissTargetEvaluationScope> GetEvaluationDetailByType(Guid SystemID, Guid TargetID, int FinYear, int FinMonth,string Type)
        {
            string sql = ORMapping.GetSelectSql<R_MissTargetEvaluationScope>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID =@_SystemID  ";
            sql += " AND TargetID =@_TargetID  ";
            sql += " AND FinYear =@_FinYear ";
            sql += " AND FinMonth =@_FinMonth  ";
            sql += " AND EvaluationType =@EvaluationType  ";
            SqlParameter pSystemID = CreateSqlParameter("@_SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pTargetID = CreateSqlParameter("@_TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pEvaluationType = CreateSqlParameter("@EvaluationType", System.Data.DbType.String, Type);
            SqlParameter pFinYear = CreateSqlParameter("@_FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@_FinMonth", System.Data.DbType.Int32, FinMonth);

            return ExecuteQuery(sql, pSystemID, pTargetID, pFinYear, pFinMonth, pEvaluationType);
        }

         
    }
}
