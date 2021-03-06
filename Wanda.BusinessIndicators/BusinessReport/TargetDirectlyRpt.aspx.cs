﻿using Lib.Core;
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
    public partial class DirectlyMonthReport : System.Web.UI.Page
    {
        private string sysID;
        private int finMonth;
        private int finYear;
        public bool IsLastestVersion = false;

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
            if (!string.IsNullOrEmpty(Request.QueryString["IsLastestVersion"]))
            {
                IsLastestVersion = bool.Parse(HttpUtility.UrlDecode(Request.QueryString["IsLastestVersion"]));
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
                var organizationalList = BLL.BizBLL.S_OrganizationalActionOperator.Instance.GetUserSystemDataNoIsDelete(Common.WebHelper.GetCurrentLoginUser());

                var systemIdList = organizationalList.Select(m => m.SystemID).ToArray();

                chkIsLastestVersion.Checked = IsLastestVersion;

                var systemList = StaticResource.Instance.SystemList;

                if (systemIdList != null && systemIdList.Length > 0)
                {
                    sysList.AddRange(systemList.Where(p => systemIdList.Contains(p.ID)).ToList());

                    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();

                    //获取Tree数据
                    string Sys_str = "'" + string.Join("','", sysList.Distinct().ToList().Select(S => S.ID).ToList()) + "'";
                    var TreeData = C_SystemTreeOperator.Instance.GetSystemTreeData(Sys_str);

                    TreeDataJson = JsonConvert.SerializeObject(TreeData);
                }

                //if (sysList.Count > 0)
                //{
                //    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                //}
                //else
                //{
                //    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                //}

                //ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
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

                detailhidden.Value = "hide";
                //setMonthReportSearch();
                if (list1 != null)
                {
                    SubManage();

                }
                GetTargetVersionType(ddlSystem.SelectedValue.ToString(), finYear, finMonth, IsLastestVersion);
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
        private void SubManage()
        {

            if (list1!=null&&list1.Count > 0)
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
            TransferOtherPage(ddlSystem.SelectedValue.ToGuid());
            SubManage();
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
                Response.Redirect("ProMonthReport.aspx?_sysid=" + HttpUtility.UrlEncode(sysID.ToString()) + "&_finMonth=" + HttpUtility.UrlEncode(finMonth.ToString()) + "&_finYear=" + HttpUtility.UrlEncode(finYear.ToString()) + "&IsLastestVersion=" + chkIsLastestVersion.Checked);
            }
            else if (sysModel.Category == 3)
            {
                Response.Redirect("TargetGroupRpt.aspx?_sysid=" + HttpUtility.UrlEncode(sysID.ToString()) + "&_finMonth=" + HttpUtility.UrlEncode(finMonth.ToString()) + "&_finYear=" + HttpUtility.UrlEncode(finYear.ToString()) + "&IsLastestVersion=" + chkIsLastestVersion.Checked);
            }
            else if (sysModel.Category == 4)
            {
                return;
            }

        }

        protected void ContentPlaceHolder1_LinkButton1_ServerClick(object sender, EventArgs e)
        {
            TransferOtherPage(ddlSystem.SelectedValue.ToGuid());
            SubManage();
        }
        /// <summary>
        /// 获取指标版本数据
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="IsLatestVersion"></param>
        private void GetTargetVersionType(string sid, int year, int month, bool IsLatestVersion)
        {
            ddlVersionType.Items.Clear();
            //是否查询审批中数据
            if (IsLatestVersion)
            {
                var result = B_TargetplanOperator.Instance.GetTargetVersionType(sid, year, month);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        ddlVersionType.Items.Add(new ListItem(item.VersionName, item.ID.ToString()));
                    }
                }
            }
            else
            {
                var result = A_TargetplanOperator.Instance.GetTargetVersionType(sid, year, month);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        ddlVersionType.Items.Add(new ListItem(item.VersionName, item.ID.ToString()));
                    }
                }
            }
        }
    }
}