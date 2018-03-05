using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanda.BusinessIndicators.ViewModel
{
    public class V_GroupMonthlyReportDetail
    {
        /// <summary>
        /// 明细主键ID
        /// </summary>
        public Guid ID {get;set;}

        /// <summary>
        /// 系统ID
        /// </summary>
        public Guid SystemID{get;set;}

        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid CompanyID{get;set;}

        /// <summary>
        /// 指标ID
        /// </summary>
        public Guid TargetID{get;set;}

        /// <summary>
        /// 计划数
        /// </summary>
         public decimal NPlanAmmount { get; set; }

        /// <summary>
        /// 实际数
        /// </summary>
        public decimal NActualAmmount { get; set; }

        /// <summary>
        ///完成率
        /// </summary>
        public string NActualRate{get;set;}
        /// <summary>
        /// 完成率（前台显示）
        /// </summary>
        public string NDisplayRate { get; set; }

        /// <summary>
        /// 累计计划数
        /// </summary>
        public decimal NAccumulativePlanAmmount { get; set; }

        /// <summary>
        /// 累计实际数
        /// </summary>
        public decimal NAccumulativeActualAmmount { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>
        public string NAccumulativeActualRate{get;set;}

        /// <summary>
        /// 完成率（前台显示）
        /// </summary>
        public string NAccumulativeDisplayRate { get; set; }

        
    }
}
