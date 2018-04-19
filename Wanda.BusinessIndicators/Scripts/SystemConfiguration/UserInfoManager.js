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
        QueryRoleData();
    });
    //用户查询 
    $(".QueryUsers_Button").off('click').on('click', function () {
        UsersLoadPage();
    })
    // 设置用户确定
    $(".user_add_sumbit").off('click').on('click', function () {
        SaveRole();
    })
    // 设置用户取消
    $(".user_add_cancel").off('click').on('click', function () {
        $(".user-model").css("display", "none");
    })
}

//保存数据
function SaveRole() {
    var el = $(".userChecked");
    var LoginNames = "";
    for (var i = 0; i < el.length; i++) {
        var name = el.eq(i).attr("name");
        if (el.eq(i).attr("checked") == "checked") {
            LoginNames += name + ",";
        }
    }
    if (!LoginNames.length) {
        $.MsgBox.Alert("提示", "请选择用户");
        return false;
    }
    console.log(LoginNames.slice(0, LoginNames.length - 1));
    var S_Role = {
        "PageLoginNames": LoginNames.slice(0, LoginNames.length - 1),
        "RoleID": getQueryString("RoleId")
    }
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/SaveUser_Role",
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

// 设置角色
function SetUsersRole(el) {
    $(".user-model").css("display", "block");
    Load();
    var RoleName = $(el).attr("data-role");
    var LoginName = $(el).attr("data-login");
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetUserRoles",
        args: { "cnName": RoleName, "loginName": LoginName},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                console.log(resultData)
                //$('#UsersMenuData').empty();
                //loadTmpl('#UsersMenuDataTmpl').tmpl(resultData).appendTo('#UsersMenuData');
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
