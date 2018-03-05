using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;


namespace Wanda.BusinessIndicators.Model
{
    [ORTableMapping("dbo.B_TargetPlanAction")]
    public class B_TargetPlanAction : BaseModel
    {
        #region Public Properties



        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }



        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("TargetPlanID")]
        public Guid TargetPlanID { get; set; }



        [ORFieldMapping("Action")]
        public string Action { get; set; }


        [ORFieldMapping("Operator")]
        public string Operator { get; set; }

        [ORFieldMapping("OperatorTime")]
        public DateTime OperatorTime { get; set; }

        [ORFieldMapping("Description")]
        public string Description { get; set; }



        #endregion
    }
}
