
/*

依赖 jquery1.4.3
     zTree 3.0
*/


; (function ($) {

    var userWindowHtml = [

     "   <div Style='width:670px;margin:5px auto' id='selectPanel'>",
     "   <div style='width: 670px; text-decoration: none; font-weight: normal; padding-bottom: 0px'>",
     "       <div id='TreeView' style='width: 670px'>",
     "           <ul class='main_bd_tab'>",
     "               <li alt='#paneplan' class='main_bd_tab_active' >按组织架构</li>",
     "               <li alt='#panebackup' >组合查询</li>",
     "           </ul>",
     "           <div class='main_bd_tab_content mb8' style='margin-bottom: 2px; clear: both; padding: 5px;max-width:650px'>",
     "               <div id='paneplan' class='panetabls  ztree' style='height: 100px;display:none'>",
     "               </div>",
     "               <div id='panebackup' class='panetabls' style='display: none;'>",
     "                   <div style='height: 20px'>",
     "                       <!--垫高-->",
     "                   </div>",
     "                   <div style='padding: 0 5px;'>",
     "                       <table align='center' style='width: 100%'>",
     "                           <tr>",
     "                               <td style=''>姓名/用户名：",
     "                               <input type='text' class='w120' id='txtSearchUser' style='' />",
     "                               </td>",
     "                               <td>部门：",
     "                               <input type='text' class='w120' id='txtSearchdeptName' style='' />",
     "                               </td>",
     "                               <td>岗位：",
     "                               <input type='text' class='w120' id='txtSearchjobName' style='' />",
     "                               </td>",
     "                               <td nowrap>",
     "                                   <a href='javascript:void(0)' class='btn_search'  ><span>查询</span></a>",
     "                               </td>",
     "                           </tr>",
     "                       </table>",
     "                   </div>",
     "                   <div style='height: 10px'>",
     "                       <!--垫高-->",
     "                   </div>",
     "               </div>",
     "           </div>",
     "           <!--浮层修改-->",
     "           <table style='width:663px;margin:0px;border:none'>",
     "               <tr>",
     "                   <td valign='top' style='width:306px;font-weight:bolder'>查询结果:",
     "                   </td>",
     "                   <td valign='top' style='width:35px;'>",
     "                   </td>",
     "                   <td valign='top' style='width:306px;font-weight:bolder'>待转发用户:",
     "                   </td>",
     "               </tr>",
     "               <tr>",
     "                   <td valign='top' style='width:306px; height: 150px'>",
     "                       <!--修改高度 260-->",
     "                       <select id='tbSearchUserList'   multiple='multiple' style='height: 100%; width: 100%'>",
     "                       </select>",
     "                   </td>",
     "                   <td align='center' valign='top' style='width:35px'>",
     "                       <input id='btnup' class='list_btn' type='button'   title='向上移动' style='background-image: url(../Images/listbutton_up.png); height: 24px;margin:3px 0px' />",
     "                       <input id='btnSingRight' class='list_btn' type='button'   title='移至右侧' value='' style='background-image: url(../Images/listbutton_oneright.png); height: 18px;margin:3px 0px' />",
     "                       <input id='btnAllRight' class='list_btn' type='button'   title='全部移至右侧' value='' style='background-image: url(../Images/listbutton_right.png); height: 24px;margin:3px 0px' />",
     "                       <input id='btnSingLeft' type='button' class='list_btn'  title='移至左侧' value='' style='background-image: url(../Images/listbutton_oneleft.png); height: 18px;margin:3px 0px' />",
     "                       <input id='btnAllLeft' type='button' class='list_btn'  title='全部移至左侧' value='' style='background-image: url(../Images/listbutton_left.png); height: 24px;margin:3px 0px' />",
     "                       <input id='btndown' class='list_btn' type='button'   title='向下移动' style='background-image: url(../Images/listbutton_down.png); height: 26px; ;margin:3px 0px;margin-bottom: 0' />",
     "                   </td>",
     "                   <td valign='top' style='width:306px; height: 150px'>",
     "                       <select size='20' id='tbSelectedUserList'   style='height: 100%; width:100%'>",
     "                       </select>",
     "                   </td>",
     "               </tr>",
     "           </table>",
     "           <table style='width:665px;margin:0px;border:none' id='note'>",
     "               <tr>",
     "                   <td style='width:55px;font-weight:bolder;padding-top:5px' valign='middle'>审批意见:",
     "                   </td>",
     "                   <td valign='top' style='width:600px;'>",
     "                       <!--修改高度 260-->",
     "                   </td>",
     "                   <td style='color:red;width:5px;'>",
     "                   </td>",
     "               </tr>",
     "               <tr>",
     "                   <td valign='top' colspan='2' style='width:655px; height: 55px;padding-top:5px'>",
     "                       <!--修改高度 260-->",
     "                       <textarea id='approvalnotes' style='width: 99%;height:50px' ></textarea>",
     "                   </td>",
     "                   <td style='color:red;width:5px;'>*",
     "                   </td>",
     "               </tr>",
     "           </table>",
     "       </div>",
     "   </div>",
   " </div>"

    ].join("");

    var TreeNodes = [];
    var setting = {
        data: {
            key: {
                name: "OrgName"
            },
            simpleData: {
                enable: true,
                idKey: "OrgID",
                pIdKey: "ParentID"
            }
        },
        view: {
            expandSpeed: "",
            showIcon: false
        },
        callback: {
            onClick: nodeClick
        }
    };

    function _filterShow(obj) {
        $(".main_bd_tab li").removeClass("main_bd_tab_active").filter(obj).addClass("main_bd_tab_active");
        $(".panetabls").hide().filter($(obj).attr("alt")).show();
    }


    function _GetSearchUserList() {

        var name = $.trim($("#txtSearchUser").val());
        if (name.length == 0) {
            alert("请输入姓名！");
            return;
        }
        //获取用户输入的部门名称
        var deptName = $.trim($("#txtSearchdeptName").val());
        //获取用户输入的岗位名称
        var jobName = $.trim($("#txtSearchjobName").val());

        WebUtil.ajax({
            url: "/UserInfo/SearchUser",
            args: { keyword: name, deptName: deptName, jobName: jobName },
            successReturn: function (result) {
                AjaxSearchUserCallBack(result);
            }
        });

    }

    function nodeClick(event, treeId, treeNode, clickFlag) {
        var zTree = $.fn.zTree.getZTreeObj("paneplan");
        if (treeNode.OrgID != null && treeNode.OrgID != 0) {
            WebUtil.ajax({
                async: true,
                asyncBlock: false,
                url: "/UserInfo/AjaxSearchOrgUser",
                args: { orgid: treeNode.OrgID },
                successReturn: function (result) {
                    AjaxSearchUserCallBacktree(result);
                }
            });
        }
    }

    function trim(str) {
        if ($.type(str) == "string") {
            return $.trim(str.replaceAll("&nbsp;", ""));
        }
        return str;
    }

    //用于组合查询
    function AjaxSearchUserCallBack(r) {
        $("#tbSearchUserList").empty();
        var userarray = r;
        if (userarray.length == 0) {
            alert("没有找到用户，请重新查找。");
        }
        for (i = 0; i < userarray.length; i++) {
            var item = userarray[i];
            var str = "<option value='{0}' title='{1}' data-value='{4}'>{1}|{2}|{3}</option>".formatBy(item.UserId, trim(item.UserName), trim(item.UserDepts), trim(item.Job), WebUtil.jsonToString(item));

            $("#tbSearchUserList").append(str);
        }

        $("#tbSearchUserList option:first").attr("selected", true);

    }


    //用户选择树形结构时的查询
    function AjaxSearchUserCallBacktree(r) {
        $("#tbSearchUserList").empty();
        var userarray = r;
        if (userarray.length == 0) {
            //            alert("没有找到用户，请重新查找。");
            return;

        }
        for (i = 0; i < userarray.length; i++) {

            var item = userarray[i];
            var str = "<option value='{0}' title='{1}' data-value='{4}'>{1}|{2}|{3}</option>".formatBy(item.UserId, trim(item.UserName), trim(item.UserDepts), trim(item.Job), WebUtil.jsonToString(item));

            $("#tbSearchUserList").append(str);
        }
        $("#tbSearchUserList option:first").attr("selected", true);

    }
    //<!--树形用户选择相关-->
    function _btnSingRightClick(allowMulti) {
        if (!allowMulti && $("#tbSelectedUserList option").length > 0) return;
        if (!allowMulti && $("#tbSearchUserList option:selected").length > 1) { alert("只能选择一个用户"); return; }
        if ($("#tbSearchUserList option").length == 0) { alert("请先选择用户"); return; }
        $("#tbSearchUserList option:selected").each(function (data) {
            var current = $(this);
            var this_val = $(this).val();

            $("#tbSelectedUserList option").each(function () {
                var thisa = $(this);
                $(thisa).removeAttr("selected");
                var c_t = $(this).val();
                if (c_t == this_val) {
                    thisa.remove();
                }
            });
            $("#tbSelectedUserList").append(current);
        });
    };

    //添加右侧添加到左侧
    function _btnSingLeftClick() {
        if ($("#tbSelectedUserList option:selected").length == 0) { alert("请先选择用户"); return; }
        $("#tbSelectedUserList option:selected").each(function () {
            $("#tbSearchUserList").append($(this));
        });
    };

    //添加左侧双击
    function _tbSearchUserListDbClick() {
        $("#btnSingRight").trigger("click");
    };

    //添加右侧双击
    function _tbSelectedUserListDbClick() {
        $("#btnSingLeft").trigger("click");
    };

    //添加全部到右侧
    function _btnAllRightClick(allowMulti) {
        if (!allowMulti) return;
        $("#tbSearchUserList option").each(function (data) {
            var current = $(this);
            var this_val = $(this).val();
            $("#tbSelectedUserList option").each(function () {

                var thisa = $(this);
                $(thisa).removeAttr("selected");
                var c_t = $(this).val();

                if (c_t == this_val) {
                    thisa.remove();
                }
            });
            $("#tbSelectedUserList").append(current);
        });
    };
    //全部添加到左侧
    function _btnAllLeftClick() {
        if ($("#tbSelectedUserList option").length == 0) {
            alert("请先选择用户");
            return;
        }

        $("#tbSelectedUserList option").each(function (data) {
            var current = $(this);
            var this_val = $(this).val();
            $("#tbSearchUserList").append(current);
        });
    };

    function _btnupClick() {
        $("#tbSelectedUserList option:selected").each(function () {
            $(this).insertBefore($(this).prev());
        });
    };
    //向下
    function _btndownClick() {
        $("#tbSelectedUserList option:selected").each(function () {
            $(this).insertAfter($(this).next());
        });
    };

    $.fn.extend({
        userWindow:
            function (options) {
                var requesetUrl = "/userinfocontroller/GetUserInfo";

                var _setting = {
                    allowMulti: true,
                    onSubmit: function (context, users) { return true; },
                    onCancel: function (context) { return true; },
                    other: "",
                    showNote: false,
                    anchorTop: false,
                    showOne: true,
                };
                $.extend(_setting, options);

                var $ctrl = $(this);

                $ctrl.bind("click", function () {
                    if ($ctrl.attr("valid") == "false" && _setting.showOne) return;
                    if ($ctrl.is(":disabled")) { return; }

                    $(userWindowHtml).OpenDiv({
                        title: "选择用户",
                        anchorTop: _setting.anchorTop,
                        onInit: function (context) {
                            $ctrl.attr("valid", "false");
                            context.find(".main_bd_tab li").click(function () { _filterShow($(this)); });
                            context.find("a.btn_search").click(function () { _GetSearchUserList($(this)); });
                            context.find("#tbSearchUserList").dblclick(function () { _tbSearchUserListDbClick(); });
                            context.find("#tbSelectedUserList").dblclick(function () { _tbSelectedUserListDbClick(); });
                            context.find("#btnup").click(function () { _btnupClick(); });
                            context.find("#btnSingRight").click(function () { _btnSingRightClick(_setting.allowMulti); });
                            context.find("#btnAllRight").click(function () { _btnAllRightClick(_setting.allowMulti); });
                            context.find("#btnSingLeft").click(function () { _btnSingLeftClick(); });
                            context.find("#btnAllLeft").click(function () { _btnAllLeftClick(); });
                            context.find("#btndown").click(function () { _btndownClick(); });
                            if (!_setting.allowMulti) {
                                context.find("#btnup").hide();
                                context.find("#btnAllRight").hide();
                                context.find("#btnAllLeft").hide();
                                context.find("#btndown").hide();
                            } else {
                                context.find("#btnup").show();
                                context.find("#btnAllRight").show();
                                context.find("#btnAllLeft").show();
                                context.find("#btndown").show();
                            }
                            if (_setting.showNote) {
                                context.find("#note").show();
                            } else {
                                context.find("#note").hide();
                            }
                            if (TreeNodes.length == 0) {
                                var model = undefined;
                                WebUtil.ajax({
                                    url: "/WdOrgController/GetWdOrgList",
                                    args: {},
                                    successReturn: function (res) {
                                        TreeNodes = res;
                                        $.each(TreeNodes, function (i, item) {
                                            if (item.ParentID == 0) {
                                                item.open = true;
                                            }
                                        });
                                        $.fn.zTree.init($("#paneplan"), setting, TreeNodes);
                                        var zTree1 = $.fn.zTree.getZTreeObj("paneplan");
                                        var nodes1 = zTree1.getNodes();
                                        zTree1.selectNode(nodes1[0]);
                                        zTree1.expandNode(nodes1[0], true, false, true);
                                    }
                                });
                            } else {
                                $.fn.zTree.init($("#paneplan"), setting, TreeNodes);
                                var zTree = $.fn.zTree.getZTreeObj("paneplan");
                                var nodes = zTree.getNodes();
                                zTree.selectNode(nodes[0]);
                                zTree.expandNode(nodes[0], true, false, true);
                            }
                            setTimeout(function () { $("#paneplan").show(); }, 10, null);
                        },
                        scrollY: true,
                        onSubmit: function (context) {
                            // 将选中值该成 {text:"", value:""}对象
                            var users = $.map(
                                  $("#tbSelectedUserList").find("option"),
                                  function (opt) {
                                      var opt = $(opt);
                                      var boj = new Object();
                                      boj.text = opt.text();
                                      boj.value = opt.attr("value");
                                      boj.data = $.parseJSON(opt.attr("data-value"));
                                      return boj;
                                  }
                                );
                            return _setting.onSubmit(context, users, _setting.other);
                        },
                        onCancel: _setting.onCancel
                    });
                });


            }
    });

})(jQuery)