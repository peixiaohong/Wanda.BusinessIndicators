using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.BusinessIndicators.Web.AjaxHandler;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    public partial class DownExcelTemplete
    {
        //读取webconfig的值
        private string siteUrl = ConfigurationManager.AppSettings["SiteURL"];
        /// <summary>
        /// 下载Excel月度经营报告
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public void DownExcelMonthReportSummary(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            string str = HttpContext.Current.Server.MapPath("../");
            string Title = FinYear.ToString() + "年" + FinMonth + "月";
            List<DictionaryVmodel> listDVM = null;
            if (rpt != null)
            {
                listDVM = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false); // .GetSummaryRptDataSource(out strs, false);

                //判断当前系统是否有自己的模板
                if(!string.IsNullOrEmpty(listDVM[2].HtmlTemplate))
                {
                   string[] strHtmlTemplates = listDVM[2].HtmlTemplate.Split(',');
                   if (!string.IsNullOrEmpty(strHtmlTemplates[2]))
                   {
                       templeteName = strHtmlTemplates[2];
                   }
                }
            }
            List<MonthReportSummaryViewModel> list = (List<MonthReportSummaryViewModel>)listDVM[2].ObjValue;

            if (sytemID != ConfigurationManager.AppSettings["MonthSG"].ToGuid())
            {
                list.ForEach(p =>
                {
                    double temp = 0;
                    if (double.TryParse(p.MeasureRate, out temp))
                    {
                        p.MeasureRate1 = Convert.ToDouble(p.MeasureRate);
                    }

                });
            }
            else
            {
                list.ForEach(p =>
                {

                    //因租金收缴率使用String类型，将其余几个项目也如此赋值。
                    if (p.TargetID != ConfigurationManager.AppSettings["MonthSGRent"].ToGuid())
                    {
                        p.NPlanStr = string.Format("{0:N2}", p.NPlanAmmount);
                        p.NActualStr = string.Format("{0:N2}", p.NActualAmmount);
                        p.NAccumulativePlanStr = string.Format("{0:N2}", p.NAccumulativePlanAmmount);
                        p.NAccumulativeActualStr = string.Format("{0:N2}", p.NAccumulativeActualAmmount);
                        p.NDifferStr = string.Format("{0:N2}", p.NDifference);
                        p.NAccDiffereStr = string.Format("{0:N2}", p.NAccumulativeDifference);
                        double temp = 0;
                        if (double.TryParse(p.MeasureRate, out temp))
                        {
                            p.MeasureRate1 = Convert.ToDouble(p.MeasureRate);
                        }
                        p.MeasureRate = string.Format("{0:N2}", p.MeasureRate1);
                    }
                    else if (p.TargetID == ConfigurationManager.AppSettings["MonthSGRent"].ToGuid())
                    {
                        if (p.NDifference == 0)
                        {
                            p.NDifferStr = string.Format("{0:N0}", p.NDifference) + "\r\n (无欠收)";
                        }
                        else
                        {
                            p.NDifferStr = string.Format("{0:N2}", p.NDifference) + "\r\n (当月欠收)";
                        }

                        if (p.NAccumulativeDifference == 0)
                        {
                            p.NAccDiffereStr = string.Format("{0:N0}", p.NAccumulativeDifference) + "\r\n (无欠收)";
                        }
                        else
                        {
                            p.NAccDiffereStr = string.Format("{0:N2}", p.NAccumulativeDifference) + "\r\n (累计欠收)";
                        }
                    }
                   


                });
            }
            


            MemoryStream stream = ExportExcel(list, "MonthReportSummary", templetePath, templeteName,rpt);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2")+"_"+dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();

        }

        /// <summary>
        /// 导出数据生成文件流下载
        /// </summary>
        /// <param name="list">导出的数据列表</param>
        /// <param name="listName">列表名</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        public MemoryStream ExportExcel<T>(List<T> list, string listName, string templetePath, string templeteName,ReportInstance rpt)
        {

            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                designer.Workbook = new Workbook(fileStream);

                designer.SetDataSource("Title", rpt._System.SystemName + "月度报告");
                designer.SetDataSource("Date", "报告期：" + rpt.FinYear.ToString() + "年1-" + rpt.FinMonth.ToString() + "月");
                designer.SetDataSource("Date_Group", rpt.FinYear.ToString() + "年" + rpt.FinMonth.ToString()+"月");


                designer.SetDataSource(listName, list);
                designer.Process();
                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                return stream;

            }
        }

    }
}