<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportMonthTemplate.aspx.cs" Inherits="Mobile.web.Report.ReportMonthTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <header id="header">
        <!---->
        <div class="mobile-header">
            <div id="navbar" role="navigation" class="navbar navbar-default">
                <button type="button" data-toggle="collapse" data-target="#sm-navbar-collapse" class="navbar-toggle btn-toggle mobile-toggle collapsed" aria-expanded="false">
                    <span class="sr-only">Toggle navigation</span> 
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <h1 class="header-t">月度经营报告
                </h1>
                <div id="sm-navbar-collapse" class="navbar-collapse mobile-navbar-collapse collapse" aria-expanded="false" style="height: 1px;">
                    <ul id="navbar-menu" class="nav navbar-nav mobile-nav">
                        <li class="all-template">
                            <a href="TargetRptNew.html" class="clear"><span>月度经营报告</span></a>
                        </li>
                        <li class="quick-start">
                            <a href="TargetDisplay.html" class="clear"><span>分解指标查询</span></a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </header>
    <section id="content" style="padding-top: 70px;">
        <div class="container">
            <div class="row">
                <div class="page-content col-lg-9 col-md-9 col-sm-9 col-xs-9">
                    <div id="vue">
                        <div class="widget-body clear" style="padding-top: 0px;">
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
                                                            <td style="width: 25%">2月</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li class="active">
                                            <h3>销售额(本月 单位:万元)<span class="collection-updown-icon"></span></h3>
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
                                                        <tr>
                                                            <td>总部</td>
                                                            <td>2,000</td>
                                                            <td>2,000</td>
                                                            <td>100%</td>
                                                        </tr>
                                                        <tr>
                                                            <td>环京区域</td>
                                                            <td>4,400</td>
                                                            <td>3,861</td>
                                                            <td class="color-height">88%</td>
                                                        </tr>
                                                        <tr class="new-tem">
                                                            <td colspan="4" style="padding: 0;">
                                                                <div class="clear table-tem-new">
                                                                    <table>
                                                                        <tr>
                                                                            <th style="padding: 0; width: 33.8%">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td style="width: 20%">1</td>
                                                                                        <td style="width: 80%">涿州区域</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </th>
                                                                            <td style="width: 22.1%">2,900</td>
                                                                            <td style="width: 22.1%">2,361</td>
                                                                            <td style="width: 22%" class="color-height">82%</td>
                                                                        </tr>
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
                                                                        <tr>
                                                                            <th style="padding: 0; width: 33.8%">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td style="width: 20%">2</td>
                                                                                        <td style="width: 80%">廊坊区域</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </th>
                                                                            <td>1,500</td>
                                                                            <td>1,500</td>
                                                                            <td>100%</td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>冀南区域</td>
                                                            <td>3,500</td>
                                                            <td>3,000</td>
                                                            <td class="color-height">86%</td>
                                                        </tr>
                                                        <tr>
                                                            <td>合计</td>
                                                            <td>8,900</td>
                                                            <td>8,861</td>
                                                            <td>99.6%</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
