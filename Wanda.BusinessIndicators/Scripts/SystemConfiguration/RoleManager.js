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
        InsertRole();
    });
}

//新增角色数据
function InsertRole()
{
    var CnName= $("#CnName").val();
    var Description = $("#Description").val();
    Load();
    WebUtil.ajax({
        async: false,
        url: "/RoleManagerControll/GetRoles",
        args: { CnName: CnName, Description: Description},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                console.log("添加成功" + resultData.Message);
            }
            else
            {
                console.log("添加失败" + resultData.Message);
            }
            Fake();
        }
    });
}