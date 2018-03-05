using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetApprove : System.Web.UI.Page//,IActivityHost
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["BusinessID"] != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"].ToString()))
                {
                    hideMonthReportID.Value = Request.QueryString["BusinessID"].ToString();
                    B_MonthlyReport bmonthReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(Guid.Parse(hideMonthReportID.Value));
                    if (bmonthReport != null)
                    {
                        hideSystemID.Value = bmonthReport.SystemID.ToString();
                        hideFinYear.Value = bmonthReport.FinYear.ToString();
                        hideFinMonth.Value = bmonthReport.FinMonth.ToString();
                        HideProcessCode.Value = StaticResource.Instance[bmonthReport.SystemID,DateTime.Now].Configuration.Element("ProcessCode").Value;
                        C_System system=StaticResource.Instance.SystemList.Where(p => p.ID == bmonthReport.SystemID).FirstOrDefault();
                        if (system != null)
                        {
                            lblName.Text = system.SystemName+bmonthReport.FinYear + "-" + bmonthReport.FinMonth + lblName.Text;
                        }
                    }
                }
            }
        }
        
        //public string BusinessID
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public string CurrentUser
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public string OperaionInfo
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public string Recovery()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Resident(string processJSON)
        //{
        //    throw new NotImplementedException();
        //}
    }
}