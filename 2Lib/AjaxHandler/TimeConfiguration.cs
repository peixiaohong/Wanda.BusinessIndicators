using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.BusinessIndicators.Web;

namespace Wanda.BusinessIndicators.Web.AjaxHandler
{
    public class TimeConfiguration : BaseController
    {
        [LibAction]
        public C_ReportTime GetReportTime() 
        {
            return C_ReportTimeOperator.Instance.GetReportTime();
        }

        [LibAction]
        public Guid UpdateReportTime(string time, string status, string openday)
        {
            C_ReportTime model = GetReportTime();

            if (status == "1")
            {
                if (openday != "" && time !="")
                {
                  model.SysOpenDay = DateTime.Parse(openday);
                  model.WantTime = DateTime.Parse(time);
                }
            }
            else if (status == "2")
            {
                if (time != "")
                {
                    model.ReportTime = DateTime.Parse(time);
                }
            }
            else
            {
                model.ReportTime = null;
                model.SysOpenDay = null;
                model.WantTime = null;
            }
            model.OpenStatus = status;
            return C_ReportTimeOperator.Instance.UpdateReportTime(model);
        }

        /// <summary>
        /// 获取上报日期
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public DateTime GetReportDateTime() 
        {
            DateTime time = StaticResource.Instance.GetReportDateTime();
            return time;
        }
    }
}
