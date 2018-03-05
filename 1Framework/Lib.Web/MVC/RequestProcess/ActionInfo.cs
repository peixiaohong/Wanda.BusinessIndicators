using Lib.Core;
using Lib.Web.Mvc;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Lib.Web.MVC
{
    internal class ActionInfo
    {
        private MethodInfo methodInfo;
        private HttpContext context;
        private BaseController baseController;

        private LibViewModelType ActionResultType { get; set; }

        private Action<NameValueCollection> InternalArgsHandler = null;


        public ActionInfo(HttpContext context, BaseController baseController, MethodInfo methodInfo)
        {
            this.context = context;
            this.baseController = baseController;
            this.methodInfo = methodInfo;

            InternalArgsHandler = new Action<NameValueCollection>(ClientInfoRegister);
        }

        /// <summary>
        /// 登记客户端信息, 包括浏览器信息和操作系统信息
        /// </summary>
        /// <param name="obj"></param>
        private void ClientInfoRegister(NameValueCollection obj)
        {
            string[] args = { "__1osInfo", "__2browserInfo" };
            foreach (var item in args)
            {
                HttpContext.Current.Items[item] = obj[item];
            }
        }

        internal object Invoke()
        {

            object[] arguments = PrepareActionParams(context, methodInfo);

            object result = methodInfo.Invoke(baseController, arguments);
            return result;
        }



        private object[] PrepareActionParams(HttpContext context, MethodInfo mi)
        {
            HttpRequest request = context.Request;
            ParameterInfo[] parameters = mi.GetParameters();
            object[] paramValues = new object[parameters.Length];

            NameValueCollection requestParams = new NameValueCollection(string.Compare(request.RequestType, "get", true) == 0 ? request.QueryString : request.Form);

            if (InternalArgsHandler != null) InternalArgsHandler(requestParams);

            for (int i = 0; i < parameters.Length; i++)
            {
                string queryValue = requestParams[parameters[i].Name];

                if (string.IsNullOrEmpty(queryValue))
                    paramValues[i] = null;
                else
                    paramValues[i] = DataConverter.ChangeType(queryValue, parameters[i].ParameterType);

                if (parameters[i].ParameterType == typeof(string) && paramValues[i] != null)
                {
                    paramValues[i] = HtmlDecode(paramValues[i].ToString());
                }
            }
            return paramValues;
        }

        internal void Invoke(ControllerInfo ctrlInfo)
        {
            throw new NotImplementedException();
        }

        private static string HtmlDecode(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString))
            {
                return encodedString;
            }

            string result = encodedString.Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Replace("&nbsp;", " ")
                    .Replace("&#39;", "'")
                    .Replace("&quot;", "\"");
            return result;
        }
    }
}
