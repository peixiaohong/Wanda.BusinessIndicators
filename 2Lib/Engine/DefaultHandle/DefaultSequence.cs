using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    public class DefaultSequence : ISequence
    {
        public List<MonthlyReportDetail> GetSequence(List<MonthlyReportDetail> RptDetailList, string Paramters)
        {
            RptDetailList.Sort(new MonthlyReportActualRateCompare());
            return RptDetailList;
        }
    }

    /// <summary>
    /// 默认按照公司排序
    /// </summary>
    public class DefaultCompanySequence : ISequence
    {
        public List<MonthlyReportDetail> GetSequence(List<MonthlyReportDetail> RptDetailList, string Paramters)
        {
            return RptDetailList.OrderBy(p => p.Company.Sequence).ToList(); ;
        }
    }



    /// <summary>
    /// 当月排序
    /// </summary>
    public class CurrentMonthSequence : ISequence
    {
        public List<MonthlyReportDetail> GetSequence(List<MonthlyReportDetail> RptDetailList, string Paramters)
        {
            List<MonthlyReportDetail> result = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> r1 = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> r2 = new List<MonthlyReportDetail>();

            r1.AddRange(RptDetailList.FindAll(R => R.Counter <= 0)); //完成的
            r2.AddRange(RptDetailList.FindAll(R => R.Counter > 0)); //未完成
            r1.Sort(new MonthlyReportMonthlyActualRateCompare()); //
            r2.Sort(new MonthlyReportMonthlyActualRateCompare());
            result.AddRange(r1);
            result.AddRange(r2);

            return result;
        }
    }

    /// <summary>
    /// 当月排序规则
    /// </summary>
    public class MonthlyReportMonthlyActualRateCompare : IComparer<MonthlyReportDetail>
    {
        public int Compare(MonthlyReportDetail x, MonthlyReportDetail y)
        {
            if (x == null)
            {
                return 1;  // 两两比较  1：在上 ， -1 ：在下 0:无序排列
            }
            if (y == null)
            {
                return -1;
            }

            //完成和未完成对比
            if ((x.IsMissTargetCurrent && !y.IsMissTargetCurrent) || (x.NDifference < 0 && y.NDifference >= 0))
            {
                return 1;
            }
            if ((!x.IsMissTargetCurrent && y.IsMissTargetCurrent) || (x.NDifference >= 0 && y.NDifference < 0))
            {
                return -1;
            }



            //计划实际均为0的显示排序
            if (x.NPlanAmmount == 0 && x.NActualAmmount == 0)
            {
                return 1;
            }
            if (y.NPlanAmmount == 0 && y.NActualAmmount == 0)
            {
                return -1;
            }

            ActualRate tx = JsonHelper.Deserialize<ActualRate>(x.NActualRate);
            ActualRate ty = JsonHelper.Deserialize<ActualRate>(y.NActualRate);


            if (tx.RateType > ty.RateType)
                return 1;
            if (tx.RateType == ty.RateType)
            {
                if (tx.Rate < ty.Rate)
                {
                    if (tx.RateType != 1 && x.IsMissTargetCurrent)
                    {
                        return -1;
                    }//筹备门店的单独算法 Update 2015-5-14 Start
                    else if (tx.RateType != 1 && x.IsMissTargetCurrent == false)
                    {
                        if (x.NDisplayRate.Substring(0, 2) == "增亏")
                            return -1;
                        else
                            return 1;
                    }
                    //筹备门店的单独算法 Update 2015-5-14 End
                    return 1;
                }
                else if (tx.Rate == ty.Rate)
                {
                    if (x.NActualAmmount < y.NActualAmmount)
                    {
                        return 1;
                    }
                    else if (x.NActualAmmount == y.NActualAmmount)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (tx.RateType != 1 && x.IsMissTargetCurrent)
                    {
                        return 1;
                    }
                    else if (tx.RateType != 1 && x.IsMissTargetCurrent == false) //筹备门店的单独算法
                    {
                        if (x.NDisplayRate.Substring(0, 2) == "增亏")
                            return 1;
                        else
                            return -1;
                    }
                    return -1;
                }
            }
            else
                return -1;
        }
    }


    /// <summary>
    /// 累计排序规则
    /// </summary>
    public class MonthlyReportActualRateCompare : IComparer<MonthlyReportDetail>
    {
        public int Compare(MonthlyReportDetail x, MonthlyReportDetail y)
        {

            if (x == null) 
            { 
                return 1; 
            }
            if (y == null) 
            { 
                return -1; 
            }

            if(x.CompanyProperty1 !="筹备门店" && y.CompanyProperty1 !="筹备门店" )
            {
                //完成和未完成对比
                if ( (x.IsMissTarget && !y.IsMissTarget) || (x.NAccumulativeDifference<0 && y.NAccumulativeDifference>=0))
                {
                    return 1;
                }
                if ((!x.IsMissTarget && y.IsMissTarget) || (x.NAccumulativeDifference>=0 && y.NAccumulativeDifference<0))
                {
                    return -1;
                }
            }

            if (string.IsNullOrEmpty(x.NAccumulativeActualRate)) { return 1; }
            if (string.IsNullOrEmpty(y.NAccumulativeActualRate)) { return -1; }

            //计划实际均为0的显示排序
            if (x.NAccumulativePlanAmmount == 0 && x.NAccumulativeActualAmmount == 0)
            {
                return -1;
            }
            if (y.NAccumulativePlanAmmount == 0 && y.NAccumulativeActualAmmount == 0)
            {
                return 1;
            }


            ActualRate tx = JsonHelper.Deserialize<ActualRate>(x.NAccumulativeActualRate);
            ActualRate ty = JsonHelper.Deserialize<ActualRate>(y.NAccumulativeActualRate);

            //这里RateType=1 ：数字， RateType =2 :增亏，减亏 RateType=3：超计划

            if (tx.RateType > ty.RateType)  //增亏 ，减亏，超计划
                return 1;
            if (tx.RateType == ty.RateType)  //当类型相同时，这里特殊处理增亏，减亏
            {
                if (tx.Rate < ty.Rate)
                {
                    if (tx.RateType != 1 && x.IsMissTarget )
                    {
                        return -1;
                    }//筹备门店的单独算法 Update 2015-5-14 Start
                    else if (tx.RateType != 1 && x.IsMissTarget == false) 
                    {
                        if (x.NAccumulativeDisplayRate.Substring(0, 2) == "增亏")
                            return -1;
                        else
                            return 1;
                    }
                    //筹备门店的单独算法 Update 2015-5-14 End
                    return 1;
                }
                else if (tx.Rate == ty.Rate)
                {
                    if (x.NAccumulativeActualAmmount < y.NAccumulativeActualAmmount)
                    {
                        return 1;
                    }
                    else if (x.NAccumulativeActualAmmount == y.NAccumulativeActualAmmount)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (tx.RateType != 1 && x.IsMissTarget)
                    {
                        return 1;
                    }//筹备门店的单独算法 Update 2015-5-14 Start
                    else if (tx.RateType != 1 && x.IsMissTarget == false) 
                    {
                        if (x.NAccumulativeDisplayRate.Substring(0, 2) == "增亏")
                            return 1;
                        else
                            return -1;
                    }//筹备门店的单独算法 Update 2015-5-14 End
                    return -1;
                }
            }
            else
                return -1;
        }
    }

}
