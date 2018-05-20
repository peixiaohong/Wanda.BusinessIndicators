<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TargetReportUserControl.ascx.cs" Inherits="LJTH.BusinessIndicators.Web.UserControl.TargetReportUserControl" %>

<html>
<head>
    <title>TargetReportUserControl</title>
    <script src="../Scripts/BusinessReport/UserControl.js"></script>
    <link href="../Styles/css/NavStlye.css" type="text/css" rel="stylesheet" />

      <style type="text/css">
        .Tishi {
            color: #9d2328;
            font-weight: 700;
        }
    </style>
</head>
<body>
    <!--导航条 开始-->
    <div class="fnav">
        <div class="flow_nav">
            <ul>
                <li class="qi1" id="downLoadTemplate" onclick="ClickItems('downLoadTemplate')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="downLoadTemplateTitle" >1</span>
                            <span id="downLoadTemplateSpan" class="txtdisable">模版下载</span>
                        </div>
                    </div>
                </li>
                <li class="qi2" id="dataUpload" onclick="ClickItems('dataUpload')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="dataUploadTitle">2</span>
                            <span id="dataUploadSpan" class="txtdisable">数据导入</span>
                        </div>
                    </div>
                </li>
                <li class="qi3" id="missTargetReport" onclick="ClickItems('missTargetReport')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="missTargetReportTitle">3</span>
                            <span id="missTargetReportSpan" class="txtdisable">累计未完成</span>
                        </div>
                    </div>
                </li>

                  <li class="qi3_1" id="missCurrentTargetReport" onclick="ClickItems('missCurrentTargetReport')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="missCurrentTargetReportTitle">4</span>
                            <span id="missCurrentTargetReportSpan" class="txtdisable">当月未完成</span>
                        </div>
                    </div>
                </li>

                <li class="qi4" id="monthReport" onclick="ClickItems('monthReport')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="monthReportTitle">5</span>
                            <span id="monthReportSpan" class="txtdisable">月报说明</span>
                        </div>
                    </div>
                </li>

                <li class="qi6" id="monthReportReady" onclick="ClickItems('monthReportReady')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="monthReportReady">6</span>
                            <span id="monthReportReadySpan" class="txtdisable">月报保存</span>
                        </div>
                    </div>
                </li>

                <li class="qi5" id="monthReportSubmit" onclick="ClickItems('monthReportSubmit')">
                    <div class="btn_l">
                        <div class="btn_r">
                            <span class="postion_num" id="monthReportSubmitTitle">7</span>
                            <span id="monthReportSubmitSpan" class="txtdisable">月报提交</span>
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
    <asp:HiddenField ID="ItemStateHidden" runat="server" />
    <!--导航条 结束-->
</body>
</html>

