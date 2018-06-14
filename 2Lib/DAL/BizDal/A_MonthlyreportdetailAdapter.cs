
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using Lib.Data;
using System.Linq;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Monthlyreportdetail对象的数据访问适配器
    /// </summary>
    sealed class A_MonthlyreportdetailAdapter : AppBaseAdapterT<A_MonthlyReportDetail>
    {

        public IList<A_MonthlyReportDetail> GetMonthlyreportdetailList()
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        internal DataSet GetAMonthlyreportdetailSimpleRpt(Guid SystemID, int Year, int Month)
        {
            string sql = string.Empty;
            DateTime CurrentDate = DateTime.Now;

            C_System _cSystem = new C_SystemAdapter().GetSystemList(CurrentDate).Where(p => p.ID == SystemID).FirstOrDefault();
            if (_cSystem.Category == 2)
            {
                #region
                sql = @"SELECT CT.ID, CT.TargetName ,
        T.*
FROM    dbo.C_Target CT
        INNER JOIN ( SELECT D.TargetID ,
                           SUM(D.NAccumulativePlanAmmount) AS NPlanAmmount ,
                            SUM(D.NAccumulativeActualAmmount) AS NActualAmmount ,
                            SUM(D.NAccumulativeDifference) AS NDifference
                     FROM   dbo.A_MonthlyReportDetail D
                            INNER JOIN dbo.C_Target C ON D.TargetID = C.ID
                     WHERE  D.IsDeleted = 0
                            AND D.SystemID = @sid
                            AND D.FinYear = @year
                            AND D.FinMonth = @month
                            AND c.IsDeleted = 0
                            AND c.NeedReport = 1
                     GROUP BY TargetID
                   ) AS T ON CT.ID = t.TargetID  order by ct.Sequence
SELECT  CT.ID,CT.TargetName ,
        T.*
FROM    dbo.C_Target CT
        INNER JOIN ( SELECT D.TargetID ,
                           SUM(D.NPlanAmmount) AS NPlanAmmount ,
                            SUM(D.NActualAmmount) AS NActualAmmount ,
                            SUM(D.NDifference) AS NDifference
                     FROM   dbo.A_MonthlyReportDetail D
                            INNER JOIN dbo.C_Target C ON D.TargetID = C.ID
                     WHERE  D.IsDeleted = 0
                            AND D.SystemID = @sid
                            AND D.FinYear = @year
                            AND D.FinMonth = @month
                            AND c.IsDeleted = 0
                            AND c.NeedReport = 1
                     GROUP BY TargetID
                   ) AS T ON CT.ID = t.TargetID order by ct.Sequence";
                #endregion
            }
            else if (_cSystem.Category == 3)
            {
                #region
                sql = @"SELECT CT.ID, CT.TargetName ,
        T.*
FROM    dbo.C_Target CT
        INNER JOIN ( SELECT D.TargetID ,
                           SUM(D.NAccumulativePlanAmmount) AS NPlanAmmount ,
                            SUM(D.NAccumulativeActualAmmount) AS NActualAmmount ,
                            SUM(D.NAccumulativeDifference) AS NDifference
                     FROM   dbo.A_MonthlyReportDetail D
                            INNER JOIN dbo.C_Target C ON D.TargetID = C.ID
                     WHERE  D.IsDeleted = 0
                            AND D.SystemID = @sid
                            AND D.FinYear = @year
                            AND D.FinMonth = @month
                            AND c.IsDeleted = 0
                            AND c.NeedReport = 1
                     GROUP BY TargetID
                   ) AS T ON CT.ID = t.TargetID  order by ct.Sequence

SELECT CT.ID, CT.TargetName ,
        T.*
FROM    dbo.C_Target CT
        INNER JOIN ( SELECT D.TargetID ,
                           SUM(D.NAccumulativePlanAmmount) AS NPlanAmmount ,
                            SUM(D.NAccumulativeActualAmmount) AS NActualAmmount ,
                            SUM(D.NAccumulativeDifference) AS NDifference
                     FROM   dbo.A_MonthlyReportDetail D
                            INNER JOIN dbo.C_Target C ON D.TargetID = C.ID
                     WHERE  D.IsDeleted = 0
                            AND D.SystemID = @sid
                            AND D.FinYear = @year
                            AND D.FinMonth = @month
                            AND c.IsDeleted = 0
                            AND c.NeedReport = 1
                     GROUP BY TargetID
                   ) AS T ON CT.ID = t.TargetID  order by ct.Sequence";
                #endregion
            }
            else
            {
                #region
                sql = @"
SELECT CT.ID, CT.TargetName ,
        T.*
FROM    dbo.C_Target CT
        INNER JOIN ( SELECT D.TargetID ,
                            SUM(D.NAccumulativePlanAmmount) AS NPlanAmmount ,
                            SUM(D.NAccumulativeActualAmmount) AS NActualAmmount ,
                            SUM(D.NAccumulativeDifference) AS NDifference
                     FROM   dbo.A_MonthlyReportDetail D
                            INNER JOIN dbo.C_Target C ON D.TargetID = C.ID
                     WHERE  D.IsDeleted = 0
                            AND D.SystemID = @sid
                            AND D.FinYear = @year
                            AND D.FinMonth = @month
                            AND c.IsDeleted = 0
                            AND c.NeedReport = 1
                     GROUP BY TargetID
                   ) AS T ON CT.ID = t.TargetID  order by ct.Sequence


SELECT  CT.ID,CT.TargetName ,
        T.*
FROM    dbo.C_Target CT
        INNER JOIN ( SELECT D.TargetID ,
                            SUM(ISNULL(D.NPlanAmmount, 0)) AS NPlanAmmount ,
                            SUM(ISNULL(D.NActualAmmount, 0)) AS NActualAmmount ,
                            SUM(ISNULL(D.NDifference, 0)) AS NDifference
                     FROM   dbo.A_MonthlyReportDetail D
                            INNER JOIN dbo.C_Target C ON D.TargetID = C.ID
                     WHERE  D.IsDeleted = 0
                            AND D.SystemID = @sid
                            AND D.FinYear = @year
                            AND D.FinMonth = @month
                            AND c.IsDeleted = 0
                            AND c.NeedReport = 1
                     GROUP BY TargetID
                   ) AS T ON CT.ID = t.TargetID order by ct.Sequence ";
                #endregion
            }

            SqlParameter pSystemID = CreateSqlParameter("@sid", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@month", System.Data.DbType.String, Month);
            return ExecuteReturnDataSet(sql, pSystemID, pYear, pMonth);
        }
        internal IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);

            return ExecuteQuery(sql, pSystemID, pYear, pMonth);
        }


        internal List<MonthlyReportDetail> GetMonthlyReportDetailList_Result(Guid SystemID, int Year, int Month, Guid TargetPlanID)
        {
            string sql = "GetMonthlyReportDetailList_Result ";

            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, CreateSqlParameter("@SystemID", DbType.Guid, SystemID), CreateSqlParameter("@FinYear", DbType.Int32, Year), CreateSqlParameter("@FinMonth", DbType.Int32, Month), CreateSqlParameter("@TargetPlanID", DbType.Guid, TargetPlanID));
            List<MonthlyReportDetail> data = new List<MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportDetail item = new MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                item.NDisplayRateByYear = item.NPlanAmmountByYear == 0 ? "--" : Math.Round((item.NAccumulativeActualAmmount / item.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                data.Add(item);
            });
            return data;
        }

        internal IList<MonthlyReportVM> GetAVMonthlyReport(Guid SystemID, int Year, int Month)
        {
            string sql = @"
SELECT  TargetID ,
        dbo.A_MonthlyReportDetail.SystemID ,
        CompanyID ,
        CompanyName ,
        A_MonthlyReportDetail.CompanyProperty1 ,
        IsMissTarget ,
        Counter ,
        IsMissTargetCurrent ,
        NAccumulativeActualAmmount
FROM    dbo.A_MonthlyReportDetail
        LEFT JOIN dbo.C_Company ON C_Company.ID = A_MonthlyReportDetail.CompanyID
WHERE   A_MonthlyReportDetail.SystemID = @SystemID
        AND FinYear = @FinYear
        AND FinMonth = @FinMonth
        AND A_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0";
            SqlParameter pSystem = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, Year);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, Month);

            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pSystem, pYear, pMonth);
            List<MonthlyReportVM> data = new List<MonthlyReportVM>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportVM item = new MonthlyReportVM();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }
        internal IList<A_MonthlyReportDetail> GetAMReportDetailIsMissTargetList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);
            sql += "WHERE" + base.NotDeleted;
            sql += "AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND IsMissTarget=1 ";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);

            return ExecuteQuery(sql, pSystemID, pYear, pMonth);
        }
        internal IList<MonthlyReportDetail> GetVMissDetail(Guid SystemID, int Year, int Month, Guid TargetID, bool IsSpecial)
        {
            string sql = @"SELECT * FROM (
 SELECT A_MonthlyReportDetail.*,C_Company.CompanyName,C_Company.NeedEvaluation,C_Company.Sequence FROM dbo.A_MonthlyReportDetail	INNER JOIN dbo.C_Company ON 
                dbo.A_MonthlyReportDetail.SystemID = dbo.C_Company.SystemID AND 
                    dbo.A_MonthlyReportDetail.CompanyID = dbo.C_Company.ID ) aa 
					WHERE  aa.SystemID=@SystemID AND aa.TargetID=@TargetID
					AND aa.FinMonth=@Month AND aa.FinYear=@Year";

            if (IsSpecial == false)
            {
                sql += " AND IsMissTarget=1";
                sql += " ORDER BY Sequence DESC ";
            }
            else
            {
                sql += " AND NAccumulativeDifference<0 ";
                sql += " ORDER BY Sequence DESC ";
            }
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTargetID, pSystemID, pYear, pMonth);
            List<MonthlyReportDetail> data = new List<MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportDetail item = new MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }
        internal IList<MonthlyReportDetail> GetVMissDetail_defaultPlan(Guid SystemID, int Year, int Month, Guid TargetID, bool IsSpecial)
        {
            string sql = @"SELECT * FROM (
 SELECT A_MonthlyReportDetail.*,C_Company.CompanyName,C_Company.NeedEvaluation,C_Company.Sequence FROM dbo.A_MonthlyReportDetail 
INNER JOIN dbo.A_TargetPlan ON dbo.A_MonthlyReportDetail.TargetPlanID=A_TargetPlan.ID AND A_TargetPlan.VersionDefault=1 
INNER JOIN dbo.C_Company ON 
                dbo.A_MonthlyReportDetail.SystemID = dbo.C_Company.SystemID AND 
                    dbo.A_MonthlyReportDetail.CompanyID = dbo.C_Company.ID ) aa 
					WHERE  aa.SystemID=@SystemID AND aa.TargetID=@TargetID
					AND aa.FinMonth=@Month AND aa.FinYear=@Year";

            if (IsSpecial == false)
            {
                sql += " AND IsMissTarget=1";
                sql += " ORDER BY Sequence DESC ";
            }
            else
            {
                sql += " AND NAccumulativeDifference<0 ";
                sql += " ORDER BY Sequence DESC ";
            }
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTargetID, pSystemID, pYear, pMonth);
            List<MonthlyReportDetail> data = new List<MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportDetail item = new MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }
        internal IList<A_MonthlyReportDetail> GetAMReportDetailDifferenceList(Guid SystemID, int Year, int Month, Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);
            sql += "WHERE" + base.NotDeleted;
            sql += "AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND TargetID=@TargetID AND NAccumulativeDifference<0 ";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pSystemID, pYear, pMonth, pTargetID);
        }
        internal IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int Year, int Month, Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month and TargetID=@TargetID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);

            return ExecuteQuery(sql, pSystemID, pYear, pMonth, pTargetID);
        }

        internal IList<A_MonthlyReportDetail> GetAMonthlyReportDetailListForTargetPlanID(Guid SystemID, int Year, int Month, Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month and TargetPlanID=@TargetPlanID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);

            return ExecuteQuery(sql, pSystemID, pYear, pMonth, pTargetPlanID);
        }

        internal IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid MonthlyReportID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and MonthlyReportID=@MonthlyReportID";
            SqlParameter pMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);

            return ExecuteQuery(sql, pMonthlyReportID);
        }

        /// <summary>
        /// 通过系统ID，获取全年的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        internal IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and SystemID=@SystemID and FinYear=@Year";
            SqlParameter _SystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter _FinYear = CreateSqlParameter("@Year", System.Data.DbType.Int32, Year);

            return ExecuteQuery(sql, _SystemID, _FinYear);
        }


        internal A_MonthlyReportDetail GetAMonthlyreportdetail(Guid SystemID, Guid CompanyID, Guid TargetID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND CompanyID=@CompanyID AND FinYear=@Year AND FinMonth=@Month and TargetID=@TargetID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);

            return ExecuteQuery(sql, pSystemID, pCompanyID, pYear, pMonth, pTargetID).FirstOrDefault();
        }
        internal A_MonthlyReportDetail GetAMonthlyreportdetail(Guid SystemID, Guid CompanyID, Guid TargetID, int Year, int Month, Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND CompanyID=@CompanyID AND FinYear=@Year AND FinMonth=@Month and TargetID=@TargetID AND TargetPlanID=@TargetPlanID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);

            return ExecuteQuery(sql, pSystemID, pCompanyID, pYear, pMonth, pTargetID, pTargetPlanID).FirstOrDefault();
        }
        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int UpdateMonthlyreportdetailLisr(List<A_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {

                    sb.Append(ORMapping.GetUpdateSql<A_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
                });

                if (sb.Length > 0)
                {
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        i = ExecuteSql(sb.ToString());
                        scope.Complete();
                    }
                }

                return i;
            }
            else
            {
                return i;
            }
        }


        /// <summary>
        /// 添加数据到列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int AddMonthlyreportdetailLisr(List<A_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetInsertSql<A_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
                });

                if (sb.Length > 0)
                {
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        i = ExecuteSql(sb.ToString());
                        scope.Complete();
                    }
                }

                return i;
            }
            else
            {
                return i;
            }
        }
        /// <summary>
        /// 万达电影固定报表，国内影城-票房收入指标查询
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<A_MonthlyReportDetail> GetMonthTargetDetailCNPF(Guid SystemID, int FinYear, int FinMonth, Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth AND TargetID=@TargetID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int64, FinMonth);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);

            return ExecuteQuery(sql, pSystemID, pFinYear, pFinMonth, pTargetID);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="pTargetPlanID"></param>
        /// <returns></returns>
        public List<A_MonthlyReportDetail> GetAMonthlyReportDetailListForActualAmmount(Guid SystemID, int FinYear, int FinMonth)
        {
            string sql = @"
            SELECT  A.*
            FROM    dbo.A_MonthlyReportDetail AS A
                    INNER JOIN ( SELECT TOP 1
                                        FinYear ,
                                        FinMonth ,
                                        TargetPlanID
                                 FROM   dbo.A_MonthlyReport
                                 WHERE  SystemID = @SystemID
                                        AND IsDeleted = 0
                                        AND FinYear = @FinYear
                                        AND FinMonth<@FinMonth
                                 ORDER BY CreateTime DESC
                               ) AS T ON A.FinYear = T.FinYear
                                         AND A.FinMonth = T.FinMonth
                                         AND A.TargetPlanID = T.TargetPlanID
            WHERE   A.IsDeleted = 0
                    AND A.SystemID = @SystemID
                    AND A.FinYear = @FinYear;";


            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int64, FinMonth);

            return ExecuteQuery(sql, pSystemID, pFinYear, pFinMonth);
        }

        internal int DeleteMonthlyreportdetailLisr(List<A_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetDeleteSql<A_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
                });

                if (sb.Length > 0)
                {
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        i = ExecuteSql(sb.ToString());
                        scope.Complete();
                    }
                }

                return i;
            }
            else
            {
                return i;
            }
        }

        internal int DeleteModel(A_MonthlyReportDetail Model)
        {
            int i = 0;
            string SQL = ORMapping.GetDeleteSql<A_MonthlyReportDetail>(Model, TSqlBuilder.Instance);
            if (SQL.Length > 0)
            {
                i = ExecuteSql(SQL.ToString());
            }
            return i;
        }




    }
}

