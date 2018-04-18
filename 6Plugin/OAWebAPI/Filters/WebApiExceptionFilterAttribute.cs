using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            logger.Warn("【请求】"+actionExecutedContext.ActionContext.Request.ToString(), actionExecutedContext.Exception);

        }
    }
}