using LJTH.BusinessIndicators.Common;
using LJTH.Lib.AuthCenter.BLL;
using System.Web.Http;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebHelper.Instance.GetUser += UserinfoOperator.Instance.GetUserInfoByName;
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
