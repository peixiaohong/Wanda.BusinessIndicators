<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/EmptyMaster.Master" AutoEventWireup="true" CodeBehind="TargetApprove.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.TargetApprove" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetApprove.js?ver=2"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="form_name mb8 form_name_build" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">

            <span style="float: right; font-size: 13px; font-weight: normal; padding-right: 10px;"></span>
            <asp:Label ID="lblName" Style="font-size: 18px; text-indent: 3px; height: 26px; line-height: 26px; border: none; width: 500px;"
                runat="server" Text="月度经营报告"></asp:Label>
        </div>
        <asp:HiddenField runat="server" ID="hideMonthReportID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideBusinessID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideFinMonth" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideProType" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="HideOpinions" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideProcessCode" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideFinYear" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideSystemID" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hideBatchID" ClientIDMode="Static" />

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
                    <li class="selected m_1"><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>月度经营报告</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>完成情况明细</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>当月未完成</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>累计未完成</span></a></li>
                    <li><a class="active_sub2" onclick="ChangeTargetDetail(this,'Tab');"><span>补回情况明细</span></a></li>
                </ul>

                <div class="DownExcelDiv">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport(this);">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出月度经营报告</label></span>
                        </div>
                    </span>
                </div>
            </div>
            <!--tabs-->




            <div class="tabs_content">
                <!--内容-->

                <%-- <div id="panel" runat="server" class="panelSearch">
                  </div>
                <!--收缩按钮 开始-->
                <div style="margin-left: 45%; margin-bottom: 0px">
                    <img id="imgtableUpDown" style="display: none" onclick="UpDownTableClick()" src="../Images/images1/Down.png" alt="" />
                </div>
                <!--收缩按钮 结束-->--%>
                <!--明细项 结束-->

                <!--月度报告 开始-->
                <div id="T1" style="margin-top: 5px;">
                    <div class="scrolldoorFrame copy">
                        <table id="MonthReportSummaryTable" class="tab_005">
                            <thead id="MonthReportSummaryHead" style="width: 100%">
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
                        <table class="tab_005" id="importedDataFloatTable2">
                            <thead>
                            </thead>
                        </table>
                        <table class="tab_005" id="importedDataTable2">
                            <thead id="CompleteDetailHead" style="">
                            </thead>
                            <tbody id="tab2_rows">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--完成情况明细 end-->


                   <!--未完成说明（当月） 开始-->
                <div class="TClassHide" id="T3_1">
                    <ul class="tabs" id="U2_1" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>
                    <table class="tab_005" id="Tab_CurrentMissFloatTarget">
                        <thead>
                        </thead>

                    </table>
                    <table class="tab_005" id="Tab_CurrentMissTarget">
                        <thead id="Tab_CurrentMissTargetHead">
                        </thead>
                        <tbody id="Tbody_CurrentMissTargetData">
                        </tbody>

                    </table>

                </div>
                <!--未完成说明 结束-->


                <!--未完成说明 开始-->
                <div id="T3" class="TClassHide">

                    <ul class="tabs" id="U2" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>
                    <table class="tab_005" id="Tab_MissFloatTarget">
                        <thead>
                           
                           
                        </thead>

                    </table>
                    <table class="tab_005" id="Tab_MissTarget">
                        <thead  id="Tab_MissTargetHead" >
                           
                           
                        </thead>
                        <tbody id="Tbody_MissTargetData">
                        </tbody>

                    </table>
                </div>
                <!--未完成说明 结束-->


                <!--补回情况 开始-->
                <div id="T4" class="TClassHide">
                    <ul class="tabs" id="U1" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                    </ul>
                    <table class="tab_005" id="Tab_FloatReturn">
                        <thead>
                         
                        </thead>
                    </table>
                    <table class="tab_005" id="Tab_Return">
                        <thead id ="Tab_ReturnHead">
                         
                        </thead>
                        <tbody id="Tbody_Data" class="tab_001">
                        </tbody>

                    </table>
                </div>
                <!--补回情况 结束-->

                <div id="ApproveAttachDiv" style="padding-bottom: 10px; padding-top: 10px;">
                    <div style="border: 1px solid #ccc; min-height: 50px; padding-top: 5px;padding-bottom:5px;background:#fff;">
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
