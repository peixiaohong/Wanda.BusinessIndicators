using Lib.Core;
using Lib.Web;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using System.Web.Configuration;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class DepartmentStoreReportController : BaseController
    {
        Guid SysDescriptionID = WebConfigurationManager.AppSettings["MonthDescription"].ToGuid();//百货系统ID
        /// <summary>
        /// 新的方法
        /// </summary>
        /// <param name="c"></param>
        /// <param name="LastMonthReportDetails"></param>
        /// <param name="CurrentMonthReportDetails"></param>
        /// <param name="itemt"></param>
        /// <param name="IsCurrentMonth"></param>
        /// <returns></returns>
        public MonthlyReportDetail GetMonthlyReportDetail(C_Company c, List<MonthlyReportDetail> LastMonthReportDetails, List<MonthlyReportDetail> CurrentMonthReportDetails, C_Target itemt, bool IsCurrentMonth)
        {
            MonthlyReportDetail ReportDetailModel = new MonthlyReportDetail();
            
            if (IsCurrentMonth)
            {
                ReportDetailModel = CurrentMonthReportDetails.SingleOrDefault(t => t.TargetID == itemt.ID && t.CompanyID == c.ID);
            }
            else
            {
                ReportDetailModel = LastMonthReportDetails.SingleOrDefault(t => t.TargetID == itemt.ID && t.CompanyID == c.ID);
            }

            return ReportDetailModel;
        }


        /// <summary>
        /// 百货系统经营指标完成门店数量情况
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        [LibAction]
        public List<ShowDSTargetCompleted> GetDSTargetCompleted(int Year, int Month, bool IsLatestVersion)
        {
            List<ShowDSTargetCompleted> showList = new List<ShowDSTargetCompleted>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Company> CompanyList = C_CompanyOperator.Instance.GetCompanyList(SystemModel.ID).ToList();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();
                List<MonthlyReportDetail> LastMonthRptList = null;
                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthRptList = LastMonthReport.ReportDetails;
                }


                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthRptList = CurrentMonthReport.ReportDetails;
                   List<C_Target> TargetList=new List<C_Target>();
                if (CurrentMonthRptList!=null)
                {
                    TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, CurrentMonthRptList[0].CreateTime).Where(t => t.NeedReport == true && t.TargetName != "总部管理费用").ToList();

                }
                else
                {
                    TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部管理费用").ToList();

                }
                #region 判断完成了几个指标

                List<DSTargetCompleted> list = new List<DSTargetCompleted>();
                foreach (C_Company c in CompanyList)
                {
                    int LastCompletedCount = 0;
                    int CurCompletedCount = 0;
                    int ToCurCompletedCount = 0;
                    DSTargetCompleted DSmodel = new DSTargetCompleted();
                    DSmodel.CompanyName = c.CompanyName;
                    DSmodel.AreaName = c.CompanyProperty3;
                    foreach (C_Target itemt in TargetList)
                    {
                        if (Month > 1)
                        {
                            //上一个月
                            MonthlyReportDetail LastDetailModel = GetMonthlyReportDetail(c, LastMonthRptList,CurrentMonthRptList, itemt, false);
                            if (LastDetailModel != null && LastDetailModel.ID != Guid.Empty)
                            {
                                if (LastDetailModel.IsMissTarget)
                                {
                                    LastCompletedCount++;
                                }
                            }
                        }
                        //当前月
                        MonthlyReportDetail CurDetailModel = GetMonthlyReportDetail(c, LastMonthRptList, CurrentMonthRptList, itemt, true);
                        if (CurDetailModel != null && CurDetailModel.ID != Guid.Empty)
                        {
                            if (CurDetailModel.IsMissTargetCurrent)
                            {
                                CurCompletedCount++;
                            }
                            if (CurDetailModel.IsMissTarget)
                            {
                                ToCurCompletedCount++;
                            }
                        }
                    }

                    DSmodel.LastCount = LastCompletedCount;
                    DSmodel.CurrentCount = CurCompletedCount;
                    DSmodel.ToCurrentCount = ToCurCompletedCount;

                    if ( LastMonthRptList != null || CurrentMonthRptList.Count > 0)
                    {
                        list.Add(DSmodel);
                    }
                }
                //判断指标完成几个 
                if (list.Count >= 0)
                {
                    List<DSTargetCompleted> NorthList = list.Where(t => t.AreaName == "北区").ToList();
                    List<DSTargetCompleted> SouthList = list.Where(t => t.AreaName == "南区").ToList();
                    List<DSTargetCompleted> CenterList = list.Where(t => t.AreaName == "中区").ToList();

                    //单双指标名称的枚举List
                    EnumItemDescriptionList  enumList =  EnumItemDescriptionAttribute.GetDescriptionList(typeof(DSProjectType) );

                    for (int i = 0; i < enumList.Count; i++)
                    {
                        ShowDSTargetCompleted showModel = new ShowDSTargetCompleted();
                        showModel.PorjectName =  EnumHelper.GetEnumDescription(typeof(DSProjectType), i);

                        if (Month > 1 && LastMonthRptList != null && LastMonthRptList.Count > 0)
                        {
                            showModel.LastNorth = NorthList.Where(t => t.LastCount == i).ToList().Count;
                            showModel.LastCenter = CenterList.Where(t => t.LastCount == i).ToList().Count;
                            showModel.LastSouth = SouthList.Where(t => t.LastCount == i).ToList().Count;
                            showModel.LastTotal = showModel.LastNorth + showModel.LastCenter + showModel.LastSouth;
                        }
                        else
                        {
                            showModel.LastNorth = 0;
                            showModel.LastCenter = 0;
                            showModel.LastSouth = 0;
                            showModel.LastTotal = 0;
                        }

                        if (CurrentMonthRptList.Count > 0)
                        {
                            showModel.CurrentNorth = NorthList.Where(t => t.CurrentCount == i).ToList().Count;
                            showModel.CurrentCenter = CenterList.Where(t => t.CurrentCount == i).ToList().Count;
                            showModel.CurrentSouth = SouthList.Where(t => t.CurrentCount == i).ToList().Count;
                            showModel.CurrentTotal = showModel.CurrentNorth + showModel.CurrentCenter + showModel.CurrentSouth;

                            showModel.ToCurrentNorth = NorthList.Where(t => t.ToCurrentCount == i).ToList().Count;
                            showModel.ToCurrentCenter = CenterList.Where(t => t.ToCurrentCount == i).ToList().Count;
                            showModel.ToCurrentSouth = SouthList.Where(t => t.ToCurrentCount == i).ToList().Count;
                            showModel.ToCurrentTotal = showModel.ToCurrentNorth + showModel.ToCurrentCenter + showModel.ToCurrentSouth;
                        }
                        else
                        { 
                            showModel.CurrentNorth = 0;
                            showModel.CurrentCenter = 0;
                            showModel.CurrentSouth = 0;
                            showModel.CurrentTotal = 0;

                            showModel.ToCurrentNorth = 0;
                            showModel.ToCurrentCenter = 0;
                            showModel.ToCurrentSouth = 0;
                            showModel.ToCurrentTotal = 0;
                        }
                        showList.Add(showModel); 
                    }
                }
                #endregion
                //合计
                ShowDSTargetCompleted hjshowModel = new ShowDSTargetCompleted();
                hjshowModel.PorjectName = "合计";
                hjshowModel.LastNorth = showList.Sum(t => t.LastNorth);
                hjshowModel.LastCenter = showList.Sum(t => t.LastCenter);
                hjshowModel.LastSouth = showList.Sum(t => t.LastSouth);
                hjshowModel.LastTotal = showList.Sum(t => t.LastTotal);
                hjshowModel.CurrentNorth = showList.Sum(t => t.CurrentNorth);
                hjshowModel.CurrentCenter = showList.Sum(t => t.CurrentCenter);
                hjshowModel.CurrentSouth = showList.Sum(t => t.CurrentSouth);
                hjshowModel.CurrentTotal = showList.Sum(t => t.CurrentTotal);
                hjshowModel.ToCurrentNorth = showList.Sum(t => t.ToCurrentNorth);
                hjshowModel.ToCurrentCenter = showList.Sum(t => t.ToCurrentCenter);
                hjshowModel.ToCurrentSouth = showList.Sum(t => t.ToCurrentSouth);
                hjshowModel.ToCurrentTotal = showList.Sum(t => t.ToCurrentTotal);
                showList.Add(hjshowModel);
                #region 判断哪个指标未完成
                List<DSTargetCompleted> MissTargetlist = new List<DSTargetCompleted>();
                foreach (C_Target itemt in TargetList.OrderBy(T => T.Sequence))
                {
                    #region//北区
                    List<int> NorthList = GetMissTargetCountList(CompanyList, LastMonthRptList, CurrentMonthRptList, itemt, "北区", Month);
                    #endregion

                    #region//中区
                    List<int> CenterList = GetMissTargetCountList(CompanyList, LastMonthRptList, CurrentMonthRptList, itemt, "中区", Month);
                    #endregion
                    #region//南区
                    List<int> SouthList = GetMissTargetCountList(CompanyList, LastMonthRptList, CurrentMonthRptList, itemt, "南区", Month);
                    #endregion


                    ShowDSTargetCompleted ShowDSmodel = new ShowDSTargetCompleted();
                    ShowDSmodel.PorjectName = itemt.TargetName + "未完成门店数";
                    ShowDSmodel.LastNorth = NorthList[0];
                    ShowDSmodel.LastCenter = CenterList[0]; ;
                    ShowDSmodel.LastSouth = SouthList[0]; ;
                    ShowDSmodel.LastTotal = ShowDSmodel.LastNorth + ShowDSmodel.LastCenter + ShowDSmodel.LastSouth;
                    ShowDSmodel.CurrentNorth = NorthList[1];
                    ShowDSmodel.CurrentCenter = CenterList[1];
                    ShowDSmodel.CurrentSouth = SouthList[1];
                    ShowDSmodel.CurrentTotal = ShowDSmodel.CurrentNorth + ShowDSmodel.CurrentCenter + ShowDSmodel.CurrentSouth;
                    ShowDSmodel.ToCurrentNorth = NorthList[2];
                    ShowDSmodel.ToCurrentCenter = CenterList[2];
                    ShowDSmodel.ToCurrentSouth = SouthList[2];
                    ShowDSmodel.ToCurrentTotal = ShowDSmodel.ToCurrentNorth + ShowDSmodel.ToCurrentCenter + ShowDSmodel.ToCurrentSouth;
                    showList.Add(ShowDSmodel);
                }
                #endregion
            }
            return showList;
        }
        
        /// <summary>
        /// 获取每一个区域的每一个未完成的指标的店铺的数量
        /// </summary>
        /// <param name="CompanyList"></param>
        /// <param name="LastMonthReportDetails"></param>
        /// <param name="BLastMonthReportDetails"></param>
        /// <param name="CurrentMonthReportDetails"></param>
        /// <param name="BCurrentMonthReportDetails"></param>
        /// <param name="itemt"></param>
        /// <param name="AreaName"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public List<int> GetMissTargetCountList(List<C_Company> CompanyList, List<MonthlyReportDetail> LastMonthReportDetails, List<MonthlyReportDetail> CurrentMonthReportDetails,
            C_Target itemt, string AreaName, int Month)
        {
            List<int> MissTargetCountList = new List<int>();
            int NorthLastMissCount = 0;
            int NorthCurMissCount = 0;
            int NorthToCurMissCount = 0;
            List<C_Company> NorthCompanyList = CompanyList.Where(t => t.CompanyProperty3 == AreaName).ToList();
            foreach (C_Company c in NorthCompanyList)
            {
                if (Month > 1)
                {
                    //上一个月
                    MonthlyReportDetail LastDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails,  CurrentMonthReportDetails, itemt, false);
                    if (LastDetailModel != null && LastDetailModel.ID != Guid.Empty)
                    {
                        if (LastDetailModel.IsMissTarget)
                        {
                            NorthLastMissCount++;
                        }
                    }
                }
                //当前月
                MonthlyReportDetail CurDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, true);
                if (CurDetailModel != null && CurDetailModel.ID != Guid.Empty)
                {
                    if (CurDetailModel.IsMissTargetCurrent)
                    {
                        NorthCurMissCount++;
                    }
                    if (CurDetailModel.IsMissTarget)
                    {
                        NorthToCurMissCount++;
                    }
                }
            }
            MissTargetCountList.Add(NorthLastMissCount);
            MissTargetCountList.Add(NorthCurMissCount);
            MissTargetCountList.Add(NorthToCurMissCount);
            return MissTargetCountList;
        }
       
        /// <summary>
        /// 百货系统经营指标完成情况对比
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        [LibAction]
        public List<ShowDSTargetArea> GetDSTargetCompletedDetail(int Year, int Month, bool IsLatestVersion)
        {
            List<ShowDSTargetArea> Showlist = new List<ShowDSTargetArea>();
            //计算完成率的集合
            List<MonthReportSummaryViewModel> listMonthReportSummaryViewModel = new List<MonthReportSummaryViewModel>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部管理费用").ToList();
                List<C_Company> CompanyList = C_CompanyOperator.Instance.GetCompanyList(SystemModel.ID).ToList();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = null;
                List<MonthlyReportDetail> LastMonthRptList = null;
              
                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    //这是上月的数据
                    LastMonthRptList = LastMonthReport.ReportDetails;
                }

                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                
                //当月的数据
                List<MonthlyReportDetail> CurrentMonthRptList = CurrentMonthReport.ReportDetails;

                int RateIndex = 1;
                for (int i = 1; i < EnumUtil.GetItems(typeof(DSDetailAreaName)).Count + 1; i++)
                {
                    ShowDSTargetArea DSTargetArea = new ShowDSTargetArea();
                    DSTargetArea.ID = i;
                    DSTargetArea.AreaName = EnumUtil.GetEnumDescription(typeof(DSDetailAreaName), i);
                    if (i < 4)
                    {
                        List<ShowDSTargetCompletedDetail> list = new List<ShowDSTargetCompletedDetail>();
                        foreach (C_Target itemt in TargetList.OrderBy(t => t.Sequence))
                        {
                            //计算完成率实体（上一个月）
                            MonthReportSummaryViewModel Lastmrsvm = new MonthReportSummaryViewModel();
                            //计算完成率实体（当前月及其累计）
                            MonthReportSummaryViewModel mrsvm = new MonthReportSummaryViewModel();
                            decimal LastSumPlan = 0;
                            decimal LastSumActual = 0;
                            decimal CurrentSumPlan = 0;
                            decimal CurrentSumActual = 0;
                            decimal CurrentSumAccumulativePlan = 0;
                            decimal CurrentSumAccumulativeActual = 0;
                            ShowDSTargetCompletedDetail DSModel = new ShowDSTargetCompletedDetail();
                            DSModel.DetailAreaID = i;
                            DSModel.DetailTargetName = itemt.TargetName;
                            List<C_Company> NorthCompanyList = CompanyList.Where(t => t.CompanyProperty3 == DSTargetArea.AreaName).ToList();
                            foreach (C_Company c in NorthCompanyList)
                            {
                                if (Month > 1)
                                {
                                    //上一个
                                    MonthlyReportDetail LastReportDetailModel = GetMonthlyReportDetail(c, LastMonthRptList, CurrentMonthRptList, itemt, false);
                                    if (LastReportDetailModel != null && LastReportDetailModel.ID != Guid.Empty)
                                    {
                                        LastSumPlan += LastReportDetailModel.NPlanAmmount;
                                        LastSumActual += LastReportDetailModel.NActualAmmount;
                                    }
                                }
                                //当前月以及当前月累计
                                MonthlyReportDetail CurrentReportDetailModel = GetMonthlyReportDetail(c, LastMonthRptList, CurrentMonthRptList,itemt, true);
                                if (CurrentReportDetailModel != null && CurrentReportDetailModel.ID != Guid.Empty)
                                {
                                    CurrentSumPlan += CurrentReportDetailModel.NPlanAmmount;
                                    CurrentSumActual += CurrentReportDetailModel.NActualAmmount;
                                    CurrentSumAccumulativePlan += CurrentReportDetailModel.NAccumulativePlanAmmount;
                                    CurrentSumAccumulativeActual += CurrentReportDetailModel.NAccumulativeActualAmmount;
                                }
                            }
                            DSModel.LastPlan = Convert.ToDecimal(LastSumPlan.ToString("N2"));
                            DSModel.LastActual = Convert.ToDecimal(LastSumActual.ToString("N2"));
                            DSModel.LastDifference = Convert.ToDecimal((LastSumActual - LastSumPlan).ToString("N2"));
                            DSModel.CurrentPlan = Convert.ToDecimal(CurrentSumPlan.ToString("N2"));
                            DSModel.CurrentActual = Convert.ToDecimal(CurrentSumActual.ToString("N2"));
                            DSModel.CurrentDifference = Convert.ToDecimal((CurrentSumActual - CurrentSumPlan).ToString("N2"));
                            DSModel.ToCurrentPlan = Convert.ToDecimal(CurrentSumAccumulativePlan.ToString("N2"));
                            DSModel.ToCurrentActual = Convert.ToDecimal(CurrentSumAccumulativeActual.ToString("N2"));
                            DSModel.ToCurrentDifference = Convert.ToDecimal((CurrentSumAccumulativeActual - CurrentSumAccumulativePlan).ToString("N2"));
                            //计算完成率(上一个月)
                            Lastmrsvm.ID = RateIndex;
                            Lastmrsvm.TargetID = itemt.ID;
                            Lastmrsvm.SystemID = itemt.SystemID;
                            Lastmrsvm.TargetName = itemt.TargetName;
                            Lastmrsvm.NPlanAmmount = (double)(DSModel.LastPlan);
                            Lastmrsvm.NActualAmmount = (double)(DSModel.LastActual);
                            Lastmrsvm.FinYear = Year;
                            Lastmrsvm.MeasureRate = "1";
                            
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(Lastmrsvm));
                            //计算完成率(当前月及其累计)
                            mrsvm.ID = RateIndex + 1;
                            mrsvm.TargetID = itemt.ID;
                            mrsvm.SystemID = itemt.SystemID;
                            mrsvm.TargetName = itemt.TargetName;
                            mrsvm.NPlanAmmount = (double)(DSModel.CurrentPlan);
                            mrsvm.NActualAmmount = (double)(DSModel.CurrentActual);
                            mrsvm.NAccumulativePlanAmmount = (double)(DSModel.ToCurrentPlan);
                            mrsvm.NAccumulativeActualAmmount = (double)(DSModel.ToCurrentActual);
                            mrsvm.FinYear = Year;
                            mrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                            //显示集合
                            list.Add(DSModel);
                        }
                        DSTargetArea.DetailList = list;
                        Showlist.Add(DSTargetArea);
                    }
                    if (i == 4)
                    {
                        List<ShowDSTargetCompletedDetail> Totallist = new List<ShowDSTargetCompletedDetail>();
                        foreach (C_Target itemt in TargetList.OrderBy(t => t.Sequence))
                        {
                            //计算完成率实体（上一个月）
                            MonthReportSummaryViewModel Lastmrsvm = new MonthReportSummaryViewModel();
                            //计算完成率实体（当前月及其累计）
                            MonthReportSummaryViewModel mrsvm = new MonthReportSummaryViewModel();
                            decimal LastSumPlan = 0;
                            decimal LastSumActual = 0;
                            decimal CurrentSumPlan = 0;
                            decimal CurrentSumActual = 0;
                            decimal CurrentSumAccumulativePlan = 0;
                            decimal CurrentSumAccumulativeActual = 0;
                            foreach (ShowDSTargetArea area in Showlist)
                            {
                                foreach (ShowDSTargetCompletedDetail item in area.DetailList)
                                {
                                    if (item.DetailAreaID != i && item.DetailTargetName == itemt.TargetName)
                                    {
                                        LastSumPlan += item.LastPlan;
                                        LastSumActual += item.LastActual;
                                        CurrentSumPlan += item.CurrentPlan;
                                        CurrentSumActual += item.CurrentActual;
                                        CurrentSumAccumulativePlan += item.ToCurrentPlan;
                                        CurrentSumAccumulativeActual += item.ToCurrentActual;
                                    }
                                }
                            }
                            ShowDSTargetCompletedDetail DSModel = new ShowDSTargetCompletedDetail();
                            DSModel.DetailAreaID = i;
                            DSModel.DetailTargetName = itemt.TargetName;
                            DSModel.LastPlan = Convert.ToDecimal(LastSumPlan.ToString("N2"));
                            DSModel.LastActual = Convert.ToDecimal(LastSumActual.ToString("N2"));
                            DSModel.LastDifference = Convert.ToDecimal((LastSumActual - LastSumPlan).ToString("N2"));
                            DSModel.CurrentPlan = Convert.ToDecimal(CurrentSumPlan.ToString("N2"));
                            DSModel.CurrentActual = Convert.ToDecimal(CurrentSumActual.ToString("N2"));
                            DSModel.CurrentDifference = Convert.ToDecimal((CurrentSumActual - CurrentSumPlan).ToString("N2"));
                            DSModel.ToCurrentPlan = Convert.ToDecimal(CurrentSumAccumulativePlan.ToString("N2"));
                            DSModel.ToCurrentActual = Convert.ToDecimal(CurrentSumAccumulativeActual.ToString("N2"));
                            DSModel.ToCurrentDifference = Convert.ToDecimal((CurrentSumAccumulativeActual - CurrentSumAccumulativePlan).ToString("N2"));
                            //计算完成率(上一个月)
                            Lastmrsvm.ID = RateIndex;
                            Lastmrsvm.TargetID = itemt.ID;
                            Lastmrsvm.SystemID = itemt.SystemID;
                            Lastmrsvm.TargetName = itemt.TargetName;
                            Lastmrsvm.NPlanAmmount = (double)(DSModel.LastPlan);
                            Lastmrsvm.NActualAmmount = (double)(DSModel.LastActual);
                            Lastmrsvm.FinYear = Year;
                            Lastmrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(Lastmrsvm));
                            //计算完成率(当前月及其累计)
                            mrsvm.ID = RateIndex + 1;
                            mrsvm.TargetID = itemt.ID;
                            mrsvm.SystemID = itemt.SystemID;
                            mrsvm.TargetName = itemt.TargetName;
                            mrsvm.NPlanAmmount = (double)(DSModel.CurrentPlan);
                            mrsvm.NActualAmmount = (double)(DSModel.CurrentActual);
                            mrsvm.NAccumulativePlanAmmount = (double)(DSModel.ToCurrentPlan);
                            mrsvm.NAccumulativeActualAmmount = (double)(DSModel.ToCurrentActual);
                            mrsvm.FinYear = Year;
                            mrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                            //显示集合
                            Totallist.Add(DSModel);
                        }
                        DSTargetArea.DetailList = Totallist;
                        Showlist.Add(DSTargetArea);
                    }
                }
            }
            //遍历集合修改完成率
            List<ShowDSTargetArea> sShowlist = new List<ShowDSTargetArea>();
            int d = 0;
            for (int b = 0; b < Showlist.Count; b++)
            {
                for (int c = 0; c < Showlist[b].DetailList.Count; c++)
                {
                    Showlist[b].DetailList[c].LastRate = listMonthReportSummaryViewModel[d].NActualRate;
                    d++;
                    Showlist[b].DetailList[c].CurrentRate = listMonthReportSummaryViewModel[d].NActualRate;
                    Showlist[b].DetailList[c].ToCurrentRate = listMonthReportSummaryViewModel[d].NAccumulativeActualRate;
                    d++;
                }
                sShowlist.Add(Showlist[b]);
            }
            return sShowlist;
        }

        /// <summary>
        /// 百货系统经营指标补回情况
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        [LibAction]
        public List<DSTargetReturnDataCompany> GetDSTargetReturnDataList(int Year, int Month, bool IsLatestVersion)
        {
            List<DSTargetReturnDataCompany> Showlist = new List<DSTargetReturnDataCompany>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部管理费用").ToList();
                List<C_Company> CompanyList = new List<C_Company>();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();
                List<MonthlyReportDetail> LastMonthReportDetails = null;

                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthReportDetails = LastMonthReport.ReportDetails;//.Where(p => p.ReturnType >= (int)EnumReturnType.Accomplish).ToList();  
                    CompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemID(Year, Month - 1, SystemModel.ID, 1, IsLatestVersion).ToList();
                }

                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthReportDetails = CurrentMonthReport.ReportDetails;//.Where(p => p.ReturnType >= (int)EnumReturnType.Accomplish).ToList();
              
                //新建百货系统经营指标补回情况的集合
                if (CompanyList != null && CompanyList.Count > 0)
                {
                    int i = 1;
                    foreach (C_Company c in CompanyList)
                    {
                        DSTargetReturnDataCompany DSCompany = new DSTargetReturnDataCompany();
                        DSCompany.ID = i;
                        DSCompany.CompanyName = c.CompanyName;
                        int isIsMissTargetCount = 0;
                        int isReturnCount = 0;

                        List<DSTargetReturnData> dataList = new List<DSTargetReturnData>();
                        foreach (C_Target ct in TargetList.OrderBy(t => t.Sequence))
                        {
                            DSTargetReturnData DSReturnData = new DSTargetReturnData();
                            DSReturnData.CompanyID = i;
                            DSReturnData.ReturnTargetName = ct.TargetName;
                            //上个月
                            MonthlyReportDetail LastDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, false);
                            if (LastDetail != null && LastDetail.ID != Guid.Empty)
                            {
                                DSReturnData.LastAccumulativePlan = Convert.ToDecimal(LastDetail.NAccumulativePlanAmmount.ToString("N2"));
                                DSReturnData.LastAccumulativeActual = Convert.ToDecimal(LastDetail.NAccumulativeActualAmmount.ToString("N2"));
                                DSReturnData.LastAccumulativeDifference = Convert.ToDecimal((LastDetail.NAccumulativeActualAmmount - LastDetail.NAccumulativePlanAmmount).ToString("N2"));
                            }
                            //当前月
                            MonthlyReportDetail CurrentDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, true);
                            if (CurrentDetail != null && CurrentDetail.ID != Guid.Empty)
                            {
                                DSReturnData.CurrentReturnAmount = Convert.ToDecimal((CurrentDetail.NAccumulativeActualAmmount - CurrentDetail.NAccumulativePlanAmmount).ToString("N2"));
                                DSReturnData.CurrentAccumulativePlan = Convert.ToDecimal(CurrentDetail.NAccumulativePlanAmmount.ToString("N2"));
                                DSReturnData.CurrentAccumulativeActual = Convert.ToDecimal(CurrentDetail.NAccumulativeActualAmmount.ToString("N2"));
                                DSReturnData.CurrentAccumulativeDifference = Convert.ToDecimal((CurrentDetail.NAccumulativeActualAmmount - CurrentDetail.NAccumulativePlanAmmount).ToString("N2"));
                                DSReturnData.CurrentAccumulativeRate = CurrentDetail.NAccumulativeDisplayRate.ToString();
                                //if (string.IsNullOrEmpty(CurrentDetail.NAccumulativeDisplayRate))
                                //{
                                //    DSReturnData.CurrentAccumulativeRate = ((CurrentDetail.NAccumulativeActualRate) * 100).ToString("N2") + "%";
                                //}
                                //else
                                //{
                                //    DSReturnData.CurrentAccumulativeRate = CurrentDetail.NAccumulativeDisplayRate.ToString() + ((CurrentDetail.NAccumulativeActualRate) * 100).ToString("N2") + "%";
                                //}
                                DSReturnData.CommitDate = string.Format("{0:yyyy-MM-dd}", CurrentDetail.CommitDate);
                                DSReturnData.ReturnType = CurrentDetail.ReturnType;
                                if (!string.IsNullOrEmpty(CurrentDetail.ReturnType.ToString()) && CurrentDetail.ReturnType > 0)
                                {
                                    DSReturnData.ReturnTypeDescrible = EnumUtil.GetEnumDescription(typeof(EnumReturnType), CurrentDetail.ReturnType);
                                }
                                else
                                {
                                    DSReturnData.ReturnTypeDescrible = "--";
                                }
                                DSReturnData.Counter = CurrentDetail.Counter;

                                if (CurrentDetail.ReturnType >= (int)EnumReturnType.Accomplish)
                                {
                                    isReturnCount++;
                                }

                                if (CurrentDetail.IsMissTarget == false)
                                {
                                    isIsMissTargetCount++;
                                }
                            }
                            dataList.Add(DSReturnData);
                        }

                        if (isIsMissTargetCount > 0)
                        {
                            if (isIsMissTargetCount == TargetList.Count)
                            {
                                if (isReturnCount > 0)
                                {
                                    DSCompany.IsAllReturn = true;
                                    DSCompany.ReturnDataList = dataList;
                                    Showlist.Add(DSCompany);
                                }
                            }
                            else
                            {
                                if (isReturnCount > 0)
                                {
                                    DSCompany.IsAllReturn = false;
                                    DSCompany.ReturnDataList = dataList;
                                    Showlist.Add(DSCompany);
                                }
                            }
                      
                            i++;
                        }
                    }
                }
            }
            return Showlist;
        }
        /// <summary>
        /// 百货系统经营指标新增未完成指标的门店情况
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        [LibAction]
        public List<DSTargetReturnDataCompany> GetDSTargetAddMissDataList(int Year, int Month, bool IsLatestVersion)
        {
            List<DSTargetReturnDataCompany> Showlist = new List<DSTargetReturnDataCompany>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部管理费用").ToList();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();
                List<MonthlyReportDetail> LastMonthReportDetails = null;
              
                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthReportDetails = LastMonthReport.ReportDetails;
                }

                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthReportDetails = CurrentMonthReport.ReportDetails;
              
                //新建百货系统新增经营指标未完成情况的集合
                int i = 1;
                foreach (C_Target titem in TargetList.OrderBy(t => t.Sequence))
                {
                    List<C_Company> CompanyList = new List<C_Company>();
                    if (Month == 1)
                    {
                        CompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemIDAndTargetID(Year, Month, SystemModel.ID, 1, titem.ID, IsLatestVersion).ToList();
                    }
                    else
                    {
                        List<C_Company> LastCompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemIDAndTargetID(Year, Month - 1, SystemModel.ID, 0, titem.ID, IsLatestVersion).ToList();
                        List<C_Company> CurrentCompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemIDAndTargetID(Year, Month, SystemModel.ID, 1, titem.ID, IsLatestVersion).ToList();
                        foreach (C_Company cc in LastCompanyList)
                        {
                            C_Company cModel = CurrentCompanyList.SingleOrDefault(t => t.ID == cc.ID);
                            if (cModel != null && cModel.ID != Guid.Empty)
                            {
                                CompanyList.Add(cModel);
                            }
                        }
                    }

                    #region 添加公司
                    
                    if (CompanyList != null && CompanyList.Count > 0)
                    {
                        foreach (C_Company c in CompanyList)
                        {
                            DSTargetReturnDataCompany DSCompany = new DSTargetReturnDataCompany();
                            DSCompany.ID = i;
                            DSCompany.CompanyName = c.CompanyName;
                            DSCompany.AddTargetName = titem.TargetName;
                            List<DSTargetReturnData> dataList = new List<DSTargetReturnData>();

                            #region 根据指标
                            
                            
                            foreach (C_Target ct in TargetList.OrderBy(t => t.Sequence))
                            {
                                DSTargetReturnData DSReturnData = new DSTargetReturnData();
                                DSReturnData.CompanyID = i;
                                DSReturnData.ReturnTargetName = ct.TargetName;
                                MonthlyReportDetail LastDetail = null;
                                if (Month == 1)
                                    LastDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, true);
                                else
                                    LastDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, false);

                                if (LastDetail != null && LastDetail.ID != Guid.Empty)
                                {
                                    DSReturnData.LastAccumulativePlan = Convert.ToDecimal(LastDetail.NAccumulativePlanAmmount.ToString("N2"));
                                    DSReturnData.LastAccumulativeActual = Convert.ToDecimal(LastDetail.NAccumulativeActualAmmount.ToString("N2"));
                                    DSReturnData.LastAccumulativeDifference = Convert.ToDecimal((LastDetail.NAccumulativeDifference).ToString("N2"));
                                }
                                MonthlyReportDetail CurrentDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, true);

                                if (CurrentDetail != null && CurrentDetail.ID != Guid.Empty)
                                {
                                    DSReturnData.CurrentReturnAmount = Convert.ToDecimal((CurrentDetail.NAccumulativeDifference - LastDetail.NAccumulativeDifference).ToString("N2"));
                                    DSReturnData.CurrentAccumulativePlan = Convert.ToDecimal(CurrentDetail.NAccumulativePlanAmmount.ToString("N2"));
                                    DSReturnData.CurrentAccumulativeActual = Convert.ToDecimal(CurrentDetail.NAccumulativeActualAmmount.ToString("N2"));
                                    DSReturnData.CurrentAccumulativeDifference = Convert.ToDecimal((CurrentDetail.NAccumulativeDifference).ToString("N2"));
                                    DSReturnData.CurrentAccumulativeRate = CurrentDetail.NAccumulativeDisplayRate.ToString();
                                    DSReturnData.CommitDate = string.Format("{0:yyyy-MM-dd}", CurrentDetail.CommitDate);
                                    DSReturnData.ReturnType = CurrentDetail.ReturnType;

                                    if (!string.IsNullOrEmpty(CurrentDetail.ReturnType.ToString()) && CurrentDetail.ReturnType > 0)
                                    {
                                        DSReturnData.ReturnTypeDescrible = EnumUtil.GetEnumDescription(typeof(EnumReturnType), CurrentDetail.ReturnType);
                                    }
                                    else
                                    {
                                        DSReturnData.ReturnTypeDescrible = "--";
                                    }
                                    DSReturnData.Counter = CurrentDetail.Counter;
                                }
                                else
                                {
                                    DSReturnData.CurrentAccumulativeRate = "--";
                                    DSReturnData.ReturnTypeDescrible = "--";
                                    DSReturnData.CommitDate = "--";
                                    DSReturnData.Counter = 0;
                                }


                                dataList.Add(DSReturnData);
                            }

                            #endregion

                            if (dataList.Where(p => p.ReturnType == (int)EnumReturnType.New).ToList().Count > 0)
                            {
                                DSCompany.ReturnDataList = dataList;
                                Showlist.Add(DSCompany);
                            }
                            i++;
                        }
                    }

                    #endregion

                }
            }
            return Showlist;
        }
    }
}
