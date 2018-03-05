using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Lib.Core;
using Lib.Web.MVC;
using Lib.Web.Json;
using System.Web.Hosting;
using System.Collections;

namespace Lib.Web
{
    /// <summary>
    /// Web帮助类
    /// </summary>
    /// <remarks>Web帮助类</remarks>
    public static class WebUtility
    {
        /// <summary>
        /// 检查HttpContext
        /// </summary>
        public static void CheckHttpContext()
        {
            ExceptionHelper.FalseThrow(HttpContext.Current != null,
                "无法取得HttpContext。代码必须运行在Web请求的处理过程中");
        }

        /// <summary>
        /// 是否允许向客户端输出异常详细信息
        /// </summary>
        public static bool AllowResponseExceptionStackTrace()
        {
            return GetWebApplicationCompilationDebug();
        }

        /// <summary>
        /// 处理URI 字符串
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static string ResolveUri(string uriString)
        {
            if (string.IsNullOrWhiteSpace(uriString))
                return string.Empty;

            Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

            if (!url.IsAbsoluteUri)
            {
                HttpRequest request = HttpContext.Current.Request;
                string appPathAndQuery = string.Empty;

                if (uriString[0].Equals('~'))
                    appPathAndQuery = request.ApplicationPath + uriString.Substring(1);
                else
                    if (!uriString[0].Equals('/'))
                        appPathAndQuery = string.Format("{0}/{1}", request.ApplicationPath, uriString);
                    else
                        appPathAndQuery = uriString;

                appPathAndQuery = appPathAndQuery.Replace("//", "/");
                uriString = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) + appPathAndQuery;
            }
            return uriString;
        }



        //private static readonly string PageRenderModeQueryStringName = "PageRenderMode" + (new object()).GetHashCode().ToString();
        /// <summary>
        /// 沈峥注释，改为常量
        /// </summary>
        private static readonly string PageRenderModeQueryStringName = "PageRenderMode";
        internal static object PageRenderControlItemKey = new object();

        /// <summary>
        /// 根据ContentTypeKey，得到Response的ContentType
        /// </summary>
        /// <param name="key">ContentTypeKey</param>
        /// <returns>ContentType</returns>
        /// <remarks>根据ContentTypeKey，得到Response的ContentType</remarks>
        public static string GetContentTypeByKey(string key)
        {
            ContentTypesSection section = WebConfigFactory.GetContentTypesSection();

            key = key.ToLower();
            ContentTypeConfigElement elt = section.ContentTypes[key];

            string contentType = elt != null ? elt.ContentType : string.Empty;

            return contentType;
        }

        /// <summary>
        /// 根据ContentTypeKey，得到Response的ContentType，如果值为空则返回默认key的ContentType
        /// </summary>
        /// <param name="key">ContentTypeKey</param>
        /// <param name="defaultKey">默认ContentTypeKey</param>
        /// <returns>ContentType</returns>
        /// <remarks>根据ContentTypeKey，得到Response的ContentType，如果值为空则返回默认key的ContentType</remarks>
        public static string GetContentTypeByKey(string key, string defaultKey)
        {
            string contentType = GetContentTypeByKey(key);
            if (contentType == string.Empty) contentType = GetContentTypeByKey(defaultKey);

            return contentType;
        }

        ///// <summary>
        ///// 根据ContentTypeKey，得到Response的ContentType
        ///// </summary>
        ///// <param name="key">ContentTypeKey</param>
        ///// <returns>ContentType</returns>
        ///// <remarks>根据ContentTypeKey，得到Response的ContentType</remarks>
        //public static string GetContentTypeByKey(ResponseContentTypeKey key)
        //{
        //    return GetContentTypeByKey(key.ToString());
        //}

        /// <summary>
        /// 根据文件名，得到Response的ContentType
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>ContentType</returns>
        /// <remarks>根据文件名，得到Response的ContentType</remarks>
        public static string GetContentTypeByFileName(string fileName)
        {
            string fileExtesionName = GetFileExtesionName(fileName);

            return GetContentTypeByFileExtesionName(fileExtesionName);
        }

        private static string GetFileExtesionName(string fileName)
        {
            string fileExtesionName = Path.GetExtension(fileName);

            return string.IsNullOrEmpty(fileExtesionName) ? fileExtesionName : fileExtesionName.Substring(1);
        }

        /// <summary>
        /// 根据文件扩展名，得到Response的ContentType
        /// </summary>
        /// <param name="fileExtesionName">文件扩展名</param>
        /// <returns>ContentType</returns>
        /// <remarks>根据文件扩展名，得到Response的ContentType</remarks>
        public static string GetContentTypeByFileExtesionName(string fileExtesionName)
        {
            ContentTypesSection section = WebConfigFactory.GetContentTypesSection();

            foreach (ContentTypeConfigElement elt in section.ContentTypes)
            {
                if (StringInCollection(fileExtesionName, elt.FileExtensionNames, true))
                {
                    return elt.ContentType;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 得到某一项Request的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="name">所要获取的Request的数据项</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>某一项Request的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Request的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestQueryString(string name, string defaultValue)
        {
            string str = HttpContext.Current.Request.QueryString[name];

            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        /// <summary>
        /// 得到某一项Request的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <typeparam name="T">获取值的类型</typeparam>
        /// <param name="name">所要获取的Request的数据项名称</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>某一项Request的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Request的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static T GetRequestQueryValue<T>(string name, T defaultValue)
            where T : IConvertible
        {
            string str = GetRequestQueryString(name, null);
            return str == null ? defaultValue : (T)DataConverter.ChangeType(str, typeof(T));
        }

        /// <summary>
        /// 得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="name">所要获取的Form的数据项</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestFormString(string name, string defaultValue)
        {
            string str = HttpContext.Current.Request.Form[name];

            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        /// <summary>
        /// 得到某一项ServerVariables数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="name">所要获取的ServerVariables的数据项</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项ServerVariables数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestServerVariable(string name, string defaultValue)
        {
            string str = HttpContext.Current.Request.ServerVariables[name];

            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        /// <summary>
        /// 得到某一项Cookies数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="cookieName">所要获取的Cookies的数据项</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Cookies数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestCookieString(string cookieName, string defaultValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];

            string str = cookie == null ? defaultValue : cookie.Value;
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        /// <summary>
        /// 得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替
        /// </summary>
        /// <param name="strName">所要获取的Params的数据项</param>
        /// <param name="strDefault">缺省值</param>
        /// <returns>某一项Post的数据，如果不存在该数据项，则用缺省值代替</returns>
        /// <remarks>得到某一项Post提交的数据，如果不存在该数据项，则用缺省值代替</remarks>
        public static string GetRequestParamString(string strName, string strDefault)
        {
            string str = HttpContext.Current.Request.Params[strName];

            return string.IsNullOrEmpty(str) ? strDefault : str;
        }


        /// <summary>
        /// 根据当前的HttpRequest中ExecutionUrl的，增加queryString，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </summary>
        /// <param name="appendQueryString">Url中的查询串，例如：uid=sz&amp;name=Haha</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中ExecutionUrl的，增加queryString，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。</remarks>
        public static string GetRequestExecutionUrl(string appendQueryString, params string[] ignoreParamNames)
        {
            HttpRequest request = HttpContext.Current.Request;
            string currentUrl = request.CurrentExecutionFilePath;

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;
                currentUrl = page.ResolveUrl(page.AppRelativeVirtualPath);
            }

            return GetRequestUrlInternal(currentUrl, request.QueryString, appendQueryString, ignoreParamNames);
        }

        /// <summary>
        /// 根据当前的HttpRequest中ExecutionUrl的，增加参数，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </summary>
        /// <param name="appendParamName">添加到QueryString中的参数名称</param>
        /// <param name="appendParamValue">添加到QueryString中的参数值</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中ExecutionUrl的，增加参数，并返回新的Url。通常，CurrentExecutionFilePath和Request的页面路径是
        /// 一样的，但是对于Server.Transfer类型的页面，则是不一样的。
        /// </remarks>
        public static string GetRequestExecutionUrl(string appendParamName, string appendParamValue, params string[] ignoreParamNames)
        {
            ignoreParamNames = StringArrayAdd(ignoreParamNames, appendParamName);

            string appendQueryString = string.Format("{0}={1}", appendParamName, appendParamValue);

            string result = GetRequestExecutionUrl(appendQueryString, ignoreParamNames);

            return result;
        }

        /// <summary>
        /// 根据当前的HttpRequest中Url的，增加queryString，并返回新的Url。
        /// </summary>
        /// <param name="appendQueryString">Url中的查询串，例如：uid=sz&amp;name=Haha</param>
        /// <param name="ignoreParamNames">忽略原始的QueryString中参数名称</param>
        /// <returns>结果Url</returns>
        /// <remarks>根据当前的HttpRequest中Url的，增加queryString，并返回新的Url。</remarks>
        public static string GetRequestUrl(string appendQueryString, params string[] ignoreParamNames)
        {
            HttpRequest request = HttpContext.Current.Request;

            string result = GetRequestUrlInternal(request.FilePath, request.QueryString, appendQueryString, ignoreParamNames);

            return result;
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，判断它是否是PostBack状态
        /// </summary>
        /// <returns>是否是PostBack状态</returns>
        public static bool IsCurrentHandlerPostBack()
        {
            bool result = false;

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("IsPostBack", BindingFlags.Instance | BindingFlags.Public);

                if (pi != null)
                    result = (bool)pi.GetValue(page, null);
            }

            return result;
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，判断它是否是Callback状态
        /// </summary>
        /// <returns>是否是Callback状态</returns>
        public static bool IsCurrentHandlerIsCallback()
        {
            bool result = false;

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("IsCallback", BindingFlags.Instance | BindingFlags.Public);

                if (pi != null)
                    result = (bool)pi.GetValue(page, null);
            }

            return result;
        }

        /// <summary>
        /// 是否为输出指定控件页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static bool IsRenderSpecialControlPage(Page page)
        {
            Control ctr = page.Items[PageRenderControlItemKey] as Control;

            return ctr != null;
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，那么找到其ViewState属性，获取其中的值
        /// </summary>
        /// <param name="key">ViewState的key</param>
        /// <returns>ViewState中的对象</returns>
        public static object LoadViewStateFromCurrentHandler(string key)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            object result = null;

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("ViewState", BindingFlags.Instance | BindingFlags.NonPublic);

                if (pi != null)
                {
                    StateBag vs = (StateBag)pi.GetValue(page, null);

                    if (vs != null)
                        result = vs[key];
                }
            }

            return result;
        }

        /// <summary>
        /// 如果当前的HtppHandler是Page，那么将数据存入到ViewState中
        /// </summary>
        /// <param name="key">ViewState的键值</param>
        /// <param name="data">需要存入的数据</param>
        public static void SaveViewStateToCurrentHandler(string key, object data)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;

                PropertyInfo pi = page.GetType().GetProperty("ViewState", BindingFlags.Instance | BindingFlags.NonPublic);

                if (pi != null)
                {
                    StateBag vs = (StateBag)pi.GetValue(page, null);

                    if (vs != null)
                        vs[key] = data;
                }
            }
        }

        private static string GetRequestUrlInternal(string filePath, NameValueCollection queryString, string appendQueryString, params string[] ignoreParamNames)
        {
            string result = filePath;

            string originalQuery = BuildQueryString(queryString, ignoreParamNames);

            if (originalQuery != string.Empty)
                result += "?" + originalQuery + "&" + appendQueryString;
            else
                result += "?" + appendQueryString;

            return result;
        }

        private static string BuildQueryString(NameValueCollection queryString, params string[] ignoreParamKeys)
        {
            StringBuilder strB = new StringBuilder(1024);

            foreach (string key in queryString.Keys)
            {
                if (StringInCollection(key, ignoreParamKeys, true) == false)
                {
                    if (strB.Length > 0)
                        strB.Append("&");

                    strB.Append(key + "=" + queryString[key]);
                }
            }

            return strB.ToString();
        }

        /// <summary>
        /// 在集合中找字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stringCollection"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        private static bool StringInCollection(string value, IEnumerable<string> stringCollection, bool ignoreCase)
        {
            bool bResult = false;

            StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

            foreach (string str in stringCollection)
            {
                if (string.Equals(value, str, comparison))
                {
                    bResult = true;
                    break;
                }
            }

            return bResult;
        }

        private static string[] StringArrayAdd(string[] array, string addStr)
        {
            StringCollection strList = new StringCollection();
            strList.AddRange(array);
            strList.Add(addStr);
            string[] result = new string[strList.Count];
            strList.CopyTo(result, 0);

            return result;
        }

        /// <summary>
        /// 检查脚本中的字符串，替换掉双引号和回车
        /// </summary>
        /// <param name="strData">字符串</param>
        /// <returns>替换后的结果</returns>
        public static string CheckScriptString(string strData)
        {
            strData = strData.Replace("\\", "\\\\");
            strData = strData.Replace("\"", "\\\"");
            strData = strData.Replace("/", "\\/");
            strData = strData.Replace("\n\r", "\\n");
            strData = strData.Replace("\r\n", "\\n");
            strData = strData.Replace("\n", "\\n");

            strData = strData.Replace("\\n", "<br/>");

            return strData;
        }



        /// <summary>
        /// 在Debug模式下，禁止使用
        /// </summary>
        public static void SetResponseNoCacheWhenDebug()
        {
            if (GetWebApplicationCompilationDebug())
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        /// <summary>
        /// 获取客户端的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string ip = WebUtility.GetRequestServerVariable("HTTP_X_FORWARDED_FOR", string.Empty);

            if (!IsIPAddress(ip))
                ip = WebUtility.GetRequestServerVariable("REMOTE_ADDR", string.Empty);

            if (!IsIPAddress(ip))
                ip = HttpContext.Current.Request.UserHostAddress;

            return ip;
        }

        internal static bool GetWebApplicationCompilationDebug()
        {
            bool debug = false;
            CompilationSection compilation = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");

            if (compilation != null)
            {
                debug = compilation.Debug;
            }

            return debug;
        }


        private static bool IsIPAddress(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length < 7 || str.Length > 15)
                return false;

            string regFormat = @"^([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$";

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regFormat, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return regex.IsMatch(str);
        }

        public static T GetSessionValue<T>(string name, T defaultValue)
        {
            object sessionValue = HttpContext.Current.Session[name];
            if (sessionValue == null)
            {
                return defaultValue;
            }
            return (T)sessionValue;
        }

        public static void SetSessionValue<T>(string name, T value)
        {
            HttpContext.Current.Session[name] = value;
        }


        public static T GetValue<T>(this NameValueCollection dict, string name, T defaultValue)
        {
            string value = dict[name];
            T result = defaultValue;
            if (value == null)
            {
                return result;
            }
            try
            {
                result = DataConverter.ChangeType<object, T>(value);
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }

        public static T GetValue<T>(this IDictionary dict, string name, T defaultValue)
        {
            object value = dict[name];
            T result = defaultValue;
            if (value == null)
            {
                return result;
            }
            try
            {
                result = DataConverter.ChangeType<object, T>(value);
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将影响页面的Filter参数转换为Base64加密的querystring, 如 23ufjawjf9q
        /// </summary>
        /// <param name="pageFilter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>
        /// Encrypted/Decode , 5w/7s
        /// </remarks>
        public static string Base64EncryptedQueryString(object pageFilter, Type type)
        {
            string jsonFilter = JsonHelper.Serialize(pageFilter, type);
            byte[] codes = Encoding.Unicode.GetBytes(jsonFilter.ToCharArray());
            string base64Encrypted = Convert.ToBase64String(codes);

            string result = HttpUtility.UrlEncode(base64Encrypted);
            return result;
        }

        public static T DecodeBase64edQueryString<T>(string encryptedString)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(encryptedString), "Argument encryptedString is empty!");


            //string base64String = HttpUtility.UrlDecode(encryptedString);
            byte[] codes = Convert.FromBase64String(encryptedString);
            string jsonFilter = new string(Encoding.Unicode.GetChars(codes));

            T result = JsonHelper.Deserialize<T>(jsonFilter);
            return result;

        }

        public static string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}
