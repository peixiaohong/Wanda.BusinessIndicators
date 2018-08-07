using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using Plugin.OAMessage;

namespace Plugin.Filters
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = new HttpResponseMessage();
            if (actionExecutedContext.Exception.GetType()==typeof(Plugin.OAMessage.OAMessageException))
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(commonResult.OAError(actionExecutedContext.Exception));
                actionExecutedContext.Response.Content = new StringContent(json);
            }
            else
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(commonResult.Excepiton(actionExecutedContext.Exception));
                actionExecutedContext.Response.Content = new StringContent(json);
            }
            StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream);
            string request = HttpUtility.UrlDecode(reader.ReadToEnd());
            var ex = Newtonsoft.Json.JsonConvert.SerializeObject(actionExecutedContext.Exception);
            logger.Error("【ExceptionFilter】【输入】" + request + "【异常】" + ex);

        }
    }
}