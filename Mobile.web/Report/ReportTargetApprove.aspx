<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterApprove.Master" AutoEventWireup="true" CodeBehind="ReportTargetApprove.aspx.cs" Inherits="Mobile.web.Report.ReportTargetApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportTargetApprove.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <section id="ReportApproveContent">
        <div class="container" style="padding-top:0;">
            <div class="row">

                <div id="taskCollectionViewMobile" class="white-bg clear">
                    <div class="white-wrap col-md-12 col-sm-12">
                        <div class="collection-top text-center">
                            <h1 class="collection-t">房地产事业部2018年02月度报告</h1>
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
                                                    <td>房地产事业部</td>
                                                </tr>
                                                <tr>
                                                    <td>上报年份</td>
                                                    <td>2018年</td>

                                                </tr>
                                                <tr>
                                                    <td>上报月份</td>
                                                    <td>2月</td>
                                                </tr>
                                                <tr>
                                                    <td>上报说明</td>
                                                    <td>
                                                        <p>1、销售收入：计划17,800万元，实际完成 17,655万元，累计完成率99.2%</p>
                                                        <p>2、回款收入：计划35,600万元，实际完成35,488万元，累计完成率99.7%</p>
                                                        <p>3、利润额收入：计划8,900万元，实际完成8,893万元，累计完成率99.7%</p>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                                <li class="active">
                                    <h3>月度经营报告（本月）<span class="collection-updown-icon"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div>
                                        <table class="from-table alignCenter">
                                            <thead>
                                                <tr>
                                                    <th>项目</th>
                                                    <th>本月计划（万元）</th>
                                                    <th>本月实际（万元）</th>
                                                    <th>完成率</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>销售</td>
                                                    <td>8,900</td>
                                                    <td>8,861</td>
                                                    <td>99.6%</td>
                                                </tr>
                                                <tr>
                                                    <td>回款</td>
                                                    <td>17,800</td>
                                                    <td>17,758</td>
                                                    <td>99.8%</td>
                                                </tr>
                                                <tr>
                                                    <td>利润额</td>
                                                    <td>4,450</td>
                                                    <td>4,448</td>
                                                    <td>100%</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                                <li class="active">
                                    <h3>月度经营报告（累计）<span class="collection-updown-icon"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div>
                                        <table class="from-table alignCenter">
                                            <thead>
                                                <tr>
                                                    <th>项目</th>
                                                    <th>本月计划（万元）</th>
                                                    <th>本月实际（万元）</th>
                                                    <th>完成率</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>销售</td>
                                                    <td>17,800</td>
                                                    <td>17,655</td>
                                                    <td>99.2%</td>
                                                </tr>
                                                <tr>
                                                    <td>回款</td>
                                                    <td>35,600</td>
                                                    <td>35,488</td>
                                                    <td>99.7%</td>
                                                </tr>
                                                <tr>
                                                    <td>利润额</td>
                                                    <td>8,900</td>
                                                    <td>8,893</td>
                                                    <td>99.7%</td>
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
