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
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.BLL.BizBLL;

namespace WebApi.Controllers
{
    public class ReportController : ApiController
    {



        /// <summary>
        /// 获取月报的年与版块列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultContext GetSystemAndYearList()
        {
            try
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                int finYear = datetime.Year;
                int finMonth = datetime.Month;
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }

                var _SystemIds = S_OrganizationalActionOperator.Instance.GetUserSystemData(WebHelper.GetCurrentLoginUser());


                return new ResultContext(new { Year, SelectYear = finYear, SelectMonth = finMonth, System = _SystemIds.Select(x => new { ID = x.SystemID, SystemName = x.CnName }).ToList() });
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 获取月报版本
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultContext GetTargetPlanVersionList(string SystemID, string Year, string Month)
        {
            try
            {
                MonthlyReportController tc = new MonthlyReportController();
                List<A_TargetPlan> list = (List<A_TargetPlan>)tc.GetTargetVersionType(SystemID, int.Parse(Year), int.Parse(Month));

                return new ResultContext(list);
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
        [HttpGet]
        public ResultContext GetMonthList(string SystemID, int Year, int Month, string TargetPlanID, bool IsLatestVersion = false, string DataSource = "Draft", bool IsAll = false)
        {
            try
            {
                MonthlyReportController ms = new MonthlyReportController();
                List<DictionaryVmodel> listM = ms.GetReportInstance(SystemID, Year, Month, TargetPlanID, IsLatestVersion, DataSource, IsAll);
                Guid result = Guid.Empty;
                Guid.TryParse(SystemID, out result);
                if (result == Guid.Empty)
                    return new ResultContext((int)StatusCodeEnum.isFalse, "系统编码错误");
                DictionaryVmodel dv = new DictionaryVmodel();
                if (listM.Count > 0)
                    dv = listM[2];

                return new ResultContext(new { title = listM[1].ObjValue, list = JsonConvert.SerializeObject(dv) });
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
        [HttpGet]
        public ResultContext GetMonthDetailList(string SystemID, int Year, int Month, string TargetName, string TargetPlanID, bool IncludeHaveDetail, bool IsLatestVersion = false, string DataSource = "Draft", bool IsAll = false)
        {
            try
            {
                MonthlyReportController ms = new MonthlyReportController();
                S_OrganizationalActionOperator oa = new S_OrganizationalActionOperator();
                TargetApproveController ta = new TargetApproveController();
                List<DictionaryVmodel> li = ms.GetReportInstance(SystemID, Year, Month, TargetPlanID, IsLatestVersion, DataSource, IsAll);
                if (li == null || li.Count == 0 || li[0].ObjValue == null)
                    return new ResultContext();
                List<DictionaryVmodel> listM = new List<DictionaryVmodel>();
                bool type = true;
                if (StaticResource.Instance.GetSystem_Regional(SystemID.ToGuid()))
                    //经营报告明细
                    listM = ms.GetManageDetailRptDataSource((ReportInstance)li[0].ObjValue, "", "Detail", IncludeHaveDetail);
                else
                {
                    type = false;
                    //完成情况明细
                    listM = ms.GetDetailRptDataSource((ReportInstance)li[0].ObjValue, "", "Detail", IncludeHaveDetail);
                }

                Guid result = Guid.Empty;
                Guid.TryParse(SystemID, out result);
                if (result == Guid.Empty)
                    return new ResultContext((int)StatusCodeEnum.isFalse, "系统编码错误");

                C_System system = new C_System();
                system = StaticResource.Instance[SystemID.ToGuid(), DateTime.Now];

                return new ResultContext(new { title = system.SystemName, type, list = listM });
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
        [HttpGet]
        public ResultContext MonthApprove(string strMonthReportID, string strBacthID, string strProType)
        {
            TargetApproveController ta = new TargetApproveController();
            List<DictionaryVmodel> list = ta.GetReportInstance(strMonthReportID, strBacthID, strProType);
            return new ResultContext(JsonConvert.SerializeObject(list));

        }


        [HttpGet]
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
        /// 获取分解指标的年与版块列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultContext GetTargetCollectInit()
        {
            try
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                int finYear = datetime.Year;
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }

                var _SystemIds = S_OrganizationalActionOperator.Instance.GetUserSystemData(WebHelper.GetCurrentLoginUser());

                return new ResultContext(new { Year, SelectYear = finYear, System = _SystemIds.Select(x => new { ID = x.SystemID, SystemName = x.CnName }).ToList() });

            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 获取分解指标版本
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultContext GetTargetPlanVersionList(string SystemID, string Year)
        {
            try
            {
                MonthlyReportController tc = new MonthlyReportController();
                List<A_TargetPlan> list = (List<A_TargetPlan>)tc.GetTargetVersionType(SystemID, int.Parse(Year));

                return new ResultContext(list);
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
        [HttpGet]
        public ResultContext GetTargetPlanList(string SysID, string FinYear, string TargetPlanID)
        {
            try
            {
                CompanyController cc = new CompanyController();
                List<C_Target> result = cc.GetVerTargetList(SysID, FinYear);
                A_TargetplandetailOperator tc = new A_TargetplandetailOperator();
                List<TargetDetail> list = tc.GetSumMonthTargetDetailByTID_A(TargetPlanID.ToGuid());
                return new ResultContext(new { head = result, list });
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
        [HttpGet]
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
                TargetController tc = new TargetController();
                string title = Year + "年" + system.SystemName + "指标分解";
                if (!string.IsNullOrEmpty(_BTargetPlan.VersionName))
                    title += "-" + _BTargetPlan.VersionName;
                if (system != null && system.Category == 3)
                {
                    return new ResultContext(tp.GetTargetPlanDetail(SystemID, Year, BusinessID, IsLatestVersion));
                }
                else
                {
                    return new ResultContext(new { title, systemName = system.SystemName, Year, list = tc.GetSumMonthTargetDetailByTID(BusinessID) });
                }
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }

        }

        [HttpGet]
        public string api()
        {
            return "aaa";
        }
    }
}