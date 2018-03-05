using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;
using Wanda.BusinessIndicators.Model;
using Lib.Data;

namespace Wanda.BusinessIndicators.ViewModel
{
    public class CompanyVModel
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyProperty1 { get; set; }
        public string CompanyProperty2 { get; set; }
        public string CompanyProperty3 { get; set; }
        public string CompanyProperty4 { get; set; }
        public string CompanyProperty5 { get; set; }
        public string CompanyProperty6 { get; set; }
        public string CompanyProperty7 { get; set; }
        public string CompanyProperty8 { get; set; }
        public string CompanyProperty9 { get; set; }
        public string OpeningTime { get; set; }
        public int Sequence { get; set; }
    }

    /// <summary>
    /// 未完成家数对比
    /// </summary>
    public class ContrastMisstarget
    {
        public Guid TargetID { get; set; }

        public string TargetName { get; set; }

        #region 本月数据
        /// <summary>
        /// 本月考核家数
        /// </summary>
        public int ThisEvaluationCompany { get; set; }

        /// <summary>
        /// 本月累计未完成公司数量
        /// </summary>
        public int ThisIsMissTarget { get; set; }

        /// <summary>
        /// 本月当月未完成公司数量
        /// </summary>
        public int ThisIsMissCurrent { get; set; }

        /// <summary>
        /// 本月累计指标占比
        /// </summary>
        public string ThisProportion { get; set; }

        /// <summary>
        /// 本月当月指标占比
        /// </summary>
        public string ThisProportionCurrent { get; set; }

        #endregion

        #region 上月数据
        /// <summary>
        /// 上期考核家数
        /// </summary>
        public int LastEvaluationCompany { get; set; }

        /// <summary>
        /// 上月累计未完成公司数量
        /// </summary>
        public int LastIsMissTarget { get; set; }

        /// <summary>
        /// 上月当月未完成公司数量
        /// </summary>
        public int LastIsMissCurrent { get; set; }

        /// <summary>
        /// 上月累计指标占比
        /// </summary>
        public string LastProportion { get; set; }

        /// <summary>
        /// 上月当月指标占比
        /// </summary>
        public string LastProportionCurrent { get; set; }
        #endregion

        #region 环比数据
        /// <summary>
        /// 环比累计未完成家数变化
        /// </summary>
        public int HuanMissTargetChange { get; set; }

        /// <summary>
        /// 环比当月未完成家数变化
        /// </summary>
        public int HuanMissTargetChangeCurrent { get; set; }

        /// <summary>
        /// 环比累计占比变化
        /// </summary>
        public string HuanMissTargetProportion { get; set; }
        /// <summary>
        /// 环比当月占比变化
        /// </summary>
        public string HuanMissTargetPCurrent { get; set; }
        #endregion

        #region 去年数据
        /// <summary>
        /// 去年考核家数
        /// </summary>
        public int YearEvaluationCompany { get; set; }

        /// <summary>
        /// 上月累计未完成公司数量
        /// </summary>
        public int YearIsMissTarget { get; set; }

        /// <summary>
        /// 上月当月未完成公司数量
        /// </summary>
        public int YearIsMissCurrent { get; set; }

        /// <summary>
        /// 上月累计指标占比
        /// </summary>
        public string YearProportion { get; set; }

        /// <summary>
        /// 上月当月指标占比
        /// </summary>
        public string YearProportionCurrent { get; set; }

        #endregion

        #region 同比数据
        /// <summary>
        /// 同比累计未完成家数变化
        /// </summary>
        public int TongMissTargetChange { get; set; }

        /// <summary>
        /// 同比当月未完成家数变化
        /// </summary>
        public int TongMissTargetChangeCurrent { get; set; }

        /// <summary>
        /// 同比累计占比变化
        /// </summary>
        public string TongMissTargetProportion { get; set; }
        /// <summary>
        /// 同比当月占比变化
        /// </summary>
        public string TongMissTargetPCurrent { get; set; }
        #endregion
    }

    public class ContrastMisstargetList
    {
        public string SystemName { get; set; }

        public List<ContrastMisstarget> ContrastMisstarget { get; set; }
    }
    public class YearModel
    {
        public string Year { get; set; }
        public string YearValue { get; set; }
    }
    public class MonthlyReportVM : BaseModel, IBaseComposedModel
    {


         [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

         [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

         [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

         [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

         [ORFieldMapping("CompanyProperty1")]
        public string CompanyProperty1 { get; set; }

         [ORFieldMapping("IsMissTarget")]
        public bool IsMissTarget { get; set; }

         [ORFieldMapping("Counter")]
        public int Counter { get; set; }

         [ORFieldMapping("IsMissTargetCurrent")]
        public bool IsMissTargetCurrent { get; set; }

         [ORFieldMapping("NAccumulativeActualAmmount")]
        public decimal NAccumulativeActualAmmount { get; set; }
    }
}
