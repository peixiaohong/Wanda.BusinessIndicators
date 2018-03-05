using Lib.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Common.Web;

namespace Wanda.BusinessIndicators.Web.Control
{
    /// <summary>
    /// UpLoad 的摘要说明
    /// </summary>
    public class UpLoad : IHttpHandler, IRequiresSessionState
    {

        private string _uploadFilePath = null;

        private string UploadFilePath
        {
            get
            {
                if (_uploadFilePath == null) { _uploadFilePath = ConfigurationManager.AppSettings["UploadFilePath"]; }

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

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpPostedFile file = context.Request.Files["FileData"];
            string action = context.Request["action"];
            string[] actions = action.Split(new char[] { '|' });


            string filePath = "Attachments\\" + WebHelper.DateTimeNow.ToString("yyyy_MM") + "\\";
            //string uploadpath = context.Server.MapPath(filePath);
            string uploadpath = Path.Combine(UploadFilePath, filePath);
            if (file != null)
            {
                try
                {
                    if (!Directory.Exists(uploadpath))
                    {
                        Directory.CreateDirectory(uploadpath);
                    }
                    //Attachment _attachment = new Attachment();
                    //_attachment.CreatorName = WebHelper.GetCurrentUser().DisplayName;
                    //string ID = AttachmentOperator.Instance.AddAttachment(_attachment);
                    //string filename = ID + Path.GetExtension(file.FileName);
                    string filename = Path.GetExtension(file.FileName);

                    //_attachment.ID = ID;

                    //_attachment.BusinessType = action;
                    //if (actions.Count() > 1)
                    //{
                    //    _attachment.BusinessType = actions[0];
                    //    _attachment.BusinessID = actions[1];
                    //}
                    //_attachment.FileName = file.FileName;
                    //_attachment.Url = filePath + filename;

                    //if (file.ContentLength > 1024 * 1024)
                    //{
                    //    _attachment.Size = (file.ContentLength / (1024 * 1024)).ToString("D2") + " M";
                    //}
                    //else
                    //{
                    //    _attachment.Size = (file.ContentLength / (1024)).ToString("D2") + " KB";
                    //}
                    file.SaveAs(uploadpath + filename);
                    context.Response.Write(filePath + filename);
                    //AttachmentOperator.Instance.UpdateAttachment(_attachment);
                    //string response = "{"
                    //    + string.Format("ID:\"{0}\",CreatorName:\"{1}\",CreateTime:\"{2}\",Url:\"{3}\",Size:\"{4}\",FileName:\"{5}\""
                    //    , new string[] {
                    //        _attachment.ID, 
                    //        _attachment.CreatorName, 
                    //        _attachment.CreateTime.ToShortDateString(),
                    //        HttpUtility.UrlEncode(_attachment.Url), 
                    //        _attachment.Size, 
                    //        _attachment.FileName })
                    //    + "}";

                    //string response ="{" + string.Format("ID:\"{0}\",CreatorName:\"{1}\",CreateTime:\"{2}\",Url:\"{3}\",Size:\"{4}\",FileName:\"{5}\"",new string[]{} +"}"

                    //response = Encoding.GetEncoding("GB2312").GetString(Encoding.Default.GetBytes(response));

                    //context.Response.Write(response);
                }
                catch (Exception exp)
                {
                    context.Response.Write("Faild:" + exp.Message);
                }
            }
            else
            {
                context.Response.Write("Faild:Empty file.");
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