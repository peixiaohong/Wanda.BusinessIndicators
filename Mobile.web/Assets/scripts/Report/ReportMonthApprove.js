$(function () {
    var businessId = utils.getQueryString("businessID");
    var proType = "";
    if (utils.getQueryString("ProType")) {
        proType = utils.getQueryString("ProType");
    }
    Task.Init(businessId, proType);
});
var model = null;
var Task = {
    Init: function (businessId, proType) {
        Task.LoadData(businessId, proType, Task.BuildData);
    },
    BuildData: function (data) {
        model = new Vue({
            el: '#MonthApproveContent',
            data: data,
            created: function () {
                var self = this;
                console.log(self.head);
                console.log(self.list);
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
        var url = api_url + 'Approval/MonthProcessRequest';
        var strPrcessStatus = "";
        if (args.WorkflowContext.ProcessInstance.Status == 2 && args.WorkflowContext.ProcessInstance.RunningNodeID == args.WorkflowContext.ProcessInstance.StartNodeID) {
            strPrcessStatus = "Draft";
        } else if (args.WorkflowContext.ProcessInstance.Status == -1) {
            PrcessStatus = "Cancel";
        } else if (args.WorkflowContext.ProcessInstance.Status == 3) {

            // 审批结束
            if (args.WorkflowContext.CurrentUserNodeID != null && args.WorkflowContext.CurrentUserNodeID != "") {
                var nodeInfo = args.WorkflowContext.NodeInstanceList[args.WorkflowContext.CurrentUserNodeID];
                if (nodeInfo != null && (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7)) {
                    strPrcessStatus = "Approved";
                } else {
                    strPrcessStatus = null;
                }
            } else {
                strPrcessStatus = null;
            }
        }
        else {
            strPrcessStatus = "Progress";
        }
        if (strPrcessStatus != null) {
            utils.ajax({
                type: 'GET',
                url: url,
                args: {
                    "BusinessID": businessID,
                    "strProType": utils.getQueryString("ProType"),
                    "ExecuteType": "afterAction",
                    "OperatorType": args.OperatorType,
                    "PrcessStatus": strPrcessStatus
                },
                success: function (data) {
                    func();
                }
            });

        }
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
        debugger;
        //TODO审批通过时修改数据状态，修改成功后请调用WFOperator_SJSJ.AfterActionRedirect(args);做跳转
        WFOperator_SJSJ.ApprovePage.AfterAction(argsT,
            {
                Approval: function (args) {
                    debugger;
                    Task.Approve(args);
                    setTimeout(function () { location.href = '/APP/todoListMobile.html'; }, 1000)
                },
                Return: function (args) { Task.Reject(args); setTimeout(function () { location.href = '/APP/todoListMobile.html'; }, 1000) },
                Redirect: function (args) {
                    Task.Approve(args);
                    setTimeout(function () { location.href = '/APP/todoListMobile.html'; }, 1000)
                }
            });
    },
    LoadData: function (businessId, proType, callback) {

        var ShowProecessNodeName = false;
        var url = api_url + 'Report/MonthApprove';
        utils.ajax({
            type: 'GET',
            url: url,
            args: {
                "strMonthReportID": businessId,
                "strBacthID": businessId,
                "strProType": proType
            },
            success: function (data) {
                if (data.IsSuccess && data.StatusCode == 200) {
                    var listData = JSON.parse(data.Data);
                    var result = {
                        "head": {
                            "SystemName": listData[0].ObjValue._System.SystemName,
                            "FinYear": listData[0].ObjValue.FinYear,
                            "FinMonth": listData[0].ObjValue.FinMonth,
                            "ObjValue": listData[1].ObjValue
                        },
                        "list": listData[2],
                        "reportState": false,
                        "currentState": false,
                        "totalState": false,
                        "yearlyState": false,
                    }
                    callback(result);
                } else {
                    utils.alertMessage(data.StatusMessage)
                }
                WFOperator_SJSJ.InitSetting({
                    UserSelectSetting: {
                        IsNeedHiddenNav: utils.mobileBrower(),
                        TopValue: 14
                    },
                    OnAfterExecute: Task.AfterAction//执行后调用（进行回滚或其它操作（例如跳转））
                    , IsView: utils.getQueryString("v").length > 0 ? true : false
                });
                if (businessId != "") {
                    WFOperator_SJSJ.GetProcess({ BusinessID: businessId, CheckUserInProcess: utils.getQueryString("v").length > 0 ? false : true }, function () {
                    });
                }
            }
        });
    }
};