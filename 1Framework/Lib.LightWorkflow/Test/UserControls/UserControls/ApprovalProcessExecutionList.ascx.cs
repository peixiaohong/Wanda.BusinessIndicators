using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.Financing.Common;
using Wanda.Financing.Entities;
using Wanda.Financing.Bll;

namespace Wanda.Financing.Web.UserControls
{
    public partial class ApprovalProcessExecutionList : BaseUserControl
    {
        #region 公有成员
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
            CtlApprovalExCurrent.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx1.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx2.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx3.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx4.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx5.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx6.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx7.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
            CtlApprovalEx8.CreateWorkflowInstanceUrl = CreateWorkflowInstanceUrl;
        }

        #region 私有方法
        private void BindData()
        {
            #region
            CtlApprovalExCurrent.BizStageID = BizStageID;
            CtlApprovalExCurrent.ProcessTypeList = null;

            CtlApprovalEx1.BizStageID = BizStageID;
            CtlApprovalEx1.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.LoanRepayPlanAdjustment };

            CtlApprovalEx2.BizStageID = BizStageID;
            CtlApprovalEx2.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.LoanImplementationApproval, Enumerator.ProcessType.LoanImplementationApply };

            CtlApprovalEx3.BizStageID = BizStageID;
            CtlApprovalEx3.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.OntimeReimbursementApproval, Enumerator.ProcessType.OntimeReimbursementApply, Enumerator.ProcessType.EarlyRepaymentApproval, Enumerator.ProcessType.EarlyRepaymentApply };

            CtlApprovalEx4.BizStageID = BizStageID;
            CtlApprovalEx4.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.InterestPaymentApproval, Enumerator.ProcessType.InterestPaymentApply, Enumerator.ProcessType.RepayFinancingCost, Enumerator.ProcessType.RepayFinancingCostApply };

            CtlApprovalEx5.BizStageID = BizStageID;
            CtlApprovalEx5.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.InterestRateAdjustment, Enumerator.ProcessType.ChangeContract };

            CtlApprovalEx6.BizStageID = BizStageID;
            CtlApprovalEx6.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.CollateralMortgage };

            CtlApprovalEx7.BizStageID = BizStageID;
            CtlApprovalEx7.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.ChangeTaskInfo };

            CtlApprovalEx8.BizStageID = BizStageID;
            CtlApprovalEx8.ProcessTypeList = new List<Enumerator.ProcessType>() { Enumerator.ProcessType.CommonProcess };

            #endregion
        }
        private string CreateWorkflowInstanceUrl(Wanda.LightWorkflow.WorkflowInstance wf)
        {
            #region
            BizProcess bp = BizProcessBll.Instance.LoadBizProcess(wf.ProcessInstance.BizProcessID);
            string strUrlTop = "~/Projects/FinancingExecution";
            string strUrl = "";
            int intWfbpid = wf.ProcessInstance.BizProcessID;
            switch (bp.ProcessType)
            {
                case (int)Enumerator.ProcessType.LoanRepayPlanAdjustment:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/LoanRepayPlan/Edit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/LoanRepayPlan/Approval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.LoanImplementationApproval:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Loan/Edit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Loan/Approval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.OntimeReimbursementApproval:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Repayment/OnTimeRepaymentEdit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Repayment/OnTimeRepaymentApproval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.EarlyRepaymentApproval:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Repayment/EarlyRepaymentEdit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Repayment/EarlyRepaymentApproval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.InterestPaymentApproval:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Repayment/InterestEdit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Repayment/InterestApproval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.RepayFinancingCost:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Repayment/CostEdit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Repayment/CostApproval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.InterestRateAdjustment:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Interest/Edit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Interest/Approval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.CollateralMortgage:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Repayment/GuaranteeEdit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Repayment/GuaranteeApproval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.ChangeContract:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/ChangeContract/Edit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/ChangeContract/Approval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.ChangeTaskInfo:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/ChangeTaskInfo/ChangeProjectsInfoEdit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/ChangeTaskInfo/ChangeProjectsInfoApproval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.EarlyRepaymentApply:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Confirm/EarlyRepaymentApply.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Confirm/EarlyRepaymentConfirm.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.InterestPaymentApply:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Confirm/InterestApply.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Confirm/InterestConfirm.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.RepayFinancingCostApply:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Confirm/CostApply.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Confirm/CostConfirm.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.LoanImplementationApply:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Confirm/LoanApply.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Confirm/LoanConfirm.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.OntimeReimbursementApply:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("{1}/Confirm/OnTimeRepaymentApply.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("{1}/Confirm/OnTimeRepaymentConfirm.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
                case (int)Enumerator.ProcessType.CommonProcess:
                    if (wf.ProcessInstance.Status == (int)Wanda.LightWorkflow.Common.ProcessInstanceStatus.Rejected)
                    {
                        strUrl = string.Format("~/Projects/Commons/CommonProcess/Edit.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    else
                    {
                        strUrl = string.Format("~/Projects/Commons/CommonProcess/Approval.aspx?BizProcessID={0}", intWfbpid, strUrlTop);
                    }
                    break;
            }
            return strUrl;
            #endregion
        }
        #endregion
    }
}