using Aspose.Cells;
using Lib.Config;
using Lib.Core;
using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Common.Web;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web.AjaxHandler;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// ExcelReport 的摘要说明
    /// </summary>
    public class ExcelReport : IHttpHandler, IRequiresSessionState
    {
        private string _uploadFilePath = null;

        private string UploadFilePath
        {
            get
            {
                if (_uploadFilePath == null)
                {
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
        int FinYear = DateTime.Now.Year;
        int FinMonth = DateTime.Now.Month - 1;
        Guid MonthRptID = Guid.Empty;

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
                MonthRptID = Guid.Parse(HttpContext.Current.Request["MonthReportID"]);
            }

            //文件相对路径
            string filePath = "UploadFile\\" + WebHelper.DateTimeNow.ToString("yyyy-MM-dd");
            string uploadpath = Path.Combine(UploadFilePath, filePath);

            //创建文件夹
            if (!Directory.Exists(uploadpath))
            {
                Directory.CreateDirectory(uploadpath);
            }

            List<MonthlyReportDetail> list = null;
            StringBuilder error = new StringBuilder();
            ReportInstance rpt = new ReportInstance(MonthRptID, true); //这里上报的时候，需要的是草稿状态

            string templetePath = uploadpath;
            string filePathName = Path.Combine(uploadpath, file.FileName);
            file.SaveAs(filePathName);

            switch (FileType)
            {
                case "MissTargetRpt": //上传未完成说明模版（上报） 方法1

                    list = ReadMonthlyReportDetailExcel(filePathName, error, rpt, MonthRptID);

                    if (list != null && list.Count != 0)
                    {
                        List<B_MonthlyReportDetail> MissTargetData = new List<B_MonthlyReportDetail>();

                        list.ForEach(P => MissTargetData.Add(P.ToBModel()));

                        B_MonthlyReport monthRptModel = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthRptID);

                        //判断状态，如果小于3，
                        if (monthRptModel.Status < 4)
                        {
                            monthRptModel.Status = 4;
                            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(monthRptModel);
                        }
                        //入库
                        B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportDetailList(MissTargetData);

                        //未完成 更新数据后，修改Json
                        SaveJsonData(MonthRptID);

                    }
                    else
                    {
                        error = error.Append("上传的模版数据不正确，请确认后重新上传");
                        context.Response.Write(error);

                    }
                    break;

                case "CurrentMissTargetRpt": //上传 当月 未完成说明模版（上报） 方法1

                    list = ReadCurrentMonthlyReportDetailExcel(filePathName, error, rpt, MonthRptID);

                    if (list != null && list.Count != 0)
                    {
                        List<B_MonthlyReportDetail> MissTargetData = new List<B_MonthlyReportDetail>();

                        list.ForEach(P => MissTargetData.Add(P.ToBModel()));

                        B_MonthlyReport monthRptModel = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthRptID);

                        //判断状态，如果小于3，
                        if (monthRptModel.Status < 4)
                        {
                            monthRptModel.Status = 4;
                            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(monthRptModel);
                        }
                        //入库
                        B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportDetailList(MissTargetData);

                        //未完成 更新数据后，修改Json
                        SaveJsonData(MonthRptID);

                    }
                    else
                    {
                        error = error.Append("上传的模版数据不正确，请确认后重新上传");
                        context.Response.Write(error);

                    }
                    break;


                case "DirectlyMissTargetRpt": //上传直属公司未完成说明模版（上报）

                    list = ReadDirectlyMonthlyReportDetailExcel(filePathName, error, rpt, MonthRptID);

                    if (list != null && list.Count != 0)
                    {
                        List<B_MonthlyReportDetail> MissTargetData = new List<B_MonthlyReportDetail>();
                        list.ForEach(P => MissTargetData.Add(P.ToBModel()));
                        //入库
                        B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportDetailList(MissTargetData);

                        //未完成 更新数据后，修改Json
                        SaveJsonData(MonthRptID);
                    }
                    else
                    {
                        context.Response.Write(error);

                    }
                    break;

                case "CurrentDirectlyMissTargetRpt": //上传  当月 直属公司未完成说明模版（上报）

                    list = ReadDirectlyMonthlyReportDetailExcel(filePathName, error, rpt, MonthRptID);

                    if (list != null && list.Count != 0)
                    {
                        List<B_MonthlyReportDetail> MissTargetData = new List<B_MonthlyReportDetail>();
                        list.ForEach(P => MissTargetData.Add(P.ToBModel()));
                        //入库
                        B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportDetailList(MissTargetData);

                        //未完成 更新数据后，修改Json
                        SaveJsonData(MonthRptID);
                    }
                    else
                    {
                        context.Response.Write(error);

                    }
                    break;


            }




        }


        /// <summary>
        /// 当月未完成数据导入
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="error"></param>
        /// <param name="rpt"></param>
        /// <param name="MonthRptID"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> ReadCurrentMonthlyReportDetailExcel(string filePathName, StringBuilder error, ReportInstance rpt, Guid MonthRptID)
        {
            List<MonthlyReportDetail> list = new List<MonthlyReportDetail>();
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(filePathName);
            Worksheet sheet = book.Worksheets[0];

            if (excel.GetStringCustomProperty(sheet, "SheetName") != "MissTargetRptExcel")
            {
                error.Append("请上传正确的报表文件");
                return null;
            }

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error.Append("模板错误");
                return null;
            }

            WorksheetCollection sheets = book.Worksheets;

            //当次上报的 所有未完成公司的指标
            List<MonthlyReportDetail> missTargetList = rpt.ReportDetails;

            //循环Sheet
            for (int i = 0; i < count; i++)
            {
                //当次上报的 所有未完成公司的指标
                int startRow = 4;
                int endRow = 0;
                int startColumn = 1;
                int endColumn = 15;

                Cells cells = sheets[i].Cells;
                endRow = cells.MaxDataRow;
                endColumn = cells.MaxDataColumn;


                DataTable dt = new DataTable();
                if (endRow >= startRow) //这里判断Excel里最大行数必须大于开始读取的行数
                {
                    //读取Excel中的数据
                    dt = cells.ExportDataTableAsString(startRow, startColumn, endRow - startRow + 1, endColumn);
                }

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime PromissDate = DateTime.Now;
                        //Excel中公司的名称和指标名称==来确定给Model

                        if (dr[1].ToString() != "未完成合计") // 这里为了排除商管多加出来的合计项
                        {
                            MonthlyReportDetail data = missTargetList.Where(M => M.CompanyName == dr[1].ToString() && M.TargetName == dr[2].ToString()).FirstOrDefault();
                            if (data == null)
                            {
                                continue; //如果是不包含指标跳过
                            }
                            
                           
                            if (data.TargetID.ToString().ToUpper() == AppSettingConfig.GetSetting("MonthSGRent", "").ToUpper() && data.NDifference < 0 && data.IsMissTarget == false)
                            {
                                data.CurrentMIssTargetReason = dr[11].ToString() + "\n" ;//原因
                                data.CurrentMIssTargetDescription = dr[12].ToString();//采取措施
                            }
                            else
                            {
                                data.CurrentMIssTargetReason = dr[11].ToString() + "\n";//原因
                                data.CurrentMIssTargetDescription = dr[12].ToString();//采取措施
                            }
                           

                            list.Add(data);
                        }
                    }
                }

            }
            return list;
        }



        /// <summary>
        /// 累计未完成数据导入(经营模版的)
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="error"></param>
        /// <param name="rpt"></param>
        /// <param name="MonthRptID"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> ReadMonthlyReportDetailExcel(string filePathName, StringBuilder error, ReportInstance rpt, Guid MonthRptID)
        {
            List<MonthlyReportDetail> list = new List<MonthlyReportDetail>();
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(filePathName);
            Worksheet sheet = book.Worksheets[0];

            if (excel.GetStringCustomProperty(sheet, "SheetName") != "MissTargetRptExcel")
            {
                error.Append("请上传正确的报表文件");
                return null;
            }

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error.Append("模板错误");
                return null;
            }

            WorksheetCollection sheets = book.Worksheets;

            //当次上报的 所有未完成公司的指标
            List<MonthlyReportDetail> missTargetList = rpt.ReportDetails;

            //循环Sheet
            for (int i = 0; i < count; i++)
            {
                //当次上报的 所有未完成公司的指标
                int startRow = 4;
                int endRow = 0;
                int startColumn = 1;
                int endColumn = 15;

                Cells cells = sheets[i].Cells;
                endRow = cells.MaxDataRow;
                endColumn = cells.MaxDataColumn;


                DataTable dt = new DataTable();
                if (endRow >= startRow) //这里判断Excel里最大行数必须大于开始读取的行数
                {
                    //读取Excel中的数据
                    dt = cells.ExportDataTableAsString(startRow, startColumn, endRow - startRow + 1, endColumn);
                }

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime PromissDate = DateTime.Now;
                        //Excel中公司的名称和指标名称==来确定给Model

                        if (dr[1].ToString() != "未完成合计") // 这里为了排除商管多加出来的合计项
                        {
                            MonthlyReportDetail data = missTargetList.Where(M => M.CompanyName == dr[1].ToString() && M.TargetName == dr[2].ToString()).FirstOrDefault();
                            if (data == null)
                            {
                                continue; //如果是不包含指标跳过
                            }

                            //if (data.IsMissTargetCurrent) // 如果是当月未完成
                            //{
                                data.MIssTargetReason = dr[11].ToString() +"\n" ;//原因
                                data.MIssTargetDescription = dr[12].ToString();//采取措施

                                data.CurrentMIssTargetReason = dr[11].ToString() + "\n";//原因
                                data.CurrentMIssTargetDescription = dr[12].ToString();//采取措施
                            //}
                            //else
                            //{
                            //    data.MIssTargetReason = dr[11].ToString();//原因
                            //    data.MIssTargetDescription = dr[12].ToString();//采取措施
                            //}
                            
                            data.ReturnDescription = dr[14].ToString();//补回情况

                            if (data.ReturnType < (int)EnumReturnType.Accomplish && data.ReturnType != 0) //如果该数据是已补回，不用填写补回时间
                            {
                                if (!string.IsNullOrEmpty(dr[13].ToString())) //承诺时间
                                {
                                    try
                                    {
                                        PromissDate = DateTime.Parse(dr[13].ToString());
                                        data.PromissDate = PromissDate;

                                        //这里直接生成文字
                                        if (data.ReturnType == 1)
                                        {
                                            data.ReturnDescription = "承诺提前至" + PromissDate.Month.ToString() + "月份补回\n";
                                            //+ data.ReturnDescription;
                                        }
                                        else if (data.ReturnType == 2 || data.ReturnType == 5)
                                        {
                                            data.ReturnDescription = "承诺" + PromissDate.Month.ToString() + "月份补回\n";
                                            //+data.ReturnDescription;
                                        }

                                        data.ReturnType_Sub = EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_Returning);
                                    }
                                    catch
                                    {
                                        error.Append("请填写正确的承诺时间");
                                        return null;
                                    }
                                }
                                else
                                {
                                    error.Append("承诺时间为必填项");
                                    return null;
                                }
                            }

                            list.Add(data);
                        }
                    }
                }

            }
            return list;
        }

        /// <summary>
        /// 累计未完成数据导入(直管模版的)
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="error"></param>
        /// <param name="rpt"></param>
        /// <param name="MonthRptID"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> ReadDirectlyMonthlyReportDetailExcel(string filePathName, StringBuilder error, ReportInstance rpt, Guid MonthRptID)
        {
            List<MonthlyReportDetail> list = new List<MonthlyReportDetail>();
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(filePathName);
            Worksheet sheet = book.Worksheets[0];

            if (excel.GetStringCustomProperty(sheet, "SheetName") != "MissTargetRptExcel")
            {
                error.Append("请上传正确的报表文件");
                return null;
            }

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error.Append("模板错误");
                return null;
            }

            WorksheetCollection sheets = book.Worksheets;

            //当次上报的 所有未完成公司的指标
            List<MonthlyReportDetail> missTargetList = rpt.ReportDetails;

            //循环Sheet
            for (int i = 0; i < count; i++)
            {
                //当次上报的 所有未完成公司的指标
                int startRow = 4;
                int endRow = 0;
                int startColumn = 1;
                int endColumn = 15;

                Cells cells = sheets[i].Cells;
                endRow = cells.MaxDataRow;
                endColumn = cells.MaxDataColumn;

                //读取Excel中的数据
                DataTable dt = cells.ExportDataTableAsString(startRow, startColumn, endRow - startRow + 1, endColumn);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime PromissDate = DateTime.Now;

                        MonthlyReportDetail data = missTargetList.Where(M => M.TargetName == dr[2].ToString()).OrderBy(p => p.CreateTime).FirstOrDefault();
                        if (data == null)
                        {
                            error.Append("请重新下载Excel后，填写上报");
                            return null;
                        }

                        //if (data.IsMissTargetCurrent) // 如果是当月未完成
                        //{
                            data.MIssTargetReason = dr[11].ToString();//原因
                            data.MIssTargetDescription = dr[12].ToString();//采取措施

                            data.CurrentMIssTargetReason = dr[11].ToString();//原因
                            data.CurrentMIssTargetDescription = dr[12].ToString();//采取措施
                        //}
                        //else
                        //{
                        //    data.MIssTargetReason = dr[11].ToString();//原因
                        //    data.MIssTargetDescription = dr[12].ToString();//采取措施
                        //}
                        
                        data.ReturnDescription = dr[14].ToString();//补回情况

                        if (data.ReturnType < (int)EnumReturnType.Accomplish && data.ReturnType != 0) //如果该数据是已补回，不用填写补回时间
                        {
                            if (!string.IsNullOrEmpty(dr[13].ToString())) //承诺时间
                            {
                                try
                                {
                                    PromissDate = DateTime.Parse(dr[13].ToString());
                                    data.PromissDate = PromissDate;
                                }
                                catch
                                {
                                    error.Append("请填写正确的承诺时间");
                                    return null;
                                }
                            }
                            else
                            {
                                error.Append("承诺时间为必填项");
                                return null;
                            }
                        }

                        list.Add(data);
                    }

                }

            }
            return list;
        }



        /// <summary>
        /// 当月未完成数据导入（直管模版的）
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="error"></param>
        /// <param name="rpt"></param>
        /// <param name="MonthRptID"></param>
        /// <returns></returns>
        private List<MonthlyReportDetail> ReadCurrentDirectlyMonthlyReportDetailExcel(string filePathName, StringBuilder error, ReportInstance rpt, Guid MonthRptID)
        {
            List<MonthlyReportDetail> list = new List<MonthlyReportDetail>();
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(filePathName);
            Worksheet sheet = book.Worksheets[0];

            if (excel.GetStringCustomProperty(sheet, "SheetName") != "MissTargetRptExcel")
            {
                error.Append("请上传正确的报表文件");
                return null;
            }

            int count = book.Worksheets.Count;// sheet个数
            if (count == 0)
            {
                error.Append("模板错误");
                return null;
            }

            WorksheetCollection sheets = book.Worksheets;

            //当次上报的 所有未完成公司的指标
            List<MonthlyReportDetail> missTargetList = rpt.ReportDetails;

            //循环Sheet
            for (int i = 0; i < count; i++)
            {
                //当次上报的 所有未完成公司的指标
                int startRow = 4;
                int endRow = 0;
                int startColumn = 1;
                int endColumn = 15;

                Cells cells = sheets[i].Cells;
                endRow = cells.MaxDataRow;
                endColumn = cells.MaxDataColumn;

                //读取Excel中的数据
                DataTable dt = cells.ExportDataTableAsString(startRow, startColumn, endRow - startRow + 1, endColumn);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime PromissDate = DateTime.Now;

                        MonthlyReportDetail data = missTargetList.Where(M => M.TargetName == dr[2].ToString()).OrderBy(p => p.CreateTime).FirstOrDefault();
                        if (data == null)
                        {
                            error.Append("请重新下载Excel后，填写上报");
                            return null;
                        }
                        
                        if (data.TargetID.ToString().ToUpper() == AppSettingConfig.GetSetting("MonthSGRent", "").ToUpper() && data.NDifference < 0 && data.IsMissTarget == false)
                        {
                            data.CurrentMIssTargetReason = dr[11].ToString();//原因
                            data.CurrentMIssTargetDescription = dr[12].ToString();//采取措施
                        }
                        else   //这有当月未完成，累计完成的情况，才会填写原因以及措施
                        {
                            data.CurrentMIssTargetReason = dr[11].ToString();//原因
                            data.CurrentMIssTargetDescription = dr[12].ToString();//采取措施
                        }
                      
                        
                        list.Add(data);
                    }

                }

            }
            return list;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        private decimal ParseCellValue(DataRow row, int column)
        {
            decimal result = 0;
            if (row[column] != null)
            {
                string cellText = row[column].ToString();
                bool b = decimal.TryParse(cellText, out result);
                if (b == false)
                {
                    result = 0;
                }
            }
            return result;
        }


        /// <summary>
        /// 保存，上报的时候序列化后的Json数据
        /// </summary>
        /// <param name="MonthReportID"></param>
        private void SaveJsonData(Guid MonthReportID)
        {
            B_MonthlyReportJsonData Update_JsonData;
            ReportInstance CurrentRpt = new ReportInstance(MonthReportID, true);

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

                TargetReportedControll trc = new TargetReportedControll();

                //这是上报页面的Json 数据

                if (!string.IsNullOrEmpty(Update_JsonData.ReportJsonData))
                {
                    ListObj = JsonHelper.Deserialize<List<DictionaryVmodel>>(Update_JsonData.ReportJsonData);

                    ListObj.ForEach(L =>
                    {
                        if (L.Name == "Misstarget")
                        {
                            L.ObjValue = trc.GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true);
                        }

                        if (L.Name == "CurrentMisstarget")
                        {
                            L.ObjValue = trc.GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true);
                        }

                    });
                }
                else
                {
                    ListObj.Add(new DictionaryVmodel("ReportInstance", CurrentRpt)); //
                    ListObj.Add(new DictionaryVmodel("MonthDetail", trc.GetTargetDetailList(CurrentRpt, "Detail", true))); // 
                    ListObj.Add(new DictionaryVmodel("Misstarget", trc.GetMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                    ListObj.Add(new DictionaryVmodel("MonthReportDescription", trc.GetMonthTRptDescription(CurrentRpt)));
                    ListObj.Add(new DictionaryVmodel("CurrentMisstarget", trc.GetCurrentMissTargetList(CurrentRpt, MonthReportID.ToString(), true)));
                }

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
    }
}