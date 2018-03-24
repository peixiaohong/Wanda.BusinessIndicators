
using Aspose.Cells;
using Lib.Core;
using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web.AppCode;
using System.Web.Configuration;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    public partial class DownLoadDSExcel : System.Web.UI.Page
    {
        string ImageFilePath = null;
        Guid SysDescriptionID = WebConfigurationManager.AppSettings["MonthDescription"].ToGuid();//百货系统ID
        protected void Page_Load(object sender, EventArgs e)
        {
            string type = Request.QueryString["ActionType"];
            ImageFilePath = HttpContext.Current.Server.MapPath("../Images/images1");
            string param = Request.QueryString["Param"];
            int FinYear = 0;
            int FinMonth = 0;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear = Convert.ToInt32(HttpContext.Current.Request["FinYear"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinMonth"]))
            {
                FinMonth = Convert.ToInt32(HttpContext.Current.Request["FinMonth"]);
            }
            bool v = true;
            string error = "";
            switch (type)
            {
                case "TargetSum":
                    if (!v)
                    {
                        error = "TargetSum 参数有误";
                    }
                    else
                    {
                        v = DownLoadDSTargetReport(ref error, param, FinYear, FinMonth);
                    }
                    break;
                case "TargetCompleted":
                    if (!v)
                    {
                        error = "TargetCompleted 参数有误";
                    }
                    else
                    {
                        v = DownLoadDSTargetCompletedReport(ref error, param, FinYear, FinMonth);
                    }
                    break;
                case "TargetReturnData":
                    if (!v)
                    {
                        error = "TargetCompleted 参数有误";
                    }
                    else
                    {
                        v = DownLoadDSTargetReturnDataReport(ref error, param, FinYear, FinMonth);
                    }
                    break;
                case "TargetAddData":
                    if (!v)
                    {
                        error = "TargetCompleted 参数有误";
                    }
                    else
                    {
                        v = DownLoadDSTargetAddDataReport(ref error, param, FinYear, FinMonth);
                    }
                    break;
                default:
                    error = "ActionType 参数有误";
                    break;
            }

        }
        /// <summary>
        /// 设置Excel中的Cell的样式
        /// </summary>
        /// <param name="designer"></param>
        /// <param name="IsSetBackground"></param>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Bule"></param>
        /// <returns></returns>
        private Aspose.Cells.Style SetCellStyle(WorkbookDesigner designer, bool IsSetBackground, int Red, int Green, int Bule,bool IsTitle)
        {
            Aspose.Cells.Style style = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            style.Pattern = BackgroundType.Solid;
            style.IsLocked = true;//设置只读
            style.Font.Name = "Arial";
            style.Font.Size = 11;
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Center;
            if (IsSetBackground)
            {
                if (IsTitle)
                {
                    style.Font.Size = 14;
                    style.HorizontalAlignment = TextAlignmentType.Left;
                }
                style.ForegroundColor = System.Drawing.Color.FromArgb(Red, Green, Bule);
            }
            style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            return style;
        }
        /// <summary>
        /// 保存excel附件
        /// </summary>
        /// <param name="designer"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        private void SaveExcelFiles(WorkbookDesigner designer, FileStream fileStream, string fileName)
        {
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }


        /// <summary>
        /// 新的方法
        /// </summary>
        /// <param name="c"></param>
        /// <param name="LastMonthReportDetails"></param>
        /// <param name="CurrentMonthReportDetails"></param>
        /// <param name="itemt"></param>
        /// <param name="IsCurrentMonth"></param>
        /// <returns></returns>
        public MonthlyReportDetail GetMonthlyReportDetail(C_Company c, List<MonthlyReportDetail> LastMonthReportDetails, List<MonthlyReportDetail> CurrentMonthReportDetails, C_Target itemt, bool IsCurrentMonth)
        {
            MonthlyReportDetail ReportDetailModel = new MonthlyReportDetail();

            if (IsCurrentMonth)
            {
                ReportDetailModel = CurrentMonthReportDetails.SingleOrDefault(t => t.TargetID == itemt.ID && t.CompanyID == c.ID);
            }
            else
            {
                ReportDetailModel = LastMonthReportDetails.SingleOrDefault(t => t.TargetID == itemt.ID && t.CompanyID == c.ID);
            }

            return ReportDetailModel;
        }





        /// <summary>
        /// 百货系统经营指标完成门店数量情况excel下载
        /// </summary>
        /// <param name="error"></param>
        /// <param name="param"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        private bool DownLoadDSTargetReport(ref string error, string param, int FinYear, int FinMonth)
        {
            string fileName = String.Format("百货系统经营指标完成门店数量情况({0}月)", FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成门店数量情况.xls";
            if (FinMonth == 1)
            {
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成门店数量情况(1月).xls";
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(fileStream);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);

            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0,false);
            Aspose.Cells.Style style2 = SetCellStyle(designer, true, 252, 213, 180, false);
            #endregion
            //表头
            if (FinMonth == 1)
            {
                string htitle = string.Format("报告期：{0}年1月", FinYear, FinMonth);
                sheet.Cells[1, 1].PutValue(htitle);
                sheet.Cells[2, 2].PutValue("1月");
                sheet.Cells[2, 6].PutValue("1月累计");
            }
            else
            {
                string htitle = string.Format("报告期：{0}年1~{1}月", FinYear, FinMonth);
                sheet.Cells[1, 1].PutValue(htitle);
                sheet.Cells[2, 2].PutValue(string.Format("1-{0}月累计", FinMonth - 1));
                sheet.Cells[2, 6].PutValue(string.Format("{0}月", FinMonth));
                sheet.Cells[2, 10].PutValue(string.Format("1-{0}月累计", FinMonth));
            }
            List<ShowDSTargetCompleted> list = GetDSTargetCompleted(FinYear,FinMonth,Convert.ToBoolean(param));
            if (list != null)
            {
                for (int rowindex = 4; rowindex < (4 + list.Count); rowindex++)
                {
                    ShowDSTargetCompleted model = list.SingleOrDefault(t => t.PorjectName == list[rowindex - 4].PorjectName);
                    int colIndex = 1;
                    Aspose.Cells.Style setStyle = style;
                    setStyle.Number = 0;

                    if (model.PorjectName == "合计")
                    {
                        setStyle = style2;
                    }
                    sheet.Cells[rowindex, colIndex].PutValue(model.PorjectName); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    if (FinMonth > 1)
                    {
                        sheet.Cells[rowindex, colIndex].PutValue(model.LastNorth); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(model.LastCenter); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(model.LastSouth); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(model.LastTotal); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    }
                    sheet.Cells[rowindex, colIndex].PutValue(model.CurrentNorth); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.CurrentCenter); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.CurrentSouth); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.CurrentTotal); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.ToCurrentNorth); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.ToCurrentCenter); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.ToCurrentSouth); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                    sheet.Cells[rowindex, colIndex].PutValue(model.ToCurrentTotal); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                }
            }
            //保存excel附件
            SaveExcelFiles(designer, fileStream, fileName);
            return true;
        }
        /// <summary>
        /// 百货系统经营指标完成门店数量情况
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public List<ShowDSTargetCompleted> GetDSTargetCompleted(int Year, int Month, bool IsLatestVersion)
        {
            List<ShowDSTargetCompleted> showList = new List<ShowDSTargetCompleted>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).ToList();
                TargetList = TargetList.Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                List<C_Company> CompanyList = C_CompanyOperator.Instance.GetCompanyList(SystemModel.ID).ToList();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();
                List<MonthlyReportDetail> LastMonthReportDetails = new List<MonthlyReportDetail>();

                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthReportDetails = LastMonthReport.ReportDetails;
                    
                }
                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthReportDetails = CurrentMonthReport.ReportDetails;
                
                #region 判断完成了几个指标
                List<DSTargetCompleted> list = new List<DSTargetCompleted>();
                foreach (C_Company c in CompanyList)
                {
                    int LastCompletedCount = 0;
                    int CurCompletedCount = 0;
                    int ToCurCompletedCount = 0;
                    DSTargetCompleted DSmodel = new DSTargetCompleted();
                    DSmodel.CompanyName = c.CompanyName;
                    DSmodel.AreaName = c.CompanyProperty3;
                    foreach (C_Target itemt in TargetList)
                    {
                        if (Month > 1)
                        {
                            //上一个月
                            MonthlyReportDetail LastDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, false);
                            if (LastDetailModel != null && LastDetailModel.ID != Guid.Empty)
                            {
                                if (LastDetailModel.IsMissTarget)
                                {
                                    LastCompletedCount++;
                                }
                            }
                        }
                        //当前月
                        MonthlyReportDetail CurDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, true);
                        if (CurDetailModel != null && CurDetailModel.ID != Guid.Empty)
                        {
                            if (CurDetailModel.IsMissTargetCurrent)
                            {
                                CurCompletedCount++;
                            }
                            if (CurDetailModel.IsMissTarget)
                            {
                                ToCurCompletedCount++;
                            }
                        }
                    }
                    DSmodel.LastCount = LastCompletedCount;
                    DSmodel.CurrentCount = CurCompletedCount;
                    DSmodel.ToCurrentCount = ToCurCompletedCount;
                    list.Add(DSmodel);
                }
                //判断指标完成几个 
                if (list.Count > 0)
                {
                    List<DSTargetCompleted> NorthList = list.Where(t => t.AreaName == "北区").ToList();
                    List<DSTargetCompleted> SouthList = list.Where(t => t.AreaName == "南区").ToList();
                    List<DSTargetCompleted> CenterList = list.Where(t => t.AreaName == "中区").ToList();
                    //单双指标名称的枚举List
                    EnumItemDescriptionList enumList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(DSProjectType));

                    for (int i = 0; i < enumList.Count; i++)
                    {
                        ShowDSTargetCompleted showModel = new ShowDSTargetCompleted();
                        showModel.PorjectName = EnumHelper.GetEnumDescription(typeof(DSProjectType), i);

                        if (Month > 1 && LastMonthReportDetails != null && LastMonthReportDetails.Count > 0)
                        {
                            showModel.LastNorth = NorthList.Where(t => t.LastCount == i).ToList().Count;
                            showModel.LastCenter = CenterList.Where(t => t.LastCount == i).ToList().Count;
                            showModel.LastSouth = SouthList.Where(t => t.LastCount == i).ToList().Count;
                            showModel.LastTotal = showModel.LastNorth + showModel.LastCenter + showModel.LastSouth;
                        }
                        else
                        {
                            showModel.LastNorth = 0;
                            showModel.LastCenter = 0;
                            showModel.LastSouth = 0;
                            showModel.LastTotal = 0;
                        }

                        if (CurrentMonthReportDetails.Count > 0)
                        {
                            showModel.CurrentNorth = NorthList.Where(t => t.CurrentCount == i).ToList().Count;
                            showModel.CurrentCenter = CenterList.Where(t => t.CurrentCount == i).ToList().Count;
                            showModel.CurrentSouth = SouthList.Where(t => t.CurrentCount == i).ToList().Count;
                            showModel.CurrentTotal = showModel.CurrentNorth + showModel.CurrentCenter + showModel.CurrentSouth;

                            showModel.ToCurrentNorth = NorthList.Where(t => t.ToCurrentCount == i).ToList().Count;
                            showModel.ToCurrentCenter = CenterList.Where(t => t.ToCurrentCount == i).ToList().Count;
                            showModel.ToCurrentSouth = SouthList.Where(t => t.ToCurrentCount == i).ToList().Count;
                            showModel.ToCurrentTotal = showModel.ToCurrentNorth + showModel.ToCurrentCenter + showModel.ToCurrentSouth;
                        }
                        else
                        {
                            showModel.CurrentNorth = 0;
                            showModel.CurrentCenter = 0;
                            showModel.CurrentSouth = 0;
                            showModel.CurrentTotal = 0;

                            showModel.ToCurrentNorth = 0;
                            showModel.ToCurrentCenter = 0;
                            showModel.ToCurrentSouth = 0;
                            showModel.ToCurrentTotal = 0;
                        }
                        showList.Add(showModel);
                    }
                }
                #endregion
                //合计
                ShowDSTargetCompleted hjshowModel = new ShowDSTargetCompleted();
                hjshowModel.PorjectName = "合计";
                hjshowModel.LastNorth = showList.Sum(t => t.LastNorth);
                hjshowModel.LastCenter = showList.Sum(t => t.LastCenter);
                hjshowModel.LastSouth = showList.Sum(t => t.LastSouth);
                hjshowModel.LastTotal = showList.Sum(t => t.LastTotal);
                hjshowModel.CurrentNorth = showList.Sum(t => t.CurrentNorth);
                hjshowModel.CurrentCenter = showList.Sum(t => t.CurrentCenter);
                hjshowModel.CurrentSouth = showList.Sum(t => t.CurrentSouth);
                hjshowModel.CurrentTotal = showList.Sum(t => t.CurrentTotal);
                hjshowModel.ToCurrentNorth = showList.Sum(t => t.ToCurrentNorth);
                hjshowModel.ToCurrentCenter = showList.Sum(t => t.ToCurrentCenter);
                hjshowModel.ToCurrentSouth = showList.Sum(t => t.ToCurrentSouth);
                hjshowModel.ToCurrentTotal = showList.Sum(t => t.ToCurrentTotal);
                showList.Add(hjshowModel);
                #region 判断哪个指标未完成
                List<DSTargetCompleted> MissTargetlist = new List<DSTargetCompleted>();
                foreach (C_Target itemt in TargetList.OrderBy(T => T.Sequence))
                {
                    #region//北区
                    List<int> NorthList = GetMissTargetCountList(CompanyList, LastMonthReportDetails, CurrentMonthReportDetails, itemt, "北区", Month);
                    #endregion
                    #region//中区
                    List<int> CenterList = GetMissTargetCountList(CompanyList, LastMonthReportDetails, CurrentMonthReportDetails, itemt, "中区", Month);
                    #endregion
                    #region//南区
                    List<int> SouthList = GetMissTargetCountList(CompanyList, LastMonthReportDetails, CurrentMonthReportDetails, itemt, "南区", Month);
                    #endregion
                    ShowDSTargetCompleted ShowDSmodel = new ShowDSTargetCompleted();
                    ShowDSmodel.PorjectName = itemt.TargetName + "未完成门店数";
                    ShowDSmodel.LastNorth = NorthList[0];
                    ShowDSmodel.LastCenter = CenterList[0]; ;
                    ShowDSmodel.LastSouth = SouthList[0]; ;
                    ShowDSmodel.LastTotal = ShowDSmodel.LastNorth + ShowDSmodel.LastCenter + ShowDSmodel.LastSouth;
                    ShowDSmodel.CurrentNorth = NorthList[1];
                    ShowDSmodel.CurrentCenter = CenterList[1];
                    ShowDSmodel.CurrentSouth = SouthList[1];
                    ShowDSmodel.CurrentTotal = ShowDSmodel.CurrentNorth + ShowDSmodel.CurrentCenter + ShowDSmodel.CurrentSouth;
                    ShowDSmodel.ToCurrentNorth = NorthList[2];
                    ShowDSmodel.ToCurrentCenter = CenterList[2];
                    ShowDSmodel.ToCurrentSouth = SouthList[2];
                    ShowDSmodel.ToCurrentTotal = ShowDSmodel.ToCurrentNorth + ShowDSmodel.ToCurrentCenter + ShowDSmodel.ToCurrentSouth;
                    showList.Add(ShowDSmodel);
                }
                #endregion
            }
            return showList;
        }
        /// <summary>
        /// 获取每一个区域的每一个未完成的指标的店铺的数量
        /// </summary>
        /// <param name="CompanyList"></param>
        /// <param name="LastMonthReportDetails"></param>
        /// <param name="BLastMonthReportDetails"></param>
        /// <param name="CurrentMonthReportDetails"></param>
        /// <param name="BCurrentMonthReportDetails"></param>
        /// <param name="itemt"></param>
        /// <param name="AreaName"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public List<int> GetMissTargetCountList(List<C_Company> CompanyList,
            List<MonthlyReportDetail> LastMonthReportDetails,
            List<MonthlyReportDetail> CurrentMonthReportDetails,
            C_Target itemt, string AreaName, int Month)
        {
            List<int> MissTargetCountList = new List<int>();
            int NorthLastMissCount = 0;
            int NorthCurMissCount = 0;
            int NorthToCurMissCount = 0;
            List<C_Company> NorthCompanyList = CompanyList.Where(t => t.CompanyProperty3 == AreaName).ToList();
            foreach (C_Company c in NorthCompanyList)
            {
                if (Month > 1)
                {
                    //上一个月
                    MonthlyReportDetail LastDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, false);
                    if (LastDetailModel != null && LastDetailModel.ID != Guid.Empty)
                    {
                        if (LastDetailModel.IsMissTarget)
                        {
                            NorthLastMissCount++;
                        }
                    }
                }
                //当前月
                MonthlyReportDetail CurDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, true);
                if (CurDetailModel != null && CurDetailModel.ID != Guid.Empty)
                {
                    if (CurDetailModel.IsMissTargetCurrent)
                    {
                        NorthCurMissCount++;
                    }
                    if (CurDetailModel.IsMissTarget)
                    {
                        NorthToCurMissCount++;
                    }
                }
            }
            MissTargetCountList.Add(NorthLastMissCount);
            MissTargetCountList.Add(NorthCurMissCount);
            MissTargetCountList.Add(NorthToCurMissCount);
            return MissTargetCountList;
        }
        /// <summary>
        /// 百货系统经营指标完成情况对比Excel下载
        /// </summary>
        /// <param name="error"></param>
        /// <param name="param"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        private bool DownLoadDSTargetCompletedReport(ref string error, string param, int FinYear, int FinMonth)
        {
            string fileName = String.Format("百货系统经营指标完成情况对比({0}月)", FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成情况对比.xls";
            if (FinMonth == 1)
            {
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成情况对比(1月).xls";
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(fileStream);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);
            List<ShowDSTargetArea> list = GetDSTargetCompletedDetail(FinYear, FinMonth, Convert.ToBoolean(param));
            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0,false);
            Aspose.Cells.Style style2 = SetCellStyle(designer, true, 184, 204, 228, false);
            #endregion
            //表头
            if (FinMonth == 1)
            {
                sheet.Cells[0, 1].PutValue("1月经营指标完成情况");
                sheet.Cells[1, 1].PutValue(string.Format("报告期：{0}年1月", FinYear));
                sheet.Cells[2, 3].PutValue(string.Format("{0}月", FinMonth));
                sheet.Cells[2, 7].PutValue(string.Format("{0}月累计", FinMonth));
            }
            else
            {
                sheet.Cells[0, 1].PutValue(string.Format("{0}月与{1}月经营指标完成情况对比", FinMonth, FinMonth - 1));
                sheet.Cells[1, 1].PutValue( string.Format("报告期：{0}年1~{1}月", FinYear, FinMonth));
                sheet.Cells[2, 3].PutValue(string.Format("{0}月", FinMonth - 1));
                sheet.Cells[2, 7].PutValue(string.Format("{0}月", FinMonth));
                sheet.Cells[2, 11].PutValue(string.Format("1-{0}月累计", FinMonth));
            }
            //给excel赋值
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).ToList();
                TargetList = TargetList.Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                int targetCount = TargetList.Count;
                int rowindex = 4;
                StyleFlag flg = new StyleFlag();
                flg.All = true;
                foreach (ShowDSTargetArea area in list)
                {
                    Aspose.Cells.Style setStyle = style;
                    if (area.ID == 4)
                    {
                        setStyle = style2;
                    }
                    Range range1 = sheet.Cells.CreateRange(rowindex, 1, targetCount, 1);
                    range1.Merge();
                    range1.ApplyStyle(setStyle, flg);
                    sheet.Cells[rowindex, 1].PutValue(area.AreaName);
                    foreach (ShowDSTargetCompletedDetail item in area.DetailList)
                    {
                        int colIndex = 2;
                        sheet.Cells[rowindex, colIndex].PutValue(item.DetailTargetName); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        if (FinMonth > 1)
                        {
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastPlan); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastActual); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastDifference); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastRate); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        }
                        sheet.Cells[rowindex, colIndex].PutValue(item.CurrentPlan); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.CurrentActual); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.CurrentDifference); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.CurrentRate); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.ToCurrentPlan); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.ToCurrentActual); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.ToCurrentDifference); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        sheet.Cells[rowindex, colIndex].PutValue(item.ToCurrentRate); sheet.Cells[rowindex, colIndex].SetStyle(setStyle); colIndex++;
                        rowindex++;
                    }
                }
            }
            //保存excel附件
            SaveExcelFiles(designer, fileStream, fileName);
            return true;
        }
        /// <summary>
        /// 获取完成情况详情的集合
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public List<ShowDSTargetArea> GetDSTargetCompletedDetail(int Year, int Month, bool IsLatestVersion)
        {
            List<ShowDSTargetArea> Showlist = new List<ShowDSTargetArea>();
            //计算完成率的集合
            List<MonthReportSummaryViewModel> listMonthReportSummaryViewModel = new List<MonthReportSummaryViewModel>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).ToList();
                TargetList = TargetList.Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                List<C_Company> CompanyList = C_CompanyOperator.Instance.GetCompanyList(SystemModel.ID).ToList();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();

                List<MonthlyReportDetail> LastMonthReportDetails = new List<MonthlyReportDetail>();
              
                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthReportDetails = LastMonthReport.ReportDetails;
                }

                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthReportDetails = CurrentMonthReport.ReportDetails;
                
                int RateIndex = 1;
                for (int i = 1; i < EnumUtil.GetItems(typeof(DSDetailAreaName)).Count + 1; i++)
                {
                    ShowDSTargetArea DSTargetArea = new ShowDSTargetArea();
                    DSTargetArea.ID = i;
                    DSTargetArea.AreaName = EnumUtil.GetEnumDescription(typeof(DSDetailAreaName), i);
                    if (i < 4)
                    {
                        List<ShowDSTargetCompletedDetail> list = new List<ShowDSTargetCompletedDetail>();
                        foreach (C_Target itemt in TargetList.OrderBy(t => t.Sequence))
                        {
                            //计算完成率实体（上一个月）
                            MonthReportSummaryViewModel Lastmrsvm = new MonthReportSummaryViewModel();
                            //计算完成率实体（当前月及其累计）
                            MonthReportSummaryViewModel mrsvm = new MonthReportSummaryViewModel();
                            decimal LastSumPlan = 0;
                            decimal LastSumActual = 0;
                            decimal CurrentSumPlan = 0;
                            decimal CurrentSumActual = 0;
                            decimal CurrentSumAccumulativePlan = 0;
                            decimal CurrentSumAccumulativeActual = 0;
                            ShowDSTargetCompletedDetail DSModel = new ShowDSTargetCompletedDetail();
                            DSModel.DetailAreaID = i;
                            DSModel.DetailTargetName = itemt.TargetName;
                            List<C_Company> NorthCompanyList = CompanyList.Where(t => t.CompanyProperty3 == DSTargetArea.AreaName).ToList();
                            foreach (C_Company c in NorthCompanyList)
                            {
                                if (Month > 1)
                                {
                                    //上一个
                                    MonthlyReportDetail LastReportDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, false);
                                    if (LastReportDetailModel != null && LastReportDetailModel.ID != Guid.Empty)
                                    {

                                        LastSumPlan += LastReportDetailModel.NPlanAmmount;
                                        LastSumActual += LastReportDetailModel.NActualAmmount;
                                    }
                                }
                                //当前月以及当前月累计
                                MonthlyReportDetail CurrentReportDetailModel = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, itemt, true);
                                if (CurrentReportDetailModel != null && CurrentReportDetailModel.ID != Guid.Empty)
                                {
                                    CurrentSumPlan += CurrentReportDetailModel.NPlanAmmount;
                                    CurrentSumActual += CurrentReportDetailModel.NActualAmmount;
                                    CurrentSumAccumulativePlan += CurrentReportDetailModel.NAccumulativePlanAmmount;
                                    CurrentSumAccumulativeActual += CurrentReportDetailModel.NAccumulativeActualAmmount;
                                }
                            }
                            DSModel.LastPlan = Convert.ToDecimal(LastSumPlan.ToString("N2"));
                            DSModel.LastActual = Convert.ToDecimal(LastSumActual.ToString("N2"));
                            DSModel.LastDifference = Convert.ToDecimal((LastSumActual - LastSumPlan).ToString("N2"));
                            DSModel.CurrentPlan = Convert.ToDecimal(CurrentSumPlan.ToString("N2"));
                            DSModel.CurrentActual = Convert.ToDecimal(CurrentSumActual.ToString("N2"));
                            DSModel.CurrentDifference = Convert.ToDecimal((CurrentSumActual - CurrentSumPlan).ToString("N2"));
                            DSModel.ToCurrentPlan = Convert.ToDecimal(CurrentSumAccumulativePlan.ToString("N2"));
                            DSModel.ToCurrentActual = Convert.ToDecimal(CurrentSumAccumulativeActual.ToString("N2"));
                            DSModel.ToCurrentDifference = Convert.ToDecimal((CurrentSumAccumulativeActual - CurrentSumAccumulativePlan).ToString("N2"));
                            //计算完成率(上一个月)
                            Lastmrsvm.ID = RateIndex;
                            Lastmrsvm.TargetID = itemt.ID;
                            Lastmrsvm.SystemID = itemt.SystemID;
                            Lastmrsvm.TargetName = itemt.TargetName;
                            Lastmrsvm.FinYear = Year;
                            Lastmrsvm.NPlanAmmount = (double)(DSModel.LastPlan);
                            Lastmrsvm.NActualAmmount = (double)(DSModel.LastActual);
                            Lastmrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(Lastmrsvm));
                            //计算完成率(当前月及其累计)
                            mrsvm.ID = RateIndex + 1;
                            mrsvm.TargetID = itemt.ID;
                            mrsvm.SystemID = itemt.SystemID;
                            mrsvm.TargetName = itemt.TargetName;
                            mrsvm.NPlanAmmount = (double)(DSModel.CurrentPlan);
                            mrsvm.NActualAmmount = (double)(DSModel.CurrentActual);
                            mrsvm.NAccumulativePlanAmmount = (double)(DSModel.ToCurrentPlan);
                            mrsvm.FinYear = Year;
                            mrsvm.NAccumulativeActualAmmount = (double)(DSModel.ToCurrentActual);
                            mrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                            //显示集合
                            list.Add(DSModel);
                        }
                        DSTargetArea.DetailList = list;
                        Showlist.Add(DSTargetArea);
                    }
                    if (i == 4)
                    {
                        List<ShowDSTargetCompletedDetail> Totallist = new List<ShowDSTargetCompletedDetail>();
                        foreach (C_Target itemt in TargetList.OrderBy(t => t.Sequence))
                        {
                            //计算完成率实体（上一个月）
                            MonthReportSummaryViewModel Lastmrsvm = new MonthReportSummaryViewModel();
                            //计算完成率实体（当前月及其累计）
                            MonthReportSummaryViewModel mrsvm = new MonthReportSummaryViewModel();
                            decimal LastSumPlan = 0;
                            decimal LastSumActual = 0;
                            decimal CurrentSumPlan = 0;
                            decimal CurrentSumActual = 0;
                            decimal CurrentSumAccumulativePlan = 0;
                            decimal CurrentSumAccumulativeActual = 0;
                            foreach (ShowDSTargetArea area in Showlist)
                            {
                                foreach (ShowDSTargetCompletedDetail item in area.DetailList)
                                {
                                    if (item.DetailAreaID != i && item.DetailTargetName == itemt.TargetName)
                                    {
                                        LastSumPlan += item.LastPlan;
                                        LastSumActual += item.LastActual;
                                        CurrentSumPlan += item.CurrentPlan;
                                        CurrentSumActual += item.CurrentActual;
                                        CurrentSumAccumulativePlan += item.ToCurrentPlan;
                                        CurrentSumAccumulativeActual += item.ToCurrentActual;
                                    }
                                }
                            }
                            ShowDSTargetCompletedDetail DSModel = new ShowDSTargetCompletedDetail();
                            DSModel.DetailAreaID = i;
                            DSModel.DetailTargetName = itemt.TargetName;
                            DSModel.LastPlan = Convert.ToDecimal(LastSumPlan.ToString("N2"));
                            DSModel.LastActual = Convert.ToDecimal(LastSumActual.ToString("N2"));
                            DSModel.LastDifference = Convert.ToDecimal((LastSumActual - LastSumPlan).ToString("N2"));
                            DSModel.CurrentPlan = Convert.ToDecimal(CurrentSumPlan.ToString("N2"));
                            DSModel.CurrentActual = Convert.ToDecimal(CurrentSumActual.ToString("N2"));
                            DSModel.CurrentDifference = Convert.ToDecimal((CurrentSumActual - CurrentSumPlan).ToString("N2"));
                            DSModel.ToCurrentPlan = Convert.ToDecimal(CurrentSumAccumulativePlan.ToString("N2"));
                            DSModel.ToCurrentActual = Convert.ToDecimal(CurrentSumAccumulativeActual.ToString("N2"));
                            DSModel.ToCurrentDifference = Convert.ToDecimal((CurrentSumAccumulativeActual - CurrentSumAccumulativePlan).ToString("N2"));
                            //计算完成率(上一个月)
                            Lastmrsvm.ID = RateIndex;
                            Lastmrsvm.TargetID = itemt.ID;
                            Lastmrsvm.SystemID = itemt.SystemID;
                            Lastmrsvm.TargetName = itemt.TargetName;
                            Lastmrsvm.FinYear = Year;
                            Lastmrsvm.NPlanAmmount = (double)(DSModel.LastPlan);
                            Lastmrsvm.NActualAmmount = (double)(DSModel.LastActual);
                            Lastmrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(Lastmrsvm));
                            //计算完成率(当前月及其累计)
                            mrsvm.ID = RateIndex + 1;
                            mrsvm.TargetID = itemt.ID;
                            mrsvm.SystemID = itemt.SystemID;
                            mrsvm.TargetName = itemt.TargetName;
                            mrsvm.FinYear = Year;
                            mrsvm.NPlanAmmount = (double)(DSModel.CurrentPlan);
                            mrsvm.NActualAmmount = (double)(DSModel.CurrentActual);
                            mrsvm.NAccumulativePlanAmmount = (double)(DSModel.ToCurrentPlan);
                            mrsvm.NAccumulativeActualAmmount = (double)(DSModel.ToCurrentActual);
                            mrsvm.MeasureRate = "1";
                            listMonthReportSummaryViewModel.Add(TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(mrsvm));
                            //显示集合
                            Totallist.Add(DSModel);
                        }
                        DSTargetArea.DetailList = Totallist;
                        Showlist.Add(DSTargetArea);
                    }
                }
            }
            //遍历集合修改完成率
            List<ShowDSTargetArea> sShowlist = new List<ShowDSTargetArea>();
            int d = 0;
            for (int b = 0; b < Showlist.Count; b++)
            {
                for (int c = 0; c < Showlist[b].DetailList.Count; c++)
                {
                    Showlist[b].DetailList[c].LastRate = listMonthReportSummaryViewModel[d].NActualRate;
                    d++;
                    Showlist[b].DetailList[c].CurrentRate = listMonthReportSummaryViewModel[d].NActualRate;
                    Showlist[b].DetailList[c].ToCurrentRate = listMonthReportSummaryViewModel[d].NAccumulativeActualRate;
                    d++;
                }
                sShowlist.Add(Showlist[b]);
            }
            return sShowlist;
        }
        /// <summary>
        /// 百货系统补回一季度经营指标缺口情况Excel下载
        /// </summary>
        /// <param name="error"></param>
        /// <param name="param"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public bool DownLoadDSTargetReturnDataReport(ref string error, string param, int FinYear, int FinMonth)
        {
            string fileName = String.Format("百货系统补回{0}月经营指标缺口情况({1}月)",FinMonth-1, FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标缺口情况.xls";
            if (FinMonth == 1)
            {
                fileName = String.Format("百货系统1月经营指标缺口情况({0}月)", FinMonth.ToString());
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标缺口情况(1月).xls";
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(fileStream);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);
            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0,false);
            Aspose.Cells.Style style2 = SetCellStyle(designer, true, 184, 204, 228,true);
            #endregion
            //表头
            if (FinMonth == 1)
            {
                sheet.Cells[2, 5].PutValue("1月累计");
            }
            else
            {
                string htitle = string.Format("报告期：{0}年1~{1}月", FinYear, FinMonth);
                sheet.Cells[1, 1].PutValue(htitle);
                sheet.Cells[2, 5].PutValue(string.Format("1-{0}月累计", FinMonth - 1));
                sheet.Cells[2, 8].PutValue(string.Format("1-{0}月累计", FinMonth));
            }
            List<DSTargetReturnDataCompany> list = GetDSTargetReturnDataList(FinYear, FinMonth, Convert.ToBoolean(param));
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).ToList();
                TargetList = TargetList.Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                List<DSTargetReturnDataCompany> AllReturnlist = list.Where(t => t.IsAllReturn == true).ToList();
                int rowindex = 5;
                int targetCount = TargetList.Count;
                int allIndex = 1;
                StyleFlag flg = new StyleFlag();
                flg.All = true;
                if (AllReturnlist.Count > 0)
                {
                    sheet.Cells[4, 1].PutValue(string.Format("一、完全补回{0}月指标缺口共计{1}家", FinMonth - 1, AllReturnlist.Count));
                    foreach (DSTargetReturnDataCompany dsc in AllReturnlist)
                    {
                        sheet.Cells[rowindex, 1].PutValue(allIndex);
                        sheet.Cells[rowindex, 2].PutValue(dsc.CompanyName);
                        Range range1 = sheet.Cells.CreateRange(rowindex, 1, targetCount, 1);
                        range1.Merge();
                        range1.ApplyStyle(style, flg);
                        Range range2 = sheet.Cells.CreateRange(rowindex, 2, targetCount, 1);
                        range2.Merge();
                        range2.ApplyStyle(style, flg);
                        foreach (DSTargetReturnData item in dsc.ReturnDataList)
                        {
                            int colIndex = 3;
                            sheet.Cells[rowindex, colIndex].PutValue(item.ReturnTargetName); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentReturnAmount); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativePlan); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativeActual); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativeDifference); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativePlan); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeActual); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeDifference); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeRate); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(string.Format("{0:yyyy-MM-dd}", item.CommitDate)); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.ReturnTypeDescrible); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            //sheet.Cells[rowindex, colIndex].PutValue(item.Counter.ToString()); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].SetStyle(style); 
                            if (item.Counter > 0)
                            {
                                int pictureIndex = sheet.Pictures.Add(rowindex, colIndex, ImageFilePath + "\\image" + item.Counter + ".png");
                                Aspose.Cells.Drawing.Picture picture = sheet.Pictures[pictureIndex];
                                picture.Left = 33;
                                picture.Top = 5;
                            }
                           
                            rowindex++;
                        }
                        allIndex++;
                    }
                }
                else
                {
                    sheet.Cells[4, 1].PutValue("一、完全补回指标缺口共计0家");
                }
                List<DSTargetReturnDataCompany> PartReturnlist = list.Where(t => t.IsAllReturn == false).ToList();
                if (PartReturnlist.Count > 0)
                {
                    Range range = sheet.Cells.CreateRange(rowindex, 1, 1, 14);
                    if (FinMonth == 1)
                    {
                        range = sheet.Cells.CreateRange(rowindex, 1, 1, 11);
                    }
                    range.Merge();
                    range.ApplyStyle(style2, flg);
                    sheet.Cells.Rows[rowindex].Height = 44.25;
                    sheet.Cells[rowindex, 1].PutValue(string.Format("二、部分补回{0}月指标缺口共{1}家", FinMonth - 1, PartReturnlist.Count));
                    rowindex++;
                    int partIndex = 1;
                    foreach (DSTargetReturnDataCompany dsc in PartReturnlist)
                    {
                        sheet.Cells[rowindex, 1].PutValue(partIndex);
                        sheet.Cells[rowindex, 2].PutValue(dsc.CompanyName);
                        Range range1 = sheet.Cells.CreateRange(rowindex, 1, targetCount, 1);
                        range1.Merge();
                        range1.ApplyStyle(style, flg);
                        Range range2 = sheet.Cells.CreateRange(rowindex, 2, targetCount, 1);
                        range2.Merge();
                        range2.ApplyStyle(style, flg);
                        foreach (DSTargetReturnData item in dsc.ReturnDataList)
                        {
                            int colIndex = 3;
                            sheet.Cells[rowindex, colIndex].PutValue(item.ReturnTargetName); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentReturnAmount); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativePlan); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativeActual); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativeDifference); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativePlan); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeActual); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeDifference); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeRate); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(string.Format("{0:yyyy-MM-dd}", item.CommitDate)); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.ReturnTypeDescrible); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].SetStyle(style); 
                            if (item.Counter > 0)
                            {
                                int pictureIndex = sheet.Pictures.Add(rowindex, colIndex, ImageFilePath + "\\image" + item.Counter + ".png");
                                Aspose.Cells.Drawing.Picture picture = sheet.Pictures[pictureIndex];
                                picture.Left = 33;
                                picture.Top = 5;
                            }
                           
                            //sheet.Cells[rowindex, colIndex].PutValue(item.Counter.ToString()); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            rowindex++;
                        }
                        partIndex++;
                    }
                }
                else
                {
                    Range range = sheet.Cells.CreateRange(rowindex, 1, 1, 14);
                    if (FinMonth == 1)
                    {
                        range = sheet.Cells.CreateRange(rowindex, 1, 1, 11);
                    }
                    range.Merge();
                    range.ApplyStyle(style2, flg);
                    sheet.Cells.Rows[rowindex].Height = 44.25;
                    sheet.Cells[rowindex, 1].PutValue("二、部分补回指标缺口共计0家");
                }
            }
            //保存excel附件
            SaveExcelFiles(designer, fileStream, fileName);
            return true;
        }


        /// <summary>
        /// 百货系统经营指标补回情况
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        
        public List<DSTargetReturnDataCompany> GetDSTargetReturnDataList(int Year, int Month, bool IsLatestVersion)
        {
            List<DSTargetReturnDataCompany> Showlist = new List<DSTargetReturnDataCompany>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                List<C_Company> CompanyList = new List<C_Company>();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();
                List<MonthlyReportDetail> LastMonthReportDetails = null;

                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthReportDetails = LastMonthReport.ReportDetails;//.Where(p => p.ReturnType >= (int)EnumReturnType.Accomplish).ToList();  
                    CompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemID(Year, Month - 1, SystemModel.ID, 1, IsLatestVersion).ToList();
                }

                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthReportDetails = CurrentMonthReport.ReportDetails;//.Where(p => p.ReturnType >= (int)EnumReturnType.Accomplish).ToList();

                //新建百货系统经营指标补回情况的集合
                if (CompanyList != null && CompanyList.Count > 0)
                {
                    int i = 1;
                    foreach (C_Company c in CompanyList)
                    {
                        DSTargetReturnDataCompany DSCompany = new DSTargetReturnDataCompany();
                        DSCompany.ID = i;
                        DSCompany.CompanyName = c.CompanyName;
                        int isIsMissTargetCount = 0;
                        int isReturnCount = 0;

                        List<DSTargetReturnData> dataList = new List<DSTargetReturnData>();
                        foreach (C_Target ct in TargetList.OrderBy(t => t.Sequence))
                        {
                            DSTargetReturnData DSReturnData = new DSTargetReturnData();
                            DSReturnData.CompanyID = i;
                            DSReturnData.ReturnTargetName = ct.TargetName;
                            //上个月
                            MonthlyReportDetail LastDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, false);
                            if (LastDetail != null && LastDetail.ID != Guid.Empty)
                            {
                                DSReturnData.LastAccumulativePlan = Convert.ToDecimal(LastDetail.NAccumulativePlanAmmount.ToString("N2"));
                                DSReturnData.LastAccumulativeActual = Convert.ToDecimal(LastDetail.NAccumulativeActualAmmount.ToString("N2"));
                                DSReturnData.LastAccumulativeDifference = Convert.ToDecimal((LastDetail.NAccumulativeActualAmmount - LastDetail.NAccumulativePlanAmmount).ToString("N2"));
                            }
                            //当前月
                            MonthlyReportDetail CurrentDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, true);
                            if (CurrentDetail != null && CurrentDetail.ID != Guid.Empty)
                            {
                                DSReturnData.CurrentReturnAmount = Convert.ToDecimal((CurrentDetail.NAccumulativeActualAmmount - CurrentDetail.NAccumulativePlanAmmount).ToString("N2"));
                                DSReturnData.CurrentAccumulativePlan = Convert.ToDecimal(CurrentDetail.NAccumulativePlanAmmount.ToString("N2"));
                                DSReturnData.CurrentAccumulativeActual = Convert.ToDecimal(CurrentDetail.NAccumulativeActualAmmount.ToString("N2"));
                                DSReturnData.CurrentAccumulativeDifference = Convert.ToDecimal((CurrentDetail.NAccumulativeActualAmmount - CurrentDetail.NAccumulativePlanAmmount).ToString("N2"));
                                DSReturnData.CurrentAccumulativeRate = CurrentDetail.NAccumulativeDisplayRate.ToString();
                                //if (string.IsNullOrEmpty(CurrentDetail.NAccumulativeDisplayRate))
                                //{
                                //    DSReturnData.CurrentAccumulativeRate = ((CurrentDetail.NAccumulativeActualRate) * 100).ToString("N2") + "%";
                                //}
                                //else
                                //{
                                //    DSReturnData.CurrentAccumulativeRate = CurrentDetail.NAccumulativeDisplayRate.ToString() + ((CurrentDetail.NAccumulativeActualRate) * 100).ToString("N2") + "%";
                                //}
                                DSReturnData.CommitDate = string.Format("{0:yyyy-MM-dd}", CurrentDetail.CommitDate);
                                DSReturnData.ReturnType = CurrentDetail.ReturnType;
                                if (!string.IsNullOrEmpty(CurrentDetail.ReturnType.ToString()) && CurrentDetail.ReturnType > 0)
                                {
                                    DSReturnData.ReturnTypeDescrible = EnumUtil.GetEnumDescription(typeof(EnumReturnType), CurrentDetail.ReturnType);
                                }
                                else
                                {
                                    DSReturnData.ReturnTypeDescrible = "--";
                                }
                                DSReturnData.Counter = CurrentDetail.Counter;

                                if (CurrentDetail.ReturnType >= (int)EnumReturnType.Accomplish)
                                {
                                    isReturnCount++;
                                }

                                if (CurrentDetail.IsMissTarget == false)
                                {
                                    isIsMissTargetCount++;
                                }
                            }
                            dataList.Add(DSReturnData);
                        }

                        if (isIsMissTargetCount > 0)
                        {
                            if (isIsMissTargetCount == TargetList.Count)
                            {
                                if (isReturnCount > 0)
                                {
                                    DSCompany.IsAllReturn = true;
                                    DSCompany.ReturnDataList = dataList;
                                    Showlist.Add(DSCompany);
                                }
                            }
                            else
                            {
                                if (isReturnCount > 0)
                                {
                                    DSCompany.IsAllReturn = false;
                                    DSCompany.ReturnDataList = dataList;
                                    Showlist.Add(DSCompany);
                                }
                            }

                            i++;
                        }
                    }
                }
            }
            return Showlist;
        }


        /// <summary>
        /// 百货系统一季度经营指标缺口新增情况Excel下载
        /// </summary>
        /// <param name="error"></param>
        /// <param name="param"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public bool DownLoadDSTargetAddDataReport(ref string error, string param, int FinYear, int FinMonth)
        {
            string fileName = String.Format("百货系统{0}月经营指标新增情况({1}月)", FinMonth.ToString(), FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标新增情况.xls";
            if (FinMonth == 1)
            {
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标新增情况(1月).xls";
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(fileStream);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);
            //表头
            if (FinMonth == 1)
            {
                sheet.Cells[1, 1].PutValue(string.Format("报告期：{0}年1月", FinYear));
                sheet.Cells[2, 5].PutValue("1月累计");
            }
            else
            {
                string htitle = string.Format("报告期：{0}年1~{1}月", FinYear, FinMonth);
                sheet.Cells[1, 1].PutValue(htitle);
                sheet.Cells[2, 5].PutValue(string.Format("1-{0}月累计", FinMonth - 1));
                sheet.Cells[2, 8].PutValue(string.Format("1-{0}月累计", FinMonth));
            }
            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0,false);
            Aspose.Cells.Style style2 = SetCellStyle(designer, true, 184, 204, 228,true);
            #endregion
            List<DSTargetReturnDataCompany> list = GetDSTargetAddMissDataList(FinYear, FinMonth, Convert.ToBoolean(param));
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).ToList();
                TargetList = TargetList.Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                int rowindex = 4;
                int targetCount = TargetList.Count;
                int targetIndex = 1;
                StyleFlag flg = new StyleFlag();
                flg.All = true;
                foreach (C_Target c in TargetList.OrderBy(t => t.Sequence))
                {
                    List<DSTargetReturnDataCompany> AddTargetlist = list.Where(t => t.AddTargetName == c.TargetName).ToList();
                    Range range = sheet.Cells.CreateRange(rowindex, 1, 1, 14);
                    if (FinMonth == 1)
                    {
                        range = sheet.Cells.CreateRange(rowindex, 1, 1, 11);
                    }
                    range.Merge();
                    range.ApplyStyle(style2, flg);
                    sheet.Cells.Rows[rowindex].Height = 44.25;
                    sheet.Cells[rowindex, 1].PutValue(string.Format("{0}、新增{1}未完成门店共计{2}家。", targetIndex, c.TargetName, AddTargetlist.Count)); rowindex++;
                    int allIndex = 1;
                    foreach (DSTargetReturnDataCompany dsc in AddTargetlist)
                    {
                        sheet.Cells[rowindex, 1].PutValue(allIndex);
                        sheet.Cells[rowindex, 2].PutValue(dsc.CompanyName);
                        Range range1 = sheet.Cells.CreateRange(rowindex, 1, targetCount, 1);
                        range1.Merge();
                        range1.ApplyStyle(style, flg);
                        Range range2 = sheet.Cells.CreateRange(rowindex, 2, targetCount, 1);
                        range2.Merge();
                        range2.ApplyStyle(style, flg);
                        foreach (DSTargetReturnData item in dsc.ReturnDataList)
                        {
                            int colIndex = 3;
                            sheet.Cells[rowindex, colIndex].PutValue(item.ReturnTargetName); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentReturnAmount); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            if (FinMonth > 1)
                            {
                                sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativePlan); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                                sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativeActual); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                                sheet.Cells[rowindex, colIndex].PutValue(item.LastAccumulativeDifference); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            }
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativePlan); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeActual); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeDifference); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.CurrentAccumulativeRate); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(string.Format("{0:yyyy-MM-dd}", item.CommitDate)); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].PutValue(item.ReturnTypeDescrible); sheet.Cells[rowindex, colIndex].SetStyle(style); colIndex++;
                            sheet.Cells[rowindex, colIndex].SetStyle(style); 
                            if (item.Counter > 0)
                            {
                                int pictureIndex = sheet.Pictures.Add(rowindex, colIndex, ImageFilePath + "\\image" + item.Counter + ".png");
                                Aspose.Cells.Drawing.Picture picture = sheet.Pictures[pictureIndex];
                                picture.Left = 33;
                                picture.Top = 7;
                            }
                            rowindex++;
                        }
                        allIndex++;
                    }
                    targetIndex++;
                }
            }
            //保存excel附件
            SaveExcelFiles(designer, fileStream, fileName);
            return true;
        }



        /// <summary>
        /// 百货系统经营指标新增未完成指标的门店情况
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public List<DSTargetReturnDataCompany> GetDSTargetAddMissDataList(int Year, int Month, bool IsLatestVersion)
        {
            List<DSTargetReturnDataCompany> Showlist = new List<DSTargetReturnDataCompany>();
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID,DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
                //上一个月以及当前月的信息
                ReportInstance LastMonthReport = new ReportInstance();
                List<MonthlyReportDetail> LastMonthReportDetails = null;

                if (Month > 1)
                {
                    LastMonthReport = new ReportInstance(SystemModel.ID, Year, Month - 1, IsLatestVersion);
                    LastMonthReportDetails = LastMonthReport.ReportDetails;
                }

                ReportInstance CurrentMonthReport = new ReportInstance(SystemModel.ID, Year, Month, IsLatestVersion);
                List<MonthlyReportDetail> CurrentMonthReportDetails = CurrentMonthReport.ReportDetails;

                //新建百货系统新增经营指标未完成情况的集合
                int i = 1;
                foreach (C_Target titem in TargetList.OrderBy(t => t.Sequence))
                {
                    List<C_Company> CompanyList = new List<C_Company>();
                    if (Month == 1)
                    {
                        CompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemIDAndTargetID(Year, Month, SystemModel.ID, 1, titem.ID, IsLatestVersion).ToList();
                    }
                    else
                    {
                        List<C_Company> LastCompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemIDAndTargetID(Year, Month - 1, SystemModel.ID, 0, titem.ID, IsLatestVersion).ToList();
                        List<C_Company> CurrentCompanyList = C_CompanyOperator.Instance.GetCompanyListBySystemIDAndTargetID(Year, Month, SystemModel.ID, 1, titem.ID, IsLatestVersion).ToList();
                        foreach (C_Company cc in LastCompanyList)
                        {
                            C_Company cModel = CurrentCompanyList.SingleOrDefault(t => t.ID == cc.ID);
                            if (cModel != null && cModel.ID != Guid.Empty)
                            {
                                CompanyList.Add(cModel);
                            }
                        }
                    }

                    #region 添加公司

                    if (CompanyList != null && CompanyList.Count > 0)
                    {
                        foreach (C_Company c in CompanyList)
                        {
                            DSTargetReturnDataCompany DSCompany = new DSTargetReturnDataCompany();
                            DSCompany.ID = i;
                            DSCompany.CompanyName = c.CompanyName;
                            DSCompany.AddTargetName = titem.TargetName;
                            List<DSTargetReturnData> dataList = new List<DSTargetReturnData>();

                            #region 根据指标


                            foreach (C_Target ct in TargetList.OrderBy(t => t.Sequence))
                            {
                                DSTargetReturnData DSReturnData = new DSTargetReturnData();
                                DSReturnData.CompanyID = i;
                                DSReturnData.ReturnTargetName = ct.TargetName;
                                MonthlyReportDetail LastDetail = null;
                                if (Month == 1)
                                    LastDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, true);
                                else
                                    LastDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, false);

                                if (LastDetail != null && LastDetail.ID != Guid.Empty)
                                {
                                    DSReturnData.LastAccumulativePlan = Convert.ToDecimal(LastDetail.NAccumulativePlanAmmount.ToString("N2"));
                                    DSReturnData.LastAccumulativeActual = Convert.ToDecimal(LastDetail.NAccumulativeActualAmmount.ToString("N2"));
                                    DSReturnData.LastAccumulativeDifference = Convert.ToDecimal((LastDetail.NAccumulativeDifference).ToString("N2"));
                                }
                                MonthlyReportDetail CurrentDetail = GetMonthlyReportDetail(c, LastMonthReportDetails, CurrentMonthReportDetails, ct, true);

                                if (CurrentDetail != null && CurrentDetail.ID != Guid.Empty)
                                {
                                    DSReturnData.CurrentReturnAmount = Convert.ToDecimal((CurrentDetail.NAccumulativeDifference - LastDetail.NAccumulativeDifference).ToString("N2"));
                                    DSReturnData.CurrentAccumulativePlan = Convert.ToDecimal(CurrentDetail.NAccumulativePlanAmmount.ToString("N2"));
                                    DSReturnData.CurrentAccumulativeActual = Convert.ToDecimal(CurrentDetail.NAccumulativeActualAmmount.ToString("N2"));
                                    DSReturnData.CurrentAccumulativeDifference = Convert.ToDecimal((CurrentDetail.NAccumulativeDifference).ToString("N2"));
                                    DSReturnData.CurrentAccumulativeRate = CurrentDetail.NAccumulativeDisplayRate.ToString();
                                    DSReturnData.CommitDate = string.Format("{0:yyyy-MM-dd}", CurrentDetail.CommitDate);
                                    DSReturnData.ReturnType = CurrentDetail.ReturnType;

                                    if (!string.IsNullOrEmpty(CurrentDetail.ReturnType.ToString()) && CurrentDetail.ReturnType > 0)
                                    {
                                        DSReturnData.ReturnTypeDescrible = EnumUtil.GetEnumDescription(typeof(EnumReturnType), CurrentDetail.ReturnType);
                                    }
                                    else
                                    {
                                        DSReturnData.ReturnTypeDescrible = "--";
                                    }
                                    DSReturnData.Counter = CurrentDetail.Counter;
                                }
                                else
                                {
                                    DSReturnData.CurrentAccumulativeRate = "--";
                                    DSReturnData.ReturnTypeDescrible = "--";
                                    DSReturnData.CommitDate = "--";
                                    DSReturnData.Counter = 0;
                                }


                                dataList.Add(DSReturnData);
                            }

                            #endregion

                            if (dataList.Where(p => p.ReturnType == (int)EnumReturnType.New).ToList().Count > 0)
                            {
                                DSCompany.ReturnDataList = dataList;
                                Showlist.Add(DSCompany);
                            }
                            i++;
                        }
                    }

                    #endregion

                }
            }
            return Showlist;
        }
        
    }
}