<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalProcessExecutionList.ascx.cs" Inherits="Wanda.Financing.Web.UserControls.ApprovalProcessExecutionList" %>
<%@ Register Src="~/UserControls/ApprovalProcessExecution.ascx" TagName="ApprovalExecution"
    TagPrefix="uc1" %>
<div class="main_step_listtitle">
    审批流程</div>
<div class="main_step_listcontent mb8">
    <ul id="ulDraft" class="nav0104">
        <li alt="#drloancurrent" class="nav0104_active">当前流程</li>
        <li alt="#drloanrepayplan">放还款计划</li>
        <li alt="#drloan">放款流程</li>
        <li alt="#drontimerepay">还款流程</li>
        <li alt="#drearlyrepa">支付融资成本</li>
        <li alt="#drinterestpay">调整利率/合同</li>
        <li alt="#drcostpay">调整抵押担保</li>
        <li alt="#drinterest">维护基础信息</li>
        <li alt="#drcommon">通用流程</li>
    </ul>
    <table border="0" cellpadding="0" cellspacing="0">
        <tr id="drloancurrent" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalExCurrent" runat="server" />
            </td>
        </tr>
        <tr id="drloanrepayplan" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx1" runat="server" />
            </td>
        </tr>
        <tr id="drloan" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx2" runat="server" />
            </td>
        </tr>
        <tr id="drontimerepay" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx3" runat="server" />
            </td>
        </tr>
        <tr id="drearlyrepa" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx4" runat="server" />
            </td>
        </tr>
        <tr id="drinterestpay" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx5" runat="server" />
            </td>
        </tr>
        <tr id="drcostpay" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx6" runat="server" />
            </td>
        </tr>
        <tr id="drinterest" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx7" runat="server" />
            </td>
        </tr>
        <tr id="drcommon" class="trdraftlist">
            <td>
                <uc1:ApprovalExecution id="CtlApprovalEx8" runat="server" />
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript">
    $(function ()
    {
        $("table tr.trdraftlist").hide().filter($("#ulDraft li.nav0104_active").attr("alt")).show();
        $("#ulDraft > li").css({"padding":"0px 6px","margin":"0px 4px"}).click(function ()
        {
            $("#ulDraft li").removeClass("nav0104_active").filter(this).addClass("nav0104_active");
            $("table tr.trdraftlist").hide().filter($(this).attr("alt")).show();
            autoheight();
        });
        autoheight();
    });
</script>
