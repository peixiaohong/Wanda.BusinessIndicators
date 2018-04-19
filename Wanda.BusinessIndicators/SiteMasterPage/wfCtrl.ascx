<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wfCtrl.ascx.cs" Inherits="WebApplication.WebPortal.SiteMaster.wfCtrl" %>

<script src="http://192.168.50.72:81/UserSelectService/js/bpf-sdk-core.js"></script>
<link href="http://192.168.50.72:81/RuntimeService/css/bpf-workflow-<%=HttpContext.Current.Request.Browser.IsMobileDevice?"mobile":"pc" %>.css" rel="stylesheet" />
<script src="http://192.168.50.72:81/RuntimeService/js/bpf-workflow-client.js"></script>
<script src="http://192.168.50.72:81/RuntimeService/js/bpf-workflow-<%=HttpContext.Current.Request.Browser.IsMobileDevice?"mobile":"pc" %>.js"></script>
<script src="http://192.168.50.72:81/RuntimeService/js/bpf-workflow-lang-zh-cn.js"></script>
<script src="<%=ResolveUrl("~/scripts/workflow/js/Workflow.js?v=1") %>"></script>

<%--<link href="<%=ResolveUrl("~/Assets/vendors/wf/css/common.workflow.css") %>" rel="stylesheet" />


<script type="text/html" id="btnArea_pc">
    <div id="wanda-wfclient-button-content" class="wanda-wfclient wanda-wfclient-button-default">
        <table id="wanda-wfclient-button-table">
            <tbody>
                <tr style="height: 40px;">
                    <td>&nbsp;</td>
                    <td>
                        <div id="wanda-wfclient-button-div">
                            <!--按钮区域-->
                            <div class="wanda-wfclient-button-btn wanda-wfclient-button-save">保存</div>
                            <div class="wanda-wfclient-button-btn wanda-wfclient-button-submit">提交</div>
                        </div>
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </tbody>
        </table>
    </div>

</script>
<script type="text/html" id="btnArea_mobile">
    <div class="inset_wrapper">
        <div class="wanda_workflow_approve_result">
            <div class="wanda_workflow_approve_center">
                <a id="wanda_workflow_approve_top" href="#" class="wanda_workflow_approve_top" style="bottom: 50px;"></a>
                <div class="wanda_workflow_approve_container" style="padding-bottom: 50px;">
                    <ul class="wanda_workflow_result_list"></ul>
                </div>
                <div class="wanda_workflow_approve_actions">
                    <span class="wanda_workflow_btn wanda_workflow_modal_sure save" v-on:click="Save()">保存</span>
                    <span class="wanda_workflow_btn wanda_workflow_modal_sure wanda_workflow_modal_cancel submit" v-on:click="Submit()">提交</span>
                </div>
            </div>
        </div>
    </div>
</script>

<script type="text/javascript">

    var WFOperator = {
        setting: {},
        InitSetting: function (args) {
            WFOperator.setting.Container = args.Container;
            WFOperator.setting.OnSaveApplicationData = args.OnSaveApplicationData;
            WFOperator.setting.OnAfterExecute = args.OnAfterExecute;
            WFOperator.setting.OnExecuteCheck = args.OnExecuteCheck;
        },
        LoadProcess: function (args, callback) {
            var func = function () {
                WFOperator.setting.OnAfterExecute();
            }
            WFOperator.setting.FlowCode = args.FlowCode;
            if (utils.mobileBrower()) {

                var workflow_model = new Vue({
                    el: WFOperator.setting.Container,
                    template: "#btnArea_mobile",
                    methods: {
                        Save: function () {
                            WFOperator.setting.OnExecuteCheck(function (result) {
                                if (result) {
                                    WFOperator.setting.OnSaveApplicationData.Submit(null, function () {
                                        WFOperator.setting.OnAfterExecute({ ActionType: 1 });
                                    });
                                }
                            });
                        }, Submit: function () {
                            WFOperator.setting.OnExecuteCheck(function (result) {
                                if (result) {
                                    WFOperator.setting.OnSaveApplicationData.Submit(null, function () {
                                        WFOperator.setting.OnAfterExecute({ ActionType: 2 });
                                    });
                                }
                            });
                        }
                    }
                });

            } else {
                var $html = $($("#btnArea_pc").html());
                $(WFOperator.setting.Container).append($html);

                $(WFOperator.setting.Container).find(" #wanda-wfclient-button-content #wanda-wfclient-button-div .wanda-wfclient-button-save").click(function () {
                    WFOperator.setting.OnExecuteCheck(function (result) {
                        if (result) {
                            WFOperator.setting.OnSaveApplicationData.Save(null, function () {
                                WFOperator.setting.OnAfterExecute({ ActionType: 1 });
                            });
                        }
                    });
                });
                $(WFOperator.setting.Container).find("#wanda-wfclient-button-content #wanda-wfclient-button-div .wanda-wfclient-button-submit").click(function () {
                    WFOperator.setting.OnExecuteCheck(function (result) {
                        if (result) {
                            WFOperator.setting.OnSaveApplicationData.Submit(null, function () {
                                WFOperator.setting.OnAfterExecute({ ActionType: 2 });
                            });
                        }
                    });
                });
            }
            callback();
        }
    };
</script>--%>

