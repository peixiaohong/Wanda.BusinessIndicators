using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.Model
{
    [ORTableMapping("dbo.A_SystemBatch")]
    public class A_SystemBatch : BaseModel
    {
        /// <summary>
        /// 批次类型
        /// </summary>
        [ORFieldMapping("BatchType")]
        public string BatchType { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }

        [ORFieldMapping("WFBatchStatus")]
        public string WFBatchStatus { get; set; }


        [ORFieldMapping("SubReport")]
        public string SubReport { get; set; }

        [ORFieldMapping("Opinions")]
        public string Opinions { get; set; }


        /// <summary>
        /// 批次的审批流程意见
        /// </summary>
        [ORFieldMapping("Batch_Opinions")]
        public string Batch_Opinions { get; set; }


        [ORFieldMapping("ReportApprove")]
        public string ReportApprove { get; set; }
        [ORFieldMapping("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 指标计划ID
        /// </summary>
        [ORFieldMapping("TargetPlanID")]
        public Guid TargetPlanID { get; set; }
    }
}
