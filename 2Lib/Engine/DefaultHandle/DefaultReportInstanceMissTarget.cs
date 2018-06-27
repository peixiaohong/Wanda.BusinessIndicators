using Lib.Expression;
using Lib.Web.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;
using Lib.Config;

namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 更具获取系统，填充某个系统的补充项
    /// </summary>
    public class DefaultReportInstanceMissTarget : IReportInstanceMissTarget
    {
        /// <summary>
        /// 当月未完成列表
        /// </summary>
        List<MonthlyReportDetail> MissTargetList = null;
        /// <summary>
        /// 上个月未完成列表
        /// </summary>
        List<MonthlyReportDetail> LastMissTargetList = null;
        /// <summary>
        /// 
        /// </summary>
        List<MonthlyReportDetail> MissTargetTextList = null;

        List<MonthlyReportDetail> ReportDetails = null;
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        List<C_Target> targetList = null;
        bool IsSingleTarget = false; // Update 2015-5-13  是否单个指标

        public List<DictionaryVmodel> GetMissTargetRptDataSource(ReportInstance RptModel, C_System sys)
        {
            #region 初始化
            MissTargetList = new List<MonthlyReportDetail>();
            MissTargetTextList = new List<MonthlyReportDetail>();
            ReportDetails = new List<MonthlyReportDetail>();
            targetList = new List<C_Target>();

            #endregion

            _System = sys;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            targetList = RptModel._Target;


            List<VGroup> GroupList = new List<VGroup>();
            XElement element = null;

            //加载XML，系统配置信息
            element = _System.Configuration; //局部

            if (element.Elements("Misstarget").Elements("Group") != null)
            {
                List<XElement> Groups = element.Elements("Misstarget").Elements("Group").ToList();
                foreach (XElement group in Groups)
                {
                    GroupList.Add(new VGroup(group));
                }
            }

            //-------Update 2015 -5-13 区分未完成的时候是单个指标，还是组合指标 start
            if (element.Elements("Misstarget").ToList().Count > 0)
            {
                XElement subElement = element.Elements("Misstarget").ToList()[0];

                IsSingleTarget = subElement.GetAttributeValue("IsSingleTarget", false);
            }
            //-------Update 2015 -5-13 区分未完成的时候是单个指标，还是组合指标  End


            ReportDetails = RptModel.ReportDetails;

            //当月未完成A或B表指标
            if (ReportDetails != null && ReportDetails.Count > 0)
            {
                MissTargetList = ReportDetails; //当月的数据

                int lastFinMonth = 0;

                if (FinMonth == 1)
                    lastFinMonth = 0;
                else
                    lastFinMonth = FinMonth - 1;

                //找到上个月的数据 , 上个月的数据只能在A表中
                //List<A_MonthlyReportDetail> AList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(RptModel._System.ID, FinYear, lastFinMonth).ToList();
                List<A_MonthlyReportDetail> AList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyReportDetailListForTargetPlanID(RptModel._System.ID, FinYear, lastFinMonth, RptModel.TargetPlanID).ToList();


                // 获取各个公司全年的总指标数据
                List<V_PlanTargetModel> VPlanTargeList = A_TargetplandetailOperator.Instance.GetAnnualPlanTarget(ReportDetails[0].TargetPlanID, FinYear);

                //上月的数据
                LastMissTargetList = new List<MonthlyReportDetail>();
                AList.ForEach(P => LastMissTargetList.Add(P.ToVModel()));

                foreach (MonthlyReportDetail item in MissTargetList)
                {

                    MonthlyReportDetail LastTempDetai = LastMissTargetList.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID);

                    V_PlanTargetModel PTM = VPlanTargeList.Find(Tp => Tp.CompanyID == item.CompanyID && Tp.TargetID == item.TargetID);

                    //将全年指标总数加进去
                    if (PTM != null)
                        item.AnnualTargetPlanValue = PTM.Target;

                    //判断上个月是否有数据
                    if (LastTempDetai != null)
                    {
                        item.LastNAccumulativeActualAmmount = LastTempDetai.NAccumulativeActualAmmount;
                        item.LastNAccumulativeDifference = LastTempDetai.NAccumulativeDifference;
                        item.LastNAccumulativePlanAmmount = LastTempDetai.NAccumulativePlanAmmount;
                        item.AddDifference = item.NAccumulativeDifference - item.LastNAccumulativeDifference;
                        item.LastIsMissTarget = LastTempDetai.IsMissTarget;
                        item.LastIsCommitDate = LastTempDetai.IsCommitDate;
                    }
                    else
                    {
                        item.AddDifference = item.NAccumulativeDifference - 0;
                    }

                    MissTargetTextList.Add(item);
                }
            }

            Dictionary<string, object> Alldata = new Dictionary<string, object>();

            List<DictionaryVmodel> ReturnList = new List<DictionaryVmodel>();

            //补回情况，分组List
            foreach (var Group in GroupList.OrderBy(G => G.Senquence))
            {
                DictionaryVmodel Vmodel = new DictionaryVmodel();
                Vmodel.Name = Group.TargetName;
                Vmodel.ObjValue = FormartVData(Group, "MissTarget", Group.TargetName);
                Vmodel.Mark = "Group";
                Vmodel.GuoupID = Group.TargetName;
                Vmodel.TargetGroupCount = Group.TargetIDs.Count();
                Vmodel.SystemName = _System.SystemName;
                ReturnList.Add(Vmodel);
            }
            return ReturnList;
        }

        List<MonthlyReportDetail> VLastList = new List<MonthlyReportDetail>();

        /// <summary>
        /// 分组数据
        /// </summary>
        /// <param name="Group">分组条件</param>
        /// <param name="XmlType">补回情况/未完成说明</param>
        /// <returns></returns>
        List<DictionaryVmodel> FormartVData(VGroup Group, string XmlType, string GroupStr)
        {
            List<DictionaryVmodel> result = new List<DictionaryVmodel>();

            //循环Group下的数据
            foreach (var counter in Group.CounterList.OrderBy(C => C.Senquence))
            {
                if (counter.SubCounterList != null && counter.SubCounterList.Count > 0)
                {
                    DictionaryVmodel dictC = new DictionaryVmodel(counter.Title, FormartCounterData(counter, Group, XmlType, GroupStr, ref VLastList));
                    dictC.Mark = "Counter";
                    dictC.GuoupID = GroupStr;
                    dictC.TargetGroupCount = Group.TargetIDs.Count();
                    dictC.SystemName = _System.SystemName;
                    result.Add(dictC);
                }
                else
                {
                    result.AddRange(FormartCounterData(counter, Group, XmlType, GroupStr, ref VLastList));
                }
            }
            return result;
        }


        /// <summary>
        /// 分组数据
        /// </summary>
        /// <param name="Group">分组条件</param>
        /// <param name="XmlType">补回情况/未完成说明</param>
        /// <returns></returns>
        List<DictionaryVmodel> FormartCounterData(VCounter counter, VGroup Group, string XmlType, string GroupStr, ref List<MonthlyReportDetail> Source)
        {
            List<DictionaryVmodel> result = new List<DictionaryVmodel>();

            List<MonthlyReportDetail> tempList = CalclateMissTargetdata(counter, Group.TargetIDs);

            string TitleName = MonthDescriptionTools(counter.Title, Group.TargetIDs, counter.Expression, tempList);

            DictionaryVmodel dictM = new DictionaryVmodel(TitleName, tempList);

            dictM.Mark = "Data";
            dictM.GuoupID = GroupStr;
            dictM.TargetGroupCount = Group.TargetIDs.Count();
            dictM.SystemName = _System.SystemName;
            dictM.Value = counter.Senquence.ToString();
            result.Add(dictM);

            return result;
        }


        /// <summary>
        /// 未完成说明
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="TargetIDs"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> CalclateMissTargetdata(VCounter counter, List<Guid> TargetIDs)
        {

            // List<Guid> Companys = new List<Guid>();

            Hashtable CompanyHt = new Hashtable();

            List<MonthlyReportDetail> Result = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> Filter = new List<MonthlyReportDetail>();
            foreach (MonthlyReportDetail d in MissTargetList)
            {
                if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
                {
                    d.TargetGroupCount = TargetIDs.Count();
                    Filter.Add(d); //添加

                    Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(MissTargetList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
                    ExpressionParser _parser = new ExpressionParser(bizContext);

                    //解析这个公式
                    //一个公司多个指标数据属于同一个Group
                    //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
                    if (_parser.CacluateCondition(counter.Expression))
                    {
                        //if (!Companys.Contains(d.CompanyID))
                        //{
                        //    Companys.Add(d.CompanyID);
                        //}

                        if (!CompanyHt.ContainsKey(d.CompanyID))
                        {
                            CompanyHt.Add(d.CompanyID, counter.TextExpression);
                        }

                    }
                }
            }


            foreach (Guid CompanyID in CompanyHt.Keys)
            {
                //判断当前公司的数量
                if (Filter.Where(r => r.CompanyID == CompanyID).Count() == TargetIDs.Count())
                {
                    //将筛选出来的值，添加到结果后，马上移除
                    Result.AddRange(Filter.FindAll(R => R.CompanyID == CompanyID));

                    if (CompanyHt[CompanyID].ToString() == "Return") // 针对的是补回项的数据，如果是补回数据，则未完成原因和采取措施清空 by dubiao 2015/4/15 
                    {
                        foreach (var item in Result)
                        {
                            item.MIssTargetReason = "";
                            item.MIssTargetDescription = "";
                        }
                    }
                }
                else
                {
                    //xml定义的指标数比数据库里的指标多
                    int rowCount = TargetIDs.Count() - Filter.Where(r => r.CompanyID == CompanyID).Count();

                    MonthlyReportDetail model = Filter.Where(p => p.CompanyID == CompanyID).FirstOrDefault();

                    List<MonthlyReportDetail> NewDetail = new List<MonthlyReportDetail>();

                    //根据相差指标数目，补齐这些数据
                    for (int i = 0; i < rowCount; i++)
                    {
                        NewDetail.Add(new MonthlyReportDetail()
                        {
                            GroupTile = model.GroupTile,
                            TargetGroupCount = model.TargetGroupCount,
                            Title = model.Title,
                            CompanyName = model.CompanyName,
                            CompanyID = model.CompanyID,
                            TargetID = Guid.Empty,
                            ReturnType = 0,
                            Counter = 0,
                            NAccumulativeActualAmmount = 0,
                            NAccumulativeDifference = 0,
                            AddDifference = 0,
                            NAccumulativePlanAmmount = 0,
                            LastNAccumulativeActualAmmount = 0,
                            LastNAccumulativeDifference = 0,
                            LastNAccumulativePlanAmmount = 0,
                            TargetName = "--",
                            SystemID = model.SystemID,
                            ReturnDescription = "",
                            MIssTargetDescription = "",
                            MIssTargetReason = ""
                        });
                    }

                    Result.AddRange(Filter.FindAll(R => R.CompanyID == CompanyID));
                    Result.AddRange(NewDetail);
                }

                //从Group的总数据中移除已经分类的数据
                MissTargetList.RemoveAll(Re => Result.Exists(R => R.ID == Re.ID));
            }


            List<MonthlyReportDetail> ResultTempSum = new List<MonthlyReportDetail>();

            //SystemEngine 对商管做了特殊处理，其它系统直接调用排序的Engine
            object NewModel = SystemEngine.SystemEngineService.GetSystem(Result, _System.ID, TargetIDs);
            ResultTempSum = (List<MonthlyReportDetail>)NewModel;




            #region  这里只有项目系统需要单独处理，其它系统都是需要合并的
            //if (_System.GroupType != "ProSystem")
            //{
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
            //}
            # endregion

            return ResultTempSum;
        }


        #region 未完成说明文字


        /// <summary>
        /// 
        /// </summary>
        /// <param name="StrValue"></param>
        /// <param name="TargetIDs"></param>
        /// <param name="CounterExpression">主要是为了计算上个月的计算公式</param>
        /// <returns></returns>
        private string MonthDescriptionTools(string StrValue, List<Guid> TargetIDs, string CounterExpression, List<MonthlyReportDetail> MRDList)
        {
            //双指标:包含上月未完成+补回
            List<MonthlyReportDetail> DoubleResult = MRDList.Select(x => x).ToList();

            //单个指标：包含上月未完成+补回
            List<MonthlyReportDetail> SingleResult = MRDList.Select(c => c).ToList();

            //上月的补回公司

            List<MonthlyReportDetail> _LastList = MissTargetTextList.Where(p => p.ReturnType >= (int)EnumReturnType.Accomplish).ToList();
            List<Guid> CompanyIDs = new List<Guid>();

            _LastList.ForEach(M =>
            {
                if (TargetIDs.Contains(M.TargetID) && !CompanyIDs.Contains(M.CompanyID))
                {
                    CompanyIDs.Add(M.CompanyID);
                }
            });

            //未完成list 
            List<MonthlyReportDetail> Mlist = MissTargetTextList.Where(p => (p.ReturnType > 0 && p.ReturnType <= (int)EnumReturnType.Accomplish) && (TargetIDs.Contains(p.TargetID))).ToList();

            Mlist.ForEach(M =>
            {
                if (CompanyIDs.Contains(M.TargetID))
                {
                    CompanyIDs.Remove(M.TargetID);
                }
            });

            List<MonthlyReportDetail> LastList = new List<MonthlyReportDetail>();

            CompanyIDs.ForEach(C => { LastList.AddRange(MissTargetTextList.Where(M => M.CompanyID == C).ToList()); });

            //----------------------Update 2015 -5- 7  这里的筛选是为了配合组合指标的，单个指标不再这里做处理 ---------

            if (TargetIDs.Count > 1) //如果XMl里的指标是一个就是单个，其它的都是组合指标
                IsSingleTarget = false;
            else
                IsSingleTarget = true;

            if (!IsSingleTarget) //是否单个指标针对（商管，物管的），这里不用理会补回的公司
            {
                LastList.ForEach(p =>
                {
                    if (p.ReturnType >= (int)EnumReturnType.Accomplish)
                    {
                        //如果该数据是补回的，搜索该数据的公司是否有新增的未完成，如果有排除掉； 因该数据不是 补回数据
                        var TempReturnList = LastList.Where(f => f.CompanyID == p.CompanyID && f.IsMissTarget == true).ToList();
                        if (TempReturnList.Count > 0)
                        {
                            CompanyIDs.Remove(p.CompanyID);
                        }
                    }
                });
                //清空，然后重新再次加载
                LastList.RemoveRange(0, LastList.Count);
                CompanyIDs.ForEach(C => { LastList.AddRange(MissTargetTextList.Where(M => M.CompanyID == C).ToList()); });
            }
            //----------------------Update 2015 -5- 7   end ---------


            if (LastList.Count > 0)
            {
                //循环上月的补回公司
                foreach (var itemLast in LastList)
                {

                    int AccomplishCount = MissTargetTextList.FindAll(z => z.CompanyID == itemLast.CompanyID && z.ReturnType >= (int)EnumReturnType.Accomplish).Count;

                    //这个是组合指标都完成
                    if (AccomplishCount == TargetIDs.Count)
                    {
                        //双指标 ,//本月的数据 同时必须比对 指标
                        if (DoubleResult.FindAll(p => p.CompanyID == itemLast.CompanyID && p.TargetID == itemLast.TargetID).Count == 0)
                        {
                            DoubleResult.Add(itemLast);
                        }
                    }
                    else
                    {
                        if (SingleResult.FindAll(p => p.CompanyID == itemLast.CompanyID && p.TargetID == itemLast.TargetID).Count == 0)
                        {
                            SingleResult.Add(itemLast);
                        }
                    }
                }
            }


            //提供不同的数据源
            MissTargetDataSource DataSource = (MissTargetDataSource)SystemTargetCountEngine.SystemTargetCountEngineService.GetSystemTargetCount(MissTargetTextList, LastMissTargetList, SingleResult, DoubleResult, MRDList, _System);


            if (IsSingleTarget && _System.SystemName == "万达旅业")
            {
                DoubleResult = DataSource.MissTargetDataSource1;

                SingleResult = DataSource.MissTargetDataSource2;

                MRDList = DataSource.MissTargetDataSource1;
            }
            else
            {
                if (DataSource.MissTargetDataSource3 != null && DataSource.MissTargetDataSource3.Count > 0)
                    DoubleResult = DataSource.MissTargetDataSource3;
                else
                    DoubleResult = new List<MonthlyReportDetail>();


                if (DataSource.MissTargetDataSource4 != null && DataSource.MissTargetDataSource4.Count > 0)
                    SingleResult = DataSource.MissTargetDataSource4;
                else
                    SingleResult = new List<MonthlyReportDetail>();


                MRDList = DataSource.MissTargetDataSource5;
            }

            if (!string.IsNullOrEmpty(StrValue))
            {
                //正则表达式，定义要替换的部分
                var matchResults = System.Text.RegularExpressions.Regex.Match(StrValue, @".*?{(.*?)}");
                Hashtable ht = new Hashtable();
                while (matchResults.Success)
                {
                    //var num1 = matchResults.Groups[0].Value; // 第一个数字
                    string _Expression = matchResults.Groups[1].Value; // 第二个数字
                    string _Month = _Expression.Substring(0, 2);

                    string Expression = _Expression.Replace(_Month, "").TrimStart().ToString();

                    //公司数量
                    int CompanyCount = 0;

                    if (_Month == "上双")
                    {
                        //Expression 计算有多少个公司
                        CompanyCount = GetLastMonthCompanyCount(Expression, TargetIDs, DoubleResult);
                    }
                    else if (_Month == "上单")
                    {
                        //Expression 计算有多少个公司
                        CompanyCount = GetLastMonthCompanyCount(Expression, TargetIDs, SingleResult);
                    }
                    else //本月
                    {
                        //Expression 计算有多少个公司
                        CompanyCount = GetCompanyCount(Expression, TargetIDs, MRDList);
                    }
                    //存入Hashtable中
                    string red_color_CompanyCount = @"<span style='color: red;'>" + CompanyCount.ToString() + "</span>";

                    if (ht.ContainsKey(_Expression) == false)
                        ht.Add(_Expression, red_color_CompanyCount);
                    else
                        ht[_Expression] = red_color_CompanyCount;

                    matchResults = matchResults.NextMatch();
                }

                //循环Hashtable，替换公式，显示返回公司的数量
                foreach (string key in ht.Keys)
                {
                    StrValue = StrValue.Replace("{" + key + "}", ht[key].ToString());
                }
            }
            return StrValue;
        }


        /// <summary>
        /// 根据未完成公式，获取当月未完成公司的数量
        /// </summary>
        /// <param name="Expression">未完成公式</param>
        /// <param name="TargetIDs">未完成指标ID组合</param>
        /// <returns></returns>
        private int GetCompanyCount(string Expression, List<Guid> TargetIDs, List<MonthlyReportDetail> _CurrMRDList)
        {
            List<Guid> Companys = new List<Guid>();
            //计算本月的
            foreach (MonthlyReportDetail d in _CurrMRDList)
            {
                if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
                {
                    Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(_CurrMRDList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
                    ExpressionParser _parser = new ExpressionParser(bizContext);

                    //解析这个公式
                    //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
                    if (_parser.CacluateCondition(Expression))
                    {
                        if (!Companys.Contains(d.CompanyID))
                        {
                            Companys.Add(d.CompanyID);
                        }
                    }
                }
            }
            return Companys.Count;
        }

        /// <summary>
        /// 获取上月的未完成公司
        /// </summary>
        /// <param name="Expression">未完成公式</param>
        /// <param name="TargetIDs">未完成指标ID组合</param>
        /// <returns></returns>
        private int GetLastMonthCompanyCount(string Expression, List<Guid> TargetIDs, List<MonthlyReportDetail> _LastMissTargetList)
        {
            List<Guid> Companys = new List<Guid>();

            //计算上月的
            foreach (MonthlyReportDetail d in _LastMissTargetList)
            {
                if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
                {
                    Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(_LastMissTargetList.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
                    ExpressionParser _parser = new ExpressionParser(bizContext);

                    //解析这个公式
                    //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
                    if (_parser.CacluateCondition(Expression))
                    {
                        if (!Companys.Contains(d.CompanyID))
                        {
                            Companys.Add(d.CompanyID);
                        }
                    }
                }
            }

            //---Update 2015-5-13  这里只有单个指标不用理会公司（商管，物管)

            string MissTargetSystemName = AppSettingConfig.GetSetting("MissTargetSystemName", "");
            if (MissTargetSystemName.Contains(_System.SystemName) == false)
            {
                if (!IsSingleTarget)
                {
                    //将补回的数据从总的数据中移除掉
                    MissTargetTextList.RemoveAll(Re => Companys.Exists(R => R == Re.CompanyID));
                }
            }
            //--Update 2015-5-13  End


            return Companys.Count;
        }


        #endregion

    }

    /// <summary>
    /// 直属公司的未完成原因
    /// </summary>
    public class Directly_ReportInstanceMissTarget : IReportInstanceMissTarget
    {


        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        List<MonthlyReportDetail> VLastList = new List<MonthlyReportDetail>();
        public List<DictionaryVmodel> GetMissTargetRptDataSource(ReportInstance RptModel, C_System sys)
        {
            _System = sys;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            ReportDetails = RptModel.ReportDetails;

            List<MonthlyReportDetail> MissTargetList = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> LastMissTargetList = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> MissTargetTextList = new List<MonthlyReportDetail>();

            //加载XML
            List<VGroup> lstVGroup = GetDirectlyMissTargetXml(_System.Configuration);

            //当月未完成A或B表指标
            if (ReportDetails != null && ReportDetails.Count > 0)
            {
                MissTargetList = ReportDetails; //当月的数据
                int lastFinMonth = 0;
                if (FinMonth == 1)
                    lastFinMonth = 0;
                else
                    lastFinMonth = FinMonth - 1;

                //找到上个月的数据 , 上个月的数据只能在A表中
                //List<A_MonthlyReportDetail> AList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(RptModel._System.ID, FinYear, lastFinMonth).ToList();
                List<A_MonthlyReportDetail> AList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyReportDetailListForTargetPlanID(RptModel._System.ID, FinYear, lastFinMonth, RptModel.TargetPlanID).ToList();


                // 获取各个公司全年的总指标数据
                List<V_PlanTargetModel> VPlanTargeList = A_TargetplandetailOperator.Instance.GetAnnualPlanTarget(ReportDetails[0].TargetPlanID, FinYear);

                //上月的数据
                LastMissTargetList = new List<MonthlyReportDetail>();
                AList.ForEach(P => LastMissTargetList.Add(P.ToVModel()));

                foreach (MonthlyReportDetail item in MissTargetList)
                {

                    V_PlanTargetModel PTM = VPlanTargeList.Find(Tp => Tp.CompanyID == item.CompanyID && Tp.TargetID == item.TargetID);

                    //将全年指标总数加进去
                    if (PTM != null)
                        item.AnnualTargetPlanValue = PTM.Target;

                    //判断上个月是否有数据
                    MonthlyReportDetail LastMrd = LastMissTargetList.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID);
                    if (LastMrd != null)
                    {
                        item.LastNAccumulativeActualAmmount = LastMrd.NAccumulativeActualAmmount;
                        item.LastNAccumulativeDifference = LastMrd.NAccumulativeDifference;
                        item.LastNAccumulativePlanAmmount = LastMrd.NAccumulativePlanAmmount;
                        item.AddDifference = item.NAccumulativeDifference - item.LastNAccumulativeDifference;
                    }
                    else
                    {
                        item.AddDifference = item.NAccumulativeDifference - 0;
                    }

                    MissTargetTextList.Add(item);
                }
            }

            Dictionary<string, object> Alldata = new Dictionary<string, object>();

            List<DictionaryVmodel> ReturnList = new List<DictionaryVmodel>();
            List<MonthlyReportDetail> AllMissTargetDetail = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> ListMonthReportDetailTemp = new List<MonthlyReportDetail>();
            ListMonthReportDetailTemp.Clear();
            AllMissTargetDetail.Clear();
            foreach (VGroup vGroup in lstVGroup)
            {
                foreach (Guid guid in vGroup.TargetIDs)
                {
                    ListMonthReportDetailTemp.AddRange(MissTargetTextList.Where(p => p.TargetID == guid));
                }
                AllMissTargetDetail.AddRange(FormatData(ListMonthReportDetailTemp, vGroup));
                ListMonthReportDetailTemp.Clear();
            }

            ReturnList.Add(new DictionaryVmodel("", AllMissTargetDetail));

            return ReturnList;
        }

        public List<MonthlyReportDetail> FormatData(List<MonthlyReportDetail> ListMissTargetDetails, VGroup vGroup)
        {
            List<MonthlyReportDetail> lstDetailis = new List<MonthlyReportDetail>();
            ExpressionParser _parser = null;

            foreach (MonthlyReportDetail mrd in ListMissTargetDetails)
            {
                foreach (VCounter vcounter in vGroup.CounterList)
                {
                    C_Target cTarget = StaticResource.Instance.TargetList[mrd.SystemID].Where(p => p.ID == mrd.TargetID).FirstOrDefault();
                    if (cTarget != null)
                    {
                        if (cTarget.TargetName == vcounter.Title)
                        {
                            Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(ListMissTargetDetails.FindAll(p => p.CompanyID == mrd.CompanyID), "MonthlyReportDetail");
                            _parser = new ExpressionParser(bizContext);
                            //区分月累计算式与年累计算式。
                            if (_parser.CacluateCondition(vcounter.Expression))
                            {
                                lstDetailis.Add(mrd);
                            }
                        }
                    }
                }
            }

            return lstDetailis;
        }

        public List<VGroup> GetDirectlyMissTargetXml(XElement element)
        {
            List<VGroup> GroupList = new List<VGroup>();

            if (element.Elements("Misstarget").Elements("Group") != null)
            {
                List<XElement> Groups = element.Elements("Misstarget").Elements("Group").ToList();
                foreach (XElement group in Groups)
                {
                    GroupList.Add(new VGroup(group));
                }
            }
            return GroupList;
        }

    }

}
