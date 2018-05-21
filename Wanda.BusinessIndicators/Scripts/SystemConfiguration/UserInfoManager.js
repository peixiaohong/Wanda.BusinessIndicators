//#region 公共方法

//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/RoleManagerTmpl.html", selector);
}
//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}
//#endregion 公共方法
var PageSize = 10;
var PageNumber = 1;
$(document).ready(function () {

    //注册事件
    RegisterEvent();
    //数据加载
    UsersLoadPage();

});

// 用户加载
function UsersLoadPage() {
    Load();
    var keyword = $("#UsersName").val();
    //console.log(keyword)
    if (keyword) {
        PageNumber = 1;
    }
    var roleData = {
        "keyWord": keyword,
        "PageIndex": PageNumber,
        "PageSize": PageSize
    }
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetAllUser",
        args: { data: JSON.stringify(roleData) },
        successReturn: function (resultData) {
            var totalCount = resultData.TotalCount;
            if (resultData.Success == 1) {
                $('#UsersMenuData').empty();
                loadTmpl('#UsersMenuDataTmpl').tmpl(resultData).appendTo('#UsersMenuData');
                $("#pager").empty();
                $("#pager").paginationex({
                    current: PageNumber,
                    pageSize: PageSize,
                    totalCount: totalCount,
                    navTo: function (pageIndex) {
                        PageNumber = pageIndex;
                        UsersLoadPage();
                    }
                });
            }
            else {
                //console.log(resultData.Message);
            }
            Fake();
        }
    });

}

//注册事件
function RegisterEvent() {
    //新增角色弹框页面确定按钮
    $(".InsertRoleData_OK").off('click').on('click', function () {
        $(".user-model").css("display", "block");
        UsersLoadPage();
    });
    $(".QueryConditions_Button").off("click").on("click", function () {
        SetUsersRole();
    });
    //用户查询 
    $(".QueryUsers_Button").off('click').on('click', function () {
        UsersLoadPage();
    })
    // 用户键盘Enter
    $("#UsersName").keydown(function (e) {
        if (e.keyCode == 13) {
            UsersLoadPage();
        }
    })
    // 设置角色确定
    $(".set_role_sumbit").off('click').on('click', function () {
        SaveRole();
    })
    // 设置角色取消
    $(".set_role_cancel").off('click').on('click', function () {
        $(".user-model").css("display", "none");
    })
    // 设置组织确定
    $(".set_orgs_sumbit").off('click').on('click', function () {
        var name = $(this).attr("name");
        SaveOrgs(name);
    })
    // 设置组织取消
    $(".set_orgs_cancel").off('click').on('click', function () {
        $(".organization-model").css("display", "none");
    })
}

//保存数据
function SaveRole() {
    var el = $(".userChecked");
    var loginName = $("#UsersRoleName").attr("data-login");
    var roleIDs = ""
    for (var i = 0; i < el.length; i++) {
        var roleId = el.eq(i).attr("data-role");
        if (el.eq(i).attr("checked") == "checked") {
            roleIDs += roleId + ",";
        }
    }
    var S_Role = {
        "roleIDs": roleIDs.slice(0, roleIDs.length - 1),
        "loginName": loginName
    }
    if (!roleIDs) {
        $.MsgBox.Confirm("提示", "确认不设置任何角色？", "", function () {
            $("#mb_box,#mb_con").remove();
            S_Role.roleIDs = "";
            //console.log(S_Role);
            SaveRoleFun(S_Role);
        })
    } else {
        SaveRoleFun(S_Role);
    }
    //console.log(S_Role);
}
function SaveRoleFun(data) {
    WebUtil.ajax({
        async: false,
        url: "/UserInfoManagerControll/SaveUsesRoles",
        args: data,
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $(".user-model").css("display", "none");
                $.MsgBox.Alert("提示", "添加成功");
                UsersLoadPage();
                //console.log("添加成功" + resultData.Message);
            }
            else {
                $(".user-model").css("display", "none");
                $.MsgBox.Alert("提示", "添加失败");
                //console.log("添加失败:" + resultData.Message);
            }
            Fake();
        }
    });
}

// 设置/查询角色
function SetUsersRole(el) {
    $(".user-model").css("display", "block");
    Load();
    if (el) {
        var LoginName = $(el).attr("data-login");
        $("#UsersRoleName").attr("data-login", LoginName);
    } else {
        var LoginName = $("#UsersRoleName").attr("data-login");
    }
    var RoleName = $("#UsersRoleName").val();
    if (!RoleName) {
        RoleName = ""
    }
    WebUtil.ajax({
        async: false,
        url: "/UserInfoManagerControll/GetUserRoles",
        args: { "cnName": RoleName, "loginName": LoginName},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                //console.log(resultData)
                $('#UsersRolesData').empty();
                loadTmpl('#UsersRolesDataTmpl').tmpl(resultData).appendTo('#UsersRolesData');
            }
            else {
                //console.log(resultData.Message);
            }
            Fake();
        }
    });

}
// 设置架构
function SetOrgs(el) {
    $(".organization-model").css("display", "block");
    var loginName = $(el).attr("data-login");
    //console.log(loginName);
    Load();
    WebUtil.ajax({
        async: false,
        url: "/UserInfoManagerControll/GetUserOrgs",
        args: { "loginName": loginName},
        successReturn: function (resultData) {
            //console.log(resultData);
            if (resultData.Success == 1) {
                $(".set_orgs_sumbit").attr("name", loginName)
                Ztree(resultData);
            } else {
                //console.log(resultData.Message);
                $.MsgBox.Alert("提示", "获取数据失败");
            }
        }
    });
}
function Ztree(data) {
    //console.log(data);
    var zNodes = [];
    data.Data.forEach(function (one) {
        var zNodesObj = { id: "", pId: "", name: "", checked: "", systemID: "" };
        if (one.ID == "00000000-0000-0000-0000-000000000000") {
            zNodesObj.open = true;
        }
        if (one.Level >= 4) {
            zNodesObj.doCheck = false;
        }
        zNodesObj.id = one.ID;
        zNodesObj.pId = one.ParentID;
        zNodesObj.name = one.CnName;
        zNodesObj.checked = one.IsChecked;
        zNodesObj.systemID = one.SystemID;
        zNodes.push(zNodesObj);
    });
    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true
            }
        },
        callback: {
            beforeCheck: beforeCheck,
        }
    };
    var className = "dark"
    function beforeCheck(treeId, treeNode) {
        className = (className === "dark" ? "" : "dark");
        return (treeNode.doCheck !== false);
    }
    $.fn.zTree.init($("#tree"), setting, zNodes);
    var zTree = $.fn.zTree.getZTreeObj("tree");
    zTree.setting.check.chkboxType = { "Y": "ps", "N": "ps" };
    Fake();
}
function SaveOrgs(name) {
    var treeObj = $.fn.zTree.getZTreeObj("tree");
    var nodes = treeObj.getCheckedNodes(true);
    if (!nodes.length) {
        $.MsgBox.Confirm("提示", "确认取消所有权限？", "", function () {
            $("#mb_box,#mb_con").remove();
            SaveOrgsFun(nodes, name);
        })
    } else {
        SaveOrgsFun(nodes,name)
    }
    
}
function SaveOrgsFun(nodes,name) {
    WebUtil.ajax({
        async: false,
        url: "/UserInfoManagerControll/SaveUser_org",
        args: {
            loginName: name,
            data: FilterChecked(nodes, name)
        },
        successReturn: function (resultData) {
            $(".organization-model").css("display", "none");
            if (resultData.Success == 1) {
                $.MsgBox.Alert("提示", "保存成功");
            } else {
                //console.log(resultData.Message)
                $.MsgBox.Alert("提示", "保存失败");
            }
        }
    });
}
function FilterChecked(data,name) {
    var match = [];
    data.forEach(function (one) {
        if (one.check_Child_State == -1 && one.checked) {
            var obj = {
                "SystemID": one.systemID,
                "LoginName": name,
                "CompanyID": one.id,
                //"IsChecked": one.checked
            }
            match.push(obj);
        }
    })
    return JSON.stringify(match);
}