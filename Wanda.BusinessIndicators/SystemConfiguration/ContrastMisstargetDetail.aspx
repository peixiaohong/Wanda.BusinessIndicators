<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContrastMisstargetDetail.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.ContrastMisstargetDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/ContrastMisstarget.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
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
                           
                            <select style="width:120px" id="ddlSystem"></select>

                        </td>

 <%--                       <th>查询时间:</th>
                        <td>
                            <input type="text" id="OpenDate" onchange="TimeChange()" style="width: 150px; margin-left: 20px" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM', maxDate: '%y-%M' })" /></td>--%>
                         <th>年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>月份</th>
                            <td>
                                <asp:DropDownList ID="ddlMonth" ClientIDMode="Static" onchange="TimeChange()" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                        <th>显示类型:</th>
                        <td>
                            <select style="width: 120px; height: 25px" id="TargetValue">
                                <option selected="selected" value="0">累计指标</option>
                                <option value="1">当月指标</option>
                            </select></td>
                        <th>对比类型:</th>
                        <td>
                            <select style="width: 120px; height: 25px" id="ContrastValue">
                                <option id="SelectHuan" value="0">环比</option>
                                <option selected="selected" value="1">同比</option>
                            </select></td>
                        <th>  
                            <input type="checkbox" id="IsPro"  />包含审批中
                            
                        </th>

                        <th><a class="btn_search" id="ContentPlaceHolder1_LinkButton1" onclick="Select()" href="#"><span>查询</span></a></th>
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
                    <li class="selected m_1"><a class="active_sub2"><span>未完成统计表</span></a></li>
                </ul>
                <div class="DownExcelDiv" id="DownExcel">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload" onclick="DownExcel();">导出数据</label></span>
                        </div>
                    </span>
                </div>
            </div>
        </div>
        <div class="tabs_content">
            <div class="scrolldoorFrame copy">
                <table class="tab_005" id="importedDataTable2">
                    <thead>
                        <tr>
                            <th rowspan="4" style="width: 130px">系统</th>
                            <th rowspan="4" style="width: 130px">指标</th>
                            <th colspan="8" rowspan="1" id="InnerContrastValue"></th>

                        </tr>

                        <tr>
                            <th colspan="3" id="LastTime"></th>
                            <th colspan="3" id="NowTime"></th>
                            <th colspan="2" id="InnerContrastValue2"></th>

                        </tr>
                        <tr>
                            <th rowspan="2">本期考核范围内家数</th>

                            <th colspan="2"  class="InnerTargetValue"></th>
                            <th rowspan="2">本期考核范围内家数</th>

                            <th colspan="2"  class="InnerTargetValue"></th>

                            <th colspan="2"  class="InnerTargetValue"></th>
                        </tr>
                        <tr>
                            <th>未完成家数</th>
                            <th>占比</th>


                            <th>未完成家数</th>
                            <th>占比</th>
                            <th>未完成家数变化</th>
                            <th>占比变化</th>


                        </tr>
                    </thead>
                    <tbody id="rows"></tbody>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
