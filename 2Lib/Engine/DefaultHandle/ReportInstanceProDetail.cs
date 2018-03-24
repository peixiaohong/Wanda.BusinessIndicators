using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;
using Lib.Expression;
using System.Collections;
using Lib.Core;


namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 根据获取系统，填充某个系统的补充项
    /// </summary>
    public class ReportInstanceProDetail : IReportInstanceDetail
    {
        /// <summary>
        /// 是否是计划指标
        /// </summary>
        string IsPlanStr;
        /// <summary>
        /// 是否是最新版本，True:B表，false :A 表
        /// </summary>
        bool IsLatestVersion;

        /// <summary>
        /// 这里只是为了判断调用来源
        /// </summary>
        string strCompanyPropertys = string.Empty;

        /// <summary>
        /// 本区明细数据
        /// </summary>
        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        List<C_Target> _Target = null;
        Guid _SystemID = Guid.Empty;

        ReportInstance currRptModel = null;

        public List<DictionaryVmodel> GetDetailRptDataSource(ReportInstance RptModel, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
        {
            _System = RptModel._System;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            _Target = RptModel._Target;
            ReportDetails = RptModel.ReportDetails;
            _SystemID = _System.ID;
            IsLatestVersion = IncludeHaveDetail;
            IsPlanStr = strOrderType;
            currRptModel = RptModel;

            if (!string.IsNullOrEmpty(strCompanyProperty))
            {
                if (strCompanyProperty == "Reported")
                {
                    _System = StaticResource.Instance[_System.ID, DateTime.Now];
                }
                else
                {
                    strCompanyPropertys = strCompanyProperty;
                    if (ReportDetails != null && ReportDetails.Count() > 0)
                    {
                        _System = StaticResource.Instance[_System.ID, ReportDetails[0].CreateTime];
                        _Target = StaticResource.Instance.GetTargetList(_System.ID, ReportDetails[0].CreateTime).ToList();
                    }
                }
            }
            else
            {
                strCompanyPropertys = "";
            }


            List<ProjectCounter> ProjectList = new List<ProjectCounter>();

            XElement element = null;
            //加载XML，系统配置信息
            element = _System.Configuration;

            if (element.Elements("ProjectCompanyDetail").Elements("ProjectCounter") != null)
            {
                List<XElement> ProjectCompanys = element.Elements("ProjectCompanyDetail").Elements("ProjectCounter").ToList();
                foreach (XElement ProCompany in ProjectCompanys)
                {
                    ProjectList.Add(new ProjectCounter(ProCompany));
                }
            }

            List<DictionaryVmodel> list = FormartProData(ProjectList);

            return list;
        }

        private List<DictionaryVmodel> FormartProData(List<ProjectCounter> ProCounter)
        {
            List<DictionaryVmodel> result = new List<DictionaryVmodel>();

            List<V_ProjectCompany> ProList = new List<V_ProjectCompany>();

            List<V_ProjectCompany> VP = new List<V_ProjectCompany>();

            /*这里XML确保“区域公司小计”的排序在“总计”的前面,这样在计算的时候容易，
             * 但是在展示的时候 总计需要在区域公司的前面，这个时候需要重新排序，同时还需要将明细数据放到最后
             */

            foreach (ProjectCounter item in ProCounter.OrderBy(g => g.Senquence))
            {
                if (!item.IsDetail) //不是明细项
                {
                    if (item.Title == "总计")
                    {
                        //显示的项
                        ProList.AddRange(GetProjectModel(ProList, item)); //2 总计
                    }
                    else
                    {
                        ProList.AddRange(GetProjectModel(item));  //1 区域小计
                    }
                }
                else
                {   //是明细项
                    VP = ProList.OrderBy(p => p.ProCompanySequence).ToList(); //重新排序  //3
                    VP.AddRange(GetProjectCompany());
                }
            }
            DictionaryVmodel Dict = new DictionaryVmodel();
            Dict.Mark = "Data";
            Dict.ObjValue = VP;


            result.Add(Dict);
            return result;
        }

        List<MonthlyReportDetail> ProjectCompanyDetails = new List<MonthlyReportDetail>();



        #region 计算本区域明细数据

        /// <summary>
        /// 项目公司详细数据（第三步）
        /// </summary>
        /// <param name="IsProLatestVersion">false ：从A表中获取 ， true:从B表中获取</param>
        /// <param name="IsPlan">是否获取计划指标</param>
        /// <returns></returns>
        public List<V_ProjectCompany> GetProjectCompany()
        {
            List<V_ProjectCompany> ProCompanyList = new List<V_ProjectCompany>();
            ProjectCompanyDetails = ReportDetails;
            List<C_Company> companyList = StaticResource.Instance.CompanyList[_SystemID].ToList();

            List<C_Target> targetList = StaticResource.Instance.TargetList[_SystemID].ToList();
            List<A_TargetPlanDetail> TargetPlanList = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear, FinMonth);

            foreach (C_Company itemCompany in companyList.OrderBy(o => o.Sequence))
            {
                //已经上报的明细数据
                List<MonthlyReportDetail> TempProCompany = ProjectCompanyDetails.FindAll(p => p.CompanyID == itemCompany.ID).ToList();
                //本月计划指标分解表
                List<A_TargetPlanDetail> TempTargetPlan = TargetPlanList.FindAll(p => p.CompanyID == itemCompany.ID).ToList(); //指标计划详细

                List<V_ProjectTarget> ProTargetList = new List<V_ProjectTarget>();

                if (IsPlanStr == "ProPlan")
                {
                    if (TempTargetPlan.Count == 0) continue;
                }
                else
                {
                    if (TempProCompany.Count == 0) continue;
                }

                V_ProjectCompany tempProCompany = new V_ProjectCompany();
                tempProCompany.ProCompanySequence = itemCompany.Sequence;
                tempProCompany.SystemID = itemCompany.SystemID;
                tempProCompany.ProCompayName = itemCompany.CompanyName;
                tempProCompany.ProCompayID = itemCompany.ID;
                tempProCompany.ProCompanyProperty1 = itemCompany.CompanyProperty1;
                tempProCompany.FinYear = FinYear;
                tempProCompany.FinMonth = FinMonth;
                tempProCompany.CompayModel = itemCompany;
                tempProCompany.ProRowSpan = 1;
                tempProCompany.ProCompanyNumber = 0;
                if (itemCompany.CompanyProperty1 == "尾盘")
                {
                    tempProCompany.ProDataType = "Remain";
                }
                else
                {
                    tempProCompany.ProDataType = "Data";
                }
                tempProCompany.ProjectTargets = ProTargetLists(itemCompany, IsPlanStr, TempProCompany, TempTargetPlan);
                ProCompanyList.Add(tempProCompany);
            }


            //必须要有数据，否则直接过
            if (ProCompanyList.Count != 0)
            {

                //获取当前的系统指标
                List<A_TargetPlanDetail> TargetPlanListByYear = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);

                #region 对有分组的数据 分组数据 属性：CompanyProperty2

                List<string> value = (from v in companyList select v.CompanyProperty2).Distinct().ToList();
                foreach (var item in value)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        List<MonthlyReportDetail> FilterMonthRptDetails = new List<MonthlyReportDetail>();
                        List<A_TargetPlanDetail> FilterTargetPlanList = new List<A_TargetPlanDetail>();

                        //过滤分组后的数据
                        List<C_Company> Filter = companyList.FindAll(p => p.CompanyProperty2 == item.ToString()).OrderBy(o => o.Sequence).ToList();
                        for (int i = 0; i < Filter.Count; i++)
                        {
                            //得到分组数据的第一条数据，设置通行的行数
                            V_ProjectCompany proModel = ProCompanyList.Find(p => p.ProCompayID == Filter[i].ID);

                            //是否是在计划指标下载
                            if (IsPlanStr != "ProPlan")
                            {
                                //页面展示时，通行
                                if (i == 0)
                                    proModel.ProRowSpan = Filter.Count + 1;
                                else
                                    proModel.ProRowSpan = 0;

                                // MonthRptDetails分组的数据，包含有指标，便于计算合计
                                FilterMonthRptDetails.AddRange(ProjectCompanyDetails.FindAll(f => f.CompanyID == Filter[i].ID).ToList());

                                //年度计划指标
                                FilterTargetPlanList.AddRange(TargetPlanListByYear.FindAll(f => f.CompanyID == Filter[i].ID).ToList());
                            }
                            else
                            {
                                //页面展示时，通行
                                if (i == 0)
                                    proModel.ProRowSpan = Filter.Count;
                                else
                                    proModel.ProRowSpan = 0;
                            }

                        }

                        #region 添加 分组合计数据 ,只在查询的时候出现，其它时候不出现

                        if (IsPlanStr != "ProPlan")
                        {
                            V_ProjectCompany _LastGroupData = ProCompanyList.Find(p => p.ProCompayID == Filter[Filter.Count - 1].ID);

                            V_ProjectCompany groupSumProModel = new V_ProjectCompany();
                            groupSumProModel.ProCompayID = Guid.Parse("88888888-8888-8888-8888-888888888888");
                            groupSumProModel.ProCompayName = item + "小计";
                            groupSumProModel.ProDataType = _LastGroupData.ProDataType;
                            groupSumProModel.ProRowSpan = 0;
                            groupSumProModel.SystemID = _LastGroupData.SystemID;
                            groupSumProModel.FinYear = FinYear;
                            groupSumProModel.FinMonth = FinMonth;
                            groupSumProModel.ProCompanyNumber = _LastGroupData.ProCompanyNumber;
                            groupSumProModel.ProjectTargets = ProTargetListByGroupSum(FilterMonthRptDetails, FilterTargetPlanList);
                            groupSumProModel.ProCompanySequence = _LastGroupData.ProCompanySequence;

                            ProCompanyList.Add(groupSumProModel);
                        }


                        #endregion

                    }
                }

                #endregion


                #region 分组数据 属性：CompanyProperty1 尾盘

                List<string> RemainData = (from v in companyList select v.CompanyProperty1).Distinct().ToList();
                foreach (string itemR in RemainData)
                {
                    if (!string.IsNullOrEmpty(itemR))
                    {
                        List<C_Company> _Filter = companyList.FindAll(p => p.CompanyProperty1 == itemR.ToString()).OrderBy(o => o.Sequence).ToList();
                        for (int i = 0; i < _Filter.Count; i++)
                        {
                            //得到分组数据的第一条数据，设置通行的行数
                            V_ProjectCompany proModel = ProCompanyList.Find(p => p.ProCompayID == _Filter[i].ID);
                            if (proModel != null)
                            {
                                if (i == 0)
                                    proModel.ExcelGroupRow = _Filter.Count;
                                else
                                    proModel.ExcelGroupRow = 0;
                            }
                        }
                    }
                }

                #endregion


                #region 循环添加项目的序号

                int j = 1;
                foreach (var itemNumber in ProCompanyList.FindAll(p => p.ProRowSpan > 0).OrderBy(p => p.ProCompanySequence).ToList())
                {
                    itemNumber.ProCompanyNumber = j++;
                }

                #endregion
            }

            return ProCompanyList.OrderBy(d => d.ProCompanySequence).ToList();
        }


        /// <summary>
        /// 项目公司的指标
        /// </summary>
        /// <param name="companyModel"></param>
        /// <param name="_isPlan">是获取计划指标分解表</param>
        /// <param name="_ReportDetail">明细数据</param>
        /// <param name="_TargetPlanList">指标分解表</param>
        /// <returns></returns>
        private List<V_ProjectTarget> ProTargetLists(C_Company companyModel, string _isPlan, List<MonthlyReportDetail> _ReportDetail, List<A_TargetPlanDetail> _TargetPlanList)
        {
            List<V_ProjectTarget> _ProTargetLists = new List<V_ProjectTarget>();

            List<C_Target> targetList = StaticResource.Instance.TargetList[_SystemID].ToList();

            //获取当前的系统指标
            List<A_TargetPlanDetail> TargetPlanListByYear = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);

            //循环指标

            if (_isPlan == "ProPlan") //计划指标分解表
            {
                foreach (C_Target itemTarget in targetList.OrderBy(o => o.Sequence))
                {
                    V_ProjectTarget VModel = new V_ProjectTarget();

                    A_TargetPlanDetail targetModel = null;
                    if (_TargetPlanList.Count > 0)
                        targetModel = _TargetPlanList.Find(f => f.TargetID == itemTarget.ID);
                    else
                        continue;

                    if (targetModel != null && targetModel.ID != Guid.Empty)
                    {
                        VModel.ProTargetID = itemTarget.ID;
                        VModel.ProTargetName = itemTarget.TargetName;
                        VModel.ProTargetSequence = itemTarget.Sequence;
                        VModel.ProCompayID = companyModel.ID;
                        VModel.IsMissTarget = false;
                        VModel.IsMissTargetCurrent = false;
                        VModel.Counter = 0;

                        VModel.NPlanAmmount = targetModel.Target;
                        VModel.NActualAmmount = 0;

                        VModel.NAccumulativePlanAmmount = TargetPlanListByYear.Where(p => p.FinMonth <= FinMonth && p.TargetID == itemTarget.ID && p.CompanyID == companyModel.ID).Sum(s => s.Target);  // ?累计的指标怎么算？
                        VModel.NAccumulativeActualAmmount = 0;

                        _ProTargetLists.Add(VModel);
                    }
                }
            }
            else //上报过，在明细表中了
            {
                foreach (C_Target itemTarget in targetList.OrderBy(o => o.Sequence))
                {
                    V_ProjectTarget VModel = new V_ProjectTarget();

                    MonthlyReportDetail tempModel = null;
                    if (_ReportDetail.Count > 0)
                        tempModel = _ReportDetail.Find(f => f.TargetID == itemTarget.ID);
                    else
                        continue;
                    if (tempModel != null && tempModel.ID != Guid.Empty)
                    {
                        VModel.ProMonthlyReportDetailID = tempModel.ID;
                        VModel.ProTargetID = itemTarget.ID;
                        VModel.ProTargetName = tempModel.TargetName;
                        VModel.ProTargetSequence = itemTarget.Sequence;
                        VModel.ProCompayID = companyModel.ID;
                        VModel.IsMissTarget = tempModel.IsMissTarget;
                        VModel.IsMissTargetCurrent = tempModel.IsMissTargetCurrent;
                        VModel.Counter = tempModel.Counter;
                        VModel.FirstMissTargetDate = tempModel.FirstMissTargetDate;

                        VModel.NPlanAmmount = tempModel.NPlanAmmount;
                        VModel.NActualAmmount = tempModel.NActualAmmount;
                        VModel.NActualRate = tempModel.NActualRate;
                        VModel.NDisplayRate = tempModel.NDisplayRate;

                        VModel.NAccumulativePlanAmmount = tempModel.NAccumulativePlanAmmount;
                        VModel.NAccumulativeActualAmmount = tempModel.NAccumulativeActualAmmount;
                        VModel.NAccumulativeActualRate = tempModel.NAccumulativeActualRate;
                        VModel.NAccumulativeDisplayRate = tempModel.NAccumulativeDisplayRate;

                        VModel.NActualAmmountByYear = 0;

                        //年度计划值
                        VModel.NPlanAmmountByYear = TargetPlanListByYear.Where(t => t.CompanyID == companyModel.ID && t.TargetID == itemTarget.ID).Sum(s => s.Target);
                        if (VModel.NPlanAmmountByYear != 0)
                        {
                            VModel.NDisplayRateByYear = Math.Round((VModel.NAccumulativeActualAmmount / VModel.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                        }
                        else
                        {
                            VModel.NPlanAmmountByYear = 0;
                            VModel.NDisplayRateByYear = "/";
                        }
                        VModel.NActualRateByYear = "";

                        _ProTargetLists.Add(VModel);
                    }
                }
            }
            return _ProTargetLists;
        }


        /// <summary>
        /// 计算分组小计的指标
        /// </summary>
        /// <param name="FilterMonthRptDetails"></param>
        /// <param name="FilterTargetPlanList"></param>
        /// <returns></returns>
        private List<V_ProjectTarget> ProTargetListByGroupSum(List<MonthlyReportDetail> FilterMonthRptDetails, List<A_TargetPlanDetail> FilterTargetPlanList)
        {
            List<V_ProjectTarget> _ProTargetLists = new List<V_ProjectTarget>();

            List<C_Target> targetList = StaticResource.Instance.TargetList[_SystemID].ToList();

            //循环指标
            foreach (C_Target itemTarget in targetList.OrderBy(o => o.Sequence))
            {
                V_ProjectTarget VModel = new V_ProjectTarget();

                List<MonthlyReportDetail> tempRptDetail = null;

                if (FilterMonthRptDetails.Count > 0)
                    tempRptDetail = FilterMonthRptDetails.FindAll(f => f.TargetID == itemTarget.ID);
                else
                    continue;

                if (tempRptDetail.Count > 0)
                {
                    VModel.ProMonthlyReportDetailID = Guid.Empty;
                    VModel.ProTargetID = itemTarget.ID;
                    VModel.ProTargetName = itemTarget.TargetName;
                    VModel.ProTargetSequence = itemTarget.Sequence;
                    VModel.ProCompayID = Guid.Parse("88888888-8888-8888-8888-888888888888");
                    VModel.IsMissTarget = false;
                    VModel.IsMissTargetCurrent = false;
                    VModel.Counter = 0;
                    //VModel.FirstMissTargetDate = tempModel.FirstMissTargetDate;

                    VModel.NPlanAmmount = tempRptDetail.Sum(s => s.NPlanAmmount);
                    VModel.NActualAmmount = tempRptDetail.Sum(s => s.NActualAmmount);

                    //当前
                    Hashtable currHt = GetActualRateAndDisplayRate(VModel.NActualAmmount, VModel.NPlanAmmount);
                    foreach (DictionaryEntry obj in currHt)
                    {
                        VModel.NActualRate = obj.Key.ToString();
                        VModel.NDisplayRate = obj.Value.ToString();
                    }

                    VModel.NAccumulativePlanAmmount = tempRptDetail.Sum(s => s.NAccumulativePlanAmmount);
                    VModel.NAccumulativeActualAmmount = tempRptDetail.Sum(s => s.NAccumulativeActualAmmount);

                    //累计的
                    Hashtable accumulativeHt = GetActualRateAndDisplayRate(VModel.NAccumulativeActualAmmount, VModel.NAccumulativePlanAmmount);
                    foreach (DictionaryEntry obj in accumulativeHt)
                    {
                        VModel.NAccumulativeActualRate = obj.Key.ToString();
                        VModel.NAccumulativeDisplayRate = obj.Value.ToString();
                    }


                    VModel.NActualAmmountByYear = 0;

                    //年度计划值
                    VModel.NPlanAmmountByYear = FilterTargetPlanList.Where(t => t.TargetID == itemTarget.ID).Sum(s => s.Target);
                    if (VModel.NPlanAmmountByYear != 0)
                    {
                        VModel.NDisplayRateByYear = Math.Round((VModel.NAccumulativeActualAmmount / VModel.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                    }
                    else
                    {
                        VModel.NPlanAmmountByYear = 0;
                        VModel.NDisplayRateByYear = "/";
                    }
                    VModel.NActualRateByYear = "";

                    _ProTargetLists.Add(VModel);
                }
            }

            return _ProTargetLists;
        }


        #region 计算分组小计的完成率

        /// <summary>
        ///  Hashtable : [ key: _ActualRate  , value: _DisplayRate ]
        /// </summary>
        /// <param name="actualValue">实际</param>
        /// <param name="planValue">计划</param>
        /// <returns></returns>
        private Hashtable GetActualRateAndDisplayRate(decimal actualValue, decimal planValue)
        {
            Hashtable ht = new Hashtable();

            string _ActualRate = string.Empty;

            string _NDisplayRate = string.Empty;

            decimal DisplayRate = 0;

            if (planValue > 0) //计划 大于 0
            {
                if (actualValue >= 0)
                {
                    DisplayRate = Math.Abs(actualValue / planValue);
                    _ActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    _NDisplayRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, 1, "");

                    ht.Add(_ActualRate, _NDisplayRate);
                }
                else
                {
                    DisplayRate = 0;
                    _ActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    _NDisplayRate = "/";
                    ht.Add(_ActualRate, _NDisplayRate);
                }
            }
            else if (planValue == 0)
            {
                DisplayRate = 0;
                _ActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                _NDisplayRate = "/";
                ht.Add(_ActualRate, _NDisplayRate);
            }
            else if (planValue < 0)
            {
                if (actualValue > 0) // A<0,B>0 ： 完成率：1+(B-A)/ABS(A)
                {
                    DisplayRate = 1 + (actualValue - planValue) / Math.Abs(planValue);
                    _ActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    _NDisplayRate = Utility.Instance.Pro_CalculateDisplayRate(DisplayRate, 1, "", 1);
                    ht.Add(_ActualRate, _NDisplayRate);
                }
                else  //A<0,B<=0 ： 完成率："/"
                {
                    DisplayRate = 0;
                    _ActualRate = JsonHelper.Serialize(new ActualRate(1, DisplayRate));
                    _NDisplayRate = "/";

                    ht.Add(_ActualRate, _NDisplayRate);
                }
            }

            return ht;
        }

        #endregion


        #endregion

        #region 计算区域总计的

        /// <summary>
        /// 获取区域公司总计的实体(第一步)
        /// </summary>
        /// <param name="ProCounter"></param>
        /// <returns></returns>
        private List<V_ProjectCompany> GetProjectModel(ProjectCounter ProCounter)
        {
            Guid SysId = Guid.Empty; //从XML中获取的系统ID

            Guid _CompanyId = Guid.Empty;

            if (!string.IsNullOrEmpty(ProCounter.CompanyID))
            {
                _CompanyId = Guid.Parse(ProCounter.CompanyID);
            }

            //公司，区域小计的ID，从XML中获取的
            C_Company itemCompany = C_CompanyOperator.Instance.GetCompany(_CompanyId);

            SysId = itemCompany.SystemID;

            //指标List
            List<C_Target> targetList = StaticResource.Instance.TargetList[SysId].ToList();

            //项目公司
            List<V_ProjectCompany> ProCompanyList = new List<V_ProjectCompany>();

            ReportInstance rpt = null;

            //获取批次ID，如果_MonthReportID 不是为Guid.Empty ，表示：是从上报页面过来的
            //反之，则代表是从查询页面传递过来的
            if (currRptModel._MonthReportID != Guid.Empty)
            {
                //上报页面
                ExceptionHelper.TrueThrow(currRptModel.LastestMonthlyReport == null, "B_MonthlyReport表为Null");

                if (currRptModel.LastestMonthlyReport.SystemBatchID != Guid.Empty)
                {
                    B_SystemBatch _BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(currRptModel.LastestMonthlyReport.SystemBatchID);

                    List<V_SubReport> subRptList = JsonHelper.Deserialize<List<V_SubReport>>(_BatchModel.SubReport);
                    subRptList.ForEach(p =>
                    {
                        if (p.SystemID == SysId)
                        {
                            rpt = new ReportInstance(p.ReportID, true);// 从B表中获取根据ReportID获取
                        }
                    });
                }
                else
                {
                    rpt = currRptModel;
                }
            }
            else
            {
                //查询页面, 通过系统倒序查找相应的项目系统
                rpt = new ReportInstance(SysId, FinYear, FinMonth, IsLatestVersion);
            }


            ProjectCompanyDetails = rpt.ReportDetails;

            //计划指标,包含了 区域小计的指标
            List<A_TargetPlanDetail> TargetPlanList = new List<A_TargetPlanDetail>();
            if (currRptModel._MonthReportID != Guid.Empty)
            { 
                //上报的时候总是获取最新的指标。
                TargetPlanList = StaticResource.Instance.GetTargetPlanList(SysId, FinYear, FinMonth);
            }else
            {
                //查询的时候按照版本查询,从B表中查出后转换成A表的数据
                List<B_TargetPlanDetail> _bTargetPlanList = new List<B_TargetPlanDetail>();

                _bTargetPlanList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(rpt.ReportDetails[0].TargetPlanID).ToList();
                
                //将B 表数据添加到 A表中
                _bTargetPlanList.ForEach(tp => TargetPlanList.Add(tp.ToAModel()));

               
                
            }

            List<V_ProjectTarget> ProTargetList = new List<V_ProjectTarget>();
            V_ProjectCompany tempProCompany = new V_ProjectCompany();
            tempProCompany.ProCompanySequence = itemCompany.Sequence;
            tempProCompany.SystemID = itemCompany.SystemID;
            tempProCompany.ProCompayName = itemCompany.CompanyName;
            tempProCompany.ProCompayID = itemCompany.ID;
            tempProCompany.ProCompanyProperty1 = itemCompany.CompanyProperty1;
            tempProCompany.FinYear = FinYear;
            tempProCompany.FinMonth = FinMonth;
            tempProCompany.CompayModel = itemCompany;
            tempProCompany.ProRowSpan = 0;
            tempProCompany.ProCompanyNumber = 0;

            //获取批次ID，如果_MonthReportID 不是为Guid.Empty ，表示：是从上报页面过来的
            //反之，则代表是从查询页面传递过来的
            if (currRptModel._MonthReportID != Guid.Empty)
            {
                tempProCompany.ProjectTargets = SingleProTargetLists(targetList, rpt.ReportDetails, TargetPlanList, itemCompany,false);
            }
            else
            {
                tempProCompany.ProjectTargets = SingleProTargetLists(targetList, rpt.ReportDetails, TargetPlanList, itemCompany,true);
            }
            
            tempProCompany.ProDataType = "XML";
            ProCompanyList.Add(tempProCompany);

            return ProCompanyList;
        }

       /// <summary>
        /// 项目公司的总数(第一步：1)
       /// </summary>
       /// <param name="_TargetList">指标基础表</param>
       /// <param name="_ReportDetail">明细数据</param>
       /// <param name="_TargetPlanList">计划指标表</param>
       /// <param name="companyModel">公司实体</param>
       /// <param name="IsQuery">上报：false ,查询：true (因有历史版本，所以需要区分)</param>
       /// <returns></returns>
        private List<V_ProjectTarget> SingleProTargetLists(List<C_Target> _TargetList, List<MonthlyReportDetail> _ReportDetail, List<A_TargetPlanDetail> _TargetPlanList, C_Company companyModel, bool IsQuery)
        {
            /* 此段代码的解释：
             * 这里计算项目系统的合计数据，因计划数都是从指标计划表中获取，而实际数都是从人员上报得到的，导致不同的数据源
             * 这里考虑到以后计算方便，只在基础表C_Company表中添加了每个系统的“南区合计”的公司，但是在B_MonthlyReportDetail表中不存储数据
             * 同时在指标计划表中，也做了“南区合计”的指标按月分解数据，这样便于取数和计算。
             * 本方法主要就是将这些数据拼装成一条新的List，追加到现有的明细表上，好用于前台的展示
             */

            List<V_ProjectTarget> _ProTargetLists = new List<V_ProjectTarget>();


            List<A_TargetPlanDetail> TargetPlanListByYear = new List<A_TargetPlanDetail>();
            //如果从查询页面进入
            if (IsQuery)
            {
                //查询的时候按照版本查询,从B表中查出后转换成A表的数据
                List<B_TargetPlanDetail> _bTargetPlanList = new List<B_TargetPlanDetail>();

                _bTargetPlanList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(_ReportDetail[0].TargetPlanID).ToList();

                _bTargetPlanList.ForEach(tp => TargetPlanListByYear.Add(tp.ToAModel()));
            }
            else
            {
                //获取当前的系统指标
                TargetPlanListByYear = StaticResource.Instance.GetTargetPlanList(companyModel.SystemID, FinYear);
            }
            

            foreach (C_Target itemTarget in _TargetList.OrderBy(g => g.Sequence))
            {
                V_ProjectTarget VModel = new V_ProjectTarget();

                //从明细表中获取数据
                MonthlyReportDetail tempModel = null;
                if (_ReportDetail.Count > 0)
                    tempModel = _ReportDetail.Find(f => f.TargetID == itemTarget.ID);


                if (tempModel == null)
                {
                    tempModel = new MonthlyReportDetail();

                    VModel.ProMonthlyReportDetailID = Guid.Empty;
                }
                VModel.ProMonthlyReportDetailID = tempModel.ID;
                VModel.ProTargetID = itemTarget.ID;
                VModel.ProTargetName = itemTarget.TargetName;
                VModel.ProTargetSequence = itemTarget.Sequence;
                VModel.ProCompayID = companyModel.ID;
                VModel.IsMissTarget = false;
                VModel.IsMissTargetCurrent = false;
                VModel.Counter = 0;
                //VModel.FirstMissTargetDate = tempModel.FirstMissTargetDate;

                //去指标分解表中找到 区域合计的指标
                A_TargetPlanDetail targetModel = null;
                if (_TargetPlanList.Count > 0)
                {
                    targetModel = _TargetPlanList.Find(f => f.TargetID == itemTarget.ID && f.FinMonth == FinMonth && f.FinYear == FinYear && f.CompanyID == companyModel.ID);

                    if (targetModel != null)
                    {
                        //区域：当月计划数 ,这个从指标计划中获取
                        VModel.NPlanAmmount = targetModel.Target; //区域：当月计划数 ,这个从指标计划中获取
                        //区域：当月累计计划数
                        VModel.NAccumulativePlanAmmount = TargetPlanListByYear.Where(p => p.FinMonth <= FinMonth && p.TargetID == itemTarget.ID && p.CompanyID == companyModel.ID).Sum(s => s.Target);  // ?累计的指标怎么算？

                        //年度计划值
                        VModel.NPlanAmmountByYear = TargetPlanListByYear.Where(t => t.CompanyID == companyModel.ID && t.TargetID == itemTarget.ID).Sum(s => s.Target);
                    }
                    else
                    {
                        VModel.NPlanAmmount = 0; //区域：当月计划数 ,这个从指标计划中获取
                        //区域：当月累计计划数
                        VModel.NAccumulativePlanAmmount = TargetPlanListByYear.Where(p => p.FinMonth <= FinMonth && p.TargetID == itemTarget.ID && p.CompanyID == companyModel.ID).Sum(s => s.Target);  // ?累计的指标怎么算？

                        //年度计划值
                        VModel.NPlanAmmountByYear = TargetPlanListByYear.Where(t => t.CompanyID == companyModel.ID && t.TargetID == itemTarget.ID).Sum(s => s.Target);
                    }
                }
                else
                {
                    //没有给计划指标
                    VModel.NPlanAmmount = 0;
                    VModel.NAccumulativePlanAmmount = 0;
                    VModel.NPlanAmmountByYear = 0;
                    //   continue;
                }


                if (_ReportDetail.Count > 0)
                    VModel.NActualAmmount = _ReportDetail.Where(p => p.FinMonth <= FinMonth && p.TargetID == itemTarget.ID).Sum(p => p.NActualAmmount);  //区域：从B表明细数据中汇总当月的实际数
                else
                    VModel.NActualAmmount = 0;

                if (VModel.NPlanAmmount == 0)
                {
                    VModel.NActualRate = "";
                    VModel.NDisplayRate = "/";
                }
                else
                {
                    VModel.NActualRate = "";
                    VModel.NDisplayRate = Math.Round((VModel.NActualAmmount / VModel.NPlanAmmount), 5, MidpointRounding.AwayFromZero).ToString("P1"); ;
                }

                //区域：当月累计计划数
                // VModel.NAccumulativePlanAmmount = TargetPlanListByYear.Where(p => p.FinMonth <= FinMonth && p.TargetID == itemTarget.ID && p.CompanyID == companyModel.ID).Sum(s => s.Target);  // ?累计的指标怎么算？

                if (_ReportDetail.Count > 0)
                    VModel.NAccumulativeActualAmmount = _ReportDetail.Where(p => p.FinMonth <= FinMonth && p.TargetID == itemTarget.ID).Sum(p => p.NAccumulativeActualAmmount);
                else
                    VModel.NAccumulativeActualAmmount = 0;

                if (VModel.NAccumulativePlanAmmount == 0)
                {
                    VModel.NAccumulativeActualRate = "";
                    VModel.NAccumulativeDisplayRate = "/";
                }
                else
                {
                    VModel.NAccumulativeActualRate = "";
                    VModel.NAccumulativeDisplayRate = Math.Round((VModel.NAccumulativeActualAmmount / VModel.NAccumulativePlanAmmount), 5, MidpointRounding.AwayFromZero).ToString("P1"); ;
                }

                VModel.NActualAmmountByYear = 0;

                ////年度计划值
                //VModel.NPlanAmmountByYear = TargetPlanListByYear.Where(t => t.CompanyID == companyModel.ID && t.TargetID == itemTarget.ID).Sum(s => s.Target);

                //年度指标完成比例
                if (VModel.NPlanAmmountByYear != 0)
                {
                    VModel.NDisplayRateByYear = Math.Round((VModel.NAccumulativeActualAmmount / VModel.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                }
                else
                {
                    VModel.NPlanAmmountByYear = 0;
                    VModel.NDisplayRateByYear = "/";
                }
                VModel.NActualRateByYear = "";

                _ProTargetLists.Add(VModel);
            }

            return _ProTargetLists;
        }

        /// <summary>
        /// 获取总计的数据(第二步)
        /// </summary>
        /// <param name="ProList"></param>
        /// <returns></returns>
        private List<V_ProjectCompany> GetProjectModel(List<V_ProjectCompany> ProList, ProjectCounter ProCounter)
        {
            //项目公司
            List<V_ProjectCompany> ProCompanyList = new List<V_ProjectCompany>();
            //项目公司指标
            List<V_ProjectTarget> _ProTargetLists = new List<V_ProjectTarget>();



            List<V_ProjectTarget> ProTargetListSum = new List<V_ProjectTarget>();
            //把所有区域的指标拿出来
            ProList.ForEach(p => ProTargetListSum.AddRange(p.ProjectTargets));

            /*
             * 这里的总计需要注意：因每个区域系统都是独立的指标，所以只能按照指标名称来比对
             * 所以在各个区域系统的指标名称一定要一样，不然无法统计
             */

            #region 总计的指标总和

            List<C_Target> _TargetList = StaticResource.Instance.TargetList[_SystemID].ToList();

            foreach (C_Target itemTarget in _TargetList.OrderBy(g => g.Sequence))
            {
                V_ProjectTarget VModel = new V_ProjectTarget();

                //从明细表中获取数据
                MonthlyReportDetail tempModel = new MonthlyReportDetail();

                VModel.ProMonthlyReportDetailID = Guid.Empty;

                VModel.ProTargetID = itemTarget.ID;
                VModel.ProTargetName = itemTarget.TargetName;
                VModel.ProTargetSequence = itemTarget.Sequence;
                VModel.ProCompayID = Guid.Empty;
                VModel.IsMissTarget = false;
                VModel.IsMissTargetCurrent = false;
                VModel.Counter = 0;

                //总计： 当月数据字段
                VModel.NPlanAmmount = ProTargetListSum.FindAll(f => f.ProTargetName == itemTarget.TargetName).Sum(s => s.NPlanAmmount); //总计：当月计划数 ,从区域小计上获取
                VModel.NActualAmmount = ProTargetListSum.FindAll(f => f.ProTargetName == itemTarget.TargetName).Sum(s => s.NActualAmmount);  //总计：当月实际数 ,从区域小计上获取
                if (VModel.NPlanAmmount == 0)
                {
                    VModel.NActualRate = ""; VModel.NDisplayRate = "/";
                }
                else
                {
                    VModel.NActualRate = "";
                    VModel.NDisplayRate = Math.Round((VModel.NActualAmmount / VModel.NPlanAmmount), 5, MidpointRounding.AwayFromZero).ToString("P1"); ;
                }

                //总计：当月累计计划数
                VModel.NAccumulativePlanAmmount = ProTargetListSum.FindAll(f => f.ProTargetName == itemTarget.TargetName).Sum(s => s.NAccumulativePlanAmmount);  // ?累计的指标怎么算？
                VModel.NAccumulativeActualAmmount = ProTargetListSum.FindAll(f => f.ProTargetName == itemTarget.TargetName).Sum(s => s.NAccumulativeActualAmmount);
                if (VModel.NAccumulativePlanAmmount == 0)
                {
                    VModel.NAccumulativeActualRate = "";
                    VModel.NAccumulativeDisplayRate = "/";
                }
                else
                {
                    VModel.NAccumulativeActualRate = "";
                    VModel.NAccumulativeDisplayRate = Math.Round((VModel.NAccumulativeActualAmmount / VModel.NAccumulativePlanAmmount), 5, MidpointRounding.AwayFromZero).ToString("P1"); ;
                }

                //总计：年度计划值
                VModel.NActualAmmountByYear = 0;
                VModel.NPlanAmmountByYear = ProTargetListSum.FindAll(f => f.ProTargetName == itemTarget.TargetName).Sum(s => s.NPlanAmmountByYear); ;
                if (VModel.NPlanAmmountByYear != 0)
                {
                    VModel.NDisplayRateByYear = Math.Round((VModel.NAccumulativeActualAmmount / VModel.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                }
                else
                {
                    VModel.NPlanAmmountByYear = 0;
                    VModel.NDisplayRateByYear = "/";
                }
                VModel.NActualRateByYear = "";

                _ProTargetLists.Add(VModel);
            }

            #endregion


            V_ProjectCompany tempProCompany = new V_ProjectCompany();
            tempProCompany.ProCompanySequence = -100;
            tempProCompany.SystemID = Guid.Empty;
            tempProCompany.ProCompayName = ProCounter.Title;
            tempProCompany.ProCompayID = Guid.Empty;
            tempProCompany.ProCompanyProperty1 = "";
            tempProCompany.FinYear = FinYear;
            tempProCompany.FinMonth = FinMonth;
            //tempProCompany.CompayModel = itemCompany;
            tempProCompany.ProRowSpan = 0;
            tempProCompany.ProCompanyNumber = 0;
            tempProCompany.ProjectTargets = _ProTargetLists;
            tempProCompany.ProDataType = "XML";
            ProCompanyList.Add(tempProCompany);

            return ProCompanyList;
        }


        #endregion







    }

}
