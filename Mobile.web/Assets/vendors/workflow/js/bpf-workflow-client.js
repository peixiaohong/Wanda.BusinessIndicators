var bpf_wf_data = {
    BizContext: {
        AppCode: "",
        FlowCode: "",
        BusinessID: "",
        WFToken: "",
        FormParams: null,
        DynamicRoleUserList: null,
        CurrentUser: null,
        CheckUserInProcess: true,
        ProcessRunningNodeID: "",
        ProcessTitle: "",
        ProcessURL: "",
        ProcessMobileURL: "",
        NodeInstanceList: null,
        CcNodeInstanceList: null,
        ApprovalContent: "",
        ExtensionCommond: {}
    },
    WorkFlowContext: {
        ProcessLogList: []
    },
    ClientContext: {
        CcUserList: [],
        ForwardUserList: [],
        AddNodeUserList: [],
        CurrentAddNode: {}
    },
    OtherContext: {
        NodeInstanceArray: [],
        StartNode: null,
        NavNodeDom: {}
    },
    _buildAjaxData: function (methodName, step) {
        bpf_workflow_tool.buildCommonData();
        if (step == undefined) step = 0;
        return { MethodName: methodName, MethodMode: step, Version: '1.0', BizContext: JSON.stringify(bpf_wf_data.BizContext), BizWFURL: this._bizWFURL };
    },
    _clearContext: function () {
        //bpf_wf_data.ClientContext.CcUserList = [];
        //bpf_wf_data.ClientContext.ForwardUserList = [];
        //bpf_wf_data.ClientContext.CurrentAddNode = {};
        bpf_wf_data.OtherContext.NodeInstanceArray = [];
        bpf_wf_data.OtherContext.StartNode = null;
    },
    _bizWFURL: "",
    _getRunningNode: function () {
        //return bpf_wf_data.BizContext.NodeInstanceList[bpf_wf_data.BizContext.ProcessRunningNodeID];
        return bpf_wf_data.WorkFlowContext.NodeInstanceList[bpf_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID];
    },
    _getCloneNode: function (selectCloneNode) {
        var runningNode;
        if (selectCloneNode != undefined) {
            runningNode = selectCloneNode;
        }
        else {
            runningNode = bpf_wf_data._getRunningNode();
        }
        var runningNodeType = runningNode.NodeType;
        var cloneNode = null;
        if (runningNodeType == 0) {
            //发起节点使用下一节点来作为克隆节点
            var nextNode = bpf_wf_data.BizContext.NodeInstanceList[runningNode.NextNodeID];
            while (nextNode != null && (nextNode.NodeType == 5 || nextNode.NodeType == 6 || bpf_wf_data.ClientContext.CurrentAddNode[nextNode.NodeID] != null)) {
                if (nextNode.NextNodeID != "") {
                    nextNode = bpf_wf_data.BizContext.NodeInstanceList[nextNode.NextNodeID];
                }
                else {
                    nextNode = null;
                }
            }
            if (nextNode != null) {
                cloneNode = JQZepto.extend(true, {}, nextNode);
            }
        }
        else {
            //其它节点使用当前节点作为克隆节点
            cloneNode = JQZepto.extend(true, {}, runningNode);
        }
        return cloneNode;
    },
    _getNodeNameAndUserFormateText: function (node, nodeDataArray) {
        //返回的格式：发起人【XXX】，会签活动【XXX,XX】,
        var result = "";
        if (node.NodeType == 2 || node.NodeType == 3) {
            result = result + node.NodeName + "【";
            if (nodeDataArray == undefined || nodeDataArray == null) {
                nodeDataArray = bpf_wf_data.OtherContext.NodeInstanceArray;
            }
            var itemNodeList = Enumerable.From(nodeDataArray).Where("f=>f.ParentNodeID == '" + node.NodeID + "'").OrderBy("f=>f.NodeOrder").ToArray();
            var len = itemNodeList.length;
            JQZepto.each(itemNodeList, function (i, item) {
                result = result + item.User.UserName;
                if ((len - 1) != i) {
                    result = result + ",";
                }
            })
            result = result + "】";
        }
        else {
            if (node.User != null) {
                result = result + node.NodeName + "【" + node.User.UserName + "】";
            }
            else {
                result = result + node.NodeName;
            }
        }
        return result;
    },
    _getReturnNodeList: function (currentNode) {

    },
    _getAddNodeList: function (currentNode) {
        var nextNodeList = [];
        if (currentNode == null) return nextNodeList;
        nextNodeList.push(currentNode);
        var nextNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[currentNode.NextNodeID];
        while (nextNode != null) {
            if (nextNode.NodeType == 1 || nextNode.NodeType == 2 || nextNode.NodeType == 0 || nextNode.NodeType == 3 || nextNode.NodeType == 7) {
                nextNodeList.push(nextNode);
            }
            if (nextNode.NextNodeID != "") {
                nextNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[nextNode.NextNodeID]
            }
            else {
                nextNode = null;
            }
        }
        return nextNodeList;
    },
    _buildPostData: function (methodName, operatorType) {
        bpf_workflow_tool.buildCommonData();
        var executeParam = {
            OperatorType: operatorType,
            MethodName: methodName,
            Version: '1.0',
            BizContext: bpf_wf_data.BizContext,
            WorkflowContext: bpf_wf_data.WorkFlowContext
        }
        return executeParam;
    },
    _initBizContextByWorkFlowContext: function (workflowContext) {
        bpf_wf_data.BizContext.AppCode = workflowContext.AppCode;
        bpf_wf_data.BizContext.BusinessID = workflowContext.BusinessID;
        bpf_wf_data.BizContext.NodeInstanceList = null;
        bpf_wf_data.BizContext.NodeInstanceList = workflowContext.NodeInstanceList;
        bpf_wf_data.BizContext.CcNodeInstanceList = workflowContext.CcNodeInstanceList;
        if (workflowContext.ProcessInstance != null) {
            bpf_wf_data.BizContext.FlowCode = workflowContext.ProcessInstance.FlowCode;
            bpf_wf_data.BizContext.ProcessTitle = workflowContext.ProcessInstance.ProcessTitle;
            bpf_wf_data.BizContext.ProcessURL = workflowContext.ProcessInstance.ProcessURL;
            bpf_wf_data.BizContext.ProcessMobileURL = workflowContext.ProcessInstance.ProcessMobileURL;
            bpf_wf_data.BizContext.ProcessRunningNodeID = workflowContext.ProcessInstance.RunningNodeID;
        }
    },
    _initOtherContext: function (workflowContext) {
        //处理其它Context信息
        var nodeDataObject = workflowContext.NodeInstanceList;
        bpf_wf_data.OtherContext.NodeInstanceArray = [];
        bpf_wf_data.OtherContext.StartNode = null;
        if (nodeDataObject != null) {
            for (var key in nodeDataObject) {
                var nodeTemp = nodeDataObject[key];
                bpf_wf_data.OtherContext.NodeInstanceArray.push(nodeTemp);
                if (bpf_wf_data.OtherContext.StartNode == null) {
                    //如果服务器端的StartNodeID错误，则重新计算获取startNode
                    if (nodeTemp.NodeType == 0) {
                        bpf_wf_data.OtherContext.StartNode = nodeDataObject[key];
                    }
                }
            }
        }
    },
    _buildCcNode: function (user, nodeID) {
        return {
            NodeID: nodeID,
            NodeName: "抄送节点",
            NodeType: 4,
            Status: 0,
            ExtendProperties: {
                IsStartUser: "False",
                AllowMergeNode: "False",
                RejectNeedRepeatRun: "True",
                AllowChooseAnyUser: "True"
            },
            User: user,
            CreateDateTime: new Date(),
            CreateUser: bpf_wf_data.BizContext.CurrentUser,
            UpdateDateTime: new Date(),
            UpdateUser: bpf_wf_data.BizContext.CurrentUser
        };
    },
    _setWFURL: function (url) {
        this._bizWFURL = url;
    }
}

var bpf_wf_tool = {
    isJson: function (obj) {
        var objType = typeof (obj);
        if (objType == "object") {
            return Object.prototype.toString.call(obj).toLowerCase() == "[object object]" && !obj.length;
        }
        else if (objType == "string") {
            try {
                JSON.parse(obj);
                return true;
            } catch (e) {
                return false;
            }
        }
        else {
            return false;
        }
    },
    alert: function (value) {
        bpf_sdk_tool.alert(value);
    },
    tips: function (value) {
        bpf_sdk_tool.tips(value);
    },
    getQueryString: function (name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    },
    //显示全屏Loading图
    showLoading: function (msg) {
        bpf_sdk_tool.showLoading();
    },
    //隐藏全屏Loading图
    hideLoading: function () {
        bpf_sdk_tool.hideLoading();
    },
    ajax: function (func, data) {
        //JQZepto.ajax({
        //    type: "POST",
        //    url: "ProcessHandler.ashx?t=" + new Date().getMilliseconds(),
        //    dataType: "json",
        //    async: true,
        //    data: data,
        //    beforeSend: function () {
        //        bpf_sdk_tool.showLoading();
        //    },
        //    success: function (dataTemp) {
        //        bpf_sdk_tool.hideLoading();
        //        //try {
        //        func(dataTemp);
        //        //}
        //        //catch (ee) {
        //        //    bpf_wf_tool.alert(ee.name + ": " + ee.message);
        //        //}
        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        bpf_wf_tool.alert(errorThrown);
        //        bpf_wf_tool.hideLoading();
        //        throw (errorThrown);
        //    }
        //});
        var dataTemp = {
            "AppCode": "YY_SJSJ",
            "AppID": "",
            "BusinessID": "0c720c90-5f3c-44f4-9c49-756133de76c1",
            "CurrentUser": {
                "UserID": "3644",
                "UserCode": "3644",
                "UserName": "郑桂 ",
                "UserLoginID": "zhengguilong",
                "UserJobName": "专业总监",
                "UserOrgPathID": "/2/6/18/446",
                "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                "UserOrgID": "446"
            },
            "WFToken": "163e98b0-721f-4995-a039-b241d95deec2_0.782787124803221_20180428094858367",
            "ProcessInstance": {
                "ProcessID": "fed1e3ba-16f6-4e35-b627-836f29fddb11",
                "ProcessTitle": "审批测试",
                "ProcessURL": "/Application/Task/TaskCollectionView.aspx",
                "ProcessMobileURL": "",
                "Status": 3,
                "RunTimes": 1,
                "CreateDateTime": "2018-04-19T17:30:00.08",
                "CreateUser": {
                    "UserID": "3644",
                    "UserCode": "3644",
                    "UserName": "郑桂 ",
                    "UserLoginID": "zhengguilong",
                    "UserJobName": "专业总监",
                    "UserOrgPathID": "/2/6/18/446",
                    "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    "UserOrgID": "446"
                },
                "StartDateTime": "2018-04-27T09:48:27.743",
                "StartUser": {
                    "UserID": "3644",
                    "UserCode": "3644",
                    "UserName": "郑桂 ",
                    "UserLoginID": "zhengguilong",
                    "UserJobName": "专业总监",
                    "UserOrgPathID": "/2/6/18/446",
                    "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    "UserOrgID": "446"
                },
                "FinishDateTime": "2018-04-27T09:52:12.777",
                "FinishUser": {
                    "UserID": "3803",
                    "UserCode": "3803",
                    "UserName": "范冰 ",
                    "UserLoginID": "fanbing",
                    "UserJobName": "专业总监",
                    "UserOrgPathID": "/2/6/18/446",
                    "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    "UserOrgID": "446"
                },
                "UpdateDateTime": "2018-04-27T09:52:12.777",
                "UpdateUser": null,
                "FlowID": "fd89f647-f8b0-436b-adc8-d6b25223091b",
                "FlowCode": "YY_SJSJ-Standard",
                "FlowName": "标准流程",
                "RunningNodeID": "",
                "StartNodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                "ExtensionInfo": ""
            },
            "NodeInstanceList": {
                "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d": {
                    "NodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    "NodeType": 0,
                    "NodeName": "数据填报",
                    "NodeCode": "",
                    "NodeTitle": "",
                    "NodeURL": "/Application/Task/TaskCollection.aspx",
                    "NodeMobileURL": "",
                    "Status": 2,
                    "User": {
                        "UserID": "3644",
                        "UserCode": "3644",
                        "UserName": "郑桂 ",
                        "UserLoginID": "zhengguilong",
                        "UserJobName": "专业总监",
                        "UserOrgPathID": "/2/6/18/446",
                        "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                        "UserOrgID": ""
                    },
                    "ParentNodeID": "",
                    "NextNodeID": "7d151118-28b4-4b1e-92a3-e3861def031d",
                    "PrevNodeID": "",
                    "NodeOrder": 0,
                    "CloneNodeID": "",
                    "CloneNodeName": "",
                    "CloneNodeType": 0,
                    "IsDeleted": false,
                    "ExtendProperties": {
                        "IsStartUser": "False",
                        "AllowMergeNode": "True",
                        "RejectNeedRepeatRun": "True",
                        "AllowChooseAnyUser": "False",
                        "OnlyAllowChooseAnyUser": "False"
                    },
                    "ActivityProperties": null,
                    "StartDateTime": "2018-04-27T09:48:27.697",
                    "FinishDateTime": "2018-04-27T09:48:27.743",
                    "CreateDateTime": "2018-04-27T09:48:27.697",
                    "CreateUser": {
                        "UserID": "3644",
                        "UserCode": "3644",
                        "UserName": "郑桂 ",
                        "UserLoginID": "zhengguilong",
                        "UserJobName": "",
                        "UserOrgPathID": "",
                        "UserOrgPathName": "",
                        "UserOrgID": ""
                    },
                    "UpdateDateTime": "2018-04-27T09:48:27.743",
                    "UpdateUser": {
                        "UserID": "3644",
                        "UserCode": "3644",
                        "UserName": "郑桂 ",
                        "UserLoginID": "zhengguilong",
                        "UserJobName": "",
                        "UserOrgPathID": "",
                        "UserOrgPathName": "",
                        "UserOrgID": ""
                    },
                    "ActivityID": "ce041d7b-3303-4f5c-98d5-7bfef27f63dc",
                    "NomineeList": null,
                    "ExtensionInfo": ""
                },
                "7d151118-28b4-4b1e-92a3-e3861def031d": {
                    "NodeID": "7d151118-28b4-4b1e-92a3-e3861def031d",
                    "NodeType": 1,
                    "NodeName": "审批",
                    "NodeCode": "",
                    "NodeTitle": "",
                    "NodeURL": "",
                    "NodeMobileURL": "",
                    "Status": 2,
                    "User": {
                        "UserID": "3803",
                        "UserCode": "3803",
                        "UserName": "范冰 ",
                        "UserLoginID": "fanbing",
                        "UserJobName": "专业总监",
                        "UserOrgPathID": "",
                        "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                        "UserOrgID": ""
                    },
                    "ParentNodeID": "",
                    "NextNodeID": "",
                    "PrevNodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    "NodeOrder": 0,
                    "CloneNodeID": "",
                    "CloneNodeName": "",
                    "CloneNodeType": 0,
                    "IsDeleted": false,
                    "ExtendProperties": {
                        "IsStartUser": "False",
                        "AllowMergeNode": "True",
                        "RejectNeedRepeatRun": "True",
                        "AllowChooseAnyUser": "True",
                        "OnlyAllowChooseAnyUser": "False"
                    },
                    "ActivityProperties": null,
                    "StartDateTime": "2018-04-27T09:48:27.763",
                    "FinishDateTime": "2018-04-27T09:52:12.777",
                    "CreateDateTime": "2018-04-27T09:48:27.697",
                    "CreateUser": {
                        "UserID": "3644",
                        "UserCode": "3644",
                        "UserName": "郑桂 ",
                        "UserLoginID": "zhengguilong",
                        "UserJobName": "",
                        "UserOrgPathID": "",
                        "UserOrgPathName": "",
                        "UserOrgID": ""
                    },
                    "UpdateDateTime": "2018-04-27T09:52:12.777",
                    "UpdateUser": {
                        "UserID": "3803",
                        "UserCode": "3803",
                        "UserName": "范冰 ",
                        "UserLoginID": "fanbing",
                        "UserJobName": "",
                        "UserOrgPathID": "",
                        "UserOrgPathName": "",
                        "UserOrgID": ""
                    },
                    "ActivityID": "12ad625c-05ed-46ff-a926-782bb4620cf4",
                    "NomineeList": null,
                    "ExtensionInfo": ""
                }
            },
            "CcNodeInstanceList": null,
            "ProcessLogList": [{
                "LogID": "32363253-78d8-4699-8759-f79090b7f650",
                "NodeID": "7d151118-28b4-4b1e-92a3-e3861def031d",
                "NodeName": "审批",
                "NodeType": 1,
                "OpertationName": "批准",
                "User": {
                    "UserID": "3803",
                    "UserCode": "3803",
                    "UserName": "范冰 ",
                    "UserLoginID": "fanbing",
                    "UserJobName": "专业总监",
                    "UserOrgPathID": "/2/6/18/446",
                    "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    "UserOrgID": ""
                },
                "LogContent": "同意",
                "LogType": 1,
                "RequestDateTime": "2018-04-27T09:48:27.833",
                "FinishDateTime": "2018-04-27T09:52:12.797",
                "ExtensionInfo": ""
            }, {
                "LogID": "ffd730dd-32dd-4a6b-b7b2-08c770eb92b1",
                "NodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                "NodeName": "数据填报",
                "NodeType": 0,
                "OpertationName": "发起",
                "User": {
                    "UserID": "3644",
                    "UserCode": "3644",
                    "UserName": "郑桂 ",
                    "UserLoginID": "zhengguilong",
                    "UserJobName": "专业总监",
                    "UserOrgPathID": "/2/6/18/446",
                    "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    "UserOrgID": ""
                },
                "LogContent": "测试",
                "LogType": 1,
                "RequestDateTime": "2018-04-27T09:48:27.787",
                "FinishDateTime": "2018-04-27T09:48:27.793",
                "ExtensionInfo": ""
            }],
            "CurrentUserNodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
            "CurrentUserSceneSetting": {
                "ShowNavigationBar": true,
                "ShowCCBar": true,
                "AllowNewCC": false,
                "ShowApprovalTextArea": false,
                "ShowButtonBar": true,
                "ActionButtonList": [{
                    "ButtonType": 2,
                    "ButtonName": {
                        "CN ": "转发 ",
                        "EN ": "Forward "
                    },
                    "ButtonDisplayName": "转发",
                    "ButtonMethodName": "ForwardUser"
                }],
                "ShowApprovalLog": true
            },
            "CurrentUserHasTodoTask": false,
            "CurrentUserTodoTaskIsRead": false,
            "CurrentUserActivityPropertiesList": null,
            "ExtensionInfos": null,
            "StatusCode": 0,
            "StatusMessage": "",
            "LastException": null
        }
        func(dataTemp);
    },
    _workFlowServerURL: "",
    _applicationCode: "",
    getApplicationCode: function () {
        //返回的是不带/的地址
        if (bpf_wf_tool._applicationCode != "") {
            return bpf_wf_tool._applicationCode;
        }
        this.ajaxCommonHandler({
            RequestType: "AppSetting",
            Action: "ApplicationCode",
            SuccessReturn: function (data) {
                if (data != "") {
                    if (data.charAt(data.length - 1) == "/") {
                        data = data.substr(0, data.length - 1);
                    }
                    bpf_wf_tool._applicationCode = data;
                }
            }
        });
        return bpf_wf_tool._applicationCode;
    },
    getWorkFlowServerURL: function () {
        //返回的是不带/的地址
        if (bpf_wf_tool._workFlowServerURL != "") {
            return bpf_wf_tool._workFlowServerURL;
        }
        this.ajaxCommonHandler({
            RequestType: "AppSetting",
            Action: "WorkflowServerUrl",
            SuccessReturn: function (data) {
                if (data != "") {
                    if (data.charAt(data.length - 1) == "/") {
                        data = data.substr(0, data.length - 1);
                    }
                    bpf_wf_tool._workFlowServerURL = data;
                }
            }
        });
        return bpf_wf_tool._workFlowServerURL;
    },
    ajaxMaintenanace: function (func, methodName, data, isAsync) {
        this.ajaxCommon(func, "ProcessMaintenance", methodName, data, isAsync, undefined, "ProcessMaintenanceHandler");
    },
    ajaxCommon: function (func, action, methodName, data, isAsync, token, handler) {
        var defaultOption = {
            SuccessReturn: func,//回调函数
            Action: action,//动作类别
            Method: methodName,//方法名称
            Token: token,//标识
            IsAsync: isAsync,//是否异步
            Data: data,//POST的数据
            Handler: handler//请求的Handler
        }
        this.ajaxCommonHandler(defaultOption);
    },
    ajaxCommonHandler: function (option) {
        var defaultOption = {
            SuccessReturn: function () { },//回调函数
            RequestType: "Request",//请求类型
            Action: "",//动作类别
            Method: "",//方法名称
            Token: "",//标识
            IsAsync: false,//是否异步
            Data: {},//POST的数据
            Handler: "CommonHandler"//请求的Handler
        }
        JQZepto.extend(true, defaultOption, option);
        if (defaultOption.Token == "") {
            defaultOption.Token = bpf_sdk_tool.generalToken();
        }
        JQZepto.ajax({
            type: "POST",
            url: defaultOption.Handler + ".ashx?RequestType=" + defaultOption.RequestType + "&Action=" + defaultOption.Action + "&Method=" + defaultOption.Method + "&Token=" + defaultOption.Token + "&t=" + new Date().getMilliseconds(),
            dataType: "json",
            async: defaultOption.IsAsync,
            data: defaultOption.Data,
            beforeSend: function (xhr) {
                bpf_sdk_tool.showLoading();
            },
            success: function (dataTemp) {
                bpf_sdk_tool.hideLoading();
                if (dataTemp.IsSuccess) {
                    if (defaultOption.SuccessReturn != undefined) {
                        defaultOption.SuccessReturn(dataTemp.Data);
                    }
                }
                else {
                    if (dataTemp.StatusCode > 0 && dataTemp.StatusCode <= 200) {
                        bpf_sdk_tool.tips(dataTemp.StatusMessage);
                    }
                    else if (dataTemp.StatusCode > 200) {
                        bpf_sdk_tool.alert(dataTemp.StatusMessage);
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                bpf_sdk_tool.alert(errorThrown);
                bpf_sdk_tool.hideLoading();
                throw (errorThrown);
            }
        });
    },
    // 将简单数组对象转换为js string
    // 注：未处理双引号
    bindUserSelect: function (obj, func, appcode, allowMulti, allowAll, waitUser, checkedUser, exceptArray) {
        allowMulti = (allowMulti == undefined || allowMulti == null) ? true : allowMulti;
        allowAll = (allowAll == undefined || allowAll == null) ? true : allowAll;
        appcode = (appcode == undefined || appcode == null) ? bpf_wf_data.BizContext.AppCode : appcode;
        waitUser = (waitUser == undefined || waitUser == null) ? [] : waitUser;
        checkedUser = (checkedUser == undefined || checkedUser == null) ? [] : checkedUser;

        JQZepto(obj).bind("click", function () {
            bpf_userselect_client.selectUser({
                func: function (data) {
                    if (func != undefined) {
                        if (appcode == "YY_ZCPT") {
                            data = Enumerable.From(data).Where("f=>f.UserCode!='103779'").Distinct("f=>f.UserCode").ToArray();
                        }
                        else {
                            data = Enumerable.From(data).Distinct("f=>f.UserCode").ToArray();
                        }
                        if (exceptArray != null && exceptArray != undefined) {
                            data = Enumerable.From(data).Except(exceptArray, "f=>f.UserCode").ToArray();
                        }
                        func(data)
                    }
                },
                appCode: appcode,
                checkedUserList: checkedUser,
                waitUserList: waitUser,
                allowAll: allowAll,
                allowMulti: allowMulti
            });
        })
    },
    appendUserListToDom: function (domObj, data, isClearDom, funcAdd, funcDelete) {
        isClearDom = (isClearDom == undefined || isClearDom == null) ? false : isClearDom;
        JQZepto.each(data, function (i, item) {
            var span = JQZepto("<span>", { "class": "" });
            var spDel = JQZepto("<span>", { "class": "" }).append("<img class='imgDelete' alt='删除'>");
            span.append(JQZepto("<span>").text(item.UserName).attr("title", bpf_wf_tool.formateUserTips(item)));
            span.append(spDel);
            spDel.click(function () {
                JQZepto(this).parent().remove();
                if (funcDelete != undefined) {
                    funcDelete(item);
                }
            });
            span.append(JQZepto("<span>").text("；"));
            domObj.append(span);
            if (funcAdd != undefined) {
                funcAdd(item);
            }
        })
    },
    deleteArrayItem: function (array, item) {
        if (array != undefined && item != undefined) {
            array.splice(jQuery.inArray(item, array), 1);
        }
    },
    formateUserTips: function (userInfo) {
        if (userInfo == null) {
            return "";
        }
        //格式化用户显示
        var showUserOrgPathName = bpf_wf_client._otherSetting.CustomerSceneSetting.ShowUserOrgPathName;
        var showUserJobName = bpf_wf_client._otherSetting.CustomerSceneSetting.ShowUserJobName;
        var result = "姓名：" + userInfo.UserName + "(" + userInfo.UserLoginID + ")\r\n";
        if (showUserJobName == true) {
            result = result + "职务：" + userInfo.UserJobName + "\r\n";
        }
        if (showUserOrgPathName == true) {
            result = result + "组织机构：" + userInfo.UserOrgPathName + "\r\n";
        };
        return result;
    },
    formateNodeTips: function (node) {
        if (node == null || node == undefined) { return ""; }
        var nodeType = node.NodeType;
        switch (nodeType) {
            case 0:
                return "发起节点";
            case 1:
                return "审批节点";
            case 2:
                return "会签节点";
            case 3:
                return "通知节点";
            case 7:
                return "虚拟节点";
            default:
                return "";
        }
    },
    formatDate: function (date) {
        if (date.indexOf(".") >= 0) {
            return date.replace("T", " ").substr(0, date.indexOf(".")).replace(" ", "<br/>");
        }
        else {
            return date.replace("T", " ").replace(" ", "<br/>");
        }
    },
    convertToDate: function (value) {
        value = value.toString();
        if (value == "") { return null; }
        return new Date(value);
    },
    convertToInt: function (value) {
        value = value.toString();
        if (value == "") { return null; }
        return parseInt(value);
    },
    convertToFloat: function (value) {
        value = value.toString();
        if (value == "") { return null; }
        return parseFloat(value);
    },
    convertToBool: function (value) {
        value = value.toString().toLowerCase();
        if (value == "") { return false; }
        return value == "true" || value == "1";
    },
    convertToString: function (value) {
        value = value.toString();
        return value;
    },
    convertToType: function (value, type) {
        /*0:string,1:bool,2:number,3:date*/
        type = type.toString();
        switch (type) {
            case "1":
                return this.convertToBool(value);
            case "2":
                return this.convertToFloat(value);
            case "3":
                return this.convertToDate(value);
            default:
                return this.convertToString(value);

        }
    },
    initNodeByCloneNode: function (runningNode, cloneNode, nodeType, userInfo, preNodeID, nodeName) {
        var nodeID = uuid.v4();
        var nodeTemp = JQZepto.extend(true, {}, cloneNode);
        nodeTemp.NodeType = nodeType;
        nodeTemp.ParentNodeID = "";
        nodeTemp.CloneNodeID = cloneNode.NodeID;
        nodeTemp.CloneNodeName = cloneNode.NodeName;
        nodeTemp.CloneNodeType = cloneNode.NodeType;
        nodeTemp.User = userInfo;
        nodeTemp.NodeID = nodeID;
        nodeTemp.NodeName = nodeName;
        nodeTemp.Status = 0;
        nodeTemp.PrevNodeID = preNodeID;
        nodeTemp.NextNodeID = "";
        nodeTemp.StartDateTime = "0001-01-01T00:00:00";
        nodeTemp.FinishDateTime = "0001-01-01T00:00:00";
        return nodeTemp;
    },
    initAddNodeOrderList: function (runningNode, cloneNode, nodeName, addNodeUserList) {
        var preNodeID = "";
        var firstNodeID = "";
        var lastNodeID = "";
        var addNodeList = {};
        //顺序审批
        JQZepto.each(addNodeUserList, function (i, item) {
            var nodeTemp = bpf_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 1, item, preNodeID, nodeName);
            addNodeList[nodeTemp.NodeID] = nodeTemp;
            if (preNodeID != "") {
                addNodeList[preNodeID].NextNodeID = nodeTemp.NodeID;
            }
            if (firstNodeID == "") {
                firstNodeID = nodeTemp.NodeID;
            }
            lastNodeID = nodeTemp.NodeID;
            preNodeID = nodeTemp.NodeID;
        })
        return {
            FirstNodeID: firstNodeID,
            LastNodeID: lastNodeID,
            AddNodeList: addNodeList,
            AddNodeArray: []
        }
    },
    initAddNodeCosignerList: function (runningNode, cloneNode, nodeName, addNodeUserList) {
        //同时审批
        var addNodeList = {};
        var addNodeArray = [];
        var nodeTemp = bpf_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 2, null, "", nodeName);
        addNodeList[nodeTemp.NodeID] = nodeTemp;

        var firstNodeID = nodeTemp.NodeID;

        JQZepto.each(addNodeUserList, function (i, item) {
            var nodeItemTemp = bpf_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 2, item, "", nodeName);
            nodeItemTemp.ParentNodeID = nodeTemp.NodeID;
            nodeItemTemp.PrevNodeID = "";
            nodeItemTemp.NextNodeID = "";
            addNodeList[nodeItemTemp.NodeID] = nodeItemTemp;
            addNodeArray.push(nodeItemTemp);
        })
        return {
            FirstNodeID: firstNodeID,
            LastNodeID: firstNodeID,
            AddNodeList: addNodeList,
            AddNodeArray: addNodeArray
        }
    },
    initAddNodeAutoInformList: function (runningNode, cloneNode, nodeName, addNodeUserList) {
        //通知
        var addNodeList = {};
        var addNodeArray = [];
        var nodeTemp = bpf_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 3, null, "", nodeName);
        addNodeList[nodeTemp.NodeID] = nodeTemp;

        var firstNodeID = nodeTemp.NodeID;

        JQZepto.each(addNodeUserList, function (i, item) {
            var nodeItemTemp = bpf_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 3, item, "", nodeName);
            nodeItemTemp.ParentNodeID = nodeTemp.NodeID;
            nodeItemTemp.PrevNodeID = "";
            nodeItemTemp.NextNodeID = "";
            addNodeList[nodeItemTemp.NodeID] = nodeItemTemp;
            addNodeArray.push(nodeItemTemp);
        })
        return {
            FirstNodeID: firstNodeID,
            LastNodeID: firstNodeID,
            AddNodeList: addNodeList,
            AddNodeArray: addNodeArray
        }
    },
    _addNode: function (setting) {
        var _setting = {
            NodeName: "",
            NodeUserList: [],//List<UserInfo>完成的用户信息，用户信息必须准确且如果是自定义人员则必须与授权平台的人员信息一致
            AddNodeAuditType: 1,//（1：顺序审批，2：同时审批，3：通知节点）
            AddNodeType: 1,//加签类型（1：后加签，2：前加签）目前只支持后加签
            NodeID: ""
        };
        if (bpf_wf_data.WorkFlowContext == null) {
            //bpf_wf_tool.alert("请创建或获取流程信息成功后，再调用此方法");
            return;
        }
        JQZepto.extend(true, _setting, setting);
        if (_setting.NodeUserList.length == 0) {
            bpf_wf_tool.alert("未设置处理人，无法执行加签操作");
            return;
        }

        var runningNode = bpf_wf_data._getRunningNode();
        if (runningNode.NodeType != 0 && runningNode.NodeType != 1) {
            bpf_wf_tool.alert("只有发起节点和审批节点允许加签");
            return;
        }
        if (runningNode.NodeType == 0) {
            if (_setting.NodeID == "") {
                _setting.NodeID = bpf_wf_data.WorkFlowContext.ProcessInstance.StartNodeID;
            }
        }
        else {
            _setting.NodeID = runningNode.NodeID;
        }

        if (_setting.NodeName == "") {
            _setting.NodeName = runningNode.User.UserName + "加签"
        }

        var cloneNodeListObj = null;
        var selectNodeID = _setting.NodeID;
        var selectNode = bpf_wf_data.BizContext.NodeInstanceList[selectNodeID];
        if (selectNode == null) {
            bpf_wf_tool.alert("加签的节点不正确！");
            return;
        }
        var cloneNode = bpf_wf_data._getCloneNode(bpf_wf_data.BizContext.NodeInstanceList[selectNodeID]);
        if (cloneNode == null) {
            bpf_wf_tool.alert("克隆节点为空，不允许加签！");
            return;
        }
        var addNodeType = _setting.AddNodeType;//加签类型（1：后加签，2：前加签）
        var nodeNameCustomer = _setting.NodeName;
        var addNodeAuditType = _setting.AddNodeAuditType;//审批类型（1：顺序审批，2：同时审批）
        var copyBizContextNodeInstanceList = JQZepto.extend(true, {}, bpf_wf_data.BizContext.NodeInstanceList);//拷贝一份节点数据，保证加签出错时，数据可以正常
        if (_setting.NodeUserList.length < 2 && addNodeAuditType == 2) {
            bpf_wf_tool.alert("同时审批的处理人必须为两人及以上！");
            return false;
        }

        if (addNodeAuditType == 1) {
            cloneNodeListObj = bpf_wf_tool.initAddNodeOrderList(runningNode, cloneNode, nodeNameCustomer, _setting.NodeUserList);
        }
        else if (addNodeAuditType == 2) {
            cloneNodeListObj = bpf_wf_tool.initAddNodeCosignerList(runningNode, cloneNode, nodeNameCustomer, _setting.NodeUserList);
        }
        else if (addNodeAuditType == 3) {
            cloneNodeListObj = bpf_wf_tool.initAddNodeAutoInformList(runningNode, cloneNode, nodeNameCustomer, _setting.NodeUserList);
        }
        var addNodeList = cloneNodeListObj.AddNodeList;
        var addNodeArray = cloneNodeListObj.AddNodeArray;
        var firstNode = addNodeList[cloneNodeListObj.FirstNodeID];
        var lastNode = addNodeList[cloneNodeListObj.LastNodeID];

        //目前只支持后加签
        var navAddNodeDom = [];
        var node = firstNode;
        while (node != null) {
            var spDelNav = JQZepto("<span>", { "class": "bpf-wfclient-deletenav", "nodeid": node.NodeID }).append(bpf_wf_client._domBuild.buildDeleteImgDom());
            var spDom = bpf_wf_client._initItemControl.initNavItem(node, false, addNodeArray);
            spDom.append(spDelNav);

            spDelNav.click(function () {
                var nodeID = JQZepto(this).attr("nodeid");
                var nodeDelete = bpf_wf_data.BizContext.NodeInstanceList[nodeID];
                if (nodeDelete.NextNodeID == "") {
                    bpf_wf_data.BizContext.NodeInstanceList[nodeDelete.PrevNodeID].NextNodeID = "";
                    JQZepto(this).parent().prev().remove();
                }
                else {
                    var deletePreNode = bpf_wf_data.BizContext.NodeInstanceList[nodeDelete.PrevNodeID];
                    var deleteNextNode = bpf_wf_data.BizContext.NodeInstanceList[nodeDelete.NextNodeID];
                    deletePreNode.NextNodeID = deleteNextNode.NodeID;
                    deleteNextNode.PrevNodeID = deletePreNode.NodeID;
                }
                delete bpf_wf_data.BizContext.NodeInstanceList[nodeID];
                delete bpf_wf_data.OtherContext.NavNodeDom[nodeID];
                delete bpf_wf_data.ClientContext.CurrentAddNode[nodeID];
                JQZepto(this).parent().next().remove();
                JQZepto(this).parent().remove();
            })

            navAddNodeDom.push({ Dom: spDom, Arrow: bpf_wf_client._domBuild.buildNavArrowSpDom(), NodeID: node.NodeID });
            bpf_wf_data.ClientContext.CurrentAddNode[node.NodeID] = node;
            var nextNodeID = node.NextNodeID;
            if (nextNodeID != "") {
                node = addNodeList[nextNodeID];
            }
            else {
                node = null;
            }
        }
        var navCurrentDom = bpf_wf_data.OtherContext.NavNodeDom[selectNode.NodeID];
        var nowDom = navCurrentDom;
        JQZepto.each(navAddNodeDom, function (i, item) {
            nowDom.after(item.Arrow);
            item.Arrow.after(item.Dom);
            bpf_wf_data.OtherContext.NavNodeDom[item.NodeID] = item.Dom;
            nowDom = item.Dom;
        })

        var nextNode = bpf_wf_data.BizContext.NodeInstanceList[selectNode.NextNodeID];
        if (selectNode.NodeType == 0) {
            while (nextNode != null && (nextNode.NodeType == 5 || nextNode.NodeType == 6)) {
                if (nextNode.NextNodeID != "") {
                    nextNode = bpf_wf_data.BizContext.NodeInstanceList[nextNode.NextNodeID];
                }
                else {
                    nextNode = null;
                }
            }
        }
        if (nextNode != null) {
            var preNodeID = nextNode.PrevNodeID;
            lastNode.NextNodeID = nextNode.NodeID;
            nextNode.PrevNodeID = lastNode.NodeID;
            var preNode = bpf_wf_data.BizContext.NodeInstanceList[preNodeID]
            firstNode.PrevNodeID = preNode.NodeID;
            preNode.NextNodeID = firstNode.NodeID;
        }
        else {
            firstNode.PrevNodeID = selectNode.NodeID;
            selectNode.NextNodeID = firstNode.NodeID;
        }
        JQZepto.each(addNodeList, function (i, item) {
            bpf_wf_data.BizContext.NodeInstanceList[i] = item;
        })
        bpf_wf_data.BizContext.ExtensionCommond["AddAfterNode"] = "True";
        return true;
    },
    hiddenScroll: function () {
        //bpf_sdk_tool.hiddenScroll();
    },
    resetScroll: function () {
        //bpf_sdk_tool.resetScroll();
    },
    formateError: function (workflowContext, type, is) {
        type = (type == undefined || type == null) ? "error" : type;
        var message = "错误信息：";
        if (type == "info") {
            message = "提示信息：" + workflowContext.StatusMessage;
        }
        else {
            message = message + workflowContext.StatusMessage;
            if (workflowContext.LastException != null) {
                message = message + "\n" + workflowContext.LastException.Message;
            }
        }
        return message;
    }
}

var bpf_wf_client = {
    _setting: {
        AppCode: "",
        BusinessID: "",
        FlowCode: "",
        ProcessTitle: "",
        ProcessURL: "",
        ProcessMobileURL: "",
        DynamicRoleUserList: {},
        FormParams: {},
        CurrentUser: null,
        CheckUserInProcess: true,
        CcNodeInstanceList: null
    },
    _otherSetting: {
        ContainerDomID: "",
        ExecuteType: 1,//1：JS请求处理，2：POST处理
        IsAsync: false,//是否为异步请求。true表示异步请求，false表示同步请求
        IsShowContextMenu: false,//是否显示右键菜单
        PageContextMenu: true,//是否为页面级右键菜单
        IsSaveDraftToDoWork: true,//保存流程时是否生成待办
        EnableDebug: false,//是否开启Debug模式，建议仅调试时设置为true。
        ShowNodeName: true,//是否显示节点名称
        IsView: false,//是否仅查看
        CustomerProcessLog: null,//自定义审批日志
        CustomerSceneSetting: {
            CustomerButtons: [], //自定义的按钮及事件 [{ Name: "test", CssClass: "", OnClick: function () { } }] ,这是参数格式
            Language: 'CN',//应用系统语言配置
            AddNodeName: "",//加签节点名称
            ShowCc: true,//是否显示抄送
            ShowFowardButton: true,//是否显示转发按钮
            ShowUserOrgPathName: true,//审批日志审批人是否显示组织机构
            ShowUserJobName: true,//审批日志审批人是否显示职称
            AlwaysReturnToStart: false//不再弹出选择退回节点，始终退回发起节点
        },
        UserSelectSetting: {
            IsNeedHiddenNav: false,
            TopValue: 0
        },
        OnRejectNodeExecute: function (rejectNodeList) { return rejectNodeList },//处理退回节点列表
        IsCanDeleteAddNode: false,//是否允许删除加签的节点
        ButtonCssType: "default",//default:审批按钮居于底部,middle:"审批按钮居于审批日志和审批意见框中间"
        OnBeforeExecute: function (args, func) { if (func != undefined) { func(); } else { return true; } },//执行前(准备参数)
        OnSaveApplicationData: function (args, func) { if (func != undefined) { func(); } else { return true; } },//执行中，此处进行提交业务数据
        OnAfterExecute: function (args) { },//执行后调用（进行回滚或其它操作（例如跳转））
        OnExecuteCheck: function (operatorType) { return true; }//客户端JS校验
    },
    _constSetting: {
        //TODO：去掉图片地址
        WFURL: "",
        hasInit: false,
        resourceHostPath: function () {
            if (bpf_wf_client._constSetting.WFURL != "") {
                return bpf_wf_client._constSetting.WFURL + "/RuntimeService/img/";
            }
            else {
                return bpf_wf_tool.getWorkFlowServerURL() + "/RuntimeService/img/";
            }
        },
        okImgSrc: function () { return bpf_wf_client._constSetting.resourceHostPath() + "ok.png" },
        upImgSrc: function () { return bpf_wf_client._constSetting.resourceHostPath() + "up.png" },
        downImgSrc: function () { return bpf_wf_client._constSetting.resourceHostPath() + "down.png" },
        arrowImgSrc: function () { return bpf_wf_client._constSetting.resourceHostPath() + "arrow.png" },
        deleteImgSrc: function () { return bpf_wf_client._constSetting.resourceHostPath() + "delete.png" }
    },
    _setWFURL: function (url) {
        bpf_wf_client._constSetting.WFURL = url;
        bpf_wf_data._setWFURL(url);
    },
    initPostSetting: function (domID, otherSetting) {
        if (otherSetting != null && otherSetting != undefined) {
            if (typeof otherSetting.OnExecuteCheck != "function") {
                otherSetting.OnExecuteCheck = eval(otherSetting.OnExecuteCheck);
            }
        }
        this._initSetting(domID, 2, undefined, otherSetting);
    },
    initAjaxSetting: function (domID, isAsync, otherSetting) {
        this._initSetting(domID, 1, isAsync, otherSetting);
    },
    removeAjaxSetting: function () {
        //20170619增加移除配置的方法用于适配单页应用
        this._constSetting.hasInit = false;
        var otherSetting = {
            ContainerDomID: "",
            ExecuteType: 1,//1：JS请求处理，2：POST处理
            IsAsync: false,//是否为异步请求。true表示异步请求，false表示同步请求
            IsShowContextMenu: false,//是否显示右键菜单
            PageContextMenu: true,//是否为页面级右键菜单
            IsSaveDraftToDoWork: true,//保存流程时是否生成待办
            EnableDebug: false,//是否开启Debug模式，建议仅调试时设置为true。
            ShowNodeName: true,//是否显示节点名称
            IsView: false,//是否仅查看
            CustomerProcessLog: null,//自定义审批日志
            CustomerSceneSetting: {
                Language: 'CN',//应用系统语言配置
                ShowCc: true,//是否显示抄送
                ShowFowardButton: true,//是否显示转发按钮
                ShowUserOrgPathName: true,//审批日志审批人是否显示组织机构
                ShowUserJobName: true,//审批日志审批人是否显示职称
                AlwaysReturnToStart: false//不再弹出选择退回节点，始终退回发起节点
            },
            IsCanDeleteAddNode: false,//是否允许删除加签的节点
            ButtonCssType: "default",//default:审批按钮居于底部,middle:"审批按钮居于审批日志和审批意见框中间"
            OnBeforeExecute: function (args, func) { if (func != undefined) { func(); } else { return true; } },//执行前(准备参数)
            OnSaveApplicationData: function (args, func) { if (func != undefined) { func(); } else { return true; } },//执行中，此处进行提交业务数据
            OnAfterExecute: function (args) { },//执行后调用（进行回滚或其它操作（例如跳转））
            OnExecuteCheck: function (operatorType) { return true; }//客户端JS校验
        }
        //将配置都还原到初始设置
        $.extend(true, this._otherSetting, otherSetting);
        var setting = {
            AppCode: "",
            BusinessID: "",
            FlowCode: "",
            ProcessTitle: "",
            ProcessURL: "",
            ProcessMobileURL: "",
            DynamicRoleUserList: {},
            FormParams: {},
            CurrentUser: null,
            CheckUserInProcess: true,
            CcNodeInstanceList: null
        }
        $.extend(true, this._setting, setting);

        bpf_wf_data.BizContext = {
            AppCode: "",
            FlowCode: "",
            BusinessID: "",
            WFToken: "",
            FormParams: null,
            DynamicRoleUserList: null,
            CurrentUser: null,
            CheckUserInProcess: true,
            ProcessRunningNodeID: "",
            ProcessTitle: "",
            ProcessURL: "",
            ProcessMobileURL: "",
            NodeInstanceList: null,
            CcNodeInstanceList: null,
            ApprovalContent: "",
            ExtensionCommond: {}
        };
        bpf_wf_data.WorkFlowContext = null;
    },
    _initSetting: function (domID, executeType, isAsync, otherSetting) {
        //bpf_wf_tool.getWorkFlowServerURL();
        if (!this._constSetting.hasInit) {
            if (domID == "" || domID == undefined) {
                bpf_wf_tool.alert("未配置流程显示的Dom元素");
                return;
            }
            if (domID.substr(0, 1) != "#") {
                domID = "#" + domID;
            }
            this._otherSetting.ExecuteType = executeType;
            this._otherSetting.ContainerDomID = domID;
            if (isAsync != undefined && isAsync != null) {
                this._otherSetting.IsAsync = isAsync;
            }
            if (otherSetting != null && otherSetting != undefined) {
                //初始化所需数据FlowCode，BusinessID,
                //TODO：处理$.extend
                JQZepto.extend(true, this._otherSetting, otherSetting);
            }
            if (this._otherSetting.IsSaveDraftToDoWork) {
                bpf_wf_data.BizContext.ExtensionCommond.DrafterAddTodoTask = "True";
            }
            else {
                bpf_wf_data.BizContext.ExtensionCommond.DrafterAddTodoTask = "False";
            }
            this._constSetting.hasInit = true;
        }
        else {
            bpf_wf_tool.alert("已经初始化设置，请勿重复初始化！");
            throw ("已经初始化设置，请勿重复初始化！");
        }
    },
    getProcess: function (option, func, isRefresh) {
        this._initOption(option);
        if (isRefresh == null || isRefresh == undefined) {
            isRefresh = false;
        }
        //如果需要刷新，且当前节点为发起节点，当前用户为发起人，则允许刷新
        if (isRefresh
            //&& bpf_wf_data.WorkFlowContext.ProcessInstance.StartNodeID == bpf_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID
            //&& bpf_wf_data.WorkFlowContext.CurrentUser.UserCode == bpf_wf_data.WorkFlowContext.ProcessInstance.CreateUser.UserCode
        ) {
            bpf_wf_data.BizContext.GetProcessForceRefresh = "True";
        }
        //AJAX请求Handler获取数据
        bpf_wf_data.BizContext.WFToken = bpf_sdk_tool.generalToken();
        //TODO：AJAX处理，_buildAjaxData处理
        bpf_wf_tool.ajax(function (data) {
            bpf_wf_data.BizContext.GetProcessForceRefresh = "False";
            bpf_wf_client._processAjaxReturn(data);
            if (func != undefined && func != null) {
                func(data);
            }
        }, bpf_wf_data._buildAjaxData("GetProcess")
        );
    },
    createProcess: function (option, func) {
        this._initOption(option);
        this._createProcess(func);
    },
    refreshProcess: function (option) {
        this._initOption(option);
        bpf_wf_data.BizContext.WFToken = bpf_sdk_tool.generalToken();
        bpf_wf_tool.ajax(function (data) {
            bpf_wf_client._processAjaxReturn(data);
        }, bpf_wf_data._buildAjaxData("RefreshProcess")
        );
    },
    exist: function (businessID, trueFunc, falseFunc) {
        try {
            var result;
            bpf_wf_tool.ajaxCommon(function (data) {
                result = data.toString().toLowerCase() == "true";
                if (trueFunc != undefined && falseFunc != undefined) {
                    if (result) {
                        trueFunc();
                    }
                    else {
                        falseFunc();
                    }
                }
            }, "ProcessOperator", "ExistProcess", { BusinessID: businessID });
            return result;
        }
        catch (e) {
            return undefined;
        }
    },
    showProcess: function (workFlowContext) {
        if (workFlowContext != undefined && workFlowContext != null) {
            bpf_wf_client._processAjaxReturn(workFlowContext);
        }
        else {
            //此方法仅提供给POST方式调用，用于在GetProcess，CreateProcess，及POST后注册到前端显示
            if (bpf_wf_client._otherSetting.ExecuteType != 2) {
                bpf_wf_tool.alert("调用失败，该方法为POST接入方式调用！");
                return;
            }
            //如果是POST形式，则直接使用SDK注册的JSON对象MCS_WF_WORKFLOWCONTEXT_JSON显示数据
            bpf_wf_client._processAjaxReturn(MCS_WF_WORKFLOWCONTEXT_JSON);
        }
    },
    _initOption: function (option) {
        this._setting.BusinessID = "";
        this._setting.DynamicRoleUserList = {};
        this._setting.FormParams = {};
        this._setting.CurrentUser = null;
        this._setting.CheckUserInProcess = true;

        bpf_wf_data.BizContext.DynamicRoleUserList = {};
        bpf_wf_data.BizContext.FormParams = {};
        bpf_wf_data.BizContext.CcNodeInstanceList = {};

        if (option != undefined && option != null) {
            if (typeof option == "object") {
                //初始化所需数据FlowCode，BusinessID,
                JQZepto.extend(true, this._setting, option);
            }
            else {
                this._setting.BusinessID = option;
            }
        }
        this._repaireSetting();
    },
    _createProcess: function (func) {
        var isCanCreate = this._checkCanCreateProcess();
        if (isCanCreate) {
            bpf_wf_data.BizContext.WFToken = bpf_sdk_tool.generalToken();
            bpf_wf_tool.ajax(function (data) {
                bpf_wf_client._processAjaxReturn(data);
                if (func != undefined && func != null) {
                    func(data);
                }
            }, bpf_wf_data._buildAjaxData("CreateProcess")
            );
        }
    },
    _checkCanCreateProcess: function () {
        var msg = "";
        if (JQZepto.trim(this._setting.FlowCode) == "") {
            msg = msg + "FlowCode不能为空\r";
        }
        //暂确定校验，如果没有赋值，使用流程定义的名称和url
        //if ($.trim(this._setting.ProcessTitle) == "") {
        //    msg = msg + "ProcessTitle不能为空\r";
        //}
        //if ($.trim(this._setting.ProcessURL) == "") {
        //    msg = msg + "ProcessURL不能为空\r";
        //}
        //if ($.trim(this._setting.ProcessMobileURL) == "") {
        //    msg = msg + "ProcessMobileURL不能为空\r";
        //}
        if (msg != "") {
            bpf_wf_tool.tips(msg);
        }
        else {
            return true;
        }
    },
    _processAjaxReturn: function (workflowContext) {
        bpf_sdk_tool.log(JSON.stringify(workflowContext), bpf_wf_client._otherSetting.EnableDebug);
        var code = workflowContext.StatusCode;
        //处理null数据，改为[]，可以减少后边的判断
        if (workflowContext.CcNodeInstanceList == null) {
            workflowContext.CcNodeInstanceList = {};
        }
        if (workflowContext.ProcessLogList == null) {
            workflowContext.ProcessLogList = [];
        }
        if (workflowContext.ExtensionInfos == null) {
            workflowContext.ExtensionInfos = {};
        }
        if (workflowContext.CurrentUserSceneSetting != null && workflowContext.CurrentUserSceneSetting.ActionButtonList == null) {
            workflowContext.CurrentUserSceneSetting.ActionButtonList = [];
        }

        bpf_wf_data._clearContext();
        bpf_wf_data.BizContext.CurrentUser = workflowContext.CurrentUser;
        if (code >= 0 && code < 200) {
            if (code == 11 || code == 21 || code == 0) {
                if (workflowContext.NodeInstanceList != null) {
                    bpf_wf_data._initBizContextByWorkFlowContext(workflowContext);
                    bpf_wf_data._initOtherContext(workflowContext);
                    bpf_wf_data.WorkFlowContext = workflowContext;
                    //try {
                    bpf_wf_client._initProcessControl();
                    //}
                    //catch (a) {
                    //    alert(a);
                    //}
                }
                if (code == 11 || code == 21) {
                    //bpf_wf_tool.log(workflowContext,bpf_wf_client._otherSetting.EnableDebug);
                    bpf_wf_tool.alert(bpf_wf_tool.formateError(workflowContext, "info"));
                }
            }
            else {
                //bpf_wf_tool.log(workflowContext, bpf_wf_client._otherSetting.EnableDebug);
                bpf_wf_tool.alert(bpf_wf_tool.formateError(workflowContext, "info"));
            }
        }
        else if (code >= 200) {
            //bpf_wf_tool.log(workflowContext, bpf_wf_client._otherSetting.EnableDebug);
            bpf_wf_tool.alert(bpf_wf_tool.formateError(workflowContext));
        }

    },
    _repaireSetting: function () {
        bpf_wf_data.BizContext.CurrentUser = null;
        JQZepto.extend(true, bpf_wf_data.BizContext, this._setting);
        if (bpf_wf_data.WorkFlowContext != null && bpf_wf_data.WorkFlowContext.ProcessInstance != null) {
            var processInstance = bpf_wf_data.WorkFlowContext.ProcessInstance;
            if (bpf_wf_data.BizContext.FlowCode == "") {
                bpf_wf_data.BizContext.FlowCode = processInstance.FlowCode;
            }
            if (bpf_wf_data.BizContext.ProcessTitle == "") {
                bpf_wf_data.BizContext.ProcessTitle = processInstance.ProcessTitle;
            }
            if (bpf_wf_data.BizContext.ProcessURL == "") {
                bpf_wf_data.BizContext.ProcessURL = processInstance.ProcessURL;
            }
            if (bpf_wf_data.BizContext.ProcessMobileURL == "") {
                bpf_wf_data.BizContext.ProcessMobileURL = processInstance.ProcessMobileURL;
            }
        }
    },
    _initProcessControl: function () {
        //var div = $(this._otherSetting.ContainerDomID);
        //加载整体Dom内容
        //div.html(this.template.content());

        //加载局部内容
        var scene = bpf_wf_data.WorkFlowContext.CurrentUserSceneSetting;
        if (scene == null) return;
        if (this._otherSetting.IsView) {
            scene.ShowApprovalTextArea = false;
            scene.AllowNewCC = false;
            scene.ActionButtonList = [];
        }
        //是否显示抄送
        if (!this._otherSetting.CustomerSceneSetting.ShowCc) {
            scene.ShowCCBar = false;
        }
        //是否显示加签按钮
        if (!this._otherSetting.CustomerSceneSetting.ShowFowardButton) {
            bpf_wf_data.WorkFlowContext.CurrentUserSceneSetting.ActionButtonList = Enumerable.From(bpf_wf_data.WorkFlowContext.CurrentUserSceneSetting.ActionButtonList).Where("f=>f.ButtonType!=2").ToArray();
        }
        var approveTextDefault = "";
        //草稿状态下，将审批日志中的内容写入到审批意见框中
        if (bpf_wf_data.WorkFlowContext.ProcessInstance.Status == 0) {
            var draftLogInfo = Enumerable.From(bpf_wf_data.WorkFlowContext.ProcessLogList).SingleOrDefault({ "LogContent": "" }, "f=>f.OpertationName=='保存'");
            approveTextDefault = draftLogInfo.LogContent;
        }
        bpf_workflow_tool.initProcessControl(this._otherSetting.ContainerDomID, bpf_wf_data.WorkFlowContext, this._otherSetting, bpf_wf_client);
    },
    _execute: {
        execute: function (actionButtonItem, func) {
            var result = true;
            try {
                result = bpf_wf_client._otherSetting.OnExecuteCheck(actionButtonItem.ButtonType);
            }
            catch (e) {
                bpf_wf_tool.alert(e);
                return;
            }
            if (result == true) {
                var methodName = actionButtonItem.ButtonMethodName;
                bpf_wf_data.BizContext.WFToken = bpf_sdk_tool.generalToken();
                if (bpf_wf_client._otherSetting.ExecuteType == 1) {
                    bpf_wf_client._execute.executeAjax(methodName, actionButtonItem, func);
                }
                else if (bpf_wf_client._otherSetting.ExecuteType == 2) {
                    bpf_wf_client._execute.executePost(methodName, actionButtonItem, func);
                }
                else {
                    bpf_wf_tool.tips("不支持的请求方式");
                }
            }
        },
        executePost: function (methodName, actionButtonItem, func) {
            var execureParam = bpf_wf_data._buildPostData(methodName, actionButtonItem.ButtonType);
            var hfJsonDom = JQZepto("<input type='hidden'/>")
                .attr("id", "MCS_WF_OPERATIONJSON")
                .attr("name", "MCS_WF_OPERATIONJSON").insertBefore(JQZepto(bpf_wf_client._otherSetting.ContainerDomID));
            hfJsonDom.val(JSON.stringify(execureParam));
            bpf_wf_tool.showLoading();
            document.getElementById("MCS_WF_OPERATIONJSON").form.submit();
        },
        executeAjax: function (methodName, actionButtonItem, func) {
            var executeParam = {
                BizContext: bpf_wf_data.BizContext,
                OperatorType: actionButtonItem.ButtonType,
                MethodName: methodName,
                Version: '1.0',
                WorkflowContext: bpf_wf_data.WorkFlowContext
            };
            //如果执行的是撤回操作，则直接执行工作流方法，然后调用After。
            if (executeParam.OperatorType == 7) {
                bpf_wf_client._execute.executeAjaxOnlyCallAfter(executeParam, methodName, func);
                return;
            }
            if (bpf_wf_client._otherSetting.IsAsync) {
                bpf_wf_client._execute.executeAjaxAsync(executeParam, methodName, func);
            }
            else {
                bpf_wf_client._execute.executeAjaxNoAsync(executeParam, methodName, func);
            }
        },
        executeAjaxOnlyCallAfter: function (executeParam, methodName, func) {
            bpf_wf_tool.ajax(function (data) {
                bpf_wf_client._processAjaxReturn(data);
                if (data.StatusCode == 0) {
                    if (func != undefined) {
                        func(true);
                    }
                }
                else {
                    if (func != undefined) {
                        func(false);
                    }
                }
                executeParam.WorkflowContext = data;
                bpf_wf_client._otherSetting.OnAfterExecute(executeParam);
            }, bpf_wf_data._buildAjaxData(methodName, 0))
        },
        executeAjaxAsync: function (executeParam, methodName, func) {
            //BeforeExecute
            bpf_wf_client._otherSetting.OnBeforeExecute(executeParam, function () {
                if (executeParam.BizContext.BusinessID == "") {
                    bpf_wf_tool.alert("业务ID不能为空！");
                    return;
                }
                //执行Step1
                bpf_wf_tool.ajax(function (data) {
                    if (data.StatusCode == 0) {
                        //调用SaveData
                        bpf_wf_client._otherSetting.OnSaveApplicationData(executeParam, function () {
                            bpf_wf_tool.ajax(function (data) {
                                bpf_wf_client._processAjaxReturn(data);
                                if (data.StatusCode == 0) {
                                    if (func != undefined) {
                                        func(true);
                                    }
                                }
                                else {
                                    if (func != undefined) {
                                        func(false);
                                    }
                                }
                                executeParam.WorkflowContext = data;
                                bpf_wf_client._otherSetting.OnAfterExecute(executeParam);
                            }, bpf_wf_data._buildAjaxData(methodName, 2))
                        });
                    }
                    else {
                        bpf_wf_tool.hideLoading();
                        bpf_wf_client._processAjaxReturn(data);
                    }
                }, bpf_wf_data._buildAjaxData(methodName, 1));
            });

        },
        executeAjaxNoAsync: function (executeParam, methodName, func) {
            //BeforeExecute
            var result = bpf_wf_client._otherSetting.OnBeforeExecute(executeParam);
            if (result == false) { return; }
            if (executeParam.BizContext.BusinessID == "") {
                bpf_wf_tool.alert("业务ID不能为空！");
                return;
            }

            //执行Step1
            bpf_wf_tool.ajax(function (data) {
                if (data.StatusCode == 0) {
                    //调用SaveData
                    var isSuccess = bpf_wf_client._otherSetting.OnSaveApplicationData(executeParam);
                    //Step2
                    if (isSuccess || isSuccess == undefined) {
                        bpf_wf_tool.ajax(function (data) {
                            bpf_wf_client._processAjaxReturn(data);
                            if (data.StatusCode == 0) {
                                if (func != undefined) {
                                    func(true);
                                }
                            }
                            else {
                                if (func != undefined) {
                                    func(false);
                                }
                            }
                            executeParam.WorkflowContext = data;
                            bpf_wf_client._otherSetting.OnAfterExecute(executeParam);
                        }, bpf_wf_data._buildAjaxData(methodName, 2))
                    }
                    else {
                        bpf_wf_tool.hideLoading();
                    }
                }
                else {
                    bpf_wf_tool.hideLoading();
                    bpf_wf_client._processAjaxReturn(data);
                }
            }, bpf_wf_data._buildAjaxData(methodName, 1));
        }
    }
}