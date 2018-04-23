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

    //[ORViewMapping(@"
    //        SELECT A_MonthlyReportDetail.*,C_Company.CompanyName,C_Company.NeedEvaluation,C_Company.Sequence FROM dbo.A_MonthlyReportDetail	INNER JOIN dbo.C_Company ON 
    //            dbo.A_MonthlyReportDetail.SystemID = dbo.C_Company.SystemID AND 
    //                dbo.A_MonthlyReportDetail.CompanyID = dbo.C_Company.ID  ",
    //             "MonthlyReportDetailVModel")]

    public class MonthlyReportDetail : BaseModel, IBaseComposedModel
    {
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }

        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("TargetPlanID")]
        public Guid TargetPlanID { get; set; }

        [ORFieldMapping("OPlanAmmount")]
        public decimal OPlanAmmount { get; set; }

        [ORFieldMapping("OActualAmmount")]
        public decimal OActualAmmount { get; set; }

        [ORFieldMapping("OActualRate")]
        public string OActualRate { get; set; }

        [ORFieldMapping("ODisplayRate")]
        public string ODisplayRate { get; set; }

        [ORFieldMapping("NPlanAmmount")]
        public decimal NPlanAmmount { get; set; }
        
        [ORFieldMapping("NActualAmmount")]
        public decimal NActualAmmount { get; set; }


        [ORFieldMapping("NeedEvaluation")]
        public int NeedEvaluation { get; set; }

        [ORFieldMapping("NActualRate")]
        public string NActualRate { get; set; }

        [ORFieldMapping("NDisplayRate")]
        public string NDisplayRate { get; set; }

        [ORFieldMapping("OAccumulativePlanAmmount")]
        public decimal OAccumulativePlanAmmount { get; set; }

        [ORFieldMapping("OAccumulativeActualAmmount")]
        public decimal OAccumulativeActualAmmount { get; set; }

        [ORFieldMapping("OAccumulativeActualRate")]
        public string OAccumulativeActualRate { get; set; }

        [ORFieldMapping("OAcccumulativeDisplayRate")]
        public string OAcccumulativeDisplayRate { get; set; }

        [ORFieldMapping("NAccumulativePlanAmmount")]
        public decimal NAccumulativePlanAmmount { get; set; }

        [ORFieldMapping("NAccumulativeActualAmmount")]
        public decimal NAccumulativeActualAmmount { get; set; }

   
        [ORFieldMapping("NAccumulativeActualRate")]
        public string NAccumulativeActualRate { get; set; }

        [ORFieldMapping("NAccumulativeDisplayRate")]
        public string NAccumulativeDisplayRate { get; set; }

        [ORFieldMapping("IsMissTarget")]
        public bool IsMissTarget { get; set; }

        [ORFieldMapping("Counter")]
        public int Counter { get; set; }

        [ORFieldMapping("FirstMissTargetDate")]
        public DateTime? FirstMissTargetDate { get; set; }

        [ORFieldMapping("PromissDate")]
        public DateTime? PromissDate { get; set; }

        [ORFieldMapping("CommitDate")]
        public DateTime? CommitDate { get; set; }

        [ORFieldMapping("MIssTargetReason")]
        public string MIssTargetReason { get; set; }

        [ORFieldMapping("MIssTargetDescription")]
        public string MIssTargetDescription { get; set; }
        /// <summary>
        /// 年度指标
        /// </summary>
        public decimal NPlanAmmountByYear { get; set; }
        /// <summary>
        /// 年度实际
        /// </summary>
        public decimal NActualAmmountByYear { get; set; }
        /// <summary>
        /// 年度完成率
        /// </summary>
        public string NDisplayRateByYear { get; set; }

        /// <summary>
        /// 项目公司上级区域ID
        /// </summary>
        public Guid AreaID { get; set; }

        /// <summary>
        /// 项目公司上级区域层级
        /// </summary>
        public int AreaLevel { get; set; }

        /// <summary>
        /// 项目公司上级区域Name
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 当月未完成说明
        /// </summary>
        [ORFieldMapping("CurrentMIssTargetReason")]
        public string CurrentMIssTargetReason { get; set; }


        /// <summary>
        /// 当月采取措施
        /// </summary>
        [ORFieldMapping("CurrentMIssTargetDescription")]
        public string CurrentMIssTargetDescription { get; set; }



        [ORFieldMapping("ReturnType")]
        public int ReturnType { get; set; }

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

       [ORFieldMapping("IsMissTargetCurrent")]
        public bool IsMissTargetCurrent { get; set; }

       /// <summary>
       /// 差值(新)
       /// </summary>
       [ORFieldMapping("NDifference")]
       public decimal NDifference { get; set; }

       /// <summary>
       /// 差值(旧)
       /// </summary>
       [ORFieldMapping("ODifference")]
       public decimal ODifference { get; set; }

       /// <summary>
       /// 累计差值(新)
       /// </summary>
       [ORFieldMapping("NAccumulativeDifference")]
       public decimal NAccumulativeDifference { get; set; }

       /// <summary>
       /// 累计差值(旧)
       /// </summary>
       [ORFieldMapping("OAccumulativeDifference")]
       public decimal OAccumulativeDifference { get; set; }

       /// <summary>
       /// 月度上报ID 
       /// </summary>
       [ORFieldMapping("MonthlyReportID")]
       public Guid MonthlyReportID { get; set; }


       /// <summary>
       /// 当月的考核指数
       /// </summary>
       [ORFieldMapping("MeasureRate")]
       public decimal MeasureRate { get; set; }


       /// <summary>
       /// 补回情况
       /// </summary>
       [ORFieldMapping("ReturnDescription")]
       public string ReturnDescription { get; set; }

       /// <summary>
       /// 是否是当月要求时间
       /// </summary>
       [ORFieldMapping("IsCommitDate")]
       public int IsCommitDate { get; set; }


       /// <summary>
       /// 要求说明(在没有要求时间的时候，显示)
       /// </summary>
       [ORFieldMapping("CommitReason")]
       public string CommitReason { get; set; }

        /// <summary>
        /// 延迟完成
        /// </summary>
       [ORFieldMapping("IsDelayComplete")]
       public bool IsDelayComplete { get; set; }


       /// <summary>
       /// 本月未完成要求时间
       /// </summary>
       [ORFieldMapping("CurrentMonthCommitDate")]
       public DateTime? CurrentMonthCommitDate { get; set; }


       /// <summary>
       /// 本月未完成要求说明 //---业务管理设置的字段
       /// </summary>
       [ORFieldMapping("CurrentMonthCommitReason")]
       public string CurrentMonthCommitReason { get; set; }


       /// <summary>
       /// 补回状态（子状态）
       /// </summary>
       [ORFieldMapping("ReturnType_Sub")]
       public string ReturnType_Sub { get; set; }

       /// <summary>
       /// 重新计算的Counter
       /// </summary>
       [ORFieldMapping("NewCounter")]
       public int NewCounter { get; set; }


       /// <summary>
       /// 排序
       /// </summary>
       [ORFieldMapping("Sequence")]
       public int Sequence { get; set; }

       /// <summary>
       /// 上月累计差值(新)
       /// </summary>
       public decimal LastNAccumulativeDifference { get; set; }

        /// <summary>
        /// 上月计划
        /// </summary>
       public decimal LastNAccumulativePlanAmmount { get; set; }

       /// <summary>
       /// 上月实际
       /// </summary>
       public decimal LastNAccumulativeActualAmmount { get; set; }

        /// <summary>
        /// 本月补回/增加差额
        /// </summary>
       public decimal AddDifference { get; set; }

       /// <summary>
       /// 上月累计未完成
       /// </summary>
       public bool LastIsMissTarget { get; set; }

       public int LastIsCommitDate { get; set; }


        [NoMapping]
        public C_Company Company { get; set; }
        [NoMapping]
        public int CompanySequence { get; set; }
        [NoMapping]
        public string Title { get; set; }
        [NoMapping]
        public string GroupTile { get; set; }

        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }

        [ORFieldMapping("TargetType")]
        public int TargetType { get; set; }

        [NoMapping]
        public string ReturnTypeDescription { get; set; }

        /// <summary>
        /// 分组指标个数
        /// </summary>
        [NoMapping]
        public int TargetGroupCount { get; set; }

        [ORFieldMapping("SystemName")]
        public string SystemName { get; set; }

        /// <summary>
        /// 是否显示当前数据到页面
        /// </summary>
        [ORFieldMapping("Display")]
        public bool Display { get; set; }

        [ORFieldMapping("CompanyProperty1")]
        public string CompanyProperty1 { get; set; }


        [ORFieldMapping("CompanyProperty")]
        public string CompanyProperty{ get; set; }

        /// <summary>
        /// 全年的总指标数据（未完成选择必保指标时用到）
        /// </summary>
        [NoMapping]
        public decimal AnnualTargetPlanValue { get; set; }

    }


        [ORViewMapping(@"
            SELECT A_TargetPlanDetail.*,C_Company.CompanyName FROM dbo.A_TargetPlanDetail	INNER JOIN dbo.C_Company ON 
                dbo.A_TargetPlanDetail.SystemID = dbo.C_Company.SystemID AND 
                    dbo.A_TargetPlanDetail.CompanyID = dbo.C_Company.ID  ",
                 "TargetPlanDetailVModel")]
    public class TargetPlanDetail : BaseModel, IBaseComposedModel
    {

        #region Public Properties
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }



        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }



        [ORFieldMapping("Target")]
        public decimal Target { get; set; }



        [ORFieldMapping("VersionStart")]
        public DateTime VersionStart { get; set; }



        [ORFieldMapping("Versionend")]
        public DateTime Versionend { get; set; }

        [ORFieldMapping("TargetPlanID")]
        public Guid TargetPlanID { get; set; }

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("OpeningTime")]
        public DateTime OpeningTime { get; set; }
        #endregion
    }
}
