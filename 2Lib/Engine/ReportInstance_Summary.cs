using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.BusinessIndicators.Model;
using Lib.Web;
using Lib.Expression;
using System.Collections;
using System.Xml.Linq;
using Lib.Xml;

namespace Wanda.BusinessIndicators.Engine
{
    public partial class ReportInstance
    {

        
        public List<DictionaryVmodel> GetSummaryRptDataSource(out string SummaryTemplate, bool IsLatestVersion = false)
        {
            //InitialData(IsLatestVersion);
            SummaryTemplate = "";
            List<MonthReportSummaryViewModel> listMRSVM = GetMonthlyReportDetailSummaryList(IsLatestVersion);
            List<B_Attachment> listAtt= GetListAtt(_System.ID,IsLatestVersion);
            List<DictionaryVmodel> listMonthReportSummary = new List<DictionaryVmodel>();
            string strReportDescription = "";
            if (Report != null)
            {
                strReportDescription = Report.Description;
            }
            listMonthReportSummary.Add(new DictionaryVmodel("ReportInstance", this));
            listMonthReportSummary.Add(new DictionaryVmodel("月报说明", strReportDescription));
            listMonthReportSummary.Add(new DictionaryVmodel("月度经营报告", listMRSVM, "", GetSummaryMonthlyReportHtmlTemplate(_System.Configuration)));
            listMonthReportSummary.Add(new DictionaryVmodel("附件",listAtt));
            listMonthReportSummary.Add(new DictionaryVmodel("完成情况明细查询条件", SpliteCompanyPropertyXml("Search",_System.Configuration)));
            return listMonthReportSummary;
        }

        public List<MonthReportSummaryViewModel> GetMonthlyReportDetailSummaryList(bool IsLatestVersion = false)
        {
            List<MonthReportSummaryViewModel> listMonthReportSummaryViewModel = new List<MonthReportSummaryViewModel>();
            MonthReportSummaryViewModel mrsvm=null;
            List<MonthlyReportDetail> MRDList = ReportDetails.Where(p => p.SystemID == _System.ID && (p.Display == true)).ToList();
            List<C_TargetKpi> targetKpiList = StaticResource.Instance.GetKpiList(_System.ID, FinYear);
            
            int i = 1;
            foreach (C_Target target in _Target.OrderBy(p=>p.Sequence))
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
                    mrsvm.NDifference = mrsvm.NActualAmmount-mrsvm.NPlanAmmount;
                    mrsvm.NAccumulativePlanAmmount = (double)(MRDList.Where(p => p.TargetID == target.ID).Sum(e => e.NAccumulativePlanAmmount));
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
                        mrsvm.MeasureRate1 = (double)Math.Round(StaticResource.Instance.GetTargetPlanList(_System.ID, FinYear).FindAll(P => P.TargetID == target.ID).Sum(P => P.Target), 7, MidpointRounding.AwayFromZero);
                        mrsvm.MeasureRate = mrsvm.MeasureRate1.ToString();
                    }
                    listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                    i++;
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
        public List<B_Attachment> GetListAtt(Guid systemID,bool IsLatestVersion = false)
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
            string strValue="";
            if (element != null)
            {
                if (element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("SummaryMonthlyReport").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "")+",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "");
                }
            }
            return strValue;
        }


        /// <summary>
        /// 获取计划指标是否可以修改（根据指标中的Configuration配置）
        /// </summary>
        public List<DictionaryVmodel> GetIsModifyTargetPlanDetail(XElement element)
        {
            List<DictionaryVmodel> listdv = new List<DictionaryVmodel>();
            bool IsModifyTargetPlanDetail=false;
            bool IsHaveAccumulativePlanDetail = false;
            if(element!=null)
            {
                if (element.Elements("IsModifyTargetPlanDetail").ToList().Count > 0)
                {
                    XElement xt = element.Elements("IsModifyTargetPlanDetail").ToList()[0];
                    if (xt.GetAttributeValue("IsModifiy", "").ToLower() == "true")
                    {
                        IsModifyTargetPlanDetail = true;
                    }

                    if (xt.GetAttributeValue("IsHaveAccumulativePlanDetail", "").ToLower() == "true")
                    {
                        IsHaveAccumulativePlanDetail = true;
                    }
                }
            }
            listdv.Add(new DictionaryVmodel("IsModifyTargetPlanDetail",IsModifyTargetPlanDetail));
            listdv.Add(new DictionaryVmodel("IsHaveAccumulativePlanDetail", IsHaveAccumulativePlanDetail));
            return listdv;
        }
    }
}
