using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Engine;

namespace Wanda.BusinessIndicators.Engine
{
    public class DisplayRateBuilder : BaseBuilder<IGetDisplayRate>
    {
        private static DisplayRateBuilder _Instance = new DisplayRateBuilder();

        public static DisplayRateBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }

    public class MonthDescriptionValueBuilder : BaseBuilder<IGetMonthDescriptionValue>
    {
        private static MonthDescriptionValueBuilder _Instance = new MonthDescriptionValueBuilder();

        public static MonthDescriptionValueBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }

}
