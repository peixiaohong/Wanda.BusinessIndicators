
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
using Lib.Web.Json;
using LJTH.BusinessIndicators.Engine.Engine;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownExcelTargetPlan 的摘要说明
    /// </summary>
    public class DownExcelTargetPlan : IHttpHandler
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
        Guid SysId = Guid.Empty;
        Guid TargetPlanID = Guid.Empty;
        int FinYear = 0;
        string FileType;
        bool IsLatestVersion = true;
        bool IsNull = false;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (HttpContext.Current.Request.QueryString["FileType"] != null)
            {
                FileType = HttpContext.Current.Request.QueryString["FileType"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SystemID"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SystemID"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                //在计划指标查询页面的excel下载处,加入了参数,这样的话把改页面的下载和其他页面区分开,取时间时,就不用去系统时间
                if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsReported"]))
                {
                    FinYear = Convert.ToInt32(HttpContext.Current.Request["FinYear"]);
                }
                else
                {
                    DateTime datetime = new DateTime();
                    datetime = StaticResource.Instance.GetReportDateTime();
                    FinYear = datetime.Year;
                }
                //FinMonth = datetime.Month;
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["TargetPlanID"]))
            {
                TargetPlanID = Guid.Parse(HttpContext.Current.Request["TargetPlanID"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsLatestVersion"]))
            {
                if (HttpContext.Current.Request["IsLatestVersion"].ToLower() == "false")
                {
                    IsLatestVersion = false;
                }
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsNull"]))
            {
                if (HttpContext.Current.Request["IsNull"].ToLower() == "true")
                {
                    IsNull = true;
                }
            }

            #region 根据系统的不同种类，调用不同的方法。（未启用）
            //switch (Category)
            //{ 
            //    case 1:

            //        break;
            //    case 2:

            //        break;
            //    case 3:

            //        break;
            //    case 4:

            //        break;
            //    default:

            //        break;
            //}
            #endregion
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            templetePath = ExcelTempletePath;
            templeteName = "计划指标上传模板V1.xlsx";
            fileName = "计划指标";
            if (FileType != "Reported")
            {
                DownExcleTargetPlanDetail(templetePath, templeteName, fileName, SysId, FinYear);
            }
            else
            {
                DownExcleTargetPlanDetailReported(templetePath, templeteName, fileName, SysId, FinYear);
            }
        }

        public void DownExcleTargetPlanDetail(string templetePath, string templeteName, string fileName, Guid sytemID, int Year)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            List<DictionaryVmodel> listDV = (List<DictionaryVmodel>)TargetPlanEngine.TargetPlanEngineService.GetTargetPlanSource(SysId, FinYear, TargetPlanID, IsLatestVersion);
            bool IsYearTargetPlan = false;
            if (listDV.Count > 0)
            {
                if (!string.IsNullOrEmpty(listDV[0].HtmlTemplate))
                {
                    string[] str = listDV[0].HtmlTemplate.Split(',');
                    if (str.Count() > 0)
                    {
                        templeteName = str[2];
                        if (str[3].ToLower() == "true")
                        {
                            IsYearTargetPlan = true;
                        }
                    }
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            #region style1 样式
            style1.Font.Size = 12;
            #endregion
            #region style2 样式 无加粗
            style2.Font.Size = 12;
            style2.IsLocked = false;

            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region style3 样式
            style3.Font.Size = 12;
            style3.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style3.Pattern = BackgroundType.Solid;
            style3.Font.IsBold = true;

            style3.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion
            #region style4 样式
            style4.HorizontalAlignment = TextAlignmentType.Center;
            style4.VerticalAlignment = TextAlignmentType.Center;

            #endregion
            // List<DictionaryVmodel> listDV = (List<DictionaryVmodel>)TargetPlanEngine.TargetPlanEngineService.GetTargetPlanSource(SysId, FinYear, TargetPlanID, IsLatestVersion);

            string strSystemName = StaticResource.Instance[SysId, DateTime.Now].SystemName;

            for (int i = 0; i < listDV.Count; i++)
            {
                if (i > 0)
                {
                    worksheets.AddCopy(1);
                }

                worksheets[i + 1].Name = listDV[i].Name;
                excel.SetCustomProperty(worksheets[i + 1], "SystemID", SysId.ToString());
                excel.SetCustomProperty(worksheets[i + 1], "SheetName", "UpTargetPlanDetail");
                worksheets[i + 1].Cells[0, 1].PutValue(FinYear + "年" + strSystemName + listDV[i].Name + "指标分解表");
            }

            //创建指标Sheet,
            for (int sheetIndex = 0; sheetIndex < listDV.Count; sheetIndex++)
            {
                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列
                List<DictionaryVmodel> listCompanyDV = (List<DictionaryVmodel>)listDV[sheetIndex].ObjValue;
                for (int i = 0; i < listCompanyDV.Count; i++)
                {
                    Worksheet worksheet = worksheets[sheetIndex + 1];
                    List<B_TargetPlanDetail> listBTargetPlanDetail = new List<B_TargetPlanDetail>();
                    if (listCompanyDV[i].ObjValue is List<B_TargetPlanDetail>)
                    {
                        listBTargetPlanDetail = (List<B_TargetPlanDetail>)listCompanyDV[i].ObjValue;
                    }
                    else
                    {
                        List<A_TargetPlanDetail> listATargetPlanDetail = (List<A_TargetPlanDetail>)listCompanyDV[i].ObjValue;
                        listATargetPlanDetail.ForEach(p => listBTargetPlanDetail.Add(p.ToBModel()));
                    }
                    if (listBTargetPlanDetail == null)
                    {
                        continue;
                    }
                    if (!IsYearTargetPlan)
                    {
                        #region 设置样式
                        worksheet.Cells[rowStart, colStart].SetStyle(style2);
                        worksheet.Cells[rowStart, colStart + 1].SetStyle(style2);
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                        worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                        #endregion

                        #region 赋值
                        worksheet.Cells[rowStart, colStart].PutValue(i + 1);
                        worksheet.Cells[rowStart, colStart + 1].PutValue(listCompanyDV[i].Name);
                        DateTime dtime = StaticResource.Instance.CompanyList[SysId].Where(p => p.CompanyName == listCompanyDV[i].Name).FirstOrDefault().OpeningTime;
                        if (dtime != DateTime.MinValue)
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(dtime.ToShortDateString());
                        }
                        worksheet.Cells[rowStart, colStart + 3].Formula = "=SUM(F" + (rowStart + 1) + ":Q" + (rowStart + 1) + ")";
                        #endregion

                        style2 = worksheet.Cells[rowStart, colStart + 3].GetStyle();
                        style2.Number = 3;
                        worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);

                        for (int j = 0; j < 12; j++)
                        {
                            #region 设置样式
                            worksheet.Cells[rowStart, colStart + 4 + j].SetStyle(style2);
                            #endregion

                            #region 赋值
                            List<B_TargetPlanDetail> singleMonthTargetPlanDetail = listBTargetPlanDetail == null ? null : listBTargetPlanDetail.Where(p => p.FinMonth == (j + 1)).ToList();
                            worksheet.Cells[rowStart, colStart + 4 + j].PutValue(0);
                            if (singleMonthTargetPlanDetail != null)
                            {
                                if (singleMonthTargetPlanDetail.Count() > 0)
                                {
                                    //var TargetStr = string.Format("{0:N0}", singleMonthTargetPlanDetail.FirstOrDefault().Target);
                                    worksheet.Cells[rowStart, colStart + 4 + j].PutValue(singleMonthTargetPlanDetail.FirstOrDefault().Target);
                                }
                            }
                            #endregion
                            style2 = worksheet.Cells[rowStart, colStart + 4 + j].GetStyle();
                            style2.Number = 3;
                            worksheet.Cells[rowStart, colStart + 4 + j].SetStyle(style2);
                        }
                    }
                    else
                    {
                        #region 设置样式
                        worksheet.Cells[rowStart, colStart].SetStyle(style2);
                        worksheet.Cells[rowStart, colStart + 1].SetStyle(style2);
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                        #endregion

                        #region 赋值
                        worksheet.Cells[rowStart, colStart].PutValue(i + 1);
                        worksheet.Cells[rowStart, colStart + 1].PutValue(listCompanyDV[i].Name);
                        //判断参数是否包含小数点。
                        if (!string.IsNullOrEmpty(listCompanyDV[i].Mark))
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(decimal.Parse(listCompanyDV[i].Mark));
                        }
                        else
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(0.0);
                        }
                        #endregion

                        style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                        style2.Number = 3;
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                    }

                    rowStart++;

                }


                //则表示，是最后一行，添加合计


                var _SysModel = StaticResource.Instance[SysId, DateTime.Now];

                if (_SysModel.Category == 3)
                {
                    #region 设置样式
                    
                    StyleFlag flg = new StyleFlag();
                    flg.All = true;
                    Range range = worksheets[sheetIndex + 1].Cells.CreateRange(rowStart, colStart, 1, 2);
                    range.Merge();
                    range.ApplyStyle(style3, flg);

                    worksheets[sheetIndex + 1].Cells[rowStart, colStart + 2].SetStyle(style2);
                    #endregion

                    #region 赋值
                    //worksheets[sheetIndex+1].Cells[rowStart , colStart].PutValue(listCompanyDV.Count + 1);
                    worksheets[sheetIndex + 1].Cells[rowStart, colStart].PutValue("合计");

                    worksheets[sheetIndex + 1].Cells[rowStart, colStart + 2].Formula = "=SUM(D5" + ":D" + rowStart + ")";

                    #endregion
                }
                
                //worksheets[sheetIndex].Protect(ProtectionType.All);
            }
            worksheets.RemoveAt(0);

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(StaticResource.Instance[SysId, DateTime.Now].SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }

        public void DownExcleTargetPlanDetailReported(string templetePath, string templeteName, string fileName, Guid sytemID, int Year)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            List<DictionaryVmodel> listDV = (List<DictionaryVmodel>)TargetPlanEngine.TargetPlanEngineService.GetTargetPlanSource(SysId, FinYear, TargetPlanID, IsLatestVersion);
            bool IsYearTargetPlan = false;
            if (listDV.Count > 0)
            {
                if (!string.IsNullOrEmpty(listDV[0].HtmlTemplate))
                {
                    string[] str = listDV[0].HtmlTemplate.Split(',');
                    templeteName = str[2];
                    if (str[3].ToLower() == "true")
                    {
                        IsYearTargetPlan = true;
                    }
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            #region style1 样式
            style1.Font.Size = 12;
            #endregion
            #region style2 样式 无加粗
            style2.Font.Size = 12;
            style2.IsLocked = false;

            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region style3 样式
            style3.Font.Size = 12;
            style3.IsTextWrapped = true;
            style3.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style3.Pattern = BackgroundType.Solid;


            style3.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion
            #region style4 样式
            style4.HorizontalAlignment = TextAlignmentType.Center;
            style4.VerticalAlignment = TextAlignmentType.Center;

            #endregion
            #region 副标样式(时间和单位)
            Aspose.Cells.Style StyleTitle2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            StyleTitle2.Font.Size = 12;
            StyleTitle2.Font.Name = "Arial";
            StyleTitle2.Font.IsBold = true;
            StyleTitle2.HorizontalAlignment = TextAlignmentType.Center;
            StyleTitle2.VerticalAlignment = TextAlignmentType.Center;
            StyleTitle2.Pattern = BackgroundType.Solid;

            #endregion


            #region 表头样式
            Aspose.Cells.Style StyleTitle3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            StyleTitle3.Font.Size = 12;
            StyleTitle3.Font.Name = "Arial";
            StyleTitle3.Font.IsBold = true;
            StyleTitle3.Font.Color = Color.White;
            StyleTitle3.ForegroundColor = Color.FromArgb(22, 54, 92);
            StyleTitle3.HorizontalAlignment = TextAlignmentType.Center;
            StyleTitle3.VerticalAlignment = TextAlignmentType.Center;
            StyleTitle3.Pattern = BackgroundType.Solid;
            StyleTitle3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            StyleTitle3.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            StyleTitle3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            StyleTitle3.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            StyleTitle3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            StyleTitle3.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            StyleTitle3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            StyleTitle3.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region 汇总标题样式
            Aspose.Cells.Style StyleTitle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            StyleTitle.Font.Size = 16;
            StyleTitle.Font.Name = "黑体";
            StyleTitle.Font.IsBold = true;
            StyleTitle.HorizontalAlignment = TextAlignmentType.Center;
            StyleTitle.VerticalAlignment = TextAlignmentType.Center;
            StyleTitle.Pattern = BackgroundType.Solid;

            #endregion


            Aspose.Cells.Style monthstyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            monthstyle.Font.Size = 12;
            monthstyle.Font.Name = "Arial";
            monthstyle.Font.IsBold = true;
            monthstyle.HorizontalAlignment = TextAlignmentType.Center;
            monthstyle.Pattern = BackgroundType.Solid;
            monthstyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            monthstyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            monthstyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            monthstyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            monthstyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            monthstyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            monthstyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            monthstyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;


            #region 数据样式
            Aspose.Cells.Style RowStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowStyle.Font.Size = 12;
            RowStyle.Font.Name = "Arial";
            RowStyle.HorizontalAlignment = TextAlignmentType.Center;
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
            #region 累计数据样式
            Aspose.Cells.Style RowSumStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            RowSumStyle.Font.Size = 12;
            RowSumStyle.Font.Name = "Arial";
            RowSumStyle.Number = 1;
            RowSumStyle.ForegroundColor = Color.FromArgb(217, 217, 217);
            RowSumStyle.HorizontalAlignment = TextAlignmentType.Center;
            RowSumStyle.Pattern = BackgroundType.Solid;
            RowSumStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            RowSumStyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            RowSumStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            RowSumStyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            RowSumStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            RowSumStyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            RowSumStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            RowSumStyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region 分解表公司名称
            Aspose.Cells.Style TargetCompanystyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            TargetCompanystyle.Font.Size = 12;
            TargetCompanystyle.Font.Name = "Arial";
            TargetCompanystyle.Font.IsBold = true;
            TargetCompanystyle.HorizontalAlignment = TextAlignmentType.Center;
            TargetCompanystyle.Pattern = BackgroundType.Solid;
            TargetCompanystyle.ForegroundColor = Color.FromArgb(255, 255, 204);
            TargetCompanystyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            TargetCompanystyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            TargetCompanystyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            TargetCompanystyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            TargetCompanystyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            TargetCompanystyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            TargetCompanystyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            TargetCompanystyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region 分解表累计
            Aspose.Cells.Style TargetSumstyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            TargetSumstyle.Font.Size = 12;
            TargetSumstyle.Font.Name = "Arial";
            TargetSumstyle.Number = 1;
            TargetSumstyle.Font.IsBold = true;
            TargetSumstyle.HorizontalAlignment = TextAlignmentType.Center;
            TargetSumstyle.Pattern = BackgroundType.Solid;
            TargetSumstyle.ForegroundColor = Color.FromArgb(253, 213, 180);
            TargetSumstyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            TargetSumstyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            TargetSumstyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            TargetSumstyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            TargetSumstyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            TargetSumstyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            TargetSumstyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            TargetSumstyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion
            #region 表尾
            Aspose.Cells.Style lastStyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            lastStyle.Font.Size = 12;
            lastStyle.Font.Name = "Arial";
            lastStyle.HorizontalAlignment = TextAlignmentType.Center;
            lastStyle.Pattern = BackgroundType.Solid;
            lastStyle.ForegroundColor = Color.FromArgb(189, 215, 238);
            lastStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            lastStyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            lastStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            lastStyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            lastStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            lastStyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            lastStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            lastStyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            Aspose.Cells.Style hejistyle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            hejistyle.Font.Size = 12;
            hejistyle.Font.Name = "Arial";
            hejistyle.Font.IsBold = true;
            hejistyle.Number = 1;
            hejistyle.HorizontalAlignment = TextAlignmentType.Center;
            hejistyle.Pattern = BackgroundType.Solid;
            hejistyle.ForegroundColor = Color.FromArgb(189, 215, 238);
            hejistyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            hejistyle.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            hejistyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            hejistyle.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            hejistyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            hejistyle.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            hejistyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            hejistyle.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;



            #endregion

            //List<DictionaryVmodel> listDV = (List<DictionaryVmodel>)TargetPlanEngine.TargetPlanEngineService.GetTargetPlanSource(SysId, FinYear, TargetPlanID, IsLatestVersion);
            C_System _cSystem = StaticResource.Instance.SystemList.Where(p => p.ID == SysId).FirstOrDefault();


            string strSystemName = _cSystem.SystemName;

            if (IsYearTargetPlan)
            {
                for (int i = 0; i < listDV.Count; i++)
                {
                    if (i > 0)
                    {
                        worksheets.AddCopy(0);
                    }
                    worksheets[i].Name = listDV[i].Name;
                    excel.SetCustomProperty(worksheets[0], "SystemID", SysId.ToString());
                    excel.SetCustomProperty(worksheets[0], "SheetName", "UpTargetPlanDetail");
                    worksheets[i].Cells[0, 1].PutValue(FinYear + "年" + strSystemName + listDV[i].Name + "指标分解表");

                    C_Target TargetUnit = StaticResource.Instance.GetTargetList(SysId, DateTime.Now).Where(p => p.TargetName == listDV[i].Name).FirstOrDefault();
                    //worksheets[i].Cells[1, 2].PutValue(TargetUnit.Unit);
                    if (!string.IsNullOrEmpty(TargetUnit.Unit))
                    {
                        worksheets[i].Cells[1, 2].PutValue(TargetUnit.Unit);
                    }
                    else
                    {
                        worksheets[i].Cells[1, 2].PutValue("万元");
                    }


                }
            }
            else
            {

                for (int i = 0; i < listDV.Count; i++)
                {
                    if (i > 0)
                    {

                        worksheets.AddCopy(1);
                    }
                    worksheets[i + 1].Name = listDV[i].Name;
                    excel.SetCustomProperty(worksheets[0], "SystemID", SysId.ToString());
                    excel.SetCustomProperty(worksheets[0], "SheetName", "UpTargetPlanDetail");
                    worksheets[i + 1].Cells[0, 1].PutValue(FinYear + "年" + strSystemName + listDV[i].Name + "指标分解表");

                    C_Target TargetUnit = StaticResource.Instance.GetTargetList(SysId, DateTime.Now).Where(p => p.TargetName == listDV[i].Name).FirstOrDefault();
                    //worksheets[i+1].Cells[1, 2].PutValue(TargetUnit.Unit);
                    if (!string.IsNullOrEmpty(TargetUnit.Unit))
                    {
                        worksheets[i + 1].Cells[1, 2].PutValue(TargetUnit.Unit);
                    }
                    else
                    {
                        worksheets[i + 1].Cells[1, 2].PutValue("万元");
                    }

                }
            }

            //创建指标Sheet,
            for (int sheetIndex = 0; sheetIndex < listDV.Count; sheetIndex++)
            {
                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列
                List<DictionaryVmodel> listCompanyDV = (List<DictionaryVmodel>)listDV[sheetIndex].ObjValue;
                for (int i = 0; i < listCompanyDV.Count; i++)
                {
                    if (!IsYearTargetPlan)
                    {
                        Worksheet worksheet = worksheets[sheetIndex + 1];
                        #region 设置样式
                        worksheet.Cells[rowStart, colStart].SetStyle(style3);
                        worksheet.Cells[rowStart, colStart + 1].SetStyle(style3);
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                        worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                        #endregion

                        #region 赋值
                        worksheet.Cells[rowStart, colStart].PutValue(i + 1);
                        worksheet.Cells[rowStart, colStart + 1].PutValue(listCompanyDV[i].Name);
                        DateTime dtime = StaticResource.Instance.CompanyList[SysId].Where(p => p.CompanyName == listCompanyDV[i].Name).FirstOrDefault().OpeningTime;
                        if (dtime != DateTime.MinValue)
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(dtime.ToShortDateString());
                        }
                        worksheet.Cells[rowStart, colStart + 3].Formula = "=SUM(F" + (rowStart + 1) + ":Q" + (rowStart + 1) + ")";
                        #endregion
                        List<B_TargetPlanDetail> listBTargetPlanDetail = (List<B_TargetPlanDetail>)listCompanyDV[i].ObjValue;
                        style3 = worksheet.Cells[rowStart, colStart + 3].GetStyle();
                        style3.Number = 3;
                        worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);

                        for (int j = 0; j < 12; j++)
                        {
                            #region 设置样式
                            worksheet.Cells[rowStart, colStart + 4 + j].SetStyle(style2);

                            style2 = worksheet.Cells[rowStart, colStart + 4 + j].GetStyle();
                            style2.Number = 3;
                            worksheet.Cells[rowStart, colStart + 4 + j].SetStyle(style2);
                            #endregion
                            #region 赋值
                            List<B_TargetPlanDetail> singleMonthTargetPlanDetail = listBTargetPlanDetail == null ? null : listBTargetPlanDetail.Where(p => p.FinMonth == (j + 1)).ToList();
                            worksheet.Cells[rowStart, colStart + 4 + j].PutValue(0);
                            if (singleMonthTargetPlanDetail != null)
                            {
                                if (singleMonthTargetPlanDetail.Count() > 0)
                                {
                                    //var TargetStr = string.Format("{0:N0}", singleMonthTargetPlanDetail.FirstOrDefault().Target);
                                    worksheet.Cells[rowStart, colStart + 4 + j].PutValue(singleMonthTargetPlanDetail.FirstOrDefault().Target);
                                }
                            }
                            #endregion
                        }


                    }
                    else
                    {
                        Worksheet worksheet = worksheets[sheetIndex];
                        #region 设置样式
                        worksheet.Cells[rowStart, colStart].SetStyle(style3);
                        worksheet.Cells[rowStart, colStart + 1].SetStyle(style3);
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                        #endregion

                        #region 赋值
                        worksheet.Cells[rowStart, colStart].PutValue(i + 1);
                        worksheet.Cells[rowStart, colStart + 1].PutValue(listCompanyDV[i].Name);
                        //判断参数是否包含小数点。

                        if (!string.IsNullOrEmpty(listCompanyDV[i].Mark))
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(decimal.Parse(listCompanyDV[i].Mark));
                        }
                        else
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(0.0);
                        }

                        #endregion
                        style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                        style2.Number = 3;
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                    }
                    rowStart++;

                }
                worksheets[sheetIndex].Protect(ProtectionType.All);
            }
            string Titles = FinYear + "年" + strSystemName + "经营指标分解汇总表";//表头名称

            #region 汇总部分
            if (!IsYearTargetPlan)
            {

                for (int i = 1; i <= 16; i++)//除了第一行,汇总表所有用到的行行高均为20
                {
                    worksheets[0].Cells.SetRowHeight(i, 20);
                }
                worksheets[0].Cells.Merge(0, 1, 1, (listDV.Count) * 2 + 1);//为标题栏合并列,所需要合并列的数量为 指标数量*2+1
                worksheets[0].Cells[0, 1].PutValue(Titles);
                worksheets[0].Cells[0, 1].SetStyle(StyleTitle);


                //为表格设置列宽

                for (int i = 1; i <= (listDV.Count) * 2; i++)
                {
                    worksheets[0].Cells.SetColumnWidth(i + 1, 18);
                }

                worksheets[0].Cells[1, 1].PutValue("年度：" + FinYear);
                worksheets[0].Cells[1, 1].SetStyle(StyleTitle2);
                worksheets[0].Cells[1, (listDV.Count) * 2 + 1].PutValue("单位：万元");
                worksheets[0].Cells[1, (listDV.Count) * 2 + 1].SetStyle(StyleTitle2);


                //创建表头
                worksheets[0].Cells.Merge(2, 1, 2, 1);
                worksheets[0].Cells.Merge(2, 2, 1, listDV.Count);//合并列
                worksheets[0].Cells.Merge(2, 2 + listDV.Count, 1, listDV.Count);//合并列
                worksheets[0].Cells[2, 1].PutValue("月份");
                worksheets[0].Cells[2, 1].SetStyle(StyleTitle3);
                worksheets[0].Cells[3, 1].SetStyle(StyleTitle3);
                worksheets[0].Cells[2, 2].PutValue("当月数");
                worksheets[0].Cells[2, 2 + listDV.Count].PutValue("累计数");
                worksheets[0].Cells[2, 2].SetStyle(StyleTitle3);
                worksheets[0].Cells[2, 2 + listDV.Count].SetStyle(StyleTitle3);



                int Nowcol = 2;
                int Allcol = listDV.Count + 2;
                //为表头赋值(指标名称)
                for (int i = 0; i < listDV.Count; i++)
                {
                    worksheets[0].Cells[3, Nowcol + i].PutValue(listDV[i].Name);
                    worksheets[0].Cells[3, Allcol + i].PutValue(listDV[i].Name);
                    worksheets[0].Cells[2, Nowcol + i].SetStyle(StyleTitle3);
                    worksheets[0].Cells[2, Allcol + i].SetStyle(StyleTitle3);
                    worksheets[0].Cells[3, Nowcol + i].SetStyle(StyleTitle3);
                    worksheets[0].Cells[3, Allcol + i].SetStyle(StyleTitle3);
                }





                //给表格内部赋值

                List<TargetDetail> TargetDetail = A_TargetplandetailOperator.Instance.GetSumMonthTargetDetailByTID(TargetPlanID);
                for (int a = 0; a < 12; a++)
                {
                    worksheets[0].Cells[4 + a, 1].PutValue(a + 1 + "月");
                    worksheets[0].Cells[4 + a, 1].SetStyle(monthstyle);
                }
                for (int i = 0; i < listDV.Count; i++)
                {
                    if (i + 1 <= TargetDetail[0].TargetDetailList.Count)
                    {
                        for (int a = 0; a < TargetDetail.Count; a++)
                        {
                            if (TargetDetail[a].TargetDetailList[i].Target != null)
                            {
                                worksheets[0].Cells[4 + a, Nowcol + i].PutValue(TargetDetail[a].TargetDetailList[i].Target);
                                worksheets[0].Cells[4 + a, Nowcol + i].SetStyle(RowStyle);
                            }
                            else
                            {
                                worksheets[0].Cells[4 + a, Nowcol + i].PutValue("--");
                                worksheets[0].Cells[4 + a, Nowcol + i].SetStyle(RowStyle);
                            }
                            if (TargetDetail[a].TargetDetailList[i].SumTarget != null)
                            {
                                worksheets[0].Cells[4 + a, Allcol + i].PutValue(TargetDetail[a].TargetDetailList[i].SumTarget);
                                worksheets[0].Cells[4 + a, Allcol + i].SetStyle(RowStyle);
                            }
                            else
                            {
                                worksheets[0].Cells[4 + a, Allcol + i].PutValue("--");
                                worksheets[0].Cells[4 + a, Allcol + i].PutValue("--");
                            }
                        }

                    }
                    else
                    {
                        for (int s = 0; s < 12; s++)
                        {
                            worksheets[0].Cells[4 + s, Nowcol + i].PutValue("--");
                            worksheets[0].Cells[4 + s, Allcol + i].PutValue("--");
                            worksheets[0].Cells[4 + s, Nowcol + i].SetStyle(RowStyle);
                            worksheets[0].Cells[4 + s, Allcol + i].SetStyle(RowStyle);
                        }
                    }
                }

                worksheets[0].Cells[16, 1].PutValue("合计");
                worksheets[0].Cells[16, 1].SetStyle(hejistyle);
                for (int i = 0; i < listDV.Count; i++)
                {
                    if (i + 1 <= TargetDetail[11].TargetDetailList.Count)
                    {
                        if (TargetDetail[11].TargetDetailList[i].SumTarget != null)
                        {
                            worksheets[0].Cells[16, i + 2].PutValue(TargetDetail[11].TargetDetailList[i].SumTarget);
                            worksheets[0].Cells[16, i + 2 + listDV.Count].PutValue("--");
                        }
                        else
                        {
                            worksheets[0].Cells[16, i + 2].PutValue("--");
                            worksheets[0].Cells[16, i + 2 + listDV.Count].PutValue("--");
                        }
                    }
                    else
                    {
                        worksheets[0].Cells[16, i + 2].PutValue("--");
                        worksheets[0].Cells[16, i + 2 + listDV.Count].PutValue("--");
                    }

                    worksheets[0].Cells[16, i + 2].SetStyle(lastStyle);
                    worksheets[0].Cells[16, i + 2 + listDV.Count].SetStyle(lastStyle);
                }

            }
            #endregion
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(StaticResource.Instance.SystemList.Where(p => p.ID == SysId).FirstOrDefault().SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + "_" + dateNow + ".xls");
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