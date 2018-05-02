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


namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 更具获取系统，填充某个系统的补充项
    /// </summary>
    public class DefaultReportInstanceDetail : IReportInstanceDetail
    {

        string strMonthReportOrderType;
        string strCompanyPropertys = "";
        bool IncludeHaveDetail = true;

        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        List<C_Target> _Target = null;
        public List<DictionaryVmodel> GetDetailRptDataSource(ReportInstance RptModel, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
        {
            _System = RptModel._System;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            _Target = RptModel._Target;
            ReportDetails = RptModel.ReportDetails;
            this.IncludeHaveDetail = IncludeHaveDetail;
            strMonthReportOrderType = strOrderType;

            //这里strCompanyProperty参数，不仅是公司属性，同时也是判断：调用的来源(strCompanyProperty=="Reported"),表示是从上报页面调用，反之不是则是从查询页面调用的
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
                if (ReportDetails != null && ReportDetails.Count() > 0)
                {
                    _System = StaticResource.Instance[_System.ID, ReportDetails[0].CreateTime];
                    _Target = StaticResource.Instance.GetTargetList(_System.ID, ReportDetails[0].CreateTime).ToList();
                }
            }
            List<VTarget> lstVTarget = SplitCompleteTargetDetailXml(_System.Configuration);
            List<VItemCompanyProperty> lstVItemCompanyProperty = null;
            if (SpliteCompanyPropertyXml("List", _System.Configuration).Count > 0)
            {
                lstVItemCompanyProperty = SpliteCompanyPropertyXml("List", _System.Configuration)[0].listCP.ToList();
            }
            List<DictionaryVmodel> listvmodel = FromatData(lstVTarget, lstVItemCompanyProperty);
            if (lstVTarget.Where(m => m.IsBlendTarget == true).Any())
            {
                listvmodel = GetMergeComplateMonthReportDetail(listvmodel, _Target);
            }
            return listvmodel;
        }

        /// <summary>
        /// 拼装数据
        /// </summary>
        /// <param name="lstVTarget">模型</param>
        /// <param name="lstVCompanyProperty">公司类型</param>
        /// <returns>ViewModel</returns>
        private List<DictionaryVmodel> FromatData(List<VTarget> lstVTarget, List<VItemCompanyProperty> lstVCompanyProperty)
        {
            List<C_Company> listCompany = SearchCompanyData();//根据条件获取公司
            List<DictionaryVmodel> listDictionaryVModel = new List<DictionaryVmodel>();
            List<DictionaryVmodel> ItemCompanyPropertyViewModel = null;
            List<C_Target> listTempC_target = _Target;
            string strHtmlTemplate = string.Empty;
            //是否存在明细指标
            if (IncludeHaveDetail == false)
            {
                listTempC_target = _Target.Where(p => p.HaveDetail == true).ToList();
            }
            foreach (VTarget vtarget in lstVTarget)
            {
                List<MonthlyReportDetail> listMRD = new List<MonthlyReportDetail>();
                List<C_Target> listC_target = listTempC_target.Where(c => c.TargetName == vtarget.TargetName).ToList();
                if (listC_target.Count > 0)
                {
                    //为当前指标赋值
                    C_Target CurrentTarget = listC_target[0];
                    ItemCompanyPropertyViewModel = new List<DictionaryVmodel>();
                    //根据当前指标获取数据
                    List<MonthlyReportDetail> listm = ReportDetails.Where(p => p.TargetID == CurrentTarget.ID && (p.Display == true )).ToList();

                    //判断是否是明细项考核数据
                    if (CurrentTarget.HaveDetail == true && CurrentTarget.NeedEvaluation==true)
                    {
                        //判断当前指标是否存在分组，如果存在使用指标中的分组，否则使用系统的分组。
                        if (SpliteCompanyPropertyXml("List", CurrentTarget.Configuration).Count > 0)
                        {
                            lstVCompanyProperty = SpliteCompanyPropertyXml("List", CurrentTarget.Configuration)[0].listCP.ToList();
                        }
                        if (lstVCompanyProperty != null)
                        {
                            foreach (VItemCompanyProperty vcp in lstVCompanyProperty)
                            {
                                //根据分组筛选出公司
                                List<C_Company> lstCompany = listCompany.Where(p => p.CompanyProperty1 == vcp.ItemCompanyPropertyValue).ToList();
                                //根据筛选出的公司筛选数据
                                List<MonthlyReportDetail> listCompanyPertyMRD = SetMonthlyReportDetail(listm, lstCompany);
                                listMRD.AddRange(listCompanyPertyMRD);
                                //如果当前分组没有数据，则跳出当前循环。
                                if (listCompanyPertyMRD.Count == 0)
                                {
                                    continue;
                                }
                                ItemCompanyPropertyViewModel.Add(new DictionaryVmodel(vcp.ItemCompanyPropertyName, EditData(listCompanyPertyMRD, vtarget, listC_target[0], listCompany, vcp), "CompanyProperty"));
                               
                            }
                            //把当前分组制空，以便于下个指标分组
                            if (SpliteCompanyPropertyXml("List", CurrentTarget.Configuration).Count > 0)
                            {
                                lstVCompanyProperty = null;
                            }
                        }
                        else
                        {
                            //根据公司筛选数据
                            listMRD = SetMonthlyReportDetail(listm, listCompany);
                            ItemCompanyPropertyViewModel.Add(new DictionaryVmodel("", EditData(listMRD, vtarget, listC_target[0], listCompany, null), "CompanyProperty"));
                        }
                    }
                    else
                    {
                        //根据公司筛选数据
                        listMRD = SetMonthlyReportDetail(listm, listCompany);
                        listMRD = SequenceEngine.SequenceService.GetSequence(_System.ID, strMonthReportOrderType, listMRD, null); //strMonthReportOrderType这是个参数前台传递过来

                        ItemCompanyPropertyViewModel.Add(new DictionaryVmodel("HaveDetail", listMRD, "Counter"));
                    }
                    ItemCompanyPropertyViewModel.Add(new DictionaryVmodel("SummaryData", GetSummaryDate(listMRD, listC_target[0]), "Counter"));

                    #region 判断当前指标是否存在自己的表头、Tmpl模板、Excle模板
                    if (CurrentTarget.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
                    {
                        strHtmlTemplate = GetComplateMonthReportDetailHtmlTemplate(CurrentTarget.Configuration);
                    }
                    else
                    {
                        //如果当前指标不存在表头、Tmpl模板、Excle模板，则使用系统的。
                        if (_System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
                        {
                            strHtmlTemplate = GetComplateMonthReportDetailHtmlTemplate(_System.Configuration);
                        }
                    }
                    listDictionaryVModel.Add(new DictionaryVmodel(vtarget.TargetName, ItemCompanyPropertyViewModel, "Target", strHtmlTemplate, 0, vtarget.Senquence));
                    #endregion
                }
            }
            return listDictionaryVModel;
        }
        /// <summary>
        /// 计算合计
        /// </summary>
        /// <returns></returns>
        private List<B_MonthlyReportDetail> GetSummaryDate(List<MonthlyReportDetail> listMRD, C_Target CTarget)
        {
            List<B_MonthlyReportDetail> listSummaryMRD = new List<B_MonthlyReportDetail>();
            B_MonthlyReportDetail bmrd = new B_MonthlyReportDetail();
            bmrd.ID = Guid.NewGuid();
            bmrd.SystemID = _System.ID;
            bmrd.FinYear = FinYear;
            bmrd.FinMonth = FinMonth;
            bmrd.TargetID = CTarget.ID;
            if (listMRD.Count > 0)
            {
                bmrd.NPlanAmmount = listMRD.Sum(p => p.NPlanAmmount);
                bmrd.NActualAmmount = listMRD.Sum(p => p.NActualAmmount);
                bmrd.NDifference = listMRD.Sum(p => p.NDifference);
                bmrd.NAccumulativePlanAmmount = listMRD.Sum(p => p.NAccumulativePlanAmmount);
                bmrd.NAccumulativeActualAmmount = listMRD.Sum(p => p.NAccumulativeActualAmmount);
                bmrd.NAccumulativeDifference = listMRD.Sum(p => p.NAccumulativeDifference);
                bmrd.NPlanAmmountByYear = listMRD.Sum(p => p.NPlanAmmountByYear);
                if (bmrd.NPlanAmmountByYear != 0)
                {
                    bmrd.NDisplayRateByYear = Math.Round((bmrd.NAccumulativeActualAmmount / bmrd.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                }
                else
                {
                    bmrd.NDisplayRateByYear = "--";
                }

            }
            bmrd = TargetEvaluationEngine.TargetEvaluationService.Calculation(bmrd, false);
            listSummaryMRD.Add(bmrd);
            return listSummaryMRD;
        }

        /// <summary>
        /// 数据筛选
        /// </summary>
        /// <param name="strCP">公司类型Item</param>
        /// <param name="listMRDetail">数据</param>
        /// <param name="vt">指标项</param>
        /// <returns></returns>

        private List<DictionaryVmodel> EditData(List<MonthlyReportDetail> listMRD, VTarget vt, C_Target CTarget, List<C_Company> listCompany, VItemCompanyProperty vcp)
        {
            #region 声明变量
            List<DictionaryVmodel> lstDVM = new List<DictionaryVmodel>();
            List<MonthlyReportDetail> VCounterListMonthlyReportDetailViewModel = null;
            ExpressionParser _parser = null;
            int rowSpanCount = 0;
            B_MonthlyReportDetail bmrd = null;
            List<VCounter> listVCounter = null;
            #endregion

            #region 数据判断与拼装
            //判断当前指标是否存在模板，如果存在使用指标模板，如果不存在使用当前系统的模板。
            if (CTarget.Configuration.Elements("ComplateTargetDetail").Elements("Target").ToList().Count > 0)
            {
                listVCounter = SplitCompleteTargetDetailXml(CTarget.Configuration)[0].CounterList;
            }
            else
            {
                listVCounter = vt.CounterList;
            }

            if (vcp != null)
            {
                if (vcp.IsHideCounter)
                {
                    List<VCounter> lstVC = new List<VCounter>();
                    VCounter vc = new VCounter();
                    vc.Expression = "";
                    vc.TextExpression = "";
                    string senquence = "0";
                    int t = 10000;
                    int.TryParse(senquence, out t);
                    vc.Senquence = t;
                    vc.Title = vcp.ItemCompanyPropertyName;
                    vc.Display = "true";
                    vc.HaveDetail = "false";
                    vc.DetailMonthlyExpression = "1==1";
                    vc.DetailExpression = "1==1";
                    vc.DetailExpression = "";
                    lstVC.Add(vc);
                    listVCounter = lstVC;
                }
            }

            for (int i = 0; listVCounter.Count > i; i++)
            {
                DictionaryVmodel dv = dv = new DictionaryVmodel();
                VCounterListMonthlyReportDetailViewModel = new List<MonthlyReportDetail>();
                foreach (MonthlyReportDetail mrd in listMRD)
                {
                    
                    Hashtable bizContext = BizContextEngine.BizContextService.GetBizContext(listMRD.FindAll(p => p.CompanyID == mrd.CompanyID), "MonthlyReportDetail");
                    _parser = new ExpressionParser(bizContext);
                    //区分月累计算式与年累计算式。
                    string Expression = strMonthReportOrderType == "DetailMonthly" ? listVCounter.ToList()[i].DetailMonthlyExpression : listVCounter.ToList()[i].DetailExpression;
                    if (_parser.CacluateCondition(Expression))
                    {
                        VCounterListMonthlyReportDetailViewModel.Add(mrd);
                    }
                }
                //明细项数据排序
                VCounterListMonthlyReportDetailViewModel = SequenceEngine.SequenceService.GetSequence(_System.ID, strMonthReportOrderType, VCounterListMonthlyReportDetailViewModel);
                dv.Name = listVCounter.ToList()[i].Title;

                //判断是否隐藏Counter明细项中数据
                if (listVCounter[i].Display.ToLower() == "true")
                {
                    dv.Mark = "DetailShow";
                    //计算隐藏的行数
                    rowSpanCount = rowSpanCount + VCounterListMonthlyReportDetailViewModel.Count;
                }
                else
                {

                    dv.Mark = "DetailHide";
                }
                //判断Counter明细项中是否存在该数据。
                if (listVCounter[i].HaveDetail.ToLower() == "false")
                {
                    dv.Mark = "DetailDelete";
                }
                //判断是否存在公司属性
                if (vcp != null && i == 0)
                {
                    if (!vcp.IsHideCounter)
                    {
                        dv.Value = vcp.ItemCompanyPropertyName;
                        dv.RowSpanCount = 0;
                    }
                }


                #region 计算明细项项合计和小计

                if (listVCounter.ToList()[i].Title != "小计" || listVCounter.ToList()[i].IsSummaryDetail.ToLower()=="true")
                {
                    bmrd = SummaryData(VCounterListMonthlyReportDetailViewModel, bmrd, CTarget);
                }
                else
                {
                    bmrd = SummaryData(listMRD, bmrd, CTarget);
                }
                #endregion

                //调用计算完成率的方法
                bmrd = TargetEvaluationEngine.TargetEvaluationService.Calculation(bmrd, false);
                dv.BMonthReportDetail = bmrd;
                dv.ObjValue = VCounterListMonthlyReportDetailViewModel;
                lstDVM.Add(dv);
            }
            //计算页面要通行数
            if (vcp != null && lstDVM.Count()>0)
            {
                lstDVM[0].RowSpanCount = rowSpanCount + listVCounter.ToList().Count;
            }
            #endregion

            return lstDVM;
        }
        
        /// <summary>
        /// 计算明细项项合计和小计
        /// </summary>
        /// <param name="listData"></param>
        /// <param name="bmrd"></param>
        /// <param name="CTarget"></param>
        /// <returns></returns>
        private B_MonthlyReportDetail SummaryData(List<MonthlyReportDetail> listData, B_MonthlyReportDetail bmrd, C_Target CTarget)
        {
            //特殊处理差额，针对指标
            XElement element = null;
            element = CTarget.Configuration;
            XElement subElement = null;

            bool IsDifferenceException = false;

            if (element.Elements("IsDifferenceExceptionTarget").ToList().Count>0)
            {
                subElement = element.Elements("IsDifferenceExceptionTarget").ToList()[0];
                IsDifferenceException = subElement.GetAttributeValue("value", false);
            }

            bmrd = new B_MonthlyReportDetail();
            bmrd.ID = Guid.NewGuid();
            bmrd.SystemID = _System.ID;
            bmrd.FinYear = FinYear;
            bmrd.FinMonth = FinMonth;
            bmrd.TargetID = CTarget.ID;
            bmrd.NPlanAmmount = listData.Sum(p => p.NPlanAmmount);
            bmrd.NActualAmmount = listData.Sum(p => p.NActualAmmount);
            bmrd.NDifference = listData.Sum(p => p.NDifference);
            bmrd.NAccumulativePlanAmmount = listData.Sum(p => p.NAccumulativePlanAmmount);
            bmrd.NAccumulativeActualAmmount = listData.Sum(p => p.NAccumulativeActualAmmount);
            bmrd.NAccumulativeDifference = listData.Sum(p => p.NAccumulativeDifference);
            bmrd.NPlanAmmountByYear = listData.Sum(p => p.NPlanAmmountByYear);
            if (bmrd.NPlanAmmountByYear != 0)
            {
                bmrd.NDisplayRateByYear = Math.Round((bmrd.NAccumulativeActualAmmount / bmrd.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
            }
            else
            {
                bmrd.NDisplayRateByYear = "--";
            }
            if (IsDifferenceException) //异常
            {
              List<MonthlyReportDetail> aa = listData.Where(p => p.NAccumulativeDifference < 0).ToList();
                
                //差额异常指标
              bmrd.OAccumulativeDifference = bmrd.NAccumulativeDifference = aa.Sum(p => p.NAccumulativeDifference);

              bmrd.ODifference =bmrd.NDifference = listData.Where(p => p.NDifference <= 0).Sum(p => p.NDifference);
            }



            return bmrd;
        }
        /// <summary>
        ///根据公司ID找到完成情况明细
        /// </summary>
        /// <param name="listm"></param>
        /// <param name="listCompany"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> SetMonthlyReportDetail(List<MonthlyReportDetail> listm, List<C_Company> listCompany)
        {

            listm = listm.Where(x => listCompany.Exists(c => c.ID == x.CompanyID) ).ToList();

            listm.ForEach(F =>
            {
                F.Company = listCompany.Where(c => c.ID == F.CompanyID).FirstOrDefault();
            });
            
            return listm;
        }
        /// <summary>
        /// 根据查询条件找到公司
        /// </summary>
        /// <returns></returns>
        private List<C_Company> SearchCompanyData()
        {
            List<C_Company> listCompany = StaticResource.Instance.CompanyList[_System.ID].ToList();
            string[] strs = strCompanyPropertys.Split(';');
            for (int i = 0; i < strs.Count(); i++)
            {
                if (!string.IsNullOrEmpty(strs[i]))
                {
                    string[] tempStrs = strs[i].Split(':');
                    if (!string.IsNullOrEmpty(tempStrs[1]))
                    {
                        switch (tempStrs[0])
                        {
                            case "CompanyProperty1":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty1.Trim()) || p.CompanyProperty1 == null).ToList();
                                break;
                            case "CompanyProperty2":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty2.Trim()) || p.CompanyProperty2 == null).ToList();
                                break;
                            case "CompanyProperty3":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty3.Trim()) || p.CompanyProperty3 == null).ToList();
                                break;
                            case "CompanyProperty4":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty4.Trim()) || p.CompanyProperty4 == null).ToList();
                                break;
                            case "CompanyProperty5":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty5.Trim()) || p.CompanyProperty5 == null).ToList();
                                break;
                            case "CompanyProperty6":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty6.Trim()) || p.CompanyProperty6 == null).ToList();
                                break;
                            case "CompanyProperty7":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty7.Trim())||p.CompanyProperty7==null).ToList();
                                break;
                        }
                    }

                }
            }
            return listCompany;
        }
        /// <summary>
        /// 读取xml文件（ComplateTargetDetail.xml）
        /// </summary>
        /// <returns>完成情况明细模板</returns>
        private List<VTarget> SplitCompleteTargetDetailXml(XElement xelement)
        {
            List<VTarget> targetList = new List<VTarget>();

            XElement elementCTD = xelement;
            if (elementCTD.Elements("ComplateTargetDetail").Elements("BlendTargets").Count()>0)
            {
                //完成情况明细模板
                List<XElement> Targets = elementCTD.Elements("ComplateTargetDetail").Elements("BlendTargets").Elements("Target").ToList();
                VTarget vt = null;
                foreach (XElement target in Targets)
                {
                    vt = new VTarget(target);
                    targetList.Add(vt);
                }

            }
            if (elementCTD.Elements("ComplateTargetDetail").Elements("Target").Count() > 0)
            {
                //完成情况明细模板
                List<XElement> Targets = elementCTD.Elements("ComplateTargetDetail").Elements("Target").ToList();
                VTarget vt = null;
                foreach (XElement target in Targets)
                {
                    vt = new VTarget(target);
                    targetList.Add(vt);
                }

            }
            return targetList;
        }
        /// <summary>
        /// 读取xml文件（CompanyProperty.xml）
        /// </summary>
        /// <param name="DisplayType">显示类型（如Search为查询条件，list为列表分组）</param>
        /// <returns>公司属性</returns>
        private List<VCompanyProperty> SpliteCompanyPropertyXml(string DisplayType, XElement elementCP)
        {
            List<VCompanyProperty> listVCP = new List<VCompanyProperty>();
            //强转字符串类型
            if (elementCP.Elements("ListCompanyProperty").Elements("CompanyProperty") != null)
            {
                //公司属性
                List<XElement> ListCP = elementCP.Elements("ListCompanyProperty").Elements("CompanyProperty").ToList();
                VCompanyProperty VCP = null;
                foreach (XElement cp in ListCP)
                {
                    VCP = new VCompanyProperty(cp);
                    listVCP.Add(VCP);
                }
            }
            List<VCompanyProperty> returnListVCP = null;
            if (DisplayType == "List")
            {
                returnListVCP = listVCP.Where(p => p.CompanyPropertyDisplay == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
            }
            else
            {
                returnListVCP = listVCP.Where(p => p.CompanyPropertySearchGroup == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
            }
            return returnListVCP;
        }

        /// <summary>
        /// 获取表头名称和tmpl名称
        /// </summary>
        /// <param name="element">XML</param>
        /// <returns>月度经营报告表头、Tmpl、模板、Execl模板</returns>
        public string GetComplateMonthReportDetailHtmlTemplate(XElement element)
        {
            string strValue = "";
            if (element != null)
            {
                if (element.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("RargetReportTableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "") + ",";
                }
                if (element.Elements("ReportMonthlyDetail").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("ReportMonthlyDetail").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("ReportMonthlyDetailTemplate", "") + ",";
                }
            }
            return strValue;
        }
        /// <summary>
        /// 获取计划数
        /// </summary>
        /// <returns></returns>
        public List<DictionaryVmodel> GetTagetPlanViewModel()
        {

            //获取计划数据
            List<A_TargetPlanDetail> listTargetPlan = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear, FinMonth);
            //得到当前系统的所有公司
            List<C_Company> listComPany = StaticResource.Instance.CompanyList[_System.ID];

            List<DictionaryVmodel> listDV = new List<DictionaryVmodel>();
            List<TargetPlanViewModel> listTargetPlanViewModel = null;
            TargetPlanViewModel tpvm = null;
            foreach (C_Target Ctarget in _Target.Where(p=>p.NeedReport==true))
            {
                listTargetPlanViewModel = new List<TargetPlanViewModel>();
                //根据指标得到此指标下的计划数
                List<A_TargetPlanDetail> listTP = listTargetPlan.Where(p => p.TargetID == Ctarget.ID).ToList();
                foreach (C_Company company in listComPany)
                {
                    //判断计划指标分解表中是否存在当前公司
                    if (listTP.Where(p => p.CompanyID == company.ID).Count() > 0)
                    {
                        A_TargetPlanDetail btp = listTP.Where(p => p.CompanyID == company.ID).ToList().FirstOrDefault();
                        tpvm = new TargetPlanViewModel();
                        tpvm.ID = company.ID;
                        tpvm.CompanyName = company.CompanyName;
                        tpvm.NPlanAmmount = btp.Target;
                        listTargetPlanViewModel.Add(tpvm);
                    }
                }
                string strHtmlTemplate = GetComplateMonthReportDetailHtmlTemplate(_System.Configuration);
                listDV.Add(new DictionaryVmodel(Ctarget.TargetName, listTargetPlanViewModel, "Target", strHtmlTemplate));

            }
            return listDV;
        }
        /// <summary>
        ///  合并完成情况明细（混合指标）
        /// </summary>
        /// <param name="dvCurrent">已计算的单项指标集合</param>
        /// <returns></returns>
        public List<DictionaryVmodel> GetMergeComplateMonthReportDetail(List<DictionaryVmodel> dvCurrentList,List<C_Target> _target)
        {
            List<DictionaryVmodel> dvResult = new List<DictionaryVmodel>();

            var completeBlendTargetDetailXml = SplitCompleteBlendTargetDetailXml(_System.Configuration);
            //遍历完成情况明细中包含几个混合指标（目前需求一个版块只允许有一个）
            //合并混合指标为Object            
            List<DictionaryVmodel> dvMergeList = new List<DictionaryVmodel>();

            DictionaryVmodel dv = new DictionaryVmodel();
            dv.Name = completeBlendTargetDetailXml[0].TargetName;
            dv.IsBlendTarget = true;
            dv.Mark = "Target";
            dv.Senquence = completeBlendTargetDetailXml[0].Senquence;
            //获取混合指标名称集合
            var blendtargetArray = completeBlendTargetDetailXml[0].VTargetList.Select(m => m.TargetName).ToArray();
            //混去混合指标数据集合
            var blendTargetList = dvCurrentList.Where(m => blendtargetArray.Contains(m.Name)).ToList();
            dv.HtmlTemplate = blendTargetList[0].HtmlTemplate;
            List<DictionaryVmodel> dvlist = (List<DictionaryVmodel>)blendTargetList[0].ObjValue;
            List<DictionaryVmodel> dvlist2 = (List<DictionaryVmodel>)blendTargetList[1].ObjValue;
            if (dvlist.Count > 0)
            {
                //获取CompanyProperty
                var companyPropertyList = (List<DictionaryVmodel>)dvlist.Where(m => m.Mark == "CompanyProperty").FirstOrDefault().ObjValue;
                var companyPropertyList2 = (List<DictionaryVmodel>)dvlist2.Where(m => m.Mark == "CompanyProperty").FirstOrDefault().ObjValue;

                //完成部分
                List<MonthlyReportDetail> brdComplete1 = (List<MonthlyReportDetail>)companyPropertyList.Where(m => m.Mark == "DetailHide").FirstOrDefault().ObjValue;
                List<MonthlyReportDetail> brdComplete2 = (List<MonthlyReportDetail>)companyPropertyList2.Where(m => m.Mark == "DetailHide").FirstOrDefault().ObjValue;

                //未完成部分
                List<MonthlyReportDetail> brdNoComplete1 = (List<MonthlyReportDetail>)companyPropertyList.Where(m => m.Mark == "DetailShow").FirstOrDefault().ObjValue;
                List<MonthlyReportDetail> brdNoComplete2 = (List<MonthlyReportDetail>)companyPropertyList2.Where(m => m.Mark == "DetailShow").FirstOrDefault().ObjValue;

                //重组两个指标中的完成与未完成，如果同一公司下其中一个指标未完成，那么把另一个指标中已完成指标的公司也移动到未完成中
                foreach (var item in brdNoComplete1)
                {
                    //说明当前未完成指标的公司在另一个指标中是完成的；移除另一个指标完成，加入到未完成中
                    if (!brdNoComplete2.Where(m => m.CompanyID == item.CompanyID).Any())
                    {
                        var mrd = brdComplete2.Where(m => m.CompanyID == item.CompanyID).ToList();
                        if (mrd.Any())
                        {
                            brdComplete2.Remove(mrd.FirstOrDefault());
                            brdNoComplete2.Add(mrd.FirstOrDefault());
                        }
                    }
                }
                foreach (var item in brdNoComplete2)
                {
                    //说明当前未完成指标的公司在另一个指标中是完成的；移除另一个指标完成，加入到未完成中
                    if (!brdNoComplete1.Where(m => m.CompanyID == item.CompanyID).Any())
                    {
                        var mrd = brdComplete1.Where(m => m.CompanyID == item.CompanyID).ToList();
                        if (mrd.Any())
                        {
                            brdComplete1.Remove(mrd.FirstOrDefault());
                            brdNoComplete1.Add(mrd.FirstOrDefault());
                        }
                    }
                }
                //重新按照项目进行排序，保证前端用下标获取时数据能对应

                //完成部分
                companyPropertyList.Where(m => m.Mark == "DetailHide").FirstOrDefault().ObjValue = brdComplete1.OrderBy(m => m.Company.Sequence).ThenBy(m => m.TargetName).ToList();
                companyPropertyList2.Where(m => m.Mark == "DetailHide").FirstOrDefault().ObjValue = brdComplete2.OrderBy(m => m.Company.Sequence).ThenBy(m => m.TargetName).ToList();
                //未完成部分
                companyPropertyList.Where(m => m.Mark == "DetailShow").FirstOrDefault().ObjValue = brdNoComplete1.OrderBy(m => m.Company.Sequence).ThenBy(m => m.TargetName).ToList();
                companyPropertyList2.Where(m => m.Mark == "DetailShow").FirstOrDefault().ObjValue = brdNoComplete2.OrderBy(m => m.Company.Sequence).ThenBy(m => m.TargetName).ToList();

                //重算小计汇总信息
                //完成部分
                B_MonthlyReportDetail bmrd = new B_MonthlyReportDetail();
                companyPropertyList.Where(m => m.Mark == "DetailHide").FirstOrDefault().BMonthReportDetail = TargetEvaluationEngine.TargetEvaluationService.Calculation(SummaryData(brdComplete1, bmrd, _target.Where(m => m.TargetName == blendTargetList[0].Name).FirstOrDefault()), false);
                companyPropertyList2.Where(m => m.Mark == "DetailHide").FirstOrDefault().BMonthReportDetail = TargetEvaluationEngine.TargetEvaluationService.Calculation(SummaryData(brdComplete2, bmrd, _target.Where(m => m.TargetName == blendTargetList[1].Name).FirstOrDefault()), false);
                //未完成部分
                companyPropertyList.Where(m => m.Mark == "DetailShow").FirstOrDefault().BMonthReportDetail = TargetEvaluationEngine.TargetEvaluationService.Calculation(SummaryData(brdNoComplete1, bmrd, _target.Where(m => m.TargetName == blendTargetList[0].Name).FirstOrDefault()), false);
                companyPropertyList2.Where(m => m.Mark == "DetailShow").FirstOrDefault().BMonthReportDetail = TargetEvaluationEngine.TargetEvaluationService.Calculation(SummaryData(brdNoComplete2, bmrd, _target.Where(m => m.TargetName == blendTargetList[1].Name).FirstOrDefault()), false);
            }

            dv.ObjValue = blendTargetList;
            dvResult.Add(dv);
            //获取单指标数据集合
            dvResult.AddRange(dvCurrentList.Where(m => !blendtargetArray.Contains(m.Name)).ToList());
            //重新排序
            return dvResult.OrderBy(m => m.Senquence).ToList();
        }
        /// <summary>
        /// 读取xml文件（ComplateTargetDetail.xml）
        /// </summary>
        /// <returns>完成情况明细模板-混合指标</returns>
        private List<VBlendTarget> SplitCompleteBlendTargetDetailXml(XElement xelement)
        {
            List<VBlendTarget> targetList = new List<VBlendTarget>();

            XElement elementCTD = xelement;

            if (elementCTD.Elements("ComplateTargetDetail").Elements("BlendTargets").Count() > 0)
            {
                //完成情况明细模板
                List<XElement> Targets = elementCTD.Elements("ComplateTargetDetail").Elements("BlendTargets").ToList();
                VBlendTarget vt = null;
                foreach (XElement target in Targets)
                {
                    vt = new VBlendTarget(target);
                    targetList.Add(vt);
                }
            }
            return targetList;
        }

    }

    public class ReportInstanceGroupDetail : IReportInstanceDetail
    {
        int FinYear = 0;
        int FinMonth = 0;
        C_System _System = null;
        List<C_Target> _Target = null;
        string strCompanyPropertys = string.Empty;
        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        public List<DictionaryVmodel> GetDetailRptDataSource(ReportInstance RptModel, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
        {
            List<DictionaryVmodel> lstDV = new List<DictionaryVmodel>();
            _System = RptModel._System;
            FinMonth = RptModel.FinMonth;
            FinYear = RptModel.FinYear;
            _Target = RptModel._Target;

            ReportDetails = RptModel.ReportDetails;

            //这里strCompanyProperty参数，不仅是公司属性，同时也是判断：调用的来源(strCompanyProperty=="Reported"),表示是从上报页面调用，反之不是则是从查询页面调用的
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


            if (!string.IsNullOrEmpty(strOrderType))
            {
                FormatDownData(strOrderType);
            }

            lstDV.Add(new DictionaryVmodel("Group", FormatData()));
            
            return lstDV;
        }

        /// <summary>
        /// 格式化数据，以便于在前台显示。
        /// </summary>
        /// <returns>字典对象</returns>
        public List<GroupDictionaryVmodel> FormatData()
        {
            List<GroupDictionaryVmodel> lstDV = new List<GroupDictionaryVmodel>();
            List<C_Company> lstCompany = StaticResource.Instance.CompanyList[_System.ID];
            //获取集团明细XML
            List<V_GroupTargetXElement> lstVGroupTargetXElement = SplitGroupTargetXml(_System.Configuration);
            //筛选非明细指标
            List<C_Target> lstHaveNotDetailTarget = _Target.Where(p => p.HaveDetail == false&&p.VersionEnd>=DateTime.Now&&p.VersionStart<DateTime.Now).OrderBy(p => p.Sequence).ToList();
            //获取非明细数据
            List<MonthlyReportDetail> lstHaveNotMRD = SearchData(false, null);

            List<MonthlyReportDetail> lstSummaryAllData = new List<MonthlyReportDetail>();
            //循环非明细指标
            for (int i = 0; i < lstHaveNotDetailTarget.Count(); i++)
            {
                List<MonthlyReportDetail> lstVGMRD = null;
                //根据集团明细XML得到有明细数据的明细指标

                V_GroupTargetXElement HaveDetailTarget = lstVGroupTargetXElement.Where(p => p.TargetValue.ToGuid() == lstHaveNotDetailTarget[i].ID).FirstOrDefault();
                bool GroupDetail = false;
                if (HaveDetailTarget == null)
                {
                    if (i == 0)
                        GroupDetail = true;
                }
                else
                {
                    if (HaveDetailTarget.GroupDetail)
                        GroupDetail = true;
                }
                if (GroupDetail)
                {
                    //获取明细数据
                    List<MonthlyReportDetail> lstMRD = SearchData(true, null);

                    //拼装明细数据
                    List<V_GroupCompany> lstVGC = FiltrateDataDetail(lstCompany, lstMRD, _Target.Where(p => p.HaveDetail == true && p.VersionEnd >= DateTime.Now && p.VersionStart < DateTime.Now).ToList());

                    //拼装非明细数据
                    lstVGMRD = FiltrateDataSummary(null, _Target.Where(p => p.HaveDetail == true && p.VersionEnd >= DateTime.Now && p.VersionStart < DateTime.Now).ToList(), null, true);

                    //清除用于计算当前非明细指标的数据
                    lstSummaryCompanyTargetReportDetail.Clear();

                    lstDV.Add(new GroupDictionaryVmodel(lstHaveNotDetailTarget[i].ID,lstHaveNotDetailTarget[i].TargetName, lstVGMRD, lstVGC, "", ""));
                }
                else
                {
                    //拼装非明细数据
                    lstVGMRD = FiltrateDataSummary(lstHaveNotMRD.Where(p => p.TargetID == lstHaveNotDetailTarget[i].ID).FirstOrDefault(), _Target.Where(p => p.HaveDetail == true).ToList(), null, false);

                    lstDV.Add(new GroupDictionaryVmodel(lstHaveNotDetailTarget[i].ID, lstHaveNotDetailTarget[i].TargetName, lstVGMRD, null, "", ""));
                }
                lstSummaryAllData.AddRange(lstVGMRD);
            }
            List<MonthlyReportDetail> temp = FiltrateDataSummary(null, _Target.Where(p => p.HaveDetail == true && p.VersionEnd >= DateTime.Now && p.VersionStart < DateTime.Now).ToList(), lstSummaryAllData, true);
            lstDV.Add(new GroupDictionaryVmodel(Guid.Empty,"合计", temp, null, "", ""));
            return lstDV;
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="HaveDetail">是否为明细数据</param>
        /// <returns>返回数据集</returns>
        public List<MonthlyReportDetail> SearchData(bool HaveDetail, List<MonthlyReportDetail> lstMRD)
        {
            List<MonthlyReportDetail> listMrd = new List<MonthlyReportDetail>();
            if (lstMRD != null)
            {
                listMrd = lstMRD;
            }
            else
            {
                listMrd = ReportDetails;
            }
            var jquery = from target in _Target
                         join mrd in listMrd on target.ID equals mrd.TargetID
                         where target.HaveDetail == HaveDetail
                         select mrd;
            listMrd = jquery.ToList<MonthlyReportDetail>();
            return listMrd;
        }

        /// <summary>
        /// //拼装非明细指标数据
        /// </summary>
        /// <param name="ReportDetail">非明细指标数据</param>
        /// <param name="lstTarget">明细指标</param>
        /// <param name="lstTarget">所有数据（包括明细指标的合计和费明细指标数据）</param>
        /// <param name="HaveDetail">是否包括明细合计数据</param>
        /// <returns>返回V_GroupMonthlyReportDetail集合</returns>
        public List<MonthlyReportDetail> FiltrateDataSummary(MonthlyReportDetail ReportDetail, List<C_Target> lstTarget, List<MonthlyReportDetail> lstSummaryAllData, bool HaveDetail)
        {
            List<MonthlyReportDetail> lstMRD = new List<MonthlyReportDetail>();

            List<C_Target> lstHaveNotDetailTarget = _Target.Where(p => p.HaveDetail == false && p.VersionEnd >= DateTime.Now && p.VersionStart < DateTime.Now).OrderBy(p => p.Sequence).ToList();


            MonthlyReportDetail mrd = new MonthlyReportDetail();
            mrd.TargetID = lstHaveNotDetailTarget[0].ID;
            mrd.SystemID = _System.ID;
            mrd.FinYear = FinYear;
            mrd.FinMonth = FinMonth;
            if (lstSummaryAllData == null)
            {
                #region 用于汇总明细指标各个指标数据
                B_MonthlyReportDetail mrd_target = null;
                foreach (C_Target CurrentTarget in lstTarget.OrderBy(p => p.Sequence))
                {
                    List<MonthlyReportDetail> lstSummaryTargetReportDetail = lstSummaryCompanyTargetReportDetail.Where(p => p.TargetID == CurrentTarget.ID).ToList();
                    mrd_target = new B_MonthlyReportDetail();
                    mrd_target.ID = Guid.NewGuid();
                    mrd_target.TargetID = CurrentTarget.ID;
                    mrd_target.SystemID = _System.ID;
                    mrd_target.FinYear = FinYear;
                    mrd_target.FinMonth = FinMonth;
                    if (lstSummaryTargetReportDetail.Count > 0)
                    {
                        mrd_target.NPlanAmmount = lstSummaryTargetReportDetail.Sum(p => p.NPlanAmmount);
                        mrd_target.NActualAmmount = lstSummaryTargetReportDetail.Sum(p => p.NActualAmmount);
                        mrd_target.NAccumulativePlanAmmount = lstSummaryTargetReportDetail.Sum(p => p.NAccumulativePlanAmmount);
                        mrd_target.NAccumulativeActualAmmount = lstSummaryTargetReportDetail.Sum(p => p.NAccumulativeActualAmmount);
                    }
                    mrd_target = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(mrd_target, "");
                    lstMRD.Add(mrd_target.ToVModel());
                }
                #endregion


            }
            else
            {
                #region 汇总所有的数据（合计）
                B_MonthlyReportDetail mrd_ALLData = null;
                foreach (C_Target CurrentTarget in lstTarget.OrderBy(p => p.Sequence))
                {
                    List<MonthlyReportDetail> lstSummaryTargetAllData = lstSummaryAllData.Where(p => p.TargetID == CurrentTarget.ID).ToList();
                    mrd_ALLData = new B_MonthlyReportDetail();
                    mrd_ALLData.ID = Guid.NewGuid();
                    mrd_ALLData.TargetID = CurrentTarget.ID;
                    mrd_ALLData.SystemID = _System.ID;
                    mrd_ALLData.FinYear = FinYear;
                    mrd_ALLData.FinMonth = FinMonth;
                    mrd_ALLData.NPlanAmmount = lstSummaryTargetAllData.Sum(p => p.NPlanAmmount);
                    mrd_ALLData.NActualAmmount = lstSummaryTargetAllData.Sum(p => p.NActualAmmount);
                    mrd_ALLData.NAccumulativePlanAmmount = lstSummaryTargetAllData.Sum(p => p.NAccumulativePlanAmmount);
                    mrd_ALLData.NAccumulativeActualAmmount = lstSummaryTargetAllData.Sum(p => p.NAccumulativeActualAmmount);

                    mrd_ALLData = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(mrd_ALLData, "");
                    lstMRD.Add(mrd_ALLData.ToVModel());
                }
                #endregion
            }

            if (HaveDetail)
            {
                #region 用于汇总明细指标(合计)
                List<MonthlyReportDetail> lstSAD = null;
                if (lstSummaryAllData != null)
                {
                    lstSAD = lstSummaryAllData.Where(p => p.TargetID == Guid.Empty).ToList();
                    lstSAD.AddRange(SearchData(false, lstSummaryAllData));
                }
                List<MonthlyReportDetail> tempData = lstSummaryAllData != null ? lstSAD : lstMRD;
                mrd.NPlanAmmount = tempData.Sum(p => p.NPlanAmmount);
                mrd.NActualAmmount = tempData.Sum(p => p.NActualAmmount);
                mrd.NAccumulativePlanAmmount = tempData.Sum(p => p.NAccumulativePlanAmmount);
                mrd.NAccumulativeActualAmmount = tempData.Sum(p => p.NAccumulativeActualAmmount);
                //mrd.TargetID =Guid.Parse("0904D131-8EA4-450D-83B3-18FE09C3BFFD");
                mrd = ((B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(mrd.ToBModel(), "")).ToVModel();
                #endregion
            }
            else
            {
                #region 非明细指标数据
                if (ReportDetail != null)
                {
                    mrd = ReportDetail;
                    mrd.NPlanAmmount = ReportDetail.NPlanAmmount;
                    mrd = ((B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(mrd.ToBModel(), "")).ToVModel();
                }
                #endregion
            }
            //mrd = TargetEvaluationEngine.TargetEvaluationService.Calculation(mrd, false);
            lstMRD.Add(mrd);
            return lstMRD;
        }

        List<MonthlyReportDetail> lstSummaryCompanyTargetReportDetail = new List<MonthlyReportDetail>();
        /// <summary>
        /// 筛选明细数据
        /// </summary>
        /// <returns></returns>
        public List<V_GroupCompany> FiltrateDataDetail(List<C_Company> lstCompany, List<MonthlyReportDetail> lstReportDetail, List<C_Target> lstTarget)
        {

            List<V_GroupCompany> lstVGC = new List<V_GroupCompany>();
            V_GroupCompany vgc = null;
            
            foreach (C_Company company in StaticResource.Instance.CompanyList[_System.ID])
            {
                #region 初始化当前公司对象
                vgc = new V_GroupCompany();
                vgc.ID = company.ID;
                vgc.CompanyName = company.CompanyName;
                vgc.SystemID = company.SystemID;
                #endregion
                //获取当前公司的数据
                List<MonthlyReportDetail> lstCurrentCompanyReportDetail = lstReportDetail.Where(p => p.CompanyID == company.ID).ToList();
                List<MonthlyReportDetail> lstVGMRD = null;
                lstVGMRD = new List<MonthlyReportDetail>();
                //循环明细指标，得到当前公司当前指标的数据，并重新拼装。
                MonthlyReportDetail mrd = null;
                foreach (C_Target currentTarget in lstTarget.OrderBy(p => p.Sequence))
                {

                    mrd = new MonthlyReportDetail();
                    //当前公司当前指标一个月之内不可能出现两条数据，如果出现取创建时间最近的数据。
                    MonthlyReportDetail tempMRD = lstCurrentCompanyReportDetail.Where(p => p.TargetID == currentTarget.ID).OrderBy(p => p.CreateTime).FirstOrDefault();
                    if (tempMRD == null)
                    {
                        mrd.CompanyID = company.ID;
                        mrd.TargetID = currentTarget.ID;
                        mrd.SystemID = _System.ID;
                    }
                    else
                    {
                        mrd = tempMRD;
                    }

                    lstVGMRD.Add(mrd);
                    lstSummaryCompanyTargetReportDetail.Add(mrd);

                }
                #region 计算当前公司各个明细指标的合计
                if (lstVGMRD.Count > 0)
                {
                    mrd = new MonthlyReportDetail();
                    mrd.CompanyID = company.ID;
                    mrd.TargetID = _Target[0].ID;
                    mrd.SystemID = _System.ID;
                    mrd.FinYear = FinYear;
                    mrd.FinMonth = FinMonth;
                    mrd.NPlanAmmount = lstVGMRD.Sum(p => p.NPlanAmmount);
                    mrd.NActualAmmount = lstVGMRD.Sum(p => p.NActualAmmount);
                    mrd.NAccumulativePlanAmmount = lstVGMRD.Sum(p => p.NAccumulativePlanAmmount);
                    mrd.NAccumulativeActualAmmount = lstVGMRD.Sum(p => p.NAccumulativeActualAmmount);
                    mrd = ((B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(mrd.ToBModel(), "")).ToVModel();
                    lstVGMRD.Add(mrd);
                }
                #endregion
                vgc.ListGroupTargetDetail = lstVGMRD;
                if (lstVGMRD.Sum(p => p.NAccumulativePlanAmmount) > 0)
                {
                    lstVGC.Add(vgc);
                }
            }
            return lstVGC;
        }

        /// <summary>
        /// 读取集团总部xml
        /// </summary>
        /// <returns>XMList</returns>
        protected List<V_GroupTargetXElement> SplitGroupTargetXml(XElement xelement)
        {
            List<V_GroupTargetXElement> lstGroupTargetXMl = new List<V_GroupTargetXElement>();
            XElement elementCTD = xelement;
            if (xelement != null)
            {
                if (elementCTD.Elements("ComplateTargetDetail").Elements("Target") != null)
                {
                    List<XElement> Targets = elementCTD.Elements("ComplateTargetDetail").Elements("Target").ToList();
                    V_GroupTargetXElement vt = null;
                    foreach (XElement target in Targets)
                    {
                        vt = new V_GroupTargetXElement(target);
                        lstGroupTargetXMl.Add(vt);
                    }
                }
            }
            return lstGroupTargetXMl;
        }

        /// <summary>
        /// 格式化导出数据(计划指标用于指标上报)
        /// </summary>
        /// <returns></returns>
        private void FormatDownData(string strType)
        {
            //声明变量
            List<MonthlyReportDetail> lstmrd = new List<MonthlyReportDetail>();
            if (strType == "DownGroupTargetPlan")
            {
                List<A_TargetPlanDetail> listTargetPlan = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);
                if (listTargetPlan.Count() > 0)
                {
                    MonthlyReportDetail mrd = new MonthlyReportDetail();
                    foreach (A_TargetPlanDetail atpd in listTargetPlan)
                    {
                        mrd = new MonthlyReportDetail();
                        mrd.ID = Guid.NewGuid();
                        mrd.SystemID = atpd.SystemID;
                        mrd.CompanyID = atpd.CompanyID;
                        mrd.TargetID = atpd.TargetID;
                        mrd.FinYear = atpd.FinYear;
                        mrd.FinMonth = atpd.FinMonth;
                        mrd.IsDeleted = atpd.IsDeleted;
                        mrd.NPlanAmmount = atpd.Target;
                        mrd.NAccumulativePlanAmmount = atpd.Target;
                        lstmrd.Add(mrd);
                    }
                }
                ReportDetails = lstmrd;
            }
          

        }
    }

    public class ReportInstanceDirectlyDetail : IReportInstanceDetail
    {
        public List<DictionaryVmodel> GetDetailRptDataSource(ReportInstance RptModel, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
        {
            List<DictionaryVmodel> lstSummary=ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(RptModel, false);
            return lstSummary;
        }
    }

}
