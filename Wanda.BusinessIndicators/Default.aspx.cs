using Lib.Config;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Configuration;
using System.Web.Security;
using Wanda.Lib.AuthCenter.BLL;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.AuthCenter.ViewModel;
using Wanda.BusinessIndicators.Common;

namespace Wanda.BusinessIndicators.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        private bool _enabledSSO = true;

        public bool EnabledSSO
        {

            get
            {
                string enabledSSO = System.Configuration.ConfigurationManager.AppSettings["EnabledSSO"] ?? "true";
                bool.TryParse(enabledSSO, out _enabledSSO);
                return _enabledSSO;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            if (EnabledSSO)
            {
            }
            else
            {
                if (Request.Url.AbsolutePath.Contains(ConstSet.URL_LOGIN) == false)
                {
                    HttpContext.Current.Response.Redirect(ConstSet.URL_LOGIN + "?ReturnUrl=" +
                        HttpUtility.UrlEncode(Request.RawUrl));
                }
            }

            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoginUserInfo user = WebHelper.GetCurrentUser(false);
                if (user!=null)
                {
                        string returnUrl = string.Empty;
                        if (string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                        {
                            Response.Redirect(ConstSet.DEFAULT_URL, true);
                        }
                        returnUrl = Request.QueryString["ReturnUrl"];
                      
                        Response.Redirect(ConstSet.DEFAULT_URL, true);
                }
                else
                {
                    #region
                    if (EnabledSSO)
                    {
                        string action = Request.QueryString["action"];
                        if (Request.Url.ToString().Contains("wd_sso_logout") && !string.IsNullOrEmpty(action) && action == "goto")
                        {
                            string url = Request.QueryString["url"];
                            Response.Redirect(url, true);
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 获取输入值的MD5值
        /// </summary>
        /// <param name="input">输入数据</param>
        /// <returns></returns>
        private string MSGetMd5Hash(string input)
        {
            MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 验证输入数据的合法性
        /// </summary>
        /// <param name="input">输入数据</param>
        /// <param name="hash">比对数据</param>
        /// <returns></returns>
        private bool VerifyMd5Hash(string input, string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                string hashOfInput = MSGetMd5Hash(input);
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else {
                return true;
            }            
        }

     
    }
}