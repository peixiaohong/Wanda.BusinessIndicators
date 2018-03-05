using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.DAL;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.BLL
{
    public class V_HistoryReturnDateOperator : BizOperatorBase<HistoryReturnDateVModel>
    {

        public static readonly V_HistoryReturnDateOperator Instance = PolicyInjection.Create<V_HistoryReturnDateOperator>();

        private static V_HistoryReturnDateAdapter _historyReturnDateAdapter = AdapterFactory.GetAdapter<V_HistoryReturnDateAdapter>();

        public List<HistoryReturnDateVModel> GetList(Guid SystemID, int FinYear)
        {

            List<HistoryReturnDateVModel> list = new List<HistoryReturnDateVModel>();

            list = _historyReturnDateAdapter.GetList(FinYear, SystemID);

            return list;
        }



        protected override BaseAdapterT<HistoryReturnDateVModel> GetAdapter()
        {
            return _historyReturnDateAdapter;
        }
    }



    public class V_ComprehensiveReportFormsOperator : BizOperatorBase<ComprehensiveReportVModel>
    {

        public static readonly V_ComprehensiveReportFormsOperator Instance = PolicyInjection.Create<V_ComprehensiveReportFormsOperator>();

        private static V_ComprehensiveReportDateAdapter _comprehensiveReportDateAdapter = AdapterFactory.GetAdapter<V_ComprehensiveReportDateAdapter>();

        protected override BaseAdapterT<ComprehensiveReportVModel> GetAdapter()
        {
            return _comprehensiveReportDateAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SysIDs">系统ID：用，分割</param>
        /// <param name="FinYears">年：用，分割</param>
        /// <param name="Targets">指标名称：中文，用，分割</param>
        /// <param name="DataType">数据类型：实际数，指标数，完成率 ，差额</param>
        /// <param name="IsCurrent">当月数，累计数</param>
        /// <returns></returns>
        public List<ComprehensiveReportVModel> GetList(string SysIDs, string FinYears, string Targets, string DataType, string IsCurrent, int CurrentYear)
        {

            string DataTypeStr = string.Empty;
            string IsMonthlyReport = "MonthlyReport";

            if (IsCurrent == "1") //当月数
            {
                if (DataType == "1")
                {
                    DataTypeStr = "NActualAmmount";
                }
                else if (DataType == "2")
                {
                    DataTypeStr = "NPlanAmmount";
                    IsMonthlyReport = string.Empty;
                }
                else if (DataType == "3")
                {
                    DataTypeStr = "IsMissTargetCurrent";
                }
                else if (DataType == "4")
                {
                    DataTypeStr = "Current";
                }
                else
                {
                    DataTypeStr = "NActualAmmount";
                }
                
            }
            else // 累计
            {
                if (DataType == "1")
                {
                    DataTypeStr = "NAccumulativeActualAmmount"; // 上报数
                }
                else if (DataType == "2")
                {
                    DataTypeStr = "NAccumulativePlanAmmount"; // 指标数
                    IsMonthlyReport = string.Empty;
                }
                else if (DataType == "3")
                {
                    DataTypeStr = "IsMissTarget"; // 未完成成数
                }
                else if (DataType == "4")
                {
                    DataTypeStr = "Accumulative"; // 上报数和指标数
                }
                else
                {
                    DataTypeStr = "NAccumulativeActualAmmount";
                }
                
            }

            List<ComprehensiveReportVModel> list = new List<ComprehensiveReportVModel>();
            list = _comprehensiveReportDateAdapter.GetComprehensiveReportForms(SysIDs, FinYears, Targets, DataTypeStr, IsMonthlyReport, CurrentYear);
            
             var GList = list.GroupBy(G => G.SystemID).Select(p => new ComprehensiveReportGroupModel { SystemID = p.Key, sys_count=p.Count()}) ; // 分组获取数量

            var TList = list.GroupBy(G => new {G.SystemID, G.TargetID}).Select(p => new ComprehensiveReportGroupModel { SystemID = p.FirstOrDefault().TargetID , sys_count = p.Count() }); // 分组获取数量

            GList.ForEach(p => {
                list.Where(c => c.SystemID == p.SystemID).OrderBy(s => s.T_Sequence).First().SrowSpan_str = p.sys_count;
            });


            TList.ForEach(T =>
            {
                list.Where(c => c.TargetID == T.SystemID).OrderBy(s => s.FinYear).First().TrowSpan_str = T.sys_count;
            });
            
            return list;
        }

        /// <summary>
        /// 万达电影公司
        /// </summary>
        /// <param name="Year">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="DataType">“_A”</param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetFilmCompany(int Year, int FinMonth, string DataType)
        {
            var RT = C_ReportTimeOperator.Instance.GetReportTime();
            if (RT != null && RT.ReportTime.Value.Month == FinMonth && RT.ReportTime.Value.Year == Year)
            {
                //如果是当前月，去B表中获取数据，否则去A表中获取  //B表获取数据
                return _comprehensiveReportDateAdapter.GetFilmCompany(Year, FinMonth, " ");

            }
            else
            {
                //A表获取数据
                return _comprehensiveReportDateAdapter.GetFilmCompany(Year, FinMonth, "_A");
            }
        }


        /// <summary>
        /// 儿童娱乐公司
        /// </summary>
        /// <param name="Year">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="DataType">“_A”</param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetChildrenCompany(int Year, int FinMonth, string DataType)
        {

            var RT = C_ReportTimeOperator.Instance.GetReportTime();
            if (RT != null && RT.ReportTime.Value.Month == FinMonth && RT.ReportTime.Value.Year == Year)
            {
                //如果是当前月，去B表中获取数据，否则去A表中获取  //B表获取数据
                return _comprehensiveReportDateAdapter.GetChildrenCompany(Year, FinMonth, " ");

            }
            else
            {
                //A表获取数据
                return _comprehensiveReportDateAdapter.GetChildrenCompany(Year, FinMonth, "_A");
            }
            
        }

        /// <summary>
        /// 获取万达商业
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetWandaBusiness(int Year, int FinMonth, string DataType)
        {
            var RT = C_ReportTimeOperator.Instance.GetReportTime();
            if (RT != null && RT.ReportTime.Value.Month == FinMonth && RT.ReportTime.Value.Year == Year)
            {
                //如果是当前月，去B表中获取数据，否则去A表中获取  //B表获取数据
                return _comprehensiveReportDateAdapter.GetWandaBusiness_B(Year, FinMonth, DataType);

            }
            else
            {
                //A表获取数据
                return _comprehensiveReportDateAdapter.GetWandaBusiness_A(Year, FinMonth, DataType);
            }

        }

        /// <summary>
        /// 文化集团
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="FinMonth"></param>
       
        /// <returns></returns>
        public List<MonthReportSummaryViewModel> GetWandaCulture(int Year, int FinMonth)
        {
            var RT = C_ReportTimeOperator.Instance.GetReportTime();
            if (RT != null && RT.ReportTime.Value.Month == FinMonth && RT.ReportTime.Value.Year == Year)
            {
                //如果是当前月，去B表中获取数据，否则去A表中获取  //B表获取数据
                return _comprehensiveReportDateAdapter.GetWandaCulture_B(Year, FinMonth);

            }
            else
            {
                //A表获取数据
                return _comprehensiveReportDateAdapter.GetWandaCulture_A(Year, FinMonth);
            }

        }

    }
}
