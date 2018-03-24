using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Lib.Core;

namespace LJTH.BusinessIndicators.Web.SystemConfiguration
{
    public partial class TargetPlanDetailReported : System.Web.UI.Page
    {
        private int finMonth;
        private int finYear;
        private string sysID;
       


        public string TreeDataJson
        {
            get
            {
                if (ViewState["TreeDataJson"] == null)
                {
                    return null;
                }
                return ViewState["TreeDataJson"].ToString();
            }
            set
            {
                ViewState["TreeDataJson"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["_sysid"]))
            {
                sysID = HttpUtility.UrlDecode(Request.QueryString["_sysid"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["_finMonth"]))
            {
                finMonth = int.Parse(HttpUtility.UrlDecode(Request.QueryString["_finMonth"]));
            }

            if (!string.IsNullOrEmpty(Request.QueryString["_finYear"]))
            {
                finYear = int.Parse(HttpUtility.UrlDecode(Request.QueryString["_finYear"]));
            }

            if (finMonth == 0 && finYear == 0)
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                finMonth = datetime.Month;
                finYear = datetime.Year;
            }

            List<C_System> sysList = new List<C_System>();
            if (!IsPostBack)
            {
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                    }
                }

                if (sysList.Count > 0)
                {
                    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();

                    //获取Tree数据
                    string Sys_str = "'" + string.Join("','", sysList.Distinct().ToList().Select(S => S.ID).ToList()) + "'";
                    var TreeData = C_SystemTreeOperator.Instance.GetSystemTreeData(Sys_str);
                    TreeDataJson = JsonConvert.SerializeObject(TreeData);
                }
                else
                {
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                    string Sys_str = "'" + string.Join("','", StaticResource.Instance.SystemList.ToList().Select(S => S.ID).ToList()) + "'";
                    var TreeData = C_SystemTreeOperator.Instance.GetSystemTreeData(Sys_str);
                    TreeDataJson = JsonConvert.SerializeObject(TreeData);
                }

                //if (sysList.Count > 0)
                //{
                //    ddlSystem.DataSource = sysList.Distinct().OrderBy(or => or.Sequence).ToList();
                //}
                //else
                //{
                //    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                //}
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
                ddlSystem.Items.Insert(0, new ListItem("请选择", "all"));
                if (!string.IsNullOrEmpty(sysID))
                {
                    ddlSystem.SelectedValue = sysID;
                }

                if (finMonth == 0 && finYear == 0)
                {
                    DateTime datetime = StaticResource.Instance.GetReportDateTime();
                    finMonth = datetime.Month;
                    finYear = datetime.Year;
                }
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();

                ddlYear.SelectedValue = finYear.ToString();
            }
        }



        List<string> list = null;
        List<string> list1 = null;
        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                list = PermissionHelper.GetStartProcessList();
                list1 = PermissionHelper.Getsubmanage();
                ExceptionHelper.TrueThrow<ArgumentNullException>(list == null, "Argument GetStartProcessList is Empty");
            }
        }

    }
}