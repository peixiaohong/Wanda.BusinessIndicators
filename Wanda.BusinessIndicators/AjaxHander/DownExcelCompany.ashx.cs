
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
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Common.Web;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using Lib.Xml;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using System.Data;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownExcelCompany 的摘要说明
    /// </summary>
    public class DownExcelCompany : IHttpHandler, IRequiresSessionState
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
        List<XElement> SysXML;
        string FileType = string.Empty; //下载的文件类型
        string fileName = "公司属性管理";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["SysId"]))
            {
                SysId = Guid.Parse(HttpContext.Current.Request["SysId"]);
            }
            DownExcel();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private void DownExcel()
        {
            string PropertyName = "";
            string PropertyValue = "";
            List<string> PName = null;
            List<string> PValue = null;
            C_System model = C_SystemOperator.Instance.GetSystem(SysId);
            XElement xelement = model.Configuration;
            if (xelement != null)
            {
                SysXML = xelement.Elements("ListCompanyProperty").Elements("CompanyProperty").ToList();
            }
            for (int i = 0; i < SysXML.Count; i++)
            {
                PropertyName += SysXML[i].GetAttributeValue("PropertyName", "") + ",";
                PropertyValue += SysXML[i].GetAttributeValue("ColumnName", "") + ",";
            }
            if (PropertyName != "" && PropertyValue != "")//检测公司是否
            {
                PName = PropertyName.TrimEnd(',').Split(',').ToList();
                PValue = PropertyValue.TrimEnd(',').Split(',').ToList();
            }
            List<C_Company> CompanyList = C_CompanyOperator.Instance.GetCompanyList(SysId).ToList();
            CompanyList.RemoveAll(R => R.Sequence <= 0);//过滤掉总部以及其他不需要显示的公司名称  这些名称的Sequence均小于等于0
            CompanyVModel CompanyModel = null;
            List<CompanyVModel> CompanyModelList = new List<CompanyVModel>();



            ExcelEngine excel = new ExcelEngine();
            WorkbookDesigner designer = new WorkbookDesigner();
            string path = System.IO.Path.Combine(ExcelTempletePath, "公司属性管理V1.xls");

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            designer.Workbook = new Workbook(fileStream);
            WorksheetCollection worksheets = designer.Workbook.Worksheets;


            int s = 1;
            if (CompanyList != null)
            {
                foreach (C_Company item in CompanyList)
                {
                    CompanyModel = new CompanyVModel();
                    CompanyModel.ID = item.ID.ToString();
                    CompanyModel.CompanyName = item.CompanyName;
                    CompanyModel.CompanyProperty1 = item.CompanyProperty1;
                    CompanyModel.CompanyProperty2 = item.CompanyProperty2;
                    CompanyModel.CompanyProperty3 = item.CompanyProperty3;
                    CompanyModel.CompanyProperty4 = item.CompanyProperty4;
                    CompanyModel.CompanyProperty5 = item.CompanyProperty5;
                    CompanyModel.CompanyProperty6 = item.CompanyProperty6;
                    CompanyModel.CompanyProperty7 = item.CompanyProperty7;
                    CompanyModel.CompanyProperty8 = item.CompanyProperty8;
                    CompanyModel.CompanyProperty9 = item.CompanyProperty9;
                    if (item.OpeningTime != DateTime.MinValue)
                    {
                        CompanyModel.OpeningTime = item.OpeningTime.ToShortDateString().ToString();
                    }
                    else
                    {
                        CompanyModel.OpeningTime = "--";
                    }
                    CompanyModel.Sequence = item.Sequence;
                    CompanyModel.Index = s;
                    s++;
                    CompanyModelList.Add(CompanyModel);
                }
            }

            #region  从数据库中取出该系统的CompanyProperty,并将其他的CompanyProperty隐藏
            //设置表头  
            List<string> PVValue = new List<string>() { "CompanyProperty1", "CompanyProperty2", "CompanyProperty3", "CompanyProperty4", "CompanyProperty5", "CompanyProperty6", "CompanyProperty7", "CompanyProperty8", "CompanyProperty9" };
            if (PValue != null)
            {
                for (int a = 0; a < PValue.Count; a++)
                {
                    PVValue.Remove(PValue[a]);
                }
                for (int b = 0; b < PValue.Count; b++)
                {
                    if (PValue[b] == "CompanyProperty1")
                    {
                        if (model.Category==2)
                        {
                             worksheets[0].Cells[2, 3].PutValue("是否尾盘");
                        }
                        else
                        {
                            worksheets[0].Cells[2, 3].PutValue(PName[b]);
                        }
                     

                    }
                    else if (PValue[b] == "CompanyProperty2")
                    {
                        worksheets[0].Cells[2, 4].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty3")
                    {
                        worksheets[0].Cells[2, 5].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty4")
                    {
                        worksheets[0].Cells[2, 6].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty5")
                    {
                        worksheets[0].Cells[2, 7].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty6")
                    {
                        worksheets[0].Cells[2, 8].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty7")
                    {
                        worksheets[0].Cells[2, 9].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty8")
                    {
                        worksheets[0].Cells[2, 10].PutValue(PName[b]);

                    }
                    else if (PValue[b] == "CompanyProperty9")
                    {
                        worksheets[0].Cells[2, 11].PutValue(PName[b]);

                    }
                }
            }
            if (model.Category == 2)
            {
                worksheets[0].Cells[2, 4].PutValue("小计");
            }
            for (int i = 0; i < PVValue.Count; i++)
            {
                if (PVValue[i] == "CompanyProperty1")
                {
                    worksheets[0].Cells.HideColumn(3);
                }
                else if (PVValue[i] == "CompanyProperty2")
                {
                    if (model.Category != 2)
                    {
                        worksheets[0].Cells.HideColumn(4);
                    }
                }
                else if (PVValue[i] == "CompanyProperty3")
                {
                    worksheets[0].Cells.HideColumn(5);
                }
                else if (PVValue[i] == "CompanyProperty4")
                {
                    worksheets[0].Cells.HideColumn(6);
                }
                else if (PVValue[i] == "CompanyProperty5")
                {
                    worksheets[0].Cells.HideColumn(7);
                }
                else if (PVValue[i] == "CompanyProperty6")
                {
                    worksheets[0].Cells.HideColumn(8);
                }
                else if (PVValue[i] == "CompanyProperty7")
                {
                    worksheets[0].Cells.HideColumn(9);
                }
                else if (PVValue[i] == "CompanyProperty8")
                {
                    worksheets[0].Cells.HideColumn(10);
                }
                else if (PVValue[i] == "CompanyProperty9")
                {
                    worksheets[0].Cells.HideColumn(11);
                }
            }
            #endregion

            designer.SetDataSource("Title", C_SystemOperator.Instance.GetSystem(SysId).SystemName + fileName);
            designer.SetDataSource("Date", "修改日期:" + DateTime.Now.ToLongDateString().ToString());
            designer.SetDataSource("CompanyList", CompanyModelList);
            excel.SetCustomProperty(worksheets[0], "SystemID", SysId.ToString());


            designer.Process();
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
    }
}