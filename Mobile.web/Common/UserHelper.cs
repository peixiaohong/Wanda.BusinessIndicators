using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Mobile.web.Common
{
    public static class UserHelper
    {
        public static string GetUserName()
        {
            try
            {

                var request = HttpContext.Current.Request;
                var param1 = "";
                if(string.IsNullOrEmpty(param1))
                {
                    if (HttpContext.Current.User != null)
                        param1 = HttpContext.Current.User.Identity.Name;
                }
                if(string.IsNullOrEmpty(param1))
                {
                    param1 = HttpContext.Current.Request.Cookies["loginId"].ToString() ;
                }
                if(string.IsNullOrEmpty(param1))
                {
                    if (HttpContext.Current.Request.Cookies["user"] != null)
                    {
                        var t = HttpContext.Current.Request.Cookies["user"].Value;
                        if (!string.IsNullOrEmpty(t) && t.Length < 11)
                        {
                            param1 = DecryptBase64(t.Substring(10));
                        }
                    }
                }

                //if (string.IsNullOrEmpty(param1) || param1.Length < 11)
                //{
                //    return "";
                //}
                return param1;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 对字符串进行base64解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string DecryptBase64(string input)
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
    }
}