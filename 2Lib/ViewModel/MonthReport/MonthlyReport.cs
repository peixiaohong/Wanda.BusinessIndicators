using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class MonthlyReport: BaseModel
    {
        public Guid SystemID { get; set; }
        public int FinYear { get; set; }
        public int FinMonth { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string WFStatus { get; set; }
        public Guid SystemBatchID { get; set; }
        public Guid TargetPlanID { get; set; }

    }
}
