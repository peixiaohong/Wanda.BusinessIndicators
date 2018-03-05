<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="TargetDirectlyReported.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.BusinessReport.TargetDirectlyReported" %>
<%@ Register Src="~/UserControl/MonthReportedAction.ascx" TagPrefix="MonthLyReportActionUC" TagName="MonthReportedAction" %>
<%@ Register Src="~/UserControl/TargetReportUserControl.ascx" TagPrefix="targetReportUC" TagName="TargetReportUserControl" %>
<%@ Register Src="~/UserControl/MutipleUpload.ascx" TagPrefix="targetReportUC" TagName="MutipleUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../ProcessResource/css/wfStyle-201.88.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetDirectlyReported.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetReported1.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
    <script type="text/javascript" src="../ProcessResource/js/wanda-wf-client-201.88.js"></script>
 <link href="http://zbgk.wf.wanda-dev.cn/RuntimeService/css/wanda-wf-client.css?version=1" type="text/css" rel="Stylesheet" />
    <script src="http://zbgk.wf.wanda-dev.cn/RuntimeService/js/wanda-wf-client.js?version=1" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="form_name mb8 form_name_build" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">
            <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="ddlSystem_TextChanged" runat="server" Style="width: 150px; height:25px;"></asp:DropDownList>
            <asp:Label ID="lblName" Style="font-size:18px; text-indent:3px; height:26px; line-height:26px; border:none; width:200px;" runat="server" Text="月度经营报告上报 "></asp:Label>
            <asp:HiddenField runat="server" ID="hideMonthReportID" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="HidSystemText" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="hideFinYear" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="hideFinMonth" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="HideProcessCode" ClientIDMode="Static" /> <br />
              <span style="color:red">注：上传excel后，若页面没有出现相应数据，请稍等几分钟再次刷新页面（数据正在后台进行计算），即可恢复正常。请勿因此重复上传数据。</span>
        </div>
        <!--导航栏 开始-->
        <targetReportUC:TargetReportUserControl runat="server" ID="UserControl" />
        <!--导航栏 结束-->
        <!--下载填报模版  开始-->
        <div id="weiwancheng1" style="margin-top: 10px;">
            <div id="DownLoadModel" class="">
                <span style="font-size: 13px; height: 30px; line-height: 60px; display: block; top: -35px; right: 0; z-index: 100; padding-right: 0px;">
                    <div class="uploadify-button " id="Div4" style="HEIGHT: 25px; LINE-HEIGHT: 25px; text-indent: 0px; WIDTH: 120px; cursor: pointer;">
                        <span class="uploadify-button-text">
                            <a id="A4" style="text-decoration: none;" href="javascript:DownLoadTargetPlanExcel(this)">请点此下载填报模版</a></span>
                    </div>
                </span>
            </div>
            <!--上传数据 Start-->
            <div id="UpLoadData" class="TClassHide">
                <span class="UpLoadExcelspan">
                    <span>
                        <input type="file" name="upload" id="file1" />
                    </span>
                </span>
            </div>
        </div>
        <!--上传数据 End-->
        <!--下载填报模版  结束-->


        <!--下载按钮 开始-->
        <div id="Down1" class="TClassHide">
            <div class="DownExcelDiv2 ">
                <span class="DownExcelspan">
                    <span>
                        <input type="file" name="file_upload" id="file_upload" />
                    </span>
                </span>
            </div>
            <div class="DownExcelDiv">
                <span class="DownExcelspan">
                    <div class="uploadify-button W100P" id="file_upload-button1" onclick="DownExcel(this);">
                        <span>
                            <label class="DownExcelLabel" id="BtnDownload">导出数据</label></span>
                    </div>
                </span>
            </div>
        </div>


        <!--下载按钮 结束-->

       <!--下载和上报按钮 开始-->
        <div id="UpLoadDataDiv" class="TClassHide">
            <div class="DownExcelDiv2 ">
                <span class="DownExcelspan">
                    <span>
                        <input type="file" name="file_upload" id="file2" />
                    </span>
                </span>
            </div>
            <div class="DownExcelDiv">
                <span class="DownExcelspan">
                    <div class="uploadify-button W100P" id="Div2" onclick="DownExcelMonthReport();">
                        <span>
                            <label class="DownExcelLabel" id="Label1">导出数据</label></span>
                    </div>
                </span>
            </div>
        </div>
        <!--下载和上报按钮 结束-->

        <!--完成情况明细 start-->
        <div class="TClassHide" id="T2">
            <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: 20px; margin-bottom: 0px;">
            </ul>
            <table class="tab_005" id="importedDataTable2">
                <thead id="CompleteDetailHead">
                </thead>
                <tbody id="tab2_rows">
                </tbody>
            </table>
        </div>
        <!--完成情况明细 end-->

        <!--未完成说明 累计 开始-->
        <div class="TClassHide" id="T3">
            <ul class="tabs" id="U2" style="border-bottom-color: #FFF; margin-top: 5px; height: 20px; margin-bottom: 0px;">
            </ul>

            <table class="tab_005" id="Tab_MissTarget">
                <thead  id="Tab_MissTargetHead">
                </thead>
                <tbody id="Tbody_MissTargetData">
                </tbody>
            </table>

        </div>
        <!--未完成说明 结束-->

            <!--未完成说明（当月） 开始-->
        <div class="TClassHide" id="T3_1">
            <ul class="tabs" id="U2_1" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
            </ul>

            <table class="tab_005" id="Tab_CurrentMissTarget">
                <thead  id="Tab_CurrentMissTargetHead">
                    
                </thead>
                <tbody id="Tbody_CurrentMissTargetData">
                </tbody>

            </table>

        </div>
        <!--未完成说明 结束-->



        <!--月报说明 开始-->
        
        <div class="TClassHide" id="T4" style="padding: 10px 0px 10px 0px;">

            <div class="content_title1_Left" style="padding-bottom: 5px; margin-bottom: 10px; border-bottom-color: #014696; border-bottom-style: solid;">
                <span class="blue_txt">月报说明
                </span>
            </div>


            <div style="width: 100%; position: relative; padding-bottom: 5px;"></div>
            <div style="padding: 0px;">
                <textarea id="MonthGetDescription" rows="4" cols="5">
                  </textarea>
            </div>

        </div>
        <!--月报说明 结束-->

        <!--附件上传 开始-->

        <div id="RptAttachments" class="TClassHide">
              <div class="content_title1_Left" style="padding-bottom: 10px; margin-bottom: 10px; border-bottom-color: #014696; border-bottom-style: solid;">
                <span class="blue_txt">附件上传
                </span>
            </div>
            <targetReportUC:MutipleUpload runat="server" ID="MutipleUpload" AttachmentType="月报上传" />
        </div>

        <!--附件上传 结束-->

        <!--日志文件-->
        <div id="divMonthLyReportAction" class="TClassHide">
            <MonthLyReportActionUC:MonthReportedAction runat="server" />
        </div>
        <!--上报审批 开始-->
         <div id="process">
            </div>

        <!--上报审批 结束-->


        <input type="text" class="TClassHide" id="HidSystemID" runat="server" />
    </div>
    <!--编辑弹出层  开始-->

   <div id="divMonthReportDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <div class="pop_content" id="MonthReportDetailContent_Edit">

        </div>
        <div id="ssss" class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'MonthReportDetailContent_Edit' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" SaveMonthReportDetail();" />
        </div>
        
    </div>


     
    <!--编辑弹出层  结束-->

    <input type="hidden" id="wanda_wf_hd_process" value="" />
    <input type="hidden" id="wanda_wf_opInfo" value="" />
</asp:Content>

