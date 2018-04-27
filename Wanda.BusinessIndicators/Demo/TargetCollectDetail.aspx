<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetCollectDetail.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master"   Inherits="LJTH.BusinessIndicators.Web.Demo.TargetCollectDetail" %>


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
                            <tr>
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
                            </tr>
                        </thead>
                        <tbody id="rows">
                            <tr>
                                <td class="Td_Center">1月</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">2月</td>
                               <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">200</td>
                                <td class="Td_Right">300</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">40</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">3月</td>
                              <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">300</td>
                                <td class="Td_Right">450</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">60</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">4月</td>
                               <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">400</td>
                                <td class="Td_Right">600</td>
                                <td class="Td_Right">200</td>
                                <td class="Td_Right">80</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">5月</td>
                               <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">500</td>
                                <td class="Td_Right">750</td>
                                <td class="Td_Right">250</td>
                                <td class="Td_Right">100</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">6月</td>
                               <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">600</td>
                                <td class="Td_Right">900</td>
                                <td class="Td_Right">300</td>
                                <td class="Td_Right">120</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">7月</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">700</td>
                                <td class="Td_Right">1050</td>
                                <td class="Td_Right">350</td>
                                <td class="Td_Right">140</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">8月</td>
                                 <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">800</td>
                                <td class="Td_Right">1200</td>
                                <td class="Td_Right">400</td>
                                <td class="Td_Right">160</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">9月</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">900</td>
                                <td class="Td_Right">1350</td>
                                <td class="Td_Right">450</td>
                                <td class="Td_Right">180</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">10月</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">1000</td>
                                <td class="Td_Right">1500</td>
                                <td class="Td_Right">500</td>
                                <td class="Td_Right">200</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">11月</td>
                                <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">1100</td>
                                <td class="Td_Right">1650</td>
                                <td class="Td_Right">550</td>
                                <td class="Td_Right">220</td>
                            </tr>
                            <tr>
                                <td class="Td_Center">12月</td>
                               <td class="Td_Right">100</td>
                                <td class="Td_Right">150</td>
                                <td class="Td_Right">50</td>
                                <td class="Td_Right">20</td>
                                <td class="Td_Right">1200</td>
                                <td class="Td_Right">1800</td>
                                <td class="Td_Right">600</td>
                                <td class="Td_Right">240</td>
                            </tr>
                             <tr>

                                
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">合计</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">1,200</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">1,800</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">600</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">240</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">7,800</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">11,700</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">3,900</th>
                                <th class="th_Sub" style="border-right: 1px solid #cde1fb !important; border-top: 1px solid #cde1fb !important; word-break:break-all">1,560</th>
                                


                            </tr>
                                
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div></div>
</asp:Content>