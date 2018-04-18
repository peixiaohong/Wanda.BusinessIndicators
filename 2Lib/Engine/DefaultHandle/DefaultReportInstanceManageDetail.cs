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

    public class DefaultReportInstanceManageDetail : IReportInstanceManageDetail
    {
        string strMonthReportOrderType;
        string strCompanyPropertys = "";
        bool IncludeHaveDetail = true;

        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        List<C_Target> _Target = null;

        public List<DictionaryVmodel> GetManageDetailRptDataSource(ReportInstance RptModel, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
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
                listvmodel = GetMergeComplateMonthReportDetail(listvmodel);
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
                    List<MonthlyReportDetail> listm = ReportDetails.Where(p => p.TargetID == CurrentTarget.ID && (p.Display == true)).ToList();

                    //判断是否是明细项考核数据
                    if (CurrentTarget.HaveDetail == true && CurrentTarget.NeedEvaluation == true)
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
                    if (CurrentTarget.Configuration.Elements("ManageTargetDetail").Elements("TableTemplate").ToList().Count > 0)
                    {
                        strHtmlTemplate = GetComplateMonthReportDetailHtmlTemplate(CurrentTarget.Configuration);
                    }
                    else
                    {
                        //如果当前指标不存在表头、Tmpl模板、Excle模板，则使用系统的。
                        if (_System.Configuration.Elements("ManageTargetDetail").Elements("TableTemplate").ToList().Count > 0)
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
            if (CTarget.Configuration.Elements("ManageTargetDetail").Elements("Target").ToList().Count > 0)
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

                if (listVCounter.ToList()[i].Title != "小计" || listVCounter.ToList()[i].IsSummaryDetail.ToLower() == "true")
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
            if (vcp != null && lstDVM.Count() > 0)
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

            if (element.Elements("IsDifferenceExceptionTarget").ToList().Count > 0)
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

                bmrd.ODifference = bmrd.NDifference = listData.Where(p => p.NDifference <= 0).Sum(p => p.NDifference);
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

            listm = listm.Where(x => listCompany.Exists(c => c.ID == x.CompanyID)).ToList();

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
                                listCompany = listCompany.Where(p => tempStrs[1].Contains(p.CompanyProperty7.Trim()) || p.CompanyProperty7 == null).ToList();
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
            if (elementCTD.Elements("ManageTargetDetail").Elements("BlendTargets").Count() > 0)
            {
                //完成情况明细模板
                List<XElement> Targets = elementCTD.Elements("ManageTargetDetail").Elements("BlendTargets").Elements("Target").ToList();
                VTarget vt = null;
                foreach (XElement target in Targets)
                {
                    vt = new VTarget(target);
                    targetList.Add(vt);
                }

            }
            if (elementCTD.Elements("ManageTargetDetail").Elements("Target").Count() > 0)
            {
                //完成情况明细模板
                List<XElement> Targets = elementCTD.Elements("ManageTargetDetail").Elements("Target").ToList();
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
                if (element.Elements("ManageTargetDetail").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("ManageTargetDetail").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("RargetReportTableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "") + ",";
                }
            }
            return strValue;
        }
        /// <summary>
        ///  合并经营报告明细（混合指标）
        /// </summary>
        /// <param name="dvCurrent">已计算的单项指标集合</param>
        /// <returns></returns>
        public List<DictionaryVmodel> GetMergeComplateMonthReportDetail(List<DictionaryVmodel> dvCurrentList)
        {
            List<DictionaryVmodel> dvResult = new List<DictionaryVmodel>();

            var completeBlendTargetDetailXml = SplitCompleteBlendTargetDetailXml(_System.Configuration);
            //遍历完成情况明细中包含几个混合指标（目前需求一个版块只允许有一个）
            foreach (var item in completeBlendTargetDetailXml)
            {
                //合并混合指标为Object            
                List<DictionaryVmodel> dvMergeList = new List<DictionaryVmodel>();

                DictionaryVmodel dv = new DictionaryVmodel();
                dv.Name = item.TargetName;
                dv.IsBlendTarget = true;
                dv.Mark = "Target";
                dv.Senquence = item.Senquence;
                foreach (var dvItem in dvCurrentList)
                {
                    dv.HtmlTemplate = dvItem.HtmlTemplate;
                    var TargetList = item.VTargetList.Where(m => m.TargetName == dvItem.Name).ToList();
                    //判断当前指标是否为混合指标
                    if (TargetList != null && TargetList.Count() > 0)
                    {
                        dvMergeList.Add(dvItem);
                    }
                    else
                    {
                        //如果不存在单指标才重新添加
                        if (dvResult.Where(m => m.Name == dvItem.Name).Count() == 0)
                            dvResult.Add(dvItem);
                    }
                }
                if (dvMergeList.Count > 0)
                {
                    dv.ObjValue = dvMergeList;
                    dvResult.Add(dv);
                }
            }
            //重新排序
            return dvResult.OrderBy(m => m.Senquence).ToList();
        }
        /// <summary>
        /// 读取xml文件（ComplateTargetDetail.xml）
        /// </summary>
        /// <returns>经营报告况明细模板-混合指标</returns>
        private List<VBlendTarget> SplitCompleteBlendTargetDetailXml(XElement xelement)
        {
            List<VBlendTarget> targetList = new List<VBlendTarget>();

            XElement elementCTD = xelement;

            if (elementCTD.Elements("ManageTargetDetail").Elements("BlendTargets").Count() > 0)
            {
                //完成情况明细模板
                List<XElement> Targets = elementCTD.Elements("ManageTargetDetail").Elements("BlendTargets").ToList();
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
}
