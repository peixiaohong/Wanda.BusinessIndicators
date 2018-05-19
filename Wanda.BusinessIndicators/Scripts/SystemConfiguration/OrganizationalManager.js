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
    // 项目查询
    $(".company_search").off("click").on("click", function () {
        var val = $("#companyName").val();
        initCompany(val)
    })
    // 项目保存
    $(".company_sumbit").off("click").on("click", function () {
        SaveCompany();
    })

}
//是否选择到板块
function CheckOrganization(type, c) {
    $(".organization-box").css("display", "none");
    var el = $(".organization_name").find("input");
    var isCompany = el.attr("data-isCompany");
    if (isCompany == "true") {
        isCompany = true
    } else if (isCompany == "false") {
        isCompany = false;
    }
    var level = el.attr("data-level");
    var id = el.attr("data-id");
    var name = el.attr("data-name");
    var systemID = el.attr("data-systemID");
    if (type == "Add") {
        if (level == 1) {
            $.MsgBox.Alert("提示", "系统板块不能新增");
            return false;
        }
        if (isCompany) {
            $.MsgBox.Alert("提示", "项目类型不能新增");
            return false;
        }
        CheckCompany(c);
        $(".organization-title").html(name);
        $(".old_name").css("display", "none");
        $(".organization_edit").css("display", "block");
        $(".add_submit").css("display", "block");
        $(".edit_submit").css("display", "none");

    } else if (type == "Edit") {
        if (level == 1 || level == 2) {
            $.MsgBox.Alert("提示", "系统板块不能修改");
            return false;
        }
        CheckCompany(c);
        GetSystemInfo(systemID);
        $(".old_name").css("display", "block");
        $(".organization_edit").css("display", "block");
        $(".old_name").find("input").val(name);
        $(".add_submit").css("display", "none");
        $(".edit_submit").css("display", "block");
    } else if (type == "Delete") {
        $(".organization_content").css("display", "none");
        $(".select_content").css("display", "none");
        if (level == 1 || level == 2) {
            $.MsgBox.Alert("提示", "系统板块不能删除");
            return false;
        }
        CheckCompany(c, "del");
        DeleteOrganizationData(id, isCompany);
    }
}

function CheckCompany(c, d) {
    if (c == "company") {
        if (d == "del") {
            $(".organization-box").css("display", "none");
            $(".select_content").css("display", "none");
        } else {
            $(".organization-box").css("display", "block");
            $(".select_content").css("display", "none");
            $(".organization_content").css("display", "block");
            $(".organization-title").html("组织名称");
        }
        
    } else {
        $(".organization-box").css("display", "block");
        $(".select_content").css("display", "block");
        $(".organization_content").css("display", "none");
        initCompany();
    }
    var rMenu = $("#rMenu");
    if (rMenu) {
        rMenu.css({ "visibility": "hidden" });
    }
    $("body").unbind("mousedown", function (event) {
        if (!(event.target.id == "rMenu" || $(event.target).parents("#rMenu").length > 0)) {
            rMenu.css({ "visibility": "hidden" });
        }
    });
}

function GetSystemInfo(id) {
    WebUtil.ajax({
        async: false,
        url: "/S_OrganizationalManagerControll/GetSystemInfo",
        args: {
            "id": id
        },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $(".organization-title").html(resultData.Data.SystemName);
            }
            else {
                $.MsgBox.Alert("提示", resultData.Message);
            }
        }
    });
}

var PageSize = 10;
var PageNumber = 1;
//初始项目列表
function initCompany(content) {
    Load();
    var el = $(".organization_name").find("input");
    var systemID = el.attr("data-systemID");
    var keyWord = "";
    if (content) {
        keyWord = content;
    }
    WebUtil.ajax({
        async: false,
        url: "/S_OrganizationalManagerControll/GetCompanyInfo",
        args: {
            "systemID": systemID,
            "keyWord": keyWord,
            "pageIndex": PageNumber,
            "pageSize": PageSize
        },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                var totalCount = resultData.TotalCount;
                $('#CompanyMenuData').empty();
                $("#companyName").val("");
                loadTmpl('#OrganizationCompany').tmpl(resultData).appendTo('#CompanyMenuData');
                $("#pager").empty();
                $("#pager").paginationex({
                    current: PageNumber,
                    pageSize: PageSize,
                    totalCount: totalCount,
                    navTo: function (pageIndex) {
                        PageNumber = pageIndex;
                        initCompany();
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

// 项目选中保存
function SaveCompany() {
    var el = $(".organization_name").find("input");
    var systemID = el.attr("data-systemID");
    var pid = el.attr("data-id");
    var level = Number(el.attr("data-level")) + 1;
    var elCompany = $(".companyChecked");
    var data = [];
    for (var i = 0; i < elCompany.length; i++) {
        var id = elCompany.eq(i).attr("data-id");
        var name = elCompany.eq(i).attr("data-name");
        if (elCompany.eq(i).attr("checked") == "checked") {
            var obj = {
                "ID": id,
                "SystemID": systemID,
                "CnName": name,
                "Code": "",
                "ParentID": pid,
                "Level": level,
                "IsCompany": true
            }
            data.push(obj);
        }
    }
    if (!data.length) {
        $.MsgBox.Alert("提示", "请选择项目");
        return false;
    }
    WebUtil.ajax({
        async: false,
        url: "/S_OrganizationalManagerControll/SaveData",
        args: {
            "type": "Add",
            "data": JSON.stringify(data),
            "IsCompany": true,
        },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                initCompany();
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
    $(".organization_content").css("display", "none");
    $(".select_content").css("display", "none");
}
//保存数据
function SaveOrganization(type) {
    var el = $(".organization_name").find("input");
    var systemID = el.attr("data-systemID");
    var pid = el.attr("data-id");
    var id = "00000000-0000-0000-0000-000000000000";
    var level = Number(el.attr("data-level")) + 1;
    var val = $(".organization_edit_name").val();
    if (type == "Edit") {
        level = Number(el.attr("data-level"));
        id = el.attr("data-id");
        pid = el.attr("data-pid");
    }
    //console.log(isCompany);
    var data = [{
        "ID": id,
        "SystemID": systemID,
        "CnName": val,
        "Code": "",
        "ParentID": pid,
        "Level": level,
        "IsCompany": false
    }];
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
            "IsCompany": false,
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
    var iconD = "../Styles/ztree/img/diy/logo.png";
    data.Data.forEach(function (one) {
        var zNodesObj = { id: "", pId: "", name: "", level: "", systemID: "", isCompany: false,};
        zNodesObj.id = one.ID;
        zNodesObj.pId = one.ParentID;
        zNodesObj.name = one.CnName;
        zNodesObj.level = one.Level;
        zNodesObj.systemID = one.SystemID;
        zNodesObj.isCompany = one.IsCompany;
        if (one.Level >= 2 && !one.IsCompany) {
            zNodesObj.icon = "../Styles/ztree/img/diy/2.png";
        }
        zNodes.push(zNodesObj);
        zNodes[0].open = true;
        zNodes[0].iconOpen = iconD;
        zNodes[0].iconClose = iconD;
        zNodes[0].icon = iconD;
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
            onRightClick: OnRightClick

        }
    };
    $.fn.zTree.init($("#tree"), setting, zNodes);
    var zTree = $.fn.zTree.getZTreeObj("tree");
    zTree.setting.check.chkboxType = { "Y": "ps", "N": "ps" };
    var rMenu = $("#rMenu");
    Fake();
    function OnRightClick(event, treeId, treeNode) {
        var el = $(".organization_name").find("input");
        if (treeNode) {
            el.attr({
                "data-id": treeNode.id,
                "data-pid": treeNode.pId,
                "data-name": treeNode.name,
                "data-systemID": treeNode.systemID,
                "data-level": treeNode.level + 1,
                "data-isCompany": treeNode.isCompany
            });
        }
        if (!treeNode && event.target.tagName.toLowerCase() != "button" && $(event.target).parents("a").length == 0) {
            zTree.cancelSelectedNode();
            showRMenu("root", event.clientX, event.clientY, treeNode);
        } else if (treeNode && !treeNode.noR) {
            zTree.selectNode(treeNode);
            showRMenu("node", event.clientX, event.clientY, treeNode);
        }
    };
    function showRMenu(type, x, y, treeNode) {
        var level = 0;
        var company = false;
        if (treeNode) {
            level = treeNode.level + 1;
            company = treeNode.isCompany;
        }
        if (type == "root" || level == 1) {
            $("#rMenu ul").hide();
        } else {
            $("#rMenu ul").show();
            if (level == 2) {
                $("#m_add").show();
                $("#m_addC").show();
                $("#m_edit").hide();
                $("#m_del").hide();
            } else if (level >= 3) {
                if (company) {
                    $("#m_add").hide();
                    $("#m_addC").hide();
                    $("#m_edit").hide();
                    $("#m_del").show();
                } else {
                    $("#m_add").show();
                    $("#m_addC").show();
                    $("#m_edit").show();
                    $("#m_del").show();
                }

            }

        }
        y += document.body.scrollTop;
        x += document.body.scrollLeft;
        rMenu.css({ "top": y + "px", "left": x + "px", "visibility": "visible" });

        $("body").bind("mousedown", onBodyMouseDown);
    }
    function onBodyMouseDown(event) {
        if (!(event.target.id == "rMenu" || $(event.target).parents("#rMenu").length > 0)) {
            rMenu.css({ "visibility": "hidden" });
        }
    }
}
