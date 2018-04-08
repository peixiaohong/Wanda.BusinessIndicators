using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Plugin.SSO
{
    public class AuthModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.AuthorizeRequest += Context_AuthorizeRequest;
        }


        /// <summary>
        /// 排除验证的目录
        /// </summary>
        private static string[] excludeFolders = new string[] { "Public" };
        /// <summary>
        /// 排除验证的文件
        /// </summary>
        private static string[] excludeFiles = new string[] { };
        /// <summary>
        /// 排除验证的文件后缀名
        /// </summary>
        private static string[] fileTypes = new string[] { ".aspx", ".ashx", "" };
        /// <summary>
        /// 验证失败后不进行重定向的目录
        /// </summary>
        private static string[] excludeRedirectFolders = new string[] { "WebAPI" };
        /// <summary>
        /// 登陆页面
        /// </summary>
        private static string LoginPage = "/Public/Login.aspx";

        private void Context_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            string pagePath = context.Request.Url.AbsolutePath;
            //if (pagePath == "/"||string.IsNullOrEmpty(pagePath))
            //{
            //    pagePath = "/Application/Task/TaskList.aspx";
            //}

            if (NeedAuthorizationCheck(pagePath) == false) //不需要检查
            {
                return;
            }
            string loginId = string.Empty;
            //if (SSOToolkit.Instance.ValidationAuthWithSSO(out loginId))
            //{
            //    context.Items.Add("LoginIdentity", loginId);
            //    return;
            //}
            var result = SSOToolkit.Instance.ValidationAuth(out loginId);
            if (result && !string.IsNullOrEmpty(loginId))
            {
                context.Items.Add("LoginIdentity", loginId);
                SSOClaimsIdentity claimsIdentity = new SSOClaimsIdentity
                {
                    UserName = loginId

                };
                SSOClaimsPrincipal claimsPrincipal = new SSOClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
                Thread.CurrentPrincipal = claimsPrincipal;
            }
            else
            {
                //根据路径判断是否需要进行重定向到登陆页面的操作
                if (NeedAuthorizationRedirect(pagePath))
                {
                    //重定向
                    context.Response.Redirect(string.Format("{0}?returnUrl={1}", LoginPage, context.Server.UrlEncode(context.Request.Url.PathAndQuery)));
                }
            }
        }
        /// <summary>
        /// 是否需要重定向到登陆页面
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        private bool NeedAuthorizationRedirect(string pagePath)
        {
            char[] delimiters = "/\\".ToCharArray();
            string path = pagePath.TrimStart(delimiters);
            string[] parts = path.Split(delimiters);
            // 不包括的路径
            if (excludeFolders.Contains(parts[0], StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 是否需要进行权限验证
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        private bool NeedAuthorizationCheck(string pagePath)
        {
            char[] delimiters = "/\\".ToCharArray();
            string path = pagePath.TrimStart(delimiters);
            string[] parts = path.Split(delimiters);
            // 不包括的路径
            if (excludeFolders.Contains(parts[0], StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }
            //根路径下不包括的文件
            if (excludeFiles.Contains(parts[parts.Length - 1], StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }
            // 非指定的文件
            string fileExtension = System.IO.Path.GetExtension(path);
            if (fileTypes.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase) == false)
            {
                return false;
            }

            return true;
        }
    }
}
