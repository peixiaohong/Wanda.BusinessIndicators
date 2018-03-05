using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    public interface ISequence
    {
        List<MonthlyReportDetail> GetSequence( List<MonthlyReportDetail> RptDetailList, string Paramter=null);
    }
}
