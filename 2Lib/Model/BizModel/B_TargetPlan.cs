using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a TargetPlan.
	/// </summary>
    [ORTableMapping("dbo.B_TargetPlan")]
    public class B_TargetPlan : BaseModel
	{
	 
		#region Public Properties 
		 
		
       
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }



        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("Description")]
        public string Description { get; set; }



        [ORFieldMapping("VersionStart")]
        public DateTime VersionStart { get; set; }



        [ORFieldMapping("Versionend")]
        public DateTime Versionend { get; set; }

        [ORFieldMapping("status")]
        public int Status { get; set; }

        /// <summary>
        /// 工作流状态
        /// </summary>
        [ORFieldMapping("WFStatus")]
        public string WFStatus { get; set; }


        /// <summary>
        /// 当前提交人
        /// </summary>
        [ORFieldMapping("ProcessOwn")]
        public string ProcessOwn { get; set; }

        /// <summary>
        /// 批次ID
        /// </summary>
        [ORFieldMapping("SystemBatchID")]
        public Guid SystemBatchID { get; set; }

        
        /// <summary>
        /// 判断似否生成Excel
        /// </summary>
        [ORFieldMapping("CreatorID")]
        public int CreatorID { get; set; }

        /// <summary>
        /// 实时上报审批情况 
        /// </summary>
        [ORFieldMapping("ReportApprove")]
        public string ReportApprove { get; set; }

        /// <summary>
        /// 是否是当前指标
        /// </summary>
        [NoMapping]
        public bool IfCurrentTarget { get; set; }
		#endregion
		
		 
	} 
}

