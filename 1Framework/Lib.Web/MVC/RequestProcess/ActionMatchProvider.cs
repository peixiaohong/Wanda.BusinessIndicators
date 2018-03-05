using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using Lib.Core;

namespace Lib.Web.MVC
{
    //internal class ActionMatchProvider
    //{
    //    //public static readonly string ActionKey = "action";

    //    protected Type ControllerType { get; set; }

    //    protected HttpRequest Request { get; set; }

    //    protected ControllerInfo ControllerInfo { get; set; }

    //    public ActionMatchProvider(Type controller, HttpRequest request)
    //    {
    //        this.ControllerType = controller;
    //        this.Request = request;
    //    }

    //    public static ActionMatchProvider CreateProvider(Type controller, HttpRequest request)
    //    {
    //        return new ActionMatchProvider(controller, request);
    //    }

    //    public MethodInfo GetMatchMethod()
    //    {
    //        ExceptionHelper.FalseThrow<ArgumentNullException>(this.ControllerType != null, "ControllerType");

    //        ControllerInfo handlerInfo = null;
    //        if (!ControllerInfoCache.Instance.TryGetValue(this.ControllerType, out handlerInfo))
    //        {
    //            handlerInfo = GetControllerMethods();
    //            ControllerInfoCache.Instance.Add(this.ControllerType, handlerInfo);
    //        }
    //        this.ControllerInfo = handlerInfo;

    //        MethodInfo mi = GetMatchMethodByParams(this.ControllerInfo.DefaultMethod);

    //        return mi;
    //    }

    //    private ControllerInfo GetControllerMethods()
    //    {
    //        MethodInfo[] mis = this.ControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

    //        ControllerInfo controllers = new ControllerInfo();

    //        foreach (MethodInfo mi in mis)
    //        {
    //            LibActionAttribute attr = AttributeHelper.GetCustomAttribute<LibActionAttribute>(mi);
    //            if (attr != null)
    //            {
    //                controllers.Methods.Add(mi);
    //                if (controllers.DefaultMethod == null && attr.Default)
    //                    controllers.DefaultMethod = mi;
    //            }
    //        }
    //        return controllers;
    //    }

    //    /// <summary>
    //    /// 在Controller中找符合的Action， 如果没找到， 则返回默认方法
    //    /// </summary>
    //    /// <param name="defaultMethod">默认方法</param>
    //    /// <returns></returns>
    //    protected virtual MethodInfo GetMatchMethodByParams(MethodInfo defaultMethod)
    //    {
    //        string methodName = string.Empty;
    //        NameValueCollection requestParams = GetRequestParameters(out methodName);

    //        if (string.IsNullOrEmpty(methodName)==false)
    //        {
    //            HttpContext.Current.Items["action"] = methodName; //保存在Context中，  以方便其他地方可能需要查看、使用
    //        }

    //        List<MethodInfo> methodList = GetMatchedMethods(methodName);

    //        MethodInfo result = null;
    //        int maxMatchLevel = 0;

    //        foreach (MethodInfo mi in methodList)
    //        {
    //            int level = GetParamsMatchedLevel(mi, requestParams);
    //            if (level > maxMatchLevel)
    //            {
    //                maxMatchLevel = level;
    //                result = mi;
    //            }
    //        }

    //        if (result == null)
    //        {
    //            result = this.ControllerInfo.DefaultMethod;
    //            ExceptionHelper.TrueThrow(result == null, "无法找到Action:{0}，且无默认的处理方法。", methodName);
    //        }

    //        return result;
    //    }

    //    /// <summary>
    //    /// 根据方法名称找到对应的方法，不区分大小写。如果方法名称为空， 则返回所有的方法列表。
    //    /// </summary>
    //    /// <param name="methodName"></param>
    //    /// <returns></returns>
    //    protected virtual List<MethodInfo> GetMatchedMethods(string methodName)
    //    {
    //        List<MethodInfo> methodList = new List<MethodInfo>();
    //        if (!string.IsNullOrEmpty(methodName))
    //        {
    //            this.ControllerInfo.Methods.ForEach(method =>
    //            {
    //                if (string.Compare(methodName, method.Name, true) == 0)
    //                {
    //                    methodList.Add(method);
    //                }
    //            });
    //        }
    //        else
    //            methodList = this.ControllerInfo.Methods;

    //        return methodList;
    //    }

    //    protected virtual NameValueCollection GetRequestParameters(out string methodName)
    //    {
    //        NameValueCollection result = null;
    //        if (Request.RequestType.ToUpper() == "GET")
    //        {
    //            result = new NameValueCollection(Request.QueryString);
    //        }
    //        else
    //        { result = new NameValueCollection(Request.Form); }

    //        //methodName = Request.QueryString[ActionKey];
    //        //if (String.IsNullOrEmpty(methodName))
    //        //    methodName = Request.Form[ActionKey];

    //        methodName = result[ActionKey];
    //        result.Remove(ActionKey);



    //        return result;
    //    }

    //    protected virtual int GetParamsMatchedLevel(MethodInfo mi, NameValueCollection queryString)
    //    {
    //        int result = 0;
    //        ParameterInfo[] parameters = mi.GetParameters();

    //        if (queryString.Count == 0 && parameters.Length == 0)
    //            return 1;
    //        else
    //        {
    //            foreach (ParameterInfo param in parameters)
    //            {
    //                if (queryString[param.Name] != null)
    //                    result++;
    //            }
    //        }

    //        result -= Math.Abs(parameters.Length - result);	//方法参数的个数减去匹配程度，形成新的匹配度

    //        return result;
    //    }


    //}

   
   
}
