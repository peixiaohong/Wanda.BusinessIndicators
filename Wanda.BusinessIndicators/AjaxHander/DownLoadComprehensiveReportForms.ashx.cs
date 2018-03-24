using Aspose.Cells;
using Lib.Config;
using Lib.Web;
using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.ViewModel;
using System.Web.Configuration;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Engine;
using System.Drawing;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownLoadComprehensiveReportForms 的摘要说明
    /// </summary>
    public class DownLoadComprehensiveReportForms : IHttpHandler, IRequiresSessionState
    {

        private string _excelTempletePath = null;
        //Guid SysID = WebConfigurationManager.AppSettings["MovieCN"].ToGuid();
        //Guid TarID = WebConfigurationManager.AppSettings["MovieCNpf"].ToGuid();
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


        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            // string SysIDs, string FinYears, string Targets, string DataType, string IsCurrent

            string SysIDs = HttpContext.Current.Request.QueryString["_SysIDs"];
            string FinYears = HttpContext.Current.Request.QueryString["_FinYears"];
            string FinMonths = HttpContext.Current.Request.QueryString["_FinMonths"];
            string Targets = HttpContext.Current.Request.QueryString["_Targets"];
            string DataType = HttpContext.Current.Request.QueryString["_DataType"];
            string IsCurrent = HttpContext.Current.Request.QueryString["_IsCurrent"];
            //下载报表类型
            string RptType = HttpContext.Current.Request.QueryString["_RptType"];

            if (RptType == "DownExcel") // 综合报表下载
            {
                String[] str = SysIDs.Split(',');
                List<string> list_str = str.ToList();
                string a = string.Join(",", list_str.Select(S => S.ToString()).ToList());


                List<int> list_year = JsonHelper.Deserialize<List<int>>(FinYears);
                string b = string.Join(",", list_year.Select(S => S.ToString()).ToList());


                List<string> list_target = JsonHelper.Deserialize<List<string>>(Targets);
                string c = string.Join(",", list_target.Select(S => S.ToString()).ToList());

                var d = DataType;
                var e = IsCurrent;

                //获取下，当前年份，从数据库中获取
                var f = C_ReportTimeOperator.Instance.GetReportTime().ReportTime.Value.Year;
                var list = V_ComprehensiveReportFormsOperator.Instance.GetList(a, b, c, d, e, f);
                ExcelComprehensiveReportForms(list);
            }
            else
            {
                List<ComprehensiveReportForm> List = new List<ComprehensiveReportForm>();
                int Year = 0;
                int Month = Convert.ToInt32(FinMonths);
                DataType = "_A";
              
                List<int> list_year = JsonHelper.Deserialize<List<int>>(FinYears);

                if (list_year.Count == 1)
                {
                    //选择一个年份的时候，会用
                    Year = list_year.First(); 
                }
                else
                {
                    //反之，则默认为上报的年
                    Year = C_ReportTimeOperator.Instance.GetReportTime().ReportTime.Value.Year;
                }


                if (RptType == "Movie") //万达电影公司
                {
                    var FilmList = V_ComprehensiveReportFormsOperator.Instance.GetFilmCompany(Year, Month, DataType);

                    List<MonthReportSummaryViewModel> list = new List<MonthReportSummaryViewModel>();

                    if (FilmList != null && FilmList.Count > 0)
                    {
                        FilmList.ForEach(p =>
                        {
                            var B = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(p);
                            list.Add(B);
                        });
                    }
                    ExcelFilmCompany(list, Month , Year);
                }
                else if (RptType == "Children") //儿童娱乐公司
                {
                    var ChildrenList = V_ComprehensiveReportFormsOperator.Instance.GetChildrenCompany(Year, Month, DataType);
                    
                    List<MonthReportSummaryViewModel> list = new List<MonthReportSummaryViewModel>();
                    
                    if (ChildrenList != null && ChildrenList.Count > 0)
                    {
                        ChildrenList.ForEach(p =>
                        {
                            var B = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(p);
                            list.Add(B);
                        });
                    }
                    
                    ExcelChildrenCompany(list, Month, Year);
                }
                else if (RptType == "Business") //万达商业
                {
                    var BusinessList = V_ComprehensiveReportFormsOperator.Instance.GetWandaBusiness(Year, Month, DataType);
                    List<MonthReportSummaryViewModel> list = new List<MonthReportSummaryViewModel>();
                    if (BusinessList != null && BusinessList.Count > 0)
                    {
                        BusinessList.ForEach(p =>
                        {
                            var B = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(p);
                            list.Add(B);
                        });
                    }
                    ExcelWandaBusiness(list, Month, Year);
                }
                else if (RptType == "Culture") //万达文化集团
                {
                    var CultureList = V_ComprehensiveReportFormsOperator.Instance.GetWandaCulture(Year, Month);
                    List<MonthReportSummaryViewModel> list = new List<MonthReportSummaryViewModel>();                    
                    if (CultureList != null && CultureList.Count > 0)
                    {
                        CultureList.ForEach(p =>
                        {
                            var B = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(p);
                            list.Add(B);
                        });
                    }
                    ExcelWandaCulture(list, Month, Year);
                }
            }
        }

        /// <summary>
        /// 综合报表查询生成的Excel
        /// </summary>
        /// <param name="List"></param>
        private void ExcelComprehensiveReportForms(List<ComprehensiveReportVModel> List)
        {

            string templetePath = ExcelTempletePath;
            string templeteName = "综合报表查询模版V1.xlsx";
            string fileName = "综合报表";


            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

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

            int rowStartR = 3;  //开始行
            int colStart = 2; // 每日开始列
            int MergeCol = 0; //合并列


            worksheets[0].Name = "综合查询报表";

            List.ForEach(P =>
            {

                worksheets[0].Cells[rowStartR, colStart].SetStyle(style2); // 系统名称
                worksheets[0].Cells[rowStartR, colStart + 1].SetStyle(style2); // 指标名称
                worksheets[0].Cells[rowStartR, colStart + 2].SetStyle(style2); // 年份
                worksheets[0].Cells[rowStartR, colStart + 3].SetStyle(style2); // 月份1
                worksheets[0].Cells[rowStartR, colStart + 4].SetStyle(style2); // 月份2
                worksheets[0].Cells[rowStartR, colStart + 5].SetStyle(style2); // 月份3
                worksheets[0].Cells[rowStartR, colStart + 6].SetStyle(style2); // 月份4
                worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(style2); // 月份5
                worksheets[0].Cells[rowStartR, colStart + 8].SetStyle(style2); // 月份6
                worksheets[0].Cells[rowStartR, colStart + 9].SetStyle(style2); // 月份7
                worksheets[0].Cells[rowStartR, colStart + 10].SetStyle(style2); // 月份8
                worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(style2); // 月份9
                worksheets[0].Cells[rowStartR, colStart + 12].SetStyle(style2); // 月份10
                worksheets[0].Cells[rowStartR, colStart + 13].SetStyle(style2); // 月份11
                worksheets[0].Cells[rowStartR, colStart + 14].SetStyle(style2); // 月份12
                worksheets[0].Cells[rowStartR, colStart + 15].SetStyle(style2); // 月份 合计


                worksheets[0].Cells[rowStartR, colStart].PutValue(P.SystemName); // 系统名称
                worksheets[0].Cells[rowStartR, colStart + 1].PutValue(P.TargetName); // 指标名称
                string _str = P.TType == 0 ? "实际" : "指标";
                worksheets[0].Cells[rowStartR, colStart + 2].PutValue(P.FinYear + "年" + _str); // 年份

                worksheets[0].Cells[rowStartR, colStart + 3].PutValue(P._1); // 月份1
                worksheets[0].Cells[rowStartR, colStart + 4].PutValue(P._2); // 月份2
                worksheets[0].Cells[rowStartR, colStart + 5].PutValue(P._3); // 月份3
                worksheets[0].Cells[rowStartR, colStart + 6].PutValue(P._4); // 月份4
                worksheets[0].Cells[rowStartR, colStart + 7].PutValue(P._5); // 月份5
                worksheets[0].Cells[rowStartR, colStart + 8].PutValue(P._6); // 月份6
                worksheets[0].Cells[rowStartR, colStart + 9].PutValue(P._7); // 月份7
                worksheets[0].Cells[rowStartR, colStart + 10].PutValue(P._8); // 月份8
                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(P._9); // 月份9
                worksheets[0].Cells[rowStartR, colStart + 12].PutValue(P._10); // 月份10
                worksheets[0].Cells[rowStartR, colStart + 13].PutValue(P._11); // 月份11
                worksheets[0].Cells[rowStartR, colStart + 14].PutValue(P._12); // 月份12

                decimal _sum = P._1 + P._2 + P._3 + P._4 + P._5 + P._6 + P._7 + P._8 + P._9 + P._10 + P._11 + P._12;
                worksheets[0].Cells[rowStartR, colStart + 15].PutValue(_sum); // 月份 合计

                rowStartR = rowStartR + 1;
            });



            // 分组系统名称
            var S1 = from p in List
                     group p by p.SystemID into G
                     select new { G.Key, RowCount = G.Count() };

            var GroupList = S1.ToList();

            if (GroupList != null && GroupList.Count > 0)
            {
                int S1_rowStart = 3;

                StyleFlag flg = new StyleFlag();
                flg.All = true;

                for (int i = 0; i < GroupList.Count; i++)
                {
                    Range range = worksheets[0].Cells.CreateRange(S1_rowStart, 2, GroupList[i].RowCount, 1);
                    range.Merge();
                    range.ApplyStyle(style2, flg);

                    S1_rowStart = S1_rowStart + GroupList[i].RowCount;

                }
            }


            //分组指标名称
            var S2 = from p in List
                     group p by p.TargetID into G
                     select new { G.Key, RowCount = G.Count() };

            var GTList = S2.ToList();

            if (GTList != null && GTList.Count > 0)
            {
                int S2_rowStart = 3;
                StyleFlag flg = new StyleFlag();
                flg.All = true;
                for (int i = 0; i < GTList.Count; i++)
                {
                    Range range = worksheets[0].Cells.CreateRange(S2_rowStart, 3, GTList[i].RowCount, 1);
                    range.Merge();
                    range.ApplyStyle(style2, flg);

                    S2_rowStart = S2_rowStart + GTList[i].RowCount;

                }
            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();

        }

        /// <summary>
        /// 固定报表 万达电影公司
        /// </summary>
        /// <param name="List"></param>
        private void ExcelFilmCompany(List<MonthReportSummaryViewModel> BusinessList, int FinMonths ,int Year)
        {
            string templetePath = ExcelTempletePath;
            string templeteName = "万达电影固定报表V1.xlsx";
            string fileName = "万达电影";
            
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            int rowStartR = 4;  //开始行
            int colStart = 1; // 每日开始列

            worksheets[0].Name = "万达电影综合报表";
            worksheets[0].Cells[1, 2].PutValue("报告期：" + Year + "年" + FinMonths + "月");
            worksheets[0].Cells[2, 5].PutValue(FinMonths + "月当月");
            worksheets[0].Cells[2, 9].PutValue("1月-" + FinMonths + "月");

            Cells cells = worksheets[0].Cells;
            DataTable dt = cells.ExportDataTable(4, 2, cells.MaxDataRow - 3, cells.MaxDataColumn);

            //这里 我们循环Excel ，因为固定
            foreach (DataRow dr in dt.Rows)
            {
                //var aa = dr[18]; //隐藏字段 ，系统名称
                //var bb = dr[19];//隐藏字段 ，系统指标

                if (BusinessList != null && BusinessList.Count > 0 && !string.IsNullOrEmpty(dr[18].ToString()) && !string.IsNullOrEmpty(dr[19].ToString()))
                {
                    var P = BusinessList.Where(B => B.SystemName == dr[18].ToString() && B.TargetName == dr[19].ToString()).FirstOrDefault();

                    #region 单元格赋值

                    if (P != null)
                    {
                        worksheets[0].Cells[rowStartR, colStart + 4].PutValue(P.NPlanAmmount);
                        worksheets[0].Cells[rowStartR, colStart + 5].PutValue(P.NActualAmmount); // 当月实际数   
                        worksheets[0].Cells[rowStartR, colStart + 6].PutValue(P.NDifference); // 当月差额
                        worksheets[0].Cells[rowStartR, colStart + 7].PutValue(P.NActualRate); // 当月完成率

                        if (IsRedColor(P.NActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 7].GetStyle()));


                        worksheets[0].Cells[rowStartR, colStart + 8].PutValue(P.NAccumulativePlanAmmount); // 累计指标数
                        worksheets[0].Cells[rowStartR, colStart + 9].PutValue(P.NAccumulativeActualAmmount); // 累计实际数
                        worksheets[0].Cells[rowStartR, colStart + 10].PutValue(P.NAccumulativeDifference); // 累计差额
                        worksheets[0].Cells[rowStartR, colStart + 11].PutValue(P.NAccumulativeActualRate); // 累计完成率

                        if (IsRedColor(P.NActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 11].GetStyle()));


                        worksheets[0].Cells[rowStartR, colStart + 12].PutValue(decimal.Parse(P.MeasureRate)); // 全年计划额度
                        worksheets[0].Cells[rowStartR, colStart + 13].PutValue(P.NAnnualCompletionRate); // 全年完成比
                    }
                    #endregion
                }

                rowStartR = rowStartR + 1;

            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();


        }

        /// <summary>
        /// 固定报表 宝贝王公司公司
        /// </summary>
        /// <param name="List"></param>
        private void ExcelChildrenCompany(List<MonthReportSummaryViewModel> BusinessList, int FinMonths, int Year)
        {
            string templetePath = ExcelTempletePath;
            string templeteName = "儿童娱乐固定报表V1.xlsx";
            string fileName = "宝贝王公司";


            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            int rowStartR = 4;  //开始行
            int colStart = 1; // 每日开始列
         
            worksheets[0].Name = "宝贝王综合报表";
            worksheets[0].Cells[1, 2].PutValue("报告期：" + Year + "年" + FinMonths + "月");
            worksheets[0].Cells[2, 5].PutValue(FinMonths + "月当月");
            worksheets[0].Cells[2, 9].PutValue("1月-" + FinMonths + "月");


            Cells cells = worksheets[0].Cells;
            DataTable dt = cells.ExportDataTable(4, 2, cells.MaxDataRow - 3, cells.MaxDataColumn);

            //这里 我们循环Excel ，因为固定
            foreach (DataRow dr in dt.Rows)
            {
                //var aa = dr[18]; //隐藏字段 ，系统名称
                //var bb = dr[19];//隐藏字段 ，系统指标

                if (BusinessList != null && BusinessList.Count > 0 && !string.IsNullOrEmpty(dr[18].ToString()) && !string.IsNullOrEmpty(dr[19].ToString()))
                {
                    var P = BusinessList.Where(B => B.SystemName == dr[18].ToString() && B.TargetName == dr[19].ToString()).FirstOrDefault();

                    #region 单元格赋值

                    if (P != null)
                    {
                        worksheets[0].Cells[rowStartR, colStart + 4].PutValue(P.NPlanAmmount);
                        worksheets[0].Cells[rowStartR, colStart + 5].PutValue(P.NActualAmmount); // 当月实际数   
                        worksheets[0].Cells[rowStartR, colStart + 6].PutValue(P.NDifference); // 当月差额
                        worksheets[0].Cells[rowStartR, colStart + 7].PutValue(P.NActualRate); // 当月完成率

                        if (IsRedColor(P.NActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 7].GetStyle()));

                        worksheets[0].Cells[rowStartR, colStart + 8].PutValue(P.NAccumulativePlanAmmount); // 累计指标数
                        worksheets[0].Cells[rowStartR, colStart + 9].PutValue(P.NAccumulativeActualAmmount); // 累计实际数
                        worksheets[0].Cells[rowStartR, colStart + 10].PutValue(P.NAccumulativeDifference); // 累计差额
                        worksheets[0].Cells[rowStartR, colStart + 11].PutValue(P.NAccumulativeActualRate); // 累计完成率

                        if (IsRedColor(P.NAccumulativeActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 11].GetStyle()));


                        worksheets[0].Cells[rowStartR, colStart + 12].PutValue(decimal.Parse(P.MeasureRate)); // 全年计划额度
                        worksheets[0].Cells[rowStartR, colStart + 13].PutValue(P.NAnnualCompletionRate); // 全年完成比
                    }
                    #endregion
                }

                rowStartR = rowStartR + 1;

            }
            
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();


        }


        /// <summary>
        /// 固定报表 万达商业整体
        /// </summary>
        /// <param name="List"></param>
        private void ExcelWandaBusiness(List<MonthReportSummaryViewModel> BusinessList, int FinMonths, int Year)
        {
            string templetePath = ExcelTempletePath;
            string templeteName = "万达商业固定报表V1.xlsx";
            string fileName = "万达商业";


            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            
            //对传过来的数据进行分组


            int rowStartR = 4;  //开始行
            int colStart = 2; // 每日开始列

            worksheets[0].Name = "万达商业汇总";
            worksheets[0].Cells[1, 2].PutValue("报告期：" + Year + "年" + FinMonths + "月");
            worksheets[0].Cells[2, 6].PutValue(FinMonths + "月当月");
            worksheets[0].Cells[2, 10].PutValue("1月-" + FinMonths + "月");


            Cells cells = worksheets[0].Cells;
            DataTable dt = cells.ExportDataTable(4, 2, cells.MaxDataRow - 3, cells.MaxDataColumn);

            //这里 我们循环Excel ，因为固定
            foreach (DataRow dr in dt.Rows)
            {
                //var aa = dr[18]; //隐藏字段 ，系统名称
                //var bb = dr[19];//隐藏字段 ，系统指标

                if (BusinessList != null && BusinessList.Count > 0 &&  !string.IsNullOrEmpty(dr[18].ToString()) &&  !string.IsNullOrEmpty(dr[19].ToString()) )
                {
                    var P = BusinessList.Where(B => B.SystemName == dr[18].ToString() && B.TargetName == dr[19].ToString()).FirstOrDefault();

                    #region 单元格赋值

                    if (P != null)
                    {
                        worksheets[0].Cells[rowStartR, colStart + 4].PutValue(P.NPlanAmmount);
                        worksheets[0].Cells[rowStartR, colStart + 5].PutValue(P.NActualAmmount); // 当月实际数   
                        worksheets[0].Cells[rowStartR, colStart + 6].PutValue(P.NDifference); // 当月差额
                        worksheets[0].Cells[rowStartR, colStart + 7].PutValue(P.NActualRate); // 当月完成率
                        
                        if ( IsRedColor(P.NActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 7].GetStyle()));
                        

                        worksheets[0].Cells[rowStartR, colStart + 8].PutValue(P.NAccumulativePlanAmmount); // 累计指标数
                        worksheets[0].Cells[rowStartR, colStart + 9].PutValue(P.NAccumulativeActualAmmount); // 累计实际数
                        worksheets[0].Cells[rowStartR, colStart + 10].PutValue(P.NAccumulativeDifference); // 累计差额
                        worksheets[0].Cells[rowStartR, colStart + 11].PutValue(P.NAccumulativeActualRate); // 累计完成率
                        
                        if (IsRedColor(P.NAccumulativeActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 11].GetStyle()));


                        
                        if(!string.IsNullOrEmpty(P.MeasureRate))
                            worksheets[0].Cells[rowStartR, colStart + 12].PutValue(decimal.Parse(P.MeasureRate)); // 全年计划额度
                        else
                            worksheets[0].Cells[rowStartR, colStart + 12].PutValue(P.MeasureRate);

                        worksheets[0].Cells[rowStartR, colStart + 13].PutValue(P.NAnnualCompletionRate); // 全年完成比
                        
                    }
                    #endregion
                }

                rowStartR = rowStartR + 1;

            }
            
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();


        }

        /// <summary>
        /// 固定报表 文化集团整体
        /// </summary>
        /// <param name="BusinessList"></param>
        /// <param name="FinMonths"></param>
        private void ExcelWandaCulture(List<MonthReportSummaryViewModel> BusinessList, int FinMonths, int Year)
        {
            string templetePath = ExcelTempletePath;
            string templeteName = "文化集团固定报表V1.xlsx";
            string fileName = "文化集团";


            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            //对传过来的数据进行分组


            int rowStartR = 4;  //开始行
            int colStart = 1; // 每日开始列

            worksheets[0].Name = "文化集团汇总";
            worksheets[0].Cells[1, 2].PutValue("报告期："+ Year + "年" + FinMonths+ "月");
            worksheets[0].Cells[2, 5].PutValue(FinMonths + "月当月");
            worksheets[0].Cells[2, 9].PutValue("1月-" + FinMonths + "月");


            Cells cells = worksheets[0].Cells;
            DataTable dt = cells.ExportDataTable(4, 2, cells.MaxDataRow - 3, cells.MaxDataColumn);

            //这里 我们循环Excel ，因为固定
            foreach (DataRow dr in dt.Rows)
            {
                //var aa = dr[18]; //隐藏字段 ，系统名称
                //var bb = dr[19];//隐藏字段 ，系统指标

                if (BusinessList != null && BusinessList.Count > 0 && !string.IsNullOrEmpty(dr[18].ToString()) && !string.IsNullOrEmpty(dr[19].ToString()))
                {
                    var P = BusinessList.Where(B => B.SystemName == dr[18].ToString() && B.TargetName == dr[19].ToString()).FirstOrDefault();

                    #region 单元格赋值

                    if (P != null)
                    {
                        worksheets[0].Cells[rowStartR, colStart + 4].PutValue(P.NPlanAmmount);
                        worksheets[0].Cells[rowStartR, colStart + 5].PutValue(P.NActualAmmount); // 当月实际数   
                        worksheets[0].Cells[rowStartR, colStart + 6].PutValue(P.NDifference); // 当月差额
                        worksheets[0].Cells[rowStartR, colStart + 7].PutValue(P.NActualRate); // 当月完成率

                        if (IsRedColor(P.NActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 7].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 7].GetStyle()));


                        worksheets[0].Cells[rowStartR, colStart + 8].PutValue(P.NAccumulativePlanAmmount); // 累计指标数
                        worksheets[0].Cells[rowStartR, colStart + 9].PutValue(P.NAccumulativeActualAmmount); // 累计实际数
                        worksheets[0].Cells[rowStartR, colStart + 10].PutValue(P.NAccumulativeDifference); // 累计差额
                        worksheets[0].Cells[rowStartR, colStart + 11].PutValue(P.NAccumulativeActualRate); // 累计完成率

                        if (IsRedColor(P.NAccumulativeActualRate))
                            worksheets[0].Cells[rowStartR, colStart + 11].SetStyle(SetCellStyle(worksheets[0].Cells[rowStartR, colStart + 11].GetStyle()));

                        worksheets[0].Cells[rowStartR, colStart + 12].PutValue(decimal.Parse(P.MeasureRate)); // 全年计划额度
                        worksheets[0].Cells[rowStartR, colStart + 13].PutValue(P.NAnnualCompletionRate); // 全年完成比

                    }
                    #endregion
                }

                rowStartR = rowStartR + 1;

            }

            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();


        }




        /// <summary>
        /// 是否红色
        /// </summary>
        /// <returns></returns>
        private bool IsRedColor(string _ValStr)
        {
            if (!string.IsNullOrEmpty(_ValStr))
            {
                if (_ValStr.IndexOf("超支") > -1 || _ValStr.IndexOf("增亏") > -1)
                    return true;
                else if (_ValStr.IndexOf("节约") > -1 || _ValStr.IndexOf("减亏") > -1 || _ValStr.IndexOf("超计划") > -1 || _ValStr == "--")
                    return false;
                else
                {
                    string V = _ValStr.Replace("%", "").Replace("万元", "");
                    decimal _NV = 0;
                    decimal.TryParse(V, out _NV);

                    if (_NV < 100)
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
        }



        private Style SetCellStyle(Style S)
        {
            S.Font.Color = Color.Red;

            return S;
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