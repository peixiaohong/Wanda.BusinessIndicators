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
    /// 本期考核范围内家数
    /// </summary>
    [ORTableMapping("dbo.R_MissTargetEvaluationScope")]
    public class R_MissTargetEvaluationScope : BaseModel
    {
        /// <summary>
        /// 考核类型
        /// </summary>
        [ORFieldMapping("EvaluationType")]
        public string EvaluationType { get; set; }

        /// <summary>
        /// 考核数量
        /// </summary>
       [ORFieldMapping("EvaluationNumber")]
        public int EvaluationNumber { get; set; }
        

        /// <summary>
        /// 系统ID
        /// </summary>
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        /// <summary>
        /// 指标ID
        /// </summary>
        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }
        

        /// <summary>
        /// 月
        /// </summary>
        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }
      
        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("ContrastRemark")]
        public string ContrastRemark { get; set; }


        /// <summary>
        /// 未完成的公司数量
        /// </summary>
        [ORFieldMapping("MissTargetNumber")]
        public int MissTargetNumber { get; set; }
        
        

    }
}
