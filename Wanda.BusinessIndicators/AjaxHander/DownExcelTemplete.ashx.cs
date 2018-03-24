
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
using System.Text.RegularExpressions;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownExcelTemplete 的摘要说明
    /// </summary>
    public partial class DownExcelTemplete : IHttpHandler, IRequiresSessionState
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
        Guid MonthlyReportID = Guid.Empty;
        int FinYear = 0;
        int FinMonth = 0;
        bool IsLatestVersion = false;
        string ImageFilePath = HttpContext.Current.Server.MapPath("../Images/images1");
        string FileType = string.Empty; //下载的文件类型
        ReportInstance rpt = null;
        string OrderStr = "Detail";//明细下载排序字段,默认是按照累计排序的

        string DataSource = string.Empty; //B表数据源

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

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

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["MonthlyReportID"]))
            {
                MonthlyReportID = Guid.Parse(HttpContext.Current.Request["MonthlyReportID"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["OrderStr"]))
            {
                OrderStr = HttpContext.Current.Request["OrderStr"].ToString();
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["DataSource"]))
            {
                DataSource = HttpContext.Current.Request["DataSource"].ToString();
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsLatestVersion"]))
            {
                if (HttpContext.Current.Request["IsLatestVersion"] == "true")
                {
                    IsLatestVersion = true;
                }

            }

            if (FileType != "HistoryReturn")
            {
                if (MonthlyReportID == Guid.Empty)
                {
                    rpt = new ReportInstance(SysId, FinYear, FinMonth, IsLatestVersion, DataSource);
                }
                else
                {
                    rpt = new ReportInstance(MonthlyReportID, true, DataSource);
                    FinMonth = rpt.FinMonth;
                    FinYear = rpt.FinYear;
                    SysId = rpt._SystemID;
                }
            }

            switch (FileType)
            {
                case "TargetReturn":
                    DownTarget_Return(); //下载补回明细模版
                    break;
                case "TargetSummary":
                    DownTarget_Summary();   //下载指标统计模版
                    break;
                case "MissTarget":
                    DownMissTarget(); //下载未完成说明模版
                    break;
                case "CurrentMissTarget":
                    DownCurrentMissTarget(); //下载未完成说明模版
                    break;
                case "MissTargetRpt":
                    DownMissTargetRpt(); //下载未完成说明模版（上报）
                    break;
                case "TargetDetail":
                    DownTarget_Detail();      //下载指标明细模版
                    break;
                case "HistoryReturn":
                    DownHistoryReturnReport();
                    break;

            }

        }

        //未完成说明模版（上报）
        private void DownMissTargetRpt()
        {
            //根据系统自然月，自动计算

            FinYear = DateTime.Now.Year;
            FinMonth = DateTime.Now.Month - 1;

            //ReportInstance rpt = new ReportInstance(SysId, FinYear, FinMonth, true);
            List<DictionaryVmodel> targetReturn = null;
            if (rpt != null)
            {
                targetReturn = ReportInstanceReturnEngine.ReportInstanceReturnService.GetReturnRptDataSource(rpt, rpt._System);  //rpt.GetReturnRptDataSource();
            }

            string templetePath = ExcelTempletePath;
            string templeteName = "未完成说明数据模版V1.xlsx";

            string fileName = string.Empty;

            if (rpt._System != null)
                fileName = rpt._System.SystemName + "未完成说明填报";
            else
                fileName = "未完成说明填报";

            ExcelMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, true);
        }

        /// <summary>
        /// 下载完成情况明细报表
        /// </summary>
        private void DownTarget_Detail()
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细";
                    DownExcelMonthReportDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    break;
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "项目公司完成明细模版V1.xlsx";
                    fileName = "完成情况明细";

                    DownExcelMonthReportDetail_XM(rpt, templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    break;
                case 3:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-集团总部V1.xlsx";
                    fileName = "完成情况明细";
                    DownExcelMonthReportDetail_Group(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细";
                    DownExcelMonthReportDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    break;
            }

        }


        /// <summary>
        /// 累计未完成说明（月度报告）
        /// </summary>
        private void DownMissTarget()
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            DateTime Dt = DateTime.Now;
            if (rpt._MonthReportID != Guid.Empty)
            {
                B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(rpt._MonthReportID);
                if (bm != null)
                {
                    Dt = bm.CreateTime;
                }
            }
            C_System sys = StaticResource.Instance[SysId, Dt];
            switch (sys.Category)
            {
                case 1:
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "未完成说明数据模版V1.xlsx";
                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "未完成说明";
                    else
                        fileName = "未完成说明";
                    List<DictionaryVmodel> targetReturn = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                    ExcelMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    break;
                case 3:
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "未完成说明数据模版V1.xlsx";
                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "未完成说明";
                    else
                        fileName = "未完成说明";
                    List<DictionaryVmodel> DirectlyMissTarget = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                    ExcelMisssTarget_Directly(rpt, DirectlyMissTarget, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    break;
            }

        }

        /// <summary>
        /// 未完成说明
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void ExcelMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            Aspose.Cells.Style style0 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2_left = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];


            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
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

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";

            //style2.Font.IsBold = true;

            style2.IsTextWrapped = true;
            style2.Pattern = BackgroundType.Solid;
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

            #region style2_left 样式
            style2_left.Font.Size = 12;
            style2_left.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            style2_left.IsTextWrapped = true;
            style2_left.Pattern = BackgroundType.Solid;
            style2_left.Number = 3;
            style2_left.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;

            style2_left.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.Color = Color.Red;
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

            #region style4字体黑色

            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
            //style4.Font.Color = Color.Red;
            style4.Font.IsBold = true;
            style4.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style4.Pattern = BackgroundType.Solid;

            style4.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion

            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name.Replace('\\', '-');
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                List<DictionaryVmodel> itemList = (List<DictionaryVmodel>)List[i].ObjValue;
                foreach (DictionaryVmodel item in itemList)
                {
                    StyleFlag flg = new StyleFlag();
                    flg.All = true;

                    if (item.Mark == "Counter")
                    {
                        #region 插入合并项


                        Range range1 = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 14);
                        range1.Merge();
                        range1.ApplyStyle(style3, flg);
                        worksheets[i].Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //excel 加入一行数据
                        #endregion

                        List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue; //当前的objValue里的数据递归

                        rowStart = rowStart + 1;
                        int groupRowSart = rowStart;

                        MissTargetParseGroup(List[i].Name, list, worksheets[i], ref rowStart, colStart, style1, style2, style4, IsRpt);  //递归循环

                        if (rowStart - groupRowSart > 0)  //分组数据
                        {
                            int endRow = rowStart - 1;
                            worksheets[i].Cells.GroupRows(groupRowSart, endRow, true);
                        }

                    }
                    else if (item.Mark == "Data")
                    {
                        int _MergeCol = 0; //合并列

                        List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                        #region 插入合并项并且附加样式

                        Range range = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 15);
                        range.Merge();
                        range.ApplyStyle(style4, flg);

                        worksheets[i].Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //excel 加入一行数据
                        #endregion

                        int index = 1;
                        int rowStartR = rowStart + 1;

                        ReportDetailLisr.ForEach(p =>
                        {
                            #region 设置表格中的所有样式

                            //设置Excel的样式
                            worksheets[i].Cells[rowStartR, 1].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart].SetStyle(style2);

                            if (p.SystemID == Guid.Parse(ConfigurationManager.AppSettings["MonthSG"]))
                            {
                                worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                                worksheets[i].Cells.Columns[colStart + 1].Width = 0;// 指标名称, 在商管系统的时候隐藏
                            }
                            else
                            {
                                worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                            }

                            worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2_left);
                            worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 12].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 13].SetStyle(style2);

                            #endregion

                            #region 加载数据

                            //加载数据
                            _MergeCol++;

                            worksheets[i].Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称

                            if (List[i].TargetGroupCount != 1)  //这里针对分组的指标
                            {
                                if (_MergeCol < List[i].TargetGroupCount)
                                {
                                    if (_MergeCol == 1)
                                    {
                                        //指标合并
                                        worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号

                                        Range itemRangeByID = worksheets[i].Cells.CreateRange(rowStartR, 1, List[i].TargetGroupCount, 1);
                                        itemRangeByID.Merge();
                                        itemRangeByID.ApplyStyle(style2, flg);

                                        Range itemRangeByCompanyName = worksheets[i].Cells.CreateRange(rowStartR, colStart, List[i].TargetGroupCount, 1);
                                        itemRangeByCompanyName.Merge();
                                        itemRangeByCompanyName.ApplyStyle(style2, flg);

                                        Range itemRangeByMIssTargetReason = worksheets[i].Cells.CreateRange(rowStartR, colStart + 10, List[i].TargetGroupCount, 1);
                                        itemRangeByMIssTargetReason.Merge();
                                        itemRangeByMIssTargetReason.ApplyStyle(style2_left, flg);
                                    }
                                }
                                else
                                {
                                    _MergeCol = 0;
                                }
                            }
                            else  //没有分组的指标
                            {
                                worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号
                            }

                            worksheets[i].Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称

                            decimal tempDiff = p.NAccumulativeDifference - p.LastNAccumulativeDifference;

                            if (tempDiff < 0)
                            { worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1); }
                            else
                            {
                                worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2);
                            }

                            worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                            worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                            worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际

                            if (p.LastNAccumulativeDifference < 0)
                            { worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style1); }
                            else
                            { worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2); }

                            worksheets[i].Cells[rowStartR, colStart + 5].PutValue(p.LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值
                            worksheets[i].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);
                            worksheets[i].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);


                            //这里是为了给数字加红
                            if (p.NAccumulativeDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1);
                            }
                            else
                            {
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                            }

                            worksheets[i].Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                            worksheets[i].Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate);//本月完成率
                            if (IsRpt) //是上报模版
                            {
                                if (p.ReturnType == (int)EnumReturnType.New) //本月新增
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 10].PutValue("");
                                    worksheets[i].Cells[rowStartR, colStart + 11].PutValue("");
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(p.MIssTargetReason) == false || string.IsNullOrEmpty(p.MIssTargetDescription) == false)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue("未完成原因:" + p.MIssTargetReason + "\n\r采取措施:" + p.MIssTargetDescription); //未完成原因,及采取措施
                                    }

                                    if (p.CommitDate != null)
                                    {
                                        string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月"); //  CommitDate.Substring(0, CommitDate.Length - 3)
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue("——"); ;
                                    }

                                }
                            }
                            else
                            {

                                if (string.IsNullOrEmpty(p.MIssTargetReason) == false || string.IsNullOrEmpty(p.MIssTargetDescription) == false)
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.MIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.MIssTargetDescription) ); //未完成原因,及采取措施
                                }

                                if (p.CommitDate != null)
                                {
                                    string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                    worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 11].PutValue("——"); ;
                                }

                            }

                            if (p.ReturnType != 0)
                            {
                                string PromissDate = string.Empty;

                                if (p.PromissDate != null)
                                {
                                    //PromissDate = string.Format("{0:MM月}", p.PromissDate);

                                    PromissDate = "承诺" + p.PromissDate.Value.Month.ToString() + "月份补回";
                                }

                                if (p.CommitDate != null && p.CommitDate.Value.Month ==12 && p.CommitDate.Value.Day == 31 )
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                                }
                                
                            }

                            if (p.Counter > 0)
                            {
                                int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 13, ImageFilePath + "\\image" + p.Counter + ".png");
                                Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                                picture.Left = 40;
                                picture.Top = 10;
                            }



                            #endregion

                            rowStartR++;
                        });


                        #region 分组数据Excel

                        if (ReportDetailLisr.Count > 0)
                        {
                            worksheets[i].Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                        }

                        #endregion

                        rowStart = rowStartR;
                    }
                }
            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }


        /// <summary>
        /// 未完成说明
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void ExcelMisssTarget_Directly(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            Aspose.Cells.Style style0 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2_left = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];


            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
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

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";

            //style2.Font.IsBold = true;

            style2.IsTextWrapped = true;
            style2.Pattern = BackgroundType.Solid;
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

            #region style2_left 样式
            style2_left.Font.Size = 12;
            style2_left.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            style2_left.IsTextWrapped = true;
            style2_left.Pattern = BackgroundType.Solid;
            style2_left.Number = 3;
            style2_left.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;

            style2_left.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.Color = Color.Red;
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

            #region style4字体黑色

            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
            //style4.Font.Color = Color.Red;
            style4.Font.IsBold = true;
            style4.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style4.Pattern = BackgroundType.Solid;

            style4.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion

            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name;
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;
                List<MonthlyReportDetail> itemList = (List<MonthlyReportDetail>)List[i].ObjValue;
                int index = 1;
                foreach (MonthlyReportDetail p in itemList)
                {
                    StyleFlag flg = new StyleFlag();
                    flg.All = true;
                    int rowStartR = rowStart;
                    #region 设置表格中的所有样式

                    //设置Excel的样式
                    worksheets[0].Cells[rowStartR, 1].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart].SetStyle(style2);
                    worksheets[i].Cells.Columns[colStart].Width = 0;
                    worksheets[0].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                    worksheets[0].Cells[rowStartR, colStart + 2].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 3].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 4].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 5].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 6].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 8].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 9].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 10].SetStyle(style2_left);
                    worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 12].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 13].SetStyle(style2);

                    #endregion

                    #region 加载数据
                    worksheets[0].Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称
                    worksheets[0].Cells[rowStartR, 1].PutValue(index++); //序号
                    worksheets[0].Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称
                    decimal tempDiff = p.NAccumulativeDifference - p.LastNAccumulativeDifference;
                    if (tempDiff < 0)
                    { worksheets[0].Cells[rowStartR, colStart + 2].SetStyle(style1); }
                    else
                    {
                        worksheets[0].Cells[rowStartR, colStart + 2].SetStyle(style2);
                    }

                    worksheets[0].Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                    worksheets[0].Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                    worksheets[0].Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际

                    if (p.LastNAccumulativeDifference < 0)
                    { worksheets[0].Cells[rowStartR, colStart + 5].SetStyle(style1); }
                    else
                    { worksheets[0].Cells[rowStartR, colStart + 5].SetStyle(style2); }

                    worksheets[0].Cells[rowStartR, colStart + 5].PutValue(p.LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值
                    worksheets[0].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);
                    worksheets[0].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);


                    //这里是为了给数字加红
                    if (p.NAccumulativeDifference < 0)
                    {
                        worksheets[0].Cells[rowStartR, colStart + 8].SetStyle(style1);
                    }
                    else
                    {
                        worksheets[0].Cells[rowStartR, colStart + 8].SetStyle(style2);
                    }

                    worksheets[0].Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                    worksheets[0].Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate);//本月完成率
                    if (IsRpt) //是上报模版
                    {
                        if (p.ReturnType == (int)EnumReturnType.New) //本月新增
                        {
                            worksheets[0].Cells[rowStartR, colStart + 10].PutValue("");
                            worksheets[0].Cells[rowStartR, colStart + 11].PutValue("");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(p.MIssTargetReason) == false || string.IsNullOrEmpty(p.MIssTargetDescription) == false)
                            {
                                worksheets[0].Cells[rowStartR, colStart + 10].PutValue("未完成原因:" + p.MIssTargetReason + "\n\r采取措施:" + p.MIssTargetDescription); //未完成原因,及采取措施
                            }

                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3) 
                            }
                            else
                            {
                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue("——");
                            }

                        }
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(p.MIssTargetReason) == false || string.IsNullOrEmpty(p.MIssTargetDescription) == false)
                        {
                            worksheets[0].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.MIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.MIssTargetDescription) ); //未完成原因,及采取措施
                        }

                        if (p.CommitDate != null)
                        {
                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                            worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                        }
                        else
                        {
                            worksheets[0].Cells[rowStartR, colStart + 11].PutValue("——");
                        }

                    }

                    if (p.ReturnType != 0)
                    {
                        string PromissDate = string.Empty;

                        if (p.PromissDate != null)
                        {
                            //PromissDate = string.Format("{0:M月}", p.PromissDate);
                            PromissDate = "承诺" + p.PromissDate.Value.Month.ToString() + "月份补回";
                        }

                        if (p.CommitDate != null && p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                        {
                            worksheets[0].Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                        }
                        else
                        {
                            worksheets[0].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                        }

                       // worksheets[0].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                    }

                    if (p.Counter > 0)
                    {
                        int pictureIndex = worksheets[0].Pictures.Add(rowStartR, colStart + 13, ImageFilePath + "\\image" + p.Counter + ".png");
                        Aspose.Cells.Drawing.Picture picture = worksheets[0].Pictures[pictureIndex];
                        picture.Left = 40;
                        picture.Top = 10;
                    }
                    #endregion
                    rowStartR++;
                    rowStart = rowStartR;
                }
            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }



        /// <summary>
        /// 当月未完成说明（月度报告）
        /// </summary>
        private void DownCurrentMissTarget()
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            DateTime Dt = DateTime.Now;
            if (rpt._MonthReportID != Guid.Empty)
            {
                B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(rpt._MonthReportID);
                if (bm != null)
                {
                    Dt = bm.CreateTime;
                }
            }
            C_System sys = StaticResource.Instance[SysId, Dt];
            switch (sys.Category)
            {
                case 1:
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "当月未完成说明数据模版V1.xlsx";
                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "当月未完成说明";
                    else
                        fileName = "当月未完成说明";

                    List<DictionaryVmodel> targetReturn = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);          
                    ExcelCurrentMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    break;
                case 3:
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "当月未完成说明数据模版V1.xlsx";
                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "当月未完成说明";
                    else
                        fileName = "当月未完成说明";

                    List<DictionaryVmodel> DirectlyMissTarget = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);
                    ExcelCurrentMisssTarget_Directly(rpt, DirectlyMissTarget, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    break;
            }

        }

        /// <summary>
        /// 当月未完成说明
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void ExcelCurrentMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            Aspose.Cells.Style style0 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2_left = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];


            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
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

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";

            //style2.Font.IsBold = true;

            style2.IsTextWrapped = true;
            style2.Pattern = BackgroundType.Solid;
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

            #region style2_left 样式
            style2_left.Font.Size = 12;
            style2_left.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            style2_left.IsTextWrapped = true;
            style2_left.Pattern = BackgroundType.Solid;
            style2_left.Number = 3;
            style2_left.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;

            style2_left.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.Color = Color.Red;
            style3.Font.IsBold = true;
            style3.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style3.Pattern = BackgroundType.Solid;


            style3.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion

            #region style4字体黑色

            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
            //style4.Font.Color = Color.Red;
            style4.Font.IsBold = true;
            style4.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style4.Pattern = BackgroundType.Solid;

            style4.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion

            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "当月经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "当月未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name.Replace('\\', '-');
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                List<DictionaryVmodel> itemList = (List<DictionaryVmodel>)List[i].ObjValue;
                foreach (DictionaryVmodel item in itemList)
                {
                    StyleFlag flg = new StyleFlag();
                    flg.All = true;

                    if (item.Mark == "Counter")
                    {
                        #region 插入合并项


                        Range range1 = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 14);
                        range1.Merge();
                        range1.ApplyStyle(style3, flg);
                        worksheets[i].Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //excel 加入一行数据
                        #endregion

                        List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue; //当前的objValue里的数据递归

                        rowStart = rowStart + 1;
                        int groupRowSart = rowStart;

                        CurrentMissTargetParseGroup(List[i].Name, list, worksheets[i], ref rowStart, colStart, style1, style2, style4, IsRpt);  //递归循环

                        if (rowStart - groupRowSart > 0)  //分组数据
                        {
                            int endRow = rowStart - 1;
                            worksheets[i].Cells.GroupRows(groupRowSart, endRow, true);
                        }

                    }
                    else if (item.Mark == "Data")
                    {
                        int _MergeCol = 0; //合并列

                        List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                        #region 插入合并项并且附加样式

                        Range range = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 15);
                        range.Merge();
                        range.ApplyStyle(style4, flg);

                        worksheets[i].Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //excel 加入一行数据
                        #endregion

                        int index = 1;
                        int rowStartR = rowStart + 1;

                        ReportDetailLisr.ForEach(p =>
                        {
                            if (p.CompanyName != "未完成合计")
                            {
                                #region 设置表格中的所有样式

                                //设置Excel的样式
                                worksheets[i].Cells[rowStartR, 1].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart].SetStyle(style2);

                                if (p.SystemID == Guid.Parse(ConfigurationManager.AppSettings["MonthSG"]))
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                                    worksheets[i].Cells.Columns[colStart + 1].Width = 0;// 指标名称, 在商管系统的时候隐藏
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                                }

                                worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2_left);
                                worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 12].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 13].SetStyle(style2);

                                #endregion

                                #region 加载数据

                                //加载数据
                                _MergeCol++;

                                worksheets[i].Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称

                                if (List[i].TargetGroupCount != 1)  //这里针对分组的指标
                                {
                                    if (_MergeCol < List[i].TargetGroupCount)
                                    {
                                        if (_MergeCol == 1)
                                        {
                                            //指标合并
                                            worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号

                                            Range itemRangeByID = worksheets[i].Cells.CreateRange(rowStartR, 1, List[i].TargetGroupCount, 1);
                                            itemRangeByID.Merge();
                                            itemRangeByID.ApplyStyle(style2, flg);

                                            Range itemRangeByCompanyName = worksheets[i].Cells.CreateRange(rowStartR, colStart, List[i].TargetGroupCount, 1);
                                            itemRangeByCompanyName.Merge();
                                            itemRangeByCompanyName.ApplyStyle(style2, flg);

                                            Range itemRangeByMIssTargetReason = worksheets[i].Cells.CreateRange(rowStartR, colStart + 10, List[i].TargetGroupCount, 1);
                                            itemRangeByMIssTargetReason.Merge();
                                            itemRangeByMIssTargetReason.ApplyStyle(style2_left, flg);
                                        }
                                    }
                                    else
                                    {
                                        _MergeCol = 0;
                                    }
                                }
                                else  //没有分组的指标
                                {
                                    worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号
                                }

                                worksheets[i].Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称

                                //decimal tempDiff = p.NAccumulativeDifference - p.LastNAccumulativeDifference;

                                //if (tempDiff < 0)
                                //{ worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1); }
                                //else
                                //{
                                //    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2);
                                //}

                                worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NPlanAmmount); //当月计划
                                worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.NActualAmmount); //当月实际
                                worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.NDifference);//当月差值

                                if (p.NDifference < 0)
                                { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style1); }
                                else
                                { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2); }

                                worksheets[i].Cells[rowStartR, colStart + 5].PutValue(p.NDisplayRate); //当月完成率
                                worksheets[i].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount); // 当月累计计划
                                worksheets[i].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);// 当月累计实际


                                //这里是为了给数字加红
                                if (p.NAccumulativeDifference < 0)
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1);
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                                }

                                worksheets[i].Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月累计差值

                                worksheets[i].Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate);//本月累计完成率
                                if (IsRpt) //是上报模版
                                {
                                    if (p.ReturnType == (int)EnumReturnType.New) //本月新增
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue("");
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue("");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(p.CurrentMIssTargetReason) == false || string.IsNullOrEmpty(p.CurrentMIssTargetDescription) == false)
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 10].PutValue("未完成原因:" + p.CurrentMIssTargetReason + "\n\r采取措施:" + p.CurrentMIssTargetDescription); //未完成原因,及采取措施
                                        }

                                        if (p.CommitDate != null)
                                        {
                                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月"); //  CommitDate.Substring(0, CommitDate.Length - 3)
                                        }
                                        else
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue("——"); ;
                                        }

                                    }
                                }
                                else
                                {

                                    if (string.IsNullOrEmpty(p.CurrentMIssTargetReason) == false || string.IsNullOrEmpty(p.CurrentMIssTargetDescription) == false)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.CurrentMIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.CurrentMIssTargetDescription) ); //未完成原因,及采取措施
                                    }

                                    if (p.CommitDate != null)
                                    {
                                        string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue("——"); ;
                                    }

                                }

                                if (p.ReturnType != 0)
                                {
                                    string PromissDate = string.Empty;

                                    if (p.PromissDate != null)
                                    {
                                        PromissDate = "承诺" + p.PromissDate.Value.Month.ToString() + "月份补回";
                                    }

                                    if (p.CommitDate != null && p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                                    }

                                    //worksheets[i].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 12].PutValue("累计完成");
                                }

                                if (p.Counter > 0)
                                {
                                    int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 13, ImageFilePath + "\\image" + p.Counter + ".png");
                                    Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                                    picture.Left = 40;
                                    picture.Top = 10;
                                }



                                #endregion

                                rowStartR++;
                            }
                        });


                        #region 分组数据Excel

                        if (ReportDetailLisr.Count > 0)
                        {
                            worksheets[i].Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                        }

                        #endregion

                        rowStart = rowStartR;
                    }
                }
            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }


        /// <summary>
        /// 当月未完成说明 直管
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void ExcelCurrentMisssTarget_Directly(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            Aspose.Cells.Style style0 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2_left = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];


            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
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

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";

            //style2.Font.IsBold = true;

            style2.IsTextWrapped = true;
            style2.Pattern = BackgroundType.Solid;
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

            #region style2_left 样式
            style2_left.Font.Size = 12;
            style2_left.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            style2_left.IsTextWrapped = true;
            style2_left.Pattern = BackgroundType.Solid;
            style2_left.Number = 3;
            style2_left.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;

            style2_left.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_left.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_left.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_left.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.Color = Color.Red;
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

            #region style4字体黑色

            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
            //style4.Font.Color = Color.Red;
            style4.Font.IsBold = true;
            style4.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style4.Pattern = BackgroundType.Solid;

            style4.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion

            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "当月经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "当月未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name;
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;
                List<MonthlyReportDetail> itemList = (List<MonthlyReportDetail>)List[i].ObjValue;
                int index = 1;
                foreach (MonthlyReportDetail p in itemList)
                {
                    StyleFlag flg = new StyleFlag();
                    flg.All = true;
                    int rowStartR = rowStart;
                    #region 设置表格中的所有样式

                    //设置Excel的样式
                    worksheets[0].Cells[rowStartR, 1].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart].SetStyle(style2);
                    worksheets[i].Cells.Columns[colStart].Width = 0;
                    worksheets[0].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                    worksheets[0].Cells[rowStartR, colStart + 2].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 3].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 4].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 5].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 6].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 8].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 9].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 10].SetStyle(style2_left);
                    worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 12].SetStyle(style2);
                    worksheets[0].Cells[rowStartR, colStart + 13].SetStyle(style2);

                    #endregion

                    #region 加载数据
                    worksheets[0].Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称
                    worksheets[0].Cells[rowStartR, 1].PutValue(index++); //序号
                    worksheets[0].Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称
                    
                    worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NPlanAmmount); //当月计划
                    worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.NActualAmmount); //当月实际
                    worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.NDifference);//当月差值

                    if (p.NDifference < 0)
                    { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style1); }
                    else
                    { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2); }

                    worksheets[i].Cells[rowStartR, colStart + 5].PutValue(p.NDisplayRate); //当月完成率
                    worksheets[i].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount); // 当月累计计划
                    worksheets[i].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);// 当月累计实际


                    //这里是为了给数字加红
                    if (p.NAccumulativeDifference < 0)
                    {
                        worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1);
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                    }



                    worksheets[0].Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                    worksheets[0].Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate);//本月完成率
                    if (IsRpt) //是上报模版
                    {
                        if (p.ReturnType == (int)EnumReturnType.New) //本月新增
                        {
                            worksheets[0].Cells[rowStartR, colStart + 10].PutValue("");
                            worksheets[0].Cells[rowStartR, colStart + 11].PutValue("");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(p.CurrentMIssTargetReason) == false || string.IsNullOrEmpty(p.CurrentMIssTargetDescription) == false)
                            {
                                worksheets[0].Cells[rowStartR, colStart + 10].PutValue("未完成原因:" + p.CurrentMIssTargetReason + "\n\r采取措施:" + p.CurrentMIssTargetDescription); //未完成原因,及采取措施
                            }

                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3) 
                            }
                            else
                            {
                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue("——");
                            }

                        }
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(p.CurrentMIssTargetReason) == false || string.IsNullOrEmpty(p.CurrentMIssTargetDescription) == false)
                        {
                            worksheets[0].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.CurrentMIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.CurrentMIssTargetDescription)); //未完成原因,及采取措施
                        }

                        if (p.CommitDate != null)
                        {
                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                            worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                        }
                        else
                        {
                            worksheets[0].Cells[rowStartR, colStart + 11].PutValue("——");
                        }

                    }

                    if (p.ReturnType != 0)
                    {
                        string PromissDate = string.Empty;

                        if (p.PromissDate != null)
                        {
                            //PromissDate = string.Format("{0:M月}", p.PromissDate);

                            PromissDate = "承诺" + p.PromissDate.Value.Month.ToString() + "月份补回";
                        }

                        if (p.CommitDate != null && p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                        {
                            worksheets[0].Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                        }
                        else
                        {
                            worksheets[0].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                        }

                        //worksheets[0].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                    }
                    else
                    {
                        worksheets[0].Cells[rowStartR, colStart + 12].PutValue("累计完成");
                    }

                    if (p.Counter > 0)
                    {
                        int pictureIndex = worksheets[0].Pictures.Add(rowStartR, colStart + 13, ImageFilePath + "\\image" + p.Counter + ".png");
                        Aspose.Cells.Drawing.Picture picture = worksheets[0].Pictures[pictureIndex];
                        picture.Left = 40;
                        picture.Top = 10;
                    }
                    #endregion
                    rowStartR++;
                    rowStart = rowStartR;
                }
            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }


        /// <summary>
        /// 未完成说明，递归
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="MissTargetList"></param>
        /// <param name="worksheet"></param>
        /// <param name="rowStart"></param>
        /// <param name="colStart"></param>
        /// <param name="style1"></param>
        /// <param name="style2"></param>
        /// <param name="style3"></param>
        /// <param name="IsRpt"></param>
        public void CurrentMissTargetParseGroup(string TargetName, List<DictionaryVmodel> MissTargetList, Worksheet worksheet, ref int rowStart, int colStart, Style style1, Style style2, Style style3, bool IsRpt)
        {
            //返回的数据
            foreach (var item in MissTargetList)
            {
                StyleFlag flg = new StyleFlag();
                flg.All = true;

                if (item.Mark == "Counter")
                {
                    #region 插入合并项 ，添加标题


                    Range range1 = worksheet.Cells.CreateRange(rowStart, 1, 1, 14);
                    range1.Merge();
                    range1.ApplyStyle(style3, flg);
                    worksheet.Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //添加一行

                    #endregion

                    List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue;

                    rowStart = rowStart + 1;

                    CurrentMissTargetParseGroup(TargetName, list, worksheet, ref rowStart, colStart, style1, style2, style3, IsRpt);

                }
                else if (item.Mark == "Data")
                {
                    List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;


                    #region 插入合并项并且附加样式 ，添加标题

                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 14);
                    range.Merge();
                    range.ApplyStyle(style3, flg);
                    worksheet.Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //excel 加入一行数据

                    #endregion

                    int rowStartR = rowStart + 1;


                    int index = 1;
                    int _MergeCol = 0;//指标合并标识

                    ReportDetailLisr.ForEach(p =>
                    {
                        #region 设置表格中的所有样式

                        //设置Excel的样式
                        worksheet.Cells[rowStartR, 1].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 1].SetStyle(style2);

                        worksheet.Cells[rowStartR, colStart + 2].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 3].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 4].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 5].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 6].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 7].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 8].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 9].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 10].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 11].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 12].SetStyle(style2);

                        #endregion

                        #region 加载数据

                        //加载数据
                        _MergeCol++;

                        worksheet.Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称

                        if (MissTargetList[0].TargetGroupCount != 1) //分组指标
                        {
                            if (_MergeCol < MissTargetList[0].TargetGroupCount)
                            {
                                if (_MergeCol == 1)
                                {
                                    //指标合并
                                    worksheet.Cells[rowStartR, 1].PutValue(index++); //序号

                                    Range itemRangeByID = worksheet.Cells.CreateRange(rowStartR, 1, MissTargetList[0].TargetGroupCount, 1);
                                    itemRangeByID.Merge();
                                    itemRangeByID.ApplyStyle(style2, flg);

                                    Range itemRangeByCompanyName = worksheet.Cells.CreateRange(rowStartR, colStart, MissTargetList[0].TargetGroupCount, 1);
                                    itemRangeByCompanyName.Merge();
                                    itemRangeByCompanyName.ApplyStyle(style2, flg);

                                    Range itemRangeByMIssTargetReason = worksheet.Cells.CreateRange(rowStartR, colStart + 10, MissTargetList[0].TargetGroupCount, 1);
                                    itemRangeByMIssTargetReason.Merge();
                                    itemRangeByMIssTargetReason.ApplyStyle(style2, flg);
                                }
                            }
                            else
                            {
                                _MergeCol = 0;
                            }
                        }
                        else //单个指标
                        {
                            worksheet.Cells[rowStartR, 1].PutValue(index++); //序号
                        }

                        worksheet.Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称

                        
                        worksheet.Cells[rowStartR, colStart + 2].PutValue(p.NPlanAmmount); //当月计划
                        worksheet.Cells[rowStartR, colStart + 3].PutValue(p.NActualAmmount); //当月实际
                        worksheet.Cells[rowStartR, colStart + 4].PutValue(p.NDifference);//当月差值

                        if (p.NDifference < 0)
                        { worksheet.Cells[rowStartR, colStart + 4].SetStyle(style1); }
                        else
                        { worksheet.Cells[rowStartR, colStart + 4].SetStyle(style2); }


                        worksheet.Cells[rowStartR, colStart + 5].PutValue(p.NDisplayRate); //当月完成率

                        worksheet.Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);
                        worksheet.Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);
                        worksheet.Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                        worksheet.Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate); //本月完成率

                        if (IsRpt) //是上报模版
                        {
                            if (p.ReturnType == (int)EnumReturnType.New) //本月新增
                            {
                                worksheet.Cells[rowStartR, colStart + 10].PutValue("");
                                worksheet.Cells[rowStartR, colStart + 11].PutValue("");
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 10].PutValue("未完成原因:" + p.CurrentMIssTargetReason + "\n\r采取措施:" + p.CurrentMIssTargetDescription); //未完成原因,及采取措施

                                if (p.CommitDate != null)
                                {
                                    string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                                }
                                else
                                {
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue("——");
                                }

                            }
                        }
                        else
                        {
                            worksheet.Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.CurrentMIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.CurrentMIssTargetDescription)); //未完成原因,及采取措施

                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 11].PutValue("——");
                            }

                        }

                        if (p.ReturnType != 0)
                        {
                            string PromissDate = string.Empty;

                            if (p.PromissDate != null)
                            {
                                PromissDate = string.Format("{0:M月}", p.PromissDate);

                                PromissDate = "承诺" + PromissDate + "月份补回";
                            }

                            if (p.CommitDate != null && p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                            {
                                worksheet.Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                            }

                            //worksheet.Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                        }
                        if (p.Counter > 0)
                        {
                            int pictureIndex = worksheet.Pictures.Add(rowStartR, colStart + 13, ImageFilePath + "\\image" + p.Counter + ".png");
                            Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                            picture.Left = 40;
                            picture.Top = 10;

                        }
                        //worksheet.Cells[rowStartR, colStart + 13].PutValue(p.Counter);

                        #endregion

                        rowStartR++;
                    });

                    #region 分组数据Excel

                    if (ReportDetailLisr.Count > 0)
                    {
                        worksheet.Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                    }

                    #endregion

                    rowStart = rowStartR;
                }

            }
        }






        private void DownTarget_Summary()
        {
            if (SysId == ConfigurationManager.AppSettings["MonthSG"].ToGuid())
            {
                string templetePath = ExcelTempletePath;
                string templeteName = "月度经营报告_商管系统查询V1.xlsx";
                string fileName = "月度报告";
                DownExcelMonthReportSummary(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
            }
            else
            {
                string templetePath = ExcelTempletePath;
                string templeteName = "月度经营报告V1.xlsx";
                string fileName = "月度报告";
                DownExcelMonthReportSummary(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
            }



        }

        //下载补回情况列表
        private void DownTarget_Return()
        {



            List<DictionaryVmodel> targetReturn = null;
            if (rpt != null)
            {
                C_System Sys;
                if (rpt.ReportDetails != null && rpt.ReportDetails.Count() > 0)
                {
                    Sys = StaticResource.Instance[rpt._System.ID, rpt.ReportDetails[0].CreateTime];
                }
                else
                {
                    Sys = StaticResource.Instance[rpt._System.ID, DateTime.Now];
                }

                targetReturn = ReportInstanceReturnEngine.ReportInstanceReturnService.GetReturnRptDataSource(rpt, Sys);// rpt.GetReturnRptDataSource();
            }

            switch (rpt._System.Category)
            {
                case 1:
                case 2:
                    string templetePath = ExcelTempletePath;
                    string templeteName = "补回情况数据模版V1.xlsx";
                    string fileName = string.Empty;

                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "补回情况";
                    else
                        fileName = "补回情况";
                    ExcelReturnTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth);
                    break;
                case 3:
                    break;
                case 4:
                    string directlyTempletePath = ExcelTempletePath;
                    string directlytempleteName = "补回情况数据模版V1.xlsx";
                    string directlyfileName = string.Empty;

                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "补回情况";
                    else
                        fileName = "补回情况";
                    ExcelDirectlyReturnTarget(rpt, targetReturn, directlyTempletePath, directlytempleteName, fileName, FinYear, FinMonth);
                    break;
            }

        }

        /// <summary>
        /// 补回情况
        /// </summary>
        /// <param name="List"></param>
        /// <param name="templetePath"></param>
        /// <param name="templeteName"></param>
        /// <param name="fileName"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        public void ExcelReturnTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Aspose.Cells.Style style1_Color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            #region style1_Color 样式,无边框
            style1_Color.Font.Size = 12;
            style1_Color.Font.Name = "Arial";
            style1_Color.Pattern = BackgroundType.Solid;
            style1_Color.Font.Color = Color.Red;
            style1_Color.Number = 3;
            style1_Color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;


            style1_Color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style1_Color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style1_Color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style1_Color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style1_Color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style1_Color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style1_Color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style1_Color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion



            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            //style2.ForegroundColor = System.Drawing.Color.FromArgb(253, 225, 160);

            #endregion

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";
            style2.Number = 3;
            //style2.Font.IsBold = true;
            //style2.ForegroundColor = System.Drawing.Color.FromArgb(253, 225, 160);
            style2.Pattern = BackgroundType.Solid;

            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            //style2.ForegroundColor = System.Drawing.Color.WhiteSmoke;
            //style2.Pattern = BackgroundType.Solid;

            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region style3 样式,无边框 ,居中

            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.Color = Color.Red;
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

            #region style4 样式,无边框 居左

            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
            style4.Font.Color = Color.Red;
            style4.Font.IsBold = true;
            style4.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style4.Pattern = BackgroundType.Solid;

            style4.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;



            #endregion


            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列
            int MergeCol = 0; //合并列

            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;
                worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style1);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }
                //2014年8月份百货系统未完成门店补回情况明细表
                string strName = FinYear.ToString() + "年" + FinMonth.ToString("D2") + "月份" + rpt._System.SystemName + List[i].Name + "明细表";
                worksheets[i].Cells[0, 1].PutValue(strName);

                worksheets[i].Name = List[i].Name.Replace('\\', '-');
            }



            for (int i = 0; i < List.Count; i++) //循环分组后的指标，代表有几个Sheet页面
            {
                rowStart = 4;
                List<DictionaryVmodel> itemList = (List<DictionaryVmodel>)List[i].ObjValue;

                int _MergeCol = 0;

                foreach (DictionaryVmodel item in itemList)
                {
                    int rowCount = 0;

                    // _MergeCol++;

                    StyleFlag flg = new StyleFlag();
                    flg.All = true;

                    #region 判断数据结构，递归循环

                    if (item.Mark == "Counter")
                    {
                        #region 插入合并项

                        Range range1 = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 13);
                        range1.Merge();
                        range1.ApplyStyle(style4, flg);

                        int tempRow = rowStart;//excel 加入一行数据
                        //worksheets[i].Cells[rowStart, 1].PutValue(item.Name); 

                        #endregion

                        List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue; //当前的objValue里的数据递归

                        //这里的变量请勿移动
                        rowStart = rowStart + 1;
                        int groupRowSart = rowStart;

                        //递归循环
                        rowCount = ParseGroup(List[i].Name, list, worksheets[i], ref rowStart, colStart, ref MergeCol, style1, style2, style4, style1_Color);

                        //放在这里容易计算
                        worksheets[i].Cells[tempRow, 1].PutValue(item.Name + "（共" + rowCount.ToString() + "家）公司"); //excel 加入一行数据 ,计算有多少家公司

                        //分组数据
                        if (rowStart - groupRowSart > 0)
                        {
                            int endRow = rowStart - 1;
                            worksheets[i].Cells.GroupRows(groupRowSart, endRow, true);
                        }

                    }
                    else if (item.Mark == "Data")
                    {
                        List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                        #region 插入合并项并且附加样式

                        Range range = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 13);
                        range.Merge();
                        range.ApplyStyle(style4, flg);

                        worksheets[i].Cells[rowStart, 1].PutValue(item.Name + "（共" + ReportDetailLisr.Count() / List[i].TargetGroupCount + "家）公司"); //excel 加入一行数据

                        #endregion

                        int rowStartR = rowStart + 1;

                        int index = 1;
                        ReportDetailLisr.ForEach(p =>
                        {
                            #region 设置表格中的所有样式

                            //设置Excel的样式
                            worksheets[i].Cells[rowStartR, 1].SetStyle(style2);


                            worksheets[i].Cells[rowStartR, colStart].SetStyle(style2);

                            if (p.SystemID == Guid.Parse(ConfigurationManager.AppSettings["MonthSG"]))
                            {
                                worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                                worksheets[i].Cells.Columns[colStart + 1].Width = 0;// 指标名称, 在商管系统的时候隐藏
                            }
                            else
                            {
                                worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                            }
                            worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);

                            #endregion

                            #region 加载数据

                            //加载数据
                            _MergeCol++;

                            worksheets[i].Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称

                            if (List[i].TargetGroupCount != 1) //分组指标
                            {
                                if (_MergeCol < List[i].TargetGroupCount)
                                {
                                    if (_MergeCol == 1)
                                    {
                                        //指标合并
                                        worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号

                                        Range itemRangeByID = worksheets[i].Cells.CreateRange(rowStartR, 1, List[i].TargetGroupCount, 1);
                                        itemRangeByID.Merge();
                                        itemRangeByID.ApplyStyle(style2, flg);

                                        Range itemRangeByCompanyName = worksheets[i].Cells.CreateRange(rowStartR, colStart, List[i].TargetGroupCount, 1);
                                        itemRangeByCompanyName.Merge();
                                        itemRangeByCompanyName.ApplyStyle(style2, flg);
                                    }
                                }
                                else
                                {
                                    _MergeCol = 0;
                                }
                            }
                            else //单个指标
                            {
                                int num = index++;
                                worksheets[i].Cells[rowStartR, 1].PutValue(num); //序号
                            }

                            worksheets[i].Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称


                            decimal NewDifference = p.NAccumulativeDifference - p.LastNAccumulativeDifference;
                            if (NewDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1_Color); // 红色
                            }
                            worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                            worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                            worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际
                            if (p.LastNAccumulativeDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style1_Color);
                            }
                            worksheets[i].Cells[rowStartR, colStart + 5].PutValue(p.LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值
                            worksheets[i].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);//本月计划
                            worksheets[i].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount); //本月实际

                            if (p.NAccumulativeDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1_Color);
                            }
                            worksheets[i].Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                            worksheets[i].Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate);//本月完成率

                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                worksheets[i].Cells[rowStartR, colStart + 10].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                            }
                            else
                            {
                                worksheets[0].Cells[rowStartR, colStart + 10].PutValue("——");
                            }

                            if (p.Counter > 0)
                            {
                                int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 11, ImageFilePath + "\\image" + p.Counter + ".png");
                                Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                                picture.Left = 40;
                                picture.Top = 10;

                            }

                            #endregion

                            rowStartR++;

                        });


                        #region 分组数据Excel

                        if (ReportDetailLisr.Count > 0)
                        {
                            worksheets[i].Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                        }

                        #endregion

                        rowStart = rowStartR;
                    }

                    #endregion


                }

            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 补回情况(直属公司)
        /// </summary>
        /// <param name="List"></param>
        /// <param name="templetePath"></param>
        /// <param name="templeteName"></param>
        /// <param name="fileName"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        public void ExcelDirectlyReturnTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Aspose.Cells.Style style1_Color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            #region style1_Color 样式,无边框
            style1_Color.Font.Size = 12;
            style1_Color.Font.Name = "Arial";
            style1_Color.Pattern = BackgroundType.Solid;
            style1_Color.Font.Color = Color.Red;
            style1_Color.Number = 3;
            style1_Color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;


            style1_Color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style1_Color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style1_Color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style1_Color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style1_Color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style1_Color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style1_Color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style1_Color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;

            #endregion



            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            //style2.ForegroundColor = System.Drawing.Color.FromArgb(253, 225, 160);

            #endregion

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";
            style2.Number = 3;
            //style2.Font.IsBold = true;
            //style2.ForegroundColor = System.Drawing.Color.FromArgb(253, 225, 160);
            style2.Pattern = BackgroundType.Solid;

            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            //style2.ForegroundColor = System.Drawing.Color.WhiteSmoke;
            //style2.Pattern = BackgroundType.Solid;

            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion

            #region style3 样式,无边框 ,居中

            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.Color = Color.Red;
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

            #region style4 样式,无边框 居左

            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
            style4.Font.Color = Color.Red;
            style4.Font.IsBold = true;
            style4.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style4.Pattern = BackgroundType.Solid;

            style4.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style4.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style4.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;



            #endregion


            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列
            int MergeCol = 0; //合并列

            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;
                worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style1);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }
                //2014年8月份百货系统未完成门店补回情况明细表
                string strName = FinYear.ToString() + "年" + FinMonth.ToString("D2") + "月份" + rpt._System.SystemName + List[i].Name + "明细表";
                worksheets[i].Cells[0, 1].PutValue(strName);
                worksheets[i].Name = List[i].Name;
            }



            for (int i = 0; i < List.Count; i++) //循环分组后的指标，代表有几个Sheet页面
            {
                rowStart = 4;
                List<DictionaryVmodel> itemList = (List<DictionaryVmodel>)List[i].ObjValue;

                foreach (DictionaryVmodel item in itemList)
                {
                    int rowCount = 0;

                    StyleFlag flg = new StyleFlag();
                    flg.All = true;

                    #region 判断数据结构，递归循环

                    if (item.Mark == "Counter")
                    {
                        #region 插入合并项

                        Range range1 = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 13);
                        range1.Merge();
                        range1.ApplyStyle(style4, flg);

                        int tempRow = rowStart;//excel 加入一行数据
                        //worksheets[i].Cells[rowStart, 1].PutValue(item.Name); 

                        #endregion

                        List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue; //当前的objValue里的数据递归

                        //这里的变量请勿移动
                        rowStart = rowStart + 1;
                        int groupRowSart = rowStart;

                        //递归循环
                        rowCount = ParseGroup(List[i].Name, list, worksheets[i], ref rowStart, colStart, ref MergeCol, style1, style2, style4, style1_Color);

                        //放在这里容易计算
                        worksheets[i].Cells[tempRow, 1].PutValue(item.Name + "（共" + rowCount.ToString() + "条）"); //excel 加入一行数据 ,计算有多少家公司

                        //分组数据
                        if (rowStart - groupRowSart > 0)
                        {
                            int endRow = rowStart - 1;
                            worksheets[i].Cells.GroupRows(groupRowSart, endRow, true);
                        }

                    }
                    else if (item.Mark == "Data")
                    {
                        List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                        #region 插入合并项并且附加样式

                        Range range = worksheets[i].Cells.CreateRange(rowStart, 1, 1, 13);
                        range.Merge();
                        range.ApplyStyle(style4, flg);

                        worksheets[i].Cells[rowStart, 1].PutValue(item.Name + "（共" + ReportDetailLisr.Count() + "条）"); //excel 加入一行数据

                        #endregion

                        int rowStartR = rowStart + 1;

                        int index = 1;
                        ReportDetailLisr.ForEach(p =>
                        {
                            #region 设置表格中的所有样式

                            //设置Excel的样式
                            worksheets[i].Cells[rowStartR, 1].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart].SetStyle(style2);
                            worksheets[i].Cells.Columns[colStart].Width = 0;
                            worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2);// 指标名称, 在商管系统的时候隐藏
                            worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2);
                            worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);

                            #endregion

                            #region 加载数据
                            //加载数据
                            worksheets[i].Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称
                            int num = index++;
                            worksheets[i].Cells[rowStartR, 1].PutValue(num); //序号
                            worksheets[i].Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称
                            decimal NewDifference = p.NAccumulativeDifference - p.LastNAccumulativeDifference;
                            if (NewDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1_Color); // 红色
                            }
                            worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                            worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                            worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际
                            if (p.LastNAccumulativeDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style1_Color);
                            }
                            worksheets[i].Cells[rowStartR, colStart + 5].PutValue(p.LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值
                            worksheets[i].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);//本月计划
                            worksheets[i].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount); //本月实际

                            if (p.NAccumulativeDifference < 0)
                            {
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1_Color);
                            }
                            worksheets[i].Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值
                            worksheets[i].Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate);//本月完成率
                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                worksheets[i].Cells[rowStartR, colStart + 10].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                            }
                            else
                            {
                                worksheets[0].Cells[rowStartR, colStart + 10].PutValue("——");
                            }
                            if (p.Counter > 0)
                            {
                                int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 11, ImageFilePath + "\\image" + p.Counter + ".png");
                                Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                                picture.Left = 40;
                                picture.Top = 10;
                            }

                            #endregion
                            rowStartR++;
                        });
                        #region 分组数据Excel
                        if (ReportDetailLisr.Count > 0)
                        {
                            worksheets[i].Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                        }
                        #endregion
                        rowStart = rowStartR;
                    }
                    #endregion


                }

            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }



        /// <summary>
        /// 循环添加到Excel
        /// </summary>
        /// <param name="TargetName">指标List名称</param>
        /// <param name="ReturnList">指标下的相关数据</param>
        /// <param name="worksheet">Excel的worksheet对象</param>
        /// <param name="rowStart">开始行</param>
        /// <param name="colStart">开始列</param>
        /// <param name="MergeCol">指标执行行数</param>
        /// <param name="style1">样式1</param>
        /// <param name="style2">样式2</param>
        /// <param name="style3">样式3</param>
        public int ParseGroup(string TargetName, List<DictionaryVmodel> ReturnList, Worksheet worksheet, ref int rowStart, int colStart, ref int MergeCol, Style style1, Style style2, Style style3, Style style1_Color)
        {
            int RowCount = 0;
            int tmepRow = 0;
            //返回的数据
            foreach (var item in ReturnList)
            {

                MergeCol++;

                StyleFlag flg = new StyleFlag();
                flg.All = true;

                if (item.Mark == "Counter")
                {
                    #region 插入合并项 ，添加标题

                    Range range1 = worksheet.Cells.CreateRange(rowStart, 1, 1, 13);
                    range1.Merge();
                    range1.ApplyStyle(style3, flg);

                    int TempRowStart = rowStart;  //添加一行

                    #endregion

                    List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue;
                    rowStart = rowStart + 1;
                    RowCount = ParseGroup(TargetName, list, worksheet, ref rowStart, colStart, ref MergeCol, style1, style2, style3, style1_Color);

                    worksheet.Cells[TempRowStart, 1].PutValue(item.Name + "（共" + RowCount.ToString() + "家）公司");

                }
                else if (item.Mark == "Data")
                {
                    List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;


                    #region 插入合并项并且附加样式 ，添加标题

                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 13);
                    range.Merge();
                    range.ApplyStyle(style3, flg);

                    tmepRow = ReportDetailLisr.Count() / ReturnList[0].TargetGroupCount;

                    worksheet.Cells[rowStart, 1].PutValue(item.Name + "（共" + ReportDetailLisr.Count() / ReturnList[0].TargetGroupCount + "家）公司"); //excel 加入一行数据
                    #endregion


                    int rowStartR = rowStart + 1;

                    int index = 1;
                    int _MergeCol = 0;//指标合并标识

                    ReportDetailLisr.ForEach(p =>
                    {
                        #region 设置表格中的所有样式

                        //设置Excel的样式


                        worksheet.Cells[rowStartR, 1].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 1].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 2].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 3].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 4].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 5].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 6].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 7].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 8].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 9].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 10].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 11].SetStyle(style2);

                        #endregion

                        #region 加载数据


                        _MergeCol++;


                        worksheet.Cells[rowStartR, colStart].PutValue(p.CompanyName);

                        if (ReturnList[0].TargetGroupCount != 1) //分组指标
                        {
                            if (_MergeCol < ReturnList[0].TargetGroupCount)
                            {
                                if (_MergeCol == 1)
                                {
                                    //指标合并
                                    worksheet.Cells[rowStartR, 1].PutValue(index++);

                                    Range itemRangeByID = worksheet.Cells.CreateRange(rowStartR, 1, ReturnList[0].TargetGroupCount, 1);
                                    itemRangeByID.Merge();
                                    itemRangeByID.ApplyStyle(style2, flg);

                                    Range itemRangeByCompanyName = worksheet.Cells.CreateRange(rowStartR, colStart, ReturnList[0].TargetGroupCount, 1);
                                    itemRangeByCompanyName.Merge();
                                    itemRangeByCompanyName.ApplyStyle(style2, flg);
                                }
                            }
                            else
                            {
                                _MergeCol = 0;
                            }
                        }
                        else //单个指标
                        {
                            worksheet.Cells[rowStartR, 1].PutValue(index++);
                        }

                        worksheet.Cells[rowStartR, colStart + 1].PutValue(p.TargetName);

                        decimal NewDifference = p.NAccumulativeDifference - p.LastNAccumulativeDifference;
                        if (NewDifference < 0)
                        {
                            worksheet.Cells[rowStartR, colStart + 2].SetStyle(style1_Color); // 红色
                        }

                        worksheet.Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                        worksheet.Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                        worksheet.Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际
                        if (p.LastNAccumulativeDifference < 0)
                        {
                            worksheet.Cells[rowStartR, colStart + 5].SetStyle(style1_Color); // 红色
                        }
                        worksheet.Cells[rowStartR, colStart + 5].PutValue(p.LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值
                        worksheet.Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);
                        worksheet.Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);

                        if (p.NAccumulativeDifference < 0)
                        {
                            worksheet.Cells[rowStartR, colStart + 8].SetStyle(style1_Color); // 红色
                        }

                        worksheet.Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                        worksheet.Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate); //本月完成率

                        if (p.CommitDate != null)
                        {
                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                            worksheet.Cells[rowStartR, colStart + 10].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                        }
                        else
                        {
                            worksheet.Cells[rowStartR, colStart + 10].PutValue("——");
                        }
                        if (p.Counter > 0)
                        {
                            int pictureIndex = worksheet.Pictures.Add(rowStartR, colStart + 11, ImageFilePath + "\\image" + p.Counter + ".png");
                            Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                            picture.Left = 40;
                            picture.Top = 10;
                        }

                        #endregion

                        rowStartR++;
                    });

                    #region 分组数据Excel

                    if (ReportDetailLisr.Count > 0)
                    {
                        worksheet.Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                    }

                    #endregion


                    rowStart = rowStartR;
                }
                RowCount = RowCount + tmepRow;
            }

            return RowCount;
        }

        private string ReplaceRedColor(string Title)
        {
            string str = Title.Replace(@"<span style='color: red;'>", "").Replace(@"</span>", "");
            return str;
        }

        /// <summary>
        /// 未完成说明，递归
        /// </summary>
        /// <param name="TargetName"></param>
        /// <param name="MissTargetList"></param>
        /// <param name="worksheet"></param>
        /// <param name="rowStart"></param>
        /// <param name="colStart"></param>
        /// <param name="style1"></param>
        /// <param name="style2"></param>
        /// <param name="style3"></param>
        /// <param name="IsRpt"></param>
        public void MissTargetParseGroup(string TargetName, List<DictionaryVmodel> MissTargetList, Worksheet worksheet, ref int rowStart, int colStart, Style style1, Style style2, Style style3, bool IsRpt)
        {
            //返回的数据
            foreach (var item in MissTargetList)
            {
                StyleFlag flg = new StyleFlag();
                flg.All = true;

                if (item.Mark == "Counter")
                {
                    #region 插入合并项 ，添加标题


                    Range range1 = worksheet.Cells.CreateRange(rowStart, 1, 1, 14);
                    range1.Merge();
                    range1.ApplyStyle(style3, flg);
                    worksheet.Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //添加一行

                    #endregion

                    List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue;

                    rowStart = rowStart + 1;

                    MissTargetParseGroup(TargetName, list, worksheet, ref rowStart, colStart, style1, style2, style3, IsRpt);

                }
                else if (item.Mark == "Data")
                {
                    List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;


                    #region 插入合并项并且附加样式 ，添加标题

                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 14);
                    range.Merge();
                    range.ApplyStyle(style3, flg);
                    worksheet.Cells[rowStart, 1].PutValue(ReplaceRedColor(item.Name)); //excel 加入一行数据

                    #endregion

                    int rowStartR = rowStart + 1;


                    int index = 1;
                    int _MergeCol = 0;//指标合并标识

                    ReportDetailLisr.ForEach(p =>
                    {
                        #region 设置表格中的所有样式

                        //设置Excel的样式
                        worksheet.Cells[rowStartR, 1].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 1].SetStyle(style2);

                        worksheet.Cells[rowStartR, colStart + 2].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 3].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 4].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 5].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 6].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 7].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 8].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 9].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 10].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 11].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 12].SetStyle(style2);

                        #endregion

                        #region 加载数据

                        //加载数据
                        _MergeCol++;

                        worksheet.Cells[rowStartR, colStart].PutValue(p.CompanyName); //公司名称

                        if (MissTargetList[0].TargetGroupCount != 1) //分组指标
                        {
                            if (_MergeCol < MissTargetList[0].TargetGroupCount)
                            {
                                if (_MergeCol == 1)
                                {
                                    //指标合并
                                    worksheet.Cells[rowStartR, 1].PutValue(index++); //序号

                                    Range itemRangeByID = worksheet.Cells.CreateRange(rowStartR, 1, MissTargetList[0].TargetGroupCount, 1);
                                    itemRangeByID.Merge();
                                    itemRangeByID.ApplyStyle(style2, flg);

                                    Range itemRangeByCompanyName = worksheet.Cells.CreateRange(rowStartR, colStart, MissTargetList[0].TargetGroupCount, 1);
                                    itemRangeByCompanyName.Merge();
                                    itemRangeByCompanyName.ApplyStyle(style2, flg);

                                    Range itemRangeByMIssTargetReason = worksheet.Cells.CreateRange(rowStartR, colStart + 10, MissTargetList[0].TargetGroupCount, 1);
                                    itemRangeByMIssTargetReason.Merge();
                                    itemRangeByMIssTargetReason.ApplyStyle(style2, flg);
                                }
                            }
                            else
                            {
                                _MergeCol = 0;
                            }
                        }
                        else //单个指标
                        {
                            worksheet.Cells[rowStartR, 1].PutValue(index++); //序号
                        }

                        worksheet.Cells[rowStartR, colStart + 1].PutValue(p.TargetName); //指标名称


                        worksheet.Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                        worksheet.Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                        worksheet.Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际
                        worksheet.Cells[rowStartR, colStart + 5].PutValue(p.LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值

                        worksheet.Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);
                        worksheet.Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);
                        worksheet.Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativeDifference); //本月差值

                        worksheet.Cells[rowStartR, colStart + 9].PutValue(p.NAccumulativeDisplayRate); //本月完成率

                        if (IsRpt) //是上报模版
                        {
                            if (p.ReturnType == (int)EnumReturnType.New) //本月新增
                            {
                                worksheet.Cells[rowStartR, colStart + 10].PutValue("");
                                worksheet.Cells[rowStartR, colStart + 11].PutValue("");
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 10].PutValue("未完成原因:" + p.MIssTargetReason + "\n\r采取措施:" + p.MIssTargetDescription); //未完成原因,及采取措施

                                if (p.CommitDate != null)
                                {
                                    string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                                }
                                else
                                {
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue("——");
                                }

                            }
                        }
                        else
                        {
                            worksheet.Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.MIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.MIssTargetDescription)); //未完成原因,及采取措施

                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 11].PutValue("——");
                            }

                        }

                        if (p.ReturnType != 0)
                        {
                            string PromissDate = string.Empty;

                            if (p.PromissDate != null)
                            {
                                PromissDate = string.Format("{0:M月}", p.PromissDate);

                                PromissDate = "承诺" + PromissDate + "月份补回";
                            }

                            if (p.CommitDate != null && p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                            {
                                worksheet.Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                            }

                            //worksheet.Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                        }
                        if (p.Counter > 0)
                        {
                            int pictureIndex = worksheet.Pictures.Add(rowStartR, colStart + 13, ImageFilePath + "\\image" + p.Counter + ".png");
                            Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                            picture.Left = 40;
                            picture.Top = 10;

                        }
                        //worksheet.Cells[rowStartR, colStart + 13].PutValue(p.Counter);

                        #endregion

                        rowStartR++;
                    });

                    #region 分组数据Excel

                    if (ReportDetailLisr.Count > 0)
                    {
                        worksheet.Cells.GroupRows(rowStart + 1, rowStartR - 1, true);
                    }

                    #endregion

                    rowStart = rowStartR;
                }

            }
        }




        /// <summary>
        /// 下载Excel完成情况明细--经营系统
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public void DownExcelMonthReportDetail_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            if (rpt._System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
            {
                string strXml = rpt.GetComplateMonthReportDetailHtmlTemplate(rpt._System.Configuration);
                string[] strXmls = strXml.Split(',');
                if (strXmls.Length > 1)
                {
                    templeteName = !string.IsNullOrEmpty(strXmls[3]) ? strXmls[3] : "完成情况明细模板V1.xlsx";
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

            #region style4 样式
            style4.HorizontalAlignment = TextAlignmentType.Center;
            style4.VerticalAlignment = TextAlignmentType.Center;

            #endregion


            bool IncludeHaveDetail = false;
            List<DictionaryVmodel> listMonthReportDetail = null;
            if (rpt != null)
            {
                listMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "", OrderStr, IncludeHaveDetail);
            }

            int rowStart = 4;  //开始行
            int colStart = 2; // 开始列

            //系统指标类，需要读取XML
            List<C_Target> _targetList = StaticResource.Instance.TargetList[sytemID].ToList();


            for (int i = 0; i < listMonthReportDetail.Count; i++)
            {
                if (templeteName == "完成情况明细模板V1.xlsx")
                {
                    if (i > 0)
                    {
                        worksheets.AddCopy(0);
                    }

                    worksheets[i].Name = listMonthReportDetail[i].Name;
                }
                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                worksheets[i].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[i].Cells[1, 2].SetStyle(style1);
            }

            bool IsDifferenceException = false; //商管的差额特殊处理

            int DataDisplayMode = 0; //针对旅业下载客流量保留2位小数

            //创建指标Sheet,
            for (int sheetIndex = 0; sheetIndex < listMonthReportDetail.Count; sheetIndex++)
            {
                Worksheet worksheet = worksheets[sheetIndex];

                string _targetName = string.Empty;

                if (templeteName != "完成情况明细模板V1.xlsx")
                {
                    if (worksheets[listMonthReportDetail[sheetIndex].Name] != null)
                    {
                        worksheet = worksheets[listMonthReportDetail[sheetIndex].Name];
                    }
                }

                _targetName = listMonthReportDetail[sheetIndex].Name;

                C_Target _target = _targetList.Where(p => p.TargetName == _targetName).ToList()[0];

                //特殊处理差额，针对指标
                XElement element = null;
                element = _target.Configuration;
                XElement subElement = null; //商管的节点

                XElement displayModeElement = null; //万达旅业的客流量下载，变成2位小数

                if (element.Elements("IsDifferenceExceptionTarget").ToList().Count > 0)
                {
                    subElement = element.Elements("IsDifferenceExceptionTarget").ToList()[0];
                    IsDifferenceException = subElement.GetAttributeValue("value", false);
                }
                else
                {
                    IsDifferenceException = false;
                }

                if (element.Elements("DataDisplayMode").ToList().Count > 0)
                {
                    displayModeElement = element.Elements("DataDisplayMode").ToList()[0];
                    DataDisplayMode = displayModeElement.GetAttributeValue("value", 0);
                }
                else
                {
                    DataDisplayMode = 0;
                }


                rowStart = 4;
                StyleFlag flag = new StyleFlag();
                flag.All = true;
                List<DictionaryVmodel> listCompanyProperty = (List<DictionaryVmodel>)listMonthReportDetail[sheetIndex].ObjValue;
                for (int j = 0; j < listCompanyProperty.Count; j++)
                {
                    if (listCompanyProperty[j].Name == "SummaryData")
                    {
                        List<B_MonthlyReportDetail> ListItem = ((List<B_MonthlyReportDetail>)listCompanyProperty[j].ObjValue);
                        for (int k = 0; k < ListItem.Count; k++)
                        {
                            #region 设置样式
                            Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);
                            range.Merge();
                            range.ApplyStyle(style3, flag);
                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 10].SetStyle(style3);
                            #endregion

                            #region 为单元格赋值
                            worksheet.Cells[rowStart, colStart - 1].PutValue("合计");
                            worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmount);
                            worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[k].NActualAmmount);
                            worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NDifference);
                            worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[k].NDisplayRate);
                            worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NAccumulativePlanAmmount);
                            worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[k].NAccumulativeActualAmmount);
                            worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NAccumulativeDifference);
                            worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[k].NAccumulativeDisplayRate);
                            worksheet.Cells[rowStart, colStart + 10].PutValue("");

                            //特殊差额指标，这里显示绝对值--商管系统
                            if (IsDifferenceException)
                            {
                                worksheet.Cells[rowStart, colStart + 4].PutValue(Math.Abs(ListItem[k].NDifference));
                                worksheet.Cells[rowStart, colStart + 8].PutValue(Math.Abs(ListItem[k].NAccumulativeDifference));
                            }

                            #endregion

                            #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                            style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                            if (DataDisplayMode == 0)
                                style3.Number = 3;
                            else
                                style3.Number = 4;

                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                            #endregion
                            rowStart = rowStart + 1;
                        }
                    }
                    else if (listCompanyProperty[j].Name == "HaveDetail")
                    {
                        List<MonthlyReportDetail> listMRDVM = (List<MonthlyReportDetail>)listCompanyProperty[j].ObjValue;
                        for (int k = 0; k < listMRDVM.Count; k++)
                        {
                            Range range = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                            range.Merge();
                            range.ApplyStyle(style2, flag);
                            #region 设置样式
                            worksheet.Cells[rowStart, colStart - 1].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 10].SetStyle(style2);
                            #endregion

                            #region 为单元格赋值
                            worksheet.Cells[rowStart, colStart - 1].PutValue((k + 1).ToString());
                            worksheet.Cells[rowStart, colStart].PutValue(listMRDVM[k].CompanyName.ToString());
                            worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmount);
                            worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM[k].NActualAmmount);
                            worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NDifference);
                            worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM[k].NDisplayRate);
                            worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                            worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
                            worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NAccumulativeDifference);
                            worksheet.Cells[rowStart, colStart + 9].PutValue(listMRDVM[k].NAccumulativeDisplayRate);

                            //特殊差额指标，这里显示绝对值--商管系统
                            if (IsDifferenceException)
                            {
                                worksheet.Cells[rowStart, colStart + 4].PutValue(Math.Abs(listMRDVM[k].NDifference));
                                worksheet.Cells[rowStart, colStart + 8].PutValue(Math.Abs(listMRDVM[k].NAccumulativeDifference));
                            }

                            #endregion

                            #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                            style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                            if (DataDisplayMode == 0)
                                style2.Number = 3;
                            else
                                style2.Number = 4;

                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                            #endregion
                            rowStart = rowStart + 1;
                        }
                    }
                    else
                    {
                        List<DictionaryVmodel> ListItem = ((List<DictionaryVmodel>)listCompanyProperty[j].ObjValue);
                        int count = 0;
                        for (int zz = 0; zz < ListItem.Count; zz++)
                        {
                            if (ListItem[zz].ObjValue != null)
                            {
                                count = count + ((List<MonthlyReportDetail>)ListItem[zz].ObjValue).Count();
                            }
                        }
                        int tmpcolStart = 2;
                        int tempTotalColumns = 2;
                        if (!string.IsNullOrEmpty(listCompanyProperty[j].Name) && listCompanyProperty[j].Name != "SummaryData")
                        {
                            Range range = worksheet.Cells.CreateRange(rowStart, 1, ListItem.Count + count, 1);
                            range.Merge();
                            range.ApplyStyle(style2, flag);


                            worksheet.Cells[rowStart, 1].PutValue(listCompanyProperty[j].Name);
                        }
                        else
                        {
                            tmpcolStart = colStart - 1;
                            tempTotalColumns = tempTotalColumns + 1;
                        }
                        for (int z = 0; z < ListItem.Count; z++)
                        {
                            Range itemRange = worksheet.Cells.CreateRange(rowStart, tmpcolStart, 1, tempTotalColumns);
                            itemRange.Merge();
                            itemRange.ApplyStyle(style3, flag);
                            worksheet.Cells[rowStart, tmpcolStart].PutValue(ListItem[z].Name);
                            #region 设置样式
                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 10].SetStyle(style3);
                            #endregion

                            #region 为单元格赋值
                            worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmount);
                            worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[z].BMonthReportDetail.NActualAmmount);
                            worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[z].BMonthReportDetail.NDifference);
                            worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[z].BMonthReportDetail.NDisplayRate);
                            worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[z].BMonthReportDetail.NAccumulativePlanAmmount);
                            worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeActualAmmount);
                            worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeDifference);
                            worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeDisplayRate);
                            worksheet.Cells[rowStart, colStart + 10].PutValue("");

                            //特殊差额指标，这里显示绝对值--商管系统
                            if (IsDifferenceException)
                            {
                                worksheet.Cells[rowStart, colStart + 4].PutValue(Math.Abs(ListItem[z].BMonthReportDetail.NDifference));
                                worksheet.Cells[rowStart, colStart + 8].PutValue(Math.Abs(ListItem[z].BMonthReportDetail.NAccumulativeDifference));
                            }

                            #endregion

                            #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                            style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                            if (DataDisplayMode == 0)
                                style3.Number = 3;
                            else
                                style3.Number = 4;

                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                            #endregion

                            rowStart = rowStart + 1;
                            int tempRowStart = rowStart;
                            if (ListItem[z].ObjValue == null)
                            { continue; }
                            List<MonthlyReportDetail> listMRDVM = (List<MonthlyReportDetail>)ListItem[z].ObjValue;
                            for (int k = 0; k < listMRDVM.Count; k++)
                            {
                                if (tmpcolStart != colStart)
                                {
                                    itemRange = worksheet.Cells.CreateRange(rowStart, tmpcolStart + 1, 1, 2);
                                    itemRange.Merge();
                                    itemRange.ApplyStyle(style3, flag);
                                }
                                #region 设置样式
                                worksheet.Cells[rowStart, tmpcolStart].SetStyle(style2);
                                worksheet.Cells[rowStart, tmpcolStart + 1].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 10].SetStyle(style2);
                                #endregion

                                #region 为单元格赋值
                                worksheet.Cells[rowStart, tmpcolStart].PutValue((k + 1).ToString());
                                worksheet.Cells[rowStart, tmpcolStart + 1].PutValue(listMRDVM[k].CompanyName.ToString());
                                worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NDifference);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NAccumulativeDifference);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(listMRDVM[k].NAccumulativeDisplayRate);

                                //特殊差额指标，这里显示绝对值--商管系统
                                if (IsDifferenceException)
                                {
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(Math.Abs(listMRDVM[k].NDifference));
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(Math.Abs(listMRDVM[k].NAccumulativeDifference));
                                }

                                #endregion

                                #region 设置千分位
                                if (listMRDVM[k].Counter > 0)
                                {
                                    int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 10, ImageFilePath + "\\image" + listMRDVM[k].Counter + ".png");
                                    Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                                    picture.Left = 60;
                                    picture.Top = 10;
                                }
                                style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                if (DataDisplayMode == 0)
                                    style2.Number = 3;
                                else
                                    style2.Number = 4;

                                worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                            //为当前sheet分组
                            if (listMRDVM.Count > 0 && z == 0)
                            {
                                worksheet.Cells.GroupRows(tempRowStart, tempRowStart - 1 + listMRDVM.Count, true);
                            }
                        }
                    }
                }

            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
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

        /// <summary>
        /// 下载Excel完成情况明细--项目公司系统
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="templeteName"></param>
        /// <param name="fileName"></param>
        /// <param name="sytemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        public void DownExcelMonthReportDetail_XM(ReportInstance rpt, string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];


            #region style1 样式
            style1.Font.Size = 14;
            style1.Font.IsBold = true;
            #endregion

            #region style2 样式 无加粗
            style2.Font.Size = 12;
            style2.Font.Name = "Arial";
            style2.Number = 1;
            //style2.Font.Color = Color.Red;
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

            #region style3 样式背景色洋红
            style3.Font.Size = 12;
            style3.Font.Name = "Arial";
            style3.Font.IsBold = true;
            style3.ForegroundColor = System.Drawing.Color.FromArgb(250, 191, 143);
            style3.Pattern = BackgroundType.Solid;
            style3.Number = 1;

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

            int rowStart = 5;  //开始行
            int colStart = 2; // 开始列


            worksheets[0].Cells[1, 1].PutValue("报告期：" + FinYear.ToString() + "年" + FinMonth + "月");
            worksheets[0].Cells[1, 1].SetStyle(style1);

            //if (i > 0)
            //{
            //    worksheets.AddCopy(0);
            //}

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
            //worksheets[0].Cells[1, 15].PutValue("金额单位：万元"); //
            worksheets[0].Cells[2, 9].PutValue("1-" + FinMonth.ToString() + "月情况");
            worksheets[0].Cells[3, 9].PutValue("1-" + FinMonth.ToString() + "月指标（金额）");
            worksheets[0].Cells[3, 12].PutValue("1-" + FinMonth.ToString() + "月完成（金额）");
            worksheets[0].Cells[3, 15].PutValue("1-" + FinMonth.ToString() + "月指标完成比例");
            worksheets[0].Cells[2, 18].PutValue(FinMonth.ToString() + "月份情况");
            worksheets[0].Cells[3, 18].PutValue(FinMonth.ToString() + "月份指标");
            worksheets[0].Cells[3, 21].PutValue(FinMonth.ToString() + "月份完成（金额）");
            worksheets[0].Cells[3, 24].PutValue(FinMonth.ToString() + "月份指标完成比例");

            #endregion

            List<DictionaryVmodel> ProCompanyList = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "", "", IsLatestVersion); //这里的参数需要注意
            List<V_ProjectCompany> dicList = (List<V_ProjectCompany>)ProCompanyList[0].ObjValue;

            bool IsCulture = false;


            int GroupRow = 0;
            int index = 1;

            for (int i = 0; i < dicList.Count; i++) //引擎拼装好的数据，直接用
            {
                index++;

                StyleFlag flg = new StyleFlag();
                flg.All = true;

                int temprowStart = 0;
                temprowStart = rowStart + i;

                if (dicList[i].ProDataType == "XML")
                {
                    if (rpt._System.GroupType != "ProSystem") // 这里单独对文旅做处理
                    {
                        worksheets[0].Cells[temprowStart, colStart - 1].PutValue(dicList[i].ProCompayName);//公司名称
                        IsCulture = true;
                    }
                    else
                    {
                        worksheets[0].Cells[temprowStart, colStart - 1].PutValue(dicList[i].ProCompayName);//公司名称
                    }

                    worksheets[0].Cells[temprowStart, colStart + 1].PutValue(dicList[i].ProjectTargets[0].NPlanAmmountByYear);//年度指标 合同
                    worksheets[0].Cells[temprowStart, colStart + 2].PutValue(dicList[i].ProjectTargets[1].NPlanAmmountByYear);//年度指标 回款
                    worksheets[0].Cells[temprowStart, colStart + 3].PutValue(dicList[i].ProjectTargets[2].NPlanAmmountByYear);//年度指标 入伙
                    worksheets[0].Cells[temprowStart, colStart + 4].PutValue(dicList[i].ProjectTargets[0].NDisplayRateByYear); //年度指标 完成比例 合同
                    worksheets[0].Cells[temprowStart, colStart + 5].PutValue(dicList[i].ProjectTargets[1].NDisplayRateByYear);//年度指标 完成比例 回款
                    worksheets[0].Cells[temprowStart, colStart + 6].PutValue(dicList[i].ProjectTargets[2].NDisplayRateByYear);//年度指标 完成比例 入伙
                    worksheets[0].Cells[temprowStart, colStart + 7].PutValue(dicList[i].ProjectTargets[0].NAccumulativePlanAmmount); //1-8月指标 合同
                    worksheets[0].Cells[temprowStart, colStart + 8].PutValue(dicList[i].ProjectTargets[1].NAccumulativePlanAmmount);//1-8月指标 回款
                    worksheets[0].Cells[temprowStart, colStart + 9].PutValue(dicList[i].ProjectTargets[2].NAccumulativePlanAmmount);//1-8月指标 入伙
                    worksheets[0].Cells[temprowStart, colStart + 10].PutValue(dicList[i].ProjectTargets[0].NAccumulativeActualAmmount); //1-8月实际数  合同
                    worksheets[0].Cells[temprowStart, colStart + 11].PutValue(dicList[i].ProjectTargets[1].NAccumulativeActualAmmount); //1-8月实际数 回款
                    worksheets[0].Cells[temprowStart, colStart + 12].PutValue(dicList[i].ProjectTargets[2].NAccumulativeActualAmmount); //1-8月实际数 入伙
                    worksheets[0].Cells[temprowStart, colStart + 13].PutValue(dicList[i].ProjectTargets[0].NAccumulativeDisplayRate); //1-8月完成比率  合同
                    worksheets[0].Cells[temprowStart, colStart + 14].PutValue(dicList[i].ProjectTargets[1].NAccumulativeDisplayRate); //1-8月完成比率 回款
                    worksheets[0].Cells[temprowStart, colStart + 15].PutValue(dicList[i].ProjectTargets[2].NAccumulativeDisplayRate); //1-8月完成比率 入伙
                    worksheets[0].Cells[temprowStart, colStart + 16].PutValue(dicList[i].ProjectTargets[0].NPlanAmmount); //8月完指标  合同
                    worksheets[0].Cells[temprowStart, colStart + 17].PutValue(dicList[i].ProjectTargets[1].NPlanAmmount); //8月完指标 回款
                    worksheets[0].Cells[temprowStart, colStart + 18].PutValue(dicList[i].ProjectTargets[2].NPlanAmmount); //8月完指标 入伙
                    worksheets[0].Cells[temprowStart, colStart + 19].PutValue(dicList[i].ProjectTargets[0].NActualAmmount);  //8月实际数  合同
                    worksheets[0].Cells[temprowStart, colStart + 20].PutValue(dicList[i].ProjectTargets[1].NActualAmmount);  //8月实际数 回款
                    worksheets[0].Cells[temprowStart, colStart + 21].PutValue(dicList[i].ProjectTargets[2].NActualAmmount);  //8月实际数 入伙
                    worksheets[0].Cells[temprowStart, colStart + 22].PutValue(dicList[i].ProjectTargets[0].NDisplayRate);  //8月完成比率  合同
                    worksheets[0].Cells[temprowStart, colStart + 23].PutValue(dicList[i].ProjectTargets[1].NDisplayRate);  //8月完成比率 回款
                    worksheets[0].Cells[temprowStart, colStart + 24].PutValue(dicList[i].ProjectTargets[2].NDisplayRate);  //8月完成比率 入伙
                    //worksheets[0].Cells[temprowStart, colStart + 25].PutValue(dicList[i].ProjectTargets[0].Counter); // 警示灯  合同
                    //worksheets[0].Cells[temprowStart, colStart + 26].PutValue(dicList[i].ProjectTargets[1].Counter); // 警示灯  回款
                }
                else
                {
                    worksheets[0].Cells.SetRowHeight(temprowStart, 25);
                    worksheets[0].Cells[temprowStart, colStart - 1].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 1].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 2].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 3].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 4].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 5].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 6].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 7].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 8].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 9].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 10].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 11].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 12].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 13].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 14].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 15].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 16].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 17].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 18].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 19].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 20].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 21].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 22].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 23].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 24].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 25].SetStyle(style2);
                    worksheets[0].Cells[temprowStart, colStart + 26].SetStyle(style2);

                    worksheets[0].Cells[temprowStart, colStart - 1].PutValue(dicList[i].ProCompanyNumber);//序号

                    if (IsCulture) //这里是对文旅公司，做的处理，列行合并取消
                    {
                        if (temprowStart > 5 && temprowStart < 10)
                        {
                            Range itemRangeCompany = worksheets[0].Cells.CreateRange(temprowStart, 1, 1, 2);
                            itemRangeCompany.UnMerge();
                            itemRangeCompany.ApplyStyle(style2, flg);
                        }
                    }

                    if (dicList[i].ProRowSpan > 1)
                    {
                        Range itemRangeByID = worksheets[0].Cells.CreateRange(temprowStart, 1, dicList[i].ProRowSpan, 1);
                        itemRangeByID.Merge();
                        itemRangeByID.ApplyStyle(style2, flg);
                    }

                    worksheets[0].Cells[temprowStart, colStart].PutValue(dicList[i].ProCompayName);//公司名称
                    worksheets[0].Cells[temprowStart, colStart + 1].PutValue(dicList[i].ProjectTargets[0].NPlanAmmountByYear);//年度指标 合同
                    worksheets[0].Cells[temprowStart, colStart + 2].PutValue(dicList[i].ProjectTargets[1].NPlanAmmountByYear);//年度指标 回款
                    worksheets[0].Cells[temprowStart, colStart + 3].PutValue(dicList[i].ProjectTargets[2].NPlanAmmountByYear);//年度指标 入伙
                    worksheets[0].Cells[temprowStart, colStart + 4].PutValue(dicList[i].ProjectTargets[0].NDisplayRateByYear); //年度指标 完成比例 合同
                    worksheets[0].Cells[temprowStart, colStart + 5].PutValue(dicList[i].ProjectTargets[1].NDisplayRateByYear);//年度指标 完成比例 回款
                    worksheets[0].Cells[temprowStart, colStart + 6].PutValue(dicList[i].ProjectTargets[2].NDisplayRateByYear);//年度指标 完成比例 入伙
                    worksheets[0].Cells[temprowStart, colStart + 7].PutValue(dicList[i].ProjectTargets[0].NAccumulativePlanAmmount); //1-8月指标 合同
                    worksheets[0].Cells[temprowStart, colStart + 8].PutValue(dicList[i].ProjectTargets[1].NAccumulativePlanAmmount);//1-8月指标 回款
                    worksheets[0].Cells[temprowStart, colStart + 9].PutValue(dicList[i].ProjectTargets[2].NAccumulativePlanAmmount);//1-8月指标 入伙
                    worksheets[0].Cells[temprowStart, colStart + 10].PutValue(dicList[i].ProjectTargets[0].NAccumulativeActualAmmount); //1-8月实际数  合同
                    worksheets[0].Cells[temprowStart, colStart + 11].PutValue(dicList[i].ProjectTargets[1].NAccumulativeActualAmmount); //1-8月实际数 回款
                    worksheets[0].Cells[temprowStart, colStart + 12].PutValue(dicList[i].ProjectTargets[2].NAccumulativeActualAmmount); //1-8月实际数 入伙

                    worksheets[0].Cells[temprowStart, colStart + 13].PutValue(dicList[i].ProjectTargets[0].NAccumulativeDisplayRate); //1-8月完成比率  合同
                    ActualRate TempActualRate = JsonHelper.Deserialize<ActualRate>(dicList[i].ProjectTargets[0].NAccumulativeActualRate);
                    if (TempActualRate.Rate < 1 && dicList[i].ProjectTargets[0].NAccumulativeDisplayRate != "/")
                        worksheets[0].Cells[temprowStart, colStart + 13].SetStyle(style3);//修改样式

                    worksheets[0].Cells[temprowStart, colStart + 14].PutValue(dicList[i].ProjectTargets[1].NAccumulativeDisplayRate); //1-8月完成比率 回款
                    TempActualRate = JsonHelper.Deserialize<ActualRate>(dicList[i].ProjectTargets[1].NAccumulativeActualRate);
                    if (TempActualRate.Rate < 1 && dicList[i].ProjectTargets[1].NAccumulativeDisplayRate != "/")
                        worksheets[0].Cells[temprowStart, colStart + 14].SetStyle(style3);//修改样式

                    worksheets[0].Cells[temprowStart, colStart + 15].PutValue(dicList[i].ProjectTargets[2].NAccumulativeDisplayRate); //1-8月完成比率 入伙
                    TempActualRate = JsonHelper.Deserialize<ActualRate>(dicList[i].ProjectTargets[2].NAccumulativeActualRate);
                    if (TempActualRate.Rate < 1 && dicList[i].ProjectTargets[2].NAccumulativeDisplayRate != "/")
                        worksheets[0].Cells[temprowStart, colStart + 15].SetStyle(style3);//修改样式

                    worksheets[0].Cells[temprowStart, colStart + 16].PutValue(dicList[i].ProjectTargets[0].NPlanAmmount); //8月完指标  合同
                    worksheets[0].Cells[temprowStart, colStart + 17].PutValue(dicList[i].ProjectTargets[1].NPlanAmmount); //8月完指标 回款
                    worksheets[0].Cells[temprowStart, colStart + 18].PutValue(dicList[i].ProjectTargets[2].NPlanAmmount); //8月完指标 入伙
                    worksheets[0].Cells[temprowStart, colStart + 19].PutValue(dicList[i].ProjectTargets[0].NActualAmmount);  //8月实际数  合同
                    worksheets[0].Cells[temprowStart, colStart + 20].PutValue(dicList[i].ProjectTargets[1].NActualAmmount);  //8月实际数 回款
                    worksheets[0].Cells[temprowStart, colStart + 21].PutValue(dicList[i].ProjectTargets[2].NActualAmmount);  //8月实际数 入伙

                    worksheets[0].Cells[temprowStart, colStart + 22].PutValue(dicList[i].ProjectTargets[0].NDisplayRate);  //8月完成比率  合同
                    TempActualRate = JsonHelper.Deserialize<ActualRate>(dicList[i].ProjectTargets[0].NActualRate);
                    if (TempActualRate.Rate < 1 && dicList[i].ProjectTargets[0].NDisplayRate != "/")
                        worksheets[0].Cells[temprowStart, colStart + 22].SetStyle(style3);//修改样式

                    worksheets[0].Cells[temprowStart, colStart + 23].PutValue(dicList[i].ProjectTargets[1].NDisplayRate);  //8月完成比率 回款
                    TempActualRate = JsonHelper.Deserialize<ActualRate>(dicList[i].ProjectTargets[1].NActualRate);
                    if (TempActualRate.Rate < 1 && dicList[i].ProjectTargets[1].NDisplayRate != "/")
                        worksheets[0].Cells[temprowStart, colStart + 23].SetStyle(style3);//修改样式

                    worksheets[0].Cells[temprowStart, colStart + 24].PutValue(dicList[i].ProjectTargets[2].NDisplayRate);  //8月完成比率 入伙
                    TempActualRate = JsonHelper.Deserialize<ActualRate>(dicList[i].ProjectTargets[2].NActualRate);
                    if (TempActualRate.Rate < 1 && dicList[i].ProjectTargets[2].NDisplayRate != "/")
                        worksheets[0].Cells[temprowStart, colStart + 24].SetStyle(style3); //修改样式


                    if (dicList[i].ProjectTargets[0].Counter > 0)  // 警示灯  合同
                    {
                        int PictureIndex = worksheets[0].Pictures.Add(temprowStart, colStart + 25, ImageFilePath + "\\image" + dicList[i].ProjectTargets[0].Counter + ".png"); // 警示灯  合同
                        Aspose.Cells.Drawing.Picture picture = worksheets[0].Pictures[PictureIndex];
                        picture.Left = 60;
                        picture.Top = 5;
                    }

                    if (dicList[i].ProjectTargets[1].Counter > 0) // 警示灯  回款
                    {
                        int _pictureIndex = worksheets[0].Pictures.Add(temprowStart, colStart + 26, ImageFilePath + "\\image" + dicList[i].ProjectTargets[1].Counter + ".png"); // 警示灯  合同
                        Aspose.Cells.Drawing.Picture picture = worksheets[0].Pictures[_pictureIndex];
                        picture.Left = 60;
                        picture.Top = 5;
                    }

                    if (dicList[i].ExcelGroupRow > 0)
                    {
                        GroupRow = index; //尾盘分组开始行
                    }

                }
            }

            #region 分组数据Excel

            if (dicList.FindAll(p => p.ProDataType == "Remain").Count > 0)
            {
                worksheets[0].Cells.GroupRows(GroupRow + 3, dicList.Count + 4, true);
            }

            #endregion


            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
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

        /// <summary>
        /// 下载Excel完成情况明细--集团总部系统
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="templeteName"></param>
        /// <param name="fileName"></param>
        /// <param name="sytemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        public void DownExcelMonthReportDetail_Group(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            if (rpt._System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
            {
                string strXml = rpt.GetComplateMonthReportDetailHtmlTemplate(rpt._System.Configuration);
                string[] strXmls = strXml.Split(',');
                if (strXmls.Length > 1)
                {
                    templeteName = !string.IsNullOrEmpty(strXmls[3]) ? strXmls[3] : "完成情况明细模板-集团总部V1.xlsx";
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            #region style1 样式
            style1.Font.Size = 12;
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
            style2.Font.Size = 12;
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
            #region style3 样式
            style3.Font.Size = 12;
            style3.Font.IsBold = true;
            style3.Number = 3;
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
            style4.Font.Size = 12;
            style4.HorizontalAlignment = TextAlignmentType.Center;
            style4.ForegroundColor = System.Drawing.Color.White;
            style4.Pattern = BackgroundType.Solid;
            #endregion

            List<DictionaryVmodel> lstGroupMonthReportDetail = null;
            if (rpt != null)
            {
                lstGroupMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "", "", false);
            }

            int rowStart = 6;  //开始行
            int colStart = 1; // 开始列

            //创建指标Sheet,
            for (int sheetIndex = 0; sheetIndex < lstGroupMonthReportDetail.Count; sheetIndex++)
            {
                //标题
                worksheets[sheetIndex].Cells["B1"].PutValue(rpt._System.SystemName + "费用预算执行情况明细表");

                //年月
                worksheets[sheetIndex].Cells["E4"].PutValue(FinYear.ToString() + "年");
                worksheets[sheetIndex].Cells["F4"].PutValue(FinMonth);
                worksheets[sheetIndex].Cells["E4"].SetStyle(style4);
                worksheets[sheetIndex].Cells["E4"].SetStyle(style4);
                List<GroupDictionaryVmodel> lstGDV = (List<GroupDictionaryVmodel>)lstGroupMonthReportDetail[sheetIndex].ObjValue;
                for (int targetIndex = 0; targetIndex < lstGDV.Count; targetIndex++)
                {
                    #region 设置样式与赋值
                    Style tempStyle = style2;
                    if (lstGDV.Count == targetIndex + 1)
                    {
                        tempStyle = style3;
                        worksheets[sheetIndex].Cells[rowStart, colStart].SetStyle(tempStyle);
                    }
                    else
                    {
                        worksheets[sheetIndex].Cells[rowStart, colStart].SetStyle(style1);
                    }
                    worksheets[sheetIndex].Cells[rowStart, colStart].Value = lstGDV[targetIndex].Name;
                    #endregion
                    List<MonthlyReportDetail> lstHaveNotTargetMRD = (List<MonthlyReportDetail>)lstGDV[targetIndex].Value;
                    for (int i = 0; i < lstHaveNotTargetMRD.Count; i++)
                    {

                        #region 设置样式
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 1].SetStyle(tempStyle);
                        Style tempStyle1 = worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 1].GetStyle();
                        tempStyle1.HorizontalAlignment = TextAlignmentType.Center;
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 1].SetStyle(tempStyle1);
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 2].SetStyle(tempStyle1);
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 3].SetStyle(tempStyle1);
                        #endregion
                        #region 为EXCEL赋值
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 1].Value = lstHaveNotTargetMRD[i].NAccumulativeActualAmmount;
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 2].Value = lstHaveNotTargetMRD[i].NAccumulativePlanAmmount;
                        worksheets[sheetIndex].Cells[rowStart, colStart + i * 3 + 3].Value = lstHaveNotTargetMRD[i].NAccumulativeDisplayRate;
                        #endregion
                    }
                    rowStart = rowStart + 1;
                    if (lstGDV[targetIndex].ObjValue != null)
                    {
                        List<V_GroupCompany> lstHaveTargetMRD = (List<V_GroupCompany>)lstGDV[targetIndex].ObjValue;
                        for (int ii = 0; ii < lstHaveTargetMRD.Count; ii++)
                        {
                            #region 设置样式与赋值
                            worksheets[sheetIndex].Cells[rowStart, colStart].SetStyle(style2);
                            worksheets[sheetIndex].Cells[rowStart, colStart].Value = (ii + 1) + "、" + lstHaveTargetMRD[ii].CompanyName;
                            #endregion
                            List<MonthlyReportDetail> lstHaveTargetDetailMRD = (List<MonthlyReportDetail>)lstHaveTargetMRD[ii].ListGroupTargetDetail;
                            for (int j = 0; j < lstHaveTargetDetailMRD.Count; j++)
                            {
                                #region 设置样式
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 1].SetStyle(tempStyle);
                                Style tempStyle1 = worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 1].GetStyle();
                                tempStyle1.HorizontalAlignment = TextAlignmentType.Center;
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 1].SetStyle(tempStyle1);
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 2].SetStyle(tempStyle1);
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 3].SetStyle(tempStyle1);
                                #endregion
                                #region 为EXCEL赋值
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 1].Value = lstHaveTargetDetailMRD[j].NAccumulativeActualAmmount;
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 2].Value = lstHaveTargetDetailMRD[j].NAccumulativePlanAmmount;
                                worksheets[sheetIndex].Cells[rowStart, colStart + j * 3 + 3].Value = lstHaveTargetDetailMRD[j].NAccumulativeDisplayRate;
                                #endregion
                            }
                            rowStart = rowStart + 1;
                        }
                    }

                }
            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
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


        /// <summary>
        /// 下载历史补回期限汇总
        /// </summary>
        public void DownHistoryReturnReport()
        {
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;

            templetePath = ExcelTempletePath;
            templeteName = "历史要求期限统计汇总报表V1.xlsx";
            fileName = StaticResource.Instance[SysId, DateTime.Now].SystemName + "历史要求期限统计汇总报表";


            ExcelEngine excel = new ExcelEngine();
            string str = HttpContext.Current.Server.MapPath("../");
            string Title = FinYear.ToString() + "年" + FinMonth + "月";
            List<HistoryReturnDateVModel> listHistoryReturn = new List<HistoryReturnDateVModel>();

            listHistoryReturn = V_HistoryReturnDateOperator.Instance.GetList(SysId, FinYear);

            if (listHistoryReturn.Count() > 0)
            {
                listHistoryReturn.ForEach(p =>
                {

                });
            }



            MemoryStream stream = ExportExcel(listHistoryReturn, "HistoryReturn", templetePath, templeteName);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(StaticResource.Instance[SysId, DateTime.Now].SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
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
        public MemoryStream ExportExcel<T>(List<T> list, string listName, string templetePath, string templeteName)
        {

            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                designer.Workbook = new Workbook(fileStream);

                designer.SetDataSource("Title", StaticResource.Instance[SysId, DateTime.Now].SystemName + "历史要求期限统计汇总报表");
                designer.SetDataSource("Date", "报告期：" + FinYear.ToString() + "年");
                designer.SetDataSource("Date_Group", FinYear.ToString() + "年");


                designer.SetDataSource(listName, list);
                designer.Process();
                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();
                return stream;

            }
        }



        private string _TrimStr(string obj)
        {
           var str =  Regex.Replace(obj,"^(\\s*\\n)*|(\\n\\s*)*$","");
           return str;
        }


        bool IsReusable
        {
            get
            {
                return false;
            }
        }

        bool IHttpHandler.IsReusable
        {
            get { throw new NotImplementedException(); }
        }
    }
}