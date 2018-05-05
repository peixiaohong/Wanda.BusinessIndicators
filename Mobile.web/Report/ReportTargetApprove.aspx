<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterApprove.Master" AutoEventWireup="true" CodeBehind="ReportTargetApprove.aspx.cs" Inherits="Mobile.web.Report.ReportTargetApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportTargetApprove.js") %>"></script>
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
                                    <h3>基本信息<span class="collection-updown-icon"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div>
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
                            <h3>指标分解(单位:万元)<span class="collection-updown-icon"></span></h3>
                            <!--<div class="collection-result">-->
                            <div class="target" v-cloak>
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
                                                <td v-for="target in item.TargetDetailList">{{ToThousands(target.SumTarget)}}</td>
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
