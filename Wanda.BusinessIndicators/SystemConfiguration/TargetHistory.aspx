<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetHistory.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="Wanda.BusinessIndicators.Web.TargetHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/TargetHistory.js"></script>
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
                            <th>选择系统</th>
                            <td>
                                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px;" AutoPostBack="True" ></asp:DropDownList>
                            </td>
                            <th>选择年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>

                            <th>
                                <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="GetTargetList();"><span>查询</span></a>
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
                        <li class="selected m_1"><a class="active_sub2"><span>指标历史</span></a></li>
                    </ul>

                </div>
            </div>
            <div class="tabs_content">
                <div class="scrolldoorFrame copy">
                    <table class="tab_005" id="importedDataTable2">
                        <thead id="Head">
                            <tr>
                                <th style="width: 45%">上报人</th>
                                <th style="width: 45%">上报日期</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody id="rows">
                        
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
