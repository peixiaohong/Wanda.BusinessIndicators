using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Lib.Core;
using Lib.Web.Properties;

namespace Lib.Web.Security
{
    public class LibPrincipal : LibPrincipal<LoginUser>, IPrincipal
    {
        public LibPrincipal(LibIdentity identity)
            : base(identity)
        { }

        /// <summary>
        /// 从当前线程中取得用户的登录和权限信息(Principal)
        /// </summary>
        public static LibPrincipal Current
        {
            get
            {
                LibPrincipal result = GetPrincipal();

                ExceptionHelper.FalseThrow<Exception>(result != null, Resource.CanNotGetPrincipal);

                return result;
            }
        }

        /// <summary>
        /// 是否经过认证
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return GetPrincipal() != null;
            }
        }

        private static LibPrincipal GetPrincipal()
        {
            LibPrincipal result = null;

            IPrincipal principal;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                principal = HttpContext.Current.User;
            else
                principal = Thread.CurrentPrincipal;

            if (principal != null && principal is LibPrincipal)
                result = (LibPrincipal)principal;

            return result;
        }
    }
}
