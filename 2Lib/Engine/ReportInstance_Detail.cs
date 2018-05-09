using Lib.Web.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using Lib.Web;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Expression;
using System.Collections;
using System;
using Lib.Xml;

namespace LJTH.BusinessIndicators.Engine
{
    public partial class ReportInstance
    {
        string strMonthReportOrderType;
        string strCompanyPropertys = "";
        bool IncludeHaveDetail = true;
        //public List<DictionaryVmodel> GetDetailRptDataSource(out string HtmlTemplate, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
        //{
        //    this.IncludeHaveDetail = IncludeHaveDetail;
        //    strMonthReportOrderType = strOrderType;
        //    if (!string.IsNullOrEmpty(strCompanyProperty))
        //        strCompanyPropertys = strCompanyProperty;
        //    List<VTarget> lstVTarget = SplitCompleteTargetDetailXml(StaticResource.Instance[_System.ID].Configuration);
        //    List<VItemCompanyProperty> lstVItemCompanyProperty = null;
        //    if (SpliteCompanyPropertyXml("List",_System.Configuration).Count > 0)
        //    {
        //        lstVItemCompanyProperty = SpliteCompanyPropertyXml("List",_System.Configuration)[0].listCP.ToList();
        //    }
        //    List<DictionaryVmodel> listvmodel = FromatData(lstVTarget, lstVItemCompanyProperty);
        //    HtmlTemplate = "";
        //    return listvmodel;
        //}
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
            string strHtmlTemplate=string.Empty;
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
                    C_Target CurrentTarget=listC_target[0];
                    ItemCompanyPropertyViewModel = new List<DictionaryVmodel>();
                    List<MonthlyReportDetail> listm = ReportDetails.Where(p => p.TargetID == CurrentTarget.ID && (p.Display == true )).ToList();

                    //判断是否是明细项数据
                    if (CurrentTarget.HaveDetail == true)
                    {
                        //是否有公司属性
                        if (SpliteCompanyPropertyXml("List", CurrentTarget.Configuration).Count > 0)
                        {
                            lstVCompanyProperty = SpliteCompanyPropertyXml("List", CurrentTarget.Configuration)[0].listCP.ToList();
                        }
                        if (lstVCompanyProperty != null)
                        {
                            foreach (VItemCompanyProperty vcp in lstVCompanyProperty)
                            {
                                List<C_Company> lstCompany = listCompany.Where(p => p.CompanyProperty1 == vcp.ItemCompanyPropertyValue).ToList();
                                List<MonthlyReportDetail> listCompanyPertyMRD = SetMonthlyReportDetail(listm, lstCompany);
                                listMRD.AddRange(listCompanyPertyMRD);
                                if (listCompanyPertyMRD.Count == 0)
                                {
                                    continue;
                                }
                                ItemCompanyPropertyViewModel.Add(new DictionaryVmodel(vcp.ItemCompanyPropertyName, EditData(listCompanyPertyMRD, vtarget, listC_target[0], listCompany, vcp), "CompanyProperty"));
                            }
                            if (SpliteCompanyPropertyXml("List", CurrentTarget.Configuration).Count > 0)
                            {
                                lstVCompanyProperty = null;
                        }
                        }
                        else
                        {
                            listMRD = SetMonthlyReportDetail(listm, listCompany);
                            ItemCompanyPropertyViewModel.Add(new DictionaryVmodel("", EditData(listMRD, vtarget, listC_target[0], listCompany, null), "CompanyProperty"));
                        }
                    }
                    else
                    {
                        listMRD = SetMonthlyReportDetail(listm, listCompany);
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
                    listDictionaryVModel.Add(new DictionaryVmodel(vtarget.TargetName, ItemCompanyPropertyViewModel, "Target", strHtmlTemplate));
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
                listVCounter=SplitCompleteTargetDetailXml(CTarget.Configuration)[0].CounterList;
            }
            else
            {
                listVCounter = vt.CounterList;
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
                    dv.Value = vcp.ItemCompanyPropertyName;
                    dv.RowSpanCount = 0;
                }
               

                #region 计算明细项项合计和小计
                
                if (!listVCounter.ToList()[i].Title.Contains("小计") && !listVCounter.ToList()[i].Title.Contains("合计"))
                {
                    bmrd=SummaryData(VCounterListMonthlyReportDetailViewModel, bmrd, CTarget);
                }
                else
                {
                    bmrd=SummaryData(listMRD, bmrd, CTarget);
                }
                #endregion

                //调用计算完成率的方法
                bmrd = TargetEvaluationEngine.TargetEvaluationService.Calculation(bmrd, false);
                dv.BMonthReportDetail = bmrd;
                dv.ObjValue = VCounterListMonthlyReportDetailViewModel;
                lstDVM.Add(dv);
            }
            //计算页面要通行数
            if (vcp != null)
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
            List<MonthlyReportDetail> listMrd = new List<MonthlyReportDetail>();
            var jquery = from mrd in listm
                         join company in listCompany on mrd.CompanyID equals company.ID
                         where company.ID != null
                         select mrd;
            listMrd = jquery.ToList<MonthlyReportDetail>();
            return listMrd;
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
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty1.Trim())).ToList();
                                break;
                            case "CompanyProperty2":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty2.Trim())).ToList();
                                break;
                            case "CompanyProperty3":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty3.Trim())).ToList();
                                break;
                            case "CompanyProperty4":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty4.Trim())).ToList();
                                break;
                            case "CompanyProperty5":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty5.Trim())).ToList();
                                break;
                            case "CompanyProperty6":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty6.Trim())).ToList();
                                break;
                            case "CompanyProperty7":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty7.Trim())).ToList();
                                break;
                            case "CompanyProperty8":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty8.Trim())).ToList();
                                break;
                            case "CompanyProperty9":
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty9.Trim())).ToList();
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
            if (elementCTD.Elements("ComplateTargetDetail").Elements("Target") != null)
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
        public List<VCompanyProperty> SpliteCompanyPropertyXml(string DisplayType, XElement elementCP)
        {
            List<VCompanyProperty> listVCP = new List<VCompanyProperty>();
            //强转字符串类型
            //XElement elementCP = null;
            //if (_System != null)
            //{
            //    elementCP = _System.Configuration;
            //}
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
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "")+",";
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
            //获取当前月计划数据
            //List<A_TargetPlanDetail> listTargetPlan = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear, FinMonth);
            List<A_TargetPlanDetail> listTargetPlan = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear, FinMonth)
                .Where(v=>v.TargetPlanID== ReportDetails.FirstOrDefault().TargetPlanID).ToList();

            //获取当前月与之前月的计划数据
            //List<A_TargetPlanDetail> listTargetPlanToYear = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);
            List<A_TargetPlanDetail> listTargetPlanToYear = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear)
                .Where(v => v.TargetPlanID == ReportDetails.FirstOrDefault().TargetPlanID).ToList(); ;

            //得到当前系统的所有公司
            List<C_Company> listComPany = StaticResource.Instance.CompanyList[_System.ID];
            if(AreaID!=Guid.Empty)
            {
                var ids = StaticResource.Instance.GetCompanyIds(_SystemID, AreaID);
                var t = from p in listComPany join q in ids on p.ID equals q select p;
                listComPany = t.ToList();
            }
            List<DictionaryVmodel> listDV = new List<DictionaryVmodel>();
            List<TargetPlanViewModel> listTargetPlanViewModel = null;
            TargetPlanViewModel tpvm = null;

            List<C_Target> PlanTarget = new List<C_Target>();

            PlanTarget = _Target;
          

            if (listTargetPlanToYear != null && listTargetPlanToYear.Count() > 0)
            {
                B_TargetPlan Bt = B_TargetplanOperator.Instance.GetTargetPlanByID(listTargetPlanToYear[0].TargetPlanID);
                PlanTarget = StaticResource.Instance.GetTargetList(_System.ID, Bt.CreateTime).ToList();
            }
            else
            {
                PlanTarget = StaticResource.Instance.GetTargetList(_System.ID,DateTime.Now).ToList();
            }

            foreach (C_Target Ctarget in PlanTarget.Where(p => p.NeedReport == true).OrderBy(p => p.Sequence))
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
                        List<A_TargetPlanDetail> lst= listTargetPlanToYear.Where(p => p.CompanyID == company.ID
                            && p.TargetID == Ctarget.ID && p.FinMonth <= FinMonth).ToList();

                        tpvm.NAccumulativePlanAmmount = lst.Sum(p=>p.Target);
                        listTargetPlanViewModel.Add(tpvm);
                    }
                }

                string strHtmlTemplate = GetComplateMonthReportDetailHtmlTemplate(_System.Configuration);
                listDV.Add(new DictionaryVmodel(Ctarget.TargetName, listTargetPlanViewModel, "Target", strHtmlTemplate));


            }
            return listDV;
        }



        public List<DictionaryVmodel> GetTagetPlanViewModel(Guid SystemID)
        {
            //获取当前月计划数据
            List<A_TargetPlanDetail> listTargetPlan = A_TargetplandetailOperator.Instance.GetTargetplandetailListBySYNC(FinYear, FinMonth);

                //StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear, FinMonth);

            //获取当前月与之前月的计划数据
            List<A_TargetPlanDetail> listTargetPlanToYear = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);

            //得到当前系统的所有公司
            List<C_Company> listComPany = StaticResource.Instance.CompanyList[_System.ID];

            List<DictionaryVmodel> listDV = new List<DictionaryVmodel>();
            List<TargetPlanViewModel> listTargetPlanViewModel = null;
            TargetPlanViewModel tpvm = null;

            List<C_Target> PlanTarget = new List<C_Target>();

            PlanTarget = _Target;


            if (listTargetPlanToYear != null && listTargetPlanToYear.Count() > 0)
            {
                B_TargetPlan Bt = B_TargetplanOperator.Instance.GetTargetPlanByID(listTargetPlanToYear[0].TargetPlanID);
                PlanTarget = StaticResource.Instance.GetTargetList(_System.ID, Bt.CreateTime).ToList();
            }
            else
            {
                PlanTarget = StaticResource.Instance.GetTargetList(_System.ID, DateTime.Now).ToList();
            }

            foreach (C_Target Ctarget in PlanTarget.Where(p => p.NeedReport == true).OrderBy(p => p.Sequence))
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
                        List<A_TargetPlanDetail> lst = listTargetPlanToYear.Where(p => p.CompanyID == company.ID
                             && p.TargetID == Ctarget.ID && p.FinMonth <= FinMonth).ToList();

                        tpvm.NAccumulativePlanAmmount = lst.Sum(p => p.Target);
                        tpvm.NActualAmmount = btp.JQNDifference;
                        listTargetPlanViewModel.Add(tpvm);
                    }
                }

                string strHtmlTemplate = GetComplateMonthReportDetailHtmlTemplate(_System.Configuration);
                listDV.Add(new DictionaryVmodel(Ctarget.TargetName, listTargetPlanViewModel, "Target", strHtmlTemplate));


            }
            return listDV;
        }




    }
}
