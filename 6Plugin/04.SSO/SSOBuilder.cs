
using Plugin.Core;
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
    public static class SSOBuilder
    {
        /// <summary>
        /// 验证用户名密码是否正确
        /// </summary>
        /// <param name="userName">用户名（即账号）</param>
        /// <param name="userPwd">用户密码</param>
        /// <returns></returns>
        public static bool Validation(string userName, string userPwd)
        {
            try
            {
                var flag = ConfigurationManager.AppSettings["SSO.DEBUG"];
                if (string.IsNullOrEmpty(flag) || flag.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    var result = false;
                    using (SSO.WebServices.HrmUserService serivce = new SSO.WebServices.HrmUserService())
                    {
                        result = serivce.checkUser(userName, userPwd);
                    }
                    return result;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new SSOException("SSO接口调用异常", ex);
            }
        }
        /// <summary>
        /// 生成登陆成功的凭据
        /// </summary>
        /// <param name="userName">用户名（即账号）</param>
        /// <param name="userPwd">用户密码</param>
        /// <returns></returns>
        public static bool Authorization(string userName, string userPwd)
        {
            try
            {
                var flag = ConfigurationManager.AppSettings["SSO.DEBUG"];
                if (string.IsNullOrEmpty(flag) || flag.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    string accessToken = string.Empty;
                    using (SSO.WebServices.HrmUserService serivce = new SSO.WebServices.HrmUserService())
                    {
                        accessToken = serivce.getToken(userName, userPwd);
                    }
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        SSOToolkit.Instance.AuthLogin(userName, accessToken);
                    }
                    return true;
                }
                else
                {
                    SSOToolkit.Instance.AuthLogin(userName, Guid.NewGuid().ToString());
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw new SSOException("SSO接口调用异常", ex);
            }
        }


    }
}
