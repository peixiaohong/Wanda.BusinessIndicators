using Lib.Core;
using Lib.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.Platform.Permission.ClientComponent;

namespace Wanda.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetUpdate : System.Web.UI.Page
    {
        int FinYear;
         

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime datetime = new DateTime();
            datetime = StaticResource.Instance.GetReportDateTime();
            FinYear = datetime.Year;
            HideFinYear.Value = datetime.Year.ToString();
            lblName.Text = datetime.Year.ToString() + "年年计划指标上报";
            //JS_WF_JY_Starter
            if (!IsPostBack)
            {
                List<C_System> sysList = new List<C_System>();
                if (PermissionList != null && PermissionList.Count > 0)
                {
                    foreach (var item in PermissionList)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).Distinct().ToList());
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
                ddlSystem.DataBind();

                HidSystemID.Value = ddlSystem.SelectedValue;
           
                TargetPlanControl();//如果当前月不存在计划指标数据，添加一条数据
            }


      
        }

        public void TargetPlanControl()
        {
            HidSystemID.Value = ddlSystem.SelectedValue;
            HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value + "-ZB";
        

            B_TargetPlan BTargetPlan = null;
            //判断当前URL是否存在BusinessID
            if (string.IsNullOrEmpty(Request["BusinessID"]))
            {
                //添加指标说明
                BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByDraft(Guid.Parse(ddlSystem.SelectedValue), FinYear);
                if (BTargetPlan == null)
                {
                    BTargetPlan = AddTargetPlan();
                }
                else
                {
                    if (BTargetPlan.WFStatus == "Progress" || BTargetPlan.WFStatus == "Approved")
                    {
                        BTargetPlan = AddTargetPlan();

                    }
                }
                //为当前计划指标（BTargetPlan），插入计划指标明细数据
                AddTargetPlanDetail(BTargetPlan);
                hideTargetPlanID.Value = BTargetPlan.ID.ToString();
            }
            else
            {
                hideTargetPlanID.Value = Request["BusinessID"];
                BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByID(Request["BusinessID"].ToGuid());
                ddlSystem.SelectedValue = BTargetPlan.SystemID.ToString();
                HidSystemID.Value = BTargetPlan.SystemID.ToString();

                HideProcessCode.Value = StaticResource.Instance[BTargetPlan.SystemID, DateTime.Now].Configuration.Element("ProcessCode").Value + "-ZB";

                FinYear = BTargetPlan.FinYear;

            }

            //if (BTargetPlan != null)
            //{
            //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "_setStlye", "<script>setStlye('missTargetReportSpan,monthReportSpan,monthReportSubmitSpan');</script>");
            //}
        }
        /// <summary>
        /// 添加计划指标
        /// </summary>
        public B_TargetPlan AddTargetPlan()
        {
            B_TargetPlan _BTargetPlan = new B_TargetPlan();
            _BTargetPlan.SystemID = Guid.Parse(ddlSystem.SelectedValue);
            //bmr.FinMonth = FinMonth;
            _BTargetPlan.FinYear = FinYear;
            _BTargetPlan.Status = 2;
            _BTargetPlan.WFStatus = "Draft";
            _BTargetPlan.ID = B_TargetplanOperator.Instance.AddTargetplan(_BTargetPlan);
            return _BTargetPlan;
        }
        /// <summary>
        /// 添加计划指标明细
        /// </summary>
        /// <param name="_BTargetPlan">计划指标</param>
        public void AddTargetPlanDetail(B_TargetPlan _BTargetPlan)
        {
            IList<B_TargetPlanDetail> listTargetPlanDetail = B_TargetplandetailOperator.Instance.GetTargetplandetailList(_BTargetPlan.ID);

            List<B_TargetPlanDetail> B_TargetPlanDetail = new List<B_TargetPlanDetail>();
            if (listTargetPlanDetail.Count == 0)
            {
                A_TargetPlan ATargetPlan = A_TargetplanOperator.Instance.GetTargetplanList(Guid.Parse(ddlSystem.SelectedValue), FinYear).FirstOrDefault();
                //如果A表有数据从A表取数据，否则取B表上一版本的数据。
                if (ATargetPlan != null)
                {
                    //从A表获取数据插入B表
                    _BTargetPlan.Description = ATargetPlan.Description;//月报说明
                    B_TargetplanOperator.Instance.UpdateTargetplan(_BTargetPlan);//更新月报说明

                    IList<A_TargetPlanDetail> listATargetPlanDetail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(ATargetPlan.ID);// .Instance.GetAMonthlyreportdetailList(ATargetPlan.ID);
                    if (listATargetPlanDetail.Count > 0)
                    {
                        foreach (A_TargetPlanDetail A_TargetPlanDetail in listATargetPlanDetail)
                        {
                            A_TargetPlanDetail.TargetPlanID = _BTargetPlan.ID;
                            A_TargetPlanDetail.ID = Guid.NewGuid();

                            B_TargetPlanDetail.Add(A_TargetPlanDetail.ToBModel());
                        }
                    }
                }
                else
                {
                    B_TargetPlan BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByProgressOrApproved(Guid.Parse(ddlSystem.SelectedValue), FinYear);
                    if (BTargetPlan != null)
                    {
                        //从B表获取上一版本数据插入B表
                        IList<B_TargetPlanDetail> LastListBTargetPlanDetail = B_TargetplandetailOperator.Instance.GetTargetplandetailList(BTargetPlan.ID); // B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(_BTargetPlan.ID);
                        if (LastListBTargetPlanDetail.Count > 0)
                        {
                            foreach (B_TargetPlanDetail TargetPlanDetail in LastListBTargetPlanDetail)
                            {
                                TargetPlanDetail.TargetPlanID = _BTargetPlan.ID;
                                TargetPlanDetail.ID = Guid.NewGuid();
                                B_TargetPlanDetail.Add(TargetPlanDetail);
                            }

                        }
                    }
                }

                //判断当前B_TargetPlaID在明细表中是否有数据。
                if (B_TargetPlanDetail.Count > 0)
                {
                    _BTargetPlan.Status = 5;
                    B_TargetplanOperator.Instance.UpdateTargetplan(_BTargetPlan);//更新计划指标表
                    B_TargetplandetailOperator.Instance.AddOrUpdateTargetPlanDetail(B_TargetPlanDetail, "Insert");//更新计划指标明细表
                }
            }
        }

        //权限list
        List<string> PermissionList = null;

        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
                if (!string.IsNullOrEmpty(Request["BusinessID"]))
                {
                    var host = new Wanda.BusinessIndicators.Web.AjaxHander.ProcessController();
                   host.BusinessID = Request["BusinessID"];
                    if (Wanda.Workflow.Client.WFClientSDK.Exist(host.BusinessID))
                    {
                        Wanda.Workflow.Object.WorkflowContext wc = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, host.BusinessID);
                        if (!wc.CurrentUserHasTodoTask)
                        {
                            Server.Transfer("~/BusinessReport/TargetPlanDetailApprove.aspx?BusinessID=" + host.BusinessID);
                        }
                    }
                }
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                PermissionList = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList == null, "Argument GetStartProcessList is Empty");

            }// ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList.Count == 0, "Argument GetStartProcessList is Count = 0");
        }
        /// <summary>
        /// 转换系统时，如果当前系统不存在计划指标数据，添加一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void ddlSystem_TextChanged(object sender, EventArgs e)
        {
            TargetPlanControl();
        }
    }
}