using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    public class DefaultBizContext : IBizContext
    {
        public Hashtable GetBizContext(object Vmodel)
        {
            List<MonthlyReportDetail> GroupRpt = (List<MonthlyReportDetail>)Vmodel;
            Hashtable hastable = new Hashtable();
           

            if (GroupRpt != null && GroupRpt.Count > 0)
            {
                DateTime ReportDate = DateTime.MinValue;

                if (GroupRpt[0].FinMonth == 12)
                {
                    DateTime.Parse(Convert.ToInt32(GroupRpt[0].FinYear + 1) + "-01-01").AddDays(-1);
                }
                else
                {
                    ReportDate = DateTime.Parse(GroupRpt[0].FinYear + "-" + Convert.ToInt32(GroupRpt[0].FinMonth + 1) + "-01").AddDays(-1);
                }

                var TargetPlanListByYear = StaticResource.Instance.GetTargetPlanList(GroupRpt[0].SystemID, GroupRpt[0].FinYear);
                var TargetData = StaticResource.Instance.GetTargetList(GroupRpt[0].SystemID, DateTime.Now).ToList();
                foreach (MonthlyReportDetail item in GroupRpt)
                {
                    if (item.TargetID == Guid.Empty)
                    {
                        continue;
                    }
                    else if (!TargetData.Where(T => T.ID == item.TargetID).Any())
                    {
                        continue;
                    }

                    string TargetName = TargetData.Find(T => T.ID == item.TargetID).TargetName;
                    //StaticResource.Instance.TargetList[item.SystemID].Find(T => T.ID == item.TargetID).TargetName;
                    var targetPlanByYearData = TargetPlanListByYear.Where(t => t.CompanyID == item.CompanyID && t.TargetID == item.TargetID);
                    decimal SumTarget = 0;
                    if (targetPlanByYearData.Any())
                    {
                        SumTarget = targetPlanByYearData.Sum(s => s.Target);
                    }
                    if (hastable.ContainsKey(TargetName + ".NPlanAmmountByYear") == false) //年度指标
                        hastable.Add(TargetName + ".NPlanAmmountByYear", SumTarget);
                    else
                        hastable[TargetName + ".NPlanAmmountByYear"] = SumTarget;

                    if (hastable.ContainsKey(TargetName + ".NDisplayRateByYear") == false) //年度指标完成率
                    {
                        if (SumTarget != 0) {
                            hastable.Add(TargetName + ".NDisplayRateByYear", Math.Round((item.NAccumulativeActualAmmount / SumTarget), 5, MidpointRounding.AwayFromZero).ToString("P1"));
                        }
                        else
                        {
                            hastable.Add(TargetName + ".NDisplayRateByYear", "--");
                        }
                    }
                    else
                    {
                        if (SumTarget != 0)
                        {
                            hastable.Add(TargetName + ".NDisplayRateByYear", Math.Round((item.NAccumulativeActualAmmount / SumTarget), 5, MidpointRounding.AwayFromZero).ToString("P1"));
                        }
                        else
                        {
                            hastable.Add(TargetName + ".NDisplayRateByYear", "/");
                        }
                    }
                    if (hastable.ContainsKey(TargetName + ".IsDelayComplete") == false) //延迟完成
                        hastable.Add(TargetName + ".IsDelayComplete", item.IsDelayComplete);
                    else
                        hastable[TargetName + ".IsDelayComplete"] = item.IsDelayComplete;

                    if (hastable.ContainsKey(TargetName + ".IsCommitDate") == false)
                        hastable.Add(TargetName + ".IsCommitDate", item.IsCommitDate);
                    else
                        hastable[TargetName + ".IsCommitDate"] = item.IsCommitDate;


                    if (hastable.ContainsKey(TargetName + ".ReportDate") == false)
                        hastable.Add(TargetName + ".ReportDate", ReportDate);
                    else
                        hastable[TargetName + ".ReportDate"] = ReportDate;

                    if (hastable.ContainsKey(TargetName + ".ReturnType") == false)
                        hastable.Add(TargetName + ".ReturnType", item.ReturnType);
                    else
                        hastable[TargetName + ".ReturnType"] = item.ReturnType;


                    if (hastable.ContainsKey(TargetName + ".PromissDate") == false)
                        hastable.Add(TargetName + ".PromissDate", item.PromissDate);
                    else
                        hastable[TargetName + ".PromissDate"] = item.PromissDate;

                    if (hastable.ContainsKey(TargetName + ".NAccumulativePlanAmmount") == false)
                        hastable.Add(TargetName + ".NAccumulativePlanAmmount", item.NAccumulativePlanAmmount);
                    else
                        hastable[TargetName + ".NAccumulativePlanAmmount"] = item.NAccumulativePlanAmmount;

                    if (hastable.ContainsKey(TargetName + ".NAccumulativeActualAmmount") == false)
                        hastable.Add(TargetName + ".NAccumulativeActualAmmount", item.NAccumulativeActualAmmount);
                    else
                        hastable[TargetName + ".NAccumulativeActualAmmount"] = item.NAccumulativeActualAmmount;


                    if (hastable.ContainsKey(TargetName + ".NAccumulativeActualRate") == false)
                        hastable.Add(TargetName + ".NAccumulativeActualRate", item.NAccumulativeActualRate);
                    else
                        hastable[TargetName + ".NAccumulativeActualRate"] = item.NAccumulativeActualRate;

                    if (hastable.ContainsKey(TargetName + ".NAccumulativeDisplayRate") == false)
                        hastable.Add(TargetName + ".NAccumulativeDisplayRate", item.NAccumulativeDisplayRate);
                    else
                        hastable[TargetName + ".NAccumulativeDisplayRate"] = item.NAccumulativeDisplayRate;


                    if (hastable.ContainsKey(TargetName + ".NPlanAmmount") == false)
                        hastable.Add(TargetName + ".NPlanAmmount", item.NPlanAmmount);
                    else
                        hastable[TargetName + ".NPlanAmmount"] = item.NPlanAmmount;

                    if (hastable.ContainsKey(TargetName + ".NActualAmmount") == false)
                        hastable.Add(TargetName + ".NActualAmmount", item.NActualAmmount);
                    else
                        hastable[TargetName + ".NActualAmmount"] = item.NActualAmmount;

                    if (hastable.ContainsKey(TargetName + ".NActualRate") == false)
                        hastable.Add(TargetName + ".NActualRate", item.NActualRate);
                    else
                        hastable[TargetName + ".NActualRate"] = item.NActualRate;


                    if (hastable.ContainsKey(TargetName + ".NDisplayRate") == false)
                        hastable.Add(TargetName + ".NDisplayRate", item.NDisplayRate);
                    else
                        hastable[TargetName + ".NDisplayRate"] = item.NDisplayRate;

                    if (hastable.ContainsKey(TargetName + ".IsMissTarget") == false)
                        hastable.Add(TargetName + ".IsMissTarget", item.IsMissTarget);
                    else
                        hastable[TargetName + ".IsMissTarget"] = item.IsMissTarget;


                    if (hastable.ContainsKey(TargetName + ".NDifference") == false)
                        hastable.Add(TargetName + ".NDifference", item.NDifference);
                    else
                        hastable[TargetName + ".NDifference"] = item.NDifference;


                    if (hastable.ContainsKey(TargetName + ".NAccumulativeDifference") == false)
                        hastable.Add(TargetName + ".NAccumulativeDifference", item.NAccumulativeDifference);
                    else
                        hastable[TargetName + ".NAccumulativeDifference"] = item.NAccumulativeDifference;


                    if (hastable.ContainsKey(TargetName + ".IsMissTargetCurrent") == false)
                        hastable.Add(TargetName + ".IsMissTargetCurrent", item.IsMissTargetCurrent);
                    else
                        hastable[TargetName + ".IsMissTargetCurrent"] = item.IsMissTargetCurrent;

                    if (hastable.ContainsKey(TargetName + ".LastIsMissTarget") == false)
                        hastable.Add(TargetName + ".LastIsMissTarget", item.LastIsMissTarget);
                    else
                        hastable[TargetName + ".LastIsMissTarget"] = item.LastIsMissTarget;

                    if (hastable.ContainsKey(TargetName + ".CompanyProperty1") == false)
                        hastable.Add(TargetName + ".CompanyProperty1", item.CompanyProperty1);
                    else
                        hastable[TargetName + ".CompanyProperty1"] = item.CompanyProperty1;

                    if (hastable.ContainsKey(TargetName + ".CompanyName") == false)
                        hastable.Add(TargetName + ".CompanyName", item.CompanyName);
                    else
                        hastable[TargetName + ".CompanyName"] = item.CompanyName;

                    if (hastable.ContainsKey(TargetName + ".LastIsCommitDate") == false)
                        hastable.Add(TargetName + ".LastIsCommitDate", item.LastIsCommitDate);
                    else
                        hastable[TargetName + ".LastIsCommitDate"] = item.LastIsCommitDate;

                    if (hastable.ContainsKey(TargetName + ".CurrentMonthCommitDate") == false) //填写的要求时间
                        hastable.Add(TargetName + ".CurrentMonthCommitDate", item.CurrentMonthCommitDate);
                    else
                        hastable[TargetName + ".CurrentMonthCommitDate"] = item.CurrentMonthCommitDate;


                    if (hastable.ContainsKey(TargetName + ".CurrentMonthCommitReason") == false) //填写的要求说明
                        hastable.Add(TargetName + ".CurrentMonthCommitReason", item.CurrentMonthCommitReason);
                    else
                        hastable[TargetName + ".CurrentMonthCommitReason"] = item.CurrentMonthCommitReason;


                      if (hastable.ContainsKey(TargetName + ".LastNAccumulativeActualAmmount") == false)
                        hastable.Add(TargetName + ".LastNAccumulativeActualAmmount", item.LastNAccumulativeActualAmmount);
                    else
                        hastable[TargetName + ".LastNAccumulativeActualAmmount"] = item.LastNAccumulativeActualAmmount;

                      if (hastable.ContainsKey(TargetName + ".LastNAccumulativeDifference") == false)
                        hastable.Add(TargetName + ".LastNAccumulativeDifference", item.LastNAccumulativeDifference);
                    else
                        hastable[TargetName + ".LastNAccumulativeDifference"] = item.LastNAccumulativeDifference;

                      if (hastable.ContainsKey(TargetName + ".LastNAccumulativePlanAmmount") == false)
                        hastable.Add(TargetName + ".LastNAccumulativePlanAmmount", item.LastNAccumulativePlanAmmount);
                    else
                        hastable[TargetName + ".LastNAccumulativePlanAmmount"] = item.LastNAccumulativePlanAmmount;

                }
            }

            return hastable;

        }

    }
}
