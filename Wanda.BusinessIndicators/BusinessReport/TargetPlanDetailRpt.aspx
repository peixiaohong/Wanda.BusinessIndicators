<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="TargetPlanDetailRpt.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.TargetPlanDetailRpt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetPlanDetailRpt.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.base64.js"></script>

    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.all-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.min.js"></script>
    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />
    <script type="text/javascript">
        var TreeDataJson = <%=TreeDataJson%>;
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="">
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
                            <th>上报系统</th>
                            <td>
                                <input type="text" id="TxtSystem" style="width: 210px;" onclick="showMenu();" />
                                <asp:DropDownList ID="ddlSystem" AutoPostBack="true" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged" ClientIDMode="Static" runat="server" Style="width: 120px; display:none;"></asp:DropDownList>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <td style="display: none">
                                <asp:CheckBox ID="chkIsLastestVersion" ClientIDMode="Static" runat="server" Text="包含审批中" />
                            </td>
                            <th>
                                <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="SearchData();"><span>查询</span></a>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
            <!--明细项 开始-->
            <!--明细项 开始-->
            <div class="margin_t10">
                <div class="tabs_m">
                    <ul id="Ul3">
                        <li class="selected m_1"><a class="active_sub2"><span>计划指标</span></a></li>
                    </ul>
                    <div class="DownExcelDiv" id="DownExcel">
                        <span class="DownExcelspan">
                            <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport(this);">
                                <span>
                                    <label class="DownExcelLabel" id="LabelDownload">导出计划指标</label></span>
                            </div>
                        </span>
                    </div>
                </div>
            </div>
            <div class="tabs_content">
                <div class="scrolldoorFrame copy">
                    <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>
                    <table class="tab_005" id="importedDataTable2">
                        <thead id="TargetPlanDetailHead">
                        </thead>
                        <tbody id="rows">
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">

            <textarea id="Remark" cols="55" rows="5"></textarea>


            <div class="pop_operate">
                <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
                <input type="button" class="pop_btn btn_blue" value="确定" onclick=" AddRemark();" id="savetrue" />
            </div>
        </div>
        <div id="div1" class="popup" style="height: auto; display: none; padding-bottom: 10px">

            <table style="width: 600px;" class="tab_005" id="tbEdit">
                <thead>
                    <tr>
                        <th style="width: 20%">操作人</th>
                        <th style="width: 20%">操作</th>
                        <th style="width: 20%">操作时间</th>
                        <th style="width: 40%">操作原因</th>
                    </tr>
                </thead>
                <tbody id="historyaction">
                </tbody>
            </table>
            <div class="pop_operate">
                <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'div1' }).close();" />
            </div>
        </div>
    </div>
    <div id="menuContent" class="menuContent" style="display: none; position: absolute;">
        <ul id="SysTree" class="ztree_new" style="margin-top: 0; width: 200px; height: 350px; background-color: #fff; border: 1px solid #000; overflow-y: auto;"></ul>
    </div>

</asp:Content>
