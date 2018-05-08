<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterApprove.Master" AutoEventWireup="true" CodeBehind="ReportTargetApprove.aspx.cs" Inherits="Mobile.web.Report.ReportTargetApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportTargetApprove.js?v=")+System.Guid.NewGuid() %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <section id="ReportApproveContent" v-cloak>
        <div class="container" style="padding-top:0;">
            <div class="row">
                <div id="taskCollectionViewMobile" class="white-bg clear">
                    <div class="white-wrap col-md-12 col-sm-12">
                        <div class="collection-top text-center">
                            <h1 class="collection-t">{{title}}</h1>
                        </div>
                        <div class="collection-cont col-md-12 col-sm-12">
                            <ul>
                                <li class="active">
                                     <h3 v-bind:class="{'bottom': basicState}">基本信息<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': basicState}" v-on:click="basicState = !basicState"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div v-if="!basicState">
                                        <table class="from-table">
                                            <tbody>
                                                <tr>
                                                    <td width="35%">上报系统</td>
                                                    <td>{{systemName}}</td>
                                                </tr>
                                                <tr>
                                                    <td>上报年份</td>
                                                    <td>{{Year}}年</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                                <li class="active">
                            <h3 v-bind:class="{'bottom': targetState}">指标分解(单位:万元)<span v-bind:class="{'collection-updown-icon': true, 'collection-down-icon': targetState}" v-on:click="targetState = !targetState"></span></h3>
                            <!--<div class="collection-result">-->
                            <div class="target" v-cloak v-if="!targetState">
                                <img src="../Assets/images/arrow-right.png" class="target-allow" data-allow="right" v-if="list[0].TargetDetailList.length > 3" />
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
                                                <th v-for="item in list[0].TargetDetailList">{{item.TargetName}}</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="(item,index) in list">
                                                <td>{{item.FinMonth}}月</td>
                                                <td v-for="target in item.TargetDetailList">{{Vue.ToThousands(target.SumTarget)}}</td>
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
                <div id="SJSJ_LC"></div>
            </div>
    </section>

</asp:Content>
