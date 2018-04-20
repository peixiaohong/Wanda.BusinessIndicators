using Lib.Cache;
using Lib.Config;
using Lib.Core;
using System;
using System.Configuration;
using System.Web;
using System.Web.Hosting;
using LJTH.Lib.AuthCenter.ViewModel;

namespace LJTH.BusinessIndicators.Common
{
    /// <summary>
    /// 帮助类， 封装常用的Web后台调用的静态方法
    /// </summary>
    public class WebHelper
    {
        public static WebHelper Instance = new WebHelper();

        /// <summary>
        /// 需要在程序启动时注册的一个事件： 根据用户登陆名返回用户的的信息
        /// </summary>
        public event Func<string, LoginUserInfo> GetUser;

        public bool EnabledSSO
        {
            get
            {
                string enabledSSO = System.Configuration.ConfigurationManager.AppSettings["EnabledSSO"] ?? "true";
                bool result = false;
                bool.TryParse(enabledSSO, out result);
                return result;
            }
        }
        /// <summary>
        /// 操作权限模式
        /// </summary>
        public bool OperationMode
        {
            get
            {
                string operationMode = System.Configuration.ConfigurationManager.AppSettings["OperationMode"] ?? "true";
                bool result = false;
                bool.TryParse(operationMode, out result);
                return result;
            }
        }
        

        /// <summary>
        /// 获得当前的登陆用户信息
        /// </summary>
        /// <returns></returns>
        public LoginUserInfo GetLoginUserInfo()
        {
            string cacheKey = null;
            LoginUserInfo result = null;

            string ssoUsername = HttpContext.Current.Items["WD_SSO_UserName"] != null ? HttpContext.Current.Items["WD_SSO_UserName"].ToString() : string.Empty;
            string strUserName = HttpContext.Current.User.Identity != null ? HttpContext.Current.User.Identity.Name : ssoUsername;
            if (!EnabledSSO)
            {
                strUserName = ConfigurationManager.AppSettings["virtualUser"];
            }
            cacheKey = strUserName;

            if (string.IsNullOrEmpty(cacheKey))
            {
                return result;
            }

            if (!LoginUserInfoCache.Instance.TryGetValue(cacheKey, out result))
            {
                if (GetUser != null)
                {
                    result = GetUser(cacheKey);

                    if (result == null)
                    {
                        //throw new Exception("该用户无权登录绩效考核管理系统，请联系系统管理员。");
                        return null;
                    }

                    LoginUserInfoCache.Instance.Add(cacheKey, result);
                }
            }

            return result;


        }

        /// <summary>
        /// 获得当前的登陆用户信息,如果当前登录用户失效， 则抛出异常
        /// </summary>
        /// <returns></returns>
        public static LoginUserInfo GetCurrentUser() { return GetCurrentUser(true); }

        public static string GetCurrentLoginUser() { return GetCurrentUser().LoginName; }
        /// <summary>
        /// 获得当前的登陆用户信息
        /// </summary>
        /// <param name="throwExceptionWhenNull">当true时，如果当前登录用户失效， 则抛出异常</param>
        /// <returns></returns>
        /// <exception cref="LoginUserNullException"></exception>
        public static LoginUserInfo GetCurrentUser(bool Redeict=true)
        {
            LoginUserInfo result = WebHelper.Instance.GetLoginUserInfo();

            if (result == null && Redeict)
            {
                //if (throwExceptionWhenNull) { throw new LoginUserNullException(); }
                HttpContext context=HttpContext.Current;
                context.Response.Redirect(string.Format("{0}?returnurl={1}", System.Web.Security.FormsAuthentication.LoginUrl, context.Request.RawUrl));
            }
            return result;
        }
        /// <summary>
        /// 权限缓存依赖的文件
        /// </summary>
        public const string AuthCacheDependencyFile = "~/app_data/authCacheDependency.txt";

        /// <summary>
        /// 每次更新权限，包括更新角色用户， 都需要调用此方法来清除缓存
        /// </summary>
        public static void InvalidAuthCache()
        {
            string file = MapPath(AuthCacheDependencyFile);

            System.IO.File.WriteAllText(file, DateTime.Now.ToString());
        }

        public static string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }


        private const string TimeSessionKey = "DateTimeOffset";
        public static DateTime DateTimeNow
        {
            get
            {
                if (AppSettingConfig.GetSetting("UseMockDate", "false").ToLower() != "true")
                {
                    return DateTime.Now;
                }
                else if (HttpContext.Current == null || HttpContext.Current.Session == null)
                {
                    return DateTime.Now;
                }
                else if (HttpContext.Current.Session[TimeSessionKey] != null)
                {
                    TimeSpan offset = (TimeSpan)HttpContext.Current.Session[TimeSessionKey];
                    return DateTime.Now.Add(offset);
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }

        public static DateTime GetTimeNow()
        {
            return DateTimeNow;
        }
        public static string ConvertPlainTextForShow(string plainText, bool keepWhiteSpace)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            string textToFormat = plainText;
            textToFormat = textToFormat.Replace("\n", "<br/>");
            if (keepWhiteSpace)
            {
                textToFormat = textToFormat.Replace(" ", "&nbsp;");
            }

            return textToFormat;
        }

        public static string ConvertPlainTextForEdit(string plainText, bool keepWhiteSpace)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            string textToFormat = plainText;
            textToFormat = textToFormat.Replace("<br/>", "\n");
            if (keepWhiteSpace)
            {
                textToFormat = textToFormat.Replace("&nbsp;", " ");
            }

            return HttpUtility.HtmlDecode(textToFormat);
        }

        public static void ResetAppTime(DateTime newDate)
        {

            if (AppSettingConfig.GetSetting("UseMockDate", "false").ToLower() == "true")
            {

                DateTime newAppTime = newDate.Add(DateTime.Now.Subtract(DateTime.Now.Date));

                TimeSpan offset = newAppTime.Subtract(DateTime.Now);
                HttpContext.Current.Session[TimeSessionKey] = offset;
            }
        }
    }
}
