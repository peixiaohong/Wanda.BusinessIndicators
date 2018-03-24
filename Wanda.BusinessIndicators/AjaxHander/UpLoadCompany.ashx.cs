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

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// UpLoadCompany 的摘要说明
    /// </summary>
    public class UpLoadCompany : IHttpHandler
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
        C_System SystemModel = new C_System();
        C_Company CompanyModel = new C_Company();
        List<C_Company> CompanyList = new List<C_Company>();
        List<C_Company> AddCompanyList = new List<C_Company>();

        List<string> CompanyProperty1 = new List<string>();
        List<string> CompanyProperty2 = new List<string>();
        List<string> CompanyProperty3 = new List<string>();
        List<string> CompanyProperty4 = new List<string>();
        List<string> CompanyProperty5 = new List<string>();
        List<string> CompanyProperty6 = new List<string>();
        List<string> CompanyProperty7 = new List<string>();
        List<string> CompanyProperty8 = new List<string>();
        List<string> CompanyProperty9 = new List<string>();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentType = "text/plain";
            HttpPostedFile file = context.Request.Files["FileData"];
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysId"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SysId"]);
            }
            SystemModel = C_SystemOperator.Instance.GetSystem(SysId);
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
            UpLoadExcel(out  error, context, filePathName);
            context.Response.Write(error);
        }
        public void UpLoadExcel(out string error, HttpContext context, string filePathName)
        {
            string templetePath = filePathName;
            error = "保存成功";
            ExcelEngine excel = new ExcelEngine();
            Workbook book = new Workbook(templetePath);
            CompanyList = C_CompanyOperator.Instance.GetCompanyList(SysId).ToList();

            //获取该系统中公司的所有属性
            List<XElement> xelement = C_SystemOperator.Instance.GetSystem(SysId).Configuration.Elements("ListCompanyProperty").Elements("CompanyProperty").ToList();
            if (xelement.Count > 0)
            {
                foreach (XElement item in xelement)
                {
                    string ColumnName = (string)item.Attribute("ColumnName");
                    if (ColumnName == "CompanyProperty1")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty1.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty2")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty2.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty3")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty3.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty4")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty4.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty5")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty5.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty6")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty6.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty7")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty7.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty8")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty8.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                    else if (ColumnName == "CompanyProperty9")
                    {
                        List<XElement> CompanyPropertyList = item.Elements("ItemProperty").ToList();
                        foreach (XElement itom in CompanyPropertyList)
                        {
                            CompanyProperty9.Add((string)itom.Attribute("ItemPropertyValue"));
                        }
                    }
                }
            }

            int count = book.Worksheets.Count;
            for (int i = 0; i < count; i++)
            {
                if (excel.GetStringCustomProperty(book.Worksheets[i], "SystemID") != SysId.ToString())
                {
                    error = "请上传当前系统的公司列表！";
                    return;
                }
                Worksheet workSheet = book.Worksheets[i];
                Cells cells = workSheet.Cells;

                if (cells.MaxDataRow - 2 > 0 && cells.MaxDataColumn == 14)
                {
                    DataTable dt = cells.ExportDataTableAsString(3, 1, cells.MaxDataRow - 2, cells.MaxDataColumn);
                    {

                        foreach (DataRow item in dt.Rows)
                        {

                            if (item[13].ToString() != "" && item[13].ToString() != null)
                            {

                                CompanyModel = C_CompanyOperator.Instance.GetCompany(item[13].ToString().ToGuid());
                                try//获取时间
                                {
                                    if (item[11].ToString().Replace(" ", "") == "--" || item[11].ToString().Replace(" ", "") == "")
                                    {
                                        CompanyModel.OpeningTime = DateTime.MinValue;
                                    }
                                    else
                                    {
                                        DateTime time = DateTime.Parse(item[11].ToString().Replace(" ", ""));
                                        CompanyModel.OpeningTime = time;
                                    }

                                }
                                catch (Exception)
                                {
                                    error = CompanyModel.CompanyName + "的开店时间格式填写错误!";
                                    return;
                                }
                                if (item[1].ToString().Replace(" ", "") == null || item[1].ToString().Replace(" ", "") == "")
                                {
                                    error = "公司名称不能为空!";
                                    return;
                                }
                                else
                                {
                                    CompanyModel.CompanyName = item[1].ToString().Replace(" ", "");
                                }
                                if (CompanyProperty1.Count == 0)
                                {
                                    CompanyModel.CompanyProperty1 = null;
                                }
                                else
                                {
                                    if (item[2].ToString().Replace(" ", "") != null && item[2].ToString().Replace(" ", "") != "")
                                    {
                                        if (SystemModel.Category != 2)
                                        {


                                            if (CompanyProperty1.Contains(item[2].ToString().Replace(" ", "")))
                                            {
                                                CompanyModel.CompanyProperty1 = item[2].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                error = "公司属性填写错误,请重新填写";
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (item[2].ToString().Replace(" ", "") == "尾盘")
                                            {
                                                CompanyModel.CompanyProperty1 = "尾盘";
                                            }
                                            else
                                            {
                                                CompanyModel.CompanyProperty1 = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty1 = null;
                                    }

                                }
                                //CompanyProperty2
                                if (SystemModel.Category!=2)
                                {
                                    if (CompanyProperty2.Count == 0)
                                    {
                                        CompanyModel.CompanyProperty2 = null;
                                    }
                                    else
                                    {
                                        if (item[3].ToString().Replace(" ", "") != null && item[3].ToString().Replace(" ", "") != "")
                                        {
                                            if (CompanyProperty2.Contains(item[3].ToString().Replace(" ", "")))
                                            {
                                                CompanyModel.CompanyProperty2 = item[3].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                error = "公司属性填写错误,请重新填写";
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            CompanyModel.CompanyProperty2 = null;
                                        }
                                    }
                                }
                                else
                                {
                                    if (item[3].ToString().Replace(" ", "") != null && item[3].ToString().Replace(" ", "") != "")
                                    {
                                            CompanyModel.CompanyProperty2 = item[3].ToString().Replace(" ", "");
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty2 = null;
                                    }
                                }
                               

                                //CompanyProperty3
                                if (CompanyProperty3.Count == 0)
                                {
                                    CompanyModel.CompanyProperty3 = null;
                                }
                                else
                                {
                                    if (item[4].ToString().Replace(" ", "") != null && item[4].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty3.Contains(item[4].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty3 = item[4].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty3 = null;
                                    }

                                }
                                //CompanyProperty4
                                if (CompanyProperty4.Count == 0)
                                {
                                    CompanyModel.CompanyProperty4 = null;
                                }
                                else
                                {
                                    if (item[5].ToString().Replace(" ", "") != null && item[5].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty4.Contains(item[5].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty4 = item[5].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty4 = null;
                                    }
                                }
                                //CompanyProperty5
                                if (CompanyProperty5.Count == 0)
                                {
                                    CompanyModel.CompanyProperty5 = null;
                                }
                                else
                                {
                                    if (item[6].ToString().Replace(" ", "") != null && item[6].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty5.Contains(item[6].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty5 = item[6].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty5 = null;
                                    }

                                }
                                //CompanyProperty6
                                if (CompanyProperty6.Count == 0)
                                {
                                    CompanyModel.CompanyProperty6 = null;
                                }
                                else
                                {
                                    if (item[7].ToString().Replace(" ", "") != null && item[7].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty6.Contains(item[7].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty6 = item[7].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty6 = null;
                                    }

                                }
                                //CompanyProperty7
                                if (CompanyProperty7.Count == 0)
                                {
                                    CompanyModel.CompanyProperty7 = null;
                                }
                                else
                                {
                                    if (item[8].ToString().Replace(" ", "") != null && item[8].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty7.Contains(item[8].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty7 = item[8].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty7 = null;
                                    }

                                }
                                //CompanyProperty8
                                if (CompanyProperty8.Count == 0)
                                {
                                    CompanyModel.CompanyProperty8 = null;
                                }
                                else
                                {
                                    if (item[9].ToString().Replace(" ", "") != null && item[9].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty8.Contains(item[9].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty8 = item[9].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty8 = null;
                                    }

                                }
                                //CompanyProperty9
                                if (CompanyProperty9.Count == 0)
                                {
                                    CompanyModel.CompanyProperty9 = null;
                                }
                                else
                                {
                                    if (item[10].ToString().Replace(" ", "") != null && item[10].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty9.Contains(item[10].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty9 = item[10].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty9 = null;
                                    }

                                }
                                if (item[12].ToString().Replace(" ", "") == null || item[12].ToString().Replace(" ", "") == "")
                                {
                                    error = CompanyModel.CompanyName + "的排序值不能为空!";
                                }
                                else
                                {
                                    CompanyModel.Sequence = int.Parse(item[12].ToString().Replace(" ", ""));
                                }
                                CompanyList.Add(CompanyModel);
                            }
                            else
                            {
                                CompanyModel = new C_Company();
                                CompanyModel.SystemID = SysId;
                                try//获取时间
                                {
                                    if (item[11].ToString().Replace(" ", "") == "--" || item[11].ToString().Replace(" ", "") == "")
                                    {
                                        CompanyModel.OpeningTime = DateTime.MinValue;
                                    }
                                    else
                                    {
                                        DateTime time = DateTime.Parse(item[11].ToString().Replace(" ", ""));
                                        CompanyModel.OpeningTime = time;
                                    }
                                    ;
                                }
                                catch (Exception)
                                {
                                    error = CompanyModel.CompanyName + "的开店时间格式填写错误!";
                                    return;
                                }
                                if (item[1].ToString().Replace(" ", "") == null || item[1].ToString().Replace(" ", "") == "")
                                {
                                    error = "公司名称不能为空!";
                                    return;
                                }
                                else
                                {
                                    CompanyModel.CompanyName = item[1].ToString().Replace(" ", "");
                                }
                                //CompanyProperty1
                                if (CompanyProperty1.Count == 0)
                                {
                                    CompanyModel.CompanyProperty1 = null;
                                }
                                else
                                {
                                    if (item[2].ToString().Replace(" ", "") != null && item[2].ToString().Replace(" ", "") != "")
                                    {
                                        if (SystemModel.Category != 2)
                                        {


                                            if (CompanyProperty1.Contains(item[2].ToString().Replace(" ", "")))
                                            {
                                                CompanyModel.CompanyProperty1 = item[2].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                error = "公司属性填写错误,请重新填写";
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (item[2].ToString().Replace(" ", "") == "尾盘")
                                            {
                                                CompanyModel.CompanyProperty1 = "尾盘";
                                            }
                                            else
                                            {
                                                CompanyModel.CompanyProperty1 = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty1 = null;
                                    }

                                }

                                //CompanyProperty2
                                if (SystemModel.Category != 2)
                                {
                                    if (CompanyProperty2.Count == 0)
                                    {
                                        CompanyModel.CompanyProperty2 = null;
                                    }
                                    else
                                    {
                                        if (item[3].ToString().Replace(" ", "") != null && item[3].ToString().Replace(" ", "") != "")
                                        {
                                            if (CompanyProperty2.Contains(item[3].ToString().Replace(" ", "")))
                                            {
                                                CompanyModel.CompanyProperty2 = item[3].ToString().Replace(" ", "");
                                            }
                                            else
                                            {
                                                error = "公司属性填写错误,请重新填写";
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            CompanyModel.CompanyProperty2 = null;
                                        }
                                    }
                                }
                                else
                                {
                                    if (item[3].ToString().Replace(" ", "") != null && item[3].ToString().Replace(" ", "") != "")
                                    {
                                        CompanyModel.CompanyProperty2 = item[3].ToString().Replace(" ", "");
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty2 = null;
                                    }
                                }
                               

                                //CompanyProperty3
                                if (CompanyProperty3.Count == 0)
                                {
                                    CompanyModel.CompanyProperty3 = null;
                                }
                                else
                                {
                                    if (item[4].ToString().Replace(" ", "") != null && item[4].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty3.Contains(item[4].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty3 = item[4].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty3 = null;
                                    }

                                }
                                //CompanyProperty4
                                if (CompanyProperty4.Count == 0)
                                {
                                    CompanyModel.CompanyProperty4 = null;
                                }
                                else
                                {
                                    if (item[5].ToString().Replace(" ", "") != null && item[5].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty4.Contains(item[5].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty4 = item[5].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty4 = null;
                                    }
                                }
                                //CompanyProperty5
                                if (CompanyProperty5.Count == 0)
                                {
                                    CompanyModel.CompanyProperty5 = null;
                                }
                                else
                                {
                                    if (item[6].ToString().Replace(" ", "") != null && item[6].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty5.Contains(item[6].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty5 = item[6].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty5 = null;
                                    }

                                }
                                //CompanyProperty6
                                if (CompanyProperty6.Count == 0)
                                {
                                    CompanyModel.CompanyProperty6 = null;
                                }
                                else
                                {
                                    if (item[7].ToString().Replace(" ", "") != null && item[7].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty6.Contains(item[7].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty6 = item[7].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty6 = null;
                                    }

                                }
                                //CompanyProperty7
                                if (CompanyProperty7.Count == 0)
                                {
                                    CompanyModel.CompanyProperty7 = null;
                                }
                                else
                                {
                                    if (item[8].ToString().Replace(" ", "") != null && item[8].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty7.Contains(item[8].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty7 = item[8].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty7 = null;
                                    }

                                }
                                //CompanyProperty8
                                if (CompanyProperty8.Count == 0)
                                {
                                    CompanyModel.CompanyProperty8 = null;
                                }
                                else
                                {
                                    if (item[9].ToString().Replace(" ", "") != null && item[9].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty8.Contains(item[9].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty8 = item[9].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty8 = null;
                                    }
                                }
                                //CompanyProperty9
                                if (CompanyProperty9.Count == 0)
                                {
                                    CompanyModel.CompanyProperty9 = null;
                                }
                                else
                                {
                                    if (item[10].ToString().Replace(" ", "") != null && item[10].ToString().Replace(" ", "") != "")
                                    {
                                        if (CompanyProperty9.Contains(item[10].ToString().Replace(" ", "")))
                                        {
                                            CompanyModel.CompanyProperty9 = item[10].ToString().Replace(" ", "");
                                        }
                                        else
                                        {
                                            error = "公司属性填写错误,请重新填写";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        CompanyModel.CompanyProperty9 = null;
                                    }
                                }
                                if (item[12].ToString().Replace(" ", "") == null || item[12].ToString().Replace(" ", "") == "")
                                {
                                    error = CompanyModel.CompanyName + "的排序值不能为空!";
                                }
                                else
                                {
                                    CompanyModel.Sequence = int.Parse(item[12].ToString().Replace(" ", ""));
                                }
                                AddCompanyList.Add(CompanyModel);
                            }


                        }
                    }
                }
            }
            C_CompanyOperator.Instance.UpdateCompanyList(CompanyList);
            if (AddCompanyList.Count > 0)
            {
                C_CompanyOperator.Instance.AddCompanyList(AddCompanyList);
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