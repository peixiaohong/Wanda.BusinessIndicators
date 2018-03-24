using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.ViewModel
{
    public class TargetPlanViewModel : BaseModel, IBaseComposedModel
    {
        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("NPlanAmmount")]
        public decimal NPlanAmmount { get; set; }

        [ORFieldMapping("NAccumulativePlanAmmount")]
        public decimal NAccumulativePlanAmmount { get; set; }

        /// <summary>
        /// 当月实际数 --- 国内院线同步的数据
        /// </summary>
        public decimal NActualAmmount { get; set; }
    }

    /// <summary>
    /// Model是为了给未完成，选择必保指标，显示的全年计划指标数据，而建的
    /// </summary>
    public class V_PlanTargetModel 
    {
        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("Target")]
        public decimal Target { get; set; }
        
    }



}
