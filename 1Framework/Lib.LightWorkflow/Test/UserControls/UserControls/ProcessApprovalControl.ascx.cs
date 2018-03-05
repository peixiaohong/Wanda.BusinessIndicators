using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.LightWorkflow.Interface;
using Wanda.Financing.Entities;
using Wanda.Financing.Common;
using Wanda.LightWorkflow.Entities;
using Wanda.LightWorkflow;
using Wanda.Financing.Bll;
namespace Wanda.Financing.Web.UserControls
{
    public partial class ProcessApprovalControl : BaseUserControl
    {
        #region 属性
        private readonly object SaveDraftObject = new object();
        private readonly object PageValidateObject = new object();
        private readonly object ApprovalObject = new object();

        public event EventHandler SaveDraft
        {
            add
            {
                this.Events.AddHandler(SaveDraftObject, value);
            }
            remove
            {
                this.Events.RemoveHandler(SaveDraftObject, value);
            }
        }

        public event PageValidateEventHandler PageValidate
        {
            add
            {
                this.Events.AddHandler(PageValidateObject, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageValidateObject, value);
            }
        }

        public event WorkflowStartEventHandler Approval
        {
            add
            {
                this.Events.AddHandler(ApprovalObject, value);
            }
            remove
            {
                this.Events.RemoveHandler(ApprovalObject, value);
            }
        }

        public string ClientValidateFunction
        {
            get
            {
                return (string)ViewState["ClientValidateFunction"];
            }
            set
            {
                ViewState["ClientValidateFunction"] = value;
            }
        }

        public string ClientSaveFunction
        {
            get
            {
                return (string)ViewState["ClientSaveFunction"];
            }
            set
            {
                ViewState["ClientSaveFunction"] = value;
            }
        }

        private BizProcess BizProcess
        {
            get
            {
                if (ViewState["BizProcess"] == null)
                {
                    ViewState["BizProcess"] = Bll.BizProcessBll.Instance.LoadBizProcess(BizProcessId);
                }
                return (BizProcess)ViewState["BizProcess"];
            }
            set
            {
                ViewState["BizProcess"] = value;
            }
        }

        public int BizProcessId
        {
            get
            {
                if (ViewState["BizProcessId"] != null)
                {
                    return Convert.ToInt32(ViewState["BizProcessId"]);
                }
                return Request.GetQueryInt("BizProcessId", DefaultValue.NullInt);
            }
            set
            {
                ViewState["BizProcessId"] = value;
            }
        }

        private WorkflowInstance _Workflow;
        private WorkflowInstance Workflow
        {
            get
            {
                if (_Workflow == null)
                {
                    _Workflow = Wanda.LightWorkflow.WorkflowEngine.WorkflowService.GetWorkflowInstance(BizProcessId);
                }
                return _Workflow;
            }
        }

        /// <summary>
        /// 审批节点
        /// </summary>
        private List<ProcessNode> ProcessApprovalNodeList
        {
            get
            {
                return (List<ProcessNode>)ViewState["ProcessApprovalNodeList"];
            }
            set
            {
                ViewState["ProcessApprovalNodeList"] = value;
            }
        }

        /// <summary>
        /// 抄送节点
        /// </summary>
        private List<ProcessNode> ProcessCCNodeList
        {
            get
            {
                return (List<ProcessNode>)ViewState["ProcessCCNodeList"];
            }
            set
            {
                ViewState["ProcessCCNodeList"] = value;
            }
        }

        private string ContextMenu { get; set; }

        private string ProcessCode
        {
            get
            {
                return "BP" + ((int)this.BizProcess.ProcessType).ToString().PadLeft(2, '0');
            }
        }

        /// <summary>
        /// 审批实例节点(退回后再上报的页面上使用)
        /// </summary>
        private List<ProcessNodeInstance> ProcessInstanceApprovalNodeList
        {
            get
            {
                return (List<ProcessNodeInstance>)ViewState["ProcessInstanceApprovalNodeList"];
            }
            set
            {
                ViewState["ProcessInstanceApprovalNodeList"] = value;
            }
        }

        /// <summary>
        /// 抄送实例节点(退回后再上报的页面上使用)
        /// </summary>
        private List<ProcessNodeInstance> ProcessInstanceCCNodeList
        {
            get
            {
                return (List<ProcessNodeInstance>)ViewState["ProcessInstanceCCNodeList"];
            }
            set
            {
                ViewState["ProcessInstanceCCNodeList"] = value;
            }
        }

        private List<KeyValuePair<PMRole, List<PMUser>>> UserList
        {
            get
            {
                return (List<KeyValuePair<PMRole, List<PMUser>>>)ViewState["UserList"];
            }
            set
            {
                ViewState["UserList"] = value;
            }
        }

        private bool UserNotFound
        {
            get
            {
                return ViewState["UserNotFound"] == null ? true : (bool)ViewState["UserNotFound"];
            }
            set
            {
                ViewState["UserNotFound"] = value;
            }
        }

        private Wanda.LightWorkflow.Entities.TodoWork TodoWork
        {
            get;
            set;
        }

        private List<Wanda.LightWorkflow.Common.NodeOperationType> ApprovalButtonList
        {
            get
            {
                return (List<Wanda.LightWorkflow.Common.NodeOperationType>)ViewState["ApprovalButtonList"];
            }
            set
            {
                ViewState["ApprovalButtonList"] = value;
            }
        }

        private List<ApprovalLog> ApprovalLogs
        {
            get;
            set;
        }

        private bool ShowApprovalConfrim
        {
            get;
            set;
        }

        private PageValidateEventArgs ValidateArgs
        {
            get
            {
                return (PageValidateEventArgs)ViewState["ValidateArgs"];
            }
            set
            {
                ViewState["ValidateArgs"] = value;
            }
        }
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.CurrentPage.CurrentUser != null && this.BizProcessId != DefaultValue.NullInt)
            {
                CheckUserPrivilege();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Wanda.Financing.Web.AjaxUtility.RegisterAjaxType(this.Page, typeof(SearchUserAjax));
            if (!IsPostBack)
            {
                InitPage();
            }
            #region 新增 2012/05/14 功能项(-) 如果视图状态中存在当前用户的操作信息,则保存到当前请求中.
            if (ViewState["CanApproved"] != null)
            {
                if (!HttpContext.Current.Items.Contains("CanApproved"))
                {
                    HttpContext.Current.Items.Add("CanApproved", true);
                }
            }
            #endregion
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            #region 处理按钮客户端事件
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(this.ClientValidateFunction))
            {
                sb.AppendLine(string.Format("function btnValidatePage_ClientClick(){{ if({0}){{ ShowBlockUI('正在提交');setTimeout(btnActualValidatePageClick, 800);}} return false; }}", this.ClientValidateFunction));
            }
            else
            {
                sb.AppendLine("function btnValidatePage_ClientClick(){ ShowBlockUI('正在提交'); setTimeout(btnActualValidatePageClick, 800); return false;}");
            }

            if (!string.IsNullOrWhiteSpace(this.ClientSaveFunction))
            {
                sb.AppendLine(string.Format("function btnSaveDraft_ClientClick() {{if({0}){{ ShowBlockUI('正在保存'); setTimeout(btnActualSaveDraftClick, 800);}} return false;}}", this.ClientSaveFunction));
            }
            else
            {
                sb.AppendLine("function btnSaveDraft_ClientClick() {ShowBlockUI('正在保存');setTimeout(btnActualSaveDraftClick, 800); return false;}");
            }

            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ProcessApproval_ClientClick", sb.ToString(), true);
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ProcessApproval_ClientClick", sb.ToString(), true);
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "ModalDialog"))
            {
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "ModalDialog", Page.ResolveUrl("~/Scripts/jqModal.js"));
            }
            #endregion

            #region 处理右键菜单
            if (PaneApproval.Visible)
            {
                if (this.BizProcess.Status == (int)Enumerator.ProcessStatus.Approving || this.BizProcess.Status == (int)Enumerator.ProcessStatus.Approved || this.BizProcess.Status == (int)Enumerator.ProcessStatus.Completed || (this.BizProcess.Status == (int)Enumerator.ProcessStatus.Returned && this.ProcessInstanceApprovalNodeList[0].UserID != CurrentPage.CurrentUserID))
                {
                    System.Text.StringBuilder sbcontext = new System.Text.StringBuilder("<ul id='rightmenu' class='rightmenu' style='z-index: 3000'>");
                    sbcontext.AppendFormat("<li class='Entrust' style='display:none' onmousedown=\"$('#btnEntrustOK').click()\">{0}</li>", "确认");
                    sbcontext.AppendFormat("<li class='Entrust' style='display:none' onmousedown=\"$('#btnEntrustCancel').click()\">{0}</li>", "取消");
                    sbcontext.AppendFormat("<li class='Forward' style='display:none' onmousedown=\"$('#btnForwardOK').click()\">{0}</li>", "确认");
                    sbcontext.AppendFormat("<li class='Forward' style='display:none' onmousedown=\"$('#btnForwardCancel').click()\">{0}</li>", "取消");
                    if (btnSaveDraft.Visible)
                    {
                        sbcontext.AppendFormat("<li class='Approval' onmousedown=\"$('#{0}').click()\">{1}</li>", btnSaveDraft.ClientID, btnSaveDraft.Text);
                    }
                    if (!string.IsNullOrEmpty(ContextMenu))
                    {
                        sbcontext.Append(ContextMenu);
                    }
                    sbcontext.AppendFormat("<li onmousedown=\"newcopy()\">{0}</li>", "复制");
                    sbcontext.AppendFormat("<li onmousedown=\"paste()\">{0}</li>", "粘贴");
                    sbcontext.Append("</ul>");
                    sbcontext.AppendFormat("<script type='text/javascript' src='{0}'></script>", ResolveUrl("~/Scripts/contextmenu.js"));
                    ltlContextMenu.Text = sbcontext.ToString();
                }
            }
            #endregion
        }

        protected void btnApprovalSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.hidApprovalOperation.Value))
            {
                CurrentPage.Alert("错误的操作");
                return;
            }

            Wanda.LightWorkflow.Common.NodeOperationType operation = (Wanda.LightWorkflow.Common.NodeOperationType)Enum.Parse(typeof(Wanda.LightWorkflow.Common.NodeOperationType), this.hidApprovalOperation.Value, true);

            Wanda.LightWorkflow.WorkflowInstance wfi = Wanda.LightWorkflow.WorkflowEngine.WorkflowService.GetWorkflowInstance(this.BizProcessId);
            switch (operation)
            {
                case LightWorkflow.Common.NodeOperationType.Approved:
                case LightWorkflow.Common.NodeOperationType.Archive:
                case LightWorkflow.Common.NodeOperationType.Comment:
                    {
                        wfi.Submit(this.txtApprovalNote.Text, CurrentPage.CurrentUserID, 0);
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Cancel:
                    {

                        wfi.Cancel(this.txtCancel.Text, CurrentPage.CurrentUserID);
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Entrust:
                    {
                        int entrustUserId = 0;
                        PMUser entruseUser = null;
                        if (int.TryParse(this.ddlEntrustUserList.SelectedValue, out entrustUserId))
                        {
                            entruseUser = Bll.PMUserBll.Instance.Load(entrustUserId);
                            if (entruseUser != null)
                            {
                                wfi.Entrust(this.txtEntrust.Text, CurrentPage.CurrentUserID,
                                    entruseUser.UserID,
                                    entruseUser.cUserName,
                                    entruseUser.cUserID,
                                    entruseUser.UserName);
                                CurrentPage.Alert(ConstSet.MESSAGE_OPERATE_SUCCESS);
                                CurrentPage.CloseWindow();
                            }
                        }
                        if (entruseUser == null)
                        {
                            CurrentPage.Alert("用户没有找到.");
                        }
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Forward:
                    {
                        List<PMUser> users = Bll.PMUserBll.Instance.LoadUserInfoByUserId(hidForwardUserIdList.Value);
                        if (users.Count > 0)
                        {
                            int[] forwards = new int[users.Count];
                            string[] forwardusernames = new string[users.Count];
                            string[] forwardcuserids = new string[users.Count];
                            string[] forwardusercodes = new string[users.Count];
                            for (int i = 0; i < users.Count; i++)
                            {
                                forwards[i] = users[i].UserID;
                                forwardcuserids[i] = users[i].cUserID;
                                forwardusernames[i] = users[i].cUserName;
                                forwardusercodes[i] = users[i].UserName;
                            }
                            wfi.Forward(this.txtForward.Text, CurrentPage.CurrentUserID, CurrentPage.CurrentUser.cUserName, CurrentPage.CurrentUser.cUserID, CurrentPage.CurrentUser.UserName,
                                forwards, forwardusernames, forwardcuserids, forwardusercodes);
                            Response.Redirect(Request.RawUrl);
                        }
                        else
                        {
                            CurrentPage.Alert("未找到要转发的用户。");
                        }
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Reject:
                    {
                        wfi.Reject(this.txtApprovalNote.Text, CurrentPage.CurrentUserID);
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Launch:
                    {

                        btnValidatePage_Click(sender, e);
                        return;
                    }
            }
            CurrentPage.CloseWindow();
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            EventHandler handler = this.Events[SaveDraftObject] as EventHandler;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected void rptApprovalUserList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is ProcessNode)
                {
                    ProcessNode data = (ProcessNode)e.Item.DataItem;
                    Literal ltlUserName = e.Item.FindLiteral("ltlUserName");
                    DropDownList ddlUserName = e.Item.FindDropDownList("ddlUserName");
                    e.Item.FindLiteral("ltlRoleName").Text = this.ProcessApprovalNodeList[e.Item.ItemIndex].NodeName;
                    if (UserList.Exists(item => item.Key.RoleID == data.Role))
                    {
                        var users = UserList.Find(item => item.Key.RoleID == data.Role);
                        //第一个审批流程中的节点显示当前的用户
                        if (e.Item.ItemIndex == 0)
                        {
                            ltlUserName.Visible = true;
                            ltlUserName.Text = CurrentPage.CurrentUser.cUserName;
                            ddlUserName.Visible = false;
                        }
                        else
                        {
                            if (users.Value.Count > 0)
                            {
                                ltlUserName.Visible = users.Value.Count == 1;
                                ddlUserName.Visible = users.Value.Count > 1;

                                if (ltlUserName.Visible)
                                {
                                    ltlUserName.Text = users.Value[0].cUserName;
                                }
                                if (ddlUserName.Visible)
                                {
                                    ddlUserName.DataSource = users.Value;
                                    ddlUserName.DataTextField = "cUserName";
                                    ddlUserName.DataValueField = "UserID";
                                    ddlUserName.DataBind();
                                    //合同审批和合同变更的流程，在法务部后面添加了经办人节点，节点默认的审批人为发起人。
                                    if (this.BizProcess.ProcessType == (int)Common.Enumerator.ProcessType.ChangeContract || this.BizProcess.ProcessType == (int)Common.Enumerator.ProcessType.ContractApproval)
                                    {
                                        if (users.Value.Exists(u => u.UserID == this.CurrentPage.CurrentUserID))
                                        {
                                            ddlUserName.SelectedIndex = ddlUserName.Items.IndexOf(ddlUserName.Items.FindByValue(this.CurrentPage.CurrentUserID.ToString()));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ltlUserName.Visible = true;
                                ltlUserName.Text = "没有找到相关人员";
                                UserNotFound = true;
                                ddlUserName.Visible = false;
                            }
                        }
                        if (e.Item.ItemIndex == this.ProcessApprovalNodeList.Count - 1)
                        {
                            e.Item.FindControl("TbCellArrow").Visible = false;
                        }
                    }
                    else
                    {
                        ltlUserName.Visible = true;
                        ltlUserName.Text = "没有找到相关人员";
                        UserNotFound = true;
                        ddlUserName.Visible = false;
                    }
                }
                else
                {
                    ProcessNodeInstance data = (ProcessNodeInstance)e.Item.DataItem;
                    e.Item.FindLiteral("ltlUserName").Text = data.UserName;
                    e.Item.FindDropDownList("ddlUserName").Visible = false;
                    e.Item.FindLiteral("ltlRoleName").Text = data.NodeName;

                    if (e.Item.ItemIndex == this.ProcessInstanceApprovalNodeList.Count - 1)
                    {
                        e.Item.FindControl("TbCellArrow").Visible = false;
                    }

                }
            }
        }

        protected void rptCCUserList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is ProcessNode)
                {
                    ProcessNode data = (ProcessNode)e.Item.DataItem;
                    Literal ltlUserName = e.Item.FindLiteral("ltlUserName");
                    DropDownList ddlUserName = e.Item.FindDropDownList("ddlUserName");
                    e.Item.FindLiteral("ltlRoleName").Text = this.ProcessCCNodeList[e.Item.ItemIndex].NodeName;

                    if (UserList.Exists(item => item.Key.RoleID == data.Role))
                    {
                        var users = UserList.Find(item => item.Key.RoleID == data.Role);
                        if (users.Value.Count > 0)
                        {
                            ltlUserName.Visible = users.Value.Count == 1;
                            ddlUserName.Visible = users.Value.Count > 1;

                            if (ltlUserName.Visible)
                            {
                                ltlUserName.Text = users.Value[0].cUserName;
                            }
                            if (ddlUserName.Visible)
                            {
                                ddlUserName.DataSource = users.Value;
                                ddlUserName.DataTextField = "cUserName";
                                ddlUserName.DataValueField = "UserID";
                                ddlUserName.DataBind();
                            }
                        }
                        else
                        {
                            ltlUserName.Visible = true;
                            ltlUserName.Text = "无用户";
                            ddlUserName.Visible = false;
                        }
                    }
                    else
                    {
                        ltlUserName.Visible = true;
                        ltlUserName.Text = "无用户";
                        ddlUserName.Visible = false;
                    }
                    if (e.Item.ItemIndex == this.ProcessCCNodeList.Count - 1)
                    {
                        e.Item.FindControl("TbCellArrow").Visible = false;
                    }
                }
                else
                {
                    ProcessNodeInstance data = (ProcessNodeInstance)e.Item.DataItem;
                    e.Item.FindLiteral("ltlUserName").Text = data.UserName;
                    e.Item.FindDropDownList("ddlUserName").Visible = false;
                    e.Item.FindLiteral("ltlRoleName").Text = data.NodeName;
                    if (e.Item.ItemIndex == this.ProcessInstanceCCNodeList.Count - 1)
                    {
                        e.Item.FindControl("TbCellArrow").Visible = false;
                    }
                }
            }
        }

        public void btnValidatePage_Click(object sender, EventArgs e)
        {
            ValidateArgs = new PageValidateEventArgs();
            if (OnPageValidate(ValidateArgs))
            {
                BindApprovalUser(ValidateArgs);
            }
        }

        protected void rptCCingUserList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ProcessNodeInstance pni = e.Item.DataItem as ProcessNodeInstance;
                e.Item.FindLiteral("ltlRoleName").Text = pni.NodeName;

                if (e.Item.ItemIndex == this.ProcessInstanceCCNodeList.Count - 1)
                {
                    e.Item.FindControl("TbCellArrow").Visible = false;
                }
            }
        }

        protected void rptApprovalingUserList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ProcessNodeInstance pni = e.Item.DataItem as ProcessNodeInstance;
                e.Item.FindLiteral("ltlRoleName").Text = pni.NodeName;
                if (e.Item.ItemIndex == this.ProcessInstanceApprovalNodeList.Count - 1)
                {
                    e.Item.FindControl("TbCellArrow").Visible = false;
                }
                e.Item.FindControl("TbCellTick").Visible = pni.Status == (int)Wanda.LightWorkflow.Common.NodeStatus.Executed;
                if (pni.Status == (int)Wanda.LightWorkflow.Common.NodeStatus.Executing)
                {
                    var tbUser = e.Item.FindControl<System.Web.UI.HtmlControls.HtmlTable>("TbUser");
                    tbUser.Attributes.Add("class", "currentuser");
                }
            }
        }

        //删除流程
        protected void btnDeleteProcess_Click(object sender, EventArgs e)
        {
            BizProcessBll.Instance.ProcessDelete(this.BizProcess.BizProcessID, (Enumerator.ProcessType)this.BizProcess.ProcessType);
            this.CurrentPage.Alert("页面已重置，请重新打开页面。");
            this.CurrentPage.CloseWindow();
        }

        protected void btnApproval_Click(object sender, EventArgs e)
        {
            WorkflowStartEventArgs arg = CreateProcessInstance();
            if (arg == null)
            {
                return;
            }
            WorkflowStartEventHandler handler = this.Events[ApprovalObject] as WorkflowStartEventHandler;
            if (handler != null)
            {
                handler(sender, arg);
            }
        }

        #region 私有方法

        private void CheckUserPrivilege()
        {
            BizProcess CurrentBizProcess = BizProcessBll.Instance.LoadBizProcess(this.BizProcessId);

            if (CurrentBizProcess.VersionType == (int)Enumerator.VersionType.Alarm)
            {
                //如果是预警,不显示删除按钮,显示撤销按钮
                if ((this.BizProcess.ProcessType != (int)Enumerator.ProcessType.LoanImplementationApply) || (!(CurrentBizProcess.Status == (int)Enumerator.ProcessStatus.Initialize || CurrentBizProcess.Status == (int)Enumerator.ProcessStatus.Returned)))
                {
                    btnDeleteProcess.Visible = false;
                    btnSaveDraft.Visible = false;
                }
                btnRequestSaveDraft.Visible = btnSaveDraft.Visible;

                //如果预警未上报,则进行跳转
                if (CurrentBizProcess.Status == (int)Enumerator.ProcessStatus.Initialize)
                {
                    Enumerator.PrivilegeCode piCode = (Enumerator.PrivilegeCode)int.MaxValue;
                    switch ((Enumerator.ProcessType)CurrentBizProcess.ProcessType)
                    {
                        case Enumerator.ProcessType.InterestPaymentApply:
                        case Enumerator.ProcessType.OntimeReimbursementApply:
                        case Enumerator.ProcessType.EarlyRepaymentApply:
                            {
                                piCode = Enumerator.PrivilegeCode.Func_Execution_Repayment;
                                break;
                            }
                        case Enumerator.ProcessType.LoanImplementationApply:
                            {
                                piCode = Enumerator.PrivilegeCode.Func_Execution_Loan;
                                break;
                            }
                        case Enumerator.ProcessType.RepayFinancingCostApply:
                            {
                                piCode = Enumerator.PrivilegeCode.Func_Execution_Cost;
                                break;
                            }
                    }
                    if (!CurrentPage.CurrentPrivilegeList.Exists(it => it.PrivilegeItemCode == ((int)piCode).ToString()))
                    {
                        Response.Redirect("~/Projects/AlarmView.aspx?BizProcessId=" + this.BizProcessId);
                    }
                }
            }
            var HasApprovalPermission = false;
            if (Workflow != null)
            {
                this.ApprovalButtonList = Workflow.GetPermissionsList(CurrentPage.CurrentUserID);

                //检查审批权限或超级管理员
                HasApprovalPermission = this.ApprovalButtonList.Count > 0;
                //如果没有找到审批权限,则检查有无数据权限
                if (!HasApprovalPermission)
                {
                    if (CurrentBizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost)
                    {
                        #region 如果是岗位变更
                        var bp17 = BP17ChageUserBll.Instance.LoadBP17(CurrentBizProcess.BizProcessID);
                        if (CurrentBizProcess != null && !this.CurrentPage.GetUserOrgScope().Contains(bp17.OrgID))
                        {
                            if (this.CurrentPage.IsSuperAdmin)
                            {
                                this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                            }
                        }
                        else
                        {
                            this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                        }
                        #endregion
                    }
                    else if (CurrentBizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetManagement ||
                        CurrentBizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetEdit ||
                        CurrentBizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetDelete ||
                        CurrentBizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetRemoveMortgage)
                    {
                        #region 如果是资产相关的流程
                        var bp19 = BP19AssetListBll.Instance.LoadAllBP19AssetList(CurrentBizProcess.BizProcessID, true);
                        if (CurrentBizProcess != null && bp19 != null && !this.CurrentPage.GetUserOrgScope().Contains(bp19[0].OrgID))
                        {
                            if (this.CurrentPage.IsSuperAdmin)
                            {
                                this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                            }
                        }
                        else
                        {
                            this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                        }
                        #endregion
                    }
                    else if (CurrentBizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport)
                    {
                        #region 如果是现金流报表导入，设置是否包含转发按钮

                        #endregion
                    }
                    else
                    {
                        #region 如果是项目审批流程
                        //检查有无数据权限,如果有数据权限,则显示转发
                        var task = FinacingTaskBll.Instance.Load(CurrentBizProcess.TaskID);
                        if (task != null)
                        {
                            if (task.JucheType == (int)Enumerator.JucheType.RealEstate)
                            {
                                if (CurrentBizProcess != null && !this.CurrentPage.GetUserScope().Contains(CurrentBizProcess.ProjectID))
                                {
                                    if (this.CurrentPage.IsSuperAdmin)
                                    {
                                        this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                                    }
                                }
                                else
                                {
                                    this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                                }
                            }
                            else
                            {
                                if (CurrentBizProcess != null && !this.CurrentPage.GetUserScope((int)Enumerator.JucheType.HQLoan).Contains(task.BusinessshapeID))
                                {
                                    if (this.CurrentPage.IsSuperAdmin)
                                    {
                                        this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                                    }
                                }
                                else
                                {
                                    this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                                }
                            }
                        }
                        #endregion
                    }
                }
                HasApprovalPermission = this.ApprovalButtonList.Count > 0;
                if (!this.ApprovalButtonList.Exists(item => item == LightWorkflow.Common.NodeOperationType.Launch))
                {
                    if (this.Request.RawUrl.Contains("Edit.aspx") || this.Request.RawUrl.Contains("Apply.aspx"))
                    {
                        var url = this.Request.RawUrl.Replace("Edit.aspx", "Approval.aspx").Replace("Apply.aspx", "Confirm.aspx");
                        CurrentPage.Response.Redirect(url, true);
                    }
                }
                if (!HasApprovalPermission)
                {
                    //按揭贷款流程 并且不是按揭贷款的时候
                    if (CurrentBizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan
                        &&
                        CurrentBizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport)
                    {
                        throw new Exception("您无权访问本页面");
                    }
                }
            }
        }

        #region 事件方法
        private void OnSaveDraft(EventArgs e)
        {
            if (this.Events[SaveDraftObject] != null)
            {
                EventHandler handler = this.Events[SaveDraftObject] as EventHandler;
                handler(this, e);
            }
        }

        private bool OnPageValidate(PageValidateEventArgs e)
        {
            PageValidateEventHandler handler = this.Events[PageValidateObject] as PageValidateEventHandler;
            var task = Bll.FinacingTaskBll.Instance.Load(this.BizProcess.TaskID);
            bool result = true;
            if (handler != null)
            {
                result = handler(this, e);
            }

            #region 设置流程所属公司的类型
            //如果验证时未设置发起人的公司类型,则由下面的代码来实现
            if (!e.ProcessContext.ContainsKey(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType.ToString()))
            {

                Lvl.LogAdapter.App.Info(string.Format("当前用户{3}-{4} 角色:{0} 项目公司角色:{1} 商管公司角色:{2}",
                    string.Join(",", CurrentPage.CurrentUser.RoleIdList),
                    string.Join(",", ConstSet.ProjectCompanyRoleList),
                    string.Join(",", ConstSet.BusinessManagerRoleList),
                    CurrentPage.CurrentUser.cUserName,
                    CurrentPage.CurrentUser.UserName
                    ));

                if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost)
                {
                    if (ConstSet.ProjectCompanyRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                    {
                        e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "项目公司");
                    }
                    else if (ConstSet.BusinessManagerRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                    {
                        e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "商管公司");
                    }
                }
                else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport)
                {
                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "总部");
                }
                else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan)//点击上报按钮时  是按揭贷款
                {
                    //
                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "总部");
                }
                else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetManagement ||
                    this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetEdit ||
                    this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetDelete ||
                    this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetRemoveMortgage)
                {
                    //如果是资产添加，编辑，删除
                    if (this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetRemoveMortgage)
                    {
                        if (ConstSet.ProjectCompanyRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                        {
                            e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "项目公司");
                        }
                        else if (ConstSet.BusinessManagerRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                        {
                            e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "商管公司");
                        }
                    }
                    else
                    {
                        //获取BP19
                        BP19AssetList bp19 = BP19AssetListBll.Instance.LoadBP19AssetList(this.BizProcessId, true);
                        if (bp19 != null && bp19.TaskID != 0)
                        {
                            FinacingTask fTask = FinacingTaskBll.Instance.Load(bp19.TaskID);
                            if (fTask != null && (fTask.JucheType == (int)Enumerator.JucheType.HQLoan || fTask.JucheType == (int)Enumerator.JucheType.HQCulture))
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, ((Enumerator.JucheType)fTask.JucheType).GetDescription());
                            }
                            else
                            {
                                if (ConstSet.ProjectCompanyRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "项目公司");
                                }
                                else if (ConstSet.BusinessManagerRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "商管公司");
                                }
                            }
                        }
                        else
                        {
                            if (ConstSet.ProjectCompanyRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "项目公司");
                            }
                            else if (ConstSet.BusinessManagerRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "商管公司");
                            }
                        }
                    }
                }
                else
                {
                    switch ((Enumerator.JucheType)task.JucheType)
                    {
                        case Enumerator.JucheType.HQLoan:
                        case Enumerator.JucheType.HQCulture:
                            {
                                #region 设置融资任务业态信息
                                if (!e.ProcessContext.ContainsKey(Wanda.Financing.Common.Enumerator.ProcessContextName.BusinessShapeId.ToString()))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.BusinessShapeId, task.BusinessshapeID);
                                }
                                #endregion
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, ((Enumerator.JucheType)task.JucheType).GetDescription());
                                break;
                            }
                        case Enumerator.JucheType.RealEstate:
                            {
                                if (ConstSet.ProjectCompanyRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "项目公司");
                                }
                                else if (ConstSet.BusinessManagerRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "商管公司");
                                }
                                break;
                            }
                        case Enumerator.JucheType.HQCultureLoan:
                            {
                                if (ConstSet.CultureProjectCompanyRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "文化项目公司");
                                }
                                else if (ConstSet.CultureBusinessManagerRoleList.Any(item => CurrentPage.CurrentUser.RoleIdList.Any(r => r == item)))
                                {
                                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType, "文化商管公司");
                                }
                                break;
                            }
                        default:
                            {
                                throw new Exception("检测到未支持的贷款类型：" + ((Enumerator.JucheType)task.JucheType).ToString());
                            }
                    }
                }
            }
            if (!e.ProcessContext.ContainsKey(Wanda.Financing.Common.Enumerator.ProcessContextName.CompanyType.ToString()))
            {
                throw new Exception("没有找到当前用户所属项目公司,不能发起流程.");
            }
            #endregion

            #region 设置贷款类型
            if (this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport
                && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan//并且不是按揭贷款流程
                && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost
                 && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetManagement
                 && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetEdit
                 && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetDelete
                 && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetRemoveMortgage
                 && !e.ProcessContext.ContainsKey(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType.ToString()))
            {
                /// 贷款类型值可以是“项目公司开发贷”，“项目公司经营贷”，“项目公司其他贷”，“总部流贷”，“总部信托贷”，“总部其他贷款”

                var loanType = (Enumerator.LoanType)task.LoanTypeID;
                if (task.JucheType == (int)Enumerator.JucheType.RealEstate) //项目公司贷款
                {
                    switch (loanType)
                    {
                        case Common.Enumerator.LoanType.Develop:
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType, "地产项目开发贷");
                                break;
                            }
                        case Common.Enumerator.LoanType.Operate:
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType, "地产项目经营贷");
                                break;
                            }
                        case Common.Enumerator.LoanType.Other:
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType, "地产项目其他贷");
                                break;
                            }
                    }
                }
                else
                {
                    //总部贷款
                    switch (loanType)
                    {
                        case Common.Enumerator.LoanType.Develop:
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType, "总部流贷");
                                break;
                            }
                        case Common.Enumerator.LoanType.Operate:
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType, "总部信托贷");
                                break;
                            }
                        case Common.Enumerator.LoanType.Other:
                            {
                                e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.LoanType, "总部其他贷款");
                                break;
                            }
                    }
                }
            }

            #endregion

            #region 设置融资任务编号
            if (this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost
                && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport
                && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan//并且不是按揭贷款流程
                )
            {
                if (!e.ProcessContext.ContainsKey(Wanda.Financing.Common.Enumerator.ProcessContextName.TaskId.ToString()))
                {
                    e.AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName.TaskId, this.BizProcess.TaskID);
                }
            }
            else
            {
                //设置项目公司编号
            }
            #endregion

            #region  设置是否直营店
            if (this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost
                && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport
                 && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan//并且不是按揭贷款流程
                )
            {
                if (!e.ProcessContext.ContainsKey(Wanda.Financing.Common.Enumerator.ProcessContextName.IsDirectStore.ToString()))
                {
                    e.AddProcessContext(Enumerator.ProcessContextName.IsDirectStore, ConstSet.DirectStoreList.Contains(this.BizProcess.TaskID) ? "是" : "否");
                }
            }
            #endregion
            return result;
        }

        private void OnApproval(EventArgs e)
        {
            if (this.Events[ApprovalObject] != null)
            {
                EventHandler handler = this.Events[ApprovalObject] as EventHandler;
                handler(this, e);
            }
        }
        #endregion

        private void InitPage()
        {
            if (this.BizProcess.VersionType == (int)Enumerator.VersionType.Alarm)
            {
                //如果是预警,不显示删除按钮,显示撤销按钮
                if ((this.BizProcess.ProcessType != (int)Enumerator.ProcessType.LoanImplementationApply) || (!(BizProcess.Status == (int)Enumerator.ProcessStatus.Initialize || BizProcess.Status == (int)Enumerator.ProcessStatus.Returned)))
                {
                    btnDeleteProcess.Visible = false;
                    btnSaveDraft.Visible = false;
                }
                btnRequestSaveDraft.Visible = btnSaveDraft.Visible;
            }

            if (this.Workflow != null)
            {
                //处理审批中的流程
                if ((this.Workflow.NodeInstances[0].UserID != this.CurrentPage.CurrentUserID && !this.CurrentPage.IsSuperAdmin) ||
    !(this.BizProcess.Status == (int)Enumerator.ProcessStatus.Initialize || this.BizProcess.Status == (int)Enumerator.ProcessStatus.Returned))
                {
                    if (this.Request.RawUrl.Contains("Edit.aspx") || this.Request.RawUrl.Contains("Apply.aspx"))
                    {
                        var url = this.Request.RawUrl.Replace("Edit.aspx", "Approval.aspx").Replace("Apply.aspx", "Confirm.aspx");
                        CurrentPage.Response.Redirect(url, true);
                    }
                }
                this.paneRequestApproval.Visible = false;
                this.PaneApproval.Visible = true;
                this.ApprovalButtonList = Workflow.GetPermissionsList(CurrentPage.CurrentUserID);
                this.TodoWork = Workflow.TodoWorks != null && Workflow.TodoWorks.Count > 0 ? Workflow.TodoWorks[0] : null;

                //处理退回的流程
                if ((this.BizProcess.Status == (int)Enumerator.ProcessStatus.Initialize) || this.BizProcess.Status == (int)Enumerator.ProcessStatus.Returned)
                {
                    if (this.Request.RawUrl.Contains("Approval.aspx") || this.Request.RawUrl.Contains("Confirm.aspx"))
                    {
                        if (this.ApprovalButtonList.Exists(item => item == LightWorkflow.Common.NodeOperationType.Launch))
                        {
                            var url = this.Request.RawUrl.Replace("Approval.aspx", "Edit.aspx").Replace("Confirm.aspx", "Apply.aspx");
                            CurrentPage.Response.Redirect(url, true);
                        }
                    }

                    //预警退回时,不显示撤销按钮
                    if (this.BizProcess.VersionType == (int)Enumerator.VersionType.Alarm && this.ApprovalButtonList.Contains(LightWorkflow.Common.NodeOperationType.Cancel))
                    {
                        if (this.BizProcess.ProcessType != (int)Enumerator.ProcessType.LoanImplementationApply)
                        {
                            this.ApprovalButtonList.Remove(LightWorkflow.Common.NodeOperationType.Cancel);
                        }
                    }
                }


                ProcessInstanceApprovalNodeList = new List<ProcessNodeInstance>();
                ProcessInstanceCCNodeList = new List<ProcessNodeInstance>();

                //未找到待办,当前工作流可能已完成
                if (Workflow.TodoWorks != null)
                {
                    TodoWork = Workflow.TodoWorks.Find(item => item.UserID == CurrentPage.CurrentUserID);
                }
                ApprovalLogs = Workflow.ApprovalLogs;

                //如果没有找到操作,则当前用户只有转发的权限
                if (!this.ApprovalButtonList.Contains(LightWorkflow.Common.NodeOperationType.Forward))
                {
                    this.ApprovalButtonList.Add(LightWorkflow.Common.NodeOperationType.Forward);
                }

                //只有一个转发按钮时,不显示审批意见
                if (this.ApprovalButtonList.Count == 1 && this.ApprovalButtonList[0] == LightWorkflow.Common.NodeOperationType.Forward)
                {
                    ShowApprovalConfrim = false;
                }
                else
                {
                    ShowApprovalConfrim = this.ApprovalButtonList.Count > 0;
                }

                foreach (var item in Workflow.NodeInstances)
                {
                    if (item.NodeSeq < Wanda.LightWorkflow.Common.CCNodeSeqNumber || item.NodeSeq == Wanda.LightWorkflow.Common.ArchiveNodeSeqNumber)
                    {
                        ProcessInstanceApprovalNodeList.Add(item);
                    }
                    if (item.NodeSeq >= Wanda.LightWorkflow.Common.CCNodeSeqNumber && item.NodeSeq < Wanda.LightWorkflow.Common.ArchiveNodeSeqNumber)
                    {
                        ProcessInstanceCCNodeList.Add(item);
                    }
                }
                #region 设置是否显示
                //流程被退回时,不显示签字节点
                this.trApprovalNode.Visible = false;
                if (this.ApprovalButtonList.Exists(item => item == LightWorkflow.Common.NodeOperationType.Approved
                    || item == LightWorkflow.Common.NodeOperationType.Reject
                    || item == LightWorkflow.Common.NodeOperationType.Comment
                    || item == LightWorkflow.Common.NodeOperationType.Archive))
                {
                    this.trApprovalNode.Visible = true;
                }
                this.trApprovalFlags.Visible = this.trApprovalNode.Visible;
                #endregion

                BindApprovalingData();
            }
            else
            {
                this.paneRequestApproval.Visible = true;
                this.PaneApproval.Visible = false;
            }
            //加载树形第一级
            BindTreeOrgList();
            ddlJobType.Items.Clear();
            ddlJobType.Items.Add(new ListItem("在职", "0"));
        }

        private void BindApprovalingData()
        {
            this.rptApprovalingUserList.DataSource = this.ProcessInstanceApprovalNodeList;
            this.rptApprovalingUserList.DataBind();

            this.rptCCingUserList.DataSource = this.ProcessInstanceCCNodeList;
            this.rptCCingUserList.DataBind();
            this.lblNoCCUser.Visible = this.ProcessInstanceCCNodeList.Count == 0;
            this.ltlApprovalNoCCUser.Visible = this.lblNoCCUser.Visible;

            this.rptApprovalLog.DataSource = this.ApprovalLogs;
            this.rptApprovalLog.DataBind();

            //被退回的正式版
            if (this.BizProcess.VersionType != (int)Enumerator.VersionType.Alarm)
            {
                if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost)
                {
                    this.btnSaveDraft.Visible = false;
                }
                else
                {
                    this.btnSaveDraft.Visible = this.ProcessInstanceApprovalNodeList[0].UserID == CurrentPage.CurrentUserID && this.BizProcess.Status == (int)Enumerator.ProcessStatus.Returned && this.BizProcess.VersionType == (int)Enumerator.VersionType.Formal;
                }
                btnRequestSaveDraft.Visible = this.btnSaveDraft.Visible;
            }
            if (!string.IsNullOrWhiteSpace(this.ClientSaveFunction))
            {
                if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "btnSaveDraft_ClientClick", string.Format("function btnSaveDraft_ClientClick(){{if({0}){{ ShowBlockUI('正在保存'); window.setTimeout(btnActualSaveDraft_ClientClick, 800);}} return false;}}", this.ClientSaveFunction), true);
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "btnSaveDraft_ClientClick", string.Format("function btnSaveDraft_ClientClick(){{if({0}){{ ShowBlockUI('正在保存'); window.setTimeout(btnActualSaveDraft_ClientClick, 800);}} return false;}}", this.ClientSaveFunction), true);
                }
            }
            else
            {
                if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "btnSaveDraft_ClientClick", "function btnSaveDraft_ClientClick(){ ShowBlockUI('正在保存');window.setTimeout(btnActualSaveDraft_ClientClick, 800); return false;}", true);
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "btnSaveDraft_ClientClick", "function btnSaveDraft_ClientClick(){ ShowBlockUI('正在保存');window.setTimeout(btnActualSaveDraft_ClientClick, 800); return false;}", true);
                }
            }


            this.ltlApprovalButtonList.Text = string.Empty;
            foreach (var btn in this.ApprovalButtonList)
            {
                this.ltlApprovalButtonList.Text += BuildApprovalButton(btn);
            }

            //绑定委托用户节点
            if (this.ApprovalButtonList.Contains(LightWorkflow.Common.NodeOperationType.Entrust))
            {
                List<PMUser> entrustUserList = null;
                var node = this.Workflow.NodeInstances.Find(item => item.NIID == this.TodoWork.NIID);
                if (node != null)
                {
                    var entrust = new List<KeyValuePair<PMRole, List<PMUser>>>();
                    if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost)
                    {
                        entrust.AddRange(PMUserToRoleBll.Instance.GetPMUserByMapRoleAndScope(BizProcessId, new List<ApprovalProcessNodeShortInfo>() { new ApprovalProcessNodeShortInfo(node.Role, node.NodeName) }));
                    }
                    else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport)
                    {
                        entrust.AddRange(PMUserToRoleBll.Instance.GetPMUserByRole(new List<int>() { node.Role }));
                    }
                    else if(this.BizProcess.ProcessType==(int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan)//按揭贷款流程 加载委托人
                    {
                        //entrust.AddRange(PMUserToRoleBll.Instance.GetPMUserByRole(new List<int>(){node.Role}));
                        entrust.AddRange(PMUserToRoleBll.Instance.GetPMUserByRole(new List<int>(){node.Role},BizProcessId));
                    }
                    else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetManagement ||
                        this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetEdit ||
                        this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetDelete ||
                        this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetRemoveMortgage)
                    {
                        entrust.AddRange(PMUserToRoleBll.Instance.GetAssetPMUserByRoleAndScope(new List<int>() { node.Role }, BizProcessId));
                    }
                    else
                    {
                        entrust.AddRange(PMUserToRoleBll.Instance.GetPMUserByRoleAndScope(new List<int>() { node.Role }, BizProcessId));
                    }

                    if (entrust.Count > 0)
                    {
                        entrustUserList = entrust[0].Value;
                        //移除当前用户节点
                        PMUser pm = entrustUserList.Find(user => user.UserID == CurrentPage.CurrentUserID);
                        if (pm != null)
                        {
                            entrustUserList.Remove(pm);
                        }
                        //移除上一个节点
                        int nodeIndex = this.Workflow.NodeInstances.IndexOf(node);
                        if (nodeIndex > 0)
                        {
                            pm = entrustUserList.Find(user => user.UserID == this.Workflow.NodeInstances[nodeIndex - 1].UserID);
                            if (pm != null)
                            {
                                entrustUserList.Remove(pm);
                            }
                        }
                    }
                }
                ddlEntrustUserList.Items.Clear();
                if (entrustUserList != null && entrustUserList.Count > 0)
                {
                    ddlEntrustUserList.DataSource = entrustUserList;
                    ddlEntrustUserList.DataTextField = "cUserName";
                    ddlEntrustUserList.DataValueField = "UserID";
                    ddlEntrustUserList.DataBind();
                }
                else
                {
                    ddlEntrustUserList.Items.Add(new ListItem() { Value = "0", Text = "无相关委托人" });
                }
            }
        }

        //private string BuildLanchButtons()
        //{
        //    string result = "<input type='button' class='btn4 mr15' value='保存' onclick='UserApproval(\"Save\")'  />";
        //    result += "<input type='button' class='btn4 mr15' value='重置' onclick='UserApproval(\"Reset\")'  />";
        //    result += BuildApprovalButton(LightWorkflow.Common.NodeOperationType.Launch);
        //    return result;
        //}

        private string BuildApprovalButtons()
        {
            string result = string.Empty;
            foreach (var btn in this.Workflow.GetPermissionsList(CurrentPage.CurrentUserID))
            {
                result += BuildApprovalButton(btn);
            }
            return result;
        }

        private string BuildApprovalButton(Wanda.LightWorkflow.Common.NodeOperationType button)
        {
            #region 新增 工作项(-) 设置一个标志,这个标志指示当前用户是否可以审批这个流程
            if (button == LightWorkflow.Common.NodeOperationType.Approved || button == LightWorkflow.Common.NodeOperationType.Launch)
            {
                ViewState["CanApproved"] = true;
                //设置是否可以审批,这个标志在附件控件中使用
                if (!HttpContext.Current.Items.Contains("CanApproved"))
                {
                    HttpContext.Current.Items.Add("CanApproved", true);
                }
            }
            #endregion

            //丁本锡不显示委托按钮
            if (button == LightWorkflow.Common.NodeOperationType.Entrust && this.CurrentPage.CurrentUser.UserName.Equals("dingbenxi"))
            {
                return string.Empty;
            }

            ContextMenu = (ContextMenu == null ? string.Empty : ContextMenu) + string.Format("<li class='Approval' onmousedown=\"$('#AppBtn{0}').click()\">{1}</li>", button, Utilities.EnumDescription(button));
            string result = string.Format("<input type='button' class='btn4 mr15' value='{0}' onclick='UserApproval(\"{1}\")' id='AppBtn{1}'  />", Utilities.EnumDescription(button), button);

            switch (button)
            {

                case LightWorkflow.Common.NodeOperationType.Entrust: //委托,显示只选择一个人的窗口(当前项目,当前用户角色的其他用户列表)
                    {
                        result = string.Format("<input type='button' class='btn4 mr15' value='{0}' onclick='TodoWorkEntrust()'  id='AppBtn{1}' />",
                            Utilities.EnumDescription(button), button);
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Forward: //转发时,显示选择的所有用户的窗口
                    {
                        result = string.Format("<input type='button' class='btn4 mr15' value='{0}' onclick='TodoWorkForward()'  id='AppBtn{1}' />",
                            Utilities.EnumDescription(button), button);
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Cancel:
                    {
                        result = string.Format("<input type='button' class='btn4 mr15' value='{0}' onclick='ShowCancelUserDialog()'  id='AppBtn{1}' />",
                            Utilities.EnumDescription(button), button);
                        break;
                    }
                case LightWorkflow.Common.NodeOperationType.Launch:
                    {
                        //string confirm = string.Format("if(ApprovalConfirm('{0}')) ", Utilities.EnumDescription(button));
                        //if (!string.IsNullOrWhiteSpace(this.ClientValidateFunction))
                        //{
                        //    confirm = string.Format("if(!{1}){{return false;}} if(ApprovalConfirm('{0}')) ", Utilities.EnumDescription(button), ClientValidateFunction);
                        //}
                        result = string.Format("<input type='button' class='btn4 mr15' value='{0}' onclick=\"return btnValidatePage_ClientClick()\"  id='AppBtn{1}' />",
                            Utilities.EnumDescription(button), button);
                        break;
                    }
                default:
                    {
                        result = string.Format("<input type='button' class='btn4 mr15' value='{0}' onclick=\"UserApproval(\'{1}\');\"  id='AppBtn{1}' />",
                            Utilities.EnumDescription(button), button);
                        break;
                    }
            }
            return result;
        }

        private WorkflowStartEventArgs CreateProcessInstance()
        {
            BizProcess bizProcess = BizProcessBll.Instance.LoadBizProcess(BizProcessId);
            List<ProcessNodeInstance> processNodes = new List<ProcessNodeInstance>();
            #region 添加审批节点用户
            for (int i = 0; i < this.ProcessApprovalNodeList.Count; i++)
            {
                var pn = this.ProcessApprovalNodeList[i];
                PMUser user = null;
                if (i == 0)
                {
                    user = CurrentPage.CurrentUser;
                }
                else
                {
                    if (UserList.Exists(item => item.Key.RoleID == pn.Role))
                    {
                        var users = UserList.Find(item => item.Key.RoleID == pn.Role);
                        if (users.Value.Count == 0)
                        {
                            CurrentPage.Alert("未找到审批人，无法发起流程。");
                            return null;
                        }
                        else if (users.Value.Count == 1)
                        {
                            user = users.Value[0];
                        }
                        else
                        {
                            int userid = int.Parse(this.rptApprovalUserList.Items[i].FindDropDownList("ddlUserName").SelectedValue);
                            user = users.Value.Find(item => item.UserID == userid);
                        }
                    }
                }
                ProcessNodeInstance node = WorkflowEngine.BuildNodeInstance(this.BizProcessId, pn, user.UserID, user.cUserID, user.cUserName, user.UserName);
                processNodes.Add(node);
            }

            #endregion

            #region 抄送节点
            for (int i = 0; i < this.ProcessCCNodeList.Count; i++)
            {
                var pn = this.ProcessCCNodeList[i];
                PMUser user = null;
                if (UserList.Exists(item => item.Key.RoleID == pn.Role))
                {
                    var users = UserList.Find(item => item.Key.RoleID == pn.Role);
                    //没有抄送用户
                    if (users.Value.Count == 0)
                    {
                        user = null;
                        continue;
                    }
                    else
                    {
                        if (users.Value.Count == 1)
                        {
                            user = users.Value[0];
                        }
                        else
                        {
                            int userid = int.Parse(this.rptCCUserList.Items[i].FindDropDownList("ddlUserName").SelectedValue);
                            user = users.Value.Find(item => item.UserID == userid);
                        }
                    }
                }
                ProcessNodeInstance node = WorkflowEngine.BuildNodeInstance(this.BizProcessId, pn, user.UserID, user.cUserID, user.cUserName, user.UserName);
                processNodes.Add(node);
            }
            #endregion
            return new WorkflowStartEventArgs(this.ProcessCode, this.BizProcessId, this.BizProcess.TaskID, bizProcess.Title, CurrentPage.CurrentUserID, Utilities.WipeUnsafeContent(this.txtComments.Text), processNodes, ValidateArgs.ProcessContext);
        }

        private void BindApprovalUser(PageValidateEventArgs e)
        {
            var processNodeList = Wanda.LightWorkflow.WorkflowEngine.WorkflowService.GetProcessNodeList(ProcessCode, e.ProcessContext);//最后一个参数传bizprocesscontext
            List<int> roles = new List<int>();
            foreach (var item in processNodeList)
            {
                roles.Add(item.Role);
            }
            ProcessApprovalNodeList = new List<ProcessNode>();
            ProcessCCNodeList = new List<ProcessNode>();

            List<ApprovalProcessNodeShortInfo> shortNodes = new List<ApprovalProcessNodeShortInfo>();
            foreach (var item in processNodeList)
            {
                shortNodes.Add(new ApprovalProcessNodeShortInfo(item.Role, item.NodeName));
                if (item.NodeSeq < Wanda.LightWorkflow.Common.CCNodeSeqNumber)
                {
                    ProcessApprovalNodeList.Add(item);
                }
                if (item.NodeSeq >= Wanda.LightWorkflow.Common.CCNodeSeqNumber && item.NodeSeq < Wanda.LightWorkflow.Common.ArchiveNodeSeqNumber)
                {
                    ProcessCCNodeList.Add(item);
                }
            }

            if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost)
            {
                UserList = PMUserToRoleBll.Instance.GetPMUserByMapRoleAndScope(BizProcessId, shortNodes);
                for (int i = ProcessApprovalNodeList.Count - 1; i >= 0; i--)
                {
                    if (ConstSet.ChangePositionIgnoreRoleList.Contains(ProcessApprovalNodeList[i].Role))
                    {
                        if (UserList.Find(item => item.Key.RoleID == ProcessApprovalNodeList[i].Role).Value.Count == 0)
                        {
                            ProcessApprovalNodeList.RemoveAt(i);
                        }
                    }
                }
            }
            else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport)
            {
                UserList = PMUserToRoleBll.Instance.GetPMUserByRole(roles);
            }
            else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetMortgageLoan)//按揭流程加载人员
            {
                
                UserList = PMUserToRoleBll.Instance.GetPMUserByRole(roles,BizProcessId );
            }
            else if (this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetManagement ||
                this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetEdit ||
                this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetDelete ||
                this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.AssetRemoveMortgage)
            {
                UserList = PMUserToRoleBll.Instance.GetAssetPMUserByRoleAndScope(roles, BizProcessId);
            }
            else
            {
                UserList = PMUserToRoleBll.Instance.GetPMUserByRoleAndScope(roles, BizProcessId);
            }
            //foreach (var item in UserList)
            //{
            //    PMUser pm = item.Value.Find(user => user.UserID == CurrentPage.CurrentUserID);
            //    if (pm != null)
            //    {
            //        item.Value.Remove(pm);
            //    }
            //}

            //当流程只有两个节点时:如果第二个节点只有发起人一个用户,则不能上报,如果有包含发起人在内的多个用户,则在第二个节点上把发起人删除,由其它用户审批.
            //当流程节点有多个时:如果第二个节点上包含发起人,则删除第二个节点,否则走正常流程.
            if (ProcessApprovalNodeList.Count >= 2)
            {
                int secRoleId = ProcessApprovalNodeList[1].Role;
                if (UserList.Exists(item => item.Key.RoleID == secRoleId))
                {
                    var users = UserList.Find(item => item.Key.RoleID == secRoleId);
                    {
                        if (users.Value.Exists(user => user.UserID == CurrentPage.CurrentUserID))
                        {
                            //当前节点数大于2个，且流程不是变更岗位时，如果下一个节点包含发起人，则跳过。
                            if (ProcessApprovalNodeList.Count > 2 && this.BizProcess.ProcessType != (int)Wanda.Financing.Common.Enumerator.ProcessType.ChangePost && this.BizProcess.ProcessType == (int)Wanda.Financing.Common.Enumerator.ProcessType.CeoDesktopDataImport)
                            {
                                //当审批节点只有两个时,不处理,当审批节点多于两个时,删除第二个审批节点
                                ProcessApprovalNodeList.RemoveAt(1);
                            }
                            else
                            {
                                //只有两个审批节点时,把审批权限交给下一个审批节点的其它人.
                                users.Value.Remove(users.Value.Find(user => user.UserID == CurrentPage.CurrentUserID));
                            }
                        }
                    }
                }
            }


            this.lblNoCCUser.Visible = this.ProcessCCNodeList.Count == 0;
            this.ltlApprovalNoCCUser.Visible = this.lblNoCCUser.Visible;

            this.rptApprovalUserList.DataSource = this.ProcessApprovalNodeList;
            this.rptApprovalUserList.DataBind();
            this.rptCCUserList.DataSource = this.ProcessCCNodeList;
            this.rptCCUserList.DataBind();

            CurrentPage.ClientScript.RegisterStartupScript(this.GetType(), "ShowChooseUser", "$(function(){ShowChooseUserDialog();});", true);
        }

        #region 加载树形组织结构
        /// <summary>
        /// 绑定树形的数据
        /// </summary>
        private void BindTreeOrgList()
        {
            treeOrgList.Nodes.Clear();
            //加载第一级树形
            List<WdOrg> wdorgList = new List<WdOrg>();
            wdorgList = Bll.WdOrgBll.Instance.GetChildOrg(null);
            foreach (WdOrg org in wdorgList)
            {
                TreeNode tn = new TreeNode(org.OrgName, org.OrgID.ToString());
                if (org != null)
                {
                    //加载第二级节点
                    List<WdOrg> orgListE = Bll.WdOrgBll.Instance.GetChildOrg(org);
                    if (orgListE != null)
                    {
                        foreach (WdOrg o in orgListE)
                        {
                            TreeNode tn2 = new TreeNode(o.OrgName, o.OrgID.ToString());
                            tn2.ChildNodes.Add(new TreeNode("", "-1"));
                            tn2.Expanded = false;
                            tn.ChildNodes.Add(tn2);
                        }
                    }

                }
                //tn.Expanded = false;
                //tn.ChildNodes.Add(new TreeNode("", "-1"));
                treeOrgList.Nodes.Add(tn);

            }


        }
        /// <summary>
        /// 展开节点时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void treeOrgList_TreeNodeExpanded(object sender, TreeNodeEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn != null)
            {
                if (tn.ChildNodes != null)
                {
                    tn.ChildNodes.Clear();//清空数据在加载子节点
                }
                if (tn.Value != "-1")
                {
                    WdOrg wd = new WdOrg();
                    wd.OrgName = tn.Text;
                    wd.OrgID = Convert.ToInt32(tn.Value);
                    List<WdOrg> orgList = Bll.WdOrgBll.Instance.GetChildOrg(wd);
                    if (orgList != null)
                    {
                        foreach (WdOrg org in orgList)
                        {
                            TreeNode tree = new TreeNode(org.OrgName, org.OrgID.ToString());
                            tree.Expanded = false;
                            tree.Text = string.Format("<div id=\"{0}\" onclick=\"GetTreeNodeValue('{0}')\">{1}</div>", org.OrgID.ToString(), org.OrgName);
                            tree.ChildNodes.Add(new TreeNode("", "-1"));
                            tn.ChildNodes.Add(tree);
                        }
                    }
                }
            }
        }
        //当点击树形节点时 加载树形组织结构
        protected void treeOrgList_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (treeOrgList.SelectedNode != null)
            {
                if (treeOrgList.SelectedNode.Expanded == false)//节点未展开时
                {
                    if (treeOrgList.SelectedNode.ChildNodes != null)
                    {
                        treeOrgList.SelectedNode.ChildNodes.Clear();//清空数据在加载子节点
                    }
                    if (treeOrgList.SelectedNode.Value != "-1")
                    {
                        WdOrg wd = new WdOrg();
                        //wd.OrgName = treeOrgList.SelectedNode.Text;
                        wd.OrgID = Convert.ToInt32(treeOrgList.SelectedNode.Value);
                        List<WdOrg> orgList = Bll.WdOrgBll.Instance.GetChildOrg(wd);
                        if (orgList != null)
                        {
                            foreach (WdOrg org in orgList)
                            {
                                TreeNode tree = new TreeNode(org.OrgName, org.OrgID.ToString());
                                tree.Expanded = false;
                                tree.Text = string.Format("<div id=\"{0}\" onclick=\"GetTreeNodeValue('{0}')\">{1}</div>", org.OrgID.ToString(), org.OrgName);

                                tree.ChildNodes.Add(new TreeNode("", "-1"));
                                treeOrgList.SelectedNode.ChildNodes.Add(tree);
                            }
                        }
                    }
                    treeOrgList.SelectedNode.Expanded = true;//展开节点
                    //设置选择节点的状态 让下次点击节点时 触发本事件
                    if (treeOrgList.SelectedNode.Selected == true)
                    {
                        treeOrgList.SelectedNode.Selected = false;
                    }
                    else
                    {
                        treeOrgList.SelectedNode.Selected = true;
                    }
                }
                else
                {
                    treeOrgList.SelectedNode.Expanded = false;//合闭节点
                    //设置选择节点的状态 让下次点击节点时 触发本事件
                    if (treeOrgList.SelectedNode.Selected == true)
                    {
                        treeOrgList.SelectedNode.Selected = false;
                    }
                    else
                    {
                        treeOrgList.SelectedNode.Selected = true;
                    }
                }
            }
        }
        #endregion



        #endregion
    }
    public delegate bool PageValidateEventHandler(object sender, PageValidateEventArgs e);

    [Serializable]
    public class PageValidateEventArgs : EventArgs
    {
        private System.Collections.Hashtable processContext = new System.Collections.Hashtable();

        public void AddProcessContext(Wanda.Financing.Common.Enumerator.ProcessContextName name, object value)
        {
            if (this.processContext.Contains(name.ToString()))
            {
                this.processContext[name.ToString()] = value;
            }
            else
            {
                this.processContext.Add(name.ToString(), value);
            }
        }

        public bool Exist(Wanda.Financing.Common.Enumerator.ProcessContextName name)
        {
            return processContext.Contains(name);
        }

        public object this[Wanda.Financing.Common.Enumerator.ProcessContextName name]
        {
            get
            {
                return processContext[name.ToString()];
            }
            set
            {
                processContext[name.ToString()] = value;
            }
        }

        public System.Collections.Hashtable ProcessContext
        {
            get
            {
                return this.processContext;
            }
        }
    }

    public class SearchUserAjax
    {
        /// <summary>
        /// 如果为null 则返回""
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string GetString(string str)
        {
            string s = "";
            if (str == null)
            {
                return s;
            }
            return str;

        }
        /// <summary>
        /// 根据输入的用户名称信息 查询前30条用户信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="deptName">部门名称</param>
        /// <param name="jobName">职位名称</param>
        /// <returns></returns>
        [Wanda.Financing.Web.AjaxMethod(SerializeType = Wanda.Financing.Web.SerializeType.JSON, ClientFunction = "AjaxSearchUser")]
        public List<PMUserShortInfo> SearchUser(string keyword, string deptName, string jobName)
        {
            int recordcount = 0;
            keyword = System.Web.HttpUtility.UrlDecode(keyword).Trim();
            //List<PMUser> listPMUser = PMUserBll.Instance.PMUserScopeEnum(keyword, null, 1, 30000, -1, null, 1, out recordcount);
            List<PMUser> listPMUser = WdOrgBll.Instance.GetSearchOrgUsers(keyword, deptName, jobName);
            #region 设置部门，职位的查询条件
            //if (listPMUser != null && listPMUser.Count > 0)
            //{
            //    if (deptName != null && deptName.Trim() != string.Empty)
            //    {
            //        var v = listPMUser.Where(p => GetString(p.deptname).IndexOf(deptName.Trim()) > 0 || GetString(p.deptname).StartsWith(deptName.Trim()) || GetString(p.deptname).EndsWith(deptName.Trim()));
            //        if (v != null)
            //        {
            //            listPMUser = v.ToList();
            //        }
            //        else
            //        {
            //            listPMUser = null;
            //        }
            //    }
            //}
            //if (listPMUser != null && listPMUser.Count > 0)
            //{
            //    if (jobName != null && jobName.Trim() != string.Empty)
            //    {
            //        var v = listPMUser.Where(p => GetString(p.jobname).IndexOf(jobName.Trim()) > 0 || GetString(p.jobname).StartsWith(jobName.Trim()) || GetString(p.jobname).EndsWith(jobName.Trim()));
            //        if (v != null)
            //        {
            //            listPMUser = v.ToList();
            //        }
            //        else
            //        {
            //            listPMUser = null;
            //        }
            //    }
            //}
            #endregion

            return FormatUserInfo(listPMUser);
        }
        /// <summary>
        /// 根据所选择的树形节点机构去加载用户
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [Wanda.Financing.Web.AjaxMethod(SerializeType = Wanda.Financing.Web.SerializeType.JSON, ClientFunction = "AjaxSearchOrgUser")]
        public List<PMUserShortInfo> AjaxSearchOrgUser(string orgid)
        {
            orgid = System.Web.HttpUtility.UrlDecode(orgid).Trim();
            List<PMUser> listPMUser = WdOrgBll.Instance.GetSearchOrgUser(orgid);
            return FormatUserInfo(listPMUser);
        }


        private List<PMUserShortInfo> FormatUserInfo(List<PMUser> users)
        {
            List<PMUserShortInfo> result = new List<PMUserShortInfo>();
            if (users == null || users.Count == 0)
            {
                return result;
            }
            int maxCUserNameLength = users.Max(item => item.cUserName.Sum(x => x < 128 ? 1 : 2));
            int maxDeptNameLength = users.Max(item => item.deptname.Sum(x => x < 128 ? 1 : 2));

            maxCUserNameLength += maxCUserNameLength % 2;
            maxCUserNameLength = maxCUserNameLength > 10 ? maxCUserNameLength : 10;

            maxDeptNameLength = maxDeptNameLength > 28 ? 28 : maxDeptNameLength;
            int totalDetpNameLength = maxDeptNameLength + 4;
            foreach (var user in users)
            {
                int length = user.cUserName.Sum(x => x < 128 ? 1 : 2);
                user.cUserName = user.cUserName + new string(' ', maxCUserNameLength - length);

                length = user.deptname.Sum(x => x < 128 ? 1 : 2);

                if (length <= maxDeptNameLength)
                {
                    user.deptname = user.deptname + new string(' ', totalDetpNameLength - length);
                    result.Add(new PMUserShortInfo()
                    {
                        UserId = user.UserID,
                        UserCode = user.UserName,
                        Job = user.jobname,
                        UserDepts = user.deptname.TrimEnd(),
                        UserName = user.cUserName.Replace(" ", "&nbsp;"),
                        UserDept = user.deptname.Replace(" ", "&nbsp;")
                    });
                    continue;
                }
                string deptname = user.deptname;
                user.deptname = string.Empty;
                length = 0;
                foreach (var c in deptname)
                {
                    if (length + (c > 128 ? 2 : 1) <= maxDeptNameLength)
                    {
                        user.deptname += c;
                        length += (c > 128 ? 2 : 1);

                    }
                    else
                    {
                        user.deptname += new string(' ', totalDetpNameLength - length);
                        break;
                    }
                }
                result.Add(new PMUserShortInfo()
                {
                    UserId = user.UserID,
                    UserCode = user.UserName,
                    Job = user.jobname,
                    UserDepts = deptname,
                    UserName = user.cUserName.Replace(" ", "&nbsp;"),
                    UserDept = user.deptname.Replace(" ", "&nbsp;")
                });
            }
            return result;
        }
        public class PMUserShortInfo
        {
            public int UserId { get; set; }
            public string UserCode { get; set; }
            public string UserName { get; set; }
            public string UserDept { get; set; }
            public string Job { get; set; }
            public string UserDepts { get; set; }
        }
    }
}