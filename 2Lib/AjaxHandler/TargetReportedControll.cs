using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;
using System.Collections;
using Newtonsoft.Json;
using LJTH.BusinessIndicators.Common;
using Lib.Core;
using System.Text;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class TargetReportedControll : BaseController
    {
        /// <summary>
        /// ReportInstance实例
        /// </summary>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetReportInstance(string strSystemID, string strMonthReportID, string strMonthReportOrderType, bool IncludeHaveDetail, bool UploadStr)
        {
            List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

            B_MonthlyReportJsonData JsonData;

            try
            {
                JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(strMonthReportID.ToGuid());
            }
            catch (Exception ex)//去Json 表中查看下数据
            {
                JsonData = null;
            }
            
            if (JsonData != null)
            {
                if (!string.IsNullOrEmpty(JsonData.ReportJsonData))
                {
                    // 这里直接去拿Json
                    ListObj = JsonHelper.Deserialize<List<DictionaryVmodel>>(JsonData.ReportJsonData);
                }
                else
                {
                   // 修改
                    ReportInstance rpt = new ReportInstance(strMonthReportID.ToGuid(), true);
                    ListObj.Add(new DictionaryVmodel("ReportInstance", rpt));
                    ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(rpt, strMonthReportOrderType, IncludeHaveDetail)));
                    ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(rpt, strMonthReportID, UploadStr)));
                    ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(rpt)));
                    ListObj.Add(new DictionaryVmodel("CurrentMisstarget", GetCurrentMissTargetList(rpt, strMonthReportID, UploadStr)));

                    StringBuilder sb = new StringBuilder();
                    sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据

                    JsonData.SystemID = rpt._SystemID;
                    JsonData.PlanType = "M";
                    JsonData.FinMonth = rpt.FinMonth;
                    JsonData.FinYear = rpt.FinYear;
                    JsonData.ModifyTime = DateTime.Now;
                    JsonData.ReportJsonData = sb.ToString();
                    B_MonthlyReportJsonDataOperator.Instance.UpdateMonthlyReportJsonData(JsonData);
                }
            }
            else
            {
                ReportInstance rpt = new ReportInstance(strMonthReportID.ToGuid(), true);
                ListObj.Add(new DictionaryVmodel("ReportInstance", rpt));
                ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(rpt, strMonthReportOrderType, IncludeHaveDetail)));
                ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(rpt, strMonthReportID, UploadStr)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(rpt)));
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", GetCurrentMissTargetList(rpt, strMonthReportID, UploadStr)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据

                JsonData = new B_MonthlyReportJsonData();
                JsonData.ID = strMonthReportID.ToGuid();
                JsonData.SystemID = rpt._SystemID;
                JsonData.PlanType = "M";
                JsonData.FinMonth = rpt.FinMonth;
                JsonData.FinYear = rpt.FinYear;
                JsonData.CreateTime = DateTime.Now;

                JsonData.ReportJsonData = sb.ToString();
                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);
            }

            return ListObj;
        }


        /// <summary>
        /// 完成情况明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetTargetDetailList(string rpts, string strMonthReportOrderType, bool IncludeHaveDetail)
        {
            //根据角色获取相关的系统ID
            List<DictionaryVmodel> RptDictList = null;
            if (!string.IsNullOrEmpty(rpts))
            {
                ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
                //这里的“Reported” 是为了在引擎，调用的时候容易判断是在上报，还是在查询的时候调用
                RptDictList = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "Reported", strMonthReportOrderType, IncludeHaveDetail);
            }
            return RptDictList;
        }

        /// <summary>
        /// 完成情况明细
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetTargetDetailList(ReportInstance rpts, string strMonthReportOrderType, bool IncludeHaveDetail)
        {
            //根据角色获取相关的系统ID
            List<DictionaryVmodel> RptDictList = null;
            //这里的“Reported” 是为了在引擎，调用的时候容易判断是在上报，还是在查询的时候调用
            RptDictList = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpts, "Reported", strMonthReportOrderType, IncludeHaveDetail);
            return RptDictList;
        }

        /// <summary>
        /// 未完成说明，这里的数据应该都是草稿状态的数据
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetMissTargetList(ReportInstance rpts, string monthRptID, bool UploadStr)
        {
            //根据角色获取相关的系统ID
            List<DictionaryVmodel> RptDictList = null;

            if (UploadStr) //判断是否上传
            {
                Guid _monthRptID = Guid.Empty;

                if (!string.IsNullOrEmpty(monthRptID))
                {
                    _monthRptID = Guid.Parse(monthRptID);
                }
                ReportInstance rpt = new ReportInstance(_monthRptID, true);
                RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
            }
            else
            {
                if (rpts != null)
                {
                    ReportInstance rpt = rpts;

                    Guid _monthRptID = Guid.Empty;

                    if (!string.IsNullOrEmpty(monthRptID))
                    {
                        _monthRptID = Guid.Parse(monthRptID);
                    }

                    RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                }
            }
            
            return RptDictList;
        }



        /// <summary>
        /// 未完成说明，这里的数据应该都是草稿状态的数据
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetCurrentMissTargetList(ReportInstance rpts, string monthRptID, bool UploadStr)
        {
            //根据角色获取相关的系统ID
            List<DictionaryVmodel> RptDictList = null;

            if (UploadStr) //判断是否上传
            {
                Guid _monthRptID = Guid.Empty;

                if (!string.IsNullOrEmpty(monthRptID))
                {
                    _monthRptID = Guid.Parse(monthRptID);
                }
                ReportInstance rpt = new ReportInstance(_monthRptID, true);
                RptDictList = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);
            }
            else
            {
                if (rpts != null)
                {
                    ReportInstance rpt = rpts;

                    Guid _monthRptID = Guid.Empty;

                    if (!string.IsNullOrEmpty(monthRptID))
                    {
                        _monthRptID = Guid.Parse(monthRptID);
                    }

                    RptDictList = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);
                }
            }
            return RptDictList;
        }




        /// <summary>
        /// 未完成说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetMissTargetList(string rpts, string monthRptID, bool UploadStr)
        {
            //根据角色获取相关的系统ID
            List<DictionaryVmodel> RptDictList = null;

            if (UploadStr) //判断是否上传
            {
                Guid _monthRptID = Guid.Empty;

                if (!string.IsNullOrEmpty(monthRptID))
                {
                    _monthRptID = Guid.Parse(monthRptID);
                }
                ReportInstance rpt = new ReportInstance(_monthRptID, true);
                RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);

               
            }
            else
            {
                if (!string.IsNullOrEmpty(rpts))
                {
                    ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

                    Guid _monthRptID = Guid.Empty;

                    if (!string.IsNullOrEmpty(monthRptID))
                    {
                        _monthRptID = Guid.Parse(monthRptID);
                    }
                    RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                }
            }
            return RptDictList;
        }



        /// <summary>
        /// 未完成说明,上报页面用的
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetMissTargetList_Reported(string rpts, string monthRptID, bool UploadStr)
        {
            List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

            //根据角色获取相关的系统ID
            List<DictionaryVmodel> RptDictList = null;

            if (UploadStr) //判断是否上传
            {
                Guid _monthRptID = Guid.Empty;

                if (!string.IsNullOrEmpty(monthRptID))
                {
                    _monthRptID = Guid.Parse(monthRptID);
                }
                ReportInstance rpt = new ReportInstance(_monthRptID, true);
                RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);

                ListObj.Add(new DictionaryVmodel("MissTargetList", RptDictList));
                ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(rpt, monthRptID, true)));
            }
            else
            {
                if (!string.IsNullOrEmpty(rpts))
                {
                    ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

                    Guid _monthRptID = Guid.Empty;

                    if (!string.IsNullOrEmpty(monthRptID))
                    {
                        _monthRptID = Guid.Parse(monthRptID);
                    }

                    RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                    ListObj.Add(new DictionaryVmodel("MissTargetList", RptDictList));
                    ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt)));

                }
            }
            return ListObj;
        }




        /// <summary>
        /// 修改未完成说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> ModifyMissTarget(string rpts)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            List<DictionaryVmodel> RptDictList = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
            return RptDictList;
        }

        /// <summary>
        /// 添加指标数据
        /// </summary>
        /// <param name="list">指标数据</param>
        /// <returns>执行状态</returns>
        public int AddOrUpdateTargetDetail(List<B_MonthlyReportDetail> list, string strOperateType)
        {
            int error = 0;
            error = B_MonthlyreportdetailOperator.Instance.AddOrUpdateTargetDetail(list, strOperateType);
            return error;
        }

        /// <summary>
        /// 获取月报说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public string GetMonthTRptDescription(ReportInstance rpts)
        {
            string Description = string.Empty;
            XElement element = null;

            B_MonthlyReport RptModel = null;

            ReportInstance rpt = rpts;

            RptModel = rpt.LastestMonthlyReport;


            List<MonthlyReportDetail> RptList = new List<MonthlyReportDetail>();
            if (rpt.LastestMonthlyReportDetails.Count <= 0)
            {
                return "";
            }

            rpt.LastestMonthlyReportDetails.ForEach(p => RptList.Add(p.ToVModel()));

            if (rpt._System.ID != Guid.Empty)
            {
                C_System sysModel = StaticResource.Instance[rpt._System.ID, DateTime.Now];
                element = sysModel.Configuration;
                if (element.Elements("Report").Elements("Rgroup") != null)
                {
                    Description = element.Element("Report").GetElementValue("Rgroup", "");
                    if (!string.IsNullOrEmpty(Description))
                    {
                        Hashtable p = MonthDescriptionValueEngine.MonthDescriptionValueService.GetMonthDescriptionValue(RptList, rpt._System.ID, base.CurrentUserName);
                        foreach (string key in p.Keys)
                        {
                            Description = Description.Replace("【" + key + "】", p[key].ToString());
                        }

                        RptModel.Description = Description;
                        B_MonthlyreportOperator.Instance.UpdateMonthlyreport(RptModel);
                    }
                }
            }

            return Description;
        }

        /// <summary>
        /// 获取月报说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public string GetMonthTRptDescription(string rpts)
        {
            string Description = string.Empty;
            XElement element = null;

            B_MonthlyReport RptModel = null;

            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

            RptModel = rpt.LastestMonthlyReport;

            List<MonthlyReportDetail> RptList = new List<MonthlyReportDetail>();

            if (rpt.LastestMonthlyReportDetails.Count <= 0)
            {
                return "";
            }

            rpt.LastestMonthlyReportDetails.ForEach(p => RptList.Add(p.ToVModel()));

            if (rpt._System.ID != Guid.Empty)
            {
                C_System sysModel = StaticResource.Instance[rpt._System.ID, DateTime.Now];
                element = sysModel.Configuration;
                if (element.Elements("Report").Elements("Rgroup") != null)
                {
                    Description = element.Element("Report").GetElementValue("Rgroup", "");
                    if (!string.IsNullOrEmpty(Description))
                    {
                        Hashtable p = MonthDescriptionValueEngine.MonthDescriptionValueService.GetMonthDescriptionValue(RptList, rpt._System.ID, base.CurrentUserName);
                        foreach (string key in p.Keys)
                        {
                            Description = Description.Replace("【" + key + "】", p[key].ToString());
                        }

                        //首先将月度报告入库
                        RptModel.Description = Description;
                        B_MonthlyreportOperator.Instance.UpdateMonthlyreport(RptModel);
                    }
                }
            }


            return Description;
        }

        [LibAction]
        public Guid ModifyMonthTRptDescription(string rpts)
        {
            B_MonthlyReport report = JsonHelper.Deserialize<B_MonthlyReport>(rpts);
            report.Status = 5;
            return B_MonthlyreportOperator.Instance.UpdateMonthlyreport(report);
        }



        [LibAction]
        public int ModifyMissTargetRptInfo(string info)
        {
            int result = 0;
            B_MonthlyReportDetail detail = JsonHelper.Deserialize<B_MonthlyReportDetail>(info);
            
            B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(detail);
            
            //保存Json , 开始

            //ReportInstance rpt = new ReportInstance(detail.MonthlyReportID, true);

            //SaveJsonData(detail.MonthlyReportID, rpt);

            //保存Json ，结束
            return result;
        }

        /// <summary>
        /// 保存，修改后明细的数据JSon需要更新
        /// </summary>
        /// <param name="MonthReportID">月报ID</param>
        /// <param name="CurrentRpt"></param>
        /// <param name="ListObj"></param>
        private void SaveJsonData(Guid MonthReportID, ReportInstance CurrentRpt )
        {
            B_MonthlyReportJsonData Update_JsonData;

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

                List<DictionaryVmodel> ListObj ;

                if (string.IsNullOrEmpty(Update_JsonData.ReportJsonData)) //新增
                {//这是上报页面的Json 数据     

                    ListObj = new List<DictionaryVmodel>();
                    ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                    ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                    ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                    ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(CurrentRpt)));
                    ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                }
                else
                { //编辑数据
                    ListObj  = JsonHelper.Deserialize<List<DictionaryVmodel>>(Update_JsonData.ReportJsonData);
                    ListObj.ForEach(p => {
                        if (p.Name == "Misstarget")
                        {
                            p.ObjValue = GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true);
                        }
                    });
                }
               
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
                ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(CurrentRpt)));
                ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);
            }


        }






        //修改明细数据 ********************************************************************************************


        [LibAction]
        public List<DictionaryVmodel> UpdateMonthReportDetail(string rpts, string strMonthReportOrderType, string info, bool IncludeHaveDetail, string strMonthReportID)
        {
            List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();
            MonthlyReportDetail detail = JsonHelper.Deserialize<MonthlyReportDetail>(info);
            B_MonthlyReportDetail B_detail = CalculationEvaluationEngine.CalculationEvaluationService.Calculation(detail.ToBModel(), "");
            B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(B_detail);

            
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            rpt.ReportDetails.Remove(rpt.ReportDetails.Find(p => p.ID == detail.ID));
            rpt.ReportDetails.Add(B_detail.ToVModel());

            ListObj.Add(new DictionaryVmodel("ReportInstance", rpt));
            ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(rpt, strMonthReportOrderType, IncludeHaveDetail)));
            ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(rpt, strMonthReportID, true)));
            ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(rpt)));
            ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(rpt, strMonthReportID.ToString(), true)));

            SaveJsonData(strMonthReportID.ToGuid(), rpt, ListObj);


            return ListObj;

        }
        

        /// <summary>
        /// 集团总部编辑
        /// </summary>
        /// <param name="rpts"></param>
        /// <param name="strMonthReportOrderType"></param>
        /// <param name="info"></param>
        /// <param name="IncludeHaveDetail"></param>
        /// <param name="strMonthReportID"></param>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> UpGroupdateMonthReportDetail(string rpts, string strMonthReportOrderType, string info, bool IncludeHaveDetail, string strMonthReportID)
        {
            List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

            List<MonthlyReportDetail> lstMrd = null;


            V_GroupCompany v_GroupCompany = JsonHelper.Deserialize<V_GroupCompany>(info);
            lstMrd = v_GroupCompany.ListGroupTargetDetail;
            if (lstMrd == null)
            {
                GroupDictionaryVmodel gdv = JsonHelper.Deserialize<GroupDictionaryVmodel>(info);
                lstMrd = (List<MonthlyReportDetail>)gdv.Value;
            }

            List<B_MonthlyReportDetail> lstBMrd = new List<B_MonthlyReportDetail>();

            for (int i = 0; i < lstMrd.Count; i++)
            {
                if (lstMrd[i].ID != Guid.Empty)
                {
                    B_MonthlyReportDetail B_detail = CalculationEvaluationEngine.CalculationEvaluationService.Calculation(lstMrd[i].ToBModel(), "");
                    if (B_detail.CompanyID != Guid.Empty)
                    {
                        B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(B_detail);
                    }
                    lstBMrd.Add(B_detail);
                }
            }

            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

            for (int j = 0; j < lstBMrd.Count; j++)
            {
                rpt.ReportDetails.Remove(rpt.ReportDetails.Find(p => p.ID == lstBMrd[j].ID));
                rpt.ReportDetails.Add(lstBMrd[j].ToVModel());
            }
            ListObj.Add(new DictionaryVmodel("ReportInstance", rpt));
            ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(rpt, strMonthReportOrderType, IncludeHaveDetail)));
            ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(rpt, strMonthReportID, true)));
            ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(rpt)));
            ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(rpt, strMonthReportID.ToString(), true)));

            SaveJsonData(strMonthReportID.ToGuid(), rpt, ListObj);

            return ListObj;
        }


        /// <summary>
        /// 直管总部编辑
        /// </summary>
        /// <param name="rpts"></param>
        /// <param name="strMonthReportOrderType"></param>
        /// <param name="info"></param>
        /// <param name="IncludeHaveDetail"></param>
        /// <param name="strMonthReportID"></param>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> UpdateDirectlyMonthReportDetail(string rpts, string info, string strMonthReportID)
        {
            List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            MonthReportSummaryViewModel mrsvm = JsonHelper.Deserialize<MonthReportSummaryViewModel>(info);
            MonthlyReportDetail mrd = rpt.ReportDetails.Find(p => p.ID == mrsvm.MonthlyDetailID);
            if (mrd != null)
            {
                MonthlyReportDetail tempMRD = mrd;
                tempMRD.NActualAmmount = (decimal)mrsvm.NActualAmmount;
                B_MonthlyReportDetail B_detail = CalculationEvaluationEngine.CalculationEvaluationService.Calculation(tempMRD.ToBModel(), "");
                B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(B_detail);
                rpt.ReportDetails.Remove(mrd);
                rpt.ReportDetails.Add(B_detail.ToVModel());
            }
            ListObj.Add(new DictionaryVmodel("ReportInstance", rpt));
            ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(rpt, "", false)));
            ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(rpt, strMonthReportID, true)));
            ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(rpt)));
            ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(rpt, strMonthReportID.ToString(), true)));

            SaveJsonData(strMonthReportID.ToGuid(), rpt, ListObj);

            return ListObj;
        }

        /// <summary>
        /// 项目公司的修改数据
        /// </summary>
        /// <param name="rpts"></param>
        /// <param name="strMonthReportOrderType"></param>
        /// <param name="info"></param>
        /// <param name="IncludeHaveDetail"></param>
        /// <param name="strMonthReportID"></param>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> UpdateProMonthReportDetail(string rpts, string strMonthReportOrderType, string info, bool IncludeHaveDetail, string strMonthReportID)
        {
            List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

            //首先拿到数据
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

            //在难道需要编辑的数据
            V_ProjectCompany ProDetail = JsonHelper.Deserialize<V_ProjectCompany>(info);

            //循环指标
            foreach (V_ProjectTarget item in ProDetail.ProjectTargets)
            {
                //获取修改的实体
                B_MonthlyReportDetail B_detail = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetail(item.ProMonthlyReportDetailID);

                if (B_detail != null)
                {
                    B_detail.NActualAmmount = item.NActualAmmount; //当月实际值
                    B_detail.NAccumulativeActualAmmount = item.NAccumulativeActualAmmount; //当月实际累计值
                }
                //重新计算
                B_MonthlyReportDetail UpdateData = CalculationEvaluationEngine.CalculationEvaluationService.Calculation(B_detail, "");
                //修改数据
                B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(UpdateData);

                rpt.ReportDetails.Remove(rpt.ReportDetails.Find(p => p.ID == B_detail.ID));
                rpt.ReportDetails.Add(B_detail.ToVModel());
            }

            ListObj.Add(new DictionaryVmodel("ReportInstance", rpt));
            ListObj.Add(new DictionaryVmodel("MonthDetail", GetTargetDetailList(rpt, strMonthReportOrderType, IncludeHaveDetail)));
            ListObj.Add(new DictionaryVmodel("Misstarget", GetMissTargetList(rpt, strMonthReportID, true)));
            ListObj.Add(new DictionaryVmodel("MonthReportDescription", GetMonthTRptDescription(rpt)));
            ListObj.Add(new DictionaryVmodel("CurrentMissTargetList", GetCurrentMissTargetList(rpt, strMonthReportID.ToString(), true)));

            SaveJsonData(strMonthReportID.ToGuid(), rpt, ListObj);

            return ListObj;
        }


        //修改明细数据   End  ********************************************************************************************


        /// <summary>
        /// 保存，修改后明细的数据JSon需要更新
        /// </summary>
        /// <param name="MonthReportID">月报ID</param>
        /// <param name="CurrentRpt"></param>
        /// <param name="ListObj"></param>
        private void SaveJsonData(Guid MonthReportID , ReportInstance CurrentRpt , List<DictionaryVmodel> ListObj)
        {
            B_MonthlyReportJsonData Update_JsonData;

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

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);
            }


        }




        /// <summary>
        /// 附件删除
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [LibAction]
        public string Delete(string data)
        {

            if (string.IsNullOrEmpty(data)) return null;
            Guid AttachmentID = Guid.Parse(data);
            Guid res = B_AttachmentOperator.Instance.RemoveAttachment(AttachmentID);
            if (res == AttachmentID)
            {
                return "t";
            }
            return res.ToString();
        }

        /// <summary>
        /// 获取附件列表
        /// </summary>
        /// <param name="businessID"></param>
        /// <returns></returns>
        [LibAction]
        public IList<B_Attachment> GetAttachments(string businessID)
        {

            if (string.IsNullOrEmpty(businessID)) return null;

            Guid _businessID = Guid.Parse(businessID);
            IList<B_Attachment> list = B_AttachmentOperator.Instance.GetAttachmentList(_businessID, "月报上传");
            return list;
        }


        /// <summary>
        /// 项目公司批次保存
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<V_SubReport> ProSystemSave(string rpts, string strMonthReportID)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);

            B_SystemBatch BatchModel = null;
            B_MonthlyReport bmr = null;

            List<V_SubReport> V_SubReportList = null;


            if (!string.IsNullOrEmpty(strMonthReportID))
            {
                //获取月报主表
                bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(strMonthReportID.ToGuid());

                if (bmr != null)
                {
                    if (bmr.SystemBatchID != Guid.Empty)
                    {
                        //获取批次实体
                        BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(bmr.SystemBatchID);

                        B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();

                        _bMonthlyReportAction.SystemID = rpt._SystemID;
                        _bMonthlyReportAction.MonthlyReportID = strMonthReportID.ToGuid();
                        _bMonthlyReportAction.FinYear = bmr.FinYear;
                        _bMonthlyReportAction.FinMonth = bmr.FinMonth;
                        _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Save);
                        _bMonthlyReportAction.Operator = System.Web.HttpContext.Current.User.Identity.Name;
                        _bMonthlyReportAction.OperatorTime = DateTime.Now;
                        _bMonthlyReportAction.ModifierName = System.Web.HttpContext.Current.User.Identity.Name;
                        _bMonthlyReportAction.CreatorName = System.Web.HttpContext.Current.User.Identity.Name;

                        B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);

                        //if (BatchModel != null)
                        //{
                        //    //批次是草稿状态
                        //    V_SubReportList = JsonHelper.Deserialize<List<V_SubReport>>(BatchModel.SubReport);
                        //    foreach (var item in V_SubReportList)
                        //    {
                        //        //选择的是那个系统？
                        //        if (item.SystemID == rpt._SystemID)
                        //        {
                        //            //根据选择的系统
                        //            item.IsReady = false;
                        //        }

                        //    }
                        //    BatchModel.SubReport = JsonHelper.Serialize(V_SubReportList);
                        //}

                        //BatchModel.ID = B_SystemBatchOperator.Instance.UpdateSystemBatch(BatchModel);

                    }

                }
            }



            //获取批次
            //BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch("ProSystem", rpt.FinYear, rpt.FinMonth);


            if (BatchModel != null)
                return V_SubReportList;
            else
            {
                V_SubReportList = new List<V_SubReport>();
                V_SubReportList.Add(new V_SubReport { SystemName = rpt._System.SystemName, ReportID = bmr.ID, SystemID = rpt._System.ID, IsReady = false });
                return V_SubReportList;
            }
        }

        /// <summary>
        ///获取上报页面的操作状态
        /// </summary>
        /// <param name="MontlyReportID"></param>
        /// <returns></returns>
        [LibAction]
        public IList<B_MonthlyReportAction> GetMonthlyReportActionList(string MonthlyReportID)
        {
            IList<B_MonthlyReportAction> listMonthlyReportAction = B_MonthlyReportActionOperator.Instance.GetMonthlyReportActionList(MonthlyReportID.ToGuid());
            return listMonthlyReportAction;
        }

        /// <summary>
        /// 获取批次的实体（这个是为工作流日志准备的，汇总后审批）
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <returns></returns>
        [LibAction]
        public B_SystemBatch GetSystemBatchByID(string BusinessID)
        {
            if (!string.IsNullOrEmpty(BusinessID))
            {
                return B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());
            }
            else
            {
                return new B_SystemBatch();
            }
        }


        /// <summary>
        /// 通过月报ID，获取批次的实体（这个是为工作流日志准备的，汇总后退回）
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <returns></returns>
        [LibAction]
        public B_SystemBatch GetSysBatchByMonthReportID(string BusinessID)
        {
            if (!string.IsNullOrEmpty(BusinessID))
            {
                B_MonthlyReport MonthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
                if (MonthRpt != null && MonthRpt.SystemBatchID != Guid.Empty)
                    return B_SystemBatchOperator.Instance.GetSystemBatch(MonthRpt.SystemBatchID);
                else
                    return new B_SystemBatch();
            }
            else
            {
                return new B_SystemBatch();
            }
        }



    }
}
