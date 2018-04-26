using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.ViewModel.MonthReport
{
    /// <summary>
    /// 系统综合报表
    /// </summary>
    public class ComprehensiveReportViewModel
    {
        //系统ID
        public Guid SystemID { get; set; }

        /// <summary>
        /// 板块名称
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 考核指标
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 月计划数
        /// </summary>
        public decimal NPlanAmmount { get; set; }

        /// <summary>
        /// 月实际数
        /// </summary>
        public decimal NActualAmmount { get; set; }

        /// <summary>
        /// 月 差额
        /// </summary>
        public decimal NDifference { get; set; }

        /// <summary>
        /// 月完成率
        /// </summary>
        public string NDisplayRate { get; set; }

        /// <summary>
        /// 年计划数
        /// </summary>
        public decimal NAccumulativePlanAmmount { get; set; }

        /// <summary>
        /// 年实际数
        /// </summary>
        public decimal NAccumulativeActualAmmount { get; set; }

        /// <summary>
        /// 年 差额
        /// </summary>
        public decimal NAccumulativeDifference { get; set; }

        /// <summary>
        /// 年完成率
        /// </summary>
        public string NAccumulativeDisplayRate { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Number { get; set; }
    }
}
