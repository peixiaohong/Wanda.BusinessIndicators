using Aspose.Cells;
using Lib.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Common.Web;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Engine.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web.AjaxHandler;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// UpLoadExcelCommitDate 的摘要说明
    /// </summary>
    public class UpLoadExcelCommitDate : IHttpHandler
    {

        private string _uploadFilePath = null;

        private string UploadFilePath
        {
            get
            {
                if (_uploadFilePath == null)
                {
                    _uploadFilePath = ConfigurationManager.AppSettings["UploadFilePath"];

                    _uploadFilePath = ConfigurationManager.AppSettings["UploadFilePath"];
                    //_uploadFilePath = HttpContext.Current.Server.MapPath(_uploadFilePath);
                }

                if (Directory.Exists(_uploadFilePath) == false)
                {
                    if (NetworkSharedFolder.IsNetShareMappedPath(_uploadFilePath))
                    {
                        //NetworkSharedFolder nsf = new NetworkSharedFolder();
                        string remotePath = AppSettingConfig.GetSetting("FileServer_RemotePath", "");
                        string localPath = AppSettingConfig.GetSetting("FileServer_LocalPath", "");
                        string username = AppSettingConfig.GetSetting("FileServer_UserName", "");
                        string password = AppSettingConfig.GetSetting("FileServer_UserPassword", "");

                        NetworkSharedFolder.Connect(remotePath, localPath, username, password);
                        if (Directory.Exists(_uploadFilePath) == false)
                        {
                            throw new ApplicationException("未在配置中找到上传存放路径， 或指定路径不存在！");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("未在配置中找到上传存放路径， 或指定路径不存在！");
                    }
                }
                return _uploadFilePath;
            }
        }
        string FileType = string.Empty; //下载的文件类型
        Guid SysId = Guid.Empty;
        int FinYear = 0;
        int FinMonth = 0;
        C_System SystemModel = new C_System();
        DateTime T = DateTime.Now;
        A_MonthlyReportDetail Amodel = new A_MonthlyReportDetail();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpPostedFile file = context.Request.Files["FileData"];
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
            //文件相对路径
            string filePath = "UploadFile\\" + WebHelper.DateTimeNow.ToString("yyyy-MM-dd");
            string uploadpath = Path.Combine(UploadFilePath, filePath);


            string templetePath = uploadpath;
            string filePathName = Path.Combine(uploadpath, file.FileName);//合并上传文件地址
            if (Directory.Exists(uploadpath) == false)
            {
                Directory.CreateDirectory(uploadpath);//如果上传文件路径不存在，则创建该路径。
            }
            file.SaveAs(filePathName); //保存文件到磁盘
            string error = "";
            //UpTargetPlanDetailExcel(out error, context, filePathName); //读取上传Excel文件

            if (SystemModel.Category == 4)
            {
                UpLoadExcel2(out  error, context, filePathName);
            }
            else
            {
                UpLoadExcel(out  error, context, filePathName);
            }
            context.Response.Write(error);
            //context.Response.Write("Hello World");
        }
        public void UpLoadExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);
            List<C_Target> Targetlist = StaticResource.Instance.GetTargetList(SysId, T).Where(p => p.NeedEvaluation = true).ToList();
          
            //var newTargetlist = Targetlist.Where(
            //    p => p.TargetName != "客流量"
            //    ).OrderBy(p => p.Sequence);
            //List<C_Target> NewTarget = new List<C_Target>();
            //foreach (C_Target item in newTargetlist)
            //{
            //    NewTarget.Add(item);
            //}

            int count = book.Worksheets.Count;
            List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();
            List<A_MonthlyReportDetail> CommitDetailList = new List<A_MonthlyReportDetail>();


            if (excel.GetStringCustomProperty(book.Worksheets[0], "Time") != FinYear + "-" + FinMonth)
            {
                error = "请上传所选月的文件!";
                return;
            }
            for (int i = 0; i < count; i++)
            {
                #region 验证excel
                if (excel.GetStringCustomProperty(book.Worksheets[i], "SheetName") != "CommitUpdate")
                {
                    error = "请上传正确的报表文件！";
                    return;
                }
                if (excel.GetStringCustomProperty(book.Worksheets[i], "SystemID") != SysId.ToString())
                {
                    error = "请下载当前系统的模板！";
                    return;
                }
                if (Targetlist.Where(p => p.TargetName == book.Worksheets[i].Name).ToList().Count < 0)
                {
                    error = "上传数据中的指标不是当前系统的指标，请重新添加！";
                    return;
                }
                #endregion


                string TargetID = excel.GetStringCustomProperty(book.Worksheets[i], "TargetID").ToString();
                Worksheet workSheet = book.Worksheets[i];
                Cells cells = workSheet.Cells;
                if (cells.MaxDataRow - 2 > 0 && cells.MaxDataColumn == 7)
                {

                    DataTable dt = cells.ExportDataTable(3, 1, cells.MaxDataRow - 2, cells.MaxDataColumn);
                    {

                        string NowDate = null;
                        if (FinMonth == 12)
                        {
                            int a = FinYear + 01;
                            NowDate = a + "-" + 01;
                        }
                        else
                        {
                            int a = FinMonth + 1;
                            if (a < 10)
                            {
                                NowDate = FinYear + "-0" + a;
                            }
                            else
                            {
                                NowDate = FinYear + "-" + a;
                            }
                        }//获取下个月 为判断IsCommitDate做准备
                        foreach (DataRow item in dt.Rows)
                        {
                            try
                            {
                                Amodel = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(SysId, item[6].ToString().ToGuid(), TargetID.ToGuid(), FinYear, FinMonth);
                            }
                            catch (Exception)
                            {
                                error = "数据模板错误,请重新下载填写";//添加数据有问题，请重新添加。
                                return;
                            }
                            try
                            {
                                if (item[3].ToString() != "" && item[3].ToString() != null)
                                {
                                    DateTime time = DateTime.Parse(item[3].ToString());//获取excel要求时间
                                    Amodel.CurrentMonthCommitDate = time;
                                    string changtime = time.ToString("yyyy-MM");
                                    if (DateTime.Parse(changtime) < DateTime.Parse(FinYear + "-" + FinMonth))
                                    {
                                        error = "时间格式错误,请检查!";
                                        return;
                                    }
                                    if (changtime == NowDate)
                                    {
                                        Amodel.IsCommitDate = 1;
                                    }
                                    else
                                    {
                                        Amodel.IsCommitDate = 0;
                                    }
                                }
                                else
                                {
                                    error = "时间不能为空,请检查!";
                                    return;
                                }



                            }
                            catch (Exception)
                            {
                                error = "要求时间填写错误!";//添加数据有问题，请重新添加。
                                return;
                            }
                            Amodel.CurrentMonthCommitReason = item[4].ToString();
                            CommitDetailList.Add(Amodel);
                        }
                    }
                }
            }
            updateModle(CommitDetailList);
        }
        //将读出的数据update
        public void updateModle(List<A_MonthlyReportDetail> list)
        {
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    A_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(list[i]);
                }
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public void UpLoadExcel2(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);
            int count = book.Worksheets.Count;
            List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();
            List<A_MonthlyReportDetail> CommitDetailList = new List<A_MonthlyReportDetail>();

            if (excel.GetStringCustomProperty(book.Worksheets[0], "Time") != FinYear + "-" + FinMonth)
            {
                error = "请上传所选月的文件!";
                return;
            }
            for (int i = 0; i < count; i++)
            {
                #region 验证excel
                string aa = excel.GetStringCustomProperty(book.Worksheets[0], "SheetName");
                string bb = excel.GetStringCustomProperty(book.Worksheets[0], "SystemID");
                if (excel.GetStringCustomProperty(book.Worksheets[0], "SheetName") != "CommitUpdate")
                {
                    error = "请上传正确的报表文件！";
                    return;
                }
                if (excel.GetStringCustomProperty(book.Worksheets[0], "SystemID") != SysId.ToString())
                {
                    error = "请下载当前系统的模板！";
                    return;
                }
                Worksheet workSheet = book.Worksheets[i];
                Cells cells = workSheet.Cells;

                #endregion
                if (cells.MaxDataRow - 2 > 0 && cells.MaxDataColumn == 8)
                {
                    try
                    {
                        DataTable dt = cells.ExportDataTable(3, 1, cells.MaxDataRow - 2, cells.MaxDataColumn);
                        {
                            string NowDate = null;
                            if (FinMonth == 12)
                            {
                                int a = FinYear + 01;
                                NowDate = a + "-" + 01;
                            }
                            else
                            {
                                int a = FinMonth + 1;
                                if (a < 10)
                                {
                                    NowDate = FinYear + "-0" + a;
                                }
                                else
                                {
                                    NowDate = FinYear + "-" + a;
                                }
                            }//获取下个月 为判断IsCommitDate做准备
                            foreach (DataRow item in dt.Rows)
                            {
                                try
                                {
                                    Amodel = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(SysId, item[6].ToString().ToGuid(), item[7].ToString().ToGuid(), FinYear, FinMonth);
                                }
                                catch (Exception)
                                {
                                    error = "数据模板错误,请重新下载填写";//添加数据有问题，请重新添加。
                                    return;
                                }
                                try
                                {
                                    DateTime time = DateTime.Parse(item[3].ToString());//获取excel要求时间
                                    Amodel.CurrentMonthCommitDate = time;
                                    string changtime = time.ToString("yyyy-MM");
                                    if (DateTime.Parse(changtime) < DateTime.Parse(FinYear + "-" + FinMonth))
                                    {
                                        error = "要求时间填写错误!";
                                        return;

                                    }
                                    if (changtime == NowDate)
                                    {
                                        Amodel.IsCommitDate = 1;
                                    }
                                    else
                                    {
                                        Amodel.IsCommitDate = 0;
                                    }

                                }
                                catch (Exception)
                                {
                                    error = "要求时间填写错误!";//添加数据有问题，请重新添加。
                                    return;
                                }
                                Amodel.CurrentMonthCommitReason = item[4].ToString();
                                CommitDetailList.Add(Amodel);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        error = "数据填写错误!";//添加数据有问题，请重新添加。
                        return;
                    }
                }
            }
            updateModle(CommitDetailList);
        }
    }
}