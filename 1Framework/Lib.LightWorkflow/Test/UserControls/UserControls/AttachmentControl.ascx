<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentControl.ascx.cs"
    Inherits="Wanda.Financing.Web.UserControls.AttachmentControl" %>
<input type="hidden" value="<%=UploadUrl %>" id="hidUploadUrl" />
<input type="hidden" value="<%= DeleteImgUrl %>" id="hidDeleteImgUrl" />
<input type="hidden" value="<%= PreviewImgUrl %>" id="hidExcelPreviewUrl" />
<%if (UseWideMode)
  { %>
<div class="fonrm_content_title <%} %> <%= CssClass %> <%if (UseWideMode)
  { %>" id="paneUploadForm">
    <input type="hidden" value="<%= TableName %>" name="TableName" class="upLoad" />
    <input type="hidden" value="<%= TableRecordId %>" name="TableRecordId" class="upLoad" />
    <input type="hidden" value="<%= TableKeyName %>" name="TabbleKeyName" class="upLoad" />
    <span id="paneAddFile" class="addFile" runat="server" clientidmode="Static" style="float: right">
        <input type="button" class="btn4" id="btnAddFile" value="添加附件" />
    </span><span id="Span1" class="addFile" runat="server" clientidmode="Static" style="float: right;
        width: 660px; text-align: left;">
        <asp:HyperLink runat="server" ID="hylinkTemplateFile" Target="_blank" NavigateUrl="~/TemplateFile/融资摸底表单.rar"
            Text="请下载银行评估和银行反馈模板" Visible="false"></asp:HyperLink>
        <asp:HyperLink runat="server" ID="hyCeoExcelTemplateFile" Target="_blank" NavigateUrl="~/TemplateFile/商业地产现金流报表.xlsx"
            Text="请下载总裁桌面现金流模板" Visible="false"></asp:HyperLink>
        <asp:HyperLink runat="server" ID="linkTemplateFinancingplan" Target="_blank" NavigateUrl="~/TemplateFile/审批节点控制表.xlsx"
            Text="请下载审批节点控制表模板" Visible="false"></asp:HyperLink>
    </span>附件列表
</div>
<div class="fonrm_content_1 <%} %> <%= CssClass %> <%if (UseWideMode)
  { %>">
    <%} %>
    <table border="0" cellpadding="0" cellspacing="0" class="<%= UseWideMode ? "form_table" : "bd_table"%>"
        <%= UseWideMode ? "" : "style='width:740px;border-right:#cadaf1 1px solid'"%>
        id="tbUploadList">
        <tr id="trAttach">
            <th width="40px">
                编号
            </th>
            <th class="form_table_tl">
                附件名称
            </th>
            <th width="60px">
                上传用户
            </th>
            <th width="160px">
                保存时间
            </th>
            <%if (UseEditMode)
              {%>
            <th width="80">
                <asp:Literal runat="server" ID="ltlCtrl" ClientIDMode="Static">操作</asp:Literal>
            </th>
            <%} %>
        </tr>
        <tr id="trEmptyPane" clientidmode="Static" runat="server">
            <td style="line-height: 84px" colspan="6">
                无附件信息
            </td>
        </tr>
        <asp:Repeater runat="server" ID="rptAttachment" OnItemDataBound="rptAttachment_DataBound">
            <ItemTemplate>
                <tr class="attrow">
                    <td>
                        <span class="spnAttachmentIdx">
                            <%# Container.ItemIndex + 1 %></span>
                    </td>
                    <td style="text-align: left">
                        <asp:HyperLink runat="server" ID="hyFileUrl" Target="_blank" Text='<%# Eval("FileName")%>'></asp:HyperLink>
                    </td>
                    <td>
                        <%# Eval("CUserName") %>
                    </td>
                    <td>
                        <%# Convert.ToDateTime( Eval("CreateDate")).ToString("yyyy-MM-dd HH:mm:ss")%>
                    </td>
                    <%if (UseEditMode)
                      {%>
                    <td id="ctlOperation" class="AttachmentControl" runat="server" style="text-align: center"
                        clientidmode="Static">
                        <asp:Image ImageUrl="~/Images/cross.png" ID="ibnDelete" ToolTip="删除" runat="server"
                            Style="cursor: pointer;" Width="16" Height="16" ImageAlign="AbsMiddle" />
                        <asp:Image ImageUrl="~/Image/ceico.gif" ID="ibnPreview" ToolTip="预览" runat="server"
                            Style="cursor: pointer;" Width="16" Height="16" ImageAlign="AbsMiddle" />
                    </td>
                    <%} %>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <%if (UseWideMode)
      { %></div>
<%} %>