<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetTotalDetail.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master"   Inherits="Wanda.BusinessIndicators.Web.Demo.TargetTotalDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="">
            <div class="title">
                <a href="#"><span>查询条件</span></a>
            </div>
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div class="content_m">
                <table class="tab_search">
                    <tbody>
                        <tr>
                            <th>上报系统</th>
                            <td>
                                <select id="selectvalue" style="width: 120px; height: 25px; margin-top: 5px" >
                                        <option selected="selected">商管系统</option>
                                    <option>物管公司</option>
                                    <option>酒馆系统</option>
                                    <option>院线系统</option>
                                    <option>大歌星系统</option>
                                    <option>万达旅业</option>
                                    <option>百货系统</option>
                                     <option>儿童娱乐公司</option>
                                     <option>商业项目(南区)</option>
                                     <option>商业项目(北区)</option>
                                     <option>商业项目(中区)</option>
                                     <option>文旅项目</option>
                                     <option>酒店建设</option>
                                     <option>酒店设计院</option>
                                     <option>商业规划院</option>
                                     <option>万达学院</option>
                                     <option>电子商务</option>
                                     <option>游艇公司</option>
                                    <option>有机农业</option>
                                     <option>文旅规划院</option>
                                    <option>华夏时报</option>
                                    <option>影视传媒</option>
                                    <option>长白山管理公司</option>
                                    <option>昆明高尔夫</option>
                                    <option>大众电影</option>
                                    <option>投资公司</option>
                                    <option>五洲发行</option>
                                    <option>武汉电影乐园</option>
                                    <option>武汉秀场</option>
                                    <option>西双版纳管理公司</option>
                                    <option>集团总部</option>
                                    <option>商业地产总部</option>
                                     <option>文化集团总部</option>
                                </select>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <select id="select1" style="width: 120px; height: 25px; margin-top: 5px" onchange="change()">
                                    <option>2010</option>
                                    <option>2011</option>
                                    <option>2012</option>
                                    <option>2013</option>
                                    <option>2014</option>
                                    <option selected="selected">2015</option>
                                    <option>2016</option>
                                    <option>2017</option>
                                    <option>2018</option>
                                    <option>2019</option>
                                </select>
                            </td>
                          
                            <th>
                                <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" ><span>查询</span></a>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
            <!--明细项 开始-->
            <!--明细项 开始-->
            <div class="margin_t10">
                <div class="tabs_m">
                    <ul id="Ul3">
                        <li class="selected m_1"><a class="active_sub2"><span>计划指标</span></a></li>
                    </ul>
                    <div class="DownExcelDiv" id="DownExcel">
                        <span class="DownExcelspan">
                            <div class="uploadify-button" id="file_upload-button1">
                                <span>
                                    <label class="DownExcelLabel" id="LabelDownload">导出计划指标</label></span>
                            </div>
                        </span>
                    </div>
                </div>
            </div>
            <div class="tabs_content">
                <div class="scrolldoorFrame copy">
                    <table class="tab_005" id="importedDataTable2">
                        <thead id="TargetPlanDetailHead">
                            <%--           <tr>
                                <th rowspan="2" style="width: 12%">月份</th>
                                <th rowspan="1" colspan="4">当前数</th>
                                <th rowspan="1" colspan="4">累计数</th>
                            </tr>
                            <tr>
                                <th style="width: 11%">收入</th>
                                <th style="width: 11%">净利润</th>
                                <th style="width: 11%">其他指标</th>
                                <th style="width: 11%">总部管理费用<br />
                                    (成本费用)</th>
                                <th style="width: 11%">收入</th>
                                <th style="width: 11%">净利润</th>
                                <th style="width: 11%">其他指标</th>
                                <th style="width: 11%">总部管理费用<br />
                                    (成本费用)</th>
                            </tr>--%>
                            <tr>
                                <th rowspan="2" style="width: 6%">序号</th>
                                <th rowspan="2" style="width: 11%">门店</th>
                                <th rowspan="2" style="width: 11%">年度指标</th>
                                <th colspan="12">累计数</th>
                            </tr>
                            <tr>

                                <th style="width: 6%">1-1月</th>
                                <th style="width: 6%">1-2月</th>
                                <th style="width: 6%">1-3月</th>
                                <th style="width: 6%">1-4月</th>
                                <th style="width: 6%">1-5月</th>
                                <th style="width: 6%">1-6月</th>
                                <th style="width: 6%">1-7月</th>
                                <th style="width: 6%">1-8月</th>
                                <th style="width: 6%">1-9月</th>
                                <th style="width: 6%">1-10月</th>
                                <th style="width: 6%">1-11月</th>
                                <th style="width: 6%">1-12月</th>
                            </tr>
                            <tr>

                                <th class="th_Sub" colspan="2" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break: break-all">合计</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">299,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">43,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">64,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">86,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">108,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">127,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">151,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">169,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">191,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">213,000</th>

                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">240,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">273,000</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">299,000</th>


                            </tr>

                        </thead>
                        <tbody>
                            <tr>
                                <td class="Td_Right">1</td>
                                <td class="Td_Right">门店甲</td>
                                <td class="Td_Right">120,000</td>
                                <td class="Td_Right">10,000</td>
                                <td class="Td_Right">20,000</td>
                                <td class="Td_Right">30,000</td>
                                <td class="Td_Right">40,000</td>
                                <td class="Td_Right">50,000</td>
                                <td class="Td_Right">60,000</td>
                                <td class="Td_Right">70,000</td>
                                <td class="Td_Right">80,000</td>
                                <td class="Td_Right">90,000</td>
                                <td class="Td_Right">100,000</td>
                                <td class="Td_Right">110,000</td>
                                <td class="Td_Right">120,000</td>
                            </tr>
                            <tr>
                                <td class="Td_Right">2</td>
                                <td class="Td_Right">门店乙</td>
                                <td class="Td_Right">65,000</td>
                                <td class="Td_Right">10,000</td>
                                <td class="Td_Right">15,000</td>
                                <td class="Td_Right">20,000</td>
                                <td class="Td_Right">25,000</td>
                                <td class="Td_Right">30,000</td>
                                <td class="Td_Right">35,000</td>
                                <td class="Td_Right">40,000</td>
                                <td class="Td_Right">45,000</td>
                                <td class="Td_Right">50,000</td>
                                <td class="Td_Right">55,000</td>
                                <td class="Td_Right">60,000</td>
                                <td class="Td_Right">65,000</td>
                            </tr>
                            <tr>
                                <td class="Td_Right">3</td>
                                <td class="Td_Right">门店丙</td>
                                <td class="Td_Right">29,000</td>
                                <td class="Td_Right">7,000</td>
                                <td class="Td_Right">9,000</td>
                                <td class="Td_Right">11,000</td>
                                <td class="Td_Right">13,000</td>
                                <td class="Td_Right">15,000</td>
                                <td class="Td_Right">17,000</td>
                                <td class="Td_Right">19,000</td>
                                <td class="Td_Right">21,000</td>
                                <td class="Td_Right">23,000</td>
                                <td class="Td_Right">25,000</td>
                                <td class="Td_Right">27,000</td>
                                <td class="Td_Right">29,000</td>
                            </tr>
                            <tr>
                                <td class="Td_Right">4</td>
                                <td class="Td_Right">门店丁</td>
                                <td class="Td_Right">85,000</td>
                                <td class="Td_Right">16,000</td>
                                <td class="Td_Right">20,000</td>
                                <td class="Td_Right">25,000</td>
                                <td class="Td_Right">30,000</td>
                                <td class="Td_Right">32,000</td>
                                <td class="Td_Right">39,000</td>
                                <td class="Td_Right">40,000</td>
                                <td class="Td_Right">45,000</td>
                                <td class="Td_Right">50,000</td>
                                <td class="Td_Right">60,000</td>
                                <td class="Td_Right">76,000</td>
                                <td class="Td_Right">85,000</td>
                            </tr>
                        </tbody>

                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>