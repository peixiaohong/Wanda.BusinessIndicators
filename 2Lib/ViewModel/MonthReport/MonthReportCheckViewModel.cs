using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.ViewModel
{

    /// <summary>
    /// 月度审批页面展示用
    /// </summary>
    [Serializable]
    public class MonthReportCheckViewModel
    {

        /// <summary>
        /// 系统ID
        /// </summary>
       public Guid SystemID { get; set; }

        /// <summary>
        /// 模版类型
        /// </summary>
       public string SysTemplateType { get; set; }

   
        /// <summary>
        /// 指标
        /// </summary>
       public List<C_Target> TargetList { get; set; }

        /// <summary>
        /// 指标计划列表
        /// </summary>
       public List<A_TargetPlanDetail> ATargetPlanDetailList { get; set; }


       public A_MonthlyReport AMonthlyReport { get; set; }

       public B_MonthlyReport BMonthlyReport { get; set; }

       public List<B_MonthlyReportDetail> BMonthlyReportDetailList { get; set; }

       public List<A_MonthlyReportDetail> AMonthlyReportDetailList { get; set; }

       
       

    }



    /// <summary>
    /// 月度报表统计
    /// </summary>
    public class MonthReportTotalViewModel
    {
        /// <summary>
        /// 指标名称
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 指标计划数
        /// </summary>
        public double TargetPlan { get; set; }

        /// <summary>
        /// 实际数
        /// </summary>
        public double ActualAmmount { get; set; }




    }

    /// <summary>
    /// 月度报表统计
    /// </summary>
    [Serializable]
    public class MonthReportSummaryViewModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 明细ID
        /// </summary>
        public Guid MonthlyDetailID { get; set; }
        /// <summary>
        /// 指标ID
        /// </summary>
        public Guid TargetID { get; set; }
        /// <summary>
        /// 系统ID
        /// </summary>
        public Guid SystemID { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public int FinYear { get; set; }
        /// <summary>
        /// 指标名称
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 年度指标
        /// </summary>
        public string MeasureRate { get; set; }

        /// <summary>
        /// 年度指标
        /// </summary>
        public double MeasureRate1 { get; set; }

        /// <summary>
        /// 指标计划数
        /// </summary>
        public double NPlanAmmount { get; set; }

        /// <summary>
        /// 实际数
        /// </summary>
        public double NActualAmmount { get; set; }
        /// <summary>
        /// 差额
        /// </summary>
        public double NDifference { get; set; }

        /// <summary>
        /// 实际完成率
        /// </summary>
        public string NActualRate { get; set; }

        /// <summary>
        /// 年累计指标计划数
        /// </summary>
        public double NAccumulativePlanAmmount { get; set; }

        /// <summary>
        /// 年累计实际数
        /// </summary>
        public double NAccumulativeActualAmmount { get; set; }
        /// <summary>
        /// 差额
        /// </summary>
        public double NAccumulativeDifference { get; set; }

        /// <summary>
        /// 年累计完成率
        /// </summary>
        public string NAccumulativeActualRate { get; set; }

        /// <summary>
        /// 全年完成率
        /// </summary>
        public string NAnnualCompletionRate { get; set; }
        /// <summary>
        /// 警示灯
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// 指标计划数（商管专用）
        /// </summary>
        public string NPlanStr { get; set; }

        /// <summary>
        /// 实际数（商管专用）
        /// </summary>
        public string NActualStr { get; set; }

        /// <summary>
        /// 年累计指标计划数（商管专用）
        /// </summary>
        public string NAccumulativePlanStr { get; set; }

        /// <summary>
        /// 年累计实际数（商管专用）
        /// </summary>
        public string NAccumulativeActualStr { get; set; }

        /// <summary>
        /// 当月差额
        /// </summary>
        public string NDifferStr { get; set; }

        /// <summary>
        /// 年累计差额
        /// </summary>
        public string NAccDiffereStr { get; set; }


        /// <summary>
        /// 月度（该字段是在汇总报表的时候用到）
        /// </summary>
        public int FinMonth { get; set; }

        /// <summary>
        /// 系统名称（该字段是在汇总报表的时候用到）
        /// </summary>
        public string SystemName { get; set; }
    }

}
