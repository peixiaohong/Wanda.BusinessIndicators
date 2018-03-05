using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.ViewModel
{
      [Serializable]
    public class MissTargetReturnVModel
    {
        public List<C_Target> TargetList { get; set; }

        public List<MonthlyReportDetail> MonthlyReportList { get; set; }

    }


}
