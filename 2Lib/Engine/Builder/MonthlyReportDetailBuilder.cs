using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Engine
{
    class MonthlyReportDetailBuilder : BaseBuilder<A_MonthlyReportDetail>
    {
        private static MonthlyReportDetailBuilder _Instance = new MonthlyReportDetailBuilder();

        public static MonthlyReportDetailBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }
}
