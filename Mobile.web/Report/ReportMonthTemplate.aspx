<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportMonthTemplate.aspx.cs" Inherits="Mobile.web.Report.ReportMonthTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportMonthTemplate.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="ReportMonthTemplateContent" v-cloak>
        <div class="widget-body clear">
            <div class="page-body" style="margin: 0">
                <div class="collection-cont col-md-12 col-sm-12">
                    <ul style="padding-top: 0; border-top: 0 solid;">
                        <li class="active">
                            <div style="margin-top: 15px;">
                                <table class="from-table" style="line-height: 1.7">
                                    <tbody>
                                        <tr>
                                            <td style="width: 50%">房地产事业部</td>
                                            <td style="width: 30%">2018年</td>
                                            <td style="width: 25%">{{getQueryString("name")}}月</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="active">
                            <h3>{{result.Name}}(本月 单位:万元)<span class="collection-updown-icon"></span></h3>
                            <div class="showBox">
                                <table class="from-table alignCenter">
                                    <thead>
                                        <tr>
                                            <th style="width: 34%">名称</th>
                                            <th style="width: 22%">计划</th>
                                            <th style="width: 22%">实际</th>
                                            <th style="width: 22%">完成率</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <template v-for="list in result.ObjValue[0].ObjValue" v-if="result.ObjValue && result.ObjValue[0]">
                                        <tr>
                                            <td v-on:click="list.IsShow = !list.IsShow">{{list.Name}}</td>
                                            <td>{{list.BMonthReportDetail.NPlanAmmount}}</td>
                                            <td>{{list.BMonthReportDetail.NActualAmmount}}</td>
                                            <td class="color-height">{{list.BMonthReportDetail.NDisplayRate}}</td>
                                        </tr>
                                        <tr class="new-tem" v-if="list.ObjValue.length && list.IsShow">
                                            <td colspan="4" style="padding: 0;">
                                                <div class="clear table-tem-new">
                                                    <table>
                                                        <template v-for="(items,index) in list.ObjValue">
                                                        <tr>
                                                            <th style="padding: 0; width: 33.8%">
                                                                <table>
                                                                    <tr>
                                                                        <td style="width: 20%">{{index + 1}}</td>
                                                                        <td style="width: 80%">{{items.CompanyName}}</td>
                                                                    </tr>
                                                                </table>
                                                            </th>
                                                            <td style="width: 22.1%">{{items.NPlanAmmount}}</td>
                                                            <td style="width: 22.1%">{{items.NActualAmmount}}</td>
                                                            <td style="width: 22%">{{items.NDisplayRate}}</td>
                                                        </tr>
                                                        <tr class="tem-item" v-if="false">
                                                            <td colspan="4" style="padding: 0;">
                                                                <div class="clear tem-list">
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding: 0; width: 33.8%">
                                                                                <table style="width: 100%">
                                                                                    <tr>
                                                                                        <td style="width: 20%">1</td>
                                                                                        <td style="width: 80%">泰和世家</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>

                                                                            <td style="width: 22.1%">1,000</td>
                                                                            <td style="width: 22.1%">1,000</td>
                                                                            <td style="width: 22%">100%</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="padding: 0; width: 33.8%">
                                                                                <table style="width: 100%">
                                                                                    <tr>
                                                                                        <td style="width: 20%">2</td>
                                                                                        <td style="width: 80%">香邑溪谷</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>

                                                                            <td style="width: 22.1%">1,000</td>
                                                                            <td style="width: 22.1%">1,000</td>
                                                                            <td style="width: 22%">100%</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="padding: 0; width: 33.8%">
                                                                                <table style="width: 100%">
                                                                                    <tr>
                                                                                        <td style="width: 20%">3</td>
                                                                                        <td style="width: 80%">香邑原墅</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>

                                                                            <td style="width: 22.1%">900</td>
                                                                            <td style="width: 22.1%">361</td>
                                                                            <td style="width: 22%" class="color-height">41%</td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        </template>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        </template>
                                    </tbody>
                                </table>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
