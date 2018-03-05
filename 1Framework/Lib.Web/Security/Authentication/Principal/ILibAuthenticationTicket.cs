using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web.Security
{
    public interface ILibAuthenticationTicket
    {
        #region Field

        /// <summary>
        /// 登录的SessionID
        /// </summary>
        string LoginSessionID
        {
            get;
            set;
        }

        /// <summary>
        /// 登录客户端IP
        /// </summary>
        string LoginIP
        {
            get;
        }

        /// <summary>
        /// 认证服务器的域名(或者IP)
        /// </summary>
        string AuthenticateServer
        {
            get;
            set;
        }

        /// <summary>
        /// 登录的时间
        /// </summary>
        DateTime LoginTime
        {
            get;
        }

        /// <summary>
        /// 登录的过期时间
        /// </summary>
        DateTime LoginTimeout
        {
            get;
            set;
        }

        bool IsPersistent
        {
            get;
        }

        bool Expired
        {
            get;
        }

        string Name
        {
            get;
        }

        bool WindowsIntegrated
        {
            get;
        }

        ILoginUser LoginUser
        {
            get;
        }

        #endregion

        #region Method

        /// <summary>
        /// 将对象信息序列化成string类型
        /// </summary>
        string Serialize();

        /// <summary>
        /// 将数据反序列化成对象
        /// </summary>
        /// <param name="dataString"></param>
        ILibAuthenticationTicket Deserialize(string dataString);

        /// <summary>
        /// 登录信息持久化
        /// </summary>
        /// <returns></returns>
        int Persist();

        #endregion
    }
}
