using System;

namespace LJTH.BusinessIndicators.Web
{
    public partial class EmptyMaster : System.Web.UI.MasterPage
    {


        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();


        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}