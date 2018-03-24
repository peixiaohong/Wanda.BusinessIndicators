using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators
{
    public struct ActualRate
    {
        public ActualRate(int rateType, decimal rate)
        {
            RateType = rateType;
            Rate = rate;
        }
        public int RateType;
        public decimal Rate;
    }

    #region 收入类
    /// <summary>
    /// 收入类和其他
    /// A>0&B≥0，完成率=B/A
    /// A=0&B>0，完成率=超计划B万元
    /// </summary>
    public class DefaultTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            B_MonthlyReportDetail ReportDetail = obj as B_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            decimal DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTargetCurrent = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTargetCurrent = true;
                    }

                    ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
                }

            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount == 0)
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "--";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else
                {
                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 2);

                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = true;
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0&B>0，完成率=超计划B万元
                {
                    DisplayRate = 1;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }
                else
                {
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTarget = true;
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 收入类

    #region 净利润类
    /// <summary>
    /// 净利润类
    /// A>0&B≥0，完成率=B/A
    /// A>0&B<0，完成率=增亏|(B-A)/A|%
    /// A<0&B≥0，完成率=减亏|(B-A)/A|%
    /// A<B<0，完成率=减亏|(B-A)/A|%
    /// B<A<0，完成率=增亏|(B-A)/A|%
    /// A=0&B>0，完成率=减亏B万元
    /// A=0&B<0，完成率=增亏B万元
    /// </summary>
    public class ProfitTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            B_MonthlyReportDetail ReportDetail = obj as B_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }
            decimal DisplayRate = 0;

            #region 当月
            /// A>0&B≥0，完成率=B/A
            /// A>0&B<0，完成率=增亏|(B-A)/A|%
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTargetCurrent = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTargetCurrent = true;
                    }

                    ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit);
                }
                else if (ReportDetail.NActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit,2) ;
                    
                    ReportDetail.IsMissTargetCurrent = true;
                }

            }
            /// A=0&B>0，完成率=减亏B万元
            /// A=0&B<0，完成率=增亏B万元
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NActualAmmount > 0)//A=0,B>0:减亏B万元
                {
                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount == 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "--";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount < 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,2);
                    
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,2);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                    
                }
                else if (ReportDetail.NAccumulativeActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,2);

                    ReportDetail.IsMissTarget = true;
                }

            }
            /// A=0&B>0，完成率=减亏B万元
            /// A=0&B<0，完成率=增亏B万元
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0,B>0:减亏B万元
                {

                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0,B>0:减亏B万元
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount < 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTarget = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,2);
                    
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,2);
                    ReportDetail.IsMissTarget = true;
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
    public class CostTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            B_MonthlyReportDetail ReportDetail = obj as B_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit ="万元";
            if (Target != null)
            {
                strUnit=Target.Unit;
            }
            decimal DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                if (ReportDetail.NActualAmmount > ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NDisplayRate = "超支" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit,2);
                    ReportDetail.IsMissTargetCurrent = true;
                }
                else if (ReportDetail.NActualAmmount == ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount < ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = false;
                }
            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = true;
                }
                else if (ReportDetail.NActualAmmount==0)
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "--";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else
                {
                    ReportDetail.NDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = false;
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
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit,2);
                    
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit,2);
                    ReportDetail.IsMissTargetCurrent = true;
                }
            }            
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeDisplayRate = "超支" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit,2);
                    ReportDetail.IsMissTarget = true;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount < ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 2);
                    ReportDetail.IsMissTarget = false;
                }
            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTarget = true;
                }
                else if (ReportDetail.NAccumulativeActualAmmount== 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }
                else
                {
                    ReportDetail.NAccumulativeDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit, 3);
                    ReportDetail.IsMissTarget = false;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit,2);
                    
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate,strUnit,2);
                    ReportDetail.IsMissTarget = true;
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 费用/成本类

    public class InitialDataException : Exception
    {
        public InitialDataException(string Message)
            : base(Message)
        {

        }
    }

    #region 项目指标

    /// <summary>
    /// 项目指标计算
    /// </summary>
    public class ProjectTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            B_MonthlyReportDetail ReportDetail = obj as B_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            decimal DisplayRate = 0;

            #region 当月

            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTargetCurrent = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTargetCurrent = true;
                    }

                    ReportDetail.NDisplayRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit );
                }
                else
                {
                    DisplayRate = 0;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "/";
                    ReportDetail.IsMissTargetCurrent = true;
                }

            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                //A=0& B>0或B<0 或者B= 0  ,完成率 =/

                DisplayRate = 0;
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                ReportDetail.NDisplayRate = "/";
                ReportDetail.IsMissTargetCurrent = false;
            }
            else if (ReportDetail.NPlanAmmount < 0)
            {
                if (ReportDetail.NActualAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / Math.Abs(ReportDetail.NPlanAmmount);

                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);

                    ReportDetail.IsMissTargetCurrent = false;
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;

                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "/";

                    ReportDetail.IsMissTargetCurrent = false;
                }
            }

            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0) //A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NAccumulativeDisplayRate = "/";
                    ReportDetail.IsMissTarget = false;
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                //A=0& B>0或B<0 或者B= 0  ,完成率 =/

                DisplayRate = 0;
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                ReportDetail.NAccumulativeDisplayRate = "/";
                ReportDetail.IsMissTarget = false;

            }
            else if (ReportDetail.NAccumulativePlanAmmount < 0)
            {

                if (ReportDetail.NAccumulativeActualAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / Math.Abs(ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    ReportDetail.IsMissTarget = false;
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "/";
                    ReportDetail.IsMissTarget = false;
                }

            }
            #endregion 累计

            return ReportDetail;
        }
    }

    #endregion

    #region 集团指标

    /// <summary>
    /// 集团指标计算
    /// </summary>
    public class GroupjectTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            B_MonthlyReportDetail ReportDetail = obj as B_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            decimal DisplayRate = 0;

            #region 当月

            //if (ReportDetail.NPlanAmmount > 0)
            //{
            //    if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
            //    {
            //        DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
            //        ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

            //        if (DisplayRate >= Kpi.MeasureRate)
            //        {
            //            ReportDetail.IsMissTargetCurrent = false;
            //        }
            //        else
            //        {
            //            ReportDetail.IsMissTargetCurrent = true;
            //        }

            //        ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
            //    }
            //    else
            //    {
            //        DisplayRate = 0;
            //        ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

            //        ReportDetail.NDisplayRate = "--";
            //        ReportDetail.IsMissTargetCurrent = true;
            //    }

            //}
            //else if (ReportDetail.NPlanAmmount == 0)
            //{
            //    //A=0& B>0或B<0 或者B= 0  ,完成率 =/

            //    DisplayRate = 0;
            //    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

            //    ReportDetail.NDisplayRate = "--";
            //    ReportDetail.IsMissTargetCurrent = false;
            //}
            //else if (ReportDetail.NPlanAmmount < 0)
            //{
            //    if (ReportDetail.NActualAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
            //    {
            //        DisplayRate = 1 + (ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / Math.Abs(ReportDetail.NPlanAmmount);

            //        ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

            //        ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);

            //        ReportDetail.IsMissTargetCurrent = false;
            //    }
            //    else  //A<0,B<=0 ： 完成率："/"
            //    {
            //        DisplayRate = 0;

            //        ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

            //        ReportDetail.NDisplayRate = "--";

            //        ReportDetail.IsMissTargetCurrent = false;
            //    }
            //}

            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0) //A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                //A=0& B>0或B<0 或者B= 0  ,完成率 =/

                DisplayRate = 0;
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                ReportDetail.NAccumulativeDisplayRate = "--";
                ReportDetail.IsMissTarget = false;

            }
            else if (ReportDetail.NAccumulativePlanAmmount < 0)
            {

                if (ReportDetail.NAccumulativePlanAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / Math.Abs(ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    ReportDetail.IsMissTarget = false;
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }

            }
            #endregion 累计

            return ReportDetail;
        }
    }

    #endregion






//---------------------------------------重新计算A表的counter数--------------------------------


    #region 收入类
    /// <summary>
    /// 收入类和其他
    /// A>0&B≥0，完成率=B/A
    /// A=0&B>0，完成率=超计划B万元
    /// </summary>
    public class DefaultResetTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            A_MonthlyReportDetail ReportDetail = obj as A_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            decimal DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTargetCurrent = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTargetCurrent = true;
                    }

                    ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
                }

            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount == 0)
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "--";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else
                {
                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = true;
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0&B>0，完成率=超计划B万元
                {
                    DisplayRate = 1;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }
                else
                {
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTarget = true;
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 收入类

    #region 净利润类
    /// <summary>
    /// 净利润类
    /// A>0&B≥0，完成率=B/A
    /// A>0&B<0，完成率=增亏|(B-A)/A|%
    /// A<0&B≥0，完成率=减亏|(B-A)/A|%
    /// A<B<0，完成率=减亏|(B-A)/A|%
    /// B<A<0，完成率=增亏|(B-A)/A|%
    /// A=0&B>0，完成率=减亏B万元
    /// A=0&B<0，完成率=增亏B万元
    /// </summary>
    public class ResetProfitTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            A_MonthlyReportDetail ReportDetail = obj as A_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }
            decimal DisplayRate = 0;

            #region 当月
            /// A>0&B≥0，完成率=B/A
            /// A>0&B<0，完成率=增亏|(B-A)/A|%
            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTargetCurrent = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTargetCurrent = true;
                    }

                    ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else if (ReportDetail.NActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTargetCurrent = true;
                }

            }
            /// A=0&B>0，完成率=减亏B万元
            /// A=0&B<0，完成率=增亏B万元
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NActualAmmount > 0)//A=0,B>0:减亏B万元
                {
                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount == 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "--";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount < 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);

                }
                else if (ReportDetail.NAccumulativeActualAmmount < 0)//A>0&B<0，完成率=增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = true;
                }

            }
            /// A=0&B>0，完成率=减亏B万元
            /// A=0&B<0，完成率=增亏B万元
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0,B>0:减亏B万元
                {

                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0,B>0:减亏B万元
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount < 0)//A=0,B<0:增亏B万元
                {
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = true;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTarget = true;
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
    public class ResetCostTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            A_MonthlyReportDetail ReportDetail = obj as A_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }
            decimal DisplayRate = 0;

            #region 当月
            if (ReportDetail.NPlanAmmount > 0)
            {
                DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                if (ReportDetail.NActualAmmount > ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NDisplayRate = "超支" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
                }
                else if (ReportDetail.NActualAmmount == ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NActualAmmount < ReportDetail.NPlanAmmount)
                {
                    ReportDetail.NDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = false;
                }
            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NActualAmmount);
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = true;
                }
                else if (ReportDetail.NActualAmmount == 0)
                {
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NDisplayRate = "--";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else
                {
                    ReportDetail.NDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTargetCurrent = false;
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
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount == ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = 1;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "100%";
                    ReportDetail.IsMissTargetCurrent = false;
                }
                else if (ReportDetail.NPlanAmmount > ReportDetail.NActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                    ReportDetail.NDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTargetCurrent = true;
                }
            }
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeDisplayRate = "超支" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTarget = true;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativeActualAmmount < ReportDetail.NAccumulativePlanAmmount)
                {
                    ReportDetail.NAccumulativeDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTarget = false;
                }
            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(3, DisplayRate));

                if (ReportDetail.NAccumulativeActualAmmount > 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeDisplayRate = "超计划" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = true;
                }
                else if (ReportDetail.NAccumulativeActualAmmount == 0)//A=0&B>0，完成率=超计划B万元
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }
                else
                {
                    ReportDetail.NAccumulativeDisplayRate = "节约" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 3);
                    ReportDetail.IsMissTarget = false;
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
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "减亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);

                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount == ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, 1));
                    ReportDetail.NAccumulativeDisplayRate = "100%";
                    ReportDetail.IsMissTarget = false;
                }
                else if (ReportDetail.NAccumulativePlanAmmount > ReportDetail.NAccumulativeActualAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
                {
                    DisplayRate = Math.Abs((ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(2, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "增亏" + Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 2);
                    ReportDetail.IsMissTarget = true;
                }
            }
            #endregion 累计

            return ReportDetail;
        }
    }
    #endregion 费用/成本类


    #region 项目指标

    /// <summary>
    /// 项目指标计算
    /// </summary>
    public class ResetProjectTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            A_MonthlyReportDetail ReportDetail = obj as A_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            decimal DisplayRate = 0;

            #region 当月

            if (ReportDetail.NPlanAmmount > 0)
            {
                if (ReportDetail.NActualAmmount >= 0)//A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NActualAmmount / ReportDetail.NPlanAmmount);
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTargetCurrent = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTargetCurrent = true;
                    }

                    ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = 0;
                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "/";
                    ReportDetail.IsMissTargetCurrent = true;
                }

            }
            else if (ReportDetail.NPlanAmmount == 0)
            {
                //A=0& B>0或B<0 或者B= 0  ,完成率 =/

                DisplayRate = 0;
                ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                ReportDetail.NDisplayRate = "/";
                ReportDetail.IsMissTargetCurrent = false;
            }
            else if (ReportDetail.NPlanAmmount < 0)
            {
                if (ReportDetail.NActualAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (ReportDetail.NActualAmmount - ReportDetail.NPlanAmmount) / Math.Abs(ReportDetail.NPlanAmmount);

                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);

                    ReportDetail.IsMissTargetCurrent = false;
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;

                    ReportDetail.NActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NDisplayRate = "/";

                    ReportDetail.IsMissTargetCurrent = false;
                }
            }

            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0) //A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NAccumulativeDisplayRate = "/";
                    ReportDetail.IsMissTarget = false;
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                //A=0& B>0或B<0 或者B= 0  ,完成率 =/

                DisplayRate = 0;
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                ReportDetail.NAccumulativeDisplayRate = "/";
                ReportDetail.IsMissTarget = false;

            }
            else if (ReportDetail.NAccumulativePlanAmmount < 0)
            {

                if (ReportDetail.NAccumulativeActualAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / Math.Abs(ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    ReportDetail.IsMissTarget = false;
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "/";
                    ReportDetail.IsMissTarget = false;
                }

            }
            #endregion 累计

            return ReportDetail;
        }
    }

    #endregion

    #region 集团指标

    /// <summary>
    /// 集团指标计算
    /// </summary>
    public class ResetGroupjectTargetEvaluation : ITargetEvaluation
    {
        public object Calculation(object obj)
        {
            A_MonthlyReportDetail ReportDetail = obj as A_MonthlyReportDetail;
            if (ReportDetail == null) return null;

            C_TargetKpi Kpi = StaticResource.Instance.GetKpiList(ReportDetail.SystemID, ReportDetail.FinYear).Find(K => K.TargetID == ReportDetail.TargetID);
            C_Target Target = StaticResource.Instance.GetTargetListBySysID(ReportDetail.SystemID).Where(p => p.ID == ReportDetail.TargetID).ToList()[0];
            string strUnit = "万元";
            if (Target != null)
            {
                strUnit = Target.Unit;
            }

            decimal DisplayRate = 0;

            #region 当月

          
            #endregion 当月

            DisplayRate = 0;

            #region 累计
            if (ReportDetail.NAccumulativePlanAmmount > 0)
            {
                if (ReportDetail.NAccumulativeActualAmmount >= 0) //A>0&B≥0，完成率=B/A
                {
                    DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount / ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    if (DisplayRate >= Kpi.MeasureRate)
                    {
                        ReportDetail.IsMissTarget = false;
                    }
                    else
                    {
                        ReportDetail.IsMissTarget = true;
                    }

                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit);
                }
                else
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }

            }
            else if (ReportDetail.NAccumulativePlanAmmount == 0)
            {
                DisplayRate = Math.Abs(ReportDetail.NAccumulativeActualAmmount);
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                //A=0& B>0或B<0 或者B= 0  ,完成率 =/

                DisplayRate = 0;
                ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));

                ReportDetail.NAccumulativeDisplayRate = "--";
                ReportDetail.IsMissTarget = false;

            }
            else if (ReportDetail.NAccumulativePlanAmmount < 0)
            {

                if (ReportDetail.NAccumulativePlanAmmount > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (ReportDetail.NAccumulativeActualAmmount - ReportDetail.NAccumulativePlanAmmount) / Math.Abs(ReportDetail.NAccumulativePlanAmmount);
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = Utility.Instance.CalculateDisplayRate(DisplayRate, Kpi.MeasureRate, strUnit, 1);
                    ReportDetail.IsMissTarget = false;
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;
                    ReportDetail.NAccumulativeActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    ReportDetail.NAccumulativeDisplayRate = "--";
                    ReportDetail.IsMissTarget = false;
                }

            }
            #endregion 累计

            return ReportDetail;
        }
    }

    #endregion



}