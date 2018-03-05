using System;
using System.Web;
using Lib.Web.Json;

namespace Lib.Web.Security
{
    public class LibAuthenticationTicket : ILibAuthenticationTicket
    {
        #region Private

        private string name;
        private string loginSessionID;
        private string loginIP;
        private string authenticateServer;
        private DateTime loginTime;
        private DateTime loginTimeout;
        private bool isPersistent;
        private bool windowsIntegrated;

        private ILoginUser loginUser;

        #endregion Private

        #region Public Field

        /// <summary>
        /// 登录的SessionID
        /// </summary>
        public string LoginSessionID
        {
            get { return this.loginSessionID; }
            set { this.loginSessionID = value; }
        }

        /// <summary>
        /// 登录客户端IP
        /// </summary>
        public string LoginIP
        {
            get { return this.loginIP; }
        }

        /// <summary>
        /// 认证服务器的域名(或者IP)
        /// </summary>
        public string AuthenticateServer
        {
            get { return this.authenticateServer; }
            set { this.authenticateServer = value; }
        }

        /// <summary>
        /// 登录的时间
        /// </summary>
        public DateTime LoginTime
        {
            get { return this.loginTime; }
        }

        /// <summary>
        /// 登录的过期时间
        /// </summary>
        public DateTime LoginTimeout
        {
            get { return this.loginTimeout; }
            set { this.loginTimeout = value; }
        }

        public bool IsPersistent
        {
            get { return this.isPersistent; }
        }

        public bool Expired
        {
            get { return this.LoginTimeout < DateTime.Now; }
        }
        
        public string Name
        {
            get { return this.name; }
        }

        public bool WindowsIntegrated
        {
            get { return this.windowsIntegrated; }
        }
        
        public ILoginUser LoginUser
        {
            get { return this.loginUser; }
        }

        #endregion

        #region Counstruct

        public LibAuthenticationTicket()
        { }

        public LibAuthenticationTicket(ILibAuthenticationTicket ticket)
        {
            this.name = ticket.Name;
            this.loginSessionID = ticket.LoginSessionID;
            this.loginIP = ticket.LoginIP;
            this.authenticateServer = ticket.AuthenticateServer;
            this.loginTime = ticket.LoginTime;
            this.LoginTimeout = LibAuthentication.SlidingExpiration ? DateTime.Now.AddMinutes(LibAuthentication.CookieTimeout) : ticket.LoginTimeout;
            
            this.isPersistent = ticket.IsPersistent;
            this.loginUser = ticket.LoginUser;

        }

        public LibAuthenticationTicket(ILoginUser loginUser, bool isPersistent)
        {
            this.name = loginUser.LoginName;
            this.loginSessionID = Guid.NewGuid().ToString();
            this.loginIP = HttpContext.Current.Request.Url.Host;
            //this.authenticateServer = 
            this.loginTime = DateTime.Now;
            this.loginTimeout = this.loginTime.AddMinutes((double)LibAuthentication.CookieTimeout);

            this.isPersistent = isPersistent;
            this.loginUser = loginUser;            
        }

        public LibAuthenticationTicket(string name, DateTime loginTime, DateTime loginTimeout, bool isPersistent, ILoginUser loginUser)
        {
            this.name = name;
            this.loginTime = loginTime;
            this.loginTimeout = loginTimeout;
            this.isPersistent = isPersistent;
            this.loginUser = loginUser;

            this.loginSessionID = Guid.NewGuid().ToString();
            this.loginIP = HttpContext.Current.Request.Url.Host;
        }

        public LibAuthenticationTicket(string serializeString)
        { 
        
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 将对象信息序列化成string类型
        /// </summary>
        public string Serialize()
        {
            return JsonHelper.Serialize(this);
        }

        /// <summary>
        /// 将数据反序列化成对象
        /// </summary>
        /// <param name="dataString"></param>
        public ILibAuthenticationTicket Deserialize(string dataString)
        {
             return JsonHelper.Deserialize<LibAuthenticationTicket>(dataString);
        }

        /// <summary>
        /// 登录信息持久化
        /// </summary>
        /// <returns></returns>
        public int Persist()
        {
            return 0;
        }

        #endregion

    }
}
