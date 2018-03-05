using Lib.Web;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.BusinessIndicators.Model;
using System.Xml.Linq;

namespace Wanda.BusinessIndicators.Web.AjaxHandler
{
    public class MonthRptConfigControll : BaseController
    {

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        [LibAction]
        public List<C_System> GetSysList(string SystemID, int Year)
        {

            Guid  _SystemID =SystemID.ToGuid();
            DateTime CurrentDate = DateTime.Now;
            List<C_System> SysList = C_SystemOperator.Instance.GetSystemList(CurrentDate).Where(p => p.ID == _SystemID).ToList();

           return SysList;
        }

        [LibAction]
        public List<C_System> SaveSysConfiguration(string SystemID,  string SunTmpl, string DetailTmpl, string MissTargetlTmpl, string ReturnTmpl)
        {
            Guid _SystemID = SystemID.ToGuid();

            C_System SysModel = StaticResource.Instance[_SystemID, DateTime.Now];

            XElement SysConfiguration = SysModel.Configuration;

            #region 指标统计节点判断

            List<XElement> SunTmplElementList = SysConfiguration.Elements("SummaryMonthlyReport").ToList();

            #endregion

            switch (SunTmpl)
            {
                case "sum001":  //默认模版(是没有SummaryMonthlyReport 节点的)

                    if (SunTmplElementList.Count > 0)
                    {
                        SysConfiguration.Elements("SummaryMonthlyReport").Remove();
                    }

                    break;
                case "sum002": //商管模版
                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_SG");
                        xt.SetAttributeValue("TableDataTmplName", "MonthReportSummaryTemplate_SG");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度经营报告-商管系统V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_SG");
                        subElement.SetAttributeValue("TableDataTmplName", "MonthReportSummaryTemplate_SG");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告-商管系统V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }
                    break;
                case "sum003": //游艇

                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_JDJS");
                        xt.SetAttributeValue("TableDataTmplName", "MonthReportSummaryTemplate_1");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度经营报告_游艇公司V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_JDJS");
                        subElement.SetAttributeValue("TableDataTmplName", "MonthReportSummaryTemplate_1");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告_游艇公司V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }

                    break;
                    case "sum004": //农业

                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_JDJS");
                        xt.SetAttributeValue("TableDataTmplName", "MonthReportSummaryTemplate_1");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度经营报告_有机农业V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_JDJS");
                        subElement.SetAttributeValue("TableDataTmplName", "MonthReportSummaryTemplate_1");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告_有机农业V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }

                    break;

                    case "sum005": //长白山
                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_DZSW");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度经营报告_长白山管理公司V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_DZSW");
                        subElement.SetAttributeValue("TableDataTmplName", "");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告_长白山管理公司V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }
                    break;

                    case "sum006": //昆明高尔夫
                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_KMGEF");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度经营报告_昆明高尔夫V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_KMGEF");
                        subElement.SetAttributeValue("TableDataTmplName", "");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告_昆明高尔夫V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }
                    break;

                   
                    case "sum007": //武汉电影乐园 /秀场
                    case "sum008":
                    case "sum009"://西双版纳
                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_DZSW");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度经营报告_大众电影V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "MonthReportSummaryHeadTemplate_DZSW");
                        subElement.SetAttributeValue("TableDataTmplName", "");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告_大众电影V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }
                    break;

                    case "sum010"://集团总部
                    case "sum011": //商业地产总部
                    case "sum012": //文化集团总部
                    if (SunTmplElementList.Count > 0)
                    {
                        XElement xt = SunTmplElementList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "月度报告_集团总部V1.xlsx");
                    }
                    else
                    {
                        XElement sunElement = new XElement("SummaryMonthlyReport");
                        XElement subElement = new XElement("TableTemplate");

                        subElement.SetAttributeValue("TableHeadTmplName", "");
                        subElement.SetAttributeValue("TableDataTmplName", "");
                        subElement.SetAttributeValue("TableExcelTemplateName", "月度报告_集团总部V1.xlsx");
                        sunElement.Add(subElement);
                        SysConfiguration.Add(sunElement);
                    }
                    break;
            }



            #region 指标明细模版

            List<XElement> DetailTmplList = SysConfiguration.Elements("ComplateTargetDetail").ToList();

            #endregion

            switch (DetailTmpl)
            {
                case "Detail001": // 默认版本（是没有ReportMonthlyDetail 节点的）
                    if (DetailTmplList.Count > 0)
                    {
                        SysConfiguration.Elements("ComplateTargetDetail").Elements("TableTemplate").Remove();
                    }
                
                    break;
                case "Detail002":  //商管的
                    if (DetailTmplList.Count > 0)
                    {
                        XElement xt = DetailTmplList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "ComplateTargetDetailTemplate_SG");
                        xt.SetAttributeValue("TableDataTmplName", "ComplateTargetDetailTemplate_SGXT");
                        xt.SetAttributeValue("RargetReportTableDataTmplName", "TargetReportedComplateTargetDetailTemplate_SGXT");

                    }
                    else
                    {
                        XElement DetailElement = new XElement("TableTemplate");

                        DetailElement.SetAttributeValue("TableHeadTmplName", "ComplateTargetDetailTemplate_SG");
                        DetailElement.SetAttributeValue("TableDataTmplName", "ComplateTargetDetailTemplate_SGXT");
                        DetailElement.SetAttributeValue("RargetReportTableDataTmplName", "TargetReportedComplateTargetDetailTemplate_SGXT");
                        DetailElement.SetAttributeValue("TableExcelTemplateName", "月度经营报告-商管系统V1.xlsx");
                        SysConfiguration.Element("ComplateTargetDetail").Add(DetailElement);
                    }
                    break;
                case "Detail003": //物管的

                    if (DetailTmplList.Count > 0)
                    {
                        XElement xt = DetailTmplList.Elements("TableTemplate").ToList()[0];
                        
                        xt.SetAttributeValue("TableHeadTmplName", "");
                        xt.SetAttributeValue("TableDataTmplName", "ComplateTargetDetailTemplate");
                        xt.SetAttributeValue("RargetReportTableDataTmplName", "TargetReportedComplateTargetDetailTemplate");                        
                    }
                    else
                    {
                        XElement DetailElement = new XElement("TableTemplate");

                        DetailElement.SetAttributeValue("TableHeadTmplName", "");
                        DetailElement.SetAttributeValue("TableDataTmplName", "ComplateTargetDetailTemplate");
                        DetailElement.SetAttributeValue("RargetReportTableDataTmplName", "TargetReportedComplateTargetDetailTemplate");
                        DetailElement.SetAttributeValue("TableExcelTemplateName", "");
                        SysConfiguration.Element("ComplateTargetDetail").Add(DetailElement);
                    }

                    break;

                case "Detail004": //旅业的

                    if (DetailTmplList.Count > 0)
                    {
                        XElement xt = DetailTmplList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "CompleteDetailHeadTemplate");
                        xt.SetAttributeValue("TableDataTmplName", "ComplateTargetDetailTemplate_WDLY");
                        xt.SetAttributeValue("RargetReportTableDataTmplName", "TargetReportedComplateTargetDetailTemplate_WDLY");
                        xt.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-万达旅业V1.xlsx");
                        
                    }
                    else
                    {
                        XElement DetailElement = new XElement("TableTemplate");

                        DetailElement.SetAttributeValue("TableHeadTmplName", "CompleteDetailHeadTemplate");
                        DetailElement.SetAttributeValue("TableDataTmplName", "ComplateTargetDetailTemplate_WDLY");
                        DetailElement.SetAttributeValue("RargetReportTableDataTmplName", "TargetReportedComplateTargetDetailTemplate_WDLY");
                        DetailElement.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-万达旅业V1.xlsx");
                        SysConfiguration.Element("ComplateTargetDetail").Add(DetailElement);
                    }

                    break;
                    case "Detail005": //集团总部

                    if (DetailTmplList.Count > 0)
                    {
                        XElement xt = DetailTmplList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("RargetReportTableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-集团总部V1.xlsx");
                        
                    }
                    else
                    {
                        XElement DetailElement = new XElement("TableTemplate");

                        DetailElement.SetAttributeValue("TableHeadTmplName", "");
                        DetailElement.SetAttributeValue("TableDataTmplName", "");
                        DetailElement.SetAttributeValue("RargetReportTableDataTmplName", "");
                        DetailElement.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-集团总部V1.xlsx");
                        SysConfiguration.Element("ComplateTargetDetail").Add(DetailElement);
                    }

                    break;
                   
                    case "Detail006": //商业地产总部
                    if (DetailTmplList.Count > 0)
                    {
                        XElement xt = DetailTmplList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("RargetReportTableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-商业地产总部V1.xlsx");
                        
                    }
                    else
                    {
                        XElement DetailElement = new XElement("TableTemplate");

                        DetailElement.SetAttributeValue("TableHeadTmplName", "");
                        DetailElement.SetAttributeValue("TableDataTmplName", "");
                        DetailElement.SetAttributeValue("RargetReportTableDataTmplName", "");
                        DetailElement.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-商业地产总部V1.xlsx");
                        SysConfiguration.Element("ComplateTargetDetail").Add(DetailElement);
                    }

                    break;

                    case "Detail007": //文化集团总部
                    if (DetailTmplList.Count > 0)
                    {
                        XElement xt = DetailTmplList.Elements("TableTemplate").ToList()[0];

                        xt.SetAttributeValue("TableHeadTmplName", "");
                        xt.SetAttributeValue("TableDataTmplName", "");
                        xt.SetAttributeValue("RargetReportTableDataTmplName", "");
                        xt.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-文化集团总部V1.xlsx");
                        
                    }
                    else
                    {
                        XElement DetailElement = new XElement("TableTemplate");

                        DetailElement.SetAttributeValue("TableHeadTmplName", "");
                        DetailElement.SetAttributeValue("TableDataTmplName", "");
                        DetailElement.SetAttributeValue("RargetReportTableDataTmplName", "");
                        DetailElement.SetAttributeValue("TableExcelTemplateName", "完成情况明细模板-文化集团总部V1.xlsx");
                        SysConfiguration.Element("ComplateTargetDetail").Add(DetailElement);
                    }

                    break;
            }

            switch (MissTargetlTmpl)
            {
                case "001":
                    break;
                case "002":
                    break;
                case "003":
                    break;
            }

            switch (ReturnTmpl)
            {
                case "001":
                    break;
                case "002":
                    break;
                case "003":
                    break;
            }

            SysModel.Configuration = SysConfiguration;

            //C_SystemOperator.Instance.UpdateSystem(SysModel);

            DateTime CurrentDate = DateTime.Now;
            List<C_System> SysList = C_SystemOperator.Instance.GetSystemList(CurrentDate).Where(p => p.ID == _SystemID).ToList();

            return SysList;

        }

    }
}
