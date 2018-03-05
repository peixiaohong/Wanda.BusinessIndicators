using Lib.Log;
using Lib.Web.Json;
using Lib.Web.MVC;
using System;
using System.Web;
using System.Web.SessionState;
namespace Lib.Web.Mvc
{
    public class JxMvcHttpHandler : IHttpHandler, IRequiresSessionState
    {
        public virtual bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            WebUtility.CheckHttpContext();


            if (!CheckBeforeRequest(context))
            {
                ResponseException(new ApplicationException("无用户身份信息！请重新登录系统。"), context, GetErrorDetail);
            }
            try
            {
                ControllerHelper.ExecuteProcess(context);

            }
            catch (Exception ex)
            {
                Exception innerEx = ex.InnerException;
                if (innerEx == null)
                {
                    ResponseException(ex, context, GetErrorDetail);
                }
                else
                {

                    if (innerEx is ApplicationException
                        )
                    {
                        ResponseException(innerEx, context, GetErrorMsg);
                    }
                    else
                    {
                        ResponseException(innerEx, context, GetErrorDetail);
                    }
                }
            }
            AfterProcess(context);

        }
        private static void ResponseException(Exception ex, HttpContext context, Func<Exception, string> exToString)
        {
            LibViewModel failure = LibViewModel.CreateFailureJSONResponseViewModel(LibViewModel.GetExceptionCode(ex), exToString(ex));
            ResponseWrite(context, failure);
        }



        private static string GetErrorDetail(Exception ex)
        {
            string result = Log(ex.ToString());
            return result;
        }


        private static string GetErrorMsg(Exception ex)
        {

            string result = Log(ex.Message);
            return result;
        }

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
                    AutoLogger.Write(content, "JxMVC", LogPriority.AboveNormal, 0, System.Diagnostics.TraceEventType.Error, "JxMVC");
            }
            catch { }
            return content;
        }

        private static void ResponseWrite(HttpContext context, LibViewModel content)
        {
            HttpResponse response = context.Response;
            response.ContentType = "text";

            response.Write(JsonHelper.Serialize(content));
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }


        protected virtual void AfterProcess(HttpContext context)
        {

        }

        protected virtual bool CheckBeforeRequest(HttpContext context)
        {
            try
            {
                return true;
                string ssoUsername = HttpContext.Current.Items["WD_SSO_UserName"] != null ? HttpContext.Current.Items["WD_SSO_UserName"].ToString() : string.Empty;

                string strUserName = HttpContext.Current.User.Identity != null ? HttpContext.Current.User.Identity.Name : ssoUsername;
                // 必须是登陆的系统用户
                return !string.IsNullOrEmpty(strUserName);
            }
            catch
            {
                return false;
            }
        }




    }
}
