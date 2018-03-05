<%@ Page Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="HistoryReturnReport.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.StatisticReport.HistoryReturnReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/HistoryReturn.js"></script>
    

    <style type="text/css">
    </style>
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
                            <th>上报系统</th>
                            <td>
                                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px;" AutoPostBack="True" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th style="text-align: center">
                                <input id="submana" runat="server" clientidmode="Static" style="width: 80px; text-align: center; height: 27px; display: none" type="button" class="uploadify-button" value="" onclick="AddMessages()" /></th>
                            <td></td>
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



        <div class="margin_t10" id="MonthReportExplainDiv">
        </div>

        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>历史要求期限统计汇总报表</span></a></li>
                </ul>
                <div class="DownExcelDiv" id="DownExcel">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport(this);">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出历史要求期限统计汇总报表</label></span>
                        </div>
                    </span>
                </div>
            </div>
            <div class="tabs_content">

                <div id="T1" style="margin-top: 5px;">
                    <div class="scrolldoorFrame copy">
                        <table id="HistoryReturnRpt" class="tab_005">
                            <thead id="HistoryReturnRptHead" style="width: 100%">
                                <tr>
                                    <th>序号</th>
                                    <th>门店名称</th>
                                    <th>指标名称</th>
                                    <th style="width:7%" >1月份</th>
                                    <th style="width:7%" >2月份</th>
                                    <th style="width:7%" >3月份</th>
                                    <th style="width:7%" >4月份</th>
                                    <th style="width:7%" >5月份</th>
                                    <th style="width:7%" >6月份</th>
                                    <th style="width:7%" >7月份</th>
                                    <th style="width:7%" >8月份</th>
                                    <th style="width:7%" >9月份</th>
                                    <th style="width:7%" >10月份</th>
                                    <th style="width:7%" >11月份</th>
                                </tr>
                            </thead>
                            <tbody id="HistoryReturnRptBody">
                            </tbody>
                        </table>
                    </div>
                </div>

            </div>


        </div>
    </div>

</asp:Content>
