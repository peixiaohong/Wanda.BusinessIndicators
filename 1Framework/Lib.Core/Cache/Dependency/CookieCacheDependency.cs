using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Lib.Core;

namespace Lib.Cache
{
    /// <summary>
    /// 基于Cookie的Cache依赖项
    /// </summary>
    public class CookieCacheDependency : DependencyBase
    {
        HttpCookie httpCookie = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cookie">cookie</param>
        public CookieCacheDependency(HttpCookie cookie)
        {
            if (cookie == null)
                throw new ArgumentNullException("cookie");

            HttpContext.Current.Request.Cookies.Add(cookie);
            httpCookie = cookie;
        }

        /// <summary>
        /// 是否发生改变
        /// </summary>
        public override bool HasChanged
        {
            get
            {
                bool result = false;

                if (EnvironmentHelper.Mode == InstanceMode.Web && CacheManager.InScavengeThread == false)
                    result = CookieIsInvalid();

                return result;
            }
        }

        /// <summary>
        /// 最后修改日期
        /// </summary>
        public override DateTime UtcLastModified
        {
            get
            {
                return base.UtcLastModified;
            }
            set
            {
                if (EnvironmentHelper.Mode == InstanceMode.Web && CacheManager.InScavengeThread == false)
                {
                    HttpResponse response = HttpContext.Current.Response;

                    response.Cookies.Add(httpCookie);
                }

                base.UtcLastModified = value;
            }
        }

        private bool CookieIsInvalid()
        {
            HttpRequest request = HttpContext.Current.Request;

            HttpCookie cookieReq = request.Cookies[httpCookie.Name];

            return cookieReq == null;
        }
    }
}
