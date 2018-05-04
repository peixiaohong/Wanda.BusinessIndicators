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
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Common.Web;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Engine.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web.AjaxHandler;
using Lib.Xml;
using Newtonsoft.Json;
using Lib.Web.Json;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// UpLoadMonthTargetDetail 的摘要说明
    /// </summary>
    public class UpLoadMonthTargetDetail : IHttpHandler
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
        Guid MonthReportID = Guid.Empty;
        int FinYear = 0;
        int FinMonth = 0;

        ReportInstance CurrentRpt;

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
                MonthReportID = Guid.Parse(HttpContext.Current.Request["MonthReportID"]);
            }

            //文件相对路径
            string filePath = "UploadFile\\" + WebHelper.DateTimeNow.ToString("yyyy-MM-dd");
            string uploadpath = Path.Combine(UploadFilePath, filePath);






            switch (FileType)
            {
                case "UpTargetPlan": //经营系统

                    string templetePath = uploadpath;
                    string filePathName = Path.Combine(uploadpath, file.FileName);//合并上传文件地址
                    if (Directory.Exists(uploadpath) == false)
                    {
                        Directory.CreateDirectory(uploadpath);//如果上传文件路径不存在，则创建该路径。
                    }
                    file.SaveAs(filePathName); //保存文件到磁盘
                    string error = "";
                    CurrentRpt = new ReportInstance(MonthReportID, true);

                    UpTargetPlanDetailExcel(out error, context, filePathName); //读取上传Excel文件
                    context.Response.Write(error);
                    break;

                case "UpProjectTargetPlan": //项目系统
                    string templeteProPath = uploadpath;
                    string fileProPathName = Path.Combine(uploadpath, file.FileName);//合并上传文件地址
                    if (Directory.Exists(uploadpath) == false)
                    {
                        Directory.CreateDirectory(uploadpath);//如果上传文件路径不存在，则创建该路径。
                    }
                    file.SaveAs(fileProPathName); //保存文件到磁盘
                    string errorPro = "";
                    CurrentRpt = new ReportInstance(MonthReportID, true);

                    UpProjectTargetPlanDetailExcel(out errorPro, context, fileProPathName); //读取上传Excel文件
                    context.Response.Write(errorPro);
                    break;
                case "UpGroupTargetPlan": //集团系统
                    string fileGroupPathName = Path.Combine(uploadpath, file.FileName);//合并上传文件地址
                    if (Directory.Exists(uploadpath) == false)
                    {
                        Directory.CreateDirectory(uploadpath);//如果上传文件路径不存在，则创建该路径。
                    }
                    file.SaveAs(fileGroupPathName); //保存文件到磁盘
                    string errorGroup = "";
                    CurrentRpt = new ReportInstance(MonthReportID, true);

                    UpGroupTargetPlanDetailExcel(out errorGroup, context, fileGroupPathName); //读取上传Excel文件
                    context.Response.Write(errorGroup);
                    break;
                case "UpDirectlyTargetPlan"://直管公司
                    string fileDirectlyPathName = Path.Combine(uploadpath, file.FileName);//合并上传文件地址
                    if (Directory.Exists(uploadpath) == false)
                    {
                        Directory.CreateDirectory(uploadpath);//如果上传文件路径不存在，则创建该路径。
                    }
                    file.SaveAs(fileDirectlyPathName); //保存文件到磁盘
                    string errorDirectly = "";
                    //CurrentRpt = new ReportInstance(MonthReportID, true);
                    CurrentRpt = new ReportInstance(MonthReportID, true, "Draft", false);

                    UpDirectlyTargetPlanDetailExcel(out errorDirectly, context, fileDirectlyPathName); //读取上传Excel文件
                    context.Response.Write(errorDirectly);
                    break;
                case "UpTargetPlanDetail":// 全年计划指标
                    string TargetPlanDetailPathName = Path.Combine(uploadpath, file.FileName);//合并上传文件地址
                    if (Directory.Exists(uploadpath) == false)
                    {
                        Directory.CreateDirectory(uploadpath);//如果上传文件路径不存在，则创建该路径。
                    }
                    file.SaveAs(TargetPlanDetailPathName); //保存文件到磁盘
                    string errorTargetPlanDetail = "";
                    UpLoadTargetPlanDetailExcel(out errorTargetPlanDetail, context, TargetPlanDetailPathName); //读取上传Excel文件
                    context.Response.Write(errorTargetPlanDetail);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        public void UpTargetPlanDetailExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error = "文件没有数据请重新填写！";//文件没有数据请重新填写
                return;
            }
            else
            {
                //ReportInstance CurrentRpt = new ReportInstance(MonthReportID, true);  Update 2017-7-5  做成公有的，便于后面的Json序列化


                var IsBlendTager = excel.GetStringCustomProperty(book.Worksheets[0], "IsBlendTager") == "1";
                for (int j = 0; j < count; j++)
                {
                    if (excel.GetStringCustomProperty(book.Worksheets[j], "SheetName") != "MonthReportDetail")
                    {
                        error = "请上传正确的报表文件！";
                        return;
                    }
                    if (excel.GetStringCustomProperty(book.Worksheets[j], "SystemID") != SysId.ToString())
                    {
                        error = "请下载当前板块的模板！";
                        return;
                    }

                    if (excel.GetStringCustomProperty(book.Worksheets[j], "AreaID") != CurrentRpt.AreaID.ToString())
                    {
                        error = "请下载当前大区的模板！";
                        return;
                    }

                    //混合指标和
                    if (IsBlendTager && j == 0)
                    {
                        var targetNames = book.Worksheets[j].Name.Split('+').ToList();
                        if (targetNames.Count != 2
                            || CurrentRpt._Target.Where(p => p.TargetName == targetNames[0]).ToList().Count < 0
                            || CurrentRpt._Target.Where(p => p.TargetName == targetNames[1]).ToList().Count < 0)
                        {
                            error = "上传数据中的混合指标不是当前系统的指标，请重新添加！";
                            return;
                        }
                    }
                    else
                    {
                        if (CurrentRpt._Target.Where(p => p.TargetName == book.Worksheets[j].Name).ToList().Count < 0)
                        {
                            error = "上传数据中的指标不是当前系统的指标，请重新添加！";
                            return;
                        }
                    }
                }

                List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();

                List<MonthlyReportDetail> listMrd = null;
                book.CalculateFormula();
                for (int i = 0; i < count; i++) //遍历单个sheet条数
                {
                    listMrd = new List<MonthlyReportDetail>();
                    Worksheet workSheet = book.Worksheets[i];
                    Cells cells = workSheet.Cells;
                    if (cells.MaxDataRow - 3 > 0 && cells.MaxDataColumn > 0)
                    {
                        // 上传混合指标
                        if (IsBlendTager && i == 0)
                        {
                            //处理一个sheet上面两个指标，ListDV是引用类型，不用返回
                            var BlendResult = UpTargetPlanDetailExcelForBlend(excel, workSheet, ListDV, out error);
                            if (!BlendResult)
                                return;
                            continue;
                        }
                        DataTable dt = cells.ExportDataTable(4, 1, cells.MaxDataRow - 3, cells.MaxDataColumn);
                        {
                            MonthlyReportDetail mrd = null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                try
                                {
                                    mrd = new MonthlyReportDetail();
                                    if (!string.IsNullOrEmpty(dr[1].ToString()))
                                        mrd.CompanyID = dr[1].ToString().ToGuid();
                                    if (!string.IsNullOrEmpty(dr[2].ToString()))
                                        mrd.CompanyName = dr[2].ToString();
                                    if (!string.IsNullOrEmpty(dr[3].ToString()))
                                        mrd.NPlanAmmount = Convert.ToDecimal(dr[3]); //计划数
                                    if (!string.IsNullOrEmpty(dr[4].ToString()))
                                        mrd.NActualAmmount = Convert.ToDecimal(dr[4]); //实际数
                                    if (dr.ItemArray.Count() > 6)
                                    {
                                        if (!string.IsNullOrEmpty(dr[5].ToString()))
                                            mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[5]); //累计计划数
                                        if (!string.IsNullOrEmpty(dr[6].ToString()))
                                            mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[6]); //累计实际数
                                    }

                                }
                                catch (Exception)
                                {
                                    error = "实际数或计划数为数字必填项。";//添加数据有问题，请重新添加。
                                    return;
                                }

                                listMrd.Add(mrd);
                            }
                        }
                    }
                    try
                    {
                        ListDV.AddRange(FormatTargetDetailNew(out error, listMrd, SysId, FinYear, FinMonth, workSheet.Name, CurrentRpt));
                    }
                    catch (ExcelException)
                    {
                        error = "实际数或计划数为数字必填项。";
                        return;
                    }

                }
                try
                {
                    AddOrUpdateDataNew(ListDV);
                }
                catch (Exception es)
                {
                    error = es + "请添加正确实际数或计划数。";
                    return;
                }
            }

        }

        /// <summary>
        /// 添加混合指标Sheet
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="workSheet"></param>
        /// <param name="ListDV"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool UpTargetPlanDetailExcelForBlend(ExcelEngine excel, Worksheet workSheet, List<DictionaryVmodel> ListDV, out string error)
        {
            error = "";
            List<MonthlyReportDetail> listMrdOne = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> listMrdTwo = new List<MonthlyReportDetail>();
            Cells cells = workSheet.Cells;
            DataTable dt = cells.ExportDataTable(4, 1, cells.MaxDataRow - 3, cells.MaxDataColumn);
            {
                MonthlyReportDetail mrdOne = null;
                MonthlyReportDetail mrdTwo = null;

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        mrdOne = new MonthlyReportDetail();
                        mrdTwo = new MonthlyReportDetail();
                        if (!string.IsNullOrEmpty(dr[1].ToString()))
                        {
                            mrdOne.CompanyID = dr[1].ToString().ToGuid();
                            mrdTwo.CompanyID = dr[1].ToString().ToGuid();
                        }
                        if (!string.IsNullOrEmpty(dr[2].ToString()))
                        {
                            mrdOne.CompanyName = dr[2].ToString();
                            mrdTwo.CompanyName = dr[2].ToString();
                        }
                        if (!string.IsNullOrEmpty(dr[3].ToString()))
                            mrdOne.NPlanAmmount = Convert.ToDecimal(dr[3]); //计划数

                        if (!string.IsNullOrEmpty(dr[4].ToString()))
                            mrdTwo.NPlanAmmount = Convert.ToDecimal(dr[4]); //计划数

                        if (!string.IsNullOrEmpty(dr[5].ToString()))
                            mrdOne.NActualAmmount = Convert.ToDecimal(dr[5]); //实际数

                        if (!string.IsNullOrEmpty(dr[6].ToString()))
                            mrdTwo.NActualAmmount = Convert.ToDecimal(dr[6]); //实际数
                        //if (dr.ItemArray.Count() > 6)
                        //{
                        //    if (!string.IsNullOrEmpty(dr[5].ToString()))
                        //        mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[5]); //累计计划数
                        //    if (!string.IsNullOrEmpty(dr[6].ToString()))
                        //        mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[6]); //累计实际数
                        //}

                    }
                    catch (Exception)
                    {
                        error = "实际数或计划数为数字必填项。";//添加数据有问题，请重新添加。
                        return false;
                    }

                    listMrdOne.Add(mrdOne);
                    listMrdTwo.Add(mrdTwo);
                }

                ListDV.AddRange(FormatTargetDetailNew(out error, listMrdOne, SysId, FinYear, FinMonth, excel.GetStringCustomProperty(workSheet, "TragertName"), CurrentRpt));
                ListDV.AddRange(FormatTargetDetailNew(out error, listMrdTwo, SysId, FinYear, FinMonth, excel.GetStringCustomProperty(workSheet, "TragertTwoName"), CurrentRpt));
            }
            return true;
        }

        public void UpGroupTargetPlanDetailExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);


            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error = "文件没有数据请重新填写！";//文件没有数据请重新填写
                return;
            }
            else
            {
                //ReportInstance CurrentRpt = new ReportInstance(MonthReportID, true); Update 2017-7-5  做成公有的，便于后面的Json序列化

                if (excel.GetStringCustomProperty(book.Worksheets[0], "SheetName") != "GroupMonthReportDetail")
                {
                    error = "请上传正确的报表文件！";
                    return;
                }
                if (excel.GetStringCustomProperty(book.Worksheets[0], "SystemID") != SysId.ToString())
                {
                    error = "请下载当前系统的模板！";
                    return;
                }
                List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();
                Worksheet workSheet = book.Worksheets[0];

                Cells cells = workSheet.Cells;
                book.CalculateFormula();
                if (cells.MaxDataRow - 5 > 0 && cells.MaxDataColumn > 0)
                {

                    DataTable dt = cells.ExportDataTable(6, 1, cells.MaxDataRow - 5, cells.MaxDataColumn);

                    List<MonthlyReportDetail> InsertMonthReportDetail = new List<MonthlyReportDetail>();
                    List<MonthlyReportDetail> UpdateMonthReportDetail = new List<MonthlyReportDetail>();
                    MonthlyReportDetail mrd = null;

                    //获取当年指标计划ID
                    Guid targetPlanID = Guid.Empty;
                    List<A_TargetPlan> CurrentYearTargetPlan = LJTH.BusinessIndicators.BLL.A_TargetplanOperator.Instance.GetTargetplanList(CurrentRpt._System.ID, FinYear).ToList();
                    if (CurrentYearTargetPlan.Count > 0)
                    {
                        targetPlanID = CurrentYearTargetPlan[0].ID;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(dr[0].ToString()) && dr[0].ToString().Trim() != "" && dr[0].ToString().Trim() != "合计")
                        {
                            string TargetNameOrCompanyName = dr[0].ToString();
                            C_Target ctarget = CurrentRpt._Target.Where(p => p.TargetName == TargetNameOrCompanyName).FirstOrDefault();
                            //判断该条数据是否是明细数据。
                            if (ctarget != null)
                            {
                                #region 非明细指标数据
                                V_GroupTargetXElement VGTX = SplitGroupTargetXml(CurrentRpt._System.Configuration).Where(p => p.TargetValue.ToGuid() == ctarget.ID).FirstOrDefault();
                                if (VGTX == null)
                                {
                                    continue;
                                }
                                //如果当前数据有明细项，则跳过该循环。否则把当前数据以总部（公司）插入数据库。
                                if (VGTX.GroupDetail == true)
                                {
                                    continue;
                                }
                                else
                                {
                                    MonthlyReportDetail existMRD = CurrentRpt.ReportDetails.Find(p => p.TargetID == ctarget.ID);

                                    #region 判断数据库是否存在该数据
                                    if (existMRD != null)
                                    {
                                        mrd = existMRD;
                                    }
                                    else
                                    {
                                        mrd = new MonthlyReportDetail();
                                        mrd.SystemID = CurrentRpt._System.ID;
                                        mrd.TargetID = ctarget.ID;
                                        mrd.TargetName = ctarget.TargetName;
                                        mrd.CompanyID = StaticResource.Instance.CompanyList[CurrentRpt._System.ID].Find(p => p.CompanyName.Trim() == "总部").ID;
                                        mrd.MonthlyReportID = MonthReportID;
                                        mrd.FinMonth = FinMonth;
                                        mrd.FinYear = FinYear;
                                        mrd.TargetPlanID = targetPlanID;
                                        mrd.MIssTargetDescription = "";
                                        mrd.MIssTargetReason = "";
                                        mrd.CreateTime = DateTime.Now;
                                        mrd.CommitReason = "";

                                    }
                                    #endregion

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(dr[11].ToString()) && dr[11].ToString().Trim() != "")
                                        {
                                            mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[11].ToString()); //累计实际数
                                            mrd.NActualAmmount = Convert.ToDecimal(dr[11].ToString()); //当月实际数
                                        }
                                        else
                                        {
                                            mrd.NAccumulativeActualAmmount = 0;
                                            mrd.NActualAmmount = 0;
                                        }
                                        if (!string.IsNullOrEmpty(dr[12].ToString()) && dr[12].ToString().Trim() != "")
                                        {
                                            mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[12].ToString()); //累计计划数
                                            mrd.NPlanAmmount = Convert.ToDecimal(dr[12].ToString()); //当月计划数
                                        }
                                        else
                                        {
                                            mrd.NAccumulativePlanAmmount = 0;
                                            mrd.NPlanAmmount = 0;
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        error = "实际数或计划数为数字必填项。";//添加数据有问题，请重新添加。
                                        return;
                                    }
                                    //如果数据库存在当前数据，则把当期数据放入UpdateMonthReportDetail集合中，否则反之。
                                    if (existMRD != null)
                                    {
                                        mrd.ModifyTime = DateTime.Now;
                                        UpdateMonthReportDetail.Add(mrd);
                                    }
                                    else
                                    {
                                        InsertMonthReportDetail.Add(mrd);
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region 明细指标数据
                                string strCompanName = string.IsNullOrEmpty(TargetNameOrCompanyName.Split('、')[1]) ? TargetNameOrCompanyName : TargetNameOrCompanyName.Split('、')[1];
                                List<C_Company> lstCompany = StaticResource.Instance.CompanyList[CurrentRpt._SystemID];
                                C_Company company = lstCompany.Where(p => p.CompanyName == strCompanName).FirstOrDefault();
                                if (company != null)
                                {

                                    int i = 0;
                                    foreach (C_Target ct in CurrentRpt._Target.Where(p => p.HaveDetail == true).OrderBy(p => p.Sequence).ToList())
                                    {

                                        MonthlyReportDetail existMRD = CurrentRpt.ReportDetails.Find(p => p.TargetID == ct.ID && p.CompanyID == company.ID);
                                        #region 判断数据库是否存在该数据
                                        if (existMRD != null)
                                        {
                                            mrd = existMRD;
                                        }
                                        else
                                        {
                                            mrd = new MonthlyReportDetail();
                                            mrd.SystemID = CurrentRpt._System.ID;
                                            mrd.TargetID = ct.ID;
                                            mrd.TargetName = ct.TargetName;
                                            mrd.CompanyID = company.ID;
                                            mrd.MonthlyReportID = MonthReportID;
                                            mrd.FinMonth = FinMonth;
                                            mrd.FinYear = FinYear;
                                            mrd.TargetPlanID = targetPlanID;
                                            mrd.MIssTargetDescription = "";
                                            mrd.MIssTargetReason = "";
                                            mrd.CommitReason = "";
                                            mrd.CreateTime = DateTime.Now;
                                        }
                                        #endregion
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dr[i * 2 + 1].ToString()) && dr[i * 2 + 1].ToString().Trim() != "")
                                            {
                                                mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[i * 2 + 1]); //累计实际数
                                                mrd.NActualAmmount = Convert.ToDecimal(dr[i * 2 + 1]); //当月实际数
                                            }
                                            if (!string.IsNullOrEmpty(dr[i * 2 + 2].ToString()) && dr[i * 2 + 2].ToString().Trim() != "")
                                            {
                                                mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[i * 2 + 2]); //累计计划数
                                                mrd.NPlanAmmount = Convert.ToDecimal(dr[i * 2 + 2]); //当月计划数
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            error = "实际数或计划数为数字必填项。";//添加数据有问题，请重新添加。
                                            return;
                                        }

                                        i++;
                                        if (existMRD != null)
                                        {
                                            mrd.ModifyTime = DateTime.Now;
                                            UpdateMonthReportDetail.Add(mrd);
                                        }
                                        else
                                        {
                                            InsertMonthReportDetail.Add(mrd);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    try
                    {

                        ListDV.Add(new DictionaryVmodel("Insert", InsertMonthReportDetail));
                        ListDV.Add(new DictionaryVmodel("Update", UpdateMonthReportDetail));
                    }
                    catch (ExcelException)
                    {
                        error = "实际数或计划数为数字必填项。";
                        return;
                    }
                }
                try
                {
                    AddOrUpdateData(ListDV);
                }
                catch (Exception)
                {
                    error = "请添加正确实际数或计划数。";
                    return;
                }
            }

        }

        public void UpDirectlyTargetPlanDetailExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error = "文件没有数据请重新填写！";//文件没有数据请重新填写
                return;
            }
            else
            {
                //ReportInstance CurrentRpt = new ReportInstance(MonthReportID, true); Update 2017-7-5  做成公有的，便于后面的Json序列化

                if (excel.GetStringCustomProperty(book.Worksheets[0], "SheetName") != "DirectlyMonthReportDetail")
                {
                    error = "请上传正确的报表文件！";
                    return;
                }
                if (excel.GetStringCustomProperty(book.Worksheets[0], "SystemID") != SysId.ToString())
                {
                    error = "请下载当前系统的模板！";
                    return;
                }
                List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();
                Worksheet workSheet = book.Worksheets[0];

                Cells cells = workSheet.Cells;
                book.CalculateFormula();
                if (cells.MaxDataRow - 3 > 0 && cells.MaxDataColumn > 0)
                {

                    DataTable dt = cells.ExportDataTable(4, 1, cells.MaxDataRow - 3, cells.MaxDataColumn);

                    List<MonthlyReportDetail> InsertMonthReportDetail = new List<MonthlyReportDetail>();
                    List<MonthlyReportDetail> UpdateMonthReportDetail = new List<MonthlyReportDetail>();
                    MonthlyReportDetail mrd = null;
                    List<MonthlyReportDetail> listMrd = new List<MonthlyReportDetail>();

                    //获取当年指标计划ID
                    Guid targetPlanID = Guid.Empty;
                    List<A_TargetPlan> CurrentYearTargetPlan = LJTH.BusinessIndicators.BLL.A_TargetplanOperator.Instance.GetTargetplanList(CurrentRpt._System.ID, FinYear).ToList();
                    if (CurrentYearTargetPlan.Count > 0)
                    {
                        targetPlanID = CurrentYearTargetPlan[0].ID;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        mrd = new MonthlyReportDetail();
                        C_Target CurrentTarget = CurrentRpt._Target.Find(p => p.TargetName == dr[2].ToString());
                        if (CurrentTarget != null)
                        {
                            MonthlyReportDetail existMRD = CurrentRpt.ReportDetails.Find(p => p.TargetID == CurrentTarget.ID);

                            #region 判断数据库是否存在该数据
                            if (existMRD != null)
                            {
                                mrd = existMRD;
                            }
                            else
                            {
                                mrd = new MonthlyReportDetail();
                                mrd.SystemID = CurrentRpt._System.ID;
                                mrd.TargetID = CurrentTarget.ID;
                                mrd.TargetName = CurrentTarget.TargetName;
                                mrd.CompanyID = StaticResource.Instance.CompanyList[CurrentRpt._System.ID].Find(p => p.CompanyName.Trim() == "总部").ID;
                                mrd.MonthlyReportID = MonthReportID;
                                mrd.FinMonth = FinMonth;
                                mrd.FinYear = FinYear;
                                mrd.TargetPlanID = targetPlanID;
                                mrd.MIssTargetDescription = "";
                                mrd.MIssTargetReason = "";
                                mrd.CreateTime = DateTime.Now;
                                mrd.CommitReason = "";
                            }
                            #endregion
                            try
                            {

                                if (!string.IsNullOrEmpty(dr[3].ToString()) && dr[3].ToString().Trim() != "")
                                    mrd.NPlanAmmount = Convert.ToDecimal(dr[3]); //计划数
                                else
                                {
                                    mrd.NPlanAmmount = 0;
                                }
                                if (!string.IsNullOrEmpty(dr[4].ToString()) && dr[4].ToString().Trim() != "")
                                    mrd.NActualAmmount = Convert.ToDecimal(dr[4]); //实际数
                                else
                                {
                                    mrd.NActualAmmount = 0;
                                }
                                //if (dr.ItemArray.Count() > 6)
                                //{
                                //    if (!string.IsNullOrEmpty(dr[5].ToString()))
                                //        mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[5]); //累计计划数
                                //    if (!string.IsNullOrEmpty(dr[6].ToString()))
                                //        mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[6]); //累计实际数
                                //}
                            }
                            catch (Exception)
                            {
                                error = "实际数或计划数为数字必填项。";//添加数据有问题，请重新添加。
                                return;
                            }

                            //如果数据库存在当前数据，则把当期数据放入UpdateMonthReportDetail集合中，否则反之。
                            if (existMRD != null)
                            {
                                mrd.ModifyTime = DateTime.Now;
                                UpdateMonthReportDetail.Add(mrd);
                            }
                            else
                            {
                                InsertMonthReportDetail.Add(mrd);
                            }
                        }
                    }
                    try
                    {

                        ListDV.Add(new DictionaryVmodel("Insert", InsertMonthReportDetail));
                        ListDV.Add(new DictionaryVmodel("Update", UpdateMonthReportDetail));
                    }
                    catch (ExcelException)
                    {
                        error = "实际数或计划数为数字必填项。";
                        return;
                    }
                }
                try
                {
                    AddOrUpdateData(ListDV);
                }
                catch (Exception)
                {
                    error = "请添加正确实际数或计划数。";
                    return;
                }
            }
        }
        /// <summary>
        /// 上传计划指标
        /// </summary>
        /// <param name="error"></param>
        /// <param name="context"></param>
        /// <param name="filePathName"></param>
        public void UpLoadTargetPlanDetailExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);
            book.CalculateFormula();
            int count = book.Worksheets.Count;// sheet个数
                                              // B_MonthlyReport Bt = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            List<C_Target> listTarget = StaticResource.Instance.GetTargetList(SysId, DateTime.Now).OrderBy(p => p.Sequence).ToList();
            if (count == 0)
            {
                error = "文件没有数据请重新填写！";//文件没有数据请重新填写
                return;
            }
            else
            {
                if (excel.GetStringCustomProperty(book.Worksheets[0], "SheetName") != "UpTargetPlanDetail")
                {
                    error = "请上传正确的报表文件！";
                    return;
                }
                if (excel.GetStringCustomProperty(book.Worksheets[0], "SystemID") != SysId.ToString())
                {
                    error = "请下载当前系统的模板！";
                    return;
                }
                List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();

                List<B_TargetPlanDetail> listBTargetPlanDetail = B_TargetplandetailOperator.Instance.GetTargetplandetailList(MonthReportID).ToList();

                DateTime Time = DateTime.Now;
                B_TargetPlan BtargetPlan = B_TargetplanOperator.Instance.GetTargetplan(MonthReportID);
                if (BtargetPlan != null)
                {
                    Time = BtargetPlan.CreateTime;

                }


                List<C_Company> listCompany = StaticResource.Instance.CompanyList[SysId];




                C_System _cSystem = StaticResource.Instance[SysId, Time];
                bool IsYearTargetPlan = false;
                if (_cSystem.Configuration.Elements("TargetPlanDetailReported").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = _cSystem.Configuration.Elements("TargetPlanDetailReported").Elements("TableTemplate").ToList()[0];
                    if (xt.GetAttributeValue("IsYearTargetPlan", "").ToLower() == "true")
                    {
                        IsYearTargetPlan = true;
                    }
                }
                List<B_TargetPlanDetail> InsertTargetPlanDetail = new List<B_TargetPlanDetail>();
                List<B_TargetPlanDetail> UpdateTargetPlanDetail = new List<B_TargetPlanDetail>();
                for (int i = 0; i < listTarget.Count(); i++)
                {
                    Worksheet workSheet;
                    if (_cSystem.Category != 3)
                    {
                        workSheet = book.Worksheets[i + 1];
                    }
                    else
                    {
                        workSheet = book.Worksheets[i];
                    }
                    Cells cells = workSheet.Cells;
                    book.CalculateFormula();
                    if (cells.MaxDataRow - 3 > 0 && cells.MaxDataColumn > 0)
                    {
                        DataTable dt = cells.ExportDataTable(4, 1, cells.MaxDataRow - 3, cells.MaxDataColumn);
                        B_TargetPlanDetail BTargetPlanDetail = null;
                        List<MonthlyReportDetail> listMrd = new List<MonthlyReportDetail>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            C_Company company = listCompany.Where(p => p.CompanyName == dr[1].ToString()) == null ? null : listCompany.Where(p => p.CompanyName == dr[1].ToString()).FirstOrDefault();
                            if (company != null)
                            {
                                if (!IsYearTargetPlan)
                                {
                                    #region 添加12个月的数据
                                    for (int j = 1; j <= 12; j++)
                                    {
                                        List<B_TargetPlanDetail> _BTargetPlanDetail = listBTargetPlanDetail.Where(p => p.TargetID == listTarget[i].ID && p.CompanyID == company.ID && p.FinMonth == j).ToList();

                                        if (_BTargetPlanDetail.Count > 0)
                                        {
                                            BTargetPlanDetail = _BTargetPlanDetail.FirstOrDefault();
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(dr[3 + j].ToString()) && dr[3 + j].ToString().Trim() != "")
                                                {

                                                    BTargetPlanDetail.Target = Convert.ToDecimal(dr[3 + j]);
                                                }
                                                else
                                                {
                                                    BTargetPlanDetail.Target = 0;
                                                }
                                            }
                                            catch (ExcelException)
                                            {
                                                error = "计划指标为数字必填项。";
                                                return;
                                            }
                                            UpdateTargetPlanDetail.Add(BTargetPlanDetail);
                                        }
                                        else
                                        {
                                            BTargetPlanDetail = new B_TargetPlanDetail();
                                            BTargetPlanDetail.SystemID = SysId;
                                            BTargetPlanDetail.TargetID = listTarget[i].ID;
                                            BTargetPlanDetail.CompanyID = company.ID;
                                            BTargetPlanDetail.FinYear = FinYear;
                                            BTargetPlanDetail.FinMonth = j;
                                            BTargetPlanDetail.TargetPlanID = MonthReportID;
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(dr[3 + j].ToString()) && dr[3 + j].ToString().Trim() != "")
                                                {
                                                    BTargetPlanDetail.Target = decimal.Parse(dr[3 + j].ToString());
                                                }
                                                else
                                                {
                                                    BTargetPlanDetail.Target = 0;
                                                }
                                            }
                                            catch (ExcelException)
                                            {
                                                error = "计划指标为数字必填项。";
                                                return;
                                            }

                                            InsertTargetPlanDetail.Add(BTargetPlanDetail);
                                        }

                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 添加年数据（没有月份）
                                    List<B_TargetPlanDetail> _BTargetPlanDetail = listBTargetPlanDetail.Where(p => p.TargetID == listTarget[i].ID && p.CompanyID == company.ID).ToList();
                                    if (_BTargetPlanDetail.Count > 0)
                                    {
                                        #region  更新数据

                                        BTargetPlanDetail = _BTargetPlanDetail.FirstOrDefault();
                                        try
                                        {
                                            if (_cSystem.Category == 3)
                                            {
                                                if (!string.IsNullOrEmpty(dr[2].ToString()) && dr[2].ToString().Trim() != "")
                                                {
                                                    BTargetPlanDetail.Target = decimal.Parse(dr[2].ToString());
                                                }
                                                else
                                                {
                                                    BTargetPlanDetail.Target = 0;
                                                }
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(dr[3].ToString()) && dr[3].ToString().Trim() != "")
                                                {
                                                    BTargetPlanDetail.Target = decimal.Parse(dr[3].ToString());
                                                }
                                                else
                                                {
                                                    BTargetPlanDetail.Target = 0;
                                                }
                                            }

                                        }
                                        catch (ExcelException)
                                        {
                                            error = "计划指标为数字必填项。";
                                            return;
                                        }
                                        UpdateTargetPlanDetail.Add(BTargetPlanDetail);

                                        #endregion
                                    }
                                    else
                                    {
                                        #region 新增数据

                                        BTargetPlanDetail = new B_TargetPlanDetail();
                                        BTargetPlanDetail.SystemID = SysId;
                                        BTargetPlanDetail.TargetID = listTarget[i].ID;
                                        BTargetPlanDetail.CompanyID = company.ID;
                                        BTargetPlanDetail.FinYear = FinYear;
                                        BTargetPlanDetail.FinMonth = 1;
                                        BTargetPlanDetail.TargetPlanID = MonthReportID;
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(dr[2].ToString()) && dr[2].ToString().Trim() != "")
                                            {
                                                BTargetPlanDetail.Target = decimal.Parse(dr[2].ToString());
                                            }
                                            else
                                            {
                                                BTargetPlanDetail.Target = 0;
                                            }
                                        }
                                        catch (ExcelException)
                                        {
                                            error = "计划指标为数字必填项。";
                                            return;
                                        }
                                        InsertTargetPlanDetail.Add(BTargetPlanDetail);

                                        #endregion

                                    }
                                    #endregion
                                }
                            }
                        }

                    }

                }







                try
                {
                    ListDV.Add(new DictionaryVmodel("Insert", InsertTargetPlanDetail));
                    ListDV.Add(new DictionaryVmodel("Update", UpdateTargetPlanDetail));
                    AddOrUpdateTargetPlanDeatilData(ListDV);
                }
                catch (Exception)
                {
                    error = "请添加正确计划指标。";
                    return;
                }
            }


            ///将不上报不考核的原有数据删除
            foreach (C_Target view in listTarget)
            {
                //获取已经上报上去的所有该指标数据    
                List<B_TargetPlanDetail> TargetPlanDetaillistB = B_TargetplandetailOperator.Instance.GetTPListByPlanIDandTargetID(MonthReportID, view.ID).ToList();
                List<C_ExceptionTarget> Exceptionlist = C_ExceptiontargetOperator.Instance.GetNotUpdateListBytargetID(view.ID).ToList();

                for (int i = 0; i < Exceptionlist.Count; i++)
                {
                    for (int j = 0; j < TargetPlanDetaillistB.Count; j++)
                    {
                        if (TargetPlanDetaillistB[j].CompanyID == Exceptionlist[i].CompanyID)
                        {
                            if (B_TargetplandetailOperator.Instance.GetTargetplandetailByID(TargetPlanDetaillistB[j].ID) != null)
                            {
                                B_TargetplandetailOperator.Instance.RemoveTargetplandetail(TargetPlanDetaillistB[j].ID);
                            }

                        }
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xelement"></param>
        /// <returns></returns>
        protected List<V_GroupTargetXElement> SplitGroupTargetXml(XElement xelement)
        {
            List<V_GroupTargetXElement> lstGroupTargetXMl = new List<V_GroupTargetXElement>();
            XElement elementCTD = xelement;
            if (xelement != null)
            {
                if (elementCTD.Elements("ComplateTargetDetail").Elements("Target") != null)
                {
                    List<XElement> Targets = elementCTD.Elements("ComplateTargetDetail").Elements("Target").ToList();
                    V_GroupTargetXElement vt = null;
                    foreach (XElement target in Targets)
                    {
                        vt = new V_GroupTargetXElement(target);
                        lstGroupTargetXMl.Add(vt);
                    }
                }
            }
            return lstGroupTargetXMl;
        }

        /// <summary>
        /// 整合数据
        /// </summary>
        /// <param name="listTartgetDetail">完成情况明细（MonthlyReportDetail）</param>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="targetName">指标名称</param>
        public List<DictionaryVmodel> FormatTargetDetail(out string error, List<MonthlyReportDetail> listTartgetDetail, Guid SystemID, int FinYear, int FinMonth, string targetName, ReportInstance CurrentRpt)
        {
            error = "";
            List<DictionaryVmodel> listdv = new List<DictionaryVmodel>();
            List<MonthlyReportDetail> lstInsertMonthReportDetail = new List<MonthlyReportDetail>();
            List<MonthlyReportDetail> lstUpdateMonthReportDetail = new List<MonthlyReportDetail>();
            Guid targetID;
            try
            {
                targetID = CurrentRpt._Target.Where(p => p.TargetName == targetName).ToList()[0].ID;
            }
            catch (Exception)
            {
                error = "请确认上传的指标是否是本系统的指标!";
                return null;
            }

            //获取当年指标计划ID
            Guid targetPlanID = Guid.Empty;
            List<A_TargetPlan> CurrentYearTargetPlan = LJTH.BusinessIndicators.BLL.A_TargetplanOperator.Instance.GetTargetplanList(SystemID, FinYear).ToList();
            if (CurrentYearTargetPlan.Count > 0)
            {
                targetPlanID = CurrentYearTargetPlan[0].ID;
            }


            //获取当前月的数据
            List<MonthlyReportDetail> listCurrentMonthReportDetail = CurrentRpt.ReportDetails.Where(p => p.TargetID == targetID).ToList();


            MonthlyReportDetail monthlyReportDetail = null;
            //是否存在B_MonthlyReport

            foreach (MonthlyReportDetail mrd in listTartgetDetail)
            {
                string strOperateType = "Insert";
                monthlyReportDetail = new MonthlyReportDetail();
                if (listCurrentMonthReportDetail.Where(p => p.CompanyName == mrd.CompanyName).Count() == 0)//如果数据库没有当月数据，则新增；否则更新数据。
                {
                    monthlyReportDetail.ID = new Guid();
                    monthlyReportDetail.CreateTime = DateTime.Now;
                    monthlyReportDetail.CreatorName = "System";
                    monthlyReportDetail.SystemID = SystemID;//系统ID
                    monthlyReportDetail.FinYear = FinYear;//当前年
                    monthlyReportDetail.FinMonth = FinMonth;//当前月
                    monthlyReportDetail.TargetID = targetID;//指标ID
                    List<C_Company> listCompany = StaticResource.Instance.CompanyList[SystemID].ToList().Where(p => p.CompanyName == mrd.CompanyName).ToList();
                    if (listCompany.Count() == 0)
                    {
                        continue;
                    }
                    else
                    {
                        monthlyReportDetail.CompanyID = listCompany[0].ID; //公司ID
                        monthlyReportDetail.CompanyProperty1 = listCompany[0].CompanyProperty1; //公司ID
                        monthlyReportDetail.CompanyProperty = JsonConvert.SerializeObject(listCompany[0]);
                    }

                    monthlyReportDetail.TargetPlanID = targetPlanID;//计划指标ID

                }
                else
                {
                    monthlyReportDetail = listCurrentMonthReportDetail.Where(p => p.CompanyName == mrd.CompanyName).ToList()[0];

                    //修改数据
                    List<C_Company> listCompany = StaticResource.Instance.CompanyList[SystemID].ToList().Where(p => p.CompanyName == mrd.CompanyName).ToList();
                    if (listCompany.Count() == 0)
                    {
                        continue;
                    }
                    else
                    {
                        monthlyReportDetail.CompanyProperty1 = listCompany[0].CompanyProperty1; //公司ID
                        monthlyReportDetail.CompanyProperty = JsonConvert.SerializeObject(listCompany[0]);
                    }

                    monthlyReportDetail.ModifierName = "System";
                    monthlyReportDetail.ModifyTime = DateTime.Now;
                    strOperateType = "Update";//用于标识更新
                }
                monthlyReportDetail.MonthlyReportID = MonthReportID;//月度报告ID
                monthlyReportDetail.NPlanAmmount = mrd.NPlanAmmount;//计划指标
                monthlyReportDetail.NActualAmmount = mrd.NActualAmmount;// 实际数
                monthlyReportDetail.NDifference = mrd.NActualAmmount - mrd.NPlanAmmount;//差额
                monthlyReportDetail.NAccumulativePlanAmmount = mrd.NAccumulativePlanAmmount;//累计计划指标
                monthlyReportDetail.NAccumulativeActualAmmount = mrd.NAccumulativeActualAmmount;//累计实际数
                monthlyReportDetail.NAccumulativeDifference = mrd.NAccumulativeActualAmmount - mrd.NAccumulativePlanAmmount;//累计差额
                //计算年度累计
                if (strOperateType != "Update")
                {
                    lstInsertMonthReportDetail.Add(monthlyReportDetail);
                }
                else
                {
                    monthlyReportDetail.ModifyTime = DateTime.Now;
                    lstUpdateMonthReportDetail.Add(monthlyReportDetail);
                }
            }

            listdv.Add(new DictionaryVmodel("Insert", lstInsertMonthReportDetail));
            listdv.Add(new DictionaryVmodel("Update", lstUpdateMonthReportDetail));

            return listdv;

        }

        public void AddOrUpdateData(List<DictionaryVmodel> ListDV)
        {
            List<B_MonthlyReportDetail> lstInsertMonthReportDetail = new List<B_MonthlyReportDetail>();
            List<B_MonthlyReportDetail> lstUpdateMonthReportDetail = new List<B_MonthlyReportDetail>();
            foreach (DictionaryVmodel dv in ListDV)
            {
                List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();
                List<MonthlyReportDetail> Listmrd = (List<MonthlyReportDetail>)dv.ObjValue;

                Listmrd.ForEach(p => B_ReportDetails.Add(CalculationEvaluationEngine.CalculationEvaluationService.Calculation(p.ToBModel(), "")));

                if (dv.Name == "Update")
                {
                    lstUpdateMonthReportDetail.AddRange(B_ReportDetails);
                }
                else
                {
                    lstInsertMonthReportDetail.AddRange(B_ReportDetails);
                }
            }
            if (lstInsertMonthReportDetail.Count > 0)
            {
                B_MonthlyreportdetailOperator.Instance.AddOrUpdateTargetDetail(lstInsertMonthReportDetail, "Insert");
            }
            if (lstUpdateMonthReportDetail.Count > 0)
            {
                B_MonthlyreportdetailOperator.Instance.AddOrUpdateTargetDetail(lstUpdateMonthReportDetail, "Update");
            }

            B_MonthlyReport bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            if (bmr != null && lstInsertMonthReportDetail.Count > 0)
            {
                bmr.Status = 5;
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);
            }

            //上报的时候序列化后的Json数据
            SaveJsonData(MonthReportID);

        }

        public void AddOrUpdateTargetPlanDeatilData(List<DictionaryVmodel> ListDV)
        {
            List<B_TargetPlanDetail> lstInsertTargetPlanDetail = new List<B_TargetPlanDetail>();
            List<B_TargetPlanDetail> lstUpdateTargetPlanDetail = new List<B_TargetPlanDetail>();
            foreach (DictionaryVmodel dv in ListDV)
            {
                List<B_TargetPlanDetail> Listmrd = (List<B_TargetPlanDetail>)dv.ObjValue;

                if (dv.Name == "Update")
                {
                    lstUpdateTargetPlanDetail.AddRange(Listmrd);
                }
                else
                {
                    lstInsertTargetPlanDetail.AddRange(Listmrd);
                }
            }
            if (lstInsertTargetPlanDetail.Count > 0)
            {
                B_TargetplandetailOperator.Instance.AddOrUpdateTargetPlanDetail(lstInsertTargetPlanDetail, "Insert");
            }
            if (lstUpdateTargetPlanDetail.Count > 0)
            {
                B_TargetplandetailOperator.Instance.AddOrUpdateTargetPlanDetail(lstUpdateTargetPlanDetail, "Update");
            }

            B_TargetPlan _bTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByID(MonthReportID); //B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            if (_bTargetPlan != null && lstInsertTargetPlanDetail.Count > 0)
            {
                _bTargetPlan.Status = 5;
                B_TargetplanOperator.Instance.UpdateTargetplan(_bTargetPlan);
            }

            //上报的时候序列化后的Json数据
            // SaveJsonData(MonthReportID);

        }


        /// <summary>
        /// 保存，上报的时候序列化后的Json数据
        /// </summary>
        /// <param name="MonthReportID"></param>
        private void SaveJsonData(Guid MonthReportID)
        {
            B_MonthlyReportJsonData Update_JsonData;

            CurrentRpt = new ReportInstance(MonthReportID, true);

            try
            {
                Update_JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(MonthReportID);
            }
            catch (Exception ex)//去Json 表中查看下数据
            {
                Update_JsonData = null;
            }

            if (Update_JsonData != null)
            {
                Update_JsonData.SystemID = SysId;
                Update_JsonData.PlanType = "M";
                Update_JsonData.FinMonth = FinMonth;
                Update_JsonData.FinYear = FinYear;
                Update_JsonData.ModifyTime = DateTime.Now;

                List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();
                //这是上报页面的Json 数据
                TargetReportedControll trc = new TargetReportedControll();
                ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(CurrentRpt)));
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                Update_JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.UpdateMonthlyReportJsonData(Update_JsonData);

            }
            else
            {
                //新增数据
                B_MonthlyReportJsonData JsonData = new B_MonthlyReportJsonData();
                JsonData.ID = MonthReportID;
                JsonData.SystemID = SysId;
                JsonData.PlanType = "M";
                JsonData.FinMonth = FinMonth;
                JsonData.FinYear = FinYear;
                JsonData.CreateTime = DateTime.Now;

                List<DictionaryVmodel> ListObj = new List<DictionaryVmodel>();

                //这是上报页面的Json 数据
                TargetReportedControll trc = new TargetReportedControll();
                ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(CurrentRpt)));
                ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));

                StringBuilder sb = new StringBuilder();
                sb.Append(JsonHelper.Serialize(ListObj)); // 追加所有的出来的数据
                JsonData.ReportJsonData = sb.ToString();

                //这里记录 上传后的Json数据
                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// 上传项目公司的Excel
        /// </summary>
        /// <param name="error"></param>
        /// <param name="context"></param>
        /// <param name="filePathName"></param>
        public void UpProjectTargetPlanDetailExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error = "文件没有数据请重新填写！";//文件没有数据请重新填写
                return;
            }
            else
            {
                //  ReportInstance CurrentRpt = new ReportInstance(MonthReportID, true); // 这里去获取B表中的明细数据    Update 2017-7-5  做成公有的，便于后面的Json序列化

                for (int j = 0; j < count; j++)
                {
                    if (excel.GetStringCustomProperty(book.Worksheets[j], "SheetName") != "ProCompanyReportDetail")
                    {
                        error = "请上传正确的报表文件！";
                        return;
                    }
                }

                string UpFlagStr = "Update";
                List<DictionaryVmodel> ListDV = new List<DictionaryVmodel>();
                List<MonthlyReportDetail> listMrd = null;
                //需要更新的公司列表数据，CompanyProperty1：尾盘
                List<C_Company> UpdateCompanyList = new List<C_Company>();

                for (int i = 0; i < count; i++) //遍历单个sheet条数
                {
                    listMrd = new List<MonthlyReportDetail>();
                    Worksheet workSheet = book.Worksheets[i];
                    Cells cells = workSheet.Cells;

                    #region 单个Sheet页面的数据加载

                    if (cells.MaxDataRow - 5 > 0 && cells.MaxDataColumn > 0)
                    {
                        DataTable dt = cells.ExportDataTable(5, 1, cells.MaxDataRow - 4, cells.MaxDataColumn);
                        MonthlyReportDetail mrd = null;

                        //获取该系统的所有公司
                        List<C_Company> companyList = StaticResource.Instance.CompanyList[SysId].ToList();

                        List<C_Target> targetList = StaticResource.Instance.TargetList[SysId].ToList();


                        //获取当年指标计划ID
                        Guid targetPlanID = Guid.Empty;
                        List<A_TargetPlan> CurrentYearTargetPlan = LJTH.BusinessIndicators.BLL.A_TargetplanOperator.Instance.GetTargetplanList(SysId, FinYear).ToList();
                        if (CurrentYearTargetPlan.Count > 0)
                        {
                            targetPlanID = CurrentYearTargetPlan[0].ID;
                        }


                        foreach (DataRow dr in dt.Rows)
                        {
                            //try
                            //{
                            C_Company companyModel = null;
                            if (!string.IsNullOrEmpty(dr[1].ToString())) //公司
                            {
                                companyModel = companyList.Find(c => c.CompanyName == dr[1].ToString());
                                //公司属性
                                if (!string.IsNullOrEmpty(dr[2].ToString()))
                                {
                                    if (dr[2].ToString() == "是")
                                    {
                                        companyModel.CompanyProperty1 = "尾盘";
                                    }
                                    else
                                    {
                                        companyModel.CompanyProperty1 = string.Empty;
                                    }
                                    UpdateCompanyList.Add(companyModel);
                                }
                            }
                            int flag = 0;

                            #region 首先循环指标
                            //首先循环指标
                            foreach (C_Target item in targetList.OrderBy(g => g.Sequence))
                            {
                                flag++;
                                mrd = new MonthlyReportDetail();
                                mrd.CompanyID = companyModel.ID;
                                mrd.CompanyName = companyModel.CompanyName;
                                mrd.TargetID = item.ID;
                                mrd.TargetName = item.TargetName;

                                mrd.MonthlyReportID = MonthReportID;
                                mrd.SystemID = SysId;
                                mrd.FinMonth = FinMonth;
                                mrd.FinYear = FinYear;
                                mrd.TargetPlanID = targetPlanID;

                                mrd.MIssTargetDescription = "";
                                mrd.MIssTargetReason = "";
                                mrd.CommitReason = "";


                                if (!string.IsNullOrEmpty(dr[2].ToString()))
                                {
                                    if (dr[2].ToString() == "是")
                                    {
                                        mrd.CompanyProperty1 = "尾盘";
                                    }
                                    else
                                    {
                                        mrd.CompanyProperty1 = string.Empty;
                                    }
                                }

                                mrd.CompanyProperty = JsonConvert.SerializeObject(companyModel);


                                //判断在B表中的数据，是否 > 0  ? 如果有 代表Update 反之 新增
                                if (CurrentRpt.ReportDetails.Count > 0)
                                {
                                    MonthlyReportDetail temp = CurrentRpt.ReportDetails.Find(p => p.TargetID == item.ID && p.CompanyID == companyModel.ID);

                                    if (temp == null)
                                    {
                                        mrd.ID = Guid.NewGuid();
                                        UpFlagStr = "Insert";
                                    }
                                    else
                                    {
                                        mrd.ID = CurrentRpt.ReportDetails.Find(p => p.TargetID == item.ID && p.CompanyID == companyModel.ID).ID;
                                    }


                                }
                                else
                                {
                                    mrd.ID = Guid.NewGuid();
                                    UpFlagStr = "Insert";
                                }

                                if (flag == 1) //合同
                                {
                                    if (!string.IsNullOrEmpty(dr[3].ToString()) && dr[3].ToString().Trim() != "")
                                        mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[3].ToString()); //累计计划
                                    else
                                    {
                                        mrd.NAccumulativePlanAmmount = 0;
                                    }
                                    if (!string.IsNullOrEmpty(dr[6].ToString()) && dr[6].ToString().Trim() != "")
                                        mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[6].ToString()); //累计实际
                                    else
                                    {
                                        mrd.NAccumulativeActualAmmount = 0;
                                    }
                                    if (!string.IsNullOrEmpty(dr[9].ToString()) && dr[9].ToString().Trim() != "")
                                        mrd.NPlanAmmount = Convert.ToDecimal(dr[9].ToString()); //当月计划
                                    else
                                    {
                                        mrd.NPlanAmmount = 0;
                                    }
                                    if (!string.IsNullOrEmpty(dr[12].ToString()) && dr[12].ToString().Trim() != "")
                                        mrd.NActualAmmount = Convert.ToDecimal(dr[12].ToString()); //当月实际
                                    else
                                    {
                                        mrd.NActualAmmount = 0;
                                    }
                                }
                                else if (flag == 2) //回款
                                {
                                    if (!string.IsNullOrEmpty(dr[4].ToString()) && dr[4].ToString().Trim() != "")
                                    {
                                        mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[4].ToString());
                                    }
                                    else
                                    {
                                        mrd.NAccumulativePlanAmmount = 0;
                                    }
                                    if (!string.IsNullOrEmpty(dr[7].ToString()) && dr[7].ToString().Trim() != "")
                                    {
                                        mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[7].ToString());
                                    }
                                    else
                                    {
                                        mrd.NAccumulativeActualAmmount = 0;
                                    }
                                    if (!string.IsNullOrEmpty(dr[10].ToString()) && dr[10].ToString().Trim() != "")
                                    {
                                        mrd.NPlanAmmount = Convert.ToDecimal(dr[10].ToString());
                                    }
                                    else
                                    {
                                        mrd.NPlanAmmount = 0;
                                    }
                                    if (!string.IsNullOrEmpty(dr[13].ToString()) && dr[13].ToString().Trim() != "")
                                    {
                                        mrd.NActualAmmount = Convert.ToDecimal(dr[13].ToString());
                                    }
                                    else
                                    {
                                        mrd.NActualAmmount = 0;
                                    }
                                }
                                else if (flag == 3) //入伙
                                {
                                    if (!string.IsNullOrEmpty(dr[5].ToString()) && dr[5].ToString().Trim() != "")
                                    {
                                        mrd.NAccumulativePlanAmmount = Convert.ToDecimal(dr[5].ToString());
                                    }
                                    else
                                    {
                                        mrd.NAccumulativePlanAmmount = 0;
                                    }

                                    if (!string.IsNullOrEmpty(dr[8].ToString()) && dr[8].ToString().Trim() != "")
                                    {
                                        mrd.NAccumulativeActualAmmount = Convert.ToDecimal(dr[8].ToString());
                                    }
                                    else
                                    {
                                        mrd.NAccumulativeActualAmmount = 0;
                                    }

                                    if (!string.IsNullOrEmpty(dr[11].ToString()) && dr[11].ToString().Trim() != "")
                                    {
                                        mrd.NPlanAmmount = Convert.ToDecimal(dr[11].ToString());
                                    }
                                    else
                                    {
                                        mrd.NPlanAmmount = 0;
                                    }

                                    if (!string.IsNullOrEmpty(dr[14].ToString()) && dr[14].ToString().Trim() != "")
                                    {
                                        mrd.NActualAmmount = Convert.ToDecimal(dr[14].ToString());
                                    }
                                    else
                                    {
                                        mrd.NActualAmmount = 0;
                                    }
                                }
                                mrd.CreateTime = DateTime.Now;
                                listMrd.Add(mrd);
                            }
                            #endregion

                            //}
                            //catch (Exception ex)
                            //{

                            //    error = "添加数据有问题，请重新添加";
                            //    error = ex.ToString();
                            //    return;
                            //}
                        }
                    }

                    #endregion

                    DictionaryVmodel dictV = new DictionaryVmodel(UpFlagStr, listMrd);
                    DictionaryVmodel dictC = new DictionaryVmodel("Update", UpdateCompanyList);
                    ListDV.Add(dictC);
                    ListDV.Add(dictV);
                }

                AddOrUpdate(ListDV);
            }

        }

        /// <summary>
        /// 项目公司用
        /// </summary>
        /// <param name="ListDictV"></param>
        public void AddOrUpdate(List<DictionaryVmodel> ListDictV)
        {
            string _flag = "Update";

            List<B_MonthlyReportDetail> MonthReportDetail = new List<B_MonthlyReportDetail>();

            #region 更新公司属性数据

            List<C_Company> _companyList = (List<C_Company>)ListDictV[0].ObjValue;
            if (_companyList.Count > 0)
            {
                C_CompanyOperator.Instance.UpdateCompanyList(_companyList);
            }

            #endregion


            #region 更新明细数据

            List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();
            List<MonthlyReportDetail> Listmrd = (List<MonthlyReportDetail>)ListDictV[1].ObjValue;

            Listmrd.ForEach(p => B_ReportDetails.Add(CalculationEvaluationEngine.CalculationEvaluationService.Calculation(p.ToBModel(), "")));
            _flag = ListDictV[1].Name;
            if (ListDictV[1].Name == "Update")
            {
                MonthReportDetail.AddRange(B_ReportDetails);
            }
            else
            {
                MonthReportDetail.AddRange(B_ReportDetails);
            }

            B_MonthlyreportdetailOperator.Instance.AddOrUpdateTargetDetail(MonthReportDetail, _flag);

            #endregion

            B_MonthlyReport bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            if (bmr != null)
            {
                bmr.Status = 4;
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);

            }

            //上报的时候序列化后的Json数据
            SaveJsonData(MonthReportID);
        }


        #region 优化上传指标  ProcessMulitiVersionTarget 页面也有同样的方法，有时间记得整理到一起

        /// <summary>
        /// 整合数据（优化：改成删除后批量插入）
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listTartgetDetail">完成情况明细</param>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <param name="targetName">指标名称</param>
        /// <param name="CurrentRpt">当前Rpt</param>
        /// <returns></returns>
        public List<DictionaryVmodel> FormatTargetDetailNew(out string error, List<MonthlyReportDetail> listTartgetDetail, Guid SystemID, int FinYear, int FinMonth, string targetName, ReportInstance CurrentRpt)
        {
            error = "";
            List<DictionaryVmodel> listdv = new List<DictionaryVmodel>();
            List<MonthlyReportDetail> lstInsertMonthReportDetail = new List<MonthlyReportDetail>();
            Guid targetID;
            try
            {
                targetID = CurrentRpt._Target.Where(p => p.TargetName == targetName).ToList()[0].ID;
            }
            catch (Exception)
            {
                error = "请确认上传的指标是否是本系统的指标!";
                return null;
            }

            //获取当年指标计划ID
            Guid targetPlanID = Guid.Empty;
            List<A_TargetPlan> CurrentYearTargetPlan = LJTH.BusinessIndicators.BLL.A_TargetplanOperator.Instance.GetTargetplanList(SystemID, FinYear).ToList();
            if (CurrentYearTargetPlan.Count > 0)
            {
                targetPlanID = CurrentYearTargetPlan[0].ID;
            }


            //获取当前月的数据
            List<MonthlyReportDetail> listCurrentMonthReportDetail = CurrentRpt.ReportDetails.Where(p => p.TargetID == targetID).ToList();


            MonthlyReportDetail monthlyReportDetail = null;
            //是否存在B_MonthlyReport

            foreach (MonthlyReportDetail mrd in listTartgetDetail)
            {
                monthlyReportDetail = new MonthlyReportDetail();

                monthlyReportDetail.ID = new Guid();
                monthlyReportDetail.CreateTime = DateTime.Now;
                monthlyReportDetail.CreatorName = "System";
                monthlyReportDetail.SystemID = SystemID;//系统ID
                monthlyReportDetail.FinYear = FinYear;//当前年
                monthlyReportDetail.FinMonth = FinMonth;//当前月
                monthlyReportDetail.TargetID = targetID;//指标ID
                List<C_Company> listCompany = StaticResource.Instance.CompanyList[SystemID].ToList().Where(p => p.CompanyName == mrd.CompanyName).ToList();
                if (listCompany.Count() == 0)
                {
                    continue;
                }
                else
                {
                    monthlyReportDetail.CompanyID = listCompany[0].ID; //公司ID
                    monthlyReportDetail.CompanyProperty1 = listCompany[0].CompanyProperty1; //公司ID
                    monthlyReportDetail.CompanyProperty = JsonConvert.SerializeObject(listCompany[0]);
                }

                monthlyReportDetail.TargetPlanID = targetPlanID;//计划指标ID
                monthlyReportDetail.MonthlyReportID = MonthReportID;//月度报告ID
                monthlyReportDetail.NPlanAmmount = mrd.NPlanAmmount;//计划指标
                monthlyReportDetail.NActualAmmount = mrd.NActualAmmount;// 实际数
                monthlyReportDetail.NDifference = mrd.NActualAmmount - mrd.NPlanAmmount;//差额
                monthlyReportDetail.NAccumulativePlanAmmount = mrd.NAccumulativePlanAmmount;//累计计划指标
                monthlyReportDetail.NAccumulativeActualAmmount = mrd.NAccumulativeActualAmmount;//累计实际数
                monthlyReportDetail.NAccumulativeDifference = mrd.NAccumulativeActualAmmount - mrd.NAccumulativePlanAmmount;//累计差额

                //计算年度累计
                lstInsertMonthReportDetail.Add(monthlyReportDetail);
            }

            listdv.Add(new DictionaryVmodel("Insert", lstInsertMonthReportDetail));
            return listdv;
        }


        public void AddOrUpdateDataNew(List<DictionaryVmodel> ListDV)
        {
            List<B_MonthlyReportDetail> lstInsertMonthReportDetail = new List<B_MonthlyReportDetail>();
            foreach (DictionaryVmodel dv in ListDV)
            {
                List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();
                List<MonthlyReportDetail> Listmrd = (List<MonthlyReportDetail>)dv.ObjValue;

                Listmrd.ForEach(p => B_ReportDetails.Add(CalculationEvaluationEngine.CalculationEvaluationService.Calculation(p.ToBModel(), "")));
                lstInsertMonthReportDetail.AddRange(B_ReportDetails);
            }
            if (lstInsertMonthReportDetail.Count > 0)
            {
                //优化，批量插入
                B_MonthlyreportdetailOperator.Instance.BulkAddTargetDetail(lstInsertMonthReportDetail);
            }
            B_MonthlyReport bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            if (bmr != null && lstInsertMonthReportDetail.Count > 0)
            {
                bmr.Status = 5;
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);
            }

            //上报的时候序列化后的Json数据
            SaveJsonData(MonthReportID);

        }
        #endregion
    }
}