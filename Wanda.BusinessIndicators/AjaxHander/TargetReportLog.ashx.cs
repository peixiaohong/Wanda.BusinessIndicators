using Lib.Core;
using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// TargetReportLog 的摘要说明
    /// </summary>
    public class TargetReportLog : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int ActionType = 1;
            Guid SysId = Guid.Empty;
            int FinYear = DateTime.Now.Year;
            int FinMonth = DateTime.Now.Month;
            Guid TargetPlanID = Guid.Empty;
            context.Response.ContentType = "text/plain";

            if (HttpContext.Current.Request.QueryString["ActionType"] != null)
            {
                ActionType = int.Parse(HttpContext.Current.Request.QueryString["ActionType"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysId"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SysId"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear = Convert.ToInt32(HttpContext.Current.Request["FinYear"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinMonth"]))
            {
                FinMonth = Convert.ToInt32(HttpContext.Current.Request["FinMonth"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["TargetPlanID"]))
            {
                TargetPlanID = Guid.Parse(HttpContext.Current.Request["TargetPlanID"]);
            }

            AddTargetReportAction(ActionType, SysId, FinYear, TargetPlanID);

            AddBusinessBaseJson(SysId, FinYear, TargetPlanID);
        }

        public void AddTargetReportAction(int ActionType, Guid SysId, int FinYear, Guid TargetPlanID)
        {
            B_TargetPlanAction _bMonthlyReportAction = new B_TargetPlanAction();
            _bMonthlyReportAction.SystemID = SysId;
            _bMonthlyReportAction.TargetPlanID = TargetPlanID;
            _bMonthlyReportAction.FinYear = FinYear;
            _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), ActionType);
            _bMonthlyReportAction.Operator = PermissionHelper.GetCurrentUser;
            _bMonthlyReportAction.OperatorTime = DateTime.Now;
            _bMonthlyReportAction.ModifierName = PermissionHelper.GetCurrentUser;
            _bMonthlyReportAction.CreatorName = PermissionHelper.GetCurrentUser;

            B_TargetPlanActionOperator.Instance.AddTargetPlanAction(_bMonthlyReportAction);
        }


        public void AddBusinessBaseJson(Guid SystemID, int Year, Guid MonthReportID)
        {
            B_BusinessBase businessModel = null;

            var Sys = StaticResource.Instance[SystemID, DateTime.Now];

            try
            {
                businessModel = B_BusinessBaseOperator.Instance.GetBusinessBase(MonthReportID);
            }
            catch
            {

            }

            if (businessModel != null)
            {
                ReportInstance rpt = new ReportInstance(MonthReportID, true);
                List<DictionaryVmodel> listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);

                if (Sys != null)
                {
                    if (Sys.Category == 4)
                    {
                        listSRDS.RemoveAt(0);
                    }
                    else
                    {
                        listSRDS.RemoveAt(0);
                        listSRDS.RemoveAt(listSRDS.Count - 1);
                    }
                }

                var Rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);

                if (Rpt != null)
                    businessModel.Description = Rpt.Description;

                businessModel.FormData = JsonHelper.Serialize(listSRDS);
                B_BusinessBaseOperator.Instance.UpdateBusinessBase(businessModel);

            }
            else
            {
                B_BusinessBase BBModel = new B_BusinessBase();

                BBModel.MonthlyReportID = BBModel.ID = MonthReportID;
                BBModel.BusinessType = "MobileApproval";
                BBModel.CreateTime = DateTime.Now;
                BBModel.ReportPersonCTX = PermissionHelper.GetCurrentUser;
                BBModel.ReportDate = DateTime.Now;
                BBModel.ProcessType = 0;
                BBModel.SystemName = Sys.SystemName;
                BBModel.FinYear = Year;

                var Rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
                if (Rpt != null)
                    BBModel.Description = Rpt.Description;

                ReportInstance rpt = new ReportInstance(MonthReportID, true);
                List<DictionaryVmodel> listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);

                if (Sys != null)
                {
                    if (Sys.Category == 4)
                    {
                        listSRDS.RemoveAt(0);
                    }
                    else
                    {
                        listSRDS.RemoveAt(0);
                        listSRDS.RemoveAt(listSRDS.Count - 1);
                    }
                }

                BBModel.FormData = JsonHelper.Serialize(listSRDS);
                B_BusinessBaseOperator.Instance.AddBusinessBase(BBModel);


            }


        }


        public void AddMobileApprovalJson(Guid SystemID, int Year, int Month, Guid MonthReportID)
        {

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}