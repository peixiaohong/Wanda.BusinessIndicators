
var WFOperator_SJSJ = {
    Enum: {
        ProcessStatusString: {
            Draft: "Draft",
            InProcess: "InProcess",
            Approved: "Approved",
            Cancel: "Cancel"
        },
        ProcessStatusInt: {
            Draft: 0,
            InProcess: 5,
            Approved: 10,
            Cancel: 100
        }
    },
    InitSetting: function (otherSetting) {

        if (otherSetting.OnExecuteCheck == undefined) {
            otherSetting.OnExecuteCheck = function (operatorType) {
                return true;
                if (operatorType == 1 || operatorType == 6) {
                    var msg = "";
                    if (operatorType == 1) msg = "确认[提交]该操作吗?";
                    if (operatorType == 6) msg = "确认[退回]该操作吗?";

                    if (confirm(msg)) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return true;
                }
            }
        }
        else {
            var funcClientCheck = otherSetting.OnExecuteCheck;
            otherSetting.OnExecuteCheck = function (operatorType) {
                return funcClientCheck();
                if (operatorType == 1 || operatorType == 6) {
                    var msg = "";
                    if (operatorType == 1) msg = "确认[提交]该操作吗?";
                    if (operatorType == 6) msg = "确认[退回]该操作吗?";
                    if (confirm(msg)) {
                        return funcClientCheck();
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return funcClientCheck();
                }
            }
        }

        if (otherSetting.CustomerSceneSetting == null) {
            otherSetting.CustomerSceneSetting = { AlwaysReturnToStart: true };
        }
        else {
            otherSetting.CustomerSceneSetting.AlwaysReturnToStart = true;
        }
        //otherSetting.ButtonCssType = "middle";
        bpf_wf_client.initAjaxSetting("SJSJ_LC", true, otherSetting);
    },
    CreateProcess: function (option, func) {
        bpf_wf_client.createProcess(option, func);
    },
    GetProcess: function (businessID, func) {
        bpf_wf_client.getProcess(businessID, func);
    },
    RefreshProcess: function (option) {
        bpf_wf_client.refreshProcess(option);
    },
    _CommonPage: {
        LoadProcess: function (setting, func) {
            //流程Init之后调用
            var _setting = WFSetting_SJSJ.LoadSetting;
            $.extend(true, _setting, setting);
            if (_setting.BusinessID == null || _setting.BusinessID == undefined) {
                _setting.BusinessID = bpf_wf_tool.getQueryString("BusinessID");
            }
            var businessID = _setting.BusinessID;
            if (businessID != "") {
                WFOperator_SJSJ.GetProcess(businessID, function () {
                    if (!bpf_wf_data.WorkFlowContext.CurrentUserHasTodoTask) {
                        $("#aChangeUser").hide();
                        $("#AddASupplierLOG").hide();
                    }
                    if (_setting._IsStartPage || _setting.IsNeedRedirect) {
                        if (!bpf_wf_data.WorkFlowContext.CurrentUserHasTodoTask) {
                            if (!_setting.ApprovalPageHasOtherParam) {
                                location.href = _setting.ApprovalPage + '?BusinessID=' + businessID;
                            }
                            else {
                                location.href = _setting.ApprovalPage + '&BusinessID=' + businessID;
                            }
                        }
                    }
                    if (func != undefined && func != null) {
                        func();
                    }
                });
            }
            else {
                if (_setting._IsStartPage) {
                    WFOperator_SJSJ.CreateProcess({
                        FlowCode: _setting.FlowCode,
                        DynamicRoleUserList: WFParam_SJSJ.ConvertDynamicRole()
                    }, func);
                }
            }
        },
        BeforeAction: function (args, func, setting) {
            var _setting = WFSetting_SJSJ.BeforeSetting;
            $.extend(true, _setting, setting);
            args.BizContext.BusinessID = _setting.BusinessID;

            if (!_setting.IsCustomer) {
                args.BizContext.ProcessTitle = _setting.TaskName;
                args.BizContext.DynamicRoleUserList = WFParam_SJSJ.ConvertDynamicRole();
                args.BizContext.FormParams = { "TaskName": _setting.TaskName };
            }
            func();
        },
        SaveAction: function (args, func, setting) {
            //此处理方式仅对发起界面有效，依据点击的按钮来判断
            /*0：保存，1：提交，7：撤回，9：作废
            2：转发，5：加签，6：退回*/
            var _setting = WFSetting_SJSJ.SaveSetting;
            $.extend(true, _setting, setting);
            switch (args.OperatorType) {
                case 0:
                    //保存处理
                    _setting.Save(args, func);
                    break;
                case 1:
                    //提交处理
                    _setting.Submit(args, func);
                    break;
                case 2:
                    //转发处理
                    _setting.Forward(args, func);
                    break;
                case 5:
                    //前加签处理
                    _setting.PreAddNode(args, func);
                    break;
                case 6:
                    //退回处理
                    _setting.Return(args, func);
                    break;
                case 7:
                    //撤回处理
                    _setting.Save(args, func);
                    break;
                case 9:
                    //撤回处理
                    _setting.Cancel(args, func);
                    break;
                default:
                    //其它处理
                    func();
                    break;
            }
        },
        AfterAction: function (args, setting) {
            //此处理方式转发不做任何处理
            var _setting = WFSetting_SJSJ.AfterSetting;
            $.extend(true, _setting, setting);
            //TODO
            /*0：保存，1：提交，7：撤回，9：作废
            2：转发，5：加签，6：退回*/
            if (args.OperatorType != 2) {
                if (args.WorkflowContext.ProcessInstance.Status == 3) {
                    //判断本次提交是否为审批节点/会签节点/虚拟节点的提交才执行。
                    if (args.WorkflowContext.CurrentUserNodeID != null && args.WorkflowContext.CurrentUserNodeID != "") {
                        var nodeInfo = args.WorkflowContext.NodeInstanceList[args.WorkflowContext.CurrentUserNodeID];
                        if (nodeInfo != null &&
                            (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7)) {
                            _setting.Approval(args);
                            return;
                        }
                    }
                }
                else if (args.OperatorType == 6) {//else if (args.WorkflowContext.ProcessInstance.Status == 2) {
                    _setting.Return(args);
                    return;
                }
                else {
                    _setting.Redirect(args);
                    return;
                }
            }
            _setting.Redirect(args);
        }
    },
    StartPage: {
        LoadProcess: function (setting, func) {
            setting._IsStartPage = true;
            WFOperator_SJSJ._CommonPage.LoadProcess(setting, func);
        },
        BeforeAction: function (args, func, setting) {
            WFOperator_SJSJ._CommonPage.BeforeAction(args, func, setting);
        },
        SaveAction: function (args, func, setting) {
            WFOperator_SJSJ._CommonPage.SaveAction(args, func, setting);
        },
        AfterAction: function (args, setting) {
            WFOperator_SJSJ.AfterActionRedirect(args);
        }
    },
    ApprovePage: {
        LoadProcess: function (setting) {
            WFOperator_SJSJ._CommonPage.LoadProcess(setting);
        },
        AfterAction: function (args, setting) {
            WFOperator_SJSJ._CommonPage.AfterAction(args, setting);
        }
    },
    AfterActionRedirect: function (args, isAlert, msg) {
        isAlert = isAlert == undefined ? true : isAlert;
        if (isAlert) {
            alert(msg == undefined ? "操作成功" : msg);
        }
        /*0：保存，1：提交，7：撤回，9：作废，2：转发，5：加签，6：退回*/

        if (args.OperatorType == 1 || args.OperatorType == 5 || args.OperatorType == 7 || args.OperatorType == 9) {
            location.href = '/';//提交，退回,前加签后跳转至待办列表页
        }
        else if (args.OperatorType == 6) {
            window.opener = null; window.open('', '_self'); window.close();
        }
        else if (args.OperatorType == 0 || args.OperatorType == 2) {
            //不做任何跳转
        }
    }
}

var WFSetting_SJSJ =
{
    LoadSetting: {
        BusinessID: null,
        ApprovalPage: "/",//发起页面始终需要传值，审批页面如果IsNeedRedirect为true需要传值
        ApprovalPageHasOtherParam: false,
        FlowCode: "",//发起页面需要
        IsNeedRedirect: false,//审批页面独有
        _IsStartPage: false//内部使用，无需外部赋值
    },
    BeforeSetting: {
        BusinessID: "",
        TaskName: "",
        IsCustomer: false
    },
    SaveSetting: {
        Save: function (args, func) { func(); },
        Submit: function (args, func) { func(); },
        Forward: function (args, func) { func(); },
        PreAddNode: function (args, func) { func(); },
        Return: function (args, func) { func(); },
        Undo: function (args, func) { func(); },
        Cancel: function (args, func) { func(); }
    },
    AfterSetting: {
        Approval: function (args) { },
        Return: function (args) { },
        Redirect: function (args) { }//没有Approval和Return处理时传入
    }
}

var WFParam_SJSJ = {
    DyRoleList: [],//部门负责人
    ApprovalRoleList: [],//部门负责人
    ConvertDynamicRole: function () {
        var userListDyRole = [];
        $.each(WFParam_SJSJ.DyRoleList, function (i, item) {
            userListDyRole.push({ UserLoginID: item });
        });

        var userApprovalListDyRole = [];
        $.each(WFParam_SJSJ.ApprovalRoleList, function (i, item) {
            userApprovalListDyRole.push({ UserLoginID: item });
        });

        return { "DyRole": userListDyRole, "ApprovalRole": userApprovalListDyRole };
    }
}