using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class ComprehensiveReportForm : BaseModel, IBaseComposedModel
    {
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }

        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }

        [ORFieldMapping("TargetType")]
        public int TargetType { get; set; }

        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }

        //当月指标数
        [ORFieldMapping("NPlanAmmount")]
        public decimal NPlanAmmount { get; set; }

        //当月上报数
        [ORFieldMapping("NActualAmmount")]
        public decimal NActualAmmount { get; set; }

        //当月差值
        [ORFieldMapping("NDifference")]
        public decimal NDifference { get; set; }

        // 累计指标数
        [ORFieldMapping("NAccumulativePlanAmmount")]
        public decimal NAccumulativePlanAmmount { get; set; }

        // 累计上报数
        [ORFieldMapping("NAccumulativeActualAmmount")]
        public decimal NAccumulativeActualAmmount { get; set; }

        /// <summary>
        /// 累计差值
        /// </summary>
        [ORFieldMapping("NAccumulativeDifference")]
        public decimal NAccumulativeDifference { get; set; }

        [ORFieldMapping("NActualRate")]
        public decimal NActualRate { get; set; }

        [ORFieldMapping("NAccumulativeActualRate")]
        public decimal NAccumulativeActualRate { get; set; }
    }
}
