using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a MonthlyReport.
    /// </summary>
    [ORTableMapping("dbo.B_MonthlyReport")]
    public class B_MonthlyReport : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }



        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }



        [ORFieldMapping("Description")]
        public string Description { get; set; }

        [ORFieldMapping("status")]
        public int Status { get; set; }

        /// <summary>
        /// 工作流状态
        /// </summary>
        [ORFieldMapping("WFStatus")]
        public string WFStatus { get; set; }


        /// <summary>
        /// 实时上报审批情况
        /// </summary>
        [ORFieldMapping("ReportApprove")]
        public string ReportApprove { get; set; }
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
        /// 判断似否生成
        /// </summary>
        [ORFieldMapping("CreatorID")]
        public int CreatorID{get;set;}

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 数据优化，存储的ReportInstance的Json数据
        /// </summary>
        [ORFieldMapping("DataOptimizationJson")]
        public String DataOptimizationJson { get; set; }


        /// <summary>
        /// 大区ID
        /// </summary>
        //[ORFieldMapping("AreaID")]
        //public Guid AreaID { get; set; }

    #endregion


    }
}

