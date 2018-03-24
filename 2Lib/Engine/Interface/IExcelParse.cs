using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Engine
{
    public interface IExcelParse
    {
        List<string> ParseExcel(Stream ExcelFile);
    }
}
