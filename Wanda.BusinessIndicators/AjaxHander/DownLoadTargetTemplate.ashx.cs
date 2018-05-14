using Aspose.Cells;
using Lib.Config;
using Lib.Xml;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Common.Web;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownLoadTargetTemplate 的摘要说明
    /// </summary>
    public class DownLoadTargetTemplate : IHttpHandler
    {
        /// <summary>
        /// 下载模板地址
        /// </summary>
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



        string FileType = string.Empty; //下载的文件类型
        Guid SysId = Guid.Empty;

        int FinYear = 0;
        int FinMonth = 0;
        Guid MonthReportID = Guid.Empty;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpPostedFile file = context.Request.Files["FileData"];

            if (HttpContext.Current.Request.QueryString["FileType"] != null)
            {
                FileType = HttpContext.Current.Request.QueryString["FileType"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysId"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SysId"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear = Convert.ToInt32(HttpContext.Current.Request["FinYear"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinMonth"]))
            {
                FinMonth = Convert.ToInt32(HttpContext.Current.Request["FinMonth"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["MonthReportID"]))
            {
                MonthReportID = HttpContext.Current.Request["MonthReportID"].ToGuid();
            }

            switch (FileType)
            {
                case "DownTargetPlan":
                    DownTargetPlanExcel(context);
                    break;

                case "DownMonthReport":
                    DownExcelMonthReportDetail(context);
                    break;

                case "DownProMonthReport":
                    DownExcelProCompanyReportDetail(context);
                    break;

                case "DownProTargetPlan":
                    DownProTargetPlanExcel(context);
                    break;

                case "DownGroupTargetPlan":
                case "DownGroupMonthReport":
                    DownGroupTargetPlanExcel(context, FileType);
                    break;
                case "DownDirectlyTargetPlan":
                case "DownDirectlyMonthReport":
                    DownDirectLyTargetPlanExcel(context, FileType);
                    break;
            }

        }

        #region 根据是不是混合指标来确定导出的excel

        public void DownExcelMonthReportDetail(HttpContext context)
        {
            ReportInstance rpt = new ReportInstance(MonthReportID, true);

            var blendResult = GetBlendTargets(rpt);
            //item1等于true 时，表示是混合指标
            if (blendResult.Item1)
                DownExcelMonthReportDetailForBlend(context, rpt, blendResult.Item2);
            else
                DownExcelMonthReportDetailForOrigina(context, rpt);
        }
        /// <summary>
        /// 指标excel 导出(原来的)
        /// </summary>
        /// <param name="context"></param>
        private void DownExcelMonthReportDetailForOrigina(HttpContext context, ReportInstance rpt)
        {
            string templeteName = "指标上报模版V1.xlsx";
            string fileName = "月度经营报告上报";

            // ReportInstance rpt = new ReportInstance(MonthReportID, true);
            string str;
            bool IncludeHaveDetail = true;
            List<DictionaryVmodel> listMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "Reported", "Detail", IncludeHaveDetail);
            if (listMonthReportDetail.Count > 0)
            {
                string[] strHtmlTemplates = listMonthReportDetail[0].HtmlTemplate.Split(',');
                if (strHtmlTemplates.Count() > 4)
                {
                    if (!string.IsNullOrEmpty(strHtmlTemplates[4]))
                        templeteName = strHtmlTemplates[4];
                }
            }
            string templetePath = Path.Combine(ExcelTempletePath, templeteName);
            if (System.IO.File.Exists(templetePath))
            {


                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();

                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;


                Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 12;
                #endregion
                #region style2 样式 无加粗
                style2.Font.Size = 12;

                style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                style2.ForegroundColor = System.Drawing.Color.LightGray;
                style2.Pattern = BackgroundType.Solid;
                #endregion
                #region style3 样式
                style3.Font.Size = 12;
                style3.Font.Name = "Arial";
                style3.Font.IsBold = true;
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

                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列

                for (int z = 0; z < listMonthReportDetail.Count; z++)
                {
                    if (z > 0 && templeteName == "指标上报模版V1.xlsx")
                    {
                        worksheets.AddCopy(0);
                    }
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == listMonthReportDetail[z].Name).ToList();
                    if (listTarget.Count > 0)
                    {
                        excel.SetCustomProperty(worksheets[z], "SystemID", listTarget[0].SystemID.ToString());
                        excel.SetCustomProperty(worksheets[z], "TragertID", listTarget[0].ID.ToString());
                        excel.SetCustomProperty(worksheets[z], "TragertName", listTarget[0].TargetName);
                        excel.SetCustomProperty(worksheets[z], "SheetName", "MonthReportDetail");
                        excel.SetCustomProperty(worksheets[z], "AreaID", rpt.AreaID.ToString());
                    }
                    worksheets[z].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[z].Name);
                    worksheets[z].Cells[1, 3].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[z].Cells[1, 3].SetStyle(style1);
                    if (templeteName == "指标上报模版V1.xlsx")
                    {
                        worksheets[z].Name = listMonthReportDetail[z].Name;
                    }
                }


                for (int i = 0; i < listMonthReportDetail.Count; i++)
                {
                    rowStart = 4;
                    List<DictionaryVmodel> listCompanyProperty = (List<DictionaryVmodel>)listMonthReportDetail[i].ObjValue;

                    #region 判断指标是否可以编辑
                    bool IsModifyTargetPlanDetail = false; //指标是否可以编辑
                    bool IsHaveAccumulativePlanDetail = false;
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == listMonthReportDetail[i].Name).ToList();
                    if (listTarget.Count > 0)
                    {
                        XElement xmlTarget = listTarget[0].Configuration;
                        if (xmlTarget != null)
                        {
                            IsModifyTargetPlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[0].ObjValue;
                            IsHaveAccumulativePlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[1].ObjValue;
                        }
                    }
                    #endregion
                    int MonthReportDetailCount = 0;
                    for (int j = 0; j < listCompanyProperty.Count; j++)
                    {
                        if (listCompanyProperty[j].Name == "HaveDetail")
                        {
                            List<MonthlyReportDetail> listHaveDetail = (List<MonthlyReportDetail>)listCompanyProperty[j].ObjValue;
                            MonthReportDetailCount += listHaveDetail.Count;
                            for (int k = 0; k < listHaveDetail.Count; k++)
                            {
                                MonthlyReportDetail mrd = listHaveDetail[k];
                                worksheets[i].Cells[rowStart, colStart].SetStyle(style2);
                                worksheets[i].Cells[rowStart, colStart + 1].SetStyle(style2);
                                worksheets[i].Cells[rowStart, colStart + 2].SetStyle(style2);
                                worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style2);
                                worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style2);
                                worksheets[i].Cells[rowStart, colStart].PutValue(rowStart - 3);
                                worksheets[i].Cells[rowStart, colStart + 1].PutValue(mrd.CompanyID);
                                worksheets[i].Cells[rowStart, colStart + 2].PutValue(mrd.CompanyName);
                                worksheets[i].Cells[rowStart, colStart + 3].PutValue(mrd.NPlanAmmount);
                                worksheets[i].Cells[rowStart, colStart + 4].PutValue(mrd.NActualAmmount);

                                #region 仅商管系统用
                                if (IsHaveAccumulativePlanDetail)
                                {
                                    worksheets[i].Cells[rowStart, colStart + 5].PutValue(mrd.NAccumulativePlanAmmount);
                                    worksheets[i].Cells[rowStart, colStart + 6].PutValue(mrd.NAccumulativeActualAmmount);
                                }
                                #endregion
                                style2 = worksheets[i].Cells[rowStart, colStart + 3].GetStyle();
                                style2.Number = 3;
                                worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style2);
                                worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style2);

                                #region 仅商管系统用
                                if (IsHaveAccumulativePlanDetail)
                                {
                                    worksheets[i].Cells[rowStart, colStart + 5].SetStyle(style2);
                                    worksheets[i].Cells[rowStart, colStart + 6].SetStyle(style2);
                                }
                                #endregion

                                rowStart = rowStart + 1;
                            }
                        }
                        else if (listCompanyProperty[j].Name == "SummaryData")
                        {
                        }
                        else
                        {
                            List<DictionaryVmodel> listCounter = (List<DictionaryVmodel>)listCompanyProperty[j].ObjValue;
                            for (int z = 0; z < listCounter.Count; z++)
                            {
                                List<MonthlyReportDetail> listMonthReportDetil = (List<MonthlyReportDetail>)listCounter[z].ObjValue;
                                MonthReportDetailCount += listMonthReportDetil.Count;
                                if (listMonthReportDetil != null)
                                {
                                    for (int a = 0; a < listMonthReportDetil.Count; a++)
                                    {
                                        MonthlyReportDetail mrd = listMonthReportDetil[a];
                                        worksheets[i].Cells[rowStart, colStart].SetStyle(style2);
                                        worksheets[i].Cells[rowStart, colStart + 1].SetStyle(style2);
                                        worksheets[i].Cells[rowStart, colStart + 2].SetStyle(style2);
                                        worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style2);
                                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style2);
                                        worksheets[i].Cells[rowStart, colStart].PutValue(rowStart - 3);
                                        worksheets[i].Cells[rowStart, colStart + 1].PutValue(mrd.CompanyID);
                                        worksheets[i].Cells[rowStart, colStart + 2].PutValue(mrd.CompanyName);
                                        worksheets[i].Cells[rowStart, colStart + 3].PutValue(mrd.NPlanAmmount);
                                        worksheets[i].Cells[rowStart, colStart + 4].PutValue(mrd.NActualAmmount);

                                        #region 仅商管系统用
                                        if (IsHaveAccumulativePlanDetail)
                                        {
                                            worksheets[i].Cells[rowStart, colStart + 5].PutValue(mrd.NAccumulativePlanAmmount);
                                            worksheets[i].Cells[rowStart, colStart + 6].PutValue(mrd.NAccumulativeActualAmmount);
                                        }
                                        #endregion

                                        style2 = worksheets[i].Cells[rowStart, colStart + 3].GetStyle();
                                        style2.Number = 3;
                                        worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style2);
                                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style2);

                                        #region 仅商管系统用
                                        if (IsHaveAccumulativePlanDetail)
                                        {
                                            worksheets[i].Cells[rowStart, colStart + 5].SetStyle(style2);
                                            worksheets[i].Cells[rowStart, colStart + 6].SetStyle(style2);
                                        }
                                        #endregion

                                        rowStart = rowStart + 1;
                                    }
                                }
                            }
                        }
                    }

                    #region 设置保护单元格
                    Style style;
                    StyleFlag styleflag;
                    rowStart = 4;
                    for (int x = 0; x <= 255; x++)
                    {
                        style = worksheets[i].Cells.Columns[(byte)x].Style;
                        style.IsLocked = true;
                        styleflag = new StyleFlag();
                        styleflag.Locked = true;
                        worksheets[i].Cells.Columns[(byte)x].ApplyStyle(style, styleflag);
                    }
                    for (int j = 0; j < MonthReportDetailCount; j++)
                    {
                        style = worksheets[i].Cells[rowStart, colStart + 3].GetStyle();
                        style.IsLocked = false;
                        style.ForegroundColor = System.Drawing.Color.White;
                        if (IsModifyTargetPlanDetail)
                        {
                            worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style);

                        }
                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style);
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i].Cells[rowStart, colStart + 6].SetStyle(style);
                            if (IsModifyTargetPlanDetail)
                            {
                                worksheets[i].Cells[rowStart, colStart + 5].SetStyle(style);
                            }
                        }
                        rowStart = rowStart + 1;
                    }

                    worksheets[i].Protect(ProtectionType.All);
                    #endregion
                }

                var AreaName = "";
                if (rpt.AreaID != Guid.Empty)
                {
                    AreaName = "-" + S_OrganizationalActionOperator.Instance.GetDataByID(rpt.AreaID).CnName + "-";
                }

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                //设置基本信息   
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                string dateNow = DateTime.Now.ToString("HHmmss");
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + AreaName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }
        }

        /// <summary>
        ///  指标excel 导出(混合指标)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rpt"></param>
        /// <param name="blendTargetList"></param>
        private void DownExcelMonthReportDetailForBlend(HttpContext context, ReportInstance rpt, List<C_Target> blendTargetList)
        {
            string templeteName = "指标上报模版_混合V1.xlsx";
            string fileName = "月度经营报告上报";
            bool IncludeHaveDetail = true;
            // ReportInstance rpt = new ReportInstance(MonthReportID, true);
            //List<DictionaryVmodel> listTargetPlanView = new List<DictionaryVmodel>();
            List<DictionaryVmodel> listMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "Reported", "Detail", IncludeHaveDetail);
            //判断下是否是 国内院线
            //string _sysMovie = AppSettingConfig.GetSetting("MovieCN", "");
            //if (rpt._SystemID == Guid.Parse(_sysMovie))
            //    listTargetPlanView = rpt.GetTagetPlanViewModel(Guid.Parse(_sysMovie));  //院线同步数据
            //else
            //    listTargetPlanView = rpt.GetTagetPlanViewModel();  // 其他系统


            if (listMonthReportDetail.Count > 0)
            {
                string[] strHtmlTemplates = listMonthReportDetail[0].HtmlTemplate.Split(',');
                if (strHtmlTemplates.Count() > 4)
                {
                    if (!string.IsNullOrEmpty(strHtmlTemplates[4]))
                        templeteName = strHtmlTemplates[4];
                }
            }


            string templetePath = Path.Combine(ExcelTempletePath, templeteName);
            if (System.IO.File.Exists(templetePath))
            {
                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;
                Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 12;
                #endregion
                #region style2 样式 无加粗
                style2.Font.Size = 12;

                style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                style2.ForegroundColor = System.Drawing.Color.LightGray;
                style2.Pattern = BackgroundType.Solid;

                #endregion
                #region style3 样式
                style3.Font.Size = 12;
                style3.Font.Name = "Arial";
                style3.Font.IsBold = true;
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

                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列

                //插入混合指标
                InsertBlendTargetToExcelForDownload(excel, worksheets[0], rpt, listMonthReportDetail, blendTargetList, fileName, rowStart, colStart, style2);

                var otherListTargetPlanView = listMonthReportDetail.Where(v => !v.IsBlendTarget).ToList();
                //如果只有混合指标，则删除模板中的Sheet2
                if (otherListTargetPlanView == null || otherListTargetPlanView.Count == 0)
                {
                    worksheets.RemoveAt("Sheet2");
                }

                #region 复制sheet
                for (int z = 0; z < otherListTargetPlanView.Count; z++)
                {
                    if (z > 0 && templeteName == "指标上报模版_混合V1.xlsx")
                    {
                        worksheets.AddCopy(1);
                    }
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == otherListTargetPlanView[z].Name).ToList();
                    if (listTarget != null && listTarget.Count > 0)
                    {
                        excel.SetCustomProperty(worksheets[z + 1], "SystemID", listTarget[0].SystemID.ToString());
                        excel.SetCustomProperty(worksheets[z + 1], "TragertID", listTarget[0].ID.ToString());
                        excel.SetCustomProperty(worksheets[z + 1], "TragertName", listTarget[0].TargetName);
                        excel.SetCustomProperty(worksheets[z + 1], "SheetName", "MonthReportDetail");
                        excel.SetCustomProperty(worksheets[z + 1], "AreaID", rpt.AreaID.ToString());
                    }
                    worksheets[z + 1].Cells[0, 1].PutValue(rpt._System.SystemName + otherListTargetPlanView[z].Name);
                    worksheets[z + 1].Cells[2, 4].PutValue(string.Format(@"{0}月情况 ", FinMonth));
                    worksheets[z + 1].Cells[1, 3].SetStyle(style1);
                    if (templeteName == "指标上报模版_混合V1.xlsx")
                    {
                        worksheets[z + 1].Name = otherListTargetPlanView[z].Name;
                    }

                }

                #endregion

                for (int i = 0; i < otherListTargetPlanView.Count; i++)
                {
                    rowStart = 4;
                    List<DictionaryVmodel> listCompanyProperty = (List<DictionaryVmodel>)otherListTargetPlanView[i].ObjValue;

                    #region 判断指标是否可以编辑
                    bool IsModifyTargetPlanDetail = false; //指标是否可以编辑
                    bool IsHaveAccumulativePlanDetail = false;
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == otherListTargetPlanView[i].Name).ToList();
                    if (listTarget.Count > 0)
                    {
                        XElement xmlTarget = listTarget[0].Configuration;
                        if (xmlTarget != null)
                        {
                            IsModifyTargetPlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[0].ObjValue;
                            IsHaveAccumulativePlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[1].ObjValue;
                        }
                    }
                    #endregion
                    int MonthReportDetailCount = 0;
                    for (int j = 0; j < listCompanyProperty.Count; j++)
                    {
                        if (listCompanyProperty[j].Name == "HaveDetail")
                        {
                            List<MonthlyReportDetail> listHaveDetail = (List<MonthlyReportDetail>)listCompanyProperty[j].ObjValue;
                            MonthReportDetailCount += listHaveDetail.Count;
                            for (int k = 0; k < listHaveDetail.Count; k++)
                            {
                                MonthlyReportDetail mrd = listHaveDetail[k];
                                worksheets[i + 1].Cells[rowStart, colStart].SetStyle(style2);
                                worksheets[i + 1].Cells[rowStart, colStart + 1].SetStyle(style2);
                                worksheets[i + 1].Cells[rowStart, colStart + 2].SetStyle(style2);
                                worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(style2);
                                worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style2);
                                worksheets[i + 1].Cells[rowStart, colStart].PutValue(rowStart - 3);
                                worksheets[i + 1].Cells[rowStart, colStart + 1].PutValue(mrd.CompanyID);
                                worksheets[i + 1].Cells[rowStart, colStart + 2].PutValue(mrd.CompanyName);
                                worksheets[i + 1].Cells[rowStart, colStart + 3].PutValue(mrd.NPlanAmmount);
                                worksheets[i + 1].Cells[rowStart, colStart + 4].PutValue(mrd.NActualAmmount);

                                #region 仅商管系统用
                                if (IsHaveAccumulativePlanDetail)
                                {
                                    worksheets[i + 1].Cells[rowStart, colStart + 5].PutValue(mrd.NAccumulativePlanAmmount);
                                    worksheets[i + 1].Cells[rowStart, colStart + 6].PutValue(mrd.NAccumulativeActualAmmount);
                                }
                                #endregion
                                style2 = worksheets[i + 1].Cells[rowStart, colStart + 3].GetStyle();
                                style2.Number = 3;
                                worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(style2);
                                worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style2);

                                #region 仅商管系统用
                                if (IsHaveAccumulativePlanDetail)
                                {
                                    worksheets[i + 1].Cells[rowStart, colStart + 5].SetStyle(style2);
                                    worksheets[i + 1].Cells[rowStart, colStart + 6].SetStyle(style2);
                                }
                                #endregion

                                rowStart = rowStart + 1;
                            }
                        }
                        else if (listCompanyProperty[j].Name == "SummaryData")
                        {
                        }
                        else
                        {
                            List<DictionaryVmodel> listCounter = (List<DictionaryVmodel>)listCompanyProperty[j].ObjValue;
                            for (int z = 0; z < listCounter.Count; z++)
                            {
                                List<MonthlyReportDetail> listMonthReportDetil = (List<MonthlyReportDetail>)listCounter[z].ObjValue;
                                MonthReportDetailCount += listMonthReportDetil.Count;
                                if (listMonthReportDetil != null)
                                {
                                    for (int a = 0; a < listMonthReportDetil.Count; a++)
                                    {
                                        MonthlyReportDetail mrd = listMonthReportDetil[a];
                                        worksheets[i + 1].Cells[rowStart, colStart].SetStyle(style2);
                                        worksheets[i + 1].Cells[rowStart, colStart + 1].SetStyle(style2);
                                        worksheets[i + 1].Cells[rowStart, colStart + 2].SetStyle(style2);
                                        worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(style2);
                                        worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style2);
                                        worksheets[i + 1].Cells[rowStart, colStart].PutValue(rowStart - 3);
                                        worksheets[i + 1].Cells[rowStart, colStart + 1].PutValue(mrd.CompanyID);
                                        worksheets[i + 1].Cells[rowStart, colStart + 2].PutValue(mrd.CompanyName);
                                        worksheets[i + 1].Cells[rowStart, colStart + 3].PutValue(mrd.NPlanAmmount);
                                        worksheets[i + 1].Cells[rowStart, colStart + 4].PutValue(mrd.NActualAmmount);

                                        #region 仅商管系统用
                                        if (IsHaveAccumulativePlanDetail)
                                        {
                                            worksheets[i + 1].Cells[rowStart, colStart + 5].PutValue(mrd.NAccumulativePlanAmmount);
                                            worksheets[i + 1].Cells[rowStart, colStart + 6].PutValue(mrd.NAccumulativeActualAmmount);
                                        }
                                        #endregion

                                        style2 = worksheets[i + 1].Cells[rowStart, colStart + 3].GetStyle();
                                        style2.Number = 3;
                                        worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(style2);
                                        worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style2);

                                        #region 仅商管系统用
                                        if (IsHaveAccumulativePlanDetail)
                                        {
                                            worksheets[i + 1].Cells[rowStart, colStart + 5].SetStyle(style2);
                                            worksheets[i + 1].Cells[rowStart, colStart + 6].SetStyle(style2);
                                        }
                                        #endregion

                                        rowStart = rowStart + 1;
                                    }
                                }
                            }
                        }
                    }

                    #region 设置保护单元格
                    Style style;
                    StyleFlag styleflag;
                    rowStart = 4;
                    for (int x = 0; x <= 255; x++)
                    {
                        style = worksheets[i + 1].Cells.Columns[(byte)x].Style;
                        style.IsLocked = true;
                        styleflag = new StyleFlag();
                        styleflag.Locked = true;

                        worksheets[i + 1].Cells.Columns[(byte)x].ApplyStyle(style, styleflag);
                    }
                    for (int j = 0; j < MonthReportDetailCount; j++)
                    {
                        style = worksheets[i + 1].Cells[rowStart, colStart + 3].GetStyle();
                        style.IsLocked = false;
                        style.ForegroundColor = System.Drawing.Color.White;
                        worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style);
                        rowStart = rowStart + 1;
                    }

                    worksheets[i + 1].Protect(ProtectionType.All);
                    #endregion
                }
                var AreaName = "";
                if (rpt.AreaID != Guid.Empty)
                {
                    AreaName = "-" + S_OrganizationalActionOperator.Instance.GetDataByID(rpt.AreaID).CnName + "-";
                }

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                //设置基本信息   
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                string dateNow = DateTime.Now.ToString("HHmmss");
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + fileName + AreaName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }
        }

        /// <summary>
        /// 只导出混合指标的
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="ws"></param>
        /// <param name="rpt"></param>
        /// <param name="listTargetPlanView"></param>
        /// <param name="blendTargetList"></param>
        /// <param name="fileName"></param>
        /// <param name="rowStart"></param>
        /// <param name="colStart"></param>
        /// <param name="style2"></param>
        private void InsertBlendTargetToExcelForDownload(ExcelEngine excel, Worksheet ws, ReportInstance rpt,
            List<DictionaryVmodel> listMonthReportDetail, List<C_Target> blendTargetList, string fileName,
            int rowStart, int colStart, Style style2)
        {
            #region 混合指标
            ws.Name = string.Format(@"{0}+{1}", blendTargetList[0].TargetName, blendTargetList[1].TargetName);
            excel.SetCustomProperty(ws, "IsBlendTager", "1");
            excel.SetCustomProperty(ws, "SystemID", blendTargetList[0].SystemID.ToString());
            excel.SetCustomProperty(ws, "TragertID", blendTargetList[0].ID.ToString());
            excel.SetCustomProperty(ws, "TragertName", blendTargetList[0].TargetName);
            excel.SetCustomProperty(ws, "TragertTwoID", blendTargetList[1].ID.ToString());
            excel.SetCustomProperty(ws, "TragertTwoName", blendTargetList[1].TargetName);
            excel.SetCustomProperty(ws, "AreaID", rpt.AreaID.ToString());
            excel.SetCustomProperty(ws, "SheetName", "MonthReportDetail");
            ws.Cells[0, 1].PutValue(string.Format(@"{0}年度{1}{2}", FinYear, rpt._System.SystemName, fileName));
            ws.Cells[1, 4].PutValue(string.Format(@"{0}月情况", FinMonth));
            ws.Cells[2, 4].PutValue(string.Format(@"{0}月计划", FinMonth));
            ws.Cells[2, 6].PutValue(string.Format(@"{0}月完成（金额）", FinMonth));
            ws.Cells[3, 4].PutValue(string.Format(@"{0}", blendTargetList[0].TargetName));
            ws.Cells[3, 5].PutValue(string.Format(@"{0}", blendTargetList[1].TargetName));
            ws.Cells[3, 6].PutValue(string.Format(@"{0}", blendTargetList[0].TargetName));
            ws.Cells[3, 7].PutValue(string.Format(@"{0}", blendTargetList[1].TargetName));
            //List<DictionaryVmodel> oneBlendTarget = (List<DictionaryVmodel>)listMonthReportDetail.Where(v => v.Name == blendTargetList[0].TargetName).FirstOrDefault().ObjValue;
            //List<DictionaryVmodel> twoBlendTarget = (List<DictionaryVmodel>)listMonthReportDetail.Where(v => v.Name == blendTargetList[1].TargetName).FirstOrDefault().ObjValue;

            List<DictionaryVmodel> oneBlendTarget = (List<DictionaryVmodel>)((List<DictionaryVmodel>)listMonthReportDetail.Where(v => v.IsBlendTarget).FirstOrDefault().ObjValue).Where(v => v.Name == blendTargetList[0].TargetName).FirstOrDefault().ObjValue;
            List<DictionaryVmodel> twoBlendTarget = (List<DictionaryVmodel>)((List<DictionaryVmodel>)listMonthReportDetail.Where(v => v.IsBlendTarget).FirstOrDefault().ObjValue).Where(v => v.Name == blendTargetList[1].TargetName).FirstOrDefault().ObjValue;


            List<MonthlyReportDetail> oneListMonthReportDetil = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> twoListMonthReportDetil = new List<MonthlyReportDetail>();

            //第一个指标
            for (int i = 0; i < oneBlendTarget.Count; i++)
            {
                if (oneBlendTarget[i].Name == "HaveDetail" && ((List<MonthlyReportDetail>)oneBlendTarget[i].ObjValue).Count > 0)
                    oneListMonthReportDetil.AddRange((List<MonthlyReportDetail>)oneBlendTarget[i].ObjValue);
                else if (oneBlendTarget[i].Name == "SummaryData")
                {
                }
                else
                {
                    List<DictionaryVmodel> oneListHaveDetail = (List<DictionaryVmodel>)oneBlendTarget[i].ObjValue;
                    for (int z = 0; z < oneListHaveDetail.Count; z++)
                    {
                        if (((List<MonthlyReportDetail>)oneListHaveDetail[z].ObjValue).Count > 0)
                            oneListMonthReportDetil.AddRange((List<MonthlyReportDetail>)oneListHaveDetail[z].ObjValue);
                    }

                }
            }

            //第二个指标
            for (int i = 0; i < twoBlendTarget.Count; i++)
            {
                if (twoBlendTarget[i].Name == "HaveDetail" && ((List<MonthlyReportDetail>)twoBlendTarget[i].ObjValue).Count > 0)
                {
                    twoListMonthReportDetil.AddRange((List<MonthlyReportDetail>)twoBlendTarget[i].ObjValue);
                }
                else if (twoBlendTarget[i].Name == "SummaryData")
                {
                }
                else
                {
                    List<DictionaryVmodel> twoListHaveDetail = (List<DictionaryVmodel>)twoBlendTarget[i].ObjValue;
                    for (int z = 0; z < twoListHaveDetail.Count; z++)
                    {
                        if (((List<MonthlyReportDetail>)twoListHaveDetail[z].ObjValue).Count > 0)
                            twoListMonthReportDetil.AddRange((List<MonthlyReportDetail>)twoListHaveDetail[z].ObjValue);
                    }

                }
            }

            if (oneListMonthReportDetil != null)
            {
                for (int a = 0; a < oneListMonthReportDetil.Count; a++)
                {
                    MonthlyReportDetail oneMrd = oneListMonthReportDetil[a];
                    MonthlyReportDetail twoMrd = twoListMonthReportDetil.Where(v => v.CompanyID == oneMrd.CompanyID).FirstOrDefault();
                    #region 设置样式
                    ws.Cells[rowStart, colStart].SetStyle(style2);
                    ws.Cells[rowStart, colStart + 1].SetStyle(style2);
                    ws.Cells[rowStart, colStart + 2].SetStyle(style2);
                    ws.Cells[rowStart, colStart + 3].SetStyle(style2);
                    ws.Cells[rowStart, colStart + 4].SetStyle(style2);
                    ws.Cells[rowStart, colStart + 5].SetStyle(style2);
                    ws.Cells[rowStart, colStart + 6].SetStyle(style2);
                    #endregion
                    ws.Cells[rowStart, colStart].PutValue(rowStart - 3);
                    ws.Cells[rowStart, colStart + 1].PutValue(oneMrd.ID);
                    ws.Cells[rowStart, colStart + 2].PutValue(oneMrd.CompanyName);
                    ws.Cells[rowStart, colStart + 3].PutValue(oneMrd.NPlanAmmount);
                    ws.Cells[rowStart, colStart + 4].PutValue(twoMrd.NPlanAmmount);
                    ws.Cells[rowStart, colStart + 5].PutValue(oneMrd.NActualAmmount);// 同步的数据
                    ws.Cells[rowStart, colStart + 6].PutValue(twoMrd.NActualAmmount); // 同步的数据
                    #region 设置千分位
                    Style tempstyle;
                    tempstyle = ws.Cells[rowStart, colStart + 4].GetStyle();
                    tempstyle.Number = 3;
                    ws.Cells[rowStart, colStart + 3].SetStyle(tempstyle);
                    ws.Cells[rowStart, colStart + 4].SetStyle(tempstyle);
                    ws.Cells[rowStart, colStart + 5].SetStyle(tempstyle);
                    ws.Cells[rowStart, colStart + 6].SetStyle(tempstyle);
                    #endregion
                    rowStart = rowStart + 1;
                }
            }


            #region 设置保护单元格
            Style style;
            StyleFlag styleflag;
            for (int x = 0; x <= 255; x++)
            {
                style = ws.Cells.Columns[(byte)x].Style;
                style.IsLocked = true;
                styleflag = new StyleFlag();
                styleflag.Locked = true;

                ws.Cells.Columns[(byte)x].ApplyStyle(style, styleflag);
            }
            for (int j = 4; j < rowStart; j++)
            {
                style = ws.Cells[j, colStart + 4].GetStyle();
                style.IsLocked = false;
                style.ForegroundColor = System.Drawing.Color.White;
                ws.Cells[j, colStart + 5].SetStyle(style);
                ws.Cells[j, colStart + 6].SetStyle(style);
            }

            ws.Protect(ProtectionType.All);
            #endregion

            #endregion
        }
        #endregion

        #region 根据具体是不是混合指标来确定调用具体方法

        /// <summary>
        /// 下载计划指标模板
        /// </summary>
        public void DownTargetPlanExcel(HttpContext context)
        {
            if (MonthReportID == null || MonthReportID == Guid.Empty)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("请确认有默认的分解指标计划。");
                return;
            }

            ReportInstance rpt = new ReportInstance(MonthReportID, true);

            var defaultTagetPlan = StaticResource.Instance.GetDefaultTargetPlanList(rpt._SystemID, rpt.FinYear);
            if (defaultTagetPlan == null || defaultTagetPlan.Count == 0)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("请确认有默认的分解指标计划。");
                return;
            }
            var blendResult = GetBlendTargets(rpt);
            //item1等于true 时，表示是混合指标
            if (blendResult.Item1)
                DownTargetPlanExcelForBlend(context, rpt, blendResult.Item2);
            else
                DownTargetPlanExcelForOriginal(context, rpt);

        }


        /// <summary>
        /// 下载计划指标模板（原来的）
        /// </summary>
        private void DownTargetPlanExcelForOriginal(HttpContext context, ReportInstance rpt)
        {
            string templeteName = "指标上报模版V1.xlsx";
            string fileName = "月度经营报告上报";
            //ReportInstance rpt = new ReportInstance(MonthReportID, true);

            List<DictionaryVmodel> listTargetPlanView = new List<DictionaryVmodel>();
            //判断下是否是 国内院线
            string _sysMovie = AppSettingConfig.GetSetting("MovieCN", "");
            if (rpt._SystemID == Guid.Parse(_sysMovie))
                listTargetPlanView = rpt.GetTagetPlanViewModel(Guid.Parse(_sysMovie));  //院线同步数据
            else
                listTargetPlanView = rpt.GetTagetPlanViewModel();  // 其他系统



            if (listTargetPlanView.Count > 0)
            {
                string[] strHtmlTemplates = listTargetPlanView[0].HtmlTemplate.Split(',');
                if (strHtmlTemplates.Count() > 4)
                {
                    if (!string.IsNullOrEmpty(strHtmlTemplates[4]))
                        templeteName = strHtmlTemplates[4];
                }
            }

            string templetePath = Path.Combine(ExcelTempletePath, templeteName);
            if (System.IO.File.Exists(templetePath))
            {


                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;
                Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 12;
                #endregion
                #region style2 样式 无加粗
                style2.Font.Size = 12;

                style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                style2.ForegroundColor = System.Drawing.Color.LightGray;
                style2.Pattern = BackgroundType.Solid;

                #endregion
                #region style3 样式
                style3.Font.Size = 12;
                style3.Font.Name = "Arial";
                style3.Font.IsBold = true;
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

                #region 复制sheet
                for (int z = 0; z < listTargetPlanView.Count; z++)
                {
                    if (z > 0 && templeteName == "指标上报模版V1.xlsx")
                    {
                        worksheets.AddCopy(0);
                    }
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == listTargetPlanView[z].Name).ToList();
                    if (listTarget != null && listTarget.Count > 0)
                    {
                        excel.SetCustomProperty(worksheets[z], "SystemID", listTarget[0].SystemID.ToString());
                        excel.SetCustomProperty(worksheets[z], "TragertID", listTarget[0].ID.ToString());
                        excel.SetCustomProperty(worksheets[z], "TragertName", listTarget[0].TargetName);
                        excel.SetCustomProperty(worksheets[z], "SheetName", "MonthReportDetail");
                        excel.SetCustomProperty(worksheets[z], "AreaID", rpt.AreaID.ToString());

                    }
                    worksheets[z].Cells[0, 1].PutValue(rpt._System.SystemName + listTargetPlanView[z].Name);
                    worksheets[z].Cells[1, 3].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[z].Cells[1, 3].SetStyle(style1);
                    if (templeteName == "指标上报模版V1.xlsx")
                    {
                        worksheets[z].Name = listTargetPlanView[z].Name;
                    }

                }
                #endregion

                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列
                for (int i = 0; i < listTargetPlanView.Count; i++)
                {
                    rowStart = 4;

                    #region 判断指标是否可以编辑
                    bool IsModifyTargetPlanDetail = false; //指标是否可以编辑
                    bool IsHaveAccumulativePlanDetail = false;
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == listTargetPlanView[i].Name).ToList();

                    if (listTarget.Count > 0)
                    {
                        if (listTarget[0].Unit != "万元")
                        {
                            worksheets[i].Cells[1, 5].PutValue("单位：" + listTarget[0].Unit);
                        }

                        XElement xmlTarget = listTarget[0].Configuration;
                        if (xmlTarget != null)
                        {
                            IsModifyTargetPlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[0].ObjValue;
                            IsHaveAccumulativePlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[1].ObjValue;
                        }
                    }
                    #endregion

                    List<TargetPlanViewModel> listPlanTarget = (List<TargetPlanViewModel>)listTargetPlanView[i].ObjValue;
                    for (int j = 0; j < listPlanTarget.Count; j++)
                    {
                        #region 设置样式
                        worksheets[i].Cells[rowStart, colStart].SetStyle(style2);
                        worksheets[i].Cells[rowStart, colStart + 1].SetStyle(style2);
                        worksheets[i].Cells[rowStart, colStart + 2].SetStyle(style2);
                        worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style2);
                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style2);
                        #endregion

                        #region 赋值
                        worksheets[i].Cells[rowStart, colStart].PutValue(j + 1);
                        worksheets[i].Cells[rowStart, colStart + 1].PutValue(listPlanTarget[j].ID);
                        worksheets[i].Cells[rowStart, colStart + 2].PutValue(listPlanTarget[j].CompanyName);
                        worksheets[i].Cells[rowStart, colStart + 3].PutValue(listPlanTarget[j].NPlanAmmount);
                        worksheets[i].Cells[rowStart, colStart + 4].PutValue(listPlanTarget[j].NActualAmmount); // 同步的数据

                        #region 仅商管用
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i].Cells[rowStart, colStart + 5].PutValue(listPlanTarget[j].NAccumulativePlanAmmount);
                        }
                        #endregion

                        #endregion

                        #region 设置千分位
                        Style tempstyle;
                        tempstyle = worksheets[i].Cells[rowStart, colStart + 3].GetStyle();
                        tempstyle.Number = 3;
                        worksheets[i].Cells[rowStart, colStart + 3].SetStyle(tempstyle);
                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(tempstyle);

                        #region 仅商管用
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i].Cells[rowStart, colStart + 5].SetStyle(tempstyle);
                            worksheets[i].Cells[rowStart, colStart + 6].SetStyle(tempstyle);
                        }
                        #endregion

                        #endregion

                        rowStart = rowStart + 1;
                    }

                    #region 设置保护单元格
                    Style style;
                    StyleFlag styleflag;
                    rowStart = 4;
                    for (int x = 0; x <= 255; x++)
                    {
                        style = worksheets[i].Cells.Columns[(byte)x].Style;
                        style.IsLocked = true;
                        styleflag = new StyleFlag();
                        styleflag.Locked = true;

                        worksheets[i].Cells.Columns[(byte)x].ApplyStyle(style, styleflag);
                    }
                    for (int j = 0; j < listPlanTarget.Count; j++)
                    {
                        style = worksheets[i].Cells[rowStart, colStart + 3].GetStyle();
                        style.IsLocked = false;
                        style.ForegroundColor = System.Drawing.Color.White;
                        if (IsModifyTargetPlanDetail)
                        {
                            worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style);

                        }
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i].Cells[rowStart, colStart + 6].SetStyle(style);
                            if (IsModifyTargetPlanDetail)
                            {
                                worksheets[i].Cells[rowStart, colStart + 5].SetStyle(style);
                            }
                        }
                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style);
                        rowStart = rowStart + 1;
                    }

                    worksheets[i].Protect(ProtectionType.All);
                    #endregion
                }

                var AreaName = "";
                if (rpt.AreaID != Guid.Empty)
                {
                    AreaName = "-" + S_OrganizationalActionOperator.Instance.GetDataByID(rpt.AreaID).CnName + "-";
                }

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                //设置基本信息   
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                string dateNow = DateTime.Now.ToString("HHmmss");
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + fileName + AreaName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }
        }

        /// <summary>
        /// 下载计划指标模板（混合指标）
        /// </summary>
        private void DownTargetPlanExcelForBlend(HttpContext context, ReportInstance rpt, List<C_Target> blendTargetList)
        {
            string templeteName = "指标上报模版_混合V1.xlsx";
            string fileName = "月度经营报告上报";
            // ReportInstance rpt = new ReportInstance(MonthReportID, true);

            List<DictionaryVmodel> listTargetPlanView = new List<DictionaryVmodel>();
            //判断下是否是 国内院线
            string _sysMovie = AppSettingConfig.GetSetting("MovieCN", "");
            if (rpt._SystemID == Guid.Parse(_sysMovie))
                listTargetPlanView = rpt.GetTagetPlanViewModel(Guid.Parse(_sysMovie));  //院线同步数据
            else
                listTargetPlanView = rpt.GetTagetPlanViewModel();  // 其他系统


            if (listTargetPlanView.Count > 0)
            {
                string[] strHtmlTemplates = listTargetPlanView[0].HtmlTemplate.Split(',');
                if (strHtmlTemplates.Count() > 4)
                {
                    if (!string.IsNullOrEmpty(strHtmlTemplates[4]))
                        templeteName = strHtmlTemplates[4];
                }
            }


            string templetePath = Path.Combine(ExcelTempletePath, templeteName);
            if (System.IO.File.Exists(templetePath))
            {
                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;
                Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 12;
                #endregion
                #region style2 样式 无加粗
                style2.Font.Size = 12;

                style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                style2.ForegroundColor = System.Drawing.Color.LightGray;
                style2.Pattern = BackgroundType.Solid;

                #endregion
                #region style3 样式
                style3.Font.Size = 12;
                style3.Font.Name = "Arial";
                style3.Font.IsBold = true;
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

                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列

                //插入混合指标
                InsertBlendTargetToExcel(excel, worksheets[0], rpt, listTargetPlanView, blendTargetList, fileName, rowStart, colStart, style2);

                var otherListTargetPlanView = listTargetPlanView.Where(v => !blendTargetList.Select(x => x.TargetName).ToList().Contains(v.Name)).ToList();
                //如果只有混合指标，则删除模板中的Sheet2
                if (otherListTargetPlanView == null || otherListTargetPlanView.Count == 0)
                {
                    worksheets.RemoveAt("Sheet2");
                }

                #region 复制sheet
                for (int z = 0; z < otherListTargetPlanView.Count; z++)
                {
                    if (z > 0 && templeteName == "指标上报模版_混合V1.xlsx")
                    {
                        worksheets.AddCopy(1);
                    }
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == otherListTargetPlanView[z].Name).ToList();
                    if (listTarget != null && listTarget.Count > 0)
                    {
                        excel.SetCustomProperty(worksheets[z + 1], "SystemID", listTarget[0].SystemID.ToString());
                        excel.SetCustomProperty(worksheets[z + 1], "TragertID", listTarget[0].ID.ToString());
                        excel.SetCustomProperty(worksheets[z + 1], "TragertName", listTarget[0].TargetName);
                        excel.SetCustomProperty(worksheets[z + 1], "SheetName", "MonthReportDetail");
                        excel.SetCustomProperty(worksheets[z + 1], "AreaID", rpt.AreaID.ToString());
                    }
                    worksheets[z + 1].Cells[0, 1].PutValue(rpt._System.SystemName + otherListTargetPlanView[z].Name);
                    //worksheets[z + 1].Cells[1, 3].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[z + 1].Cells[2, 4].PutValue(string.Format(@"{0}月情况 ", FinMonth));
                    worksheets[z + 1].Cells[1, 3].SetStyle(style1);
                    if (templeteName == "指标上报模版_混合V1.xlsx")
                    {
                        worksheets[z + 1].Name = otherListTargetPlanView[z].Name;
                    }

                }
                #endregion

                for (int i = 0; i < otherListTargetPlanView.Count; i++)
                {
                    rowStart = 4;

                    #region 判断指标是否可以编辑
                    bool IsModifyTargetPlanDetail = false; //指标是否可以编辑
                    bool IsHaveAccumulativePlanDetail = false;
                    List<C_Target> listTarget = rpt._Target.Where(p => p.TargetName == otherListTargetPlanView[i].Name).ToList();

                    if (listTarget.Count > 0)
                    {
                        if (listTarget[0].Unit != "万元")
                        {
                            worksheets[i + 1].Cells[1, 5].PutValue("单位：" + listTarget[0].Unit);
                        }

                        XElement xmlTarget = listTarget[0].Configuration;
                        if (xmlTarget != null)
                        {
                            IsModifyTargetPlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[0].ObjValue;
                            IsHaveAccumulativePlanDetail = (bool)rpt.GetIsModifyTargetPlanDetail(xmlTarget)[1].ObjValue;
                        }
                    }
                    #endregion

                    List<TargetPlanViewModel> listPlanTarget = (List<TargetPlanViewModel>)otherListTargetPlanView[i].ObjValue;
                    for (int j = 0; j < listPlanTarget.Count; j++)
                    {
                        #region 设置样式
                        worksheets[i + 1].Cells[rowStart, colStart].SetStyle(style2);
                        worksheets[i + 1].Cells[rowStart, colStart + 1].SetStyle(style2);
                        worksheets[i + 1].Cells[rowStart, colStart + 2].SetStyle(style2);
                        worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(style2);
                        worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style2);
                        #endregion

                        #region 赋值
                        worksheets[i + 1].Cells[rowStart, colStart].PutValue(j + 1);
                        worksheets[i + 1].Cells[rowStart, colStart + 1].PutValue(listPlanTarget[j].ID);
                        worksheets[i + 1].Cells[rowStart, colStart + 2].PutValue(listPlanTarget[j].CompanyName);
                        worksheets[i + 1].Cells[rowStart, colStart + 3].PutValue(listPlanTarget[j].NPlanAmmount);
                        worksheets[i + 1].Cells[rowStart, colStart + 4].PutValue(listPlanTarget[j].NActualAmmount); // 同步的数据

                        #region 仅商管用
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i + 1].Cells[rowStart, colStart + 5].PutValue(listPlanTarget[j].NAccumulativePlanAmmount);
                        }
                        #endregion

                        #endregion

                        #region 设置千分位
                        Style tempstyle;
                        tempstyle = worksheets[i + 1].Cells[rowStart, colStart + 3].GetStyle();
                        tempstyle.Number = 3;
                        worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(tempstyle);
                        worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(tempstyle);

                        #region 仅商管用
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i + 1].Cells[rowStart, colStart + 5].SetStyle(tempstyle);
                            worksheets[i + 1].Cells[rowStart, colStart + 6].SetStyle(tempstyle);
                        }
                        #endregion

                        #endregion

                        rowStart = rowStart + 1;
                    }

                    #region 设置保护单元格
                    Style style;
                    StyleFlag styleflag;
                    rowStart = 4;
                    for (int x = 0; x <= 255; x++)
                    {
                        style = worksheets[i + 1].Cells.Columns[(byte)x].Style;
                        style.IsLocked = true;
                        styleflag = new StyleFlag();
                        styleflag.Locked = true;

                        worksheets[i + 1].Cells.Columns[(byte)x].ApplyStyle(style, styleflag);
                    }
                    for (int j = 0; j < listPlanTarget.Count; j++)
                    {
                        style = worksheets[i + 1].Cells[rowStart, colStart + 3].GetStyle();
                        style.IsLocked = false;
                        style.ForegroundColor = System.Drawing.Color.White;
                        if (IsModifyTargetPlanDetail)
                        {
                            worksheets[i + 1].Cells[rowStart, colStart + 3].SetStyle(style);

                        }
                        if (IsHaveAccumulativePlanDetail)
                        {
                            worksheets[i + 1].Cells[rowStart, colStart + 6].SetStyle(style);
                            if (IsModifyTargetPlanDetail)
                            {
                                worksheets[i + 1].Cells[rowStart, colStart + 5].SetStyle(style);
                            }
                        }
                        worksheets[i + 1].Cells[rowStart, colStart + 4].SetStyle(style);
                        rowStart = rowStart + 1;
                    }

                    worksheets[i + 1].Protect(ProtectionType.All);
                    #endregion
                }

                var AreaName = "";
                if (rpt.AreaID != Guid.Empty)
                {
                    AreaName = "-" + S_OrganizationalActionOperator.Instance.GetDataByID(rpt.AreaID).CnName + "-";
                }

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                //设置基本信息   
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                string dateNow = DateTime.Now.ToString("HHmmss");
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + fileName + AreaName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }
        }

        /// <summary>
        /// 插入混合指标内容
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="ws">sheet</param>
        /// <param name="rpt"></param>
        /// <param name="listTargetPlanView"></param>
        /// <param name="blendTargetList">混合指标</param>
        /// <param name="fileName">文件名</param>
        /// <param name="rowStart">起始行</param>
        /// <param name="colStart">起始列</param>
        /// <param name="style2">样式</param>
        private void InsertBlendTargetToExcel(ExcelEngine excel, Worksheet ws, ReportInstance rpt,
            List<DictionaryVmodel> listTargetPlanView, List<C_Target> blendTargetList, string fileName,
            int rowStart, int colStart, Style style2)
        {
            #region 混合指标
            ws.Name = string.Format(@"{0}+{1}", blendTargetList[0].TargetName, blendTargetList[1].TargetName);
            excel.SetCustomProperty(ws, "IsBlendTager", "1");
            excel.SetCustomProperty(ws, "SystemID", blendTargetList[0].SystemID.ToString());
            excel.SetCustomProperty(ws, "TragertID", blendTargetList[0].ID.ToString());
            excel.SetCustomProperty(ws, "TragertName", blendTargetList[0].TargetName);
            excel.SetCustomProperty(ws, "TragertTwoID", blendTargetList[1].ID.ToString());
            excel.SetCustomProperty(ws, "TragertTwoName", blendTargetList[1].TargetName);
            excel.SetCustomProperty(ws, "AreaID", rpt.AreaID.ToString());
            excel.SetCustomProperty(ws, "SheetName", "MonthReportDetail");
            ws.Cells[0, 1].PutValue(string.Format(@"{0}年度{1}{2}", FinYear, rpt._System.SystemName, fileName));
            ws.Cells[1, 4].PutValue(string.Format(@"{0}月情况", FinMonth));
            ws.Cells[2, 4].PutValue(string.Format(@"{0}月计划", FinMonth));
            ws.Cells[2, 6].PutValue(string.Format(@"{0}月完成（金额）", FinMonth));
            ws.Cells[3, 4].PutValue(string.Format(@"{0}", blendTargetList[0].TargetName));
            ws.Cells[3, 5].PutValue(string.Format(@"{0}", blendTargetList[1].TargetName));
            ws.Cells[3, 6].PutValue(string.Format(@"{0}", blendTargetList[0].TargetName));
            ws.Cells[3, 7].PutValue(string.Format(@"{0}", blendTargetList[1].TargetName));
            List<TargetPlanViewModel> oneBlendTarget = (List<TargetPlanViewModel>)listTargetPlanView.Where(v => v.Name == blendTargetList[0].TargetName).FirstOrDefault().ObjValue;
            List<TargetPlanViewModel> twoBlendTarget = (List<TargetPlanViewModel>)listTargetPlanView.Where(v => v.Name == blendTargetList[1].TargetName).FirstOrDefault().ObjValue;
            for (int i = 0; i < oneBlendTarget.Count; i++)
            {
                #region 设置样式
                ws.Cells[rowStart + i, colStart].SetStyle(style2);
                ws.Cells[rowStart + i, colStart + 1].SetStyle(style2);
                ws.Cells[rowStart + i, colStart + 2].SetStyle(style2);
                ws.Cells[rowStart + i, colStart + 3].SetStyle(style2);
                ws.Cells[rowStart + i, colStart + 4].SetStyle(style2);
                ws.Cells[rowStart + i, colStart + 5].SetStyle(style2);
                ws.Cells[rowStart + i, colStart + 6].SetStyle(style2);
                #endregion

                var twoBlendTargetModel = twoBlendTarget.Where(v => v.ID == oneBlendTarget[i].ID).FirstOrDefault();

                ws.Cells[rowStart + i, colStart].PutValue(i + 1);
                ws.Cells[rowStart + i, colStart + 1].PutValue(oneBlendTarget[i].ID);
                ws.Cells[rowStart + i, colStart + 2].PutValue(oneBlendTarget[i].CompanyName);
                ws.Cells[rowStart + i, colStart + 3].PutValue(oneBlendTarget[i].NPlanAmmount);
                ws.Cells[rowStart + i, colStart + 4].PutValue(twoBlendTargetModel.NPlanAmmount);
                ws.Cells[rowStart + i, colStart + 5].PutValue(oneBlendTarget[i].NActualAmmount);// 同步的数据
                ws.Cells[rowStart + i, colStart + 6].PutValue(twoBlendTargetModel.NActualAmmount); // 同步的数据

                #region 设置千分位
                Style tempstyle;
                tempstyle = ws.Cells[rowStart + i, colStart + 4].GetStyle();
                tempstyle.Number = 3;
                ws.Cells[rowStart + i, colStart + 3].SetStyle(tempstyle);
                ws.Cells[rowStart + i, colStart + 4].SetStyle(tempstyle);
                ws.Cells[rowStart + i, colStart + 5].SetStyle(tempstyle);
                ws.Cells[rowStart + i, colStart + 6].SetStyle(tempstyle);
                #endregion
            }

            #region 设置保护单元格
            Style style;
            StyleFlag styleflag;
            for (int x = 0; x <= 255; x++)
            {
                style = ws.Cells.Columns[(byte)x].Style;
                style.IsLocked = true;
                styleflag = new StyleFlag();
                styleflag.Locked = true;

                ws.Cells.Columns[(byte)x].ApplyStyle(style, styleflag);
            }
            for (int j = 0; j < oneBlendTarget.Count; j++)
            {
                style = ws.Cells[rowStart + j, colStart + 4].GetStyle();
                style.IsLocked = false;
                style.ForegroundColor = System.Drawing.Color.White;
                ws.Cells[rowStart + j, colStart + 5].SetStyle(style);
                ws.Cells[rowStart + j, colStart + 6].SetStyle(style);
            }

            ws.Protect(ProtectionType.All);
            #endregion

            #endregion
        }
        #endregion

        public void ReadExcel(string fileName, out int error)
        {
            error = 0;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// 项目公司完成明细
        /// </summary>
        /// <param name="context"></param>
        public void DownExcelProCompanyReportDetail(HttpContext context)
        {
            string templeteName = "项目公司指标上报模版V2.xlsx";
            string fileName = "指标上报";

            string templetePath = Path.Combine(ExcelTempletePath, templeteName);

            if (System.IO.File.Exists(templetePath))
            {
                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;

                Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2_NoColor = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 12;
                style1.Font.Name = "Arial";
                #endregion

                #region style2 样式 无加粗
                style2.Font.Size = 12;
                style2.Number = 3;
                style2.Font.Name = "Arial";
                style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                style2.ForegroundColor = System.Drawing.Color.LightGray;
                style2.Pattern = BackgroundType.Solid;
                #endregion

                #region style2_NoColor 样式 没有背景色

                style2_NoColor.Font.Size = 12;
                style2_NoColor.Pattern = BackgroundType.Solid;
                style2_NoColor.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2_NoColor.IsLocked = false;
                style2_NoColor.Number = 3;
                style2_NoColor.Font.Name = "Arial";

                style2_NoColor.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2_NoColor.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2_NoColor.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2_NoColor.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2_NoColor.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2_NoColor.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2_NoColor.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2_NoColor.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;



                #endregion

                #region style3 样式
                style3.Font.Size = 12;
                style3.Font.Name = "Arial";
                style3.Font.IsBold = true;
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

                int rowStart = 4;  //开始行
                                   //int colStart = 1; // 开始列

                ReportInstance rpt = new ReportInstance(MonthReportID, true);
                List<DictionaryVmodel> ProCompanyList = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "Reported", "", true);

                List<V_ProjectCompany> dicProList = (List<V_ProjectCompany>)ProCompanyList[0].ObjValue;

                //worksheets.AddCopy(0);

                #region Excel的表头


                worksheets[0].Name = rpt._System.SystemName;
                int tem = 0;

                if (FinMonth != 12)
                {
                    tem = FinMonth + 1;
                }
                else
                {
                    tem = 1;
                    FinYear = FinYear + 1;
                }
                DateTime T = DateTime.Parse(FinYear + "-" + tem + "-" + "01").AddDays(-1);
                string titleStr = FinYear.ToString() + "年度" + rpt._System.SystemName + "销售指标完成情况通报（数据截止至" + T.Month.ToString() + "." + T.Day.ToString() + "）";
                worksheets[0].Cells[0, 1].PutValue(titleStr);
                worksheets[0].Cells[1, 15].PutValue("金额单位：万元"); //
                worksheets[0].Cells[2, 4].PutValue("1-" + FinMonth.ToString() + "月情况");
                worksheets[0].Cells[2, 10].PutValue(FinMonth.ToString() + "月情况");
                worksheets[0].Cells[3, 4].PutValue("1-" + FinMonth.ToString() + "月情况");
                worksheets[0].Cells[3, 7].PutValue("1-" + FinMonth.ToString() + "月完成（金额）");
                worksheets[0].Cells[3, 10].PutValue(FinMonth.ToString() + "月情况");
                worksheets[0].Cells[3, 13].PutValue(FinMonth.ToString() + "月完成（金额）");

                #endregion


                List<V_ProjectCompany> dicAllList = dicProList.FindAll(p => p.ProCompanySequence > 0);

                //将包含有小计的数据移除
                dicAllList = dicAllList.Where(p => p.ProCompayName.Contains("小计") == false).ToList();


                excel.SetCustomProperty(worksheets[0], "SheetName", "ProCompanyReportDetail");

                for (int i = 0; i < dicAllList.Count; i++)
                {
                    rowStart = 5 + i;
                    int index = 1 + i;

                    worksheets[0].Cells[rowStart, 1].SetStyle(style2);
                    worksheets[0].Cells[rowStart, 2].SetStyle(style2);
                    worksheets[0].Cells[rowStart, 3].SetStyle(style2_NoColor);

                    worksheets[0].Cells[rowStart, 1].PutValue(index++); //序号
                    worksheets[0].Cells[rowStart, 2].PutValue(dicAllList[i].ProCompayName);

                    if (string.IsNullOrEmpty(dicAllList[i].ProCompanyProperty1)) //是否尾盘
                    {
                        worksheets[0].Cells[rowStart, 3].PutValue("否");
                    }
                    else
                    {
                        worksheets[0].Cells[rowStart, 3].PutValue("是");
                    }

                    worksheets[0].Cells[rowStart, 4].SetStyle(style2);
                    worksheets[0].Cells[rowStart, 5].SetStyle(style2);
                    worksheets[0].Cells[rowStart, 6].SetStyle(style2);

                    worksheets[0].Cells[rowStart, 4].PutValue(dicAllList[i].ProjectTargets[0].NAccumulativePlanAmmount); //年度累计指标
                    worksheets[0].Cells[rowStart, 5].PutValue(dicAllList[i].ProjectTargets[1].NAccumulativePlanAmmount); //年度累计指标
                    worksheets[0].Cells[rowStart, 6].PutValue(dicAllList[i].ProjectTargets[2].NAccumulativePlanAmmount); //年度累计指标


                    worksheets[0].Cells[rowStart, 7].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 8].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 9].SetStyle(style2_NoColor);


                    worksheets[0].Cells[rowStart, 7].PutValue(dicAllList[i].ProjectTargets[0].NAccumulativeActualAmmount); //年度累计实际
                    worksheets[0].Cells[rowStart, 8].PutValue(dicAllList[i].ProjectTargets[1].NAccumulativeActualAmmount); //年度累计实际
                    worksheets[0].Cells[rowStart, 9].PutValue(dicAllList[i].ProjectTargets[2].NAccumulativeActualAmmount); //年度累计实际

                    worksheets[0].Cells[rowStart, 10].SetStyle(style2);
                    worksheets[0].Cells[rowStart, 11].SetStyle(style2);
                    worksheets[0].Cells[rowStart, 12].SetStyle(style2);

                    worksheets[0].Cells[rowStart, 10].PutValue(dicAllList[i].ProjectTargets[0].NPlanAmmount); //年度当月指标
                    worksheets[0].Cells[rowStart, 11].PutValue(dicAllList[i].ProjectTargets[1].NPlanAmmount); //年度当月指标
                    worksheets[0].Cells[rowStart, 12].PutValue(dicAllList[i].ProjectTargets[2].NPlanAmmount); //年度当月指标

                    worksheets[0].Cells[rowStart, 13].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 14].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 15].SetStyle(style2_NoColor);

                    worksheets[0].Cells[rowStart, 13].PutValue(dicAllList[i].ProjectTargets[0].NActualAmmount); //年度当月实际
                    worksheets[0].Cells[rowStart, 14].PutValue(dicAllList[i].ProjectTargets[1].NActualAmmount); //年度当月实际
                    worksheets[0].Cells[rowStart, 15].PutValue(dicAllList[i].ProjectTargets[2].NActualAmmount); //年度当月实际

                }

                //worksheets.RemoveAt();
                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                //设置基本信息   
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                string dateNow = DateTime.Now.ToString("HHmmss");
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }


        }

        /// <summary>
        /// 下载项目公司的指标计划表
        /// </summary>
        /// <param name="context"></param>
        public void DownProTargetPlanExcel(HttpContext context)
        {
            string templeteName = "项目公司指标上报模版V2.xlsx";
            string fileName = "指标上报";

            string templetePath = Path.Combine(ExcelTempletePath, templeteName);

            if (System.IO.File.Exists(templetePath))
            {
                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;

                Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2_NoColor = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 12;
                style1.Font.Name = "Arial";
                #endregion

                #region style2 样式 无加粗
                style2.Font.Size = 12;
                style2.Number = 3;
                style2.IsLocked = true;
                style2.Font.Name = "Arial";
                style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                style2.ForegroundColor = System.Drawing.Color.LightGray;
                style2.Pattern = BackgroundType.Solid;
                #endregion

                #region style2_NoColor 样式 没有背景色

                style2_NoColor.Font.Size = 12;
                style2_NoColor.Font.Name = "Arial";
                style2_NoColor.Pattern = BackgroundType.Solid;
                style2_NoColor.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2_NoColor.IsLocked = false;
                style2_NoColor.Number = 3;

                style2_NoColor.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2_NoColor.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2_NoColor.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2_NoColor.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2_NoColor.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2_NoColor.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2_NoColor.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2_NoColor.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;



                #endregion

                #region style3 样式
                style3.Font.Size = 12;
                style3.Font.Name = "Arial";
                style3.Font.IsBold = true;
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

                int rowStart = 4;  //开始行
                                   //int colStart = 1; // 开始列

                ReportInstance rpt = new ReportInstance(SysId, FinYear, FinMonth, true);
                List<DictionaryVmodel> ProCompanyList = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "Reported", "ProPlan", true);

                List<V_ProjectCompany> dicProList = (List<V_ProjectCompany>)ProCompanyList[0].ObjValue;

                //worksheets.AddCopy(0);


                #region Excel的表头


                worksheets[0].Name = rpt._System.SystemName;

                int tem = 0;

                if (FinMonth != 12)
                {
                    tem = FinMonth + 1;
                }
                else
                {
                    tem = 1;
                    FinYear = FinYear + 1;
                }

                DateTime T = DateTime.Parse(FinYear + "-" + tem + "-" + "01").AddDays(-1);
                string titleStr = T.Year.ToString() + "年度" + rpt._System.SystemName + "销售指标完成情况通报（数据截止至" + T.Month.ToString() + "." + T.Day.ToString() + "）";
                worksheets[0].Cells[0, 1].PutValue(titleStr);
                worksheets[0].Cells[1, 15].PutValue("金额单位：万元"); //
                worksheets[0].Cells[2, 4].PutValue("1-" + FinMonth.ToString() + "月情况");
                worksheets[0].Cells[2, 10].PutValue(FinMonth.ToString() + "月情况");
                worksheets[0].Cells[3, 4].PutValue("1-" + FinMonth.ToString() + "月情况");
                worksheets[0].Cells[3, 7].PutValue("1-" + FinMonth.ToString() + "月完成（金额）");
                worksheets[0].Cells[3, 10].PutValue(FinMonth.ToString() + "月情况");
                worksheets[0].Cells[3, 13].PutValue(FinMonth.ToString() + "月完成（金额）");

                #endregion

                List<V_ProjectCompany> dicList = dicProList.FindAll(p => p.ProCompanySequence > 0);

                excel.SetCustomProperty(worksheets[0], "SheetName", "ProCompanyReportDetail");

                for (int i = 0; i < dicList.Count; i++)
                {
                    rowStart = 5 + i;
                    int index = 1 + i;

                    worksheets[0].Cells[rowStart, 1].SetStyle(style2, true);
                    worksheets[0].Cells[rowStart, 2].SetStyle(style2, true);
                    worksheets[0].Cells[rowStart, 3].SetStyle(style2_NoColor);

                    worksheets[0].Cells[rowStart, 1].PutValue(index++); //序号
                    worksheets[0].Cells[rowStart, 2].PutValue(dicList[i].ProCompayName);

                    if (string.IsNullOrEmpty(dicList[i].ProCompanyProperty1)) //是否尾盘
                    {
                        worksheets[0].Cells[rowStart, 3].PutValue("否");
                    }
                    else
                    {
                        worksheets[0].Cells[rowStart, 3].PutValue("是");
                    }

                    worksheets[0].Cells[rowStart, 4].SetStyle(style2, true);
                    worksheets[0].Cells[rowStart, 5].SetStyle(style2, true);
                    worksheets[0].Cells[rowStart, 6].SetStyle(style2, true);

                    worksheets[0].Cells[rowStart, 4].PutValue(dicList[i].ProjectTargets[0].NAccumulativePlanAmmount); //年度累计指标
                    worksheets[0].Cells[rowStart, 5].PutValue(dicList[i].ProjectTargets[1].NAccumulativePlanAmmount); //年度累计指标
                    worksheets[0].Cells[rowStart, 6].PutValue(dicList[i].ProjectTargets[2].NAccumulativePlanAmmount); //年度累计指标


                    worksheets[0].Cells[rowStart, 7].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 8].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 9].SetStyle(style2_NoColor);


                    //worksheets[0].Cells[rowStart, 7].PutValue(dicList[i].ProjectTargets[0].NAccumulativeActualAmmount); //年度累计实际
                    //worksheets[0].Cells[rowStart, 8].PutValue(dicList[i].ProjectTargets[1].NAccumulativeActualAmmount); //年度累计实际
                    //worksheets[0].Cells[rowStart, 9].PutValue(dicList[i].ProjectTargets[2].NAccumulativeActualAmmount); //年度累计实际

                    worksheets[0].Cells[rowStart, 10].SetStyle(style2, true);
                    worksheets[0].Cells[rowStart, 11].SetStyle(style2, true);
                    worksheets[0].Cells[rowStart, 12].SetStyle(style2, true);

                    worksheets[0].Cells[rowStart, 10].PutValue(dicList[i].ProjectTargets[0].NPlanAmmount); //年度当月指标
                    worksheets[0].Cells[rowStart, 11].PutValue(dicList[i].ProjectTargets[1].NPlanAmmount); //年度当月指标
                    worksheets[0].Cells[rowStart, 12].PutValue(dicList[i].ProjectTargets[2].NPlanAmmount); //年度当月指标

                    worksheets[0].Cells[rowStart, 13].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 14].SetStyle(style2_NoColor);
                    worksheets[0].Cells[rowStart, 15].SetStyle(style2_NoColor);

                    //worksheets[0].Cells[rowStart, 13].PutValue(dicList[i].ProjectTargets[0].NActualAmmount); //年度当月实际
                    //worksheets[0].Cells[rowStart, 14].PutValue(dicList[i].ProjectTargets[1].NActualAmmount); //年度当月实际
                    //worksheets[0].Cells[rowStart, 15].PutValue(dicList[i].ProjectTargets[2].NActualAmmount); //年度当月实际

                }

                //worksheets.RemoveAt();
                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                //设置基本信息   
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                string dateNow = DateTime.Now.ToString("HHmmss");
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(rpt._System.SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }

        }
        /// <summary>
        /// 下载集团总部计划指标
        /// </summary>
        /// <param name="context"></param>
        /// <param name="FileType"></param>
        public void DownGroupTargetPlanExcel(HttpContext context, string FileType)
        {
            string templeteName = "指标上报模版_集团总部V1.xlsx";
            string fileName = "指标上报";

            ReportInstance rpt = new ReportInstance(MonthReportID, true);

            List<DictionaryVmodel> listTargetPlanView = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "Reported", FileType, false);
            if (listTargetPlanView.Count > 0)
            {
                if (listTargetPlanView[0].HtmlTemplate != null)
                {
                    string[] strHtmlTemplates = listTargetPlanView[0].HtmlTemplate.Split(',');
                    if (strHtmlTemplates.Count() > 4)
                    {
                        if (!string.IsNullOrEmpty(strHtmlTemplates[4]))
                            templeteName = strHtmlTemplates[4];
                    }
                }
            }

            string templetePath = Path.Combine(ExcelTempletePath, templeteName);
            if (System.IO.File.Exists(templetePath))
            {
                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);

                WorksheetCollection worksheets = designer.Workbook.Worksheets;
                Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2_color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style5 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                #region style1 样式
                style1.Font.Size = 16;
                style1.Font.IsBold = true;

                style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                #endregion
                #region style2 样式 无加粗
                style2.Font.Size = 16;
                style2.IsLocked = false;
                style2.Number = 3;
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

                style5.Font.Size = 16;
                style5.IsLocked = false;
                style5.Number = 3;
                style5.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style5.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style5.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style5.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style5.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style5.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style5.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style5.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style5.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;


                #region style2_color 样式 无加粗 有背景色 保护单元格
                style2_color.Font.Size = 16;
                style2_color.Number = 3;
                style2_color.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
                style2_color.Pattern = BackgroundType.Solid;
                style2_color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style2_color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2_color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style2_color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style2_color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style2_color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style2_color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style2_color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2_color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                #endregion

                #region style3 样式
                style3.Font.Size = 16;
                style3.Font.IsBold = true;
                style3.Number = 3;
                //style3.HorizontalAlignment = TextAlignmentType.Center;
                style3.ForegroundColor = System.Drawing.Color.FromArgb(149, 179, 215);
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
                style4.Font.Size = 16;
                style4.HorizontalAlignment = TextAlignmentType.Center;
                style4.ForegroundColor = System.Drawing.Color.White;
                style4.Pattern = BackgroundType.Solid;
                #endregion

                int rowStart = 6;  //开始行
                int colStart = 1; // 开始列
                excel.SetCustomProperty(worksheets[0], "SystemID", rpt._System.ID.ToString());
                excel.SetCustomProperty(worksheets[0], "SheetName", "GroupMonthReportDetail");


                //创建指标Sheet,
                for (int sheetIndex = 0; sheetIndex < listTargetPlanView.Count; sheetIndex++)
                {
                    #region 截止日期赋值
                    worksheets[0].Cells["B1"].PutValue(rpt._System.SystemName + "费用预算执行情况明细表");
                    worksheets[0].Cells["C4"].PutValue(FinYear.ToString() + "年");
                    worksheets[0].Cells["D4"].PutValue(FinMonth);
                    worksheets[0].Cells["C4"].SetStyle(style4);
                    worksheets[0].Cells["D4"].SetStyle(style4);
                    #endregion
                    List<GroupDictionaryVmodel> lstGDV = (List<GroupDictionaryVmodel>)listTargetPlanView[sheetIndex].ObjValue;
                    for (int targetIndex = 0; targetIndex < lstGDV.Count; targetIndex++)
                    {
                        List<MonthlyReportDetail> lstHaveNotTargetMRD = (List<MonthlyReportDetail>)lstGDV[targetIndex].Value;
                        string[] strs = new string[26];
                        strs[0] = "1:A"; strs[1] = "2:B"; strs[2] = "3:C"; strs[3] = "4:D"; strs[4] = "5:E"; strs[5] = "6:F";
                        strs[6] = "7:G"; strs[7] = "8:H"; strs[8] = "9:I"; strs[9] = "10:J"; strs[10] = "11:K"; strs[11] = "12:L";
                        strs[12] = "13:M"; strs[13] = "14:N"; strs[14] = "15:O"; strs[15] = "16:P"; strs[16] = "17:Q"; strs[17] = "18:R";
                        strs[18] = "19:S"; strs[19] = "20:T"; strs[20] = "21:U"; strs[21] = "22:V"; strs[22] = "23:W"; strs[23] = "24:X";
                        strs[24] = "25:Y"; strs[25] = "26:Z";
                        int sumALLCompanyPay = (lstHaveNotTargetMRD.Count() * 2);
                        if (lstGDV[targetIndex].Name == "合计")
                        {
                            worksheets[sheetIndex].Cells[rowStart, 1].Value = "合计";

                            for (int w = 1; w < sumALLCompanyPay; w++)
                            {
                                worksheets[sheetIndex].Cells[rowStart, w].SetStyle(style3);
                            }
                            worksheets[sheetIndex].Cells[rowStart, sumALLCompanyPay].Formula = "=SUM(" + strs[sumALLCompanyPay].Split(':')[1] + "8:" + strs[sumALLCompanyPay].Split(':')[1] + rowStart + ")";
                            worksheets[sheetIndex].Cells[rowStart, sumALLCompanyPay + 1].Formula = "=SUM(" + strs[sumALLCompanyPay + 1].Split(':')[1] + "8:" + strs[sumALLCompanyPay + 1].Split(':')[1] + rowStart + ")";
                            worksheets[sheetIndex].Cells[rowStart, sumALLCompanyPay].SetStyle(style3);
                            worksheets[sheetIndex].Cells[rowStart, sumALLCompanyPay + 1].SetStyle(style3);
                            continue;
                        }
                        #region 设置样式与赋值


                        worksheets[sheetIndex].Cells[rowStart, colStart].SetStyle(style2_color);
                        Style tempStyle = worksheets[sheetIndex].Cells[rowStart, colStart].GetStyle();
                        tempStyle.Font.IsBold = true;//为指标名称加粗
                        worksheets[sheetIndex].Cells[rowStart, colStart].SetStyle(tempStyle);
                        worksheets[sheetIndex].Cells[rowStart, colStart].Value = lstGDV[targetIndex].Name;
                        #endregion



                        for (int i = 0; i < lstHaveNotTargetMRD.Count; i++)
                        {
                            if (lstHaveNotTargetMRD.Count != (i + 1))
                            {
                                #region 设置样式
                                worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 1].SetStyle(style2_color);
                                Style tempStyle1 = worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 1].GetStyle();
                                tempStyle1.HorizontalAlignment = TextAlignmentType.Center;

                                worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 1].SetStyle(tempStyle1);
                                worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 2].SetStyle(tempStyle1);
                                worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 3].SetStyle(tempStyle1);
                                worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 4].SetStyle(tempStyle1);
                                #endregion
                                if (lstGDV[targetIndex].ObjValue != null)
                                {
                                    List<V_GroupCompany> lstHaveTargetMRD = (List<V_GroupCompany>)lstGDV[targetIndex].ObjValue;

                                    #region

                                    #endregion
                                    #region 为EXCEL上总额赋值
                                    string str1 = strs[colStart + i * 2 + 1].Split(':')[1].ToString();
                                    string str2 = strs[colStart + i * 2 + 2].Split(':')[1].ToString();
                                    worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 1].Formula = "=SUM(" + str1 + (rowStart + 2).ToString() + ":" + str1 + (rowStart + 1 + lstHaveTargetMRD.Count).ToString() + ")";
                                    worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 2].Formula = "=SUM(" + str2 + (rowStart + 2).ToString() + ":" + str2 + (rowStart + 1 + lstHaveTargetMRD.Count).ToString() + ")";
                                    int Rown = rowStart + 1;
                                    string SUMa = string.Empty;
                                    string SUMb = string.Empty;
                                    for (int h = 2; h < strs.Count(); h++)
                                    {
                                        if (((lstHaveNotTargetMRD.Count()) * 2) == h)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (h % 2 == 0)
                                            {
                                                SUMa = SUMa + strs[h].Split(':')[1].ToString() + Rown + "+";
                                            }
                                            else
                                            {
                                                SUMb = SUMb + strs[h].Split(':')[1].ToString() + Rown + "+";
                                            }
                                        }
                                    }

                                    worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 3].Formula = "=SUM(" + SUMa.TrimEnd('+') + ")";
                                    worksheets[sheetIndex].Cells[rowStart, colStart + i * 2 + 4].Formula = "=SUM(" + SUMb.TrimEnd('+') + ")";
                                    #endregion
                                }
                                else
                                {
                                    #region 设置样式
                                    worksheets[sheetIndex].Cells[rowStart, colStart + sumALLCompanyPay].SetStyle(style2);
                                    Style tempStyle2 = worksheets[sheetIndex].Cells[rowStart, colStart + sumALLCompanyPay].GetStyle();
                                    tempStyle2.HorizontalAlignment = TextAlignmentType.Center;
                                    worksheets[sheetIndex].Cells[rowStart, colStart + sumALLCompanyPay].SetStyle(tempStyle2);
                                    worksheets[sheetIndex].Cells[rowStart, colStart + sumALLCompanyPay - 1].SetStyle(tempStyle2);
                                    #endregion
                                    //#region 为EXCEL赋值
                                    List<MonthlyReportDetail> lstAllTarget = lstGDV[targetIndex].Value;
                                    for (int j = 0; j < lstAllTarget.Count; j++)
                                    {
                                        if (lstAllTarget[j].CompanyName == "总部")
                                        {
                                            worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 1].Value = lstAllTarget[j].NAccumulativeActualAmmount;
                                            worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 2].Value = lstAllTarget[j].NAccumulativePlanAmmount;
                                        }
                                    }
                                }
                            }

                        }
                        rowStart = rowStart + 1;
                        if (lstGDV[targetIndex].ObjValue != null)
                        {
                            List<V_GroupCompany> lstHaveTargetMRD = (List<V_GroupCompany>)lstGDV[targetIndex].ObjValue;
                            for (int ii = 0; ii < lstHaveTargetMRD.Count; ii++)
                            {
                                #region 设置样式与赋值
                                worksheets[sheetIndex].Cells[rowStart, colStart].SetStyle(style2_color);
                                worksheets[sheetIndex].Cells[rowStart, colStart].Value = (ii + 1) + "、" + lstHaveTargetMRD[ii].CompanyName;
                                #endregion
                                List<MonthlyReportDetail> lstHaveTargetDetailMRD = (List<MonthlyReportDetail>)lstHaveTargetMRD[ii].ListGroupTargetDetail;
                                for (int j = 0; j < lstHaveTargetDetailMRD.Count; j++)
                                {
                                    if (lstHaveTargetDetailMRD.Count != (j + 1))
                                    {
                                        #region 设置样式
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 1].SetStyle(style2);
                                        Style tempStyle1 = worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 1].GetStyle();
                                        tempStyle1.HorizontalAlignment = TextAlignmentType.Center;
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 3].SetStyle(style2_color);
                                        Style tempStyle2 = worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 3].GetStyle();
                                        tempStyle2.HorizontalAlignment = TextAlignmentType.Center;
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 1].SetStyle(tempStyle1);
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 2].SetStyle(tempStyle1);
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 3].SetStyle(tempStyle2);
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 4].SetStyle(tempStyle2);
                                        #endregion
                                        #region 为EXCEL赋值
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 1].Value = lstHaveTargetDetailMRD[j].NAccumulativeActualAmmount;
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 2].Value = lstHaveTargetDetailMRD[j].NAccumulativePlanAmmount;
                                        int Rown = rowStart + 1;
                                        string SUMa = string.Empty; //"C" + Rown + "+E" + Rown + "+G" + Rown + "+I" + Rown + "";
                                        string SUMb = string.Empty; //D" + Rown + "+F" + Rown + "+H" + Rown + "+J" + Rown + "";
                                        for (int h = 2; h < strs.Count(); h++)
                                        {
                                            if (((lstHaveNotTargetMRD.Count()) * 2) == h)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (h % 2 == 0)
                                                {
                                                    SUMa = SUMa + strs[h].Split(':')[1].ToString() + Rown + "+";
                                                }
                                                else
                                                {
                                                    SUMb = SUMb + strs[h].Split(':')[1].ToString() + Rown + "+";
                                                }
                                            }
                                        }
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 3].Formula = "=SUM(" + SUMa.TrimEnd('+') + ")";
                                        worksheets[sheetIndex].Cells[rowStart, colStart + j * 2 + 4].Formula = "=SUM(" + SUMb.TrimEnd('+') + ")";
                                        #endregion
                                    }
                                }
                                rowStart = rowStart + 1;
                            }
                        }

                    }


                    worksheets[sheetIndex].Protect(ProtectionType.All);
                }

                designer.Workbook.CalculateFormula();


                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();

                // 设置基本信息  
                SetBaseMessage(stream, rpt._System.SystemName, fileName);

            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
            }
        }



        /// <summary>
        /// 下载直属公司计划指标
        /// </summary>
        /// <param name="context"></param>
        /// <param name="FileType"></param>
        public void DownDirectLyTargetPlanExcel(HttpContext context, string FileType)
        {
            
            string templeteName = "指标上报模版V1.xlsx";
            string fileName = "指标上报";
            ReportInstance rpt = null;
            if (MonthReportID != Guid.Empty)
            {
                rpt = new ReportInstance(MonthReportID, true);
            }
            else
            {
                rpt = new ReportInstance(SysId, FinYear, FinMonth, true);
            }

            var defaultTagetPlan = StaticResource.Instance.GetDefaultTargetPlanList(rpt._SystemID, rpt.FinYear);
            if (defaultTagetPlan == null || defaultTagetPlan.Count == 0)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("请确认有默认的分解指标计划。");
                return;
            }

                bool IsTargetPlan = false;
            if ("DownDirectlyTargetPlan" == FileType)
            {
                IsTargetPlan = true;
            }

            //你疯了？？？？
            List<DictionaryVmodel> ListDV = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, IsTargetPlan);

            string templetePath = Path.Combine(ExcelTempletePath, templeteName);
            if (System.IO.File.Exists(templetePath))
            {
                ExcelEngine excel = new ExcelEngine();
                WorkbookDesigner designer = new WorkbookDesigner();
                FileStream fileStream = new FileStream(templetePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                designer.Workbook = new Workbook(fileStream);
                WorksheetCollection worksheets = designer.Workbook.Worksheets;
                Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
                Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

                #region style1 样式
                style1.Font.Size = 16;
                style1.Font.IsBold = true;
                style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
                style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
                style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
                style1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
                style1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
                style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
                #endregion
                #region style2 样式 无加粗
                style2.Font.Size = 16;
                style2.Number = 3;
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
                style3.Font.Size = 16;
                style2.Number = 3;
                style3.ForegroundColor = System.Drawing.Color.FromArgb(149, 179, 215);
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
                style4.Font.Size = 16;
                style4.HorizontalAlignment = TextAlignmentType.Center;
                style4.ForegroundColor = System.Drawing.Color.White;
                style4.Pattern = BackgroundType.Solid;
                #endregion

                worksheets[0].Cells[0, 1].PutValue(rpt._System.SystemName + fileName);
                worksheets[0].Cells[1, 3].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Name = rpt._System.SystemName + fileName;

                int rowStart = 4;  //开始行
                int colStart = 1; // 开始列
                excel.SetCustomProperty(worksheets[0], "SystemID", rpt._System.ID.ToString());
                excel.SetCustomProperty(worksheets[0], "SheetName", "DirectlyMonthReportDetail");
                if (ListDV.Count > 0)
                {
                    List<MonthReportSummaryViewModel> listMonthReport = (List<MonthReportSummaryViewModel>)ListDV[2].ObjValue;
                    for (int i = 0; i < listMonthReport.Count(); i++)
                    {
                        #region 设置样式
                        worksheets[0].Cells[rowStart, colStart].SetStyle(style3);
                        worksheets[0].Cells[rowStart, colStart + 1].SetStyle(style3);
                        worksheets[0].Cells[rowStart, colStart + 2].SetStyle(style3);
                        worksheets[0].Cells[rowStart, colStart + 3].SetStyle(style3);
                        worksheets[0].Cells[rowStart, colStart + 4].SetStyle(style2);
                        #endregion

                        #region 赋值
                        worksheets[0].Cells[rowStart, colStart].PutValue(i + 1);
                        worksheets[0].Cells[rowStart, colStart + 1].PutValue(listMonthReport[i].ID);
                        worksheets[0].Cells[rowStart, colStart + 2].PutValue(listMonthReport[i].TargetName);
                        worksheets[0].Cells[rowStart, colStart + 3].PutValue(listMonthReport[i].NPlanAmmount);
                        worksheets[0].Cells[rowStart, colStart + 4].PutValue(listMonthReport[i].NActualAmmount);
                        #endregion

                        #region 设置千分位
                        Style tempstyle;
                        tempstyle = worksheets[0].Cells[rowStart, colStart + 3].GetStyle();
                        tempstyle.Number = 3;
                        worksheets[0].Cells[rowStart, colStart + 3].SetStyle(tempstyle);

                        tempstyle = worksheets[0].Cells[rowStart, colStart + 4].GetStyle();
                        tempstyle.IsLocked = false;
                        worksheets[0].Cells[rowStart, colStart + 4].SetStyle(tempstyle);
                        #endregion
                        rowStart = rowStart + 1;
                    }
                    worksheets[0].Protect(ProtectionType.All);
                }

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();

                // 设置基本信息  
                SetBaseMessage(stream, rpt._System.SystemName, fileName);
            }

        }



        /// <summary>
        /// 设置基本信息  
        /// </summary>
        /// <param name="stream">内存流</param>
        /// <param name="SystemName">当前系统名称</param>
        /// <param name="FileName">文件名称</param>
        private void SetBaseMessage(MemoryStream stream, string SystemName, string FileName)
        {
            //设置基本信息   
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(SystemName + FileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }



        /// <summary>
        /// 获取配置的混合指标
        /// </summary>
        /// <param name="rpt"></param>
        /// <returns></returns>
        private Tuple<bool, List<C_Target>> GetBlendTargets(ReportInstance rpt)
        {
            var blendTargetResult = new Tuple<bool, List<C_Target>>(false, null);
            try
            {
                XElement xml = rpt._System.Configuration;
                XElement isBlendTarget = xml.Elements("ComplateTargetDetail").Elements("BlendTargets").FirstOrDefault();
                if (isBlendTarget != null && isBlendTarget.GetAttributeValue("IsBlendTarget", false))
                {
                    var _TargetList = Array.ConvertAll<string, Guid>(isBlendTarget.GetAttributeValue("TargetValue", "").Split(','), s => Guid.Parse(s)).ToList();

                    //混合指标只能是两个
                    if (_TargetList != null || _TargetList.Count == 2)
                    {
                        var blendTarget = rpt._Target.Where(v => _TargetList.Contains(v.ID)).OrderBy(v => v.Sequence).ToList();
                        if (blendTarget.Count == 2)
                            blendTargetResult = new Tuple<bool, List<C_Target>>(true, blendTarget);
                    }
                }
            }
            catch (Exception e)
            {
            }
            return blendTargetResult;
        }
    }

}