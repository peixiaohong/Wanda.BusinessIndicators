using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 明细项接口
    /// </summary>
    public interface IReportInstanceDetail
    {
        /// <summary>
        /// 获取明细指标数据
        /// </summary>
        /// <param name="RptModel">ReportInstance:通用类型数据</param>
        /// <param name="strCompanyProperty">公司属性：在查询经营系统中用到，项目系统中暂时没用到, （这里占用该字段来标注，引擎的调用方式：当strCompanyProperty=“Reported”时，表示是在指标上报页面调用的，反之则是在查询的时候调用）</param>
        /// <param name="strOrderType">排序字段：在经营系统中用到，在项目系统中被当作IsPlan字段来用</param>
        /// <param name="IncludeHaveDetail">是否包含明细：在经营系统中用到，在项目系统中被当作IsLatestVersion用</param>
        /// <returns></returns>
        List<DictionaryVmodel> GetDetailRptDataSource(ReportInstance RptModel,string strCompanyProperty, string strOrderType, bool IncludeHaveDetail);
    }

    /// <summary>
    /// 月报说明接口
    /// </summary>
    public interface IReportInstanceSummary
    {
        List<DictionaryVmodel> GetSummaryRptDataSource(ReportInstance RptModel,  bool IsTargetPlan);
    }

    /// <summary>
    /// 累计未完成接口
    /// </summary>
    public interface IReportInstanceMissTarget
    {
        List<DictionaryVmodel> GetMissTargetRptDataSource(ReportInstance RptModel, C_System sys);
    }

    /// <summary>
    /// 当月完成接口
    /// </summary>
    public interface IReportInstanceCurrentMissTarget
    {
        List<DictionaryVmodel> GetCurrentMissTargetRptDataSource(ReportInstance RptModel, C_System sys);
    }


    /// <summary>
    /// 补回情况接口
    /// </summary>
    public interface IReportInstanceReturn
    {
        List<DictionaryVmodel> GetReturnRptDataSource(ReportInstance RptModel, C_System sys);
    }

    /// <summary>
    /// 补回情况接口
    /// </summary>
    public interface ITargetPlanInstance
    {
        List<DictionaryVmodel> GetTargetPlanSource(Guid SystemID, int FinYear, Guid TargetPlanID, bool IsLatestVersion);
    }
}
