
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;
using System.Linq;
using Lib.Data;
using Wanda.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data;
using Wanda.BusinessIndicators.ViewModel;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// Monthlyreportdetail对象的数据访问适配器
    /// </summary>
    sealed class B_MonthlyreportdetailAdapter : AppBaseAdapterT<B_MonthlyReportDetail>
    {

        public IList<B_MonthlyReportDetail> GetMonthlyreportdetailList()
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        




        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int UpdateMonthlyreportdetailLisr(List<B_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                  
                    sb.Append(ORMapping.GetUpdateSql<B_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
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
        internal int AddMonthlyreportdetailLisr(List<B_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetInsertSql<B_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
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

        internal IList<B_MonthlyReportDetail> GetMonthlyreportdetailList(Guid MonthlyReportID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND MonthlyReportID=@MonthlyReportID";

            SqlParameter pMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);

            return ExecuteQuery(sql, pMonthlyReportID);
        }


        /// <summary>
        /// 判断明细表中是否含有数据
        /// </summary>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        internal bool GetMonthlyReportDetailCount(Guid MonthlyReportID)
        {

            string sql = @" SELECT  TOP 10 * FROM  dbo.B_MonthlyReportDetail "; //WHERE MonthlyReportID='291E72F7-3176-496B-99C0-D6A15F8F7795'

            sql += " WHERE " + base.NotDeleted;
            sql += " AND MonthlyReportID=@MonthlyReportID";
            SqlParameter pTMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTMonthlyReportID);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

       /// <summary>
       /// A表获取数据，复制到B表
       /// </summary>
       /// <param name="MonthlyReportID"></param>
       /// <returns></returns>
        internal List<B_MonthlyReportDetail> GetMonthlyReportDetail_ByAToB( int FinYear , int FinMonth ,Guid SystemID , Guid MonthlyReportID)
        {
            string sql = "GetMonthlyReportDetail_ByAToB ";
            
            SqlParameter p1 =  CreateSqlParameter("@FinYear", DbType.Int32 , FinYear);
            SqlParameter p2 =  CreateSqlParameter("@FinMonth", DbType.Int32, FinMonth);
            SqlParameter p3 =  CreateSqlParameter("@SystemID", DbType.Guid, SystemID);
            SqlParameter p4 =  CreateSqlParameter("@MonthlyReportID", DbType.Guid, MonthlyReportID);
            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3, p4);
   
            List<B_MonthlyReportDetail> data = new List<B_MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                B_MonthlyReportDetail item = new B_MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;
        }

        /// <summary>
        /// 从B表的审批中数据，复制一份出来
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        internal List<B_MonthlyReportDetail> GetMonthlyReportDetail_ByBToB(int FinYear, int FinMonth, Guid SystemID, Guid OldMonthlyReportID  , Guid NewMonthlyReportID)
        {

            string sql = "GetMonthlyReportDetail_ByBToB ";
            SqlParameter p1 =  CreateSqlParameter("@FinYear", DbType.Int32 , FinYear);
            SqlParameter p2 =  CreateSqlParameter("@FinMonth", DbType.Int32, FinMonth);
            SqlParameter p3 =  CreateSqlParameter("@SystemID", DbType.Guid, SystemID);
            SqlParameter p4 =  CreateSqlParameter("@OldMonthlyReportID", DbType.Guid, OldMonthlyReportID);
            SqlParameter p5 = CreateSqlParameter("@NewMonthlyReportID", DbType.Guid, NewMonthlyReportID);

            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3, p4, p5);

            List<B_MonthlyReportDetail> data = new List<B_MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                B_MonthlyReportDetail item = new B_MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;

        }



        internal IList<MonthlyReportVM> GetBVMonthlyReport(Guid MonthlyReportID)
        {
            string sql = @"
SELECT  TargetID ,
        B_MonthlyReportDetail.SystemID ,
        CompanyID ,
        CompanyName ,
        B_MonthlyReportDetail.CompanyProperty1 ,
        IsMissTarget ,
        Counter ,
        IsMissTargetCurrent ,
        NAccumulativeActualAmmount
FROM    dbo.B_MonthlyReportDetail
        LEFT JOIN dbo.C_Company ON C_Company.ID = B_MonthlyReportDetail.CompanyID
WHERE   MonthlyReportID = @MonthlyReportID
        AND B_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0";
            SqlParameter pTMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTMonthlyReportID);
            List<MonthlyReportVM> data = new List<MonthlyReportVM>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportVM item = new MonthlyReportVM();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }



        internal List<MonthlyReportDetail> GetMonthlyReportDetailList(Guid MonthlyReportID)
        {


            string sql = "GetMonthlyReportDetailList ";
            SqlParameter p1 = new SqlParameter("@MonthlyReportID", MonthlyReportID);
            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1);
          
            List<MonthlyReportDetail> data = new List<MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportDetail item = new MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;

        }

        /// <summary>
        /// 万达电影固定报表，国内影城-票房收入指标查询
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<B_MonthlyReportDetail> GetMonthTargetDetailCNPF(Guid SystemID, int FinYear, int FinMonth, Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth AND TargetID=@TargetID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int64, FinMonth);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);

            return ExecuteQuery(sql, pSystemID, pFinYear, pFinMonth, pTargetID);
        }


    }
}

