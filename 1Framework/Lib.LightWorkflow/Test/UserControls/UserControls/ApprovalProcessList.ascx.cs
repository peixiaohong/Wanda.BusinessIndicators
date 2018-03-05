using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.Financing.Common;
using Wanda.Financing.Entities;
using Wanda.Financing.Bll;
using Wanda.LightWorkflow.Entities;
using Wanda.LightWorkflow;

namespace Wanda.Financing.Web.UserControls
{
    public partial class ApprovalProcessList : BaseUserControl
    {
        public Func<BizProcess, string> CreateBizProcessUrl;

        public Func<WorkflowInstance, string> CreateWorkflowInstanceUrl;

        /// <summary>
        /// 任务阶段
        /// </summary>
        public int BizStageID
        {
            get
            {
                return (int)ViewState["BizStageID"];
            }
            set
            {
                ViewState["BizStageID"] = value;
            }
        }

        private List<BizProcess> DraftList
        {
            get;
            set;
        }

        private List<WorkflowInstance> WorkflowList
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitPage();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (CreateBizProcessUrl == null || CreateWorkflowInstanceUrl == null)
            {
                throw new Exception("没有为流程或工作流生成URL地址!");
            }
        }

        protected void rptProcessList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {

                if (e.Item.DataItem is BizProcess) //草稿
                {
                    var data = e.Item.DataItem as BizProcess;
                    e.Item.FindLiteral("ltlStatus").Text = Utilities.EnumDescription((Enumerator.ProcessStatus)data.Status);
                    e.Item.FindHyperLink("hyProcessName").Text = CurrentPage.GetFormatString(data.Title, 42) + "(草稿)";
                    if (CreateBizProcessUrl != null)
                    {
                        e.Item.FindHyperLink("hyProcessName").NavigateUrl = CreateBizProcessUrl(data);
                    }
                    e.Item.FindLiteral("ltlCreateUser").Text = PMUserBll.Instance.Load(data.CreateUserID).cUserName;
                    e.Item.FindLiteral("ltlLastUpdateTime").Text = data.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else //待办
                {
                    var data = e.Item.DataItem as WorkflowInstance;
                    e.Item.FindLiteral("ltlStatus").Text = Utilities.EnumDescription((Wanda.LightWorkflow.Common.ProcessInstanceStatus)data.ProcessInstance.Status);
                    e.Item.FindHyperLink("hyProcessName").Text = CurrentPage.GetFormatString(data.ProcessInstance.InstanceName, 50);
                    if (CreateWorkflowInstanceUrl != null)
                    {
                        e.Item.FindHyperLink("hyProcessName").NavigateUrl = CreateWorkflowInstanceUrl(data);
                    }
                    e.Item.FindLiteral("ltlCreateUser").Text = data.ProcessInstance.UserName;
                    e.Item.FindLiteral("ltlCreateTime").Text = data.ProcessInstance.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"); ;
                    e.Item.FindLiteral("ltlLastUpdateTime").Text = data.ProcessInstance.LastUpdatedTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }

        private void InitPage()
        {
            if (rptDraftList.Visible)
            {
                DraftList = BizProcessBll.Instance.GetBizProcess(BizStageID, int.MaxValue);
            }
            else
            {
                DraftList = null;
            }
            List<int> FormalBizProcessIdList = new List<int>();

            //找正式版的审批流程实例
            BizProcessBll.Instance.GetBizProcess(BizStageID, int.MaxValue, Enumerator.VersionType.AllVersion).ForEach(item =>
                {
                    if (item.VersionType == (int)Enumerator.VersionType.Alarm || item.VersionType == (int)Enumerator.VersionType.Formal)
                    {
                        FormalBizProcessIdList.Add(item.BizProcessID);
                    }
                });
            if (FormalBizProcessIdList.Count > 0)
            {
                WorkflowList = Wanda.LightWorkflow.WorkflowEngine.WorkflowService.GetWorkflowInstanceList(FormalBizProcessIdList);
            }
            else
            {
                WorkflowList = new List<WorkflowInstance>();
            }
            BindData();
        }

        private void BindData()
        {
            this.rptDraftList.DataSource = DraftList;
            this.rptDraftList.DataBind();

            this.rptWorkflowList.DataSource = WorkflowList;
            this.rptWorkflowList.DataBind();

            if (this.rptDraftList.Items.Count == 0 && this.rptWorkflowList.Items.Count == 0)
            {
                trEmptyPane.Visible = true;
            }
        }

    }
}