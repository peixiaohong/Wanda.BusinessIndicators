using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.ViewModel.MonthReport;
using System.Data.Common;
using System.Reflection;

namespace LJTH.BusinessIndicators.DAL
{
    class V_HistoryReturnDateAdapter : AppBaseAdapterT<HistoryReturnDateVModel>
    {
        public static V_HistoryReturnDateAdapter Instance = new V_HistoryReturnDateAdapter();


        /// <summary>
        /// 获取历史要求补回时间数据
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public List<HistoryReturnDateVModel> GetList(int FinYear, Guid SystemID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT  A.CompanyID  ,dbo.C_Company.CompanyName ,A.SystemID ,A.FinYear ,A.TargetID ,C_Target.TargetName                  ");
            sb.Append(" ,B.FinMonth AS 'January', B.CurrentMonthCommitDate AS 'January_CommitDate' ,B.IsMissTarget AS 'January_IsMissTarget'     ");
            sb.Append(" ,C.FinMonth AS 'February', C.CurrentMonthCommitDate AS 'February_CommitDate' ,C.IsMissTarget AS 'February_IsMissTarget'  ");
            sb.Append(" ,D.FinMonth AS 'March', D.CurrentMonthCommitDate AS 'March_CommitDate',D.IsMissTarget AS 'March_IsMissTarget'            ");
            sb.Append(" ,E.FinMonth AS 'April', E.CurrentMonthCommitDate AS 'April_CommitDate',E.IsMissTarget AS 'April_IsMissTarget'            ");
            sb.Append(" ,F.FinMonth AS 'May', F.CurrentMonthCommitDate AS 'May_CommitDate',F.IsMissTarget AS 'May_IsMissTarget'                  ");
            sb.Append(" ,G.FinMonth AS 'June', G.CurrentMonthCommitDate AS 'June_CommitDate',G.IsMissTarget AS 'June_IsMissTarget'               ");
            sb.Append(" ,H.FinMonth AS 'July', H.CurrentMonthCommitDate AS 'July_CommitDate',H.IsMissTarget AS 'July_IsMissTarget'               ");
            sb.Append(" ,J.FinMonth AS 'August', J.CurrentMonthCommitDate AS 'August_CommitDate',J.IsMissTarget AS 'August_IsMissTarget'         ");
            sb.Append(" ,K.FinMonth AS 'September', K.CurrentMonthCommitDate AS 'September_CommitDate',K.IsMissTarget AS 'September_IsMissTarget'");
            sb.Append(" ,M.FinMonth AS 'October', M.CurrentMonthCommitDate AS 'October_CommitDate',M.IsMissTarget AS 'October_IsMissTarget'      ");
            sb.Append(" ,N.FinMonth AS 'November', N.CurrentMonthCommitDate AS 'November_CommitDate',N.IsMissTarget AS 'November_IsMissTarget'   ");
            sb.Append("   FROM                                                                                                                   ");
            sb.Append(" (	SELECT SystemID, CompanyID ,TargetID, MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,  MAX(FinYear) AS FinYear,  MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth < 12 AND FinYear =@FinYear   AND IsDeleted =0 GROUP BY CompanyID ,SystemID ,TargetID  ) AS A           ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID ,TargetID, MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   , MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 1 AND FinYear =@FinYear  AND IsDeleted =0  GROUP BY CompanyID,SystemID ,TargetID ) AS B                ");
            sb.Append(" ON  A.CompanyID = B.CompanyID  AND A.TargetID = B.TargetID                                                                                                                                                                                                                                                                                                                                                                             ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID ,TargetID,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 2 AND FinYear =@FinYear  AND IsDeleted =0  GROUP BY CompanyID ,SystemID,TargetID ) AS C                  ");
            sb.Append(" ON  A.CompanyID = C.CompanyID AND A.TargetID = C.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID,TargetID ,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 3 AND FinYear =@FinYear AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID,SystemID,TargetID  ) AS D ");
            sb.Append(" ON  A.CompanyID = D.CompanyID AND A.TargetID = D.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID ,TargetID,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 4 AND FinYear =@FinYear   AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID ) AS E                  ");
            sb.Append(" ON  A.CompanyID = E.CompanyID AND A.TargetID = E.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID,CompanyID,TargetID ,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 5 AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID ) AS F                    ");
            sb.Append(" ON  A.CompanyID = F.CompanyID AND A.TargetID = F.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID ,TargetID,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 6 AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID ) AS G                   ");
            sb.Append(" ON  A.CompanyID = G.CompanyID AND A.TargetID = G.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID ,TargetID,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 7 AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID ) AS H                   ");
            sb.Append(" ON  A.CompanyID = H.CompanyID AND A.TargetID = H.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID, CompanyID,TargetID ,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 8 AND FinYear =@FinYear   AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID  ) AS J                 ");
            sb.Append(" ON  A.CompanyID = J.CompanyID AND A.TargetID = J.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT  SystemID,CompanyID,TargetID ,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 9 AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID,SystemID ,TargetID ) AS K                   ");
            sb.Append(" ON  A.CompanyID = K.CompanyID AND A.TargetID = K.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT  SystemID,CompanyID,TargetID ,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 10 AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID ) AS M                  ");
            sb.Append(" ON  A.CompanyID = M.CompanyID AND A.TargetID = M.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN                                                                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" (	SELECT SystemID,CompanyID,TargetID ,MAX( CAST(IsMissTarget AS INT) )AS IsMissTarget   ,MAX(FinYear) AS FinYear, MAX(CurrentMonthCommitDate) AS CurrentMonthCommitDate , MAX(FinMonth) AS FinMonth ,COUNT(1) AS 数量   FROM  dbo.A_MonthlyReportDetail  WHERE SystemID =@SystemID AND FinMonth = 11 AND FinYear =@FinYear  AND IsDeleted =0 GROUP BY CompanyID ,SystemID,TargetID ) AS N                   ");
            sb.Append(" ON  A.CompanyID = N.CompanyID AND A.TargetID = N.TargetID                                                                                                                                                                                                                                                                                                                                                                              ");
            sb.Append(" LEFT JOIN dbo.C_Company ON A.CompanyID =dbo.C_Company.ID                                                                                                                                                                                                                                                                                                                                                                               ");
            sb.Append(" LEFT JOIN C_Target ON C_Target.ID =A.TargetID WHERE dbo.C_Target.NeedEvaluation =1 and A.IsMissTarget =1 AND C_Target.IsDeleted =0  ORDER BY  C_Company.Sequence ,C_Target.Sequence  ");

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);

            var ds = DbHelper.RunSqlReturnDS(sb.ToString(), ConnectionName, pSystemID, pFinYear);


            List<HistoryReturnDateVModel> data = new List<HistoryReturnDateVModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                HistoryReturnDateVModel item = new HistoryReturnDateVModel();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;
        }









    }


    class V_ComprehensiveReportDateAdapter : AppBaseAdapterT<ComprehensiveReportVModel>
    {
        public static V_HistoryReturnDateAdapter Instance = new V_HistoryReturnDateAdapter();



        /// <summary>
        /// 综合报表查询
        /// </summary>
        /// <param name="SysIDs">系统ID串 ， 用‘，’分割</param>
        /// <param name="FinYears">选择的年份，用‘，’分割</param>
        /// <param name="Targets">选择的指标，用‘，’分割</param>
        /// <param name="DataType">数据的类型 </param>
        /// <param name="IsMonthlyReport">是否是月报，或者指标的数据  参数：MonthlyReport ，‘’</param>
        /// <param name="CurrentYear">当前年</param>
        /// <returns></returns>
        public List<ComprehensiveReportVModel> GetComprehensiveReportForms(string SysIDs, string FinYears, string Targets, string DataType, string IsMonthlyReport, int CurrentYear)
        {


            string sql = "GetComprehensiveReportFormsList ";
            SqlParameter p1 = new SqlParameter("@SysIDs", SysIDs);
            SqlParameter p2 = new SqlParameter("@FinYears", FinYears);
            SqlParameter p3 = new SqlParameter("@Targets", Targets);
            SqlParameter p4 = new SqlParameter("@DataType", DataType);
            SqlParameter p5 = new SqlParameter("@IsMonthlyReport", IsMonthlyReport);
            SqlParameter p6 = new SqlParameter("@CurrentYear", CurrentYear);

            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3, p4, p5, p6);

            List<ComprehensiveReportVModel> data = new List<ComprehensiveReportVModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                ComprehensiveReportVModel item = new ComprehensiveReportVModel();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;


        }
        /// <summary>
        /// 万达电影公司。
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetFilmCompany(int Year, int FinMonth, string DataType)
        {
            string sql = "GetFilmCompany ";
            SqlParameter p1 = new SqlParameter("@FinYear", Year);
            SqlParameter p2 = new SqlParameter("@FinMonth", FinMonth);
            SqlParameter p3 = new SqlParameter("@DataType", DataType);

            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3);
            List<MonthReportSummaryViewModel> data = new List<MonthReportSummaryViewModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthReportSummaryViewModel item = new MonthReportSummaryViewModel();
                item.TargetName = row["TargetName"].ToString();
                item.SystemName = row["SystemName"].ToString();
                item.ID = int.Parse(row["Sequence"].ToString());
                item.MeasureRate = row["_Target"].ToString();
                item.SystemID = Guid.Parse(row["SystemID"].ToString());
                item.TargetID = Guid.Parse(row["TargetID"].ToString());
                item.FinYear = int.Parse(row["FinYear"].ToString());
                item.FinMonth = int.Parse(row["FinMonth"].ToString());
                item.NPlanAmmount = double.Parse(row["NPlanAmmount"].ToString());
                item.NActualAmmount = double.Parse(row["NActualAmmount"].ToString());
                item.NDifference = double.Parse(row["NDifference"].ToString());
                item.NAccumulativePlanAmmount = double.Parse(row["NAccumulativePlanAmmount"].ToString());
                item.NAccumulativeActualAmmount = double.Parse(row["NAccumulativeActualAmmount"].ToString());
                item.NAccumulativeDifference = double.Parse(row["NAccumulativeDifference"].ToString());

                data.Add(item);
            });

            return data;

        }

        /// <summary>
        /// 儿童娱乐公司
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetChildrenCompany(int Year, int FinMonth, string DataType)
        {
            string sql = "GetChildrenCompany ";
            SqlParameter p1 = new SqlParameter("@FinYear", Year);
            SqlParameter p2 = new SqlParameter("@FinMonth", FinMonth);
            SqlParameter p3 = new SqlParameter("@DataType", DataType);

            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3);
            List<MonthReportSummaryViewModel> data = new List<MonthReportSummaryViewModel>();

            if (ds.Tables[0].Rows.Count > 3)
            {
                ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
                {
                    MonthReportSummaryViewModel item = new MonthReportSummaryViewModel();
                    item.TargetName = row["TargetName"].ToString();
                    item.SystemName = row["SystemName"].ToString();
                    item.ID = int.Parse(row["Sequence"].ToString());
                    item.MeasureRate = row["_Target"].ToString();
                    item.SystemID = Guid.Parse(row["SystemID"].ToString());
                    item.TargetID = Guid.Parse(row["TargetID"].ToString());
                    item.FinYear = int.Parse(row["FinYear"].ToString());
                    item.FinMonth = int.Parse(row["FinMonth"].ToString());
                    item.NPlanAmmount = double.Parse(row["NPlanAmmount"].ToString());
                    item.NActualAmmount = double.Parse(row["NActualAmmount"].ToString());
                    item.NDifference = double.Parse(row["NDifference"].ToString());
                    item.NAccumulativePlanAmmount = double.Parse(row["NAccumulativePlanAmmount"].ToString());
                    item.NAccumulativeActualAmmount = double.Parse(row["NAccumulativeActualAmmount"].ToString());
                    item.NAccumulativeDifference = double.Parse(row["NAccumulativeDifference"].ToString());

                    data.Add(item);
                });
            }
            return data;
        }


        /// <summary>
        /// 万达商业
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <param name="DataType">暂时没有用</param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetWandaBusiness_A(int Year, int FinMonth, string DataType)
        {
            string sql = "GetWandaBusiness_A ";
            SqlParameter p1 = new SqlParameter("@FinYear", Year);
            SqlParameter p2 = new SqlParameter("@FinMonth", FinMonth);
            SqlParameter p3 = new SqlParameter("@DataType", " ");

            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3);

            List<MonthReportSummaryViewModel> data = new List<MonthReportSummaryViewModel>();

            if (ds.Tables[0].Rows.Count > 3)
            {
                ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthReportSummaryViewModel item = new MonthReportSummaryViewModel();

                item.TargetName = row["TargetName"].ToString();
                item.SystemName = row["SystemName"].ToString();
                item.ID = int.Parse(row["Sequence"].ToString());
                item.MeasureRate = row["_Target"].ToString();
                item.SystemID = Guid.Parse(row["SystemID"].ToString());
                item.TargetID = Guid.Parse(row["TargetID"].ToString());
                item.FinYear = int.Parse(row["FinYear"].ToString());
                item.FinMonth = int.Parse(row["FinMonth"].ToString());
                item.NPlanAmmount = double.Parse(row["NPlanAmmount"].ToString());
                item.NActualAmmount = double.Parse(row["NActualAmmount"].ToString());
                item.NDifference = double.Parse(row["NDifference"].ToString());
                item.NAccumulativePlanAmmount = double.Parse(row["NAccumulativePlanAmmount"].ToString());
                item.NAccumulativeActualAmmount = double.Parse(row["NAccumulativeActualAmmount"].ToString());
                item.NAccumulativeDifference = double.Parse(row["NAccumulativeDifference"].ToString());


                data.Add(item);
            });
            }
            return data;

        }


        /// <summary>
        /// 万达商业
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <param name="DataType">暂时没有用</param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetWandaBusiness_B(int Year, int FinMonth, string DataType)
        {
            string sql = "GetWandaBusiness_B ";
            SqlParameter p1 = new SqlParameter("@FinYear", Year);
            SqlParameter p2 = new SqlParameter("@FinMonth", FinMonth);
            SqlParameter p3 = new SqlParameter("@DataType", " ");

            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3);

            List<MonthReportSummaryViewModel> data = new List<MonthReportSummaryViewModel>();

            if (ds.Tables[0].Rows.Count > 3)
            {
                ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthReportSummaryViewModel item = new MonthReportSummaryViewModel();

                item.TargetName = row["TargetName"].ToString();
                item.SystemName = row["SystemName"].ToString();
                item.ID = int.Parse(row["Sequence"].ToString());
                item.MeasureRate = row["_Target"].ToString();
                item.SystemID = Guid.Parse(row["SystemID"].ToString());
                item.TargetID = Guid.Parse(row["TargetID"].ToString());
                item.FinYear = int.Parse(row["FinYear"].ToString());
                item.FinMonth = int.Parse(row["FinMonth"].ToString());
                item.NPlanAmmount = double.Parse(row["NPlanAmmount"].ToString());
                item.NActualAmmount = double.Parse(row["NActualAmmount"].ToString());
                item.NDifference = double.Parse(row["NDifference"].ToString());
                item.NAccumulativePlanAmmount = double.Parse(row["NAccumulativePlanAmmount"].ToString());
                item.NAccumulativeActualAmmount = double.Parse(row["NAccumulativeActualAmmount"].ToString());
                item.NAccumulativeDifference = double.Parse(row["NAccumulativeDifference"].ToString());

                data.Add(item);
            });
            }
            return data;

        }


        /// <summary>
        /// 文化集团 B表数据
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetWandaCulture_B(int Year, int FinMonth)
        {
            string sql = "GetWandaCulture_B ";
            SqlParameter p1 = new SqlParameter("@FinYear", Year);
            SqlParameter p2 = new SqlParameter("@FinMonth", FinMonth);


            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2);

            List<MonthReportSummaryViewModel> data = new List<MonthReportSummaryViewModel>();

            if (ds.Tables[0].Rows.Count > 3)
            {
                ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthReportSummaryViewModel item = new MonthReportSummaryViewModel();

                item.TargetName = row["TargetName"].ToString();
                item.SystemName = row["SystemName"].ToString();
                item.ID = int.Parse(row["Sequence"].ToString());
                item.MeasureRate = row["_Target"].ToString();

                if (string.IsNullOrEmpty(row["SystemID"].ToString()))
                {
                    item.SystemID = Guid.Empty;
                    return;
                }
                else {
                    item.SystemID = Guid.Parse(row["SystemID"].ToString());
                }

                if (string.IsNullOrEmpty(row["TargetID"].ToString()))
                {
                    item.TargetID = Guid.Empty;
                    return;
                }
                else
                {
                    item.TargetID = Guid.Parse(row["TargetID"].ToString());
                }

                if (string.IsNullOrEmpty(row["FinYear"].ToString()))
                {
                    item.FinYear = 0;
                }
                else
                {
                    item.FinYear = int.Parse(row["FinYear"].ToString());
                }

                if (string.IsNullOrEmpty(row["FinMonth"].ToString()))
                {
                    item.FinMonth = 0;
                }
                else
                {
                    item.FinMonth = int.Parse(row["FinMonth"].ToString());
                }

                if (string.IsNullOrEmpty(row["NPlanAmmount"].ToString()))
                {
                    item.NPlanAmmount = 0;
                }
                else
                {
                    item.NPlanAmmount = double.Parse(row["NPlanAmmount"].ToString());
                }

                if (string.IsNullOrEmpty(row["NActualAmmount"].ToString()))
                {
                    item.NActualAmmount = 0;
                }
                else
                {
                    item.NActualAmmount = double.Parse(row["NActualAmmount"].ToString());
                }

                

                if (string.IsNullOrEmpty(row["NDifference"].ToString()))
                {
                    item.NDifference = 0;
                }
                else
                {
                    item.NDifference = double.Parse(row["NDifference"].ToString());
                  
                }


                if (string.IsNullOrEmpty(row["NAccumulativePlanAmmount"].ToString()))
                {
                    item.NAccumulativePlanAmmount = 0;
                }
                else
                {
                    item.NAccumulativePlanAmmount = double.Parse(row["NAccumulativePlanAmmount"].ToString());
                }


                if (string.IsNullOrEmpty(row["NAccumulativeActualAmmount"].ToString()))
                {
                    item.NAccumulativeActualAmmount = 0;
                }
                else
                {
                    item.NAccumulativeActualAmmount = double.Parse(row["NAccumulativeActualAmmount"].ToString());
                }


                if (string.IsNullOrEmpty(row["NAccumulativeDifference"].ToString()))
                {
                    item.NAccumulativeDifference = 0;
                }
                else
                {
                    item.NAccumulativeDifference = double.Parse(row["NAccumulativeDifference"].ToString());
                }
                
                data.Add(item);
            });
            }

            return data;

        }


        /// <summary>
        /// 文化集团 A表数据
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetWandaCulture_A(int Year, int FinMonth)
        {
            string sql = "GetWandaCulture_A ";
            SqlParameter p1 = new SqlParameter("@FinYear", Year);
            SqlParameter p2 = new SqlParameter("@FinMonth", FinMonth);


            var ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2);

            List<MonthReportSummaryViewModel> data = new List<MonthReportSummaryViewModel>();

            if (ds.Tables[0].Rows.Count > 3)
            {
                ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthReportSummaryViewModel item = new MonthReportSummaryViewModel();

                item.TargetName = row["TargetName"].ToString();
                item.SystemName = row["SystemName"].ToString();
                item.ID = int.Parse(row["Sequence"].ToString());
                item.MeasureRate = row["_Target"].ToString();
                item.SystemID = Guid.Parse(row["SystemID"].ToString());
                item.TargetID = Guid.Parse(row["TargetID"].ToString());
                item.FinYear = int.Parse(row["FinYear"].ToString());
                item.FinMonth = int.Parse(row["FinMonth"].ToString());
                item.NPlanAmmount = double.Parse(row["NPlanAmmount"].ToString());
                item.NActualAmmount = double.Parse(row["NActualAmmount"].ToString());
                item.NDifference = double.Parse(row["NDifference"].ToString());
                item.NAccumulativePlanAmmount = double.Parse(row["NAccumulativePlanAmmount"].ToString());
                item.NAccumulativeActualAmmount = double.Parse(row["NAccumulativeActualAmmount"].ToString());
                item.NAccumulativeDifference = double.Parse(row["NAccumulativeDifference"].ToString());

                data.Add(item);
            });
            }
            return data;

        }



        /// <summary>
        /// 获取综合列表数据（新)
        /// </summary>
        /// <param name="systemID">板块ID</param>
        /// <param name="finYear">年</param>
        /// <param name="finMonth">月</param>
        /// <param name="loginName">登陆人</param>
        /// <returns></returns>
        public List<ComprehensiveReportViewModel> GetComprehensiveReportData(string systemID,int finYear,int finMonth,string loginName)
        {
            string sql = string.Format("Exec [dbo].[Pro_SystemReport] @SystemID,@Years,@Month,@LoginName");

            DbParameter[] parameters = new DbParameter[]{
                CreateSqlParameter("@SystemID",DbType.String,systemID),
                CreateSqlParameter("@Years",DbType.Int32,finYear),
                CreateSqlParameter("@Month",DbType.Int32,finMonth),
                CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            DataSet ds = ExecuteReturnDataSet(sql, parameters);
            List<ComprehensiveReportViewModel> list = new List<ComprehensiveReportViewModel>();
            if (ds != null)
            {
                DataTable returnTable = ds.Tables[0];
                string tempName = string.Empty;
                if (returnTable != null)
                {
                    foreach (DataRow dr in returnTable.Rows)
                    {
                        ComprehensiveReportViewModel obj = new ComprehensiveReportViewModel();
                        System.Reflection.PropertyInfo[] propertys = obj.GetType().GetProperties();
                        foreach (PropertyInfo pi in propertys)
                        {
                            tempName = pi.Name;
                            if (returnTable.Columns.Contains(tempName))
                            {
                                if (!pi.CanWrite) continue;
                                object value = dr[tempName];
                                if (value != DBNull.Value)
                                {
                                    pi.SetValue(obj, value, null);
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }
            }
            return list;
        }

    }

}
