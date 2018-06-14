<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/EmptyMaster.master" AutoEventWireup="true" CodeBehind="TargetPlanDetailApprove.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.TargetPlanDetailApprove" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetPlanDetailApprove.js?ver=<%=new Random(DateTime.Now.Millisecond).Next(0,10000)%>"></script>

      <script type="text/javascript">
           var SysDataJson = <%=SysDataJson%>;
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="form_name mb8 form_name_build" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">

            <span style="float: right; font-size: 13px; font-weight: normal; padding-right: 10px;"></span>
            <asp:Label ID="lblName" Style="font-size: 18px; text-indent: 3px; height: 26px; line-height: 26px; border: none; width: 500px;"
                runat="server" Text=""></asp:Label>
        </div>
        <asp:HiddenField runat="server" ID="hideTargetPlanID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideVersionName" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideSystemID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideFinYear" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="HideProcessCode" ClientIDMode="Static" />
        <!--明细项 开始-->
        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2"><span>计划指标</span></a></li>
                </ul>

                <div class="DownExcelDiv">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport(this);">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出计划指标</label></span>
                        </div>
                    </span>
                </div>
            </div>
            <!--tabs-->
            <div class="tabs_content">
                <!--内容-->
                <!--完成情况明细 start-->
                <div id="T2">
                    <div class="scrolldoorFrame copy">
                        <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                            <li class="sd" style="display: list-item">
                                <a class="active3 active_sub3" onclick="Change('sum','')" id="tabsum">汇总</a>
                            </li>
                        </ul>
                        <table class="tab_005" id="importedDataFloatTable2">
                            <thead>
                            </thead>
                        </table>
                         <table class="tab_005" id="importedDataTable2">
                            <thead id="TargetPlanDetailHead">
                            </thead>
                            <tbody id="rows_old">
                            </tbody>
                        </table>

                        <table class="tab_005" id="SumTable" style="display: none">
                            <thead id="Head" style="width: 100%">
                                <tr> <th id="Vsum" > <asp:Label ID="lblVersionName2"  runat="server" Text=""></asp:Label></th></tr>
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
                            <thead id="Thead1" style="width: 100%">
                                <tr> <th colspan="16" > <asp:Label ID="lblVersionName1"  runat="server" Text=""></asp:Label></th></tr>
                                <tr>
                                    <th rowspan="2" style="width: 4%">序号</th>
                                    <th rowspan="2" style="width: 11%">项目</th>
                                    <th rowspan="2" style="width: 7%">开业时间</th>
                                    <th rowspan="2" style="width: 7%">年度指标</th>
                                    
                                    <th colspan="12"  class="dan" >当月数</th>
                                </tr>
                                <tr>
                                  
                                    <%--当月--%>
                                    <th class="dan">1月</th>
                                    <th class="dan">2月</th>
                                    <th class="dan">3月</th>
                                    <th class="dan">4月</th>
                                    <th class="dan">5月</th>
                                    <th class="dan">6月</th>
                                    <th class="dan">7月</th>
                                    <th class="dan">8月</th>
                                    <th class="dan">9月</th>
                                    <th class="dan">10月</th>
                                    <th class="dan">11月</th>
                                    <th class="dan">12月</th>
                                </tr>
                            </thead>
                            <tbody id="SumTrTargetTable">
                            </tbody>
                            <tfoot id="TrTargetTable"></tfoot>

                        </table>


                    </div>
                </div>
                <!--完成情况明细 end-->

                <div id="process">
                </div>
                <input type="hidden" id="wanda_wf_hd_process" value="" />
                <input type="hidden" id="wanda_wf_opInfo" value="" />
            </div>
        </div>
    </div>
</asp:Content>
