using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanda.BusinessIndicators.Common
{
    /// <summary>
    /// 缓存KEY
    /// </summary>
    public enum CacheKey
    {
        CONGLOMERATE,
        PINYIN
    }

    /// <summary>
    /// 补回状态
    /// </summary>
    public enum EnumReturnType
    {
        /// <summary>
        /// 补回中
        /// </summary>
        [EnumItemDescription("补回中")]
        Returning = 1,


        /// <summary>
        /// 到期未补回
        /// </summary>
        [EnumItemDescription("到期未补回")]
        NotReturn = 2,

        /// <summary>
        /// 预计无法按期补回
        /// </summary>
        [EnumItemDescription("预计无法按期补回")]
        UnableReturnByMonth = 3,

        /// <summary>
        /// 预计年内无法补回
        /// </summary>
        [EnumItemDescription("预计年内无法补回")]
        UnableReturnByYear = 4,

        /// <summary>
        /// 本月新增指标
        /// </summary>
        [EnumItemDescription("本月新增")]
        New = 5,

        /// <summary>
        /// 按期补回
        /// </summary>
        [EnumItemDescription("按期补回")]
        Accomplish = 6,
        
        /// <summary>
        /// 提前补回
        /// </summary>
        [EnumItemDescription("提前补回")]
        AccomplishInadvance = 7
    }

    /// <summary>
    /// 补回状态->子情况
    /// </summary>
    public enum EnumReturnType_Sub
    {
        /// <summary>
        /// 无
        /// </summary>
        [EnumItemDescription("T1")]
        Sub_Nothing = 1,

        /// <summary>
        /// 承诺提前至X月份补回
        /// </summary>
        [EnumItemDescription("T2")]
        Sub_Returning = 2,

        /// <summary>
        /// 预计无法按期补回，承诺X月份补回
        /// </summary>
        [EnumItemDescription("T3")]
        Sub_UnableReturnByMonth = 3,

        /// <summary>
        /// 预计年内无法补回
        /// </summary>
        [EnumItemDescription("T4")]
        Sub_UnableReturnByYear = 4,

        /// <summary>
        /// X月X日已补回
        /// </summary>
        [EnumItemDescription("T5")]
        Sub_Accomplish = 5,

        /// <summary>
        /// 其它情况
        /// </summary>
        [EnumItemDescription("T6")]
        Sub_Other = 6
    }




    /// <summary>
    /// 指标类型：收入、支出、其他
    /// </summary>
    public enum EnumTargetType
    {
        [EnumItemDescription("收入类")]
        Revenue = 1,

        [EnumItemDescription("净利润类")]
        Profit = 2,

        [EnumItemDescription("成本类")]
        Cost = 3,

        [EnumItemDescription("其他")]
        Other = 4,
    }

    /// <summary>
    /// 类型：超支、节约、减亏、增亏
    /// </summary>
    public enum EnumType
    {
        [EnumItemDescription("超支")]
        Overspend = 1,

        [EnumItemDescription("节约")]
        Save = 2,

        [EnumItemDescription("减亏")]
        Reduced = 3,

        [EnumItemDescription("增亏")]
        IncreaseLoss = 4,
    }
    /// <summary>
    /// 百货系统经营指标完成门店数量情况中的项目类别
    /// </summary>
    public enum DSProjectType
    {
        [EnumItemDescription("两指标均完成")]
        NoCompleted = 0,

        [EnumItemDescription("一指标完成")]
        OneCompleted = 1,

        [EnumItemDescription("两指标均未完成")]
        TwoCompleted = 2
    }
    /// <summary>
    /// 经营指标完成情况对比中的项目名称
    /// </summary>
    public enum DSDetailAreaName
    {
        [EnumItemDescription("北区")]
        NorthArea = 1,

        [EnumItemDescription("中区")]
        CenterArea = 2,

        [EnumItemDescription("南区")]
        SouthArea = 3,

        [EnumItemDescription("合计")]
        TotalArea = 4
    }
    /// <summary>
    /// 用数字的序号换成中文的序号
    /// </summary>
    public enum ZHCNOrder
    {
        [EnumItemDescription("一")]
        One = 1,

        [EnumItemDescription("二")]
        Two = 2,

        [EnumItemDescription("三")]
        Three = 3,

        [EnumItemDescription("四")]
        Four = 4
    }

    /// <summary>
    /// 用户操作类型
    /// </summary>
    public enum MonthlyReportLogActionType
    {
        [EnumItemDescription("保存")]
        Save=1,

        [EnumItemDescription("提交")]
        Submit = 2,

        [EnumItemDescription("退回")]
        Return=3,

        [EnumItemDescription("撤回")]
        Withdraw = 4,

        [EnumItemDescription("作废")]
        Cancel = 5,


    }

    /// <summary>
    /// 异常指标类型
    /// </summary>
    public enum ExceptionTargetType
    {
        //上报不考核  
        [EnumItemDescription("上报不考核")]
        HaveDeTailNONeedEvaluation = 1,
        //不上报不考核
        [EnumItemDescription("不上报不考核")]
        HaveDetailNONeedReport = 2,
        //非明细指标不上报
        [EnumItemDescription("不上报")]
        HavenotDetailNONeedEvaluation = 3,

        [EnumItemDescription("全年不可比门店")]
        NotCompareCompany = 5,
    }
}
