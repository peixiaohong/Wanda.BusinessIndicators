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
using System.Configuration;

namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 更具获取系统，填充某个系统的补充项(经营系统)
    /// </summary>
    public class DefaultReportInstanceSummary : IReportInstanceSummary
    {
        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        MonthlyReport Report = null;
        List<C_Target> _Target = null;

        public List<DictionaryVmodel> GetSummaryRptDataSource(ReportInstance RptModel, bool IsTargetPlan)
        {
            _Target = RptModel._Target;
            Report = RptModel.Report;
            _System = RptModel._System;
            FinYear = RptModel.FinYear;
            FinMonth = RptModel.FinMonth;
            ReportDetails = RptModel.ReportDetails;

            //这里对系统和指标再次做一次验证
            if (ReportDetails != null && ReportDetails.Count() > 0)
            {
                B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(RptModel.ReportDetails[0].MonthlyReportID);
                _System = StaticResource.Instance[_System.ID, bm.CreateTime];
                _Target = StaticResource.Instance.GetTargetList(_System.ID, bm.CreateTime).ToList();
            }

            List<MonthReportSummaryViewModel> listMRSVM = GetMonthlyReportDetailSummaryList(IsTargetPlan);
            List<B_Attachment> listAtt = GetListAtt(_System.ID);
            List<DictionaryVmodel> listMonthReportSummary = new List<DictionaryVmodel>();
            string strReportDescription = "";
            #region 月报说明
            if (RptModel.ReportDetails.Any())
            {
                XElement element = _System.Configuration;
                if (element.Elements("Report").Elements("Rgroup") != null)
                {
                    strReportDescription = element.Element("Report").GetElementValue("Rgroup", "");
                    if (!string.IsNullOrEmpty(strReportDescription))
                    {
                        System.Collections.Hashtable p = MonthDescriptionValueEngine.MonthDescriptionValueService.GetMonthDescriptionValue(RptModel.ReportDetails, _System.ID);
                        foreach (string key in p.Keys)
                        {
                            strReportDescription = strReportDescription.Replace("【" + key + "】", p[key].ToString());
                        }
                    }
                }
            }
            #endregion

            //if (Report != null)
            //{
            //    strReportDescription = Report.Description;
            //}
            listMonthReportSummary.Add(new DictionaryVmodel("ReportInstance", RptModel));
            listMonthReportSummary.Add(new DictionaryVmodel("月报说明", strReportDescription));
            listMonthReportSummary.Add(new DictionaryVmodel("月度经营报告", listMRSVM, "", GetSummaryMonthlyReportHtmlTemplate(_System.Configuration)));
            listMonthReportSummary.Add(new DictionaryVmodel("附件", listAtt));
            listMonthReportSummary.Add(new DictionaryVmodel("完成情况明细查询条件", SpliteCompanyPropertyXml("Search", _System.Configuration)));
            return listMonthReportSummary;
        }

        /// <summary>
        /// 指标统计（两个方法，一个提供给上报，一个提供给报表查询）
        /// </summary>
        /// <param name="IsReported">true:上报 ， false :报表查询 </param>
        /// <returns></returns>
        private List<MonthReportSummaryViewModel> GetMonthlyReportDetailSummaryList(bool IsReported)
        {
            List<MonthReportSummaryViewModel> listMonthReportSummaryViewModel = new List<MonthReportSummaryViewModel>();
            MonthReportSummaryViewModel mrsvm = null;
            List<MonthlyReportDetail> MRDList = ReportDetails.Where(p => p.SystemID == _System.ID && (p.Display == true)).ToList();
            List<C_TargetKpi> targetKpiList = StaticResource.Instance.GetKpiList(_System.ID, FinYear);


            //这里总是从最新的指标计划获取
            List<A_TargetPlanDetail> ATPDList = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);
          

            int i = 1;
            if (MRDList != null)
            {
                if (MRDList.Count > 0)
                {
                    foreach (C_Target target in _Target.OrderBy(p => p.Sequence))
                    {
                        if (target.NeedReport == true)
                        {
                            mrsvm = new MonthReportSummaryViewModel();
                            mrsvm.ID = i;
                            mrsvm.FinYear = FinYear;
                            mrsvm.TargetID = target.ID;
                            mrsvm.SystemID = target.SystemID;
                            mrsvm.TargetName = target.TargetName;
                            mrsvm.NPlanAmmount = (double)(MRDList.Where(p => p.TargetID == target.ID).Sum(e => e.NPlanAmmount));
                            mrsvm.NActualAmmount = (double)(MRDList.Where(p => p.TargetID == target.ID).Sum(e => e.NActualAmmount));
                            mrsvm.NDifference = mrsvm.NActualAmmount - mrsvm.NPlanAmmount;

                            if (IsReported)
                            {
                                mrsvm.NAccumulativePlanAmmount = (double)ATPDList.FindAll(p => p.TargetID == target.ID && p.FinMonth <= FinMonth).Sum(s => s.Target);  //(double)(MRDList.Where(p => p.TargetID == target.ID).Sum(e => e.NAccumulativePlanAmmount));

                                if (MRDList[0].TargetPlanID != Guid.Empty)
                                {
                                    List<B_TargetPlanDetail> BTPDList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(MRDList[0].TargetPlanID).ToList();
                                    mrsvm.NAccumulativePlanAmmount = (double)BTPDList.FindAll(p => p.TargetID == target.ID && p.FinMonth <= FinMonth).Sum(s => s.Target);
                                }
                                else
                                {
                                    mrsvm.NAccumulativePlanAmmount = (double)ATPDList.FindAll(p => p.TargetID == target.ID && p.FinMonth <= FinMonth).Sum(s => s.Target);
                                }
                                //mrsvm.NAccumulativePlanAmmount = Math.Round(B_TargetplandetailOperator.Instance.GetTargetplandetailList(lstMrd[0].TargetPlanID).ToList().FindAll(P => P.TargetID == target.ID).Sum(P => P.Target), 7).ToString();


                            }
                            else
                            {
                                mrsvm.NAccumulativePlanAmmount = (double)(MRDList.Where(p => p.TargetID == target.ID).Sum(e => e.NAccumulativePlanAmmount));
                            }

                            mrsvm.NAccumulativeActualAmmount = (double)(MRDList.Where(p => p.TargetID == target.ID).Sum(e => e.NAccumulativeActualAmmount));
                            mrsvm.NAccumulativeDifference = mrsvm.NAccumulativeActualAmmount - mrsvm.NAccumulativePlanAmmount;
                           
                            if (target.Configuration != null
                               && target.Configuration.Element("SummaryTargetDisplay") != null
                               && target.Configuration.Element("SummaryTargetDisplay").Attribute("ShowKpi") != null
                               && target.Configuration.Element("SummaryTargetDisplay").Attribute("ShowKpi").Value.Trim().ToLower() == "true")
                            {
                                if (targetKpiList.Count > 0 && targetKpiList.Find(p => p.TargetID == target.ID) != null)
                                {
                                    mrsvm.MeasureRate = Math.Round(targetKpiList.Find(p => p.TargetID == target.ID).MeasureRate * 100, 0, MidpointRounding.AwayFromZero).ToString() + "%";
                                }
                            }
                            else
                            {
                                mrsvm.MeasureRate = Math.Round(StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear).FindAll(P => P.TargetID == target.ID).Sum(P => P.Target), 7, MidpointRounding.AwayFromZero).ToString();
                            }

                           
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                            if (target.ID == ConfigurationManager.AppSettings["MonthSGRent"].ToGuid())
                            {
                                //租金收缴率-本月
                                mrsvm.NPlanStr = 0.99 * 100 + "%";
                                var actuanub = mrsvm.NActualAmmount / mrsvm.NPlanAmmount;
                                mrsvm.NActualStr = Math.Round(actuanub * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                if (actuanub > 0.99)
                                {
                                    mrsvm.NActualRate = "超计划" + Math.Round((actuanub - 0.99) * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                    listMonthReportSummaryViewModel[0].NActualRate = mrsvm.NActualRate;
                                }
                                else
                                {
                                    mrsvm.NActualRate = Math.Round((actuanub) * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                    listMonthReportSummaryViewModel[0].NActualRate = mrsvm.NActualRate;
                                }


                                //租金收缴率-年度累计
                                mrsvm.NAccumulativePlanStr = 0.99 * 100 + "%";
                                var Naccnub = mrsvm.NAccumulativeActualAmmount / mrsvm.NAccumulativePlanAmmount;

                                mrsvm.NAccumulativeActualStr = Math.Round(Naccnub * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";

                                if (Naccnub > 0.99)
                                {
                                    mrsvm.NAccumulativeActualRate = "超计划" + Math.Round((Naccnub - 0.99) * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                    listMonthReportSummaryViewModel[0].NAccumulativeActualRate = mrsvm.NAccumulativeActualRate;
                                }
                                else
                                {
                                    mrsvm.NAccumulativeActualRate = Math.Round((Naccnub) * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                    listMonthReportSummaryViewModel[0].NAccumulativeActualRate = mrsvm.NAccumulativeActualRate;
                                }

                                double MeasureRate = (double)StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear).FindAll(P => P.TargetID == target.ID).Sum(P => P.Target);
                                var merate = mrsvm.NAccumulativeActualAmmount / MeasureRate;
                                if (merate > 0.99)
                                {
                                    mrsvm.NAnnualCompletionRate = "超计划" + Math.Round((merate - 0.99) * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                    listMonthReportSummaryViewModel[0].NAnnualCompletionRate = mrsvm.NAnnualCompletionRate;
                                }
                                else
                                {
                                    mrsvm.NAnnualCompletionRate = Math.Round((merate) * 100, 1, MidpointRounding.AwayFromZero).ToString() + "%";
                                    listMonthReportSummaryViewModel[0].NAnnualCompletionRate = mrsvm.NAnnualCompletionRate;
                                }
                                
                            }
                            i++;
                        }
                    }
                }
            }
            return listMonthReportSummaryViewModel;
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="systemID">系统ID</param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        private List<B_Attachment> GetListAtt(Guid systemID)
        {
            List<B_Attachment> listAtt = null;
            if (Report != null)
            {
                listAtt = B_AttachmentOperator.Instance.GetAttachmentList(Report.ID, "月报上传").ToList();
            }
            return listAtt;
        }

        /// <summary>
        /// 获取表头名称和tmpl名称
        /// </summary>
        /// <param name="element">XML</param>
        /// <returns>月度经营报告表头,Tmpl,模板,Execl模板</returns>
        public string GetSummaryMonthlyReportHtmlTemplate(XElement element)
        {
            string strValue = "";
            if (element != null)
            {
                if (element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "");
                }
            }
            return strValue;
        }

        /// <summary>
        /// 获取计划指标是否可以修改（根据指标中的Configuration配置）
        /// </summary>
        public bool GetIsModifyTargetPlanDetail(XElement element)
        {
            bool IsModifyTargetPlanDetail = false;
            if (element != null)
            {
                if (element.Elements("IsModifyTargetPlanDetail").ToList().Count > 0)
                {
                    XElement xt = element.Elements("IsModifyTargetPlanDetail").ToList()[0];
                    if (xt.GetAttributeValue("IsModifiy", "").ToLower() == "true")
                    {
                        IsModifyTargetPlanDetail = true;
                    }
                }
            }
            return IsModifyTargetPlanDetail;
        }

        private List<VCompanyProperty> SpliteCompanyPropertyXml(string DisplayType, XElement elementCP)
        {
            List<VCompanyProperty> listVCP = new List<VCompanyProperty>();
            if (elementCP != null)
            {
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
                    returnListVCP = listVCP.Where(p => p.CompanyPropertyDisplay == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                else
                    returnListVCP = listVCP.Where(p => p.CompanyPropertySearchGroup == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                return returnListVCP;
            }
            return null;
        }

    }


    /// <summary>
    /// 更具获取系统，填充某个系统的补充项（集团部门）
    /// </summary>
    public class ReportInstanceSummary_Group : IReportInstanceSummary
    {
        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        MonthlyReport Report = null;
        List<C_Target> _Target = null;

        public List<DictionaryVmodel> GetSummaryRptDataSource(ReportInstance RptModel, bool IsTargetPlan)
        {
            _Target = RptModel._Target;
            Report = RptModel.Report;
            _System = RptModel._System;
            FinYear = RptModel.FinYear;
            FinMonth = RptModel.FinMonth;
            ReportDetails = RptModel.ReportDetails;

            if (ReportDetails != null && ReportDetails.Count() > 0)
            {
                _System = StaticResource.Instance[_System.ID, ReportDetails[0].CreateTime];
                _Target = StaticResource.Instance.GetTargetList(_System.ID, ReportDetails[0].CreateTime).ToList();
            }

            List<MonthReportSummaryViewModel> listMRSVM = GetMonthlyReportDetailSummaryList();
            List<B_Attachment> listAtt = GetListAtt(_System.ID);
            List<DictionaryVmodel> listMonthReportSummary = new List<DictionaryVmodel>();
            string strReportDescription = "";
            if (Report != null)
            {
                strReportDescription = Report.Description;
            }
            listMonthReportSummary.Add(new DictionaryVmodel("ReportInstance", RptModel));
            listMonthReportSummary.Add(new DictionaryVmodel("月报说明", strReportDescription));
            listMonthReportSummary.Add(new DictionaryVmodel("月度经营报告", listMRSVM, "", GetSummaryMonthlyReportHtmlTemplate(_System.Configuration)));
            listMonthReportSummary.Add(new DictionaryVmodel("附件", listAtt));
            listMonthReportSummary.Add(new DictionaryVmodel("完成情况明细查询条件", SpliteCompanyPropertyXml("Search", _System.Configuration)));
            return listMonthReportSummary;
        }

        private List<MonthReportSummaryViewModel> GetMonthlyReportDetailSummaryList()
        {
            List<MonthReportSummaryViewModel> listMonthReportSummaryViewModel = new List<MonthReportSummaryViewModel>();
            MonthReportSummaryViewModel mrsvm = null;
            List<MonthlyReportDetail> MRDList = ReportDetails.Where(p => p.SystemID == _System.ID).ToList();
            List<C_TargetKpi> targetKpiList = StaticResource.Instance.GetKpiList(_System.ID, FinYear);

            //这里总是从最新的指标计划获取
            List<A_TargetPlanDetail> ATPDList = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);

            int i = 1;
            if (MRDList.Count > 0)
            {
                foreach (C_Target target in _Target.Where(p => p.HaveDetail == false).OrderBy(p => p.Sequence))
                {
                    List<MonthlyReportDetail> tempMRDList = null;
                    List<V_GroupTargetXElement> lstVTX = SplitGroupTargetXml(_System.Configuration);
                    if (lstVTX.Where(p => p.GroupDetail == true && p.TargetValue.ToGuid() == target.ID).Count() > 0)
                    {
                        var jquery = from _target in _Target.Where(p => p.HaveDetail == true)
                                     join mrd in ReportDetails on _target.ID equals mrd.TargetID
                                     select mrd;
                        tempMRDList = jquery.ToList();
                    }
                    else
                    {
                        tempMRDList = MRDList.Where(p => p.TargetID == target.ID).ToList();
                    }

                    mrsvm = new MonthReportSummaryViewModel();
                    mrsvm.FinYear = FinYear;
                    mrsvm.TargetID = target.ID;
                    mrsvm.SystemID = target.SystemID;
                    mrsvm.TargetName = target.TargetName;
                    mrsvm.NPlanAmmount = (double)(tempMRDList.Sum(e => e.NPlanAmmount));
                    mrsvm.NActualAmmount = (double)(tempMRDList.Sum(e => e.NActualAmmount));

                    mrsvm.NAccumulativePlanAmmount = (double)(tempMRDList.Sum(e => e.NAccumulativePlanAmmount));
                    mrsvm.NAccumulativeActualAmmount = (double)(tempMRDList.Sum(e => e.NAccumulativeActualAmmount));
                    mrsvm.MeasureRate = "0";

                    listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                    i++;

                }
            }
            //计算合计（月度报告）
            if (listMonthReportSummaryViewModel.Count > 0)
            {
                MonthReportSummaryViewModel summrsvm = new MonthReportSummaryViewModel();
                summrsvm.FinYear = FinYear;
                summrsvm.TargetID = listMonthReportSummaryViewModel[0].TargetID;
                summrsvm.SystemID = listMonthReportSummaryViewModel[0].SystemID;
                summrsvm.TargetName = "合计";
                summrsvm.NPlanAmmount = listMonthReportSummaryViewModel.Sum(e => e.NPlanAmmount);
                summrsvm.NActualAmmount = listMonthReportSummaryViewModel.Sum(e => e.NActualAmmount);
                summrsvm.NAccumulativePlanAmmount = listMonthReportSummaryViewModel.Sum(e => e.NAccumulativePlanAmmount);
                summrsvm.NAccumulativeActualAmmount = listMonthReportSummaryViewModel.Sum(e => e.NAccumulativeActualAmmount);
                summrsvm.MeasureRate = "0";
                listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(summrsvm));
            }
            return listMonthReportSummaryViewModel;
        }
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="systemID">系统ID</param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        private List<B_Attachment> GetListAtt(Guid systemID)
        {
            List<B_Attachment> listAtt = null;
            if (Report != null)
            {
                listAtt = B_AttachmentOperator.Instance.GetAttachmentList(Report.ID, "月报上传").ToList();
            }
            return listAtt;
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
        /// 获取表头名称和tmpl名称
        /// </summary>
        /// <param name="element">XML</param>
        /// <returns>月度经营报告表头,Tmpl,模板,Execl模板</returns>
        public string GetSummaryMonthlyReportHtmlTemplate(XElement element)
        {
            string strValue = "";
            if (element != null)
            {
                if (element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "");
                }
            }
            return strValue;
        }

        /// <summary>
        /// 获取计划指标是否可以修改（根据指标中的Configuration配置）
        /// </summary>
        public bool GetIsModifyTargetPlanDetail(XElement element)
        {
            bool IsModifyTargetPlanDetail = false;
            if (element != null)
            {
                if (element.Elements("IsModifyTargetPlanDetail").ToList().Count > 0)
                {
                    XElement xt = element.Elements("IsModifyTargetPlanDetail").ToList()[0];
                    if (xt.GetAttributeValue("IsModifiy", "").ToLower() == "true")
                    {
                        IsModifyTargetPlanDetail = true;
                    }
                }
            }
            return IsModifyTargetPlanDetail;
        }

        private List<VCompanyProperty> SpliteCompanyPropertyXml(string DisplayType, XElement elementCP)
        {
            List<VCompanyProperty> listVCP = new List<VCompanyProperty>();
            if (elementCP != null)
            {
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
                    returnListVCP = listVCP.Where(p => p.CompanyPropertyDisplay == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                else
                    returnListVCP = listVCP.Where(p => p.CompanyPropertySearchGroup == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                return returnListVCP;
            }
            return null;
        }

    }


    /// <summary>
    /// 更具获取系统，填充某个系统的补充项（直管公司）
    /// </summary>
    public class ReportInstanceSummary_Directly : IReportInstanceSummary
    {
        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        MonthlyReport Report = null;
        List<C_Target> _Target = null;

        public List<DictionaryVmodel> GetSummaryRptDataSource(ReportInstance RptModel, bool IsTargetPlan)
        {
            _Target = RptModel._Target;
            Report = RptModel.Report;
            _System = RptModel._System;
            FinYear = RptModel.FinYear;
            FinMonth = RptModel.FinMonth;
            ReportDetails = RptModel.ReportDetails;

            if (ReportDetails != null && ReportDetails.Count() > 0)
            {
                _System = StaticResource.Instance[_System.ID, ReportDetails[0].CreateTime];
                _Target = StaticResource.Instance.GetTargetList(_System.ID, ReportDetails[0].CreateTime).ToList();
            }

            bool IsReported = false;

            List<MonthReportSummaryViewModel> listMRSVM = GetMonthlyReportDetailSummaryList(IsTargetPlan, IsReported);
            List<B_Attachment> listAtt = GetListAtt(_System.ID);
            List<DictionaryVmodel> listMonthReportSummary = new List<DictionaryVmodel>();
            string strReportDescription = "";
            if (Report != null)
            {
                strReportDescription = Report.Description;
            }
            listMonthReportSummary.Add(new DictionaryVmodel("ReportInstance", RptModel));
            listMonthReportSummary.Add(new DictionaryVmodel("月报说明", strReportDescription));
            listMonthReportSummary.Add(new DictionaryVmodel("月度经营报告", listMRSVM, "", GetSummaryMonthlyReportHtmlTemplate(_System.Configuration)));
            listMonthReportSummary.Add(new DictionaryVmodel("附件", listAtt));
            return listMonthReportSummary;
        }

        private List<MonthReportSummaryViewModel> GetMonthlyReportDetailSummaryList(bool IsTargetPlan ,bool IsReported)
        {
            List<MonthReportSummaryViewModel> listMonthReportSummaryViewModel = new List<MonthReportSummaryViewModel>();
            MonthReportSummaryViewModel mrsvm = null;
            FormatDownData(IsTargetPlan);
            List<MonthlyReportDetail> MRDList = ReportDetails;
            List<C_TargetKpi> targetKpiList = StaticResource.Instance.GetKpiList(_System.ID, FinYear);

            //这里总是从最新的指标计划获取
            List<A_TargetPlanDetail> ATPDList = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear);

            int i = 1;
            if (MRDList.Count>0)
            {
                foreach (C_Target target in _Target.OrderBy(p => p.Sequence))
                {
                    if (target.NeedReport == true)
                    {
                        List<MonthlyReportDetail> lstMrd = MRDList.Where(p => p.TargetID == target.ID).ToList();
                        if (lstMrd.Count > 0)
                        {
                            mrsvm = new MonthReportSummaryViewModel();
                            mrsvm.ID = i;
                            mrsvm.FinYear = FinYear;
                            mrsvm.TargetID = target.ID;
                            mrsvm.SystemID = target.SystemID;
                            mrsvm.TargetName = target.TargetName;
                            mrsvm.MonthlyDetailID = lstMrd.FirstOrDefault().ID;
                            mrsvm.NPlanAmmount = (double)(lstMrd.FirstOrDefault().NPlanAmmount);
                            mrsvm.NActualAmmount = (double)(lstMrd.FirstOrDefault().NActualAmmount);
                            mrsvm.NDifference = mrsvm.NActualAmmount - mrsvm.NPlanAmmount;

                            if (IsReported)
                            {
                                //上报的时候
                                mrsvm.NAccumulativePlanAmmount = (double)ATPDList.FindAll(p => p.TargetID == target.ID && p.FinMonth <= FinMonth).Sum(s => s.Target); //(double)(lstMrd.FirstOrDefault().NAccumulativePlanAmmount);
                            }
                            else
                            {
                                //历史查询
                                mrsvm.NAccumulativePlanAmmount = (double)(lstMrd.FirstOrDefault().NAccumulativePlanAmmount);
                            }
                            
                            mrsvm.NAccumulativeActualAmmount = (double)(lstMrd.FirstOrDefault().NAccumulativeActualAmmount);
                            mrsvm.Counter = lstMrd.FirstOrDefault().Counter;
                            mrsvm.NAccumulativeDifference = mrsvm.NAccumulativeActualAmmount - mrsvm.NAccumulativePlanAmmount;
                            if (target.Configuration != null
                               && target.Configuration.Element("SummaryTargetDisplay") != null
                               && target.Configuration.Element("SummaryTargetDisplay").Attribute("ShowKpi") != null
                               && target.Configuration.Element("SummaryTargetDisplay").Attribute("ShowKpi").Value.Trim().ToLower() == "true")
                            {
                                if (targetKpiList.Count > 0 && targetKpiList.Find(p => p.TargetID == target.ID) != null)
                                {
                                    mrsvm.MeasureRate = Math.Round(targetKpiList.Find(p => p.TargetID == target.ID).MeasureRate * 100, 0, MidpointRounding.AwayFromZero).ToString() + "%";
                                }
                            }
                            else
                            {
                                if (IsReported)
                                {
                                    //如果是上报功能，从A表中拿到最新的 全年计划指标
                                    mrsvm.MeasureRate = Math.Round(StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear).FindAll(P => P.TargetID == target.ID).Sum(P => P.Target), 7, MidpointRounding.AwayFromZero).ToString();
                                }
                                else
                                {
                                    //如果是报表 从B表中获取,历史的计划指标
                                    if (lstMrd[0].TargetPlanID != Guid.Empty)
                                    {
                                        mrsvm.MeasureRate = Math.Round(B_TargetplandetailOperator.Instance.GetTargetplandetailList(lstMrd[0].TargetPlanID).ToList().FindAll(P => P.TargetID == target.ID).Sum(P => P.Target), 7, MidpointRounding.AwayFromZero).ToString();
                                    }
                                    else
                                    {
                                        mrsvm.MeasureRate = Math.Round(StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear).FindAll(P => P.TargetID == target.ID).Sum(P => P.Target), 7, MidpointRounding.AwayFromZero).ToString();
                                    }
                                }

                                
                            }
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                            i++;
                        }

                    }
                }

            }
            return listMonthReportSummaryViewModel;

        }
        public void FormatDownData(bool IsTargetPlan)
        {
            List<MonthlyReportDetail> lstmrd = new List<MonthlyReportDetail>();
            if (IsTargetPlan)
            {
                List<A_TargetPlanDetail> listTargetPlan = StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear, FinMonth);
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

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="systemID">系统ID</param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        private List<B_Attachment> GetListAtt(Guid systemID)
        {
            List<B_Attachment> listAtt = null;
            if (Report != null)
            {
                listAtt = B_AttachmentOperator.Instance.GetAttachmentList(Report.ID, "月报上传").ToList();
            }
            return listAtt;
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
        /// 获取表头名称和tmpl名称
        /// </summary>
        /// <param name="element">XML</param>
        /// <returns>月度经营报告表头,Tmpl,模板,Execl模板</returns>
        public string GetSummaryMonthlyReportHtmlTemplate(XElement element)
        {
            string strValue = "";
            if (element != null)
            {
                if (element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "");
                }
            }
            return strValue;
        }

        /// <summary>
        /// 获取计划指标是否可以修改（根据指标中的Configuration配置）
        /// </summary>
        public bool GetIsModifyTargetPlanDetail(XElement element)
        {
            bool IsModifyTargetPlanDetail = false;
            if (element != null)
            {
                if (element.Elements("IsModifyTargetPlanDetail").ToList().Count > 0)
                {
                    XElement xt = element.Elements("IsModifyTargetPlanDetail").ToList()[0];
                    if (xt.GetAttributeValue("IsModifiy", "").ToLower() == "true")
                    {
                        IsModifyTargetPlanDetail = true;
                    }
                }
            }
            return IsModifyTargetPlanDetail;
        }

        private List<VCompanyProperty> SpliteCompanyPropertyXml(string DisplayType, XElement elementCP)
        {
            List<VCompanyProperty> listVCP = new List<VCompanyProperty>();
            if (elementCP != null)
            {
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
                    returnListVCP = listVCP.Where(p => p.CompanyPropertyDisplay == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                else
                    returnListVCP = listVCP.Where(p => p.CompanyPropertySearchGroup == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                return returnListVCP;
            }
            return null;
        }

    }


    /// <summary>
    /// 更具获取系统，填充某个系统的补充项(经营系统)
    /// </summary>
    public class ReportInstanceSummary_Project : IReportInstanceSummary
    {
        List<MonthlyReportDetail> ReportDetails = new List<MonthlyReportDetail>();
        int FinMonth = 0;
        int FinYear = 0;
        C_System _System = null;
        MonthlyReport Report = null;
        List<C_Target> _Target = null;

        public List<DictionaryVmodel> GetSummaryRptDataSource(ReportInstance RptModel, bool IsTargetPlan)
        {
            _Target = RptModel._Target;
            Report = RptModel.Report;
            _System = RptModel._System;
            FinYear = RptModel.FinYear;
            FinMonth = RptModel.FinMonth;
            ReportDetails = RptModel.ReportDetails;

            if (ReportDetails != null && ReportDetails.Count() > 0)
            {
                _System = StaticResource.Instance[_System.ID, ReportDetails[0].CreateTime];
                _Target = StaticResource.Instance.GetTargetList(_System.ID, ReportDetails[0].CreateTime).ToList();
            }

            //List<MonthReportSummaryViewModel> listMRSVM = GetMonthlyReportDetailSummaryList(IsLatestVersion); //不用

            List<MonthReportSummaryViewModel> listMRSVM = new List<MonthReportSummaryViewModel>();

            List<B_Attachment> listAtt = GetListAtt(_System.ID);
            List<DictionaryVmodel> listMonthReportSummary = new List<DictionaryVmodel>();
            string strReportDescription = "";
            if (Report != null)
            {
                strReportDescription = Report.Description;
            }
            listMonthReportSummary.Add(new DictionaryVmodel("ReportInstance", RptModel));
            listMonthReportSummary.Add(new DictionaryVmodel("月报说明", strReportDescription));
            listMonthReportSummary.Add(new DictionaryVmodel("月度经营报告", listMRSVM, "", "")); // 项目公司没有月度经营报告
            listMonthReportSummary.Add(new DictionaryVmodel("附件", listAtt));
            listMonthReportSummary.Add(new DictionaryVmodel("完成情况明细查询条件", SpliteCompanyPropertyXml("Search", _System.Configuration)));
            return listMonthReportSummary;
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="systemID">系统ID</param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        private List<B_Attachment> GetListAtt(Guid systemID)
        {
            List<B_Attachment> listAtt = null;
            if (Report != null)
            {
                listAtt = B_AttachmentOperator.Instance.GetAttachmentList(Report.ID, "月报上传").ToList();
            }
            return listAtt;
        }

        /// <summary>
        /// 获取表头名称和tmpl名称
        /// </summary>
        /// <param name="element">XML</param>
        /// <returns>月度经营报告表头,Tmpl,模板,Execl模板</returns>
        public string GetSummaryMonthlyReportHtmlTemplate(XElement element)
        {
            string strValue = "";
            if (element != null)
            {
                if (element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "");
                }
            }
            return strValue;
        }

        /// <summary>
        /// 获取计划指标是否可以修改（根据指标中的Configuration配置）
        /// </summary>
        public bool GetIsModifyTargetPlanDetail(XElement element)
        {
            bool IsModifyTargetPlanDetail = false;
            if (element != null)
            {
                if (element.Elements("IsModifyTargetPlanDetail").ToList().Count > 0)
                {
                    XElement xt = element.Elements("IsModifyTargetPlanDetail").ToList()[0];
                    if (xt.GetAttributeValue("IsModifiy", "").ToLower() == "true")
                    {
                        IsModifyTargetPlanDetail = true;
                    }
                }
            }
            return IsModifyTargetPlanDetail;
        }

        private List<VCompanyProperty> SpliteCompanyPropertyXml(string DisplayType, XElement elementCP)
        {
            List<VCompanyProperty> listVCP = new List<VCompanyProperty>();
            if (elementCP != null)
            {
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
                    returnListVCP = listVCP.Where(p => p.CompanyPropertyDisplay == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                else
                    returnListVCP = listVCP.Where(p => p.CompanyPropertySearchGroup == "true").OrderBy(c => c.CompanyPropertySenquence).ToList();
                return returnListVCP;
            }
            return null;
        }

    }
}
