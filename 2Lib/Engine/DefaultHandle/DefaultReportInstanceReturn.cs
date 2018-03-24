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
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;

namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 更具获取系统，填充某个系统的补充项
    /// </summary>
    public class DefaultReportInstanceReturn : IReportInstanceReturn
    {
        /// <summary>
        /// 上报月上个个月的未完成数据
        /// </summary>
        List<MonthlyReportDetail> VLastList = null;

        List<MonthlyReportDetail> ReportDetails =null;
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;


        public List<DictionaryVmodel> GetReturnRptDataSource(ReportInstance RptModel, C_System sys)
        {
            VLastList = new List<MonthlyReportDetail>();
            _System = sys;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            ReportDetails = RptModel.ReportDetails;

            List<VGroup> GroupList = new List<VGroup>();
            XElement element = null;

            //加载XML，系统配置信息
            element = _System.Configuration;

            if (element.Elements("MisstargetReturn").Elements("Group") != null)
            {
                List<XElement> Groups = element.Elements("MisstargetReturn").Elements("Group").ToList();
                foreach (XElement group in Groups)
                {
                    GroupList.Add(new VGroup(group));
                }
            }

            //上个月A表单一指标
            if (VLastList == null || VLastList.Count <= 0)
            {
                int lastFinMonth = 0;

                if (FinMonth == 1)
                    lastFinMonth = 0;
                else
                    lastFinMonth = FinMonth - 1;

                //上月A表的数据
                List<A_MonthlyReportDetail> Lastlist = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(_System.ID, FinYear, lastFinMonth).ToList();
                //当月A表数据
                List<A_MonthlyReportDetail> list_A = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(_System.ID, FinYear, FinMonth).ToList();

                //当月B表数据
                List<B_MonthlyReportDetail> list_B = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(_System.ID, FinYear, FinMonth, RptModel._MonthReportID).ToList();

                List<Guid> Companies = new List<Guid>();

                //调用引擎
                Companies = LastMonthCompaniesEngine.LastMonthCompaniesService.GetLastMonthCompaniesID(Lastlist, _System.ID);

                ReportDetails.RemoveAll(R => !Companies.Contains(R.CompanyID));
                foreach (MonthlyReportDetail item in ReportDetails) //循环当月
                {
                    A_MonthlyReportDetail lastMRD = Lastlist.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID);

                    if (lastMRD != null)
                    {
                        item.LastNAccumulativeActualAmmount = lastMRD.NAccumulativeActualAmmount;
                        item.LastNAccumulativeDifference = lastMRD.NAccumulativeDifference;
                        item.LastNAccumulativePlanAmmount = lastMRD.NAccumulativePlanAmmount;
                        item.LastIsMissTarget = lastMRD.IsMissTarget;
                        item.AddDifference = item.NAccumulativeDifference - item.LastNAccumulativeDifference;
                    }
                    VLastList.Add(item);
                }

            }

            List<DictionaryVmodel> ReturnList = new List<DictionaryVmodel>();

            //补回情况，分组List
            foreach (var Group in GroupList.OrderBy(G => G.Senquence))
            {
                DictionaryVmodel Vmodel = new DictionaryVmodel();
                Vmodel.Name = Group.TargetName;
                Vmodel.ObjValue = FormartVData(Group, "TargetReturn", Group.TargetName);
                Vmodel.Mark = "Group";
                Vmodel.GuoupID = Group.TargetName;
                Vmodel.SystemName = _System.SystemName;
                Vmodel.TargetGroupCount = Group.TargetIDs.Count();
                Vmodel.HtmlTemplate = GetRetuenHtmlTemplate(element);
                ReturnList.Add(Vmodel);
            }

            return ReturnList;
        }


        /// <summary>
        /// 获取补回数据的展示模版名称
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private string GetRetuenHtmlTemplate(XElement element)
        {
            string strValue = "";
            if (element.Elements("MisstargetReturn").Elements("TableTemplate").ToList().Count > 0)
            {
                XElement xt = element.Elements("MisstargetReturn").Elements("TableTemplate").ToList()[0];
                strValue = xt.GetAttributeValue("TableDataTmplName", "");
            }
            return strValue;
        }


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
            if (counter.SubCounterList != null && counter.SubCounterList.Count > 0)
            {
                List<MonthlyReportDetail> subSource = Calclatedata(counter, Group.TargetIDs, ref Source);

                foreach (VCounter c in counter.SubCounterList)
                {
                    if (c.SubCounterList != null && c.SubCounterList.Count > 0)
                    {
                        DictionaryVmodel dictC = new DictionaryVmodel(c.Title, FormartCounterData(c, Group, XmlType, GroupStr, ref subSource));
                        dictC.Mark = "Counter";
                        dictC.GuoupID = GroupStr;
                        dictC.TargetGroupCount = Group.TargetIDs.Count();
                        dictC.SystemName = _System.SystemName;
                        result.Add(dictC);
                    }
                    else
                    {
                        result.AddRange(FormartCounterData(c, Group, XmlType, GroupStr, ref subSource));
                    }
                }
            }
            else
            {
                DictionaryVmodel dictR = new DictionaryVmodel(counter.Title, Calclatedata(counter, Group.TargetIDs, ref Source));
                dictR.Mark = "Data";
                dictR.GuoupID = GroupStr;
                dictR.TargetGroupCount = Group.TargetIDs.Count();
                dictR.SystemName = _System.SystemName;
                result.Add(dictR);
            }
            return result;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="TargetIDs"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> Calclatedata(VCounter counter, List<Guid> TargetIDs, ref List<MonthlyReportDetail> Source)
        {
            List<Guid> Companys = new List<Guid>();

            List<MonthlyReportDetail> Result = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> Filter = new List<MonthlyReportDetail>();

            foreach (MonthlyReportDetail d in Source)
            {
                if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
                {
                    d.TargetGroupCount = TargetIDs.Count();
                    Filter.Add(d); //添加

                    Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(Source.FindAll(R => R.CompanyID == d.CompanyID), "A_MonthlyReportDetail");
                    ExpressionParser _parser = new ExpressionParser(bizContext);

                    //解析这个公式
                    //一个公司多个指标数据属于同一个Group
                    //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
                    if (_parser.CacluateCondition(counter.Expression))
                    {
                        if (!Companys.Contains(d.CompanyID))
                        {
                            Companys.Add(d.CompanyID);
                        }
                    }
                }
            }

            foreach (Guid CompanyID in Companys)
            {
                //判断当前公司的数量,是否相同
                if (Filter.Where(r => r.CompanyID == CompanyID).Count() == TargetIDs.Count())
                {
                    //将筛选出来的值，添加到结果后，马上移除
                    Result.AddRange(Filter.FindAll(R => R.CompanyID == CompanyID)); ///
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
                            GroupTile = model.GroupTile,        TargetGroupCount = model.TargetGroupCount,
                            Title = model.Title,                CompanyName = model.CompanyName,
                            CompanyID = model.CompanyID,        TargetID = model.TargetID,//
                            ReturnType = model.ReturnType,      Counter = 0,
                            NAccumulativeActualAmmount = 0,     NAccumulativeDifference = 0,
                            AddDifference = 0,                  NAccumulativePlanAmmount = 0,
                            LastNAccumulativeActualAmmount = 0, LastNAccumulativeDifference = 0,
                            LastNAccumulativePlanAmmount = 0,   TargetName = "--",
                            SystemID = model.SystemID ,         CreateTime = model.CreateTime
                        });
                    }

                    //将筛选出来的值，添加到结果后，马上移除
                    Result.AddRange(Filter.FindAll(R => R.CompanyID == CompanyID));
                    Result.AddRange(NewDetail);
                }

                //从总数据中移除已经分类的数据
                Source.RemoveAll(Re => Result.Exists(R => R.ID == Re.ID));

            }

            List<MonthlyReportDetail> returnList = SequenceEngine.SequenceService.GetSequence(_System.ID, "Return", Result, JsonHelper.Serialize(TargetIDs));

            return returnList;
        }

    }


    /// <summary>
    /// 更具获取系统，填充某个系统的补充项
    /// </summary>
    public class DirectlyReportInstanceReturn : IReportInstanceReturn
    {
        /// <summary>
        /// 上报月上个个月的未完成数据
        /// </summary>
        List<MonthlyReportDetail> VLastList = null;

        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;


        public List<DictionaryVmodel> GetReturnRptDataSource(ReportInstance RptModel,C_System sys)
        {
            VLastList=new List<MonthlyReportDetail>();
            _System = sys;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            ReportDetails = RptModel.ReportDetails;

            List<VGroup> GroupList = new List<VGroup>();
            XElement element = null;

            //加载XML，系统配置信息
            element = _System.Configuration;

            if (element.Elements("MisstargetReturn").Elements("Group") != null)
            {
                List<XElement> Groups = element.Elements("MisstargetReturn").Elements("Group").ToList();
                foreach (XElement group in Groups)
                {
                    GroupList.Add(new VGroup(group));
                }
            }

            //上个月A表单一指标
            if (VLastList == null || VLastList.Count <= 0)
            {
                int lastFinMonth = 0;

                if (FinMonth == 1)
                    lastFinMonth = 0;
                else
                    lastFinMonth = FinMonth - 1;

                //上月A表的数据
                List<A_MonthlyReportDetail> Lastlist = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(_System.ID, FinYear, lastFinMonth).ToList();

                List<Guid> Companies = new List<Guid>();

                //调用引擎
                Companies = LastMonthCompaniesEngine.LastMonthCompaniesService.GetLastMonthCompaniesID(Lastlist, _System.ID);

                ReportDetails.RemoveAll(R => !Companies.Contains(R.TargetID));
                foreach (MonthlyReportDetail item in ReportDetails) //循环当月
                {
                    A_MonthlyReportDetail lastMRD = Lastlist.Find(p => p.CompanyID == item.CompanyID && p.TargetID == item.TargetID);

                    if (lastMRD != null)
                    {
                        item.LastNAccumulativeActualAmmount = lastMRD.NAccumulativeActualAmmount;
                        item.LastNAccumulativeDifference = lastMRD.NAccumulativeDifference;
                        item.LastNAccumulativePlanAmmount = lastMRD.NAccumulativePlanAmmount;
                        item.LastIsMissTarget = lastMRD.IsMissTarget;
                        item.AddDifference = item.NAccumulativeDifference - item.LastNAccumulativeDifference;
                    }
                    VLastList.Add(item);
                }

            }

            List<DictionaryVmodel> ReturnList = new List<DictionaryVmodel>();

            //补回情况，分组List
            foreach (var Group in GroupList.OrderBy(G => G.Senquence))
            {
                DictionaryVmodel Vmodel = new DictionaryVmodel();
                Vmodel.Name = Group.TargetName;
                Vmodel.ObjValue = FormartVData(Group, "TargetReturn", Group.TargetName);
                Vmodel.Mark = "Group";
                Vmodel.GuoupID = Group.TargetName;
                Vmodel.SystemName = _System.SystemName;
                Vmodel.TargetGroupCount = 1; //直管公司强制为1组一个指标，没有组合指标
                ReturnList.Add(Vmodel);
            }
            return ReturnList;
        }

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
                    dictC.TargetGroupCount = 1;
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
            if (counter.SubCounterList != null && counter.SubCounterList.Count > 0)
            {
                List<MonthlyReportDetail> subSource = Calclatedata(counter, Group.TargetIDs, ref Source);

                foreach (VCounter c in counter.SubCounterList)
                {
                    if (c.SubCounterList != null && c.SubCounterList.Count > 0)
                    {
                        DictionaryVmodel dictC = new DictionaryVmodel(c.Title, FormartCounterData(c, Group, XmlType, GroupStr, ref subSource));
                        dictC.Mark = "Counter";
                        dictC.GuoupID = GroupStr;
                        dictC.TargetGroupCount = 1;
                        dictC.SystemName = _System.SystemName;
                        result.Add(dictC);
                    }
                    else
                    {
                        result.AddRange(FormartCounterData(c, Group, XmlType, GroupStr, ref subSource));
                    }
                }
            }
            else
            {
                DictionaryVmodel dictR = new DictionaryVmodel(counter.Title, Calclatedata(counter, Group.TargetIDs, ref Source));
                dictR.Mark = "Data";
                dictR.GuoupID = GroupStr;
                dictR.TargetGroupCount = 1;
                dictR.SystemName = _System.SystemName;
                result.Add(dictR);
            }
            return result;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="TargetIDs"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> Calclatedata(VCounter counter, List<Guid> TargetIDs, ref List<MonthlyReportDetail> Source)
        {
            List<Guid> Companys = new List<Guid>();

            List<Guid> targets = new List<Guid>();

            List<MonthlyReportDetail> Result = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> Filter = new List<MonthlyReportDetail>();

            foreach (MonthlyReportDetail d in Source)
            {
                if (TargetIDs.Contains(d.TargetID)) //上月指标包含当前指标
                {
                    d.TargetGroupCount = 1;
                    Filter.Add(d); //添加

                    Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(Source.FindAll(R => R.TargetID == d.TargetID), "A_MonthlyReportDetail");
                    ExpressionParser _parser = new ExpressionParser(bizContext);

                    //解析这个公式
                    //一个公司多个指标数据属于同一个Group
                    //其中任何一个指标数据满足Counter的条件，此公司的数据都应该归类于该Counter
                 
                    if (_parser.CacluateCondition(counter.Expression))
                    {
                        if (!targets.Contains(d.TargetID))
                        {
                            targets.Add(d.TargetID);
                        }
                    }
                    
                }
            }


            foreach (Guid itemtarget in targets)
            {
                //将筛选出来的值，添加到结果后，马上移除
                Result.AddRange(Filter.FindAll(R => R.TargetID == itemtarget)); ///
                
                //从总数据中移除已经分类的数据
                Source.RemoveAll(Re => Result.Exists(R => R.ID == Re.ID));
            }

            List<MonthlyReportDetail> returnList = SequenceEngine.SequenceService.GetSequence(_System.ID, "Return", Result, JsonHelper.Serialize(TargetIDs));

            return returnList;
        }

    }
}
