using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Plugin.OAMessage;

namespace Plugin.Filters
{
    public class WebApiActionFilter:ActionFilterAttribute
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        //{
        //    return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        //}
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null)
            {
                StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream);
                string request = HttpUtility.UrlDecode(reader.ReadToEnd());
                //StreamReader reader2 = new StreamReader(HttpContext.Current.Response.OutputStream);
                string response = GetResponseValues(actionExecutedContext);
                logger.Info("【ActionFilter】【输入】" + request + "【输出】" + response);
                base.OnActionExecuted(actionExecutedContext);
            }
            else
            {
                actionExecutedContext.Response = new HttpResponseMessage();
                if (actionExecutedContext.Exception.GetType() == typeof(Plugin.OAMessage.OAMessageException))
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
                logger.Error("【ActionFilter】【输入】" + request + "【异常】" + ex);
            }
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
        }
        /// <summary>
        /// 读取request 的提交内容
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        public string GetRequestValues(HttpActionExecutedContext actionExecutedContext)
        {

            Stream stream = actionExecutedContext.Request.Content.ReadAsStreamAsync().Result;
            Encoding encoding = Encoding.UTF8;
            /*
                这个StreamReader不能关闭，也不能dispose， 关了就傻逼了
                因为你关掉后，后面的管道  或拦截器就没办法读取了
            */
            var reader = new StreamReader(stream, encoding);
            string result = reader.ReadToEnd();
            /*
            这里也要注意：   stream.Position = 0;
            当你读取完之后必须把stream的位置设为开始
            因为request和response读取完以后Position到最后一个位置，交给下一个方法处理的时候就会读不到内容了。
            */
            stream.Position = 0;
            return result;
        }
        /// <summary>
        /// 读取action返回的result
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        public string GetResponseValues(HttpActionExecutedContext actionExecutedContext)
        {
            Stream stream = actionExecutedContext.Response.Content.ReadAsStreamAsync().Result;
            Encoding encoding = Encoding.UTF8;
            /*
            这个StreamReader不能关闭，也不能dispose， 关了就傻逼了
            因为你关掉后，后面的管道  或拦截器就没办法读取了
            */
            var reader = new StreamReader(stream, encoding);
            string result = reader.ReadToEnd();
            /*
            这里也要注意：   stream.Position = 0; 
            当你读取完之后必须把stream的位置设为开始
            因为request和response读取完以后Position到最后一个位置，交给下一个方法处理的时候就会读不到内容了。
            */
            stream.Position = 0;
            return result;
        }
    }
}