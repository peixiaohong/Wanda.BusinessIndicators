<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="DSTargetAddDataReport.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.DepartmentStoreReport.DSTargetAddDataReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/DepartmentStoreReportjs/DSTargetAddDataReportjs.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
         <input type="hidden" id="hfDSTargetList" />
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
                            <th>上报系统</th>
                            <td>
                                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>上报月份</th>
                            <td>
                                <asp:DropDownList ID="ddlMonth" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th></th>
                            <td>
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
        </div>
        <!--明细项 开始-->
        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2"><span>新增经营指标未完成门店情况</span></a></li>
                </ul>
                <div class="DownExcelDiv">
                     <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport();">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出新增经营指标未完成门店情况</label></span>
                        </div>
                    </span>
                </div>
            </div>
            <!--tabs-->
            <div class="tabs_content">
                <!--新增经营指标未完成门店 开始-->
                <div style="margin-left: 0px; margin-right: 0px" id="T1">
                    <table class="dstab_5"  style="table-layout: fixed; margin-top: 5px;">
                        <thead id="DSAdd_Thead">
                        </thead>
                         <tbody id="AddTarget_Tbody" >
                        </tbody>
                    </table>
                </div>
                <!--新增经营指标未完成门店 结束-->
            </div>
        </div>
    </div>
</asp:Content>
