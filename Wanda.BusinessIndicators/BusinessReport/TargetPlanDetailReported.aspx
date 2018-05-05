<%@ Page Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="TargetPlanDetailReported.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.TargetUpdate" %>

<%@ Register Src="~/SiteMasterPage/wfCtrl.ascx" TagPrefix="uc1" TagName="wfCtrl" %>
<%@ Register Src="~/SiteMasterPage/userSelectCtrl.ascx" TagPrefix="uc1" TagName="userSelectCtrl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/css/NavStlye.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/BusinessReport/TargetPlanDetailReported.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.base64.js"></script>

    <style type="text/css">
        .Tishi {
            color: red;
            font-weight: 700;
        }
    </style>
    <uc1:wfCtrl runat="server" ID="wfCtrl" />
    <uc1:userSelectCtrl runat="server" ID="userSelectCtrl" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!--查询条件-->
        <div class="">
            <div class="form_name mb8 form_name_build" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">
                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="ddlSystem_TextChanged" runat="server" Style="width: 150px; height: 25px;"></asp:DropDownList>
                <asp:Label ID="lblName" Style="font-size: 18px; text-indent: 3px; height: 26px; line-height: 26px; border: none; width: 200px;" runat="server" Text="年计划指标上报 "></asp:Label>
                <asp:HiddenField runat="server" ID="hideTargetPlanID" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="hideVersionName" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="HidSystemID" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="HidSystemText" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="HideProcessCode" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="HideFinYear" ClientIDMode="Static" />
            </div>
        </div>
        <!--查询条件 结束-->


        <!--导航条 开始-->
        <div class="fnav">
            <div class="flow_nav">
                <ul>
                    <li class="qi1" id="monthReportReady" onclick="ClickItems('monthReportReady')">
                        <div class="btn_l">
                            <div class="btn_r">
                                <span class="postion_num" id="monthReportReadyTitle">1</span>
                                <span id="monthReportReadySpan" class="txtdisable">版本类型</span>
                            </div>
                        </div>
                    </li>
                    <li class="qi2" id="downLoadTemplate" onclick="ClickItems('downLoadTemplate')">
                        <div class="btn_l">
                            <div class="btn_r">
                                <span class="postion_num" id="downLoadTemplateTitle">2</span>
                                <span id="downLoadTemplateSpan" class="txtdisable">模版下载</span>
                            </div>
                        </div>
                    </li>
                    <li class="qi3" id="dataUpload" onclick="ClickItems('dataUpload')">
                        <div class="btn_l">
                            <div class="btn_r">
                                <span class="postion_num" id="dataUploadTitle">3</span>
                                <span id="dataUploadSpan" class="txtdisable">数据导入</span>
                            </div>
                        </div>
                    </li>



                    <li class="qi4" id="monthReportSubmit" onclick="ClickItems('monthReportSubmit')">
                        <div class="btn_l">
                            <div class="btn_r">
                                <span class="postion_num" id="monthReportSubmitTitle">4</span>
                                <span id="monthReportSubmitSpan" class="txtdisable">指标提交</span>
                            </div>
                        </div>
                    </li>
                </ul>
            </div>
            <!--flow_nav-->
            <div class="" style="clear: both"></div>
            <div class="tip">
                <div class="arrow_n"></div>
                <div class="tip_box">
                    <div class="tip_t_l">
                        <div class="tip_t_r"></div>
                    </div>
                    <!--tip_t_l-->
                    <div class="tip_m">
                        <span class="text_bold">提示信息：</span>
                        <span id="PromptMessage">请下载填报模版。</span>
                    </div>
                    <!--tip_m-->
                    <div class="tip_b_l">
                        <div class="tip_b_r"></div>
                    </div>
                    <!--tip_b_l-->
                </div>
                <!--tip_box-->
            </div>
            <!--tip-->
        </div>
        <!--导航条 结束-->





        <div id="weiwancheng1" style="margin-top: 10px;">
            <!--填写版本类型  开始-->
            <div id="VersionName" class="">
                <span class="">
                    <span style="font-weight: bold; color: #cb5c61;">版本类型：</span>
                    <span>
                        <input id="txt_VersionName" type="text" value="" style="width: 180px; margin-right: 15px; height: 25px;"/>
                    </span>
                </span>
            </div>
            <!--填写版本类型  结束-->
            <!--下载填报模版  开始-->
            <div id="DownLoadModel" class="TClassHide">
                <span style="font-size: 13px; height: 30px; line-height: 60px; display: block; top: -35px; right: 0; z-index: 100; padding-right: 0px;">
                    <div class="uploadify-button " id="Div4" style="height: 25px; line-height: 25px; text-indent: 0px; width: 150px; cursor: pointer;">
                        <span class="uploadify-button-text">
                            <a id="A4" style="text-decoration: none;" href="javascript:DownLoadTargetPlanExcel(this)">请点此下载填报模版</a></span>
                    </div>
                </span>
            </div>
            <!--下载填报模版  结束-->
            <!--上传数据 Start-->
            <div id="UpLoadData" class="TClassHide">
                <span class="UpLoadExcelspan">
                    <span>
                        <input type="file" name="upload" id="file1" />
                    </span>
                </span>
            </div>
            <!--上传数据 End-->
        </div>



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
                    <div class="uploadify-button W100P" id="file_upload-button1" onclick="DownLoadTargetPlanExcel(this);">
                        <span>
                            <label class="DownExcelLabel" id="BtnDownload">导出数据</label></span>
                    </div>
                </span>
            </div>
        </div>
        <!--下载按钮 结束-->

        <!--完成情况明细 start-->
        <div class="TClassHide" id="T2">
            <div class="scrolldoorFrame copy">
                <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                </ul>
                <table class="tab_005" id="importedDataTable2">
                    <thead id="TargetPlanDetailHead">
                    </thead>
                    <tbody id="rows">
                    </tbody>
                </table>
            </div>
        </div>
        <!--完成情况明细 end-->

        <div id="process">
        </div>

        <%--<!--上报审批 开始-->
        <div id="wanda_wf_nav_opin_content">
        </div>
        <!--上报审批 结束-->
        <input type="hidden" id="wanda_wf_hd_process" value="" />
        <input type="hidden" id="wanda_wf_opInfo" value="" />--%>
    </div>

</asp:Content>
