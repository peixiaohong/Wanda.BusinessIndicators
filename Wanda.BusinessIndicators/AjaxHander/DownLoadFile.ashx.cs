using Lib.Config;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.SessionState;
using LJTH.BusinessIndicators.Common.Web;


namespace LJTH.Fiscal.Budget.Web.AjaxHander
{
    /// <summary>
    /// DownLoadFile 的摘要说明
    /// </summary>
    public class DownLoadFile : IHttpHandler, IRequiresSessionState
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

            string path = HttpContext.Current.Request.QueryString["path"];
            path = HttpUtility.UrlDecode(path);
            string fileName = Path.Combine(UploadFilePath, path);

            if (System.IO.File.Exists(fileName))
            {
                string cFileName = HttpContext.Current.Request.QueryString["FileName"];
                if (string.IsNullOrEmpty(cFileName))
                {
                    cFileName = System.IO.Path.GetFileName(fileName);
                }
                System.IO.FileStream r = new System.IO.FileStream(fileName, FileMode.Open);    //文件下载实例化 
                long fslength = r.Length;
                byte[] b = new byte[(int)fslength];
                r.Read(b, 0, (int)fslength);
                r.Close();
                r.Dispose();

                //设置基本信息   

                context.Response.Buffer = false;
                context.Response.ContentType = "application/unknow";
                string UserAgent = context.Request.ServerVariables["http_user_agent"].ToLower();
                if (UserAgent.IndexOf("firefox") == -1)
                {
                    cFileName = HttpUtility.UrlEncode(cFileName, System.Text.Encoding.UTF8);
                }
                context.Response.AddHeader("Content-Disposition", "attachment;filename=" + cFileName); // HttpUtility.UrlEncode(cFileName)); // 此处文件名如果是中文在浏览器默认是筹码,应该加HttpUtility.UrlEncode(filename) 
                context.Response.BinaryWrite(b);
                context.Response.Flush();
                context.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("The file could not be found");
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