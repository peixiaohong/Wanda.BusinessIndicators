using Lib.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class ProMonthReport : System.Web.UI.Page
    {
        private string sysID ;
        private int finMonth;
        private int finYear;


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
                if (!string.IsNullOrEmpty(Request.QueryString["IsLastestVersion"]))
                {
                    chkIsLastestVersion.Checked = bool.Parse(HttpUtility.UrlDecode(Request.QueryString["IsLastestVersion"]));
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
                //    ddlSystem.DataSource = sysList.Distinct().OrderBy(or => or.Sequence).ToList();
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
                if (!string.IsNullOrEmpty(sysID))
                {
                    ddlSystem.SelectedValue = sysID;
                }


                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();
              
                    ddlYear.SelectedValue = finYear.ToString();
         


                List<int> Month = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    Month.Add(i);
                }
                ddlMonth.DataSource = Month;
                ddlMonth.DataBind();

                    ddlMonth.SelectedValue = finMonth.ToString();
     
                //setMonthReportSearch();
                detailhidden.Value = "hide";
                if (list1 != null)
                {
                    SubManage();
                }
            }
        }


        List<string> list = null;
        List<string> list1 = null;
        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
            
            if (PermissionHelper.EnablePermission)
            {
                list1 = PermissionHelper.Getsubmanage();
                list = PermissionHelper.GetStartProcessList();

                ExceptionHelper.TrueThrow<ArgumentNullException>(list == null, "Argument GetStartProcessList is Empty");
            }
        }

        /// <summary>
        /// 管理上报日志的显示隐藏
        /// </summary>
        private void SubManage()
        {
            if (list1!=null&&list1.Count>0)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (ddlSystem.SelectedItem.Text == list1[i])
                    {
                        detailhidden.Value = "show";
                    }
                }
            }
        }
        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            SubManage();
            //系统下拉
            TransferOtherPage(ddlSystem.SelectedValue.ToGuid());
        
        }
        public void TransferOtherPage(Guid SystemID)
        {
            Guid sysID = SystemID;


            if (ddlMonth.SelectedValue == "")
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                finMonth = datetime.Month;
                finYear = datetime.Year;
            }
            else
            {
                finMonth = int.Parse(ddlMonth.SelectedValue);

                //年下拉
                finYear = int.Parse(ddlYear.SelectedValue);

            }

            C_System sysModel = StaticResource.Instance[sysID, DateTime.Now];

            if (chkIsLastestVersion.Checked)
            {
                B_MonthlyReport B_monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyReport(sysID, finYear, finMonth);
                if (B_monthRpt != null)
                {
                    sysModel = StaticResource.Instance[sysID, B_monthRpt.CreateTime];
                }
            }
            else
            {
                A_MonthlyReport A_monthRpt = A_MonthlyreportOperator.Instance.GetAMonthlyReport(sysID, finYear, finMonth);
                if (A_monthRpt != null)
                {
                    sysModel = StaticResource.Instance[sysID, A_monthRpt.CreateTime];
                }
            }





            if (sysModel.Category == 1)
            {
                Response.Redirect("TargetRpt.aspx?_sysid=" + HttpUtility.UrlEncode(sysID.ToString()) + "&_finMonth=" + HttpUtility.UrlEncode(finMonth.ToString()) + "&_finYear=" + HttpUtility.UrlEncode(finYear.ToString()) + "&IsLastestVersion=" + chkIsLastestVersion.Checked);

            }
            else if (sysModel.Category == 2)
            {
                return;
            }
            else if (sysModel.Category == 3)
            {
                Response.Redirect("TargetGroupRpt.aspx?_sysid=" + HttpUtility.UrlEncode(sysID.ToString()) + "&_finMonth=" + HttpUtility.UrlEncode(finMonth.ToString()) + "&_finYear=" + HttpUtility.UrlEncode(finYear.ToString()) + "&IsLastestVersion=" + chkIsLastestVersion.Checked);

            }
            else if (sysModel.Category == 4)
            {
                Response.Redirect("TargetDirectlyRpt.aspx?_sysid=" + HttpUtility.UrlEncode(sysID.ToString()) + "&_finMonth=" + HttpUtility.UrlEncode(finMonth.ToString()) + "&_finYear=" + HttpUtility.UrlEncode(finYear.ToString()) + "&IsLastestVersion=" + chkIsLastestVersion.Checked);
            }
        }

        protected void ContentPlaceHolder1_LinkButton1_ServerClick(object sender, EventArgs e)
        {
            TransferOtherPage(ddlSystem.SelectedValue.ToGuid());
            SubManage();
        }


    }
}