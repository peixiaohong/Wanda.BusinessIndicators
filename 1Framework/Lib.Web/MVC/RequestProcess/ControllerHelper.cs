using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using Lib.Core;
using Lib.Web.MVC.Controller;
using Lib.Web.Mvc;

namespace Lib.Web.MVC
{
    /// <summary>
    /// 将Request映射到相应的ProcessRequest方法
    /// </summary>
    internal static class ControllerHelper
    {
        public static void ExecuteProcess(HttpContext context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(context != null, "HttpContext");

            ControllerInfo ctrlInfo = ParseController(context); // throw ContrllerParseFailException or ControllerNotFoundException
            ActionInfo actionInfo = ParseAction(context, ctrlInfo); // get action and parameters  ;throw ContrllerParseFailException



            if (ctrlInfo.JxType == JxTypeEnum.Jx)
            {
                ActionResult result = ActionResult.Create(actionInfo.Invoke());
                result.ExecuteResult(context);
            }
            else if (ctrlInfo.JxType == JxTypeEnum.JxHtml)
            {
                // 返回显示， 调试用
                context.Items["actionResult"] = actionInfo.Invoke();
                context.Server.Execute("~/_pagelet/jxhtml.aspx");
            }


        }

        private const string ajax_suffix = ".jx";
        private const string ajax_html_suffix = ".jxhtml";
        private const bool useDefaultActionWhenNotFound = true;



        private static JxTypeEnum ParseJxType(string url)
        {
            string suffix = url.Substring(url.LastIndexOf('.')).ToLower();

            JxTypeEnum result;
            if (suffix == ajax_suffix)
            {
                result = JxTypeEnum.Jx;
            }
            else if (suffix == ajax_html_suffix)
            {
                result = JxTypeEnum.JxHtml;
            }
            else
            {
                result = JxTypeEnum.Unknown;
            }
            return result;
        }

        private static ControllerInfo ParseController(HttpContext context)
        {
            string url = context.Request.Path;
            JxTypeEnum jxType = ParseJxType(url);
            //必须是.jx结尾
            if (jxType == JxTypeEnum.Unknown)
            {
                throw new ContrllerParseFailException();
            }


            string[] parts = GetUrlDivision(context);

            if (parts.Length == 0)
            {
                throw new ControllerNotFoundException(); //无Controller
            }

            BaseController controller = BaseController.GetController(parts[0]); // parts[0] should be controller name
            if (controller == null)
            {
                throw new ControllerNotFoundException(); //无Controller
            }

            Type controllerType = controller.GetType();
            ControllerInfo result = null;
            if (!ControllerInfoCache.Instance.TryGetValue(controllerType, out result))
            {
                result = new ControllerInfo(controller);
                ControllerInfoCache.Instance.Add(controllerType, result);
            }
            result.JxType = jxType;
            return result;
        }

        private static ActionInfo ParseAction(HttpContext context, ControllerInfo ctrlInfo)
        {
            string[] parts = GetUrlDivision(context);

            ActionInfo result = null;
            if (parts.Length == 1 || parts.Length == 0) // parts.Length ==0 在ParseController中已经判断过了， 
            {
                if (ctrlInfo.DefaultMethod != null)
                {
                    result = new ActionInfo(context, ctrlInfo.Controller, ctrlInfo.DefaultMethod);
                }
                else
                {
                    throw new ActionNotFoundException();
                }
            }
            else
            {
                string actionName = parts[1];
                MethodInfo methodInfo = ctrlInfo.Methods.Find(m => string.Compare(actionName, m.Name, true) == 0);

                if (methodInfo == null)
                {
                    if (useDefaultActionWhenNotFound && ctrlInfo.DefaultMethod != null)
                    {
                        methodInfo = ctrlInfo.DefaultMethod;
                    }
                    else
                    {
                        throw new ActionNotFoundException();
                    }
                }

                result = new ActionInfo(context, ctrlInfo.Controller, methodInfo);
            }
            return result;
        }

        private static string[] GetUrlDivision(HttpContext context)
        {
            string url = context.Request.Path;
            string[] parts = url
                            .Substring(0, url.LastIndexOf('.')) //删除尾部
                            .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries); // 分割
            return parts;
        }

    }

    public class ContrllerParseFailException : ApplicationException { }
    public class ControllerNotFoundException : ApplicationException { }
    public class ActionParseFailException : ApplicationException { }
    public class ActionNotFoundException : ApplicationException { }

    internal enum JxTypeEnum
    {
        Jx = 0,
        JxHtml = 1,
        Unknown = 127
    }
}
