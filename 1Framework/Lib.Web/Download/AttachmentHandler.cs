using Lib.Config;
using Lib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Threading;
using Lib.Web;
namespace Lib.Web.Download
{
    public class AttachmentHandler : IHttpHandler, IRequiresSessionState
    {
        public static object sync_lick = new object();
        public virtual bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {

            CheckBeforeDownload(context);

            try
            {
                DownloadFile(context);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                context.Response.Write("下载文件出错：" + ex.ToString());
            }
        }

        private void DownloadFile(HttpContext context)
        {
            //if (Monitor.TryEnter(sync_lick, 3000) == false)
            //{
            //    context.Response.ContentType = "text/plain";
            //    context.Response.Write("文件正在上传中请稍后");
            //    return;
            //}
            //try
            //{
            try
            {
                string filePath = WebUtility.GetRequestQueryString("filePath", "notexist");
                string fileName = WebUtility.GetRequestQueryString("fileName", "notexist");


                string uploadFilePath = "";

                uploadFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["FilePath"];

                string path = filePath;
                path = HttpUtility.UrlDecode(path);

                string FileName = Path.Combine(uploadFilePath, path);
                FileName = HttpUtility.UrlDecode(FileName);
                using (FileConnect fileConnect = new FileConnect())
                {
                    if (System.IO.File.Exists(FileName))
                    {
                        #region
                        string cFileName = HttpUtility.UrlDecode(fileName);
                        if (string.IsNullOrEmpty(cFileName))
                        {
                            cFileName = System.IO.Path.GetFileName(FileName);
                        }

                        System.IO.FileStream r =File.Open(FileName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);    //文件下载实例化 
                        //设置基本信息   

                        context.Response.Buffer = false;
                        context.Response.AddHeader("Connection", "Keep-Alive");
                        context.Response.ContentType = "application/octet-stream";
                        context.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(cFileName)); // 此处文件名如果是中文在浏览器默认是筹码,应该加HttpUtility.UrlEncode(filename) 
                        context.Response.AddHeader("Content-Length", r.Length.ToString());
                        while (true)    //如果文件大于缓冲区，通过while循环多次加载文件 
                        {
                            #region
                            //开辟缓冲区空间   
                            byte[] buffer = new byte[1024];  //读取文件的数据   
                            int leng = r.Read(buffer, 0, 1024);
                            if (leng == 0)             //到文件尾，结束   
                                break;
                            if (leng == 1024)            //读出的文件数据长度等于缓冲区长度，直接将缓冲区数据写入  
                                context.Response.BinaryWrite(buffer);           //向客户端发送数据流 
                            else
                            {
                                //读出文件数据比缓冲区小，重新定义缓冲区大小，只用于读取文件的最后一个数据块  
                                byte[] b = new byte[leng]; for (int i = 0; i < leng; i++)
                                    b[i] = buffer[i];
                                context.Response.BinaryWrite(b);
                            }
                            #endregion
                        }
                        fileConnect.DisConnect();
                        context.Response.End();//结束文件下载 
                        #endregion
                    }
                    else
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("没找到该文件");
                    }
                    fileConnect.DisConnect();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                context.Response.ContentType = "text/plain";
                context.Response.Write("文件正在上传中请稍后");
            }

            //}
            //catch (Exception ex)
            //{
            //    context.Response.ContentType = "text/plain";
            //    context.Response.Write(ex.Message);

            //}
            //finally
            //{
            //    Monitor.Exit(sync_lick);
            //}
        }

        private bool CheckBeforeDownload(System.Web.HttpContext context)
        {
            string userName = "";
            bool result = true;
            try
            {
                userName = context.User.Identity.Name;
            }
            catch
            {
                result = false;
            }

            string content = string.Format("RequestUrl:{0}\r\nIP:{1}\r\nUser:{2}",
                context.Request.Url.ToString(),
                WebUtility.GetClientIP(),
                userName);

            Log(content);
            return result;
        }



        public string FileName { get; set; }

        private static Logger _autoLogger = null;
        public static Logger AutoLogger
        {
            get
            {
                if (_autoLogger == null)
                {
                    _autoLogger = LoggerFactory.Create("AutoLog");
                }
                return _autoLogger;
            }
        }

        private static string Log(string content)
        {

            try
            {
                if (AutoLogger != null)
                    AutoLogger.Write(content, "ExcelCategoryHandler", LogPriority.AboveNormal, 0, System.Diagnostics.TraceEventType.Error, "ExcelCategoryHandler");
            }
            catch { }
            return content;
        }
    }

}
