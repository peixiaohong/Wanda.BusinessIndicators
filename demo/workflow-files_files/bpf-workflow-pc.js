var PCWorkflowClient;
var BPF_Workflow_Tool;

var bpf_workflow_tool = {
    buildCommonData: function () {
        if (bpf_wf_data.WorkFlowContext != null) {
            //构建抄送节点信息
            bpf_wf_data.BizContext.CcNodeInstanceList = bpf_wf_data.WorkFlowContext.CcNodeInstanceList;
            $.each(bpf_wf_data.ClientContext.CcUserList, function (i, item) {
                var nodeID = uuid.v4();
                bpf_wf_data.BizContext.CcNodeInstanceList[nodeID] = bpf_wf_data._buildCcNode(item, nodeID);
            });
            bpf_wf_data.ClientContext.CcUserList = [];//20170717修改提交完之后就清空数组，防止出现抄送人翻倍
        }
        if (bpf_wf_data.BizContext.CcNodeInstanceList == {
        }) {
            bpf_wf_data.BizContext.CcNodeInstanceList = null;
        }
        if (bpf_wf_data.BizContext.NodeInstanceList == {
        }) {
            bpf_wf_data.BizContext.NodeInstanceList = null;
        }
    },
    initProcessControl: function (domID, workflowContext, otherSetting, client) {
        PCWorkflowClient = client;
        BPF_Workflow_Tool = bpf_workflow_tool;
        var scene = workflowContext.CurrentUserSceneSetting;
        var div = $(domID);
        //加载整体Dom内容
        div.html(BPF_Workflow_Tool.template.content());
        if (scene.ShowNavigationBar) {
            //加载导航
            this._initItemControl.initNav("#bpf-wfclient-nav-content");
        }
        else {
            $("#bpf-wfclient-nav-content").closest("tr").hide();
        }
        if (scene.ShowCCBar) {
            //加载抄送
            this._initItemControl.initCc("#bpf-wfclient-cc-content");
        }
        else {
            $("#bpf-wfclient-cc-content").closest("tr").hide();
        }

        if (scene.ShowApprovalTextArea) {
            //绑定事件,快捷意见
            $(".bpf-wfclient-log-quicklog").click(function () {
                var currentLogDom = $("#bpf-wfclient-log-textarea");
                var currentLogValue = $.trim(currentLogDom.val());
                if (currentLogValue != "") {
                    if (currentLogValue != "同意" && currentLogValue != "不同意" && currentLogValue != "收到") {
                        if (!confirm("此操作将清空现在输入的审批意见，是否确认？")) {
                            return;
                        }
                    }
                }
                currentLogValue = $(this).html();
                currentLogDom.val(currentLogValue)
            })
            if (bpf_wf_data.WorkFlowContext.ProcessInstance != null) {
                var status = bpf_wf_data.WorkFlowContext.ProcessInstance.Status;
                if (status == 0 || status == 2) {
                    $(".bpf-wfclient-log-quicklog").closest("tr").hide();
                    $("#bpf-wfclient-log-title").html("相关说明");
                }
            }
        }
        else {
            $(".bpf-wfclient-log-quicklog").closest("tr").hide();
            $("#bpf-wfclient-log-textarea").closest("tr").hide();
        }
        if (scene.ShowApprovalLog) {  //加载审批日志
            this._initItemControl.initProcessLog("#bpf-wfclient-log-content");
        }
        else {
            $("#bpf-wfclient-log-content").closest("tr").hide();
        }

        switch (bpf_wf_client._otherSetting.ButtonCssType) {
            case "middle":
                $("#bpf-wfclient-button-content").addClass("bpf-wfclient-button-middle");
                $("#bpf-wfclient-log-content").css("margin-bottom", 0);
                break;
            default:
                $("#bpf-wfclient-button-content").addClass("bpf-wfclient-button-default");
                break;
        }

        //加载按钮
        this._initItemControl.initButton("#bpf-wfclient-button-content");
    },
    _initItemControl: {
        initProcessLog: function (logDomID) {
            //加载审批日志
            $(logDomID).html(BPF_Workflow_Tool.template.opinion());
            var table = $("#bpf-wfclient-log-logtable");
            var logDataList = [];
            if (PCWorkflowClient._otherSetting.CustomerProcessLog != null) {
                logDataList = PCWorkflowClient._otherSetting.CustomerProcessLog;
            }
            else {
                //倒序排列审批日志
                bpf_wf_data.WorkFlowContext.ProcessLogList = Enumerable.From(bpf_wf_data.WorkFlowContext.ProcessLogList).OrderByDescending("f=>f.FinishDateTime").ToArray();
                logDataList = bpf_wf_data.WorkFlowContext.ProcessLogList;
            }
            $.each(logDataList, function (i, item) {
                if (item.OpertationName != "保存") {
                    var tr = $("<tr>");
                    tr.append($("<td>").append($("<div>").text(item.NodeName)));
                    tr.append($("<td>", { style: "text-align:left" }).append($("<span>").html("<pre>" + item.LogContent + "</pre>")));
                    var userName = "";
                    if (bpf_wf_client._otherSetting.CustomerSceneSetting.ShowUserOrgPathName) {
                        userName = userName + item.User.UserOrgPathName + "<br/>";
                    }
                    if (bpf_wf_client._otherSetting.CustomerSceneSetting.ShowUserJobName) {
                        userName = userName + item.User.UserJobName + "<br/>";
                    }
                    userName = userName + item.User.UserName;
                    tr.append($("<td>", { style: "text-align:left" }).append($("<span>", { style: "line-height:20px;" }).html(userName).attr("title", item.User.UserName + "(" + item.User.UserLoginID + ")")));
                    tr.append($("<td>").append($("<div>").html(bpf_wf_tool.formatDate(item.FinishDateTime))));
                    tr.append($("<td>").append($("<div>").text(item.OpertationName)));
                    if (i > 4) {
                        tr.hide();
                    }
                    table.append(tr);
                }
                else {
                    //草稿状态下，将审批日志中的内容写入到审批意见框中
                    if (bpf_wf_data.WorkFlowContext.ProcessInstance.Status == 0) {
                        $("#bpf-wfclient-log-textarea").val(item.LogContent);
                    }
                }
            });
            if (logDataList.length > 5) {
                //隐藏/显示审批日志
                var showmoretr = $("#bpf-wfclient-log-showmore");
                showmoretr.show();
                showmoretr.find("span").click(function () {
                    var obj = $(this);
                    var img = obj.find("img");
                    var a = obj.find("a");
                    if (a.text() == "显示更多") {
                        a.html("收起");
                        img.attr("src", bpf_wf_client._constSetting.upImgSrc());
                        table.find("tr").css("display", "");
                    }
                    else {
                        a.html("显示更多");
                        img.attr("src", bpf_wf_client._constSetting.downImgSrc());
                        table.find("tr:gt(5)").hide();
                    }
                });
            }
        },
        initNav: function (navDomID) {
            var navDiv = $("<div>", { id: "bpf-wfclient-nav-div" });
            $(navDomID).append(navDiv);

            var nodeDataArray = bpf_wf_data.OtherContext.NodeInstanceArray;
            var startNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[bpf_wf_data.WorkFlowContext.ProcessInstance.StartNodeID];
            if (startNode == null) {
                startNode = bpf_wf_data.OtherContext.StartNode;
            }
            var isCanSelectUser = bpf_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID == bpf_wf_data.WorkFlowContext.ProcessInstance.StartNodeID && bpf_wf_data.WorkFlowContext.CurrentUserNodeID == bpf_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID;
            if (bpf_wf_data.WorkFlowContext.ExtensionInfos != null && bpf_wf_data.WorkFlowContext.ExtensionInfos.RefreshProcess == "True") {
                isCanSelectUser = true;
            }
            var addNodeIDList = [];
            if (bpf_wf_data.WorkFlowContext.ExtensionInfos != null && bpf_wf_data.WorkFlowContext.ExtensionInfos.AddNodeIDList != null) {
                addNodeIDList = JSON.parse(bpf_wf_data.WorkFlowContext.ExtensionInfos.AddNodeIDList);
            }
            var node = startNode;
            while (node != null) {
                if (node.NodeType != 5 && node.NodeType != 6) {
                    var spDom = BPF_Workflow_Tool._initItemControl.initNavItem(node, isCanSelectUser, nodeDataArray);
                    var nodeIsAdd = $.inArray(node.NodeID, addNodeIDList) >= 0;
                    if (isCanSelectUser && (bpf_wf_client._otherSetting.IsCanDeleteAddNode || nodeIsAdd) && node.CloneNodeID != "" && node.CreateUser != null && node.CreateUser.UserCode == bpf_wf_data.WorkFlowContext.CurrentUser.UserCode) {
                        var spDelNav = $("<span>", { "class": "bpf-wfclient-deletenav", "nodeid": node.NodeID }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                        spDelNav.click(function () {
                            var nodeID = $(this).attr("nodeid");
                            var nodeDelete = bpf_wf_data.BizContext.NodeInstanceList[nodeID];
                            if (nodeDelete.NextNodeID == "") {
                                bpf_wf_data.BizContext.NodeInstanceList[nodeDelete.PrevNodeID].NextNodeID = "";
                                $(this).parent().prev().remove();
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
                            $(this).parent().next().remove();
                            $(this).parent().remove();
                        })
                        navDiv.append(spDom.append(spDelNav)).append(BPF_Workflow_Tool._domBuild.buildNavArrowSpDom());
                    }
                    else {
                        navDiv.append(spDom).append(BPF_Workflow_Tool._domBuild.buildNavArrowSpDom());
                    }
                }
                var nextNodeID = node.NextNodeID;
                if (nextNodeID != "") {
                    node = bpf_wf_data.WorkFlowContext.NodeInstanceList[nextNodeID];
                }
                else {
                    node = null;
                }
            }
            navDiv.find(".bpf-wfclient-nav-sparrrow:last").hide();
        },
        initNavItem: function (node, isCanSelectUser, nodeDataArray, isAddNode) {
            var spDom = $("<span>", { "class": "bpf-wfclient-nav-item " + (node.NodeType == 3 ? "bpf-wfclient-nav-node-autoinform" : "") });
            var spNode = $("<span>").text(node.NodeName).attr("title", bpf_wf_tool.formateNodeTips(node));
            if (!bpf_wf_client._otherSetting.ShowNodeName && node.NodeType != 0) {
                spNode.css("display", "none");
            }
            spDom.append(spNode);
            if (node.Status == 1 && bpf_wf_data.WorkFlowContext.ProcessInstance.Status != -1) {
                //正在执行中
                spDom.css("font-weight", 600);
            }
            if (node.NodeType == 2 || node.NodeType == 3) {
                var itemNodeList = Enumerable.From(nodeDataArray).Where("f=>f.ParentNodeID == '" + node.NodeID + "'").OrderBy("f=>f.NodeOrder").ToArray();
                var len = itemNodeList.length;
                //if (len == 1) {
                //    spDom.append($("<span>").text("【" + itemNodeList[0].User.UserName + "】").attr("title", bpf_wf_tool.formateUserTips(itemNodeList[0].User)));
                //}
                //else if (len > 1) {
                spDom.append($("<span>", { "class": "bpf-wfclient-nav-item-start" }).text("【"));
                var isAllProcess = node.Status == 2;
                $.each(itemNodeList, function (i, item) {
                    spDom.append($("<span>", {}).text(item.User.UserName).attr("title", bpf_wf_tool.formateUserTips(item.User)));
                    if (item.Status == 2 && !isAllProcess) {
                        spDom.append(BPF_Workflow_Tool._domBuild.buildNavOkImgDom());
                    }
                    if (len != (i + 1)) {
                        spDom.append($("<span>").text("，"));
                    }
                });
                spDom.append($("<span>", { "class": "bpf-wfclient-nav-item-end" }).text("】"));
                //}
                if (isCanSelectUser && node.Status == 0 && node.CloneNodeID == "") {
                    var allowChooseAnyUser = node.ExtendProperties.AllowChooseAnyUser == "True";
                    //增加全局选人配置
                    if (allowChooseAnyUser) {
                        var aSelectAnyUser = $("<a>", { "class": "bpf-wfclient-nav-item-selectuser" }).text("请选择");
                        spDom.append(aSelectAnyUser);

                        aSelectAnyUser.click(function () {
                            var itemNodeCurrentList = Enumerable.From(bpf_wf_data.BizContext.NodeInstanceList).Where("f=>f.Value.ParentNodeID == '" + node.NodeID + "'");
                            var itemNodeUserList = itemNodeCurrentList.Select("g=>g.Value.User").ToArray();
                            var maxNodeOrder = 0;
                            if (itemNodeUserList.length > 0) {
                                maxNodeOrder = itemNodeCurrentList.Max("f=>f.Value.NodeOrder");
                            }

                            bpf_userselect_client.selectUser({
                                func: function (data) {
                                    if (data.length > 0) {
                                        $.each(data, function (i, item) {
                                            var nodeTempID = uuid.v4();
                                            var nodeTemp = JQZepto.extend(true, {}, node);
                                            nodeTemp.ParentNodeID = node.NodeID;
                                            nodeTemp.User = item;
                                            nodeTemp.NodeID = nodeTempID;
                                            nodeTemp.NodeOrder = maxNodeOrder++;
                                            nodeTemp.NomineeList = null;
                                            nodeTemp.Status = 0;
                                            nodeTemp.PrevNodeID = "";
                                            nodeTemp.NextNodeID = "";
                                            nodeTemp.StartDateTime = "0001-01-01T00:00:00";
                                            nodeTemp.FinishDateTime = "0001-01-01T00:00:00";
                                            bpf_wf_data.BizContext.NodeInstanceList[nodeTempID] = nodeTemp;

                                            var preEndSpan = aSelectAnyUser.prev();
                                            if (itemNodeUserList.length > 0) {
                                                var preFix = $("<span>").text("，");
                                                preFix.insertBefore(preEndSpan);
                                            }
                                            else {
                                                if (i > 0) {
                                                    var preFix = $("<span>").text("，");
                                                    preFix.insertBefore(preEndSpan);
                                                }
                                            }
                                            $("<span>", {}).text(item.UserName).attr("title", bpf_wf_tool.formateUserTips(item)).insertBefore(preEndSpan);
                                            var spDelNav = $("<span>", { "class": "bpf-wfclient-deletenav", "nodeid": nodeTempID }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                                            spDelNav.insertBefore(preEndSpan);

                                            spDelNav.click(function () {
                                                var nodeID = $(this).attr("nodeid");
                                                var nodeDelete = bpf_wf_data.BizContext.NodeInstanceList[nodeID];
                                                delete bpf_wf_data.BizContext.NodeInstanceList[nodeID];
                                                $(this).prev().remove();//移除人员Span                                                
                                                if ($(this).prev(".bpf-wfclient-nav-item-start").length < 1) {
                                                    $(this).prev().remove();//移除前面的,分隔符
                                                }
                                                else {
                                                    if ($(this).next(".bpf-wfclient-nav-item-end").length < 1) {
                                                        $(this).next().remove();//移除后面的,分隔符
                                                    }
                                                }
                                                $(this).remove();
                                            })
                                        })
                                    }
                                },
                                appCode: bpf_wf_data.BizContext.AppCode,
                                allowMulti: true,
                                allowAll: allowChooseAnyUser,
                                exceptUserList: itemNodeUserList
                            });
                        })
                    }
                }
            }
            else if (node.NodeType == 0) {
                if (bpf_wf_data.WorkFlowContext.ProcessInstance.Status != 0) {
                    if (node.User != null) {
                        spDom.append($("<span>").text("【" + node.User.UserName + "】").attr("title", bpf_wf_tool.formateUserTips(node.User)));
                    }
                }
            }
            else {
                if (isCanSelectUser && node.Status == 0 && node.CloneNodeID == "") {
                    var allowChooseAnyUser = node.ExtendProperties.AllowChooseAnyUser == "True";
                    var nomineeUser = [];
                    if (node.NomineeList != null && node.NomineeList.length > 0) {
                        nomineeUser = node.NomineeList;
                    }
                    else {
                        //如果只存在一个候选用户时，则取node.User。
                        if (node.User != null) {
                            nomineeUser.push(node.User);
                        }
                    }
                    if (nomineeUser.length == 0 && !allowChooseAnyUser) {
                        spDom.css("color", "red");
                    }
                    var nomineeUserLen = nomineeUser.length;
                    if (nomineeUserLen == 1 && !allowChooseAnyUser) {
                        spDom.append($("<span>").text("【" + nomineeUser[0].UserName + "】").attr("title", bpf_wf_tool.formateUserTips(nomineeUser[0])));
                        bpf_wf_data.BizContext.NodeInstanceList[node.NodeID].User = nomineeUser[0];
                    }
                    else {
                        var selectPerson = $("<select>", { "class": "bpf-wfclient-nav-select", id: node.NodeID });
                        selectPerson.append('<option value="">请选择</option>');
                        var isHasCurrentUser = false;
                        $.each(nomineeUser, function (i, info) {
                            if (i > 9) {
                                return false;
                            }
                            if (node.User != null && info.UserCode == node.User.UserCode) {
                                isHasCurrentUser = true;
                            }
                            selectPerson.append('<option value="' + info.UserCode + '" title="' + bpf_wf_tool.formateUserTips(info) + '">' + info.UserName + '</option>');
                        });
                        if (node.User != null) {
                            if (!isHasCurrentUser) {
                                selectPerson.append('<option value="' + node.User.UserCode + '" title="' + bpf_wf_tool.formateUserTips(node.User) + '">' + node.User.UserName + '</option>');
                            }
                            selectPerson.val(node.User.UserCode);
                        }

                        if (allowChooseAnyUser || nomineeUserLen > 10) {
                            selectPerson.append('<option value="...">...</option>');
                        }
                        spDom.append(selectPerson);
                        selectPerson.change(function () {
                            var nodeID = $(this).attr("ID");
                            var userCode = $(this).val();
                            var nodeSelect = bpf_wf_data.BizContext.NodeInstanceList[node.NodeID];

                            if (userCode == "") {
                                nodeSelect.User = null;
                                $(this).attr("title", "");
                            }
                            else if (userCode == "...") {
                                var allowChooseAnyUserSelect = node.ExtendProperties["AllowChooseAnyUser"] == "True";
                                var checkedUser = nodeSelect.User == null ? [] : [nodeSelect.User];
                                bpf_userselect_client.selectUser({
                                    func: function (data) {
                                        if (data.length == 1) {
                                            var user = data[0];
                                            if (selectPerson.find("option[value='" + user.UserCode + "']").length > 0) {
                                                selectPerson.val(user.UserCode);
                                            }
                                            else {
                                                selectPerson.find("option:last").before('<option value="' + user.UserCode + '" title="' + bpf_wf_tool.formateUserTips(user) + '">' + user.UserName + '</option>');
                                                selectPerson.val(user.UserCode);
                                                if (nodeSelect.NomineeList == null) {
                                                    nodeSelect.NomineeList = [];
                                                }
                                                nodeSelect.NomineeList.push(user);
                                            }
                                            nodeSelect.User = user;
                                        }
                                        else {
                                            selectPerson.val("");
                                        }
                                    },
                                    exit: function () {
                                        selectPerson.val("");
                                    },
                                    appCode: bpf_wf_data.BizContext.AppCode,
                                    allowMulti: false,
                                    allowAll: allowChooseAnyUserSelect,
                                    waitUserList: nodeSelect.NomineeList,
                                    checkedUserList: checkedUser
                                });
                            }
                            else {
                                var nodeNomineeList = bpf_wf_data.BizContext.NodeInstanceList[nodeID].NomineeList;
                                var userInfo = Enumerable.From(nodeNomineeList).First("f=>f.UserCode == '" + userCode + "'");
                                $(this).attr("title", bpf_wf_tool.formateUserTips(userInfo));
                                bpf_wf_data.BizContext.NodeInstanceList[node.NodeID].User = userInfo;
                            }
                        })
                    }
                }
                else {
                    if (node.User != null) {
                        spDom.append($("<span>").text("【" + node.User.UserName + "】").attr("title", bpf_wf_tool.formateUserTips(node.User)));
                    }
                }
            }
            if (node.Status == 2) {
                spDom.append(BPF_Workflow_Tool._domBuild.buildNavOkImgDom());
            }
            var nodeID = node.NodeID;
            bpf_wf_data.OtherContext.NavNodeDom[nodeID] = spDom;
            return spDom;
        },
        initCc: function (ccDomID) {
            var ccControl = $(ccDomID).html("");
            var ccDataList = bpf_wf_data.WorkFlowContext.CcNodeInstanceList;
            var runningNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[bpf_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID]
            var isShowDel = bpf_wf_data.WorkFlowContext.CurrentUserSceneSetting.AllowNewCC;
            //if (runningNode != null) {
            //    isShowDel = runningNode.NodeType == 0 && bpf_wf_data.WorkFlowContext.CurrentUserNodeID == runningNode.NodeID;
            //}

            $.each(ccDataList, function (i, item) {
                var span = $("<span>", { "class": "bpf-wfclient-cc-item" });
                var spDel = $("<span>", { "class": "bpf-wfclient-cc-deleteuser" }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                span.append($("<span>").text(item.User.UserName).attr("title", bpf_wf_tool.formateUserTips(item.User)));
                if (isShowDel) {
                    span.append(spDel);
                    spDel.click(function () { $(this).parent().remove(); delete bpf_wf_data.WorkFlowContext.CcNodeInstanceList[i] });
                }
                if (item.Status == 1) {
                    if (bpf_wf_data.WorkFlowContext.ProcessInstance.Status != -1) {
                        span.css("font-weight", 600);
                    }
                }
                else if (item.Status == 2) {
                    var ok = $("<img>", { alt: "", src: bpf_wf_client._constSetting.okImgSrc() });
                    span.append(ok);
                }
                span.append($("<span>").text("；"));
                ccControl.append(span);
            });
            var allowNewCc = bpf_wf_data.WorkFlowContext.CurrentUserSceneSetting.AllowNewCC;
            if (allowNewCc) {
                var aselect = $("<a>", { "class": "bpf-wfclient-cc-selectuser", href: "javascript:void(0)" }).text("请选择");

                aselect.click(function () {
                    bpf_userselect_client.selectUser({
                        func: function (data) {
                            $.each(data, function (i, item) {
                                bpf_wf_data.ClientContext.CcUserList.push(item);
                                var span = $("<span>", { "class": "bpf-wfclient-cc-item" });
                                var spDel = $("<span>", { "class": "bpf-wfclient-cc-deleteuser" }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                                span.append($("<span>").text(item.UserName).attr("title", bpf_wf_tool.formateUserTips(item)));
                                if (isShowDel) {
                                    span.append(spDel);
                                    spDel.click(function () { $(this).parent().remove(); bpf_wf_tool.deleteArrayItem(bpf_wf_data.ClientContext.CcUserList, item); });
                                }
                                span.append($("<span>").text("；"));
                                aselect.before(span);
                            })
                        },
                        appCode: bpf_wf_data.BizContext.AppCode,
                        exceptUserList: bpf_wf_data.ClientContext.CcUserList
                    });
                });
                ccControl.append(aselect);
            }
        },
        initButton: function (btnDomID) {
            $(btnDomID).html(BPF_Workflow_Tool.template.button());
            var div = $("#bpf-wfclient-button-div");
            var buttonDataList = bpf_wf_data.WorkFlowContext.CurrentUserSceneSetting.ActionButtonList;
            //获取语言设置
            var language = bpf_wf_client._otherSetting.CustomerSceneSetting.Language;

            var divContextMenu = $("<div>", { "class": "contextMenu", id: "divContextMenu", "style": "display:none;" });
            var ul = $("<ul>");
            divContextMenu.append(ul);
            var divContainer = $(bpf_wf_client._otherSetting.ContainerDomID);
            divContainer.append(divContextMenu);

            $.each(buttonDataList, function (i, item) {
                var buttonDefine = {};
                if (bpf_wf_tool.isJson(item.ButtonName)) {
                    buttonDefine = JSON.parse(item.ButtonName);//转换按钮信息为JSON对象
                }
                var buttonDisplayName = buttonDefine[language] == undefined ? item.ButtonDisplayName : buttonDefine[language]
                var liItem = $("<li>").text(buttonDisplayName);
                liItem.click(function () { BPF_Workflow_Tool._initItemControl.initButtonEvent($(this), item); });
                ul.append(liItem);

                var btnItem = $("<div>", { "class": "bpf-wfclient-button-btn", "optype": item.ButtonType });
                if (item.ButtonType == 1) {
                    btnItem.addClass("bpf-wfclient-button-submit");
                }
                else {
                    btnItem.addClass("bpf-wfclient-button-save");
                }
                btnItem.click(function () { BPF_Workflow_Tool._initItemControl.initButtonEvent($(this), item); });
                btnItem.text(buttonDisplayName);
                div.append(btnItem);
            });
            var isHasCustomerButtons = false;
            var customerButtons = bpf_wf_client._otherSetting.CustomerSceneSetting.CustomerButtons;
            if (bpf_wf_data.WorkFlowContext.CurrentUserHasTodoTask || bpf_wf_data.WorkFlowContext.ProcessInstance.Status == 0) {
                if (customerButtons.length > 0) {
                    isHasCustomerButtons = true;
                }
                $.each(customerButtons, function (i, item) {
                    var liItem = $("<li>").text(item.Name);
                    liItem.click(function () { item.OnClick() });
                    ul.append(liItem);

                    var btnItem = $("<div>", { "class": "bpf-wfclient-button-btn bpf-wfclient-button-save " + item.CssClass });
                    btnItem.click(function () { item.OnClick() });
                    btnItem.text(item.Name);
                    div.append(btnItem);
                })
            }

            if (buttonDataList.length == 0 && isHasCustomerButtons == false) {
                $(btnDomID).hide();
            }
            else {
                if (bpf_wf_client._otherSetting.IsShowContextMenu) {
                    if (bpf_wf_client._otherSetting.PageContextMenu) {
                        $("body").contextMenu('divContextMenu', {
                        });
                    }
                    else {
                        divContainer.contextMenu('divContextMenu', {
                        });
                    }
                }
            }
        },
        initButtonEvent: function (btnItem, actionButtonItem) {
            var methodName = actionButtonItem.ButtonMethodName;
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
                    BPF_Workflow_Tool._initItemControl.executeForward(actionButtonItem)
                    break;
                case 5://加签
                    isSubmit = false;
                    BPF_Workflow_Tool._initItemControl.executeAddNode(actionButtonItem)
                    break;
                case 6://退回
                    if (bpf_wf_client._otherSetting.CustomerSceneSetting.AlwaysReturnToStart) {
                        bpf_wf_data.BizContext.ExtensionCommond["RejectNode"] = "00000000-0000-0000-0000-000000000000";
                    }
                    else {
                        isSubmit = false;
                        BPF_Workflow_Tool._initItemControl.executeReturn(actionButtonItem)
                    }
                    break;
                case 9://作废
                    if (!confirm("确定“作废”当前流程吗？")) {
                        isSubmit = false;
                    }
                    break;
                default:
                    isSubmit = false;
                    bpf_wf_tool.alert("不支持的按钮类型");
                    break;
            }
            if (isSubmit) {
                var isPass = bpf_workflow_tool.checkSubmit(actionButtonItem);
                if (!isPass) {
                    return false;
                }
                bpf_wf_client._execute.execute(actionButtonItem);
            }
        },
        executeAddNode: function (actionButtonItem) {
            var runningNode = bpf_wf_data._getRunningNode();
            if (runningNode.NodeType != 0 && runningNode.NodeType != 1) {
                bpf_wf_tool.alert("只有发起节点和审批节点允许加签");
                return;
            }
            var divContainerAddNode = $(BPF_Workflow_Tool.template.controlTemplate.AddNode());
            divContainerAddNode.BPF_OpenDiv({
                title: "加签",
                isShowExit: true,
                isShowButton: true,
                onInit: function (context) {
                    var select = context.find("#bpf-wfclient-addnode-currentnode");
                    var nodeNameDom = context.find("#bpf-wfclient-addnode-nodename");
                    nodeNameDom.val(runningNode.User.UserName + "加签");
                    var addNodeName = bpf_wf_client._otherSetting.CustomerSceneSetting.AddNodeName;
                    if (addNodeName != null && addNodeName != "") {
                        nodeNameDom.val(addNodeName);
                    }
                    if (runningNode.NodeType == 0) {
                        context.find("#bpf-wfclient-addnode-type_before").hide();//发起节点只有后加签
                        context.find("#bpf-wfclient-addnode-type_before").next().hide();
                        var nextNodeList = bpf_wf_data._getAddNodeList(runningNode);
                        //从发起节点开始顺序添加可以加签的节点
                        var len = nextNodeList.length;
                        for (var i = 0; i < len; i++) {
                            var nodeTemp = nextNodeList[i];
                            var nodeText = bpf_wf_data._getNodeNameAndUserFormateText(nodeTemp);
                            select.append('<option value="' + nodeTemp.NodeID + '" title="' + nodeText + '" nodetype="' + nodeTemp.NodeType + '">' + nodeText + '</option>');
                        }
                    }
                    else {
                        var runningNameText = bpf_wf_data._getNodeNameAndUserFormateText(runningNode);
                        select.append('<option value="' + runningNode.NodeID + '" title="' + runningNameText + '" nodetype="' + runningNode.NodeType + '">' + runningNameText + '</option>');
                    }
                    select.change(function () {
                        var nodeType = $(this).find("option:selected").attr("nodetype");
                        if (nodeType == 3) {
                            context.find("#bpf-wfclient-addnode-audittype_autoinform").click();
                        }
                        else {
                            context.find("#bpf-wfclient-addnode-audittype-order").click();
                        }
                    })
                    select.change();
                    context.find("#bpf-wfclient-addnode-logtextarea").val($("#bpf-wfclient-log-textarea").val());

                    var aselect = context.find("#bpf-wfclient-addnode-aselect");
                    aselect.click(function () {
                        bpf_userselect_client.selectUser({
                            func: function (data) {
                                $.each(data, function (i, item) {
                                    if (item.UserCode != bpf_wf_data.WorkFlowContext.CurrentUser.UserCode) {
                                        var span = $("<span>", { "class": "bpf-wfclient-addnode-item" });
                                        var spDel = $("<span>", { "class": "bpf-wfclient-deleteuser" }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                                        span.append($("<span>").text(item.UserName).attr("title", bpf_wf_tool.formateUserTips(item))).append(spDel).append($("<span>").text(";"));
                                        bpf_wf_data.ClientContext.AddNodeUserList.push(item);
                                        spDel.click(function () {
                                            $(this).parent().remove();
                                            bpf_wf_tool.deleteArrayItem(bpf_wf_data.ClientContext.AddNodeUserList, item);
                                        });
                                        $(aselect).before(span);
                                    }
                                })
                            },
                            appCode: bpf_wf_data.BizContext.AppCode,
                            exceptUserList: bpf_wf_data.ClientContext.AddNodeUserList
                        });
                    })

                    context.find("input[type='radio'][name='bpf-wfclient-addnode-type']").change(function () {
                        var value = $(this).val();
                        if (value == 0) {
                            context.find("#bpf-wfclient-addnode-content").height(190);
                            context.find("#bpf-wfclient-addnode-trlog").show();
                            if (context.find("input[type='radio'][name='bpf-wfclient-addnode-audittype']:checked").val() == 3) {
                                context.find("#bpf-wfclient-addnode-audittype_order").click();
                            }
                            context.find("#bpf-wfclient-addnode-audittype_autoinform").hide();
                            context.find("#bpf-wfclient-addnode-audittype_autoinform").next().hide();
                            $("#bpf-wfclient-addnode-logtextarea").focus();
                        }
                        else {
                            context.find("#bpf-wfclient-addnode-content").height(120);
                            context.find("#bpf-wfclient-addnode-trlog").hide();
                            context.find("#bpf-wfclient-addnode-audittype_autoinform").show();
                            context.find("#bpf-wfclient-addnode-audittype_autoinform").next().show();
                            $("#bpf-wfclient-addnode-nodename").focus();
                        }

                    })
                    context.find("input[type='radio'][name='bpf-wfclient-addnode-audittype']").change(function () {
                        var value = $(this).val();
                        if (value == 3) {
                            nodeNameDom.val(runningNode.User.UserName + "加通知");
                        }
                        else if (value == 2) {
                            nodeNameDom.val(runningNode.User.UserName + "加会签");
                        }
                        else {
                            nodeNameDom.val(runningNode.User.UserName + "加签");
                        }
                        $("#bpf-wfclient-addnode-nodename").focus();
                    })
                },
                onSubmit: function (context) {
                    var cloneNodeListObj = null;
                    var selectNodeID = $("#bpf-wfclient-addnode-currentnode").val();
                    var selectNode = bpf_wf_data.BizContext.NodeInstanceList[selectNodeID];
                    var cloneNode = bpf_wf_data._getCloneNode(bpf_wf_data.BizContext.NodeInstanceList[selectNodeID]);
                    if (cloneNode == null) {
                        bpf_wf_tool.alert("克隆节点为空，不允许加签！");
                        return;
                    }
                    if (bpf_wf_data.ClientContext.AddNodeUserList.length == 0) {
                        bpf_wf_tool.alert("请选择处理人");
                        return false;
                    }

                    var addNodeType = $("input[type='radio'][name='bpf-wfclient-addnode-type']:checked").val();//加签类型（1：后加签，2：前加签）
                    var approvalContent = $.trim(context.find("#bpf-wfclient-addnode-logtextarea").val());
                    if (addNodeType == 0) {
                        if (approvalContent == "") {
                            bpf_wf_tool.alert("请填写加签意见");
                            return false;
                        }
                    }
                    var nodeNameCustomer = context.find("#bpf-wfclient-addnode-nodename").val();
                    if (nodeNameCustomer == "") {
                        bpf_wf_tool.alert("请填写加签节点名称");
                        return false;
                    }
                    var addNodeAuditType = $("input[type='radio'][name='bpf-wfclient-addnode-audittype']:checked").val();//审批类型（1：顺序审批，2：同时审批）
                    var copyBizContextNodeInstanceList = $.extend(true, {}, bpf_wf_data.BizContext.NodeInstanceList);//拷贝一份节点数据，保证加签出错时，数据可以正常
                    if (bpf_wf_data.ClientContext.AddNodeUserList.length < 2 && addNodeAuditType == 2) {
                        bpf_wf_tool.alert("同时审批的处理人必须为两人及以上！");
                        return false;
                    }

                    if (addNodeAuditType == 1) {
                        cloneNodeListObj = bpf_wf_tool.initAddNodeOrderList(runningNode, cloneNode, nodeNameCustomer, bpf_wf_data.ClientContext.AddNodeUserList);
                    }
                    else if (addNodeAuditType == 2) {
                        cloneNodeListObj = bpf_wf_tool.initAddNodeCosignerList(runningNode, cloneNode, nodeNameCustomer, bpf_wf_data.ClientContext.AddNodeUserList);
                    }
                    else if (addNodeAuditType == 3) {
                        cloneNodeListObj = bpf_wf_tool.initAddNodeAutoInformList(runningNode, cloneNode, nodeNameCustomer, bpf_wf_data.ClientContext.AddNodeUserList);
                    }
                    var addNodeList = cloneNodeListObj.AddNodeList;
                    var addNodeArray = cloneNodeListObj.AddNodeArray;
                    var firstNode = addNodeList[cloneNodeListObj.FirstNodeID];
                    var lastNode = addNodeList[cloneNodeListObj.LastNodeID];

                    if (addNodeType == 0) {//前加签
                        var nodeName = bpf_wf_data._getNodeNameAndUserFormateText(firstNode, addNodeArray);
                        if (confirm("流程将提交至“" + nodeName + "”\r确定提交吗？")) {
                            firstNode.PrevNodeID = runningNode.PrevNodeID;
                            bpf_wf_data.BizContext.NodeInstanceList[runningNode.PrevNodeID].NextNodeID = firstNode.NodeID;
                            lastNode.NextNodeID = runningNode.NodeID;
                            runningNode.PrevNodeID = lastNode.NodeID;
                            bpf_wf_data.BizContext.NodeInstanceList[runningNode.NodeID] = $.extend(true, {}, runningNode);//防止出现加签节点前后关联关闭不对
                            $.each(addNodeList, function (i, item) {
                                bpf_wf_data.BizContext.NodeInstanceList[i] = item;
                            })

                            var isPass = bpf_workflow_tool.checkSubmit(actionButtonItem, approvalContent);
                            if (!isPass) {
                                return false;
                            }
                            bpf_wf_client._execute.execute(actionButtonItem, function (success) {
                                if (success) {
                                    divContainerAddNode.CloseDiv();
                                }
                            });
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        var navAddNodeDom = [];
                        var node = firstNode;
                        while (node != null) {
                            var spDelNav = $("<span>", { "class": "bpf-wfclient-deletenav", "nodeid": node.NodeID }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                            var spDom = BPF_Workflow_Tool._initItemControl.initNavItem(node, false, addNodeArray);
                            spDom.append(spDelNav);

                            spDelNav.click(function () {
                                var nodeID = $(this).attr("nodeid");
                                var nodeDelete = bpf_wf_data.BizContext.NodeInstanceList[nodeID];
                                if (nodeDelete.NextNodeID == "") {
                                    bpf_wf_data.BizContext.NodeInstanceList[nodeDelete.PrevNodeID].NextNodeID = "";
                                    $(this).parent().prev().remove();
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
                                $(this).parent().next().remove();
                                $(this).parent().remove();
                            })

                            navAddNodeDom.push({ Dom: spDom, Arrow: BPF_Workflow_Tool._domBuild.buildNavArrowSpDom(), NodeID: node.NodeID });
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
                        $.each(navAddNodeDom, function (i, item) {
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

                        $.each(addNodeList, function (i, item) {
                            bpf_wf_data.BizContext.NodeInstanceList[i] = item;
                        })
                        bpf_wf_data.BizContext.ExtensionCommond["AddAfterNode"] = "True";
                    }
                    bpf_wf_data.ClientContext.AddNodeUserList = [];
                    return true;
                },
                onCancel: function () {
                    bpf_wf_data.ClientContext.AddNodeUserList = [];
                    return true;
                }
            });
        },
        executeReturn: function (actionButtonItem) {
            var divContainerReturn = $(BPF_Workflow_Tool.template.controlTemplate.Return());
            divContainerReturn.BPF_OpenDiv({
                title: "退回",
                isShowExit: true,
                isShowButton: true,
                onInit: function (context) {
                    var approvalContent = $("#bpf-wfclient-log-textarea").val();
                    context.find("#bpf-wfclient-return-logtextarea").val(approvalContent);

                    var select = context.find("#bpf-wfclient-return-select");
                    var currentNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[bpf_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID];
                    var beforeNodeList = [];
                    var beforeNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[currentNode.PrevNodeID];
                    while (beforeNode != null) {
                        if (beforeNode.NodeType == 1 || beforeNode.NodeType == 2 || beforeNode.NodeType == 0 || beforeNode.NodeType == 7) {
                            beforeNodeList.push(beforeNode);
                        }
                        if (beforeNode.PrevNodeID != "") {
                            beforeNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[beforeNode.PrevNodeID]
                        }
                        else {
                            beforeNode = null;
                        }
                    }

                    beforeNodeList = bpf_wf_client._otherSetting.OnRejectNodeExecute(beforeNodeList);

                    //从发起节点开始顺序添加可以退回的节点
                    var len = beforeNodeList.length;
                    for (var i = (len - 1); i >= 0; i--) {
                        var nodeTemp = beforeNodeList[i];
                        var nodeText = bpf_wf_data._getNodeNameAndUserFormateText(nodeTemp);
                        select.append('<option value="' + nodeTemp.NodeID + '" title="' + nodeText + '">' + nodeText + '</option>');
                    }
                },
                onSubmit: function (context) {
                    var rejectNodeID = context.find("#bpf-wfclient-return-select").val();
                    var rejectNode = bpf_wf_data.WorkFlowContext.NodeInstanceList[rejectNodeID];
                    var msg = "确定退回到“";
                    if (rejectNode.NodeType == 0 || rejectNode.NodeType == 1 || rejectNode.NodeType == 7 || rejectNode.NodeType == 2) {
                        msg = msg + context.find("#bpf-wfclient-return-select").find("option:selected").text();
                    }
                    else {
                        bpf_wf_tool.alert("退回的节点类型不正确");
                        return false;
                    }
                    msg = msg + "”节点吗？";
                    if (confirm(msg)) {
                        var approvalContent = context.find("#bpf-wfclient-return-logtextarea").val();
                        var isPass = bpf_workflow_tool.checkSubmit(actionButtonItem, approvalContent);
                        if (!isPass) {
                            return false;
                        }
                        bpf_wf_data.BizContext.ExtensionCommond["RejectNode"] = rejectNodeID;
                        bpf_wf_client._execute.execute(actionButtonItem, function (success) {
                            if (success) {
                                divContainerReturn.CloseDiv();
                            }
                        });
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            });
        },
        executeForward: function (actionButtonItem) {
            var divContainerForward = $(BPF_Workflow_Tool.template.controlTemplate.Forward());
            divContainerForward.BPF_OpenDiv({
                title: "转发",
                isShowExit: true,
                isShowButton: true,
                onInit: function (context) {
                    var approvalContent = $("#bpf-wfclient-log-textarea").val();
                    context.find("#bpf-wfclient-forward-logtextarea").val(approvalContent);
                    var aselect = context.find("#bpf-wfclient-forward-aselect");

                    aselect.click(function () {
                        bpf_userselect_client.selectUser({
                            func: function (data) {
                                $.each(data, function (i, item) {
                                    if (item.UserCode != bpf_wf_data.WorkFlowContext.CurrentUser.UserCode) {
                                        var span = $("<span>", { "class": "bpf-wfclient-cc-item" });
                                        var spDel = $("<span>", { "class": "bpf-wfclient-deleteuser" }).append(BPF_Workflow_Tool._domBuild.buildDeleteImgDom());
                                        span.append($("<span>").text(item.UserName).attr("title", bpf_wf_tool.formateUserTips(item))).append(spDel).append($("<span>").text(";"));
                                        bpf_wf_data.ClientContext.ForwardUserList.push(item);
                                        spDel.click(function () {
                                            $(this).parent().remove();
                                            bpf_wf_tool.deleteArrayItem(bpf_wf_data.ClientContext.ForwardUserList, item);
                                        });
                                        $(aselect).before(span);
                                    }
                                })
                                context.find("#bpf-wfclient-forward-logtextarea").focus();
                            },
                            appCode: bpf_wf_data.BizContext.AppCode,
                            exceptUserList: bpf_wf_data.ClientContext.ForwardUserList
                        });
                    })
                },
                onSubmit: function (context) {
                    if (bpf_wf_data.ClientContext.ForwardUserList.length == 0) {
                        bpf_wf_tool.alert("请选择接收人");
                        return false;
                    }
                    var approvalContent = context.find("#bpf-wfclient-forward-logtextarea").val();
                    var isPass = bpf_workflow_tool.checkSubmit(actionButtonItem, approvalContent);
                    if (!isPass) {
                        return false;
                    }
                    bpf_wf_data.BizContext.ExtensionCommond["ForwardUser"] = JSON.stringify(bpf_wf_data.ClientContext.ForwardUserList);
                    bpf_wf_client._execute.execute(actionButtonItem, function (success) {
                        if (success) {
                            divContainerForward.CloseDiv();
                        }
                    });
                },
                onCancel: function (context) {
                    bpf_wf_data.ClientContext.ForwardUserList = [];
                    return true;
                }
            });
        }
    },
    checkSubmit: function (actionButtonItem, approvalContent) {
        var msg = "";
        if (actionButtonItem.ButtonType != 0 && actionButtonItem.ButtonType != 9) {
            $.each(bpf_wf_data.BizContext.NodeInstanceList, function (i, item) {
                if (item.NodeType == 1 && (item.User == null || (item.User.UserCode == "" && item.User.UserLoginID == ""))) {
                    //校验节点中的人是否选择
                    msg = msg + "节点【" + item.NodeName + "】未选择审批人\r";
                }
            })
        }
        if (approvalContent == undefined) {
            approvalContent = $("#bpf-wfclient-log-textarea").val();
        }
        bpf_wf_data.BizContext.ApprovalContent = approvalContent;
        if ($.trim(bpf_wf_data.BizContext.ApprovalContent) == "" && actionButtonItem.ButtonType != 0 && actionButtonItem.ButtonType != 7) {
            msg = msg + "请输入审批意见\r"
        }

        if (msg != "") {
            bpf_wf_tool.alert(msg);
            return false;
        }
        else {
            return true;
        }
    },
    _domBuild: {
        buildDeleteImgDom: function () {
            return $("<img>", { src: bpf_wf_client._constSetting.deleteImgSrc(), title: "删除", "class": "bpf-wfclient-deleteimg" });
        },
        buildNavOkImgDom: function () {
            return $("<img>", { alt: "", src: bpf_wf_client._constSetting.okImgSrc() });
        },
        buildNavArrowSpDom: function () {
            return $("<span>", { "class": "bpf-wfclient-nav-sparrrow" }).append($("<img>", {
                alt: "", "class": "bpf-wfclient-nav-arrrow", src: bpf_wf_client._constSetting.arrowImgSrc()
            }));
        }
    },
    template: {
        content: function () {
            return [
                '<div class="bpf-wf-content" style="">',
                '<table class="bpf-wfclient-table">',
                '<tr>',
                '<td style="width:100px">审批流程</td>',
                '<td>',
                '<div id="bpf-wfclient-nav-content"></div>',
                '</td>',
                '</tr>',
                '<tr>',
                '<td>抄送</td>',
                '<td>',
                '<div id="bpf-wfclient-cc-content"></div>',
                '</td>',
                '</tr>',
                '<tr>',
                '<td>快捷意见</td>',
                '<td>',
                '<span class="bpf-wfclient-log-quicklog">同意</span><span class="spmarginright">&nbsp;</span>',
                '<span class="bpf-wfclient-log-quicklog">不同意</span><span class="spmarginright">&nbsp;</span>',
                '<span class="bpf-wfclient-log-quicklog">收到</span>',
                '</td>',
                '</tr>',
                '<tr>',
                '<td id="bpf-wfclient-log-title">审批意见</td>',
                '<td>',
                '<textarea id="bpf-wfclient-log-textarea"></textarea>',
                '</td>',
                '</tr>',
                '</table>',
                '<div id="bpf-wfclient-button-content" class="bpf-wfclient"></div>',
                '<!--审批日志-->',
                '<div id="bpf-wfclient-log-content">',
                '</div>',
                '</div>',

            ].join("");
        },
        button: function () {
            return [
                '<table id="bpf-wfclient-button-table">',
                '<tbody>',
                '<tr style="height: 40px;">',
                '<td>&nbsp;</td>',
                '<td>',
                '<div id="bpf-wfclient-button-div">',
                '<!--按钮区域-->',
                '</div>',
                '</td>',
                '<td>&nbsp;</td>',
                '</tr>',
                '</tbody>',
                '</table>'
            ].join("");
        },
        opinion: function () {
            return [
                '<table style="width: 100%; border-spacing: 0;">',
                '<tr>',
                '<td style="text-align: center;">',
                '<table id="bpf-wfclient-log-logtable" class="bpf-wfclient-table">',
                '<tbody>',
                '<tr>',
                '<th class="bpf-wfclient-log-logtable-thnode">节点</th>',
                '<th>审批意见</th>',
                '<th class="bpf-wfclient-log-logtable-approver">审批人</th>',
                '<th class="bpf-wfclient-log-logtable-approvedate">审批时间</th>',
                '<th class="bpf-wfclient-log-logtable-operator">操作</th>',
                '</tr>',
                '</tbody> ',
                '</table> ',
                '</td> ',
                '</tr> ',
                '<tr> ',
                '<td id="bpf-wfclient-log-showmore"> ',
                '<span><img alt="\" src="' + bpf_wf_client._constSetting.downImgSrc() + '"> ',
                '<a>显示更多</a></span>',
                '</td> ',
                '</tr> ',
                '</table> ',].join("");
        },
        controlTemplate: {
            AddNode: function () {
                return [
                    '<div id="bpf-wfclient-addnode-content">',
                    '<table class="bpf-wfclient-table">',
                    '<tr>',
                    '<th style="width: 100px;">加签<span style="color: red;">*</span>:</th>',
                    '<td>',
                    '<a id="bpf-wfclient-addnode-aselect">请选择</a>',
                    '</td>',
                    '</tr>',
                    '<tr>',
                    '<th>节点:</th>',
                    '<td>',
                    '<select id="bpf-wfclient-addnode-currentnode"></select>',
                    '<input id="bpf-wfclient-addnode-type_after" name="bpf-wfclient-addnode-type" type="radio" value="1" checked="checked" /><label for="bpf-wfclient-addnode-type_after">之后</label>',
                    '<input id="bpf-wfclient-addnode-type_before" name="bpf-wfclient-addnode-type" type="radio" value="0" /><label for="bpf-wfclient-addnode-type_before">之前</label>',
                    '</td>',
                    '</tr>',
                    '<tr>',
                    '<th>审批类型:</th>',
                    '<td>',
                    '<input id="bpf-wfclient-addnode-audittype_order" name="bpf-wfclient-addnode-audittype" type="radio" value="1"  checked="checked" ><label for="bpf-wfclient-addnode-audittype-order">顺序审批</label>',
                    '<input id="bpf-wfclient-addnode-audittype_cosigner" name="bpf-wfclient-addnode-audittype" type="radio" value="2"><label for="bpf-wfclient-addnode-audittype_cosigner">同时审批</label>',
                    '<input id="bpf-wfclient-addnode-audittype_autoinform" name="bpf-wfclient-addnode-audittype" type="radio" value="3"><label for="bpf-wfclient-addnode-audittype_autoinform">通知</label>',
                    '</td>',
                    '</tr>',
                    '<tr>',
                    '<th>加签节点名称:</th>',
                    '<td>',
                    '<input id="bpf-wfclient-addnode-nodename" type="text" value="1" >',
                    '</td>',
                    '</tr>',
                    '<tr style="display: none;" id="bpf-wfclient-addnode-trlog">',
                    '<th>加签意见<span style="color: red;">*</span>:</th>',
                    '<td>',
                    '<textarea id="bpf-wfclient-addnode-logtextarea"></textarea>',
                    '</td>',
                    '</tr>',
                    '</table>',
                    '</div>'
                ].join("");
            },
            Return: function () {
                return [
                    '<div id="bpf-wfclient-return-content">',
                    '<table class="bpf-wfclient-table">',
                    '<tr>',
                    '<th style="width: 100px;">退回到<span style="color: red;">*</span>：</th>',
                    '<td><select id="bpf-wfclient-return-select"></select></td>',
                    '</tr>',
                    '<tr>',
                    '<th>退回意见<span style="color: red;">*</span>：</th>',
                    '<td>',
                    '<textarea id="bpf-wfclient-return-logtextarea"></textarea>',
                    '</td>',
                    '</tr>',
                    '</table>',
                    '</div>'
                ].join("");
            },
            Forward: function () {
                return [
                    '<div id="bpf-wfclient-forward-content">',
                    '<table class="bpf-wfclient-table">',
                    '<tr>',
                    '<th style="width:100px;">接收人<span style="color: red;">*</span>：</th>',
                    '<td><a id="bpf-wfclient-forward-aselect">请选择</a></td>',
                    '</tr>',
                    '<tr>',
                    '<th>转发意见<span style="color: red;">*</span>：</th>',
                    '<td>',
                    '<textarea id="bpf-wfclient-forward-logtextarea"></textarea>',
                    '</td>',
                    '</tr>',
                    '</table>',
                    '</div>'
                ].join("");
            }
        }
    }
}

//弹出层
jQuery.fn.extend(
    {
        BPF_OpenDiv: function (options) {
            var $self = $(this);
            var _setting = {
                title: "",
                level: 1, //弹出层级别
                onInit: function (context) { },  //在对话框弹出时， 初始化对话框内容

                onCancel: function (context) { /*$(context).BPF_ContextValidation(false);*/ return true; }, //在对话框取消/关闭时， 触发的事件； 未返回为true，则无法关闭对话框
                onSubmit: function (context) { return true; }, //在对话框确定提交时， 触发的事件； 未返回为true，则无法关闭对话框
                btns: [], //自定义的按钮及事件 btns: [{ name: "test", cssClass: "", onclick: function (context) { } }] ,这是参数格式
                mode: "confirm", //or "alert" or "info"
                widthMode: "standard", //thin/small= 400px, standard=700px, wide/large=1000px
                scrollY: false, //如果弹出内容很长或者需要增长， 则设为scrollY:true
                other: "",
                isShowExit: false,
                isShowButton: false
            }
            BPF_OPENDIV_OBJARRAY.push({
                Obj: $self, CancelFunc: function () {
                    if (!!_setting.onCancel($popupFrame)) {
                        $self.ClearInputs();
                        $self.ClearFloatDiv($self);
                        $self.CloseDiv();
                    }
                }
            });
            if (!window.popupZindex) {
                window.popupZindex = 10000;
            }

            $.extend(_setting, options);


            var sWidth, sHeight;
            sWidth = window.screen.availWidth;
            if (window.screen.availHeight > document.body.scrollHeight) {
                sHeight = window.screen.availHeight;

            } else {
                sHeight = document.body.scrollHeight + 20;
            }
            var maskObj = $("<div  class='bpf_user_maskDiv'></div>");
            maskObj.css({ width: sWidth, height: sHeight, zIndex: window.popupZindex++ });
            maskObj.appendTo($("body"));
            $("body").attr("scroll", "no");

            //$("#BigDiv").data("divbox_selectlist", $("select:visible"));
            //$("select:visible").hide();
            maskObj.data("divbox_scrolltop", $.BPF_ScrollPosition().Top);
            maskObj.data("divbox_scrollleft", $.BPF_ScrollPosition().Left);
            if ($(".bpf_user_maskDiv").length == 1) {
                $("html").data("overflow", $("html").css("overflow"))
                    .css("overflow", "hidden");
            }
            //window.scrollTo($("#BigDiv").attr("divbox_scrollleft"), maskObj.attr("divbox_scrolltop"));

            if (_setting.level < 1) {
                _setting.level = 1
            }
            if (_setting.level > 4) {
                _setting.level = 4
            }

            var popupFrameHtml = [

                '<div class="bpf_user_popup context">',
                ' <div class="bpf_user_t_l"> ',
                ' </div>            ',
                ' <div class="bpf_user_t_m"> ',
                ' </div>            ',
                ' <div class="bpf_user_t_r"> ',
                ' </div>            ',
                ' <div class="bpf_user_m_l dnrHandler"> ',
                ' </div>            ',
                ' <div class="bpf_user_m_m"> ',
                '     <div class="bpf_user_pop_titile bpf_user_dnrHandler">',
                '         <span class="bpf_user_pop_icon bpf_user_pop_icon_0' + _setting.level + '"></span>',
                '         <div class="bpf_user_pop_txt">',
                '             <input type="button" style="width:0px;border:none;height:0px; border:0px;padding:0px;margin:0px;" id="closepicxxxxx"/>',
                '             <span class="bpf_user_pop_txt_title"></span>',
                //'             <img src="/images/popup/help.png" class="help" />',
                //'             <img src="/images/popup/exit.png" class="exit" style="display:none" />',
                '             <span class="bpf_user_exit" style="display:none">关闭</span>',
                '         </div>',
                '     </div>',
                '     <!--pop_titile-->',
                '     <div class="bpf_user_padding_10  bpf_user_popup_content">', //class overflow
                '     </div>',
                '     <div class="bpf_user_pop_btn line_t" style="padding: 10px; display:none">',
                '         <div class="bpf_user_btn01 bpf_user_btn_fr bpf_user_btn_cancel"  ><a><span>取消</span></a></div>',
                '         <div class="bpf_user_btn01 bpf_user_btn_fr bpf_user_btn_submit"><a><span>确定</span></a></div>',
                '     </div>',
                ' </div>',
                ' <div class="bpf_user_m_r bpf_user_dnrHandler">',
                ' </div>             ',
                ' <div class="bpf_user_b_l">  ',
                ' </div>             ',
                ' <div class="bpf_user_b_m bpf_user_dnrHandler">  ',
                ' </div>             ',
                ' <div class="bpf_user_b_r bpf_user_resizeHandler ">  ',
                ' <div class="bpf_user_resizeHandler"></div>  ',
                ' </div>             ',
                '</div>'
            ];
            var $popupFrame;//= $(popupFrameHtml.join(""));
            if ($self.parents(".bpf_user_popup").length > 0) {
                $popupFrame = $self.parents(".bpf_user_popup:first");
                $popupFrame.css({ zIndex: window.popupZindex++ });
            }
            else {
                $popupFrame = $(popupFrameHtml.join(""));
                $popupFrame.css({ zIndex: window.popupZindex++ });
                $popupFrame.appendTo($("body"));
                $popupFrame.find(".bpf_user_popup_content").append($self);  //将内容附属到弹出层上
                $popupFrame.find(".bpf_user_exit, .bpf_user_btn_cancel").click(function () {

                    if (!!_setting.onCancel($popupFrame)) {
                        $self.ClearInputs();
                        $self.ClearFloatDiv($self);
                        $self.CloseDiv();
                    }
                });
                $popupFrame.find(".bpf_user_help").click(function () {
                    //alert("TODO 显示帮助内容");
                });
                $popupFrame.find(".bpf_user_btn_submit").click(function () {
                    if (!!_setting.onSubmit($popupFrame)) {
                        $self.ClearInputs();
                        $self.ClearFloatDiv($self);
                        $self.CloseDiv();
                    }
                });
                $popupFrame.bind("submitContext", function () {
                    $popupFrame.find(".bpf_user_btn_submit").click();
                });
                $popupFrame.bind("cancelContext", function () { $popupFrame.find(".bpf_user_exit").click(); });
                $popupFrame.bind("hideContext", function () { $popupFrame.hide(); });
                $popupFrame.bind("showContext", function () { $popupFrame.show(); });
                // 设置高度
                if (_setting.scrollY) {
                    $popupFrame.find(".bpf_user_popup_content").addClass("overflow");
                }
                // 设置宽度
                if (_setting.widthMode) {
                    switch (_setting.widthMode) {
                        case "small":
                        case "thin": $popupFrame.css({ width: 400 }); break;
                        case "large":
                        case "wide": $popupFrame.css({ width: 1000 }); break;
                        default: break;
                    }
                }

                var btnContainer = $popupFrame.find(".bpf_user_pop_btn");
                if (_setting.isShowButton) {
                    btnContainer.show();
                }
                if (_setting.mode) {
                    if (_setting.mode == "info") {
                        btnContainer.find(".bpf_user_btn_submit").remove();
                        btnContainer.find(".bpf_user_btn_cancel").remove();
                    }
                    else if (_setting.mode == "alert") {
                        btnContainer.find(".bpf_user_btn_cancel").remove();
                    }
                }
                if (_setting.isShowExit) {
                    $(".bpf_user_exit").show();
                }

                // 额外的自定义按钮
                if (_setting.btns && $.isArray(_setting.btns)) {
                    $.each(_setting.btns, function (i, btnDesc) {
                        btnDesc.cssClass = btnDesc.cssClass || "";
                        var btn = $('<div class="bpf_user_btn_blue01 bpf_user_btn_fr ' + btnDesc.cssClass + '"  ><a><span>' + btnDesc.name + '</span></a></div>');
                        if (btnDesc.onclick && $.isFunction(btnDesc.onclick)) {
                            btn.click(function () {
                                var r = btnDesc.onclick($popupFrame);
                                if (r == true) {
                                    $self.CloseDiv();
                                }
                            });
                        }
                        btn.appendTo(btnContainer);
                    });
                }
            }
            $popupFrame.data("level", _setting.level);//缓存当前级别，如果需要多次弹出， 则需要取此值。
            $popupFrame
                .BPF_jqDrag($popupFrame.find(".bpf_user_dnrHandler"), $self.ClearFloatDiv) // 位置可拖拽, 第2个参数表示拖动时要“清理”的动作
                .BPF_jqResize($popupFrame.find(".bpf_user_resizeHandler"))      // 右下角拖拉可Resize
                .find(".bpf_user_pop_txt_title").text(_setting.title);
            $self.data("mask", maskObj); //关联遮罩
            if (_setting.onInit && $.isFunction(_setting.onInit)) {
                _setting.onInit($popupFrame);
            }
            var MyDiv_w = $popupFrame.width();
            var MyDiv_h = $popupFrame.height() + 15;
            MyDiv_w = parseInt(MyDiv_w, 10);
            MyDiv_h = parseInt(MyDiv_h, 10);
            var width = $.BPF_PageSize().Width;
            var height = $.BPF_PageSize().Height;
            var left = $.BPF_ScrollPosition().Left;
            var top = $.BPF_ScrollPosition().Top;
            var Div_topposition = top + (height / 2) - (MyDiv_h / 2);
            var Div_leftposition = left + (width / 2) - (MyDiv_w / 2) + _setting.level * 26
            $popupFrame.css("left", Div_leftposition + "px");
            $popupFrame.css("top", Div_topposition + "px");
            if (BPF_browser.versions.gecko) {
                $popupFrame.show();
                return;
            }
            $popupFrame.fadeIn("fast");
            document.getElementById("closepicxxxxx").focus();
            return $self;
        }
        , CloseDiv: function () {
            var $self = $(this);
            var mask = $self.data("mask")
            var $popupFrame = $self.parents(".bpf_user_popup");
            var destroy = true;
            if (destroy) {
                $popupFrame.remove();

            } else {

                if (BPF_browser.versions.gecko) {
                    $popupFrame.hide();

                } else {
                    $popupFrame.fadeOut("fast");
                }
            }


            //$("#maskDiv").data("divbox_selectlist").show();
            if (typeof (mask) != 'undefined') {
                if ($(".bpf_user_maskDiv").length == 1) { //由于是多层弹出， 只有在最后关闭的时候才重新显示滚动
                    $("html").css("overflow", $("html").data("overflow"));
                    $("html").css("overflow", "auto");
                    window.scrollTo(mask.data("divbox_scrollleft"),
                        mask.data("divbox_scrolltop"));

                    //myDebugger.log("$self.mask top" + mask.data("divbox_scrolltop"));
                    //myDebugger.log("$self.mask left" + mask.data("divbox_scrollleft"));
                }
                mask.remove();
            }
            // $(this).appendTo($("body"));
            $popupFrame.remove(); //删除外框
        }
        , ClearInputs: function () {
            var container = $(this);
            container.find("table[name='toEmpty']").find("tbody").empty();
            container.find("input:text, textarea, select").val("");
            container.find(":checkbox, :radio").attr("checked", "");
        }
        , ClearFloatDiv: function (handler) {
            //return; // 怀疑与473bug有关
            var context = $(handler).parents(".bpf_user_popup:first");
            if ($.fn.tipsy) {
                // tipsy
                //context.find("[title]").tipsy("hide");
                //context.find("[original-title]").tipsy("hide");
            }
            // autocomplete
            $(".bpf_user_ac_results").hide();
        }
    });



//================================Jquery 插件==================================
//以下实现拖拽与缩放
// h 表示句柄
// c 表示拖拽的时候清理浮动菜单的方法
// k 表示命令，
var BPF_OPENDIV_OBJARRAY = [];
(function ($) {
    $.fn.BPF_jqDrag = function (h, c) {
        return i(this, h, c, 'd');
    };
    $.fn.BPF_jqResize = function (h) {
        return i(this, h, 'r');
    };
    $.BPF_jqDnR = {
        dnr: {}, e: 0,
        drag: function (v) {

            if (M.k == 'd') E.css({ left: M.X + v.pageX - M.pX, top: M.Y + v.pageY - M.pY });
            else {
                E.css({ width: Math.max(v.pageX - M.pX + M.W, 0), height: Math.max(v.pageY - M.pY + M.H, 0) });
            }
            return false;
        },
        stop: function () {
            E.css('opacity', M.o); $("body").unbind('mousemove', J.drag).unbind('mouseup', J.stop);
        }
    };
    var J = $.BPF_jqDnR, M = J.dnr, E = J.e,
        i = function (e, h, c, k) {
            return e.each(function () {
                h = (h) ? $(h, e) : e;
                h.bind('mousedown', {
                    e: e, k: k
                }, function (v) {
                    if (c && typeof (c) == "function") {
                        c(h);
                    }
                    var d = v.data, p = {}; E = d.e;
                    // attempt utilization of dimensions plugin to fix IE issues
                    if (E.css('position') != 'relative') {
                        try { E.position(p); } catch (e) { }
                    }
                    M = { X: p.left || f('left') || 0, Y: p.top || f('top') || 0, W: f('width') || E[0].scrollWidth || 0, H: f('height') || E[0].scrollHeight || 0, pX: v.pageX, pY: v.pageY, k: d.k, o: E.css('opacity') };
                    E.css({ opacity: 0.8 }); $("body").mousemove($.BPF_jqDnR.drag).mouseup($.BPF_jqDnR.stop);
                    return false;
                });
            });
        },
        f = function (k) {
            return parseInt(E.css(k)) || false;
        };
    //使用Esc键关闭弹出窗口 
    $(document).keyup(function (event) {
        if (event.which == '27') {
            if (BPF_OPENDIV_OBJARRAY.length > 0) {
                var $self = BPF_OPENDIV_OBJARRAY[BPF_OPENDIV_OBJARRAY.length - 1];
                $self.CancelFunc();
                BPF_OPENDIV_OBJARRAY.pop();
            }
        }
    });
})(jQuery);

var BPF_browser = {
    versions: function () {
        var u = navigator.userAgent, app = navigator.appVersion;
        return {//移动终端浏览器版本信息 
            trident: u.indexOf('Trident') > -1, //IE内核
            presto: u.indexOf('Presto') > -1, //opera内核
            webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
            mobile: (!!u.match(/AppleWebKit.*Mobile.*/) || !!u.match(/AppleWebKit/)) && u.indexOf('Windows NT') < 0, //是否为移动终端
            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
            android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或者uc浏览器
            iPhone: u.indexOf('iPhone') > -1,// || u.indexOf('Mac') > -1, //是否为iPhone或者QQHD浏览器
            iPad: u.indexOf('iPad') > -1, //是否iPad
            webApp: u.indexOf('Safari') == -1 //是否web应该程序，没有头部与底部
        };
    }(),
    language: (navigator.browserLanguage || navigator.language).toLowerCase()
}

//滚动和获取页面大小
$.extend(
    {
        BPF_PageSize: function () {
            var width = 0;
            var height = 0;
            width = window.innerWidth != null ? window.innerWidth : document.documentElement && document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body != null ? document.body.clientWidth : null;
            height = window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null;
            return { Width: width, Height: height };
        },
        BPF_ScrollPosition: function () {
            var top = 0, left = 0;
            if (BPF_browser.versions.gecko) {
                top = window.pageYOffset;
                left = window.pageXOffset;
            }
            else if (BPF_browser.versions.trident) {
                top = document.documentElement.scrollTop;
                left = document.documentElement.scrollLeft;
            }
            else if (document.body) {
                top = document.body.scrollTop;
                left = document.body.scrollLeft;
            }
            return { Top: top, Left: left };
        }
    });

$.fn.extend({
    // 通过ruleModel绑定的方法来给Context中的输入内容设置错误提示tips
    // 绑定的方法中带一个参数context, 表示上下文； 方法中的this表示输入控件
    // gravity 表示tooltip方向， 默认为向下； 如果是string类型， 则为默认方向 ； 如果是对象， 则表示对指定控件分别设置
    BPF_ContextValidation: function (ruleModel, gravity) {
        var inputs = "input[type=text],input[type=checkbox],textarea,select".split(","); // 输入框
        var $context = $(this);

        if (arguments[0] == false || $.type(arguments[0]) == "undefined" || arguments[0] == null) {
            $context.find("input[data-field],textarea[data-field]").removeClass("validate_error");
            $context.find("[data-field]").each(function () { $(this).tipsy("hide") });
            return;
        }

        if ($.isPlainObject(ruleModel) == false) {
            ruleModel = {};
        }

        var result = true;
        // 
        if ($.type(gravity) == "string") {
            gravity = { base: gravity };
        }
        else {
            gravity = $.extend({ base: "w" }, gravity);
        }
        for (var property in ruleModel) {
            if (ruleModel.hasOwnProperty(property)) {
                var gv = gravity[property] || gravity.base; //设置tooltip方向
                var sender = $context.FindFieldCtrl(property);
                if (sender.length == 0) {
                    continue;
                }
                //var val = $context.val() || $context.attr["data-fieldvalue"] || "";
                if ($.isFunction(ruleModel[property])) {
                    var func = ruleModel[property];
                    if (!controlValidation($context, sender, func, gv)) {
                        result = false; /**/
                    };
                }
                else if ($.isPlainObject(ruleModel[property])) { // 多个方法
                    for (var funcName in ruleModel[property]) {
                        var func = ruleModel[property][funcName];
                        if ($.isFunction(func)) {
                            if (!controlValidation($context, sender, func, gv)) { result = false; };//验证， 一旦有错， 退出
                        }
                    }
                }
                else if ($.isArray(ruleModel[property])) { // 多个方法数组
                    for (var i = 0; i < ruleModel[property].length; i++) {
                        var func = ruleModel[property][i];
                        if ($.isFunction(func)) {
                            if (!controlValidation($context, sender, func, gv)) { result = false; }; //验证， 一旦有错， 退出
                        }
                    }
                }
                // clear
                sender.each(function () {
                    $(this).data("errMsgList", null);
                });
            }
        }
        return result;
        function controlValidation(context, controls, validateFunc, gravity) {
            // var val = control.val() || control.attr["data-fieldvalue"] || "";
            var result = true;
            $(controls).each(function () {
                var control = $(this);
                $.each(inputs, function (i, selector) {
                    if (control.is(selector)) {
                        var errMsg = validateFunc.apply(control, [context]);
                        var tips = control.data("errMsgList");
                        if ($.isArray(tips) == false) {
                            tips = [];
                        }
                        if ($.type(errMsg) == "string" || errMsg != true) {
                            tips.push(errMsg);
                            control.data("errMsgList", tips);
                        }
                        //console.log(control.val() + tips);
                        if (tips.length > 0) {
                            result = false;
                        }
                        else {
                            $(control).tipsy("disable");
                            $(control).removeClass("validate_error");
                        }



                        if (tips.length > 0) { //显示tipsy 做成异步
                            var timer = control.data("tipsTimer");
                            if (timer) {
                                clearTimeout(timer);
                            }

                            timer = setTimeout(function () {
                                var tipsHtml = tips.join("</br>");
                                $(control).addClass("validate_error");
                                $(control).attr("title", tipsHtml)
                                    .tipsy({
                                        theme: "warnTheme",
                                        opacity: 0.95,
                                        trigger: "focus",
                                        html: true,
                                        gravity: gravity || "n", //w,n,s,e...
                                        fade: false
                                    })
                                    .tipsy("enable");
                                $(control).tipsy("show");

                                setTimeout(function () { $(control).tipsy("hide") }, 2000, null);

                            }, 100, null);
                        }

                        control.data("tipsTimer", timer);
                    }
                });
            });
            return result;
        }
    }
    // 目前暂未考虑多级的情况
    , FindFieldCtrl: function (field) {
        var context = $(this);
        var ctrl = context.find("[data-field='" + field + "']");
        return ctrl;
    }
});


(function ($) {

    var menu, shadow, trigger, content, hash, currentTarget;
    var defaults = {
        menuStyle: {
            listStyle: 'none',
            padding: '1px',
            margin: '0px',
            backgroundColor: '#fff',
            border: '1px solid #999',
            width: '100px'
        },
        itemStyle: {
            margin: '0px',
            color: '#000',
            display: 'block',
            cursor: 'default',
            padding: '3px',
            border: '1px solid #fff',
            backgroundColor: 'transparent'
        },
        itemHoverStyle: {
            border: '1px solid #0a246a',
            backgroundColor: '#b6bdd2'
        },
        eventPosX: 'pageX',
        eventPosY: 'pageY',
        shadow: true,
        onContextMenu: null,
        onShowMenu: null
    };

    $.fn.contextMenu = function (id, options) {
        if (!menu) {                                      // Create singleton menu
            menu = $('<div id="jqContextMenu"></div>')
                .hide()
                .css({ position: 'absolute', zIndex: '500' })
                .appendTo('body')
                .bind('click', function (e) {
                    e.stopPropagation();
                });
        }
        if (!shadow) {
            shadow = $('<div></div>')
                .css({ backgroundColor: '#000', position: 'absolute', opacity: 0.2, zIndex: 499 })
                .appendTo('body')
                .hide();
        }
        hash = hash || [];
        hash.push({
            id: id,
            menuStyle: $.extend({}, defaults.menuStyle, options.menuStyle || {}),
            itemStyle: $.extend({}, defaults.itemStyle, options.itemStyle || {}),
            itemHoverStyle: $.extend({}, defaults.itemHoverStyle, options.itemHoverStyle || {}),
            bindings: options.bindings || {},
            shadow: options.shadow || options.shadow === false ? options.shadow : defaults.shadow,
            onContextMenu: options.onContextMenu || defaults.onContextMenu,
            onShowMenu: options.onShowMenu || defaults.onShowMenu,
            eventPosX: options.eventPosX || defaults.eventPosX,
            eventPosY: options.eventPosY || defaults.eventPosY
        });

        var index = hash.length - 1;
        $(this).bind('contextmenu', function (e) {
            // Check if onContextMenu() defined
            var bShowContext = (!!hash[index].onContextMenu) ? hash[index].onContextMenu(e) : true;
            if (bShowContext) display(index, this, e, options);
            return false;
        });
        return this;
    };

    function display(index, trigger, e, options) {
        var cur = hash[index];
        content = $('#' + cur.id).find('ul:first').clone(true);
        content.css(cur.menuStyle).find('li').css(cur.itemStyle).hover(
            function () {
                $(this).css(cur.itemHoverStyle);
            },
            function () {
                $(this).css(cur.itemStyle);
            }
        ).find('img').css({ verticalAlign: 'middle', paddingRight: '2px' });

        // Send the content to the menu
        menu.html(content);

        // if there's an onShowMenu, run it now -- must run after content has been added
        // if you try to alter the content variable before the menu.html(), IE6 has issues
        // updating the content
        if (!!cur.onShowMenu) menu = cur.onShowMenu(e, menu);

        $.each(cur.bindings, function (id, func) {
            $('#' + id, menu).bind('click', function (e) {
                hide();
                func(trigger, currentTarget);
            });
        });

        menu.css({ 'left': e[cur.eventPosX], 'top': e[cur.eventPosY] }).show();
        if (cur.shadow) shadow.css({ width: menu.width(), height: menu.height(), left: e.pageX + 2, top: e.pageY + 2 }).show();
        $(document).one('click', hide);
    }

    function hide() {
        menu.hide();
        shadow.hide();
    }

    // Apply defaults
    $.contextMenu = {
        defaults: function (userDefaults) {
            $.each(userDefaults, function (i, val) {
                if (typeof val == 'object' && defaults[i]) {
                    $.extend(defaults[i], val);
                }
                else defaults[i] = val;
            });
        }
    };

})(jQuery);

$(function () {
    $('div.contextMenu').hide();
});