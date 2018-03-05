using Lib.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Web.SystemConfiguration
{
    public partial class TargetCollectDisplay : System.Web.UI.Page
    {
        private string SystemID;


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
                  if (!string.IsNullOrEmpty(Request.QueryString["SystemID"]))
                  {
                      SystemID = HttpUtility.UrlDecode(Request.QueryString["SystemID"]);
                  }
                  if (list != null && list.Count > 0)
                  {
                      foreach (var item in list)
                      {
                          sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                      }
                  }
                //if (sysList.Count > 0)
                //{
                //    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                //}
                //else
                //{
                //    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                //}


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



                ddlSystem.DataTextField = "SystemName";
                  ddlSystem.DataValueField = "ID";
                  ddlSystem.DataBind();
                  if (!string.IsNullOrEmpty(Request.QueryString["SystemID"]))
                  {
                      ddlSystem.Items.FindByValue(SystemID).Selected = true;
                  }
                  List<int> Year = new List<int>();
                  for (int i = -5; i < 5; i++)
                  {
                      Year.Add(DateTime.Now.Year + i);
                  }
                  ddlYear.DataSource = Year;
                  ddlYear.DataBind();
                  ddlYear.SelectedValue = DateTime.Now.Year.ToString();
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
                if (list.Count > 0)
                {
                    C_System cSystem = new C_System();
                    if (!string.IsNullOrEmpty(Request.QueryString["SystemID"]))//看是否有传过来SystemID,如果有
                    {
                        SystemID = HttpUtility.UrlDecode(Request.QueryString["SystemID"]);
                        cSystem = StaticResource.Instance.SystemList.Where(p => p.ID == Guid.Parse(SystemID)).FirstOrDefault();

                    }
                    else
                    {
                        cSystem = StaticResource.Instance.SystemList.Where(p => p.SystemName == list.FirstOrDefault()).FirstOrDefault();
                   
                    }
                    if (cSystem != null)
                    {
                        TransferOtherPage(cSystem.ID);
                    }
                }
            }
        }

        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string systemid = ddlSystem.SelectedValue;

            TransferOtherPage(Guid.Parse(systemid));
        }

        public void TransferOtherPage(Guid SystemID)
        {
            Guid sysID = SystemID;
            int Year;
            if (ddlYear.SelectedValue == "")
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();

                Year = datetime.Year;
            }
            else
            {
                //年下拉
                Year = int.Parse(ddlYear.SelectedValue);
            }
            List<A_TargetPlanDetail> Detail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, Year).ToList();
            C_System sysModel = StaticResource.Instance[sysID, DateTime.Now];
            if (Detail.Count > 0)
            {
                sysModel = StaticResource.Instance[sysID, B_TargetplanOperator.Instance.GetTargetplan(Detail[0].TargetPlanID).CreateTime];
            }
            if (sysModel.Category == 3)
            {
                Response.Redirect("../BusinessReport/TargetPlanDetailRpt.aspx?SystemID=" + HttpUtility.UrlEncode(SystemID.ToString()));
            }

        }
    }
}