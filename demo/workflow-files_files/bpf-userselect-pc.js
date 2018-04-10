var PCUserSelectClient;
var PCUserSelectSetting;
var PCOpenDiv;
var selectObj = null;
var selectWord = "";

var bpf_userselect_tool = {
    groupParam: {
        groupId: "",
        selectUserListData: [],
        groupFlag: false,
        groupIsEdited: false
    },
    _constData: {
        WebUrl: "",
        Token: "",
        IFrameWidth: 845,
        IFrameHeight: 500
    },
    showSelectUser: function (setting, client) {
        bpf_userselect_tool.firstAsyncSuccessFlag = undefined;
        document.onmousedown = function () {
            if (event.button == 2) {
                try {
                    var word = "";
                    var selectionText;
                    if (window.getSelection) {//DOM,FF,Webkit,Chrome,IE10
                        word = window.getSelection().toString();
                    } else if (document.getSelection) {//IE10
                        word = document.getSelection().toString();
                    } else if (document.selection) {//IE6+10-
                        word = document.selection.createRange().text;
                    } else {
                        word = "";
                    }
                    if (word != null && word != "") {
                        selectWord = word;
                    }
                } catch (e) { }
            }
        }
        var self = this;
        JQZepto(".bpf_userselect_search_input").focus(function () {
            selectObj = this;
        });
        PCUserSelectSetting = setting;
        PCUserSelectClient = client;//JQZepto
        /*
         func: function (userList) { },
            exit: function () { },
            appCode: null,
            allowMulti: true,
            allowAll: true,
            isNeedHiddenNav: false,//只有当业务系统接入了集团App，并且业务系统中没有隐藏集团App导航时需要设置该属性为true，其余时间都需要设置为false
            waitUserList: [],
            checkedUserList: [],
            exceptUserList: []
        */
        //脚本中定义的变量
        var _constData = self._constData;

        var windowWidth = $(window).width() * 0.9;
        var windowHeight = $(window).height() * 0.9;

        if (windowWidth < _constData.IFrameWidth) {
            _constData.IFrameWidth = windowWidth;
        }
        if (windowHeight < _constData.IFrameHeight) {
            _constData.IFrameHeight = windowHeight;
        }
        //设置属性，
        if (!setting.allowAll) {
            _constData.IFrameHeight = 380;
        }

        //添加需要的DOM元素
        var div = $("<div>", { style: "display: block" });
        var divIFrame = $("<div>", { id: "bpf_user_div_iframe", width: _constData.IFrameWidth, height: _constData.IFrameHeight });
        divIFrame.append(this._content());

        //弹出窗口
        PCOpenDiv = divIFrame.BPF_User_OpenDiv({
            title: '选择用户',
            isShowExit: true,
            onCancel: function () {
                setting.exit();
                return true;
            }
        });

        //是否启用常用联系人
        PCUserSelectClient.getEnableTopUserSelect(function (enbaleTopUserSelect) {
            if (enbaleTopUserSelect === true) {
                JQZepto("#bpf_userselect_used_top").show();
            }
        });
        //设置右键操作
        bpf_userselect_tool._InitContextMenu();
        //绑定候选人和已选人
        var waitUser = setting.waitUserList == null ? [] : setting.waitUserList;
        var checkedUser = setting.checkedUserList == null ? [] : setting.checkedUserList;
        JQZepto("#bpf_userselect_sel_unSelect_user").empty();
        var objUnSelect = document.getElementById("bpf_userselect_sel_unSelect_user");
        JQZepto.each(waitUser, function (i, item) {
            bpf_userselect_tool._buildUserModel(item, objUnSelect);
        });

        JQZepto("#bpf_userselect_sel_selected_user").empty();
        var objSelect = document.getElementById("bpf_userselect_sel_selected_user");
        JQZepto.each(checkedUser, function (i, item) {
            bpf_userselect_tool._buildUserModel(item, objSelect);
        });

        //全局选人
        if (setting.allowAll) {
            //允许全局选人
            bpf_userselect_tool._onPageEvent();
            bpf_userselect_tool._loadTree();
        }
        else {
            //不允许全局选人
            JQZepto("#bpf_userselect_top_tab ").hide();
            JQZepto("#bpf_userselect_group_tree").hide();
            JQZepto("#bpf_userselect_search_panel").hide();
            JQZepto(".bpf_userselect_definedgroup_tree").hide();
            JQZepto("#bpf_userselect_btn_search_user").hide();
            JQZepto("#bpf_userselect_btn_create").hide();
            JQZepto("#bpf_userselect_btn_save").hide();

        }


    },
    getUserSelectedList: function () {
        var users = [];
        JQZepto("#bpf_userselect_sel_selected_user option").each(function () {
            var model = {
                UserID: this.data_UserID,
                UserCode: this.data_UserCode,
                UserName: this.data_UserName,
                UserLoginID: this.data_UserLoginID,
                UserJobName: this.data_UserJobName,
                UserOrgID: this.data_UserOrgID,
                UserOrgPathName: this.data_UserOrgPathName
            };
            users.push(model);
        });
        return users;
    },
    _onPageEvent: function () {
        if (PCUserSelectSetting.customerSetting.enable) {
            JQZepto("#bpf_userselect_btn_create").remove();
            JQZepto("#bpf_userselect_btn_save").remove();
            if (!PCUserSelectSetting.customerSetting.org.enable) {
                JQZepto("#bpf_userselect_org").remove();
                JQZepto(".bpf_userselect_group_tree").remove();
            }
            if (!PCUserSelectSetting.customerSetting.userFilter.enable) {
                JQZepto("#bpf_userselect_userFilter").remove();
                JQZepto(".bpf_userselect_search_panel").remove();
            }
            if (!PCUserSelectSetting.customerSetting.group.enable) {
                JQZepto("#bpf_userselect_group").remove();
                JQZepto(".bpf_userselect_definedgroup_tree").remove();
            }
        }

        JQZepto(".bpf_userselect_nav li").eq(0).addClass("active");
        JQZepto(".bpf_userselect_content_panel").eq(0).css("display", "block");
        if (JQZepto(".bpf_userselect_content_panel").eq(0).hasClass("bpf_userselect_search_panel")) {
            JQZepto("#bpf_userselect_btn_search_user").show();
        }

        JQZepto("#bpf_userselect_btn_up").click(function () {
            var so = bpf_dom_api.getSelectedObj("#bpf_userselect_sel_selected_user option");
            if (so.length > 0) {
                if (so.get(0).index != 0) {
                    so.each(function () {
                        JQZepto(this).prev().before(JQZepto(this));
                    });
                }
            }
        });
        JQZepto("#bpf_userselect_btn_down").click(function () {
            var alloptions = JQZepto("#bpf_userselect_sel_selected_user option");
            var so = bpf_dom_api.getSelectedObj("#bpf_userselect_sel_selected_user option");
            if (so.length > 0) {
                if (so.get(so.length - 1).index != alloptions.length - 1) {
                    for (i = so.length - 1; i >= 0; i--) {
                        var item = JQZepto(so.get(i));
                        item.insertAfter(item.next());
                    }
                }
            }
        });
        JQZepto("#bpf_userselect_btn_add").click(function () {
            if (!PCUserSelectSetting.allowMulti) {
                var allselectedoptions = JQZepto("#bpf_userselect_sel_selected_user option");
                var so = bpf_dom_api.getSelectedObj("#bpf_userselect_sel_unSelect_user option");
                if (allselectedoptions.length > 0 || so.length > 1) {
                    bpf_sdk_tool.tips("不允许选择多个人员");
                    return;
                }
            }
            var alloptions = JQZepto("#bpf_userselect_sel_unSelect_user option");
            var so = bpf_dom_api.getSelectedObj("#bpf_userselect_sel_unSelect_user option");
            var allselectedoptions = JQZepto("#bpf_userselect_sel_selected_user option");
            if (so.length > 0) {
                if (!bpf_userselect_tool.groupParam.groupIsEdited && allselectedoptions.length + 1 >= 2 || so.length >= 2 ) {
                    PCUserSelectClient.getPrivateGroupList(function (privateList) {
                        if (privateList.length > 0) {
                            JQZepto("#bpf_userselect_btn_create").removeClass("bpf_userselect_btn_create");
                        } else {
                            JQZepto("#bpf_userselect_btn_create").addClass("bpf_userselect_btn_create");
                        }
                        //JQZepto("#bpf_userselect_btn_create").show();
                    });
                    
                }
                so.get(so.length - 1).index == alloptions.length - 1 ? so.prev().attr("selected", true) : so.next().attr("selected", true);
                JQZepto("#bpf_userselect_sel_selected_user").append(so);

            }
        });
        //选人，支持双击
        JQZepto("#bpf_userselect_select_container").delegate("#bpf_userselect_sel_unSelect_user", "dblclick", function () {
            JQZepto("#bpf_userselect_btn_add").click();
        });
        JQZepto("#bpf_userselect_btn_add_all").click(function () {
            var alloptions = JQZepto("#bpf_userselect_sel_unSelect_user option");
            if (!bpf_userselect_tool.groupParam.groupIsEdited && alloptions.length >= 2) {
                PCUserSelectClient.getPrivateGroupList(function (privateList) {
                    if (privateList.length > 0) {
                        JQZepto("#bpf_userselect_btn_create").removeClass("bpf_userselect_btn_create");
                    } else {
                        JQZepto("#bpf_userselect_btn_create").addClass("bpf_userselect_btn_create");
                    }
                    //JQZepto("#bpf_userselect_btn_create").show();
                });
            }
            if (PCUserSelectSetting.allowMulti) {
                JQZepto("#bpf_userselect_sel_selected_user").append(JQZepto("#bpf_userselect_sel_unSelect_user option").attr("selected", true));
            }
            else {
                bpf_sdk_tool.tips("不允许选择多个人员");
            }
        });
        JQZepto("#bpf_userselect_btn_remove").click(function () {
            var alloptions = JQZepto("#bpf_userselect_sel_selected_user option");
            if (alloptions.length == 2) {
                if (!bpf_userselect_tool.groupParam.groupIsEdited) {
                    JQZepto("#bpf_userselect_btn_create").hide();
                }
            }
            var so = bpf_dom_api.getSelectedObj("#bpf_userselect_sel_selected_user option");
            if (so.length > 0) {
                so.get(so.length - 1).index == alloptions.length - 1 ? so.prev().attr("selected", true) : so.next().attr("selected", true);
                JQZepto("#bpf_userselect_sel_unSelect_user").append(so);
                if (so.length != 2 && so.length == alloptions.length) {
                    if (!bpf_userselect_tool.groupParam.groupIsEdited) {
                        JQZepto("#bpf_userselect_btn_create").hide();
                    }
                }

            }
        });
        JQZepto("#bpf_userselect_btn_remove_all").click(function () {
            if (!bpf_userselect_tool.groupParam.groupIsEdited) {
                JQZepto("#bpf_userselect_btn_create").hide();
            }
            JQZepto("#bpf_userselect_sel_unSelect_user").append(JQZepto("#bpf_userselect_sel_selected_user option").attr("selected", true));
        });
        //确定
        JQZepto("#bpf_userselect_btn_model_OK").click(function () {
            /*
            objOption.data_UserID = item.UserID;
            objOption.data_UserCode = item.UserCode;
            objOption.data_UserName = item.UserName;
            objOption.data_UserLoginID = item.UserLoginID;
            objOption.data_UserJobName = item.UserJobName;
            objOption.data_UserOrgID = item.UserOrgID;
            objOption.data_UserOrgPathName = item.UserOrgPathName;
            */
            bpf_userselect_tool.groupParam.groupFlag = false;
            bpf_userselect_tool.groupParam.groupIsEdited = false;
            var users = bpf_userselect_tool.getUserSelectedList();
            var exceptArray = PCUserSelectSetting.exceptUserList;
            var data = Enumerable.From(users).Distinct(function (data) {
                return data.UserCode
            }).ToArray();
            if (exceptArray != null && exceptArray != undefined) {
                data = Enumerable.From(data).Except(exceptArray, function (data) {
                    return data.UserCode
                }).ToArray();
            }
            if (PCUserSelectSetting.appCode == "YY_ZCPT") {
                data = Enumerable.From(data).Where("f=>f.UserCode!='103779'").ToArray();
            }
            PCUserSelectSetting.func(data);
            PCOpenDiv.User_CloseDiv();
            if (data.length > 0) {
                setTimeout(function () {
                    PCUserSelectClient.saveUserSelectResult(data, PCUserSelectSetting, 'PC');
                })
            }
        });
        //取消    
        JQZepto("#bpf_userselect_btn_model_Close").click(function () {
            bpf_userselect_tool.groupParam.groupFlag = false;
            bpf_userselect_tool.groupParam.groupIsEdited = false;
            PCUserSelectSetting.exit();
            PCOpenDiv.User_CloseDiv();
        });
        //查询常用联系人
        JQZepto("#bpf_userselect_get_used").click(function () {
            JQZepto(".bpf_userselect_cur_selected_node").removeClass("bpf_userselect_cur_selected_node");
            JQZepto("#bpf_userselect_get_used").addClass("bpf_userselect_cur_selected_node");


            PCUserSelectClient.getTopUserList(function (data) {
                var element = JQZepto("#bpf_userselect_sel_unSelect_user");
                var objSelect = document.getElementById("bpf_userselect_sel_unSelect_user");
                element.empty();
                var options = '';
                JQZepto.each(data, function (i, item) {
                    bpf_userselect_tool._buildUserModel(item, objSelect);
                    //element.append('<option value=' + item.UserCode + ' title=' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '>' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '</option>');
                });
            }, PCUserSelectSetting);
        })
        //查询用户
        JQZepto("#bpf_userselect_btn_search_user").click(function () {
            var curUserName = JQZepto.trim(JQZepto("#bpf_userselect_txt_userName").val());
            var curUserDeptName = JQZepto.trim(JQZepto("#bpf_userselect_txt_user_dept_name").val());
            var curUserPost = JQZepto.trim(JQZepto("#bpf_userselect_txt_user_post").val());
            var curUserLoginID = JQZepto.trim(JQZepto("#bpf_userselect_txt_user_loginID").val());
            var curKeyword = JQZepto.trim(JQZepto("#bpf_userselect_txt_search_keyWord").val());
            if (curUserName == "" && curUserDeptName == "" && curUserPost == "" && curUserLoginID == "" && curKeyword == "") {
                alert($userselectlang.PC.searchCheck);
                JQZepto("#bpf_userselect_txt_userName").focus();
            } else {
                var element = JQZepto("#bpf_userselect_sel_unSelect_user");
                var objSelect = document.getElementById("bpf_userselect_sel_unSelect_user");

                var userFilter = {
                    UserName: curUserName,
                    OrgName: curUserDeptName,
                    UserJobName: curUserPost,
                    UserLoginID: curUserLoginID,
                    SearchKeyWord: curKeyword
                };

                PCUserSelectClient.getUserListByFilter(userFilter, function (data) {
                    element.empty();
                    var options = '';
                    JQZepto.each(data, function (i, item) {
                        bpf_userselect_tool._buildUserModel(item, objSelect);
                    });
                });
            }
        });

        //创建组
        JQZepto("#bpf_userselect_btn_create").click(function () {

            var users = bpf_userselect_tool.getUserSelectedList();
            var data = Enumerable.From(users).Distinct(function (data) {
                return data.UserCode
            }).ToArray();
            bpf_userselect_tool.groupParam.selectUserListData = [];
            for (var i = 0; i < data.length; i++) {
                var curData = data[i];
                bpf_userselect_tool.groupParam.selectUserListData.push(curData.UserCode);
            }

            if (bpf_userselect_tool.groupParam.selectUserListData.length >= 200) {
                bpf_sdk_tool.tips("组内用户不能超过200个，请减少组用户数量！");
            } else {
                JQZepto(".bpf_userselect_createPopup").show();
                JQZepto(".bpf_userselect_createGroup").show();
            }
        });

        //新建组取消
        JQZepto("#bpf_userselect_btn_popup_Close").click(function () {
            JQZepto(".bpf_userselect_createPopup").hide();
            JQZepto(".bpf_userselect_createGroup").hide();
        });

        //新建组确定
        JQZepto("#bpf_userselect_btn_popup_OK").click(function () {
            var groupName = JQZepto.trim(JQZepto("#bpf_userselect_groupName").val());
            var groupUser = bpf_userselect_tool.groupParam.selectUserListData.toString();
            if (!groupName) {
                bpf_sdk_tool.tips("组名不能为空");
            } else {
                PCUserSelectClient.savePrivateGroup(groupName, groupUser, function (groupInfo) {
                    JQZepto(".bpf_userselect_createPopup").hide();
                    JQZepto(".bpf_userselect_createGroup").hide();
                    JQZepto("#bpf_userselect_groupName").val('');
                    var treeObj = $.fn.zTreeUser.getZTreeObj("bpf_userselect_grouptree");
                    var privateParentZNode = treeObj.getNodeByParam("GroupID", "privateGroup", null); //获取父节点
                    groupInfo = treeObj.addNodes(privateParentZNode, groupInfo);
                    JQZepto("#bpf_userselect_btn_create").hide();
                });
            }
        });

        //保存组
        JQZepto("#bpf_userselect_btn_save").click(function () {
            var users = bpf_userselect_tool.getUserSelectedList();
            var data = Enumerable.From(users).Distinct(function (data) {
                return data.UserCode
            }).ToArray();
            var selectUserListData = [];
            for (var i = 0; i < data.length; i++) {
                var curData = data[i];
                selectUserListData.push(curData.UserCode);
            }

            if (selectUserListData.length >= 200) {
                bpf_sdk_tool.tips("人员不能超过两百个，请删减人员后再进行下一步操作！");
            } else {
                var groupUser = selectUserListData.toString();
                PCUserSelectClient.savePrivateGroupUser(bpf_userselect_tool.groupParam.groupId, groupUser, function (data) {
                    bpf_userselect_tool.groupParam.groupIsEdited = false;
                    JQZepto("#bpf_userselect_btn_save").hide();
                    bpf_sdk_tool.tips("组内用户更新成功");
                });
            }
        });

    },
    _loadTree: function () {
        //加载树
        var setting = {
            data: {
                key: {
                    name: "ShortName"
                },
                simpleData: {
                    enable: true,
                    idKey: "OrgID",
                    pIdKey: "ParentOrgID"
                }
            },
            view: {
                expandSpeed: "",
                showIcon: false
            },
            async: {
                enable: true,
                getData: function (treeNode, func) {
                    var orgID = treeNode != null ? treeNode.OrgID : "0" 
                    PCUserSelectClient.getOrgListByParentOrgID(orgID, func);
                }
            },
            callback: {
                onClick: function (event, treeId, treeNode) {
                    JQZepto("#bpf_userselect_get_used").removeClass("bpf_userselect_cur_selected_node");
                    var element = JQZepto("#bpf_userselect_sel_unSelect_user");
                    var objSelect = document.getElementById("bpf_userselect_sel_unSelect_user");
                    if (treeNode.OrgID != null && treeNode.OrgID != '') {
                        PCUserSelectClient.getUserListByOrgID(treeNode.OrgID, function (data) {
                            element.empty();
                            var options = '';
                            JQZepto.each(data, function (i, item) {
                                bpf_userselect_tool._buildUserModel(item, objSelect);
                                //element.append('<option value=' + item.UserCode + ' title=' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '>' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '</option>');
                            });
                        });
                    }
                },
                onAsyncSuccess: function zTreeOnAsyncSuccess(event, treeId, msg) {
                    if (bpf_userselect_tool.firstAsyncSuccessFlag == undefined) {
                        try {
                            //调用默认展开第一个结点  
                            var zTree = bpf_userselect_tool.curDataTreeRef;
                            var nodes = zTree.getNodes();
                            zTree.expandNode(nodes[0], true);
                            bpf_userselect_tool.firstAsyncSuccessFlag = 1;
                        } catch (err) { }
                    }
                }

            }
        };
        //加载树
        bpf_userselect_tool.curDataTreeRef = JQZepto.fn.zTreeUser.init(JQZepto("#bpf_userselect_tree_dept"), setting);
    },
    //获取定义的组结构
    _loadGroupTree: function () {
        if (bpf_userselect_tool.groupParam.groupFlag) {
            return;
        }
        bpf_userselect_tool.groupParam.groupFlag = 1;

        function setRenameAndRemoveBtn(treeId, treeNode) {
            if (PCUserSelectSetting.customerSetting.enable) {
                return false;
            } else {
                if (treeNode.level == 0 || treeNode.level == 1 || treeNode.ParentGroupID == "publicGroup") {
                    return false;
                } else {
                    return !treeNode.isParent;
                }
            }
        }
        //加载树
        var setting = {
            data: {
                key: {
                    name: "GroupName"
                },
                simpleData: {
                    enable: true,
                    idKey: "GroupID",
                    pIdKey: "ParentGroupID"
                }
            },
            edit: {
                enable: true,
                removeTitle: "删除节点",
                renameTitle: "编辑节点",
                showRemoveBtn: setRenameAndRemoveBtn,
                showRenameBtn: false
            },
            view: {
                expandSpeed: "",
                showIcon: false
            },
            callback: {
                onClick: function (event, treeId, treeNode) {
                    JQZepto("#bpf_userselect_get_used").removeClass("bpf_userselect_cur_selected_node");
                    var element = JQZepto("#bpf_userselect_sel_unSelect_user");
                    var objSelect = document.getElementById("bpf_userselect_sel_unSelect_user");
                    if (treeNode.level > 1) {
                        PCUserSelectClient.getGroupUserList(treeNode.GroupID, function (data) {
                            element.empty();
                            var options = '';
                            JQZepto.each(data, function (i, item) {
                                bpf_userselect_tool._buildUserModel(item, objSelect);
                                //element.append('<option value=' + item.UserCode + ' title=' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '>' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '</option>');
                            });
                        });
                    }
                },
                onAsyncSuccess: function zTreeOnAsyncSuccess(event, treeId, msg) {
                    if (bpf_userselect_tool.firstAsyncSuccessFlag == undefined) {
                        try {
                            //调用默认展开第一个结点  
                            var zTree = bpf_userselect_tool.curDataTreeRef;
                            var nodes = zTree.getNodes();
                            zTree.expandNode(nodes[0], true);
                            bpf_userselect_tool.firstAsyncSuccessFlag = 1;
                        } catch (err) { }
                    }
                },
                beforeEditName: function (treeId, treeNode) {
                    bpf_userselect_tool.groupParam.groupIsEdited = true;
                    JQZepto("#bpf_userselect_btn_create").hide();
                    JQZepto("#bpf_userselect_btn_save").show();
                    var element = JQZepto("#bpf_userselect_sel_selected_user");
                    var objSelect = document.getElementById("bpf_userselect_sel_selected_user");
                    if (treeNode.GroupID != null && treeNode.GroupID != '') {
                        PCUserSelectClient.getGroupUserList(treeNode.GroupID, function (data) {
                            element.empty();
                            var options = '';
                            JQZepto.each(data, function (i, item) {
                                bpf_userselect_tool._buildUserModel(item, objSelect);
                                //element.append('<option value=' + item.UserCode + ' title=' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '>' + item.UserName + '|' + item.UserOrgPathName + '|' + item.UserJobName + '</option>');
                            });
                        });
                    }
                    bpf_userselect_tool.groupParam.groupId = treeNode.GroupID;

                    var treeObj = $.fn.zTreeUser.getZTreeObj(treeId);
                    treeObj.selectNode(treeNode, false);
                    return false;
                },
                beforeRemove: function (treeId, treeNode) {
                    bpf_userselect_tool.groupParam.groupId = treeNode.GroupID;
                    var msg = confirm("确认删除吗？删除后将会删除组和组内所有用户的信息！");
                    if (msg) {
                        PCUserSelectClient.deletePrivateGroup(bpf_userselect_tool.groupParam.groupId, function () {
                            JQZepto("#bpf_userselect_btn_save").hide();
                            var treeObj = $.fn.zTreeUser.getZTreeObj(treeId);
                            treeObj.removeNode(treeNode);
                        });
                    }
                    return false;
                }
            }
        };
        //加载树
        var zNodes = PCUserSelectClient._getGroupInfo();
        JQZepto.fn.zTreeUser.init(JQZepto("#bpf_userselect_grouptree"), setting, zNodes);
        var treeObj = $.fn.zTreeUser.getZTreeObj("bpf_userselect_grouptree");

        //为公有组和私有组添加子节点
        PCUserSelectClient.getPublicGroupList(function (publicGroupList) {
            if (publicGroupList.length > 0) {            
                var publicParentZNode = treeObj.getNodeByParam("GroupID", "publicGroup", null); //获取父节点
                publicGroupList = treeObj.addNodes(publicParentZNode, publicGroupList);
            }
        });
        PCUserSelectClient.getPrivateGroupList(function (privateGroupList) {
            if ( privateGroupList.length > 0 ) {
                var privateParentZNode = treeObj.getNodeByParam("GroupID", "privateGroup", null); //获取父节点 
                privateGroupList = treeObj.addNodes(privateParentZNode, privateGroupList);
            }
        });
    },
    _buildUserModel: function (item, objSelect) {
        /*UserID,UserCode,UserName,UserLoginID,UserJobName,UserOrgPathID,UserOrgPathName,UserOrgID*/
        var text = item.UserName + new Array(6 - (item.UserName.length > 6 ? 6 : item.UserName.length)).join("　") + '| ' + (item.UserOrgPathName != null ? item.UserOrgPathName.substr(0, 18) : "") + '　| ' + item.UserJobName;
        var title = item.UserName + "【" + item.UserLoginID + "】" + '  |  ' + item.UserOrgPathName + '  |  ' + item.UserJobName;
        var val = item.UserID;
        var objOption = new Option(text, val);
        objOption.title = title;
        objOption.data_UserID = item.UserID;
        objOption.data_UserCode = item.UserCode;
        objOption.data_UserName = item.UserName;
        objOption.data_UserLoginID = item.UserLoginID;
        objOption.data_UserJobName = item.UserJobName;
        objOption.data_UserOrgID = item.UserOrgID;
        objOption.data_UserOrgPathName = item.UserOrgPathName;
        objSelect.options.add(objOption);
    },
    _InitContextMenu: function () {
        var lang = $userselectlang.PC;
        var divContextMenu = JQZepto("#bpf_userselect_div_contextMenu");
        var ul = JQZepto("<ul>");
        divContextMenu.append(ul);
        ul.append(JQZepto("<li>", { id: "lisubmit" }).text(lang.submit).click(function () { JQZepto("#bpf_userselect_btn_model_OK").click(); }));
        ul.append(JQZepto("<li>", { id: "lisearch", "class": "lisearch", style: "display:none" }).text(lang.search).click(function () { JQZepto("#bpf_userselect_btn_search_user").click(); }));
        ul.append(JQZepto("<li>", { id: "lisearchclear", "class": "lisearch", style: "display:none" }).text(lang.reset).click(function () { JQZepto(".bpf_userselect_search_input").val(""); }));
        ul.append(JQZepto("<li>", { id: "licreate" }).text(lang.create).click(function () { JQZepto("#bpf_userselect_btn_create").click(); }));
        ul.append(JQZepto("<li>").text(lang.copy).click(function () { bpf_userselect_tool.copyToClipboard(selectWord); }));
        ul.append(JQZepto("<li>").text(lang.paste).click(function () { bpf_userselect_tool.pasteText(); }));
        JQZepto("body").contextMenuUser('bpf_userselect_div_contextMenu', {
            onShowMenu: function (e, menu) {
                menu.find("#licreate").hide();
                /*var selectOptions = JQZepto("#bpf_userselect_sel_selected_user option");
                if (selectOptions.length > 0) {
                    menu.find("#licreate").show();
                } else {
                    menu.find("#licreate").hide();
                }*/
                if (JQZepto("body").hasClass('searchaction')) {
                    menu.find('#lisubmit').remove();
                }
                else {
                    menu.find('#lisearch,#lisearchclear').remove();
                }
                return menu;
            }
        });
    },
    tabSelect: function (curObj) {
        //选项卡 切换
        var tabType = JQZepto(curObj).attr("data_Type");
        JQZepto(curObj).removeClass().addClass('active').siblings().removeClass();
        switch (tabType) {
            case "1":
                JQZepto(".bpf_userselect_group_tree").show();
                JQZepto(".bpf_userselect_search_panel").hide();
                JQZepto(".bpf_userselect_definedgroup_tree").hide();
                JQZepto("#bpf_userselect_btn_search_user").hide();
                if (bpf_userselect_tool.groupParam.groupIsEdited) {
                    JQZepto("#bpf_userselect_btn_save").show();
                } else {
                    JQZepto("#bpf_userselect_btn_save").hide();
                    JQZepto("#bpf_userselect_btn_create").hide();
                }
                JQZepto("body").removeClass("searchaction");
                JQZepto("body").removeClass("definedGroup");

                break;
            case "2":
                JQZepto(".bpf_userselect_group_tree").hide();
                JQZepto(".bpf_userselect_search_panel").show();
                JQZepto(".bpf_userselect_definedgroup_tree").hide();
                if (bpf_userselect_tool.groupParam.groupIsEdited) {
                    JQZepto("#bpf_userselect_btn_save").show();
                } else {
                    JQZepto("#bpf_userselect_btn_create").hide();
                    JQZepto("#bpf_userselect_btn_save").hide();
                }
                JQZepto("#bpf_userselect_btn_search_user").show();
                JQZepto("body").removeClass("definedGroup").addClass("searchaction");
                break;
            case "3":
                JQZepto(".bpf_userselect_group_tree").hide();
                JQZepto(".bpf_userselect_search_panel").hide();
                JQZepto(".bpf_userselect_definedgroup_tree").show();
                if (bpf_userselect_tool.groupParam.groupIsEdited) {
                    JQZepto("#bpf_userselect_btn_save").show();
                } else {
                    JQZepto("#bpf_userselect_btn_create").hide();
                    JQZepto("#bpf_userselect_btn_save").hide();
                }
                JQZepto("#bpf_userselect_btn_search_user").hide();
                JQZepto("body").removeClass("searchaction").addClass("definedGroup");
                bpf_userselect_tool._loadGroupTree();
                break;
        }
    },
    pasteText: function () {
        event.returnValue = false;
        var text = window.clipboardData.getData("Text");
        if (selectObj != null && text != null) {
            selectObj.value = text;
        }
    },
    copyToClipboard: function (txt) {
        if (window.clipboardData) {
            window.clipboardData.clearData();
            setTimeout(function () {
                window.clipboardData.setData("Text", txt);
            }, 500);

        } else if (navigator.userAgent.indexOf("Opera") != -1) {
            window.location = txt;
        } else if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");

            } catch (e) {
                // alert("被浏览器拒绝！\n请在浏览器地址栏输入'about:config'并回车\n然后将'signed.applets.codebase_principal_support'设置为'true'");
            }
            var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
            if (!clip) return;
            var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
            if (!trans) return;
            trans.addDataFlavor('text/unicode');
            var str = new Object();
            var len = new Object();
            var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
            var copytext = txt;
            str.data = copytext;
            trans.setTransferData("text/unicode", str, copytext.length * 2);
            var clipid = Components.interfaces.nsIClipboard;
            if (!clip) return false;
            clip.setData(trans, null, clipid.kGlobalClipboard);
        }

    },
    _content: function () {
        var lang = $userselectlang.PC;
        return ['<div id="bpf_userselect_page" class="bpf_userselect_page">',
            '            <div id="bpf_userselect_top_tab" class="bpf_userselect_page_tab">',
            '                <ul class="bpf_userselect_nav bpf_userselect_nav_tabs">',
            '                    <li id="bpf_userselect_org" role="presentation" onclick="bpf_userselect_tool.tabSelect(this);" data_type="1"><a href="javascript:void(0);" onfocus="this.blur()">' + lang.orgTab + '</a></li>',
            '                    <li id="bpf_userselect_userFilter" role="presentation" onclick="bpf_userselect_tool.tabSelect(this);" data_type="2"><a href="javascript:void(0);" onfocus="this.blur()">' + lang.searchTab + '</a></li>',
            '                    <li id="bpf_userselect_group" style="display:none" role="presentation" onclick="bpf_userselect_tool.tabSelect(this);" data_type="3"><a href="javascript:void(0);" onfocus="this.blur()">' + lang.definedGroup + '</a></li>',
            '                </ul>',
            '            </div>',
            '            <div class="bpf_userselect_content">',
            '                <div class="bpf_userselect_group_tree bpf_userselect_content_panel" id="bpf_userselect_group_tree">',
            '                    <div class="bpf_userselect_tree_wrap">',
            '                        <ul class="bpf_userselect_ztree bpf_userselect_ztree_inner" id="bpf_userselect_used_top" style="display:none">',
            '                            <li>',
            '                                <a class="bpf_userselect_frequent_contacts" href="javascript:void(0)" title="' + lang.topUser + '" id="bpf_userselect_get_used">',
            '                                    <span>' + lang.topUser + '</span>',
            '                                </a>',
            '                            </li>',
            '                        </ul>',
            '                        <div id="bpf_userselect_tree_dept" class="bpf_userselect_ztree bpf_userselect_ztree_part"></div>',
            '                    </div>',
            '                </div>',
            '                <div class="bpf_userselect_search_panel bpf_userselect_content_panel" id="bpf_userselect_search_panel">',
            '                    <div class="bpf_userselect_table_wrap">',
            '                        <table class="bpf_userselect_table_search">',
            '                            <tr>',
            '                                <th class="bpf_userselect_table_search_th">' + lang.name + '：</th>',
            '                                <td>',
            '                                    <input type="text" class="bpf_userselect_search_input" id="bpf_userselect_txt_userName">',
            '                                    <span>' + lang.nameTip + '</span>',
            '                                </td>',
            '                            </tr>',
            '                            <tr>',
            '                                <th>' + lang.dept + '：</th>',
            '                                <td>',
            '                                    <input type="text" class="bpf_userselect_search_input" id="bpf_userselect_txt_user_dept_name">',
            '                                    <span>' + lang.deptTip + '</span>',
            '                                </td>',
            '                            </tr>',
            '                            <tr>',
            '                                <th>' + lang.job + '：</th>',
            '                                <td>',
            '                                    <input type="text" class="bpf_userselect_search_input" id="bpf_userselect_txt_user_post">',
            '                                    <span>' + lang.jobTip + '</span>',
            '                                </td>',
            '                            </tr>',
            '                            <tr>',
            '                                <th>' + lang.ctx + '：</th>',
            '                                <td>',
            '                                    <input type="text" class="bpf_userselect_search_input" id="bpf_userselect_txt_user_loginID">',
            '                                    <span>' + lang.ctxTip + '</span>',
            '                                </td>',
            '                            </tr>',
            '                            <tr>',
            '                                <th>' + lang.keyWord + '：</th>',
            '                                <td>',
            '                                    <input type="text" class="bpf_userselect_search_input" id="bpf_userselect_txt_search_keyWord">',
            '                                    <span>' + lang.keyWordTip + '</span>',
            '                                </td>',
            '                            </tr>',
            '                        </table>',
            '                    </div>',
            '                </div>',
            '                <div class="bpf_userselect_definedgroup_tree bpf_userselect_content_panel" id="bpf_userselect_definedgroup_tree">',
            '                    <div class="bpf_userselect_definedgrouptree_wrap">',
            '                        <div id="bpf_userselect_grouptree" class="bpf_userselect_ztree bpf_userselect_group_ztree bpf_userselect_ztree_part"></div>',
            '                    </div>',
            '                </div>',
            '                <div id="bpf_userselect_select_container" class="bpf_userselect_user_area">',
            '                    <div class="bpf_userselect_media">',
            '                        <div class="bpf_userselect_media_left">',
            '                            <div>',
            '                                <select id="bpf_userselect_sel_unSelect_user" multiple class="bpf_userselect_form_control">',
            '                                </select>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_userselect_media_body">',
            '                            <div class="bpf_userselect_toolBtn_container">',
            '                                <div class="bpf_userselect_row">',
            '                                    <div class="bpf_userselect_col_md_12 bpf_userselect_text_center">',
            '                                        <button type="button" id="bpf_userselect_btn_up" title="' + lang.up + '" class="bpf_userselect_btn bpf_userselect_btn_link bpf_userselect_glyphiconimg bpf_userselect_glyphicon_chevron_up"></button>',
            '                                    </div>',
            '                                    <div class="bpf_userselect_col_md_12 bpf_userselect_text_center">',
            '                                        <button type="button" id="bpf_userselect_btn_add" title="' + lang.add + '" class="bpf_userselect_btn bpf_userselect_btn_link bpf_userselect_glyphiconimg bpf_userselect_glyphicon_chevron_right"></button>',
            '                                    </div>',
            '                                    <div class="bpf_userselect_col_md_12 bpf_userselect_text_center">',
            '                                        <button type="button" id="bpf_userselect_btn_remove" title="' + lang.remove + '" class="bpf_userselect_btn bpf_userselect_btn_link bpf_userselect_glyphiconimg bpf_userselect_glyphicon_chevron_left"></button>',
            '                                    </div>',
            '                                    <div class="bpf_userselect_col_md_12 bpf_userselect_text_center">',
            '                                        <button type="button" id="bpf_userselect_btn_add_all" title="' + lang.addAll + '" class="bpf_userselect_btn bpf_userselect_btn_link bpf_userselect_glyphiconimg bpf_userselect_glyphicon_chevron_forward"></button>',
            '                                    </div>',
            '                                    <div class="bpf_userselect_col_md_12 bpf_userselect_text_center">',
            '                                        <button type="button" id="bpf_userselect_btn_remove_all" title="' + lang.removeAll + '" class="bpf_userselect_btn bpf_userselect_btn_link allLeft bpf_userselect_glyphiconimg bpf_userselect_glyphicon_chevron_backward"></button>',
            '                                    </div>',
            '                                    <div class="bpf_userselect_col_md_12 bpf_userselect_text_center">',
            '                                        <button id="bpf_userselect_btn_down" title="' + lang.down + '" type="button" class="bpf_userselect_btn bpf_userselect_btn_link bpf_userselect_glyphiconimg bpf_userselect_glyphicon_chevron_down"></button>',
            '                                    </div>',
            '                                </div>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_userselect_media_right">',
            '                            <div>',
            '                                <select id="bpf_userselect_sel_selected_user" multiple class="bpf_userselect_form_control">',
            '                                </select>',
            '                            </div>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '                <div>',
            '                    <div class="bpf_userselect_actions">',
            '                        <button id="bpf_userselect_btn_search_user" type="button" class="bpf_userselect_btn" style="display: none">' + lang.search + '</button>',
            '                        <button id="bpf_userselect_btn_create" title="' + lang.createTitle + '" type="button" class="bpf_userselect_btn bpf_userselect_create" style="display: none">' + lang.create + '</button>',
            '                        <button id="bpf_userselect_btn_save" type="button" class="bpf_userselect_btn" style="display: none">' + lang.save + '</button>',
            '                        <button id="bpf_userselect_btn_model_OK" type="button" class="bpf_userselect_btn">' + lang.submit + '</button>',
            '                        <button id="bpf_userselect_btn_model_Close" type="button" class="bpf_userselect_btn">' + lang.cancel + '</button>',
            '                    </div>',
            '                </div>',
            '            </div>',
            '            <div style="display: none">',
            '                <div id="bpf_userselect_div_contextMenu">',
            '                </div>',
            '            </div>',
            '        </div>'].join("");
    }
}
//弹出层
$.fn.extend(
    {
        BPF_User_OpenDiv: function (options) {
            var $self = $(this);
            var _setting = {
                title: "",
                level: 1, //弹出层级别
                onInit: function (context) { },  //在对话框弹出时， 初始化对话框内容

                onCancel: function (context) { return true; }, //在对话框取消/关闭时， 触发的事件； 未返回为true，则无法关闭对话框
                onSubmit: function (context) { return true; }, //在对话框确定提交时， 触发的事件； 未返回为true，则无法关闭对话框
                btns: [], //自定义的按钮及事件 btns: [{ name: "test", cssClass: "", onclick: function (context) { } }] ,这是参数格式
                mode: "confirm", //or "alert" or "info"
                widthMode: "standard", //thin/small= 400px, standard=700px, wide/large=1000px
                scrollY: false, //如果弹出内容很长或者需要增长， 则设为scrollY:true
                other: "",
                isShowExit: false,
                isShowButton: false
            }
            BPF_USER_OPENDIV_OBJARRAY.push({
                Obj: $self, CancelFunc: function () {
                    if (!!_setting.onCancel($popupFrame)) {
                        $self.ClearInputs();
                        $self.ClearFloatDiv($self);
                        $self.User_CloseDiv();
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
            maskObj.data("divbox_scrolltop", $.BPF_User_ScrollPosition().Top);
            maskObj.data("divbox_scrollleft", $.BPF_User_ScrollPosition().Left);
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
                '             <input class="bpf_user_pop_input" type="button" id="closepicxxxxx"/>',
                '             <span class="bpf_user_pop_txt_title"></span>',
                //'             <img src="/images/popup/help.png" class="help" />',
                //'             <img src="/images/popup/exit.png" class="exit" style="display:none" />',
                '             <span class="bpf_user_exit" style="display:none">关闭</span>',
                '         </div>',
                '     </div>',
                '     <!--pop_titile-->',
                '     <div class="bpf_user_padding_10  bpf_user_popup_content">', //class overflow
                '     </div>',
                '     <div class="bpf_user_pop_btn line_t">',
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
                ' <div class="bpf_userselect_createPopup"></div>',
                ' <div class="bpf_userselect_createGroup">',
                '     <div class="bpf_userselect_create_top">',
                '         <span class="bpf_userselect_group_title">新建组</span>',
                '     </div>',
                '     <div class="bpf_userselect_create_cont">',
                '         <label>组名：</label>',
                '         <input id="bpf_userselect_groupName" type="text" />',
                '     </div>',
                '     <div class="bpf_userselect_create_action">',
                '         <button id="bpf_userselect_btn_popup_OK" type="button" class="bpf_userselect_btn">确定</button>',
                '         <button id="bpf_userselect_btn_popup_Close" type="button" class="bpf_userselect_btn">取消</button>',
                '     </div>',
                ' </div>',
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
                    bpf_userselect_tool.groupParam.groupFlag = false;
                    bpf_userselect_tool.groupParam.groupIsEdited = false;
                    if (!!_setting.onCancel($popupFrame)) {
                        $self.ClearInputs();
                        $self.ClearFloatDiv($self);
                        $self.User_CloseDiv();
                    }
                });
                $popupFrame.find(".bpf_user_help").click(function () {
                    //alert("TODO 显示帮助内容");
                });
                $popupFrame.find(".bpf_user_btn_submit").click(function () {
                    if (!!_setting.onSubmit($popupFrame)) {
                        $self.ClearInputs();
                        $self.ClearFloatDiv($self);
                        $self.User_CloseDiv();
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
                                    $self.User_CloseDiv();
                                }
                            });
                        }
                        btn.appendTo(btnContainer);
                    });
                }
            }
            $popupFrame.data("level", _setting.level);//缓存当前级别，如果需要多次弹出， 则需要取此值。
            $popupFrame
                .BPF_User_jqDrag($popupFrame.find(".bpf_user_dnrHandler"), $self.ClearFloatDiv) // 位置可拖拽, 第2个参数表示拖动时要“清理”的动作
                .BPF_User_jqResize($popupFrame.find(".bpf_user_resizeHandler"))      // 右下角拖拉可Resize
                .find(".bpf_user_pop_txt_title").text(_setting.title);
            $self.data("mask", maskObj); //关联遮罩
            if (_setting.onInit && $.isFunction(_setting.onInit)) {
                _setting.onInit($popupFrame);
            }
            var MyDiv_w = $popupFrame.width();
            var MyDiv_h = $popupFrame.height() + 15;
            MyDiv_w = parseInt(MyDiv_w, 10);
            MyDiv_h = parseInt(MyDiv_h, 10);
            var width = $.BPF_User_PageSize().Width;
            var height = $.BPF_User_PageSize().Height;
            var left = $.BPF_User_ScrollPosition().Left;
            var top = $.BPF_User_ScrollPosition().Top;
            var Div_topposition = top + (height / 2) - (MyDiv_h / 2);
            var Div_leftposition = left + (width / 2) - (MyDiv_w / 2) + (_setting.level - 1) * 26
            $popupFrame.css("left", Div_leftposition + "px");
            $popupFrame.css("top", Div_topposition + "px");
            if (BPF_User_browser.versions.gecko) {
                $popupFrame.show();
                return $self;
            }
            $popupFrame.fadeIn("fast");
            document.getElementById("closepicxxxxx").focus();
            return $self;
        },
        User_CloseDiv: function () {
            var $self = $(this);
            var mask = $self.data("mask")
            var $popupFrame = $self.parents(".bpf_user_popup");
            var destroy = true;
            if (destroy) {
                $popupFrame.remove();

            } else {

                if (BPF_User_browser.versions.gecko) {
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

    $.fn.contextMenuUser = function (id, options) {
        if (!menu) {                                      // Create singleton menu
            menu = $('<div id="jqContextMenu"></div>')
                .hide()
                .css({ position: 'absolute', zIndex: '99999' })
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
    $.contextMenuUser = {
        defaults: function (userDefaults) {
            $.each(userDefaults, function (i, val) {
                if (typeof val == 'object' && defaults[i]) {
                    $.extend(defaults[i], val);
                }
                else defaults[i] = val;
            });
        }
    };

})(JQZepto);

JQZepto(function () {
    JQZepto('div.contextMenu').hide();
});

(function ($) {
    var settings = {}, roots = {}, caches = {},
        //default consts of core
        _consts = {
            className: {
                BUTTON: "bpf-userselect-button",
                LEVEL: "bpf-userselect-level",
                ICO_LOADING: "bpf_userselect_ico_loading",
                SWITCH: "bpf-userselect-switch"
            },
            event: {
                NODECREATED: "ztree_nodeCreated",
                CLICK: "ztree_click",
                EXPAND: "ztree_expand",
                COLLAPSE: "ztree_collapse",
                ASYNC_SUCCESS: "ztree_async_success",
                ASYNC_ERROR: "ztree_async_error",
                REMOVE: "ztree_remove"
            },
            id: {
                A: "_a",
                ICON: "_ico",
                SPAN: "_span",
                SWITCH: "_switch",
                UL: "_ul"
            },
            line: {
                ROOT: "bpf-userselect-root",
                ROOTS: "bpf-userselect-roots",
                CENTER: "bpf-userselect-center",
                BOTTOM: "bpf-userselect-bottom",
                NOLINE: "bpf-userselect-noline",
                LINE: "bpf_userselect_line"
            },
            folder: {
                OPEN: "open",
                CLOSE: "close",
                DOCU: "docu"
            },
            node: {
                CURSELECTED: "bpf_userselect_cur_selected_node"
            }
        },
        //default setting of core
        _setting = {
            treeId: "",
            treeObj: null,
            view: {
                addDiyDom: null,
                autoCancelSelected: true,
                dblClickExpand: true,
                expandSpeed: "fast",
                fontCss: {},
                nameIsHTML: false,
                selectedMulti: true,
                showIcon: true,
                showLine: true,
                showTitle: true,
                txtSelectedEnable: false
            },
            data: {
                key: {
                    children: "children",
                    name: "name",
                    title: "",
                    url: "url"
                },
                simpleData: {
                    enable: false,
                    idKey: "id",
                    pIdKey: "pId",
                    rootPId: null
                },
                keep: {
                    parent: false,
                    leaf: false
                }
            },
            async: {
                enable: false,
                getData: function (node, func) {
                    func(data);
                }
                //type: "post",
                //dataType: "json",
                //url: "",
                //autoParam: [],
                //otherParam: [],
                //dataFilter: null,
                //isWebservice: false,
                //beforeData: function (node, setting) {
                //    var tmpParam = {};
                //    for (i = 0, l = setting.async.autoParam.length; i < l; i++) {
                //        var pKey = setting.async.autoParam[i].split("="), spKey = pKey;
                //        if (pKey.length > 1) {
                //            spKey = pKey[1];
                //            pKey = pKey[0];
                //        }
                //        if (node != undefined) {
                //            tmpParam[spKey] = node[pKey];
                //        }
                //        else {
                //            tmpParam[spKey] = null;
                //        }
                //    }
                //    if (tools.isArray(setting.async.otherParam)) {
                //        for (i = 0, l = setting.async.otherParam.length; i < l; i += 2) {
                //            tmpParam[setting.async.otherParam[i]] = setting.async.otherParam[i + 1];
                //        }
                //    } else {
                //        for (var p in setting.async.otherParam) {
                //            tmpParam[p] = setting.async.otherParam[p];
                //        }
                //    }
                //    if (setting.async.isWebservice) {
                //        tmpParam = JSON.stringify(tmpParam);
                //    }
                //    return tmpParam;
                //},
                //processData: null,
              
            },
            callback: {
                beforeAsync: null,
                beforeClick: null,
                beforeDblClick: null,
                beforeRightClick: null,
                beforeMouseDown: null,
                beforeMouseUp: null,
                beforeExpand: null,
                beforeCollapse: null,
                beforeRemove: null,

                onAsyncError: null,
                onAsyncSuccess: null,
                onNodeCreated: null,
                onClick: null,
                onDblClick: null,
                onRightClick: null,
                onMouseDown: null,
                onMouseUp: null,
                onExpand: null,
                onCollapse: null,
                onRemove: null
            }
        },
        //default root of core
        //zTree use root to save full data
        _initRoot = function (setting) {
            var r = data.getRoot(setting);
            if (!r) {
                r = {};
                data.setRoot(setting, r);
            }
            r[setting.data.key.children] = [];
            r.expandTriggerFlag = false;
            r.curSelectedList = [];
            r.noSelection = true;
            r.createdNodes = [];
            r.zId = 0;
            r._ver = (new Date()).getTime();
        },
        //default cache of core
        _initCache = function (setting) {
            var c = data.getCache(setting);
            if (!c) {
                c = {};
                data.setCache(setting, c);
            }
            c.nodes = [];
            c.doms = [];
        },
        //default bindEvent of core
        _bindEvent = function (setting) {
            var o = setting.treeObj,
                c = consts.event;
            o.bind(c.NODECREATED, function (event, treeId, node) {
                tools.apply(setting.callback.onNodeCreated, [event, treeId, node]);
            });

            o.bind(c.CLICK, function (event, srcEvent, treeId, node, clickFlag) {
                tools.apply(setting.callback.onClick, [srcEvent, treeId, node, clickFlag]);
            });

            o.bind(c.EXPAND, function (event, treeId, node) {
                tools.apply(setting.callback.onExpand, [event, treeId, node]);
            });

            o.bind(c.COLLAPSE, function (event, treeId, node) {
                tools.apply(setting.callback.onCollapse, [event, treeId, node]);
            });

            o.bind(c.ASYNC_SUCCESS, function (event, treeId, node, msg) {
                tools.apply(setting.callback.onAsyncSuccess, [event, treeId, node, msg]);
            });

            o.bind(c.ASYNC_ERROR, function (event, treeId, node, XMLHttpRequest, textStatus, errorThrown) {
                tools.apply(setting.callback.onAsyncError, [event, treeId, node, XMLHttpRequest, textStatus, errorThrown]);
            });

            o.bind(c.REMOVE, function (event, treeId, treeNode) {
                tools.apply(setting.callback.onRemove, [event, treeId, treeNode]);
            });
        },
        _unbindEvent = function (setting) {
            var o = setting.treeObj,
                c = consts.event;
            o.unbind(c.NODECREATED)
                .unbind(c.CLICK)
                .unbind(c.EXPAND)
                .unbind(c.COLLAPSE)
                .unbind(c.ASYNC_SUCCESS)
                .unbind(c.ASYNC_ERROR)
                .unbind(c.REMOVE);
        },
        //default event proxy of core
        _eventProxy = function (event) {
            var target = event.target,
                setting = data.getSetting(event.data.treeId),
                tId = "", node = null,
                nodeEventType = "", treeEventType = "",
                nodeEventCallback = null, treeEventCallback = null,
                tmp = null;

            if (tools.eqs(event.type, "mousedown")) {
                treeEventType = "mousedown";
            } else if (tools.eqs(event.type, "mouseup")) {
                treeEventType = "mouseup";
            } else if (tools.eqs(event.type, "contextmenu")) {
                treeEventType = "contextmenu";
            } else if (tools.eqs(event.type, "click")) {
                if (tools.eqs(target.tagName, "span") && target.getAttribute("treeNode" + consts.id.SWITCH) !== null) {
                    tId = tools.getNodeMainDom(target).id;
                    nodeEventType = "switchNode";
                } else {
                    tmp = tools.getMDom(setting, target, [{ tagName: "a", attrName: "treeNode" + consts.id.A }]);
                    if (tmp) {
                        tId = tools.getNodeMainDom(tmp).id;
                        nodeEventType = "clickNode";
                    }
                }
            } else if (tools.eqs(event.type, "dblclick")) {
                treeEventType = "dblclick";
                tmp = tools.getMDom(setting, target, [{ tagName: "a", attrName: "treeNode" + consts.id.A }]);
                if (tmp) {
                    tId = tools.getNodeMainDom(tmp).id;
                    nodeEventType = "switchNode";
                }
            }
            if (treeEventType.length > 0 && tId.length == 0) {
                tmp = tools.getMDom(setting, target, [{ tagName: "a", attrName: "treeNode" + consts.id.A }]);
                if (tmp) { tId = tools.getNodeMainDom(tmp).id; }
            }
            // event to node
            if (tId.length > 0) {
                node = data.getNodeCache(setting, tId);
                switch (nodeEventType) {
                    case "switchNode":
                        if (!node.isParent) {
                            nodeEventType = "";
                        } else if (tools.eqs(event.type, "click")
                            || (tools.eqs(event.type, "dblclick") && tools.apply(setting.view.dblClickExpand, [setting.treeId, node], setting.view.dblClickExpand))) {
                            nodeEventCallback = handler.onSwitchNode;
                        } else {
                            nodeEventType = "";
                        }
                        break;
                    case "clickNode":
                        nodeEventCallback = handler.onClickNode;
                        break;
                }
            }
            // event to zTree
            switch (treeEventType) {
                case "mousedown":
                    treeEventCallback = handler.onZTreeMousedown;
                    break;
                case "mouseup":
                    treeEventCallback = handler.onZTreeMouseup;
                    break;
                case "dblclick":
                    treeEventCallback = handler.onZTreeDblclick;
                    break;
                case "contextmenu":
                    treeEventCallback = handler.onZTreeContextmenu;
                    break;
            }
            var proxyResult = {
                stop: false,
                node: node,
                nodeEventType: nodeEventType,
                nodeEventCallback: nodeEventCallback,
                treeEventType: treeEventType,
                treeEventCallback: treeEventCallback
            };
            return proxyResult
        },
        //default init node of core
        _initNode = function (setting, level, n, parentNode, isFirstNode, isLastNode, openFlag) {
            if (!n) return;
            var r = data.getRoot(setting),
                childKey = setting.data.key.children;
            n.level = level;
            n.tId = setting.treeId + "_" + (++r.zId);
            n.parentTId = parentNode ? parentNode.tId : null;
            n.open = (typeof n.open == "string") ? tools.eqs(n.open, "true") : !!n.open;
            if (n[childKey] && n[childKey].length > 0) {
                n.isParent = true;
                n.zAsync = true;
            } else {
                n.isParent = (typeof n.isParent == "string") ? tools.eqs(n.isParent, "true") : !!n.isParent;
                n.open = (n.isParent && !setting.async.enable) ? n.open : false;
                n.zAsync = !n.isParent;
            }
            n.isFirstNode = isFirstNode;
            n.isLastNode = isLastNode;
            n.getParentNode = function () { return data.getNodeCache(setting, n.parentTId); };
            n.getPreNode = function () { return data.getPreNode(setting, n); };
            n.getNextNode = function () { return data.getNextNode(setting, n); };
            n.isAjaxing = false;
            data.fixPIdKeyValue(setting, n);
        },
        _init = {
            bind: [_bindEvent],
            unbind: [_unbindEvent],
            caches: [_initCache],
            nodes: [_initNode],
            proxys: [_eventProxy],
            roots: [_initRoot],
            beforeA: [],
            afterA: [],
            innerBeforeA: [],
            innerAfterA: [],
            zTreeTools: []
        },
        //method of operate data
        data = {
            addNodeCache: function (setting, node) {
                data.getCache(setting).nodes[data.getNodeCacheId(node.tId)] = node;
            },
            getNodeCacheId: function (tId) {
                return tId.substring(tId.lastIndexOf("_") + 1);
            },
            addAfterA: function (afterA) {
                _init.afterA.push(afterA);
            },
            addBeforeA: function (beforeA) {
                _init.beforeA.push(beforeA);
            },
            addInnerAfterA: function (innerAfterA) {
                _init.innerAfterA.push(innerAfterA);
            },
            addInnerBeforeA: function (innerBeforeA) {
                _init.innerBeforeA.push(innerBeforeA);
            },
            addInitBind: function (bindEvent) {
                _init.bind.push(bindEvent);
            },
            addInitUnBind: function (unbindEvent) {
                _init.unbind.push(unbindEvent);
            },
            addInitCache: function (initCache) {
                _init.caches.push(initCache);
            },
            addInitNode: function (initNode) {
                _init.nodes.push(initNode);
            },
            addInitProxy: function (initProxy, isFirst) {
                if (!!isFirst) {
                    _init.proxys.splice(0, 0, initProxy);
                } else {
                    _init.proxys.push(initProxy);
                }
            },
            addInitRoot: function (initRoot) {
                _init.roots.push(initRoot);
            },
            addNodesData: function (setting, parentNode, nodes) {
                var childKey = setting.data.key.children;
                if (!parentNode[childKey]) parentNode[childKey] = [];
                if (parentNode[childKey].length > 0) {
                    parentNode[childKey][parentNode[childKey].length - 1].isLastNode = false;
                    view.setNodeLineIcos(setting, parentNode[childKey][parentNode[childKey].length - 1]);
                }
                parentNode.isParent = true;
                parentNode[childKey] = parentNode[childKey].concat(nodes);
            },
            addSelectedNode: function (setting, node) {
                var root = data.getRoot(setting);
                if (!data.isSelectedNode(setting, node)) {
                    root.curSelectedList.push(node);
                }
            },
            addCreatedNode: function (setting, node) {
                if (!!setting.callback.onNodeCreated || !!setting.view.addDiyDom) {
                    var root = data.getRoot(setting);
                    root.createdNodes.push(node);
                }
            },
            addZTreeTools: function (zTreeTools) {
                _init.zTreeTools.push(zTreeTools);
            },
            exSetting: function (s) {
                $.extend(true, _setting, s);
            },
            fixPIdKeyValue: function (setting, node) {
                if (setting.data.simpleData.enable) {
                    node[setting.data.simpleData.pIdKey] = node.parentTId ? node.getParentNode()[setting.data.simpleData.idKey] : setting.data.simpleData.rootPId;
                }
            },
            getAfterA: function (setting, node, array) {
                for (var i = 0, j = _init.afterA.length; i < j; i++) {
                    _init.afterA[i].apply(this, arguments);
                }
            },
            getBeforeA: function (setting, node, array) {
                for (var i = 0, j = _init.beforeA.length; i < j; i++) {
                    _init.beforeA[i].apply(this, arguments);
                }
            },
            getInnerAfterA: function (setting, node, array) {
                for (var i = 0, j = _init.innerAfterA.length; i < j; i++) {
                    _init.innerAfterA[i].apply(this, arguments);
                }
            },
            getInnerBeforeA: function (setting, node, array) {
                for (var i = 0, j = _init.innerBeforeA.length; i < j; i++) {
                    _init.innerBeforeA[i].apply(this, arguments);
                }
            },
            getCache: function (setting) {
                return caches[setting.treeId];
            },
            getNextNode: function (setting, node) {
                if (!node) return null;
                var childKey = setting.data.key.children,
                    p = node.parentTId ? node.getParentNode() : data.getRoot(setting);
                for (var i = 0, l = p[childKey].length - 1; i <= l; i++) {
                    if (p[childKey][i] === node) {
                        return (i == l ? null : p[childKey][i + 1]);
                    }
                }
                return null;
            },
            getNodeByParam: function (setting, nodes, key, value) {
                if (!nodes || !key) return null;
                var childKey = setting.data.key.children;
                for (var i = 0, l = nodes.length; i < l; i++) {
                    if (nodes[i][key] == value) {
                        return nodes[i];
                    }
                    var tmp = data.getNodeByParam(setting, nodes[i][childKey], key, value);
                    if (tmp) return tmp;
                }
                return null;
            },
            getNodeCache: function (setting, tId) {
                if (!tId) return null;
                var n = caches[setting.treeId].nodes[data.getNodeCacheId(tId)];
                return n ? n : null;
            },
            getNodeName: function (setting, node) {
                var nameKey = setting.data.key.name;
                return "" + node[nameKey];
            },
            getNodeTitle: function (setting, node) {
                var t = setting.data.key.title === "" ? setting.data.key.name : setting.data.key.title;
                return "" + node[t];
            },
            getNodes: function (setting) {
                return data.getRoot(setting)[setting.data.key.children];
            },
            getNodesByParam: function (setting, nodes, key, value) {
                if (!nodes || !key) return [];
                var childKey = setting.data.key.children,
                    result = [];
                for (var i = 0, l = nodes.length; i < l; i++) {
                    if (nodes[i][key] == value) {
                        result.push(nodes[i]);
                    }
                    result = result.concat(data.getNodesByParam(setting, nodes[i][childKey], key, value));
                }
                return result;
            },
            getNodesByParamFuzzy: function (setting, nodes, key, value) {
                if (!nodes || !key) return [];
                var childKey = setting.data.key.children,
                    result = [];
                value = value.toLowerCase();
                for (var i = 0, l = nodes.length; i < l; i++) {
                    if (typeof nodes[i][key] == "string" && nodes[i][key].toLowerCase().indexOf(value) > -1) {
                        result.push(nodes[i]);
                    }
                    result = result.concat(data.getNodesByParamFuzzy(setting, nodes[i][childKey], key, value));
                }
                return result;
            },
            getNodesByFilter: function (setting, nodes, filter, isSingle, invokeParam) {
                if (!nodes) return (isSingle ? null : []);
                var childKey = setting.data.key.children,
                    result = isSingle ? null : [];
                for (var i = 0, l = nodes.length; i < l; i++) {
                    if (tools.apply(filter, [nodes[i], invokeParam], false)) {
                        if (isSingle) { return nodes[i]; }
                        result.push(nodes[i]);
                    }
                    var tmpResult = data.getNodesByFilter(setting, nodes[i][childKey], filter, isSingle, invokeParam);
                    if (isSingle && !!tmpResult) { return tmpResult; }
                    result = isSingle ? tmpResult : result.concat(tmpResult);
                }
                return result;
            },
            getPreNode: function (setting, node) {
                if (!node) return null;
                var childKey = setting.data.key.children,
                    p = node.parentTId ? node.getParentNode() : data.getRoot(setting);
                for (var i = 0, l = p[childKey].length; i < l; i++) {
                    if (p[childKey][i] === node) {
                        return (i == 0 ? null : p[childKey][i - 1]);
                    }
                }
                return null;
            },
            getRoot: function (setting) {
                return setting ? roots[setting.treeId] : null;
            },
            getRoots: function () {
                return roots;
            },
            getSetting: function (treeId) {
                return settings[treeId];
            },
            getSettings: function () {
                return settings;
            },
            getZTreeTools: function (treeId) {
                var r = this.getRoot(this.getSetting(treeId));
                return r ? r.treeTools : null;
            },
            initCache: function (setting) {
                for (var i = 0, j = _init.caches.length; i < j; i++) {
                    _init.caches[i].apply(this, arguments);
                }
            },
            initNode: function (setting, level, node, parentNode, preNode, nextNode) {
                for (var i = 0, j = _init.nodes.length; i < j; i++) {
                    _init.nodes[i].apply(this, arguments);
                }
            },
            initRoot: function (setting) {
                for (var i = 0, j = _init.roots.length; i < j; i++) {
                    _init.roots[i].apply(this, arguments);
                }
            },
            isSelectedNode: function (setting, node) {
                var root = data.getRoot(setting);
                for (var i = 0, j = root.curSelectedList.length; i < j; i++) {
                    if (node === root.curSelectedList[i]) return true;
                }
                return false;
            },
            removeNodeCache: function (setting, node) {
                var childKey = setting.data.key.children;
                if (node[childKey]) {
                    for (var i = 0, l = node[childKey].length; i < l; i++) {
                        arguments.callee(setting, node[childKey][i]);
                    }
                }
                data.getCache(setting).nodes[data.getNodeCacheId(node.tId)] = null;
            },
            removeSelectedNode: function (setting, node) {
                var root = data.getRoot(setting);
                for (var i = 0, j = root.curSelectedList.length; i < j; i++) {
                    if (node === root.curSelectedList[i] || !data.getNodeCache(setting, root.curSelectedList[i].tId)) {
                        root.curSelectedList.splice(i, 1);
                        i--; j--;
                    }
                }
            },
            setCache: function (setting, cache) {
                caches[setting.treeId] = cache;
            },
            setRoot: function (setting, root) {
                roots[setting.treeId] = root;
            },
            setZTreeTools: function (setting, zTreeTools) {
                for (var i = 0, j = _init.zTreeTools.length; i < j; i++) {
                    _init.zTreeTools[i].apply(this, arguments);
                }
            },
            transformToArrayFormat: function (setting, nodes) {
                if (!nodes) return [];
                var childKey = setting.data.key.children,
                    r = [];
                if (tools.isArray(nodes)) {
                    for (var i = 0, l = nodes.length; i < l; i++) {
                        r.push(nodes[i]);
                        if (nodes[i][childKey])
                            r = r.concat(data.transformToArrayFormat(setting, nodes[i][childKey]));
                    }
                } else {
                    r.push(nodes);
                    if (nodes[childKey])
                        r = r.concat(data.transformToArrayFormat(setting, nodes[childKey]));
                }
                return r;
            },
            transformTozTreeFormat: function (setting, sNodes) {
                var i, l,
                    key = setting.data.simpleData.idKey,
                    parentKey = setting.data.simpleData.pIdKey,
                    childKey = setting.data.key.children;
                if (!key || key == "" || !sNodes) return [];

                if (tools.isArray(sNodes)) {
                    var r = [];
                    var tmpMap = [];
                    for (i = 0, l = sNodes.length; i < l; i++) {
                        tmpMap[sNodes[i][key]] = sNodes[i];
                    }
                    for (i = 0, l = sNodes.length; i < l; i++) {
                        if (tmpMap[sNodes[i][parentKey]] && sNodes[i][key] != sNodes[i][parentKey]) {
                            if (!tmpMap[sNodes[i][parentKey]][childKey])
                                tmpMap[sNodes[i][parentKey]][childKey] = [];
                            tmpMap[sNodes[i][parentKey]][childKey].push(sNodes[i]);
                        } else {
                            r.push(sNodes[i]);
                        }
                    }
                    return r;
                } else {
                    return [sNodes];
                }
            }
        },
        //method of event proxy
        event = {
            bindEvent: function (setting) {
                for (var i = 0, j = _init.bind.length; i < j; i++) {
                    _init.bind[i].apply(this, arguments);
                }
            },
            unbindEvent: function (setting) {
                for (var i = 0, j = _init.unbind.length; i < j; i++) {
                    _init.unbind[i].apply(this, arguments);
                }
            },
            bindTree: function (setting) {
                var eventParam = {
                    treeId: setting.treeId
                },
                    o = setting.treeObj;
                if (!setting.view.txtSelectedEnable) {
                    // for can't select text
                    o.bind('selectstart', function (e) {
                        var node
                        var originalEvent = e.originalEvent;
                        var srcElement;
                        if (originalEvent == undefined) {
                            srcElement = e.srcElement;
                        }
                        else {
                            srcElement = originalEvent.srcElement;
                        }
                        var n = srcElement.nodeName.toLowerCase();

                        return (n === "input" || n === "textarea");
                    }).css({
                        "-moz-user-select": "-moz-none"
                    });
                }
                o.bind('click', eventParam, event.proxy);
                o.bind('dblclick', eventParam, event.proxy);
                o.bind('mouseover', eventParam, event.proxy);
                o.bind('mouseout', eventParam, event.proxy);
                o.bind('mousedown', eventParam, event.proxy);
                o.bind('mouseup', eventParam, event.proxy);
                o.bind('contextmenu', eventParam, event.proxy);
            },
            unbindTree: function (setting) {
                var o = setting.treeObj;
                o.unbind('click', event.proxy)
                    .unbind('dblclick', event.proxy)
                    .unbind('mouseover', event.proxy)
                    .unbind('mouseout', event.proxy)
                    .unbind('mousedown', event.proxy)
                    .unbind('mouseup', event.proxy)
                    .unbind('contextmenu', event.proxy);
            },
            doProxy: function (e) {
                var results = [];
                for (var i = 0, j = _init.proxys.length; i < j; i++) {
                    var proxyResult = _init.proxys[i].apply(this, arguments);
                    results.push(proxyResult);
                    if (proxyResult.stop) {
                        break;
                    }
                }
                return results;
            },
            proxy: function (e) {
                var setting = data.getSetting(e.data.treeId);
                if (!tools.uCanDo(setting, e)) return true;
                var results = event.doProxy(e),
                    r = true, x = false;
                for (var i = 0, l = results.length; i < l; i++) {
                    var proxyResult = results[i];
                    if (proxyResult.nodeEventCallback) {
                        x = true;
                        r = proxyResult.nodeEventCallback.apply(proxyResult, [e, proxyResult.node]) && r;
                    }
                    if (proxyResult.treeEventCallback) {
                        x = true;
                        r = proxyResult.treeEventCallback.apply(proxyResult, [e, proxyResult.node]) && r;
                    }
                }
                return r;
            }
        },
        //method of event handler
        handler = {
            onSwitchNode: function (event, node) {
                var setting = data.getSetting(event.data.treeId);
                if (node.open) {
                    if (tools.apply(setting.callback.beforeCollapse, [setting.treeId, node], true) == false) return true;
                    data.getRoot(setting).expandTriggerFlag = true;
                    view.switchNode(setting, node);
                } else {
                    if (tools.apply(setting.callback.beforeExpand, [setting.treeId, node], true) == false) return true;
                    data.getRoot(setting).expandTriggerFlag = true;
                    view.switchNode(setting, node);
                }
                return true;
            },
            onClickNode: function (event, node) {
                var setting = data.getSetting(event.data.treeId),
                    clickFlag = ((setting.view.autoCancelSelected && (event.ctrlKey || event.metaKey)) && data.isSelectedNode(setting, node)) ? 0 : (setting.view.autoCancelSelected && (event.ctrlKey || event.metaKey) && setting.view.selectedMulti) ? 2 : 1;
                if (tools.apply(setting.callback.beforeClick, [setting.treeId, node, clickFlag], true) == false) return true;
                if (clickFlag === 0) {
                    view.cancelPreSelectedNode(setting, node);
                } else {
                    view.selectNode(setting, node, clickFlag === 2);
                }
                setting.treeObj.trigger(consts.event.CLICK, [event, setting.treeId, node, clickFlag]);
                return true;
            },
            onZTreeMousedown: function (event, node) {
                var setting = data.getSetting(event.data.treeId);
                if (tools.apply(setting.callback.beforeMouseDown, [setting.treeId, node], true)) {
                    tools.apply(setting.callback.onMouseDown, [event, setting.treeId, node]);
                }
                return true;
            },
            onZTreeMouseup: function (event, node) {
                var setting = data.getSetting(event.data.treeId);
                if (tools.apply(setting.callback.beforeMouseUp, [setting.treeId, node], true)) {
                    tools.apply(setting.callback.onMouseUp, [event, setting.treeId, node]);
                }
                return true;
            },
            onZTreeDblclick: function (event, node) {
                var setting = data.getSetting(event.data.treeId);
                if (tools.apply(setting.callback.beforeDblClick, [setting.treeId, node], true)) {
                    tools.apply(setting.callback.onDblClick, [event, setting.treeId, node]);
                }
                return true;
            },
            onZTreeContextmenu: function (event, node) {
                var setting = data.getSetting(event.data.treeId);
                if (tools.apply(setting.callback.beforeRightClick, [setting.treeId, node], true)) {
                    tools.apply(setting.callback.onRightClick, [event, setting.treeId, node]);
                }
                return (typeof setting.callback.onRightClick) != "function";
            }
        },
        //method of tools for zTree
        tools = {
            apply: function (fun, param, defaultValue) {
                if ((typeof fun) == "function") {
                    return fun.apply(ztUser, param ? param : []);
                }
                return defaultValue;
            },
            canAsync: function (setting, node) {
                var childKey = setting.data.key.children;
                return (setting.async.enable && node && node.isParent && !(node.zAsync || (node[childKey] && node[childKey].length > 0)));
            },
            clone: function (obj) {
                if (obj === null) return null;
                var o = tools.isArray(obj) ? [] : {};
                for (var i in obj) {
                    o[i] = (obj[i] instanceof Date) ? new Date(obj[i].getTime()) : (typeof obj[i] === "object" ? arguments.callee(obj[i]) : obj[i]);
                }
                return o;
            },
            eqs: function (str1, str2) {
                return str1.toLowerCase() === str2.toLowerCase();
            },
            isArray: function (arr) {
                return Object.prototype.toString.apply(arr) === "[object Array]";
            },
            $: function (node, exp, setting) {
                if (!!exp && typeof exp != "string") {
                    setting = exp;
                    exp = "";
                }
                if (typeof node == "string") {
                    return JQZepto(node, setting ? setting.treeObj.get(0).ownerDocument : null);
                } else {
                    return JQZepto("#" + node.tId + exp, setting ? setting.treeObj : null);
                }
            },
            getMDom: function (setting, curDom, targetExpr) {
                if (!curDom) return null;
                while (curDom && curDom.id !== setting.treeId) {
                    for (var i = 0, l = targetExpr.length; curDom.tagName && i < l; i++) {
                        if (tools.eqs(curDom.tagName, targetExpr[i].tagName) && curDom.getAttribute(targetExpr[i].attrName) !== null) {
                            return curDom;
                        }
                    }
                    curDom = curDom.parentNode;
                }
                return null;
            },
            getNodeMainDom: function (target) {
                return ($(target).parent("li").get(0) || $(target).parentsUntil("li").parent().get(0));
            },
            isChildOrSelf: function (dom, parentId) {
                return ($(dom).closest("#" + parentId).length > 0);
            },
            uCanDo: function (setting, e) {
                return true;
            }
        },
        //method of operate ztree dom
        view = {
            addNodes: function (setting, parentNode, newNodes, isSilent) {
                if (setting.data.keep.leaf && parentNode && !parentNode.isParent) {
                    return;
                }
                if (!tools.isArray(newNodes)) {
                    newNodes = [newNodes];
                }
                if (setting.data.simpleData.enable) {
                    newNodes = data.transformTozTreeFormat(setting, newNodes);
                }
                if (parentNode) {
                    var target_switchObj = $$(parentNode, consts.id.SWITCH, setting),
                        target_icoObj = $$(parentNode, consts.id.ICON, setting),
                        target_ulObj = $$(parentNode, consts.id.UL, setting);

                    if (!parentNode.open) {
                        view.replaceSwitchClass(parentNode, target_switchObj, consts.folder.CLOSE);
                        view.replaceIcoClass(parentNode, target_icoObj, consts.folder.CLOSE);
                        parentNode.open = false;
                        target_ulObj.css({
                            "display": "none"
                        });
                    }

                    data.addNodesData(setting, parentNode, newNodes);
                    view.createNodes(setting, parentNode.level + 1, newNodes, parentNode);
                    if (!isSilent) {
                        view.expandCollapseParentNode(setting, parentNode, true);
                    }
                } else {
                    data.addNodesData(setting, data.getRoot(setting), newNodes);
                    view.createNodes(setting, 0, newNodes, null);
                }
            },
            appendNodes: function (setting, level, nodes, parentNode, initFlag, openFlag) {
                if (!nodes) return [];
                var html = [],
                    childKey = setting.data.key.children;
                for (var i = 0, l = nodes.length; i < l; i++) {
                    var node = nodes[i];
                    if (initFlag) {
                        var tmpPNode = (parentNode) ? parentNode : data.getRoot(setting),
                            tmpPChild = tmpPNode[childKey],
                            isFirstNode = ((tmpPChild.length == nodes.length) && (i == 0)),
                            isLastNode = (i == (nodes.length - 1));
                        data.initNode(setting, level, node, parentNode, isFirstNode, isLastNode, openFlag);
                        data.addNodeCache(setting, node);
                    }

                    var childHtml = [];
                    if (node[childKey] && node[childKey].length > 0) {
                        //make child html first, because checkType
                        childHtml = view.appendNodes(setting, level + 1, node[childKey], node, initFlag, openFlag && node.open);
                    }
                    if (openFlag) {

                        view.makeDOMNodeMainBefore(html, setting, node);
                        view.makeDOMNodeLine(html, setting, node);
                        data.getBeforeA(setting, node, html);
                        view.makeDOMNodeNameBefore(html, setting, node);
                        data.getInnerBeforeA(setting, node, html);
                        view.makeDOMNodeIcon(html, setting, node);
                        data.getInnerAfterA(setting, node, html);
                        view.makeDOMNodeNameAfter(html, setting, node);
                        data.getAfterA(setting, node, html);
                        if (node.isParent && node.open) {
                            view.makeUlHtml(setting, node, html, childHtml.join(''));
                        }
                        view.makeDOMNodeMainAfter(html, setting, node);
                        data.addCreatedNode(setting, node);
                    }
                }
                return html;
            },
            appendParentULDom: function (setting, node) {
                var html = [],
                    nObj = $$(node, setting);
                if (!nObj.get(0) && !!node.parentTId) {
                    view.appendParentULDom(setting, node.getParentNode());
                    nObj = $$(node, setting);
                }
                var ulObj = $$(node, consts.id.UL, setting);
                if (ulObj.get(0)) {
                    ulObj.remove();
                }
                var childKey = setting.data.key.children,
                    childHtml = view.appendNodes(setting, node.level + 1, node[childKey], node, false, true);
                view.makeUlHtml(setting, node, html, childHtml.join(''));
                nObj.append(html.join(''));
            },
            asyncNode: function (setting, node, isSilent, callback) {
                var i, l;
                if (node && !node.isParent) {
                    tools.apply(callback);
                    return false;
                } else if (node && node.isAjaxing) {
                    return false;
                } else if (tools.apply(setting.callback.beforeAsync, [setting.treeId, node], true) == false) {
                    tools.apply(callback);
                    return false;
                }
                if (node) {
                    node.isAjaxing = true;
                    var icoObj = $$(node, consts.id.ICON, setting);
                    icoObj.attr({ "style": "", "class": consts.className.BUTTON + " " + consts.className.ICO_LOADING });
                }
                //var tmpParam = setting.async.beforeData(node, setting);
                var _tmpV = data.getRoot(setting)._ver;

                setting.async.getData(node, function (msg) {
                    if (_tmpV != data.getRoot(setting)._ver) {
                        return;
                    }
                    var newNodes = msg;
                    //try {
                    //    if (!msg || msg.length == 0) {
                    //        newNodes = [];
                    //    }
                    //    else if (setting.async.processData != null) {
                    //        newNodes = setting.async.processData(msg);
                    //    }
                    //    else if (typeof msg == "string") {
                    //        newNodes = eval("(" + msg + ")");
                    //    } else {
                    //        newNodes = msg;
                    //    }
                    //} catch (err) {
                    //    newNodes = msg;
                    //}

                    if (node) {
                        node.isAjaxing = null;
                        node.zAsync = true;
                    }
                    view.setNodeLineIcos(setting, node);
                    if (newNodes && newNodes !== "") {
                        newNodes = tools.apply(setting.async.dataFilter, [setting.treeId, node, newNodes], newNodes);
                        view.addNodes(setting, node, !!newNodes ? tools.clone(newNodes) : [], !!isSilent);
                    } else {
                        view.addNodes(setting, node, [], !!isSilent);
                    }
                    setting.treeObj.trigger(consts.event.ASYNC_SUCCESS, [setting.treeId, node, msg]);
                    tools.apply(callback);
                });

                //$.ajax({
                //    cache: false,
                //    type: setting.async.type,
                //    url: tools.apply(setting.async.url, [setting.treeId, node], setting.async.url),
                //    data: tmpParam,
                //    async: true,
                //    beforeSend: function () {
                //        if (node == undefined) {
                //            bpf_sdk_tool.showLoading();
                //        }
                //    },
                //    dataType: setting.async.dataType,
                //    success: function (msg) {
                //        bpf_sdk_tool.hideLoading();
                //        if (_tmpV != data.getRoot(setting)._ver) {
                //            return;
                //        }
                //        var newNodes = [];
                //        try {
                //            if (!msg || msg.length == 0) {
                //                newNodes = [];
                //            }
                //            else if (setting.async.processData != null) {
                //                newNodes = setting.async.processData(msg);
                //            }
                //            else if (typeof msg == "string") {
                //                newNodes = eval("(" + msg + ")");
                //            } else {
                //                newNodes = msg;
                //            }
                //        } catch (err) {
                //            newNodes = msg;
                //        }

                //        if (node) {
                //            node.isAjaxing = null;
                //            node.zAsync = true;
                //        }
                //        view.setNodeLineIcos(setting, node);
                //        if (newNodes && newNodes !== "") {
                //            newNodes = tools.apply(setting.async.dataFilter, [setting.treeId, node, newNodes], newNodes);
                //            view.addNodes(setting, node, !!newNodes ? tools.clone(newNodes) : [], !!isSilent);
                //        } else {
                //            view.addNodes(setting, node, [], !!isSilent);
                //        }
                //        setting.treeObj.trigger(consts.event.ASYNC_SUCCESS, [setting.treeId, node, msg]);
                //        tools.apply(callback);
                //    },
                //    error: function (XMLHttpRequest, textStatus, errorThrown) {
                //        bpf_sdk_tool.hideLoading();
                //        if (_tmpV != data.getRoot(setting)._ver) {
                //            return;
                //        }
                //        if (node) node.isAjaxing = null;
                //        view.setNodeLineIcos(setting, node);
                //        setting.treeObj.trigger(consts.event.ASYNC_ERROR, [setting.treeId, node, XMLHttpRequest, textStatus, errorThrown]);
                //    }
                //});
                return true;
            },
            cancelPreSelectedNode: function (setting, node) {
                var list = data.getRoot(setting).curSelectedList;
                for (var i = 0, j = list.length - 1; j >= i; j--) {
                    if (!node || node === list[j]) {
                        $$(list[j], consts.id.A, setting).removeClass(consts.node.CURSELECTED);
                        if (node) {
                            data.removeSelectedNode(setting, node);
                            break;
                        }
                    }
                }
                if (!node) data.getRoot(setting).curSelectedList = [];
            },
            createNodeCallback: function (setting) {
                if (!!setting.callback.onNodeCreated || !!setting.view.addDiyDom) {
                    var root = data.getRoot(setting);
                    while (root.createdNodes.length > 0) {
                        var node = root.createdNodes.shift();
                        tools.apply(setting.view.addDiyDom, [setting.treeId, node]);
                        if (!!setting.callback.onNodeCreated) {
                            setting.treeObj.trigger(consts.event.NODECREATED, [setting.treeId, node]);
                        }
                    }
                }
            },
            createNodes: function (setting, level, nodes, parentNode) {
                if (!nodes || nodes.length == 0) return;
                var root = data.getRoot(setting),
                    childKey = setting.data.key.children,
                    openFlag = !parentNode || parentNode.open || !!$$(parentNode[childKey][0], setting).get(0);
                root.createdNodes = [];
                var zTreeHtml = view.appendNodes(setting, level, nodes, parentNode, true, openFlag);
                if (!parentNode) {
                    setting.treeObj.append(zTreeHtml.join(''));
                } else {
                    var ulObj = $$(parentNode, consts.id.UL, setting);
                    if (ulObj.get(0)) {
                        ulObj.append(zTreeHtml.join(''));
                    }
                }
                view.createNodeCallback(setting);
            },
            destroy: function (setting) {
                if (!setting) return;
                data.initCache(setting);
                data.initRoot(setting);
                event.unbindTree(setting);
                event.unbindEvent(setting);
                setting.treeObj.empty();
                delete settings[setting.treeId];
            },
            expandCollapseNode: function (setting, node, expandFlag, animateFlag, callback) {
                var root = data.getRoot(setting),
                    childKey = setting.data.key.children;
                if (!node) {
                    tools.apply(callback, []);
                    return;
                }
                if (root.expandTriggerFlag) {
                    var _callback = callback;
                    callback = function () {
                        if (_callback) _callback();
                        if (node.open) {
                            setting.treeObj.trigger(consts.event.EXPAND, [setting.treeId, node]);
                        } else {
                            setting.treeObj.trigger(consts.event.COLLAPSE, [setting.treeId, node]);
                        }
                    };
                    root.expandTriggerFlag = false;
                }
                if (!node.open && node.isParent && ((!$$(node, consts.id.UL, setting).get(0)) || (node[childKey] && node[childKey].length > 0 && !$$(node[childKey][0], setting).get(0)))) {
                    view.appendParentULDom(setting, node);
                    view.createNodeCallback(setting);
                }
                if (node.open == expandFlag) {
                    tools.apply(callback, []);
                    return;
                }
                var ulObj = $$(node, consts.id.UL, setting),
                    switchObj = $$(node, consts.id.SWITCH, setting),
                    icoObj = $$(node, consts.id.ICON, setting);

                if (node.isParent) {
                    node.open = !node.open;
                    if (node.iconOpen && node.iconClose) {
                        icoObj.attr("style", view.makeNodeIcoStyle(setting, node));
                    }

                    if (node.open) {
                        view.replaceSwitchClass(node, switchObj, consts.folder.OPEN);
                        view.replaceIcoClass(node, icoObj, consts.folder.OPEN);
                        if (animateFlag == false || setting.view.expandSpeed == "") {
                            ulObj.show();
                            tools.apply(callback, []);
                        } else {
                            if (node[childKey] && node[childKey].length > 0) {
                                ulObj.slideDown(setting.view.expandSpeed, callback);
                            } else {
                                ulObj.show();
                                tools.apply(callback, []);
                            }
                        }
                    } else {
                        view.replaceSwitchClass(node, switchObj, consts.folder.CLOSE);
                        view.replaceIcoClass(node, icoObj, consts.folder.CLOSE);
                        if (animateFlag == false || setting.view.expandSpeed == "" || !(node[childKey] && node[childKey].length > 0)) {
                            ulObj.hide();
                            tools.apply(callback, []);
                        } else {
                            ulObj.slideUp(setting.view.expandSpeed, callback);
                        }
                    }
                } else {
                    tools.apply(callback, []);
                }
            },
            expandCollapseParentNode: function (setting, node, expandFlag, animateFlag, callback) {
                if (!node) return;
                if (!node.parentTId) {
                    view.expandCollapseNode(setting, node, expandFlag, animateFlag, callback);
                    return;
                } else {
                    view.expandCollapseNode(setting, node, expandFlag, animateFlag);
                }
                if (node.parentTId) {
                    view.expandCollapseParentNode(setting, node.getParentNode(), expandFlag, animateFlag, callback);
                }
            },
            expandCollapseSonNode: function (setting, node, expandFlag, animateFlag, callback) {
                var root = data.getRoot(setting),
                    childKey = setting.data.key.children,
                    treeNodes = (node) ? node[childKey] : root[childKey],
                    selfAnimateSign = (node) ? false : animateFlag,
                    expandTriggerFlag = data.getRoot(setting).expandTriggerFlag;
                data.getRoot(setting).expandTriggerFlag = false;
                if (treeNodes) {
                    for (var i = 0, l = treeNodes.length; i < l; i++) {
                        if (treeNodes[i]) view.expandCollapseSonNode(setting, treeNodes[i], expandFlag, selfAnimateSign);
                    }
                }
                data.getRoot(setting).expandTriggerFlag = expandTriggerFlag;
                view.expandCollapseNode(setting, node, expandFlag, animateFlag, callback);
            },
            makeDOMNodeIcon: function (html, setting, node) {
                var nameStr = data.getNodeName(setting, node),
                    name = setting.view.nameIsHTML ? nameStr : nameStr.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
                html.push("<span id='", node.tId, consts.id.ICON,
                    "' title='' treeNode", consts.id.ICON, " class='", view.makeNodeIcoClass(setting, node),
                    "' style='", view.makeNodeIcoStyle(setting, node), "'></span><span id='", node.tId, consts.id.SPAN,
                    "'>", name, "</span>");
            },
            makeDOMNodeLine: function (html, setting, node) {
                html.push("<span id='", node.tId, consts.id.SWITCH, "' title='' class='", view.makeNodeLineClass(setting, node), "' treeNode", consts.id.SWITCH, "></span>");
            },
            makeDOMNodeMainAfter: function (html, setting, node) {
                html.push("</li>");
            },
            makeDOMNodeMainBefore: function (html, setting, node) {
                html.push("<li id='", node.tId, "' class='", consts.className.LEVEL, node.level, "' tabindex='0' hidefocus='true' treenode>");
            },
            makeDOMNodeNameAfter: function (html, setting, node) {
                html.push("</a>");
            },
            makeDOMNodeNameBefore: function (html, setting, node) {
                var title = data.getNodeTitle(setting, node),
                    url = view.makeNodeUrl(setting, node),
                    fontcss = view.makeNodeFontCss(setting, node),
                    fontStyle = [];
                for (var f in fontcss) {
                    fontStyle.push(f, ":", fontcss[f], ";");
                }
                html.push("<a id='", node.tId, consts.id.A, "' class='", consts.className.LEVEL, node.level, "' treeNode", consts.id.A, " onclick=\"", (node.click || ''),
                    "\" ", ((url != null && url.length > 0) ? "href='" + url + "'" : ""), " target='", view.makeNodeTarget(node), "' style='", fontStyle.join(''),
                    "'");
                if (tools.apply(setting.view.showTitle, [setting.treeId, node], setting.view.showTitle) && title) { html.push("title='", title.replace(/'/g, "&#39;").replace(/</g, '&lt;').replace(/>/g, '&gt;'), "'"); }
                html.push(">");
            },
            makeNodeFontCss: function (setting, node) {
                var fontCss = tools.apply(setting.view.fontCss, [setting.treeId, node], setting.view.fontCss);
                return (fontCss && ((typeof fontCss) != "function")) ? fontCss : {};
            },
            makeNodeIcoClass: function (setting, node) {
                var icoCss = ["bpf-userselect-ico"];
                if (!node.isAjaxing) {
                    icoCss[0] = (node.iconSkin ? node.iconSkin + "_" : "") + icoCss[0];
                    if (node.isParent) {
                        icoCss.push(node.open ? consts.folder.OPEN : consts.folder.CLOSE);
                    } else {
                        icoCss.push(consts.folder.DOCU);
                    }
                }
                return consts.className.BUTTON + " " + icoCss.join('_');
            },
            makeNodeIcoStyle: function (setting, node) {
                var icoStyle = [];
                if (!node.isAjaxing) {
                    var icon = (node.isParent && node.iconOpen && node.iconClose) ? (node.open ? node.iconOpen : node.iconClose) : node.icon;
                    if (icon) icoStyle.push("background:url(", icon, ") 0 0 no-repeat;");
                    if (setting.view.showIcon == false || !tools.apply(setting.view.showIcon, [setting.treeId, node], true)) {
                        icoStyle.push("width:0px;height:0px;");
                    }
                }
                return icoStyle.join('');
            },
            makeNodeLineClass: function (setting, node) {
                var lineClass = [];
                if (setting.view.showLine) {
                    if (node.level == 0 && node.isFirstNode && node.isLastNode) {
                        lineClass.push(consts.line.ROOT);
                    } else if (node.level == 0 && node.isFirstNode) {
                        lineClass.push(consts.line.ROOTS);
                    } else if (node.isLastNode) {
                        lineClass.push(consts.line.BOTTOM);
                    } else {
                        lineClass.push(consts.line.CENTER);
                    }
                } else {
                    lineClass.push(consts.line.NOLINE);
                }
                if (node.isParent) {
                    lineClass.push(node.open ? consts.folder.OPEN : consts.folder.CLOSE);
                } else {
                    lineClass.push(consts.folder.DOCU);
                }
                return view.makeNodeLineClassEx(node) + lineClass.join('_');
            },
            makeNodeLineClassEx: function (node) {
                return consts.className.BUTTON + " " + consts.className.LEVEL + node.level + " " + consts.className.SWITCH + " ";
            },
            makeNodeTarget: function (node) {
                return (node.target || "_blank");
            },
            makeNodeUrl: function (setting, node) {
                var urlKey = setting.data.key.url;
                return node[urlKey] ? node[urlKey] : null;
            },
            makeUlHtml: function (setting, node, html, content) {
                html.push("<ul id='", node.tId, consts.id.UL, "' class='", consts.className.LEVEL, node.level, " ", view.makeUlLineClass(setting, node), "' style='display:", (node.open ? "block" : "none"), "'>");
                html.push(content);
                html.push("</ul>");
            },
            makeUlLineClass: function (setting, node) {
                return ((setting.view.showLine && !node.isLastNode) ? consts.line.LINE : "");
            },
            removeChildNodes: function (setting, node) {
                if (!node) return;
                var childKey = setting.data.key.children,
                    nodes = node[childKey];
                if (!nodes) return;

                for (var i = 0, l = nodes.length; i < l; i++) {
                    data.removeNodeCache(setting, nodes[i]);
                }
                data.removeSelectedNode(setting);
                delete node[childKey];

                if (!setting.data.keep.parent) {
                    node.isParent = false;
                    node.open = false;
                    var tmp_switchObj = $$(node, consts.id.SWITCH, setting),
                        tmp_icoObj = $$(node, consts.id.ICON, setting);
                    view.replaceSwitchClass(node, tmp_switchObj, consts.folder.DOCU);
                    view.replaceIcoClass(node, tmp_icoObj, consts.folder.DOCU);
                    $$(node, consts.id.UL, setting).remove();
                } else {
                    $$(node, consts.id.UL, setting).empty();
                }
            },
            setFirstNode: function (setting, parentNode) {
                var childKey = setting.data.key.children, childLength = parentNode[childKey].length;
                if (childLength > 0) {
                    parentNode[childKey][0].isFirstNode = true;
                }
            },
            setLastNode: function (setting, parentNode) {
                var childKey = setting.data.key.children, childLength = parentNode[childKey].length;
                if (childLength > 0) {
                    parentNode[childKey][childLength - 1].isLastNode = true;
                }
            },
            removeNode: function (setting, node) {
                var root = data.getRoot(setting),
                    childKey = setting.data.key.children,
                    parentNode = (node.parentTId) ? node.getParentNode() : root;

                node.isFirstNode = false;
                node.isLastNode = false;
                node.getPreNode = function () { return null; };
                node.getNextNode = function () { return null; };

                if (!data.getNodeCache(setting, node.tId)) {
                    return;
                }

                $$(node, setting).remove();
                data.removeNodeCache(setting, node);
                data.removeSelectedNode(setting, node);

                for (var i = 0, l = parentNode[childKey].length; i < l; i++) {
                    if (parentNode[childKey][i].tId == node.tId) {
                        parentNode[childKey].splice(i, 1);
                        break;
                    }
                }
                view.setFirstNode(setting, parentNode);
                view.setLastNode(setting, parentNode);

                var tmp_ulObj, tmp_switchObj, tmp_icoObj,
                    childLength = parentNode[childKey].length;

                //repair nodes old parent
                if (!setting.data.keep.parent && childLength == 0) {
                    //old parentNode has no child nodes
                    parentNode.isParent = false;
                    parentNode.open = false;
                    tmp_ulObj = $$(parentNode, consts.id.UL, setting);
                    tmp_switchObj = $$(parentNode, consts.id.SWITCH, setting);
                    tmp_icoObj = $$(parentNode, consts.id.ICON, setting);
                    view.replaceSwitchClass(parentNode, tmp_switchObj, consts.folder.DOCU);
                    view.replaceIcoClass(parentNode, tmp_icoObj, consts.folder.DOCU);
                    tmp_ulObj.css("display", "none");

                } else if (setting.view.showLine && childLength > 0) {
                    //old parentNode has child nodes
                    var newLast = parentNode[childKey][childLength - 1];
                    tmp_ulObj = $$(newLast, consts.id.UL, setting);
                    tmp_switchObj = $$(newLast, consts.id.SWITCH, setting);
                    tmp_icoObj = $$(newLast, consts.id.ICON, setting);
                    if (parentNode == root) {
                        if (parentNode[childKey].length == 1) {
                            //node was root, and ztree has only one root after move node
                            view.replaceSwitchClass(newLast, tmp_switchObj, consts.line.ROOT);
                        } else {
                            var tmp_first_switchObj = $$(parentNode[childKey][0], consts.id.SWITCH, setting);
                            view.replaceSwitchClass(parentNode[childKey][0], tmp_first_switchObj, consts.line.ROOTS);
                            view.replaceSwitchClass(newLast, tmp_switchObj, consts.line.BOTTOM);
                        }
                    } else {
                        view.replaceSwitchClass(newLast, tmp_switchObj, consts.line.BOTTOM);
                    }
                    tmp_ulObj.removeClass(consts.line.LINE);
                }
            },
            replaceIcoClass: function (node, obj, newName) {
                if (!obj || node.isAjaxing) return;
                var tmpName = obj.attr("class");
                if (tmpName == undefined) return;
                var tmpList = tmpName.split("_");
                switch (newName) {
                    case consts.folder.OPEN:
                    case consts.folder.CLOSE:
                    case consts.folder.DOCU:
                        tmpList[tmpList.length - 1] = newName;
                        break;
                }
                obj.attr("class", tmpList.join("_"));
            },
            replaceSwitchClass: function (node, obj, newName) {
                if (!obj) return;
                var tmpName = obj.attr("class");
                if (tmpName == undefined) return;
                var tmpList = tmpName.split("_");
                switch (newName) {
                    case consts.line.ROOT:
                    case consts.line.ROOTS:
                    case consts.line.CENTER:
                    case consts.line.BOTTOM:
                    case consts.line.NOLINE:
                        tmpList[0] = view.makeNodeLineClassEx(node) + newName;
                        break;
                    case consts.folder.OPEN:
                    case consts.folder.CLOSE:
                    case consts.folder.DOCU:
                        tmpList[1] = newName;
                        break;
                }
                obj.attr("class", tmpList.join("_"));
                if (newName !== consts.folder.DOCU) {
                    obj.removeAttr("disabled");
                } else {
                    obj.attr("disabled", "disabled");
                }
            },
            selectNode: function (setting, node, addFlag) {
                if (!addFlag) {
                    view.cancelPreSelectedNode(setting);
                }
                $$(node, consts.id.A, setting).addClass(consts.node.CURSELECTED);
                data.addSelectedNode(setting, node);
            },
            setNodeFontCss: function (setting, treeNode) {
                var aObj = $$(treeNode, consts.id.A, setting),
                    fontCss = view.makeNodeFontCss(setting, treeNode);
                if (fontCss) {
                    aObj.css(fontCss);
                }
            },
            setNodeLineIcos: function (setting, node) {
                if (!node) return;
                var switchObj = $$(node, consts.id.SWITCH, setting),
                    ulObj = $$(node, consts.id.UL, setting),
                    icoObj = $$(node, consts.id.ICON, setting),
                    ulLine = view.makeUlLineClass(setting, node);
                if (ulLine.length == 0) {
                    ulObj.removeClass(consts.line.LINE);
                } else {
                    ulObj.addClass(ulLine);
                }
                switchObj.attr("class", view.makeNodeLineClass(setting, node));
                if (node.isParent) {
                    switchObj.removeAttr("disabled");
                } else {
                    switchObj.attr("disabled", "disabled");
                }
                icoObj.removeAttr("style");
                icoObj.attr("style", view.makeNodeIcoStyle(setting, node));
                icoObj.attr("class", view.makeNodeIcoClass(setting, node));
            },
            setNodeName: function (setting, node) {
                var title = data.getNodeTitle(setting, node),
                    nObj = $$(node, consts.id.SPAN, setting);
                nObj.empty();
                if (setting.view.nameIsHTML) {
                    nObj.html(data.getNodeName(setting, node));
                } else {
                    nObj.text(data.getNodeName(setting, node));
                }
                if (tools.apply(setting.view.showTitle, [setting.treeId, node], setting.view.showTitle)) {
                    var aObj = $$(node, consts.id.A, setting);
                    aObj.attr("title", !title ? "" : title);
                }
            },
            setNodeTarget: function (setting, node) {
                var aObj = $$(node, consts.id.A, setting);
                aObj.attr("target", view.makeNodeTarget(node));
            },
            setNodeUrl: function (setting, node) {
                var aObj = $$(node, consts.id.A, setting),
                    url = view.makeNodeUrl(setting, node);
                if (url == null || url.length == 0) {
                    aObj.removeAttr("href");
                } else {
                    aObj.attr("href", url);
                }
            },
            switchNode: function (setting, node) {
                if (node.open || !tools.canAsync(setting, node)) {
                    view.expandCollapseNode(setting, node, !node.open);
                } else if (setting.async.enable) {
                    if (!view.asyncNode(setting, node)) {
                        view.expandCollapseNode(setting, node, !node.open);
                        return;
                    }
                } else if (node) {
                    view.expandCollapseNode(setting, node, !node.open);
                }
            }
        };
    // zTree defind
    $.fn.zTreeUser = {
        consts: _consts,
        _z: {
            tools: tools,
            view: view,
            event: event,
            data: data
        },
        getZTreeObj: function (treeId) {
            var o = data.getZTreeTools(treeId);
            return o ? o : null;
        },
        destroy: function (treeId) {
            if (!!treeId && treeId.length > 0) {
                view.destroy(data.getSetting(treeId));
            } else {
                for (var s in settings) {
                    view.destroy(settings[s]);
                }
            }
        },
        init: function (obj, zSetting, zNodes) {
            var setting = tools.clone(_setting);
            $.extend(true, setting, zSetting);
            setting.treeId = obj.attr("id");
            setting.treeObj = obj;
            setting.treeObj.empty();
            settings[setting.treeId] = setting;
            //For some older browser,(e.g., ie6)
            if (typeof document.body.style.maxHeight === "undefined") {
                setting.view.expandSpeed = "";
            }
            data.initRoot(setting);
            var root = data.getRoot(setting),
                childKey = setting.data.key.children;
            zNodes = zNodes ? tools.clone(tools.isArray(zNodes) ? zNodes : [zNodes]) : [];
            if (setting.data.simpleData.enable) {
                root[childKey] = data.transformTozTreeFormat(setting, zNodes);
            } else {
                root[childKey] = zNodes;
            }

            data.initCache(setting);
            event.unbindTree(setting);
            event.bindTree(setting);
            event.unbindEvent(setting);
            event.bindEvent(setting);

            var zTreeTools = {
                setting: setting,
                addNodes: function (parentNode, newNodes, isSilent) {
                    if (!newNodes) return null;
                    if (!parentNode) parentNode = null;
                    if (parentNode && !parentNode.isParent && setting.data.keep.leaf) return null;
                    var xNewNodes = tools.clone(tools.isArray(newNodes) ? newNodes : [newNodes]);
                    function addCallback() {
                        view.addNodes(setting, parentNode, xNewNodes, (isSilent == true));
                    }

                    if (tools.canAsync(setting, parentNode)) {
                        view.asyncNode(setting, parentNode, isSilent, addCallback);
                    } else {
                        addCallback();
                    }
                    return xNewNodes;
                },
                cancelSelectedNode: function (node) {
                    view.cancelPreSelectedNode(setting, node);
                },
                destroy: function () {
                    view.destroy(setting);
                },
                expandAll: function (expandFlag) {
                    expandFlag = !!expandFlag;
                    view.expandCollapseSonNode(setting, null, expandFlag, true);
                    return expandFlag;
                },
                expandNode: function (node, expandFlag, sonSign, focus, callbackFlag) {
                    if (!node || !node.isParent) return null;
                    if (expandFlag !== true && expandFlag !== false) {
                        expandFlag = !node.open;
                    }
                    callbackFlag = !!callbackFlag;

                    if (callbackFlag && expandFlag && (tools.apply(setting.callback.beforeExpand, [setting.treeId, node], true) == false)) {
                        return null;
                    } else if (callbackFlag && !expandFlag && (tools.apply(setting.callback.beforeCollapse, [setting.treeId, node], true) == false)) {
                        return null;
                    }
                    if (expandFlag && node.parentTId) {
                        view.expandCollapseParentNode(setting, node.getParentNode(), expandFlag, false);
                    }
                    if (expandFlag === node.open && !sonSign) {
                        return null;
                    }

                    data.getRoot(setting).expandTriggerFlag = callbackFlag;
                    if (!tools.canAsync(setting, node) && sonSign) {
                        view.expandCollapseSonNode(setting, node, expandFlag, true, function () {
                            if (focus !== false) { try { $$(node, setting).focus().blur(); } catch (e) { } }
                        });
                    } else {
                        node.open = !expandFlag;
                        view.switchNode(this.setting, node);
                        if (focus !== false) { try { $$(node, setting).focus().blur(); } catch (e) { } }
                    }
                    return expandFlag;
                },
                getNodes: function () {
                    return data.getNodes(setting);
                },
                getNodeByParam: function (key, value, parentNode) {
                    if (!key) return null;
                    return data.getNodeByParam(setting, parentNode ? parentNode[setting.data.key.children] : data.getNodes(setting), key, value);
                },
                getNodeByTId: function (tId) {
                    return data.getNodeCache(setting, tId);
                },
                getNodesByParam: function (key, value, parentNode) {
                    if (!key) return null;
                    return data.getNodesByParam(setting, parentNode ? parentNode[setting.data.key.children] : data.getNodes(setting), key, value);
                },
                getNodesByParamFuzzy: function (key, value, parentNode) {
                    if (!key) return null;
                    return data.getNodesByParamFuzzy(setting, parentNode ? parentNode[setting.data.key.children] : data.getNodes(setting), key, value);
                },
                getNodesByFilter: function (filter, isSingle, parentNode, invokeParam) {
                    isSingle = !!isSingle;
                    if (!filter || (typeof filter != "function")) return (isSingle ? null : []);
                    return data.getNodesByFilter(setting, parentNode ? parentNode[setting.data.key.children] : data.getNodes(setting), filter, isSingle, invokeParam);
                },
                getNodeIndex: function (node) {
                    if (!node) return null;
                    var childKey = setting.data.key.children,
                        parentNode = (node.parentTId) ? node.getParentNode() : data.getRoot(setting);
                    for (var i = 0, l = parentNode[childKey].length; i < l; i++) {
                        if (parentNode[childKey][i] == node) return i;
                    }
                    return -1;
                },
                getSelectedNodes: function () {
                    var r = [], list = data.getRoot(setting).curSelectedList;
                    for (var i = 0, l = list.length; i < l; i++) {
                        r.push(list[i]);
                    }
                    return r;
                },
                isSelectedNode: function (node) {
                    return data.isSelectedNode(setting, node);
                },
                reAsyncChildNodes: function (parentNode, reloadType, isSilent) {
                    if (!this.setting.async.enable) return;
                    var isRoot = !parentNode;
                    if (isRoot) {
                        parentNode = data.getRoot(setting);
                    }
                    if (reloadType == "refresh") {
                        var childKey = this.setting.data.key.children;
                        for (var i = 0, l = parentNode[childKey] ? parentNode[childKey].length : 0; i < l; i++) {
                            data.removeNodeCache(setting, parentNode[childKey][i]);
                        }
                        data.removeSelectedNode(setting);
                        parentNode[childKey] = [];
                        if (isRoot) {
                            this.setting.treeObj.empty();
                        } else {
                            var ulObj = $$(parentNode, consts.id.UL, setting);
                            ulObj.empty();
                        }
                    }
                    view.asyncNode(this.setting, isRoot ? null : parentNode, !!isSilent);
                },
                refresh: function () {
                    this.setting.treeObj.empty();
                    var root = data.getRoot(setting),
                        nodes = root[setting.data.key.children]
                    data.initRoot(setting);
                    root[setting.data.key.children] = nodes
                    data.initCache(setting);
                    view.createNodes(setting, 0, root[setting.data.key.children]);
                },
                removeChildNodes: function (node) {
                    if (!node) return null;
                    var childKey = setting.data.key.children,
                        nodes = node[childKey];
                    view.removeChildNodes(setting, node);
                    return nodes ? nodes : null;
                },
                removeNode: function (node, callbackFlag) {
                    if (!node) return;
                    callbackFlag = !!callbackFlag;
                    if (callbackFlag && tools.apply(setting.callback.beforeRemove, [setting.treeId, node], true) == false) return;
                    view.removeNode(setting, node);
                    if (callbackFlag) {
                        this.setting.treeObj.trigger(consts.event.REMOVE, [setting.treeId, node]);
                    }
                },
                selectNode: function (node, addFlag) {
                    if (!node) return;
                    if (tools.uCanDo(setting)) {
                        addFlag = setting.view.selectedMulti && addFlag;
                        if (node.parentTId) {
                            view.expandCollapseParentNode(setting, node.getParentNode(), true, false, function () {
                                try { $$(node, setting).focus().blur(); } catch (e) { }
                            });
                        } else {
                            try { $$(node, setting).focus().blur(); } catch (e) { }
                        }
                        view.selectNode(setting, node, addFlag);
                    }
                },
                transformTozTreeNodes: function (simpleNodes) {
                    return data.transformTozTreeFormat(setting, simpleNodes);
                },
                transformToArray: function (nodes) {
                    return data.transformToArrayFormat(setting, nodes);
                },
                updateNode: function (node, checkTypeFlag) {
                    if (!node) return;
                    var nObj = $$(node, setting);
                    if (nObj.get(0) && tools.uCanDo(setting)) {
                        view.setNodeName(setting, node);
                        view.setNodeTarget(setting, node);
                        view.setNodeUrl(setting, node);
                        view.setNodeLineIcos(setting, node);
                        view.setNodeFontCss(setting, node);
                    }
                }
            }
            root.treeTools = zTreeTools;
            data.setZTreeTools(setting, zTreeTools);

            if (root[childKey] && root[childKey].length > 0) {
                view.createNodes(setting, 0, root[childKey]);
            } else if (setting.async.enable) {//} else if (setting.async.enable && setting.async.url && setting.async.url !== '') {
                view.asyncNode(setting);
            }
            return zTreeTools;
        }
    };

    var ztUser = $.fn.zTreeUser,
        $$ = tools.$,
        consts = ztUser.consts;
})(JQZepto);

//编辑删除插件
/*
 * JQuery zTree exedit v3.5.17
 * http://zTree.me/
 *
 * Copyright (c) 2010 Hunter.z
 *
 * Licensed same as jquery - MIT License
 * http://www.opensource.org/licenses/mit-license.php
 *
 * email: hunter.z@263.net
 * Date: 2015-02-15
 */
(function (w) {
    var I = { event: { DRAG: "ztree_drag", DROP: "ztree_drop", RENAME: "ztree_rename", DRAGMOVE: "ztree_dragmove" }, id: { EDIT: "_edit", INPUT: "_input", REMOVE: "_remove" }, move: { TYPE_INNER: "inner", TYPE_PREV: "prev", TYPE_NEXT: "next" }, node: { CURSELECTED_EDIT: "curSelectedNode_Edit", TMPTARGET_TREE: "tmpTargetzTree", TMPTARGET_NODE: "tmpTargetNode" } }, x = {
        onHoverOverNode: function (b, a) { var c = m.getSetting(b.data.treeId), d = m.getRoot(c); if (d.curHoverNode != a) x.onHoverOutNode(b); d.curHoverNode = a; f.addHoverDom(c, a) }, onHoverOutNode: function (b) {
            var b =
                m.getSetting(b.data.treeId), a = m.getRoot(b); if (a.curHoverNode && !m.isSelectedNode(b, a.curHoverNode)) f.removeTreeDom(b, a.curHoverNode), a.curHoverNode = null
        }, onMousedownNode: function (b, a) {
            function c(b) {
                if (C.dragFlag == 0 && Math.abs(N - b.clientX) < e.edit.drag.minMoveSize && Math.abs(O - b.clientY) < e.edit.drag.minMoveSize) return !0; var a, c, n, k, i; i = e.data.key.children; M.css("cursor", "pointer"); if (C.dragFlag == 0) {
                    if (g.apply(e.callback.beforeDrag, [e.treeId, l], !0) == !1) return r(b), !0; for (a = 0, c = l.length; a < c; a++) {
                        if (a == 0) C.dragNodeShowBefore =
                            []; n = l[a]; n.isParent && n.open ? (f.expandCollapseNode(e, n, !n.open), C.dragNodeShowBefore[n.tId] = !0) : C.dragNodeShowBefore[n.tId] = !1
                    } C.dragFlag = 1; t.showHoverDom = !1; g.showIfameMask(e, !0); n = !0; k = -1; if (l.length > 1) { var j = l[0].parentTId ? l[0].getParentNode()[i] : m.getNodes(e); i = []; for (a = 0, c = j.length; a < c; a++) if (C.dragNodeShowBefore[j[a].tId] !== void 0 && (n && k > -1 && k + 1 !== a && (n = !1), i.push(j[a]), k = a), l.length === i.length) { l = i; break } } n && (H = l[0].getPreNode(), R = l[l.length - 1].getNextNode()); D = o("<ul class='zTreeDragUL'></ul>",
                        e); for (a = 0, c = l.length; a < c; a++) n = l[a], n.editNameFlag = !1, f.selectNode(e, n, a > 0), f.removeTreeDom(e, n), a > e.edit.drag.maxShowNodeNum - 1 || (k = o("<li id='" + n.tId + "_tmp'></li>", e), k.append(o(n, d.id.A, e).clone()), k.css("padding", "0"), k.children("#" + n.tId + d.id.A).removeClass(d.node.CURSELECTED), D.append(k), a == e.edit.drag.maxShowNodeNum - 1 && (k = o("<li id='" + n.tId + "_moretmp'><a>  ...  </a></li>", e), D.append(k))); D.attr("id", l[0].tId + d.id.UL + "_tmp"); D.addClass(e.treeObj.attr("class")); D.appendTo(M); B = o("<span class='tmpzTreeMove_arrow'></span>",
                            e); B.attr("id", "zTreeMove_arrow_tmp"); B.appendTo(M); e.treeObj.trigger(d.event.DRAG, [b, e.treeId, l])
                } if (C.dragFlag == 1) {
                    s && B.attr("id") == b.target.id && u && b.clientX + F.scrollLeft() + 2 > w("#" + u + d.id.A, s).offset().left ? (n = w("#" + u + d.id.A, s), b.target = n.length > 0 ? n.get(0) : b.target) : s && (s.removeClass(d.node.TMPTARGET_TREE), u && w("#" + u + d.id.A, s).removeClass(d.node.TMPTARGET_NODE + "_" + d.move.TYPE_PREV).removeClass(d.node.TMPTARGET_NODE + "_" + I.move.TYPE_NEXT).removeClass(d.node.TMPTARGET_NODE + "_" + I.move.TYPE_INNER));
                    u = s = null; J = !1; h = e; n = m.getSettings(); for (var y in n) if (n[y].treeId && n[y].edit.enable && n[y].treeId != e.treeId && (b.target.id == n[y].treeId || w(b.target).parents("#" + n[y].treeId).length > 0)) J = !0, h = n[y]; y = F.scrollTop(); k = F.scrollLeft(); i = h.treeObj.offset(); a = h.treeObj.get(0).scrollHeight; n = h.treeObj.get(0).scrollWidth; c = b.clientY + y - i.top; var p = h.treeObj.height() + i.top - b.clientY - y, q = b.clientX + k - i.left, x = h.treeObj.width() + i.left - b.clientX - k; i = c < e.edit.drag.borderMax && c > e.edit.drag.borderMin; var j = p < e.edit.drag.borderMax &&
                        p > e.edit.drag.borderMin, K = q < e.edit.drag.borderMax && q > e.edit.drag.borderMin, G = x < e.edit.drag.borderMax && x > e.edit.drag.borderMin, p = c > e.edit.drag.borderMin && p > e.edit.drag.borderMin && q > e.edit.drag.borderMin && x > e.edit.drag.borderMin, q = i && h.treeObj.scrollTop() <= 0, x = j && h.treeObj.scrollTop() + h.treeObj.height() + 10 >= a, P = K && h.treeObj.scrollLeft() <= 0, Q = G && h.treeObj.scrollLeft() + h.treeObj.width() + 10 >= n; if (b.target && g.isChildOrSelf(b.target, h.treeId)) {
                            for (var E = b.target; E && E.tagName && !g.eqs(E.tagName, "li") && E.id !=
                                h.treeId;) E = E.parentNode; var S = !0; for (a = 0, c = l.length; a < c; a++) if (n = l[a], E.id === n.tId) { S = !1; break } else if (o(n, e).find("#" + E.id).length > 0) { S = !1; break } if (S && b.target && g.isChildOrSelf(b.target, E.id + d.id.A)) s = w(E), u = E.id
                        } n = l[0]; if (p && g.isChildOrSelf(b.target, h.treeId)) {
                            if (!s && (b.target.id == h.treeId || q || x || P || Q) && (J || !J && n.parentTId)) s = h.treeObj; i ? h.treeObj.scrollTop(h.treeObj.scrollTop() - 10) : j && h.treeObj.scrollTop(h.treeObj.scrollTop() + 10); K ? h.treeObj.scrollLeft(h.treeObj.scrollLeft() - 10) : G && h.treeObj.scrollLeft(h.treeObj.scrollLeft() +
                                10); s && s != h.treeObj && s.offset().left < h.treeObj.offset().left && h.treeObj.scrollLeft(h.treeObj.scrollLeft() + s.offset().left - h.treeObj.offset().left)
                        } D.css({ top: b.clientY + y + 3 + "px", left: b.clientX + k + 3 + "px" }); i = a = 0; if (s && s.attr("id") != h.treeId) {
                            var z = u == null ? null : m.getNodeCache(h, u); c = (b.ctrlKey || b.metaKey) && e.edit.drag.isMove && e.edit.drag.isCopy || !e.edit.drag.isMove && e.edit.drag.isCopy; a = !!(H && u === H.tId); i = !!(R && u === R.tId); k = n.parentTId && n.parentTId == u; n = (c || !i) && g.apply(h.edit.drag.prev, [h.treeId, l, z],
                                !!h.edit.drag.prev); a = (c || !a) && g.apply(h.edit.drag.next, [h.treeId, l, z], !!h.edit.drag.next); G = (c || !k) && !(h.data.keep.leaf && !z.isParent) && g.apply(h.edit.drag.inner, [h.treeId, l, z], !!h.edit.drag.inner); if (!n && !a && !G) { if (s = null, u = "", v = d.move.TYPE_INNER, B.css({ display: "none" }), window.zTreeMoveTimer) clearTimeout(window.zTreeMoveTimer), window.zTreeMoveTargetNodeTId = null } else {
                                    c = w("#" + u + d.id.A, s); i = z.isLastNode ? null : w("#" + z.getNextNode().tId + d.id.A, s.next()); j = c.offset().top; k = c.offset().left; K = n ? G ? 0.25 : a ?
                                        0.5 : 1 : -1; G = a ? G ? 0.75 : n ? 0.5 : 0 : -1; y = (b.clientY + y - j) / c.height(); (K == 1 || y <= K && y >= -0.2) && n ? (a = 1 - B.width(), i = j - B.height() / 2, v = d.move.TYPE_PREV) : (G == 0 || y >= G && y <= 1.2) && a ? (a = 1 - B.width(), i = i == null || z.isParent && z.open ? j + c.height() - B.height() / 2 : i.offset().top - B.height() / 2, v = d.move.TYPE_NEXT) : (a = 5 - B.width(), i = j, v = d.move.TYPE_INNER); B.css({ display: "block", top: i + "px", left: k + a + "px" }); c.addClass(d.node.TMPTARGET_NODE + "_" + v); if (T != u || U != v) L = (new Date).getTime(); if (z && z.isParent && v == d.move.TYPE_INNER && (y = !0, window.zTreeMoveTimer &&
                                            window.zTreeMoveTargetNodeTId !== z.tId ? (clearTimeout(window.zTreeMoveTimer), window.zTreeMoveTargetNodeTId = null) : window.zTreeMoveTimer && window.zTreeMoveTargetNodeTId === z.tId && (y = !1), y)) window.zTreeMoveTimer = setTimeout(function () { v == d.move.TYPE_INNER && z && z.isParent && !z.open && (new Date).getTime() - L > h.edit.drag.autoOpenTime && g.apply(h.callback.beforeDragOpen, [h.treeId, z], !0) && (f.switchNode(h, z), h.edit.drag.autoExpandTrigger && h.treeObj.trigger(d.event.EXPAND, [h.treeId, z])) }, h.edit.drag.autoOpenTime + 50),
                                                window.zTreeMoveTargetNodeTId = z.tId
                                }
                        } else if (v = d.move.TYPE_INNER, s && g.apply(h.edit.drag.inner, [h.treeId, l, null], !!h.edit.drag.inner) ? s.addClass(d.node.TMPTARGET_TREE) : s = null, B.css({ display: "none" }), window.zTreeMoveTimer) clearTimeout(window.zTreeMoveTimer), window.zTreeMoveTargetNodeTId = null; T = u; U = v; e.treeObj.trigger(d.event.DRAGMOVE, [b, e.treeId, l])
                } return !1
            } function r(b) {
                if (window.zTreeMoveTimer) clearTimeout(window.zTreeMoveTimer), window.zTreeMoveTargetNodeTId = null; U = T = null; F.unbind("mousemove", c);
                F.unbind("mouseup", r); F.unbind("selectstart", k); M.css("cursor", "auto"); s && (s.removeClass(d.node.TMPTARGET_TREE), u && w("#" + u + d.id.A, s).removeClass(d.node.TMPTARGET_NODE + "_" + d.move.TYPE_PREV).removeClass(d.node.TMPTARGET_NODE + "_" + I.move.TYPE_NEXT).removeClass(d.node.TMPTARGET_NODE + "_" + I.move.TYPE_INNER)); g.showIfameMask(e, !1); t.showHoverDom = !0; if (C.dragFlag != 0) {
                    C.dragFlag = 0; var a, i, j; for (a = 0, i = l.length; a < i; a++) j = l[a], j.isParent && C.dragNodeShowBefore[j.tId] && !j.open && (f.expandCollapseNode(e, j, !j.open),
                        delete C.dragNodeShowBefore[j.tId]); D && D.remove(); B && B.remove(); var p = (b.ctrlKey || b.metaKey) && e.edit.drag.isMove && e.edit.drag.isCopy || !e.edit.drag.isMove && e.edit.drag.isCopy; !p && s && u && l[0].parentTId && u == l[0].parentTId && v == d.move.TYPE_INNER && (s = null); if (s) {
                            var q = u == null ? null : m.getNodeCache(h, u); if (g.apply(e.callback.beforeDrop, [h.treeId, l, q, v, p], !0) == !1) f.selectNodes(x, l); else {
                                var A = p ? g.clone(l) : l; a = function () {
                                    if (J) {
                                        if (!p) for (var a = 0, c = l.length; a < c; a++) f.removeNode(e, l[a]); if (v == d.move.TYPE_INNER) f.addNodes(h,
                                            q, A); else if (f.addNodes(h, q.getParentNode(), A), v == d.move.TYPE_PREV) for (a = 0, c = A.length; a < c; a++) f.moveNode(h, q, A[a], v, !1); else for (a = -1, c = A.length - 1; a < c; c--) f.moveNode(h, q, A[c], v, !1)
                                    } else if (p && v == d.move.TYPE_INNER) f.addNodes(h, q, A); else if (p && f.addNodes(h, q.getParentNode(), A), v != d.move.TYPE_NEXT) for (a = 0, c = A.length; a < c; a++) f.moveNode(h, q, A[a], v, !1); else for (a = -1, c = A.length - 1; a < c; c--) f.moveNode(h, q, A[c], v, !1); f.selectNodes(h, A); o(A[0], e).focus().blur(); e.treeObj.trigger(d.event.DROP, [b, h.treeId, A, q,
                                        v, p])
                                }; v == d.move.TYPE_INNER && g.canAsync(h, q) ? f.asyncNode(h, q, !1, a) : a()
                            }
                        } else f.selectNodes(x, l), e.treeObj.trigger(d.event.DROP, [b, e.treeId, l, null, null, null])
                }
            } function k() { return !1 } var i, j, e = m.getSetting(b.data.treeId), C = m.getRoot(e), t = m.getRoots(); if (b.button == 2 || !e.edit.enable || !e.edit.drag.isCopy && !e.edit.drag.isMove) return !0; var p = b.target, q = m.getRoot(e).curSelectedList, l = []; if (m.isSelectedNode(e, a)) for (i = 0, j = q.length; i < j; i++) {
                if (q[i].editNameFlag && g.eqs(p.tagName, "input") && p.getAttribute("treeNode" +
                    d.id.INPUT) !== null) return !0; l.push(q[i]); if (l[0].parentTId !== q[i].parentTId) { l = [a]; break }
            } else l = [a]; f.editNodeBlur = !0; f.cancelCurEditNode(e); var F = w(e.treeObj.get(0).ownerDocument), M = w(e.treeObj.get(0).ownerDocument.body), D, B, s, J = !1, h = e, x = e, H, R, T = null, U = null, u = null, v = d.move.TYPE_INNER, N = b.clientX, O = b.clientY, L = (new Date).getTime(); g.uCanDo(e) && F.bind("mousemove", c); F.bind("mouseup", r); F.bind("selectstart", k); b.preventDefault && b.preventDefault(); return !0
        }
    }; w.extend(!0, w.fn.zTreeUser.consts, I); w.extend(!0,
        w.fn.zTreeUser._z, {
            tools: {
                getAbs: function (b) { b = b.getBoundingClientRect(); return [b.left + (document.body.scrollLeft + document.documentElement.scrollLeft), b.top + (document.body.scrollTop + document.documentElement.scrollTop)] }, inputFocus: function (b) { b.get(0) && (b.focus(), g.setCursorPosition(b.get(0), b.val().length)) }, inputSelect: function (b) { b.get(0) && (b.focus(), b.select()) }, setCursorPosition: function (b, a) {
                    if (b.setSelectionRange) b.focus(), b.setSelectionRange(a, a); else if (b.createTextRange) {
                        var c = b.createTextRange();
                        c.collapse(!0); c.moveEnd("character", a); c.moveStart("character", a); c.select()
                    }
                }, showIfameMask: function (b, a) { for (var c = m.getRoot(b); c.dragMaskList.length > 0;) c.dragMaskList[0].remove(), c.dragMaskList.shift(); if (a) for (var d = o("iframe", b), f = 0, i = d.length; f < i; f++) { var j = d.get(f), e = g.getAbs(j), j = o("<div id='zTreeMask_" + f + "' class='zTreeMask' style='top:" + e[1] + "px; left:" + e[0] + "px; width:" + j.offsetWidth + "px; height:" + j.offsetHeight + "px;'></div>", b); j.appendTo(o("body", b)); c.dragMaskList.push(j) } }
            }, view: {
                addEditBtn: function (b,
                    a) { if (!(a.editNameFlag || o(a, d.id.EDIT, b).length > 0) && g.apply(b.edit.showRenameBtn, [b.treeId, a], b.edit.showRenameBtn)) { var c = o(a, d.id.A, b), r = "<span class='" + d.className.BUTTON + " bpf-userselect-edit' id='" + a.tId + d.id.EDIT + "' title='" + g.apply(b.edit.renameTitle, [b.treeId, a], b.edit.renameTitle) + "' treeNode" + d.id.EDIT + " style='display:none;'></span>"; c.append(r); o(a, d.id.EDIT, b).bind("click", function () { if (!g.uCanDo(b) || g.apply(b.callback.beforeEditName, [b.treeId, a], !0) == !1) return !1; f.editNode(b, a); return !1 }).show() } },
                addRemoveBtn: function (b, a) {
                    if (!(a.editNameFlag || o(a, d.id.REMOVE, b).length > 0) && g.apply(b.edit.showRemoveBtn, [b.treeId, a], b.edit.showRemoveBtn)) {
                        var c = o(a, d.id.A, b), r = "<span class='" + d.className.BUTTON + " bpf-userselect-remove' id='" + a.tId + d.id.REMOVE + "' title='" + g.apply(b.edit.removeTitle, [b.treeId, a], b.edit.removeTitle) + "' treeNode" + d.id.REMOVE + " style='display:none;'></span>"; c.append(r); o(a, d.id.REMOVE, b).bind("click", function () {
                            if (!g.uCanDo(b) || g.apply(b.callback.beforeRemove, [b.treeId, a], !0) == !1) return !1; f.removeNode(b,
                                a); b.treeObj.trigger(d.event.REMOVE, [b.treeId, a]); return !1
                        }).bind("mousedown", function () { return !0 }).show()
                    }
                }, addHoverDom: function (b, a) { if (m.getRoots().showHoverDom) a.isHover = !0, b.edit.enable && (f.addEditBtn(b, a), f.addRemoveBtn(b, a)), g.apply(b.view.addHoverDom, [b.treeId, a]) }, cancelCurEditNode: function (b, a, c) {
                    var r = m.getRoot(b), k = b.data.key.name, i = r.curEditNode; if (i) {
                        var j = r.curEditInput, a = a ? a : c ? i[k] : j.val(); if (g.apply(b.callback.beforeRename, [b.treeId, i, a, c], !0) === !1) return !1; i[k] = a; o(i, d.id.A, b).removeClass(d.node.CURSELECTED_EDIT);
                        j.unbind(); f.setNodeName(b, i); i.editNameFlag = !1; r.curEditNode = null; r.curEditInput = null; f.selectNode(b, i, !1); b.treeObj.trigger(d.event.RENAME, [b.treeId, i, c])
                    } return r.noSelection = !0
                }, editNode: function (b, a) {
                    var c = m.getRoot(b); f.editNodeBlur = !1; if (m.isSelectedNode(b, a) && c.curEditNode == a && a.editNameFlag) setTimeout(function () { g.inputFocus(c.curEditInput) }, 0); else {
                        var r = b.data.key.name; a.editNameFlag = !0; f.removeTreeDom(b, a); f.cancelCurEditNode(b); f.selectNode(b, a, !1); o(a, d.id.SPAN, b).html("<input type=text class='rename' id='" +
                            a.tId + d.id.INPUT + "' treeNode" + d.id.INPUT + " >"); var k = o(a, d.id.INPUT, b); k.attr("value", a[r]); b.edit.editNameSelectAll ? g.inputSelect(k) : g.inputFocus(k); k.bind("blur", function () { f.editNodeBlur || f.cancelCurEditNode(b) }).bind("keydown", function (a) { a.keyCode == "13" ? (f.editNodeBlur = !0, f.cancelCurEditNode(b)) : a.keyCode == "27" && f.cancelCurEditNode(b, null, !0) }).bind("click", function () { return !1 }).bind("dblclick", function () { return !1 }); o(a, d.id.A, b).addClass(d.node.CURSELECTED_EDIT); c.curEditInput = k; c.noSelection =
                                !1; c.curEditNode = a
                    }
                }, moveNode: function (b, a, c, r, k, i) {
                    var j = m.getRoot(b), e = b.data.key.children; if (a != c && (!b.data.keep.leaf || !a || a.isParent || r != d.move.TYPE_INNER)) {
                        var g = c.parentTId ? c.getParentNode() : j, t = a === null || a == j; t && a === null && (a = j); if (t) r = d.move.TYPE_INNER; j = a.parentTId ? a.getParentNode() : j; if (r != d.move.TYPE_PREV && r != d.move.TYPE_NEXT) r = d.move.TYPE_INNER; if (r == d.move.TYPE_INNER) if (t) c.parentTId = null; else { if (!a.isParent) a.isParent = !0, a.open = !!a.open, f.setNodeLineIcos(b, a); c.parentTId = a.tId } var p;
                        t ? p = t = b.treeObj : (!i && r == d.move.TYPE_INNER ? f.expandCollapseNode(b, a, !0, !1) : i || f.expandCollapseNode(b, a.getParentNode(), !0, !1), t = o(a, b), p = o(a, d.id.UL, b), t.get(0) && !p.get(0) && (p = [], f.makeUlHtml(b, a, p, ""), t.append(p.join(""))), p = o(a, d.id.UL, b)); var q = o(c, b); q.get(0) ? t.get(0) || q.remove() : q = f.appendNodes(b, c.level, [c], null, !1, !0).join(""); p.get(0) && r == d.move.TYPE_INNER ? p.append(q) : t.get(0) && r == d.move.TYPE_PREV ? t.before(q) : t.get(0) && r == d.move.TYPE_NEXT && t.after(q); var l = -1, w = 0, x = null, t = null, D = c.level; if (c.isFirstNode) {
                            if (l =
                                0, g[e].length > 1) x = g[e][1], x.isFirstNode = !0
                        } else if (c.isLastNode) l = g[e].length - 1, x = g[e][l - 1], x.isLastNode = !0; else for (p = 0, q = g[e].length; p < q; p++) if (g[e][p].tId == c.tId) { l = p; break } l >= 0 && g[e].splice(l, 1); if (r != d.move.TYPE_INNER) for (p = 0, q = j[e].length; p < q; p++) j[e][p].tId == a.tId && (w = p); if (r == d.move.TYPE_INNER) { a[e] || (a[e] = []); if (a[e].length > 0) t = a[e][a[e].length - 1], t.isLastNode = !1; a[e].splice(a[e].length, 0, c); c.isLastNode = !0; c.isFirstNode = a[e].length == 1 } else a.isFirstNode && r == d.move.TYPE_PREV ? (j[e].splice(w,
                            0, c), t = a, t.isFirstNode = !1, c.parentTId = a.parentTId, c.isFirstNode = !0, c.isLastNode = !1) : a.isLastNode && r == d.move.TYPE_NEXT ? (j[e].splice(w + 1, 0, c), t = a, t.isLastNode = !1, c.parentTId = a.parentTId, c.isFirstNode = !1, c.isLastNode = !0) : (r == d.move.TYPE_PREV ? j[e].splice(w, 0, c) : j[e].splice(w + 1, 0, c), c.parentTId = a.parentTId, c.isFirstNode = !1, c.isLastNode = !1); m.fixPIdKeyValue(b, c); m.setSonNodeLevel(b, c.getParentNode(), c); f.setNodeLineIcos(b, c); f.repairNodeLevelClass(b, c, D); !b.data.keep.parent && g[e].length < 1 ? (g.isParent = !1,
                                g.open = !1, a = o(g, d.id.UL, b), r = o(g, d.id.SWITCH, b), e = o(g, d.id.ICON, b), f.replaceSwitchClass(g, r, d.folder.DOCU), f.replaceIcoClass(g, e, d.folder.DOCU), a.css("display", "none")) : x && f.setNodeLineIcos(b, x); t && f.setNodeLineIcos(b, t); b.check && b.check.enable && f.repairChkClass && (f.repairChkClass(b, g), f.repairParentChkClassWithSelf(b, g), g != c.parent && f.repairParentChkClassWithSelf(b, c)); i || f.expandCollapseParentNode(b, c.getParentNode(), !0, k)
                    }
                }, removeEditBtn: function (b, a) { o(a, d.id.EDIT, b).unbind().remove() }, removeRemoveBtn: function (b,
                    a) { o(a, d.id.REMOVE, b).unbind().remove() }, removeTreeDom: function (b, a) { a.isHover = !1; f.removeEditBtn(b, a); f.removeRemoveBtn(b, a); g.apply(b.view.removeHoverDom, [b.treeId, a]) }, repairNodeLevelClass: function (b, a, c) { if (c !== a.level) { var f = o(a, b), g = o(a, d.id.A, b), b = o(a, d.id.UL, b), c = d.className.LEVEL + c, a = d.className.LEVEL + a.level; f.removeClass(c); f.addClass(a); g.removeClass(c); g.addClass(a); b.removeClass(c); b.addClass(a) } }, selectNodes: function (b, a) { for (var c = 0, d = a.length; c < d; c++) f.selectNode(b, a[c], c > 0) }
            }, event: {},
            data: { setSonNodeLevel: function (b, a, c) { if (c) { var d = b.data.key.children; c.level = a ? a.level + 1 : 0; if (c[d]) for (var a = 0, f = c[d].length; a < f; a++) c[d][a] && m.setSonNodeLevel(b, c, c[d][a]) } } }
        }); var H = w.fn.zTreeUser, g = H._z.tools, d = H.consts, f = H._z.view, m = H._z.data, o = g.$; m.exSetting({
            edit: {
                enable: !1, editNameSelectAll: !1, showRemoveBtn: !0, showRenameBtn: !0, removeTitle: "remove", renameTitle: "rename", drag: {
                    autoExpandTrigger: !1, isCopy: !0, isMove: !0, prev: !0, next: !0, inner: !0, minMoveSize: 5, borderMax: 10, borderMin: -5, maxShowNodeNum: 5,
                    autoOpenTime: 500
                }
            }, view: { addHoverDom: null, removeHoverDom: null }, callback: { beforeDrag: null, beforeDragOpen: null, beforeDrop: null, beforeEditName: null, beforeRename: null, onDrag: null, onDragMove: null, onDrop: null, onRename: null }
        }); m.addInitBind(function (b) {
            var a = b.treeObj, c = d.event; a.bind(c.RENAME, function (a, c, d, f) { g.apply(b.callback.onRename, [a, c, d, f]) }); a.bind(c.DRAG, function (a, c, d, f) { g.apply(b.callback.onDrag, [c, d, f]) }); a.bind(c.DRAGMOVE, function (a, c, d, f) { g.apply(b.callback.onDragMove, [c, d, f]) }); a.bind(c.DROP,
                function (a, c, d, f, e, m, o) { g.apply(b.callback.onDrop, [c, d, f, e, m, o]) })
        }); m.addInitUnBind(function (b) { var b = b.treeObj, a = d.event; b.unbind(a.RENAME); b.unbind(a.DRAG); b.unbind(a.DRAGMOVE); b.unbind(a.DROP) }); m.addInitCache(function () { }); m.addInitNode(function (b, a, c) { if (c) c.isHover = !1, c.editNameFlag = !1 }); m.addInitProxy(function (b) {
            var a = b.target, c = m.getSetting(b.data.treeId), f = b.relatedTarget, k = "", i = null, j = "", e = null, o = null; if (g.eqs(b.type, "mouseover")) {
                if (o = g.getMDom(c, a, [{ tagName: "a", attrName: "treeNode" + d.id.A }])) k =
                    g.getNodeMainDom(o).id, j = "hoverOverNode"
            } else if (g.eqs(b.type, "mouseout")) o = g.getMDom(c, f, [{ tagName: "a", attrName: "treeNode" + d.id.A }]), o || (k = "remove", j = "hoverOutNode"); else if (g.eqs(b.type, "mousedown") && (o = g.getMDom(c, a, [{ tagName: "a", attrName: "treeNode" + d.id.A }]))) k = g.getNodeMainDom(o).id, j = "mousedownNode"; if (k.length > 0) switch (i = m.getNodeCache(c, k), j) { case "mousedownNode": e = x.onMousedownNode; break; case "hoverOverNode": e = x.onHoverOverNode; break; case "hoverOutNode": e = x.onHoverOutNode } return {
                stop: !1,
                node: i, nodeEventType: j, nodeEventCallback: e, treeEventType: "", treeEventCallback: null
            }
        }); m.addInitRoot(function (b) { var b = m.getRoot(b), a = m.getRoots(); b.curEditNode = null; b.curEditInput = null; b.curHoverNode = null; b.dragFlag = 0; b.dragNodeShowBefore = []; b.dragMaskList = []; a.showHoverDom = !0 }); m.addZTreeTools(function (b, a) {
            a.cancelEditName = function (a) { m.getRoot(this.setting).curEditNode && f.cancelCurEditNode(this.setting, a ? a : null, !0) }; a.copyNode = function (a, b, k, i) {
                if (!b) return null; if (a && !a.isParent && this.setting.data.keep.leaf &&
                    k === d.move.TYPE_INNER) return null; var j = this, e = g.clone(b); if (!a) a = null, k = d.move.TYPE_INNER; k == d.move.TYPE_INNER ? (b = function () { f.addNodes(j.setting, a, [e], i) }, g.canAsync(this.setting, a) ? f.asyncNode(this.setting, a, i, b) : b()) : (f.addNodes(this.setting, a.parentNode, [e], i), f.moveNode(this.setting, a, e, k, !1, i)); return e
            }; a.editName = function (a) { a && a.tId && a === m.getNodeCache(this.setting, a.tId) && (a.parentTId && f.expandCollapseParentNode(this.setting, a.getParentNode(), !0), f.editNode(this.setting, a)) }; a.moveNode =
                function (a, b, k, i) { function j() { f.moveNode(e.setting, a, b, k, !1, i) } if (!b) return b; if (a && !a.isParent && this.setting.data.keep.leaf && k === d.move.TYPE_INNER) return null; else if (a && (b.parentTId == a.tId && k == d.move.TYPE_INNER || o(b, this.setting).find("#" + a.tId).length > 0)) return null; else a || (a = null); var e = this; g.canAsync(this.setting, a) && k === d.move.TYPE_INNER ? f.asyncNode(this.setting, a, i, j) : j(); return b }; a.setEditable = function (a) { this.setting.edit.enable = a; return this.refresh() }
        }); var N = f.cancelPreSelectedNode;
    f.cancelPreSelectedNode = function (b, a) { for (var c = m.getRoot(b).curSelectedList, d = 0, g = c.length; d < g; d++) if (!a || a === c[d]) if (f.removeTreeDom(b, c[d]), a) break; N && N.apply(f, arguments) }; var O = f.createNodes; f.createNodes = function (b, a, c, d) { O && O.apply(f, arguments); c && f.repairParentChkClassWithSelf && f.repairParentChkClassWithSelf(b, d) }; var V = f.makeNodeUrl; f.makeNodeUrl = function (b, a) { return b.edit.enable ? null : V.apply(f, arguments) }; var L = f.removeNode; f.removeNode = function (b, a) {
        var c = m.getRoot(b); if (c.curEditNode ===
            a) c.curEditNode = null; L && L.apply(f, arguments)
    }; var P = f.selectNode; f.selectNode = function (b, a, c) { var d = m.getRoot(b); if (m.isSelectedNode(b, a) && d.curEditNode == a && a.editNameFlag) return !1; P && P.apply(f, arguments); f.addHoverDom(b, a); return !0 }; var Q = g.uCanDo; g.uCanDo = function (b, a) {
        var c = m.getRoot(b); if (a && (g.eqs(a.type, "mouseover") || g.eqs(a.type, "mouseout") || g.eqs(a.type, "mousedown") || g.eqs(a.type, "mouseup"))) return !0; if (c.curEditNode) f.editNodeBlur = !1, c.curEditInput.focus(); return !c.curEditNode && (Q ? Q.apply(f,
            arguments) : !0)
    }
})(JQZepto);

//================================Jquery 插件==================================
//以下实现拖拽与缩放
// h 表示句柄
// c 表示拖拽的时候清理浮动菜单的方法
// k 表示命令，
var BPF_USER_OPENDIV_OBJARRAY = [];
(function ($) {
    $.fn.BPF_User_jqDrag = function (h, c) {
        return i(this, h, c, 'd');
    };
    $.fn.BPF_User_jqResize = function (h) {
        return i(this, h, 'r');
    };
    $.BPF_User_jqDnR = {
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
    var J = $.BPF_User_jqDnR, M = J.dnr, E = J.e,
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
                    E.css({ opacity: 0.8 }); $("body").mousemove($.BPF_User_jqDnR.drag).mouseup($.BPF_User_jqDnR.stop);
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
            if (BPF_USER_OPENDIV_OBJARRAY.length > 0) {
                var $self = BPF_USER_OPENDIV_OBJARRAY[BPF_USER_OPENDIV_OBJARRAY.length - 1];
                $self.CancelFunc();
                BPF_USER_OPENDIV_OBJARRAY.pop();
            }
        }
    });
})(JQZepto);

var BPF_User_browser = {
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
        BPF_User_PageSize: function () {
            var width = 0;
            var height = 0;
            width = window.innerWidth != null ? window.innerWidth : document.documentElement && document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body != null ? document.body.clientWidth : null;
            height = window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null;
            return { Width: width, Height: height };
        },
        BPF_User_ScrollPosition: function () {
            var top = 0, left = 0;
            if (BPF_User_browser.versions.gecko) {
                top = window.pageYOffset;
                left = window.pageXOffset;
            }
            else if (BPF_User_browser.versions.trident) {
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



