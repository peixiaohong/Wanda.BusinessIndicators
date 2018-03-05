<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProcessApprovalControl.ascx.cs"
    Inherits="Wanda.Financing.Web.UserControls.ProcessApprovalControl" %>
<style type="text/css">
    .jqmWindow
    {
        display: none;
        position: fixed;
        top: 17%;
        left: 50%;
        margin-left: -350px;
        width: 700px;
        background-color: #FFF;
        color: #333;
        border: 1px solid black;
        padding: 12px;
    }
    
    .jqmOverlay
    {
        background-color: #000;
    }
    
    * iframe.jqm
    {
        position: absolute;
        top: 0;
        left: 0;
        z-index: -1;
        width: expression(this.parentNode.offsetWidth+'px');
        height: expression(this.parentNode.offsetHeight+'px');
    }
    
    * html .jqmWindow
    {
        position: absolute;
        top: expression((document.documentElement.scrollTop || document.body.scrollTop) + Math.round(17 * (document.documentElement.offsetHeight || document.body.clientHeight) / 100) + 'px');
    }
    
    * html .jqmWindow
    {
        position: absolute;
        top: expression((document.documentElement.scrollTop || document.body.scrollTop) + Math.round(17 * (document.documentElement.offsetHeight || document.body.clientHeight) / 100) + 'px');
    }
    .jqmWindowSignUp
    {
        display: none;
        position: fixed;
        top: 17%;
        left: 50%;
        margin-left: -300px;
        height: 450px;
        width: 600px;
        background-color: transparent;
        color: #333;
        border: 0px solid black;
        padding: 0px;
    }
    .jqmWindowSignUp .ui-dialog
    {
        background-color: #fff;
    }
    .jqmWindowSignUpWide
    {
        display: none; /* position: fixed; 因老总放大页面200%，所以改动定位为absolute*/
        position: absolute;
        top: 0;
        left: 50%;
        margin-left: -400px;
        height: 370px; /*修改450*/
        width: 800px;
        background-color: #FFF;
        color: #333;
        border: 0px solid black;
        padding: 0px;

    }
    .jqmWindowSignUp .BoxMain
    {
       /* height: 330px; 修改400*/
    }
    .jqmWindowSignUpWide .BoxMain
    {
      /*  height: 330px;*/
      padding:6px 0;
    }
    .jqmWindowSignUp .BoxMain select
    {
        height: 20px;
    }
    .jqmWindowSignUp .BoxMain select, .jqmWindowSignUp .BoxMain textarea
    {
        border: #e2e3ea solid 1px;
        border-top: #abadb3 solid 1px;
    }
    * html .jqmWindowSignUp
    {
        position: absolute;
        top: expression((document.documentElement.scrollTop || document.body.scrollTop) + Math.round(17 * (document.documentElement.offsetHeight || document.body.clientHeight) / 100) + 'px');
    }
    div.paneRequestApproval
    {
        text-align: right;
    }
    table.currentuser td
    {
        font-weight: bold;
    }
    .list_btn{ margin:5px; background-position:center top; }
    .main_bd_tab,.main_bd_tab_content{ width:743px;}
    .main_bd_tab_content{ height:120px; overflow-y:auto;overflow-x:hidden ;border:#93a6b4 solid 1px; position:relative; z-index:4;}
    .main_bd_tab{  height:22px; }
    .main_bd_tab li{ background-color:#fff; border:#93a6b4 solid 1px; height:22px; line-height:22px; margin-right:5px; background-image:url(../../../Images/btn_view2.jpg); border-bottom:none; z-index:1; top:1px;+top:0}
    .main_bd_tab .main_bd_tab_active{ top:1px;+top:0; z-index:50px;}
    .new_search_table td{ padding:10px;}
    .box_table{ padding-left:23px;}
    div#paneplan *{font-family: 新宋体,宋体; font-size: 14px;}
</style>
<asp:Panel CssClass="paneRequestApproval" runat="server" ID="paneRequestApproval">
    <span class="paneApprovalButtonList">
        <asp:Button runat="server" CssClass="btn4 mr15" ID="btnRequestSaveDraft" Text="保　存"
            UseSubmitBehavior="false" OnClientClick="return btnSaveDraft_ClientClick()" />
        <asp:Button runat="server" CssClass="btn4 mr15" ID="btnDeleteProcess" Text="重　置"
            OnClientClick="if(window.confirm('你确认要删除这个草稿么?')){ShowBlockUI('正在删除');setTimeout(btnActualDeleteProcessClick, 800) ;}return false;"
            UseSubmitBehavior="false" />
        <asp:Button runat="server" ID="btnValidatePage" CssClass="btn4" Text="上　报" OnClientClick="return btnValidatePage_ClientClick()"
            UseSubmitBehavior="false" />
    </span>
</asp:Panel>
<asp:Panel runat="server" ID="PaneApproval">
    <div class="fonrm_content_title pint_hide">
        <span class="paneApprovalButtonList">
            <asp:Button runat="server" ID="btnSaveDraft" CssClass="btn4 mr15 " Text="保存" OnClientClick="return btnSaveDraft_ClientClick()" />
            <asp:Literal runat="server" ID="ltlApprovalButtonList"></asp:Literal></span>
        审 批</div>
    <div class="fonrm_content_1 mb8 pint_hide">
        <table class="form_xmjbxx" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <th width="84" valign="top">
                    审批流程：
                    <div style="display: none">
                        <asp:HiddenField runat="server" ID="hidApprovalOperation" />
                        <asp:Button runat="server" ClientIDMode="Static" ID="bnApprovalSubmit" OnClick="btnApprovalSubmit_Click">
                        </asp:Button>
                        <asp:Button runat="server" ID="Button1" ClientIDMode="Static" OnClick="btnSaveDraft_Click" />
                    </div>
                </th>
                <td colspan="2">
                    <asp:Repeater runat="server" ID="rptApprovalingUserList" OnItemDataBound="rptApprovalingUserList_ItemDataBound">
                        <ItemTemplate>
                            <table id="TbUser" runat="server" style="float: left; border-collapse: collapse">
                                <tr>
                                    <td style="padding: 0px">
                                        <nobr><asp:Literal runat="server" ID="ltlRoleName"></asp:Literal></nobr>
                                    </td>
                                    <td style="padding: 0px">
                                        <nobr>【<span rel="e" class="totitle" title='<%# Eval("UserCode") %>'><%# Eval("UserName") %></span>】</nobr>
                                    </td>
                                    <td runat="server" id="TbCellTick">
                                        <asp:Image runat="server" ID="imgTick" ImageAlign="AbsMiddle" ImageUrl="~/Images/tick.png" />
                                    </td>
                                    <td runat="server" id="TbCellArrow">
                                        <asp:Image runat="server" ID="imgArrowRight" ImageUrl="~/Images/arrow_right2.png" />
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr>
                <th width="84" valign="top">
                    抄送人：
                </th>
                <td colspan="2">
                    <asp:Repeater runat="server" ID="rptCCingUserList" OnItemDataBound="rptCCingUserList_ItemDataBound">
                        <ItemTemplate>
                            <table style="float: left">
                                <tr>
                                    <td style="padding: 2px">
                                        <nobr><asp:Literal runat="server" ID="ltlRoleName"></asp:Literal></nobr>
                                    </td>
                                    <td style="padding: 2px">
                                        <nobr>【<span rel="e" class="totitle" title='<%# Eval("UserCode") %>'><%# Eval("UserName") %></span>】</nobr>
                                    </td>
                                    <td runat="server" id="TbCellArrow">
                                        ，
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label runat="server" ID="ltlApprovalNoCCUser" Text="无" Visible="false"></asp:Label>
                </td>
            </tr>
            <tr id="trApprovalNode" runat="server">
                <th style="vertical-align: top;" rowspan="2">
                    签字意见：
                </th>
                <td rowspan="2">
                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtApprovalNote" ClientIDMode="Static"
                        CssClass="textarea800" Style="height: 100px; width: 820px;"></asp:TextBox>
                </td>
                <td style="vertical-align: top; text-align: center">
                    <span class="star">*</span><br />
                </td>
            </tr>
            <tr id="trApprovalFlags" runat="server" style="height: 10px">
                <td valign="bottom">
                    <img title="加长签字意见" alt="加长签字意见" src='<%=ResolveUrl("~/Images/AddHeight.jpg") %>'
                        onclick="javascript:ToggleApprovalHeight(this,'<%=txtApprovalNote.ClientID %>');"
                        style="cursor: pointer;" />
                </td>
            </tr>
        </table>
    </div>
    <div class="fonrm_content_title">
        审批日志</div>
    <div class="fonrm_content_1 mb8">
        <table border="0" cellpadding="0" cellspacing="0" class="form_table">
            <tr>
                <th width="200">
                    节 点
                </th>
                <th>
                    审批意见
                </th>
                <th width="70">
                    审批人
                </th>
                <th width="170">
                    审批时间
                </th>
                <th width="50">
                    操作
                </th>
            </tr>
            <asp:Repeater runat="server" ID="rptApprovalLog">
                <ItemTemplate>
                    <tr>
                        <td style="text-align: left">
                            <%# Eval("NodeName") %>
                        </td>
                        <td style="text-align: left">
                            <%# Eval("ApprovalNote").ToString().Replace("\r\n","<br />") %>
                        </td>
                        <td>
                            <span rel="e" class="totitle" title='<%# Eval("UserCode") %>'>
                                <%# Eval("UserName") %></span>
                        </td>
                        <td>
                            <%# Convert.ToDateTime( Eval("CompletedTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                        </td>
                        <td>
                            <%# Wanda.Financing.Common.Utilities.EnumDescription((Wanda.LightWorkflow.Common.NodeOperationType) Convert.ToInt32( Eval("OperationType"))) %>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
</asp:Panel>
<div id="divChooseUserDialog" class="jqmWindowSignUp">
    <div class="ui-dialog ui-widget ui-widget-content ui-corner-all Boxcontent" style="width: 600px">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix"
            style="height: 24px">
            <span class="ui-dialog-title">请选择审批人和抄送人</span> <a class="ui-dialog-titlebar-close ui-corner-all"
                href="javascript:HideChooseUserDialog()" role="button"><span class="ui-icon ui-icon-closethick">
                    close</span> </a>
        </div>
        <div class="BoxMain" style="text-decoration: none; font-weight: normal; height: 400px;">
            <div style="padding: 10px">
                <table style="width: 100%">
                    <tr style="line-height: 26px">
                        <td style="width: 70px; text-align: right" valign="top">
                            审批流程：
                        </td>
                        <td>
                            <asp:Repeater runat="server" ID="rptApprovalUserList" OnItemDataBound="rptApprovalUserList_ItemDataBound">
                                <ItemTemplate>
                                    <table style="float: left">
                                        <tr style="line-height: 28px">
                                            <td style="padding: 0px 2px">
                                                <nobr><asp:Literal runat="server" ID="ltlRoleName"></asp:Literal></nobr>
                                            </td>
                                            <td style="padding: 0px 2px">
                                                <nobr>【<asp:Literal runat="server" ID="ltlUserName"></asp:Literal><asp:DropDownList runat="server"
                                                    ID="ddlUserName">
                                                </asp:DropDownList>】</nobr>
                                            </td>
                                            <td runat="server" id="TbCellArrow">
                                                <asp:Image runat="server" ID="imgArrowRight" ImageUrl="~/Images/arrow_right.png" />
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr style="line-height: 26px">
                        <td style="text-align: right" valign="top">
                            抄送人：
                        </td>
                        <td>
                            <asp:Repeater runat="server" ID="rptCCUserList" OnItemDataBound="rptCCUserList_ItemDataBound">
                                <ItemTemplate>
                                    <table style="float: left">
                                        <tr style="line-height: 28px">
                                            <td style="padding: 0px">
                                                <nobr><asp:Literal runat="server" ID="ltlRoleName"></asp:Literal></nobr>
                                            </td>
                                            <td style="padding: 0px">
                                                <nobr>【<asp:Literal runat="server" ID="ltlUserName"></asp:Literal><asp:DropDownList runat="server"
                                                    ID="ddlUserName">
                                                </asp:DropDownList>】</nobr>
                                            </td>
                                            <td runat="server" id="TbCellArrow">
                                                ，
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Label runat="server" ID="lblNoCCUser" Text="无" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="text-align: right" rowspan="2">
                            签字：
                        </td>
                        <td rowspan="2">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtComments" ClientIDMode="Static"
                                Width="99%" Height="100px"></asp:TextBox>
                        </td>
                        <td style="width: 10px; vertical-align: top">
                            <span class="star">*</span><br />
                        </td>
                    </tr>
                    <tr style="height: 10px">
                        <td valign="bottom">
                            <img title="加长签字意见" alt="加长签字意见" src='<%=ResolveUrl("~/Images/AddHeight.jpg") %>'
                                onclick="javascript:ToggleApprovalHeight(this,'<%=txtComments.ClientID %>');"
                                style="cursor: pointer;" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <div class="ui-dialog-buttonset" style="line-height: 28px">
                <asp:Button runat="server" ID="btnSubmit" CssClass="btn4" Text="上　报" UseSubmitBehavior="false"
                    OnClientClick="return RequestApprovalConfirm()"></asp:Button>
                <input class="btn4" value="取　消" type="button" onclick="HideChooseUserDialog()" />
                <asp:HiddenField runat="server" ID="HidChooseUsers" ClientIDMode="Static" />
            </div>
        </div>
    </div>
    <div style="display: none">
        <asp:Button runat="server" ID="btnActualSubmit" ClientIDMode="Static" OnClick="btnApproval_Click" />
        <asp:Button runat="server" ID="btnActualDeleteProcess" ClientIDMode="Static" OnClick="btnDeleteProcess_Click" />
        <asp:Button runat="server" ID="btnActualSaveDraft" ClientIDMode="Static" OnClick="btnSaveDraft_Click" />
        <asp:Button runat="server" ID="btnActualValidatePage" ClientIDMode="Static" OnClick="btnValidatePage_Click" />
    </div>
</div>
<div id="divEntrustUserDialog" class="jqmWindowSignUp">
    <div class="ui-dialog ui-widget ui-widget-content ui-corner-all Boxcontent" style="width: 600px">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix"
            style="height: 24px">
            <span class="ui-dialog-title">请选择委托人</span> <a class="ui-dialog-titlebar-close ui-corner-all"
                href="javascript:HideEntrustUserDialog()" role="button"><span class="ui-icon ui-icon-closethick">
                    close</span> </a>
        </div>
        <div class="BoxMain" style="text-decoration: none; font-weight: normal">
            <div style="padding: 10px">
                <table style="width: 570px; line-height: 28px">
                    <tr>
                        <td style="width: 90px; text-align: right">
                            请选择委托人：
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlEntrustUserList" Width="100px">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 10px">
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; text-align: right" rowspan="2">
                            签字意见：
                        </td>
                        <td rowspan="2">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtEntrust" ClientIDMode="Static"
                                Width="99%" Height="100px"></asp:TextBox>
                        </td>
                        <td style="vertical-align: top; width: 10px; text-align: center">
                            <span class="star">*</span>
                        </td>
                    </tr>
                    <tr style="height: 10px">
                        <td valign="bottom" style="width: 10px">
                            <img title="加长签字意见" alt="加长签字意见" src='<%=ResolveUrl("~/Images/AddHeight.jpg") %>'
                                onclick="javascript:ToggleApprovalHeight(this, '<%= txtEntrust.ClientID %>');"
                                style="cursor: pointer;" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <div class="ui-dialog-buttonset" style="line-height: 28px">
                <input class="btn4" value="确　认" id="btnEntrustOK" type="button" onclick="return UserApproval('Entrust')" />
                <input class="btn4" value="取　消" id="btnEntrustCancel" type="button" onclick="HideEntrustUserDialog()" />
            </div>
        </div>
    </div>
</div>
<!---------------------------------------------------------------------------------------------------------------弹窗---->
<div id="divForwardUserDialog" class="jqmWindowSignUp jqmWindowSignUpWide">
    <div class="ui-dialog ui-widget ui-widget-content ui-corner-all Boxcontent" style="width: 800px">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix"
            style="height: 24px">
            <span class="ui-dialog-title">请选择转发人</span> <a class="ui-dialog-titlebar-close ui-corner-all"
                href="javascript:HideForwardUserDialog()" role="button"><span class="ui-icon ui-icon-closethick">
                    close</span> </a>
        </div>
        <div class="BoxMain" style="text-decoration: none; font-weight: normal">
            <div class="box_table">
                <ul class="main_bd_tab">
                    <li alt="#paneplan" class="main_bd_tab_active">按组织架构</li>
                    <li alt="#panebackup">组合查询</li>
                </ul>
                <div id="divC2" class="main_bd_tab_content mb8" style="margin-bottom: 2px">
                    <div id="paneplan" class="panetabls">
                        <table>
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <asp:TreeView ID="treeOrgList" runat="server" ExpandImageUrl="~/Images/treeview/col.gif"
                                                CollapseImageUrl="~/Images/treeview/exp.gif" ShowExpandCollapse="True" ShowCheckBoxes="None"
                                                ShowLines="True" OnTreeNodeExpanded="treeOrgList_TreeNodeExpanded" Width="100%"
                                                OnSelectedNodeChanged="treeOrgList_SelectedNodeChanged">
                                            </asp:TreeView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="panebackup" class="panetabls">
                        <div style="height: 20px">
                            <!--垫高-->
                        </div>
                        <table class="new_search_table" align="center" style="font-family: 新宋体,宋体; font-size: 14px;">
                            <tr>
                                <td style="">
                                    姓名：
                                    <input type="text" class="input205" id="txtSearchUser" style="" />
                                </td>
                                <td colspan="2">
                                    状况：
                                    <asp:DropDownList ID="ddlJobType" runat="server" Enabled="False" Width="205" Style="+line-height: 20px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    部门：
                                    <input type="text" class="input205" id="txtSearchdeptName" style="" />
                                </td>
                                <td>
                                    岗位：
                                    <input type="text" class="input205" id="txtSearchjobName" style="" />
                                </td>
                                <td>
                                    <input type="button" class="btn4" value="查询" onclick="GetSearchUserList()" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <!--浮层修改-->
                <table>
                    <tr>
                        <td width="360" valign="top">
                            <p>
                                <b>查询结果：</b></p>
                            <!--修改高度 260-->
                            <select id="tbSearchUserList" multiple="multiple" style="height: 150px; font-family: 新宋体,宋体;
                                font-size: 14px; width: 360px">
                            </select>
                        </td>
                        <td align="center" valign="top">
                            <div style="height: 5px">
                            </div>
                            <!--onclick="AddUserToForward()"-->
                            <input id="btnup" class="list_btn" type="button" title="向上移动" style="background-image: url(../../../Images/listbutton_up.png);
                                height: 24px;" />
                            <input id="btnSingRight" class="list_btn" type="button" title="移至右侧" value="" style="background-image: url(../../../Images/listbutton_oneright.png);
                                height: 18px" />
                            <input id="btnAllRight" class="list_btn" type="button" title="全部移至右侧" value="" style="background-image: url(../../../Images/listbutton_right.png);
                                height: 24px" />
                            <input id="btnSingLeft" type="button" class="list_btn" title="移至左侧" value="" style="background-image: url(../../../Images/listbutton_oneleft.png);
                                height: 18px" />
                            <input id="btnAllLeft" type="button" class="list_btn" title="全部移至左侧" value="" style="background-image: url(../../../Images/listbutton_left.png);
                                height: 24px" />
                            <input id="btndown" class="list_btn" type="button" title="向下移动" style="background-image: url(../../../Images/listbutton_down.png);
                                height: 26px; margin-bottom: 0" />
                        </td>
                        <td width="360" valign="top">
                            <p>
                                <b>待转发用户：</b></p>
                            <select size="20" id="tbSelectedUserList" style="height: 150px; font-family: 新宋体,宋体;
                                font-size: 14px; width: 360px;">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" valign="top">
                            <b>签字意见：</b>
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtForward" ClientIDMode="Static"
                                            Width="99%" Height="45px"></asp:TextBox>
                                    </td>
                                    <td style="vertical-align: top; width: 10px; text-align: center">
                                        <span class="star">*</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <script type="text/javascript">

                $("#btnSingRight").bind("click", function () //移动到右侧
                {
                    if ($("#tbSearchUserList option").length == 0) { alert("请先选择用户"); return }

                    $("#tbSearchUserList option:selected").each(function (data) {
                        var current = $(this);
                        var this_val = $(this).val();



                        $("#tbSelectedUserList option").each(function () {

                            var thisa = $(this);
                            $(thisa).removeAttr("selected");
                            var c_t = $(this).val();

                            if (c_t == this_val) {
                                thisa.remove();
                            }

                        })

                        $("#tbSelectedUserList").append(current);

                    })
                })
                //添加右侧添加到左侧
                $("#btnSingLeft").bind("click", function () {
                    if ($("#tbSelectedUserList option:selected").length == 0) { alert("请先选择用户"); return; }
                    $("#tbSelectedUserList option:selected").each(function () {
                        $("#tbSearchUserList").append($(this));
                    })
                })

                //添加左侧双击
                $("#tbSearchUserList").dblclick(function () {
                    $("#btnSingRight").trigger("click")
                })
                //添加右侧双击
                $("#tbSelectedUserList").dblclick(function () {
                    $("#btnSingLeft").trigger("click")
                })




                //添加全部到右侧
                $("#btnAllRight").bind("click", function () //移动到右侧
                {
                    if ($("#tbSearchUserList option").length == 0) { alert("请先选择用户"); return; }

                    $("#tbSearchUserList option").each(function (data) {
                        var current = $(this);
                        var this_val = $(this).val();
                        $("#tbSelectedUserList option").each(function () {

                            var thisa = $(this);
                            $(thisa).removeAttr("selected");
                            var c_t = $(this).val();

                            if (c_t == this_val) {
                                thisa.remove();
                            }
                        })

                        $("#tbSelectedUserList").append(current);
                    })
                })
                //全部添加到左侧
                $("#btnAllLeft").bind("click", function () //移动到右侧
                {
                    if ($("#tbSelectedUserList option").length == 0) { alert("请先选择用户"); return }

                    $("#tbSelectedUserList option").each(function (data) {
                        var current = $(this);
                        var this_val = $(this).val();
                        $("#tbSearchUserList").append(current);
                    })
                })

                //向上
                $("#btnup").bind("click", function () {
                    $("#tbSelectedUserList option:selected").each(function () {
                        $(this).insertBefore($(this).prev())
                    })
                })
                //向下
                $("#btndown").bind("click", function () {
                    $("#tbSelectedUserList option:selected").each(function () {
                        $(this).insertAfter($(this).next())
                    })
                })

            </script>
        </div>
        <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <div class="ui-dialog-buttonset" style="line-height: 28px">
                <asp:HiddenField runat="server" ID="hidForwardUserIdList" ClientIDMode="Static" />
                <input class="btn4" value="确　认" id="btnForwardOK" type="button" onclick="return UserApproval('Forward')" />
                <input class="btn4" value="取　消" id="btnForwardCancel" type="button" onclick="HideForwardUserDialog()" />
            </div>
        </div>
    </div>
</div>
<!---------------------------------------------------------------------------------------------------------------弹窗结束-->
<div id="divCancelUserDialog" class="jqmWindowSignUp">
    <div class="ui-dialog ui-widget ui-widget-content ui-corner-all Boxcontent" style="width: 600px">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix"
            style="height: 24px">
            <span class="ui-dialog-title">请输入撤销流程说明</span> <a class="ui-dialog-titlebar-close ui-corner-all"
                href="javascript:HideCancelUserDialog()" role="button"><span class="ui-icon ui-icon-closethick">
                    close</span> </a>
        </div>
        <div class="BoxMain" style="text-decoration: none; font-weight: normal">
            <div style="padding: 10px">
                <table style="width: 570px; line-height: 28px">
                    <tr>
                        <td style="vertical-align: top; text-align: right; width: 80px" rowspan="2">
                            签字意见：
                        </td>
                        <td rowspan="2">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtCancel" ClientIDMode="Static"
                                Width="99%" Height="200px"></asp:TextBox>
                        </td>
                        <td style="vertical-align: top; width: 10px; text-align: center">
                            <span class="star">*</span>
                        </td>
                    </tr>
                    <tr style="height: 10px">
                        <td valign="bottom" style="width: 10px">
                            <img title="加长签字意见" alt="加长签字意见" src='<%=ResolveUrl("~/Images/AddHeight.jpg") %>'
                                onclick="javascript:ToggleApprovalHeight(this, '<%= txtCancel.ClientID %>');"
                                style="cursor: pointer;" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
            <div class="ui-dialog-buttonset" style="line-height: 28px">
                <input class="btn4" value="确　认" type="button" onclick="return UserApproval('Cancel')" />
                <input class="btn4" value="取　消" type="button" onclick="HideCancelUserDialog()" />
            </div>
        </div>
    </div>
</div>
<asp:Literal runat="server" ID="ltlContextMenu"></asp:Literal>
<script type="text/javascript">
    //选项卡
    $(function () {
        $("div.panetabls").hide().filter($(".main_bd_tab li.main_bd_tab_active").attr("alt")).show();
        $(".main_bd_tab li").click(function () {
            $(".main_bd_tab li").removeClass("main_bd_tab_active").filter(this).addClass("main_bd_tab_active");
            $("div.panetabls").hide().filter($(this).attr("alt")).show();
            autoheight();
        });
        autoheight();
    })
    // 当选择节点时
    function GetTreeNodeValue(val) {
        if (val != null && val != 0) {
            AjaxSearchOrgUser(val, AjaxSearchUserCallBacktree);
        }
    }
    $().ready(function () {
        $('#divChooseUserDialog').jqm({
            modal: true
        });
        $('#divEntrustUserDialog').jqm({
            modal: true
        });
        $('#divForwardUserDialog').jqm({
            modal: true
        });
        $('#divCancelUserDialog').jqm({
            modal: true
        });
        $('#txtSearchUser').keypress(function (evt) {
            if (evt.keyCode == 13) {
                GetSearchUserList();
                return false;
            }
        });
    });
    //如果页面验证通过,调用这个方法显示选择用户页面
    function ShowChooseUserDialog() {
        $("#divChooseUserDialog").jqmShow();
        return false;
    }
    function HideChooseUserDialog() {
        $("#divChooseUserDialog").jqmHide();
    }
    function ShowEntrustUserDialog() {
        $("li").filter(".Entrust").css("display", "block").end().filter(".Approval,.Forward").css("display", "none");
        $('#divEntrustUserDialog').jqmShow();
        return false;
    }
    function HideEntrustUserDialog() {
        $("li").filter(".Approval").css("display", "block").end().filter(".Forward,.Entrust").css("display", "none");
        $('#divEntrustUserDialog').jqmHide();
    }
    function ShowForwardUserDialog() {
        $("li").filter(".Forward").css("display", "block").end().filter(".Approval,.Entrust").css("display", "none");
        $('#divForwardUserDialog').jqmShow();
        //如果有签字意见直接代入
        var note = $("#txtApprovalNote").val();
        $('#divForwardUserDialog #txtForward').val(note);


        return false;
    }
    function HideForwardUserDialog() {
        $("li").filter(".Approval").css("display", "block").end().filter(".Forward,.Entrust").css("display", "none");
        $('#divForwardUserDialog').jqmHide();
    }
    function ShowCancelUserDialog() {
        $('#divCancelUserDialog').jqmShow();
    }
    function HideCancelUserDialog() {
        $('#divCancelUserDialog').jqmHide();
    }
    function RequestApprovalConfirm() {
        var txt = $("#txtComments");
        var content = $.trim(txt.val());

        if (content.length == 0) {
            alert("请签字。");
            txt.focus();
        }
        else if (window.confirm('确认上报?')) {
            ShowBlockUI("正在上报");
            setTimeout(btnActualSubmitClick, 800);
        }
        return false;
    }
    function btnActualSubmitClick() {
        $("#<%=btnActualSubmit.ClientID %>").click();
    }
    function btnActualSaveDraftClick() {
        $("#<%=btnActualSaveDraft.ClientID %>").click();
    }
    function btnActualDeleteProcessClick() {
        $("#<%=btnActualDeleteProcess.ClientID %>").click();
    }
    function btnActualValidatePageClick() {
        $("#<%=btnActualValidatePage.ClientID %>").click();
    }
    function ShowDialog(Id) {
        $("#" + Id).jqmShow();
        return false;
    }

    function HideDialog(Id) {
        $("#" + Id).jqmHide();
    }

    function btnActualSaveDraft_ClientClick() {
        $("#btnActualSaveDraft").click();
    }

    function UserApproval(operation) {
        if (operation == 'Entrust') {
            if ($("#<%=ddlEntrustUserList.ClientID %>").val() == 0) {
                alert("未找到委托人");
                return false;
            }
            var txt = $("#<%=txtEntrust.ClientID %>");
            if ($.trim(txt.val()).length == 0) {
                alert("请签字。");
                txt.focus();
                return;
            }
        }
        else if (operation == 'Launch') {
            var txt = $("#txtComments");
            var content = $.trim(txt.val());

            if (content.length == 0) {
                alert("请签字。");
                txt.focus();
            }
            if (!window.confirm('确认上报')) {
                return;
            }
        }
        else if (operation == 'Forward') {
            var txt = $("#<%=txtForward.ClientID %>");
            var content = $.trim(txt.val());

            if (content.length == 0) {
                alert("请签字。");
                txt.focus();
                return false;
            }
            inputs = $("#tbSelectedUserList option");
            if (inputs.length == 0) {
                alert("请选择要转发的用户。");
                return false;
            }
            var userids = [];
            inputs.each(function () { userids[userids.length] = $(this).val(); });
            $("#hidForwardUserIdList").val(userids);
        }
        else if (operation == "Cancel") {
            var txt = $("#<%=txtCancel.ClientID %>");
            var content = $.trim(txt.val());
            if (content.length == 0) {
                alert("请签字。");
                txt.focus();
                return false;
            }
        }
        else {
            var txt = $("#<%=txtApprovalNote.ClientID %>");
            var content = $.trim(txt.val());

            if (content.length == 0) {
                alert("请签字。");
                txt.focus();
                return false;
            }
            if (operation == "Reject") {
                if (!confirm("您确定要退回流程给发起人吗？"))
                    return false;
            }
        }
        $("#<%=hidApprovalOperation.ClientID %>").val(operation);
        ShowBlockUI("正在处理");
        window.setTimeout(bnApprovalSubmit_ClientClick, 800);
    }

    function bnApprovalSubmit_ClientClick() {
        $("#bnApprovalSubmit").click();
    }

    function TodoWorkEntrust() {
        ShowEntrustUserDialog();
    }

    function TodoWorkForward() {
        ShowForwardUserDialog();
    }
    function ToggleApprovalHeight(sender, ctlid) {
        var height = $("#" + ctlid).height();

        var fist = $("#" + ctlid).attr("kg");
        
       
      /*  if (height == 100)*/
      if(fist=="yes"||fist==null) {
            $("#" + ctlid).height(height+25);
            $(sender).attr("src", '<%=ResolveUrl("~/Images/DecreaseHeight.jpg") %>').attr("title", "缩短签字意见").attr("alt", "缩短签字意见");
            $("#" + ctlid).attr("kg","no");
        }
        else if(fist=="no")
        {
            $("#" + ctlid).attr("kg","yes");
            $("#" + ctlid).height(height-25);
            $(sender).attr("src", '<%=ResolveUrl("~/Images/AddHeight.jpg") %>').attr("title", "加长签字意见").attr("alt", "加长签字意见");
        }
    }
    //用于组合查询
    function AjaxSearchUserCallBack(r, s, xhr) {
        $("#tbSearchUserList").empty();
        var userarray = r;
        if (userarray.length == 0) {
            alert("没有找到用户，请重新查找。");
        }
        for (i = 0; i < userarray.length; i++) {
            var str = "<option value='" + userarray[i].UserId + "' title='" + userarray[i].UserCode + '  ' + userarray[i].UserDepts.trim() + '  ' + userarray[i].Job.trim() + "'>"
            + userarray[i].UserName;

            str = str + userarray[i].UserDept + userarray[i].Job + "</option>"

            $("#tbSearchUserList").append(str);
        }
        //设置返回函数
        try {
            $("#tbSearchUserList option").first().attr("selected");
        } catch (e) { }
    }
    //用户选择树形结构时的查询
    function AjaxSearchUserCallBacktree(r, s, xhr) {
        $("#tbSearchUserList").empty();
        var userarray = r;
        if (userarray.length == 0) {
            //            alert("没有找到用户，请重新查找。");
            return;

        }
        for (i = 0; i < userarray.length; i++) {
            var str = "<option value='" + userarray[i].UserId + "' title='" + userarray[i].UserCode + '  ' + userarray[i].UserDepts.trim() + '   ' + userarray[i].Job.trim() + "'>"
            + userarray[i].UserName;

            str = str + userarray[i].UserDept + userarray[i].Job + "</option>"

            $("#tbSearchUserList").append(str);
        }
        //设置返回函数
        try {
            $("#tbSearchUserList option").first().attr("selected");
        } catch (e) { }
    }
    function GetSearchUserList() {

        var name = $.trim($("#txtSearchUser").val());
        //        var deptName = $.trim($("#txtSearchdeptName").val());
        //        var jobName = $.trim($("#txtSearchjobName").val());
        if (name.length == 0) {
            alert("请输入姓名！");
            return;
        }
        //        string keyword,string deptName,string jobName
        //获取用户输入的部门名称
        var deptName = $.trim($("#txtSearchdeptName").val());
        //获取用户输入的岗位名称
        var jobName = $.trim($("#txtSearchjobName").val());
        AjaxSearchUser(name, deptName, jobName, AjaxSearchUserCallBack);

    }
    function AddUserToForward() {
        $("#tbSearchUserList :selected").each(function (idx, item) {
            var userid = $(item).val();
            if ($("#tbSelectedUserList option[value='" + userid + "']").length == 0) {
                $(item).appendTo("#tbSelectedUserList");
            }
            else {
                $(item).remove();
            }
        });
    }
    $("#AppBtnForward").live("click", class_floattop);
    $("#AppBtnEntrust").live("click", class_floattop);
    function class_floattop() {
        var tops = Math.max(document.body.scrollTop, document.documentElement.scrollTop);
        //$(".jqmWindowSignUpWide").animate({ "top":tops + "px" }, 300);
        $(".jqmWindowSignUpWide").css("top", tops + "px");
        $(".jqmWindowSignUp").css({ "top": tops + "px", "position": "absolute" });
    }
  
</script>
