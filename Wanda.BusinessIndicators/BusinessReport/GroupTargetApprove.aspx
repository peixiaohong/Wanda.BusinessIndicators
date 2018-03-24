<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/EmptyMaster.Master" AutoEventWireup="true" CodeBehind="GroupTargetApprove.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.GroupTargetApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/GroupTargetApprove.js"></script>
 <link href="http://zbgk.wf.wanda-dev.cn/RuntimeService/css/wanda-wf-client.css?version=1" type="text/css" rel="Stylesheet" />
    <script src="http://zbgk.wf.wanda-dev.cn/RuntimeService/js/wanda-wf-client.js?version=1" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="form_name mb8 form_name_build" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">

            <span style="float: right; font-size: 13px; font-weight: normal; padding-right: 10px;"></span>
            <asp:Label ID="lblName" Style="font-size: 18px; text-indent: 3px; height: 26px; line-height: 26px; border: none; width: 500px;"
                runat="server" Text="月度经营报告"></asp:Label>
        </div>
        <asp:HiddenField runat="server" ID="hideMonthReportID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideSystemID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideFinYear" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideFinMonth" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="HideProcessCode" ClientIDMode="Static" />
        <!--月报说明 开始-->
        <div class="margin_t10" id="MonthReportExplainDiv">
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div class="content_m">
                <div class="blue_txt">月报说明</div>
                <div style="width: 100%; position: relative; padding-bottom: 3px;"></div>
                <div id="txtDes">
                </div>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
        </div>
        <!--月报说明 结束-->

        <!--明细项 开始-->
<div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>月度报告</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>明细</span></a></li>
                </ul>

                <div class="DownExcelDiv">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReportList(this);">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出全部数据</label></span>
                        </div>
                    </span>
                </div>
            </div>
            <!--tabs-->


           

            <div class="tabs_content">
                <!--内容-->

                 <div id="SearchPanel" class="panelSearch">
                  </div>
                <!--收缩按钮 开始-->
                <div style="margin-left: 45%; margin-bottom: 0px">
                    <img id="imgtableUpDown" style="display: none" onclick="UpDownTableClick()" src="../Images/images1/Down.png" alt="" />
                </div>
                <!--收缩按钮 结束-->
                <!--明细项 结束-->

                <!--月度报告 开始-->
                <div id="T1" style="margin-top: 5px;">
                    <div class="scrolldoorFrame copy">
                        <table id="MonthReportSummaryTable" class="tab_005">
                            <thead id="MonthReportSummaryHead" style="width: 100%">
                                   <tr class='tab_5_row_alt'>
                                       <th rowspan='2' style='width: 4%'>序号</th>
                                       <th rowspan='2'>项目</th>
                                       <th colspan='3' style='width: 30%'>本月发生(万元)</th>
                                       <th colspan='3' style='width: 30%'>本年累计(万元)</th>
                                       <th colspan='2' style='width: 20%'>全年(万元)</th>
                                   </tr>
                                   <tr>
                                       <th class='th_Sub'>计划</th>
                                       <th class='th_Sub'>实际</th>
                                       <th class='th_Sub2'>完成率</th>
                                       <th class='th_Sub1'>计划</th>
                                       <th class='th_Sub'>实际</th>
                                       <th class='th_Sub2'>完成率</th>
                                       <th class='th_Sub1'>计划</th>
                                       <th class='th_Sub'>完成率</th>
                                   </tr>
                            </thead>
                            <tbody id="rows">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--月度报告 结束-->

                <!--完成情况明细 start-->
                <div class="TClassHide" id="T2">
                    <div class="scrolldoorFrame copy">
                        <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                        </ul>
                        <table class="tab_005" id="importedDataTable2" >
                            <thead id="CompleteDetailHead" style="width: 100%">
                               
                            </thead>
                            <tbody id="tab2_rows">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--完成情况明细 end-->
                <div id="ApproveAttachDiv" style="padding-bottom: 10px; padding-top: 10px;">
                    <div style="border: 1px solid #ccc; min-height: 50px; padding-top: 5px;padding-bottom:5px">
                        <div class="content_title1_Left" style="width: 100%; padding-bottom: 10px;">
                            <div>
                                <label style="padding-left: 10px; color: #012b80; font-weight: bold">
                                    附&nbsp;&nbsp;&nbsp;&nbsp;件
                                </label>
                            </div>
                        </div>
                        <div id="listAttDiv" class="AttDiv" style="padding-left: 10px">
                            <%--<a href="javascript:return vodi(0);">商管公司.xls(122 KB)</a>--%>
                        </div>

                    </div>
                </div>
                <!--附件 结束-->
               <div id="process">
            </div>
                <input type="hidden" id="wanda_wf_hd_process" value="" />
                <input type="hidden" id="wanda_wf_opInfo" value="" />
            </div>
        </div>
    </div>

</asp:Content>