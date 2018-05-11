var SysID;
var sysid;//流程查询传过来的值，注意区分
var System;
var SumMonthTargetList;
var FinYear;
var TargetList;
var SumTargetPlanList;
var HistoryList;
var TarplanID;
//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}

//加载动画结束
function Fake() {
    $.unblockUI();
}

function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/TargetCollectDisplayTmpl.html", selector);
}

function loadTmplhistory(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/TargetConfiguration.html", selector);
}



$(document).ready(function () {

   // InitSysTree()  //初始化树控件

    GetSumList();

});



//初始化Ztree的数据
function InitSysTree() {
    //Ztree的初始化设置
    var setting = {
        view: {
            dblClickExpand: false
        },
        callback: {
            beforeClick: beforeClick,
            onCheck: onCheck
        },
        check: {
            enable: false
        },
        data: {
            key: { name: "TreeNodeName" },
            simpleData: {
                enable: true,
                idKey: "ID",
                pIdKey: "ParentID",
                rootPId: '99999999-9999-9999-9999-FFFFFFFFFFFF'
            }
        }
    };


    $.fn.zTree.init($("#SysTree"), setting, TreeDataJson);

    var treeObj = $.fn.zTree.getZTreeObj("SysTree");
    var nodes = treeObj.getNodes();
    for (var i = 0; i < nodes.length; i++) { //设置节点展开
        treeObj.expandNode(nodes[i], true, false, true);
    }

}

// 树控件只能点击最末级子节点
function beforeClick(treeId, treeNode) {

    var check = (treeNode && !treeNode.isParent);

    if (check) {
        $("#TxtSystem").val(treeNode.TreeNodeName);

        if (treeNode.Category == 3) {

            window.location.href = "../BusinessReport/TargetPlanDetailRpt.aspx?SystemID=" + treeNode.ID;
        } else {
            $("#ddlSystem").val(treeNode.ID).attr("selected", true);

            //__doPostBack('ctl00$ContentPlaceHolder1$ddlSystem','');
            // setTimeout('__doPostBack(\'ctl00$ContentPlaceHolder1$ddlSystem\',\'\')', 0); // 直接调用了下拉框的事件
        }

    }
}

function onCheck(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("SysTree"),
        nodes = zTree.getCheckedNodes(true),
        v = "";
    for (var i = 0, l = nodes.length; i < l; i++) {
        v += nodes[i].name + ",";
    }
    if (v.length > 0) v = v.substring(0, v.length - 1);
    var cityObj = $("#SysTree");
    cityObj.attr("value", v);

}

function showMenu() {
    var cityObj = $("#TxtSystem");
    var cityOffset = $("#TxtSystem").offset();
    $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}

function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "menuBtn" || event.target.id == "TxtSystem" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
        hideMenu();
    } else if (event.target.id.indexOf('_span') > -1) {
        hideMenu();
    }
}





function GetSumList() {
    // $("#tabs").html("<li class=\"sd\" style=\"DISPLAY: list-item\"><a class=\"active3 active_sub3\" onclick=\"Change('sum','')\" id=\"tabsum\">汇总</a></li>");
    $("#HistoryTr").html("");
    if (GetQueryString("sysID") != null) {
        sysid = $.base64.atob(GetQueryString("sysID"));
        SysID = sysid;
        if (SysID != $("#ddlSystem").val()) {
            var sid = $("#ddlSystem").val()
            $("#ddlSystem").val(sid).attr("selected", false);
            $("#ddlSystem").val(SysID).attr("selected", true);
        }
    } else {
        SysID = $("#ddlSystem").val();
    }
    FinYear = $("#ddlYear").val();



    //默认选中
    //var treeObj = $.fn.zTree.getZTreeObj("SysTree");
    //var node = treeObj.getNodeByParam("ID", SysID);
    //treeObj.selectNode(node, false);
    var TxtSystem = $("#ddlSystem").find("option:selected").text();
    $("#TxtSystem").val(TxtSystem);

    //查询版本列表
    BangHistoryTable();



    // BangHistoryTable();


}
//查询数据
function GetList(id) {
    TarplanID = id;//记录选中版本ID

    art.dialog({
        content: $("#divList").html(),
        lock: true,
        id: 'divList',
        title: '<span></span>'
    });
    WebUtil.ajax({
        async: true,
        url: "/CompanyController/GetVerTargetListByTargetPlanID",
        args: { TargetPlanID: id },
        successReturn: function (result) {

            TargetList = result; //绑定 指标
            BangTabs();
            BangHead(result); // 汇总的表头

            BangDetail(id); //汇总的数据    

            SumTargetPlanList = ""; // 将明细数据，清空
            // HistoryList = ""; // 历史查询list

            //  GetSumTargetPlanList(); // 指标下的明细数据 
            $("#SumTable").show();
            $("#TargetTable").hide();

            Fake();

        }
    });

    var obj = $("#Thead1");
    var tab = $("#TrTargetTable");
    FloatHeader(obj, tab);
}

//设置默认版本
function updateDefault(id) {
    WebUtil.ajax({
        async: true,
        url: "/TargetPlanDetailController/UpdateVersionDefault",
        args: { ID: id, SystemID: SysID, Year: FinYear },
        successReturn: function (result) {
            GetSumList();
            if (result == "true")
                alert("变更默认版本成功");
            else
                alert("操作失败,请联系管理员！");
            Fake();
        }
    });

}

//禁用
function IsDeleteA(id) {
    WebUtil.ajax({
        async: true,
        url: "/TargetPlanDetailController/DeleteTargetPlan",
        args: { ID: id, SystemID: SysID, Year: FinYear },
        successReturn: function (result) {
            GetSumList();
            if (result == "true")
                alert("禁用成功");
            else
                alert("操作失败,请联系管理员！");
            Fake();
        }
    });

}

function BangHistoryTable() {


    WebUtil.ajax({
        async: true,
        url: "/TargetController/GetTargetHistory",
        args: { SystemID: SysID, Year: FinYear },
        successReturn: function (result) {
            var plan = new Object();
            plan.list = result;
            loadTmplhistory('#TargetPlanHistoryforVersion').tmpl(plan).appendTo('#HistoryTr');
        }
    });
}


function GetSumTargetPlanList() {
    WebUtil.ajax({
        async: false,
        url: "/TargetPlanDetailController/GetSumTargetDetail",
        args: { strFinYear: FinYear, strSystemID: SysID, strTargetPlanID: TarplanID },
        successReturn: function (result) {
            SumTargetPlanList = result;
        }
    });
}

function BangTabs() {
    loadTmpl('#TabsTarget').tmpl(TargetList).appendTo('#tabs');
    //$("#tabs").append("<li class=\"sd\" style=\"DISPLAY: list-item\"><a class=\"active3\" onclick=\"Change('History','')\" id=\"tabHistory\">指标历史查询</a></li>")
}
function Change(adj, id) {
    //Load();
    document.getElementById('tabsum').className = "active3";
    document.getElementById('tabHistory').className = "active3";

    for (var i = 0; i < TargetList.length; i++) {
        if (id == TargetList[i].ID) {
            document.getElementById('tab' + TargetList[i].ID + '').className = "active3 active_sub3";
        }
        else {
            document.getElementById('tab' + TargetList[i].ID + '').className = "active3";
        }

    }
    if (adj == 'sum') {
        document.getElementById('tabsum').className = "active3 active_sub3";
        $("#SumTable").show();
        $("#TargetTable").hide();
        $("#file_upload-button1").show();
       // $("#HistoryTable").hide();
    }
    //else if (adj == "History") {
    //    document.getElementById('tabHistory').className = "active3 active_sub3";
    //    $("#file_upload-button1").hide();
    //    $("#SumTable").hide();
    //    $("#TargetTable").hide();
    //    $("#HistoryTable").show();
    //    // 获取指标历史查询
    //    BangHistoryTable();

    //}
    else {
        $("#SumTable").hide();
        $("#TargetTable").show();
        BangSumTargetPlanList(id);
        $("#file_upload-button1").show();
        //$("#HistoryTable").hide();
    }
    //Fake();
}
function BangSumTargetPlanList(id) {
    $("#TrTargetTable").html("");
    $("#SumTrTargetTable").html("");

    if (SumTargetPlanList != null && SumTargetPlanList != undefined && SumTargetPlanList.length > 0) {
        //判断当前是否有数据
        for (var i = 0; i < SumTargetPlanList.length; i++) {
            if (SumTargetPlanList[i].TargetID == id) {
                // loadTmpl('#TrSumTargetSum').tmpl(SumTargetPlanList[i]).appendTo('#SumTrTargetTable');
                loadTmpl('#TrSumTargetPlan').tmpl(SumTargetPlanList[i].TargetPlanDetailList).appendTo('#TrTargetTable');
            }
        }
    }
    else {
        WebUtil.ajax({
            async: true,
            url: "/TargetPlanDetailController/GetSumTargetDetail",
            args: { strFinYear: FinYear, strSystemID: SysID, strTargetPlanID: TarplanID },
            successReturn: function (result) {
                SumTargetPlanList = result;
                for (var i = 0; i < SumTargetPlanList.length; i++) {
                    if (SumTargetPlanList[i].TargetID == id) {
                        // loadTmpl('#TrSumTargetSum').tmpl(SumTargetPlanList[i]).appendTo('#SumTrTargetTable');
                        loadTmpl('#TrSumTargetPlan').tmpl(SumTargetPlanList[i].TargetPlanDetailList).appendTo('#TrTargetTable');
                    }
                }
            }
        });

    }

}

//用正则表达式获取URL参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

function BangHead(result) {
    $("#Dsum").attr("colspan", result.length);
    $("#Asum").attr("colspan", result.length);
    $("#TrTarget").html("");
    for (var i = 0; i < 2; i++) {
        loadTmpl('#TrTarget').tmpl(result).appendTo('#TrTarget');
    }

}

function GettargetCount() {
    return TargetList.length;
}


function BangDetail(TID) {
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetSumMonthTargetDetailByTID",
        args: { TargetPlanID: TID },
        successReturn: function (view) {
            //console.log(JSON.stringify(view));
            SumMonthTargetList = view;
        }
    });
    if (SumMonthTargetList != null) {
        var row = "";
        for (var i = 0; i <= 11; i++) {
            row += "<tr> <td class=\"Td_Center\">" + SumMonthTargetList[i].FinMonth + "月</td>";

            for (var j = 0; j < SumMonthTargetList[i].TargetDetailList.length; j++) {

                if (SumMonthTargetList[i].TargetDetailList[j].Target != null) {
                    row += "<td class=\"Td_Right\"title=" + SumMonthTargetList[i].TargetDetailList[j].Target + ">" + MathTarget(SumMonthTargetList[i].TargetDetailList[j].Target) + "</td>";
                }
                else {
                    row += "<td class=\"Td_Right\">--</td>";
                }


            }


            for (var j = 0; j < SumMonthTargetList[i].TargetDetailList.length; j++) {

                if (SumMonthTargetList[i].TargetDetailList[j].SumTarget != null) {
                    row += "<td class=\"Td_Right\" title=" + SumMonthTargetList[i].TargetDetailList[j].SumTarget + ">" + MathTarget(SumMonthTargetList[i].TargetDetailList[j].SumTarget) + "</td>";
                }
                else {
                    row += "<td class=\"Td_Right\">--</td>";
                }
            }


            row += "</tr>";
        }
        row += "<tr><th class=\"th_Sub2\">全年</th>";

        for (var j = 0; j < SumMonthTargetList[11].TargetDetailList.length; j++) {

            if (SumMonthTargetList[11].TargetDetailList[j].SumTarget != null) {
                row += "<th class=\"th_Sub2\" style=\"text-align:right\" title=" + SumMonthTargetList[11].TargetDetailList[j].SumTarget + ">" + MathTarget(SumMonthTargetList[11].TargetDetailList[j].SumTarget) + "</th>";
            }
            else {
                row += "<td class=\"th_Sub2\" style=\"text-align:right\">--</td>";
            }


        }

        for (var i = 0; i < TargetList.length; i++) {
            row += "<th class=\"th_Sub2\"style=\"text-align:right\">--</th>";
        }
        row += "</tr>";
        $("#rows").html(row);
    }


}
function DownExcel() {
    window.open("/AjaxHander/DownExcelTargetCollect.ashx?SysId=" + SysID + "&FinYear=" + FinYear + "&TargetPlanID=" + TarplanID);
}
function DownLoad(TargetPlanID) {
    window.open("/AjaxHander/DownExcelTargetHistory.ashx?SysId=" + SysID + "&FinYear=" + FinYear + "&TargetPlanID=" + TargetPlanID);
}
function FormatTime(value) {
    if (value != null) {
        var Time = new Date(value).toDateString();
        //var NewDate = Time.getYear() + "-" + Time.getMonth() + "-" + Time.getDay();
        return Time;
    }
    else {
        return "--";
    }
}


function ClickChange(adj) {
    if (adj == "sum") {
        $(".sum").hide();
        $(".dan").show();
    }

    else {
        $(".sum").show();
        $(".dan").hide();
    }
}

function MathTarget(num) {
    var vv = Math.pow(10, 0);
    return Math.round(num * vv) / vv;
}

//激活
function ActTarget(id) {
    $("#Remark").empty();
    TarplanID = id;
    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>激活原因</span>'
    });
}

function AddRemark() {

    var Remarks = $("#Remark").val();
    WebUtil.ajax({
        async: true,
        url: "/TargetController/ChangeTargetStatus",
        args: { TargetPlanID: TarplanID, Description: Remarks },
        successReturn: function (result) {
            GetSumList();
            art.dialog({ id: 'divDetail' }).close();
            alert("激活成功");
        }
    });
}

function CheckAction(ID) {
    $("#historyaction").empty();
    WebUtil.ajax({
        async: true,
        url: "/TargetController/GetActionByTargetplanID",
        args: { TargetplanID: ID },
        successReturn: function (result) {
            loadTmplhistory('#TargetAction').tmpl(result).appendTo('#historyaction');
            art.dialog({
                content: $("#div1").html(),
                lock: true,
                id: 'div1',
                title: '<span>操作日志</span>'
            });
        }
    });





}