<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="ReportTarget.aspx.cs" Inherits="Mobile.web.Report.ReportTarget" %>

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
                <h1 class="header-t">分解指标查询
                </h1>
                <div id="sm-navbar-collapse" class="navbar-collapse mobile-navbar-collapse collapse" aria-expanded="false" style="height: 1px;">
                    <ul id="navbar-menu" class="nav navbar-nav mobile-nav">
                        <li class="quick-start">
                            <a href="TargetRptNew.html" class="clear"><span>月度经营报告</span></a>
                        </li>
                        <li class="all-template">
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
                                                            <td style="width: 40%">
                                                                <div class="select-container">
                                                                    <select class="form-control select-item mobile-select repeort-select">
                                                                        <option value="1">房地产事业部</option>
                                                                    </select>
                                                                    <span class="icon-select"></span>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%">
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
                                                                    <span class="icon-select"></span>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%">
                                                                <div class="select-container">
                                                                    <select id="taskType" class="form-control select-item mobile-select repeort-select">
                                                                        <option value="1">考核版</option>
                                                                        <option value="2">内控版</option>
                                                                    </select>
                                                                    <span class="icon-select"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>

                                        <li class="active">
                                            <h3>指标分解(单位:万元)<span class="collection-updown-icon"></span></h3>
                                            <!--<div class="collection-result">-->
                                            <div class="showBox">
                                                <table class="from-table alignCenter">
                                                    <thead>
                                                        <tr>
                                                            <th>月份</th>
                                                            <th>销售</th>
                                                            <th>回款</th>
                                                            <th>利润额</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>1月</td>
                                                            <td>8,900</td>
                                                            <td>17,800</td>
                                                            <td>4,450</td>
                                                        </tr>
                                                        <tr>
                                                            <td>2月</td>
                                                            <td>17,800</td>
                                                            <td>35,600</td>
                                                            <td>8,900</td>
                                                        </tr>
                                                        <tr>
                                                            <td>3月</td>
                                                            <td>26,700</td>
                                                            <td>53,400</td>
                                                            <td>13,350</td>
                                                        </tr>
                                                        <tr>
                                                            <td>4月</td>
                                                            <td>35,600</td>
                                                            <td>71,200</td>
                                                            <td>17,800</td>
                                                        </tr>
                                                        <tr>
                                                            <td>5月</td>
                                                            <td>44,500</td>
                                                            <td>89,000</td>
                                                            <td>22,250</td>
                                                        </tr>
                                                        <tr>
                                                            <td>6月</td>
                                                            <td>53,400</td>
                                                            <td>106,800</td>
                                                            <td>26,700</td>
                                                        </tr>
                                                        <tr>
                                                            <td>7月</td>
                                                            <td>62,300</td>
                                                            <td>124,600</td>
                                                            <td>31,150</td>
                                                        </tr>
                                                        <tr>
                                                            <td>8月</td>
                                                            <td>71,200</td>
                                                            <td>142,400</td>
                                                            <td>35,600</td>
                                                        </tr>
                                                        <tr>
                                                            <td>9月</td>
                                                            <td>80,100</td>
                                                            <td>160,200</td>
                                                            <td>40,050</td>
                                                        </tr>
                                                        <tr>
                                                            <td>10月</td>
                                                            <td>89,000</td>
                                                            <td>178,000</td>
                                                            <td>44,550</td>
                                                        </tr>
                                                        <tr>
                                                            <td>11月</td>
                                                            <td>97,900</td>
                                                            <td>195,800</td>
                                                            <td>48,950</td>
                                                        </tr>
                                                        <tr>
                                                            <td>12月</td>
                                                            <td>106,800</td>
                                                            <td>213,360</td>
                                                            <td>53,400</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                                </
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
