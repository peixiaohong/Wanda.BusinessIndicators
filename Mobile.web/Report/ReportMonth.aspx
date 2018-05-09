<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportMonth.aspx.cs" Inherits="Mobile.web.Report.ReportMonth" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportMonth.js?v=")+System.Guid.NewGuid() %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="ReportMonthContent" v-cloak>
        <div class="widget-body clear">
            <div class="collection-cont col-md-12 col-sm-12">
                <ul style="padding-top: 0; border-top: 0 solid;">
                    <li>
                        <div style="margin-top: 15px;">
                            <table class="from-table" style="line-height: 1.7">
                                <tbody>
                                    <tr>
                                        <td style="width: 28%">
                                            <div class="select-container">
                                                <select class="form-control select-item mobile-select repeort-select" v-model="systemID" v-on:change="ChangeVersion()">
                                                    <option :value="system.ID" v-for="(system,index) in systemAndYearList.System" selected>{{system.SystemName}}</option>
                                                </select>
                                            </div>
                                        </td>
                                        <td style="width: 22%">
                                            <div class="select-container clear">
                                                <select class="form-control select-item mobile-select repeort-select" v-model="yearSelect" v-on:change="ChangeVersion()">
                                                    <option :value="year" v-for="year in systemAndYearList.Year">{{year}}年</option>
                                                </select>
                                            </div>
                                        </td>
                                        <td style="width: 18%">
                                            <div class="select-container">
                                                <select id="taskType" class="form-control select-item mobile-select repeort-select" v-model="monthSelect" v-on:change="ChangeVersion()">
                                                    <option value="1">1月</option>
                                                    <option value="2">2月</option>
                                                    <option value="3">3月</option>
                                                    <option value="4">4月</option>
                                                    <option value="5">5月</option>
                                                    <option value="6">6月</option>
                                                    <option value="7">7月</option>
                                                    <option value="8">8月</option>
                                                    <option value="9">9月</option>
                                                    <option value="10">10月</option>
                                                    <option value="11">11月</option>
                                                    <option value="12">12月</option>
                                                </select>
                                            </div>
                                        </td>
                                         <td style="width: 32%">
                                            <div class="select-container clear">
                                                <select class="form-control select-item mobile-select repeort-select" v-model="versionSelect" v-on:change="ChangeData()">
                                                    <option :value="versionType.ID" v-for="(versionType,index) in versions">{{versionType.VersionName}}</option>
                                                </select>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </li>
                    <li>
                        <h3 v-bind:class="{'bottom': !reportState}">月度说明<span v-bind:class="{'collection-updown-icon': true, 'collection-up-icon': reportState}" v-on:click="if(title.length){reportState = !reportState}"></span></h3>
                        <div class="showBox report-state" v-if="reportState" v-html="Vue.Trim(title)"></div>
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
                                        <td><a v-bind:href="'/APP/Report/ReportMonthTemplate.aspx?id='+ item.SystemID + '&versionID='+ versionSelect + '&year=' + yearSelect + '&month=' + monthSelect + '&name=' + encodeURI(item.TargetName)">{{item.TargetName}}</a></td>
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
                                        <td><a v-bind:href="'/APP/Report/ReportMonthTemplate.aspx?id='+ item.SystemID + '&versionID='+ versionSelect + '&year=' + yearSelect + '&month=' + monthSelect + '&name=' + encodeURI(item.TargetName)">{{item.TargetName}}</a></td>
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
                                        <td><a v-bind:href="'/APP/Report/ReportMonthTemplate.aspx?id='+ item.SystemID + '&versionID='+ versionSelect + '&year=' + yearSelect + '&month=' + monthSelect + '&name=' + encodeURI(item.TargetName)">{{item.TargetName}}</a></td>
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
    </section>
</asp:Content>
