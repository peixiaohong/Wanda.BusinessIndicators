<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterApprove.Master" AutoEventWireup="true" CodeBehind="ReportMonthApprove.aspx.cs" Inherits="Mobile.web.Report.ReportMonthApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportMonthApprove.js?v=") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <section id="MonthApproveContent" v-cloak>
        <div class="container" style="padding-top:0;">
            <div class="row">

                <div id="taskCollectionViewMobile" class="white-bg clear">
                    <div class="white-wrap col-md-12 col-sm-12">
                        <div class="collection-top text-center">
                            <h1 class="collection-t">{{head.SystemName}}{{head.FinYear}}-{{head.FinMonth}}{{list.Name}}</h1>
                        </div>
                        <div class="collection-cont col-md-12 col-sm-12">
                            <ul>
                                <li>
                                    <h3 v-bind:class="{'bottom': !reportState}">基本信息<span v-bind:class="{'collection-updown-icon': true, 'collection-up-icon': reportState}" v-on:click="reportState = !reportState"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div v-if="reportState">
                                        <table class="from-table">
                                            <tbody>
                                                <tr>
                                                    <td width="35%">上报版块</td>
                                                    <td>{{head.SystemName}}</td>
                                                </tr>
                                                <tr>
                                                    <td>上报年份</td>
                                                    <td>{{head.FinYear}}年</td>

                                                </tr>
                                                <tr>
                                                    <td>上报月份</td>
                                                    <td>{{head.FinMonth}}月</td>
                                                </tr>
                                                <tr>
                                                    <td>上报说明</td>
                                                    <td>
                                                        <p v-html="Vue.Trim(head.ObjValue)"></p>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                                 <li class="active">
                        <h3 v-bind:class="{'bottom': currentState}">{{list.Name}}(本月 单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': currentState}" v-on:click="currentState = !currentState"></span></h3>
                        <!--<div class="collection-result">-->
                        <div class="showBox" v-if="!currentState">
                            <table class="from-table alignCenter">
                                <thead>
                                    <tr>
                                        <th>项目</th>
                                        <th>计划</th>
                                        <th>实际</th>
                                        <th>完成率</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in list.ObjValue">
                                        <td>{{item.TargetName}}</td>
                                        <td>{{Vue.ToThousands(item.NPlanAmmount)}}</td>
                                        <td>{{Vue.ToThousands(item.NActualAmmount)}}</td>
                                        <td>{{item.NActualRate}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </li>
                    <li class="active">
                        <h3 v-bind:class="{'bottom': totalState}">{{list.Name}}(累计 单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': totalState}" v-on:click="totalState = !totalState"></span></h3>
                        <!--<div class="collection-result">-->
                        <div class="showBox" v-if="!totalState">
                            <table class="from-table alignCenter">
                                <thead>
                                    <tr>
                                        <th>项目</th>
                                        <th>计划</th>
                                        <th>实际</th>
                                        <th>完成率</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in list.ObjValue">
                                        <td>{{item.TargetName}}</td>
                                        <td>{{Vue.ToThousands(item.NAccumulativePlanAmmount)}}</td>
                                        <td>{{Vue.ToThousands(item.NAccumulativeActualAmmount)}}</td>
                                        <td>{{item.NAccumulativeActualRate}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </li>
                    <li class="active">
                        <h3 v-bind:class="{'bottom': yearlyState}">{{list.Name}}(全年 单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': yearlyState}" v-on:click="yearlyState = !yearlyState"></span></h3>
                        <!--<div class="collection-result">-->
                        <div class="showBox" v-if="!yearlyState">
                            <table class="from-table alignCenter">
                                <thead>
                                    <tr>
                                        <th>项目</th>
                                        <th>计划</th>
                                        <th>实际</th>
                                        <th>完成率</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in list.ObjValue">
                                        <td>{{item.TargetName}}</td>
                                        <td>{{Vue.ToThousands(item.MeasureRate)}}</td>
                                        <td>{{Vue.ToThousands(item.NAccumulativeActualAmmount)}}</td>
                                        <td>{{item.NAnnualCompletionRate}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div id="SJSJ_LC"></div>
            </div>
    </section>

</asp:Content>
