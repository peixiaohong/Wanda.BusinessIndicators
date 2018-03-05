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
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Common.Web;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using Lib.Xml;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using System.Data;
using System.Reflection;
using System.Collections;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownExcelContrastComplete 的摘要说明
    /// </summary>
    public class DownExcelContrastComplete : IHttpHandler, IRequiresSessionState
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


        int FinYear = 0;
        int FinMonth = 0;
        bool IsPro = false;
        string fileName = "年度完成情况对比表汇总";
        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";
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
                if (m=="true")
                {
                    IsPro = true;
                }
            }
            DownExcel();
        }
        private void DownExcel()
        {
            List<ContrastDetailList> ContrastList = A_MonthlyreportdetailOperator.Instance.GetContrastDetail(FinYear, FinMonth, IsPro);
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "年度完成情况对比表-汇总V1.xls");

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


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
            #region 普通数据样式(红色字体)
            Aspose.Cells.Style RowStyleRed = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowStyleRed.Font.Size = 12;
            RowStyleRed.Font.Name = "Arial";
            RowStyleRed.HorizontalAlignment = TextAlignmentType.Center;
            RowStyleRed.VerticalAlignment = TextAlignmentType.Center;
            RowStyleRed.Pattern = BackgroundType.Solid;
            RowStyleRed.Font.Color = Color.Red;
            RowStyleRed.Number = 1;
            RowStyleRed.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowStyleRed.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowStyleRed.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowStyleRed.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowStyleRed.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowStyleRed.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowStyleRed.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowStyleRed.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region 带背景色数据样式(字体黑色)
            Aspose.Cells.Style BackStyleBlack = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            BackStyleBlack.Font.Size = 12;
            BackStyleBlack.Font.Name = "Arial";
            BackStyleBlack.HorizontalAlignment = TextAlignmentType.Center;
            BackStyleBlack.VerticalAlignment = TextAlignmentType.Center;
            BackStyleBlack.Pattern = BackgroundType.Solid;
            BackStyleBlack.Number = 1;
            BackStyleBlack.ForegroundColor = Color.FromArgb(217, 217, 217);
            BackStyleBlack.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            BackStyleBlack.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            BackStyleBlack.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            BackStyleBlack.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            BackStyleBlack.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            BackStyleBlack.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            BackStyleBlack.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            BackStyleBlack.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region 带背景色数据样式(字体红色)
            Aspose.Cells.Style BackStyleRed = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            BackStyleRed.Font.Size = 12;
            BackStyleRed.Font.Name = "Arial";
            BackStyleRed.Font.Color = Color.Red;
            BackStyleRed.HorizontalAlignment = TextAlignmentType.Center;
            BackStyleRed.VerticalAlignment = TextAlignmentType.Center;
            BackStyleRed.Pattern = BackgroundType.Solid;
            BackStyleRed.Number = 1;
            BackStyleRed.ForegroundColor = Color.FromArgb(217, 217, 217);
            BackStyleRed.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            BackStyleRed.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            BackStyleRed.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            BackStyleRed.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            BackStyleRed.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            BackStyleRed.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            BackStyleRed.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            BackStyleRed.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region 普通样式(字体加粗)
            Aspose.Cells.Style RowStyle1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowStyle1.Font.Size = 12;
            RowStyle1.Font.Name = "Arial";
            RowStyle1.HorizontalAlignment = TextAlignmentType.Center;
            RowStyle1.VerticalAlignment = TextAlignmentType.Center;
            RowStyle1.Pattern = BackgroundType.Solid;
            RowStyle1.Font.IsBold = true;
            RowStyle1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowStyle1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowStyle1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowStyle1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowStyle1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowStyle1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowStyle1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowStyle1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            string Title = "项目及经营系统"+FinYear+"年1-"+FinMonth+"月累计完成情况同期对比表";
            worksheets[0].Cells[0, 1].PutValue(Title);
            worksheets[0].Cells[0, 1].SetStyle(StyleTitle);
            worksheets[0].Cells[4, 4].PutValue(FinYear - 1 + "年");
            worksheets[0].Cells[4, 5].PutValue(FinYear + "年");
            worksheets[0].Cells[4, 8].PutValue(FinYear - 1 + "年");
            worksheets[0].Cells[4, 9].PutValue(FinYear + "年");
            worksheets[0].Cells[4, 12].PutValue(FinYear - 1 + "年");
            worksheets[0].Cells[4, 13].PutValue(FinYear + "年");

            int RowsNO = 5;
            for (int i = 0; i < ContrastList.Count; i++)
            {
                worksheets[0].Cells[4, 13].PutValue(FinYear + "年");
              
                worksheets[0].Cells[RowsNO, 1].PutValue(i+1);
                worksheets[0].Cells[RowsNO+1, 2].PutValue(ContrastList[i].SystemName);

                for (int a = 0; a < ContrastList[i].ContrastDetailMl.Count; a++)
                {
                    worksheets[0].Cells.SetRowHeight(RowsNO+a, 30);
                    worksheets[0].Cells[RowsNO + a, 3].PutValue(ContrastList[i].ContrastDetailMl[a].TargetName);
                    worksheets[0].Cells[RowsNO + a, 4].PutValue(ContrastList[i].ContrastDetailMl[a].LastAllTotal);
                    worksheets[0].Cells[RowsNO + a, 5].PutValue(ContrastList[i].ContrastDetailMl[a].NowAllTotal);
                    worksheets[0].Cells[RowsNO + a, 6].PutValue(ContrastList[i].ContrastDetailMl[a].Difference);
                    worksheets[0].Cells[RowsNO + a, 7].PutValue(ContrastList[i].ContrastDetailMl[a].Mounting);
                    worksheets[0].Cells[RowsNO + a, 8].PutValue(ContrastList[i].ContrastDetailMl[a].PossibleContrastLast);
                    worksheets[0].Cells[RowsNO + a, 9].PutValue(ContrastList[i].ContrastDetailMl[a].PossibleContrastNow);
                    worksheets[0].Cells[RowsNO + a, 10].PutValue(ContrastList[i].ContrastDetailMl[a].PossibleDifference);
                    worksheets[0].Cells[RowsNO + a, 11].PutValue(ContrastList[i].ContrastDetailMl[a].PossibleMounting);
                    worksheets[0].Cells[RowsNO + a, 12].PutValue(ContrastList[i].ContrastDetailMl[a].NotContrastLast);
                    worksheets[0].Cells[RowsNO + a, 13].PutValue(ContrastList[i].ContrastDetailMl[a].NotContrastNow);
                    worksheets[0].Cells[RowsNO + a, 14].PutValue(ContrastList[i].ContrastDetailMl[a].NotDifference);
                    worksheets[0].Cells[RowsNO + a, 15].PutValue(ContrastList[i].ContrastDetailMl[a].NotMounting);
                    worksheets[0].Cells[RowsNO + a, 16].PutValue(ContrastList[i].ContrastDetailMl[a].Remark);
                    worksheets[0].Cells[RowsNO + a, 1].SetStyle(RowStyle1);
                    worksheets[0].Cells[RowsNO + a, 2].SetStyle(RowStyle1);
                    worksheets[0].Cells[RowsNO + a, 3].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 4].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 5].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 6].SetStyle(RowStyle);

                    if (ContrastList[i].ContrastDetailMl[a].Difference<0)
                    {
                        worksheets[0].Cells[RowsNO + a, 7].SetStyle(BackStyleRed);
                    }
                    else
                    {
                        worksheets[0].Cells[RowsNO + a, 7].SetStyle(BackStyleBlack);
                    }
            
                    worksheets[0].Cells[RowsNO + a, 8].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 9].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 10].SetStyle(RowStyle);
                    if (ContrastList[i].ContrastDetailMl[a].PossibleDifference<0)
                    {
                        worksheets[0].Cells[RowsNO + a, 11].SetStyle(BackStyleRed);
                    }
                    else
                    {
                        worksheets[0].Cells[RowsNO + a, 11].SetStyle(BackStyleBlack);
                    }
                    worksheets[0].Cells[RowsNO + a, 12].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 13].SetStyle(RowStyle);
                    worksheets[0].Cells[RowsNO + a, 14].SetStyle(RowStyle);
                    if (ContrastList[i].ContrastDetailMl[a].NotDifference<0)
                    {
                        worksheets[0].Cells[RowsNO + a, 15].SetStyle(RowStyleRed);
                    }
                    else
                    {
                        worksheets[0].Cells[RowsNO + a, 15].SetStyle(RowStyle);
                    }
                    worksheets[0].Cells[RowsNO + a, 16].SetStyle(RowStyle);
                }


                worksheets[0].Cells.Merge(RowsNO, 1, ContrastList[i].ContrastDetailMl.Count, 1);
                worksheets[0].Cells.Merge(RowsNO, 2, ContrastList[i].ContrastDetailMl.Count, 1);
                RowsNO = RowsNO + ContrastList[i].ContrastDetailMl.Count;
            }


            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode( fileName + dateNow + ".xls"));
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }

 
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}