
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
    /// DownExcelCommitDate1 的摘要说明
    /// </summary>
    public class DownExcelCommitDate1 : IHttpHandler, IRequiresSessionState
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
        int FinYear = 0;
        int FinMonth = 0;
        string ImageFilePath = HttpContext.Current.Server.MapPath("../Images/images1");
        string FileType = string.Empty; //下载的文件类型
        string fileName = "要求补回期限表";
        C_System SystemModel = new C_System();
        DateTime T = DateTime.Now;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
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

            List<A_MonthlyReportDetail> am = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(SysId, FinYear, FinMonth).ToList();
         
            if (am.Count > 0)
            {
                B_MonthlyReport BM = B_MonthlyreportOperator.Instance.GetMonthlyreport(am[0].MonthlyReportID);
                T = BM.CreateTime;
            }
            SystemModel = StaticResource.Instance[SysId, T];
            if (SystemModel.Category == 4)
            {
                DownExcelCate2();
            }
            else
            {
                DownExcelCate1();
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private void DownExcelCate1()
        {
            List<MonthlyReportDetail> IsMissList = A_MonthlyreportdetailOperator.Instance.GetVMissDetail(SysId, FinYear, FinMonth).ToList();

            List<C_Target> NewTarget = StaticResource.Instance.GetTargetList(SysId, T).Where(p=>p.NeedEvaluation==true).ToList();

         

            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "补回期限管理模板V1.xls");

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            #region 样式
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            #region  style1
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            style1.Font.IsBold = true;
            style1.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style1.Pattern = BackgroundType.Solid;
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
            #region  style2
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
            #region  style4

            style4.Font.Size = 10;
            style4.Font.Name = "Arial";
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
            style4.IsTextWrapped = true;
            #endregion
            #region  style3
            style3.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style3.IsTextWrapped = true;
            #endregion
            #endregion
            int rowStart = 3;
            int colStart = 0; // 开始列
            for (int i = 0; i < NewTarget.Count; i++)
            {
                worksheets.AddCopy(0);
            }

            for (int i = 0; i < NewTarget.Count; i++)
            {


                excel.SetCustomProperty(worksheets[i], "SystemID", NewTarget[i].SystemID.ToString());
                excel.SetCustomProperty(worksheets[i], "TargetID", NewTarget[i].ID.ToString());
                excel.SetCustomProperty(worksheets[i], "TargetName", NewTarget[i].TargetName);
                excel.SetCustomProperty(worksheets[i], "SheetName", "CommitUpdate");
                excel.SetCustomProperty(worksheets[i], "Time", FinYear + "-" + FinMonth);

                worksheets[i].Name = NewTarget[i].TargetName;
                worksheets[i].Cells[0, 1].PutValue(SystemModel.SystemName + NewTarget[i].TargetName);
                worksheets[i].Cells[1, 1].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[i].Cells[1, 6].PutValue("单位:"+NewTarget[i].Unit);
                int NO = 0;
                for (int j = 0; j < IsMissList.Count; j++)
                {

                    if (NewTarget[i].ID == IsMissList[j].TargetID)
                    {

                        NO = NO + 1;
                        worksheets[i].Cells[rowStart, colStart + 1].SetStyle(style1);
                        worksheets[i].Cells[rowStart, colStart + 1].PutValue(NO);
                        worksheets[i].Cells[rowStart, colStart + 2].SetStyle(style1);
                        worksheets[i].Cells[rowStart, colStart + 2].PutValue(IsMissList[j].CompanyName);
                        worksheets[i].Cells[rowStart, colStart + 3].SetStyle(style4);
                        //IsMissList[j].ReturnTypeDescription = EnumHelper.GetEnumDescription(typeof(EnumReturnType), IsMissList[j].ReturnType);
                        //worksheets[i].Cells[rowStart, colStart + 3].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), IsMissList[j].ReturnType) + "\n" + IsMissList[j].ReturnDescription);
                        //worksheets[i].Cells[rowStart, colStart + 3].PutValue(IsMissList[i].PromissDate.Value.Year + "/" + IsMissList[i].PromissDate.Value.Month + "/" + IsMissList[i].PromissDate.Value.Day);
                        if (IsMissList[j].PromissDate != null)
                        {
                            worksheets[i].Cells[rowStart, colStart + 3].PutValue(IsMissList[j].PromissDate.Value.Year + "/" + IsMissList[j].PromissDate.Value.Month);

                        }
                        else
                        {
                            worksheets[i].Cells[rowStart, colStart + 3].PutValue("--");
                        }
                        worksheets[i].Cells[rowStart, colStart + 4].SetStyle(style2);
                        if (IsMissList[j].CurrentMonthCommitDate != null)
                        {
                            worksheets[i].Cells[rowStart, colStart + 4].PutValue(IsMissList[j].CurrentMonthCommitDate.Value.Year + "/" + IsMissList[j].CurrentMonthCommitDate.Value.Month + "/" + IsMissList[j].CurrentMonthCommitDate.Value.Day);

                        }
                        worksheets[i].Cells[rowStart, colStart + 5].SetStyle(style3);
                        worksheets[i].Cells[rowStart, colStart + 5].PutValue(IsMissList[j].CurrentMonthCommitReason);
                        worksheets[i].Cells[rowStart, colStart + 7].PutValue(IsMissList[j].CompanyID);
                        worksheets[i].Cells[rowStart, colStart + 6].SetStyle(style1);
                        if (IsMissList[j].Counter > 0)
                        {
                            int pictureIndex = worksheets[i].Pictures.Add(rowStart, colStart + 6, ImageFilePath + "\\image" + IsMissList[j].Counter + ".png");
                            Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                            picture.Left = 60;
                            picture.Top = 10;
                        }
                        rowStart = rowStart + 1;
                    }

                }
                rowStart = 3;
            }
            worksheets.RemoveAt(NewTarget.Count);
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(SystemModel.SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();


        }

        private void DownExcelCate2()
        {
            IList<MonthlyReportDetail> IsMissList = A_MonthlyreportdetailOperator.Instance.GetVMissDetail(SysId, FinYear, FinMonth).ToList();
            for (int a = 0; a < IsMissList.Count; a++)
            {
                IsMissList[a].CompanyName = C_TargetOperator.Instance.GetTarget(IsMissList[a].TargetID).TargetName;
            }
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "补回期限管理模板V1.xls");
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;
            #region 样式
            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            #region  style1
            style1.Font.Size = 12;
            style1.Font.Name = "Arial";
            style1.Font.IsBold = true;
            style1.ForegroundColor = System.Drawing.Color.FromArgb(184, 204, 228);
            style1.Pattern = BackgroundType.Solid;
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
            #region  style2
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
            #region  style4
            style4.Font.Size = 12;
            style4.Font.Name = "Arial";
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
            #region  style3
            style3.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style3.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style3.IsTextWrapped = true;
            #endregion
            #endregion
            int rowStart = 3;
            int colStart = 0; // 开始列
            excel.SetCustomProperty(worksheets[0], "SystemID", SystemModel.ID.ToString());
            excel.SetCustomProperty(worksheets[0], "SheetName", "CommitUpdate");
            excel.SetCustomProperty(worksheets[0], "Time", FinYear + "-" + FinMonth);
            worksheets[0].Name = SystemModel.SystemName;
            worksheets[0].Cells[0, 1].PutValue(SystemModel.SystemName + "补回期限填报");
            worksheets[0].Cells[1, 1].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
            int NO = 0;
            for (int i = 0; i < IsMissList.Count; i++)
            {
                NO = NO + 1;
                worksheets[0].Cells[rowStart, colStart + 1].SetStyle(style1);
                worksheets[0].Cells[rowStart, colStart + 1].PutValue(NO);
                worksheets[0].Cells[rowStart, colStart + 2].SetStyle(style1);
                worksheets[0].Cells[rowStart, colStart + 2].PutValue(IsMissList[i].CompanyName);
                worksheets[0].Cells[rowStart, colStart + 3].SetStyle(style4);
                if (IsMissList[i].PromissDate != null)
                {
                    worksheets[0].Cells[rowStart, colStart + 3].PutValue(IsMissList[i].PromissDate.Value.Year + "/" + IsMissList[i].PromissDate.Value.Month + "/" + IsMissList[i].PromissDate.Value.Day);

                }
                else
                {
                    worksheets[0].Cells[rowStart, colStart + 3].PutValue("--");
                }
                worksheets[0].Cells[rowStart, colStart + 4].SetStyle(style2);
                if (IsMissList[i].CurrentMonthCommitDate != null)
                {
                    worksheets[0].Cells[rowStart, colStart + 4].PutValue(IsMissList[i].CurrentMonthCommitDate.Value.Year + "/" + IsMissList[i].CurrentMonthCommitDate.Value.Month + "/" + IsMissList[i].CurrentMonthCommitDate.Value.Day);

                }
                worksheets[0].Cells[rowStart, colStart + 5].SetStyle(style3);
                worksheets[0].Cells[rowStart, colStart + 5].PutValue(IsMissList[i].CurrentMonthCommitReason);
                worksheets[0].Cells[rowStart, colStart + 7].PutValue(IsMissList[i].CompanyID);
                worksheets[0].Cells[rowStart, colStart + 8].PutValue(IsMissList[i].TargetID);
                worksheets[0].Cells[rowStart, colStart + 6].SetStyle(style1);
                if (IsMissList[i].Counter > 0)
                {
                    int pictureIndex = worksheets[0].Pictures.Add(rowStart, colStart + 6, ImageFilePath + "\\image" + IsMissList[i].Counter + ".png");
                    Aspose.Cells.Drawing.Picture picture = worksheets[0].Pictures[pictureIndex];
                    picture.Left = 60;
                    picture.Top = 10;
                }
                rowStart = rowStart + 1;
            }
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(SystemModel.SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.End();
        }

    }
}