using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace Wanda.BusinessIndicators.Web.MobileCommon
{
    /// <summary>
    /// 移动端安全头
    /// </summary>
    public class CredentialSoapHeader : SoapHeader
    {
        public CredentialSoapHeader()
        {
        }

        public CredentialSoapHeader(string userID, string password)
        {
            _userid = userID;
            _password = password;
        }
        private string _userid = string.Empty;
        private string _password = string.Empty;
        private string configUserID = System.Configuration.ConfigurationManager.AppSettings["MobileUser"];
        private string configPwd = System.Configuration.ConfigurationManager.AppSettings["MobilePWD"];
        public string errinfo = "头信息验证失败";
        public string UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public bool IsValid()
        {
            string Msg = string.Empty;
            return IsValid(_userid, _password, out Msg);
        }
        public bool IsValid(out string Msg)
        {
            return IsValid(_userid, _password, out Msg);
        }
        public bool IsValid(string nUserID, string nPassword, out string Msg)
        {

            Msg = "";
            return true;
            bool blResult = false;
            try
            {
                if (nUserID == configUserID && nPassword == configPwd)
                {
                    blResult = true;
                }
                else
                {
                    Msg = errinfo;
                    blResult = false;
                }
                return blResult;
            }
            catch (Exception e)
            {
                //"对不起你无权调用此WebService,可能的原因如下：1、你的帐号管理员禁用了。2、您的帐号的密码不正确！<br/>"
                Msg = errinfo + e.Message;
                return false;
            }


        }
    }
}