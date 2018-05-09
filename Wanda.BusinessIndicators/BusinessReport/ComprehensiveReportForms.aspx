<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="ComprehensiveReportForms.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.ComprehensiveReportForms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script type="text/javascript" src="../Scripts/BusinessReport/ComprehensiveReportForms.js"></script>--%>
    <script type="text/javascript" src="../Scripts/BusinessReport/ComprehensiveReportForms.js?ver=<%=new Random(DateTime.Now.Millisecond).Next(0,10000)%>"></script>
    <style type="text/css">
        .tab_search th {
            width: 5% !important;
        }
    </style>
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
                        <th>上报系统</th>
                        <td>
                            <select id="systemInfo" class="PreDataChange">
                            </select>
                        </td>
                        <th>上报年份</th>
                        <td>
                            <select id="selectYears" class="PreDataChange">
                            </selec>
                        </td>
                        <th>上报月份</th>
                        <td>
                            <select id="selectMonth" class="PreDataChange">
                                <option data-id="1">1月</option>
                                <option data-id="2">2月</option>
                                <option data-id="3">3月</option>
                                <option data-id="4">4月</option>
                                <option data-id="5">5月</option>
                                <option data-id="6">6月</option>
                                <option data-id="7">7月</option>
                                <option data-id="8">8月</option>
                                <option data-id="9">9月</option>
                                <option data-id="10">10月</option>
                                <option data-id="11">11月</option>
                                <option data-id="12">12月</option>
                            </select>
                        </td>
                        <th>版本类型</th>
                        <td>
                            <select id="selectTargetVersionType"></select>
                        </td>
                        <th>
                            <a class="btn_search" href="#"><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
                        </th>
                    </tbody>
                </table>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
        </div>


        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2"><span>综合查询</span></a></li>
                </ul>
            </div>
        </div>
        <div class="tabs_content">
            <div class="scrolldoorFrame copy">
                <div class="table-head">
                    <table class="tab_005" style="border: 1px solid black;">
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <thead>
                            <tr>
                                <th class="Td_Right" rowspan="2">板块</th>
                                <th class="Td_Right" rowspan="2">考核指标</th>
                                <th class="Td_Right" colspan="4">本月发生</th>
                                <th class="Td_Right" colspan="4">本年累计</th>
                            </tr>
                            <tr>
                                <th class="Td_Right">计划数</th>
                                <th class="Td_Right">实际数</th>
                                <th class="Td_Right">差额</th>
                                <th class="Td_Right">完成率</th>
                                <th class="Td_Right">计划数</th>
                                <th class="Td_Right">实际数</th>
                                <th class="Td_Right">差额</th>
                                <th class="Td_Right">完成率</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="table-body">
                    <table class="tab_005">
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <colgroup style="text-align: center; width: 10%"></colgroup>
                        <tbody id="ShowReportData">
                        </tbody>
                    </table>

                </div>
            </div>
        </div>
    </div>

</asp:Content>
