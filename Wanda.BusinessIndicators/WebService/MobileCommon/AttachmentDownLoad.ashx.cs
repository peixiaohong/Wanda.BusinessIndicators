using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wanda.Fiscal.Budget.Web.AjaxHander;

namespace Wanda.BusinessIndicators.Web.MobileCommon
{
    /// <summary>
    /// AttachmentDownLoad 的摘要说明
    /// </summary>
    public class AttachmentDownLoad : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            DownLoadFile Dlf = new DownLoadFile();
            Dlf.ProcessRequest(context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}