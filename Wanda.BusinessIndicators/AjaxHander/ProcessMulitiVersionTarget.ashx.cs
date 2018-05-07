using Lib.Web.Json;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web.AjaxHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// ProcessMulitiVersionTarget 的摘要说明
    /// </summary>
    public class ProcessMulitiVersionTarget : IHttpHandler
    {

        private HttpContext context = null;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            this.context = context;
            string methodName = context.Request.PathInfo.Substring(1);
            System.Reflection.MethodInfo method = this.GetType().GetMethod(methodName);
            method.Invoke(this, null);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void AddMulitiVersionMonthlyReport()
        {
            var MonthReportID = Guid.Parse(context.Request["MonthReportID"]);
            var MonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            try
            {
                var error = "";
                var newMonthlyReportGuid = Guid.Empty;
                var newMonthlyReport = new B_MonthlyReport();
                MonthlyReportDetail mrd = null;
                var CurrentMonthlyReportDetailList = new List<B_MonthlyReportDetail>();
                //CurrentRpt = new ReportInstance(currentMonthReportId, true);
                //获取默认的MonthlyReport
                int FinYear = MonthlyReport.FinYear;
                int FinMonth = MonthlyReport.FinMonth;
                var MonthlyReportDetailList = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(MonthlyReport.ID);
                var targetVersionList = A_TargetplanOperator.Instance.GetTargetplanListForMulitiVersion(MonthlyReport.SystemID, MonthlyReport.FinYear);
                var targetList = C_TargetOperator.Instance.GetTargetList().Where(t => MonthlyReportDetailList.Select(v => v.TargetID).ToList().Contains(t.ID)).ToList();
                //循环多版本

                //先删除现有的非默认下的月报
                B_MonthlyreportOperator.Instance.DeleteNoDefaultVersionMonthlyReport(MonthlyReport);

                List<DictionaryVmodel> ListDV = null;
                List<MonthlyReportDetail> listMrd = null;
                foreach (var tagerPlanItem in targetVersionList)
                {
                    ListDV = new List<DictionaryVmodel>();
                    newMonthlyReport = MonthlyReport;
                    newMonthlyReport.ID = Guid.NewGuid();
                    //表示不是当前默认版本的计划指标
                    newMonthlyReport.DefaultVersionStatus = 0;
                    newMonthlyReport.TargetPlanID = tagerPlanItem.ID;
                    newMonthlyReportGuid = B_MonthlyreportOperator.Instance.AddMonthlyreport(newMonthlyReport);

                    var targetPlanDetailList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(tagerPlanItem.ID).Where(v => v.FinYear == FinYear && v.FinMonth == FinMonth).ToList();
                    //循环指标
                    foreach (var targetItem in targetList)
                    {
                        CurrentMonthlyReportDetailList = MonthlyReportDetailList.Where(v => v.TargetID == targetItem.ID).ToList();
                        listMrd = new List<MonthlyReportDetail>();
                        //拼装指标下面的所有项
                        foreach (var monthlyReportDetailItem in CurrentMonthlyReportDetailList)
                        {
                            mrd = new MonthlyReportDetail();
                            mrd.CompanyID = monthlyReportDetailItem.CompanyID;
                            //mrd.CompanyName = monthlyReportDetailItem.CompanyName;
                            mrd.NPlanAmmount = targetPlanDetailList.Where(v => v.CompanyID == mrd.CompanyID).FirstOrDefault().Target;
                            mrd.NActualAmmount = monthlyReportDetailItem.NActualAmmount;
                            mrd.TargetPlanID = tagerPlanItem.ID;
                            listMrd.Add(mrd);
                        }
                        ListDV.AddRange(FormatTargetDetailNew(out error, listMrd, newMonthlyReport.SystemID, FinYear, FinMonth, targetItem.TargetName, new ReportInstance(newMonthlyReportGuid, true)));

                    }
                    AddOrUpdateDataNew(ListDV, newMonthlyReportGuid);

                }

            }
            catch (Exception ex)
            {
                MonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
                B_MonthlyreportOperator.Instance.DeleteNoDefaultVersionMonthlyReport(MonthlyReport);
            }
        }


        #region 优化上传指标   UpLoadMonthTargetDetail 页面也有同样的方法，有时间记得整理到一起

        /// <summary>
        /// 整合数据（优化：改成删除后批量插入）
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listTartgetDetail">完成情况明细</param>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="targetName">指标名称</param>
        /// <param name="CurrentRpt">当前Rpt</param>
        /// <returns></returns>
        public List<DictionaryVmodel> FormatTargetDetailNew(out string error, List<MonthlyReportDetail> listTartgetDetail, Guid SystemID, int FinYear, int FinMonth, string targetName, ReportInstance CurrentRpt)
        {
            error = "";
            List<DictionaryVmodel> listdv = new List<DictionaryVmodel>();
            List<MonthlyReportDetail> lstInsertMonthReportDetail = new List<MonthlyReportDetail>();
            Guid targetID;
            try
            {
                targetID = CurrentRpt._Target.Where(p => p.TargetName == targetName).ToList()[0].ID;
            }
            catch (Exception)
            {
                error = "请确认上传的指标是否是本系统的指标!";
                return null;
            }

            MonthlyReportDetail monthlyReportDetail = null;
            //是否存在B_MonthlyReport

            foreach (MonthlyReportDetail mrd in listTartgetDetail)
            {
                monthlyReportDetail = new MonthlyReportDetail();

                monthlyReportDetail.ID = new Guid();
                monthlyReportDetail.CreateTime = DateTime.Now;
                monthlyReportDetail.CreatorName = "System";
                monthlyReportDetail.SystemID = SystemID;//系统ID
                monthlyReportDetail.FinYear = FinYear;//当前年
                monthlyReportDetail.FinMonth = FinMonth;//当前月
                monthlyReportDetail.TargetID = targetID;//指标ID
                List<C_Company> listCompany = StaticResource.Instance.CompanyList[SystemID].ToList().Where(p => p.ID == mrd.CompanyID).ToList();
                if (listCompany.Count() == 0)
                {
                    continue;
                }
                else
                {
                    monthlyReportDetail.CompanyID = listCompany[0].ID; //公司ID
                    monthlyReportDetail.CompanyProperty1 = listCompany[0].CompanyProperty1; //公司ID
                    monthlyReportDetail.CompanyProperty = JsonConvert.SerializeObject(listCompany[0]);
                }

                monthlyReportDetail.TargetPlanID = mrd.TargetPlanID;//计划指标ID
                monthlyReportDetail.MonthlyReportID = CurrentRpt._MonthReportID;//月度报告ID
                monthlyReportDetail.NPlanAmmount = mrd.NPlanAmmount;//计划指标
                monthlyReportDetail.NActualAmmount = mrd.NActualAmmount;// 实际数
                monthlyReportDetail.NDifference = mrd.NActualAmmount - mrd.NPlanAmmount;//差额
                monthlyReportDetail.NAccumulativePlanAmmount = mrd.NAccumulativePlanAmmount;//累计计划指标
                monthlyReportDetail.NAccumulativeActualAmmount = mrd.NAccumulativeActualAmmount;//累计实际数
                monthlyReportDetail.NAccumulativeDifference = mrd.NAccumulativeActualAmmount - mrd.NAccumulativePlanAmmount;//累计差额

                //计算年度累计
                lstInsertMonthReportDetail.Add(monthlyReportDetail);
            }

            listdv.Add(new DictionaryVmodel("Insert", lstInsertMonthReportDetail));
            return listdv;
        }


        public void AddOrUpdateDataNew(List<DictionaryVmodel> ListDV, Guid MonthReportID)
        {
            List<B_MonthlyReportDetail> lstInsertMonthReportDetail = new List<B_MonthlyReportDetail>();
            foreach (DictionaryVmodel dv in ListDV)
            {
                List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();
                List<MonthlyReportDetail> Listmrd = (List<MonthlyReportDetail>)dv.ObjValue;

                Listmrd.ForEach(p => B_ReportDetails.Add(CalculationEvaluationEngine.CalculationEvaluationService.Calculation(p.ToBModel(), "")));
                lstInsertMonthReportDetail.AddRange(B_ReportDetails);
            }
            if (lstInsertMonthReportDetail.Count > 0)
            {
                //优化，批量插入
                B_MonthlyreportdetailOperator.Instance.BulkAddTargetDetail(lstInsertMonthReportDetail);
            }
            B_MonthlyReport bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            //if (bmr != null && lstInsertMonthReportDetail.Count > 0)
            //{
            //    bmr.Status = 5;
            //    B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);
            //}

            //上报的时候序列化后的Json数据
            SaveJsonData(MonthReportID);

        }



        /// <summary>
        /// 保存，上报的时候序列化后的Json数据
        /// </summary>
        /// <param name="MonthReportID"></param>
        private void SaveJsonData(Guid MonthReportID)
        {
            B_MonthlyReportJsonData Update_JsonData;

            var CurrentRpt = new ReportInstance(MonthReportID, true);

            try
            {
                Update_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(MonthReportID);
            }
            catch (Exception ex)//去Json 表中查看下数据
            {
                Update_JsonData = null;
            }

            if (Update_JsonData != null)
            {
                Update_JsonData.SystemID = CurrentRpt._SystemID;
                Update_JsonData.PlanType = "M";
                Update_JsonData.FinMonth = CurrentRpt.FinMonth;
                Update_JsonData.FinYear = CurrentRpt.FinYear;
                Update_JsonData.ModifyTime = DateTime.Now;

                List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();
                //这是上报页面的Json 数据
                TargetReportedControll trc = new TargetReportedControll();
                ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(CurrentRpt)));
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                Update_JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.UpdateMonthlyReportJsonData(Update_JsonData);

            }
            else
            {
                //新增数据
                B_MonthlyReportJsonData JsonData = new B_MonthlyReportJsonData();
                JsonData.ID = MonthReportID;
                JsonData.SystemID = CurrentRpt._SystemID;
                JsonData.PlanType = "M";
                JsonData.FinMonth = CurrentRpt.FinMonth;
                JsonData.FinYear = CurrentRpt.FinYear;
                JsonData.CreateTime = DateTime.Now;

                List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

                //这是上报页面的Json 数据
                TargetReportedControll trc = new TargetReportedControll();
                ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(CurrentRpt)));
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);
            }
        }
        #endregion

    }
}