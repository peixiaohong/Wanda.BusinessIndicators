using Lib.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class ComprehensiveReportForms : System.Web.UI.Page
    {

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
                 
                    //获取Tree数据
                    string Sys_str = "'" + string.Join("','", sysList.Distinct().ToList().Select(S => S.ID).ToList()) + "'";
                    var TreeData = C_SystemTreeOperator.Instance.GetSystemTreeData(Sys_str);

                    TreeDataJson = JsonConvert.SerializeObject(TreeData);
                }
                else
                {
                    string Sys_str = "'" + string.Join("','", StaticResource.Instance.SystemList.ToList().Select(S => S.ID).ToList()) + "'";
                    var TreeData = C_SystemTreeOperator.Instance.GetSystemTreeData(Sys_str);
                    TreeDataJson = JsonConvert.SerializeObject(TreeData);
                }
                
            }
        }




        List<string> list = null;
        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                list = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(list == null, "Argument GetStartProcessList is Empty");
            }
        }


    }
}