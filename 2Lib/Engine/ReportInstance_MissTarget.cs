using Lib.Core;
using Lib.Expression;
using Lib.Web;
using Lib.Web.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    public partial class ReportInstance
    {
        //#region 经营系统代码


        //List<MonthlyReportDetail> MissTargetList = new List<MonthlyReportDetail>();

        //List<MonthlyReportDetail> LastMissTargetList = new List<MonthlyReportDetail>();

        //List<MonthlyReportDetail> MissTargetTextList = new List<MonthlyReportDetail>();

        //[Obsolete("此方法已经更新为新的方法，ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource", false)]
        //public List<DictionaryVmodel> GetMissTargetRptDataSource(bool IsLatestVersion = false)
        //{
        //    List<VGroup> GroupList = new List<VGroup>();

        //    XElement element = null;

        //    //加载XML，系统配置信息
        //    element = StaticResource.Instance[_System.ID].Configuration;

        //    if (element.Elements("Misstarget").Elements("Group") != null)
        //    {
        //        List<XElement> Groups = element.Elements("Misstarget").Elements("Group").ToList();
        //        foreach (XElement group in Groups)
        //        {
        //            GroupList.Add(new VGroup(group));
        //        }
        //    }

        //    //当月未完成A或B表指标
        //    if (ReportDetails != null && ReportDetails.Count >= 0)
        //    {
        //        MissTargetList = ReportDetails; //当月的数据

        //        int lastFinMonth = 0;

        //        if (FinMonth == 1)
        //        {
        //            lastFinMonth = 1;
        //        }
        //        else
        //        {
        //            lastFinMonth = FinMonth - 1;
        //        }


        //        List<A_MonthlyReportDetail> AList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(_System.ID, FinYear, lastFinMonth).ToList();

        //        List<B_MonthlyReportDetail> BList = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(_System.ID, FinYear, lastFinMonth, Guid.Empty).ToList();


        //        //上月的数据
        //        LastMissTargetList = new List<MonthlyReportDetail>();

        //        //上月 的数据
        //        if (IsLatestVersion)
        //        {
        //            BList.ForEach(P => LastMissTargetList.Add(P.ToVModel()));
        //        }
        //        else
        //        {
        //            AList.ForEach(P => LastMissTargetList.Add(P.ToVModel()));
        //        }


        //        foreach (MonthlyReportDetail item in MissTargetList)
        //        {
        //            //判断上个月是否有数据
        //            if (LastMissTargetList.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID) != null)
        //            {
        //                item.LastNAccumulativeActualAmmount = LastMissTargetList.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID).NAccumulativeActualAmmount;
        //                item.LastNAccumulativeDifference = LastMissTargetList.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID).NAccumulativeDifference;
        //                item.LastNAccumulativePlanAmmount = LastMissTargetList.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID).NAccumulativePlanAmmount;
        //                item.AddDifference = item.NAccumulativeDifference - item.LastNAccumulativeDifference;
        //            }
        //            else
        //            {
        //                item.AddDifference = item.NAccumulativeDifference - 0;
        //            }
        //            MissTargetTextList.Add(item);
        //        }

        //    }

        //    Dictionary<string, object> Alldata = new Dictionary<string, object>();

        //    List<DictionaryVmodel> ReturnList = new List<DictionaryVmodel>();

        //    //补回情况，分组List
        //    foreach (var Group in GroupList.OrderBy(G => G.Senquence))
        //    {
        //        DictionaryVmodel Vmodel = new DictionaryVmodel();
        //        Vmodel.Name = Group.TargetName;
        //        Vmodel.ObjValue = FormartVData(Group, "MissTarget", Group.TargetName);
        //        Vmodel.Mark = "Group";
        //        Vmodel.GuoupID = Group.TargetName;
        //        Vmodel.TargetGroupCount = Group.TargetIDs.Count();
        //        Vmodel.SystemName = _System.SystemName;
        //        ReturnList.Add(Vmodel);
        //    }
        //    return ReturnList;
        //}

        //private List<DictionaryVmodel> GetMissTargetRptDataByMonthRptID(Guid MonthRptID)
        //{
        //    List<VGroup> GroupList = new List<VGroup>();
        //    XElement element = null;

        //    //加载XML，系统配置信息
        //    element = StaticResource.Instance[_System.ID].Configuration;
        //    if (element.Elements("Misstarget").Elements("Group") != null)
        //    {
        //        List<XElement> Groups = element.Elements("Misstarget").Elements("Group").ToList();
        //        foreach (XElement group in Groups)
        //        {
        //            GroupList.Add(new VGroup(group));
        //        }
        //    }
        //    //根据MonthlyReportID获取ReportDetails中的数据
        //    if (ReportDetails != null && ReportDetails.Count >= 0)
        //    {
        //        MissTargetList = ReportDetails.Where(p => p.MonthlyReportID == MonthRptID).ToList();
        //    }
        //    Dictionary<string, object> Alldata = new Dictionary<string, object>();
        //    List<DictionaryVmodel> ReturnList = new List<DictionaryVmodel>();


        //    //补回情况，分组List
        //    foreach (var Group in GroupList.OrderBy(G => G.Senquence))
        //    {
        //        DictionaryVmodel Vmodel = new DictionaryVmodel();
        //        Vmodel.Name = Group.TargetName;
        //        Vmodel.ObjValue = FormartVData(Group, "MissTarget", Group.TargetName);
        //        Vmodel.Mark = "Group";
        //        Vmodel.GuoupID = Group.TargetName;
        //        Vmodel.TargetGroupCount = Group.TargetIDs.Count();
        //        Vmodel.SystemName = _System.SystemName;
        //        ReturnList.Add(Vmodel);
        //    }
        //    return ReturnList;
        //}

        ///// <summary>
        ///// 未完成说明
        ///// </summary>
        ///// <param name="counter"></param>
        ///// <param name="TargetIDs"></param>
        ///// <returns></returns>
        //private List<MonthlyReportDetail> CalclateMissTargetdata(VCounter counter, List<Guid> TargetIDs)
        //{

        //    List<Guid> Companys = new List<Guid>();

        //    List<MonthlyReportDetail> Result = new List<MonthlyReportDetail>();
        //    List<MonthlyReportDetail> Filter = new List<MonthlyReportDetail>();
        //    foreach (MonthlyReportDetail d in MissTargetList)
        //    {
        //        if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
        //        {
        //            d.TargetGroupCount = TargetIDs.Count();
        //            Filter.Add(d); //添加

        //            Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(MissTargetList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
        //            ExpressionParser _parser = new ExpressionParser(bizContext);

        //            //解析这个公式
        //            //一个公司多个指标数据属于同一个Group
        //            //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
        //            if (_parser.CacluateCondition(counter.Expression))
        //            {
        //                if (!Companys.Contains(d.CompanyID))
        //                {
        //                    Companys.Add(d.CompanyID);
        //                }
        //            }
        //        }
        //    }

        //    foreach (Guid CompanyID in Companys)
        //    {
        //        //判断当前公司的数量
        //        if (Filter.Where(r => r.CompanyID == CompanyID).Count() == TargetIDs.Count())
        //        {
        //            //将筛选出来的值，添加到结果后，马上移除
        //            Result.AddRange(Filter.FindAll(R => R.CompanyID == CompanyID));
        //        }
        //        else
        //        {
        //            //xml定义的指标数比数据库里的指标多
        //            int rowCount = TargetIDs.Count() - Filter.Where(r => r.CompanyID == CompanyID).Count();

        //            MonthlyReportDetail model = Filter.Where(p => p.CompanyID == CompanyID).FirstOrDefault();

        //            List<MonthlyReportDetail> NewDetail = new List<MonthlyReportDetail>();

        //            //根据相差指标数目，补齐这些数据
        //            for (int i = 0; i < rowCount; i++)
        //            {
        //                NewDetail.Add(new MonthlyReportDetail()
        //                {
        //                    GroupTile = model.GroupTile,
        //                    TargetGroupCount = model.TargetGroupCount,
        //                    Title = model.Title,
        //                    CompanyName = model.CompanyName,
        //                    CompanyID = model.CompanyID,
        //                    TargetID = model.TargetID,
        //                    ReturnType = model.ReturnType,
        //                    Counter = 0,
        //                    NAccumulativeActualAmmount = 0,
        //                    NAccumulativeDifference = 0,
        //                    AddDifference = 0,
        //                    NAccumulativePlanAmmount = 0,
        //                    LastNAccumulativeActualAmmount = 0,
        //                    LastNAccumulativeDifference = 0,
        //                    LastNAccumulativePlanAmmount = 0,
        //                    TargetName = "--",
        //                    SystemID = model.SystemID,
        //                    ReturnDescription = "",
        //                    MIssTargetDescription = "",
        //                    MIssTargetReason = ""
        //                });
        //            }

        //            Result.AddRange(Filter.FindAll(R => R.CompanyID == CompanyID));
        //            Result.AddRange(NewDetail);
        //        }

        //        //从Group的总数据中移除已经分类的数据
        //        MissTargetList.RemoveAll(Re => Result.Exists(R => R.ID == Re.ID));
        //    }


        //    List<MonthlyReportDetail> ResultTempSum = new List<MonthlyReportDetail>();

        //    //SystemEngine 对商管做了特殊处理，其它系统直接调用排序的Engine
        //    object NewModel = SystemEngine.SystemEngineService.GetSystem(Result, _System.ID, TargetIDs);
        //    ResultTempSum = (List<MonthlyReportDetail>)NewModel;


        //    //未完成说明的合并
        //    for (int i = TargetIDs.Count; i <= ResultTempSum.Count; i = i + TargetIDs.Count)
        //    {
        //        if (TargetIDs.Count > 1)
        //        {
        //            string StrDescription = ResultTempSum[i - TargetIDs.Count].MIssTargetDescription;

        //            string SrtReason = ResultTempSum[i - TargetIDs.Count].MIssTargetReason;

        //            for (int j = 1; j < TargetIDs.Count; j++)
        //            {
        //                StrDescription = StrDescription + ResultTempSum[i - j].MIssTargetDescription; ;

        //                SrtReason = SrtReason + ResultTempSum[i - j].MIssTargetReason;
        //            }

        //            ResultTempSum[i - TargetIDs.Count].MIssTargetDescription = StrDescription;

        //            ResultTempSum[i - TargetIDs.Count].MIssTargetReason = SrtReason;
        //        }
        //    }

        //    return ResultTempSum;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="StrValue"></param>
        ///// <param name="TargetIDs"></param>
        ///// <param name="CounterExpression">主要是为了计算上个月的计算公式</param>
        ///// <returns></returns>
        //private string MonthDescriptionTools(string StrValue, List<Guid> TargetIDs, string CounterExpression, List<MonthlyReportDetail> MRDList)
        //{
        //    //双指标:上月未完成+补回
        //    List<MonthlyReportDetail> DoubleResult = MRDList.Select(x => x).ToList();

        //    //单个指标：上月未完成+补回
        //    List<MonthlyReportDetail> SingleResult = MRDList.Select(c => c).ToList();

        //    //上月的补回公司
        //    List<MonthlyReportDetail> LastList = MissTargetList.Where(p => p.ReturnType >= (int)EnumReturnType.Accomplish).ToList();

        //    if (LastList.Count > 0)
        //    {
        //        //循环上月的补回公司
        //        foreach (var itemLast in LastList)
        //        {
        //            if (MissTargetTextList.FindAll(z => z.CompanyID == itemLast.CompanyID && z.ReturnType >=(int)EnumReturnType.Accomplish).Count > 1)
        //            {
        //                //双指标
        //                //本月的数据
        //                if (DoubleResult.FindAll(p => p.CompanyID == itemLast.CompanyID).Count == 0)
        //                {
        //                    DoubleResult.Add(itemLast);
        //                }
        //            }
        //            else if (MissTargetTextList.FindAll(z => z.CompanyID == itemLast.CompanyID && z.ReturnType >= (int)EnumReturnType.Accomplish).Count == 1)
        //            {
        //                if (SingleResult.FindAll(p => p.CompanyID == itemLast.CompanyID).Count == 0)
        //                {
        //                    SingleResult.Add(itemLast);
        //                }
        //            }
        //        }
        //    }

        //    //提供不同的数据源
        //    MissTargetDataSource DataSource = (MissTargetDataSource)SystemTargetCountEngine.SystemTargetCountEngineService.GetSystemTargetCount(MissTargetTextList, LastMissTargetList, SingleResult, DoubleResult, MRDList, _System.ID);

        //    DoubleResult = DataSource.MissTargetDataSource3;

        //    SingleResult = DataSource.MissTargetDataSource4;

        //    MRDList = DataSource.MissTargetDataSource5;
            

        //    if (!string.IsNullOrEmpty(StrValue))
        //    {
        //        //正则表达式，定义要替换的部分
        //        var matchResults = System.Text.RegularExpressions.Regex.Match(StrValue, @".*?{(.*?)}");
        //        Hashtable ht = new Hashtable();
        //        while (matchResults.Success)
        //        {
        //            //var num1 = matchResults.Groups[0].Value; // 第一个数字
        //            string _Expression = matchResults.Groups[1].Value; // 第二个数字
        //            string _Month = _Expression.Substring(0, 2);

        //            string Expression = _Expression.Replace(_Month, "").TrimStart().ToString();

        //            //公司数量
        //            int CompanyCount = 0;

        //            if (_Month == "上双")
        //            {
        //                //Expression 计算有多少个公司
        //                CompanyCount = GetLastMonthCompanyCount(Expression, TargetIDs, DoubleResult);

        //            }else if (_Month == "上单")
        //            {
        //                //Expression 计算有多少个公司
        //                CompanyCount = GetLastMonthCompanyCount(Expression, TargetIDs, SingleResult);
        //            }
        //            else //本月
        //            {
        //                //Expression 计算有多少个公司
        //                CompanyCount = GetCompanyCount(Expression, TargetIDs, MRDList);
        //            }
        //            //存入Hashtable中

        //            if (ht.ContainsKey(_Expression) == false)
        //                ht.Add(_Expression, CompanyCount);
        //            else
        //                ht[_Expression] = CompanyCount;

        //            matchResults = matchResults.NextMatch();
        //        }

        //        //循环Hashtable，替换公式，显示返回公司的数量
        //        foreach (string key in ht.Keys)
        //        {
        //            StrValue = StrValue.Replace("{" + key + "}", ht[key].ToString());
        //        }
        //    }
        //    return StrValue;
        //}

        ///// <summary>
        ///// 根据未完成公式，获取当月未完成公司的数量
        ///// </summary>
        ///// <param name="Expression">未完成公式</param>
        ///// <param name="TargetIDs">未完成指标ID组合</param>
        ///// <returns></returns>
        //private int GetCompanyCount(string Expression, List<Guid> TargetIDs, List<MonthlyReportDetail> _CurrMRDList)
        //{
        //    List<Guid> Companys = new List<Guid>();
        //    //计算本月的
        //    foreach (MonthlyReportDetail d in _CurrMRDList)
        //    {
        //        if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
        //        {
        //            Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(_CurrMRDList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
        //            ExpressionParser _parser = new ExpressionParser(bizContext);

        //            //解析这个公式
        //            //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
        //            if (_parser.CacluateCondition(Expression))
        //            {
        //                if (!Companys.Contains(d.CompanyID))
        //                {
        //                    Companys.Add(d.CompanyID);
        //                }
        //            }
        //        }
        //    }

        //    return Companys.Count;
        //}

        ///// <summary>
        ///// 获取上月的未完成公司
        ///// </summary>
        ///// <param name="Expression">未完成公式</param>
        ///// <param name="TargetIDs">未完成指标ID组合</param>
        ///// <returns></returns>
        //private int GetLastMonthCompanyCount(string Expression, List<Guid> TargetIDs, List<MonthlyReportDetail> _LastMissTargetList)
        //{
        //    List<Guid> Companys = new List<Guid>();

        //    //计算上月的
        //    foreach (MonthlyReportDetail d in _LastMissTargetList)
        //    {
        //        if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
        //        {
        //            Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(_LastMissTargetList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
        //            ExpressionParser _parser = new ExpressionParser(bizContext);

        //            //解析这个公式
        //            //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
        //            if (_parser.CacluateCondition(Expression))
        //            {
        //                if (!Companys.Contains(d.CompanyID))
        //                {
        //                    Companys.Add(d.CompanyID);
        //                }
        //            }
        //        }
        //    }

        //    //调用引擎
        //    //SystemTargetCountEngine.SystemTargetCountEngineService.GetSystemTargetCount(ref LastMissTargetList, _System.ID, Companys);

        //    return Companys.Count;

        //}



        ///// <summary>
        ///// 筛选出上个月的公司数量
        ///// </summary>
        ///// <param name="Expression">上月未完成公司公式</param>
        ///// <param name="TargetIDs">指标组合</param>
        ///// <returns>筛选出来的</returns>
        ////private List<MonthlyReportDetail> GetLastMonthCompanyList(string Expression, List<Guid> TargetIDs)
        ////{
        ////    List<Guid> Companys = new List<Guid>();
        ////    List<MonthlyReportDetail> Filter = new List<MonthlyReportDetail>();

        ////    //计算上月的
        ////    foreach (MonthlyReportDetail d in LastMissTargetList)
        ////    {
        ////        if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
        ////        {
        ////            Filter.Add(d); //添加

        ////            Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(LastMissTargetList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
        ////            ExpressionParser _parser = new ExpressionParser(bizContext);

        ////            //解析这个公式
        ////            //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
        ////            if (_parser.CacluateCondition(Expression))
        ////            {
        ////                if (!Companys.Contains(d.CompanyID))
        ////                {
        ////                    Companys.Add(d.CompanyID);
        ////                }
        ////            }
        ////        }
        ////    }

        ////    //调用引擎 ,筛选出适合公式的数据
        ////    object ResultObject = SystemTargetCountEngine.SystemTargetCountEngineService.GetSystemTargetCount(ref LastMissTargetList, ref MissTargetList, Filter, _System.ID, Companys);

        ////    //筛选出来的数据
        ////    List<MonthlyReportDetail> Result = (List<MonthlyReportDetail>)ResultObject;

        ////    return Result;
        ////}



        //#endregion


     



    }
}
