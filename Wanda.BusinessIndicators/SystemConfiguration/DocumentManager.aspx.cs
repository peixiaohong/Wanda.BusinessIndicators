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
    public partial class DocumentManager : System.Web.UI.Page
    {

        List<string> PermissionList = null;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

                
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();//绑定前台下拉框


                DocMutipleUpload.LoadByBusinessID("");
            }
        }

        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SysID = ddlSystem.SelectedValue;
        }
    }
}