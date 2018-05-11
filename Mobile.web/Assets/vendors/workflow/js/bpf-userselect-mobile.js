var BPF_UserSelect_Model;
var VueUserSelectClient;
var bpf_userselect_tool = {
    showSelectUser: function (setting, client) {
        VueUserSelectClient = client;
        if (JQZepto("#bpf_userselect").length == 0) {
            JQZepto("body").append('<div id="bpf_userselect"></div>');
        }
        if (setting.isNeedHiddenNav) {
            bpf_userselect_tool.hiddenNavi();
        }
        if (BPF_UserSelect_Model == null) {
            JQZepto("#bpf_userselect").append(this._content());
            this._initViewModel(setting);
        }
        VueUserSelectClient.repairUserAvatartUrl(setting.waitUserList, setting.checkedUserList, function (data) {
            if (data == undefined) {
                BPF_UserSelect_Model.$data.postWaitUserList = setting.waitUserList;
                BPF_UserSelect_Model.$data.checkedUserList = setting.checkedUserList;
                //获取是否显示常用联系人
                VueUserSelectClient.getEnableTopUserSelect(function (enbaleTopUserSelect) {
                    BPF_UserSelect_Model.$data.isShowTopUserList = enbaleTopUserSelect;
                })
            }
            else {
                BPF_UserSelect_Model.$data.isShowTopUserList = data.EnbaleTopUserSelect;
                BPF_UserSelect_Model.$data.postWaitUserList = data.WaitUserList;
                BPF_UserSelect_Model.$data.checkedUserList = data.CheckedUserList;
            }
            BPF_UserSelect_Model.$data.allowMulti = setting.allowMulti;
            BPF_UserSelect_Model.$data.exceptUserList = setting.exceptUserList;
            BPF_UserSelect_Model.$data.allowAll = setting.allowAll;
            BPF_UserSelect_Model.$data.isShowAllowAll = !BPF_UserSelect_Model.isHasPostWaitUser;
            BPF_UserSelect_Model.$data.isShow = true;
            BPF_UserSelect_Model.$data.isNeedHiddenNav = setting.isNeedHiddenNav;
            BPF_UserSelect_Model.$data.callback = setting.func;
            BPF_UserSelect_Model.$data.exit = setting.exit;
            BPF_UserSelect_Model.$data.topValue = setting.topValue;
            BPF_UserSelect_Model.$data.setting = setting;
            if (BPF_UserSelect_Model.$data.isShowAllowAll) {
                BPF_UserSelect_Model.leftOrgRootClick();
            }
            else {
                BPF_UserSelect_Model.processPostWaitUserCheckedStatus();
            }
        })
    },
    imgError: function () {
        var img = event.srcElement;
        img.src = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGQAAACGCAMAAAAcsN/vAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkFDOTdCMDE2MjhBMzExRTc4RDNFRTRDMTVGMjJBMDQ1IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkFDOTdCMDE3MjhBMzExRTc4RDNFRTRDMTVGMjJBMDQ1Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6QUM5N0IwMTQyOEEzMTFFNzhEM0VFNEMxNUYyMkEwNDUiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6QUM5N0IwMTUyOEEzMTFFNzhEM0VFNEMxNUYyMkEwNDUiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz52RMIZAAAAwFBMVEUwibprsNHU5vEv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';
    },
    _initViewModel: function (setting) {
        BPF_UserSelect_Model = new Vue({
            el: "#bpf_userselect",
            data: {
                setting: null,//配置信息
                keyWord: "",//关键字
                postWaitUserList: [],//原始候选人列表
                waitUserList: [],//候选人列表
                remainUserCount: 0,//剩余未加载的人员列表数目
                checkedUserList: [],//已选人列表
                exceptUserList: [],//需要去重的人员列表
                leftOrgList: [],//左侧面包屑组织机构列表
                rightOrgList: [],//右侧组织机构列表展示
                currentUserInfo: {
                    userInfo: {},//当前显示的已选中的用户
                    isShow: false,//是否显示当前已选中用户
                    userIndex: -1//当前用户顺序号
                },
                lang: $userselectlang.Mobile,
                isNeedHiddenNav: true,
                callback: function (data) { },
                exit: function () { },
                isShowAllowAll: true,//是否显示全局选人页面，false表示显示候选人页面
                isShow: false,//是否显示选人控件
                isCheckAll: true,//是否是全选状态
                isShowOrgList: true,//是否显示右侧组织机构列表信息，在根据关键字搜索时设置为false
                isRootOrgList: false,//是否是根级的组织机构信息
                isShowTopUserList: false,//是否显示常用联系人
                allowMulti: true,//是否允许多选
                allowAll: true,//是否允许全局选人
                topValue: 0
            },
            computed: {
                checkedUserCount: function () {
                    //已选人数
                    return this.checkedUserList.length;
                },
                isShowCheckAll: function () {
                    if (this.allowMulti == false) {
                        return false;
                    }
                    //是否显示全选按钮
                    if (this.isShowAllowAll) {
                        return this.waitUserList.length > 0;
                    }
                    else {
                        return this.postWaitUserList.length > 0;
                    }
                },
                isHasPostWaitUser: function () {
                    //是否有原始候选人列表
                    return this.postWaitUserList.length > 0;
                },
                checkedUserCodeList: function () {
                    //已选择的用户Code列表
                    return Enumerable.From(this.checkedUserList).Select(function (data) {
                        return data.UserCode
                    }).ToArray();
                },
                waitUserCodeList: function () {
                    //等待选择的用户Code列表
                    return Enumerable.From(this.waitUserList).Select(function (data) {
                        return data.UserCode
                    }).ToArray();
                },
                postWaitUserCodeList: function () {
                    //原始候选人用户Code列表
                    return Enumerable.From(this.postWaitUserList).Select(function (data) {
                        return data.UserCode
                    }).ToArray();
                }
            },
            methods: {
                _processCheckedStatus: function (waitUserList, func) {
                    //处理选中的状态
                    var self = this;
                    self.isCheckAll = true;
                    self.remainUserCount = 0;
                    JQZepto.each(waitUserList, function (i, item) {
                        if (JQZepto.inArray(item.UserCode, self.checkedUserCodeList) > -1) {
                            item.checked = true;
                        }
                    })
                    if (func != undefined) {
                        func(waitUserList);
                    }
                },
                _processGetOrgListAndUserListByParentOrgID: function (orgID) {
                    //获取组织机构的人员和下级组织机构列表
                    var self = this;
                    self.isShowOrgList = true;
                    if (orgID != '1') {
                        self.isRootOrgList = false;
                    }
                    VueUserSelectClient.getOrgListAndUserListByParentOrgID(orgID, function (data) {
                        self._processCheckedStatus(data.UserList, function (userList) {
                            //处理选中状态
                            self.waitUserList = userList;
                            self.remainUserCount = data.RemainUserCount;
                        });
                        self.rightOrgList = data.OrgList;//TODO此处可能无法更新视图
                    });
                },
                searchUser: function () {
                    //根据关键字搜索用户
                    var self = this;
                    var keyWord = self.keyWord;//获取关键字
                    VueUserSelectClient.getUserListBySearchKey(keyWord, function (data) {
                        self._processCheckedStatus(data, function (userList) {
                            //处理选中状态
                            self.waitUserList = userList;
                        });
                        self.isShowOrgList = false;//隐藏组织机构
                    })
                },
                checkAllWaitUser: function () {
                    //全选或取消全选
                    var self = this;
                    var waitUserList = [];
                    if (self.isShowAllowAll) {
                        waitUserList = self.waitUserList
                    }
                    else {
                        waitUserList = self.postWaitUserList
                    }
                    if (self.isCheckAll) {
                        JQZepto.each(waitUserList, function (i, item) {
                            if (JQZepto.inArray(item.UserCode, self.checkedUserCodeList) == -1) {
                                if (!self.allowMulti && self.checkedUserList.length >= 1) {
                                    bpf_userselect_tool.tips("不允许选择多个人员");
                                    return false;
                                }
                                else {
                                    item.checked = true;
                                    self.checkedUserList.push(item);
                                }
                            }
                        })
                    }
                    else {
                        JQZepto.each(waitUserList, function (i, item) {
                            item.checked = false;
                            var index = JQZepto.inArray(item.UserCode, self.checkedUserCodeList);
                            if (index > -1) {
                                self.checkedUserList.splice(index, 1);
                            }
                        })
                    }
                    self.isCheckAll = !self.isCheckAll;
                },
                leftOrgClick: function (orgInfo, index) {
                    //点击左侧组织机构
                    var self = this;
                    if (orgInfo.OrgID == "-10000") {
                        self.showTopUserList(false);
                    }
                    else {
                        //清空当前级别后的组织机构
                        self.leftOrgList.splice(index + 1, self.leftOrgList.length);
                        //左侧面包屑组织机构点击事件
                        self._processGetOrgListAndUserListByParentOrgID(orgInfo.OrgID); //加载下一级组织机构信息
                    }
                },
                leftOrgRootClick: function () {
                    //点击组织架构按钮
                    var self = this;
                    self.isRootOrgList = true;
                    //左侧面包屑根节点组织机构点击事件
                    self.leftOrgList = [];//TODO此处可能无法更新视图
                    //加载下一级组织机构信息
                    self._processGetOrgListAndUserListByParentOrgID('2');
                },
                checkedUser: function (userInfo) {
                    //选择用户
                    var self = this;
                    if (userInfo.checked) {
                        userInfo.checked = !userInfo.checked;
                        //取消选中
                        var index = JQZepto.inArray(userInfo.UserCode, self.checkedUserCodeList);
                        if (index > -1) {
                            self.checkedUserList.splice(index, 1);
                        }
                    }
                    else {
                        //选中用户
                        if (JQZepto.inArray(userInfo.UserCode, self.checkedUserCodeList) == -1) {
                            if (self.allowMulti) {
                                userInfo.checked = !userInfo.checked;
                                self.checkedUserList.push(userInfo);
                            }
                            else {
                                JQZepto.each(self.checkedUserList, function (i, item) {
                                    self.deleteCheckedUserInfo(item, i);
                                })
                                userInfo.checked = true;
                                self.checkedUserList.push(userInfo);
                            }
                        }
                    }
                },
                rightOrgClick: function (orgInfo) {
                    //右侧组织机构列表点击事件
                    var self = this;
                    var orgID = orgInfo.OrgID;
                    //将当前组织机构添加到左侧面包屑列表
                    self.leftOrgList.push(orgInfo);
                    //加载下一级组织机构信息
                    self._processGetOrgListAndUserListByParentOrgID(orgID);
                },
                processPostWaitUserCheckedStatus: function () {
                    //处理候选人选中的状态
                    var self = this;
                    self.isCheckAll = true;
                    var waitUserList = self.postWaitUserList;
                    JQZepto.each(waitUserList, function (i, item) {
                        if (JQZepto.inArray(item.UserCode, self.checkedUserCodeList) > -1) {
                            item.checked = true;
                        }
                    })
                    self.postWaitUserList = waitUserList;
                },
                showCheckedUserInfo: function (userInfo, index) {
                    //显示选中用户信息
                    var self = this;
                    if (index == self.currentUserInfo.userIndex) {
                        self.hideCheckedUserInfo();
                    }
                    else {
                        //弹出显示人员详细信息
                        self.currentUserInfo.userInfo = userInfo;
                        self.currentUserInfo.userIndex = index;
                        self.currentUserInfo.isShow = true;
                    }
                },
                hideCheckedUserInfo: function () {
                    var self = this;
                    //隐藏显示人员详细信息
                    self.currentUserInfo.isShow = false;
                    Vue.set(self.currentUserInfo, "userInfo", {});
                    Vue.set(self.currentUserInfo, "isShow", false);
                    Vue.set(self.currentUserInfo, "userIndex", -1);
                },
                deleteCheckedUserInfo: function (deleteUserInfo, deleteUserIndex) {
                    //删除选择的用户
                    var self = this;
                    var userCode = deleteUserInfo.UserCode;
                    var userIndex = deleteUserIndex;
                    var waitIndex = JQZepto.inArray(userCode, self.waitUserCodeList);
                    if (waitIndex > -1) {
                        var waitUserInfo = self.waitUserList[waitIndex];
                        waitUserInfo.checked = false;
                        Vue.set(self.waitUserList, waitIndex, waitUserInfo);
                    }
                    var postWaitIndex = JQZepto.inArray(userCode, self.postWaitUserCodeList);
                    if (postWaitIndex > -1) {
                        var postWaitUserInfo = self.postWaitUserList[postWaitIndex];
                        postWaitUserInfo.checked = false;
                        Vue.set(self.postWaitUserList, postWaitIndex, postWaitUserInfo);
                    }
                    self.checkedUserList.splice(userIndex, 1);
                    self.hideCheckedUserInfo();
                },
                moveCheckedUserInfo: function (type) {
                    //移动用户顺序,type（Pre：前移，Next：后移）
                    var self = this;
                    var userInfo = self.currentUserInfo.userInfo;
                    var index = self.currentUserInfo.userIndex;
                    if (type == 'Pre') {
                        if (index > 0) {
                            var preUserInfo = self.checkedUserList[index - 1];
                            Vue.set(self.checkedUserList, index, preUserInfo);
                            Vue.set(self.checkedUserList, index - 1, userInfo);
                            self.currentUserInfo.userIndex = index - 1;
                        }
                    }
                    else if (type == 'Next') {
                        if (index + 1 < self.checkedUserCount) {
                            var nextUserInfo = self.checkedUserList[index + 1];
                            Vue.set(self.checkedUserList, index, nextUserInfo);
                            Vue.set(self.checkedUserList, index + 1, userInfo);
                            self.currentUserInfo.userIndex = index + 1;
                        }
                    }
                },
                showTopUserList: function (isAddLeftOrg) {
                    //显示常用联系人
                    var self = this;
                    if (isAddLeftOrg === false) {
                    }
                    else {
                        var orgInfo = {
                            OrgID: "-10000",
                            OrgCode: "-10000",
                            OrgName: self.lang.topUser,
                            ShortName: self.lang.topUser
                        }
                        //将当前组织机构添加到左侧面包屑列表
                        self.leftOrgList.push(orgInfo);
                    }
                    self.isShowOrgList = false;

                    VueUserSelectClient.getTopUserList(function (data) {
                        self._processCheckedStatus(data, function (userList) {
                            self.waitUserList = userList;
                        });
                    }, self.setting)
                },
                saveUserList: function () {
                    //确定选择用户
                    var self = this;
                    var exceptArray = self.exceptUserList;
                    var data = Enumerable.From(self.checkedUserList).Distinct(function (data) {
                        return data.UserCode
                    }).ToArray();
                    if (exceptArray != null && exceptArray != undefined) {
                        data = Enumerable.From(data).Except(exceptArray, function (data) {
                            return data.UserCode
                        }).ToArray();
                    }
                    self.callback(data);
                    if (data.length > 0) {
                        setTimeout(function () {
                            VueUserSelectClient.saveUserSelectResult(data, self.setting);
                        })
                    }
                    self._reset();
                },
                showAllowAll: function () {
                    //显示全局选人
                    var self = this;
                    self.isShowAllowAll = true;
                    if (self.leftOrgList.length == 0) {
                        self.leftOrgRootClick();
                    }
                },
                _reset: function () {
                    //
                    var self = this;
                    self.checkedUserList = [];
                    self.postWaitUserList = [];//原始候选人列表
                    self.waitUserList = [];//候选人列表
                    self.checkedUserList = [];//已选人列表
                    self.leftOrgList = [];//左侧面包屑组织机构列表
                    self.rightOrgList = [];//右侧组织机构列表展示
                    self.currentUserInfo = {
                        userInfo: {},//当前显示的已选中的用户
                        isShow: false,//是否显示当前已选中用户
                        userIndex: -1//当前用户顺序号
                    };
                    if (self.isNeedHiddenNav) {
                        //需要显示导航
                        bpf_userselect_tool.showNavi();
                    }
                    self.isShow = false;
                },
                close: function () {
                    var self = this;
                    if (self.isHasPostWaitUser && self.isShowAllowAll) {
                        //返回到候选人页面
                        this.isShowAllowAll = false;
                        self.processPostWaitUserCheckedStatus();
                    }
                    else {
                        self.$data.exit();
                        self._reset();
                    }
                },
                returnBackTop: function () {
                    var oTarget = document.getElementById("scroll-content");
                    oTarget.scrollTop = "0";
                }
            }
        });
    },
    _content: function () {
        return ['<div class="inset_wrapper" v-show="isShow" v-cloak="">',
            '            <!-- 通讯列表 -->',
            '            <div id="bpf_userselect_menu" class="bpf_userselect_menu">',
            '                <!-- 选择用户 -->',
            '                <div class="bpf_userselect_user_wrap" v-bind:style="{\'padding-top\': 38 + topValue + \'px\'}" v-if="allowAll" v-show="isShowAllowAll">',
            '                    <!-- 搜索内容 -->',
            '                    <div class="bpf_userselect_search_wrap" style="top: 0;">',
            '                        <div class="bpf_userselect_search_text" style="width:55%;">',
            '                            <input type="text" class="bpf_userselect_input bpf_userselect_search" name="search" v-model="keyWord">',
            '                        </div>',
            '                        <span class="bpf_userselect_btn bpf_userselect_search_btn" v-on:click="searchUser">{{lang.search}}</span>',
            '                      <span class="bpf_userselect_btn bpf_userselect_search_btn bpf_userselect_header_back" v-on:click="close()" style="border: 1px solid #cb5c61;background:transparent;color:#000;float:right;">返回</span>',
            '                    </div>',
            '                    <!-- 用户列表 -->',
            '                    <div class="bpf_userselect_list_wrap">',
            '                        <ul class="bpf_userselect_bread_crumbs" v-bind:style="{\'top\': 38 + topValue + \'px\'}" v-cloak="">',
            '                            <li class="bpf_userselect_bread_active" v-on:click="leftOrgRootClick"><span>{{lang.orgRoot}}</span></li>',
            '                            <li class="bpf_userselect_bread_cur" v-for="(orgInfo,index) in leftOrgList" v-on:click="leftOrgClick(orgInfo,index)"><span>{{orgInfo.ShortName}}</span></li>',
            '                        </ul>',
            '                        <div class="bpf_userselect_list">',
            '                           <div id="scroll-content" class="bpf_userselect_list_inner">',
            '                             <ul class="bpf_userselect_personal_info" v-cloak="">',
            '                                <li v-for="(userInfo,index) in waitUserList" v-on:click="checkedUser(userInfo)" v-bind:class="{\'bpf_userselect_selected_active\': (userInfo.checked==true)}">',
            '                                    <img class="bpf_userselect_defaultavatar" v-bind:src="userInfo.ExtensionInfo" onerror="bpf_userselect_tool.imgError()" alt="" style="width: auto;">',
            '                                    <div class="bpf_userselect_personal_cont" style="padding-left:70px">',
            '                                        <p class="bpf_userselect_personal_name">{{userInfo.UserName}}（{{userInfo.UserLoginID}}）</p>',
            '                                        <p style="white-space: pre-line;">{{userInfo.UserOrgPathName}}</p>',
            '                                        <p>{{userInfo.UserJobName}}</p>',
            '                                    </div>',
            '                                    <span class="bpf_userselect_selected_icon"></span>',
            '                                </li>',
            '                            </ul>',
            '                            <div class="bpf_userselect_search_more" v-if="remainUserCount>0"><p>{{lang.remainUserInfo}}</p></div>',
            '                            <ul class="bpf_userselect_list_info_top" v-show="isShowOrgList" v-cloak="">',
            '                                <li v-for="(orgInfo,index) in rightOrgList" v-on:click="rightOrgClick(orgInfo)">{{orgInfo.ShortName}}<span></span></li>',
            '                            </ul>',
            '                            <!--<p class="bpf_userselect_list_label">请选择默认列表</p>-->',
            '                            <ul class="bpf_userselect_list_bdt" v-if="isShowOrgList&&isRootOrgList&&isShowTopUserList" v-cloak="">',
            '                                <li v-on:click="showTopUserList">{{lang.topUser}}<span></span></li>',
            '                            </ul>',
            '                           </div>',
            '                        </div>',
            '                    </div>',
            '                    <!-- bpf_userselect_list_wrap -->',
            '                </div>',
            '                <!-- 候选人 -->',
            '                <div class="bpf_userselect_candidate_wrap" v-bind:style="{\'top\': 70 + topValue + \'px\'}" v-if="isHasPostWaitUser" v-show="!isShowAllowAll">',
            '                    <div class="bpf_userselect_candidate_list">',
            '                        <ul class="bpf_userselect_personal_info" v-cloak="">',
            '                            <li v-for="(userInfo,index) in postWaitUserList" v-on:click="checkedUser(userInfo)" v-bind:class="{\'bpf_userselect_selected_active\': (userInfo.checked==true)}">',
            '                                <img class="bpf_userselect_defaultavatar" v-bind:src="userInfo.ExtensionInfo" onerror="bpf_userselect_tool.imgError()" alt="">',
            '                                <div class="bpf_userselect_personal_cont">',
            '                                    <p class="bpf_userselect_personal_name">{{userInfo.UserName}}（{{userInfo.UserLoginID}}）</p>',
            '                                    <p style="white-space: pre-line;">{{userInfo.UserOrgPathName}}</p>',
            '                                    <p>{{userInfo.UserJobName}}</p>',
            '                                </div>',
            '                                <span class="bpf_userselect_selected_icon"></span>',
            '                            </li>',
            '                        </ul>',
            '                        <span class="bpf_userselect_view_more" v-if="allowAll" v-on:click="showAllowAll">{{lang.allowAll}}</span>',
            '                    </div>',
            '                </div>',
            '                <div class="bpf_userselect_selected_wrap">',
            '                    <div class="bpf_userselect_tags_wrap">',
            '                        <ul v-bind:style="{width: 42*checkedUserList.length + \'px\'}" v-cloak="">',
            '                            <li v-for="(userInfo,index) in checkedUserList" v-bind:class="{\'bpf_userselect_selected_cur\': currentUserInfo.userIndex==index}" v-on:click="showCheckedUserInfo(userInfo,index)">',
            '                                <img class="bpf_userselect_defaultavatar" v-bind:src="userInfo.ExtensionInfo" onerror="bpf_userselect_tool.imgError()" alt="">',
            '                                <span>{{userInfo.UserName}}</span>',
            '                            </li>',
            '                        </ul>',
            '                    </div>',
            '                    <div class="bpf_userselect_tags_action">',
            '                        <div class="bpf_userselect_btn bpf_userselect_btn_add" v-on:click="saveUserList">',
            '                            {{lang.submit}}',
            '                            <span v-cloak="">({{checkedUserCount}})</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '                <!-- 点击头像查看个人信息 -->',
            '                <div class="bpf_userselect_cur_info_wrap" v-cloak="" v-show="currentUserInfo.isShow" v-on:click.stop.prevent="hideCheckedUserInfo">',
            '                    <div class="bpf_userselect_current_info">',
            '                        <span class="bpf_userselect_delete_btn" v-on:click.stop.prevent="deleteCheckedUserInfo(currentUserInfo.userInfo,currentUserInfo.userIndex)">{{lang.remove}}</span>',
            '                        <div class="bpf_userselect_personal_info">',
            '                            <img class="bpf_userselect_personal_avatar bpf_userselect_defaultavatar" v-bind:src="currentUserInfo.userInfo.ExtensionInfo" onerror="bpf_userselect_tool.imgError()" alt="">',
            '                            <div class="bpf_userselect_personal_cont">',
            '                                <p class="bpf_userselect_personal_top_name">{{currentUserInfo.userInfo.UserName}}（{{currentUserInfo.userInfo.UserLoginID}}）</p>',
            '                                <p style="white-space: pre-line;">{{currentUserInfo.userInfo.UserOrgPathName}}</p>',
            '                                <p>{{currentUserInfo.userInfo.UserJobName}}</p>',
            '                            </div>',
            '                        </div>',
            '                        <div class="bpf_userselect_move_wrap">',
            '                            <span class="bpf_userselect_prev_btn" v-on:click.stop.prevent="moveCheckedUserInfo(\'Pre\')">&lt;&lt; {{lang.pre}}</span>',
            '                            <div class="bpf_userselect_count">',
            '                                <span class="bpf_userselect_num">{{currentUserInfo.userIndex+1}}</span>',
            '                                <span>/</span>',
            '                                <span class="bpf_userselect_total">{{checkedUserCount}}</span>',
            '                            </div>',
            '                            <span class="bpf_userselect_next_btn" v-on:click.stop.prevent="moveCheckedUserInfo(\'Next\')">{{lang.next}} &gt;&gt;</span>',
            '                        </div>',
            '                    </div>',
            '                </div>',
            '            </div>        <!-- 通讯列表 -->',
            '        </div>'].join("");
    },
    //隐藏集团APP的导航
    hiddenNavi: function () {
        try {
            cordova.exec(null, null, "WDNaviPlugin", "hiddenNavi", ["1"]);
        }
        catch (ex) {
            console.log(ex);
        }
    },
    //隐藏集团APP的导航
    showNavi: function () {
        try {
            cordova.exec(null, null, "WDNaviPlugin", "hiddenNavi", ["0"]);
        }
        catch (ex) {
            console.log(ex);
        }
    }
}