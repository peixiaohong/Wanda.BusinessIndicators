using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Engine
{
    public interface IBizContext
    {
        Hashtable GetBizContext(object Vmodel);
    }


    /// <summary>
    /// 获取各个系统
    /// </summary>
    public interface ISystem
    {
        object GetSystem(object Vmodel, Guid _SystemID, List<Guid> TargetIDs);
    }


    /// <summary>
    /// 计算各个系统的指标数量
    /// </summary>
    public interface ISystemTargetCount
    {
        object GetSystemTargetCount(List<MonthlyReportDetail> MissTargetList, List<MonthlyReportDetail> LastMissTargetList, List<MonthlyReportDetail> SingleMissTargetList, List<MonthlyReportDetail> DoubleMissTargetList, List<MonthlyReportDetail> FilterMissTargetList);
    }



    /// <summary>
    /// 各个系统下载Excel的引擎 (暂时没用)
    /// </summary>
    public interface ISystemDownExcel
    {
        object GetSystemSystemDownExcel(List<MonthlyReportDetail> ReportDetail, List<Guid> Companys);
    }



    /// <summary>
    /// 通过公司属性，判断该指标是否异常
    /// </summary>
    public interface ICompanyExceptionTarget
    {
        int GetCompanyExceptionTarget(C_Company company );
    }




}
