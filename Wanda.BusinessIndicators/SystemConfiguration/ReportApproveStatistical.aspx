<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportApproveStatistical.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.ReportApproveStatistical" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../Scripts/BusinessReport/ReportApprove.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.base64.js"></script>

    <script type="text/javascript" src="../Scripts/TreeGrid/TreeGrid.js"></script>
    <link href="../Scripts/TreeGrid/TreeGrid.css" rel="stylesheet" />

    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.all-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.min.js"></script>
    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />

    <script type="text/javascript">
        var TreeDataJson = <%=TreeDataJson%>;
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="title">
        <a href="#"><span>查询条件</span></a>
    </div>
    <div class="content_t_l">
        <div class="content_t_r"></div>
    </div>
    <div class="content_m">
        <table class="tab_search">
            <tbody>
                <tr>

                    <th>查询系统</th>
                    <td>
                         <input type="text" id="TxtSystem" style="width: 210px;" onclick="showMenu();" />
                        <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px;display:none; "></asp:DropDownList>
                        &nbsp;</td>
                    <th>查询年份</th>
                    <td>
                        <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>

                        &nbsp;</td>
                    <th>查询月份</th>
                    <td>
                        <asp:DropDownList ID="ddlMonth" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>

                        &nbsp;</td>

                    <th>
                        <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="GetTreeData();"><span>查询</span></a>
                    </th>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="content_b_l">
        <div class="content_b_r"></div>
    </div>

    <div class="margin_t10">
        <div class="tabs_m">
            <ul id="Ul3">
                <li class="selected m_1"><a class="active_sub2"><span>月报指标</span></a></li>
            </ul>
            <div class="uploadify-button" id="file_upload-button1" onclick="DownExcel()" style="width: 90px; float: right; display:none;">
                <span>
                    <label class="DownExcelLabel" id="LabelDownload">导出全部数据</label></span>
            </div>
        </div>
    </div>
    <div class="tabs_content" style="display: none;">
        <div class="scrolldoorFrame copy">
            <table class="tab_005" id="importedDataTable2">
                <thead id="Head">
                    <tr>
                        <th style="width: 3%">选择</th>
                        <th style="width: 3%">序号</th>
                        <th style="width: 10%">系统名称</th>
                        <th style="width: 10%">提交时间</th>
                        <th>审批情况</th>
                        <th style="width: 10%">审批状态</th>
                    </tr>
                </thead>
                <tbody id="rows">
                </tbody>
            </table>
        </div>
    </div>

    <div class="tabs_content" id="div_TreeGrid">
    </div>


      <div id="menuContent" class="menuContent" style="display:none; position: absolute;">
	<ul id="SysTree" class="ztree_new" style="margin-top:0; width:200px; height:350px; background-color :#fff; border:1px solid #000; overflow-y:auto;"></ul>
</div>



</asp:Content>
