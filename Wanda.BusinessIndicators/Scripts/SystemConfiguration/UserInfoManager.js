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
    console.log(keyword)
    var roleData = {
        "keyWord": keyword,
        "PageIndex": 1,
        "PageSize": 10
    }
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetAllUser",
        args: { data: JSON.stringify(roleData) },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                console.log(resultData)
                $('#UsersMenuData').empty();
                loadTmpl('#UsersMenuDataTmpl').tmpl(resultData).appendTo('#UsersMenuData');
            }
            else {
                console.log(resultData.Message);
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
    // 设置角色确定
    $(".set_role_sumbit").off('click').on('click', function () {
        SaveRole();
    })
    // 设置角色取消
    $(".set_role_cancel").off('click').on('click', function () {
        $(".user-model").css("display", "none");
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
    console.log(roleIDs.slice(0, roleIDs.length - 1));
    var S_Role = {
        "roleIDs": roleIDs.slice(0, roleIDs.length - 1),
        "loginName": loginName
    }
    console.log(S_Role);
    WebUtil.ajax({
        async: false,
        url: "/UserInfoManagerControll/SaveUsesRoles",
        args: S_Role,
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $(".user-model").css("display", "none");
                $.MsgBox.Alert("提示", "添加成功");
                LoadPage();
                console.log("添加成功" + resultData.Message);
            }
            else {
                $(".user-model").css("display", "none");
                $.MsgBox.Alert("提示", "添加失败");
                console.log("添加失败:" + resultData.Message);
            }
            Fake();
        }
    });
}

//查询条件
function QueryRoleData() {
    //参数
    var keyWord = $("#QrName").val();
    var roleData = {
        "keyWord": keyWord,
        "RoleID": getQueryString("RoleId"),
        "PageIndex": 1,
        "PageSize": 10
    }
    Load();
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetAllUser",
        args: { data: JSON.stringify(roleData) },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $('#ShowMenuUsersData').empty();
                loadTmpl('#ShowMenuUsersDataTmpl').tmpl(resultData).appendTo('#ShowMenuUsersData');
            }
            else {
                console.log(resultData.Message);
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
    WebUtil.ajax({
        async: false,
        url: "/UserInfoManagerControll/GetUserRoles",
        args: { "cnName": RoleName, "loginName": LoginName},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                console.log(resultData)
                $('#UsersRolesData').empty();
                loadTmpl('#UsersRolesDataTmpl').tmpl(resultData).appendTo('#UsersRolesData');
            }
            else {
                console.log(resultData.Message);
            }
            Fake();
        }
    });

}
//角色用户
function DeleteRoleUserData(el) {
    var name = $(el).attr("data-name");
    var roleData = {
        "RoleID": getQueryString("RoleId"),
        "LoginName": name
    }
    $.MsgBox.Confirm("提示", "确认删除", "", function () {
        WebUtil.ajax({
            async: false,
            url: "/RoleManagerControll/DeleteRole_User",
            args: roleData,
            successReturn: function (resultData) {
                if (resultData.Success == 1) {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", "删除成功");
                    LoadPage();
                }
                else {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", "删除失败");
                    console.log("删除失败" + resultData.Message);
                }
            }
        });
    });
}
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
} 
