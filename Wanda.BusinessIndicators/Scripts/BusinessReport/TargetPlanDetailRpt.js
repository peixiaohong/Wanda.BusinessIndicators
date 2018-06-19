var HistoryList;
var SystemID;
var FinYear;
var TargetPlanID;
var TargetPlanDeailData;
//加载模版项
function loadTmplTargetPlanDetail(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetPlanDetailReportedTmpl.html", selector);
}
function loadHistoryTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/TargetConfiguration.html", selector);
}

$(function () {

    InitSysTree()  //初始化树控件


    if (WebUtil.getQueryString("sysID") != "" && WebUtil.getQueryString("sysID") != undefined ) { // 这是从 流程查看跳过来的
        sysid = $.base64.atob(WebUtil.getQueryString("sysID"));
        SysID = sysid;
        if (SysID != $("#ddlSystem").val()) {
            var sid = $("#ddlSystem").val()
            $("#ddlSystem").val(sid).attr("selected", false);
            $("#ddlSystem").val(SysID).attr("selected", true);
        }
    } else {
        //这是从TargetCollectDisplay.aspx 页面跳转过来的

        SysID = $("#ddlSystem").val();
    }
    FinYear = $("#ddlYear").val();


    //默认选中
    var treeObj = $.fn.zTree.getZTreeObj("SysTree");
    var node = treeObj.getNodeByParam("ID", SysID);
    treeObj.selectNode(node, false);
    $("#TxtSystem").val(node.TreeNodeName);


    GetTargetPlanDetail();


})


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

        if (treeNode.Category != 3) {
            window.location.href = "../SystemConfiguration/TargetCollectDisplay.aspx?SystemID="+treeNode.ID;
        } else {
            $("#ddlSystem").val(treeNode.ID).attr("selected", true);
            setTimeout('__doPostBack(\'ctl00$ContentPlaceHolder1$ddlSystem\',\'\')', 0); // 直接调用了下拉框的事件
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
    }
}




function GetTargetPlanDetail() {

   
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    var s = TargetPlanID;
    WebUtil.ajax({
        async: true,
        url: "/TargetPlanDetailController/GetTargetPlanDetail",
        args: { strSystemID: $("#ddlSystem").attr("value"), strFinYear: $("#ddlYear").attr("value"), strTargetPlanID: TargetPlanID, IsLatestVersion: latest },
        successReturn: SplitData
    });
}
var strTemplate = new Array();
function SplitData(result) {
    GethistoryList();

    TargetPlanDeailData = result;
    $("#rows").empty();
    $("#Ul4").empty();
    $("#TargetPlanDetailHead").empty();
   
    if (TargetPlanDeailData[0] != null) {
        if (TargetPlanDeailData[0].HtmlTemplate != undefined) {
            strTemplate = TargetPlanDeailData[0].HtmlTemplate.split(',');
        }
        if (strTemplate[0] != "" && strTemplate[0] != undefined)
        {
            loadTmplTargetPlanDetail('#' + strTemplate[0]).tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
            if (strTemplate[0] == "TargetPlanDetailReportTableHeadTemplate_ToYear") {
                $("#importedDataTable2").css("width", "500px");
            } else {
                $("#importedDataTable2").css("width", "100%");
            }
        } else
        {
            loadTmplTargetPlanDetail('#TargetPlanDetailReportTableHeadTemplate').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
        }
        if (TargetPlanDeailData[0].ObjValue[0] != null) {
            loadTmplTargetPlanDetail('#TargetPlanDetailReportHeadTemplate').tmpl(TargetPlanDeailData).appendTo('#Ul4');
            LoadTargetPlanDetailData(TargetPlanDeailData[0]);
            $("#Ul4 :first a").addClass("active_sub3");

            var obj = $("#TargetPlanDetailHead");
            var tab = $("#rows");
            FloatHeader(obj, tab, false, "MonthRpt");
            $("#Ul4").append("<li class=\"sd\"><a class=\"active2\" onclick=\"TargetPlanDetailLiaddCss(this);\">历史指标查询</a></li>");

        }
        else {
            if (HistoryList.length>0) {
                loadTmplTargetPlanDetail('#TargetHistory').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
                LoadHistoryDetail();
            }
            $("#file_upload-button1").hide();
            $("#Ul4").append("<li class=\"sd\"><a class=\"active2 active_sub3\" onclick=\"TargetPlanDetailLiaddCss(this);\">历史指标查询</a></li>");

        }
    }
}

function LoadTargetPlanDetailData(sender) {
    if (TargetPlanDeailData[0].ObjValue[0]!= null) {
        if (strTemplate[1] != "" && strTemplate[1] != undefined) {
            loadTmplTargetPlanDetail('#'+strTemplate[1]).tmpl(sender).appendTo("#rows");
        }else{
            loadTmplTargetPlanDetail("#TargetPlanDetailReportTemplate").tmpl(sender).appendTo("#rows");
        }
    }
}

function SearchData() {
    GetTargetPlanDetail();
}


var currentTargetPlanDetail;
function TargetPlanDetailLiaddCss(sender) {

    var TemplData = {};
    $.each(TargetPlanDeailData, function (i, item) {
        if (item.Name == $(sender).text()) {
            TemplData = item;
            return;
        }
    });
    $("#Ul4 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });
    $(sender).addClass("active_sub3");
    currentTargetPlanDetail = sender;
  
    var obj = $("#TargetPlanDetailHead");
    var tab = $("#rows");
    FloatHeader(obj, tab, false, "MonthRpt");

    $("#rows").empty();
    $("#TargetPlanDetailHead").empty();

    if (TemplData.HtmlTemplate != undefined) {
        strTemplate = TemplData.HtmlTemplate.split(',');
    }
    if (sender.innerText == "历史指标查询") {
        loadHistoryTmpl('#TargetPlanHistoryHead').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
        $("#file_upload-button1").hide();
        LoadHistoryDetail();
    }
    else {
        if (strTemplate[0] != "" && strTemplate[0] != undefined) {
            loadTmplTargetPlanDetail('#' + strTemplate[0]).tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
            $("#file_upload-button1").show();
            LoadTargetPlanDetailData(TemplData)
        }
        else {
            loadTmplTargetPlanDetail('#TargetPlanDetailReportTableHeadTemplate').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
            $("#file_upload-button1").show();
            LoadTargetPlanDetailData(TemplData)
        }
    }
   

    //LoadTargetPlanDetailData(TemplData)
}
function LoadHistoryDetail() {

    loadHistoryTmpl('#TargetPlanHistory').tmpl(HistoryList).appendTo('#rows');
}
function GethistoryList() {
    FinYear = $("#ddlYear").val();
    SystemID = $("#ddlSystem").val();
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetTargetHistory",
        args: { SystemID: SystemID, Year: FinYear },
        successReturn: function (result) {
            HistoryList = result;
        }
    });

}
function DownLoad(TargetPlanID) {
    window.open("/AjaxHander/DownExcelTargetHistory.ashx?SysId=" + SystemID + "&FinYear=" + FinYear + "&TargetPlanID=" + TargetPlanID);
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

function DownExcelReport(sender) {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    window.open("/AjaxHander/DownExcelTargetPlan.ashx?FileType='MissTargetRpt'&SystemID=" + $("#ddlSystem").attr("value") + "&TargetPlanID=&FinYear=" + $("#ddlYear").attr("value") + "&IsLatestVersion=" + latest + "&IsReported=true");
}

//格式化日期类型
function FormatDate(obj, displayTime, local) {
    try {
        var date = new Date(obj);
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var hour = date.getHours();
        var minute = date.getMinutes();
        var second = date.getSeconds();

        if (displayTime) {

            if (obj == "0001-01-01T00:00:00" ||obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null || obj == "0001/1/1 0:00:00") {
                return "---";
            } else {
                if (local == "CN") {
                    return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                } else {
                    return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                }
            }
        } else {
            if (obj == "0001-01-01T00:00:00" ||obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null || obj == "0001/1/1 0:00:00") {
                return "---";
            } else {
                if (local == "CN") { //如果是CN的显示中文年月
                    return "{1}月{2}".formatBy(year, month, doubleDigit(day));
                } else {

                    return "{0}-{1}-{2}".formatBy(year, doubleDigit(month), day);
                }
            }
        }


    } catch (e) {
        return "";
    }

    function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }





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
            GetTargetPlanDetail();
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
            loadHistoryTmpl('#TargetAction').tmpl(result).appendTo('#historyaction');
            art.dialog({
                content: $("#div1").html(),
                lock: true,
                id: 'div1',
                title: '<span>操作日志</span>'
            });
        }
    });

}