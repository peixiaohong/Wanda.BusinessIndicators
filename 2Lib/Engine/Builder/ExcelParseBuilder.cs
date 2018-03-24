using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Engine;

namespace LJTH.BusinessIndicators.Engine
{
    public class ExcelParseBuilder: BaseBuilder<IExcelParse>
    {
        private static ExcelParseBuilder _Instance = new ExcelParseBuilder();

        public static ExcelParseBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }
}
