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
function LoadPage() {
    Load();
    WebUtil.ajax({
        async: false,
        url: "/S_OrganizationalManagerControll/GetAllOrgData",
        args: {},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                //console.log(resultData);
                Ztree(resultData);
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
    // 新增确定
    $(".add_submit").off("click").on("click", function () {
        SaveOrganization("Add");
    });
    // 修改确定
    $(".edit_submit").off("click").on("click", function () {
        SaveOrganization("Edit");
    });

}
//是否选择到板块
function CheckOrganization(type) {
    var el = $(".organization_name").find("input");
    var val = el.val();
    var isCompany = el.attr("data-isCompany");
    if (isCompany == "true") {
        isCompany = true
    } else if (isCompany == "false") {
        isCompany = false;
    }
    //console.log(isCompany);
    var level = el.attr("data-level");
    var id = el.attr("data-id");
    if (!val) {
        $.MsgBox.Alert("提示", "请选择板块");
        return false;
    }
    if (type == "Add") {
        if (level == 1) {
            $.MsgBox.Alert("提示", "系统板块不能新增");
            return false;
        }
        if (isCompany) {
            $.MsgBox.Alert("提示", "项目类型不能新增");
            return false;
        }
        $(".organization_name").css("display", "block");
        $(".organization_edit").css("display", "block");
        $(".organization_edit_name").val("");
        $(".organization_edit").find("input[type=checkbox]").removeAttr("disabled");
        $(".organization_edit").find("input[type=checkbox]").css("cursor", "default");
        $(".add_submit").css("display", "block");
        $(".edit_submit").css("display", "none");

    } else if (type == "Edit") {
        if (level == 1 || level == 2) {
            $.MsgBox.Alert("提示", "系统板块不能修改");
            return false;
        }
        $(".organization_name").css("display", "none");
        $(".organization_edit").css("display", "block");
        $(".organization_edit_name").val(el.val());
        $(".organization_edit").find("input[type=checkbox]").attr({
            "disabled": "disabled",
            "checked": isCompany
        });
        $(".organization_edit").find("input[type=checkbox]").css("cursor", "not-allowed");
        $(".add_submit").css("display", "none");
        $(".edit_submit").css("display", "block");
    } else if (type == "Delete") {
        if (level == 1 || level == 2) {
            $.MsgBox.Alert("提示", "系统板块不能删除");
            return false;
        }
        DeleteOrganizationData(id, isCompany);
    }
}
//确定后初始
function initPage() {
    $(".organization_name").find("input").val("");
    $(".organization_name").find("input").attr({
        "data-id": "",
        "data-pid": "",
        "data-name": "",
        "data-systemID": "",
        "data-level": "",
        "data-isCompany": ""
    });
    $(".organization_name").css("display", "block");
    $(".organization_edit").css("display", "none");
}
//保存数据
function SaveOrganization(type) {
    var el = $(".organization_name").find("input");
    var systemID = el.attr("data-systemID");
    var pid = el.attr("data-id");
    var id = "00000000-0000-0000-0000-000000000000";
    var level = Number(el.attr("data-level")) + 1;
    var isCompany = $(".organization_edit").find("input[type=checkbox]").attr("checked");
    var val = $(".organization_edit_name").val();
    if (type == "Edit") {
        level = Number(el.attr("data-level"));
        id = el.attr("data-id");
        pid = el.attr("data-pid");
        isCompany = el.attr("data-isCompany");
    }
    //console.log(isCompany);
    if (isCompany == "true") {
        isCompany = true;
    } else {
        isCompany = false;
    }
    var data = {
        "ID": id,
        "SystemID": systemID,
        "CnName": val,
        "Code": "",
        "ParentID": pid,
        "Level": level,
        "IsCompany": isCompany
    }
    var companyData = {
        "ID": id,
        "SystemID": systemID,
        "CompanyName": val
    }

    if (!val) {
        $.MsgBox.Alert("提示", "组织名称不能为空");
        return false;
    }
    //console.log(data);
    WebUtil.ajax({
        async: false,
        url: "/S_OrganizationalManagerControll/SaveData",
        args: {
            "type": type,
            "data": JSON.stringify(data),
            "IsCompany": isCompany,
            "companyData": JSON.stringify(companyData),
        },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                initPage();
                $.MsgBox.Alert("提示", resultData.Message);
                LoadPage();
            }
            else {
                $.MsgBox.Alert("提示", resultData.Message);
            }
        }
    });

}

//删除组织
function DeleteOrganizationData(id, isCompany) {

    $.MsgBox.Confirm("提示", "确认删除", "", function () {
        WebUtil.ajax({
            async: false,
            url: "/S_OrganizationalManagerControll/DeleteData",
            args: {
                "id": id,
                "isCompany": isCompany
            },
            successReturn: function (resultData) {
                if (resultData.Success == 1) {
                    $("#mb_box,#mb_con").remove();
                    $.MsgBox.Alert("提示", "删除成功");
                    initPage();
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

function Ztree(data) {
    var zNodes = [];
    data.Data.forEach(function (one) {
        var zNodesObj = { id: "", pId: "", name: "", level: "", systemID: "", isCompany: false };
        zNodesObj.id = one.ID;
        zNodesObj.pId = one.ParentID;
        zNodesObj.name = one.CnName;
        zNodesObj.level = one.Level;
        zNodesObj.systemID = one.SystemID;
        zNodesObj.isCompany = one.IsCompany;
        zNodes.push(zNodesObj);
        zNodes[0].open = true;
    });
    var setting = {
        view: {
            dblClickExpand: false,
            showLine: true,
            selectedMulti: false
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        },
        callback: {
            onClick: zTreeOnClick
        }
    };
    $.fn.zTree.init($("#tree"), setting, zNodes);
    var zTree = $.fn.zTree.getZTreeObj("tree");
    zTree.setting.check.chkboxType = { "Y": "ps", "N": "ps" };
    Fake();
    function zTreeOnClick(event, treeId, treeNode) {
        //console.log(treeNode)
        var el = $(".organization_name").find("input");
        el.attr({
            "data-id": treeNode.id,
            "data-pid": treeNode.pId,
            "data-name": treeNode.name,
            "data-systemID": treeNode.systemID,
            "data-level": treeNode.level + 1,
            "data-isCompany": treeNode.isCompany
        });
        el.val(treeNode.name);
        $(".organization_name").css("display", "block");
        $(".organization_edit").css("display", "none");
    };
}