using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
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
using LJTH.BusinessIndicators.Web;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class MonthlyReportController : BaseController
    {
        /// <summary>
        /// 补回说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetTargetReturnList(string rpts, bool IsLatestVersion)
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
        public List<DictionaryVmodel> GetMissTargetList(string rpts, bool IsLatestVersion)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            List<DictionaryVmodel> MissTargetData= new List<DictionaryVmodel>();
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
                else {

                    MissTargetData = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                }
            }
            return MissTargetData;
        }


        /// <summary>
        /// 当月未完成说明
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetCurrentMissTargetList(string rpts, bool IsLatestVersion)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            List<DictionaryVmodel> CurrentMissTargetData = new List<DictionaryVmodel>();
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
                if (B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerryCurrentMissJsonData))
                {
                    CurrentMissTargetData = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerryCurrentMissJsonData);
                }
                else
                {
                    CurrentMissTargetData = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);
                }
            }
            return CurrentMissTargetData;
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
        public List<DictionaryVmodel> GetReportInstance(string SystemID, int Year, int Month, bool IsLatestVersion = false, string DataSource = "Draft",bool IsAll=false)
        {
            ReportInstance rpt = null;

            List<DictionaryVmodel> listSRDS = null;

            if (!IsLatestVersion) // 是否从A表中获取数据
            {
                //获取A表的数据

                B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();

                var A_Rpt = A_MonthlyreportOperator.Instance.GetAMonthlyReport(SystemID.ToGuid(), Year, Month);

                if (A_Rpt != null)
                {
                    listSRDS = GetJsonData(A_Rpt.ID);

                    if (listSRDS != null)
                    {
                        return listSRDS;
                    }
                    else
                    {
                        rpt = new ReportInstance(SystemID.ToGuid(), Year, Month, IsLatestVersion, DataSource, IsAll);
                        listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                        return listSRDS;
                    }
                }
                else
                {

                    rpt = new ReportInstance(SystemID.ToGuid(), Year, Month, IsLatestVersion, DataSource, IsAll);
                    listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                    return listSRDS;
                }


            }
            else
            {// 从B中获取数据

                if (DataSource == "Progress")
                {
                    //Progress 这里只有审批中的数据
                    var B_Rpt = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(SystemID.ToGuid(), Year, Month);

                    if (B_Rpt != null)
                    {
                        listSRDS = GetJsonData(B_Rpt.ID);

                        if (listSRDS != null)
                        {
                            return listSRDS;
                        }
                        else
                        {
                            rpt = new ReportInstance(SystemID.ToGuid(), Year, Month, IsLatestVersion, DataSource, IsAll);
                            listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                            return listSRDS;
                        }
                    }
                    else
                    {
                        rpt = new ReportInstance(SystemID.ToGuid(), Year, Month, IsLatestVersion, DataSource, IsAll);
                        listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                        return listSRDS;
                    }

                }
                else
                {
                    //Draft  包含有草稿,但是不一定都是草稿的数据
                    var B_Rpt = B_MonthlyreportOperator.Instance.GetLastMonthlyReportList(SystemID.ToGuid(), Year, Month);

                    if (B_Rpt != null)
                    {
                        listSRDS = GetJsonData(B_Rpt.ID);

                        if (listSRDS != null)
                        {
                            return listSRDS;
                        }
                        else
                        {
                            rpt = new ReportInstance(SystemID.ToGuid(), Year, Month, IsLatestVersion, DataSource, IsAll);
                            listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                            return listSRDS;
                        }
                    }
                    else
                    {
                        rpt = new ReportInstance(SystemID.ToGuid(), Year, Month, IsLatestVersion, DataSource, IsAll);
                        listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                        return listSRDS;
                    }
                }

            }
            
        }


        private List<DictionaryVmodel> GetJsonData(Guid MonthlyReportID)
        {
            return null;
            //List<DictionaryVmodel> listSRDS = null;

            //B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();

            //try
            //{
            //    B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(MonthlyReportID);
            //}
            //catch (Exception)
            //{
            //    B_JsonData = null;
            //}

            ////获取 表中的JSon数据
            //if (B_JsonData != null && !string.IsNullOrEmpty( B_JsonData.QuerrySumJsonData))
            //{
            //    listSRDS = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerrySumJsonData);
            //}

            //return listSRDS;
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
                //B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();
                //try
                //{
                //    B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(rpt._MonthReportID);
                //}
                //catch (Exception)
                //{
                //    B_JsonData = null;
                //}

                ////获取 表中的JSon数据
                //if (B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerryDetaileJsonData) && strMonthReportOrderType == "Detail")
                //{
                //    AllData = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerryDetaileJsonData);
                //}
                //else
                //{
                    AllData = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, strCompanyProperty, strMonthReportOrderType, IncludeHaveDetail);
                //}
            }
            return AllData;
        }




        /// <summary>
        /// 经营报告明细
        /// </summary>  
        /// <param name="SystemID">系统ID</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="IsLatestVersion">包含审批中</param>
        /// <returns>ist<MonthReportSummaryViewModel></returns>
        [LibAction]
        public List<DictionaryVmodel> GetManageDetailRptDataSource(string rpts, string strCompanyProperty, string strMonthReportOrderType, bool IncludeHaveDetail)
        {
            ReportInstance rpt = JsonHelper.Deserialize<ReportInstance>(rpts);
            List<DictionaryVmodel> AllData = new List<DictionaryVmodel>();
             if (rpt._MonthReportID != Guid.Empty)
            {
                //B_MonthlyReportJsonData B_JsonData = new B_MonthlyReportJsonData();
                //try
                //{
                //    B_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(rpt._MonthReportID);
                //}
                //catch (Exception)
                //{
                //    B_JsonData = null;
                //}

                ////获取 表中的JSon数据
                //if ( B_JsonData != null && !string.IsNullOrEmpty(B_JsonData.QuerryDetaileJsonData) && strMonthReportOrderType == "Detail")
                //{
                //    AllData = JsonHelper.Deserialize<List<DictionaryVmodel>>(B_JsonData.QuerryDetaileJsonData);
                //}
                //else
                //{
                    //默认按照公司排序字段
                    AllData = ReportInstanceManageDetailEngine.ReportInstanceManageDetailService.GetManageDetailRptDataSource(rpt, strCompanyProperty, "", IncludeHaveDetail);
                //}
            }
            return AllData;
        }

        //商管专用
        [LibAction]
        public int UpdateTargetReturn(string info,string isday)
        {
            int result = 0;

            try
            {
                A_MonthlyReportDetail detail = JsonHelper.Deserialize<A_MonthlyReportDetail>(info);
                if (isday == "true")
                {
                    detail.IsMissTarget = false;
                    detail.IsDelayComplete = true;
                    detail.Counter = 0;


                    //detail.CurrentMonthCommitDate = null;
                }
                A_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(detail);
            }
            catch (Exception)
            {

                result = 100;
            }
         
            return result;
        }

        //其他项目
        [LibAction]
        public int UpdateTargetDetail(string info)
        {
            int result = 0;

            try
            {
                A_MonthlyReportDetail detail = JsonHelper.Deserialize<A_MonthlyReportDetail>(info);
                A_MonthlyreportdetailOperator.Instance.UpdateReportdetail(detail);
            }
            catch (Exception)
            {

                result = 100;
            }

            return result;
        }
        /// <summary>
        /// 上报日志明细
        /// </summary>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        [LibAction]
        public List<B_MonthlyReportAction> MonthlyReportAction(string SystemID,int year,int month)
        {
            List<B_MonthlyReportAction> ReportAction = null;
            ReportAction = B_MonthlyReportActionOperator.Instance.GetsystemctionList(SystemID.ToGuid(), year, month).ToList();
            return ReportAction;
        }
        [LibAction]
        public string GetMonthlyReportActionID(int year, int month, string SystemID)
        {
            string MonthlyRID = "";
            List<A_MonthlyReportDetail> listdetail = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(SystemID.ToGuid(), year, month).ToList();
            if (listdetail.Count > 0)
            {
                MonthlyRID = listdetail.FirstOrDefault().MonthlyReportID.ToString();
            }

            return MonthlyRID;
        }
        [LibAction]
        public string SendMessage(string sysid, int month, int year)
        {
            string ReturnMess = "";
            B_Subscription model = new B_Subscription();
            string name = HttpContext.Current.User.Identity.Name;
            //string name = "zhengguilong";
            //首先查出表中某年某月某人的数据
            List<B_Subscription> GetList = B_SubscriptionOperator.Instance.GeSubscriptionListByFile(sysid, month, year, name).ToList();
            if (GetList.Count > 0)//若存在数据,则修改即可
            {
                if (GetList[0].Operating == true)//若数据库中的为true  则改为false
                {
                    foreach (B_Subscription item in GetList)
                    {
                        item.Operating = false;
                        B_SubscriptionOperator.Instance.UpdateSubscription(item);
                        ReturnMess = "false";
                    }
                }
                else//若数据库中的为false  则改为true
                {
                    foreach (B_Subscription item in GetList)
                    {
                        item.Operating = true;
                        B_SubscriptionOperator.Instance.UpdateSubscription(item);
                        ReturnMess = "true";
                    }
                }
            }
            else//若不存在  则添加
            {
                model.ID = Guid.NewGuid();
                model.Operating = true;
                model.SystemID = sysid.ToGuid();
                model.FinMonth = month;
                model.FinYear = year;
                model.CreatorName = name;
                Guid sid = B_SubscriptionOperator.Instance.AddSubscription(model);
                if (sid != null)
                {
                    ReturnMess = "true";
                }
            }
            return ReturnMess;
        }
        [LibAction]
        public string SelectMessage(string sysid, int month, int year)
        {
            string ReturnMess = "";
            string name = HttpContext.Current.User.Identity.Name;
            //string name = "zhengguilong";
            //首先查出表中某年某月某人的数据
            List<B_Subscription> GetList = B_SubscriptionOperator.Instance.GeSubscriptionListByFile(sysid, month, year, name).ToList();
            if (GetList.Count > 0)
            {
                if (GetList[0].Operating == true)
                {
                    ReturnMess = "false";//已经订阅 显示取消订阅
                }
                else
                {

                    ReturnMess = "true";//没有订阅 ,显示订阅

                }
            }
            else
            {
                ReturnMess = "true";
            }
            return ReturnMess;
        }
        [LibAction]
        public List<B_MonthlyReport> GetMonthlyReportByTime(int Year, int Month)
        {
            List<B_MonthlyReport> GetList = B_MonthlyreportOperator.Instance.GetMonthlyReportByApproveList(Year, Month);
            return GetList;
        }

        [LibAction]
        public A_MonthlyReportDetail GetMonthLyRModel(string ID)
        {
            A_MonthlyReportDetail result = A_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetail(Guid.Parse(ID));
            return result;
        }
        [LibAction]
        public string GetNewMonthlyreport(int FinYear, int FinMonth, string SystemID)
        {
            string ID = "";
            B_MonthlyReport model = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(Guid.Parse(SystemID), FinYear, FinMonth);
            if (model!=null)
            {
                ID = model.ID.ToString();
            }
            return ID;

        }
    }
}
