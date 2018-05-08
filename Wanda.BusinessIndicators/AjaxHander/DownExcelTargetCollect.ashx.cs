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
    /// DownExcelTargetCollect 的摘要说明
    /// </summary>
    public class DownExcelTargetCollect : IHttpHandler, IRequiresSessionState
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
        string FinYear = string.Empty;
        string FileType = string.Empty; //下载的文件类型
        string fileName = "年度经营指标月度分解表汇总表";
        Guid TargetPlanID = Guid.Empty;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysId"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SysId"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear = HttpContext.Current.Request["FinYear"];
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["TargetPlanID"]))
            {
                TargetPlanID = Guid.Parse(HttpContext.Current.Request["TargetPlanID"]);
            }
            DownExcel();
        }
        private void DownExcel()
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "指标汇总V1.xls");
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
            #region 指标标题样式
            Aspose.Cells.Style StyleTargetTitle = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            StyleTargetTitle.Font.Size = 22;
            StyleTargetTitle.Font.Name = "黑体";
            StyleTargetTitle.Font.IsBold = true;
            StyleTargetTitle.HorizontalAlignment = TextAlignmentType.Center;
            StyleTargetTitle.VerticalAlignment = TextAlignmentType.Center;
            StyleTargetTitle.Pattern = BackgroundType.Solid;

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
            #endregion

            C_System SysModel = C_SystemOperator.Instance.GetSystem(SysId);
            // List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SysId).ToList();
           // List<TargetPlanDetailVList> TargetPlanDetailVList = A_TargetplandetailOperator.Instance.GetSumTargetDetail(int.Parse(FinYear), SysId);
            List<TargetPlanDetailVList> TargetPlanDetailVList = A_TargetplandetailOperator.Instance.GetSumTargetDetail(int.Parse(FinYear), SysId,TargetPlanID);
            
            //复制sheet
            for (int i = 0; i < TargetPlanDetailVList.Count; i++)
            {
                worksheets.AddCopy(1);
                //worksheets[i + 1].Name = TargetList[i].TargetName;
            }


            #region 汇总表文档下载



            string Titles = SysModel.SystemName + "经营指标分解汇总表";//表头名称

            worksheets[0].Cells.SetRowHeight(0, 50);//表格第一行放表头 所以第一行行高增加到50
            for (int i = 1; i <= 16; i++)//除了第一行,汇总表所有用到的行行高均为20
            {
                worksheets[0].Cells.SetRowHeight(i, 20);
            }
            worksheets[0].Cells.Merge(0, 1, 1, (TargetPlanDetailVList.Count) * 2 + 1);//为标题栏合并列,所需要合并列的数量为 指标数量*2+1
            worksheets[0].Cells[0, 1].PutValue(Titles);
            worksheets[0].Cells[0, 1].SetStyle(StyleTitle);


            //为表格设置列宽

            for (int i = 1; i <= (TargetPlanDetailVList.Count) * 2; i++)
            {
                worksheets[0].Cells.SetColumnWidth(i + 1, 20);
            }



            worksheets[0].Cells[1, 1].PutValue("年度：" + FinYear);
            worksheets[0].Cells[1, 1].SetStyle(StyleTitle2);
         //   worksheets[0].Cells[1, (TargetPlanDetailVList.Count) * 2 + 1].PutValue("单位：万元");
          //  worksheets[0].Cells[1, (TargetPlanDetailVList.Count) * 2 + 1].SetStyle(StyleTitle2);


            //创建表头
            worksheets[0].Cells.Merge(2, 1, 2, 1);
            worksheets[0].Cells.Merge(2, 2, 1, TargetPlanDetailVList.Count);//合并列
            worksheets[0].Cells.Merge(2, 2 + TargetPlanDetailVList.Count, 1, TargetPlanDetailVList.Count);//合并列
            worksheets[0].Cells[2, 1].PutValue("月份");
            worksheets[0].Cells[2, 1].SetStyle(StyleTitle3);
            worksheets[0].Cells[3, 1].SetStyle(StyleTitle3);
            worksheets[0].Cells[2, 2].PutValue("当月数");
            worksheets[0].Cells[2, 2 + TargetPlanDetailVList.Count].PutValue("累计数");
            //worksheets[0].Cells[2, 2].SetStyle(StyleTitle3);
            //worksheets[0].Cells[2, 2 + TargetList.Count].SetStyle(StyleTitle3);

            int Nowcol = 2;
            int Allcol = TargetPlanDetailVList.Count + 2;
            //为表头赋值(指标名称)
            for (int i = 0; i < TargetPlanDetailVList.Count; i++)
            {
                worksheets[0].Cells[3, Nowcol + i].PutValue(TargetPlanDetailVList[i].TargetName);
                worksheets[0].Cells[3, Allcol + i].PutValue(TargetPlanDetailVList[i].TargetName);
                worksheets[0].Cells[2, Nowcol + i].SetStyle(StyleTitle3);
                worksheets[0].Cells[2, Allcol + i].SetStyle(StyleTitle3);
                worksheets[0].Cells[3, Nowcol + i].SetStyle(StyleTitle3);
                worksheets[0].Cells[3, Allcol + i].SetStyle(StyleTitle3);
            }


            //给表格内部赋值
            //List<TargetDetail> TargetDetail = A_TargetplandetailOperator.Instance.GetSumMonthTargetDetail(int.Parse(FinYear), SysId);
            List<TargetDetail> TargetDetail = A_TargetplandetailOperator.Instance.GetSumMonthTargetDetailByTID(TargetPlanID);

            for (int a = 0; a < 12; a++)
            {
                worksheets[0].Cells[4 + a, 1].PutValue(a + 1 + "月");
                worksheets[0].Cells[4 + a, 1].SetStyle(monthstyle);
            }
            for (int j = 0; j < TargetPlanDetailVList.Count; j++) 
            {
                if (j + 1 <= TargetDetail[0].TargetDetailList.Count)
                {
                    for (int a = 0; a < TargetDetail.Count; a++)
                    {
                        if (TargetDetail[a].TargetDetailList[j].Target != null)
                        {
                            worksheets[0].Cells[4 + a, Nowcol + j].PutValue(TargetDetail[a].TargetDetailList[j].Target);
                            worksheets[0].Cells[4 + a, Nowcol + j].SetStyle(RowStyle);
                        }
                        else
                        {
                            worksheets[0].Cells[4 + a, Nowcol + j].PutValue("--");
                            worksheets[0].Cells[4 + a, Nowcol + j].SetStyle(RowStyle);
                        }
                        if (TargetDetail[a].TargetDetailList[j].SumTarget != null)
                        {
                            worksheets[0].Cells[4 + a, Allcol + j].PutValue(TargetDetail[a].TargetDetailList[j].SumTarget);
                            worksheets[0].Cells[4 + a, Allcol + j].SetStyle(RowStyle);
                        }
                        else
                        {
                            worksheets[0].Cells[4 + a, Allcol + j].PutValue("--");
                            worksheets[0].Cells[4 + a, Allcol + j].SetStyle(RowStyle);
                        }
                    }
                }
                else
                {
                    for (int s = 0; s < 12; s++)
                    {
                        worksheets[0].Cells[4 + s, Nowcol + j].PutValue("--");
                        worksheets[0].Cells[4 + s, Allcol + j].PutValue("--");
                        worksheets[0].Cells[4 + s, Nowcol + j].SetStyle(RowStyle);
                        worksheets[0].Cells[4 + s, Allcol + j].SetStyle(RowStyle);
                    }
                }
            }
            worksheets[0].Cells[16, 1].PutValue("合计");
            worksheets[0].Cells[16, 1].SetStyle(hejistyle);
            for (int j = 0; j < TargetPlanDetailVList.Count; j++)
            {
                if (j + 1 <= TargetDetail[0].TargetDetailList.Count)
                {
                    if (TargetDetail[11].TargetDetailList[j].SumTarget != null)
                    {
                        worksheets[0].Cells[16, j + 2].PutValue(TargetDetail[11].TargetDetailList[j].SumTarget);
                        worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].PutValue("--");

                        worksheets[0].Cells[16, j + 2].SetStyle(RowStyle);
                        worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].SetStyle(RowStyle);

                        

                    }
                    else
                    {
                        worksheets[0].Cells[16, j + 2].PutValue("--");
                        worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].PutValue("--");

                        worksheets[0].Cells[16, j + 2].SetStyle(RowStyle);
                        worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].SetStyle(RowStyle);
                    }
                }
                else
                {
                    worksheets[0].Cells[16, j + 2].PutValue("--");
                    worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].PutValue("--");

                    worksheets[0].Cells[16, j + 2].SetStyle(RowStyle);
                    worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].SetStyle(RowStyle);

                }
                worksheets[0].Cells[16, j + 2].SetStyle(lastStyle);
                worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].SetStyle(lastStyle);

                worksheets[0].Cells[16, j + 2].SetStyle(RowStyle);
                worksheets[0].Cells[16, j + 2 + TargetPlanDetailVList.Count].SetStyle(RowStyle);
            }
        

            #endregion

            #region 分解表下载
            for (int i = 0; i < TargetPlanDetailVList.Count; i++)
            {
                worksheets[i + 1].Name = TargetPlanDetailVList[i].TargetName;
                string TargetTitles = SysModel.SystemName + "经营指标分解明细表--" + TargetPlanDetailVList[i].TargetName;//表头名称
                worksheets[i + 1].Cells[0, 1].PutValue(TargetTitles);
                worksheets[i + 1].Cells[0, 1].SetStyle(StyleTargetTitle);
                worksheets[i + 1].Cells[1, 1].PutValue("年度：" + FinYear);
                worksheets[i + 1].Cells[1, 1].SetStyle(StyleTitle2);
                worksheets[i + 1].Cells[1, 26].PutValue("单位：" + TargetPlanDetailVList[i].Unit);
                worksheets[i + 1].Cells[1, 26].SetStyle(StyleTitle2);

                worksheets[i + 1].Cells[4, 3].PutValue(TargetPlanDetailVList[i].SumTargetSum12);
                //当月
                worksheets[i + 1].Cells[4, 4].PutValue(TargetPlanDetailVList[i].SumTarget1);

                worksheets[i + 1].Cells[4, 5].PutValue(TargetPlanDetailVList[i].SumTarget2);
                worksheets[i + 1].Cells[4, 6].PutValue(TargetPlanDetailVList[i].SumTarget3);
                worksheets[i + 1].Cells[4, 7].PutValue(TargetPlanDetailVList[i].SumTarget4);
                worksheets[i + 1].Cells[4, 8].PutValue(TargetPlanDetailVList[i].SumTarget5);
                worksheets[i + 1].Cells[4, 9].PutValue(TargetPlanDetailVList[i].SumTarget6);
                worksheets[i + 1].Cells[4, 10].PutValue(TargetPlanDetailVList[i].SumTarget7);
                worksheets[i + 1].Cells[4, 11].PutValue(TargetPlanDetailVList[i].SumTarget8);
                worksheets[i + 1].Cells[4, 12].PutValue(TargetPlanDetailVList[i].SumTarget9);
                worksheets[i + 1].Cells[4, 13].PutValue(TargetPlanDetailVList[i].SumTarget10);
                worksheets[i + 1].Cells[4, 14].PutValue(TargetPlanDetailVList[i].SumTarget11);
                worksheets[i + 1].Cells[4, 15].PutValue(TargetPlanDetailVList[i].SumTarget12);

                //累计
                worksheets[i + 1].Cells[4, 16].PutValue(TargetPlanDetailVList[i].SumTargetSum1);
                worksheets[i + 1].Cells[4, 17].PutValue(TargetPlanDetailVList[i].SumTargetSum2);
                worksheets[i + 1].Cells[4, 18].PutValue(TargetPlanDetailVList[i].SumTargetSum3);
                worksheets[i + 1].Cells[4, 19].PutValue(TargetPlanDetailVList[i].SumTargetSum4);
                worksheets[i + 1].Cells[4, 20].PutValue(TargetPlanDetailVList[i].SumTargetSum5);
                worksheets[i + 1].Cells[4, 21].PutValue(TargetPlanDetailVList[i].SumTargetSum6);
                worksheets[i + 1].Cells[4, 22].PutValue(TargetPlanDetailVList[i].SumTargetSum7);
                worksheets[i + 1].Cells[4, 23].PutValue(TargetPlanDetailVList[i].SumTargetSum8);
                worksheets[i + 1].Cells[4, 24].PutValue(TargetPlanDetailVList[i].SumTargetSum9);
                worksheets[i + 1].Cells[4, 25].PutValue(TargetPlanDetailVList[i].SumTargetSum10);
                worksheets[i + 1].Cells[4, 26].PutValue(TargetPlanDetailVList[i].SumTargetSum11);
                worksheets[i + 1].Cells[4, 27].PutValue(TargetPlanDetailVList[i].SumTargetSum12);

                for (int n = 0; n < TargetPlanDetailVList[i].TargetPlanDetailList.Count; n++)
                {
                    worksheets[i + 1].Cells.SetRowHeight(n + 5, 18);
                    worksheets[i + 1].Cells[n + 5, 0].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].seq);
                    worksheets[i + 1].Cells[n + 5, 0].SetStyle(TargetCompanystyle);
                    worksheets[i + 1].Cells[n + 5, 1].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].CompanyName);
                    worksheets[i + 1].Cells[n + 5, 1].SetStyle(TargetCompanystyle);


                    worksheets[i + 1].Cells[n + 5, 2].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].OpenTime);
                    worksheets[i + 1].Cells[n + 5, 2].SetStyle(TargetCompanystyle);
                    worksheets[i + 1].Cells[n + 5, 3].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum12);
                    worksheets[i + 1].Cells[n + 5, 3].SetStyle(TargetSumstyle);

                    worksheets[i + 1].Cells[n + 5, 4].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target1);
                    worksheets[i + 1].Cells[n + 5, 5].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target2);
                    worksheets[i + 1].Cells[n + 5, 6].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target3);
                    worksheets[i + 1].Cells[n + 5, 7].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target4);
                    worksheets[i + 1].Cells[n + 5, 8].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target5);
                    worksheets[i + 1].Cells[n + 5, 9].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target6);
                    worksheets[i + 1].Cells[n + 5, 10].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target7);
                    worksheets[i + 1].Cells[n + 5, 11].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target8);
                    worksheets[i + 1].Cells[n + 5, 12].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target9);
                    worksheets[i + 1].Cells[n + 5, 13].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target10);
                    worksheets[i + 1].Cells[n + 5, 14].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target11);
                    worksheets[i + 1].Cells[n + 5, 15].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].Target12);

                    worksheets[i + 1].Cells[n + 5, 16].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum1);
                    worksheets[i + 1].Cells[n + 5, 17].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum2);
                    worksheets[i + 1].Cells[n + 5, 18].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum3);
                    worksheets[i + 1].Cells[n + 5, 19].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum4);
                    worksheets[i + 1].Cells[n + 5, 20].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum5);
                    worksheets[i + 1].Cells[n + 5, 21].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum6);
                    worksheets[i + 1].Cells[n + 5, 22].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum7);
                    worksheets[i + 1].Cells[n + 5, 23].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum8);
                    worksheets[i + 1].Cells[n + 5, 24].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum9);
                    worksheets[i + 1].Cells[n + 5, 25].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum10);
                    worksheets[i + 1].Cells[n + 5, 26].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum11);
                    worksheets[i + 1].Cells[n + 5, 27].PutValue(TargetPlanDetailVList[i].TargetPlanDetailList[n].TargetSum12);
                    for (int a = 0; a < 12; a++)
                    {
                        worksheets[i + 1].Cells[n + 5, 4 + a].SetStyle(RowStyle);
                        worksheets[i + 1].Cells[n + 5, 16 + a].SetStyle(RowSumStyle);
                    }
                }
            }


            #endregion


            worksheets.RemoveAt(TargetPlanDetailVList.Count + 1);
            MemoryStream stream = designer.Workbook.SaveToStream();
            fileStream.Close();
            fileStream.Dispose();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";

            string dateNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(C_SystemOperator.Instance.GetSystem(SysId).SystemName + fileName + dateNow + ".xls"));
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