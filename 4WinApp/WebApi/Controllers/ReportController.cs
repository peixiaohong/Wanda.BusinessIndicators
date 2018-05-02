using LJTH.BusinessIndicators.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LJTH.BusinessIndicators.Web.AjaxHandler;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using WebApi.Models;
using Newtonsoft.Json;
using Lib.Core;
using LJTH.BusinessIndicators.Common;

namespace WebApi.Controllers
{
    public class ReportController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMonthReportID"></param>
        /// <param name="strSystemId"></param>
        /// <param name="strProType"></param>
        /// <returns></returns>
        public ResultContext GetReportInstance(string strMonthReportID, string strSystemId, string strProType)
        {
            TargetApproveController ta = new TargetApproveController();
            List<DictionaryVmodel> list = ta.GetReportInstance(strMonthReportID, strSystemId, strProType);
            return new ResultContext(list);
        }

        public ResultContext GetDetailRptDataSource(string rpts, string strCompanyProperty, string strMonthReportOrderType, bool IncludeHaveDetail)
        {
            TargetApproveController ta = new TargetApproveController();
            List<DictionaryVmodel> list = ta.GetDetailRptDataSource(rpts, strCompanyProperty, strMonthReportOrderType, IncludeHaveDetail);
            return new ResultContext(list);
        }
        /// <summary>
        /// 获取月报的年与业态列表
        /// </summary>
        /// <returns></returns>
        public ResultContext GetSystemAndYearList()
        {
            try
            {
                List<string> Year = new List<string>();

                B_MonthlyreportOperator.Instance.GetMonthlyReportYearList().ForEach(x => Year.Add(x.FinYear.ToString()));
                List<C_System> System = new List<C_System>();
                System = C_SystemOperator.Instance.GetSystemList(DateTime.Now).OrderBy(S => S.Sequence).ToList();

                return new ResultContext(new { Year, System });
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 月报
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public ResultContext GetMonthList(string SystemID, int Year, int Month, bool IsLatestVersion = false, string DataSource = "Draft", bool IsAll = false)
        {
            try
            {
                MonthlyReportController ms = new MonthlyReportController();
                List<DictionaryVmodel> listM = ms.GetReportInstance(SystemID, Year, Month, IsLatestVersion, DataSource, IsAll);
                Guid result = Guid.Empty;
                Guid.TryParse(SystemID, out result);
                if (result == Guid.Empty)
                    return new ResultContext((int)StatusCodeEnum.isFalse, "系统编码错误");
                List<B_MonthlyReport> li = B_MonthlyreportOperator.Instance.GetMonthlyReportBySysIDList(result, Year, Month);
                string Dis = "";
                if (li.Count > 0)
                    Dis = li.FirstOrDefault().Description;

                return new ResultContext(new { title = Dis, list = listM[2] });
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }

        }
        /// <summary>
        /// 月报明细
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public ResultContext GetMonthDetailList(string SystemID, int Year, int Month, string TargetName, bool IncludeHaveDetail, bool IsLatestVersion = false, string DataSource = "Draft", bool IsAll = false)
        {
            try
            {
                MonthlyReportController ms = new MonthlyReportController();
                TargetApproveController ta = new TargetApproveController();
                List<DictionaryVmodel> li = ms.GetReportInstance(SystemID, Year, Month);
                List<DictionaryVmodel> listM = ms.GetDetailRptDataSource(li[0].ObjValue.ToString(), "", "Detail", IncludeHaveDetail);
                Guid result = Guid.Empty;
                Guid.TryParse(SystemID, out result);
                if (result == Guid.Empty)
                    return new ResultContext((int)StatusCodeEnum.isFalse, "系统编码错误");

                List<DictionaryVmodel> listS = new List<DictionaryVmodel>();
                if (!string.IsNullOrEmpty(TargetName))
                    listS = listM.Where(x => x.Name == TargetName).ToList();
                List<B_MonthlyReport> lis = B_MonthlyreportOperator.Instance.GetMonthlyReportBySysIDList(result, Year, Month);
                string Dis = "";
                if (lis.Count > 0)
                    Dis = lis.FirstOrDefault().Description;

                return new ResultContext(new { title = Dis, list = listS });
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }

        }

        /// <summary>
        /// 获取月报审批数据
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="ProType"></param>
        /// <returns></returns>
        public ResultContext MonthApprove(string BusinessID, string ProType, bool IsLatestVersion)
        {
            MonthlyReportController mr = new MonthlyReportController();
            string SystemID = string.Empty;
            int Year = 0;
            int Month = 0;
            string systemName = string.Empty;
            if (string.IsNullOrEmpty(ProType))
            {
                B_MonthlyReport bmonthReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(Guid.Parse(BusinessID));
                if (bmonthReport != null)
                {
                    SystemID = bmonthReport.SystemID.ToString();
                    Year = bmonthReport.FinYear;
                    Month = bmonthReport.FinMonth;
                    C_System system = StaticResource.Instance.SystemList.Where(p => p.ID == bmonthReport.SystemID).FirstOrDefault();
                    if (system != null)
                    {
                        systemName = system.SystemName;
                    }
                }
            }
            else
            {
                B_SystemBatch _BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(Guid.Parse(BusinessID));
                List<V_SubReport> BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(_BatchModel.SubReport);

                C_System c_System = StaticResource.Instance.SystemList.Where(x => x.GroupType == _BatchModel.BatchType).FirstOrDefault();
                SystemID = c_System.ID.ToString();
                Month = _BatchModel.FinMonth;
                Year = _BatchModel.FinYear;
            }
            List<DictionaryVmodel> list = mr.GetReportInstance(SystemID, Year, Month, IsLatestVersion);
            return new ResultContext(list);

        }



        public ResultContext GetShowPrcessNodeName(string strSystemID, string strProcessCode)
        {
            try
            {
                TargetController tc = new TargetController();
                bool ShowProecessNodeName = tc.GetShowPrcessNodeName(strSystemID, strProcessCode);
                return new ResultContext(ShowProecessNodeName);
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
        }



        #region 分解指标查询

        /// <summary>
        /// 获取分解指标的年与业态列表
        /// </summary>
        /// <returns></returns>
        public ResultContext GetTargetCollectInit()
        {
            try
            {
                List<string> Year = new List<string>();

                A_TargetplanOperator.Instance.GetPlanYearList().ForEach(x => Year.Add(x.FinYear.ToString()));
                List<C_System> System = new List<C_System>();
                System = C_SystemOperator.Instance.GetSystemList(DateTime.Now).OrderBy(S => S.Sequence).ToList();

                return new ResultContext(new { Year, System });
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
        }


        /// <summary>
        /// 分解指标数据
        /// </summary>
        /// <param name="SysID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public ResultContext GetVerTargetList(string SysID, string FinYear)
        {
            try
            {
                CompanyController cc = new CompanyController();
                List<C_Target> result = cc.GetVerTargetList(SysID, FinYear);
                return new ResultContext(result);
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }

        }
        #endregion
        /// <summary>
        /// 获取指标分解审批页面展示数据
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public ResultContext TargetPlanApprove(string BusinessID, bool IsLatestVersion)
        {
            try
            {
                B_TargetPlan _BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByID(Guid.Parse(BusinessID));
                string SystemID = string.Empty;
                string Year = "";
                C_System system = new C_System();
                if (_BTargetPlan != null)
                {
                    SystemID = _BTargetPlan.SystemID.ToString();
                    Year = _BTargetPlan.FinYear.ToString();
                    system = StaticResource.Instance[_BTargetPlan.SystemID, DateTime.Now];

                }
                TargetPlanDetailController tp = new TargetPlanDetailController();

                if (system != null && system.Category == 3)
                {
                    return new ResultContext(tp.GetTargetPlanDetail(SystemID, Year, BusinessID, IsLatestVersion));
                }
                else
                {
                    return new ResultContext(tp.GetVerTargetList(BusinessID));
                }
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }

        }


    }
}