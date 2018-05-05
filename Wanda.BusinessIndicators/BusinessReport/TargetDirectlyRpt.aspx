<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="TargetDirectlyRpt.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.DirectlyMonthReport" %>

<%@ Register Src="~/SiteMasterPage/wfCtrl.ascx" TagPrefix="uc1" TagName="wfCtrl" %>
<%@ Register Src="~/SiteMasterPage/userSelectCtrl.ascx" TagPrefix="uc1" TagName="userSelectCtrl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.base64.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>

    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.all-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.min.js"></script>
    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />

    <script type="text/javascript" src="../Scripts/BusinessReport/DirectlyMonthReport.js?ver=2"></script>
    <script type="text/javascript">
        var TreeDataJson = <%=TreeDataJson%>;
    </script>
    <style type="text/css">
        .tab_search th {
            width:5%!important;
        }
    </style>
    <uc1:wfCtrl runat="server" ID="wfCtrl" />
    <uc1:userSelectCtrl runat="server" ID="userSelectCtrl" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="">
            <div class="title">
                <a href="#"><span>查询条件</span></a>
            </div>
            <!--title-->
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div class="content_m">
                <table class="tab_search">
                    <tbody>
                        <tr>
                            <th>板块</th>
                            <td>
                                <input type="text" id="TxtSystem" style="width: 210px;" onclick="showMenu();" />
                                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px; display: none;" AutoPostBack="True" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>上报月份</th>
                            <td>
                                <asp:DropDownList ID="ddlMonth" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>版本类型</th>
                            <td>
                                <asp:DropDownList ID="ddlVersionType" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th style="text-align: center">
                                <input id="submana" runat="server" clientidmode="Static" style="width: 80px; text-align: center; height: 27px; display: none" type="button" class="uploadify-button" value="" onclick="AddMessages()" /></th>
                            <td>
                                <asp:CheckBox ID="chkIsLastestVersion" ClientIDMode="Static" runat="server" Text="包含审批中" />
                            </td>
                            <th>
                                <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="javascript:void(0)" onclick="f_search()" ><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
        </div>



        <div class="margin_t10" id="MonthReportExplainDiv">
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div class="content_m">
                <div class="blue_txt">月报说明</div>
                <div style="width: 100%; position: relative; padding-bottom: 3px;"></div>
                <div id="txtDes">
                </div>

            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
        </div>

        <!--月报说明 结束-->

        <!--明细项 开始-->
        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>月度经营报告</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>当月未完成</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>累计未完成</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>补回情况明细</span></a></li>
                    <li id="detailmana" style="display: none" runat="server" clientidmode="Static"><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>上报日志</span></a></li>
                </ul>
                <div class="DownExcelDiv" id="DownExcel">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReportList(this);">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出全部数据</label></span>
                        </div>
                    </span>
                </div>
            </div>
            <!--tabs-->
            <div class="tabs_content">
                <!--内容-->

                <div id="SearchPanel" class="panelSearch">
                </div>
                <!--收缩按钮 开始-->
                <div style="margin-left: 45%; margin-bottom: 0px">
                    <img id="imgtableUpDown" style="display: none" onclick="UpDownTableClick()" src="../Images/images1/Down.png" alt="" />
                </div>
                <!--收缩按钮 结束-->
                <!--明细项 结束-->

                <!--月度报告 开始-->
                <div id="T1" style="margin-top: 5px;">
                    <div class="scrolldoorFrame copy">
                        <table id="MonthReportSummaryTable" class="tab_005">
                            <thead id="MonthReportSummaryHead" style="width: 100%">
                                <tr class='tab_5_row_alt'>
                                    <th rowspan='2' style='width: 4%'>序号</th>
                                    <th rowspan='2'>项目</th>
                                    <th colspan='3' style='width: 30%'>本月发生(万元)</th>
                                    <th colspan='3' style='width: 30%'>本年累计(万元)</th>
                                    <th colspan='2' style='width: 20%'>全年(万元)</th>
                                </tr>
                                <tr>
                                    <th class='th_Sub'>计划</th>
                                    <th class='th_Sub'>实际</th>
                                    <th class='th_Sub2'>完成率</th>
                                    <th class='th_Sub1'>计划</th>
                                    <th class='th_Sub'>实际</th>
                                    <th class='th_Sub2'>完成率</th>
                                    <th class='th_Sub1'>计划</th>
                                    <th class='th_Sub'>完成率</th>
                                </tr>
                            </thead>
                            <tbody id="rows">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--月度报告 结束-->

                <!--完成情况明细 start-->
                <div class="TClassHide" id="T2">
                    <div class="scrolldoorFrame copy">
                        <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                        </ul>
                        <table class="tab_005" id="importedDataTable2">
                            <thead id="CompleteDetailHead" style="width: 100%">
                            </thead>
                            <tbody id="tab2_rows">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--完成情况明细 end-->


                <!--未完成说明（当月） 开始-->
                <div class="TClassHide" id="T3_1">
                    <ul class="tabs" id="U2_1" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>

                    <table class="tab_005" id="Tab_CurrentMissTarget">
                        <thead id="Tab_CurrentMissTargetHead">
                        </thead>
                        <tbody id="Tbody_CurrentMissTargetData">
                        </tbody>

                    </table>

                </div>
                <!--未完成说明 结束-->


                <!--未完成说明 开始-->
                <div id="T3" class="TClassHide">

                    <ul class="tabs" id="U2" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>

                    <table class="tab_005" id="Tab_MissTarget" style="width: auto;">
                        <thead id="Tab_MissTargetHead">
                        </thead>
                        <tbody id="Tbody_MissTargetData">
                        </tbody>

                    </table>
                </div>
                <!--未完成说明 结束-->



                <!--补回情况 开始-->
                <div id="T4" class="TClassHide">
                    <ul class="tabs" id="U1" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>

                    <table class="tab_005" id="Tab_Return">
                        <thead id="Tab_ReturnHead">
                        </thead>
                        <tbody id="Tbody_Data">
                        </tbody>

                    </table>
                </div>
                <!--补回情况 结束-->

                <!--上报日志 开始-->
                <input type="hidden" id="detailhidden" runat="server" clientidmode="Static" />
                <div id="T5" class="TClassHide">
                    <ul class="tabs" id="Ul1" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>

                    <table class="tab_005" id="Table1">
                        <thead id="ActionHead">
                        </thead>
                        <tbody id="Action_Row">
                        </tbody>

                    </table>
                </div>
                <!--上报日志 结束-->



                <div id="ApproveAttachDiv" style="padding-bottom: 10px; padding-top: 10px;">
                    <div style="border: 1px solid #ccc; min-height: 50px; padding-top: 5px; padding-bottom: 5px; background: #fff;">
                        <div class="content_title1_Left" style="width: 100%; padding-bottom: 10px;">
                            <div>
                                <label style="padding-left: 10px; color: #012b80; font-weight: bold">
                                    附&nbsp;&nbsp;&nbsp;&nbsp;件
                                </label>
                            </div>
                        </div>
                        <div id="listAttDiv" class="AttDiv" style="padding-left: 10px">
                            <%--<a href="javascript:return vodi(0);">商管公司.xls(122 KB)</a>--%>
                        </div>

                    </div>
                </div>
                <!--附件 结束-->

            </div>
        </div>
    </div>


    <div id="menuContent" class="menuContent" style="display: none; position: absolute;">
        <ul id="SysTree" class="ztree_new" style="margin-top: 0; width: 200px; height: 350px; background-color: #fff; border: 1px solid #000; overflow-y: auto;"></ul>
    </div>

</asp:Content>

