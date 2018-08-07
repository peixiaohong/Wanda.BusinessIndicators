using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Plugin.Filters;

namespace OAWebAPI.Controllers
{
    [WebApiExceptionFilter]
    [WebApiActionFilter]
    public class BaseController : ApiController
    {
    }
}
