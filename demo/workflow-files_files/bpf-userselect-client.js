var bpf_userselect_client = {
    setting: {
    },//配置信息
    selectUser: function (setting) {
        var _setting = {
            func: function (userList) { },
            exit: function () { },
            appCode: "",
            allowMulti: true,
            allowAll: true,
            isNeedHiddenNav: false,//只有当业务系统接入了集团App，并且业务系统中没有隐藏集团App导航时需要设置该属性为true，其余时间都需要设置为false
            waitUserList: [],
            checkedUserList: [],
            exceptUserList: [],
            currentUserSourceType: null,
            topValue: 0,
            customerSetting: {
                enable: false,//是否启用自定义数据源，启用后常用联系人功能将被禁用
                org: {
                    enable: false,//是否启用组织机构选人，启用后请按照使用方式（PC/手机）注册以下相关方法
                    getOrgListByParentOrgID: function (orgID, func) { func() },//PC端必须
                    getUserListByOrgID: function (orgID, func) { func() },//PC端必须
                    getOrgListAndUserListByParentOrgID: function (orgID, func) { }//手机端必须
                },
                userFilter: {
                    enable: false,//是否启用查询条件选人，启用后请按照使用方式（PC/手机）注册以下相关方法
                    getUserListByFilter: function (filter, func) { func() },//PC端必须
                    getUserListBySearchKey: function (keyWord, func) { func() }//手机端必须
                },
                group: {
                    enable: false,//是否启用定义的组选人，启用后请按照使用方式（PC/手机）注册以下相关方法，启用后仅支持查看，不支持删除和修改功能
                    getPublicGroupList: function (func) { func() },
                    getPrivateGroupList: function (func) { func() },
                    getGroupUserList: function (groupID, func) { func() }
                }
            }
        };
        JQZepto.extend(true, _setting, setting);
        bpf_userselect_client.setting = _setting;
        bpf_userselect_tool.showSelectUser(_setting, this);
    },
    getOrgListByParentOrgID: function (orgID, func) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            setting.org.getOrgListByParentOrgID(orgID, func);//PC端必须
        } else {
            //加载组织下一级组织机构信息
            bpf_userselect_client.ajaxUserSimple("GetOrgListByParentOrgID", { OrgID: orgID }, func);
        }
    },       
    getUserListByOrgID: function (orgID, func) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            setting.org.getUserListByOrgID(orgID, func);//PC端必须
        } else {
            //加载当前组织机构的人员信息
            bpf_userselect_client.ajaxUserSimple("GetUserListByOrgID", { OrgID: orgID }, func);
        }
    },
    getUserListBySearchKey: function (keyWord, func) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            setting.userFilter.getUserListBySearchKey(keyWord, func);
        } else {
            //根据关键字获取人员信息
            bpf_userselect_client.ajaxUserSimple("GetUserListBySearchKey", { KeyWord: keyWord }, func);
        }
    },
    getUserListByFilter: function (filter, func) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            setting.userFilter.getUserListByFilter(filter, func);
        } else {
            //根据关键字获取人员信息
            bpf_userselect_client.ajaxUserSimple("GetUserListByFilter", { UserFilter: filter }, func);
        }
    },
    getOrgListAndUserListByParentOrgID: function (orgID, func) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            setting.org.getOrgListAndUserListByParentOrgID(orgID, func);
        } else {
            //加载当前组织机构的人员信息
            bpf_userselect_client.ajaxUserSimple("GetOrgListAndUserListByParentOrgID", { OrgID: orgID }, func);
        }
    },
    getTopUserList: function (func, _setting) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            return false;
        }else {
        //加载常用联系人
            bpf_userselect_client.ajaxUserSimple("GetTopUserList", { "MCS_PARAM_CURRENTUSER": null, "MCS_PARAM_CURRENTUSERSOURCETYPE": _setting.currentUserSourceType }, func);
        }
    },
    getEnableTopUserSelect: function (func) {
        var setting = bpf_userselect_client.setting.customerSetting;
        if (setting.enable) {
            return false;
        } else {
            //加载常用联系人
            bpf_userselect_client.ajaxUserSimple("GetEnableTopUserSelect", {}, func);
        }
    },
    repairUserAvatartUrl: function (waitUserList, checkedUserList, func) {
        if ((waitUserList != undefined && waitUserList.length > 0) || (checkedUserList != undefined && checkedUserList.length > 0)) {
            //根据关键字获取人员信息
            bpf_userselect_client.ajaxUserSimple("RepairUserAvatartUrl", { WaitUserList: waitUserList, CheckedUserList: checkedUserList }, func);
        }
        else {
            func();
        }
    },
    saveUserSelectResult: function (checkedUserList, _setting, dataFrom) {
        //dataFrom = dataFrom == undefined ? 'Mobile' : dataFrom;
        //var setting = {
        //    url: "UserSelectHandler.ashx?t=" + new Date().getMilliseconds(),
        //    dataType: "json",
        //    async: true,
        //    data: { BizContext: JSON.stringify({ MethodName: "SaveUserSelectResult", Param: { "MCS_PARAM_CURRENTUSER": null, "MCS_PARAM_CURRENTUSERSOURCETYPE": _setting.currentUserSourceType, CheckedUserList: checkedUserList, DataFrom: dataFrom } }) },
        //    beforeSend: function (xhr, settings) {
        //    },
        //    success: function (dataTemp) {
        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //    }
        //};
        //bpf_sdk_tool.ajax(setting);
    },
    _getGroupInfo: function () {
        var zNodes = [
            {
                GroupID: "1",
                GroupName: "定义的组",
                ParentGroupID: null,
                children: [
                    {
                        GroupID: "publicGroup",
                        GroupName: "公有组"
                    },
                    {
                        GroupID: "privateGroup",
                        GroupName: "私有组"
                    }
                ],
                open: true,
                isParent: true
            }
        ];
        return zNodes;
    },
    getPublicGroupList: function (func) {
        //var setting = bpf_userselect_client.setting.customerSetting;
        //if (setting.enable) {
        //    setting.group.getPublicGroupList(func);
        //} else {
        //    //获取公有组信息GetPublicGroupList
        //    bpf_userselect_client.ajaxUserSimple("GetPublicGroupList", {}, func);
        //}
    },
    getPrivateGroupList: function (func) {
        //var setting = bpf_userselect_client.setting.customerSetting;
        //if (setting.enable) {
        //    setting.group.getPrivateGroupList(func);
        //} else {
        //    //获取私有组信息
        //    bpf_userselect_client.ajaxUserSimple("GetPrivateGroupList", { "MCS_PARAM_CURRENTUSER": null, "MCS_PARAM_CURRENTUSERSOURCETYPE": null }, func, undefined, "None");
        //}
    },
    getGroupUserList: function (groupID, func) {
        //var setting = bpf_userselect_client.setting.customerSetting;
        //if (setting.enable) {
        //    setting.group.getGroupUserList(groupID, func);
        //} else {
        //    //获取组用户信息
        //    bpf_userselect_client.ajaxUserSimple("GetGroupUserList", { GroupID: groupID }, func, undefined, "None");
        //}
    },
    savePrivateGroup: function (groupName, groupUser, func) {
        ////新建私有组
        //bpf_userselect_client.ajaxUserSimple("SavePrivateGroup", { GroupName: groupName, GroupUser: groupUser, "MCS_PARAM_CURRENTUSER": null, "MCS_PARAM_CURRENTUSERSOURCETYPE": null }, func);
    },
    savePrivateGroupUser: function (groupID, groupUser, func) {
        ////编辑私有组用户
        //bpf_userselect_client.ajaxUserSimple("SavePrivateGroupUser", { GroupID: groupID, GroupUser: groupUser }, func);
    },
    deletePrivateGroup: function (groupID, func) {
        ////删除私有组
        //bpf_userselect_client.ajaxUserSimple("DeletePrivateGroup", { GroupID: groupID }, func);        
    },
    ajaxUserSimple: function (methodName, param, successFunc, handlerName, expirationType) {
        var setting = {
            BizContext: { MethodName: methodName, Param: param, AppCode: bpf_userselect_client.setting.appCode, ExpirationType: expirationType },
            SuccessFunc: successFunc,
            HandlerName: handlerName || "UserSelectHandler"
        }
        bpf_sdk_tool.ajaxBizContext(setting);
    }
}


