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
        //var url = api_url + 'api/todos/load/' + businessId;
        //utils.ajax({
        //    type: 'GET',
        //    url: url,
        //    success: function (data) {
                //工作流操作开始
        var data = {
            "TaskAction": 0,
            "TaskID": "184f7ccb-a133-edb4-1089-ab9251e680a7",
            "BusinessID": "0c720c90-5f3c-44f4-9c49-756133de76c1",
            "TaskTitle": "审批测试",
            "CreatorLoginName": "zhengguilong",
            "CreatorName": "郑桂 ",
            "CreateDate": "2018-04-19T17:29:59.55+08:00",
            "ReceiveDate": "2018-04-19T17:29:59.55+08:00",
            "AssignLoginName": "zhengguilong",
            "AssignName": "郑桂 ",
            "AssignDate": "2018-04-19T17:29:59.52+08:00",
            "TaskAttachment": {
                "ID": "503694b8-0ac9-4bc1-8657-c6cd3a179f92",
                "FileName": "审批测试-郑桂+（zhengguilong）-20180427095121.xlsx",
                "FileSize": "31 KB",
                "Name": "审批测试-郑桂+（zhengguilong）-20180427095121",
                "FileCode": "\\\\192.168.50.72\\files\\d3f087c0-93ca-4b13-8a9a-bd33b1313df0",
                "IsUseV1": false,
                "CreateDate": "2018-04-27T09:51:35.38+08:00",
                "BusinessID": "0c720c90-5f3c-44f4-9c49-756133de76c1",
                "BusinessType": "UploadTaskData",
                "CreateLoginName": "zhengguilong",
                "CreateName": "郑桂 "
            },
            "TaskRemark": "",
            "TaskReportRemark": "",
            "TaskAttachments": [],
            "TaskReportAttachments": [],
            "TemplateID": "c22b0944-f778-48d4-93c5-aaa5d5e21d6e",
            "IsNeedApprove": true,
            "TaskStatus": 2,
            "ReceiveName": "郑桂 ",
            "ReceiveLoginName": "zhengguilong",
            "WorkflowCode": "YY_SJSJ-Standard",
            "EmployeeLoginName": "zhengguilong"
        }
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
        //    }
        //});
    }
};