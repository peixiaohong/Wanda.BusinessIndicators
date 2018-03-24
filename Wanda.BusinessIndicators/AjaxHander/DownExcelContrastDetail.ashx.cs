using Aspose.Cells;
using Lib.Config;
using Lib.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Common.Web;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using System.Data;
using System.Reflection;
using System.Collections;
namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownExcelContrastDetail 的摘要说明
    /// </summary>
    public class DownExcelContrastDetail : IHttpHandler, IRequiresSessionState
    {

        private string _excelTempletePath = null;

        private string ExcelTempletePath
        {
            get
            {
                if (_excelTempletePath == null)
                {
                    _excelTempletePath = ConfigurationManager.AppSettings["ExcelTempletePath"];
                    _excelTempletePath = HttpContext.Current.Server.MapPath(_excelTempletePath);
                }
                if (Directory.Exists(_excelTempletePath) == false)
                {
                    if (NetworkSharedFolder.IsNetShareMappedPath(_excelTempletePath))
                    {
                        //NetworkSharedFolder nsf = new NetworkSharedFolder();
                        string remotePath = AppSettingConfig.GetSetting("FileServer_RemotePath", "");
                        string localPath = AppSettingConfig.GetSetting("FileServer_LocalPath", "");
                        string username = AppSettingConfig.GetSetting("FileServer_UserName", "");
                        string password = AppSettingConfig.GetSetting("FileServer_UserPassword", "");

                        NetworkSharedFolder.Connect(remotePath, localPath, username, password);
                        if (Directory.Exists(_excelTempletePath) == false)
                        {
                            throw new ApplicationException("未在配置中找到附件文件存放路径， 或指定路径不存在！");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("未在配置中找到附件文件存放路径， 或指定路径不存在！");
                    }
                }

                return _excelTempletePath;
            }
        }


        Guid SystemID = Guid.Empty;
        Guid MonthlyReportID = Guid.Empty;  
        int FinYear = 0;
        int FinMonth = 0;
        bool IsPro = false;
        C_System SystemModel = new C_System();
        string fileName = "年度完成情况对比表";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysID"]))
            {
                SystemID = Guid.Parse(HttpContext.Current.Request["SysID"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["MonthlyReportID"]))
            {
                MonthlyReportID = Guid.Parse(HttpContext.Current.Request["MonthlyReportID"]);
               
            }
            else
            {
                MonthlyReportID = Guid.Empty;
               
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear = Convert.ToInt32(HttpContext.Current.Request["FinYear"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinMonth"]))
            {
                FinMonth = Convert.ToInt32(HttpContext.Current.Request["FinMonth"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsPro"]))
            {
                string m = HttpContext.Current.Request["IsPro"];
                if (m == "true")
                {
                    IsPro = true;
                }
            }
            DownExcel();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }



        private void DownExcel() 
        {
            C_System sys=StaticResource.Instance[SystemID,DateTime.Now];
            List<ContarstTargetDetailList> result = new List<ContarstTargetDetailList>();

            if (sys.GroupType != "ProSystem")
	        {
                result = A_MonthlyreportdetailOperator.Instance.GetContarstTargetDetailList(MonthlyReportID, FinYear, FinMonth, SystemID, IsPro);
	        }
            else
            {
                result = A_MonthlyreportdetailOperator.Instance.GetProContarstTargetDetailList(MonthlyReportID, FinYear, FinMonth, SystemID, IsPro);

            }
            SystemModel = C_SystemOperator.Instance.GetSystem(SystemID);
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "年度完成情况对比表V1.xls");

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;



            //-------------------样式-------------------------------------//
            #region 汇总标题样式
            Aspose.Cells.Style StyleTitle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            StyleTitle.Font.Size = 16;
            StyleTitle.Font.Name = "黑体";
            StyleTitle.Font.IsBold = true;
            StyleTitle.HorizontalAlignment = TextAlignmentType.Center;
            StyleTitle.VerticalAlignment = TextAlignmentType.Center;
            StyleTitle.Pattern = BackgroundType.Solid;

            #endregion



            #region 普通数据样式
            Aspose.Cells.Style RowStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowStyle.Font.Size = 12;
            RowStyle.Font.Name = "Arial";
            RowStyle.HorizontalAlignment = TextAlignmentType.Center;
            RowStyle.VerticalAlignment = TextAlignmentType.Center;
            RowStyle.Pattern = BackgroundType.Solid;
            RowStyle.Number = 1;
            RowStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowStyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowStyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowStyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowStyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region 时间数据样式
            Aspose.Cells.Style RowTimeStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowTimeStyle.Font.Size = 12;
            RowTimeStyle.Font.Name = "Arial";
            RowTimeStyle.HorizontalAlignment = TextAlignmentType.Center;
            RowTimeStyle.VerticalAlignment = TextAlignmentType.Center;
            RowTimeStyle.Pattern = BackgroundType.Solid;
   
            RowTimeStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowTimeStyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowTimeStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowTimeStyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowTimeStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowTimeStyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowTimeStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowTimeStyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region 可比不可比标题样式
            
          
            Aspose.Cells.Style Smtitle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Smtitle.Font.Size = 12;
            Smtitle.Font.Name = "Arial";
            Smtitle.Font.IsBold = true;
            Smtitle.Number = 1;
            Smtitle.Font.Color = Color.Black;
            Smtitle.ForegroundColor = Color.FromArgb(184, 204, 228);
            Smtitle.HorizontalAlignment = TextAlignmentType.Center;
            Smtitle.VerticalAlignment = TextAlignmentType.Center;
            Smtitle.Pattern = BackgroundType.Solid;
            Smtitle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            Smtitle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            Smtitle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            Smtitle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            Smtitle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            Smtitle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            Smtitle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            Smtitle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

       
            #endregion
            //复制sheet
            for (int i = 0; i < result.Count; i++)
            {
                worksheets.AddCopy(0);
                worksheets[i+1].Name = result[i].TargetName;
                string SheetTitle = "";
                if (SystemModel.GroupType=="ProSystem")
                {

                     SheetTitle = "境内项目" + result[i].TargetName + "完成情况年度对比表";
                }
                else
                {
                     SheetTitle = SystemModel.SystemName + result[i].TargetName + "完成情况年度对比表";
                }
               
                worksheets[i + 1].Cells[0, 1].PutValue(SheetTitle);
                worksheets[i + 1].Cells[0, 1].SetStyle(StyleTitle);

                //编辑表头以及可比的列头 
                string Nowtime = FinYear + "年1-" + FinMonth + "月";
                string Lasttime = FinYear-1 + "年1-" + FinMonth + "月";
                worksheets[i + 1].Cells[1, 2].PutValue(Nowtime);
                worksheets[i + 1].Cells[1, 2].PutValue(Nowtime);
                worksheets[i + 1].Cells[2, 4].PutValue(Lasttime);
                worksheets[i + 1].Cells[2, 5].PutValue(Nowtime);
                worksheets[i + 1].Cells.Merge(3, 1, 1, 2);
                worksheets[i + 1].Cells[3, 1].PutValue("可比门店小计");
                worksheets[i + 1].Cells.SetRowHeight(3, 25);
                worksheets[i + 1].Cells[3, 1].SetStyle(Smtitle);
                worksheets[i + 1].Cells[3, 2].SetStyle(Smtitle);
                worksheets[i + 1].Cells[3, 3].PutValue("——");
                worksheets[i + 1].Cells[3, 3].SetStyle(Smtitle);
                worksheets[i + 1].Cells[3, 4].PutValue(result[i].ContractLastTotal);
                worksheets[i + 1].Cells[3, 4].SetStyle(Smtitle);
                worksheets[i + 1].Cells[3, 5].PutValue(result[i].ContractNowTotal);
                worksheets[i + 1].Cells[3, 5].SetStyle(Smtitle);
                worksheets[i + 1].Cells[3, 6].PutValue(result[i].ContractDifference);
                worksheets[i + 1].Cells[3, 6].SetStyle(Smtitle);
                worksheets[i + 1].Cells[3, 7].PutValue(result[i].ContractMounting);
                worksheets[i + 1].Cells[3, 7].SetStyle(Smtitle);


                //编辑可比门店明细
                for (int a = 0; a < result[i].ContrastList.Count; a++)
                {

                    worksheets[i + 1].Cells.SetRowHeight(4 + a, 25);
                    worksheets[i + 1].Cells[4 + a, 1].PutValue(a+1);
                    worksheets[i + 1].Cells[4 + a, 1].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[4 + a, 2].PutValue(result[i].ContrastList[a].CompanyName);
                    worksheets[i + 1].Cells[4 + a, 2].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[4 + a, 3].PutValue(result[i].ContrastList[a].OpeningTime.ToShortDateString());
                    worksheets[i + 1].Cells[4 + a, 3].SetStyle(RowTimeStyle);
                    worksheets[i + 1].Cells[4 + a, 4].PutValue(result[i].ContrastList[a].LastAllTotal);
                    worksheets[i + 1].Cells[4 + a, 4].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[4 + a, 5].PutValue(result[i].ContrastList[a].NowAllTotal);
                    worksheets[i + 1].Cells[4 + a, 5].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[4 + a, 6].PutValue(result[i].ContrastList[a].Difference);
                    worksheets[i + 1].Cells[4 + a, 6].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[4 + a, 7].PutValue(result[i].ContrastList[a].Mounting);
                    worksheets[i + 1].Cells[4 + a, 7].SetStyle(RowStyle);
                }

                //对可比门店分组
                if (result[i].ContrastList.Count!=0)
                {
                    worksheets[i + 1].Cells.GroupRows(4, result[i].ContrastList.Count + 3, true);
  
                }

                //可比门店列头
                worksheets[i + 1].Cells.Merge(result[i].ContrastList.Count + 4, 1, 1, 2);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 1].PutValue("不可比门店小计");
                worksheets[i + 1].Cells.SetRowHeight(result[i].ContrastList.Count + 4, 25);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 1].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 2].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 3].PutValue("——");
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 3].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 4].PutValue(result[i].NotContractLastTotal);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 4].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 5].PutValue(result[i].NotContractNowTotal);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 5].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 6].PutValue(result[i].NotContractDifference);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 6].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 7].PutValue(result[i].NotContractMounting);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + 4, 7].SetStyle(Smtitle);

                //编辑可比门店明细
                for (int a = 0; a < result[i].NotContrastList.Count; a++)
                {

                    worksheets[i + 1].Cells.SetRowHeight(result[i].ContrastList.Count + 5 + a, 25);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count+5 + a, 1].PutValue(a + 1);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 1].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 2].PutValue(result[i].NotContrastList[a].CompanyName);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 2].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 3].PutValue(result[i].NotContrastList[a].OpeningTime.ToShortDateString());
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 3].SetStyle(RowTimeStyle);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 4].PutValue(result[i].NotContrastList[a].LastAllTotal);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 4].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 5].PutValue(result[i].NotContrastList[a].NowAllTotal);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 5].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 6].PutValue(result[i].NotContrastList[a].Difference);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 6].SetStyle(RowStyle);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 7].PutValue(result[i].NotContrastList[a].Mounting);
                    worksheets[i + 1].Cells[result[i].ContrastList.Count + 5 + a, 7].SetStyle(RowStyle);
                }

                //对可比门店分组
                if (result[i].NotContrastList.Count!=0)
                {
                    worksheets[i + 1].Cells.GroupRows(result[i].ContrastList.Count + 5, result[i].ContrastList.Count + result[i].NotContrastList.Count + 4, true); 
                }

                //合计
                worksheets[i + 1].Cells.Merge(result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 1, 1, 2);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 1].PutValue("合计");
                worksheets[i + 1].Cells.SetRowHeight(result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 25);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 1].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 2].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 3].PutValue("——");
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 3].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 4].PutValue(result[i].LastTotal);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 4].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 5].PutValue(result[i].NowTotal);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 5].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 6].PutValue(result[i].Difference);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 6].SetStyle(Smtitle);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 7].PutValue(result[i].Mounting);
                worksheets[i + 1].Cells[result[i].ContrastList.Count + result[i].NotContrastList.Count + 5, 7].SetStyle(Smtitle);
            }


    
            worksheets.RemoveAt(0);
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(SystemModel.SystemName + fileName + dateNow + ".xls"));
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
 
        }
    }
}