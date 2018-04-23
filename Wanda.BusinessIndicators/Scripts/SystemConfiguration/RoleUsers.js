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
    LoadPage();

});

//页面数据加载
function LoadPage() {
    Load();
    var roleData = {
        "RoleID": getQueryString("RoleId"),
        "PageIndex": PageNumber,
        "PageSize": PageSize
    }
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetAllUser",
        args: { data: JSON.stringify(roleData)},
        successReturn: function (resultData) {
            var totalCount = resultData.TotalCount;
            if (resultData.Success == 1) {           
                $('#ShowMenuUsersData').empty();
                loadTmpl('#ShowMenuUsersDataTmpl').tmpl(resultData).appendTo('#ShowMenuUsersData');
                $("#InitPager").paginationex({
                    current: PageNumber,
                    pageSize: PageSize,
                    totalCount: totalCount,
                    navTo: function (pageIndex) {
                        PageNumber = pageIndex;
                        LoadPage();
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
// 用户加载
function UsersLoadPage() {
    Load();
    var keyword = $("#UsersNameAdd").val();
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
                $('#UsersData').empty();
                loadTmpl('#UsersDataTmpl').tmpl(resultData).appendTo('#UsersData');
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
        $("#QrName").val("");
        PageSize = 10;
        PageNumber = 1;
        UsersLoadPage();
    });
    $(".QueryConditions_Button").off("click").on("click", function () {
        QueryRoleData();
    });
    //用户查询 
    $(".QueryUsers_Button").off('click').on('click', function () {
        UsersLoadPage();
    })
    // 添加用户确定
    $(".user_add_sumbit").off('click').on('click', function () {
        SaveRole();
    })
    // 添加用户取消
    $(".user_add_cancel").off('click').on('click', function () {
        $(".user-model").css("display", "none");
        $("#UsersNameAdd").val("");
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
    var S_Role = {
        "PageLoginNames": LoginNames.slice(0, LoginNames.length-1),
        "RoleID": getQueryString("RoleId")
    }

    if (!LoginNames.length) {
        $.MsgBox.Confirm("提示", "没有选中", "", function () {
            $("#mb_box,#mb_con").remove();
            S_Role.PageLoginNames = "";
            SaveRoleFun(S_Role);
        })
    } else {
        SaveRoleFun(S_Role);
    }
    
}
function SaveRoleFun(data) {
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/SaveUser_Role",
        args: data,
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $(".user-model").css("display", "none");
                $("#UsersNameAdd").val("");
                $.MsgBox.Alert("提示", "添加成功");
                LoadPage();
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
                //console.log(resultData.Message);
            }
            Fake();
        }
    });
}

//删除角色用户
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
                    //console.log("删除失败" + resultData.Message);
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
