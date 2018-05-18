
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data;
using LJTH.BusinessIndicators.ViewModel;
using System.Linq;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Targetplandetail对象的数据访问适配器
    /// </summary>
    sealed class A_TargetplandetailAdapter : AppBaseAdapterT<A_TargetPlanDetail>
    {

        public IList<A_TargetPlanDetail> GetTargetplandetailList()
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public IList<A_TargetPlanDetail> GetTargetplandetailList(Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += "AND TargetPlanID=@TargetPlanID";

            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);

            return ExecuteQuery(sql, pTargetPlanID);
        }

        public IList<A_TargetPlanDetail> GetTargetplandetailList(Guid SystemID, int FinYear)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += "AND SystemID=@SystemID AND FinYear=@FinYear";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);

            return ExecuteQuery(sql, pSystemID, pFinYear);
        }

        public IList<A_TargetPlanDetail> GetTargetplandetailList(int FinYear)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += "AND FinYear=@FinYear";

            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);

            return ExecuteQuery(sql, pFinYear);
        }


        /// <summary>
        /// 获取默认指标详情
        /// </summary>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public IList<A_TargetPlanDetail> GetDefaultTargetplandetailList(int FinYear)
        {
            string sql = @"SELECT  B.*
                            FROM    dbo.A_TargetPlan AS A WITH ( NOLOCK )
                                    INNER JOIN dbo.A_TargetPlanDetail AS B ON B.TargetPlanID = A.ID
                                                                              AND B.IsDeleted = 0
                            WHERE   A.FinYear = @FinYear
                                    AND A.VersionDefault = 1
                                    AND A.IsDeleted = 0;";

            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);

            return ExecuteQuery(sql, pFinYear);
        }

        /// <summary>
        /// 同步数据--从久其系统里得到相关的数据 , 固定了 系统ID，是院线的ID
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public List<A_TargetPlanDetail> GetTargetplandetailListBySYNC(int FinYear,int FinMonth)
        {

            string sql = "P_SYNC_FilmCompanyData ";
            SqlParameter p1 = CreateSqlParameter("@FinYear", DbType.Int32, FinYear);
            SqlParameter p2 = CreateSqlParameter("@FinMonth", DbType.Int32, FinMonth);
          
            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2);

            List<A_TargetPlanDetail> data = new List<A_TargetPlanDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                A_TargetPlanDetail item = new A_TargetPlanDetail();
                item.ID = Guid.Parse(row["ID"].ToString());
                item.SystemID = Guid.Parse(row["ID"].ToString());
                item.CompanyID = Guid.Parse(row["CompanyID"].ToString());
                item.FinYear = int.Parse(row["FinYear"].ToString());
                item.FinMonth = int.Parse(row["FinMonth"].ToString());
                item.TargetID = Guid.Parse(row["TargetID"].ToString());

                if(!string.IsNullOrEmpty(row["Target"].ToString()))
                    item.Target = decimal.Parse(row["Target"].ToString());
                else
                    item.Target = 0;

                //item.VersionStart = DateTime.Parse(row["VersionStart"].ToString());
                //item.Versionend = DateTime.Parse(row["Versionend"].ToString());
                item.TargetPlanID = Guid.Parse(row["TargetPlanID"].ToString());

                if(!string.IsNullOrEmpty(row["JQNDifference"].ToString()))
                    item.JQNDifference = decimal.Parse(row["JQNDifference"].ToString());
                else
                    item.JQNDifference = 0;

                item.CompanyName = row["CompanyName"].ToString();
                item.TargetName = row["TargetName"].ToString();

                //ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;
            
        }





        public IList<A_TargetPlanDetail> GetTargetplandetailList(Guid SystemID, int FinYear, Guid CompanyID)
        {
            string sql = ORMapping.GetSelectSql<A_TargetPlanDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += "AND SystemID=@SystemID AND FinYear=@FinYear AND CompanyID=@CompanyID ";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);
            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            return ExecuteQuery(sql, pSystemID, pFinYear, pCompanyID);
        }

        /// <summary>
        /// 添加数据到列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int AddMonthlyreportdetailLisr(List<A_TargetPlanDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetInsertSql<A_TargetPlanDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
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


        internal int DeleteTargetPlanDetailList(List<A_TargetPlanDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetDeleteSql<A_TargetPlanDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
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
        /// 获取累计指标汇总数据(经营系统)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailJY(int FinYear, int FinMonth, Guid SystemID, DateTime Date)
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
                                 FROM   dbo.A_TargetPlanDetail
                                 WHERE  FinYear = @FinYear
                                        AND FinMonth <= @FinMonth
                                        AND SystemID = @SystemID
                                        AND IsDeleted = 0
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
                                         FROM   dbo.A_TargetPlanDetail
                                         WHERE  FinYear = @FinYear
                                                AND FinMonth = @FinMonth
                                                AND SystemID = @SystemID
                                                AND IsDeleted = 0
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
            return ExecuteReturnTable(sql, pSystemID, pYear, pMonth, pCurrentDate);
        }

        /// <summary>
        /// 根据TargetPlanID获取A表汇总数据
        /// </summary>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailByTID_A(int FinMonth,Guid TargetPlanID,DateTime Date)
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
                                 FROM   dbo.A_TargetPlanDetail
                                 WHERE  FinMonth <= @FinMonth
                                        AND TargetPlanID = @TargetPlanID
                                        AND IsDeleted = 0
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
                                         FROM   dbo.A_TargetPlanDetail
                                         WHERE  FinMonth = @FinMonth
                                                AND TargetPlanID = @TargetPlanID
                                                AND IsDeleted = 0
                                         GROUP BY TargetID
                                       ) t2 ON t1.ID = t2.TargetID
                    WHERE   t1.IsDeleted = 0
                            AND t1.VersionStart <= @CurrentDate
                            AND @CurrentDate < t1.VersionEnd
                  ) A2 ON A1.TargetID = A2.TargetID
ORDER BY A1.Sequence ASC
";
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Date);
            return ExecuteReturnTable(sql, pTargetPlanID, pMonth, pCurrentDate);
        }


        /// <summary>
        /// 根据TargetPlanID获取汇总数据
        /// </summary>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailByTID(int FinMonth, Guid TargetPlanID, DateTime Date)
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
                                 WHERE  FinMonth <= @FinMonth
                                        AND TargetPlanID = @TargetPlanID
                                        AND IsDeleted = 0
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
                                         WHERE  FinMonth = @FinMonth
                                                AND TargetPlanID = @TargetPlanID
                                                AND IsDeleted = 0
                                         GROUP BY TargetID
                                       ) t2 ON t1.ID = t2.TargetID
                    WHERE   t1.IsDeleted = 0
                            AND t1.VersionStart <= @CurrentDate
                            AND @CurrentDate < t1.VersionEnd
                  ) A2 ON A1.TargetID = A2.TargetID
ORDER BY A1.Sequence ASC
";
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Date);
            return ExecuteReturnTable(sql, pTargetPlanID, pMonth, pCurrentDate);
        }
        /// <summary>
        /// 获取累计指标汇总数据(经营系统)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailPro(int FinYear, int FinMonth, Guid CompanyID, DateTime Date)
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
          FROM      dbo.A_TargetPlanDetail
                    LEFT JOIN dbo.C_Target ON C_Target.ID = A_TargetPlanDetail.TargetID
          WHERE     CompanyID = @CompanyID
                    AND FinYear = @FinYear
                    AND FinMonth = @FinMonth
                    AND C_Target.VersionStart <= @CurrentDate
                    AND @CurrentDate < C_Target.VersionEnd
        ) A1
        LEFT JOIN ( SELECT  TargetID ,
                            SUM(Target) SumTarget
                    FROM    dbo.A_TargetPlanDetail
                            LEFT JOIN dbo.C_Target ON C_Target.ID = A_TargetPlanDetail.TargetID
                    WHERE   CompanyID = @CompanyID
                            AND FinYear = @FinYear
                            AND FinMonth <= @FinMonth
                            AND C_Target.VersionStart <= @CurrentDate
                            AND @CurrentDate < C_Target.VersionEnd
                    GROUP BY TargetID
                  ) t2 ON A1.TargetID = t2.TargetID
ORDER BY A1.Sequence
";
            SqlParameter pSystemID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.String, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Date);
            return ExecuteReturnTable(sql, pSystemID, pYear, pMonth, pCurrentDate);
        }
        /// <summary>
        /// 根绝TargetPlanID获取累计指标汇总数据(经营系统)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailProById(int FinMonth, Guid CompanyID,Guid TargetPlanID,DateTime Time)
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
                    AND FinMonth = @FinMonth
                    AND TargetPlanID = @TargetPlanID
                    AND dbo.C_Target.VersionStart <= @CurrentDate
                    AND @CurrentDate < dbo.C_Target.VersionEnd
        ) A1
        LEFT JOIN ( SELECT  TargetID ,
                            SUM(Target) SumTarget
                    FROM    dbo.B_TargetPlanDetail
                            LEFT JOIN dbo.C_Target ON C_Target.ID = B_TargetPlanDetail.TargetID
                    WHERE   CompanyID = @CompanyID
                            AND TargetPlanID = @TargetPlanID
                            AND FinMonth <= @FinMonth
                            AND dbo.C_Target.VersionStart <= @CurrentDate
                            AND @CurrentDate < dbo.C_Target.VersionEnd
                    GROUP BY TargetID
                  ) t2 ON A1.TargetID = t2.TargetID
ORDER BY A1.Sequence
";
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.String, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Time);
            return ExecuteReturnTable(sql, pTargetPlanID, pMonth, pCompanyID, pCurrentDate);
        }

        /// <summary>
        /// 根绝TargetPlanID获取A表累计指标汇总数据(经营系统)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        internal DataTable GetSumMonthTargetDetailProById_A(int FinMonth, Guid CompanyID, Guid TargetPlanID, DateTime Time)
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
          FROM      dbo.A_TargetPlanDetail
                    LEFT JOIN dbo.C_Target ON C_Target.ID = A_TargetPlanDetail.TargetID
          WHERE     CompanyID = @CompanyID
                    AND FinMonth = @FinMonth
                    AND TargetPlanID = @TargetPlanID
                    AND dbo.C_Target.VersionStart <= @CurrentDate
                    AND @CurrentDate < dbo.C_Target.VersionEnd
        ) A1
        LEFT JOIN ( SELECT  TargetID ,
                            SUM(Target) SumTarget
                    FROM    dbo.A_TargetPlanDetail
                            LEFT JOIN dbo.C_Target ON C_Target.ID = A_TargetPlanDetail.TargetID
                    WHERE   CompanyID = @CompanyID
                            AND TargetPlanID = @TargetPlanID
                            AND FinMonth <= @FinMonth
                            AND dbo.C_Target.VersionStart <= @CurrentDate
                            AND @CurrentDate < dbo.C_Target.VersionEnd
                    GROUP BY TargetID
                  ) t2 ON A1.TargetID = t2.TargetID
ORDER BY A1.Sequence
";
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.String, FinMonth);
            SqlParameter pCurrentDate = CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, Time);
            return ExecuteReturnTable(sql, pTargetPlanID, pMonth, pCompanyID, pCurrentDate);
        }
        /// <summary>
        /// 获取累计指标分解数据(按指标分解)
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        internal DataTable GetSumTargetDetail(int FinYear, Guid SystemID, Guid TargetID)
        {
            string sql = string.Empty;
            sql += @"
SELECT * FROM (
SELECT dbo.C_Company.CompanyName,dbo.C_Company.ID,dbo.C_Company.OpeningTime,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=1 AND dbo.C_Company.ID=CompanyID) AS SumTarget1,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=2 AND dbo.C_Company.ID=CompanyID) AS SumTarget2,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=3 AND dbo.C_Company.ID=CompanyID) AS SumTarget3,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=4 AND dbo.C_Company.ID=CompanyID) AS SumTarget4,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=5 AND dbo.C_Company.ID=CompanyID) AS SumTarget5,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=6 AND dbo.C_Company.ID=CompanyID) AS SumTarget6,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=7 AND dbo.C_Company.ID=CompanyID) AS SumTarget7,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=8 AND dbo.C_Company.ID=CompanyID) AS SumTarget8,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=9 AND dbo.C_Company.ID=CompanyID) AS SumTarget9,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=10 AND dbo.C_Company.ID=CompanyID) AS SumTarget10,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=11 AND dbo.C_Company.ID=CompanyID) AS SumTarget11,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=12 AND dbo.C_Company.ID=CompanyID) AS SumTarget12,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=1 AND dbo.C_Company.ID=CompanyID) AS Target1,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=2 AND dbo.C_Company.ID=CompanyID) AS Target2,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=3 AND dbo.C_Company.ID=CompanyID) AS Target3,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=4 AND dbo.C_Company.ID=CompanyID) AS Target4,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=5 AND dbo.C_Company.ID=CompanyID) AS Target5,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=6 AND dbo.C_Company.ID=CompanyID) AS Target6,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=7 AND dbo.C_Company.ID=CompanyID) AS Target7,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=8 AND dbo.C_Company.ID=CompanyID) AS Target8,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=9 AND dbo.C_Company.ID=CompanyID) AS Target9,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=10 AND dbo.C_Company.ID=CompanyID) AS Target10,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=11 AND dbo.C_Company.ID=CompanyID) AS Target11,
   (SELECT SUM(Target) AS Sumtarget1 FROM dbo.A_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=12 AND dbo.C_Company.ID=CompanyID) AS Target12,Sequence
  FROM dbo.C_Company WHERE C_Company.SystemID=@SystemID AND C_Company.IsDeleted=0 
  ) AS a  WHERE target1 IS NOT NULL AND a.Sequence>=0 ORDER BY a.Sequence
";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, FinYear);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteReturnTable(sql, pSystemID, pYear, pTargetID);
        }

        /// <summary>
        /// 获取累计指标分解数据(按指标分解) 获取B表的数据。
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        internal DataTable GetSumTargetDetail_B(int FinYear, Guid SystemID, Guid TargetID , Guid TargetPlanID)
        {
            string sql = string.Empty;
            sql += @"
SELECT * FROM (
SELECT dbo.C_Company.CompanyName,dbo.C_Company.ID,dbo.C_Company.OpeningTime,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=1 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID ) AS SumTarget1,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=2 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget2,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=3 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget3,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=4 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget4,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=5 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget5,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=6 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget6,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=7 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget7,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=8 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget8,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=9 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget9,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=10 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget10,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=11 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget11,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth<=12 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS SumTarget12,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=1 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target1,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=2 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target2,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=3 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target3,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=4 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target4,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=5 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target5,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=6 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target6,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=7 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target7,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=8 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target8,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=9 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target9,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=10 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target10,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=11 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target11,
   (SELECT SUM(Target) AS Sumtarget1 FROM B_TargetPlanDetail 
   WHERE TargetID=@TargetID AND FinYear=@FinYear AND IsDeleted=0 AND FinMonth=12 AND dbo.C_Company.ID=CompanyID AND TargetPlanID =@TargetPlanID) AS Target12,Sequence
  FROM dbo.C_Company WHERE C_Company.SystemID=@SystemID AND C_Company.IsDeleted=0 
  ) AS a  WHERE target1 IS NOT NULL AND a.Sequence>=0 ORDER BY a.Sequence
";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, FinYear);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            return ExecuteReturnTable(sql, pSystemID, pYear, pTargetID, pTargetPlanID);
        }


        /// <summary>
        /// 获取项目系统小计的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public IList<A_TargetPlanDetail> GetProTargetplan(Guid SystemID, Guid TargetID, int FinYear)
        {
            string sql = @"SELECT  dbo.A_TargetPlanDetail.*
FROM    dbo.C_Company
        LEFT JOIN dbo.A_TargetPlanDetail ON A_TargetPlanDetail.CompanyID = C_Company.ID
WHERE   TargetID = @TargetID
        AND C_Company.SystemID = @SystemID
        AND FinYear = @FinYear
        AND Sequence < 0
		AND A_TargetPlanDetail.IsDeleted=0
		AND C_Company.IsDeleted=0
ORDER BY FinMonth";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pSystemID, pFinYear, pTargetID);
        }



        /// <summary>
        /// 获取公司当年的整个指标总数
        /// </summary>
        public List<V_PlanTargetModel> GetAnnualPlanTarget(Guid TargetPlanID ,int FinYear )
        {
            string sql = string.Empty;
            sql += @" SELECT FinYear ,TargetID ,  CompanyID ,SUM(Target)  AS Target FROM 
					 dbo.B_TargetPlanDetail WHERE TargetPlanID =@TargetPlanID AND FinYear =@FinYear 
					  GROUP BY FinYear ,TargetID ,CompanyID";

            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
           
            DataTable dt =  ExecuteReturnTable(sql,pTargetPlanID, pYear);
            
            List<V_PlanTargetModel> data = new List<V_PlanTargetModel>();

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Rows.Cast<System.Data.DataRow>().ForEach(row =>
                {
                    V_PlanTargetModel item = new V_PlanTargetModel();
                    ORMapping.DataRowToObject(row, item);
                    data.Add(item);
                });
            }
           
            return data;
            
        }

        public int DeletePlanDetail(Guid PlanID)
        {
            string sql = @"DELETE dbo.A_TargetPlanDetail  WHERE TargetPlanID=@PlanID;";
            return ExecuteSql(sql, CreateSqlParameter("@PlanID", System.Data.DbType.Guid, PlanID));
        }
    }
}

