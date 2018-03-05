
//变量误删
var ComplateDetailData = {};
var MonthReportData = {};
var MonthReportActionData = {};

var Year;
var Month;
var SystemID;
var IsLatestVersion;
var IsNewDataIndex = "";
var MonthlyReportID;
var LastMonth;
var LastYear;
var detailhiden = "";
//流程查询跳转参数
var sysid;
var FinYears;
var FinMonths;
function SearchData() {
    var temp;
    ColumnAmount = 0;
    temp = $(".m_1 > a");
    $(temp).addClass("selected");
    IsNewDataIndex = "";
    ChangeTargetDetail(temp, "Search");
}

//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetGroupRptTmpl.html", selector);

}
function loadActionTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/MonthlyReportActionTmpl.html", selector);
}

//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}

//初始化数据方法
$(document).ready(function () {

    InitSysTree();//初始化数控件数据

    if (GetQueryString("sysID") != null) {
        sysid = $.base64.atob(GetQueryString("sysID"));
        FinYears = $.base64.atob(GetQueryString("FinYears"));
        FinMonths = $.base64.atob(GetQueryString("FinMonths"));
        SystemID = sysid;
        if (SystemID != $("#ddlSystem").val()) {
            var sid = $("#ddlSystem").val()
            $("#ddlSystem").val(sid).attr("selected", false);
            $("#ddlSystem").val(SystemID).attr("selected", true);
            $("#ddlYear").val(FinYears);
            $("#ddlMonth").val(FinMonths);
        } else {
            sysid = GetQueryString("_sysid");
            FinYears = GetQueryString("_finYear");
            FinMonths = GetQueryString("_finMonth");

            $("#ddlSystem").val(sysid).attr("selected", true);
            $("#ddlYear").val(FinYears);
            $("#ddlMonth").val(FinMonths);
        }
    } else {
        sysid = GetQueryString("_sysid");
        FinYears = GetQueryString("_finYear");
        FinMonths = GetQueryString("_finMonth");

        if (FinMonths == $("#ddlMonth").val())
            $("#ddlMonth").val(FinMonths);
        else
            Month = $("#ddlMonth").val();

        if (sysid == $("#ddlSystem").val()) {
            $("#ddlSystem").val(sysid).attr("selected", true);
            SystemID = sysid;
        }
        else
            SystemID = $("#ddlSystem").val();


        if (FinYears == $("#ddlYear").val())
            $("#ddlYear").val(FinYears);
        else
            Year = $("#ddlYear").val();
    }


    //默认选中
    var treeObj = $.fn.zTree.getZTreeObj("SysTree");
    var node = treeObj.getNodeByParam("ID", SystemID);
    treeObj.selectNode(node, false);
    $("#TxtSystem").val(node.TreeNodeName);

    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    IsLatestVersion = latest;
    ShowUpdateDetail();

    //月度经营报告
    getMonthReportSummaryData();

    GetReportTime();
    SelectMessages();


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

        if (treeNode.Category == 1) {
            window.location.href = "TargetRpt.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
        }
        if (treeNode.Category == 2) {
            window.location.href = "ProMonthReport.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
        } else if (treeNode.Category == 3) {
            window.location.href = "TargetGroupRpt.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
        } else if (treeNode.Category == 4) {
            window.location.href = "TargetDirectlyRpt.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
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



//获取上报时间
function GetReportTime() {
    WebUtil.ajax({
        async: false,
        url: "/TimeConfiguration/GetReportDateTime",
        successReturn: function (dateresult) {
            var dt = new Date(dateresult);
            LastMonth = dt.getMonth() + 1;
            LastYear = dt.getFullYear();

        }
    });
}

//管理上报日志以及订阅按钮的显示
function ShowUpdateDetail() {
    detailhiden = $("#detailhidden").val();
    if (detailhiden == "show") {
        $("#detailmana").show();
        $("#submana").show();
    }
    else {
        $("#detailmana").hide();
        $("#submana").hide();
    }
}

//切换不同的报表
function ChangeTargetDetail(sender, TabOrSearch) {
    $(".active_sub2").each(function () {

        $(this).removeClass("active_sub2");
        $(this).parent().removeClass("selected");
    });
    $(sender).addClass("active_sub2");
    $(sender).parent().addClass("selected");

    //$('#LabelDownload').text("导出" + $(sender).text());
    $('#LabelDownload').text("导出全部数据");

    var CTDSystemID = $("#ddlSystem").val();
    var CTDYear = $("#ddlYear").val();
    var CTDMonth = $("#ddlMonth").val();
    var CTDlatest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        CTDlatest = true;
    }

    var CTDIsLatestVersion = CTDlatest;

    if ($(sender).text() == "月度报告" || TabOrSearch == "Search") {
        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv,#DownExcel').show();
        $('#T2,#T3').hide();
        //月度报告
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }
    } else if ($(sender).text() == "明细" && TabOrSearch != "Search") {

        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv,#T3').hide();
        $('#T2,#DownExcel').show();
        var obj = $("#CompleteDetailHead");
        var tab = $("#tab2_rows");
        FloatHeader(obj, tab, false, "MonthRpt");

        //明细
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "B") == true) {
            getMonthReprotDetailData();
        }
    }
    else if ($(sender).text() == "上报日志" && TabOrSearch != "Search") {
        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv,#T2,#DownExcel').hide();
        $('#T3').show();
        var obj = $("#ActionHead");
        var tab = $("#Action_Row");
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportActionData[0], "C") == true) {
            getUpdateDetail();
        }
    }
    if (TabOrSearch != "Search") {
        SystemID = $("#ddlSystem").val();
        Year = $("#ddlYear").val();
        Month = $("#ddlMonth").val();
        var latest = false;
        if ($("#chkIsLastestVersion").attr("checked") == "checked") {
            latest = true;
        }
        IsLatestVersion = latest;
    }
}
//
function TransitionCondition(TCYear, TCMonth, TCSystemID, TCIsLatestVersion, resultDate, index) {
    if (resultDate == undefined) {
        return true;
    }
    if (IsNewDataIndex.indexOf(index) < 0) {
        return true;
    }
    if (TCYear == Year && TCMonth == Month && TCSystemID == SystemID && TCIsLatestVersion == IsLatestVersion) {
        return false;
    } else {
        return true;
    }

}


var ReportInstance = {};
//----------------------月度经营报告-----------------------------------------------------
function SplitData(resultData) {
    // GetMonthReportID(resultData);

    MonthReportData = resultData
    if (resultData) {
        ReportInstance = resultData[0].ObjValue;
        var strSummaryTabel;
        if (resultData[1] != null) {
            $("#txtDes").html("");
            var strTemp = resultData[1].ObjValue;
            strTemp = strTemp.replace(/\n/g, "<br/>").replace(/ /g, "&nbsp;");
            $("#txtDes").html(strTemp);
        }
        if (resultData[2] != null) {
            strSummaryTabel = new Array();
            strSummaryTabel = resultData[2].HtmlTemplate.split(',');
            $("#MonthReportSummaryHead").empty();
            if (strSummaryTabel[0] != "" && strSummaryTabel[0] != undefined) {
                loadTmpl('#' + strSummaryTabel[0]).tmpl().appendTo('#MonthReportSummaryHead');
            } else {
                loadTmpl('#GroupMonthReportSummaryHeadTemplate').tmpl().appendTo('#MonthReportSummaryHead');
            }

            $('#rows').empty();
            if (strSummaryTabel[1] != "" && strSummaryTabel[1] != undefined) {
                loadTmpl('#' + strSummaryTabel[1]).tmpl(resultData[2].ObjValue).appendTo('#rows');
            } else {
                loadTmpl('#GroupMonthReportSummaryTemplate').tmpl(resultData[2].ObjValue).appendTo('#rows');
            }
        }
        if (resultData[3] != null) {
            var lstAtt = {};
            lstAtt = resultData[3].ObjValue;
            if (lstAtt != null) {
                $('#listAttDiv').empty();
                loadTmpl('#listAtt').tmpl(lstAtt).appendTo('#listAttDiv');
                $("#listAttDiv span:last-child").css({ display: "none" });
            }
        }
    }
}
function GetMonthReportID(resultData) {
    SystemID = $("#ddlSystem").val();
    Year = $("#ddlYear").val();
    Month = $("#ddlMonth").val();
    MonthlyReportID = "";
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        MonthlyReportID = resultData[0].ObjValue._MonthReportID;
    }
    else {
        WebUtil.ajax({
            async: false,
            url: "/MonthlyReportController/GetMonthlyReportActionID",
            args: { year: Year, month: Month, SystemID: SystemID },
            successReturn: function (MonthlyRID) {
                if (MonthlyRID != "") {
                    MonthlyReportID = MonthlyRID;
                }

            }
        });
    }
}

function getMonthReportSummaryData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
        IsLatestVersion = latest;
    }

    //这里判断数据源是草稿状态，还是审批状态的
    var dataSource = "Draft";
    if ($("#submana").is(":visible")) {
        dataSource = "Progress";
    }

    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetReportInstance",
        args: { SystemID: $("#ddlSystem").val(), Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest, DataSource: dataSource },
        successReturn: SplitData
    });
    if (IsNewDataIndex.indexOf("A") < 0) {
        IsNewDataIndex = IsNewDataIndex + "A";
    }
}


//---------------------完成情况明细---------------------------------------------------------------------

var MonthReportOrderType = "Detail";
var IncludeHaveDetail = false;
var CompanyProperty = "";
//加载完成情况明细数据
function getMonthReprotDetailData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetDetailRptDataSource",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strCompanyProperty: CompanyProperty, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: function (ResultData) {
            ComplateDetailData = ResultData;

            $('#CompleteDetailHead').empty();
            loadTmpl('#GroupMonthReportDetailHeadTmpl').tmpl().appendTo('#CompleteDetailHead');
            $('#tab2_rows').empty();
            loadTmpl('#GroupComplateTargetDetailTemplate').tmpl(ComplateDetailData).appendTo('#tab2_rows');
        }
    });

    if (IsNewDataIndex.indexOf("B") < 0) {
        IsNewDataIndex = IsNewDataIndex + "B";
    }
}

var DataRowsCount = 0;
function SetDataRowsCount(sender) {
    DataRowsCount = sender.length;
    return "";
}

//区分下载报表
function DownExcelReport(sender) {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var FinMonth = $("#ddlMonth").val();

    //这里判断数据源是草稿状态，还是审批状态的
    var dataSource = "Draft";
    if ($("#submana").is(":visible")) {
        dataSource = "Progress";
    }

    if ($(sender).text().indexOf("月度报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetDetail&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    }
}

//打包下载报表
function DownExcelReportList(sender) {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var FinMonth = $("#ddlMonth").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }

    //这里判断数据源是草稿状态，还是审批状态的
    var dataSource = "Draft";
    if ($("#submana").is(":visible")) {
        dataSource = "Progress";
    }
    window.open("/AjaxHander/DownMonthRptFileList.ashx?SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
}


//加载上报日志
function getUpdateDetail() {
    $("#ActionHead").empty();
    $("#Action_Row").empty();
    loadActionTmpl('#MonthlyReportActionHeadTmpl').tmpl().appendTo('#ActionHead');

    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/MonthlyReportAction",
        args: { SystemID: SystemID, year: Year, month: Month },
        successReturn: function (result) {
            MonthReportActionData = result;
            if (MonthReportActionData.length > 0) {
                for (var i = 0; i < MonthReportActionData.length; i++) {
                    loadActionTmpl('#MonthlyReportActionTmpl').tmpl(MonthReportActionData[i]).appendTo('#Action_Row');
                }

            }
        }
    });

    if (IsNewDataIndex.indexOf("C") < 0) {
        IsNewDataIndex = IsNewDataIndex + "C";
    }
}
//用正则表达式获取URL参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
function AddMessages() {
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/SendMessage",
        args: { sysid: SystemID, month: LastMonth, year: LastYear },
        successReturn: function (ReturnMess) {
            if (ReturnMess == "true") {
                $("#submana").val("取消订阅");
            }
            else {
                $("#submana").val("订阅");
            }
        }
    });
}
function SelectMessages() {
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/SelectMessage",
        args: { sysid: SystemID, month: LastMonth, year: LastYear },
        successReturn: function (ReturnMess) {
            if (ReturnMess == "true") {
                $("#submana").val("订阅");
            }
            else {
                $("#submana").val("取消订阅");
            }
        }
    });
}
