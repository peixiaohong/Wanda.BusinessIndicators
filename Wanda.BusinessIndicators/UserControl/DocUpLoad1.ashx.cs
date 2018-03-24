using Lib.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Common.Web;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.UserControl
{
    /// <summary>
    /// DocUpLoad1 的摘要说明
    /// </summary>
    public class DocUpLoad1 : IHttpHandler, IRequiresSessionState
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
            string BizID = context.Request["BusinessID"];
            string ValueA = context.Request["ValueA"];
            string ValueB = context.Request["ValueB"];
            string ValueC = context.Request["ValueC"];
            string ValueD = context.Request["ValueD"];
            string FileYear = context.Request["FileYear"];
            string[] actions = action.Split(new char[] { '|' });


            string filePath = "DocAttachments\\" + WebHelper.DateTimeNow.ToString("yyyy_MM") + "\\";
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
                    B_DocumentAttachments _attachment = new B_DocumentAttachments();

                    _attachment.CreatorName = PermissionHelper.GetCurrentUser;// WebHelper.GetCurrentUser().DisplayName;  //当前上传附件的人，记录下来

                    Guid ID = B_DocumentAttachmentsOperator.Instance.AddDocumentAttachments(_attachment);


                    if (ValueA != "0" && ValueA != null)
                    {
                        C_DocumentTree Model = C_DocumentTreeOperator.Instance.GetDocumentTree(Guid.Parse(ValueA));
                        _attachment.ValueAString = Model.TreeNodeName;
                        _attachment.ValueA = ValueA.ToGuid();
                    }

             
                    if (ValueB != "0" && ValueB != null)
                    {
                        C_DocumentTree Model = C_DocumentTreeOperator.Instance.GetDocumentTree(Guid.Parse(ValueB));
                        _attachment.ValueBString = Model.TreeNodeName;
                        _attachment.ValueB = ValueB.ToGuid() ;

                    }
                    if (ValueC != null && ValueC != "0")
                    {
                        C_DocumentTree Model = C_DocumentTreeOperator.Instance.GetDocumentTree(Guid.Parse(ValueC));
                        _attachment.ValueCString = Model.TreeNodeName;
                        _attachment.ValueC = ValueC.ToGuid();
                    }
                    if (ValueD != null && ValueD != "0")
                    {
                        C_DocumentTree Model = C_DocumentTreeOperator.Instance.GetDocumentTree(Guid.Parse(ValueD));
                        _attachment.ValueDString = Model.TreeNodeName;
                        _attachment.ValueD = ValueD.ToGuid();
                    }
                    if (FileYear != null && FileYear != "0")
                    {
                        _attachment.FinYear = int.Parse(FileYear);
                    }
                   
                    string filename = ID + Path.GetExtension(file.FileName);
                    _attachment.ID = ID;

                    _attachment.BusinessType = action;
                    if (actions.Count() > 1)
                    {
                        _attachment.BusinessType = actions[0];
                        if (string.IsNullOrEmpty(actions[1]))
                        {
                            _attachment.BusinessID = Guid.Empty;
                        }
                        else
                            _attachment.BusinessID = Guid.Parse(actions[1]);
                    }
                    if (_attachment.BusinessID == Guid.Empty && !string.IsNullOrEmpty(BizID))
                    {
                        _attachment.BusinessID = Guid.Parse(BizID);
                    }

                    _attachment.FileName = file.FileName;
                    _attachment.Url = filePath + filename;

                    if (file.ContentLength > 1024 * 1024)
                    {
                        _attachment.Size = (file.ContentLength / (1024 * 1024)).ToString("D2") + " M";
                    }
                    else
                    {
                        _attachment.Size = (file.ContentLength / (1024)).ToString("D2") + " KB";
                    }

                    //_attachment.SystemID = 

                    file.SaveAs(uploadpath + filename);
                    context.Response.Write(filePath + filename);
                    B_DocumentAttachmentsOperator.Instance.UpdateDocumentAttachments(_attachment);
                    string response = "{"
                        + string.Format("ID:\"{0}\",CreatorName:\"{1}\",CreateTime:\"{2}\",Url:\"{3}\",Size:\"{4}\",FileName:\"{5}\""
                        , new string[] {
                            _attachment.ID.ToString(), 
                            _attachment.CreatorName, 
                            _attachment.CreateTime.ToShortDateString(),
                            HttpUtility.UrlEncode(_attachment.Url), 
                            _attachment.Size, 
                            _attachment.FileName })
                        + "}";

                    response = Encoding.GetEncoding("GB2312").GetString(Encoding.Default.GetBytes(response));

                    context.Response.Write(response);
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