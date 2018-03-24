using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    class TargetApproveController : BaseController
    {
        #region 变量

        List<MonthlyReportDetail> ReturnDataList = new List<MonthlyReportDetail>();

        List<MonthlyReportDetail> ReturnMissTargetDataList = new List<MonthlyReportDetail>();


        #endregion

        /// <summary>
        /// 补回说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetTargetReturnList(string rpts)
        {
           ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

            List<DictionaryVmodel> ReturnData = new List<DictionaryVmodel>();

            if (rpt._MonthReportID != Guid.Empty)
            {
                B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();
                try
                {
                    B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(rpt._MonthReportID);
                }
                catch (Exception)
                {
                    B_JsonData = null;
                }

                //获取 表中的JSon数据
                if (B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerryReturnJsonData))
                {
                    ReturnData = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerryReturnJsonData);
                }
                else
                {
                    C_System sys;
                    if (rpt.ReportDetails != null && rpt.ReportDetails.Count() > 0)
                    {
                        sys = StaticResource.Instance[rpt._System.ID, rpt.ReportDetails[0].CreateTime];
                    }
                    else
                    {
                        sys = StaticResource.Instance[rpt._System.ID, DateTime.Now];
                    }

                    ReturnData = ReportInstanceReturnEngine.ReportInstanceReturnService.GetReturnRptDataSource(rpt, sys); //rpt.GetReturnRptDataSource();
                }
            }
            return ReturnData;
        }
      
        /// <summary>
        /// 未完成说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetMissTargetList(string rpts)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            List<DictionaryVmodel> MissTargetData = new List<DictionaryVmodel>();
            if (rpt._MonthReportID != Guid.Empty)
            {
                B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();
                try
                {
                    B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(rpt._MonthReportID);
                }
                catch (Exception)
                {
                    B_JsonData = null;
                }

                //获取 表中的JSon数据
                if (B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerryMissJsonData))
                {
                    MissTargetData = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerryMissJsonData);
                }
                else
                {

                    MissTargetData = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                }
            }
            return MissTargetData;
        }
        /// <summary>
        /// 获取月报说明，附件，月度经营报告
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetReportInstance(string strMonthReportID)
        {
            ReportInstance rpt = new ReportInstance(strMonthReportID.ToGuid(), true);
            List<DictionaryVmodel> listSRDS = new List<DictionaryVmodel>();
            
            B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();
            
            listSRDS = GetJsonData(strMonthReportID.ToGuid());

            if (listSRDS != null)
            {
                return listSRDS;
            }
            else
            {
                rpt =  new ReportInstance(strMonthReportID.ToGuid(), true);
                listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                return listSRDS;
            }
        }


        private List<DictionaryVmodel> GetJsonData(Guid MonthlyReportID)
        {
            List<DictionaryVmodel> listSRDS = null;

            B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();

            try
            {
                B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(MonthlyReportID);
            }
            catch (Exception)
            {
                B_JsonData = null;
            }

            //获取 表中的JSon数据
            if (B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerrySumJsonData))
            {
                listSRDS = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerrySumJsonData);
            }

            return listSRDS;
        }




        /// <summary>
        /// 项目公司
        /// </summary>
        /// <param name="strMonthReportID"></param>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetProReportInstance(string strMonthReportID, string strProType, string strSystemID, string strBusinessID)
        {
            ReportInstance rpt = null;
            if (string.IsNullOrEmpty(strProType))
            {
                rpt = new ReportInstance(strMonthReportID.ToGuid(), true);
            }
            else
            {
                B_SystemBatch _BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(strBusinessID.ToGuid());
               
                if (_BatchModel != null)
                {
                    List<V_SubReport> rptList = JsonHelper.Deserialize<List<V_SubReport>>(_BatchModel.SubReport);

                    V_SubReport rptModel = rptList.Find(p => p.SystemID == strSystemID.ToGuid());

                    rpt = new ReportInstance(rptModel.ReportID, true);
                }
            
            }


            List<DictionaryVmodel> listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
            return listSRDS;
        }


        /// <summary>
        /// 完成情况明细
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="IsLatestVersion">包含审批中</param>
        /// <returns>ist<MonthReportSummaryViewModel></returns>
        [LibAction]
        public List<DictionaryVmodel> GetDetailRptDataSource(string rpts, string strCompanyProperty, string strMonthReportOrderType, bool IncludeHaveDetail)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            List<DictionaryVmodel> AllData = new List<DictionaryVmodel>();

            if (rpt._MonthReportID != Guid.Empty)
            {
                B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();
                try
                {
                    B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(rpt._MonthReportID);
                }
                catch (Exception)
                {
                    B_JsonData = null;
                }

                //获取 表中的JSon数据
                if (B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerryDetaileJsonData))
                {
                    AllData = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerryDetaileJsonData);
                }
                else
                {
                    AllData = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, strCompanyProperty, strMonthReportOrderType, IncludeHaveDetail);
                }
            }
            return AllData;
        }
    }
}
