using Lib.Core;
using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web.AjaxHandler;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// MonthlyReportLog 的摘要说明
    /// </summary>
    public class MonthlyReportLog : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int ActionType = 1;
            Guid SysId = Guid.Empty;
            int FinYear = DateTime.Now.Year;
            int FinMonth = DateTime.Now.Month;
            Guid MonthReportID = Guid.Empty;
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

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["MonthReportID"]))
            {
                MonthReportID = Guid.Parse(HttpContext.Current.Request["MonthReportID"]);
            }


            ReportInstance rpt = new ReportInstance(MonthReportID, true);


            AddMonthlyReportAction(ActionType, SysId, FinYear, FinMonth, MonthReportID);

            AddBusinessBaseJson(SysId, FinYear, FinMonth, MonthReportID , rpt);
            
            SaveJsonData(MonthReportID,SysId, FinYear, FinMonth, rpt);
        }


        /// <summary>
        /// 操作记录日志
        /// </summary>
        /// <param name="ActionType"></param>
        /// <param name="SysId"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="MonthReportID"></param>
        public void AddMonthlyReportAction(int ActionType, Guid SysId, int FinYear, int FinMonth, Guid MonthReportID)
        {
            B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
            _bMonthlyReportAction.SystemID = SysId;
            _bMonthlyReportAction.MonthlyReportID = MonthReportID;
            _bMonthlyReportAction.FinYear = FinYear;
            _bMonthlyReportAction.FinMonth = FinMonth;
            _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), ActionType);
            _bMonthlyReportAction.Operator = PermissionHelper.GetCurrentUser;
            _bMonthlyReportAction.OperatorTime = DateTime.Now;
            _bMonthlyReportAction.ModifierName = PermissionHelper.GetCurrentUser;
            _bMonthlyReportAction.CreatorName = PermissionHelper.GetCurrentUser;

            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);
        }


        /// <summary>
        /// 提供给手机端审批的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthReportID"></param>
        public void AddBusinessBaseJson(Guid SystemID, int Year, int Month, Guid MonthReportID, ReportInstance rpt)
        {
            B_BusinessBase businessModel = null;

            var Sys = StaticResource.Instance[SystemID, DateTime.Now];
            
            try {
                 businessModel = B_BusinessBaseOperator.Instance.GetBusinessBase(MonthReportID);
            }
            catch 
            {

            }
            
            if (businessModel != null )
            {
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
               
                BBModel.MonthlyReportID  = BBModel.ID = MonthReportID;
                BBModel.BusinessType = "MobileApproval";
                BBModel.CreateTime = DateTime.Now;
                BBModel.ReportPersonCTX = PermissionHelper.GetCurrentUser;
                BBModel.ReportDate = DateTime.Now;
                BBModel.ProcessType = 0;
                BBModel.SystemName = Sys.SystemName;
                BBModel.FinMonth = Month;
                BBModel.FinYear = Year;

                var Rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
                if (Rpt != null)
                    BBModel.Description = Rpt.Description;


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



        /// <summary>
        /// 保存，上报的时候序列化后的Json数据
        /// </summary>
        /// <param name="MonthReportID"></param>
        private void SaveJsonData(Guid MonthReportID, Guid SysId, int FinYear, int FinMonth, ReportInstance CurrentRpt)
        {
            B_MonthlyReportJsonData Update_JsonData;
            
            try
            {
                Update_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(MonthReportID);
            }
            catch (Exception )//去Json 表中查看下数据
            {
                Update_JsonData = null;
            }
            if (Update_JsonData != null)
            {
                //如果修改的话，顺便修改下B_MonthlyReport的字段，便于服务重构Json
                var BMR = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);


                Update_JsonData.SystemID = SysId;
                Update_JsonData.PlanType = "M";
                Update_JsonData.FinMonth = FinMonth;
                Update_JsonData.FinYear = FinYear;
                Update_JsonData.ModifyTime = DateTime.Now;

                List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();
                //这是上报页面的Json 数据
                TargetReportedControll trc = new TargetReportedControll();

                ReportInstance rpt = new ReportInstance(MonthReportID, true);
                ListObj.Add(new DictionaryVmodel("ReportInstance", rpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(rpt, "Detail", true))); // 

                ReportInstance rpt2 = new ReportInstance(MonthReportID, true);

                ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(rpt2, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(rpt2)));

                ReportInstance rpt3 = new ReportInstance(MonthReportID, true);
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(rpt3, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                Update_JsonData.ReportJsonData = sb.ToString();

                //修改的话，将查询的数据赋值为Null
                Update_JsonData.QuerryCurrentMissJsonData = string.Empty;
                Update_JsonData.QuerryDetaileJsonData = string.Empty;
                Update_JsonData.QuerryMissJsonData = string.Empty;
                Update_JsonData.QuerryReturnJsonData = string.Empty;
                Update_JsonData.QuerrySumJsonData = string.Empty;
                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.UpdateMonthlyReportJsonData(Update_JsonData);

                //让服务重新计算。
                BMR.DataOptimizationJson = string.Empty;
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(BMR);

            }
            else
            {
                //新增数据
                B_MonthlyReportJsonData JsonData = new B_MonthlyReportJsonData();
                JsonData.ID = MonthReportID;
                JsonData.SystemID = SysId;
                JsonData.PlanType = "M";
                JsonData.FinMonth = FinMonth;
                JsonData.FinYear = FinYear;
                JsonData.CreateTime = DateTime.Now;

                List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

                //这是上报页面的Json 数据
                TargetReportedControll trc = new TargetReportedControll();

                ReportInstance rpt = new ReportInstance(MonthReportID, true);
                ListObj.Add(new DictionaryVmodel("ReportInstance", rpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(rpt, "Detail", true))); // 

                ReportInstance rpt2 = new ReportInstance(MonthReportID, true);
                ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(rpt2, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(rpt2)));

                ReportInstance rpt3 = new ReportInstance(MonthReportID, true);
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(rpt3, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);
            }
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