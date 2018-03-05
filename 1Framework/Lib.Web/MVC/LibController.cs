using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Lib.Validation;
using System.Data.SqlClient;
using Lib.Web.Exceptions;
using Lib.Web.Json;
using System.Collections;

namespace Lib.Web.Mvc
{
    //[Obsolete("使用同一的MvcHttpHandler来处理MVC，此对象不再使用",true)]
    //public abstract class LibController : IHttpHandler
    //{
    //    public virtual bool IsReusable
    //    {
    //        get { return false; }
    //    }

    //    public void ProcessRequest(HttpContext context)
    //    {
    //        WebUtility.CheckHttpContext();


    //        // 最好在此有一层异常封装的机制
    //        // TODO
    //        BeforeProcess(context);
    //        context.Response.Write("Hello");
    //        // try...catch..
    //        try
    //        {
    //            //WebUtility.ExecuteProcess(this);
    //        }
    //        catch (ApplicationException ex)
    //        {
    //            ResponseException(ex, GetErrorDetail);
    //        }

    //        catch (Exception ex)
    //        {
    //            Exception innerEx = ex.InnerException;
    //            if (innerEx == null)
    //            {
    //                ResponseException(ex, GetErrorDetail);
    //            }
    //            else
    //            {

    //                if (innerEx is ValidationException ||
    //                    innerEx is CustomizedException ||
    //                    innerEx is ArgumentException ||
    //                    innerEx is SqlException
    //                    )
    //                {
    //                    ResponseException(innerEx, GetErrorMsg);
    //                }
    //                else
    //                {
    //                    ResponseException(innerEx, GetErrorDetail);
    //                }
    //            }
    //        }
    //        AfterProcess(context);

    //    }
    //    private static void ResponseException(Exception ex, Func<Exception, string> exToString)
    //    {
    //        //LibViewModel failure = LibViewModel.CreateFailureJSONResponseViewModel(exToString(ex));
    //        //ResponseWriter.Write(failure);
    //    }
    //    private static void ResponseException(Exception ex, LibViewModelCode code, Func<Exception, string> exToString)
    //    {
    //        LibViewModel failure = LibViewModel.CreateFailureJSONResponseViewModel(code, exToString(ex));
    //        ResponseWriter.Write(failure);
    //    }

    //    private static string GetErrorDetail(Exception ex)
    //    {
    //        return ex.ToString();
    //    }


    //    private static string GetErrorMsg(Exception ex)
    //    { 
    //        return ex.Message;
    //    }



    //    protected virtual void AfterProcess(HttpContext context)
    //    {

    //    }

    //    protected virtual void BeforeProcess(HttpContext context)
    //    {

    //    }

    //    /// <summary>
    //    /// 反序列化Json数据， 并对重要字段检查
    //    /// </summary>
    //    /// <typeparam name="T1">集合</typeparam>
    //    /// <typeparam name="T2"></typeparam>
    //    /// <param name="jsonPostData">json数据</param>
    //    /// <param name="check">检查， 防止重点数据未能反序列化后加载； 如果为null或者Empty的话， 则不检查</param>
    //    /// <returns></returns>
    //    protected static T1 DeserializeAndCheck<T1, T2>(string jsonPostData, Predicate<T2> check)
    //        where T1 : IEnumerable<T2>
    //    {
    //        if (string.IsNullOrEmpty(jsonPostData))
    //        {
    //            return default(T1);
    //        }
    //        try
    //        {
    //            T1 result = JsonHelper.Deserialize<T1>(jsonPostData);
    //            if (((IEnumerable<T2>)result).Count() == 0)
    //            {
    //                return result;
    //            }
    //            T2 obj = ((IEnumerable<T2>)result).ToList().First();
    //            if (check != null && !check(obj))
    //            {
    //                throw new ArgumentException("数据反序列化异常：" + jsonPostData);
    //            }
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ArgumentException("数据反序列化异常：" + jsonPostData, ex);
    //        }
    //    }

    //    /// <summary>
    //    /// 反序列化Json数据， 并对重要字段检查
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="jsonPostData">json数据</param>
    //    /// <param name="check">检查， 防止重点数据未能反序列化后加载</param>
    //    /// <returns></returns>
    //    protected static T DeserializeAndCheck<T>(string jsonPostData, Predicate<T> check)
    //    {
    //        if (string.IsNullOrEmpty(jsonPostData))
    //        {
    //            return default(T);
    //        }
    //        try
    //        {
    //            T result = JsonHelper.Deserialize<T>(jsonPostData);

    //            if (check != null && !check(result))
    //            {
    //                throw new ArgumentException("数据反序列化异常：" + jsonPostData);
    //            }
    //            return result;

    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ArgumentException("数据反序列化异常：" + jsonPostData, ex);
    //        }
    //    }

    //    /// <summary>
    //    /// Default Action, 以方便显示的提醒开发人员
    //    /// </summary>
    //    [LibAction(true)]
    //    public virtual void DefaultAction()
    //    {
    //        string msg = "DefaultAction Executing.";
    //        if (HttpContext.Current != null && HttpContext.Current.Items["action"] != null)
    //        {
    //            msg += "The expected action is " + HttpContext.Current.Items["action"].ToString();
    //        }

    //        LibViewModel model = LibViewModel.CreateFailureJSONResponseViewModel(LibViewModelCode.InvalidActionFlag, msg);
    //        ResponseWriter.Write(model);
    //    }


    //    //protected virtual void ProcessResponse(LibViewModel result)
    //    //{
    //    //    ResponseWriter writer = ResponseWriter.CreateResponseWriter(result);
    //    //    writer.Write();
    //    //}

    //    //protected virtual LibViewModel ProcessRequest()
    //    //{
    //    //    LibViewModel obj = WebUtility.ExecuteProcess(this) as LibViewModel;
    //    //    if (obj == null)
    //    //    {
    //    //        obj = new LibViewModel(LibViewModelType.Detect, LibViewModelCode.UnhandledErrors, "服务端未知错误");
    //    //    }
    //    //    return obj;
    //    //}
    //}
}
