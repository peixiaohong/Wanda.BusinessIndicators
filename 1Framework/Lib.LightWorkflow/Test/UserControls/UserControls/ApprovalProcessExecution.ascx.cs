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
    public partial class ApprovalProcessExecution : BaseUserControl
    {
        #region 公有成员
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
        /// <summary>
        /// 多个流程类型
        /// </summary>
        public List<Enumerator.ProcessType> ProcessTypeList
        {
            get
            {
                return (List<Enumerator.ProcessType>)ViewState["ProcessTypeList"];
            }
            set
            {
                ViewState["ProcessTypeList"] = value;
            }
        }
        #endregion

        #region 私有成员
        private List<WorkflowInstance> WorkflowList
        {
            get;
            set;
        }
        #endregion

        #region 页面事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitPage();
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (CreateWorkflowInstanceUrl == null)
            {
                throw new Exception("没有为流程或工作流生成URL地址!");
            }
        }
        protected void rptProcessList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var data = e.Item.DataItem as WorkflowInstance;
                e.Item.FindLiteral("ltlStatus").Text = Utilities.EnumDescription((Wanda.LightWorkflow.Common.ProcessInstanceStatus)data.ProcessInstance.Status);
                e.Item.FindHyperLink("hyProcessName").Text = GetFormatString(data.ProcessInstance.InstanceName, 50);
                e.Item.FindHyperLink("hyProcessName").ToolTip = data.ProcessInstance.InstanceName;
                if (CreateWorkflowInstanceUrl != null)
                {
                    e.Item.FindHyperLink("hyProcessName").NavigateUrl = CreateWorkflowInstanceUrl(data);
                }
                e.Item.FindLiteral("ltlCreateUser").Text = data.ProcessInstance.UserName;
                e.Item.FindLiteral("ltlCreateTime").Text = data.ProcessInstance.CreatedTime.ToString("yyyy-MM-dd HH:mm");
                e.Item.FindLiteral("ltlLastUpdateTime").Text = data.ProcessInstance.LastUpdatedTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
        #endregion

        #region 私有方法
        private void InitPage()
        {
            string strMultiProcessType = GetMultiProcessType();
            List<int> FormalBizProcessIdList = new List<int>();
            //找正式版的审批流程实例
            if (strMultiProcessType != "")
            {
                BizProcessBll.Instance.LoadMultiProcessType(BizStageID, strMultiProcessType, int.MaxValue, Enumerator.VersionType.AllVersion).ForEach(item => {
                    if (item.VersionType == (int)Enumerator.VersionType.Alarm || item.VersionType == (int)Enumerator.VersionType.Formal)
                    {
                        FormalBizProcessIdList.Add(item.BizProcessID);
                    }
                });
            }
            else
            {
                BizProcessBll.Instance.GetBizProcess(BizStageID, int.MaxValue, Enumerator.VersionType.AllVersion).ForEach(item =>
                {
                    if (item.VersionType == (int)Enumerator.VersionType.Alarm || item.VersionType == (int)Enumerator.VersionType.Formal)
                    {
                        if ((item.Status == (int)Enumerator.ProcessStatus.Approving) || (item.Status == (int)Enumerator.ProcessStatus.Approved) || (item.Status == (int)Enumerator.ProcessStatus.Returned))
                        {
                            FormalBizProcessIdList.Add(item.BizProcessID);
                        }
                    }
                });
            }
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
            this.rptWorkflowList.DataSource = WorkflowList;
            this.rptWorkflowList.DataBind();
            if (this.rptWorkflowList.Items.Count > 0)
            {
                trEmptyPane.Visible = false;
            }
            else
            {
                trEmptyPane.Visible = true;
            }
        }
        private string GetMultiProcessType()
        {
            string str = "";
            if (ProcessTypeList != null && ProcessTypeList.Count > 0)
            {
                for (int i=0; i < ProcessTypeList.Count; i++)
                {
                    str += ((int)ProcessTypeList[i]).ToString() + ",";
                }
            }
            str = str.Trim(',');
            return str;
        }
        #endregion
    }
}