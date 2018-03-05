<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContrastDetail.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.ContrastDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/ContrastDetail.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

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
                        <th style="text-align: center;display:none"  >
                             
                            <input type="checkbox" id="IsPro"  />包含审批中
                        </th>
                        <td>
                            <asp:CheckBox ID="chkIsLastestVersion" Style="display: none;" ClientIDMode="Static" runat="server" Text="包含审批中" />
                        </td>
                        <th>
                            <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="GetList();"><span>查询</span></a>
                        </th>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="content_b_l">
            <div class="content_b_r"></div>
        </div>
    </div>
    <div class="margin_t10">
        <div class="tabs_m">
            <table style="width: 99%">
                <tbody>
                    <tr>
                        <td>
                            <ul id="tabs" class="tabs" style="MARGIN-BOTTOM: 0px; HEIGHT: auto; BORDER-BOTTOM-COLOR: #fff; MARGIN-TOP: 5px">
                            </ul>
                        </td>
                        <td>
               

                            <div class="uploadify-button" id="file_upload-button1" style="width: 90px; float: right; margin-right: 20px" onclick="DownExcel();">
                                <span>
                                    <label class="DownExcelLabel" id="LabelDownload">导出数据</label></span>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

            <%--   <ul id="Ul3">
                <li class="selected m_1"><a class="active_sub2"><span>完成情况同期对比表</span></a></li>
            </ul>--%>
        </div>
    </div>
    <div class="tabs_content">
        <div class="scrolldoorFrame copy">
            <table class="tab_005" id="importedDataTable2">
                <thead>
                    <tr>
                        <th style="width: 8%">序号</th>
                        <th style="width: 20%">名称</th>
                        <th style="width: 18%" id="LastYear"></th>
                        <th style="width: 18%" id="ThisYear"></th>
                        <th style="width: 18%">增长额</th>
                        <th style="width: 18%">增长率</th>
                    </tr>

                </thead>

                <tbody id="rows">
                </tbody>

            </table>
        </div>
    </div>

    <asp:HiddenField runat="server" ClientIDMode="Static" ID="SysID" />
    <asp:HiddenField runat="server" ClientIDMode="Static" ID="FinMonth" />
    <asp:HiddenField runat="server" ClientIDMode="Static" ID="FinYear" />

</asp:Content>
