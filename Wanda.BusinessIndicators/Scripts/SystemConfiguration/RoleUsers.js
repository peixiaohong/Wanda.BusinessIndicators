﻿//#region 公共方法

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
    LoadPage();

});

//页面数据加载
function LoadPage() {
    Load();
    var roleData = {
        "RoleID": "B960DEB4-E281-4C44-B611-1B1CF709E410",
        "PageIndex": 1,
        "PageSize":10
    }
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetAllUser",
        args: { data: JSON.stringify(roleData)},
        successReturn: function (resultData) {
            console.log(resultData);
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
// 用户加载
function UsersLoadPage() {
    Load();
    var keyword = $("#UsersNameAdd").val();
    var roleData = {
        "keyWord": keyword,
        "PageIndex": 1,
        "PageSize": 200
    }
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetAllUser",
        args: { data: JSON.stringify(roleData) },
        successReturn: function (resultData) {
            console.log(resultData);
            if (resultData.Success == 1) {

                $('#UsersData').empty();
                loadTmpl('#UsersDataTmpl').tmpl(resultData).appendTo('#UsersData');
                window.isLoadUsers = false;
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
    // 添加用户确定
    $(".user_add_sumbit").off('click').on('click', function () {
        
    })
    // 添加用户取消
    $(".user_add_cancel").off('click').on('click', function () {
        $(".user-model").css("display", "none");
    })
}

//保存数据
function SaveRole(type, data) {
    var title = "添加角色";
    var msg = "添加成功";
    var datamsg = ""
    if (type == "edit") {
        title = "编辑角色";
        msg = "编辑成功";
        datamsg = JSON.stringify(data);
    }
    $.MsgBox.Confirm(title, datamsg, type, function () {
        var CnName = $("#CnName").val();
        var Description = $("#Description").val();
        $(".CnNameAction").css("display", "none");
        if (!CnName) {
            $(".CnNameAction").css("display", "block");
            return;
        }
        Load();
        var S_Role = {
            "CnName": CnName,
            "Description": Description
        };
        if (type == "edit") {
            S_Role = {
                "CnName": CnName,
                "Description": Description,
                "CreateTime": data.CreateTime,
                "CreatorName": data.CreatorName,
                "EnName": data.EnName,
                "ID": data.ID,
                "IsDeleted": data.IsDeleted
            };
        }
        WebUtil.ajax({
            async: false,
            url: "/RoleManagerControll/SaveRole",
            args: { RoleData: JSON.stringify(S_Role) },
            successReturn: function (resultData) {
                if (resultData.Success == 1) {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", msg);
                    LoadPage();
                    console.log("添加成功" + resultData.Message);
                }
                else {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", resultData.Message);
                    console.log("添加失败" + resultData.Message);
                }
                Fake();
            }
        });
    });
}

//查询条件
function QueryRoleData() {
    //参数
    var keyWord = $("#QrName").val();
    var roleData = {
        "keyWord": keyWord,
        "RoleID": "B960DEB4-E281-4C44-B611-1B1CF709E410",
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

//删除角色用户
function DeleteRoleUserData(el) {
    var name = $(el).attr("data-name");
    var roleData = {
        "RoleID": "B960DEB4-E281-4C44-B611-1B1CF709E410",
        "LoginName": name
    }
    $.MsgBox.Confirm("提示", "确认删除", "", function () {
        WebUtil.ajax({
            async: false,
            url: "/RoleManagerControll/GetAllUser",
            args: { data: JSON.stringify(roleData) },
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

//编辑数据[获取数据]
function EditRole(el) {
    var id = $(el).attr("data-id");
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetRoleDataByID",
        args: { ID: id },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                SaveRole("edit", resultData.Data);
            } else {
                $.MsgBox.Alert("提示", "获取数据失败");
                console.log("获取数据失败" + resultData.Message);
            }
        }
    });
}
