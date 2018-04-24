<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportMonth.aspx.cs" Inherits="Mobile.web.Report.ReportMonth" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <header id="header">
        <div class="mobile-header">
            <div id="navbar" role="navigation" class="navbar navbar-default">
                <button type="button" data-toggle="collapse" data-target="#sm-navbar-collapse" class="navbar-toggle btn-toggle mobile-toggle collapsed" aria-expanded="false">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <h1 class="header-t">
                    月度经营报告
                </h1>
                <div id="sm-navbar-collapse" class="navbar-collapse mobile-navbar-collapse collapse" aria-expanded="false" style="height: 1px;">
                    <ul id="navbar-menu" class="nav navbar-nav mobile-nav">
                        <li class="all-template">
                            <a href="#" class="clear"><span>月度经营报告</span></a>
                        </li>
                        <li class="quick-start">
                            <a href="#" class="clear"><span>分解指标查询</span></a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </header>
    <section id="content">
        <div class="container">
            <div class="row">
                <div class="page-content col-lg-9 col-md-9 col-sm-9 col-xs-9">
                    <div id="vue">
                        <div class="widget-body clear" style="padding-top: 0px;">
                            <div class="page-body" style="margin:0">
                                <div class="collection-cont col-md-12 col-sm-12">
                                    <ul style="padding-top:0;border-top: 0 solid;">
                                            <li>
                                                <div style="margin-top:15px;">
                                                    <table class="from-table" style="line-height: 1.7">
                                                        <tbody>
                                                            <tr>
                                                                <td style="width:50%">
                                                                    <div class="select-container">
                                                                        <select class="form-control select-item mobile-select repeort-select">
                                                                            <option value="1">房地产事业部</option>
                                                                        </select>
                                                                    </div>
                                                                </td>
                                                                <td style="width:30%">
                                                                    <div class="select-container clear">
                                                                        <select class="form-control select-item mobile-select repeort-select">
                                                                            <option value="2">2013年</option>
                                                                            <option value="3">2014年</option>
                                                                            <option value="4">2015年</option>
                                                                            <option value="5">2016年</option>
                                                                            <option value="6">2017年</option>
                                                                            <option value="1" selected="selected">2018年</option>
                                                                            <option value="7">2019年</option>
                                                                            <option value="8">2020年</option>
                                                                            <option value="9">2021年</option>
                                                                            <option value="10">2022年</option>
                                                                        </select>          
                                                                    </div>
                                                                </td>
                                                                <td style="width:25%">
                                                                    <div class="select-container">
                                                                        <select id="taskType" class="form-control select-item mobile-select repeort-select">
                                                                            <option value="1">1月</option>
                                                                            <option value="4" selected="selected">2月</option>
                                                                            <option value="3">3月</option>
                                                                            <option value="2">4月</option>
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
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </li>
                                            <li>
                                                <h3 style="border-bottom:1px solid #ced6e0">月度说明<span class="collection-updown-icon report-state-down"></span></h3>
                                                <div class="showBox report-state" style="display: none;">
                                                    <p>一、2月累计经营指标完成情况：</p>
                                                    <p>1、销售收入：计划20,000万元，实际完成 22,600万元，累计完成率113%</p>
                                                    <p>2、回款收入：计划30,000万元，实际完成24,500万元，累计完成率82%</p>
                                                    <p>3、利润额收入：计划9,000万元，实际完成8,700万元，累计完成率97%</p>
                                                    <p>二、2月当月经营指标完成情况：</p>
                                                    <p>1、销售收入：计划8,900万元，实际完成 8,861万元，累计完成率99.6%</p>
                                                    <p>2、回款收入：计划17,800万元，实际完成17,758万元，累计完成率99.8%</p>
                                                    <p>3、利润额收入：计划4,450万元，实际完成4,448万元，累计完成率100%</p>
                                                </div>
                                            </li>

                                            <li class="active">
                                                <h3>月度经营报告(本月 单位:万元)<span class="collection-updown-icon"></span></h3>
                                                <!--<div class="collection-result">-->
                                                <div class="showBox">
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
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">销售</a>
                                                                </td>
                                                                <td>8,900</td>
                                                                <td>8,861</td>
                                                                <td>99.6%</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">回款</a>
                                                                </td>
                                                                <td>17,800</td>
                                                                <td>17,758</td>
                                                                <td>99.8%</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">利润额</a>
                                                                </td>
                                                                <td>4,450</td>
                                                                <td>4,448</td>
                                                                <td>100%</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </li>
                                            <li class="active">
                                                <h3>月度经营报告(累计  单位:万元)<span class="collection-updown-icon"></span></h3>
                                                <!--<div class="collection-result">-->
                                                <div class="showBox">
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
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">销售</a>
                                                                </td>
                                                                <td>20,000</td>
                                                                <td>22,600</td>
                                                                <td>113%</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">回款</a>
                                                                </td>
                                                                <td>30,000</td>
                                                                <td>24,500</td>
                                                                <td>82%</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">利润额</a>
                                                                </td>
                                                                <td>9,000</td>
                                                                <td>8,700</td>
                                                                <td>97%</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </li>
                                            <li class="active">
                                                <h3>月度经营报告(全年  单位:万元)<span class="collection-updown-icon"></span></h3>
                                                <!--<div class="collection-result">-->
                                                <div class="showBox">
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
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">销售</a>
                                                                </td>
                                                                <td>110,000</td>
                                                                <td>22,600</td>
                                                                <td>21%</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">回款</a>
                                                                </td>
                                                                <td>180,000</td>
                                                                <td>24,500</td>
                                                                <td>14%</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="./ReportMonthTemplate.aspx">利润额</a>
                                                                </td>
                                                                <td>54,000</td>
                                                                <td>8,700</td>
                                                                <td>16%</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </li>
                                        </ul>

                                </div>

                             
                            </div>
                        </div>

                        <!---->
                    </div>

                </div>

            </div>
        </div>
    </section>
</asp:Content>
