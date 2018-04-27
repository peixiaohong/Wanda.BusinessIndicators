using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Plugin.SSO
{
    public class SSOToolkit
    {
        private static SSOToolkit _Instance = null;
        public static SSOToolkit Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SSOToolkit();
                }
                return _Instance;
            }
        }
        private static readonly ILogger logger = LogManager.GetLogger("SSOLogin", typeof(SSOToolkit));
        /// <summary>
        /// 为OA待办追加签名
        /// </summary>
        /// <param name="url"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public string GetAuthOAUrlWithSSO(string url, string receiver, string flowid)
        {
            string safe_url = "/Public/OABridge.aspx";
            var returnurl = HttpUtility.UrlEncode(url);
            safe_url += "?";
            safe_url += "uid=" + receiver;
            safe_url += "&businessid=" + flowid;
            safe_url += "&returnurl=" + returnurl;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("uid", receiver);
            dict.Add("businessid", flowid);
            dict.Add("returnurl", returnurl);
            var sign = Plugin.SSO.SSOToolkit.GetSignature(dict);
            safe_url += "&signature=" + sign;

            return safe_url;
        }
        /// <summary>
        /// 对调用接口验证成功以后的token进行客户端授权
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accessToken"></param>
        internal void AuthLogin(string userName, string accessToken)
        {

            var loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var expiryMinutes = (24 * 60).ToString();

            var dict = new Dictionary<string, string>();
            dict.Add("accessToken", HttpUtility.UrlEncode(accessToken));
            dict.Add("loginId", userName);
            dict.Add("loginTime", loginTime);
            dict.Add("expiryMinutes", expiryMinutes);

            WriteCookie("accessToken", HttpUtility.UrlEncode(accessToken));
            WriteCookie("loginId", userName);
            WriteCookie("loginTime", loginTime);
            WriteCookie("expiryMinutes", expiryMinutes);
            WriteCookie("signature", GetSignature(dict));

        }
        /// <summary>
        /// 退出
        /// </summary>
        public void Logout()
        {

            WriteCookie("accessToken", "");
            WriteCookie("loginId", "");
            WriteCookie("loginTime", "");
            WriteCookie("expiryMinutes", "");
            WriteCookie("signature", "");
            WriteCookie("user", "");

        }
        private void RemoveCookie(string key)
        {
            var context = HttpContext.Current;
            if (context.Request.Cookies[key] != null)
            {
                context.Request.Cookies.Remove(key);
            }
            if (context.Response.Cookies[key] != null)
            {
                context.Response.Cookies.Remove(key);
            }
        }
        /// <summary>
        /// 对cookie进行写入操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void WriteCookie(string key, string value)
        {
            var context = HttpContext.Current;
            RemoveCookie(key);
            context.Request.Cookies.Add(new HttpCookie(key, value));
            context.Response.Cookies.Add(new HttpCookie(key, value));
        }
        /// <summary>
        /// 针对OA系统传来的地址及参数校验登陆用户
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public bool ValidationWithOASSO(out string errorMsg, out string returnUrl)
        {
            returnUrl = "";
            var request = HttpContext.Current.Request;
            var param1 = request.QueryString["uid"];
            var param2 = request.QueryString["businessid"];
            var param3 = request.QueryString["signature"];
            var param4 = request.QueryString["returnurl"];
            if (string.IsNullOrEmpty(param1) || string.IsNullOrEmpty(param2) || string.IsNullOrEmpty(param3) || string.IsNullOrEmpty(param4))
            {
                errorMsg = "缺少必要参数";
                return false;
            }
            //if (request.UrlReferrer == null)
            //{
            //    errorMsg = "请求来源不正确";
            //    return false;
            //}
            //var refer = request.UrlReferrer.Host;
            //if (refer.IndexOf(ConfigurationManager.AppSettings["OA.Host"], StringComparison.CurrentCultureIgnoreCase) < 0)
            //{
            //    errorMsg = "请求来源不正确";
            //    return false;
            //}
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("uid", param1);
            dict.Add("businessid", param2);
            dict.Add("returnurl", HttpUtility.UrlEncode(param4));
            var sign = SSOToolkit.GetSignature(dict);
            //todo 这里还要验证签名
            if (!sign.Equals(param3, StringComparison.CurrentCultureIgnoreCase))
            {
                errorMsg = "获取到的客户端数据同实际签名不符";
                return false;
            }
            errorMsg = "";
            returnUrl = param4;

            var loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var expiryMinutes = (24 * 60).ToString();

            var accessToken = Guid.NewGuid().ToString() + "_t";

            dict = new Dictionary<string, string>();
            dict.Add("accessToken", accessToken);
            dict.Add("loginId", param1);
            dict.Add("loginTime", loginTime);
            dict.Add("expiryMinutes", expiryMinutes);

            WriteCookie("accessToken", accessToken);
            WriteCookie("loginId", param1);
            WriteCookie("loginTime", loginTime);
            WriteCookie("expiryMinutes", expiryMinutes);
            WriteCookie("signature", GetSignature(dict));
            return true;
        }
        /// <summary>
        /// 从OA首页点击进来的登陆验证
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ValidationWithOAMainSSO(out string errorMsg)
        {
            var request = HttpContext.Current.Request;
            var param1 = request.QueryString["sso_loginid"];
            var param2 = request.QueryString["sso_pwd"];
            if (string.IsNullOrEmpty(param1) || string.IsNullOrEmpty(param2))
            {
                errorMsg = "缺少必要参数";
                return false;
            }
            //if (request.UrlReferrer == null)
            //{
            //    errorMsg = "请求来源不正确";
            //    return false;
            //}
            //var refer = request.UrlReferrer.Host;
            //if (refer.IndexOf(ConfigurationManager.AppSettings["OA.Host"], StringComparison.CurrentCultureIgnoreCase) < 0)
            //{
            //    errorMsg = "请求来源不正确";
            //    return false;
            //}


            var loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var expiryMinutes = (24 * 60).ToString();

            var accessToken = Guid.NewGuid().ToString() + "_t";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("accessToken", accessToken);
            dict.Add("loginId", param1);
            dict.Add("loginTime", loginTime);
            dict.Add("expiryMinutes", expiryMinutes);

            WriteCookie("accessToken", accessToken);
            WriteCookie("loginId", param1);
            WriteCookie("loginTime", loginTime);
            WriteCookie("expiryMinutes", expiryMinutes);
            WriteCookie("signature", GetSignature(dict));
            errorMsg = null;
            return true;
        }
        /// <summary>
        /// 对字符串进行base64解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string DecryptBase64(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            try
            {
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 验证客户端授权
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        internal bool ValidationAuth(out string loginId)
        {
            try
            {
                loginId = string.Empty;
                var request = HttpContext.Current.Request;
                if (request.Cookies["loginId"] == null || request.Cookies["loginTime"] == null || request.Cookies["expiryMinutes"] == null || request.Cookies["accessToken"] == null || request.Cookies["signature"] == null)
                {
                    //名称为user的cookie是手机端的cookie  如果没有pc端cookie 则检查手机端cookie
                    if (request.Cookies["user"] == null)
                    {
                        loginId = null;
                        logger.Info("缺少必要Cookie");
                        return false;
                    }
                    var mobileUser = request.Cookies["user"].Value.ToString();
                    var encryptUser = mobileUser.Substring(10);
                    var userName = DecryptBase64(encryptUser);
                    if (string.IsNullOrEmpty(userName))
                    {
                        loginId = null;
                        logger.Info("手机端cookie检测失败");
                        return false;
                    }
                    loginId = userName;
                    return true;
                }


                loginId = request.Cookies["loginId"].Value;
                var loginTime = request.Cookies["loginTime"].Value;
                var expiryMinutes = request.Cookies["expiryMinutes"].Value;
                var accessToken = request.Cookies["accessToken"].Value;
                var signature = request.Cookies["signature"].Value;

                var dict = new Dictionary<string, string>();
                dict.Add("accessToken", accessToken);
                dict.Add("loginId", loginId);
                dict.Add("loginTime", loginTime);
                dict.Add("expiryMinutes", expiryMinutes);

                var sign = GetSignature(dict);
                if (!sign.Equals(signature))
                {
                    loginId = null;
                    logger.Info("获取到的客户端数据同实际签名不符");
                    return false;
                }
                if (Convert.ToDateTime(loginTime).AddMinutes(Convert.ToInt32(expiryMinutes)) < DateTime.Now)
                {
                    loginId = null;
                    logger.Info("登陆状态已过期");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {

                loginId = null;
                logger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 按指定数据进行MD5签名
        /// </summary>
        /// <param name="loginId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static string GetSignature(Dictionary<string, string> dict)
        {
            dict.Add("privatekey", ConfigurationManager.AppSettings["OA.PrivateKey"]);
            var sign = string.Join(",", dict.OrderBy(x => x.Key).Select(x => string.Format("{0}={1}", x.Key, x.Value)));
            return MD5Encrypt(sign);
        }
        /// <summary>
        /// 生成md5加密字符串
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        private static string MD5Encrypt(string input)
        {
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }


        /// <summary>
        /// 从手机端应用跳转过来进任务列表
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ValidationWithMobileAppMainSSO(out string errorMsg)
        {
            var request = HttpContext.Current.Request;
            var param1 = request.Cookies["user"].Value;
            if (string.IsNullOrEmpty(param1) || param1.Length < 11)
            {
                errorMsg = "获取登陆人信息失败";
                return false;
            }
            param1 = DecryptBase64(param1.Substring(10));
            //if (request.UrlReferrer == null)
            //{
            //    errorMsg = "请求来源不正确";
            //    return false;
            //}
            //var refer = request.UrlReferrer.Host;
            //if (refer.IndexOf(ConfigurationManager.AppSettings["OA.Host"], StringComparison.CurrentCultureIgnoreCase) < 0)
            //{
            //    errorMsg = "请求来源不正确";
            //    return false;
            //}


            var loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var expiryMinutes = (24 * 60).ToString();

            var accessToken = Guid.NewGuid().ToString() + "_t";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("accessToken", accessToken);
            dict.Add("loginId", param1);
            dict.Add("loginTime", loginTime);
            dict.Add("expiryMinutes", expiryMinutes);

            WriteCookie("accessToken", accessToken);
            WriteCookie("loginId", param1);
            WriteCookie("loginTime", loginTime);
            WriteCookie("expiryMinutes", expiryMinutes);
            WriteCookie("signature", GetSignature(dict));
            errorMsg = null;
            return true;
        }
    }
}
