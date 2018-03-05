
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Aspose.Cells;
using Lib.Config;
using Lib.Web.Json;

using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Common.Web;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using System.Drawing;
using Lib.Core;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// 下载需要上传的模版数据
    /// </summary>
    public class DownLoadView : IHttpHandler, IRequiresSessionState
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

        string FileType = string.Empty; //下载的文件类型
        Guid SysId = Guid.Empty;
        int FinYear = 0;
        int FinMonth = 0;
        Guid MonthReportID = Guid.Empty;
        ReportInstance rpt = null;
        string ImageFilePath = HttpContext.Current.Server.MapPath("../Images/images1");
        SimpleReportVModel smodel = new SimpleReportVModel();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (HttpContext.Current.Request.QueryString["FileType"] != null)
            {
                FileType = HttpContext.Current.Request.QueryString["FileType"];
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
            {
                FinYear = Convert.ToInt32(HttpContext.Current.Request["FinYear"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["FinMonth"]))
            {
                FinMonth = Convert.ToInt32(HttpContext.Current.Request["FinMonth"]);
            }


            if (HttpContext.Current.Request.QueryString["MonthReportID"] != null)
            {
                MonthReportID = Guid.Parse(HttpContext.Current.Request.QueryString["MonthReportID"]);
            }
            
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysId"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SysId"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["Simple"]))
            {
                string strSimple = HttpContext.Current.Request["Simple"].ToString();
                string[] strSimples = strSimple.Split(',');
                SRptModel srptm=null;
                int i = 1;
                foreach (string sp in strSimples)
                {
                    if (!string.IsNullOrEmpty(sp))
                    { 
                        srptm=new SRptModel();
                        srptm.ID = sp.Split(':')[0];
                        srptm.Name = StaticResource.Instance.SystemList.Where(p => p.ID == sp.Split(':')[0].ToGuid()).FirstOrDefault().SystemName;
                        srptm.Unit = sp.Split(':')[1];
                        srptm.Year = FinYear.ToString();
                        srptm.Month = FinMonth.ToString();
                        srptm.Index = i;
                        srptm.Time = FinYear.ToString() + "-" + FinMonth.ToString();
                        
                        i++;
                        smodel.List.Add(srptm);
                    }
                }
                //smodel = JsonHelper.Deserialize<SimpleReportVModel>(HttpContext.Current.Request["Simple"].ToString());
            }
            if (SysId != Guid.Empty)
            {
                if (MonthReportID == Guid.Empty)
                {
                    rpt = new ReportInstance(SysId, FinYear, FinMonth, true);
                }
                else
                {
                    rpt = new ReportInstance(MonthReportID, true);
                }
            }


            switch (FileType)
            {
                case "MissTargetRpt": //累计的 经营下载未完成说明模版（上报）
                    DownMissTarget();
                    break;
                case "TargetSimpleRpt":
                    DownTargetSimpleRpt(); //下载简报
                    break;
                case "DirectlyMissTargetRpt":  // 累计的 直管 下载未完成说明模版（上报）
                    DownDirectlyMissTarget();
                    break;
                case "missCurrentTargetReport": //当月数据 , 经营未完成模版（上报）
                    DownCurrentMissTarget();
                    break;

                case "CurrentDirectlyMissTargetRpt": //当月数据,下载未完成说明模版（上报）
                    DownCurrentDirectlyMissTarget(); 
                    break;


            }

        }


        /// <summary>
        /// 累计的 未完成说明（月度报告）
        /// </summary>
        private void DownMissTarget()
        {

            List<DictionaryVmodel> targetReturn = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt); 

            string templetePath = ExcelTempletePath;
            string templeteName = "未完成说明数据上报模版V2.xls";

            string fileName = string.Empty;

            if (rpt._System != null)
                fileName = rpt._System.SystemName + "累计未完成说明上报";
            else
                fileName = "累计未完成说明上报";

            ExcelMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
        }

        /// <summary>
        /// 未完成说明(上报模版)
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
            Aspose.Cells.Style style2_Color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "华文细黑";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
            style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style1.ForegroundColor = Color.LightGray;

            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style1.IsLocked = true; 
            #endregion

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "华文细黑";
            style2.Pattern = BackgroundType.Solid;
            //style2.Number = 3;
            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2.IsLocked = false;

            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style2_Color 样式
            style2_Color.Font.Size = 12;
            style2_Color.Font.Name = "华文细黑";
            style2_Color.IsLocked = true;
            //style2.Font.IsBold = true;
            style2.IsTextWrapped = true;

            style2_Color.Pattern = BackgroundType.Solid;
            style2_Color.Number = 3;
            style2_Color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2_Color.ForegroundColor = Color.LightGray;
            
            style2_Color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; 
            style2_Color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_Color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "华文细黑";
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


            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期:" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "累计未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name.Replace('\\', '-');
                }

                excel.SetCustomProperty(worksheets[i], "SheetName", "MissTargetRptExcel");
            }


            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                List<DictionaryVmodel> itemList = (List<DictionaryVmodel>)List[i].ObjValue;


                foreach (DictionaryVmodel item in itemList)
                {
                    if (item.Name.Contains("补回公司"))
                    {
                        continue;
                    }

                    StyleFlag flg = new StyleFlag();
                    flg.All = true;

                    if (item.Mark == "Counter")
                    {
                        List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue; //当前的objValue里的数据递归

                        MissTargetParseGroup(List[i].Name, list, worksheets[i], ref rowStart, colStart, style1, style2, style3,style2_Color ,IsRpt,false);  //递归循环
                    }
                    else if (item.Mark == "Data")
                    {
                        int _MergeCol = 0; //合并列

                        List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                        int index = 1;
                        int rowStartR = rowStart;

                        ReportDetailLisr.ForEach(p =>
                        {
                            if (p.CompanyName != "未完成合计") //针对商管，物管系统的 未完成合计排除
                            {

                                #region 设置表格中的所有样式

                                //设置Excel的样式
                                worksheets[i].Cells[rowStartR, 1].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart].SetStyle(style2_Color, true);

                                //特殊处理的系统
                                if (System.Configuration.ConfigurationManager.AppSettings["ExceptionSystem"].ToUpper().Contains(p.SystemID.ToString().ToUpper()))
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2_Color, true);// 指标名称, 在商管系统的时候隐藏
                                    worksheets[i].Cells.Columns[colStart + 1].Width = 0;// 指标名称, 在商管系统的时候隐藏
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2_Color, true);// 指标名称, 在商管系统的时候隐藏
                                }

                                worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color, true);


                                worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 12].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 13].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 14].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 15].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 16].SetStyle(style2_Color, true);

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
                                            itemRangeByID.ApplyStyle(style2_Color, flg);

                                            Range itemRangeByCompanyName = worksheets[i].Cells.CreateRange(rowStartR, colStart, List[i].TargetGroupCount, 1);
                                            itemRangeByCompanyName.Merge();
                                            itemRangeByCompanyName.ApplyStyle(style2_Color, flg);

                                            if (item.Value == "1" && rpt._System.Category == 2) //如果是未完成双指标，同时是项目系统,则特殊处理
                                            {

                                            }
                                            else
                                            {
                                                Range itemRangeByMIssTargetReason = worksheets[i].Cells.CreateRange(rowStartR, colStart + 10, List[i].TargetGroupCount, 1); //未完成说明
                                                itemRangeByMIssTargetReason.Merge();
                                                itemRangeByMIssTargetReason.ApplyStyle(style2, flg);

                                                Range itemRangeByMIssTargetDescription = worksheets[i].Cells.CreateRange(rowStartR, colStart + 11, List[i].TargetGroupCount, 1); //采取措施
                                                itemRangeByMIssTargetDescription.Merge();
                                                itemRangeByMIssTargetDescription.ApplyStyle(style2, flg);
                                            }

                                            //Range itemRangeByReturnDescription = worksheets[i].Cells.CreateRange(rowStartR, colStart + 13, List[i].TargetGroupCount, 1); //补回状况
                                            //itemRangeByReturnDescription.Merge();
                                            //itemRangeByReturnDescription.ApplyStyle(style2, flg);

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
                                    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color);
                                }

                                worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NAccumulativeDifference - p.LastNAccumulativeDifference); //差值- 差值
                                worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.LastNAccumulativePlanAmmount); //上月计划
                                worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.LastNAccumulativeActualAmmount);//上月实际

                                if (p.LastNAccumulativeDifference < 0)
                                { worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style1); }
                                else
                                { worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2_Color); }

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
                                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color);
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
                                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue(p.MIssTargetReason); //未完成原因
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.MIssTargetDescription);//及采取措施

                                        //承诺补回时间

                                        //要求期限
                                        if (p.CommitDate != null)
                                        {
                                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                                        }
                                        else
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                                        }
                                    }
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 10].PutValue(p.MIssTargetReason); //未完成原因
                                    worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.MIssTargetDescription);//及采取措施
                                    //要求期限
                                    if (p.CommitDate != null)
                                    {
                                        string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                        worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                                    }
                                }

                                if (p.ReturnType != 0)
                                {
                                    //承诺补回时间
                                    string PromissDate = string.Empty;
                                    if (p.PromissDate != null)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 12].PutValue(p.PromissDate.Value.ToString("yyyy-MM-dd")); //承诺补回时间    
                                    }

                                    worksheets[i].Cells[rowStartR, colStart + 13].PutValue(p.ReturnDescription); //补回情况                                
                                    worksheets[i].Cells[rowStartR, colStart + 15].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType)); //补回状态
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 15].PutValue("---");//补回状态
                                }


                                if (p.Counter > 0)
                                {
                                    //警示灯
                                    int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 16, ImageFilePath + "\\image" + p.Counter + ".png");
                                    Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                                    picture.Left = 40;
                                    picture.Top = 10;
                                }

                                #endregion

                                rowStartR++;
                            }
                        });

                        rowStart = rowStartR;
                    }


                    worksheets[i].Protect(ProtectionType.Contents);
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
        /// <param name="IsRpt">是否上报模版：</param>
        /// <param name="IsCurrent">是否当月未完成模版：</param>
        public void MissTargetParseGroup(string TargetName, List<DictionaryVmodel> MissTargetList, Worksheet worksheet, ref int rowStart, int colStart, Style style1, Style style2, Style style3, Style style2_Color, bool IsRpt , bool IsCurrent)
        {
            //返回的数据
            foreach (var item in MissTargetList)
            {
                StyleFlag flg = new StyleFlag();
                flg.All = true;

                if (item.Mark == "Counter")
                {
                    List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue;

                    MissTargetParseGroup(TargetName, list, worksheet, ref rowStart, colStart, style1, style2, style3,style2_Color, IsRpt , IsCurrent);

                }
                else if (item.Mark == "Data")
                {
                    List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                    int rowStartR = rowStart;

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
                        worksheet.Cells[rowStartR, colStart + 13].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 14].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 15].SetStyle(style2);
                        worksheet.Cells[rowStartR, colStart + 16].SetStyle(style2);

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

                                    Range itemRangeByMIssTargetReason = worksheet.Cells.CreateRange(rowStartR, colStart + 10, MissTargetList[0].TargetGroupCount, 1);//原因
                                    itemRangeByMIssTargetReason.Merge();
                                    itemRangeByMIssTargetReason.ApplyStyle(style2, flg);


                                    Range itemRangeByMIssTargetDescription = worksheet.Cells.CreateRange(rowStartR, colStart + 11, MissTargetList[0].TargetGroupCount, 1); //采取措施
                                    itemRangeByMIssTargetDescription.Merge();
                                    itemRangeByMIssTargetDescription.ApplyStyle(style2, flg);

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
                        worksheet.Cells[rowStartR, colStart + 8].PutValue(p.NAccumulativePlanAmmount - p.NAccumulativeActualAmmount); //本月差值
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

                                if (IsCurrent) // 当月未完成
                                {
                                    worksheet.Cells[rowStartR, colStart + 10].PutValue(p.CurrentMIssTargetReason);
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CurrentMIssTargetDescription);
                                }
                                else
                                {  //累计
                                    worksheet.Cells[rowStartR, colStart + 10].PutValue(p.MIssTargetReason);
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(p.MIssTargetDescription);
                                }
                               

                                if (p.CommitDate != null)
                                {
                                    string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                                }
                                else
                                {
                                    worksheet.Cells[rowStartR, colStart + 12].PutValue("---");
                                }

                            }
                        }
                        else
                        {

                            if (IsCurrent) // 当月未完成
                            {
                                worksheet.Cells[rowStartR, colStart + 10].PutValue(p.CurrentMIssTargetReason);
                                worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CurrentMIssTargetDescription);
                            }
                            else
                            {  //累计
                                worksheet.Cells[rowStartR, colStart + 10].PutValue(p.MIssTargetReason);
                                worksheet.Cells[rowStartR, colStart + 11].PutValue(p.MIssTargetDescription);
                            }
                            
                            if (p.CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                worksheet.Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                            }
                            else
                            {
                                worksheet.Cells[rowStartR, colStart + 14].PutValue("---");
                            }

                        }

                        if (p.ReturnType != 0)
                        {
                            //承诺补回时间
                            string PromissDate = string.Empty;
                            if (p.PromissDate != null)
                            {
                                worksheet.Cells[rowStartR, colStart + 12].PutValue(p.PromissDate.Value.ToString("yyyy-MM-dd")); //承诺补回时间
                            }

                            worksheet.Cells[rowStartR, colStart + 13].PutValue(p.ReturnDescription); //补回情况

                            worksheet.Cells[rowStartR, colStart + 15].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType));
                        }
                        else
                        {
                            worksheet.Cells[rowStartR, colStart + 15].PutValue("---");
                        }

                        if (p.Counter > 0)
                        {
                            int pictureIndex = worksheet.Pictures.Add(rowStartR, colStart + 16, ImageFilePath + "\\image" + p.Counter + ".png");
                            Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                            picture.Left = 40;
                            picture.Top = 10;

                        }
                        #endregion

                        rowStartR++;
                    });

                    rowStart = rowStartR;
                }

            }
        }


        /// <summary>
        /// 下载当月未完成说明
        /// </summary>
        private void DownCurrentMissTarget()
        {

            List<DictionaryVmodel> targetReturn = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);

            string templetePath = ExcelTempletePath;
            string templeteName = "当月未完成说明数据上报模版V1.xlsx";

            string fileName = string.Empty;

            if (rpt._System != null)
                fileName = rpt._System.SystemName + "当月未完成说明上报";
            else
                fileName = "当月未完成说明上报";

            CurrentExcelMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
        }

        /// <summary>
        ///  当月的未完成说明(上报模版)
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void CurrentExcelMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
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
            Aspose.Cells.Style style2_Color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "华文细黑";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
            style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style1.ForegroundColor = Color.LightGray;

            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style1.IsLocked = true;
            #endregion

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "华文细黑";
            style2.Pattern = BackgroundType.Solid;
            //style2.Number = 3;
            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style2.VerticalAlignment = Aspose.Cells.TextAlignmentType.Top;
            style2.IsLocked = false;

            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style2_Color 样式
            style2_Color.Font.Size = 12;
            style2_Color.Font.Name = "华文细黑";
            style2_Color.IsLocked = true;
            style2.Font.IsBold = true;
            style2.IsTextWrapped = true;

            style2_Color.Pattern = BackgroundType.Solid;
            style2_Color.Number = 3;
            style2_Color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2_Color.ForegroundColor = Color.LightGray;

            style2_Color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_Color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style2_Color.Font.IsBold = true;
            #endregion


            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "华文细黑";
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


            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期:" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "经营指标当月未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "当月未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name.Replace('\\', '-');
                }

                excel.SetCustomProperty(worksheets[i], "SheetName", "MissTargetRptExcel");
            }


            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                List<DictionaryVmodel> itemList = (List<DictionaryVmodel>)List[i].ObjValue;


                foreach (DictionaryVmodel item in itemList)
                {
                    if (item.Name.Contains("补回公司"))
                    {
                        continue;
                    }

                    StyleFlag flg = new StyleFlag();
                    flg.All = true;

                    if (item.Mark == "Counter")
                    {
                        List<DictionaryVmodel> list = (List<DictionaryVmodel>)item.ObjValue; //当前的objValue里的数据递归

                        MissTargetParseGroup(List[i].Name, list, worksheets[i], ref rowStart, colStart, style1, style2, style3, style2_Color, IsRpt ,true);  //递归循环
                    }
                    else if (item.Mark == "Data")
                    {
                        int _MergeCol = 0; //合并列

                        List<MonthlyReportDetail> ReportDetailLisr = (List<MonthlyReportDetail>)item.ObjValue;

                        int index = 1;
                        int rowStartR = rowStart;

                        ReportDetailLisr.ForEach(p =>
                        {
                            if (p.CompanyName != "未完成合计") //针对商管，物管系统的 未完成合计排除
                            {

                                #region 设置表格中的所有样式

                                //设置Excel的样式
                                worksheets[i].Cells[rowStartR, 1].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart].SetStyle(style2_Color, true);

                                //特殊处理的系统
                                if (System.Configuration.ConfigurationManager.AppSettings["ExceptionSystem"].ToUpper().Contains(p.SystemID.ToString().ToUpper()))
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2_Color, true);// 指标名称, 在商管系统的时候隐藏
                                    worksheets[i].Cells.Columns[colStart + 1].Width = 0;// 指标名称, 在商管系统的时候隐藏
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2_Color, true);// 指标名称, 在商管系统的时候隐藏
                                }

                                worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color, true);


                                worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);
                                worksheets[i].Cells[rowStartR, colStart + 12].SetStyle(style2_Color,true);
                                worksheets[i].Cells[rowStartR, colStart + 13].SetStyle(style2_Color,true);
                                worksheets[i].Cells[rowStartR, colStart + 14].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 15].SetStyle(style2_Color, true);
                                worksheets[i].Cells[rowStartR, colStart + 16].SetStyle(style2_Color, true);

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
                                            itemRangeByID.ApplyStyle(style2_Color, flg);

                                            Range itemRangeByCompanyName = worksheets[i].Cells.CreateRange(rowStartR, colStart, List[i].TargetGroupCount, 1);
                                            itemRangeByCompanyName.Merge();
                                            itemRangeByCompanyName.ApplyStyle(style2_Color, flg);

                                            if (item.Value == "1" && rpt._System.Category == 2) //如果是未完成双指标，同时是项目系统,则特殊处理
                                            {

                                            }
                                            else
                                            {
                                                Range itemRangeByMIssTargetReason = worksheets[i].Cells.CreateRange(rowStartR, colStart + 10, List[i].TargetGroupCount, 1); //未完成说明
                                                itemRangeByMIssTargetReason.Merge();
                                                itemRangeByMIssTargetReason.ApplyStyle(style2, flg);

                                                Range itemRangeByMIssTargetDescription = worksheets[i].Cells.CreateRange(rowStartR, colStart + 11, List[i].TargetGroupCount, 1); //采取措施
                                                itemRangeByMIssTargetDescription.Merge();
                                                itemRangeByMIssTargetDescription.ApplyStyle(style2, flg);
                                            }

                                            //Range itemRangeByReturnDescription = worksheets[i].Cells.CreateRange(rowStartR, colStart + 13, List[i].TargetGroupCount, 1); //补回状况
                                            //itemRangeByReturnDescription.Merge();
                                            //itemRangeByReturnDescription.ApplyStyle(style2, flg);

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

                                //update date by 2017-7-26 ,这里没有新增差额

                                //decimal tempDiff = p.NAccumulativeDifference - p.LastNAccumulativeDifference;
                                //if (tempDiff < 0)
                                //{ worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1); }
                                //else
                                //{
                                //    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color);
                                //}

                               
                                worksheets[i].Cells[rowStartR, colStart + 2].PutValue(p.NPlanAmmount); //当月计划

                                worksheets[i].Cells[rowStartR, colStart + 3].PutValue(p.NActualAmmount); //当月实际
                                worksheets[i].Cells[rowStartR, colStart + 4].PutValue(p.NDifference);//当月差值

                                if (p.NDifference < 0)
                                { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style1); }
                                else
                                { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2_Color); }

                                worksheets[i].Cells[rowStartR, colStart + 5].PutValue(p.NDisplayRate); //当月完成率
                                worksheets[i].Cells[rowStartR, colStart + 6].PutValue(p.NAccumulativePlanAmmount);
                                worksheets[i].Cells[rowStartR, colStart + 7].PutValue(p.NAccumulativeActualAmmount);


                                //这里是为了给数字加红
                                if (p.NAccumulativeDifference < 0)
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1);
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color);
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
                                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue(p.CurrentMIssTargetReason); //未完成原因
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CurrentMIssTargetDescription);//及采取措施

                                        //承诺补回时间

                                        //要求期限
                                        if (p.CommitDate != null)
                                        {
                                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                                        }
                                        else
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                                        }
                                    }
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 10].PutValue(p.CurrentMIssTargetReason); //未完成原因
                                    worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CurrentMIssTargetDescription);//及采取措施
                                    //要求期限
                                    if (p.CommitDate != null)
                                    {
                                        string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                        worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                                    }
                                }

                                if (p.ReturnType != 0)
                                {
                                    //承诺补回时间
                                    string PromissDate = string.Empty;
                                    if (p.PromissDate != null)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 12].PutValue(p.PromissDate.Value.ToString("yyyy-MM-dd")); //承诺补回时间    
                                    }

                                    worksheets[i].Cells[rowStartR, colStart + 13].PutValue(p.ReturnDescription); //补回情况                                
                                    worksheets[i].Cells[rowStartR, colStart + 15].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType)); //补回状态
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 15].PutValue("---");//补回状态
                                }


                                if (p.Counter > 0)
                                {
                                    //警示灯
                                    int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 16, ImageFilePath + "\\image" + p.Counter + ".png");
                                    Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                                    picture.Left = 40;
                                    picture.Top = 10;
                                }
                                else
                                {
                                    worksheets[i].Cells[rowStartR, colStart + 16].PutValue("---");
                                }

                                #endregion

                                rowStartR++;
                            }
                        });

                        rowStart = rowStartR;
                    }


                    worksheets[i].Protect(ProtectionType.Contents);
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
        /// 累计的直管模版
        /// </summary>
        private void DownDirectlyMissTarget()
        {

            List<DictionaryVmodel> targetReturn = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);

            string templetePath = ExcelTempletePath;
            string templeteName = "未完成说明数据上报模版V2.xls";

            string fileName = string.Empty;

            if (rpt._System != null)
                fileName = rpt._System.SystemName + "累计未完成说明上报";
            else
                fileName = "累计未完成说明上报";

            ExcelDirectlyMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
        }

        /// <summary>
        /// 未完成说明(上报模版)
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void ExcelDirectlyMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
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
            Aspose.Cells.Style style2_Color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "华文细黑";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
            style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style1.ForegroundColor = Color.LightGray;

            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style1.IsLocked = true;
            #endregion

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "华文细黑";
            style2.Pattern = BackgroundType.Solid;
            //style2.Number = 3;
            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2.IsLocked = false;

            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style2_Color 样式
            style2_Color.Font.Size = 12;
            style2_Color.Font.Name = "华文细黑";
            style2_Color.IsLocked = true;
            //style2.Font.IsBold = true;
            style2.IsTextWrapped = true;

            style2_Color.Pattern = BackgroundType.Solid;
            style2_Color.Number = 3;
            style2_Color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2_Color.ForegroundColor = Color.LightGray;

            style2_Color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_Color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "华文细黑";
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


            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期:" + FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "累计未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name;
                }

                excel.SetCustomProperty(worksheets[i], "SheetName", "MissTargetRptExcel");
            }

            
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;
                List<MonthlyReportDetail> lstMRD = (List < MonthlyReportDetail >) List[0].ObjValue;

                int index = 1;
                for (int j = 0; j < lstMRD.Count(); j++)
                {
                    int rowStartR = rowStart;
                    #region 设置表格中的所有样式

                    //设置Excel的样式
                    worksheets[i].Cells[rowStartR, 1].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart].SetStyle(style2_Color, true);
                    worksheets[i].Cells.Columns[colStart].Width = 0;
                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2);
                    worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);
                    worksheets[i].Cells[rowStartR, colStart + 12].SetStyle(style2);
                    worksheets[i].Cells[rowStartR, colStart + 13].SetStyle(style2);
                    worksheets[i].Cells[rowStartR, colStart + 14].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 15].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 16].SetStyle(style2_Color, true);
                    #endregion

                    #region 加载数据

                    //加载数据

                    worksheets[i].Cells[rowStartR, colStart].PutValue(lstMRD[j].CompanyName); //公司名称
                    worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号
                    worksheets[i].Cells[rowStartR, colStart + 1].PutValue(lstMRD[j].TargetName); //指标名称
                    decimal tempDiff = lstMRD[j].NAccumulativeDifference - lstMRD[j].LastNAccumulativeDifference;
                    if (tempDiff < 0)
                    {
                        worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1);
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color);
                    }

                    worksheets[i].Cells[rowStartR, colStart + 2].PutValue(lstMRD[j].NAccumulativeDifference - lstMRD[j].LastNAccumulativeDifference); //差值- 差值
                    worksheets[i].Cells[rowStartR, colStart + 3].PutValue(lstMRD[j].LastNAccumulativePlanAmmount); //上月计划
                    worksheets[i].Cells[rowStartR, colStart + 4].PutValue(lstMRD[j].LastNAccumulativeActualAmmount);//上月实际

                    if (lstMRD[j].LastNAccumulativeDifference < 0)
                    { worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style1); }
                    else
                    { worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2_Color); }

                    worksheets[i].Cells[rowStartR, colStart + 5].PutValue(lstMRD[j].LastNAccumulativeDifference); //上月计划 - 上月实际 ==上月差值
                    worksheets[i].Cells[rowStartR, colStart + 6].PutValue(lstMRD[j].NAccumulativePlanAmmount);
                    worksheets[i].Cells[rowStartR, colStart + 7].PutValue(lstMRD[j].NAccumulativeActualAmmount);


                    //这里是为了给数字加红
                    if (lstMRD[j].NAccumulativeDifference < 0)
                    {
                        worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1);
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color);
                    }

                    worksheets[i].Cells[rowStartR, colStart + 8].PutValue(lstMRD[j].NAccumulativeDifference); //本月差值
                    worksheets[i].Cells[rowStartR, colStart + 9].PutValue(lstMRD[j].NAccumulativeDisplayRate);//本月完成率

                    if (IsRpt) //是上报模版
                    {
                        if (lstMRD[j].ReturnType == (int)EnumReturnType.New) //本月新增
                        {
                            worksheets[i].Cells[rowStartR, colStart + 10].PutValue("");
                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue("");
                        }
                        else
                        {
                            worksheets[i].Cells[rowStartR, colStart + 10].PutValue(lstMRD[j].MIssTargetReason); //未完成原因
                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(lstMRD[j].MIssTargetDescription);//及采取措施

                            //承诺补回时间

                            //要求期限
                            if (lstMRD[j].CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", lstMRD[j].CommitDate);
                                worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                            }
                            else
                            {
                                worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                            }
                        }
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue(lstMRD[j].MIssTargetReason); //未完成原因
                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(lstMRD[j].MIssTargetDescription);//及采取措施
                        //要求期限
                        if (lstMRD[j].CommitDate != null)
                        {
                            string CommitDate = string.Format("{0:yyyy年MM月dd}", lstMRD[j].CommitDate);
                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                        }
                        else
                        {
                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                        }
                    }

                    if (lstMRD[j].ReturnType != 0)
                    {
                        //承诺补回时间
                        string PromissDate = string.Empty;
                        if (lstMRD[j].PromissDate != null)
                        {
                            worksheets[i].Cells[rowStartR, colStart + 12].PutValue(lstMRD[j].PromissDate.Value.ToString("yyyy-MM-dd")); //承诺补回时间    
                        }

                        worksheets[i].Cells[rowStartR, colStart + 13].PutValue(lstMRD[j].ReturnDescription); //补回情况                                
                        worksheets[i].Cells[rowStartR, colStart + 15].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), lstMRD[j].ReturnType)); //补回状态
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 15].PutValue("---");//补回状态
                    }


                    if (lstMRD[j].Counter > 0)
                    {
                        //警示灯
                        int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 16, ImageFilePath + "\\image" +lstMRD[j].Counter + ".png");
                        Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                        picture.Left = 40;
                        picture.Top = 10;
                    }
                    #endregion
                    rowStartR++;
                    rowStart = rowStartR;
                }
            }
            worksheets[0].Protect(ProtectionType.Contents);
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
        /// 当月的直管
        /// </summary>
        private void DownCurrentDirectlyMissTarget()
        {

            List<DictionaryVmodel> targetReturn = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt);


            string templetePath = ExcelTempletePath;
            string templeteName = "当月未完成说明数据上报模版V1.xlsx";

            string fileName = string.Empty;

            if (rpt._System != null)
                fileName = rpt._System.SystemName + "当月未完成说明上报";
            else
                fileName = "当月未完成说明上报";

            
            CurrentExcelDirectlyMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
        }
        
        /// <summary>
        /// 直管 当月的 未完成说明(上报模版)
        /// </summary>
        /// <param name="List">未完成数据</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        /// <param name="fileName">到处文件名称</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="IsRpt">是否是上报模版</param>
        public void CurrentExcelDirectlyMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
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
            Aspose.Cells.Style style2_Color = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];

            style0.Font.Size = 12;
            style0.Font.Name = "Arial";

            #region style1 样式,无边框
            style1.Font.Size = 12;
            style1.Font.Name = "华文细黑";
            style1.Pattern = BackgroundType.Solid;
            style1.Font.Color = Color.Red;
            style1.Number = 3;
            style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style1.ForegroundColor = Color.LightGray;

            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style1.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style1.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style1.IsLocked = true;
            #endregion

            #region style2 样式
            style2.Font.Size = 12;
            style2.Font.Name = "华文细黑";
            style2.Pattern = BackgroundType.Solid;
            //style2.Number = 3;
            style2.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Left;
            style2.VerticalAlignment = Aspose.Cells.TextAlignmentType.Top;
            style2.IsLocked = false;

            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style2_Color 样式
            style2_Color.Font.Size = 12;
            style2_Color.Font.Name = "华文细黑";
            style2_Color.IsLocked = true;
            //style2.Font.IsBold = true;
            style2.IsTextWrapped = true;

            style2_Color.Pattern = BackgroundType.Solid;
            style2_Color.Number = 3;
            style2_Color.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            style2_Color.ForegroundColor = Color.LightGray;

            style2_Color.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; ;
            style2_Color.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style2_Color.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2_Color.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            #endregion


            #region style3 样式,无边框

            style3.Font.Size = 12;
            style3.Font.Name = "华文细黑";
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


            int rowStart = 4;  //开始行
            int colStart = 2; // 每日开始列


            //创建指标Sheet,
            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;

                worksheets[0].Cells[1, 1].PutValue("报告期:" +FinYear.ToString() + "年" + FinMonth + "月");
                worksheets[0].Cells[1, 1].SetStyle(style0);

                if (i > 0)
                {
                    worksheets.AddCopy(0);
                }

                worksheets[i].Cells[0, 1].PutValue(rpt._System.SystemName + List[i].Name + "经营指标未完成公司具体原因及补回措施");
                if (List.Count == 1)
                {
                    worksheets[i].Name = "当月未完成说明";
                }
                else
                {
                    worksheets[i].Name = List[i].Name;
                }

                excel.SetCustomProperty(worksheets[i], "SheetName", "MissTargetRptExcel");
            }


            for (int i = 0; i < List.Count; i++)
            {
                rowStart = 4;
                List<MonthlyReportDetail> lstMRD = (List<MonthlyReportDetail>)List[0].ObjValue;

                int index = 1;

                for (int j = 0; j < lstMRD.Count(); j++)
                {

                    int rowStartR = rowStart;

                    #region 设置表格中的所有样式

                    //设置Excel的样式
                    worksheets[i].Cells[rowStartR, 1].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart].SetStyle(style2_Color, true);
                    worksheets[i].Cells.Columns[colStart].Width = 0;
                    worksheets[i].Cells[rowStartR, colStart + 1].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 3].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 5].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 6].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 7].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 9].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 10].SetStyle(style2);
                    worksheets[i].Cells[rowStartR, colStart + 11].SetStyle(style2);
                    worksheets[i].Cells[rowStartR, colStart + 12].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 13].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 14].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 15].SetStyle(style2_Color, true);
                    worksheets[i].Cells[rowStartR, colStart + 16].SetStyle(style2_Color, true);
                    #endregion

                    #region 加载数据

                    //加载数据

                   
                    worksheets[i].Cells[rowStartR, colStart].PutValue(lstMRD[j].CompanyName); //公司名称
                    worksheets[i].Cells[rowStartR, 1].PutValue(index++); //序号
                    worksheets[i].Cells[rowStartR, colStart + 1].PutValue(lstMRD[j].TargetName); //指标名称
                    //decimal tempDiff = lstMRD[j].NAccumulativeDifference - lstMRD[j].LastNAccumulativeDifference;
                    //if (tempDiff < 0)
                    //{
                    //    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style1);
                    //}
                    //else
                    //{
                    //    worksheets[i].Cells[rowStartR, colStart + 2].SetStyle(style2_Color);
                    //}

                    //worksheets[i].Cells[rowStartR, colStart + 2].PutValue(lstMRD[j].NAccumulativeDifference - lstMRD[j].LastNAccumulativeDifference); //差值- 差值

                    worksheets[i].Cells[rowStartR, colStart + 2].PutValue(lstMRD[j].NPlanAmmount); //当月计划
                    worksheets[i].Cells[rowStartR, colStart + 3].PutValue(lstMRD[j].NActualAmmount);//当月实际

                    if (lstMRD[j].NDifference < 0)
                    { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style1); }
                    else
                    { worksheets[i].Cells[rowStartR, colStart + 4].SetStyle(style2_Color); }

                    worksheets[i].Cells[rowStartR, colStart + 4].PutValue(lstMRD[j].NDifference); //当月差额
                    worksheets[i].Cells[rowStartR, colStart + 5].PutValue(lstMRD[j].NDisplayRate); //当月完成率

                    worksheets[i].Cells[rowStartR, colStart + 6].PutValue(lstMRD[j].NAccumulativePlanAmmount); //本月累计计划
                    worksheets[i].Cells[rowStartR, colStart + 7].PutValue(lstMRD[j].NAccumulativeActualAmmount);//本月累计实际


                    //这里是为了给数字加红
                    if (lstMRD[j].NAccumulativeDifference < 0)
                    {
                        worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style1);
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 8].SetStyle(style2_Color);
                    }

                    worksheets[i].Cells[rowStartR, colStart + 8].PutValue(lstMRD[j].NAccumulativeDifference); //本月差值
                    worksheets[i].Cells[rowStartR, colStart + 9].PutValue(lstMRD[j].NAccumulativeDisplayRate);//本月完成率

                    if (IsRpt) //是上报模版
                    {
                        if (lstMRD[j].ReturnType == (int)EnumReturnType.New) //本月新增
                        {
                            worksheets[i].Cells[rowStartR, colStart + 10].PutValue("");
                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue("");
                        }
                        else
                        {
                            worksheets[i].Cells[rowStartR, colStart + 10].PutValue(lstMRD[j].CurrentMIssTargetReason); //未完成原因
                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(lstMRD[j].CurrentMIssTargetDescription);//及采取措施

                            //承诺补回时间

                            //要求期限
                            if (lstMRD[j].CommitDate != null)
                            {
                                string CommitDate = string.Format("{0:yyyy年MM月dd}", lstMRD[j].CommitDate);
                                worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                            }
                            else
                            {
                                worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                            }
                        }
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue(lstMRD[j].CurrentMIssTargetReason); //未完成原因
                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(lstMRD[j].CurrentMIssTargetDescription);//及采取措施
                        //要求期限
                        if (lstMRD[j].CommitDate != null)
                        {
                            string CommitDate = string.Format("{0:yyyy年MM月dd}", lstMRD[j].CommitDate);
                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue(CommitDate.Substring(0, CommitDate.Length - 3) + "月份");
                        }
                        else
                        {
                            worksheets[i].Cells[rowStartR, colStart + 14].PutValue("---");
                        }
                    }

                    if (lstMRD[j].ReturnType != 0)
                    {
                        //承诺补回时间
                        string PromissDate = string.Empty;
                        if (lstMRD[j].PromissDate != null)
                        {
                            worksheets[i].Cells[rowStartR, colStart + 12].PutValue(lstMRD[j].PromissDate.Value.ToString("yyyy-MM-dd")); //承诺补回时间    
                        }

                        worksheets[i].Cells[rowStartR, colStart + 13].PutValue(lstMRD[j].ReturnDescription); //补回情况                                
                        worksheets[i].Cells[rowStartR, colStart + 15].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), lstMRD[j].ReturnType)); //补回状态
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 15].PutValue("---");//补回状态
                    }


                    if (lstMRD[j].Counter > 0)
                    {
                        //警示灯
                        int pictureIndex = worksheets[i].Pictures.Add(rowStartR, colStart + 16, ImageFilePath + "\\image" + lstMRD[j].Counter + ".png");
                        Aspose.Cells.Drawing.Picture picture = worksheets[i].Pictures[pictureIndex];
                        picture.Left = 40;
                        picture.Top = 10;
                    }
                    else
                    {
                        worksheets[i].Cells[rowStartR, colStart + 16].PutValue("---");
                    }
                    #endregion
                    rowStartR++;
                    rowStart = rowStartR;
                }
            }
            worksheets[0].Protect(ProtectionType.Contents);
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







        //上报简报
        private void DownTargetSimpleRpt()
        {
            string templetePath = ExcelTempletePath;
            string templeteName = "月度经营简报模版.xlsx";
            string fileName = "月度经营简报";

            ExcelTargetSimpleRpt(smodel, templetePath, templeteName, fileName,FinYear, FinMonth);
        }

        public void ExcelTargetSimpleRpt(SimpleReportVModel smodel, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;

            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];



            #region style1 样式,无边框
            style1.Font.Size = 16;
            style1.Font.Name = "Arial";
            //style2.Font.IsBold = true;
            //style2.ForegroundColor = System.Drawing.Color.FromArgb(253, 225, 160);

            #endregion

            #region style2 样式
            style2.Font.Size = 16;
            style2.Font.Name = "Arial";

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

            int nindex = 0;
            for (int i = 0; i < smodel.List.Count; i++)
            {
                SRptModel m = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailSimpleRpt(smodel.List[i]);
                List<B_MonthlyReportDetail> cdetails = new List<B_MonthlyReportDetail>();
                List<B_MonthlyReportDetail> tdetails = new List<B_MonthlyReportDetail>();
                for (int j = 0; j < m.CurrentList.Count;j++ )
                {
                    B_MonthlyReportDetail BMRD=TargetEvaluationEngine.TargetEvaluationService.Calculation(m.CurrentList[j], false);
                    if (BMRD.NAccumulativeActualAmmount == 0 && BMRD.NAccumulativePlanAmmount == 0)
                    {
                        BMRD.NAccumulativeDisplayRate = "--";
                    }
                    if (BMRD.NActualAmmount == 0 && BMRD.NPlanAmmount == 0)
                    {
                        BMRD.NDisplayRate = "--";
                    }
                    cdetails.Add(BMRD);
                }
                for(int k=0;k<m.TotalList.Count;k++)
                {
                    B_MonthlyReportDetail BMRD = TargetEvaluationEngine.TargetEvaluationService.Calculation(m.TotalList[k], false);
                    if (BMRD.NAccumulativeActualAmmount == 0 && BMRD.NAccumulativePlanAmmount == 0)
                    {
                        BMRD.NAccumulativeDisplayRate = "--";
                    }
                    if (BMRD.NActualAmmount == 0 && BMRD.NPlanAmmount == 0)
                    {
                        BMRD.NDisplayRate = "--";
                    }
                    tdetails.Add(BMRD);
                }
                m.CurrentList = cdetails;
                m.TotalList = tdetails;
                nindex = designer.Workbook.Worksheets.AddCopy(0);
                string month = smodel.List[i].Month == "1" ? "1" : "1-" + smodel.List[i].Month + "月";
                designer.SetDataSource("Time", "报告期：" + month);
                designer.SetDataSource("Unit", "单位：" + smodel.List[i].Unit);
                designer.SetDataSource("Name", smodel.List[i].Name + "简报-累计");
                designer.SetDataSource("entry", m.TotalList);
                designer.Workbook.Worksheets[nindex].Name = smodel.List[i].Name + "简报-累计";
                designer.Process(nindex, true);
                nindex = designer.Workbook.Worksheets.AddCopy(1);
                designer.SetDataSource("Time", "报告期：" + smodel.List[i].Month + "月");
                designer.SetDataSource("Unit", "单位：" + smodel.List[i].Unit);
                designer.SetDataSource("Name", smodel.List[i].Name + "简报-当月");
                designer.SetDataSource("Aentry", m.CurrentList);
                designer.Workbook.Worksheets[nindex].Name = smodel.List[i].Name + "简报-当月";
                designer.Process(nindex, true);
                designer.ClearDataSource();
            }
            designer.Workbook.Worksheets.RemoveAt(0);
            designer.Workbook.Worksheets.RemoveAt(0);
            designer.Process();

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
       

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }



    }






}