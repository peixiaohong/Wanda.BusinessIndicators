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
using Wanda.BusinessIndicators.Web.AjaxHandler;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownExcelContrastMisstarget 的摘要说明
    /// </summary>
    public class DownExcelContrastMisstarget : IHttpHandler, IRequiresSessionState
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
        string SysID = string.Empty;
        int FinYear = 0;
        int FinMonth = 0;
        string FileType = string.Empty; //下载的文件类型
        string fileName = "未完成统计表";
        bool IsPro = false;

        public void ProcessRequest(HttpContext context)
        {
            //  context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysID"]))
            {
                SysID = HttpContext.Current.Request["SysID"];
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear =int.Parse(HttpContext.Current.Request["FinYear"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinMonth"]))
            {
                FinMonth =int.Parse( HttpContext.Current.Request["FinMonth"]);
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
        private void DownExcel()
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "未完成家数统计表.xlsx");
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            #region 汇总标题样式
            Aspose.Cells.Style StyleTitle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            StyleTitle.Font.Size = 24;
            StyleTitle.Font.Name = "黑体";
            StyleTitle.Font.IsBold = true;
            StyleTitle.HorizontalAlignment = TextAlignmentType.Center;
            StyleTitle.VerticalAlignment = TextAlignmentType.Center;
            StyleTitle.Pattern = BackgroundType.Solid;

            #endregion
            #region 普通数据样式
            Aspose.Cells.Style RowFirstStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowFirstStyle.Font.Size = 11;
            RowFirstStyle.Font.Name = "华文细黑";
            RowFirstStyle.HorizontalAlignment = TextAlignmentType.Center;
            RowFirstStyle.VerticalAlignment = TextAlignmentType.Center;
            RowFirstStyle.Pattern = BackgroundType.Solid;
            RowFirstStyle.Font.IsBold = true;
            RowFirstStyle.IsTextWrapped = true;
            RowFirstStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowFirstStyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowFirstStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowFirstStyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowFirstStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowFirstStyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowFirstStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowFirstStyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region 普通数据样式
            Aspose.Cells.Style RowStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowStyle.Font.Size = 11;
            RowStyle.Font.Name = "Arial";
            RowStyle.HorizontalAlignment = TextAlignmentType.Center;
            RowStyle.VerticalAlignment = TextAlignmentType.Center;
            RowStyle.Pattern = BackgroundType.Solid;
            RowStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowStyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowStyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowStyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowStyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region 普通数据(对比时字体带颜色)
            Aspose.Cells.Style RowStyleRed = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowStyleRed.Font.Size = 11;
            RowStyleRed.Font.Name = "Arial";
            RowStyleRed.Font.Color = Color.Red;
            RowStyleRed.HorizontalAlignment = TextAlignmentType.Center;
            RowStyleRed.VerticalAlignment = TextAlignmentType.Center;
            RowStyleRed.Pattern = BackgroundType.Solid;
            RowStyleRed.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowStyleRed.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowStyleRed.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowStyleRed.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowStyleRed.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowStyleRed.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowStyleRed.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowStyleRed.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region 累计指标未完成家数带背景色
            Aspose.Cells.Style RowBackGround = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowBackGround.Font.Size = 11;
            RowBackGround.Font.Name = "Arial";
            RowBackGround.HorizontalAlignment = TextAlignmentType.Center;
            RowBackGround.VerticalAlignment = TextAlignmentType.Center;
            RowBackGround.Pattern = BackgroundType.Solid;
            RowBackGround.Font.IsBold = true;
            RowBackGround.ForegroundColor = Color.FromArgb(228, 223, 236);
            RowBackGround.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowBackGround.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowBackGround.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowBackGround.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowBackGround.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowBackGround.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowBackGround.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowBackGround.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region 累计指标未完成家数带背景色(字体带颜色)
            Aspose.Cells.Style RowBackGroundRed = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowBackGroundRed.Font.Size = 11;
            RowBackGroundRed.Font.Name = "Arial";
            RowBackGroundRed.Font.Color = Color.Red;
            RowBackGroundRed.HorizontalAlignment = TextAlignmentType.Center;
            RowBackGroundRed.VerticalAlignment = TextAlignmentType.Center;
            RowBackGroundRed.Pattern = BackgroundType.Solid;
            RowBackGroundRed.Font.IsBold = true;
            RowBackGroundRed.ForegroundColor = Color.FromArgb(228, 223, 236);
            RowBackGroundRed.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowBackGroundRed.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowBackGroundRed.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowBackGroundRed.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowBackGroundRed.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowBackGroundRed.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowBackGroundRed.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowBackGroundRed.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            List<ContrastMisstargetList> returnresult = A_MonthlyreportdetailOperator.Instance.GetContrastMisstarget(FinYear, FinMonth, SysID,IsPro);
            string Titles = string.Empty;
            if (SysID == "0")
            {
                Titles = FinYear + "年各系统未完成家数统计表";
            }
            else
            {
                Titles = FinYear + "年" + (FinMonth).ToString() + "月" + returnresult[0].SystemName + "未完成家数统计表";
            }

            worksheets[0].Cells[0, 1].PutValue(Titles);
            worksheets[0].Cells[0, 1].SetStyle(StyleTitle);


            worksheets[0].Cells[2, 3].PutValue(FinMonth - 1 + "月");
            // worksheets[0].Cells[2, 3].SetStyle(RowStyleName);
            worksheets[0].Cells[2, 8].PutValue(FinMonth + "月");
            // worksheets[0].Cells[2, 4].SetStyle(RowStyleName);
            worksheets[0].Cells[2, 17].PutValue(FinYear - 1+ "年" + FinMonth + "月");

            int n = 5;
            for (int i = 0; i < returnresult.Count; i++)
            {
                worksheets[0].Cells.Merge(n, 1, returnresult[i].ContrastMisstarget.Count, 1);
                worksheets[0].Cells[n, 1].PutValue(returnresult[i].SystemName);
                for (int s = 0; s < returnresult[i].ContrastMisstarget.Count; s++)
                {
                    worksheets[0].Cells[n, 2].PutValue(returnresult[i].ContrastMisstarget[s].TargetName);
                    worksheets[0].Cells[n, 2].SetStyle(RowFirstStyle);
                    #region 环比数据

                    worksheets[0].Cells[n, 3].PutValue(returnresult[i].ContrastMisstarget[s].LastEvaluationCompany);
                    if (returnresult[i].ContrastMisstarget[s].LastEvaluationCompany == 0)
                    {
                        worksheets[0].Cells[n, 4].PutValue("--");
                        worksheets[0].Cells[n, 5].PutValue("--");
                        worksheets[0].Cells[n, 6].PutValue("--");
                        worksheets[0].Cells[n, 7].PutValue("--");
                    }
                    else
                    {
                        worksheets[0].Cells[n, 4].PutValue(returnresult[i].ContrastMisstarget[s].LastIsMissCurrent);
                        worksheets[0].Cells[n, 5].PutValue(returnresult[i].ContrastMisstarget[s].LastProportionCurrent);
                        worksheets[0].Cells[n, 6].PutValue(returnresult[i].ContrastMisstarget[s].LastIsMissTarget);
                        worksheets[0].Cells[n, 7].PutValue(returnresult[i].ContrastMisstarget[s].LastProportion);
                    }
                    worksheets[0].Cells[n, 8].PutValue(returnresult[i].ContrastMisstarget[s].ThisEvaluationCompany);
                    if (returnresult[i].ContrastMisstarget[s].ThisEvaluationCompany == 0)
                    {
                        worksheets[0].Cells[n, 9].PutValue("--");
                        worksheets[0].Cells[n, 10].PutValue("--");
                        worksheets[0].Cells[n, 11].PutValue("--");
                        worksheets[0].Cells[n, 12].PutValue("--");
                    }
                    else
                    {
                        worksheets[0].Cells[n, 9].PutValue(returnresult[i].ContrastMisstarget[s].ThisIsMissCurrent);
                        worksheets[0].Cells[n, 10].PutValue(returnresult[i].ContrastMisstarget[s].ThisProportionCurrent);
                        worksheets[0].Cells[n, 11].PutValue(returnresult[i].ContrastMisstarget[s].ThisIsMissTarget);
                        worksheets[0].Cells[n, 12].PutValue(returnresult[i].ContrastMisstarget[s].ThisProportion);
                    }
                    worksheets[0].Cells[n, 3].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 4].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 5].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 6].SetStyle(RowBackGround);
                    worksheets[0].Cells[n, 7].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 8].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 9].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 10].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 11].SetStyle(RowBackGround);
                    worksheets[0].Cells[n, 12].SetStyle(RowStyle);
                    #endregion

                    #region 环比

                    if (returnresult[i].ContrastMisstarget[s].LastEvaluationCompany == 0 || returnresult[i].ContrastMisstarget[s].ThisEvaluationCompany == 0)
                    {
                        worksheets[0].Cells[n, 13].PutValue("--");
                        worksheets[0].Cells[n, 14].PutValue("--");
                        worksheets[0].Cells[n, 15].PutValue("--");
                        worksheets[0].Cells[n, 16].PutValue("--");
                        worksheets[0].Cells[n, 13].SetStyle(RowStyle);
                        worksheets[0].Cells[n, 14].SetStyle(RowStyle);
                        worksheets[0].Cells[n, 15].SetStyle(RowBackGround);
                        worksheets[0].Cells[n, 16].SetStyle(RowStyle);
                    }
                    else
                    {
                        worksheets[0].Cells[n, 13].PutValue(returnresult[i].ContrastMisstarget[s].HuanMissTargetChangeCurrent);
                        worksheets[0].Cells[n, 14].PutValue(returnresult[i].ContrastMisstarget[s].HuanMissTargetPCurrent);
                        worksheets[0].Cells[n, 15].PutValue(returnresult[i].ContrastMisstarget[s].HuanMissTargetChange);
                        worksheets[0].Cells[n, 16].PutValue(returnresult[i].ContrastMisstarget[s].HuanMissTargetProportion);
                        if (returnresult[i].ContrastMisstarget[s].HuanMissTargetChangeCurrent > 0)
                        {
                            worksheets[0].Cells[n, 13].SetStyle(RowStyleRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 13].SetStyle(RowStyle);
                        }
                        if (Convert.ToDouble(returnresult[i].ContrastMisstarget[s].HuanMissTargetPCurrent.Trim('%')) > 0)
                        {
                            worksheets[0].Cells[n, 14].SetStyle(RowStyleRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 14].SetStyle(RowStyle);
                        }
                        if (returnresult[i].ContrastMisstarget[s].HuanMissTargetChange > 0)
                        {
                            worksheets[0].Cells[n, 15].SetStyle(RowBackGroundRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 15].SetStyle(RowBackGround);
                        }
                        if (Convert.ToDouble(returnresult[i].ContrastMisstarget[s].HuanMissTargetProportion.Trim('%')) > 0)
                        {
                            worksheets[0].Cells[n, 16].SetStyle(RowStyleRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 16].SetStyle(RowStyle);
                        }
                    }


                    #endregion

                    #region 同比数据
                    worksheets[0].Cells[n, 17].PutValue(returnresult[i].ContrastMisstarget[s].YearEvaluationCompany);
                    if (returnresult[i].ContrastMisstarget[s].YearEvaluationCompany == 0)
                    {
                        worksheets[0].Cells[n, 18].PutValue("--");
                        worksheets[0].Cells[n, 19].PutValue("--");
                        worksheets[0].Cells[n, 20].PutValue("--");
                        worksheets[0].Cells[n, 21].PutValue("--");
                    }
                    else
                    {
                        worksheets[0].Cells[n, 18].PutValue(returnresult[i].ContrastMisstarget[s].YearIsMissCurrent);
                        worksheets[0].Cells[n, 19].PutValue(returnresult[i].ContrastMisstarget[s].YearProportionCurrent);
                        worksheets[0].Cells[n, 20].PutValue(returnresult[i].ContrastMisstarget[s].YearIsMissTarget);
                        worksheets[0].Cells[n, 21].PutValue(returnresult[i].ContrastMisstarget[s].YearProportion);
                    }
                    worksheets[0].Cells[n, 17].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 18].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 19].SetStyle(RowStyle);
                    worksheets[0].Cells[n, 20].SetStyle(RowBackGround);
                    worksheets[0].Cells[n, 21].SetStyle(RowStyle);
                    #endregion

                    #region 环比

                    if (returnresult[i].ContrastMisstarget[s].YearEvaluationCompany == 0 || returnresult[i].ContrastMisstarget[s].ThisEvaluationCompany == 0)
                    {
                        worksheets[0].Cells[n, 22].PutValue("--");
                        worksheets[0].Cells[n, 23].PutValue("--");
                        worksheets[0].Cells[n, 24].PutValue("--");
                        worksheets[0].Cells[n, 25].PutValue("--");
                        worksheets[0].Cells[n, 22].SetStyle(RowStyle);
                        worksheets[0].Cells[n, 23].SetStyle(RowStyle);
                        worksheets[0].Cells[n, 24].SetStyle(RowBackGround);
                        worksheets[0].Cells[n, 25].SetStyle(RowStyle);
                    }
                    else
                    {
                        worksheets[0].Cells[n, 22].PutValue(returnresult[i].ContrastMisstarget[s].TongMissTargetChangeCurrent);
                        worksheets[0].Cells[n, 23].PutValue(returnresult[i].ContrastMisstarget[s].TongMissTargetPCurrent);
                        worksheets[0].Cells[n, 24].PutValue(returnresult[i].ContrastMisstarget[s].TongMissTargetChange);
                        worksheets[0].Cells[n, 25].PutValue(returnresult[i].ContrastMisstarget[s].TongMissTargetProportion);
                        if (returnresult[i].ContrastMisstarget[s].TongMissTargetChangeCurrent > 0)
                        {
                            worksheets[0].Cells[n, 22].SetStyle(RowStyleRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 22].SetStyle(RowStyle);
                        }
                        if (Convert.ToDouble(returnresult[i].ContrastMisstarget[s].TongMissTargetPCurrent.Trim('%')) > 0)
                        {
                            worksheets[0].Cells[n, 23].SetStyle(RowStyleRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 23].SetStyle(RowStyle);
                        }
                        if (returnresult[i].ContrastMisstarget[s].TongMissTargetChange > 0)
                        {
                            worksheets[0].Cells[n, 24].SetStyle(RowBackGroundRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 24].SetStyle(RowBackGround);
                        }
                        if (Convert.ToDouble(returnresult[i].ContrastMisstarget[s].TongMissTargetProportion.Trim('%')) > 0)
                        {
                            worksheets[0].Cells[n, 25].SetStyle(RowStyleRed);
                        }
                        else
                        {
                            worksheets[0].Cells[n, 25].SetStyle(RowStyle);
                        }
                    }


                    #endregion

                    n++;
                }
                //n = n + returnresult[i].ContrastMisstarget.Count;
            }
            for (int i = 5; i < n; i++)
            {
                worksheets[0].Cells.SetRowHeight(i, 30);
                worksheets[0].Cells[i, 1].SetStyle(RowFirstStyle);
            }
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName + dateNow + ".xls"));
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