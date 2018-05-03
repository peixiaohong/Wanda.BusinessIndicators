using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string Origins = System.Configuration.ConfigurationManager.AppSettings["cors.Origins"];
            string Headers = System.Configuration.ConfigurationManager.AppSettings["cors.Headers"];
            string Methods = System.Configuration.ConfigurationManager.AppSettings["cors.Methods"];
            //跨域配置
            config.EnableCors(new EnableCorsAttribute(Origins, Headers, Methods));
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
