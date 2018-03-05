using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;
using Wanda.BusinessIndicators.Model;
using Wanda.Platform.WorkFlow.ClientComponent;

namespace Wanda.BusinessIndicators.ViewModel
{
    public class TargetPlanDetailVList
    {
        public Guid TargetID { get; set; }
        public string TargetName { get; set; }
        public string Unit { get; set; }
        public List<TargetPlanDetailVModel> TargetPlanDetailList { get; set; }
        #region 单公司当月合计
        public decimal SumTarget1 { get; set; }
        public decimal SumTarget2 { get; set; }
        public decimal SumTarget3 { get; set; }
        public decimal SumTarget4 { get; set; }
        public decimal SumTarget5 { get; set; }
        public decimal SumTarget6 { get; set; }
        public decimal SumTarget7 { get; set; }
        public decimal SumTarget8 { get; set; }
        public decimal SumTarget9 { get; set; }
        public decimal SumTarget10 { get; set; }
        public decimal SumTarget11 { get; set; }
        public decimal SumTarget12 { get; set; }
        #endregion
        #region 单公司累计合计     
        public decimal SumTargetSum1 { get; set; }
        public decimal SumTargetSum2 { get; set; }
        public decimal SumTargetSum3 { get; set; }
        public decimal SumTargetSum4 { get; set; }
        public decimal SumTargetSum5 { get; set; }
        public decimal SumTargetSum6 { get; set; }
        public decimal SumTargetSum7 { get; set; }
        public decimal SumTargetSum8 { get; set; }
        public decimal SumTargetSum9 { get; set; }
        public decimal SumTargetSum10 { get; set; }
        public decimal SumTargetSum11 { get; set; }
        public decimal SumTargetSum12 { get; set; }
        #endregion
    }
    public class TargetPlanDetailVModel
    {
        public int seq { get; set; }       
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string OpenTime { get; set; }
        #region 单公司当月
        public decimal Target1 { get; set; }
        public decimal Target2 { get; set; }
        public decimal Target3 { get; set; }
        public decimal Target4 { get; set; }
        public decimal Target5 { get; set; }
        public decimal Target6 { get; set; }
        public decimal Target7 { get; set; }
        public decimal Target8 { get; set; }
        public decimal Target9 { get; set; }
        public decimal Target10 { get; set; }
        public decimal Target11 { get; set; }
        public decimal Target12 { get; set; }
        #endregion
        #region   单公司累计
        public decimal TargetSum1 { get; set; }//1月累计
        public decimal TargetSum2 { get; set; }//1-2月累计
        public decimal TargetSum3 { get; set; }
        public decimal TargetSum4 { get; set; }
        public decimal TargetSum5 { get; set; }
        public decimal TargetSum6 { get; set; }
        public decimal TargetSum7 { get; set; }
        public decimal TargetSum8 { get; set; }
        public decimal TargetSum9 { get; set; }
        public decimal TargetSum10 { get; set; }
        public decimal TargetSum11 { get; set; }
        public decimal TargetSum12{ get; set; }
        #endregion
    }
    public class TargetDetail
    {
        public int FinMonth { get; set; }

        public List<TargetDetailList> TargetDetailList { get; set; }
    }

    public class TargetDetailList
    {
        public Guid TargetID { get; set; }

        public string TargetName { get; set; }

        public decimal? Target { get; set; }

        public decimal? SumTarget { get; set; }
    }

    /// <summary>
    /// 新建导航Model 加入SystemID  可以从B_MonthlyReport中的system取得传入前台
    /// </summary>
    public class V_ReportApprove
    {
        public string SystemName { get; set; }
        public Guid SystemID { get; set; }
        public string CreatTime { get; set; }
        public int seq { get; set; }
        public bool IfComplate { get; set; }
        public string WFStatus { get; set; }
        public string ReportApprove { get; set; }
        public int Category { get; set; }
        public int Group { get; set; }
        public Guid BusinessID { get; set; }
        /// <summary>
        /// 每个系统下可能有多个流程,集合在一起
        /// </summary>
        public List<List<NavigatActivity>> list { get; set; }
    }
    /// <summary>
    /// 完成情况同期对比表中的备注
    /// </summary>
    public class MissTargetEvaluationModel
    {
        public string SystemName { get; set; }

        public string TargetName { get; set; }

        public string EvaluationRemark { get; set; }
    }




    /// <summary>
    /// 分解指标，移动端展示数据
    /// </summary>
    public class V_TargetPlan_Mobile
    {
        public Guid TargetID { get; set; }
        
        public decimal Target { get; set; }

        public string TargetName { get; set; }

        public int FinMonth { get; set; }

        //
        public Guid SystemID { get; set;}
        
    }




}
