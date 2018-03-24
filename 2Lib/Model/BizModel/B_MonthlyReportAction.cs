using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a MonthlyReportDetail.
    /// </summary>
    [ORTableMapping("dbo.B_MonthlyReportAction")]
    public class B_MonthlyReportAction:BaseModel
    {
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }


        [ORFieldMapping("MonthlyReportID")]
        public Guid MonthlyReportID { get; set; }


        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }


        [ORFieldMapping("Action")]
        public string Action { get; set; }


        [ORFieldMapping("Operator")]
        public string Operator { get; set; }


        [ORFieldMapping("OperatorTime")]
        public DateTime? OperatorTime { get; set; }


        [ORFieldMapping("Description")]
        public string Description { get; set; }

    }
}
