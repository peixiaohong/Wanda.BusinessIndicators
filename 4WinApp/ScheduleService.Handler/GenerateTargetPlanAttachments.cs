using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Engine;
using System.Configuration;
using System.IO;
using Lib.Web;
using Lib.Config;
using Aspose.Cells;
using System.Drawing;
using System.Reflection;
using LJTH.BusinessIndicators.Engine.Engine;
using System.Xml.Linq;
using Lib.Xml;
using Lib.Core;
using Lib.Web.Json;
using LJTH.BusinessIndicators;
using System.Transactions;
using Lib.Data;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    class GenerateTargetPlanAttachments : Quartz.IJob
    {

        private string _excelTempletePath = null;

        /// <summary>
        /// 图片地址
        /// </summary>
        string ImageFilePath = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + ConfigurationManager.AppSettings["ImagesPath"];

        /// <summary>
        /// 生成的Excel存放地址， （例子：C:\Excel\DowExportFile\2017-05-10 ，这样的文件夹中）
        /// </summary>
        string filePath = AppSettingConfig.GetSetting("FileServer_LocalPath", "") + ConfigurationManager.AppSettings["ExcelFilePath"] + "\\" + WebHelper.DateTimeNow.ToString("yyyy-MM-dd");

        /// <summary>
        /// 证书存放的地址
        /// </summary>
        string licenseFile = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + ConfigurationManager.AppSettings["AsposePath"];

        /// <summary>
        /// 这个是Excle 的模版路径
        /// </summary> 
        private string ExcelTempletePath
        {
            get
            {
                if (_excelTempletePath == null)
                {
                    _excelTempletePath = GetCurrentPath + ConfigurationManager.AppSettings["ExcelTempletePath"];
                }
                if (Directory.Exists(_excelTempletePath) == false)
                {
                    if (NetworkSharedFolder.IsNetShareMappedPath(_excelTempletePath))
                    {
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

        /// <summary>
        /// 获取当前文件路径
        /// </summary>
        private string GetCurrentPath
        {
            get
            {
                string c_Path = Assembly.GetExecutingAssembly().Location;
                c_Path = c_Path.Substring(0, c_Path.LastIndexOf('\\'));//删除文件名
                return c_Path;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("Service GenerateTargetPlanAttachments 年度计划指标分解 开始 execute");

            Common.ScheduleService.Log.Instance.Info(string.Format("Service GenerateTargetPlanAttachments：filePath =>{0}; ImageFilePath =>{1}; ImageFilePath =>{2}", filePath, ImageFilePath, licenseFile));

            C_ReportTime RptTime = C_ReportTimeOperator.Instance.GetReportTime();

            try
            {

                //获取当年的所有的计划分解指标
                var TPList = B_TargetplanOperator.Instance.GetTargetPlanByApprovedAndApproved(RptTime.ReportTime.Value.Year).ToList();

                ///数据时间
                if (TPList != null && TPList.Count > 0)
                {
                    var CList = TPList.Where(p => p.CreatorID == 0).ToList();

                    if (CList != null && CList.Count > 0)
                    {
                        //循环生成 附件
                        CList.ForEach(F =>
                        {
                            Common.ScheduleService.Log.Instance.Info("Service GenerateTargetPlanAttachments 生成Excel的附件信息：" + F.ID.ToString() + " ;  系统名称 ：" + F.SystemID);

                            bool IsUrl = GenerateAttachmentsUrl(F);  //这里写生成Excel

                            if (IsUrl)
                                F.CreatorID = 100;
                            else
                                F.CreatorID = -100;

                            //修改已经生成的Excel 
                            B_TargetplanOperator.Instance.UpdateTargetplan(F);

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ScheduleService.Log.Instance.Error("Service GenerateTargetPlanAttachments 年度计划指标分解 错误：" + ex.ToString());
            }

            Common.ScheduleService.Log.Instance.Info("Service GenerateTargetPlanAttachments 年度计划指标分解 结束 execute");
        }


        //excle的生成
        private bool GenerateAttachmentsUrl(B_TargetPlan Tp)
        {
            bool IsUrl = false;

            if (Tp != null)
            {
                try
                {
                    var Sys = StaticResource.Instance[Tp.SystemID, Tp.CreateTime];
                    if (Sys != null && Sys.Category == 3)
                    {
                        string templetePath = string.Empty;
                        string templeteName = string.Empty;
                        templetePath = ExcelTempletePath;
                        templeteName = "计划指标上传模板V1.xlsx";
                        DownExclel_Group(templetePath, templeteName, Tp);
                    }
                    else
                        DownExcel(Tp.SystemID, Tp.FinYear, Tp);

                    IsUrl = true;
                }
                catch (Exception ex)
                {
                    IsUrl = false;
                    Common.ScheduleService.Log.Instance.Error("Service GenerateTargetPlanAttachments 年度计划指标分解:GenerateAttachmentsUrl : 系统ID：" + Tp.ID.ToString() + "  错误：" + ex.ToString());
                }
            }
            return IsUrl;
        }



        /// <summary>
        /// 生成Excle ，就是有12个月的 系统都可以用这个下载
        /// </summary>
        /// <param name="SysId"></param>
        /// <param name="FinYear"></param>
        /// <param name="Tp"></param>
        private void DownExcel(Guid SysId, int FinYear, B_TargetPlan Tp)
        {
            ExcelEngine excel = new ExcelEngine(licenseFile);
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "指标汇总V1.xls");
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
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
            List<TargetPlanDetailVList> TargetPlanDetailVList = B_TargetplandetailOperator.Instance.GetTargetHistory(FinYear, SysId, Tp.ID);   //A_TargetplandetailOperator.Instance.GetSumTargetDetail(FinYear, SysId);
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
            List<TargetDetail> TargetDetail = B_TargetplandetailOperator.Instance.GetSumMonthTargetDetail(FinYear, SysId, Tp);   //A_TargetplandetailOperator.Instance.GetSumMonthTargetDetail(FinYear, SysId);

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



            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);//如果上传文件路径不存在，则创建该路径。
            }

            string fileName = "年度经营指标月度分解汇总表";
            string dateNow = DateTime.Now.ToString("HHmmss");

            string t_fileName = SysModel.SystemName + FinYear.ToString() + fileName + "_" + dateNow + ".xls";
            t_fileName = Path.Combine(filePath, t_fileName); // 文件路径
            designer.Workbook.Save(t_fileName); // 生成文件
            string url = ConfigurationManager.AppSettings["ExcelFilePath"] + "\\" + WebHelper.DateTimeNow.ToString("yyyy-MM-dd") + t_fileName.Replace(filePath, "");
            AddAttachments(Tp.ID, t_fileName.Replace(filePath, "").Replace("\\", ""), url.Substring(1, url.Length - 1), fileStream);

            fileStream.Close();
            fileStream.Dispose();


        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="FileName"></param>
        /// <param name="FileUrl"></param>
        /// <param name="fileStream"></param>
        private void AddAttachments(Guid BusinessID, string FileName, string FileUrl, FileStream fileStream)
        {
            string _Size = string.Empty;

            if (fileStream.Length > 1024 * 1024)
                _Size = (fileStream.Length / (1024 * 1024)).ToString("D2") + " M";
            else
                _Size = (fileStream.Length / (1024)).ToString("D2") + " KB";

            B_Attachment Att = new B_Attachment()
            {
                BusinessID = BusinessID,
                FileName = FileName,
                Url = FileUrl,
                BusinessType = "系统生成Excel",
                CreateTime = DateTime.Now,
                CreatorName = "SystemTargetPlan",
                IsDeleted = false,
                Size = _Size
            };

            //判断是否存在
            var AttList = B_AttachmentOperator.Instance.GetAttachmentList(BusinessID, "系统生成Excel").ToList();

            if (AttList != null && AttList.Count > 0)
            {
                var UAtt = AttList.Where(A => A.FileName == FileName).FirstOrDefault();

                if (UAtt != null)
                {
                    UAtt.BusinessID = BusinessID;
                    UAtt.FileName = FileName;
                    UAtt.Url = FileUrl;
                    UAtt.BusinessType = "系统生成Excel";
                    UAtt.CreateTime = DateTime.Now;
                    UAtt.CreatorName = "SystemTargetPlan";
                    UAtt.IsDeleted = false;
                    UAtt.Size = _Size;
                    B_AttachmentOperator.Instance.UpdateAttachment(UAtt); // 更新
                }
                else
                {
                    B_AttachmentOperator.Instance.AddAttachment(Att);
                }
            }
            else
            {
                B_AttachmentOperator.Instance.AddAttachment(Att);
            }
        }
        

        /// <summary>
        /// 生成 集团总部 的分解指标
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="templeteName"></param>
        /// <param name="Tp"></param>
        public void DownExclel_Group(string templetePath, string templeteName, B_TargetPlan Tp)
        {
            ExcelEngine excel = new ExcelEngine(licenseFile);
            WorkbookDesigner designer = new WorkbookDesigner();
            List<DictionaryVmodel> listDV = (List<DictionaryVmodel>)TargetPlanEngine.TargetPlanEngineService.GetTargetPlanSource(Tp.SystemID, Tp.FinYear, Tp.ID, true);
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

            string strSystemName = StaticResource.Instance[Tp.SystemID, DateTime.Now].SystemName;

            for (int i = 0; i < listDV.Count; i++)
            {
                if (i > 0)
                {
                    worksheets.AddCopy(1);
                }

                worksheets[i + 1].Name = listDV[i].Name;
                excel.SetCustomProperty(worksheets[i + 1], "SystemID", Tp.SystemID.ToString());
                excel.SetCustomProperty(worksheets[i + 1], "SheetName", "UpTargetPlanDetail");
                worksheets[i + 1].Cells[0, 1].PutValue(Tp.FinYear + "年" + strSystemName + listDV[i].Name + "指标分解表");
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
                        DateTime dtime = StaticResource.Instance.CompanyList[Tp.SystemID].Where(p => p.CompanyName == listCompanyDV[i].Name).FirstOrDefault().OpeningTime;
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
                                    var TargetStr = string.Format("{0:N0}", singleMonthTargetPlanDetail.FirstOrDefault().Target);
                                    worksheet.Cells[rowStart, colStart + 4 + j].PutValue(TargetStr);
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
                        if (listCompanyDV[i].Mark.Contains("."))
                        {
                            var MarkStr = listCompanyDV[i].Mark.Split('.');
                            worksheet.Cells[rowStart, colStart + 2].PutValue(MarkStr[0]);
                        }
                        else
                        {
                            worksheet.Cells[rowStart, colStart + 2].PutValue(listCompanyDV[i].Mark);
                        }
                        #endregion

                        style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                        style2.Number = 3;
                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                    }
                    rowStart++;

                }
               
            }
            worksheets.RemoveAt(0);
            
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);//如果上传文件路径不存在，则创建该路径。
            }

            string fileName = "年度经营指标月度分解汇总表";
            string dateNow = DateTime.Now.ToString("HHmmss");

            string t_fileName = strSystemName + Tp.FinYear.ToString() + fileName + "_" + dateNow + ".xls";
            t_fileName = Path.Combine(filePath, t_fileName); // 文件路径
            designer.Workbook.Save(t_fileName); // 生成文件
            string url = ConfigurationManager.AppSettings["ExcelFilePath"] + "\\" + WebHelper.DateTimeNow.ToString("yyyy-MM-dd") + t_fileName.Replace(filePath, "");
            AddAttachments(Tp.ID, t_fileName.Replace(filePath, "").Replace("\\", ""), url.Substring(1, url.Length - 1), fileStream);

            fileStream.Close();
            fileStream.Dispose();
            
        }





    }
}
