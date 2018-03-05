using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Lib.Core
{
    /// <summary>
    /// 提供和Uri相关处理的相关函数。这里采用静态方法的形式提供出Uri中的参数提取及Uri解析等功能。 
    /// </summary>
    public static class UriHelper
    {
        #region Public
        /// <summary>
        /// 分析Url，得到所有的参数集合
        /// </summary>
        /// <param name="url">Uri类型的Url，绝对路径或相对路径</param>
        /// <returns>NameValueCollection，参数集合</returns>
        public static NameValueCollection GetUriParamsCollection(Uri url)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(url != null, "url");

            return GetUriParamsCollection(url.ToString());
        }

        /// <summary>
        /// 从url中，获取参数的集合
        /// </summary>
        /// <param name="uriString">url</param>
        /// <returns>参数集合</returns>
        public static NameValueCollection GetUriParamsCollection(string uriString)
        {
            return GetUriParamsCollection(uriString, true);
        }

        /// <summary>
        /// 从url中，获取参数的集合
        /// </summary>
        /// <param name="uriString">url</param>
        /// <param name="urlDecode">是否执行decode</param>
        /// <returns>参数集合</returns>
        public static NameValueCollection GetUriParamsCollection(string uriString, bool urlDecode)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

            string bookmarkString = GetBookmarkStringInUrl(uriString);

            if (bookmarkString != string.Empty)
                uriString = uriString.Remove(uriString.Length - bookmarkString.Length, bookmarkString.Length);

            string query = uriString;

            int startIndex = query.IndexOf("?");

            if (startIndex >= 0)
                query = query.Substring(startIndex + 1);

            if (query != string.Empty)
            {
                string[] parts = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts.Length; i++)
                {
                    int equalsSignIndex = parts[i].IndexOf("=");

                    string paramName = string.Empty;
                    string paramValue = string.Empty;

                    if (equalsSignIndex >= 0)
                    {
                        //存在等号
                        paramName = parts[i].Substring(0, equalsSignIndex);
                        paramValue = parts[i].Substring(equalsSignIndex + 1);
                    }

                    if (string.IsNullOrEmpty(paramName) == false)
                    {
                        if (urlDecode)
                        {
                            paramName = HttpUtility.UrlDecode(paramName);
                            paramValue = HttpUtility.UrlDecode(paramValue);
                        }

                        AddValueToCollection(paramName, paramValue, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 将Url中的参数进行排序，返回参数排序后的url串
        /// </summary>
        /// <param name="url">Uri类型的Url，绝对路径或相对路径</param>
        /// <returns>参数排序后的url串</returns>
        public static string GetUrlWithSortedParams(Uri url)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(url != null, "url");

            return GetUrlWithSortedParams(url.ToString());
        }

        /// <summary>
        /// 将Url中的参数进行排序，返回参数排序后的url串
        /// </summary>
        /// <param name="uriString">Url，绝对路径或相对路径</param>
        /// <returns>参数排序后的url串</returns>
        public static string GetUrlWithSortedParams(string uriString)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

            string query = string.Empty;
            string leftPart = string.Empty;

            int startIndex = uriString.IndexOf("?");

            if (startIndex >= 0)
            {
                leftPart = uriString.Substring(0, startIndex) + "?";
                query = uriString.Substring(startIndex + 1);
            }
            else
                leftPart = uriString;

            StringBuilder strB = new StringBuilder(2048);

            if (query != string.Empty)
            {
                NameValueCollection paramCollection = GetUriParamsCollection(query);
                string[] allKeys = paramCollection.AllKeys;

                Array.Sort(allKeys, System.Collections.CaseInsensitiveComparer.Default);

                for (int i = 0; i < allKeys.Length; i++)
                {
                    string key = allKeys[i];

                    if (strB.Length > 0)
                        strB.Append("&");

                    strB.Append(HttpUtility.UrlEncode(key));

                    if (key != string.Empty)
                        strB.Append("=");

                    strB.Append(HttpUtility.UrlEncode(paramCollection[key]));
                }
            }

            return leftPart + strB.ToString();
        }

        /// <summary>
        /// 将参数重新组合成Url
        /// </summary>
        /// <param name="uriString">url</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="requestParamsArray">参数集合的数组</param>
        /// <returns>补充了参数的url</returns>
        public static string CombineUrlParams(string uriString, Encoding encoding, params NameValueCollection[] requestParamsArray)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");
            ExceptionHelper.FalseThrow<ArgumentNullException>(encoding != null, "encoding");
            ExceptionHelper.FalseThrow<ArgumentNullException>(requestParamsArray != null, "requestParamsArray");

            NameValueCollection requestParams = MergeParamsCollection(requestParamsArray);

            StringBuilder strB = new StringBuilder(1024);

            string leftPart = string.Empty;

            int startIndex = uriString.IndexOf("?");

            if (startIndex >= 0)
                leftPart = uriString.Substring(0, startIndex);
            else
                leftPart = uriString;

            for (int i = 0; i < requestParams.Count; i++)
            {
                if (i == 0)
                    strB.Append("?");
                else
                    strB.Append("&");

                strB.Append(HttpUtility.UrlEncode(requestParams.Keys[i], encoding));
                strB.Append("=");
                strB.Append(HttpUtility.UrlEncode(requestParams[i], encoding));
            }

            return leftPart + strB.ToString();
        }

        /// <summary>
        /// 将参数重新组合成Url
        /// </summary>
        /// <param name="uriString">url</param>
        /// <param name="requestParamsArray">参数集合的数组</param>
        /// <returns>补充了参数的url</returns>
        public static string CombineUrlParams(string uriString, params NameValueCollection[] requestParamsArray)
        {
            return CombineUrlParams(uriString, Encoding.UTF8, requestParamsArray);
        }

        /// <summary>
        /// 得到url中的书签部分。“#”后面的部分
        /// </summary>
        /// <param name="queryString">http://localhost/lianhome#littleTurtle</param>
        /// <returns>littleTurtle</returns>
        public static string GetBookmarkStringInUrl(string queryString)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(queryString != null, "queryString");

            int bookmarkStart = -1;

            for (int i = queryString.Length - 1; i >= 0; i--)
            {
                if (queryString[i] == '#')
                    bookmarkStart = i;
                else
                    if (queryString[i] == '&' || queryString[i] == '?')
                        break;
            }

            string result = string.Empty;

            if (bookmarkStart >= 0)
                result = queryString.Substring(bookmarkStart);

            return result;
        }

        /// <summary>
        /// 解析Uri，如果Uri为相对路径，处理Uri中~，将其替换为当前的Web应用
        /// </summary>
        /// <param name="uriString">Uri</param>
        /// <returns>如果Uri为相对路径，处理Uri中~，将其替换为当前的Web应用</returns>
        public static Uri ResolveUri(string uriString)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

            Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

            if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(uriString) == false)
            {
                if (EnvironmentHelper.Mode == InstanceMode.Web)
                {
                    if (uriString[0] == '~')
                    {
                        HttpRequest request = HttpContext.Current.Request;
                        string appPathAndQuery = request.ApplicationPath + uriString.Substring(1);

                        appPathAndQuery = appPathAndQuery.Replace("//", "/");

                        uriString = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
                                    appPathAndQuery;

                        url = new Uri(uriString);
                    }
                }
            }

            return url;
        }
        #endregion

        #region Private
        private static NameValueCollection MergeParamsCollection(NameValueCollection[] requestParams)
        {
            NameValueCollection result = new NameValueCollection();

            for (int i = 0; i < requestParams.Length; i++)
                MergeTwoParamsCollection(result, requestParams[i]);

            return result;
        }

        private static void MergeTwoParamsCollection(NameValueCollection target, NameValueCollection src)
        {
            foreach (string key in src.Keys)
            {
                if (target[key] == null)
                    target.Add(key, src[key]);
            }
        }

        private static void AddValueToCollection(string paramName, string paramValue, NameValueCollection result)
        {
            string oriValue = result[paramName];

            if (oriValue == null)
                result.Add(paramName, paramValue);
            else
            {
                string rValue = oriValue;

                if (oriValue.Length > 0)
                    rValue += ",";

                rValue += paramValue;

                result[paramName] = rValue;
            }
        }
        #endregion
    }
}
