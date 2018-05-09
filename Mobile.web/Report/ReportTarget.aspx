<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportTarget.aspx.cs" Inherits="Mobile.web.Report.ReportTarget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportTarget.js?v=")+System.Guid.NewGuid() %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="ReportTargetContent" v-cloak>
        <div class="widget-body clear">
            <div class="page-body" style="margin: 0">
                <div class="collection-cont col-md-12 col-sm-12">
                    <ul style="padding-top: 0; border-top: 0 solid;">
                        <li class="active">
                            <div style="margin-top: 15px;">
                                <table class="from-table" style="line-height: 1.7">
                                    <tbody>
                                        <tr>
                                            <td style="width: 35%">
                                                <div class="select-container">
                                                    <select class="form-control select-item mobile-select repeort-select target-select" v-model="systemID" v-on:change="InitVersion()">
                                                        <option :value="system.ID" v-for="(system,index) in systemAndYearList.System">{{system.SystemName}}</option>
                                                    </select>
                                                </div>
                                            </td>
                                            <td style="width: 30%">
                                                <div class="select-container clear">
                                                    <select class="form-control select-item mobile-select repeort-select target-select" v-model="yearSelect" v-on:change="InitVersion()">
                                                        <option :value="year" v-for="year in systemAndYearList.Year">{{year}}年</option>
                                                    </select>
                                                </div>
                                            </td>
                                            <td style="width: 35%">
                                                <div class="select-container">
                                                    <select class="form-control select-item mobile-select repeort-select target-select" v-model="versionSelect" v-on:change="ChangeData()">
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
                            <h3 v-if="!head.length" style="text-align:center">暂无数据...</h3>
                            <h3 v-bind:class="{'bottom': !reportState}"  v-if="head.length">指标分解(单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-up-icon': reportState}" v-on:click="reportState = !reportState"></span></h3>
                            <!--<div class="collection-result">-->
                            <div class="target" v-cloak v-if="reportState">
                                <img src="../Assets/images/arrow-right.png" class="target-allow" data-allow="right" v-if="head.length > 3" />
                                <div class="target-main" v-if="list.length">
                                    <div>
                                        <table class="from-table alignCenter target-name">
                                            <thead>
                                                <tr>
                                                    <th>月份</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(item,index) in list">
                                                    <td>{{item.FinMonth}}月</td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                    <table class="from-table alignCenter target-content">
                                        <thead>
                                            <tr>
                                                <th>月份</th>
                                                <th v-for="item in head">{{item.TargetName}}</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="(item,index) in list">
                                                <td>{{item.FinMonth}}月</td>
                                                <td v-for="target in item.TargetDetailList">{{Vue.ToThousands(target.Target)}}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    </
                                </div>

                            </div>
                        </li>
                    </ul>
                </div>

            </div>
        </div>

    </section>
</asp:Content>
