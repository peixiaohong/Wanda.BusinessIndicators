var user_select_model = null;


var user_select_ctrl = {
    _content: function () {

        return [
            "<div class=\"inset_wrapper\" v-if=\"show\">",
            "",
            "            <div id=\"wanda_userselect_menu\" class=\"wanda_userselect_menu\">",
            "                <a id=\"wanda_userselect_back_top\" href=\"#\" v-on:click=\"top()\" class=\"wanda_userselect_back_top\"></a>",
            "                <div class=\"wanda_userselect_header\">",
            "                    <h1 style=\"padding-top:16px\">",
            "                        <span class=\"wanda_userselect_header_back\" v-on:click=\"back()\" style=\"top:13px;\"></span>",
            "                        选择用户",
            "                        <span style=\"top:13px;\" v-if=\"currentlist.userlist.length>0\" v-on:click=\"selectall()\" v-bind:class=\"[isselectall?'wanda_userselect_unselect_all':'wanda_userselect_select_all']\"></span>",
            "                        <!-- wanda_userselect_unselect_all -->",
            "                    </h1>",
            "                </div>",
            "",
            "                <div class=\"wanda_userselect_user_wrap\" style=\"padding-top:108px\">",
            "                    <!-- 搜索内容 -->",
            "                    <div class=\"wanda_userselect_search_wrap\" style=\"top:56px;\">",
            "                        <div class=\"wanda_userselect_search_text\">",
            "                            <input type=\"text\" class=\"wanda_userselect_input wanda_userselect_search\" name=\"search\" v-model=\"searchkey\">",
            "                        </div>",
            "                        <span class=\"wanda_userselect_btn wanda_userselect_search_btn\" v-on:click=\"search()\">搜索</span>",
            "                    </div>",
            "                    <!-- 用户列表 -->",
            "                    <div class=\"wanda_userselect_list_wrap\">",
            "                        <ul class=\"wanda_userselect_bread_crumbs\" style=\"top:108px;\">",
            "                            <li v-if=\"!bysearch\" v-bind:class=\"{'wanda_userselect_bread_active':index==0 ,'wanda_userselect_bread_cur':index>0}\" v-for=\"(org,index) in orgpath\" v-on:click=\"orgclick(org)\"><span v-text=\"org.DeptName\"></span></li>",
            "                            <li v-if=\"bysearch\" class=\"wanda_userselect_bread_active\" v-on:click=\"orgclick({ID:2})\"><span>隆基泰和控股集团</span></li>",
            "                        </ul>",
            "                        <div class=\"wanda_userselect_list\">",
            "                            <div id=\"scroll-content\" class=\"wanda_userselect_list_inner\">",
            "",
            "                                <ul class=\"wanda_userselect_personal_info\">",
            "                                    <li class=\"wanda_userselect_selected\" v-for=\"(user,index) in currentlist.userlist\" v-on:click=\"userclick(user)\"",
            "                                        v-bind:class=\"{'wanda_userselect_selected_active':user.selected}\">",
            "                                        <img class=\"wanda_userselect_defaultavatar\" v-bind:src=\"user.Thumb\" v-on:error=\"error(user)\">",
            "                                        <div class=\"wanda_userselect_personal_cont\">",
            "                                            <p class=\"wanda_userselect_personal_name\" v-text=\"user.DisplayName+'('+user.LoginName+')'\"></p>",
            "                                            <p style=\"white-space: pre-line;\" v-text=\"user.Department\"></p>",
            "                                            <p v-text=\"user.JobTitle\"></p>",
            "                                        </div>",
            "                                        <span class=\"wanda_userselect_selected_icon\"></span>",
            "                                    </li>",
            "                                </ul>",
            "",
            "",
            "                                <ul class=\"wanda_userselect_list_info_top\" v-if=\"currentlist.orglist.length>0\">",
            "                                    <li v-for=\"(org,index) in currentlist.orglist\" v-on:click=\"orgclick(org)\">{{org.DeptName}}<span></span></li>",
            "                                </ul>",
            "",
            "                            </div>",
            "",
            "",
            "",
            "                        </div>",
            "",
            "                    </div>",
            "                </div>",
            "",
            "",
            "                <div class=\"wanda_userselect_selected_wrap\">",
            "                    <div class=\"wanda_userselect_tags_wrap\">",
            "                        <ul v-bind:style=\"{width: 42*checkuserlist.length + 'px'}\" v-cloak=\"\">",
            "                            <li v-for=\"(user,index) in checkuserlist\"",
            "                                v-bind:class=\"currentselect!=null&&currentselect.LoginName==user.LoginName?'wanda_userselect_selected_cur':''\" v-on:click=\"showinfo(user)\">",
            "                                <img class=\"wanda_userselect_defaultavatar\" v-bind:src=\"user.Thumb\" v-on:error=\"error(user)\">",
            "                                <span v-text=\"user.DisplayName\"></span>",
            "                            </li>",
            "                        </ul>",
            "                    </div>",
            "                    <div class=\"wanda_userselect_tags_action\">",
            "                        <div class=\"wanda_userselect_btn wanda_userselect_btn_add\" v-on:click=\"save()\">",
            "                            确定<span v-text=\"'('+checkuserlist.length+')'\"></span>",
            "                        </div>",
            "                    </div>",
            "                </div>",
            "",
            "                <div class=\"wanda_userselect_cur_info_wrap\" v-if=\"currentselect!=null\" v-on:click.stop.prevent=\"hideinfo()\">",
            "                    <div class=\"wanda_userselect_current_info\">",
            "                        <span class=\"wanda_userselect_delete_btn\"",
            "                              v-on:click.stop.prevent=\"remove()\">移除</span>",
            "                        <div class=\"wanda_userselect_personal_info\">",
            "                            <img class=\"wanda_userselect_personal_avatar wanda_userselect_defaultavatar\" v v-bind:src=\"currentselect.Thumb\" v-on:error=\"error(currentselect) \">",
            "                            <div class=\"wanda_userselect_personal_cont\">",
            "                                <p class=\"wanda_userselect_personal_top_name\" v-text=\"currentselect.DisplayName+'('+currentselect.LoginName+')'\"></p>",
            "                                <p style=\"white-space: pre-line;\" v-text=\"currentselect.Department\"></p>",
            "                                <p v-text=\"currentselect.JobTitle\"></p>",
            "                            </div>",
            "                        </div>",
            "                        <div class=\"wanda_userselect_move_wrap\">",
            "                            <span class=\"wanda_userselect_prev_btn\" v-on:click.stop.prevent=\"prev()\">&lt;&lt; 前移</span>",
            "                            <div class=\"wanda_userselect_count\">",
            "                                <span class=\"wanda_userselect_num\" v-text=\"checkuserindex\"></span>",
            "                                <span>/</span>",
            "                                <span class=\"wanda_userselect_total\" v-text=\"checkuserlist.length\"></span>",
            "                            </div>",
            "                            <span class=\"wanda_userselect_next_btn\" v-on:click.stop.prevent=\"next()\">后移 &gt;&gt;</span>",
            "                        </div>",
            "                    </div>",
            "                </div>",
            "",
            "           </div>",
            "",
            "        </div>"

        ].join("")
    },
    _ajax: function (url, data, callback) {
        //1.创建
        var oAjax = null;
        if (window.XMLHttpRequest) { //对ie6来说，直接用XMLHttpRequest是不存在的会出错
            oAjax = new XMLHttpRequest(); //ie6以上
        } else {
            oAjax = new ActiveXObject("Microsoft.XMLHTTP"); //ie6
        }
        //2.连接服务器，open(方法，url，是否异步)
        oAjax.open('GET', url, true);
        //3.发送请求
        oAjax.send(data);
        //4.接收返回 OnReadyStateChange
        oAjax.onreadystatechange = function () {    //onreadystatechange事件
            if (oAjax.readyState == 4) { //readyState属性：请求状态 4是完成（完成不代表成功）
                if (oAjax.status == 200) { //status属性：请求结果 200代表成功
                    if (callback != undefined) {
                        callback(JSON.parse(oAjax.responseText)); //responseText属性：服务器发回给我们的内容
                    }
                }
                else {
                    alert('获取数据失败');
                }
            }
        };
    },
    show: function (callback) {
        var warp = document.getElementById("wanda_userselect");
        if (warp == null) {
            warp = document.createElement('div');
            warp.id = "wanda_userselect";
            document.body.appendChild(warp);
        }


        if (user_select_model == null) {
            user_select_model = new Vue({
                el: "#wanda_userselect",
                template: user_select_ctrl._content(),
                data: {
                    show: false,
                    bysearch: false,
                    searchkey: "",
                    currentorg: null,
                    orgpath: null,
                    currentselect: null,
                    checkuserlist: [],
                    currentlist: {
                        checkuserlist: [],
                        userlist: [], orglist: []
                    }
                },
                computed: {
                    isselectall: function () {
                        var data = this;
                        var isall = true;
                        for (var i = 0; i < data.currentlist.userlist.length; i++) {
                            if (!data.currentlist.userlist[i].selected) {
                                isall = false;
                                break;
                            }
                        }
                        return isall;
                    },
                    checkuserindex: function () {
                        var data = this;
                        var index = 0;
                        for (var i = 0; i < data.checkuserlist.length; i++) {
                            index++;
                            if (data.checkuserlist[i].LoginName == data.currentselect.LoginName) {
                                break;
                            }

                        }
                        return index;
                    }
                    //,
                    //checkuserlist: function () {
                    //    var data = this;
                    //    var array = new Array();
                    //    for (var i = 0; i < data.currentlist.userlist.length; i++) {
                    //        if (data.currentlist.userlist[i].selected) {
                    //            array.push(data.currentlist.userlist[i]);
                    //        }
                    //    }
                    //    return array;
                    //}
                },
                watch: {
                    currentorg: function () {
                        var data = this;
                        data.bysearch = false;
                        data.initcurrentorg(data.currentorg.ID, "", data.bysearch, function (result) {
                            data.currentlist.orglist = result.Children;
                            data.orgpath = result.Previous;
                            var users = data.builduserlist(result.Users);
                            data.currentlist.userlist = users;
                        });
                    },
                    bysearch: function () {
                        var data = this;
                        if (!data.bysearch) {
                            data.searchkey = "";
                        }
                    }
                },
                mounted: function () {
                    var data = this;
                    data.bysearch = false;
                    //初始化时加载默认组织架构
                    data.initcurrentorg("-1", "", data.bysearch, function (result) {
                        data.currentorg = result.Children[0];
                    })
                },
                methods: {
                    builduserlist: function (users) {
                        var data = this;
                        for (var i = 0; i < users.length; i++) {
                            users[i].selected = false;
                            for (var j = 0; j < data.checkuserlist.length; j++) {
                                if (data.checkuserlist[j].LoginName == users[i].LoginName) {
                                    users[i].selected = true;
                                }
                            }
                            users[i].Thumb = "AvatarHandller.ashx?uid=" + users[i].EmployeeID;
                        }
                        return users;
                    },
                    error: function (user) {
                        user.Thumb = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGQAAACGCAMAAAAcsN/vAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkFDOTdCMDE2MjhBMzExRTc4RDNFRTRDMTVGMjJBMDQ1IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkFDOTdCMDE3MjhBMzExRTc4RDNFRTRDMTVGMjJBMDQ1Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6QUM5N0IwMTQyOEEzMTFFNzhEM0VFNEMxNUYyMkEwNDUiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6QUM5N0IwMTUyOEEzMTFFNzhEM0VFNEMxNUYyMkEwNDUiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz52RMIZAAAAwFBMVEUwibprsNHU5vEv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";
                    },
                    selectall: function () {
                        var data = this;
                        var isselectall = data.isselectall;
                        var userlist = new Array();
                        var list = data.checkuserlist;
                        for (var i = 0; i < data.currentlist.userlist.length; i++) {
                            var item = data.currentlist.userlist[i];
                            item.selected = !isselectall;
                            userlist.push(item);
                        }
                        //处理checkuserlist集合
                        var checkuserlist = data.checkuserlist;
                        for (var i = 0; i < data.currentlist.userlist.length; i++) {
                            var item = data.currentlist.userlist[i];
                            var existIndex = -1;
                            for (var j = 0; j < checkuserlist.length; j++) {
                                var inner = checkuserlist[j];
                                if (inner.LoginName == item.LoginName) {
                                    existIndex = j;
                                }
                            }
                            //如果是选中状态
                            if (item.selected) {
                                //看原来的集合中是否存在，如果存在则无需新增 如果不存在 则需要新增
                                if (existIndex < 0) {
                                    checkuserlist.push(item);
                                }
                            }
                            //如果不是选中状态
                            else {
                                //看原来的集合中是否存在，如果存在则需要移除 如果不存在则不需要处理
                                if (existIndex >= 0) {
                                    for (var j = 0; j < checkuserlist.length; j++) {
                                        var inner = checkuserlist[j];
                                        if (inner.LoginName == item.LoginName) {
                                            checkuserlist.splice(j, 1);
                                            break;
                                        }
                                    }

                                }
                            }

                        }
                        data.checkuserlist = checkuserlist;
                    },
                    remove: function () {
                        var data = this;
                        var userlist = data.checkuserlist;
                        var array = new Array();
                        for (var i = 0; i < userlist.length; i++) {
                            if (userlist[i].LoginName != data.currentselect.LoginName) {
                                array.push(userlist[i]);
                            }
                        }
                        data.checkuserlist = array;
                        var list = new Array();
                        for (var i = 0; i < data.currentlist.userlist.length; i++) {
                            var item = data.currentlist.userlist[i];
                            var existindex = -1;
                            for (var j = 0; j < array.length; j++) {
                                var inner = array[j];
                                if (inner.LoginName == item.LoginName) {
                                    item.selected = true;
                                    existindex = j;
                                }
                            }
                            if (existindex < 0) {
                                item.selected = false;
                            }
                        }
                        data.currentselect = null;
                    },
                    prev: function () {
                        var data = this;
                        var array = new Array();
                        for (var i = 0; i < data.checkuserlist.length; i++) {
                            var item = data.checkuserlist[i];
                            if (item.LoginName == data.currentselect.LoginName) {
                                if (i == 0) {
                                    array.push(item);
                                } else {
                                    var prev = array[i - 1];
                                    array.splice(i - 1, 1);
                                    array.push(item);
                                    array.push(prev);
                                }
                            } else {
                                array.push(item);
                            }
                        }
                        data.checkuserlist = array;
                    },
                    next: function () {
                        var data = this;
                        var array = new Array();
                        for (var i = 0; i < data.checkuserlist.length; i++) {
                            var item = data.checkuserlist[i];
                            if (item.LoginName == data.currentselect.LoginName) {
                                if (i == data.checkuserlist.length - 1) {
                                    array.push(item);
                                } else {
                                    array.push(data.checkuserlist[i + 1]);
                                    array.push(item);
                                    i++;
                                }
                            }
                            else {
                                array.push(item);
                            }
                        }
                        data.checkuserlist = array;
                    },
                    hideinfo: function () {
                        var data = this;
                        data.currentselect = null;
                    },
                    showinfo: function (user) {
                        var data = this;
                        data.currentselect = user;
                    },
                    save: function () {
                        var data = this;
                        data.show = false;
                        callback(data.checkuserlist);
                    },
                    initcurrentorg: function (orgid, searchkey, bysearch, callback) {
                        var data = this;
                        var url = "UserSelectHandler.ashx" + "?request_type=init_tree_all&pid=" + (orgid == undefined ? "-1" : orgid) + "&key=" + searchkey + "&bysearch=" + (bysearch ? 1 : 0);
                        user_select_ctrl._ajax(url, null, function (result) {
                            data.top();
                            callback(result);
                        });
                    },
                    userclick: function (user) {
                        var data = this;
                        var userlist = new Array();
                        var isselect = false;
                        for (var i = 0; i < data.currentlist.userlist.length; i++) {
                            var item = data.currentlist.userlist[i];
                            if (user.LoginName == item.LoginName) {
                                isselect = !item.selected;
                                item.selected = isselect;
                                user.selected = isselect;
                            }
                            userlist.push(item);
                        }
                        data.currentlist.userlist = userlist;
                        var list = new Array();
                        for (var i = 0; i < data.checkuserlist.length; i++) {
                            if (data.checkuserlist[i].LoginName != user.LoginName) {
                                list.push(data.checkuserlist[i]);
                            }
                        }
                        if (isselect) {
                            list.push(user);
                        }
                        data.checkuserlist = list;
                    },
                    orgclick: function (org) {
                        var data = this;
                        data.currentorg = org;
                    },
                    trim: function (str, char) {
                        return str.replace(new RegExp('^\\' + char + '+|\\' + char + '+$', 'g'), '');
                    },
                    top: function () {
                        var target = document.getElementById("scroll-content");
                        if (target != null) {
                            target.scrollTop = "0";
                        }
                    },
                    show: function () {
                        var data = this;
                        data.show = true;
                    },
                    back: function () {
                        var data = this;
                        data.show = false;

                    },
                    search: function () {
                        var data = this;
                        data.bysearch = true;
                        data.initcurrentorg("-1", data.searchkey, data.bysearch, function (result) {
                            var users = data.builduserlist(result.Users);
                            data.currentlist.userlist = users;
                            data.currentlist.orglist = new Array();
                        })
                    }
                },

            });
        }
        user_select_model.$data.show = true;

    }
};