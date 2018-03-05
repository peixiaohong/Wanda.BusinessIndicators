<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalProcessExecution.ascx.cs" Inherits="Wanda.Financing.Web.UserControls.ApprovalProcessExecution" %>
<table border="0" cellpadding="0" cellspacing="0" class="bd_table">
    <tr>
        <th width="60">
            状态
        </th>
        <th>
            流程名称
        </th>
        <th width="60">
            发起人
        </th>
        <th width="135">
            发起日期
        </th>
        <th width="135">
            最后修改时间
        </th>
    </tr>
    <asp:Repeater runat="server" ID="rptWorkflowList" OnItemDataBound="rptProcessList_ItemDataBound">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Literal runat="server" ID="ltlStatus"></asp:Literal>
                </td>
                <td style="text-align: left; padding-left: 5px">
                    <asp:HyperLink runat="server" ID="hyProcessName" Target="_blank"></asp:HyperLink>
                </td>
                <td>
                    <asp:Literal runat="server" ID="ltlCreateUser"></asp:Literal>
                </td>
                <td>
                    <asp:Literal runat="server" ID="ltlCreateTime"></asp:Literal>
                </td>
                <td>
                    <asp:Literal runat="server" ID="ltlLastUpdateTime"></asp:Literal>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
    <tr id="trEmptyPane" clientidmode="Static" runat="server" visible="false">
        <td style="line-height: 84px" colspan="6">
            无审批信息
        </td>
    </tr>
</table>
