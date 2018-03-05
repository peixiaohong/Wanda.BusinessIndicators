using Lib.Log;
using Lib.Web.Json;
using Lib.Web.MVC;
using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace Lib.Web.Download
{
    public class ExcelHttpHandler : IHttpHandler, IRequiresSessionState
    {
        public virtual bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {

             ExcelGenerator.GenerateExcel(context);



        }






    }
}
