<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetCollectDisplay_B.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master"Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.TargetCollectDisplay_B" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetCollect_B.js"></script>

    <script type="text/javascript" src="../Scripts/jquery.base64.js"></script>

    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.all-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.min.js"></script>
    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />

       <script type="text/javascript">
           var TreeDataJson = <%=TreeDataJson%>;
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
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
                        <th>查询系统</th>
                        <td>
                              <input type="text" id="TxtSystem" style="width: 210px;" onclick="showMenu();" />
                            <asp:DropDownList ID="ddlSystem" runat="server" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged" AutoPostBack="True"  ClientIDMode="Static" Style="width: 120px;display:none;">
                            </asp:DropDownList>
                        </td>
                        <th>查询年份</th>
                        <td>
                            <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                        </td>

                        <th>
                            <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="GetSumList()"><span>查询</span></a>
                        </th>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="content_b_l">
            <div class="content_b_r"></div>
        </div>
        <div class="margin_t10">
            <div class="tabs_m">

                <table style="width: 99%">
                    <tbody>
                        <tr>
                            <td>
                                <ul class="tabs" id="tabs" style="MARGIN-BOTTOM: 0px; HEIGHT: auto; BORDER-BOTTOM-COLOR: #fff; MARGIN-TOP: 5px">
                                    <li class="sd" style="DISPLAY: list-item">
                                        <a class="active3 active_sub3" onclick="Change('sum','')" id="tabsum">汇总</a>
                                    </li>
                                  
                                </ul>
                            </td>
                            <td>

                                <div class="uploadify-button" id="file_upload-button1" onclick="DownExcel()" style="width: 90px; float: right">
                                    <span>
                                        <label class="DownExcelLabel" id="LabelDownload">导出数据</label></span>
                                </div>

                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="tabs_content">
            <div class="scrolldoorFrame copy">
                <table class="tab_005" id="SumTable" style="display: none">
                    <thead id="Head" style="width:100%">
                        <%-- 因为空的tr和rowspan会自动消除 因此备份一份--%>
                        <%--  <tr>  
                            <th style="width: 12%" rowspan="2">月份</th>
                            <th id="Dsum">当前数</th>
                            <th id="Asum">累计数</th>
                        </tr>
                        <tr id="TrTarget">
                        </tr>--%>
                        <tr>
                            <th style="width: 12%" rowspan="2">月份</th>
                            <th id="Dsum">当前数</th>
                            <th id="Asum">累计数</th>
                        </tr>
                        <tr id="TrTarget">
                            <%--<td></td>--%>
                        </tr>
                    </thead>

                    <tbody id="rows">
                    </tbody>
                </table>
                <table class="tab_005" id="TargetTable" style="display: none">
                    <thead id="Thead1" style="width:100%">

                        <tr>
                            <th rowspan="2" style="width: 6%">序号</th>
                            <th rowspan="2" style="width: 11%">门店</th>
                            <th rowspan="2" style="width: 11%">年度指标</th>
                            <th colspan="12" class="sum" style="cursor: pointer" onclick="ClickChange('sum')">累计数  &nbsp;  &nbsp;<img src="../Images/btn02.png" /></th>
                            <th colspan="12" style="display: none; cursor: pointer" class="dan" onclick="ClickChange('dan')">当月数  &nbsp;  &nbsp;<img src="../Images/btn02.png" /></th>
                        </tr>
                        <tr>
                            <th class="sum">1-1月</th>
                            <th class="sum">1-2月</th>
                            <th class="sum">1-3月</th>
                            <th class="sum">1-4月</th>
                            <th class="sum">1-5月</th>
                            <th class="sum">1-6月</th>
                            <th class="sum">1-7月</th>
                            <th class="sum">1-8月</th>
                            <th class="sum">1-9月</th>
                            <th class="sum">1-10月</th>
                            <th class="sum">1-11月</th>
                            <th class="sum">1-12月</th>
                            <%--当月--%>
                            <th style="display: none" class="dan">1月</th>
                            <th style="display: none" class="dan">2月</th>
                            <th style="display: none" class="dan">3月</th>
                            <th style="display: none" class="dan">4月</th>
                            <th style="display: none" class="dan">5月</th>
                            <th style="display: none" class="dan">6月</th>
                            <th style="display: none" class="dan">7月</th>
                            <th style="display: none" class="dan">8月</th>
                            <th style="display: none" class="dan">9月</th>
                            <th style="display: none" class="dan">10月</th>
                            <th style="display: none" class="dan">11月</th>
                            <th style="display: none" class="dan">12月</th>
                        </tr>
                    </thead>
                    <tbody id="SumTrTargetTable">
                    </tbody>
                    <tfoot id="TrTargetTable"></tfoot>

                </table>
                <table class="tab_005" id="HistoryTable" style="display: none">
                    <thead id="Thead2">
                        <tr>
                            <th style="width: 30%">上报人</th>
                            <th style="width: 30%">上报日期</th>
                            <th style="width: 10%">文档下载</th>
                            <th style="width: 10%">指标状态</th>
                            <th style="width: 10%">操作</th>
                            <th style="width: 10%">查看日志</th>
                        </tr>
                    </thead>
                    <tbody id="HistoryTr">
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">

        <textarea id="Remark" cols="55" rows="5"></textarea>


        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" AddRemark();" id="savetrue" />
        </div>
    </div>
   
     <div id="menuContent" class="menuContent" style="display: none; position: absolute;">
        <ul id="SysTree" class="ztree_new" style="margin-top: 0; width: 200px; height: 350px; background-color: #fff; border: 1px solid #000; overflow-y: auto;"></ul>
    </div>

</asp:Content>