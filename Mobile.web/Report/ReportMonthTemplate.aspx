<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportMonthTemplate.aspx.cs" Inherits="Mobile.web.Report.ReportMonthTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportMonthTemplate.js?v=")+System.Guid.NewGuid() %>"></script>
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
                                            <td style="width: 50%">{{title}}</td>
                                            <td style="width: 30%">{{utils.getQueryString("year")}}年</td>
                                            <td style="width: 25%">{{utils.getQueryString("month")}}月</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </li>
                        <li class="active">
                            <h3 v-bind:class="{'bottom': currentState}">{{result.Name}}(本月 单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': currentState}" v-on:click="currentState = !currentState"></span></h3>
                            <div class="showBox" v-if="!currentState">
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
                                            <template v-for="list in result.ObjValue">
                                            <tr>
                                                <td v-on:click="list.IsCurrentShow = !list.IsCurrentShow">{{list.Name}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NPlanAmmount)}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NActualAmmount)}}</td>
                                                <td class="color-height">{{list.BMonthReportDetail.NDisplayRate}}</td>
                                            </tr>
                                            <tr class="new-tem" v-if="list.ObjValue.length && list.IsCurrentShow">
                                                <td colspan="4" style="padding: 0;">
                                                    <div class="clear table-tem-new">
                                                        <table>
                                                            <template v-for="(items,index) in list.ObjValue">
                                                            <tr>
                                                                <th style="padding: 0; width: 33.8%">
                                                                    <table>
                                                                        <tr>
                                                                            <td style="width: 20%">{{index + 1}}</td>
                                                                            <td style="width: 80%" v-on:click="items.IsCurrentShow = !items.IsCurrentShow">{{items.CompanyName}}</td>
                                                                        </tr>
                                                                    </table>
                                                                </th>
                                                                <td style="width: 22.1%">{{Vue.ToThousands(items.NPlanAmmount)}}</td>
                                                                <td style="width: 22.1%">{{Vue.ToThousands(items.NActualAmmount)}}</td>
                                                                <td style="width: 22%">{{items.NDisplayRate}}</td>
                                                            </tr>
                                                                <template v-if="false && items.IsCurrentShow">
                                                                     <tr class="tem-item">
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
                         <li class="active">
                            <h3 v-bind:class="{'bottom': totalState}">{{result.Name}}(累计 单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': totalState}" v-on:click="totalState = !totalState"></span></h3>
                            <div class="showBox" v-if="!totalState">
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
                                            <template v-for="list in result.ObjValue">
                                            <tr>
                                                <td v-on:click="list.IsTotalShow = !list.IsTotalShow">{{list.Name}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NPlanAmmount)}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NActualAmmount)}}</td>
                                                <td class="color-height">{{list.BMonthReportDetail.NDisplayRate}}</td>
                                            </tr>
                                            <tr class="new-tem" v-if="list.ObjValue.length && list.IsTotalShow">
                                                <td colspan="4" style="padding: 0;">
                                                    <div class="clear table-tem-new">
                                                        <table>
                                                            <template v-for="(items,index) in list.ObjValue">
                                                            <tr>
                                                                <th style="padding: 0; width: 33.8%">
                                                                    <table>
                                                                        <tr>
                                                                            <td style="width: 20%">{{index + 1}}</td>
                                                                            <td style="width: 80%" v-on:click="items.IsTotalShow = !items.IsTotalShow">{{items.CompanyName}}</td>
                                                                        </tr>
                                                                    </table>
                                                                </th>
                                                                <td style="width: 22.1%">{{Vue.ToThousands(items.NPlanAmmount)}}</td>
                                                                <td style="width: 22.1%">{{Vue.ToThousands(items.NActualAmmount)}}</td>
                                                                <td style="width: 22%">{{items.NDisplayRate}}</td>
                                                            </tr>
                                                                <template v-if="false && items.IsTotalShow">
                                                                     <tr class="tem-item">
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
