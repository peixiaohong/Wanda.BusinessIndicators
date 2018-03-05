using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;
using Wanda.BusinessIndicators.Model;
namespace Wanda.BusinessIndicators.ViewModel
{
    public class SimpleReportVModel : BaseModel
    {
        public List<SRptModel> List = new List<SRptModel>();
       
    }
    public class SRptModel
    {
         public int Index { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Unit { get; set; }

        public List<B_MonthlyReportDetail> TotalList { get; set; }
        public List<B_MonthlyReportDetail> CurrentList { get; set; }
    }
}
