var BPF_Workflow_Model;
var VueWorkflowClient;

var bpf_workflow_tool = {
    buildCommonData: function () {
        //workflowModel.$data
        if (BPF_Workflow_Model != null) {
            var vmData = BPF_Workflow_Model.$data;
            var workFlowContext = vmData.workflowContext;
            //bpf_wf_data.BizContext.AppCode = "";
            //bpf_wf_data.BizContext.FlowCode = "";
            //bpf_wf_data.BizContext.BusinessID = "";
            //bpf_wf_data.BizContext.WFToken = "";
            //bpf_wf_data.BizContext.FormParams = "";
            //bpf_wf_data.BizContext.DynamicRoleUserList = "";
            //bpf_wf_data.BizContext.CurrentUser = "";
            //bpf_wf_data.BizContext.CheckUserInProcess = "";
            //bpf_wf_data.BizContext.ProcessRunningNodeID = "";
            //bpf_wf_data.BizContext.ProcessTitle = "";
            //bpf_wf_data.BizContext.ProcessURL = "";
            //bpf_wf_data.BizContext.ProcessMobileURL = "";
            bpf_wf_data.BizContext.NodeInstanceList = workFlowContext.NodeInstanceList;
            bpf_wf_data.BizContext.CcNodeInstanceList = workFlowContext.CcNodeInstanceList;
            bpf_wf_data.BizContext.ApprovalContent = vmData.approveText;
            //bpf_wf_data.BizContext.ExtensionCommond = "";
        }
    },
    initProcessControl: function (domID, workflowContext, otherSetting, client) {
        VueWorkflowClient = client;
        var approveTextDefault = "";
        //草稿状态下，将审批日志中的内容写入到审批意见框中
        if (workflowContext.ProcessInstance.Status == 0) {
            var draftLogInfo = Enumerable.From(workflowContext.ProcessLogList).SingleOrDefault({ "LogContent": "" }, "f=>f.OpertationName=='保存'");
            approveTextDefault = draftLogInfo.LogContent;
        }
        else {
            approveTextDefault = "";
        }
        if (BPF_Workflow_Model == null) {
            JQZepto(domID).append(this._content());
            this._initViewModel(domID, workflowContext, otherSetting, approveTextDefault);
        }
        else {
            BPF_Workflow_Model.$data.workflowContext = workflowContext;
        }
    },
    _initViewModel: function (domID, workflowContext, otherSetting, approveText) {
        BPF_Workflow_Model = new Vue({
            el: domID,
            data: {
                lang: $workflowlang.Mobile,
                workflowContext: workflowContext,//流程数据对象
                isExpandArea: {
                    nav: true,//是否折叠导航
                    cc: true,//是否折叠抄送
                    approveText: true,//是否折叠审批意见
                    approveLog: true//是否折叠审批记录
                },
                currentUserInfo: {
                    userInfo: {},//显示的用户信息
                    isHasRemove: false,//是否显示移除按钮
                    isShow: false,//是否显示用户信息
                    approveLogArray: [],//日志信息
                    removeFunc: function () { }//移除事件
                },
                isShowModal: {
                    reject: false,
                    addNode: false,
                    forward: false
                },
                currentAddNodeInfo: {
                    addNodeUserArray: [],
                    nodeName: "",
                    cloneNodeArray: [],
                    isBeforeAndAfter: 1,
                    auditType: 1,
                    isHasBeforeAddNode: true,
                    isShowAddAutoInform: true,
                    cloneNode: null,
                    currentUserName: ""
                },
                returnNodeArray: [],
                returnNode: null,
                forwardUserArray: [],
                isLoadingAllLog: false,
                isMoreActions: false,
                showNodeName: true,
                approveText: approveText
            },
            computed: {
                processLogList: function () {
                    //审批日志
                    var self = this;
                    return Enumerable.From(self.workflowContext.ProcessLogList).OrderByDescending("f=>f.FinishDateTime").ToArray();
                },
                runningNode: function () {
                    return this.workflowContext.NodeInstanceList[this.workflowContext.ProcessInstance.RunningNodeID];
                },
                processStatus: function () {
                    //流程状态：-1作废、0草稿、1审批中、2已退回、3审批通过
                    return this.workflowContext.ProcessInstance.Status;
                },
                sceneSetting: function () {
                    return this.workflowContext.CurrentUserSceneSetting;
                },
                isCanSelectUser: function () {
                    //是否可以选择用户
                    var workflowContext = this.workflowContext;
                    return workflowContext.ProcessInstance.RunningNodeID == workflowContext.ProcessInstance.StartNodeID && workflowContext.CurrentUserNodeID == workflowContext.ProcessInstance.RunningNodeID;;
                },
                startNode: function () {
                    var self = this;
                    var workflowContext = self.workflowContext;
                    var startNode = workflowContext.NodeInstanceList[workflowContext.ProcessInstance.StartNodeID];
                    if (startNode == null) {
                        startNode = bpf_wf_data.OtherContext.StartNode;
                    }
                    return startNode;
                },
                nodeInstanceArray: function () {
                    //审批导航列表
                    var self = this;
                    var nodeinstancearray = [];
                    var workflowContext = self.workflowContext;

                    var node = self.startNode;
                    while (node != null) {
                        if (node.NodeType == 2 || node.NodeType == 3) {
                            var itemNodeList = Enumerable.From(workflowContext.NodeInstanceList).Where("f=>f.Value.ParentNodeID == '" + node.NodeID + "'").Select("f=>f.Value").OrderBy("f=>f.NodeOrder").ToArray();
                            node.ChildNodeList = itemNodeList;
                        }
                        if (node.NomineeList == null) {
                            node.NomineeList = [];
                        }
                        if (node.NomineeList.length == 0) {
                            if (node.User != null) {
                                node.NomineeList.push(node.User);
                            }
                        }
                        else if (node.NomineeList.length > 10) {
                            if (node.User != null) {
                                var index = Enumerable.From(node.NomineeList).IndexOf("f=>f.UserCode=='" + node.User.UserCode + "'");
                                if (index > 9) {
                                    node.NomineeList.splice(index, 1);
                                    node.NomineeList.unshift(node.User);
                                }
                            }
                        }
                        node.SelectUserCode = node.User != null ? node.User.UserCode : "";
                        nodeinstancearray.push(node);

                        var nextNodeID = node.NextNodeID;
                        if (nextNodeID != "") {
                            node = workflowContext.NodeInstanceList[nextNodeID];
                        }
                        else {
                            node = null;
                        }
                    }
                    return nodeinstancearray;
                },
                actionButton: function () {
                    //按钮顺序列表
                    //保存、提交、退回、加签、转发、作废、撤销
                    var self = this;
                    //self.workflowContext = {
                    //    "AppCode": "YY_SJSJ",
                    //    "AppID": "",
                    //    "BusinessID": "0c720c90-5f3c-44f4-9c49-756133de76c1",
                    //    "CurrentUser": {
                    //        "UserID": "3644",
                    //        "UserCode": "3644",
                    //        "UserName": "郑桂 ",
                    //        "UserLoginID": "zhengguilong",
                    //        "UserJobName": "专业总监",
                    //        "UserOrgPathID": "/2/6/18/446",
                    //        "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //        "UserOrgID": "446"
                    //    },
                    //    "WFToken": "84df167a-d1c0-4276-85bb-57858a9f2fe6_0.6235180056912153_20180428100458408",
                    //    "ProcessInstance": {
                    //        "ProcessID": "fed1e3ba-16f6-4e35-b627-836f29fddb11",
                    //        "ProcessTitle": "审批测试",
                    //        "ProcessURL": "/Application/Task/TaskCollectionView.aspx",
                    //        "ProcessMobileURL": "",
                    //        "Status": 3,
                    //        "RunTimes": 1,
                    //        "CreateDateTime": "2018-04-19T17:30:00.08",
                    //        "CreateUser": {
                    //            "UserID": "3644",
                    //            "UserCode": "3644",
                    //            "UserName": "郑桂 ",
                    //            "UserLoginID": "zhengguilong",
                    //            "UserJobName": "专业总监",
                    //            "UserOrgPathID": "/2/6/18/446",
                    //            "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //            "UserOrgID": "446"
                    //        },
                    //        "StartDateTime": "2018-04-27T09:48:27.743",
                    //        "StartUser": {
                    //            "UserID": "3644",
                    //            "UserCode": "3644",
                    //            "UserName": "郑桂 ",
                    //            "UserLoginID": "zhengguilong",
                    //            "UserJobName": "专业总监",
                    //            "UserOrgPathID": "/2/6/18/446",
                    //            "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //            "UserOrgID": "446"
                    //        },
                    //        "FinishDateTime": "2018-04-27T09:52:12.777",
                    //        "FinishUser": {
                    //            "UserID": "3803",
                    //            "UserCode": "3803",
                    //            "UserName": "范冰 ",
                    //            "UserLoginID": "fanbing",
                    //            "UserJobName": "专业总监",
                    //            "UserOrgPathID": "/2/6/18/446",
                    //            "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //            "UserOrgID": "446"
                    //        },
                    //        "UpdateDateTime": "2018-04-27T09:52:12.777",
                    //        "UpdateUser": null,
                    //        "FlowID": "fd89f647-f8b0-436b-adc8-d6b25223091b",
                    //        "FlowCode": "YY_SJSJ-Standard",
                    //        "FlowName": "标准流程",
                    //        "RunningNodeID": "",
                    //        "StartNodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    //        "ExtensionInfo": ""
                    //    },
                    //    "NodeInstanceList": {
                    //        "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d": {
                    //            "NodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    //            "NodeType": 0,
                    //            "NodeName": "数据填报",
                    //            "NodeCode": "",
                    //            "NodeTitle": "",
                    //            "NodeURL": "/Application/Task/TaskCollection.aspx",
                    //            "NodeMobileURL": "",
                    //            "Status": 2,
                    //            "User": {
                    //                "UserID": "3644",
                    //                "UserCode": "3644",
                    //                "UserName": "郑桂 ",
                    //                "UserLoginID": "zhengguilong",
                    //                "UserJobName": "专业总监",
                    //                "UserOrgPathID": "/2/6/18/446",
                    //                "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //                "UserOrgID": ""
                    //            },
                    //            "ParentNodeID": "",
                    //            "NextNodeID": "7d151118-28b4-4b1e-92a3-e3861def031d",
                    //            "PrevNodeID": "",
                    //            "NodeOrder": 0,
                    //            "CloneNodeID": "",
                    //            "CloneNodeName": "",
                    //            "CloneNodeType": 0,
                    //            "IsDeleted": false,
                    //            "ExtendProperties": {
                    //                "IsStartUser": "False",
                    //                "AllowMergeNode": "True",
                    //                "RejectNeedRepeatRun": "True",
                    //                "AllowChooseAnyUser": "False",
                    //                "OnlyAllowChooseAnyUser": "False"
                    //            },
                    //            "ActivityProperties": null,
                    //            "StartDateTime": "2018-04-27T09:48:27.697",
                    //            "FinishDateTime": "2018-04-27T09:48:27.743",
                    //            "CreateDateTime": "2018-04-27T09:48:27.697",
                    //            "CreateUser": {
                    //                "UserID": "3644",
                    //                "UserCode": "3644",
                    //                "UserName": "郑桂 ",
                    //                "UserLoginID": "zhengguilong",
                    //                "UserJobName": "",
                    //                "UserOrgPathID": "",
                    //                "UserOrgPathName": "",
                    //                "UserOrgID": ""
                    //            },
                    //            "UpdateDateTime": "2018-04-27T09:48:27.743",
                    //            "UpdateUser": {
                    //                "UserID": "3644",
                    //                "UserCode": "3644",
                    //                "UserName": "郑桂 ",
                    //                "UserLoginID": "zhengguilong",
                    //                "UserJobName": "",
                    //                "UserOrgPathID": "",
                    //                "UserOrgPathName": "",
                    //                "UserOrgID": ""
                    //            },
                    //            "ActivityID": "ce041d7b-3303-4f5c-98d5-7bfef27f63dc",
                    //            "NomineeList": null,
                    //            "ExtensionInfo": ""
                    //        },
                    //        "7d151118-28b4-4b1e-92a3-e3861def031d": {
                    //            "NodeID": "7d151118-28b4-4b1e-92a3-e3861def031d",
                    //            "NodeType": 1,
                    //            "NodeName": "审批",
                    //            "NodeCode": "",
                    //            "NodeTitle": "",
                    //            "NodeURL": "",
                    //            "NodeMobileURL": "",
                    //            "Status": 2,
                    //            "User": {
                    //                "UserID": "3803",
                    //                "UserCode": "3803",
                    //                "UserName": "范冰 ",
                    //                "UserLoginID": "fanbing",
                    //                "UserJobName": "专业总监",
                    //                "UserOrgPathID": "",
                    //                "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //                "UserOrgID": ""
                    //            },
                    //            "ParentNodeID": "",
                    //            "NextNodeID": "",
                    //            "PrevNodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    //            "NodeOrder": 0,
                    //            "CloneNodeID": "",
                    //            "CloneNodeName": "",
                    //            "CloneNodeType": 0,
                    //            "IsDeleted": false,
                    //            "ExtendProperties": {
                    //                "IsStartUser": "False",
                    //                "AllowMergeNode": "True",
                    //                "RejectNeedRepeatRun": "True",
                    //                "AllowChooseAnyUser": "True",
                    //                "OnlyAllowChooseAnyUser": "False"
                    //            },
                    //            "ActivityProperties": null,
                    //            "StartDateTime": "2018-04-27T09:48:27.763",
                    //            "FinishDateTime": "2018-04-27T09:52:12.777",
                    //            "CreateDateTime": "2018-04-27T09:48:27.697",
                    //            "CreateUser": {
                    //                "UserID": "3644",
                    //                "UserCode": "3644",
                    //                "UserName": "郑桂 ",
                    //                "UserLoginID": "zhengguilong",
                    //                "UserJobName": "",
                    //                "UserOrgPathID": "",
                    //                "UserOrgPathName": "",
                    //                "UserOrgID": ""
                    //            },
                    //            "UpdateDateTime": "2018-04-27T09:52:12.777",
                    //            "UpdateUser": {
                    //                "UserID": "3803",
                    //                "UserCode": "3803",
                    //                "UserName": "范冰 ",
                    //                "UserLoginID": "fanbing",
                    //                "UserJobName": "",
                    //                "UserOrgPathID": "",
                    //                "UserOrgPathName": "",
                    //                "UserOrgID": ""
                    //            },
                    //            "ActivityID": "12ad625c-05ed-46ff-a926-782bb4620cf4",
                    //            "NomineeList": null,
                    //            "ExtensionInfo": ""
                    //        }
                    //    },
                    //    "CcNodeInstanceList": {},
                    //    "ProcessLogList": [{
                    //        "LogID": "32363253-78d8-4699-8759-f79090b7f650",
                    //        "NodeID": "7d151118-28b4-4b1e-92a3-e3861def031d",
                    //        "NodeName": "审批",
                    //        "NodeType": 1,
                    //        "OpertationName": "批准",
                    //        "User": {
                    //            "UserID": "3803",
                    //            "UserCode": "3803",
                    //            "UserName": "范冰 ",
                    //            "UserLoginID": "fanbing",
                    //            "UserJobName": "专业总监",
                    //            "UserOrgPathID": "/2/6/18/446",
                    //            "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //            "UserOrgID": ""
                    //        },
                    //        "LogContent": "同意",
                    //        "LogType": 1,
                    //        "RequestDateTime": "2018-04-27T09:48:27.833",
                    //        "FinishDateTime": "2018-04-27T09:52:12.797",
                    //        "ExtensionInfo": ""
                    //    }, {
                    //        "LogID": "ffd730dd-32dd-4a6b-b7b2-08c770eb92b1",
                    //        "NodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    //        "NodeName": "数据填报",
                    //        "NodeType": 0,
                    //        "OpertationName": "发起",
                    //        "User": {
                    //            "UserID": "3644",
                    //            "UserCode": "3644",
                    //            "UserName": "郑桂 ",
                    //            "UserLoginID": "zhengguilong",
                    //            "UserJobName": "专业总监",
                    //            "UserOrgPathID": "/2/6/18/446",
                    //            "UserOrgPathName": "/隆基泰和控股集团/隆基泰和置业/运营管理中心/信息管理部",
                    //            "UserOrgID": ""
                    //        },
                    //        "LogContent": "测试",
                    //        "LogType": 1,
                    //        "RequestDateTime": "2018-04-27T09:48:27.787",
                    //        "FinishDateTime": "2018-04-27T09:48:27.793",
                    //        "ExtensionInfo": ""
                    //    }],
                    //    "CurrentUserNodeID": "5b8ca850-9d8b-4401-a3a9-0e9d1f61ff9d",
                    //    "CurrentUserSceneSetting": {
                    //        "ShowNavigationBar": true,
                    //        "ShowCCBar": true,
                    //        "AllowNewCC": false,
                    //        "ShowApprovalTextArea": false,
                    //        "ShowButtonBar": true,
                    //        "ActionButtonList": [{
                    //            "ButtonType": 2,
                    //            "ButtonName": {
                    //                "CN ": "转发 ",
                    //                "EN ": "Forward "
                    //            },
                    //            "ButtonDisplayName": "转发",
                    //            "ButtonMethodName": "ForwardUser"
                    //        }],
                    //        "ShowApprovalLog": true
                    //    },
                    //    "CurrentUserHasTodoTask": false,
                    //    "CurrentUserTodoTaskIsRead": false,
                    //    "CurrentUserActivityPropertiesList": null,
                    //    "ExtensionInfos": {},
                    //    "StatusCode": 0,
                    //    "StatusMessage": "",
                    //    "LastException": null
                    //}
                    var result = {
                        normalButtonList: [],
                        moreButtonList: []
                    };
                    var language = otherSetting.CustomerSceneSetting.Language;
                    $.each(self.workflowContext.CurrentUserSceneSetting.ActionButtonList, function (i, item) {
                        var buttonDefine = {};
                        if (bpf_wf_tool.isJson(item.ButtonName)) {
                            //buttonDefine = JSON.parse(item.ButtonName);//转换按钮信息为JSON对象
                            buttonDefine = { CN: "转发", EN: "Forward" }
                        }
                        item.ButtonDisplayName = buttonDefine[language] == undefined ? item.ButtonDisplayName : buttonDefine[language]
                        if (i < 2) {
                            result.normalButtonList.push(item);
                        }
                        else {
                            result.moreButtonList.push(item);
                        }
                    })
                    return result;
                },
                hasCcUser: function () {
                    var self = this;
                    var hasProp = false;
                    for (var prop in self.workflowContext.CcNodeInstanceList) {
                        hasProp = true;
                        break;
                    }
                    return hasProp;
                }
            },
            methods: {
                foldArea: function (type) {
                    //是否折叠组显示内容
                    this.isExpandArea[type] = !this.isExpandArea[type];
                },
                hideShowModal: function (type) {
                    //隐藏退回、转发、加签弹窗
                    bpf_wf_tool.resetScroll();
                    this.isShowModal[type] = false;
                },
                showModal: function (type) {
                    //显示退回、转发、加签弹窗
                    bpf_wf_tool.hiddenScroll();
                    this.isMoreActions = false;
                    this.isShowModal[type] = true;
                },
                showHideNodeName: function () {
                    //显示/隐藏节点名称
                    this.showNodeName = !this.showNodeName;
                },
                showHideMoreActions: function () {
                    this.isMoreActions = !this.isMoreActions;
                },
                quickLog: function (quicklog) {
                    //快捷意见
                    if (this.approveText == "") {
                        this.approveText = quicklog;
                    }
                    else {
                        if (this.approveText != "同意" && this.approveText != "不同意" && this.approveText != "收到") {
                            if (!confirm("此操作将清空现在输入的审批意见，是否确认？")) {
                                return;
                            }
                        }
                        this.approveText = quicklog;
                    }
                },
                showUserInfoUser: function (setting) {
                    //显示用户信息（）
                    bpf_wf_tool.hiddenScroll();
                    var settingInfo = $.extend(this.currentUserInfo, setting, true)
                    settingInfo.isShow = true;
                    this.currentUserInfo = settingInfo;
                },
                hideUserInfo: function () {
                    //关闭用户信息弹窗，将属性赋值为原始属性
                    bpf_wf_tool.resetScroll();
                    this.currentUserInfo.isShow = false;
                    this.currentUserInfo.approveLogArray = [];
                    this.currentUserInfo.isHasRemove = false;
                    this.currentUserInfo.userInfo = {};
                    this.currentUserInfo.removeFunc = function () { };
                },
                removeUserInfo: function () {
                    //移除用户事件
                    if (this.currentUserInfo.isHasRemove && this.currentUserInfo.removeFunc != undefined) {
                        this.currentUserInfo.removeFunc();
                    }
                    this.hideUserInfo();
                },
                selectCcUser: function () {
                    //添加抄送人
                    var self = this;
                    bpf_userselect_client.selectUser({
                        func: function (data) {
                            $.each(data, function (i, item) {
                                var nodeID = uuid.v4();
                                Vue.set(self.workflowContext.CcNodeInstanceList, nodeID, bpf_wf_data._buildCcNode(item, nodeID));
                            })
                        },
                        topValue: otherSetting.UserSelectSetting.TopValue,
                        isNeedHiddenNav: otherSetting.UserSelectSetting.IsNeedHiddenNav
                    })
                },
                showCcUser: function (ccnode) {
                    //显示抄送用户信息
                    var self = this;
                    self.showUserInfoUser({
                        userInfo: ccnode.User,
                        isHasRemove: self.sceneSetting.AllowNewCC,
                        removeFunc: function () {
                            Vue.delete(self.workflowContext.CcNodeInstanceList, ccnode.NodeID);
                        }
                    });
                },
                deleteNode: function (node, index) {
                    //删除加签的节点
                    var self = this;
                    var workflowContext = self.workflowContext;
                    if (node.NextNodeID == "") {
                        workflowContext.NodeInstanceList[node.PrevNodeID].NextNodeID = "";
                    }
                    else {
                        var deletePreNode = workflowContext.NodeInstanceList[node.PrevNodeID];
                        var deleteNextNode = workflowContext.NodeInstanceList[node.NextNodeID];
                        deletePreNode.NextNodeID = deleteNextNode.NodeID;
                        deleteNextNode.PrevNodeID = deletePreNode.NodeID;
                    }
                    Vue.delete(workflowContext.NodeInstanceList, node.NodeID)
                },
                showNodeUser: function (node) {
                    //显示节点用户信息
                    var self = this;
                    var approveLogArray = Enumerable.From(self.processLogList).Where("f=>f.User.UserCode=='" + node.User.UserCode + "'").ToArray();
                    self.showUserInfoUser({
                        userInfo: node.User,
                        approveLogArray: approveLogArray
                    });
                },
                selectNodeUser: function (node) {
                    //选择节点用户。
                    var userCode = node.SelectUserCode;
                    if (userCode == "") {
                        node.User = null;
                    }
                    else if (userCode == "...") {
                        var self = this;
                        var allowChooseAnyUserSelect = node.ExtendProperties["AllowChooseAnyUser"] == "True";
                        bpf_userselect_client.selectUser({
                            func: function (data) {
                                if (data.length == 1) {
                                    var userInfo = data[0];
                                    var nomineeUserInfo = Enumerable.From(node.NomineeList).FirstOrDefault(null, "f=>f.UserCode == '" + userInfo.UserCode + "'");
                                    if (nomineeUserInfo == null) {
                                        node.NomineeList.push(userInfo);
                                        node.User = userInfo;
                                    }
                                    else {
                                        node.User = null;
                                        node.User = nomineeUserInfo;
                                    }
                                }
                                else {
                                    node.User = {};
                                    node.User = null;
                                }
                            },
                            exit: function () {
                                node.User = {};
                                node.User = null;
                            },
                            allowAll: true,
                            allowMulti: false,
                            waitUserList: node.NomineeList,
                            topValue: otherSetting.UserSelectSetting.TopValue,
                            isNeedHiddenNav: otherSetting.UserSelectSetting.IsNeedHiddenNav
                        })
                    }
                    else {
                        var userInfo = Enumerable.From(node.NomineeList).FirstOrDefault(null, "f=>f.UserCode == '" + userCode + "'");
                        node.User = userInfo;
                    }
                },
                showHideMoreApprovalResults: function () {
                    //查看更多审批记录
                    var self = this;
                    self.isLoadingAllLog = !self.isLoadingAllLog;

                },
                buttonClick: function (actionButtonItem) {
                    var self = this;
                    var isSubmit = true;
                    /*
                        actionButtonItem.ButtonType含义
                        0：保存，1：提交，7：撤回，9：作废
                        2：转发，5：加签，6：退回
                    */
                    switch (actionButtonItem.ButtonType) {
                        case 0:
                        case 1:
                        case 7:
                            break;
                        case 2://转发
                            isSubmit = false;
                            self._executeForward(actionButtonItem)
                            break;
                        case 5://加签
                            isSubmit = false;
                            self._executeAddNode(actionButtonItem)
                            break;
                        case 6://退回
                            if (VueWorkflowClient._otherSetting.CustomerSceneSetting.AlwaysReturnToStart) {
                                bpf_wf_data.BizContext.ExtensionCommond["RejectNode"] = "00000000-0000-0000-0000-000000000000";
                            }
                            else {
                                isSubmit = false;
                                self._executeReturn(actionButtonItem)
                            }
                            break;
                        case 9://作废
                            if (!confirm(self.lang.cancelConfirm)) {
                                isSubmit = false;
                            }
                            break;
                        default:
                            isSubmit = false;
                            bpf_wf_tool.alert(self.lang.notSupportButtonType);
                            break;
                    }
                    if (isSubmit) {
                        var isPass = self.checkSubmit(actionButtonItem);
                        if (!isPass) {
                            return false;
                        }
                        VueWorkflowClient._execute.execute(actionButtonItem);
                    }
                },
                _executeAddNode: function (actionButtonItem) {
                    //显示加签弹窗
                    var self = this;
                    var runningNode = self.runningNode;
                    if (runningNode.NodeType != 0 && runningNode.NodeType != 1) {
                        bpf_wf_tool.alert(self.lang.addNodeTypeError);
                        return;
                    }
                    self.showModal("addNode");
                    self.currentAddNodeInfo.currentUserName = runningNode.User.UserName;
                    self.currentAddNodeInfo.nodeName = self.currentAddNodeInfo.currentUserName + self.lang.addApprove;// "加签";
                    if (runningNode.NodeType == 0) {
                        self.currentAddNodeInfo.isHasBeforeAddNode = false;
                        //从发起节点开始顺序添加可以加签的节点
                        var cloneNodeArray = [];
                        $.each(self.nodeInstanceArray, function (i, item) {
                            if (!item.isNowAddNode && (item.NodeType == 1 || item.NodeType == 2 || item.NodeType == 0 || item.NodeType == 3 || item.NodeType == 7)) {
                                item.NodeNameAndUserFormateText = self.getNodeNameAndUserFormateText(item);
                                cloneNodeArray.push(item);
                            }
                        })
                        self.currentAddNodeInfo.cloneNodeArray = cloneNodeArray;
                        self.currentAddNodeInfo.cloneNode = cloneNodeArray[0];
                    }
                    else {
                        runningNode.NodeNameAndUserFormateText = self.getNodeNameAndUserFormateText(runningNode);
                        self.currentAddNodeInfo.cloneNodeArray = [runningNode];
                        self.currentAddNodeInfo.cloneNode = runningNode;
                    }
                },
                submitAddNode: function () {
                    var self = this;
                    var workflowContext = self.workflowContext;
                    var actionButtonItem = Enumerable.From(self.workflowContext.CurrentUserSceneSetting.ActionButtonList).SingleOrDefault(null, 'f=>f.ButtonType==5');
                    if (actionButtonItem != null) {
                        var runningNode = self.runningNode;
                        if (runningNode.NodeType != 0 && runningNode.NodeType != 1) {
                            bpf_wf_tool.alert(self.lang.addNodeTypeError);
                            return;
                        }
                        var cloneNodeListObj = null;
                        var selectNode = self.currentAddNodeInfo.cloneNode;
                        var cloneNode = self._getCloneNode(selectNode);
                        if (cloneNode == null) {
                            bpf_wf_tool.alert(self.lang.noCloneNode);
                            return;
                        }
                        var addNodeUserArray = self.currentAddNodeInfo.addNodeUserArray;
                        if (addNodeUserArray.length == 0) {
                            bpf_wf_tool.alert(self.lang.selectProcessUser);
                            return false;
                        }

                        var addNodeType = self.currentAddNodeInfo.isBeforeAndAfter;//加签类型（1：后加签，0：前加签）
                        var approvalContent = self.approveText;
                        if (addNodeType == 0) {
                            if (approvalContent == "") {
                                bpf_wf_tool.alert(self.lang.inputAddNodeApproveText);
                                return false;
                            }
                        }
                        var nodeNameCustomer = self.currentAddNodeInfo.nodeName;
                        if (nodeNameCustomer == "") {
                            bpf_wf_tool.alert(self.lang.inputAddNodeName);
                            return false;
                        }
                        var addNodeAuditType = self.currentAddNodeInfo.auditType;//审批类型（1：顺序审批，2：同时审批）
                        //var copyBizContextNodeInstanceList = $.extend(true, {}, bpf_wf_data.BizContext.NodeInstanceList);//拷贝一份节点数据，保证加签出错时，数据可以正常
                        if (addNodeUserArray.length < 2 && addNodeAuditType == 2) {
                            bpf_wf_tool.alert(self.lang.userMastExceedTwo);
                            return false;
                        }

                        if (addNodeAuditType == 1) {
                            cloneNodeListObj = bpf_wf_tool.initAddNodeOrderList(runningNode, cloneNode, nodeNameCustomer, addNodeUserArray);
                        }
                        else if (addNodeAuditType == 2) {
                            cloneNodeListObj = bpf_wf_tool.initAddNodeCosignerList(runningNode, cloneNode, nodeNameCustomer, addNodeUserArray);
                        }
                        else if (addNodeAuditType == 3) {
                            cloneNodeListObj = bpf_wf_tool.initAddNodeAutoInformList(runningNode, cloneNode, nodeNameCustomer, addNodeUserArray);
                        }
                        var addNodeList = cloneNodeListObj.AddNodeList;
                        var addNodeArray = cloneNodeListObj.AddNodeArray;
                        var firstNode = addNodeList[cloneNodeListObj.FirstNodeID];
                        var lastNode = addNodeList[cloneNodeListObj.LastNodeID];

                        if (addNodeType == 0) {//前加签
                            var nodeName = bpf_wf_data._getNodeNameAndUserFormateText(firstNode, addNodeArray);
                            if (confirm(self.getResourceWithReplacer(self.lang.submitConfirm, [nodeName]))) {
                                firstNode.PrevNodeID = runningNode.PrevNodeID;
                                workflowContext.NodeInstanceList[runningNode.PrevNodeID].NextNodeID = firstNode.NodeID;
                                lastNode.NextNodeID = runningNode.NodeID;
                                runningNode.PrevNodeID = lastNode.NodeID;
                                workflowContext.NodeInstanceList[runningNode.NodeID] = $.extend(true, {}, runningNode);//防止出现加签节点前后关联关闭不对
                                $.each(addNodeList, function (i, item) {
                                    workflowContext.NodeInstanceList[i] = item;
                                })

                                var isPass = self.checkSubmit(actionButtonItem, approvalContent);
                                if (!isPass) {
                                    return false;
                                }
                                VueWorkflowClient._execute.execute(actionButtonItem, function (success) {
                                    if (success) {
                                        self.hideShowModal("addNode");
                                    }
                                });
                            }
                            else {
                                return false;
                            }
                        }
                        else {
                            var nextNode = workflowContext.NodeInstanceList[selectNode.NextNodeID];
                            if (selectNode.NodeType == 0) {
                                while (nextNode != null && (nextNode.NodeType == 5 || nextNode.NodeType == 6)) {
                                    if (nextNode.NextNodeID != "") {
                                        nextNode = workflowContext.NodeInstanceList[nextNode.NextNodeID];
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
                                var preNode = workflowContext.NodeInstanceList[preNodeID]
                                firstNode.PrevNodeID = preNode.NodeID;
                                preNode.NextNodeID = firstNode.NodeID;
                            }
                            else {
                                firstNode.PrevNodeID = selectNode.NodeID;
                                selectNode.NextNodeID = firstNode.NodeID;
                            }

                            $.each(addNodeList, function (i, item) {
                                item.isCanDelete = true;
                                item.isNowAddNode = true;
                                workflowContext.NodeInstanceList[i] = item;
                            })
                            bpf_wf_data.BizContext.ExtensionCommond["AddAfterNode"] = "True";
                        }
                        self.currentAddNodeInfo.addNodeUserArray = [];
                        self.hideShowModal("addNode");
                    }
                },
                _getCloneNode: function (selectCloneNode) {
                    var runningNode;
                    if (selectCloneNode != undefined) {
                        runningNode = selectCloneNode;
                    }
                    else {
                        runningNode = self.runningNode;
                    }
                    var runningNodeType = runningNode.NodeType;
                    var cloneNode = null;
                    if (runningNodeType == 0) {
                        //发起节点使用下一节点来作为克隆节点
                        var nextNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[runningNode.NextNodeID];
                        while (nextNode != null && (nextNode.NodeType == 5 || nextNode.NodeType == 6)) {
                            if (nextNode.NextNodeID != "") {
                                nextNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[nextNode.NextNodeID];
                            }
                            else {
                                nextNode = null;
                            }
                        }
                        if (nextNode != null) {
                            cloneNode = $.extend(true, {}, nextNode);
                        }
                    }
                    else {
                        //其它节点使用当前节点作为克隆节点
                        cloneNode = $.extend(true, {}, runningNode);
                    }
                    return cloneNode;
                },
                getNodeNameAndUserFormateText: function (node) {
                    var self = this;
                    //返回的格式：发起人【XXX】，会签活动【XXX,XX】,
                    var result = "";
                    if (node.NodeType == 2 || node.NodeType == 3) {
                        result = result + node.NodeName + "【";
                        var itemNodeList = node.ChildNodeList
                        var len = itemNodeList.length;
                        $.each(itemNodeList, function (i, item) {
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
                showAddNodeUser: function (userInfo, index) {
                    //显示加签人员信息
                    var self = this;
                    self.showUserInfoUser({
                        userInfo: userInfo,
                        isHasRemove: true,
                        removeFunc: function () {
                            self.currentAddNodeInfo.addNodeUserArray.splice(index, 1);
                        }
                    });
                },
                selectAddNodeUser: function () {
                    //选择加签的人员
                    var self = this;
                    bpf_userselect_client.selectUser({
                        func: function (data) {
                            $.each(data, function (i, item) {
                                if (item.UserCode != bpf_wf_data.WorkFlowContext.CurrentUser.UserCode) {
                                    self.currentAddNodeInfo.addNodeUserArray.push(item);
                                }
                            })
                        },
                        exceptUserList: self.currentAddNodeInfo.addNodeUserArray,
                        topValue: otherSetting.UserSelectSetting.TopValue,
                        isNeedHiddenNav: otherSetting.UserSelectSetting.IsNeedHiddenNav
                    })
                },
                _executeForward: function (actionButtonItem) {
                    //弹出转发窗口
                    var self = this;
                    self.showModal("forward");
                },
                submitForward: function () {
                    //确认转发
                    var self = this;
                    var actionButtonItem = Enumerable.From(self.workflowContext.CurrentUserSceneSetting.ActionButtonList).SingleOrDefault(null, 'f=>f.ButtonType==2');
                    if (actionButtonItem != null) {
                        if (self.forwardUserArray.length == 0) {
                            bpf_wf_tool.alert(self.lang.selectRecieveUser);
                            return false;
                        }
                        var isPass = self.checkSubmit(actionButtonItem);
                        if (!isPass) {
                            return false;
                        }
                        bpf_wf_data.BizContext.ExtensionCommond["ForwardUser"] = JSON.stringify(self.forwardUserArray);
                        VueWorkflowClient._execute.execute(actionButtonItem, function (success) {
                            if (success) {
                                self.hideShowModal("forward");
                            }
                        });
                    }
                },
                selectForwardUser: function () {
                    //选择转发人员
                    var self = this;
                    bpf_userselect_client.selectUser({
                        func: function (data) {
                            $.each(data, function (i, item) {
                                self.forwardUserArray.push(item);
                            })
                        },
                        exceptUserList: self.forwardUserArray,
                        topValue: otherSetting.UserSelectSetting.TopValue,
                        isNeedHiddenNav: otherSetting.UserSelectSetting.IsNeedHiddenNav
                    })
                },
                showForwardUser: function (user, index) {
                    //显示转发人员信息
                    var self = this;
                    self.showUserInfoUser({
                        userInfo: user,
                        isHasRemove: true,
                        removeFunc: function () {
                            self.forwardUserArray.splice(index, 1);
                        }
                    });
                },
                _executeReturn: function (actionButtonItem) {
                    var self = this;
                    self.showModal("reject");
                    var workflowContext = self.workflowContext;
                    var beforeNodeList = [];
                    $.each(self.nodeInstanceArray, function (i, item) {
                        if (item.NodeID == workflowContext.ProcessInstance.RunningNodeID) {
                            return false;
                        }
                        else {
                            if (item.NodeType == 1 || item.NodeType == 2 || item.NodeType == 0 || item.NodeType == 7) {
                                item.NodeNameAndUserFormateText = self.getNodeNameAndUserFormateText(item);
                                beforeNodeList.push(item);
                            }
                        }
                    });
                    self.returnNodeArray = beforeNodeList;
                    self.returnNode = beforeNodeList[0];
                },
                submitReturn: function () {
                    var self = this;
                    var actionButtonItem = Enumerable.From(self.workflowContext.CurrentUserSceneSetting.ActionButtonList).SingleOrDefault(null, 'f=>f.ButtonType==6');
                    if (actionButtonItem != null) {
                        var rejectNode = self.returnNode;

                        var nodeNameAndUserFormateText = "";
                        if (rejectNode.NodeType == 0 || rejectNode.NodeType == 1 || rejectNode.NodeType == 7 || rejectNode.NodeType == 2) {
                            nodeNameAndUserFormateText = rejectNode.NodeNameAndUserFormateText;
                        }
                        else {
                            bpf_wf_tool.alert(self.lang.rejectNodeTypeError);
                            return false;
                        }
                        var msg = self.getResourceWithReplacer(self.lang.rejectConfirm, [nodeNameAndUserFormateText]);// "确定退回到“" +  nodeNameAndUserFormateText+ "”节点吗？";
                        if (confirm(msg)) {
                            var isPass = self.checkSubmit(actionButtonItem);
                            if (!isPass) {
                                return false;
                            }
                            bpf_wf_data.BizContext.ExtensionCommond["RejectNode"] = rejectNode.NodeID;
                            VueWorkflowClient._execute.execute(actionButtonItem, function (success) {
                                if (success) {
                                    self.hideShowModal("reject");
                                }
                            });
                            return true;
                        }
                        else {
                            return false;
                        }
                    }

                },
                checkSubmit: function (actionButtonItem) {
                    var self = this;
                    var msg = "";
                    if (actionButtonItem.ButtonType != 0 && actionButtonItem.ButtonType != 9) {
                        $.each(self.workflowContext.NodeInstanceList, function (i, item) {
                            if (item.NodeType == 1 && (item.User == null || (item.User.UserCode == "" && item.User.UserLoginID == ""))) {
                                //校验节点中的人是否选择
                                msg = msg + self.getResourceWithReplacer(self.lang.noCheckAuditUser, [item.NodeName]);// "节点【" +item.NodeName + "】未选择审批人\r";
                            }
                        })
                    }
                    if ($.trim(self.approveText) == "" && actionButtonItem.ButtonType != 0 && actionButtonItem.ButtonType != 7) {
                        msg = msg + self.lang.inputApproveText;// "请输入审批意见\r"
                    }

                    if (msg != "") {
                        bpf_wf_tool.alert(msg);
                        return false;
                    }
                    else {
                        return true;
                    }
                },
                formatDate: function (date) {
                    if (date.indexOf(".") >= 0) {
                        return date.replace("T", " ").substr(0, date.indexOf("."));
                    }
                    else {
                        return date.replace("T", " ");
                    }
                },
                showStrDate: function (datestr) {
                    if (datestr == "") return "";
                    var self = this;
                    var date = getDateTimeStamp(datestr);
                    return getDateDiff(date, self);
                    function getDateTimeStamp(dateStr) {
                        var dateTime = dateStr.split('T');
                        var dateBits = dateTime[0].split('-');
                        var timeBits = dateTime[1].split(':');
                        return new Date(dateBits[0], parseInt(dateBits[1]) - 1, dateBits[2], timeBits[0], timeBits[1], timeBits[2]).valueOf();
                        //return Date.parse(dateStr.replace(/-/gi, "/").replace("T", " "));
                    }
                    function getDateDiff(dateTimeStamp, self) {
                        //JavaScript函数：
                        var minute = 1000 * 60;
                        var hour = minute * 60;
                        var day = hour * 24;
                        var halfamonth = day * 15;
                        var month = day * 30;
                        var year = month * 12;
                        var now = new Date().getTime();
                        var diffValue = now - dateTimeStamp;
                        if (diffValue < 0) {
                            //若日期不符则弹出窗口告之
                            //alert("结束日期不能小于开始日期！");
                            return self.lang.just; //"刚刚";
                        }
                        var yearC = diffValue / year;
                        var monthC = diffValue / month;
                        var weekC = diffValue / (7 * day);
                        var dayC = diffValue / day;
                        var hourC = diffValue / hour;
                        var minC = diffValue / minute;
                        if (yearC >= 1) {
                            result = "" + parseInt(yearC) + self.lang.yearsBefore; //"年前";
                        }
                        else if (monthC >= 1) {
                            result = "" + parseInt(monthC) + self.lang.monthBefore; //"个月前";
                        }
                        else if (weekC >= 1) {
                            result = "" + parseInt(weekC) + self.lang.weekBefore; //"周前";
                        }
                        else if (dayC >= 1) {
                            result = "" + parseInt(dayC) + self.lang.dayBefore; //"天前";
                        }
                        else if (hourC >= 1) {
                            result = "" + parseInt(hourC) + self.lang.hourBefore; //"小时前";
                        }
                        else if (minC >= 1) {
                            result = "" + parseInt(minC) + self.lang.minuteBefore; //"分钟前";
                        } else
                            result = self.lang.just; //"刚刚";
                        return result;
                    }
                },
                getResourceWithReplacer: function (resource, replaceValueArray) {
                    //得到带占位符的资源，并且用传入的参数替换
                    /*           someStuff{0},{1}someStuff                  */
                    if (replaceValueArray != undefined && replaceValueArray != null && typeof replaceValueArray == "object" && resource.length > 0) {
                        for (var i = 0; i < replaceValueArray.length; i++) {
                            resource = resource.replace("{" + i + "}", replaceValueArray[i]);
                        }
                    }
                    return resource;
                }
            },
            watch: {
                "currentAddNodeInfo.cloneNode": function (val, oldVal) {
                    if (val != null && val != undefined) {
                        if (val == 3) {
                            this.currentAddNodeInfo.auditType = 3;
                        }
                        else {
                            this.currentAddNodeInfo.auditType = 1;
                        }
                    }
                },
                "currentAddNodeInfo.auditType": function (val, oldVal) {
                    if (val != null && val != undefined) {
                        var self = this;
                        if (val == 3) {
                            this.currentAddNodeInfo.nodeName = this.currentAddNodeInfo.currentUserName + self.lang.addAutoInform; // "加通知";
                        }
                        else if (val == 2) {
                            this.currentAddNodeInfo.nodeName = this.currentAddNodeInfo.currentUserName + self.lang.addCousign; //"加会签";
                        }
                        else {
                            this.currentAddNodeInfo.nodeName = this.currentAddNodeInfo.currentUserName + self.lang.addApprove; //"加签";
                        }
                    }
                },
                "currentAddNodeInfo.isBeforeAndAfter": function (val, oldVal) {
                    if (val != null && val != undefined) {
                        if (val == 0) {
                            this.currentAddNodeInfo.isShowAddAutoInform = false;
                            if (this.currentAddNodeInfo.auditType == 3) {
                                this.currentAddNodeInfo.auditType = 1;
                            }
                        }
                        else {
                            this.currentAddNodeInfo.isShowAddAutoInform = true;
                        }
                    }
                }
            }
        });
    },
    _content: function () {
        return ['<div class="inset_wrapper" v-cloak>',
            '            <!-- 审批流程 -->',
            '            <div class="bpf_workflow_approve_result">',
            '                <div class="bpf_workflow_approve_center">',
            '                    <a id="bpf_workflow_approve_top" href="#" class="bpf_workflow_approve_top" v-bind:style="{\'bottom\': actionButton.normalButtonList.length > 0 ? \'50px\': \'3px\'}"></a>',
            '                    <!--<div class="bpf_workflow_header">',
            '                        <h1 v-bind:style="{ \'padding-top\': 16 + topValue + \'px\'}">审批流程</h1>',
            '                    </div>-->',
            '                    <div class="bpf_workflow_approve_container" v-bind:style="{\'padding-bottom\': actionButton.normalButtonList.length > 0 ? \'50px\': \'0\' }">',
            '                        <ul class="bpf_workflow_result_list">',
            '                            <li v-bind:class="{\'bpf_workflow_result_active\': isExpandArea.nav}"  v-if="sceneSetting.ShowNavigationBar">',
            '                                <h3>',
            '                                    {{lang.navTitle}}',
            '                                    <span v-bind:class="[showNodeName?\'bpf_workflow_hide_default\':\'bpf_workflow_hide_active\']" v-on:click="showHideNodeName" v-if="processStatus==1"></span>',
            '                                    <!-- <span class="bpf_workflow_hide_active"></span> -->',
            '                                    <span class="bpf_workflow_updown_icon" v-on:click="foldArea(\'nav\')"></span>',
            '                                </h3>',
            '                                <div class="bpf_workflow_result_cont">',
            '                                    <ul class="bpf_workflow_approve_list">',
            '                                        <template v-for="(node,index) in nodeInstanceArray">',
            '                                            <li v-if="node.NodeType!=5 && node.NodeType!=6">',
            '                                                <div class="bpf_workflow_approve_node" v-bind:class="{\'bpf_workflow_ongoing\':(node.Status==1 && processStatus != -1),\'bpf_workflow_inform\': node.NodeType == 3}">',
            '                                                    <span class="bpf_workflow_approve_arrow"></span>',
            '                                                    <span v-show="showNodeName || node.NodeType == 0"  v-bind:style="{\'color\': (node.ExtendProperties.AllowChooseAnyUser !=\'True\'&& (node.NomineeList.length==0)&& isCanSelectUser && node.Status == 0 && (node.NodeType == 1 || node.NodeType == 7)) ? \'red\':\'#333\'}" >{{node.NodeName}}</span>',
            '                                                    <div class="bpf_workflow_tag" v-bind:class="{\'bpf_workflow_tag_complete\':(node.Status==2)}">',
            '                                                        <template v-if="node.NodeType == 0">',
            '                                                            <span v-if="processStatus!=0 && node.User != null" v-on:click="showNodeUser(node)"><span>【</span><span v-text="node.User.UserName"></span><span>】</span></span>',
            '                                                            <span class="bpf_workflow_tag_icon"></span>',
            '                                                        </template>',
            '                                                        <template v-if="(node.NodeType == 2 || node.NodeType == 3)">',
            '                                                            <span v-if="node.ChildNodeList!=null&& node.ChildNodeList.length >0">',
            '                                                                <span v-show="showNodeName">【</span>',
            '                                                                <template v-for="(childnode,childindex) in node.ChildNodeList">',
            '                                                                    <div class="bpf_workflow_tag" v-bind:class="{\'bpf_workflow_tag_complete\':(childnode.Status==2)}">',
            '                                                                        <span v-on:click="showNodeUser(childnode)">{{childnode.User.UserName}}</span>',
            '                                                                        <span class="bpf_workflow_tag_icon" v-show="node.Status!=2"></span>',
            '                                                                    </div>',
            '                                                                    <span v-if="childindex+1!=node.ChildNodeList.length">，</span>',
            '                                                                </template>',
            '                                                                <span v-show="showNodeName">】</span>',
            '                                                                <span class="bpf_workflow_tag_icon"></span>',
            '                                                            </span>',
            '                                                        </template>',
            '                                                        <template v-if="(node.NodeType == 1 || node.NodeType == 7)">',
            '                                                            <span v-if="isCanSelectUser && node.CloneNodeID ==\'\' && (node.ExtendProperties.AllowChooseAnyUser ==\'True\'|| (node.NomineeList.length>1))">',
            '                                                                <select class="bpf-wfclient-nav-select" v-on:change="selectNodeUser(node)" v-model="node.SelectUserCode">',
            '                                                                    <option value="">{{lang.selectOption}}</option>',
            '                                                                    <option v-for="(nomineuser,index) in node.NomineeList" v-if="index<=9" v-bind:value="nomineuser.UserCode">{{nomineuser.UserName}}</option>',
            '                                                                    <option value="..." v-if="node.ExtendProperties.AllowChooseAnyUser ==\'True\'">...</option>',
            '                                                                </select>',
            '                                                            </span>',
            '                                                            <span v-else>',
            '                                                                <span v-if="node.User != null" v-on:click="showNodeUser(node)"><span v-show="showNodeName">【</span><span v-text="node.User.UserName"></span><span v-show="showNodeName">】</span></span>',
            '                                                            </span>',
            '                                                            <span class="bpf_workflow_tag_icon"></span>',
            '                                                        </template>',
            '                                                    </div>',
            '                                                    <span v-if="node.isCanDelete" class="bpf_workflow_tag_del" v-on:click="deleteNode(node,index)"></span>',
            '                                                </div>',
            '                                            </li>',
            '                                        </template>',
            '                                    </ul>',
            '                                </div>',
            '                            </li>',
            '                            <li v-bind:class="{\'bpf_workflow_result_active\': isExpandArea.cc}" v-if="(sceneSetting.AllowNewCC || hasCcUser) && sceneSetting.ShowCCBar">',
            '                                <h3>{{lang.ccTitle}}<span class="bpf_workflow_updown_icon" v-on:click="foldArea(\'cc\')"></span></h3>',
            '                                <div class="bpf_workflow_result_cont">',
            '                                    <p class="bpf_workflow_text_link" v-show="sceneSetting.AllowNewCC && !hasCcUser">',
            '                                        <span class="bpf_workflow_blue" id="mcs_workflow_selectccuser" v-on:click="selectCcUser()">{{lang.addCcUser}}</span>',
            '                                    </p>',
            '                                    <div class="bpf_workflow_tag bpf_workflow_tag_bg" v-for="(ccnode,index) in workflowContext.CcNodeInstanceList"',
            '                                         v-bind:class="{\'bpf_workflow_tag_complete\':(ccnode.Status==2),\'bpf_workflow_tag_ongoing\':(ccnode.Status==1)}"',
            '                                         v-on:click="showCcUser(ccnode)">',
            '                                        <span>{{ccnode.User.UserName}}</span>',
            '                                        <span class="bpf_workflow_tag_icon"></span>',
            '                                    </div>',
            '                                    <span class="bpf_workflow_add_tag" v-on:click="selectCcUser()" v-show="sceneSetting.AllowNewCC && hasCcUser"></span>',
            '                                </div>',
            '                            </li>',
            '                            <li v-bind:class="{\'bpf_workflow_result_active\': isExpandArea.approveText}" v-if="sceneSetting.ShowApprovalTextArea">',
            '                                <h3>{{(processStatus==0||processStatus==2)?lang.remarkTitle:lang.approveTitle}}<span class="bpf_workflow_updown_icon" v-on:click="foldArea(\'approveText\')"></span></h3>',
            '                                <div class="bpf_workflow_result_cont">',
            '                                    <div class="bpf_workflow_quick_views" v-if="(processStatus!=0&&processStatus!=2)">',
            '                                        <p>',
            '                                            <span>{{lang.quickLog}}：</span>',
            '                                            <span class="bpf_workflow_approve_tag bpf_workflow_blue" v-on:click="quickLog(lang.approve)">{{lang.approve}}</span>',
            '                                            <span class="bpf_workflow_approve_tag bpf_workflow_blue" v-on:click="quickLog(lang.noApprove)">{{lang.noApprove}}</span>',
            '                                            <span class="bpf_workflow_approve_tag bpf_workflow_blue" v-on:click="quickLog(lang.receive)">{{lang.receive}}</span>',
            '                                        </p>',
            '                                    </div>',
            '                                    <div class="bpf_workflow_approve_views">',
            '                                        <textarea class="bpf_workflow_modal_textarea" name="approveViews" id="" cols="30" rows="10" v-model="approveText"></textarea>',
            '                                    </div>',
            '                                </div>',
            '                            </li>',
            '                            <li v-bind:class="{\'bpf_workflow_result_active\': isExpandArea.approveLog}" v-if="processLogList.length>0 &&processStatus!=0 && sceneSetting.ShowApprovalLog">',
            '                                <h3>{{lang.approveLogTitle}}<span class="bpf_workflow_updown_icon" v-on:click="foldArea(\'approveLog\')"></span></h3>',
            '                                <div class="bpf_workflow_result_cont bpf_workflow_opinions">',
            '                                    <ul>',
            '                                        <li v-for="(processitem,index) in processLogList" v-show="(index<3||isLoadingAllLog)&&processitem.OpertationName!=\'保存\'">',
            '                                            <div class="bpf_workflow_opinion_info">',
            '                                                <div class="bpf_workflow_opinion_name">',
            '                                                    <p class="bpf_workflow_opinion_t">',
            '                                                        {{processitem.User.UserName}}',
            '                                                    </p>',
            '                                                    <p class="bpf_workflow_time">{{showStrDate(processitem.FinishDateTime)}}</p>',
            '                                                    <span class="bpf_workflow_icon_default" v-bind:class="{\'bpf_workflow_icon_init\':(processitem.OpertationName == \'发起\')',
            '                                                          ,\'bpf_workflow_icon_approve\':(processitem.OpertationName == \'批准\')',
            '                                                          ,\'bpf_workflow_icon_back\':(processitem.OpertationName == \'退回\')',
            '                                                          ,\'bpf_workflow_icon_cancel\':(processitem.OpertationName == \'作废\')',
            '                                                          ,\'bpf_workflow_icon_posti\':(processitem.OpertationName == \'批注\')',
            '                                                          ,\'bpf_workflow_icon_operations\':(processitem.OpertationName == \'运维\')',
            '                                                          ,\'bpf_workflow_icon_save\':(processitem.OpertationName == \'保存\')',
            '                                                          ,\'bpf_workflow_icon_forwarding\':(processitem.OpertationName == \'转发\')',
            '                                                          ,\'bpf_workflow_icon_add_tags\':(processitem.OpertationName == \'加签\')',
            '                                                          ,\'bpf_workflow_icon_withdraw\':(processitem.OpertationName == \'撤回\')',
            '                                                          }"></span>',
            '                                                </div>',
            '                                                <div class="bpf_workflow_opinion_body">',
            '                                                    <div class="bpf_workflow_opinion_bg">',
            '                                                        <h4>{{processitem.NodeName}}</h4>',
            '                                                        <div class="bpf_workflow_opinion_cont">',
            '                                                            <p>{{processitem.LogContent}}</p>',
            '                                                        </div>',
            '                                                        <p class="bpf_workflow_date">{{formatDate(processitem.FinishDateTime)}}</p>',
            '                                                    </div>',
            '                                                </div>',
            '                                            </div>',
            '                                        </li>',
            '                                    </ul>',
            '                                    <div class="bpf_workflow_view_more" v-on:click="showHideMoreApprovalResults()" v-if="processLogList.length>3">',
            '                                        <p><span v-bind:class="{\'bpf_workflow_view_active\': isLoadingAllLog}">{{isLoadingAllLog ? lang.fold: lang.more}}</span></p>',
            '                                    </div>',
            '                                </div>',
            '                            </li>',
            '                        </ul>',
            '                    </div>',
            '                    <div class="bpf_workflow_approve_actions" v-if="actionButton.normalButtonList.length>0 && sceneSetting.ShowButtonBar" v-bind:class="{\'bpf_workflow_approve_actions_active\': isMoreActions}">',
            '                        <span class="bpf_workflow_btn bpf_workflow_modal_sure" v-for="(button,index) in actionButton.normalButtonList" v-bind:class="{\'bpf_workflow_modal_cancel\':(button.ButtonType==1)}" v-on:click="buttonClick(button)">{{button.ButtonDisplayName}}</span>',
            '                        <span class="bpf_workflow_more_action" v-on:click="showHideMoreActions()" v-if="actionButton.moreButtonList.length>0"></span>',
            '                        <div class="bpf_workflow_approve_more_bg" v-if="actionButton.moreButtonList.length>0">',
            '                            <span v-on:click="buttonClick(button)" v-for="(button,index) in actionButton.moreButtonList">{{button.ButtonDisplayName}}</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '            </div>',
            '            <!-- 审批流程 -->',
            '            <!-- 个人资料详情页modal -->',
            '            <div class="bpf_workflow_modal bpf_workflow_personal_modal" v-show="currentUserInfo.isShow">',
            '                <div class="bpf_workflow_modal_bg">',
            '                    <h2>{{currentUserInfo.userInfo.UserName}}（{{currentUserInfo.userInfo.UserLoginID}}）</h2>',
            '                    <span class="bpf_workflow_modal_close" v-on:click="hideUserInfo()"></span>',
            '                    <div class="bpf_workflow_modal_body">',
            '                        <ul class="bpf_workflow_modal_body_info">',
            '                            <li><span>{{lang.department}}：</span>{{currentUserInfo.userInfo.UserOrgPathName}}</li>',
            '                            <li><span>{{lang.job}}：</span>{{currentUserInfo.userInfo.UserJobName}}</li>',
            '                        </ul>',
            '                        <ul class="bpf_workflow_opinions" v-if="currentUserInfo.approveLogArray.length>0">',
            '                            <li v-for="(processitem,index) in currentUserInfo.approveLogArray">',
            '                                <div class="bpf_workflow_opinion_info">',
            '                                    <div class="bpf_workflow_opinion_name">',
            '                                        <p class="bpf_workflow_opinion_t">',
            '                                            {{processitem.User.UserName}}',
            '                                        </p>',
            '                                        <p class="bpf_workflow_time">{{showStrDate(processitem.FinishDateTime)}}</p>',
            '                                        <span class="bpf_workflow_icon_default" v-bind:class="{\'bpf_workflow_icon_init\':(processitem.OpertationName == \'发起\')',
            '                                                          ,\'bpf_workflow_icon_approve\':(processitem.OpertationName == \'批准\')',
            '                                                          ,\'bpf_workflow_icon_back\':(processitem.OpertationName == \'退回\')',
            '                                                          ,\'bpf_workflow_icon_cancel\':(processitem.OpertationName == \'作废\')',
            '                                                          ,\'bpf_workflow_icon_posti\':(processitem.OpertationName == \'批注\')',
            '                                                          ,\'bpf_workflow_icon_operations\':(processitem.OpertationName == \'运维\')',
            '                                                          ,\'bpf_workflow_icon_save\':(processitem.OpertationName == \'保存\')',
            '                                                          ,\'bpf_workflow_icon_forwarding\':(processitem.OpertationName == \'转发\')',
            '                                                          ,\'bpf_workflow_icon_add_tags\':(processitem.OpertationName == \'加签\')',
            '                                                          ,\'bpf_workflow_icon_withdraw\':(processitem.OpertationName == \'撤回\')',
            '                                                          }"></span>',
            '                                    </div>',
            '                                    <div class="bpf_workflow_opinion_body">',
            '                                        <div class="bpf_workflow_opinion_bg">',
            '                                            <h4>{{processitem.NodeName}}</h4>',
            '                                            <div class="bpf_workflow_opinion_cont">',
            '                                                <p>{{processitem.LogContent}}</p>',
            '                                            </div>',
            '                                            <p class="bpf_workflow_date">{{formatDate(processitem.FinishDateTime)}}</p>',
            '                                        </div>',
            '                                    </div>',
            '                                </div>',
            '                            </li>',
            '                        </ul>',
            '                        <div class="bpf_workflow_modal_actions">',
            '                            <span v-show="currentUserInfo.isHasRemove" class="bpf_workflow_btn_remove" v-on:click="removeUserInfo()">{{lang.remove}}</span>',
            '                            <span v-bind:style="{ width: currentUserInfo.isHasRemove ? \'40%\':\'94%\' }" class="bpf_workflow_btn_close" v-on:click="hideUserInfo()">{{lang.close}}</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '            </div>',
            '            <!-- 退回modal -->',
            '            <div class="bpf_workflow_modal" v-show="isShowModal.reject">',
            '                <div class="bpf_workflow_modal_bg">',
            '                    <h2>{{lang.reject}}</h2>',
            '                    <span class="bpf_workflow_modal_close" v-on:click="hideShowModal(\'reject\')"></span>',
            '                    <div class="bpf_workflow_modal_body">',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <label class="bpf_workflow_modal_label" for=""><span class="bpf_workflow_red">*</span>{{lang.rejectTo}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <select name="select_name" v-model="returnNode">',
            '                                    <option v-for="(node ,index) in  returnNodeArray" v-bind:value="node">{{node.NodeNameAndUserFormateText}}</option>',
            '                                </select>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <label class="bpf_workflow_modal_label" for=""><span class="bpf_workflow_red">*</span>{{lang.rejectApproveText}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <textarea class="bpf_workflow_modal_textarea" name="" id="" cols="30" rows="10" v-model="approveText"></textarea>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_btns">',
            '                            <span class="bpf_workflow_btn bpf_workflow_modal_sure" v-on:click="submitReturn">{{lang.ok}}</span>',
            '                            <span class="bpf_workflow_btn bpf_workflow_modal_cancel" v-on:click="hideShowModal(\'reject\')">{{lang.cancel}}</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '            </div>',
            '            <!-- 转发modal -->',
            '            <div class="bpf_workflow_modal" v-show="isShowModal.forward">',
            '                <div class="bpf_workflow_modal_bg">',
            '                    <h2>{{lang.forward}}</h2>',
            '                    <span class="bpf_workflow_modal_close" v-on:click="hideShowModal(\'forward\')"></span>',
            '                    <div class="bpf_workflow_modal_body">',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <div>',
            '                                <label class="bpf_workflow_modal_label" for=""><span class="bpf_workflow_red">*</span>{{lang.forwardUser}}</label>',
            '                            </div>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <p class="bpf_workflow_text_link" v-show="forwardUserArray.length == 0">',
            '                                    <span class="bpf_workflow_blue" v-on:click="selectForwardUser()">{{lang.addForwardUser}}</span>',
            '                                </p>',
            '                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" v-for="(user,index) in forwardUserArray" v-on:click="showForwardUser(user,index)">',
            '                                    <span>{{user.UserName}}</span>',
            '                                    <span class="bpf_workflow_tag_icon"></span>',
            '                                </div>',
            '                                <span class="bpf_workflow_add_tag" v-on:click="selectForwardUser()" v-show="forwardUserArray.length>0"></span>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <label class="bpf_workflow_modal_label" for=""><span class="bpf_workflow_red">*</span>{{lang.forwardApproveText}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <textarea class="bpf_workflow_modal_textarea" name="" id="" cols="30" rows="10" v-model="approveText"></textarea>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_btns">',
            '                            <span class="bpf_workflow_btn bpf_workflow_modal_sure" v-on:click="submitForward()">{{lang.ok}}</span>',
            '                            <span class="bpf_workflow_btn bpf_workflow_modal_cancel" v-on:click="hideShowModal(\'forward\')">{{lang.cancel}}</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '            </div>',
            '            <!-- 加签modal -->',
            '            <div class="bpf_workflow_modal" v-show="isShowModal.addNode">',
            '                <div class="bpf_workflow_modal_bg">',
            '                    <h2>{{lang.addNode}}</h2>',
            '                    <span class="bpf_workflow_modal_close" v-on:click="hideShowModal(\'addNode\')"></span>',
            '                    <div class="bpf_workflow_modal_body">',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <div>',
            '                                <label class="bpf_workflow_modal_label" for=""><span class="bpf_workflow_red">*</span>{{lang.addNode}}</label>',
            '                            </div>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <p class="bpf_workflow_text_link" v-on:click="selectAddNodeUser()" v-show="currentAddNodeInfo.addNodeUserArray.length==0">',
            '                                    <span class="bpf_workflow_blue">{{lang.addNodeUser}}</span>',
            '                                </p>',
            '                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" v-for="(userInfo,index) in currentAddNodeInfo.addNodeUserArray" v-on:click="showAddNodeUser(userInfo,index)">',
            '                                    <span>{{userInfo.UserName}}</span>',
            '                                    <span class="bpf_workflow_tag_icon"></span>',
            '                                </div>',
            '                                <span class="bpf_workflow_add_tag" v-on:click="selectAddNodeUser()" v-show="currentAddNodeInfo.addNodeUserArray.length>0"></span>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <label class="bpf_workflow_modal_label" for="currentAddNodeInfo.cloneNode">{{lang.node}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <select class="bpf_workflow_modal_select" v-model="currentAddNodeInfo.cloneNode">',
            '                                    <option v-for="(node,index) in currentAddNodeInfo.cloneNodeArray" v-bind:value="node">{{node.NodeNameAndUserFormateText}}</option>',
            '                                </select>',
            '                                <input type="radio" id="bpf_workflow_after" name="radio_input" value="1" v-model="currentAddNodeInfo.isBeforeAndAfter" />',
            '                                <label for="bpf_workflow_after" class="bpf_workflow_radio">',
            '                                    {{lang.next}}',
            '                                </label>',
            '                                <input type="radio" id="bpf_workflow_before" name="radio_input" value="0" v-model="currentAddNodeInfo.isBeforeAndAfter" v-show="currentAddNodeInfo.isHasBeforeAddNode" />',
            '                                <label for="bpf_workflow_before" class="bpf_workflow_radio" v-show="currentAddNodeInfo.isHasBeforeAddNode">',
            '                                    {{lang.pre}}',
            '                                </label>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <label class="bpf_workflow_modal_label" for="">{{lang.auditType}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <input type="radio" id="bpf_workflow_order" name="radio_type" value="1" v-model="currentAddNodeInfo.auditType" />',
            '                                <label for="bpf_workflow_order" class="bpf_workflow_radio">',
            '                                    {{lang.approveAuditType}}',
            '                                </label>',
            '                                <input type="radio" id="bpf_workflow_both" name="radio_type" value="2" v-model="currentAddNodeInfo.auditType" />',
            '                                <label for="bpf_workflow_both" class="bpf_workflow_radio">',
            '                                    {{lang.cousignAuditType}}',
            '                                </label>',
            '                                <input type="radio" id="bpf_workflow_tz" name="radio_type" value="3" v-model="currentAddNodeInfo.auditType" v-show="currentAddNodeInfo.isShowAddAutoInform" />',
            '                                <label for="bpf_workflow_tz" class="bpf_workflow_radio" v-show="currentAddNodeInfo.isShowAddAutoInform">',
            '                                    {{lang.autoInformAuditType}}',
            '                                </label>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_form">',
            '                            <label class="bpf_workflow_modal_label" for="">{{lang.addNodeName}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <p class="bpf_workflow_modal_nodename">',
            '                                    <input type="text" placeholder="" name="tagName" v-model="currentAddNodeInfo.nodeName">',
            '                                </p>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_form" v-show="currentAddNodeInfo.isBeforeAndAfter==0">',
            '                            <label class="bpf_workflow_modal_label" for=""><span class="bpf_workflow_red">*</span>{{lang.addNodeApproveText}}</label>',
            '                            <div class="bpf_workflow_received_list">',
            '                                <textarea class="bpf_workflow_modal_textarea bpf_workflow_add_tags" name="" id="" cols="30" rows="10" v-model="approveText"></textarea>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_workflow_modal_btns">',
            '                            <span class="bpf_workflow_btn bpf_workflow_modal_sure" v-on:click="submitAddNode()">{{lang.ok}}</span>',
            '                            <span class="bpf_workflow_btn bpf_workflow_modal_cancel" v-on:click="hideShowModal(\'addNode\')">{{lang.cancel}}</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '            </div>',
            '        </div>'].join("");
    }
}