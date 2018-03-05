using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
    [ORTableMapping("dbo.B_BusinessBase")]
    [Serializable]
    public class B_BusinessBase : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("BusinessType")]
        public string BusinessType { get; set; }



        [ORFieldMapping("MonthlyReportID")]
        public Guid MonthlyReportID { get; set; }



        [ORFieldMapping("SystemName")]
        public string SystemName { get; set; }


        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }


        [ORFieldMapping("Description")]
        public string Description { get; set; }



        [ORFieldMapping("ProcessType")]
        public int ProcessType { get; set; }


        [ORFieldMapping("Status")]
        public int Status { get; set; }


        [ORFieldMapping("ReportPerson")]
        public string ReportPerson { get; set; }


        [ORFieldMapping("ReportPersonCTX")]
        public string ReportPersonCTX { get; set; }


        [ORFieldMapping("ReportPersonDepart")]
        public string ReportPersonDepart { get; set; }


        [ORFieldMapping("ReportDate")]
        public DateTime ? ReportDate { get; set; }


        [ORFieldMapping("FormData")]
        public string FormData { get; set; }





        #endregion
    }
}
