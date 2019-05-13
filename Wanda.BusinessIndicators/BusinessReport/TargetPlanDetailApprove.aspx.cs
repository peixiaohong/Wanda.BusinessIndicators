using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetPlanDetailApprove : System.Web.UI.Page
    {

        public string SysDataJson
        {
            get
            {
                if (ViewState["SysDataJson"] == null)
                {
                    return null;
                }
                return ViewState["SysDataJson"].ToString();
            }
            set
            {
                ViewState["SysDataJson"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["BusinessID"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"].ToString()))
                    {
                        //if (ConfigurationManager.AppSettings["HideProcessCode"] != null)
                        //{
                        //    HideProcessCode.Value = ConfigurationManager.AppSettings["HideProcessCode"];
                        //}
                        hideTargetPlanID.Value = Request.QueryString["BusinessID"].ToString();
                        B_TargetPlan _BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByID(Guid.Parse(hideTargetPlanID.Value));
                        if (_BTargetPlan != null)
                        {
                            HideProcessCode.Value = StaticResource.Instance[_BTargetPlan.SystemID, DateTime.Now].Configuration.Element("ProcessCode").Value;

                            hideSystemID.Value = _BTargetPlan.SystemID.ToString();
                            hideFinYear.Value = _BTargetPlan.FinYear.ToString();

                            C_System system = StaticResource.Instance[_BTargetPlan.SystemID, DateTime.Now];

                            SysDataJson = JsonConvert.SerializeObject(system);

                            if (system != null)
                            {
                                lblName.Text = _BTargetPlan.FinYear + "年" + system.SystemName + "指标分解";
                                if (!string.IsNullOrEmpty(_BTargetPlan.VersionName))
                                    lblName.Text += "-" + _BTargetPlan.VersionName;
                            }
                            lblVersionName2.Text = _BTargetPlan.VersionName;
                            lblVersionName1.Text = _BTargetPlan.VersionName;
                            hideVersionName.Value= _BTargetPlan.VersionName;
                        }
                    }
                }
            }
        }
    }
}