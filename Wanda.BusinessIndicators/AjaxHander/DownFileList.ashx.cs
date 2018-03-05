using Lib;
using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.BusinessIndicators.Web.AjaxHander;
using System.Web.Configuration;

namespace Wanda.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// DownFileList 的摘要说明
    /// </summary>
    public class DownFileList : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string BusinessID = HttpContext.Current.Request.QueryString["BusinessID"];
            string BusinessType = HttpContext.Current.Request.QueryString["BusinessType"];
            string IsMonth = HttpContext.Current.Request.QueryString["IsMonth"];
            string[] str = new string[1];
            List<B_Attachment> Attachment = new List<B_Attachment>();
            List<B_Attachment> AttachmentList = new List<B_Attachment>();

            if (!string.IsNullOrEmpty(BusinessID))
            {
                if (BusinessID.Contains(","))
                {
                    str = BusinessID.Split(',');
                    for (int i = 0; i < str.Count(); i++)
                    {
                        Attachment = B_AttachmentOperator.Instance.GetAttachmentList(Guid.Parse(str[i])).ToList();
                        for (int j = 0; j < Attachment.Count(); j++)
                        {
                            AttachmentList.Add(Attachment[j]);
                        }
                    }
                    if (AttachmentList.Count > 0)
                    {
                        var zip = ZipFile(AttachmentList, "批量下载", 1000, IsMonth);
                        if (zip != null)
                        {
                            DownloadBytes(zip, ".zip", "批量下载");
                        }
                    }
                }
                else
                {
                    AttachmentList = B_AttachmentOperator.Instance.GetAttachmentList(Guid.Parse(BusinessID)).ToList();
                    if (AttachmentList.Count > 0)
                    {
                        var zip = ZipFile(AttachmentList, "批量下载", 1000, IsMonth);
                        if (zip != null)
                        {
                            DownloadBytes(zip, ".zip", "批量下载");
                        }
                    }
                }


            }
        }
        public byte[] ZipFile(List<B_Attachment> atts, string zipedFile, int blockSize, string IsMonth)
        {
            C_System Sys = new C_System();
            
            try
            {
                //文件临时存储地址
                string tmpFilePath = System.IO.Path.GetTempPath() + "\\" + Guid.NewGuid().ToString();
                //压缩包地址
                string tmpZipFilePath = tmpFilePath + "\\" + zipedFile + ".zip";
                using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(Encoding.GetEncoding("GB2312")))
                {
                    foreach (var itemKey in atts)
                    {
                        //根据参数判断月报流程查询和指标流程查询
                        string path = "";
                        if (IsMonth == "1")
                        {
                            B_MonthlyReport month = B_MonthlyreportOperator.Instance.GetMonthlyreport(itemKey.BusinessID);
                            Sys = C_SystemOperator.Instance.GetSystem(month.SystemID);
                        }
                        else if (IsMonth == "2")
                        {
                            B_TargetPlan target = B_TargetplanOperator.Instance.GetTargetPlanByID(itemKey.BusinessID);
                            Sys = C_SystemOperator.Instance.GetSystem(target.SystemID);
                        }
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
                        //有相同名称时，将文件流更新到文件夹里
                        zipFile.UpdateDirectory(newPath, folderName);
                        //从附件表中取到文件存放路径
                        string fileName = itemKey.FileName;
                        //生成存放文件的路径
                        string filePath = Path.Combine(newPath, fileName);
                        var urlRonte = WebConfigurationManager.AppSettings["UploadFilePath"];
                        string url = Path.Combine(urlRonte, itemKey.Url);
                        //读取文件流
                        FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read);
                        try
                        {
                            byte[] bte = new byte[fs.Length];
                            //将FileStream转换成byte流
                            fs.Read(bte, 0, (int)fs.Length);
                            File.WriteAllBytes(filePath, bte);
                            zipFile.AddFile(filePath, folderName);
                            files.Add(fileName);
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            if (fs != null)
                            {
                                //关闭资源
                                fs.Close();
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

        private void DownloadBytes(byte[] bytes, string fileExt, string fileName = "NoTitle")
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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}