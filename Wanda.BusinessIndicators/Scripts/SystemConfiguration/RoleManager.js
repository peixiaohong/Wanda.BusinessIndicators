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
    LoadPage();
});

//页面数据加载
function LoadPage()
{
    Load();
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetRoles",
        args: {},
        successReturn: function (resultData) {
            console.log(resultData);
            if (resultData.Success == 1) {
                $('#ShowMenuData').empty();
                loadTmpl('#ShowMenuDataTmpl').tmpl(resultData).appendTo('#ShowMenuData');
            }
            else
            {
                console.log(resultData.Message);
            }
            Fake();
        }
    });

}

//注册事件
function RegisterEvent()
{
    //新增角色弹框页面确定按钮
    $(".InsertRoleData_OK").off('click').on('click', function ()
    {
        SaveRole();
    });
    $(".QueryConditions_Button").off("click").on("click", function ()
    {
        QueryRoleData();
    });
}
//保存数据
function SaveRole()
{
    $.MsgBox.Confirm("添加角色", "", "add", function () {
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
        WebUtil.ajax({
            async: false,
            url: "/RoleManagerControll/SaveRole",
            args: { RoleData: JSON.stringify(S_Role) },
            successReturn: function (resultData) {
                if (resultData.Success == 1) {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", "添加成功");
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
function QueryRoleData()
{
    //参数
    var keyWord = $("#QrName").val();
    Load();
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetRoles",
        args: { CnName: keyWord},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $('#ShowMenuData').empty();
                loadTmpl('#ShowMenuDataTmpl').tmpl(resultData).appendTo('#ShowMenuData');
            }
            else {
                console.log(resultData.Message);
            }
            Fake();
        }
    });
}

//删除角色
function DeleteRoleData(el)
{
    var id = $(el).attr("data-id");
    console.log(id);
    $.MsgBox.Confirm("提示", "确认删除", "", function () {
        WebUtil.ajax({
            async: false,
            url: "/RoleManagerControll/DeleteRoleData",
            args: { ID: id },
            successReturn: function (resultData) {
                if (resultData.Success == 1) {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", "删除成功");
                    LoadPage();
                    console.log("删除成功" + resultData.Message);
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
function EditRole(el)
{
    var id = $(el).attr("data-id");
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetRoleDataByID",
        args: { ID: "7AAEC46E-E885-4C40-97DF-5DAF5816683E" },
        successReturn: function (resultData) {
            console.log(resultData);
        }
    });
}
