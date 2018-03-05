using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.Common;
using Wanda.Platform.Permission.ClientComponent;

namespace Wanda.BusinessIndicators.Web
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