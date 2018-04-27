$(function () {
    var businessId = utils.getQueryString("businessID");
    Task.Init(businessId);
});
var model = null;
var Task = {
    Init: function (businessId) {
        Task.LoadData(businessId, Task.BuildData);
    },
    BuildData: function (data) {
        model = new Vue({
            el: '#ApproveContent',
            data: data,
            created: function () {
                var self = this;
                if (!self.IsNeedApprove) {
                    //if (Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting != null) {
                    //    //Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.ActionButtonList=false;
                    //    Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.AllowNewCC = false;
                    //    Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.ShowApprovalLog = false;
                    //    Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.ShowApprovalTextArea = false;
                    //    Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.ShowButtonBar = false;
                    //    Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.ShowCCBar = false;
                    //    Wanda_Workflow_Model.workflowContext.CurrentUserSceneSetting.ShowNavigationBar = false;
                    //    Wanda_Workflow_Model.approveText = "无需审批"
                    //}
                    $('.bpf_workflow_result_list').hide();
                    var $container = $("#SJSJ_LC").closest(".approval-process");
                    $container.find("#bpf-wfclient-log-textarea").val("无需审批");
                    var $wfButtons = $container.find("#bpf-wfclient-button-content");
                    var $wfButtonClone = $wfButtons.clone();
                    $wfButtonClone.appendTo("body");
                    $container.hide();
                    $wfButtonClone.find(".bpf-wfclient-button-btn").click(function () {
                        var op = $(this).attr("optype");
                        $("[optype='" + op + "']", $("#bpf-wfclient-button-content")).click();
                    });
                }

            },
            mounted: function () {
            },
            methods: {
            }
        });


    },
    BeforeAction: function (args, func, data) {
        WFParam_SJSJ.DyRoleList = [data.ReceiveLoginName];
        WFParam_SJSJ.ApprovalRoleList = [data.AssignLoginName];
        WFOperator_SJSJ.StartPage.BeforeAction(args, func, {
            BusinessID: data.BusinessID,
            TaskName: data.TaskTitle
        });
    },
    CommonSave: function (action, args, func) {
        var businessID = utils.getQueryString("businessID");
        var url = api_url + 'api/todos/workflow/' + action + '/' + businessID;
        utils.ajax({
            type: 'POST',
            url: url,
            args: { BusinessID: businessID, TaskReportRemark: model.TaskReportRemark },
            success: function (data) {
                func();
            }
        });
    },
    Save: function (args, func) {
        Task.CommonSave("save", args, func);
    },
    Approve: function (args) {
        Task.CommonSave("approve", args, function () {
            window.close();
        });
    },
    Reject: function (args) {
        Task.CommonSave("reject", args, function () {
            WFOperator_SJSJ.AfterActionRedirect(args);
        });
    },
    AfterAction: function (argsT) {

        //TODO审批通过时修改数据状态，修改成功后请调用WFOperator_SJSJ.AfterActionRedirect(args);做跳转
        WFOperator_SJSJ.ApprovePage.AfterAction(argsT,
            {
                Approval: function (args) { Task.Approve(args); setTimeout(function () { location.href = '/todoListMobile.html'; }, 1000) },
                Return: function (args) { Task.Reject(args); setTimeout(function () { location.href = '/todoListMobile.html'; }, 1000) },
                Redirect: function (args) { setTimeout(function () { location.href = '/todoListMobile.html'; }, 1000) }
            });
    },
    LoadData: function (businessId, callback) {
        var url = api_url + 'api/todos/load/' + businessId;
        utils.ajax({
            type: 'GET',
            url: url,
            success: function (data) {
                //工作流操作开始
                WFOperator_SJSJ.InitSetting({
                    UserSelectSetting: {
                        IsNeedHiddenNav: utils.mobileBrower(),
                        TopValue: 14
                    },
                    OnAfterExecute: Task.AfterAction//执行后调用（进行回滚或其它操作（例如跳转））
                    , IsView: utils.getQueryString("v").length > 0 ? true : false
                });
                if (data.BusinessID != "") {
                    WFOperator_SJSJ.GetProcess({ BusinessID: data.BusinessID, CheckUserInProcess: utils.getQueryString("v").length > 0 ? false : true }, function () {
                        callback(data);
                    });
                }
            }
        });
    }
};