using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators
{

    #region 收入类
    /// <summary>
    /// 收入类和其他
    /// A>0&B≥0，完成率=B/A
    /// A=0&B>0，完成率=超计划B万元
    /// </summary>
    public class DefaultSummaryTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            MonthReportSummaryViewModel ReportDetail = obj as MonthReportSummaryViewModel;
            DateTime CurrentDate = DateTime.Now;
            C_System  Sys  = StaticResource.Instance[ReportDetail.SystemID,CurrentDate];
            
            if (ReportDetail == null) return null;
            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            double DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);

                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    else
                        ReportDetail.NActualRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);

                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    else
                        ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = "超计划" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    else
                        ReportDetail.NActualRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NActualAmmount == 0)
                {
                    ReportDetail.NActualRate = "--";
                }
                else
                {
                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    else
                        ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }

            }
            else
            {
                if (ReportDetail.NPlanAmmount < ReportDetail.NActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = "减亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    else
                        ReportDetail.NActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = "100%";
                    else
                        ReportDetail.NActualRate = "100.0%";
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NActualRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    else
                        ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    //这里为项目系统单独调用方法，百分率保留1位小数
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    else
                        ReportDetail.NAccumulativeActualRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    else
                        ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = "超计划" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    else
                        ReportDetail.NAccumulativeActualRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)
                {
                    ReportDetail.NAccumulativeActualRate = "--";
                }
                else
                {
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    else
                        ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
            }
            else
            {
                if (ReportDetail.NAccumulativePlanAmmount < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = "减亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    else
                        ReportDetail.NAccumulativeActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = "100.0%";
                    else
                        ReportDetail.NAccumulativeActualRate = "100%";
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    
                    if (Sys != null && Sys.Category == 2)
                        ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    else
                        ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 累计

            DisplayRate = 0;

            #region 全年
            double temp = 0;

            if (double.TryParse(ReportDetail.MeasureRate.ToString(), out temp))
            {
                decimal MeasureRate = 0;
                    MeasureRate = Convert.ToDecimal(ReportDetail.MeasureRate);
                if (MeasureRate > 0)
                {
                    if (ReportDetail.NAccumulativeActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                    {
                        DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / (double)MeasureRate);

                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 1);
                        else
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 1);
                    }
                    else
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - (double)MeasureRate) / (double)MeasureRate);
                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 2);
                        else
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 2);
                    }

                }
                else if (MeasureRate == 0)
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                    if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                    {
                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = "超计划" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 3);
                        else
                            ReportDetail.NAnnualCompletionRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 3);
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount == 0)
                    {
                        ReportDetail.NAnnualCompletionRate = "--";
                    }
                    else
                    {
                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 3);
                        else
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 3);
                    }
                }
                else
                {
                    if ((double)MeasureRate < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - (double)MeasureRate) / (double)MeasureRate);
                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = "减亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 2);
                        else
                            ReportDetail.NAnnualCompletionRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 2);

                    }
                    else if ((double)MeasureRate == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = "100.0%";
                        else
                            ReportDetail.NAnnualCompletionRate = "100%";
                    }
                    else if ((double)MeasureRate > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - (double)MeasureRate) / (double)MeasureRate);
                        if (Sys != null && Sys.Category == 2)
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 2);
                        else
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, MeasureRate, strUnit, 2);
                    }
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 收入类

    #region 净利润类 
    /// <summary>
    /// 净利润类  项目系统
    /// A>0&B≥0，完成率=B/A
    /// A>0&B<0，完成率=增亏|(B-A)/A|%
    /// A<0&B≥0，完成率=减亏|(B-A)/A|%
    /// A<B<0，完成率=减亏|(B-A)/A|%
    /// B<A<0，完成率=增亏|(B-A)/A|%
    /// A=0&B>0，完成率=减亏B万元
    /// A=0&B<0，完成率=增亏B万元
    /// </summary>
    public class ProfitSummaryTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            MonthReportSummaryViewModel ReportDetail = obj as MonthReportSummaryViewModel;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }
            double DisplayRate = 0;

            #region 当月
            /// A>0&B≥0，完成率=B/A
            /// A>0&B<0，完成率=增亏|(B-A)/A|%
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                }
                else if (ReportDetail.NActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }

            }
            /// A=0&B>0，完成率=减亏B万元
            /// A=0&B<0，完成率=增亏B万元
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                if (ReportDetail.NActualAmmount > 0)//A=0,B>0:减亏B万元
                {
                    ReportDetail.NActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NActualAmmount == 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NActualRate = "--";
                }
                else if (ReportDetail.NActualAmmount < 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
            }
            /// A<0&B≥0，完成率=减亏|(B-A)/A|%
            /// A<B<0，完成率=减亏|(B-A)/A|%
            /// B<A<0，完成率=增亏|(B-A)/A|%
            else if (ReportDetail.NPlanAmmount < 0)
            {
                if (ReportDetail.NPlanAmmount < ReportDetail.NActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NActualRate = "100%";
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            /// A>0&B≥0，完成率=B/A
            /// A>0&B<0，完成率=增亏|(B-A)/A|%
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                }
                else if (ReportDetail.NAccumulativeActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }

            }
            /// A=0&B>0，完成率=减亏B万元
            /// A=0&B<0，完成率=增亏B万元
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0,B>0:减亏B万元
                {
                    ReportDetail.NAccumulativeActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NAccumulativeActualRate = "--";
                }
                else if (ReportDetail.NAccumulativeActualAmmount < 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
            }

            /// A<0&B≥0，完成率=减亏|(B-A)/A|%
            /// A<B<0，完成率=减亏|(B-A)/A|%
            /// B<A<0，完成率=增亏|(B-A)/A|%
            else if (ReportDetail.NAccumulativePlanAmmount < 0)
            {
                if (ReportDetail.NAccumulativePlanAmmount < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = "100%";
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 累计

            DisplayRate = 0;

            #region 年度
            /// A>0&B≥0，完成率=B/A
            /// A>0&B<0，完成率=增亏|(B-A)/A|%
            double temp = 0;

            if (double.TryParse(ReportDetail.MeasureRate.ToString(), out temp))
            {
                double MeasureRate = double.Parse(ReportDetail.MeasureRate);
                if (MeasureRate > 0)
                {
                    if (ReportDetail.NAccumulativeActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                    {
                        DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / MeasureRate);
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 1);
                        else
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 2);
                        else
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    }

                }
                /// A=0&B>0，完成率=减亏B万元
                /// A=0&B<0，完成率=增亏B万元
                else if (MeasureRate == 0)
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                    if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0,B>0:减亏B万元
                    {
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 3);
                        else
                            ReportDetail.NAnnualCompletionRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);

                    }
                    else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0,B<0:增亏B万元
                    {
                        ReportDetail.NAnnualCompletionRate = "--";
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount < 0)//A=0,B<0:增亏B万元
                    {
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 3);
                        else
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    }
                }

                /// A<0&B≥0，完成率=减亏|(B-A)/A|%
                /// A<B<0，完成率=减亏|(B-A)/A|%
                /// B<A<0，完成率=增亏|(B-A)/A|%
                else if (MeasureRate < 0)
                {
                    if (MeasureRate < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 2);
                        else
                            ReportDetail.NAnnualCompletionRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    }
                    else if (MeasureRate == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        ReportDetail.NAnnualCompletionRate = "100%";
                    }
                    else if (MeasureRate > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 2);
                        else
                            ReportDetail.NAnnualCompletionRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    }
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 净利润类

    #region 费用/成本类
    /// <summary>
    /// 费用/成本类
    /// 0<A<B，完成率=超支|(B-A)/A|%
    /// 0≤B<A，完成率=节约|(B-A)/A|%
    /// A=0&B>0，完成率=超计划B万元
    /// </summary>
    public class CostSummaryTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            MonthReportSummaryViewModel ReportDetail = obj as MonthReportSummaryViewModel;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }
            double DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                if (ReportDetail.NActualAmmount > ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NActualRate = "超支" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NActualAmmount == ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NActualRate = "100%";
                }
                else if (ReportDetail.NActualAmmount < ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NActualRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NActualRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NActualAmmount == 0)
                {
                    ReportDetail.NActualRate = "--";
                }
                else
                {
                    ReportDetail.NActualRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
            }
            else
            {
                if (ReportDetail.NPlanAmmount < ReportDetail.NActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = "100%";
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                if (ReportDetail.NAccumulativeActualAmmount > ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeActualRate = "超支" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NAccumulativeActualAmmount == ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeActualRate = "100%";
                }
                else if (ReportDetail.NAccumulativeActualAmmount < ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeActualRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeActualRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeActualRate = "--";
                }
                else
                {
                    ReportDetail.NAccumulativeActualRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
            }
            else
            {
                if (ReportDetail.NAccumulativePlanAmmount < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeActualRate = "100%";
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 累计

            DisplayRate = 0;

            #region 年度
            double temp = 0;
            if (double.TryParse(ReportDetail.MeasureRate.ToString(), out temp))
            {
                double MeasureRate = double.Parse(ReportDetail.MeasureRate);
                if (MeasureRate > 0)
                {

                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount  / MeasureRate);
                    if (ReportDetail.NAccumulativeActualAmmount > MeasureRate)
                    {
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 1);
                        else
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,1);
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount == MeasureRate)
                    {
                        ReportDetail.NAnnualCompletionRate = "100%";
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount < MeasureRate)
                    {
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit,1);
                        else
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    }

                }
                else if (MeasureRate == 0)
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                    if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                    {
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate =   Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 1);
                        else
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,1);
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0&B>0，完成率=超计划B万元
                    {
                        ReportDetail.NAnnualCompletionRate = "--";
                    }
                    else
                    {
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit, 1);
                        else
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    }
                }
                else
                {
                    if (MeasureRate < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit,1);
                        else
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    }
                    else if (MeasureRate == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        ReportDetail.NAnnualCompletionRate = JsonHelper.Serialize(new ActualRate(1, 1));
                        ReportDetail.NAnnualCompletionRate = "100%";
                    }
                    else if (MeasureRate > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        if (Kpi == null)
                            ReportDetail.NAnnualCompletionRate =  Utility.Instance.CalculateDisplayRate(DisplayRate, Convert.ToDecimal(MeasureRate), strUnit,1);
                        else
                            ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    }
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 费用/成本类


    #region 集团总部算法
    /// <summary>
    /// 收入类和其他
    /// A>0&B≥0，完成率=B/A
    /// A=0&B>0，完成率=超计划B万元
    /// </summary>
    public class GroupSummaryTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            MonthReportSummaryViewModel ReportDetail = obj as MonthReportSummaryViewModel;
            if (ReportDetail == null) return null;
            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            double DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "--"; //"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = "--";
                //if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                //{
                //    ReportDetail.NActualRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                //}
                //else if (ReportDetail.NActualAmmount == 0)
                //{
                //    ReportDetail.NActualRate = "--";
                //}
                //else
                //{
                //    ReportDetail.NActualRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                //}

            }
            else
            {
                if (ReportDetail.NPlanAmmount < ReportDetail.NActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "--"; //"减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = "--";//"100%";
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = "--"; //"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit); //Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                }
                else
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "--";// Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeActualRate = "--"; //"超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)
                {
                    ReportDetail.NAccumulativeActualRate = "--";
                }
                else
                {
                    ReportDetail.NAccumulativeActualRate = "--"; //"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                }
            }
            else
            {
                if (ReportDetail.NAccumulativePlanAmmount < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "--"; //"减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = "--"; //"100%";
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = "--";//"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                }
            }
            #endregion 累计

            DisplayRate = 0;

            #region 全年
            double temp = 0;
            if (double.TryParse(ReportDetail.MeasureRate.ToString(), out temp))
            {
                double MeasureRate = double.Parse(ReportDetail.MeasureRate);
                if (MeasureRate > 0)
                {
                    if (ReportDetail.NAccumulativeActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                    {
                        DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / MeasureRate);
                        ReportDetail.NAnnualCompletionRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit); //Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    }
                    else
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                        //ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                        ReportDetail.NAnnualCompletionRate = "--"; //"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    }

                }
                else if (MeasureRate == 0)
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                    if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                    {
                        ReportDetail.NAnnualCompletionRate = "--";//"超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    }
                    else if (ReportDetail.NAccumulativeActualAmmount == 0)
                    {
                        ReportDetail.NAnnualCompletionRate = "--";
                    }
                    else
                    {
                        ReportDetail.NAnnualCompletionRate = "--";//"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    }
                }
                else
                {
                    if (MeasureRate < ReportDetail.NAccumulativeActualAmmount)//A<0,B≥0:减亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        ReportDetail.NAnnualCompletionRate = "--"; //"减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    }
                    else if (MeasureRate == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        ReportDetail.NAnnualCompletionRate = "--";
                    }
                    else if (MeasureRate > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                    {
                        DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - MeasureRate) / MeasureRate);
                        ReportDetail.NAnnualCompletionRate = "--";//"增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    }
                }
            }
            #endregion 全年

            return ReportDetail;
        }
    }
    #endregion 收入类
}