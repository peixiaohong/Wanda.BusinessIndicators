
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
using System.Text;
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownMonthRptFileList 的摘要说明
    /// </summary>
    public partial class DownMonthRptFileList : IHttpHandler, IRequiresSessionState
    {
        private string _excelTempletePath = null;
        Guid SysDescriptionID = WebConfigurationManager.AppSettings["MonthDescription"].ToGuid();//百货系统ID
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
        List<B_Attachment> Attachment = new List<B_Attachment>();
        string param;
        string[] strID = new string[1];//接多个系统ID
        string OrderStr = "Detail";//明细下载排序字段,默认是按照累计排序的
        string OrderStrTwo = "DetailMonthly";//明细下载排序字段,默认是按照当月排序的
        bool IsBlendTargets = false;//是否混合指标
        bool isHaveArea = false;//板块下是否存在区域
        bool IsAll = false;//是否获取全部数据（经营月报查询获取全部数据，月报上报获取非全部数据）
        string DataSource = string.Empty; //B表数据源
        Guid TargetPlanID = Guid.Empty;//指标版本ID
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            string param = HttpContext.Current.Request.QueryString["Param"];
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

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsAll"]))
            {
                IsAll = bool.Parse(HttpContext.Current.Request["IsAll"].ToString());
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["IsLatestVersion"]))
            {
                if (HttpContext.Current.Request["IsLatestVersion"] == "true")
                {
                    IsLatestVersion = true;
                }
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["TargetPlanID"]))
            {
                TargetPlanID = HttpContext.Current.Request["TargetPlanID"].ToGuid();
            }
            List<StreamModel> Models = new List<StreamModel>();
            var datetime = DateTime.Now;
            C_System Sys = C_SystemOperator.Instance.GetSystem(SysId, datetime);
            IsBlendTargets = GetIsBlendTargets(Sys.Configuration);
            for (int i = 0; i < 1; i++)
            {
                if (MonthlyReportID == Guid.Empty)
                {
                    rpt = new ReportInstance(SysId, FinYear, FinMonth, TargetPlanID, IsLatestVersion, DataSource, IsAll);
                }
                else
                {
                    rpt = new ReportInstance(MonthlyReportID, true, DataSource, IsAll);
                    FinMonth = rpt.FinMonth;
                    FinYear = rpt.FinYear;
                    SysId = rpt._SystemID;
                }
                ReportInstance rpt_Sum = new ReportInstance();
                rpt_Sum = rpt;
                ReportInstance rpt_Detail = new ReportInstance();
                rpt_Detail = rpt;

                //经营报告明细
                ReportInstance rpt_ManageDetailTarget = null;
                isHaveArea = StaticResource.Instance.GetSystem_Regional(SysId);
                if (isHaveArea)
                {
                    rpt_ManageDetailTarget = rpt;
                }

                //这里的未完成和 补回不能用一个 ReportInstance 对象，引用会把集合的对象删除，导致数据错误。
                ReportInstance rpt_MissTarget = new ReportInstance();
                //这里的未完成和 补回不能用一个 ReportInstance 对象，引用会把集合的对象删除，导致数据错误。

                ReportInstance rpt_CurrentMissTarget = new ReportInstance();
                //if (rpt._MonthReportID != Guid.Empty)
                //{
                    //rpt_MissTarget = new ReportInstance(rpt._MonthReportID, true, "Progress", IsAll);
                    //rpt_CurrentMissTarget = new ReportInstance(rpt._MonthReportID, true, "Progress", IsAll);
                    rpt_MissTarget = new ReportInstance(SysId, FinYear, FinMonth, TargetPlanID, IsLatestVersion, DataSource, IsAll);
                    rpt_CurrentMissTarget = new ReportInstance(SysId, FinYear, FinMonth, TargetPlanID, IsLatestVersion, DataSource, IsAll);
                //}
                //else
                //{
                //rpt_MissTarget = rpt;
                //    rpt_CurrentMissTarget = rpt;
                //}

                ReportInstance rpt_Return = new ReportInstance();
                rpt_Return = rpt;

                //查找附件表中是否有上传的附件
                Attachment = B_AttachmentOperator.Instance.GetAttachmentList(rpt._MonthReportID, "月报上传").ToList();
                if (Attachment.Count() > 0)
                {
                    //对获取的附件进行分组操作
                    var AttList = Attachment.GroupBy(o => o.FileName);
                    foreach (var item in AttList)
                    {
                        //查询
                        var Attcount = Attachment.Where(o => o.FileName == item.Key).ToList();
                        if (Attcount.Count() > 1)
                        {
                            for (int a = 0; a < Attcount.Count(); a++)
                            {
                                if (a == 0)
                                {
                                    StreamModel streamAtt = new StreamModel();
                                    streamAtt.Atturl = Attcount[a].Url;
                                    streamAtt.Attfilename = Attcount[a].FileName;
                                    Models.Add(streamAtt);
                                }
                                else
                                {
                                    StreamModel streamAtt = new StreamModel();
                                    streamAtt.Atturl = Attcount[a].Url;
                                    var filebd = Attcount[a].FileName.Split('.')[0];
                                    var fh = "(" + a + ")" + " .";
                                    var filebd1 = Attcount[a].FileName.Split('.')[1];
                                    var fileies = filebd + fh + filebd1;
                                    streamAtt.Attfilename = fileies;
                                    Models.Add(streamAtt);
                                }
                            }
                        }
                        else
                        {
                            for (int a = 0; a < Attcount.Count(); a++)
                            {
                                StreamModel streamAtt = new StreamModel();
                                streamAtt.Atturl = Attcount[a].Url;
                                streamAtt.Attfilename = Attcount[a].FileName;
                                Models.Add(streamAtt);
                            }
                        }
                    }
                }
                try
                {
                    if (Sys.Category == 2)
                    {
                        //Models.Add(DownMissTarget(rpt_MissTarget)); //下载未完成说明模版
                        //Models.Add(DownTarget_Detail(rpt_Detail));      //下载指标明细模版
                        //                                                // Models.Add(DownTarget_DetailMonthly(rpt_Detail)); //下载指标明细（当月）模板
                        //Models.Add(DownTarget_Return(rpt_Return)); //下载补回明细模版
                        //Models.Add(DownCurrentMissTarget(rpt_CurrentMissTarget)); //下载当月未完成说明模版
                    }
                    else if (Sys.Category == 3)
                    {
                        //Models.Add(DownTarget_Summary(rpt_Sum));   //下载月度报告模版
                        //Models.Add(DownTarget_Detail(rpt_Detail));      //下载指标明细模版
                        ////Models.Add(DownTarget_DetailMonthly(rpt_Detail)); //下载指标明细（当月）模板

                    }
                    else if (Sys.Category == 4)
                    {
                        Models.Add(DownTarget_Summary(rpt_Sum));   //下载月度报告模版
                        Models.Add(DownMissTarget(rpt_MissTarget)); //下载未完成说明模版
                        Models.Add(DownCurrentMissTarget(rpt_CurrentMissTarget)); //下载当月未完成说明模版
                        Models.Add(DownTarget_Return(rpt_Return)); //下载补回明细模版
                    }
                    else if (Sys.ID == ConfigurationManager.AppSettings["MonthDescription"].ToGuid())
                    {
                        //Models.Add(DownTarget_Summary(rpt_Sum));   //下载月度报告模版
                        //Models.Add(DownMissTarget(rpt_MissTarget)); //下载未完成说明模版
                        //Models.Add(DownTarget_Detail(rpt_Detail));      //下载指标明细模版
                        //Models.Add(DownTarget_DetailMonthly(rpt_Detail)); //下载指标明细（当月）模板
                        //Models.Add(DownTarget_Return(rpt_Return)); //下载补回明细模版
                        //Models.Add(DownCurrentMissTarget(rpt_CurrentMissTarget)); //下载当月未完成说明模版
                        //Models.Add(DownLoadDSTargetReport(param, FinYear, FinMonth));//百货系统-其它情况-完成门店数量下载
                        //Models.Add(DownLoadDSTargetCompletedReport(param, FinYear, FinMonth));//百货系统-其它情况-完成情况明细
                        //Models.Add(DownLoadDSTargetReturnDataReport(param, FinYear, FinMonth));//百货系统-其它情况-补回上月缺口
                        //Models.Add(DownLoadDSTargetAddDataReport(param, FinYear, FinMonth));//百货系统-其它情况-新增未完成门店
                    }
                    else
                    {
                        //混合指标
                        if (IsBlendTargets)
                        {
                            Models.Add(DownTarget_BlendTargetDetail(rpt_Detail));      //混合指标-下载指标明细模版
                            //Models.Add(DownTarget_BlendDetailMonthly(rpt_Detail)); //混合指标-下载指标明细（当月）模板
                            if (isHaveArea)
                            {
                                //经营报告明细
                                Models.Add(DownTarget_BlendManageReportTargetDetail(rpt_Detail));      //下载混合指标明细模版
                            }
                        }
                        else
                        {
                            Models.Add(DownTarget_Detail(rpt_Detail));      //下载指标明细模版
                            //Models.Add(DownTarget_DetailMonthly(rpt_Detail)); //下载指标明细（当月）模板
                            if (isHaveArea)
                            {
                                //经营报告明细
                                Models.Add(DownTarget_ManageReportTargetDetail(rpt_Detail));      //下载单指标明细模版
                            }
                        }
                        Models.Add(DownTarget_Summary(rpt_Sum));   //下载月度报告模版
                        Models.Add(DownMissTarget(rpt_MissTarget)); //下载未完成说明模版
                        Models.Add(DownTarget_Return(rpt_Return)); //下载补回明细模版
                        Models.Add(DownCurrentMissTarget(rpt_CurrentMissTarget)); //下载当月未完成说明模版
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            var zip = ZipFile(Models, Sys.SystemName + "_" + DateTime.Now.ToString("yyyy-MM-dd"), 2000);
            if (zip != null)
            {
                DownloadByte(zip, ".zip", Sys.SystemName + "_" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
        }
        public byte[] ZipFile(List<StreamModel> atts, string zipedFile, int blockSize)
        {
            C_System Sys = new C_System();
            string url = string.Empty;
            string fileName = string.Empty;
            string filePath = string.Empty;

            try
            {
                //文件临时存储地址
                string tmpFilePath = System.IO.Path.GetTempPath() + "\\" + Guid.NewGuid().ToString();
                //压缩包地址
                string tmpZipFilePath = tmpFilePath + "\\" + zipedFile + ".zip";

                using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(Encoding.GetEncoding("utf-8")))
                {
                    foreach (var itemKey in atts)
                    {
                        string path = "";
                        Sys = C_SystemOperator.Instance.GetSystem(SysId);
                        //获取系统名称
                        var folderName = string.Format("{0}", Sys.SystemName);
                        List<string> files = new List<string>();
                        //生成压缩包里文件夹。且判断其中是否有相同名称的文件名称
                        string newPath = Path.Combine(tmpZipFilePath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            //不相同时创建一个该系统名称的文件夹。
                            if (path != newPath && !path.Contains(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                                path += newPath;
                                zipFile.AddDirectory(newPath, folderName);
                            }
                        }
                        else
                        {
                            //有相同名称时，将文件流更新到文件夹里
                            zipFile.UpdateDirectory(newPath, folderName);
                        }
                        //获取文件名称
                        string ext = "xls";
                        if (!string.IsNullOrEmpty(itemKey.FileExt))
                        {
                            ext = itemKey.FileExt;
                        }
                        //判断是否是上传的附件。
                        if (itemKey.Atturl != null)
                        {
                            fileName = itemKey.Attfilename;
                            //生成存放文件的路径
                            filePath = Path.Combine(newPath, fileName);
                            var urlRonte = WebConfigurationManager.AppSettings["UploadFilePath"];
                            url = Path.Combine(urlRonte, itemKey.Atturl);
                        }
                        else
                        {
                            fileName = string.Format("{0}.{1}", itemKey.FileName, ext);
                            //生成存放文件的路径
                            filePath = Path.Combine(newPath, fileName);
                        }

                        try
                        {
                            if (itemKey.Atturl != null)
                            {
                                //读取文件流
                                FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read);
                                byte[] bte = new byte[fs.Length];
                                fs.Read(bte, 0, bte.Length);
                                // 设置当前流的位置为流的开始
                                //itemKey.Streams.Seek(0, SeekOrigin.Begin);
                                File.WriteAllBytes(filePath, bte);
                                zipFile.AddFile(filePath, folderName);

                                files.Add(fileName);
                            }
                            else
                            {
                                //byte[] bte = new byte[fs.Length];
                                //itemKey.Streams.Read(bte, 0, bte.Length);
                                // 设置当前流的位置为流的开始
                                itemKey.Streams.Seek(0, SeekOrigin.Begin);
                                File.WriteAllBytes(filePath, itemKey.Streams.ToArray());
                                zipFile.AddFile(filePath, folderName);

                                files.Add(fileName);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            if (itemKey.Streams != null)
                            {
                                //关闭资源
                                itemKey.Streams.Close();
                            }
                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    zipFile.Save(stream);

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private void DownloadByte(byte[] bytes, string fileExt, string fileName = "NoTitle")
        {

            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.Buffer = true;
            context.Response.Charset = "utf-8";
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + fileExt);
            context.Response.ContentEncoding = System.Text.Encoding.Default;
            if (fileExt == ".xls")
            {
                context.Response.ContentType = "application/vnd.ms-excel";
            }
            else if (fileExt == ".xlsx")
            {
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else
            {
                context.Response.ContentType = "application/unknow";
            }
            context.Response.BinaryWrite(bytes);
            context.Response.End();
        }


        /// <summary>
        /// 读取xml文件（ComplateTargetDetail.xml）
        /// </summary>
        /// <returns>是否为混合指标</returns>
        private bool GetIsBlendTargets(XElement xelement)
        {
            List<VTarget> targetList = new List<VTarget>();

            XElement elementCTD = xelement;
            if (elementCTD.Elements("ComplateTargetDetail").Elements("BlendTargets").Count() > 0)
            {
                return true;
            }
            return false;
        }

        #region 百货系统其它情况下载
        /// <summary>
        /// 设置Excel中的Cell的样式
        /// </summary>
        /// <param name="designer"></param>
        /// <param name="IsSetBackground"></param>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Bule"></param>
        /// <returns></returns>
        private Aspose.Cells.Style SetCellStyle(WorkbookDesigner designer, bool IsSetBackground, int Red, int Green, int Bule, bool IsTitle)
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
        private StreamModel DownLoadDSTargetReport(string param, int FinYear, int FinMonth)
        {
            StreamModel stream = new StreamModel();
            string fileName = String.Format("百货系统经营指标完成门店数量情况({0}月)", FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成门店数量情况.xls";
            if (FinMonth == 1)
            {
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成门店数量情况(1月).xls";
            }
            //FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(filePath);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);

            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0, false);
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
            List<ShowDSTargetCompleted> list = GetDSTargetCompleted(FinYear, FinMonth, Convert.ToBoolean(param));
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

            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            designer.Workbook.Save(stream.Streams, xls);
            stream.FileExt = "xls";
            stream.FileName = fileName;
            return stream;
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
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).ToList();
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
        private StreamModel DownLoadDSTargetCompletedReport(string param, int FinYear, int FinMonth)
        {
            StreamModel stream = new StreamModel();
            string fileName = String.Format("百货系统经营指标完成情况对比({0}月)", FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成情况对比.xls";
            if (FinMonth == 1)
            {
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统经营指标完成情况对比(1月).xls";
            }
            //FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(filePath);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);
            List<ShowDSTargetArea> list = GetDSTargetCompletedDetail(FinYear, FinMonth, Convert.ToBoolean(param));
            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0, false);
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
                sheet.Cells[1, 1].PutValue(string.Format("报告期：{0}年1~{1}月", FinYear, FinMonth));
                sheet.Cells[2, 3].PutValue(string.Format("{0}月", FinMonth - 1));
                sheet.Cells[2, 7].PutValue(string.Format("{0}月", FinMonth));
                sheet.Cells[2, 11].PutValue(string.Format("1-{0}月累计", FinMonth));
            }
            //给excel赋值
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).ToList();
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
            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            designer.Workbook.Save(stream.Streams, xls);
            stream.FileExt = "xls";
            stream.FileName = fileName;
            return stream;
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
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).ToList();
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
        public StreamModel DownLoadDSTargetReturnDataReport(string param, int FinYear, int FinMonth)
        {
            StreamModel stream = new StreamModel();
            string fileName = String.Format("百货系统补回{0}月经营指标缺口情况({1}月)", FinMonth - 1, FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标缺口情况.xls";
            if (FinMonth == 1)
            {
                fileName = String.Format("百货系统1月经营指标缺口情况({0}月)", FinMonth.ToString());
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标缺口情况(1月).xls";
            }
            //FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(filePath);
            Worksheet sheet = designer.Workbook.Worksheets[0];
            ExcelHelper excelHelper = new ExcelHelper(sheet);
            #region 设置样式
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0, false);
            Aspose.Cells.Style style2 = SetCellStyle(designer, true, 184, 204, 228, true);
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
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).ToList();
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
            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            designer.Workbook.Save(stream.Streams, xls);
            stream.FileExt = "xls";
            stream.FileName = fileName;
            return stream;
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
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
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
        public StreamModel DownLoadDSTargetAddDataReport(string param, int FinYear, int FinMonth)
        {
            StreamModel stream = new StreamModel();
            string fileName = String.Format("百货系统{0}月经营指标新增情况({1}月)", FinMonth.ToString(), FinMonth.ToString());
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标新增情况.xls";
            if (FinMonth == 1)
            {
                filePath = HttpContext.Current.Request.PhysicalApplicationPath + @"Excel\百货系统补回一季度经营指标新增情况(1月).xls";
            }
            //FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(filePath);
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
            Aspose.Cells.Style style = SetCellStyle(designer, false, 0, 0, 0, false);
            Aspose.Cells.Style style2 = SetCellStyle(designer, true, 184, 204, 228, true);
            #endregion
            List<DSTargetReturnDataCompany> list = GetDSTargetAddMissDataList(FinYear, FinMonth, Convert.ToBoolean(param));
            C_System SystemModel = C_SystemOperator.Instance.GetSystem(SysDescriptionID);
            if (SystemModel != null)
            {
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).ToList();
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
            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            designer.Workbook.Save(stream.Streams, xls);
            stream.FileExt = "xls";
            stream.FileName = fileName;
            return stream;
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
                List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemModel.ID, DateTime.Now).Where(t => t.NeedReport == true && t.TargetName != "总部费用").ToList();
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
        #endregion

        //未完成说明模版（上报）
        private StreamModel DownMissTargetRpt()
        {
            //根据系统自然月，自动计算

            FinYear = DateTime.Now.Year;
            FinMonth = DateTime.Now.Month - 1;
            StreamModel stream = new StreamModel();
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

            stream.Streams = ExcelMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, true);
            stream.FileName = fileName;
            return stream;
        }

        /// <summary>
        /// 下载完成情况明细报表
        /// </summary>
        private StreamModel DownTarget_Detail(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细";
                    stream.Streams = DownExcelMonthReportDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "项目公司完成明细模版V1.xlsx";
                    fileName = "完成情况明细";

                    stream.Streams = DownExcelMonthReportDetail_XM(rpt, templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 3:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-集团总部V1.xlsx";
                    fileName = "完成情况明细";
                    stream.Streams = DownExcelMonthReportDetail_Group(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细";
                    stream.Streams = DownExcelMonthReportDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }

        /// <summary>
        /// 下载完成情况明细报表（当月）
        /// </summary>
        private StreamModel DownTarget_DetailMonthly(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            //仅经营系统需要当月和累计。此方法只为经营系统添加一个当月完成情况明细报表
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细（当月）";
                    stream.Streams = DownExcelMonthReportDetailMonthly_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细（当月）";
                    stream.Streams = DownExcelMonthReportDetailMonthly_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }


        /// <summary>
        /// 混合指标-下载完成情况明细报表
        /// </summary>
        private StreamModel DownTarget_BlendTargetDetail(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-混合指标V1.xlsx";
                    fileName = "完成情况明细";
                    stream.Streams = DownExcelMonthReportBlendTargetDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "项目公司完成明细模版V1.xlsx";
                    fileName = "完成情况明细";

                    stream.Streams = DownExcelMonthReportDetail_XM(rpt, templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 3:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-集团总部V1.xlsx";
                    fileName = "完成情况明细";
                    stream.Streams = DownExcelMonthReportDetail_Group(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-混合指标V1.xlsx";
                    fileName = "完成情况明细";
                    stream.Streams = DownExcelMonthReportDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }


        /// <summary>
        /// 混合指标-下载完成情况明细报表（当月）
        /// </summary>
        private StreamModel DownTarget_BlendDetailMonthly(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            //仅经营系统需要当月和累计。此方法只为经营系统添加一个当月完成情况明细报表
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-混合指标V1.xlsx";
                    fileName = "完成情况明细（当月）";
                    stream.Streams = DownExcelMonthReportDetailBlendMonthly_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-混合指标V1.xlsx";
                    fileName = "完成情况明细（当月）";
                    stream.Streams = DownExcelMonthReportDetailBlendMonthly_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }

        /// <summary>
        /// 单指标-下载经营报告明细报表
        /// </summary>
        private StreamModel DownTarget_ManageReportTargetDetail(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-单指标V1.xlsx";
                    fileName = "经营报告明细";
                    stream.Streams = DownExcelManageReportTargetDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "项目公司完成明细模版V1.xlsx";
                    fileName = "经营报告明细";

                    stream.Streams = DownExcelMonthReportDetail_XM(rpt, templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 3:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-集团总部V1.xlsx";
                    fileName = "经营报告明细";
                    stream.Streams = DownExcelMonthReportDetail_Group(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-单指标V1.xlsx";
                    fileName = "经营报告明细";
                    stream.Streams = DownExcelManageReportTargetDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }

        /// <summary>
        /// 混合指标-下载经营报告明细报表
        /// </summary>
        private StreamModel DownTarget_BlendManageReportTargetDetail(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-混合指标V1.xlsx";
                    fileName = "经营报告明细";
                    stream.Streams = DownExcelManageReportBlendTargetDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 2:
                    templetePath = ExcelTempletePath;
                    templeteName = "项目公司完成明细模版V1.xlsx";
                    fileName = "经营报告明细";

                    stream.Streams = DownExcelMonthReportDetail_XM(rpt, templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 3:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-集团总部V1.xlsx";
                    fileName = "经营报告明细";
                    stream.Streams = DownExcelMonthReportDetail_Group(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板-混合指标V1.xlsx";
                    fileName = "经营报告明细";
                    stream.Streams = DownExcelMonthReportDetail_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }

        /// <summary>
        /// 混合指标-下载完成情况明细报表（当月）
        /// </summary>
        private StreamModel DownTarget_BlendTargetDetailMonthly(ReportInstance rpt)
        {
            //系统类型, 1:经营系统 2：项目公司系统 3：集团直属部门 4：集团直属公司
            //仅经营系统需要当月和累计。此方法只为经营系统添加一个当月完成情况明细报表
            string templetePath = string.Empty;
            string templeteName = string.Empty;
            string fileName = string.Empty;
            StreamModel stream = new StreamModel();
            switch (rpt._System.Category)
            {
                case 1:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细（当月）";
                    stream.Streams = DownExcelMonthReportDetailBlendMonthly_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "完成情况明细模板V1.xlsx";
                    fileName = "完成情况明细（当月）";
                    stream.Streams = DownExcelMonthReportDetailMonthly_JY(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
        }



        /// <summary>
        /// 未完成说明（月度报告）
        /// </summary>
        private StreamModel DownMissTarget(ReportInstance rpt)
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
            StreamModel stream = new StreamModel();
            C_System sys = StaticResource.Instance[SysId, Dt];
            switch (sys.Category)
            {
                case 1:
                case 2:
                    templetePath = ExcelTempletePath;

                    templeteName = "未完成说明数据模版V1.xlsx";
                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "累计未完成说明";
                    else
                        fileName = "累计未完成说明";
                    List<DictionaryVmodel> targetReturn = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                    stream.Streams = ExcelMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
                case 3:
                    break;
                case 4:
                    templetePath = ExcelTempletePath;
                    templeteName = "未完成说明数据模版V1.xlsx";
                    if (rpt._System != null)
                        fileName = rpt._System.SystemName + "累计未完成说明";
                    else
                        fileName = "累计未完成说明";
                    List<DictionaryVmodel> DirectlyMissTarget = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt);
                    stream.Streams = ExcelMisssTarget_Directly(rpt, DirectlyMissTarget, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;

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
        public MemoryStream ExcelMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();
            string path = System.IO.Path.Combine(templetePath, templeteName);

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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

                                        if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                        }
                                        else
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                        }
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
                                    worksheets[i].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.MIssTargetReason) + "\n\r采取措施:" + _TrimStr(p.MIssTargetDescription)); //未完成原因,及采取措施
                                }

                                if (p.CommitDate != null)
                                {
                                    string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                                    if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                    }
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
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;
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
        public MemoryStream ExcelMisssTarget_Directly(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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

                                if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                {
                                    worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                }
                                else
                                {
                                    worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                }
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
                            worksheets[0].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.MIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.MIssTargetDescription)); //未完成原因,及采取措施
                        }

                        if (p.CommitDate != null)
                        {
                            string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);

                            if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                            {
                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                            }
                            else
                            {
                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                            }
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
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);

            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;
        }

        /// <summary>
        /// 当月未完成说明（月度报告）
        /// </summary>
        private StreamModel DownCurrentMissTarget(ReportInstance rpt)
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
            StreamModel stream = new StreamModel();
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
                    stream.Streams = ExcelCurrentMisssTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
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
                    stream.Streams = ExcelCurrentMisssTarget_Directly(rpt, DirectlyMissTarget, templetePath, templeteName, fileName, FinYear, FinMonth, false);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
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
        public MemoryStream ExcelCurrentMisssTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            //FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(path);
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

                                            if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                            {
                                                worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                            }
                                            else
                                            {
                                                worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                            }
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
                                        worksheets[i].Cells[rowStartR, colStart + 10].PutValue("未完成原因:\n\r" + _TrimStr(p.CurrentMIssTargetReason) + "\n\r采取措施:\n\r" + _TrimStr(p.CurrentMIssTargetDescription)); //未完成原因,及采取措施
                                    }

                                    if (p.CommitDate != null)
                                    {
                                        string CommitDate = string.Format("{0:yyyy年MM月dd}", p.CommitDate);
                                        if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                        }
                                        else
                                        {
                                            worksheets[i].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                        }
                                       
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

                                    if (p.CommitDate != null && p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 12].PutValue(p.ReturnDescription);
                                    }
                                    else
                                    {
                                        worksheets[i].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
                                    }

                                    //  worksheets[i].Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
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

            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);

            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;
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
        public MemoryStream ExcelCurrentMisssTarget_Directly(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth, bool IsRpt)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

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

                                if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                {
                                    worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                }
                                else
                                {
                                    worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                }

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
                            if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                            {
                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                            }
                            else
                            {
                                worksheets[0].Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                            }
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
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);

            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

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
                                    if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                    {
                                        worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                    }
                                    else
                                    {
                                        worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                    }
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

                                if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                {
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                }
                                else
                                {
                                    worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                }
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
                            // worksheet.Cells[rowStartR, colStart + 12].PutValue(EnumHelper.GetEnumDescription(typeof(EnumReturnType), p.ReturnType) + "\n\r" + p.ReturnDescription);
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


        private StreamModel DownTarget_Summary(ReportInstance rpt)
        {
            StreamModel stream = new StreamModel();
            if (SysId == ConfigurationManager.AppSettings["MonthSG"].ToGuid())
            {
                string templetePath = ExcelTempletePath;
                string templeteName = "月度经营报告_商管系统查询V1.xlsx";
                string fileName = "月度报告";
                stream = DownExcelMonthReportSummary(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                return stream;
            }
            else
            {
                string templetePath = ExcelTempletePath;
                string templeteName = "月度经营报告V1.xlsx";
                string fileName = "月度报告";
                stream = DownExcelMonthReportSummary(templetePath, templeteName, fileName, SysId, FinYear, FinMonth, IsLatestVersion);
                return stream;
            }



        }


        //读取webconfig的值
        private string siteUrl = ConfigurationManager.AppSettings["SiteURL"];
        /// <summary>
        /// 下载Excel月度经营报告
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public StreamModel DownExcelMonthReportSummary(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            string str = HttpContext.Current.Server.MapPath("../");
            string Title = FinYear.ToString() + "年" + FinMonth + "月";
            List<DictionaryVmodel> listDVM = null;
            if (rpt != null)
            {
                listDVM = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false); // .GetSummaryRptDataSource(out strs, false);

                //判断当前系统是否有自己的模板
                if (!string.IsNullOrEmpty(listDVM[2].HtmlTemplate))
                {
                    string[] strHtmlTemplates = listDVM[2].HtmlTemplate.Split(',');
                    if (!string.IsNullOrEmpty(strHtmlTemplates[2]))
                    {
                        templeteName = strHtmlTemplates[2];
                    }
                }
            }
            List<MonthReportSummaryViewModel> list = (List<MonthReportSummaryViewModel>)listDVM[2].ObjValue;

            if (sytemID != ConfigurationManager.AppSettings["MonthSG"].ToGuid())
            {
                list.ForEach(p =>
                {
                    double temp = 0;
                    if (double.TryParse(p.MeasureRate, out temp))
                    {
                        p.MeasureRate1 = Convert.ToDouble(p.MeasureRate);
                    }

                });
            }
            else
            {
                list.ForEach(p =>
                {

                    //因租金收缴率使用String类型，将其余几个项目也如此赋值。
                    if (p.TargetID != ConfigurationManager.AppSettings["MonthSGRent"].ToGuid())
                    {
                        p.NPlanStr = string.Format("{0:N2}", p.NPlanAmmount);
                        p.NActualStr = string.Format("{0:N2}", p.NActualAmmount);
                        p.NAccumulativePlanStr = string.Format("{0:N2}", p.NAccumulativePlanAmmount);
                        p.NAccumulativeActualStr = string.Format("{0:N2}", p.NAccumulativeActualAmmount);
                        p.NDifferStr = string.Format("{0:N2}", p.NDifference);
                        p.NAccDiffereStr = string.Format("{0:N2}", p.NAccumulativeDifference);
                        double temp = 0;
                        if (double.TryParse(p.MeasureRate, out temp))
                        {
                            p.MeasureRate1 = Convert.ToDouble(p.MeasureRate);
                        }
                        p.MeasureRate = string.Format("{0:N2}", p.MeasureRate1);
                    }
                    else if (p.TargetID == ConfigurationManager.AppSettings["MonthSGRent"].ToGuid())
                    {
                        if (p.NDifference == 0)
                        {
                            p.NDifferStr = string.Format("{0:N0}", p.NDifference) + "\r\n (无欠收)";
                        }
                        else
                        {
                            p.NDifferStr = string.Format("{0:N2}", p.NDifference) + "\r\n (当月欠收)";
                        }

                        if (p.NAccumulativeDifference == 0)
                        {
                            p.NAccDiffereStr = string.Format("{0:N0}", p.NAccumulativeDifference) + "\r\n (无欠收)";
                        }
                        else
                        {
                            p.NAccDiffereStr = string.Format("{0:N2}", p.NAccumulativeDifference) + "\r\n (累计欠收)";
                        }
                    }



                });
            }

            StreamModel stream = ExportExcels(list, "MonthReportSummary", templetePath, templeteName, rpt);
            return stream;

        }

        /// <summary>
        /// 导出数据生成文件流下载
        /// </summary>
        /// <param name="list">导出的数据列表</param>
        /// <param name="listName">列表名</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        public StreamModel ExportExcels<T>(List<T> list, string listName, string templetePath, string templeteName, ReportInstance rpt)
        {

            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();
            string path = System.IO.Path.Combine(templetePath, templeteName);
            //using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            //{

            designer.Workbook = new Workbook(path);

            designer.SetDataSource("Title", rpt._System.SystemName + "月度报告");
            designer.SetDataSource("Date", "报告期：" + rpt.FinYear.ToString() + "年1-" + rpt.FinMonth.ToString() + "月");
            designer.SetDataSource("Date_Group", rpt.FinYear.ToString() + "年" + rpt.FinMonth.ToString() + "月");


            designer.SetDataSource(listName, list);
            designer.Process();
            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);

            //designer.Workbook.SaveToStream();
            stream.FileName = rpt._System.SystemName + "月度报告";
            //designer.Workbook.Save("d:\\abc.xlsx");
            stream.FileExt = "xlsx";
            //fileStream.Close();
            //fileStream.Dispose();
            return stream;

            //}

        }
        //下载补回情况列表
        private StreamModel DownTarget_Return(ReportInstance rpt)
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
            StreamModel stream = new StreamModel();
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
                    stream.Streams = ExcelReturnTarget(rpt, targetReturn, templetePath, templeteName, fileName, FinYear, FinMonth);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
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
                    stream.Streams = ExcelDirectlyReturnTarget(rpt, targetReturn, directlyTempletePath, directlytempleteName, fileName, FinYear, FinMonth);
                    stream.FileName = fileName;
                    stream.FileExt = "xlsx";
                    break;
            }
            return stream;
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
        public MemoryStream ExcelReturnTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);
            StreamModel stream = new StreamModel();
            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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

            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            return stream.Streams;
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
        public MemoryStream ExcelDirectlyReturnTarget(ReportInstance rpt, List<DictionaryVmodel> List, string templetePath, string templeteName, string fileName, int FinYear, int FinMonth)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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
            stream.Streams = new MemoryStream();

            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;
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

                                    if (p.CommitDate.Value.Month == 12 && p.CommitDate.Value.Day == 31)
                                    {
                                        worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitReason);
                                    }
                                    else
                                    {
                                        worksheet.Cells[rowStartR, colStart + 11].PutValue(p.CommitDate.Value.Month.ToString() + "月");// CommitDate.Substring(0, CommitDate.Length - 3)
                                    }
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
        public MemoryStream DownExcelMonthReportDetail_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

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

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

        }

        /// <summary>
        /// 下载Excel混合指标-完成情况明细--经营系统
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public MemoryStream DownExcelMonthReportBlendTargetDetail_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            if (rpt._System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
            {
                string strXml = rpt.GetComplateMonthReportDetailHtmlTemplate(rpt._System.Configuration);
                string[] strXmls = strXml.Split(',');
                if (strXmls.Length > 1)
                {
                    templeteName = !string.IsNullOrEmpty(strXmls[3]) ? strXmls[3] : "完成情况明细模板-混合指标V1.xlsx";
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style5 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
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

            #region style5样式
            style5.Font.Size = 12;
            style5.Font.Name = "Arial";
            #endregion
            bool IncludeHaveDetail = false;
            List<DictionaryVmodel> listMonthReportDetail = null;
            if (rpt != null)
            {
                listMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "", OrderStr, IncludeHaveDetail);
            }

            int rowStart = 5;  //开始行
            int colStart = 2; // 开始列

            //系统指标类，需要读取XML
            List<C_Target> _targetList = StaticResource.Instance.TargetList[sytemID].ToList();

            for (int i = 0; i < listMonthReportDetail.Count; i++)
            {
                #region 生成Excel中的页签
                if (listMonthReportDetail[i].IsBlendTarget)
                {

                    worksheets[0].Name = listMonthReportDetail[i].Name;
                    worksheets[0].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                    worksheets[0].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[0].Cells[1, 2].SetStyle(style1);
                    var thisdv = ((List<DictionaryVmodel>)listMonthReportDetail[i].ObjValue);
                    C_Target _target = _targetList.Where(p => p.TargetName == thisdv[0].Name).ToList()[0];
                    worksheets[0].Cells[1, 21].PutValue("单位：" + _target.Unit);
                    worksheets[0].Cells[1, 21].SetStyle(style5);
                    worksheets[0].Replace("$targetName1", thisdv[0].Name);
                    worksheets[0].Replace("$targetName2", thisdv[1].Name);
                }
                else
                {
                    C_Target _target = _targetList.Where(p => p.TargetName == listMonthReportDetail[i].Name).ToList()[0];

                    if (worksheets[1].Name == "单指标")
                    {
                        worksheets[1].Name = listMonthReportDetail[i].Name;
                        worksheets[1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                        worksheets[1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                        worksheets[1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                        worksheets[1].Cells[1, 12].SetStyle(style5);
                        worksheets[1].Cells[1, 2].SetStyle(style1);
                    }
                    else
                    {
                        worksheets.AddCopy(1);
                        worksheets[worksheets.Count - 1].Name = listMonthReportDetail[i].Name;
                        worksheets[worksheets.Count - 1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                        worksheets[worksheets.Count - 1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                        worksheets[worksheets.Count - 1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                        worksheets[worksheets.Count - 1].Cells[1, 12].SetStyle(style5);
                        worksheets[worksheets.Count - 1].Cells[1, 2].SetStyle(style1);
                    }
                }
                #endregion
            }

            bool IsDifferenceException = false; //商管的差额特殊处理

            int DataDisplayMode = 0; //针对旅业下载客流量保留2位小数

            for (int sheetIndex = 0; sheetIndex < listMonthReportDetail.Count; sheetIndex++)
            {
                Worksheet worksheet = worksheets[listMonthReportDetail[sheetIndex].Name];
                string _targetName = string.Empty;

                if (listMonthReportDetail[sheetIndex].IsBlendTarget)
                {
                    #region 多指标
                    var thisdv = (List<DictionaryVmodel>)listMonthReportDetail[sheetIndex].ObjValue;
                    _targetName = thisdv[0].Name;// listMonthReportDetail[sheetIndex].Name;

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
                    rowStart = 5;
                    StyleFlag flag = new StyleFlag();
                    flag.All = true;
                    List<DictionaryVmodel> listCompanyProperty = (List<DictionaryVmodel>)thisdv[0].ObjValue;
                    List<DictionaryVmodel> listCompanyProperty2 = (List<DictionaryVmodel>)thisdv[1].ObjValue;

                    for (int j = 0; j < listCompanyProperty.Count; j++)
                    {
                        if (listCompanyProperty[j].Name == "SummaryData")
                        {
                            List<B_MonthlyReportDetail> ListItem = ((List<B_MonthlyReportDetail>)listCompanyProperty[j].ObjValue);
                            List<B_MonthlyReportDetail> ListItem2 = ((List<B_MonthlyReportDetail>)listCompanyProperty2[j].ObjValue);
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);
                                #endregion

                                #region 为单元格赋值
                                worksheet.Cells[rowStart, colStart - 1].PutValue("合计");
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem2[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmountByYear == 0 ? "--" : ListItem[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem2[k].NPlanAmmountByYear == 0 ? "--" : ListItem2[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem2[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem2[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(ListItem2[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(ListItem2[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(ListItem2[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(ListItem2[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                        }
                        else if (listCompanyProperty[j].Name == "HaveDetail")
                        {
                            List<MonthlyReportDetail> listMRDVM = (List<MonthlyReportDetail>)listCompanyProperty[j].ObjValue;
                            List<MonthlyReportDetail> listMRDVM2 = (List<MonthlyReportDetail>)listCompanyProperty2[j].ObjValue;
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style2);
                                #endregion

                                #region 为单元格赋值
                                var listMRDVMOther = listMRDVM2.Where(m => m.CompanyID == listMRDVM[k].CompanyID).FirstOrDefault();

                                worksheet.Cells[rowStart, colStart - 1].PutValue((k + 1).ToString());
                                worksheet.Cells[rowStart, colStart].PutValue(listMRDVM[k].CompanyName.ToString());
                                worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVMOther.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVMOther.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVMOther.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(listMRDVMOther.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(listMRDVM[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(listMRDVMOther.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(listMRDVMOther.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(listMRDVMOther.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(listMRDVM[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(listMRDVMOther.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                        }
                        else
                        {
                            List<DictionaryVmodel> ListItem = ((List<DictionaryVmodel>)listCompanyProperty[j].ObjValue);
                            List<DictionaryVmodel> ListItem2 = ((List<DictionaryVmodel>)listCompanyProperty2[j].ObjValue);
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);

                                #endregion

                                #region 为单元格赋值
                                var listItemOther = ListItem2.Where(m => m.Name == ListItem[z].Name).FirstOrDefault();
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(listItemOther.BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[z].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(listItemOther.BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(listItemOther.BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[z].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(listItemOther.BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[z].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(listItemOther.BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[z].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(listItemOther.BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(listItemOther.BMonthReportDetail.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(listItemOther.BMonthReportDetail.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");


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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);

                                #endregion

                                rowStart = rowStart + 1;
                                int tempRowStart = rowStart;
                                if (ListItem[z].ObjValue == null)
                                { continue; }
                                List<MonthlyReportDetail> listMRDVM = (List<MonthlyReportDetail>)ListItem[z].ObjValue;
                                List<MonthlyReportDetail> listMRDVM2 = (List<MonthlyReportDetail>)ListItem2[z].ObjValue;
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
                                    worksheet.Cells[rowStart, colStart + 11].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 16].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 17].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 18].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 19].SetStyle(style2);
                                    #endregion

                                    #region 为单元格赋值
                                    var listMRDVMOther = listMRDVM2.Where(m => m.CompanyID == listMRDVM[k].CompanyID).FirstOrDefault();
                                    worksheet.Cells[rowStart, tmpcolStart].PutValue((k + 1).ToString());
                                    worksheet.Cells[rowStart, tmpcolStart + 1].PutValue(listMRDVM[k].CompanyName.ToString());
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVMOther.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVMOther.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVMOther.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(listMRDVMOther.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue(listMRDVM[k].NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 11].PutValue(listMRDVMOther.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 12].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 13].PutValue(listMRDVMOther.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 14].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 15].PutValue(listMRDVMOther.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 16].PutValue(listMRDVM[k].NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 17].PutValue(listMRDVMOther.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                    worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                        int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 18, ImageFilePath + "\\image" + listMRDVM[k].Counter + ".png");
                                        Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                                        picture.Left = 60;
                                        picture.Top = 10;
                                    }
                                    if (listMRDVM2[k].Counter > 0)
                                    {
                                        int pictureIndex2 = worksheet.Pictures.Add(rowStart, colStart + 19, ImageFilePath + "\\image" + listMRDVM2[k].Counter + ".png");
                                        Aspose.Cells.Drawing.Picture picture2 = worksheet.Pictures[pictureIndex2];
                                        picture2.Left = 60;
                                        picture2.Top = 10;

                                    }
                                    style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style2.Number = 3;
                                    else
                                        style2.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);

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
                    #endregion
                }
                else
                {
                    #region 单指标

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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NAccumulativeActualAmmount);
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
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
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
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[z].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[z].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[z].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[z].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeActualAmmount);
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
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
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
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM[k].NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM[k].NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
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
                                    worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
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
                    #endregion
                }
            }
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

        }


        /// <summary>
        /// 下载Excel完成情况明细--经营系统(参数默认当月)
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public MemoryStream DownExcelMonthReportDetailMonthly_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

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

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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
                listMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "", OrderStrTwo, IncludeHaveDetail);
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
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

        }

        /// <summary>
        /// 混合指标--下载Excel完成情况明细--经营系统(参数默认当月)
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public MemoryStream DownExcelMonthReportDetailBlendMonthly_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            if (rpt._System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
            {
                string strXml = rpt.GetComplateMonthReportDetailHtmlTemplate(rpt._System.Configuration);
                string[] strXmls = strXml.Split(',');
                if (strXmls.Length > 1)
                {
                    templeteName = !string.IsNullOrEmpty(strXmls[3]) ? strXmls[3] : "完成情况明细模板-混合指标V1.xlsx";
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style5 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
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

            #region style5样式
            style5.Font.Size = 12;
            style5.Font.Name = "Arial";
            #endregion

            bool IncludeHaveDetail = false;
            List<DictionaryVmodel> listMonthReportDetail = null;
            if (rpt != null)
            {
                listMonthReportDetail = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt, "", OrderStrTwo, IncludeHaveDetail);
            }

            int rowStart = 5;  //开始行
            int colStart = 2; // 开始列

            //系统指标类，需要读取XML
            List<C_Target> _targetList = StaticResource.Instance.TargetList[sytemID].ToList();
            
            for (int i = 0; i < listMonthReportDetail.Count; i++)
            {
                #region 生成Excel中的页签
                if (listMonthReportDetail[i].IsBlendTarget)
                {

                    worksheets[0].Name = listMonthReportDetail[i].Name;
                    worksheets[0].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                    worksheets[0].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[0].Cells[1, 2].SetStyle(style1);
                    var thisdv = ((List<DictionaryVmodel>)listMonthReportDetail[i].ObjValue);
                    C_Target _target = _targetList.Where(p => p.TargetName == thisdv[0].Name).ToList()[0];
                    worksheets[0].Cells[1, 21].PutValue("单位：" + _target.Unit);
                    worksheets[0].Cells[1, 21].SetStyle(style5);
                    worksheets[0].Replace("$targetName1", thisdv[0].Name);
                    worksheets[0].Replace("$targetName2", thisdv[1].Name);
                }
                else
                {
                    C_Target _target = _targetList.Where(p => p.TargetName == listMonthReportDetail[i].Name).ToList()[0];

                    if (worksheets[1].Name == "单指标")
                    {
                        worksheets[1].Name = listMonthReportDetail[i].Name;
                        worksheets[1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                        worksheets[1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                        worksheets[1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                        worksheets[1].Cells[1, 12].SetStyle(style5);
                        worksheets[1].Cells[1, 2].SetStyle(style1);
                    }
                    else
                    {
                        worksheets.AddCopy(1);
                        worksheets[worksheets.Count - 1].Name = listMonthReportDetail[i].Name;
                        worksheets[worksheets.Count - 1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                        worksheets[worksheets.Count - 1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                        worksheets[worksheets.Count - 1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                        worksheets[worksheets.Count - 1].Cells[1, 12].SetStyle(style5);
                        worksheets[worksheets.Count - 1].Cells[1, 2].SetStyle(style1);
                    }
                }
                #endregion
            }

            bool IsDifferenceException = false; //商管的差额特殊处理

            int DataDisplayMode = 0; //针对旅业下载客流量保留2位小数

            for (int sheetIndex = 0; sheetIndex < listMonthReportDetail.Count; sheetIndex++)
            {
                Worksheet worksheet = worksheets[listMonthReportDetail[sheetIndex].Name];
                string _targetName = string.Empty;

                if (listMonthReportDetail[sheetIndex].IsBlendTarget)
                {
                    #region 多指标
                    var thisdv = (List<DictionaryVmodel>)listMonthReportDetail[sheetIndex].ObjValue;
                    _targetName = thisdv[0].Name;// listMonthReportDetail[sheetIndex].Name;

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
                    rowStart = 5;
                    StyleFlag flag = new StyleFlag();
                    flag.All = true;
                    List<DictionaryVmodel> listCompanyProperty = (List<DictionaryVmodel>)thisdv[0].ObjValue;
                    List<DictionaryVmodel> listCompanyProperty2 = (List<DictionaryVmodel>)thisdv[1].ObjValue;

                    for (int j = 0; j < listCompanyProperty.Count; j++)
                    {
                        if (listCompanyProperty[j].Name == "SummaryData")
                        {
                            List<B_MonthlyReportDetail> ListItem = ((List<B_MonthlyReportDetail>)listCompanyProperty[j].ObjValue);
                            List<B_MonthlyReportDetail> ListItem2 = ((List<B_MonthlyReportDetail>)listCompanyProperty2[j].ObjValue);
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);
                                #endregion

                                #region 为单元格赋值
                                worksheet.Cells[rowStart, colStart - 1].PutValue("合计");
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem2[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmountByYear == 0 ? "--" : ListItem[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem2[k].NPlanAmmountByYear == 0 ? "--" : ListItem2[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem2[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem2[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(ListItem2[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(ListItem2[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(ListItem2[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(ListItem2[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                        }
                        else if (listCompanyProperty[j].Name == "HaveDetail")
                        {
                            List<MonthlyReportDetail> listMRDVM = (List<MonthlyReportDetail>)listCompanyProperty[j].ObjValue;
                            List<MonthlyReportDetail> listMRDVM2 = (List<MonthlyReportDetail>)listCompanyProperty2[j].ObjValue;
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style2);
                                #endregion

                                #region 为单元格赋值
                                worksheet.Cells[rowStart, colStart - 1].PutValue((k + 1).ToString());
                                worksheet.Cells[rowStart, colStart].PutValue(listMRDVM[k].CompanyName.ToString());
                                worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM2[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM2[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM2[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(listMRDVM2[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(listMRDVM[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(listMRDVM2[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(listMRDVM2[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(listMRDVM2[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(listMRDVM[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(listMRDVM2[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                        }
                        else
                        {
                            List<DictionaryVmodel> ListItem = ((List<DictionaryVmodel>)listCompanyProperty[j].ObjValue);
                            List<DictionaryVmodel> ListItem2 = ((List<DictionaryVmodel>)listCompanyProperty2[j].ObjValue);
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);

                                #endregion

                                #region 为单元格赋值
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem2[z].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[z].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem2[z].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem2[z].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[z].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem2[z].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[z].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(ListItem2[z].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[z].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(ListItem2[z].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(ListItem2[z].BMonthReportDetail.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(ListItem2[z].BMonthReportDetail.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");


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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);

                                #endregion

                                rowStart = rowStart + 1;
                                int tempRowStart = rowStart;
                                if (ListItem[z].ObjValue == null)
                                { continue; }
                                List<MonthlyReportDetail> listMRDVM = (List<MonthlyReportDetail>)ListItem[z].ObjValue;
                                List<MonthlyReportDetail> listMRDVM2 = (List<MonthlyReportDetail>)ListItem2[z].ObjValue;
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
                                    worksheet.Cells[rowStart, colStart + 11].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 16].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 17].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 18].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 19].SetStyle(style2);
                                    #endregion

                                    #region 为单元格赋值
                                    worksheet.Cells[rowStart, tmpcolStart].PutValue((k + 1).ToString());
                                    worksheet.Cells[rowStart, tmpcolStart + 1].PutValue(listMRDVM[k].CompanyName.ToString());
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM2[k].NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM2[k].NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM2[k].NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(listMRDVM2[k].NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue(listMRDVM[k].NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 11].PutValue(listMRDVM2[k].NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 12].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 13].PutValue(listMRDVM2[k].NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 14].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 15].PutValue(listMRDVM2[k].NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 16].PutValue(listMRDVM[k].NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 17].PutValue(listMRDVM2[k].NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                    worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                        int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 18, ImageFilePath + "\\image" + listMRDVM[k].Counter + ".png");
                                        Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                                        picture.Left = 60;
                                        picture.Top = 10;
                                    }
                                    if (listMRDVM2[k].Counter > 0)
                                    {
                                        int pictureIndex2 = worksheet.Pictures.Add(rowStart, colStart + 19, ImageFilePath + "\\image" + listMRDVM2[k].Counter + ".png");
                                        Aspose.Cells.Drawing.Picture picture2 = worksheet.Pictures[pictureIndex2];
                                        picture2.Left = 60;
                                        picture2.Top = 10;
                                    }
                                    style2 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style2.Number = 3;
                                    else
                                        style2.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 3].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);

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
                    #endregion
                }
                else
                {
                    #region 单指标

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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NAccumulativeActualAmmount);
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
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
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
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[z].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[z].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[z].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[z].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[z].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[z].BMonthReportDetail.NAccumulativeActualAmmount);
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
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
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
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(listMRDVM[k].NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listMRDVM[k].NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(listMRDVM[k].NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listMRDVM[k].NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(listMRDVM[k].NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listMRDVM[k].NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(listMRDVM[k].NAccumulativeActualAmmount);
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
                                    worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
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
                    #endregion
                }
            }

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

                
            }
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

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
        public MemoryStream DownExcelMonthReportDetail_XM(ReportInstance rpt, string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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


            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

        }

        /// <summary>
        /// 下载Excel单指标-经营报告明细--经营系统
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public MemoryStream DownExcelManageReportTargetDetail_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            if (rpt._System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
            {
                string strXml = rpt.GetComplateMonthReportDetailHtmlTemplate(rpt._System.Configuration);
                string[] strXmls = strXml.Split(',');
                if (strXmls.Length > 1)
                {
                    templeteName = !string.IsNullOrEmpty(strXmls[3]) ? strXmls[3] : "完成情况明细模板-单指标V1.xlsx";
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style5 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
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

            #region style5样式
            style5.Font.Size = 12;
            style5.Font.Name = "Arial";
            #endregion
            bool IncludeHaveDetail = false;
            List<DictionaryVmodel> listMonthReportDetail = null;
            if (rpt != null)
            {
                listMonthReportDetail = ReportInstanceManageDetailEngine.ReportInstanceManageDetailService.GetManageDetailRptDataSource(rpt, "", OrderStr, IncludeHaveDetail);
            }

            int rowStart = 4;  //开始行
            int colStart = 2; // 开始列

            //系统指标类，需要读取XML
            List<C_Target> _targetList = StaticResource.Instance.TargetList[sytemID].ToList();
            for (int i = 0; i < listMonthReportDetail.Count; i++)
            {
                #region 生成Excel中的页签
                C_Target _target = _targetList.Where(p => p.TargetName == listMonthReportDetail[i].Name).ToList()[0];
                if (worksheets[0].Name == "单指标")
                {
                    worksheets[0].Name = listMonthReportDetail[i].Name;
                    worksheets[0].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                    worksheets[0].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[0].Cells[1, 12].PutValue("单位：" + _target.Unit);
                    worksheets[0].Cells[1, 12].SetStyle(style5);
                    worksheets[0].Cells[1, 2].SetStyle(style1);
                }
                else
                {
                    worksheets.AddCopy(0);
                    worksheets[worksheets.Count - 1].Name = listMonthReportDetail[i].Name;
                    worksheets[worksheets.Count - 1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                    worksheets[worksheets.Count - 1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[worksheets.Count - 1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                    worksheets[worksheets.Count - 1].Cells[1, 12].SetStyle(style5);
                    worksheets[worksheets.Count - 1].Cells[1, 2].SetStyle(style1);
                }
                #endregion
            }

            bool IsDifferenceException = false; //商管的差额特殊处理

            int DataDisplayMode = 0; //针对旅业下载客流量保留2位小数

            //创建指标Sheet,
            for (int sheetIndex = 0; sheetIndex < listMonthReportDetail.Count; sheetIndex++)
            {
                Worksheet worksheet = worksheets[listMonthReportDetail[sheetIndex].Name];
                string _targetName = string.Empty;
                #region 单指标
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
                            worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                            worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[k].NPlanAmmountByYear == 0 ? "--" : ListItem[k].NDisplayRateByYear);
                            worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmount);
                            worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[k].NActualAmmount);
                            worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NDisplayRate);
                            worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[k].NAccumulativePlanAmmount);
                            worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NAccumulativeActualAmmount);
                            worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[k].NAccumulativeDisplayRate);
                            worksheet.Cells[rowStart, colStart + 10].PutValue("");

                            #endregion

                            #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                            style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                            if (DataDisplayMode == 0)
                                style3.Number = 3;
                            else
                                style3.Number = 4;

                            worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                            #endregion
                            rowStart = rowStart + 1;
                        }
                    }
                    else if (listCompanyProperty[j].Mark == "CompanyProperty")
                    {
                        List<DictionaryVmodel> ListItem = ((List<DictionaryVmodel>)listCompanyProperty[j].ObjValue);
                        if (ListItem.FirstOrDefault().Mark == "Area")
                        {
                            for (int i = 0; i < ListItem.Count; i++)
                            {
                                Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);
                                range.Merge();
                                range.ApplyStyle(style3, flag);
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
                                worksheet.Cells[rowStart, colStart - 1].PutValue(ListItem[i].Name);
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[i].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[i].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[i].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[i].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                #endregion

                                #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                if (DataDisplayMode == 0)
                                    style3.Number = 3;
                                else
                                    style3.Number = 4;

                                worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                #endregion

                                rowStart = rowStart + 1;

                                if (ListItem[i].ObjValue == null)
                                { continue; }

                                List<DictionaryVmodel> ListItemLastArea = ((List<DictionaryVmodel>)ListItem[i].ObjValue);

                                for (int k = 0; k < ListItemLastArea.Count; k++)
                                {
                                    Range itemRange = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                    itemRange.Merge();
                                    itemRange.ApplyStyle(style3, flag);

                                    #region 设置样式
                                    worksheet.Cells[rowStart, colStart - 1].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart].SetStyle(style3);
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
                                    worksheet.Cells[rowStart, colStart - 1].PutValue(k + 1);
                                    worksheet.Cells[rowStart, colStart].PutValue(ListItemLastArea[k].Name);
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(ListItemLastArea[k].BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(ListItemLastArea[k].BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(ListItemLastArea[k].BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(ListItemLastArea[k].BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(ListItemLastArea[k].BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                    #endregion

                                    #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                    style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style3.Number = 3;
                                    else
                                        style3.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                    #endregion

                                    rowStart = rowStart + 1;

                                    if (ListItemLastArea[k].ObjValue == null)
                                    { continue; }

                                    List<MonthlyReportDetail> listCompany = (List<MonthlyReportDetail>)ListItemLastArea[k].ObjValue;

                                    for (int l = 0; l < listCompany.Count; l++)
                                    {
                                        Range rangeCompany = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                        rangeCompany.Merge();
                                        rangeCompany.ApplyStyle(style2, flag);
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
                                        worksheet.Cells[rowStart, colStart - 1].PutValue((l + 1).ToString());
                                        worksheet.Cells[rowStart, colStart].PutValue(listCompany[l].CompanyName.ToString());
                                        worksheet.Cells[rowStart, colStart + 2].PutValue(listCompany[l].NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 3].PutValue(listCompany[l].NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 4].PutValue(listCompany[l].NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 5].PutValue(listCompany[l].NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 6].PutValue(listCompany[l].NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 7].PutValue(listCompany[l].NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 8].PutValue(listCompany[l].NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 9].PutValue(listCompany[l].NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                        #endregion
                                        #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                        if (listCompany[l].Counter > 0)
                                        {
                                            int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 10, ImageFilePath + "\\image" + listCompany[l].Counter + ".png");
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
                                        worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                        #endregion
                                        rowStart = rowStart + 1;
                                    }
                                }
                            }
                        }
                        else if (ListItem.FirstOrDefault().Mark == "LastArea")
                        {
                            for (int i = 0; i < ListItem.Count; i++)
                            {
                                Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);
                                range.Merge();
                                range.ApplyStyle(style3, flag);
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
                                worksheet.Cells[rowStart, colStart - 1].PutValue(ListItem[i].Name);
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[i].BMonthReportDetail.NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[i].BMonthReportDetail.NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[i].BMonthReportDetail.NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[i].BMonthReportDetail.NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                #endregion

                                #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                if (DataDisplayMode == 0)
                                    style3.Number = 3;
                                else
                                    style3.Number = 4;

                                worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                #endregion

                                rowStart = rowStart + 1;

                                if (ListItem[i].ObjValue == null)
                                { continue; }

                                List<MonthlyReportDetail> listCompany = (List<MonthlyReportDetail>)ListItem[i].ObjValue;

                                for (int l = 0; l < listCompany.Count; l++)
                                {
                                    Range rangeCompany = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                    rangeCompany.Merge();
                                    rangeCompany.ApplyStyle(style2, flag);

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
                                    worksheet.Cells[rowStart, colStart - 1].PutValue((l + 1).ToString());
                                    worksheet.Cells[rowStart, colStart].PutValue(listCompany[l].CompanyName.ToString());
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(listCompany[l].NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listCompany[l].NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(listCompany[l].NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listCompany[l].NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(listCompany[l].NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listCompany[l].NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(listCompany[l].NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(listCompany[l].NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                    #endregion

                                    #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                    if (listCompany[l].Counter > 0)
                                    {
                                        int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 10, ImageFilePath + "\\image" + listCompany[l].Counter + ".png");
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
                                    worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                    #endregion
                                    rowStart = rowStart + 1;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;
        }

        /// <summary>
        /// 下载Excel混合指标-经营报告明细--经营系统
        /// </summary>
        /// <param name="templetePath">模板路径</param>
        /// <param name="templeteName">模板名称</param>
        /// <param name="fileName">下载文件名称</param>
        /// <param name="sytemID">系统ID</param>
        /// <param name="Year">年度</param>
        /// <param name="Month">月份</param>
        /// <param name="IsLatestVersion">是否包含审批中</param>
        public MemoryStream DownExcelManageReportBlendTargetDetail_JY(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            if (rpt._System.Configuration.Elements("ComplateTargetDetail").Elements("TableTemplate").ToList().Count > 0)
            {
                string strXml = rpt.GetComplateMonthReportDetailHtmlTemplate(rpt._System.Configuration);
                string[] strXmls = strXml.Split(',');
                if (strXmls.Length > 1)
                {
                    templeteName = !string.IsNullOrEmpty(strXmls[3]) ? strXmls[3] : "完成情况明细模板-混合指标V1.xlsx";
                }
            }

            string path = System.IO.Path.Combine(templetePath, templeteName);//合并路径

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            Aspose.Cells.Style style1 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style2 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Style style3 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style4 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
            Aspose.Cells.Style style5 = designer.Workbook.Styles[designer.Workbook.Styles.Add()];
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

            #region style5样式
            style5.Font.Size = 12;
            style5.Font.Name = "Arial";
            #endregion
            bool IncludeHaveDetail = false;
            List<DictionaryVmodel> listMonthReportDetail = null;
            if (rpt != null)
            {
                listMonthReportDetail = ReportInstanceManageDetailEngine.ReportInstanceManageDetailService.GetManageDetailRptDataSource(rpt, "", OrderStr, IncludeHaveDetail);
            }

            int rowStart = 5;  //开始行
            int colStart = 2; // 开始列

            //系统指标类，需要读取XML
            List<C_Target> _targetList = StaticResource.Instance.TargetList[sytemID].ToList();


            for (int i = 0; i < listMonthReportDetail.Count; i++)
            {
                #region 生成Excel中的页签
                if (listMonthReportDetail[i].IsBlendTarget)
                {

                    worksheets[0].Name = listMonthReportDetail[i].Name;
                    worksheets[0].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                    worksheets[0].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                    worksheets[0].Cells[1, 2].SetStyle(style1);
                    var thisdv = ((List<DictionaryVmodel>)listMonthReportDetail[i].ObjValue);
                    C_Target _target = _targetList.Where(p => p.TargetName == thisdv[0].Name).ToList()[0];
                    worksheets[0].Cells[1, 21].PutValue("单位：" + _target.Unit);
                    worksheets[0].Cells[1, 21].SetStyle(style5);
                    worksheets[0].Replace("$targetName1", thisdv[0].Name);
                    worksheets[0].Replace("$targetName2", thisdv[1].Name);
                }
                else
                {
                    C_Target _target = _targetList.Where(p => p.TargetName == listMonthReportDetail[i].Name).ToList()[0];

                    if (worksheets[1].Name == "单指标")
                    {
                        worksheets[1].Name = listMonthReportDetail[i].Name;
                        worksheets[1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                        worksheets[1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                        worksheets[1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                        worksheets[1].Cells[1, 12].SetStyle(style5);
                        worksheets[1].Cells[1, 2].SetStyle(style1);
                    }
                    else
                    {
                        worksheets.AddCopy(1);
                        worksheets[worksheets.Count - 1].Name = listMonthReportDetail[i].Name;
                        worksheets[worksheets.Count - 1].Cells[0, 1].PutValue(rpt._System.SystemName + listMonthReportDetail[i].Name);
                        worksheets[worksheets.Count - 1].Cells[1, 2].PutValue(FinYear.ToString() + "年" + FinMonth + "月");
                        worksheets[worksheets.Count - 1].Cells[1, 12].PutValue("单位：" + _target.Unit);
                        worksheets[worksheets.Count - 1].Cells[1, 12].SetStyle(style5);
                        worksheets[worksheets.Count - 1].Cells[1, 2].SetStyle(style1);
                    }
                }
                #endregion
            }

            bool IsDifferenceException = false; //商管的差额特殊处理

            int DataDisplayMode = 0; //针对旅业下载客流量保留2位小数

            //创建指标Sheet,
            for (int sheetIndex = 0; sheetIndex < listMonthReportDetail.Count; sheetIndex++)
            {
                Worksheet worksheet = worksheets[listMonthReportDetail[sheetIndex].Name];
                string _targetName = string.Empty;

                if (listMonthReportDetail[sheetIndex].IsBlendTarget)
                {
                    #region 多指标
                    var thisdv = (List<DictionaryVmodel>)listMonthReportDetail[sheetIndex].ObjValue;
                    _targetName = thisdv[0].Name;// listMonthReportDetail[sheetIndex].Name;

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
                    rowStart = 5;
                    StyleFlag flag = new StyleFlag();
                    flag.All = true;
                    List<DictionaryVmodel> listCompanyProperty = (List<DictionaryVmodel>)thisdv[0].ObjValue;
                    List<DictionaryVmodel> listCompanyProperty2 = (List<DictionaryVmodel>)thisdv[1].ObjValue;

                    for (int j = 0; j < listCompanyProperty.Count; j++)
                    {
                        if (listCompanyProperty[j].Name == "SummaryData")
                        {
                            List<B_MonthlyReportDetail> ListItem = ((List<B_MonthlyReportDetail>)listCompanyProperty[j].ObjValue);
                            List<B_MonthlyReportDetail> ListItem2 = ((List<B_MonthlyReportDetail>)listCompanyProperty2[j].ObjValue);
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
                                worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);
                                #endregion

                                #region 为单元格赋值
                                worksheet.Cells[rowStart, colStart - 1].PutValue("合计");
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem2[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmountByYear == 0 ? "--" : ListItem[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem2[k].NPlanAmmountByYear == 0 ? "--" : ListItem2[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem2[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem2[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 11].PutValue(ListItem2[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 13].PutValue(ListItem2[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 15].PutValue(ListItem2[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 17].PutValue(ListItem2[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                worksheet.Cells[rowStart, colStart + 19].PutValue("");

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
                                worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                        }
                        else if (listCompanyProperty[j].Mark == "CompanyProperty")
                        {
                            List<DictionaryVmodel> ListItem = ((List<DictionaryVmodel>)listCompanyProperty[j].ObjValue);
                            List<DictionaryVmodel> ListItem2 = ((List<DictionaryVmodel>)listCompanyProperty2[j].ObjValue);
                            if (ListItem.Count > 0 && ListItem.FirstOrDefault().Mark == "Area")
                            {
                                for (int i = 0; i < ListItem.Count; i++)
                                {
                                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);

                                    range.Merge();
                                    range.ApplyStyle(style3, flag);
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
                                    worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);
                                    #endregion

                                    #region 为单元格赋值
                                    var listItemOther = ListItem2.Where(m => m.Name == ListItem[i].Name).FirstOrDefault();

                                    worksheet.Cells[rowStart, colStart - 1].PutValue(ListItem[i].Name);
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listItemOther.BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[i].BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listItemOther.BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listItemOther.BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[i].BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(listItemOther.BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[i].BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 11].PutValue(listItemOther.BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[i].BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 13].PutValue(listItemOther.BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 15].PutValue(listItemOther.BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 17].PutValue(listItemOther.BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                    worksheet.Cells[rowStart, colStart + 19].PutValue("");
                                    #endregion

                                    #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                    style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style3.Number = 3;
                                    else
                                        style3.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);

                                    #endregion

                                    rowStart = rowStart + 1;

                                    if (ListItem[i].ObjValue == null)
                                    { continue; }

                                    List<DictionaryVmodel> ListItemLastArea = ((List<DictionaryVmodel>)ListItem[i].ObjValue);
                                    List<DictionaryVmodel> ListItem2LastArea = ((List<DictionaryVmodel>)listItemOther.ObjValue);

                                    for (int k = 0; k < ListItemLastArea.Count; k++)
                                    {
                                        Range itemRange = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                        itemRange.Merge();
                                        itemRange.ApplyStyle(style3, flag);

                                        #region 设置样式
                                        worksheet.Cells[rowStart, colStart - 1].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 10].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);
                                        #endregion

                                        #region 为单元格赋值
                                        var ListItemLastAreaOther = ListItem2LastArea.Where(m => m.Name == ListItemLastArea[k].Name).FirstOrDefault();

                                        worksheet.Cells[rowStart, colStart - 1].PutValue(k + 1);
                                        worksheet.Cells[rowStart, colStart].PutValue(ListItemLastArea[k].Name);
                                        worksheet.Cells[rowStart, colStart + 2].PutValue(ListItemLastArea[k].BMonthReportDetail.NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 3].PutValue(ListItemLastAreaOther.BMonthReportDetail.NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 4].PutValue(ListItemLastArea[k].BMonthReportDetail.NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 5].PutValue(ListItemLastAreaOther.BMonthReportDetail.NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 6].PutValue(ListItemLastArea[k].BMonthReportDetail.NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 7].PutValue(ListItemLastAreaOther.BMonthReportDetail.NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 8].PutValue(ListItemLastArea[k].BMonthReportDetail.NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 9].PutValue(ListItemLastAreaOther.BMonthReportDetail.NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 10].PutValue(ListItemLastArea[k].BMonthReportDetail.NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 11].PutValue(ListItemLastAreaOther.BMonthReportDetail.NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 12].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 13].PutValue(ListItemLastAreaOther.BMonthReportDetail.NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 14].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 15].PutValue(ListItemLastAreaOther.BMonthReportDetail.NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 16].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 17].PutValue(ListItemLastAreaOther.BMonthReportDetail.NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                        worksheet.Cells[rowStart, colStart + 19].PutValue("");
                                        #endregion

                                        #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                        style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                        if (DataDisplayMode == 0)
                                            style3.Number = 3;
                                        else
                                            style3.Number = 4;

                                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);

                                        #endregion

                                        rowStart = rowStart + 1;

                                        if (ListItemLastArea[k].ObjValue == null)
                                        { continue; }

                                        List<MonthlyReportDetail> listCompany = (List<MonthlyReportDetail>)ListItemLastArea[k].ObjValue;
                                        List<MonthlyReportDetail> listCompany2 = (List<MonthlyReportDetail>)ListItemLastAreaOther.ObjValue;

                                        for (int l = 0; l < listCompany.Count; l++)
                                        {
                                            Range rangeCompany = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                            rangeCompany.Merge();
                                            rangeCompany.ApplyStyle(style2, flag);
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
                                            worksheet.Cells[rowStart, colStart + 11].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 16].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 17].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 18].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 19].SetStyle(style2);
                                            #endregion
                                            #region 为单元格赋值
                                            var listCompany2Item = listCompany2.Where(m => m.CompanyID == listCompany[l].CompanyID).FirstOrDefault();
                                            worksheet.Cells[rowStart, colStart - 1].PutValue((l + 1).ToString());
                                            worksheet.Cells[rowStart, colStart].PutValue(listCompany[l].CompanyName.ToString());
                                            worksheet.Cells[rowStart, colStart + 2].PutValue(listCompany[l].NPlanAmmountByYear);
                                            worksheet.Cells[rowStart, colStart + 3].PutValue(listCompany2Item.NPlanAmmountByYear);
                                            worksheet.Cells[rowStart, colStart + 4].PutValue(listCompany[l].NDisplayRateByYear);
                                            worksheet.Cells[rowStart, colStart + 5].PutValue(listCompany2Item.NDisplayRateByYear);
                                            worksheet.Cells[rowStart, colStart + 6].PutValue(listCompany[l].NPlanAmmount);
                                            worksheet.Cells[rowStart, colStart + 7].PutValue(listCompany2Item.NPlanAmmount);
                                            worksheet.Cells[rowStart, colStart + 8].PutValue(listCompany[l].NActualAmmount);
                                            worksheet.Cells[rowStart, colStart + 9].PutValue(listCompany2Item.NActualAmmount);
                                            worksheet.Cells[rowStart, colStart + 10].PutValue(listCompany[l].NDisplayRate);
                                            worksheet.Cells[rowStart, colStart + 11].PutValue(listCompany2Item.NDisplayRate);
                                            worksheet.Cells[rowStart, colStart + 12].PutValue(listCompany[l].NAccumulativePlanAmmount);
                                            worksheet.Cells[rowStart, colStart + 13].PutValue(listCompany2Item.NAccumulativePlanAmmount);
                                            worksheet.Cells[rowStart, colStart + 14].PutValue(listCompany[l].NAccumulativeActualAmmount);
                                            worksheet.Cells[rowStart, colStart + 15].PutValue(listCompany2Item.NAccumulativeActualAmmount);
                                            worksheet.Cells[rowStart, colStart + 16].PutValue(listCompany[l].NAccumulativeDisplayRate);
                                            worksheet.Cells[rowStart, colStart + 17].PutValue(listCompany2Item.NAccumulativeDisplayRate);
                                            worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                            worksheet.Cells[rowStart, colStart + 19].PutValue("");
                                            #endregion
                                            #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                            if (listCompany[l].Counter > 0)
                                            {
                                                int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 18, ImageFilePath + "\\image" + listCompany[l].Counter + ".png");
                                                Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                                                picture.Left = 60;
                                                picture.Top = 10;

                                            }
                                            if (listCompany2Item.Counter > 0)
                                            {
                                                int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 19, ImageFilePath + "\\image" + listCompany2Item.Counter + ".png");
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
                                            worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                            #endregion
                                            rowStart = rowStart + 1;
                                        }
                                    }
                                }
                            }
                            else if (ListItem.Count > 0 && ListItem.FirstOrDefault().Mark == "LastArea")
                            {
                                for (int i = 0; i < ListItem.Count; i++)
                                {
                                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);
                                    range.Merge();
                                    range.ApplyStyle(style3, flag);
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
                                    worksheet.Cells[rowStart, colStart + 11].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 16].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 17].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 18].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 19].SetStyle(style3);
                                    #endregion

                                    #region 为单元格赋值
                                    var listItemOther = ListItem2.Where(m => m.Name == ListItem[i].Name).FirstOrDefault();

                                    worksheet.Cells[rowStart, colStart - 1].PutValue(ListItem[i].Name);
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(listItemOther.BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[i].BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(listItemOther.BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(listItemOther.BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[i].BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(listItemOther.BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue(ListItem[i].BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 11].PutValue(listItemOther.BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 12].PutValue(ListItem[i].BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 13].PutValue(listItemOther.BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 14].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 15].PutValue(listItemOther.BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 16].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 17].PutValue(listItemOther.BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                    worksheet.Cells[rowStart, colStart + 19].PutValue("");
                                    #endregion

                                    #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                    style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style3.Number = 3;
                                    else
                                        style3.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 3].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 6].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 9].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 12].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 13].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 14].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 15].SetStyle(style3);

                                    #endregion

                                    rowStart = rowStart + 1;

                                    if (ListItem[i].ObjValue == null)
                                    { continue; }

                                    List<MonthlyReportDetail> listCompany = (List<MonthlyReportDetail>)ListItem[i].ObjValue;
                                    List<MonthlyReportDetail> listCompany2 = (List<MonthlyReportDetail>)listItemOther.ObjValue;

                                    for (int l = 0; l < listCompany.Count; l++)
                                    {
                                        Range rangeCompany = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                        rangeCompany.Merge();
                                        rangeCompany.ApplyStyle(style2, flag);
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
                                        worksheet.Cells[rowStart, colStart + 11].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 16].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 17].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 18].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 19].SetStyle(style2);
                                        #endregion
                                        #region 为单元格赋值
                                        var listCompany2Item = listCompany2.Where(m => m.CompanyID == listCompany[l].CompanyID).FirstOrDefault();
                                        worksheet.Cells[rowStart, colStart - 1].PutValue((l + 1).ToString());
                                        worksheet.Cells[rowStart, colStart].PutValue(listCompany[l].CompanyName.ToString());
                                        worksheet.Cells[rowStart, colStart + 2].PutValue(listCompany[l].NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 3].PutValue(listCompany2Item.NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 4].PutValue(listCompany[l].NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 5].PutValue(listCompany2Item.NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 6].PutValue(listCompany[l].NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 7].PutValue(listCompany2Item.NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 8].PutValue(listCompany[l].NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 9].PutValue(listCompany2Item.NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 10].PutValue(listCompany[l].NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 11].PutValue(listCompany2Item.NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 12].PutValue(listCompany[l].NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 13].PutValue(listCompany2Item.NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 14].PutValue(listCompany[l].NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 15].PutValue(listCompany2Item.NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 16].PutValue(listCompany[l].NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 17].PutValue(listCompany2Item.NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 18].PutValue("");
                                        worksheet.Cells[rowStart, colStart + 19].PutValue("");
                                        #endregion
                                        #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                        if (listCompany[l].Counter > 0)
                                        {
                                            int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 18, ImageFilePath + "\\image" + listCompany[l].Counter + ".png");
                                            Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                                            picture.Left = 60;
                                            picture.Top = 10;

                                        }
                                        if (listCompany2Item.Counter > 0)
                                        {
                                            int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 19, ImageFilePath + "\\image" + listCompany2Item.Counter + ".png");
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
                                        worksheet.Cells[rowStart, colStart + 6].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 9].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 12].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 13].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 14].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 15].SetStyle(style2);
                                        #endregion
                                        rowStart = rowStart + 1;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 单指标
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
                                worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[k].NPlanAmmountByYear);
                                worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[k].NPlanAmmountByYear == 0 ? "--" : ListItem[k].NDisplayRateByYear);
                                worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[k].NPlanAmmount);
                                worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[k].NActualAmmount);
                                worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[k].NDisplayRate);
                                worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[k].NAccumulativePlanAmmount);
                                worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[k].NAccumulativeActualAmmount);
                                worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[k].NAccumulativeDisplayRate);
                                worksheet.Cells[rowStart, colStart + 10].PutValue("");

                                #endregion

                                #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                if (DataDisplayMode == 0)
                                    style3.Number = 3;
                                else
                                    style3.Number = 4;

                                worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                #endregion
                                rowStart = rowStart + 1;
                            }
                        }
                        else if (listCompanyProperty[j].Mark == "CompanyProperty")
                        {
                            List<DictionaryVmodel> ListItem = ((List<DictionaryVmodel>)listCompanyProperty[j].ObjValue);
                            if (ListItem.Count > 0 && ListItem.FirstOrDefault().Mark == "Area")
                            {
                                for (int i = 0; i < ListItem.Count; i++)
                                {
                                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);
                                    range.Merge();
                                    range.ApplyStyle(style3, flag);
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
                                    worksheet.Cells[rowStart, colStart - 1].PutValue(ListItem[i].Name);
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[i].BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[i].BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[i].BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[i].BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                    #endregion

                                    #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                    style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style3.Number = 3;
                                    else
                                        style3.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                    #endregion

                                    rowStart = rowStart + 1;

                                    if (ListItem[i].ObjValue == null)
                                    { continue; }

                                    List<DictionaryVmodel> ListItemLastArea = ((List<DictionaryVmodel>)ListItem[i].ObjValue);

                                    for (int k = 0; k < ListItemLastArea.Count; k++)
                                    {
                                        Range itemRange = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                        itemRange.Merge();
                                        itemRange.ApplyStyle(style3, flag);

                                        #region 设置样式
                                        worksheet.Cells[rowStart, colStart - 1].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart].SetStyle(style3);
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
                                        worksheet.Cells[rowStart, colStart - 1].PutValue(k + 1);
                                        worksheet.Cells[rowStart, colStart].PutValue(ListItemLastArea[k].Name);
                                        worksheet.Cells[rowStart, colStart + 2].PutValue(ListItemLastArea[k].BMonthReportDetail.NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 3].PutValue(ListItemLastArea[k].BMonthReportDetail.NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 4].PutValue(ListItemLastArea[k].BMonthReportDetail.NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 5].PutValue(ListItemLastArea[k].BMonthReportDetail.NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 6].PutValue(ListItemLastArea[k].BMonthReportDetail.NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 7].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 8].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 9].PutValue(ListItemLastArea[k].BMonthReportDetail.NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                        #endregion

                                        #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                        style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                        if (DataDisplayMode == 0)
                                            style3.Number = 3;
                                        else
                                            style3.Number = 4;

                                        worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                        worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                        #endregion

                                        rowStart = rowStart + 1;

                                        if (ListItemLastArea[k].ObjValue == null)
                                        { continue; }

                                        List<MonthlyReportDetail> listCompany = (List<MonthlyReportDetail>)ListItemLastArea[k].ObjValue;

                                        for (int l = 0; l < listCompany.Count; l++)
                                        {
                                            Range rangeCompany = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                            rangeCompany.Merge();
                                            rangeCompany.ApplyStyle(style2, flag);
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
                                            worksheet.Cells[rowStart, colStart - 1].PutValue((l + 1).ToString());
                                            worksheet.Cells[rowStart, colStart].PutValue(listCompany[l].CompanyName.ToString());
                                            worksheet.Cells[rowStart, colStart + 2].PutValue(listCompany[l].NPlanAmmountByYear);
                                            worksheet.Cells[rowStart, colStart + 3].PutValue(listCompany[l].NDisplayRateByYear);
                                            worksheet.Cells[rowStart, colStart + 4].PutValue(listCompany[l].NPlanAmmount);
                                            worksheet.Cells[rowStart, colStart + 5].PutValue(listCompany[l].NActualAmmount);
                                            worksheet.Cells[rowStart, colStart + 6].PutValue(listCompany[l].NDisplayRate);
                                            worksheet.Cells[rowStart, colStart + 7].PutValue(listCompany[l].NAccumulativePlanAmmount);
                                            worksheet.Cells[rowStart, colStart + 8].PutValue(listCompany[l].NAccumulativeActualAmmount);
                                            worksheet.Cells[rowStart, colStart + 9].PutValue(listCompany[l].NAccumulativeDisplayRate);
                                            worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                            #endregion
                                            #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                            if (listCompany[l].Counter > 0)
                                            {
                                                int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 10, ImageFilePath + "\\image" + listCompany[l].Counter + ".png");
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
                                            worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                            worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                            #endregion
                                            rowStart = rowStart + 1;
                                        }
                                    }
                                }
                            }
                            else if (ListItem.Count > 0 && ListItem.FirstOrDefault().Mark == "LastArea")
                            {
                                for (int i = 0; i < ListItem.Count; i++)
                                {
                                    Range range = worksheet.Cells.CreateRange(rowStart, 1, 1, 3);
                                    range.Merge();
                                    range.ApplyStyle(style3, flag);
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
                                    worksheet.Cells[rowStart, colStart - 1].PutValue(ListItem[i].Name);
                                    worksheet.Cells[rowStart, colStart + 2].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmountByYear);
                                    worksheet.Cells[rowStart, colStart + 3].PutValue(ListItem[i].BMonthReportDetail.NDisplayRateByYear);
                                    worksheet.Cells[rowStart, colStart + 4].PutValue(ListItem[i].BMonthReportDetail.NPlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 5].PutValue(ListItem[i].BMonthReportDetail.NActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 6].PutValue(ListItem[i].BMonthReportDetail.NDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 7].PutValue(ListItem[i].BMonthReportDetail.NAccumulativePlanAmmount);
                                    worksheet.Cells[rowStart, colStart + 8].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeActualAmmount);
                                    worksheet.Cells[rowStart, colStart + 9].PutValue(ListItem[i].BMonthReportDetail.NAccumulativeDisplayRate);
                                    worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                    #endregion

                                    #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                    style3 = worksheet.Cells[rowStart, colStart + 2].GetStyle();
                                    if (DataDisplayMode == 0)
                                        style3.Number = 3;
                                    else
                                        style3.Number = 4;

                                    worksheet.Cells[rowStart, colStart + 2].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 4].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 5].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 7].SetStyle(style3);
                                    worksheet.Cells[rowStart, colStart + 8].SetStyle(style3);
                                    #endregion

                                    rowStart = rowStart + 1;

                                    if (ListItem[i].ObjValue == null)
                                    { continue; }

                                    List<MonthlyReportDetail> listCompany = (List<MonthlyReportDetail>)ListItem[i].ObjValue;

                                    for (int l = 0; l < listCompany.Count; l++)
                                    {
                                        Range rangeCompany = worksheet.Cells.CreateRange(rowStart, 2, 1, 2);
                                        rangeCompany.Merge();
                                        rangeCompany.ApplyStyle(style2, flag);

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
                                        worksheet.Cells[rowStart, colStart - 1].PutValue((l + 1).ToString());
                                        worksheet.Cells[rowStart, colStart].PutValue(listCompany[l].CompanyName.ToString());
                                        worksheet.Cells[rowStart, colStart + 2].PutValue(listCompany[l].NPlanAmmountByYear);
                                        worksheet.Cells[rowStart, colStart + 3].PutValue(listCompany[l].NDisplayRateByYear);
                                        worksheet.Cells[rowStart, colStart + 4].PutValue(listCompany[l].NPlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 5].PutValue(listCompany[l].NActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 6].PutValue(listCompany[l].NDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 7].PutValue(listCompany[l].NAccumulativePlanAmmount);
                                        worksheet.Cells[rowStart, colStart + 8].PutValue(listCompany[l].NAccumulativeActualAmmount);
                                        worksheet.Cells[rowStart, colStart + 9].PutValue(listCompany[l].NAccumulativeDisplayRate);
                                        worksheet.Cells[rowStart, colStart + 10].PutValue("");
                                        #endregion

                                        #region 设置Number = 3 :千分位,Number = 4 保留2位小数
                                        if (listCompany[l].Counter > 0)
                                        {
                                            int pictureIndex = worksheet.Pictures.Add(rowStart, colStart + 10, ImageFilePath + "\\image" + listCompany[l].Counter + ".png");
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
                                        worksheet.Cells[rowStart, colStart + 4].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 5].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 7].SetStyle(style2);
                                        worksheet.Cells[rowStart, colStart + 8].SetStyle(style2);
                                        #endregion
                                        rowStart = rowStart + 1;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;
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
        public MemoryStream DownExcelMonthReportDetail_Group(string templetePath, string templeteName, string fileName, Guid sytemID, int Year, int Month, bool IsLatestVersion)
        {
            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

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

            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            designer.Workbook = new Workbook(path);
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

            stream.Streams = new MemoryStream();
            XlsSaveOptions xls = new XlsSaveOptions();
            xls.SaveFormat = SaveFormat.Xlsx;
            designer.Workbook.Save(stream.Streams, xls);
            //MemoryStream stream = designer.Workbook.SaveToStream();
            //fileStream.Close();
            //fileStream.Dispose();
            return stream.Streams;

        }


        /// <summary>
        /// 下载历史补回期限汇总
        /// </summary>
        public StreamModel DownHistoryReturnReport()
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



            StreamModel stream = ExportExcel(listHistoryReturn, "HistoryReturn", templetePath, templeteName);
            return stream;
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.Buffer = true;
            //HttpContext.Current.Response.Charset = "utf-8";
            //string dateNow = DateTime.Now.ToString("HHmmss");
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(StaticResource.Instance[SysId, DateTime.Now].SystemName + fileName, System.Text.Encoding.UTF8) + FinYear.ToString() + FinMonth.ToString("D2") + "_" + dateNow + ".xls");
            //HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            //HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            //HttpContext.Current.Response.End();

        }

        /// <summary>
        /// 导出数据生成文件流下载
        /// </summary>
        /// <param name="list">导出的数据列表</param>
        /// <param name="listName">列表名</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        public StreamModel ExportExcel<T>(List<T> list, string listName, string templetePath, string templeteName)
        {

            WorkbookDesigner designer = new WorkbookDesigner();
            StreamModel stream = new StreamModel();

            string path = System.IO.Path.Combine(templetePath, templeteName);
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                designer.Workbook = new Workbook(fileStream);

                designer.SetDataSource("Title", StaticResource.Instance[SysId, DateTime.Now].SystemName + "历史要求期限统计汇总报表");
                designer.SetDataSource("Date", "报告期：" + FinYear.ToString() + "年");
                designer.SetDataSource("Date_Group", FinYear.ToString() + "年");


                designer.SetDataSource(listName, list);
                designer.Process();
                stream.Streams = new MemoryStream();

                XlsSaveOptions xls = new XlsSaveOptions();
                xls.SaveFormat = SaveFormat.Xlsx;
                designer.Workbook.Save(stream.Streams, xls);

                //designer.Workbook.SaveToStream();
                stream.FileName = rpt._System.SystemName + "月度报告";
                //designer.Workbook.Save("d:\\abc.xlsx");
                stream.FileExt = "xlsx";
                //fileStream.Close();
                //fileStream.Dispose();
                return stream;

            }
        }

        bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 正则表达式，去掉首尾的回车和空格
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string _TrimStr(string obj)
        {
            var str = Regex.Replace(obj, "^(\\s*\\n)*|(\\n\\s*)*$", "");
            return str;
        }

        bool IHttpHandler.IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public class StreamModel
        {
            private string systemid;
            public string Systemid
            {
                get { return systemid; }
                set { systemid = value; }
            }

            private MemoryStream streams;
            public MemoryStream Streams
            {
                get { return streams; }
                set { streams = value; }
            }

            private string filename;

            public string FileName
            {
                get { return filename; }
                set { filename = value; }
            }

            public string FileExt
            {
                get;
                set;
            }

            private string attfilename;
            public string Attfilename
            {
                get { return attfilename; }
                set { attfilename = value; }
            }

            private string atturl;

            public string Atturl
            {
                get { return atturl; }
                set { atturl = value; }
            }
        }
    }
}