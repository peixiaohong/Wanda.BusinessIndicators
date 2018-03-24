
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using System.Linq;
using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Targetplandetail对象的数据访问适配器
    /// </summary>
    sealed class B_TargetplandetailAdapter : AppBaseAdapterT<B_TargetPlanDetail>
	{

        public IList<B_TargetPlanDetail> GetTargetPlandetailList()
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 计划指标
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <returns>计划指标</returns>
        public IList<B_TargetPlanDetail> GetTargetPlanDetailList(Guid SystemID, int FinYear, int FinMonth)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += "AND Systemid=@SystemID";

            sql += "AND FinYear=@FinYear";

            sql += "AND FinMonth=@FinMonth";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Guid, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Guid, FinMonth);

            return ExecuteQuery(sql, pSystemID, pFinYear, pFinMonth);
        }

        /// <summary>
        /// 获取项目系统小计的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public List<B_TargetPlanDetail> GetBProTargetplan(Guid SystemID, Guid TargetID, int FinYear, Guid TargetPlanID)
        {
            string sql = @"SELECT  dbo.B_TargetPlanDetail.*
FROM    dbo.C_Company
        LEFT JOIN dbo.B_TargetPlanDetail ON B_TargetPlanDetail.CompanyID = C_Company.ID
WHERE   TargetID = @TargetID
        AND C_Company.SystemID = @SystemID
        AND FinYear = @FinYear
		AND TargetPlanID=@TargetPlanID
        AND Sequence < 0
		AND B_TargetPlanDetail.IsDeleted=0
		AND C_Company.IsDeleted=0
ORDER BY FinMonth";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            return ExecuteQuery(sql, pSystemID, pFinYear, pTargetID, pTargetPlanID);
        }

        /// <summary>
        /// 计划指标
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <returns>计划指标</returns>
        public IList<B_TargetPlanDetail> GetTargetPlanDetailList(Guid SystemID, int FinYear)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += "AND Systemid=@SystemID ";

            sql += "AND FinYear=@FinYear";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);

            return ExecuteQuery(sql, pSystemID, pFinYear);
        }

        public B_TargetPlanDetail GetTargetplandetailByID(Guid ID)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID=@ID";

            SqlParameter pID = CreateSqlParameter("@ID", System.Data.DbType.Guid, ID);
            List<B_TargetPlanDetail> result = ExecuteQuery(sql, pID);
            return (result != null && result.Count > 0) ? result.FirstOrDefault() : null;
        }
        public IList<B_TargetPlanDetail> GetTargetPlanDetailList(Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND TargetPlanID=@TargetPlanID";

            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);

            return ExecuteQuery(sql, pTargetPlanID);
        }
        public IList<B_TargetPlanDetail> GetTPListByPlanIDandTargetID(Guid TargetPlanID,Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND TargetPlanID=@TargetPlanID";
            sql += " AND TargetID=@TargetID";
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pTargetPlanID, pTargetID);
        }

        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int UpdateB_TargetPlanDetailLisr(List<B_TargetPlanDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {

                    sb.Append(ORMapping.GetUpdateSql<B_TargetPlanDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
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
        internal int AddB_TargetPlanDetailLisr(List<B_TargetPlanDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetInsertSql<B_TargetPlanDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
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
        /// 获取累计指标分解数据(按指标分解)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        internal DataTable GetTargetHistory(int FinYear, Guid SystemID, Guid TargetID,Guid TargetPlanID)
        {
            string sql = string.Empty;
            sql += @"
SELECT * FROM (
SELECT dbo.C_Company.CompanyName,dbo.C_Company.ID,dbo.C_Company.OpeningTime,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=1 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target1,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=2 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target2,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=3 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target3,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=4 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target4,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=5 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target5,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=6 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target6,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=7 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target7,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=8 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target8,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=9 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target9,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=10 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target10,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=11 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target11,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=12 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS Target12,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=1 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget1,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=2 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget2,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=3 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget3,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=4 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget4,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=5 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget5,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=6 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget6,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=7 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget7,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=8 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget8,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=9 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget9,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=10 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget10,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=11 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget11,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=12 AND TargetPlanID=@TargetPlanID AND dbo.C_Company.ID=CompanyID) AS SumTarget12,Sequence
  FROM dbo.C_Company WHERE C_Company.SystemID=@SystemID AND C_Company.IsDeleted=0 
  ) AS a  WHERE target1 IS NOT NULL ORDER BY Sequence
";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, FinYear);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            return ExecuteReturnTable(sql, pSystemID, pYear, pTargetID,pTargetPlanID);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="TargetPlanID"></param>
        /// <returns></returns>
        public IList<V_TargetPlan_Mobile> GetTargetPlanDetailByMobile(Guid TargetPlanID)
        {
            string sql = "GetTargetPlanByMobile ";
            SqlParameter p1 = new SqlParameter("@TargetPlanID", TargetPlanID);
            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1);

            List<V_TargetPlan_Mobile> data = new List<V_TargetPlan_Mobile>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                V_TargetPlan_Mobile item = new V_TargetPlan_Mobile();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }



        /// <summary>
        /// 获取累计指标汇总数据(经营系统)--移动端
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailJY(int FinYear, int FinMonth, Guid SystemID, Guid TargetPlanID, DateTime Date)
        {
            string sql = string.Empty;
            //是否查询累计值
            sql += @"
SELECT  A1.TargetName ,
        A1.TargetID ,
        A2.target ,
        A1.SumTarget ,
        A1.Sequence
FROM    ( SELECT    t1.TargetName ,
                    TargetID ,
                    t2.SumTarget ,
                    t1.Sequence
          FROM      dbo.C_Target t1
                    RIGHT JOIN ( SELECT TargetID ,
                                        SUM(Target) SumTarget
                                 FROM   dbo.B_TargetPlanDetail
                                 WHERE  FinYear = @FinYear
                                        AND FinMonth <= @FinMonth
                                        AND SystemID = @SystemID
                                        AND IsDeleted = 0
                                        AND TargetPlanID =@TargetPlanID
                                 GROUP BY TargetID
                               ) t2 ON t1.ID = t2.TargetID
          WHERE     t1.IsDeleted = 0
                    AND t1.VersionStart <= @CurrentDate
                    AND @CurrentDate < t1.VersionEnd
        ) A1
        LEFT JOIN ( SELECT  t1.TargetName ,
                            TargetID ,
                            t2.target
                    FROM    dbo.C_Target t1
                            RIGHT JOIN ( SELECT TargetID ,
                                                SUM(Target) target
                                         FROM   dbo.B_TargetPlanDetail
                                         WHERE  FinYear = @FinYear
                                                AND FinMonth = @FinMonth
                                                AND SystemID = @SystemID
                                                AND IsDeleted = 0
                                                AND TargetPlanID =@TargetPlanID
                                         GROUP BY TargetID
                                       ) t2 ON t1.ID = t2.TargetID
                    WHERE   t1.IsDeleted = 0
                            AND t1.VersionStart <= @CurrentDate
                            AND @CurrentDate < t1.VersionEnd
                  ) A2 ON A1.TargetID = A2.TargetID
ORDER BY A1.Sequence ASC
";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.String, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Date);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            return ExecuteReturnTable(sql, pSystemID, pYear, pMonth, pCurrentDate,pTargetPlanID);
        }


        /// <summary>
        /// 获取累计指标汇总数据(经营系统)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailPro(int FinYear, int FinMonth, Guid CompanyID, Guid TargetPlanID, DateTime Date)
        {
            string sql = string.Empty;
            //是否查询累计值
            sql += @"
SELECT  A1.TargetName ,
        A1.TargetID ,
        Target ,
        SumTarget ,
        Sequence
FROM    ( SELECT    TargetName ,
                    TargetID ,
                    Target ,
                    Sequence
          FROM      dbo.B_TargetPlanDetail
                    LEFT JOIN dbo.C_Target ON C_Target.ID = B_TargetPlanDetail.TargetID
          WHERE     CompanyID = @CompanyID
                    AND FinYear = @FinYear
                    AND FinMonth = @FinMonth
                    AND C_Target.VersionStart <= @CurrentDate
                    AND @CurrentDate < C_Target.VersionEnd
                    AND TargetPlanID =@TargetPlanID
        ) A1
        LEFT JOIN ( SELECT  TargetID ,
                            SUM(Target) SumTarget
                    FROM    dbo.B_TargetPlanDetail
                            LEFT JOIN dbo.C_Target ON C_Target.ID = B_TargetPlanDetail.TargetID
                    WHERE   CompanyID = @CompanyID
                            AND FinYear = @FinYear
                            AND FinMonth <= @FinMonth
                            AND C_Target.VersionStart <= @CurrentDate
                            AND @CurrentDate < C_Target.VersionEnd
                            AND TargetPlanID =@TargetPlanID
                    GROUP BY TargetID
                  ) t2 ON A1.TargetID = t2.TargetID
ORDER BY A1.Sequence
";
            SqlParameter pSystemID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.String, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Date);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            return ExecuteReturnTable(sql, pSystemID, pYear, pMonth, pCurrentDate, pTargetPlanID);
        }



    } 
}

