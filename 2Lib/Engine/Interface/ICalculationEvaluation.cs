using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators
{
    /// <summary>
    /// 正常计算
    /// </summary>
    public interface ICalculationEvaluation
    {
        B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true);
    }

    /// <summary>
    /// 重新计算
    /// </summary>
    public interface IResetCalculationEvaluation
    {
        List<A_MonthlyReportDetail> ResetCalculation(List<A_MonthlyReportDetail> RptDetailList, List<A_MonthlyReportDetail> AllRptDetailList);
    }

}

