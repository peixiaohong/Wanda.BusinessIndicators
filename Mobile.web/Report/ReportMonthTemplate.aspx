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
                                                <td v-on:click="if(list.ObjValue && list.ObjValue.length){list.IsCurrentShow = !list.IsCurrentShow}">{{list.Name}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NPlanAmmount)}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NActualAmmount)}}</td>
                                                <td :class="{'color-height': Vue.Height(list.BMonthReportDetail.NDisplayRate)}">{{list.BMonthReportDetail.NDisplayRate}}</td>
                                            </tr>
                                            <tr class="new-tem" v-if="list.ObjValue && list.ObjValue.length && list.IsCurrentShow">
                                                <td colspan="4" style="padding: 0;">
                                                    <div class="clear table-tem-new">
                                                        <table>
                                                            <template v-for="(items,index) in list.ObjValue">
                                                            <tr>
                                                                <th style="padding: 0; width: 33.8%">
                                                                    <table>
                                                                        <tr>
                                                                            <td style="width: 30%">{{index + 1}}</td>
                                                                            <td style="width: 70%" v-on:click="if(items.ObjValue && items.ObjValue.length){items.IsCurrentShow = !items.IsCurrentShow}">{{type ? items.Name : items.CompanyName}}</td>
                                                                        </tr>
                                                                    </table>
                                                                </th>
                                                                <td style="width: 22.1%">{{type ? Vue.ToThousands(items.BMonthReportDetail.NPlanAmmount) : Vue.ToThousands(items.NPlanAmmount)}}</td>
                                                                <td style="width: 22.1%">{{type ? Vue.ToThousands(items.BMonthReportDetail.NActualAmmount) : Vue.ToThousands(items.NActualAmmount)}}</td>
                                                                <td style="width: 22%" :class="{'color-height': type ? Vue.Height(items.BMonthReportDetail.NDisplayRate) : Vue.Height(items.NDisplayRate)}">{{type ? items.BMonthReportDetail.NDisplayRate : items.NDisplayRate}}</td>
                                                            </tr>
                                                                <template v-if="items.ObjValue && items.ObjValue.length && items.IsCurrentShow">
                                                               <tr class="tem-item">
                                                                <td colspan="4" style="padding: 0;">
                                                                    <div class="clear tem-list">
                                                                        <table>
                                                                            <tr v-for="(item,c) in items.ObjValue">
                                                                                <td style="padding: 0; width: 33.8%">
                                                                                    <table style="width: 100%">
                                                                                        <tr>
                                                                                            <td style="width: 30%">{{c + 1}}</td>
                                                                                            <td style="width: 70%">{{item.CompanyName}}</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>

                                                                                <td style="width: 22.1%">{{Vue.ToThousands(item.NPlanAmmount)}}</td>
                                                                                <td style="width: 22.1%">{{Vue.ToThousands(item.NActualAmmount)}}</td>
                                                                                <td style="width: 22%" :class="{'color-height': Vue.Height(item.NDisplayRate)}">{{item.NDisplayRate}}</td>
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
                                                <td v-on:click="if(list.ObjValue && list.ObjValue.length){list.IsTotalShow = !list.IsTotalShow}">{{list.Name}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NAccumulativePlanAmmount)}}</td>
                                                <td>{{Vue.ToThousands(list.BMonthReportDetail.NAccumulativeActualAmmount)}}</td>
                                                <td :class="{'color-height': Vue.Height(list.BMonthReportDetail.NAccumulativeDisplayRate)}">{{list.BMonthReportDetail.NAccumulativeDisplayRate}}</td>
                                            </tr>
                                            <tr class="new-tem" v-if="list.ObjValue && list.ObjValue.length && list.IsTotalShow">
                                                <td colspan="4" style="padding: 0;">
                                                    <div class="clear table-tem-new">
                                                        <table>
                                                            <template v-for="(items,index) in list.ObjValue">
                                                            <tr>
                                                                <th style="padding: 0; width: 33.8%">
                                                                    <table>
                                                                        <tr>
                                                                            <td style="width: 30%">{{index + 1}}</td>
                                                                            <td style="width: 70%" v-on:click="if(items.ObjValue && items.ObjValue.length){items.IsTotalShow = !items.IsTotalShow}">{{type ? items.Name : items.CompanyName}}</td>
                                                                        </tr>
                                                                    </table>
                                                                </th>
                                                                <td style="width: 22.1%">{{type ? Vue.ToThousands(items.BMonthReportDetail.NAccumulativePlanAmmount): Vue.ToThousands(items.NAccumulativePlanAmmount)}}</td>
                                                                <td style="width: 22.1%">{{type ? Vue.ToThousands(items.BMonthReportDetail.NAccumulativeActualAmmount) : Vue.ToThousands(items.NAccumulativeActualAmmount)}}</td>
                                                                <td style="width: 22%" :class="{'color-height': type ? Vue.Height(items.BMonthReportDetail.NAccumulativeDisplayRate) : Vue.Height(items.NAccumulativeActualRate)}">{{type ? items.BMonthReportDetail.NAccumulativeDisplayRate : items.NAccumulativeActualRate}}</td>
                                                            </tr>
                                                                <template v-if="items.ObjValue && items.ObjValue.length && items.IsTotalShow">
                                                               <tr class="tem-item">
                                                                <td colspan="4" style="padding: 0;">
                                                                    <div class="clear tem-list">
                                                                        <table>
                                                                            <tr v-for="(item,c) in items.ObjValue">
                                                                                <td style="padding: 0; width: 33.8%">
                                                                                    <table style="width: 100%">
                                                                                        <tr>
                                                                                            <td style="width: 30%">{{c + 1}}</td>
                                                                                            <td style="width: 70%">{{item.CompanyName}}</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>

                                                                                <td style="width: 22.1%">{{Vue.ToThousands(item.NAccumulativePlanAmmount)}}</td>
                                                                                <td style="width: 22.1%">{{Vue.ToThousands(item.NAccumulativeActualAmmount)}}</td>
                                                                                <td style="width: 22%" :class="{'color-height': Vue.Height(item.NAccumulativeDisplayRate)}">{{item.NAccumulativeDisplayRate}}</td>
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
