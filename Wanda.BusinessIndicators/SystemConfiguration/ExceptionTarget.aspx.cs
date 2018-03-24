using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.SystemConfiguration
{
    public partial class ExceptionTarget : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID=Request.QueryString["ID"];
                string CFrom = Request.QueryString["ComeFrom"];
                SysID.Value = ID;
                ComeFrom.Value = CFrom;
                List<C_System> sysList = new List<C_System>();
                if (PermissionList != null && PermissionList.Count > 0)
                {
                    foreach (var item in PermissionList)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                    }
                }
                if (sysList.Count > 0)
                {

                    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                }
                else
                {
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                }

                //sysList = StaticResource.Instance.SystemList.ToList();
                //ddlSystem.DataSource = sysList.OrderBy(or => or.Sequence).ToList();
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();//绑定前台下拉框
                //ddlSystem_TextChanged(ddlSystem.SelectedValue, null);
            }
        }
        List<string> PermissionList = null;
        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                PermissionList = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList == null, "Argument GetStartProcessList is Empty");
            }
        }
    }
}