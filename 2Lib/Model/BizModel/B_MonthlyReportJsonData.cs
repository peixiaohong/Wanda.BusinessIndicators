using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
    [ORTableMapping("dbo.B_MonthlyReportJsonData")]
    public class B_MonthlyReportJsonData : BaseModel
    {
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }
        
        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }
        
        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }
        
        /// <summary>
        /// 计划类型（月报计划=M ，分机指标计划=T）
        /// </summary>
        [ORFieldMapping("PlanType")]
        public string PlanType { get; set; }

        /// <summary>
        /// 上报页面用到的JSon数据
        /// </summary>
        [ORFieldMapping("ReportJsonData")]
        public string ReportJsonData { get; set; }

        /// <summary>
        /// 查询页面--月报的JSon数据
        /// </summary>
        [ORFieldMapping("QuerrySumJsonData")]
        public string QuerrySumJsonData { get; set; }


        /// <summary>
        /// 查询页面--明细的JSon数据
        /// </summary>
        [ORFieldMapping("QuerryDetaileJsonData")]
        public string QuerryDetaileJsonData { get; set; }


        /// <summary>
        /// 查询页面--未完成的JSon数据
        /// </summary>
        [ORFieldMapping("QuerryMissJsonData")]
        public string QuerryMissJsonData { get; set; }



        /// <summary>
        /// 查询页面--当月未完成的JSon数据
        /// </summary>
        [ORFieldMapping("QuerryCurrentMissJsonData")]
        public string QuerryCurrentMissJsonData { get; set; }



        /// <summary>
        /// 查询页面--补回的JSon数据
        /// </summary>
        [ORFieldMapping("QuerryReturnJsonData")]
        public string QuerryReturnJsonData { get; set; }
        
    }
}
