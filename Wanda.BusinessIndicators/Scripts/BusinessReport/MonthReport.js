
//变量误删
var ReturnData = {};
var MissTargetData = {};
var CurrentMissTargetData = {};
var ComplateDetailData = {};
var ManageReportDetailData = {};
var MonthReportData = {};
var MonthReportActionData = {};
var Monthsg;//从webconfig中读取的商管ID
var Monthsgrent;//从webconfig中读取的商管租金收缴率的ID
var MonthsgBig;

var Year;
var Month;
var SystemID;
var IsLatestVersion;
var IsNewDataIndex = "";
var MonthReportOrderType = "Detail";
var IncludeHaveDetail = false;
var MonthlyReportID;
var LastMonth;
var LastYear;
//流程查询跳转参数
var sysid;
var FinYears;
var FinMonths;
var detailhiden = "";
var currentDetailTarget = null;
var currentManageReportDetailTarget = null;
var unit = "";
function SearchData() {
    var temp;
    ColumnAmount = 0;
    temp = $(".m_1 > a");
    $(temp).addClass("selected");
    IsNewDataIndex = "";
    ChangeTargetDetail(temp, "Search");
    MonthReportOrderType = "Detail";
    currentDetailTarget = null;
    currentManageReportDetailTarget = null;
}

//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetRptTmpl.html", selector);
}

function loadActionTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/MonthlyReportActionTmpl.html", selector);
}

//加载模版项-------------------------------------------------------------------------
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/TargetReturnTmpl.html", selector);
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

    InitSysTree(); //初始化数控件数据

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



    Monthsg = $("#sg").val();
    Monthsgrent = $("#sgrent").val();
    MonthsgBig = $("#sgBig").val();
    Monthsg = Monthsg.toLowerCase();
    Monthsgrent = Monthsgrent.toLowerCase();
    MonthsgBig = MonthsgBig.toLowerCase();

    GetReportTime();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    IsLatestVersion = latest;

    ShowUpdateDetail();

    //月度经营报告
    getMonthReportSummaryData();

    SelectMessages();

    //获取系统选择框的值,当值不是百货时,隐藏那四个(暂时全部隐藏)
    if ($("#ddlSystem option:selected").text() != "百货系统") {
        $("#BHhide1").hide();
        //$("#BHhide2").hide();
        //$("#BHhide3").hide();
        //$("#BHhide4").hide();
    }

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
//用正则表达式获取URL参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
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

    if ($(sender).text() == "月度经营报告" || TabOrSearch == "Search") {
        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv,#DownExcel').show();
        $('#T2,#T2_1,#T3,#T3_1,#T4,#T5,#T6,#T7,#T8,#T9').hide();

        //月度经营报告
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }
    } else if ($(sender).text() == "完成情况明细" && TabOrSearch != "Search") {

        $('#T1,#T2_1,#T3,#T3_1,#T4,#MonthReportExplainDiv,#ApproveAttachDiv,#T5,#T6,#T7,#T8,#T9').hide();
        $('#T2,#DownExcel').show();
        var obj = $("#CompleteDetailHead");
        var tab = $("#tab2_rows");
        FloatHeader(obj, tab, false, "MonthRpt");

        //完成情况明细
        GetMonthReportDetailSearchCondition();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "B") == true) {
            getMonthReprotDetailData();
        }
    }
    else if ($(sender).text() == "经营报告明细" && TabOrSearch != "Search") {

        $('#T1,#T2,#T3,#T3_1,#T4,#MonthReportExplainDiv,#ApproveAttachDiv,#T5,#T6,#T7,#T8,#T9').hide();
        $('#T2_1,#DownExcel').show();
        var obj = $("#CompleteDetailHead_1");
        var tab = $("#tab2_rows_1");
        FloatHeader(obj, tab, false, "MonthRpt");

        //经营报告明细
        GetMonthReportDetailSearchCondition();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ManageReportDetailData[0], "B") == true) {
            getManageReprotDetailData();
        }
    }
    else if ($(sender).text() == "当月未完成" && TabOrSearch != "Search") {
        $("#T4,#T1,#T2,#T2_1,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T5,#T6,#T7,#T8,#T9").hide();
        $("#T3_1,#DownExcel").show();
        var obj = $("#Tab_MissTargetHead");
        var tab = $("#Tbody_MissTargetData");
        //FloatHeader(obj, tab, false, "MonthRpt");
        //未完成说明
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, CurrentMissTargetData[0], "W") == true) {
            getCurrentMonthReportMissTargetData();
        }
    }
    else if ($(sender).text() == "累计未完成" && TabOrSearch != "Search") {
        $("#T4,#T1,#T2,#T2_1,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5,#T6,#T7,#T8,#T9").hide();
        $("#T3,#DownExcel").show();

        var obj = $("#Tab_MissTargetHead");
        var tab = $("#Tbody_MissTargetData");

        FloatHeader(obj, tab, false, "MonthRpt");
        //未完成说明
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MissTargetData[0], "C") == true) {
            getMonthReportMissTargetData();
        }

    } else if ($(sender).text() == "补回情况明细" && TabOrSearch != "Search") {
        $("#T4,#DownExcel").show();
        $("#T1,#T2,#T2_1,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5,#T6,#T7,#T8,#T9").hide();

        //$("#Tab_Return").removeAttr("style");
        var obj = $("#Tab_ReturnHead");
        var tab = $("#Tbody_Data");

        FloatHeader(obj, tab, false, "MonthRpt");
        //补回情况明细
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "D") == true) {
            getMonthReportReturnData();
        }

    } else if ($(sender).text() == "上报日志" || TabOrSearch == "Search") {
        $("#T9").show();
        $("#T1,#T2,#T2_1,#T3,#T3_1,#T5,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#DownExcel").hide();

        var obj = $("#ActionHead");
        var tab = $("#Action_Row");

        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportActionData[0], "I") == true) {
            getUpdateDetail();
        }
    }
    else if ($(sender).text() == "其它情况" || TabOrSearch != "Search") {
        $("#T5,#DownExcel").show();
        $("#T1,#T2,#T2_1,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#T9").hide();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "E") == true) {
            getDSReportSummaryData();
        }
        var obj = $("#DSMonthReportSummaryHead");
        var tab = $("#DSrows");
    }

    else if ($(sender).text() == "指标完成情况明细" || TabOrSearch == "Search") {
        //$("#T6,#DownExcel").show();
        //$("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T5,#T7,#T8,#T9").hide();
        $("#T5,#DownExcel").show();
        $("#T1,#T2,#T2_1,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#T9").hide();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "F") == true) {
            getDSReprotDetailData();
        }
        var obj = $("#DSMonthReportSummaryHead");
        var tab = $("#DSrows");
        //var obj = $("#DSCompleteDetailHead");
        //var tab = $("#DStab2_rows");
    } else if ($(sender).text() == "补回指标缺口情况" || TabOrSearch == "Search") {
        $("#T7,#DownExcel").show();
        $("#T1,#T2,#T2_1,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T5,#T6,#T8,#T9").hide();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "G") == true) {
            getDSMonthReportReturnData();
        }
        var obj = $("#DSReturn_Thead");
        var tab = $("#DSTbody_Data");
    } else if ($(sender).text() == "新增未完成门店情况" || TabOrSearch == "Search") {
        $("#T8,#DownExcel").show();
        $("#T1,#T2,#T2_1,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T5,#T6,#T7,#T9").hide();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "H") == true) {
            getMonthReportAddMissTargetData();
        }
        var obj = $("#DSAdd_Thead");
        var tab = $("#AddTarget_Tbody");
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
    if ($(sender).text() != "完成情况明细") {
        $('#imgtableUpDown,#SearchPanel').hide();
        $('#imgtableUpDown').attr("src", "../Images/images1/Down.png");
    } else {
        $('#imgtableUpDown').show();
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

//是否显示查询条件动画方法
function UpDownTableClick() {
    if ($('#imgtableUpDown').attr("src") == "../Images/images1/Down.png") {
        $('#imgtableUpDown').attr("src", "../Images/images1/Up.png");
        $('#SearchPanel').slideDown("slow");
    }
    else {
        $('#imgtableUpDown').attr("src", "../Images/images1/Down.png");
        $('#SearchPanel').slideUp("slow");
    }
}


//每组行数量
function getGroupCount(obj, send) {
    if (obj.length != 0 && obj != undefined) {

        var RowCount = 0;

        $.each(obj, function (i, item) {

            if (item.Mark != undefined) { //数据

                if (item.Mark == "Counter") // 分组
                {
                    $.each(item.ObjValue, function (j, item1) {

                        if (item.Mark != undefined)
                        { } else { $.each(item.ObjValue, function (m, data2) { RowCount++; }); }
                    });
                } else { $.each(item.ObjValue, function (m, data1) { RowCount++; }); }

            } else { RowCount = obj.length; }

        });

        if (obj[0].TargetGroupCount == 1) {
            if (send == "Miss") {
                if (obj[0].SystemName == "商管系统") {
                    return RowCount - 1;
                } else {
                    return RowCount;
                }
            } else {
                return RowCount;
            }
        }
        else {
            return (RowCount) / obj[0].TargetGroupCount;
        }
    } else { return 0; }
}


function IsCounter(obj) {
    if (obj.Mark != null && obj.Mark != undefined && obj.Mark == "Counter") {
        return true;
    }
    return false;
}

function IsGroup(obj) {
    if (obj.Mark != null && obj.Mark != undefined && obj.Mark == "Group") {
        return true;
    }
    return false;
}

var ReportInstance = {};

//----------------------月度经营报告-----------------------------------------------------
function SplitData(resultData) {
    // GetMonthReportID(resultData);

    //MonthlyReportID = 'C5A4E3BA-727C-4DC0-A83D-503F3D1FC2D5';//开发用   
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
                loadTmpl('#MonthReportSummaryHeadTemplate').tmpl().appendTo('#MonthReportSummaryHead');

            }

            $('#rows').empty();
            if (strSummaryTabel[1] != "" && strSummaryTabel[1] != undefined) {
                loadTmpl('#' + strSummaryTabel[1]).tmpl(resultData[2].ObjValue).appendTo('#rows');
            } else {
                loadTmpl('#MonthReportSummaryTemplate').tmpl(resultData[2].ObjValue).appendTo('#rows');
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
        if (resultData[4] != null) {
            var SearchList = {};
            SearchList = resultData[4].ObjValue;
            $('#SearchPanel').empty();
            $('#MonthlyReportDetailSearhTemplate').tmpl(SearchList).appendTo('#SearchPanel');

            for (var i = 0; i < SearchList.length; i++) {
                if (SearchList.length <= 5) {
                    $("#txt" + SearchList[i].ColumnName).MultDropList("select" + SearchList[i].ColumnName, "s", null, true);

                }

            }
            $(".multiselect").css("float", "left");
            $(".list").blur(function () {

                getMonthReprotDetailData();
            }
            );
        }
        if (resultData[5] != null) {
            if (resultData[5].ObjValue) {
                $(".jybgmx").removeClass("hide");
            }
        }
    }
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

    if (IsNewDataIndex.indexOf("I") < 0) {
        IsNewDataIndex = IsNewDataIndex + "I";
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
//加载完成情况明细数据
function getMonthReprotDetailData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    GetMonthReportDetailSearchCondition();
    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetDetailRptDataSource",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strCompanyProperty: CompanyProperty, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: function (ResultData) {
            ComplateDetailData = ResultData;
            SetComplateTargetDetailData(ComplateDetailData[0], 1);
        }
    });

    if (IsNewDataIndex.indexOf("B") < 0) {
        IsNewDataIndex = IsNewDataIndex + "B";
    }
}
var CompanyProperty = "";
function GetMonthReportDetailSearchCondition() {
    CompanyProperty = "";
    $("#CompanyProperty1 .SearchCheckBox").each(function (i, item) {
        if (i == 0) {
            CompanyProperty += "CompanyProperty1:"
        }
        if ($(this).attr("checked") == "checked") {
            CompanyProperty += $(this).context.value + ",";
        }
        if (i == $("#CompanyProperty1 .SearchCheckBox").length - 1) {
            CompanyProperty += ";"
        }
    })
    if ($("#CompanyProperty1 .SearchCheckBox").length == 0) {
        if ($("#hid_CompanyProperty1") != null) {
            if ($("#hid_CompanyProperty1").val() != undefined)
                CompanyProperty += "CompanyProperty1:" + $("#hid_CompanyProperty1").val() + ";";
        }
    }
    $("#CompanyProperty2 .SearchCheckBox").each(function (i, item) {
        if (i == 0) {
            CompanyProperty += "CompanyProperty2:"
        }
        if ($(this).attr("checked") == "checked") {
            CompanyProperty += $(this).context.value + ",";
        }
        if (i == $("#CompanyProperty2 .SearchCheckBox").length - 1) {
            CompanyProperty += ";"
        }
    })
    if ($("#CompanyProperty2 .SearchCheckBox").length == 0) {
        if ($("#hid_CompanyProperty2") != null) {
            if ($("#hid_CompanyProperty2").val() != undefined)
                CompanyProperty += "CompanyProperty2:" + $("#hid_CompanyProperty2").val() + ";";
        }
    }
    $("#CompanyProperty3 .SearchCheckBox").each(function (i, item) {
        if (i == 0) {
            CompanyProperty += "CompanyProperty3:"
        }
        if ($(this).attr("checked") == "checked") {
            CompanyProperty += $(this).context.value + ",";
        }
        if (i == $("#CompanyProperty3 .SearchCheckBox").length - 1) {
            CompanyProperty += ";"
        }
    })
    if ($("#CompanyProperty3 .SearchCheckBox").length == 0) {
        if ($("#hid_CompanyProperty3") != null) {
            if ($("#hid_CompanyProperty3").val() != undefined)
                CompanyProperty += "CompanyProperty3:" + $("#hid_CompanyProperty3").val() + ";";
        }
    }
    $("#CompanyProperty4 .SearchCheckBox").each(function (i, item) {
        if (i == 0) {
            CompanyProperty += "CompanyProperty4:"
        }
        if ($(this).attr("checked") == "checked") {
            CompanyProperty += $(this).context.value + ",";
        }
        if (i == $("#CompanyProperty4 .SearchCheckBox").length - 1) {
            CompanyProperty += ";"
        }
    })
    if ($("#CompanyProperty4 .SearchCheckBox").length == 0) {
        if ($("#hid_CompanyProperty4") != null) {
            if ($("#hid_CompanyProperty4").val() != undefined)
                CompanyProperty += "CompanyProperty4:" + $("#hid_CompanyProperty4").val() + ";";
        }
    }
    $("#CompanyProperty5 .SearchCheckBox").each(function (i, item) {
        if (i == 0) {
            CompanyProperty += "CompanyProperty5:"
        }
        if ($(this).attr("checked") == "checked") {
            CompanyProperty += $(this).context.value + ",";
        }
        if (i == $("#CompanyProperty5 .SearchCheckBox").length - 1) {
            CompanyProperty += ";"
        }
    })
    if ($("#CompanyProperty5 .SearchCheckBox").length == 0) {
        if ($("#hid_CompanyProperty5") != null) {
            if ($("#hid_CompanyProperty5").val() != undefined)
                CompanyProperty += "CompanyProperty5:" + $("#hid_CompanyProperty5").val() + ";";
        }
    }
}

function ClickCompanyProperty() {
    getMonthReprotDetailData();
}
function ComplateDetailLiaddCss(sender) {
    var TemplData = {};
    $.each(ComplateDetailData, function (i, item) {
        if (item.Name == $(sender).text()) {
            TemplData = item;
            return;
        }
    });
    $("#Ul4 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });
    $(sender).addClass("active_sub3");

    currentDetailTarget = $("#Ul4 li .active_sub3 ");
    $("#importedDataTable2").css("width", "100%");
    strComplateMonthReportDetilHtmlTemplate = new Array();
    if (TemplData.HtmlTemplate != undefined) {
        strComplateMonthReportDetilHtmlTemplate = TemplData.HtmlTemplate.split(',');
    }

    $("#CompleteDetailHead").empty();
    //表头Tmpl名称
    if (strComplateMonthReportDetilHtmlTemplate[0] != "" && strComplateMonthReportDetilHtmlTemplate[0] != undefined) {
        loadTmpl('#' + strComplateMonthReportDetilHtmlTemplate[0]).tmpl(TemplData).appendTo('#CompleteDetailHead');
    } else {
        loadTmpl('#CompleteDetailHeadTemplate').tmpl().appendTo('#CompleteDetailHead');
    }

    //tmpl模板名称
    if (strComplateMonthReportDetilHtmlTemplate[1] != "" && strComplateMonthReportDetilHtmlTemplate[1] != undefined) {
        ComplateTargetDetailTemplate = strComplateMonthReportDetilHtmlTemplate[1];
    } else {
        ComplateTargetDetailTemplate = "ComplateTargetDetailTemplate"
    }

    var dataArray = [];
    var data = {
        "data": TemplData
    };
    dataArray.push(data);

    $("#tab2_rows").empty();
    loadTmpl('#' + ComplateTargetDetailTemplate).tmpl(dataArray).appendTo('#tab2_rows');
    SetComplateTargetDetailData(TemplData, 2);
}
var ComplateTargetDetailTemplate = null;
function SetComplateTargetDetailData(sender, Type) {
    strComplateMonthReportDetilHtmlTemplate = new Array();
    if (sender != null || sender != undefined) {
        if (sender.HtmlTemplate != undefined) {
            strComplateMonthReportDetilHtmlTemplate = sender.HtmlTemplate.split(',');
        }
    }

    $("#CompleteDetailHead").empty();
    //表头Tmpl名称
    if (strComplateMonthReportDetilHtmlTemplate[0] != "" && strComplateMonthReportDetilHtmlTemplate[0] != undefined) {
        loadTmpl('#' + strComplateMonthReportDetilHtmlTemplate[0]).tmpl(sender).appendTo('#CompleteDetailHead');
    } else {
        loadTmpl('#TmplCompleteDetail_Head').tmpl(sender).appendTo('#CompleteDetailHead');
    }
    //tmpl模板名称
    if (strComplateMonthReportDetilHtmlTemplate[1] != "" && strComplateMonthReportDetilHtmlTemplate[1] != undefined) {
        ComplateTargetDetailTemplate = strComplateMonthReportDetilHtmlTemplate[1];
    } else {
        ComplateTargetDetailTemplate = "TmplCompleteDetail_Data"
    }

    if (currentDetailTarget == null) {
        $("#Ul4").empty();

        //if ($("#ddlSystem option:selected").text() != "百货系统") {

        //    $("#BHhide1").hide();
        //    var jsonBH = [{ 'Name': '完成门店数量' }, { 'Name': '完成情况明细' }, { 'Name': '补回上月缺口' }, { 'Name': '新增未完成门店' }];

        //    for (var i = 0; i < jsonBH.length; i++) {

        //        ComplateDetailData.push(jsonBH[i]);
        //    }
        //}
        if (ComplateDetailData.length > 0) {
            loadTmpl('#ComplateTargetDetailHeadTemplate').tmpl(ComplateDetailData).appendTo('#Ul4');
            $("#tab2_rows").empty();

            var dataArray = [];
            var data = { "data": sender };
            dataArray.push(data);
            loadTmpl('#' + ComplateTargetDetailTemplate).tmpl(dataArray).appendTo('#tab2_rows');
        }
        $("#Ul4 :first a").addClass("active_sub3");
    } else {
        if (Type == 1) {
            ComplateDetailLiaddCss(currentDetailTarget);
        }
    }
    AddBackGroundColor();
}


//此三方法仅限于完成情况明细模板使用
var ColumnAmount = 0;

function setColumnAmount(sender) {
    ColumnAmount = 1;
    return sender;
}

function upateColumnAmount(sender) {

    var temp = sender - ColumnAmount;

    return temp;
}

//汇总数据
var complateCount = 0;
var unComplateCount = 0;

//为完成情况明细排序（本月排序和年累计排序）
function MonthReportOrder(sender) {
    currentDetailTarget = $("#Ul4 li .active_sub3 ");
    MonthReportOrderType = sender;
    getMonthReprotDetailData();
}
function showMonthReprotDetail(companyId) {
    var targetData = [];
    for (var i = 0; i < ComplateDetailData.length; i++) {
        if (ComplateDetailData[i].IsBlendTarget) {
            //1、取指标
            for (var j = 0; j < ComplateDetailData[i].ObjValue.length; j++) {
                //targetData.push(ComplateDetailData[i].ObjValue[j]);
                //2、取指标下的完成与未完成信息
                for (var k = 0; k < ComplateDetailData[i].ObjValue[j].ObjValue[0].ObjValue.length; k++) {
                    //  3、取完成与未完成信息下的公司信息
                    var companyData = Enumerable.From(ComplateDetailData[i].ObjValue[j].ObjValue[0].ObjValue[k].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                    if (companyData.length > 0) {
                        targetData.push(companyData[0]);
                    }
                }
            }
        }
        else {
            //1、取指标下的完成与未完成信息
            if (ComplateDetailData[i].ObjValue[0].Mark == "CompanyProperty") {
                for (var j = 0; j < ComplateDetailData[i].ObjValue[0].ObjValue.length; j++) {
                    if (ComplateDetailData[i].ObjValue[0].ObjValue[j].ObjValue != undefined) {
                        //targetData.push(ComplateDetailData[i].ObjValue[j]);
                        //  2、取完成与未完成信息下的公司信息
                        var companyData = Enumerable.From(ComplateDetailData[i].ObjValue[0].ObjValue[j].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                        if (companyData.length > 0) {
                            targetData.push(companyData[0]);
                        }
                    }
                }
            }
            else if (ComplateDetailData[i].ObjValue[0].Mark == "Counter" && ComplateDetailData[i].ObjValue[0].Name == "HaveDetail") {
                //  2、取完成与未完成信息下的公司信息
                var companyData = Enumerable.From(ComplateDetailData[i].ObjValue[0].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                if (companyData.length > 0) {
                    targetData.push(companyData[0]);
                }
            }
        }
    }
    $("#tbCompanyDataDetail").empty();
    var resultData = { "data": targetData };
    loadTmpl('#CompanyDataDetail_tmpl').tmpl(resultData).appendTo('#tbCompanyDataDetail');

    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>指标完成情况</span>'
    });
}
//-------------经营报告明细------------------------------------------------------------------------------------------------------

//为经营报告明细排序（本月排序和年累计排序）
function ManageReportOrder(sender) {
    currentManageReportDetailTarget = $("#Ul4_1 li .active_sub3 ");
    MonthReportOrderType = sender;
    getManageReprotDetailData();
}
//获取经营报告明细数据
function getManageReprotDetailData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    GetMonthReportDetailSearchCondition();
    //经营报告明细
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetManageDetailRptDataSource",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strCompanyProperty: CompanyProperty, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: function (ResultData) {
            ManageReportDetailData = ResultData;
            SetManageMonthReprotDetailData(ManageReportDetailData[0], 1);
        }
    });

    if (IsNewDataIndex.indexOf("B") < 0) {
        IsNewDataIndex = IsNewDataIndex + "B";
    }
}
//填充经营报告明细模板数据
function SetManageMonthReprotDetailData(sender, Type) {

    var strManageMonthReprotDetailHtmlTemplate = new Array();
    var strManageMonthReprotDetailTemplate = "";
    if (sender != null || sender != undefined) {
        if (sender.HtmlTemplate != undefined) {
            strManageMonthReprotDetailHtmlTemplate = sender.HtmlTemplate.split(',');
        }
    }

    $("#CompleteDetailHead_1").empty();
    //表头Tmpl名称
    if (strManageMonthReprotDetailHtmlTemplate[0] != "" && strManageMonthReprotDetailHtmlTemplate[0] != undefined) {
        loadTmpl('#' + strManageMonthReprotDetailHtmlTemplate[0]).tmpl(sender).appendTo('#CompleteDetailHead_1');
    } else {
        loadTmpl('#TmplCompleteDetail_Head').tmpl(sender).appendTo('#CompleteDetailHead_1');
    }
    //tmpl模板名称
    if (strManageMonthReprotDetailHtmlTemplate[1] != "" && strManageMonthReprotDetailHtmlTemplate[1] != undefined) {
        strManageMonthReprotDetailTemplate = strManageMonthReprotDetailHtmlTemplate[1];
    } else {
        strManageMonthReprotDetailTemplate = "TmplManageTargetDetail_Data"
    }

    if (currentManageReportDetailTarget == null) {
        $("#Ul4_1").empty();
        loadTmpl('#ManageTargetDetailHeadTemplate').tmpl(ManageReportDetailData).appendTo('#Ul4_1');
        $("#tab2_rows_1").empty();

        if (ManageReportDetailData.length > 0) {

            var dataArray = [];
            var data = { "data": sender };
            dataArray.push(data);
            loadTmpl('#' + strManageMonthReprotDetailTemplate).tmpl(dataArray).appendTo('#tab2_rows_1');
        }
        $("#Ul4_1 :first a").addClass("active_sub3");
    } else {
        if (Type == 1) {
            ManageMonthReprotDetailLiaddCss(currentManageReportDetailTarget);
        }
    }
    AddBackGroundColor();
}
function ManageMonthReprotDetailLiaddCss(sender) {
    var TemplData = {};
    $.each(ManageReportDetailData, function (i, item) {
        if (item.Name == $(sender).text()) {
            TemplData = item;
            return;
        }
    });
    $("#Ul4_1 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });
    $(sender).addClass("active_sub3");

    currentManageReportDetailTarget = $("#Ul4_1 li .active_sub3 ");
    $("#importedDataTable2_1").css("width", "100%");
    strManageReportDetailTargetHtmlTemplate = new Array();
    if (TemplData.HtmlTemplate != undefined) {
        strManageReportDetailTargetHtmlTemplate = TemplData.HtmlTemplate.split(',');
    }

    $("#CompleteDetailHead_1").empty();
    //表头Tmpl名称
    if (strManageReportDetailTargetHtmlTemplate[0] != "" && strManageReportDetailTargetHtmlTemplate[0] != undefined) {
        loadTmpl('#' + strManageReportDetailTargetHtmlTemplate[0]).tmpl(TemplData).appendTo('#CompleteDetailHead_1');
    } else {
        loadTmpl('#CompleteDetailHeadTemplate').tmpl(TemplData).appendTo('#CompleteDetailHead_1');
    }

    //tmpl模板名称
    if (strManageReportDetailTargetHtmlTemplate[1] != "" && strManageReportDetailTargetHtmlTemplate[1] != undefined) {
        ManageReportDetailTargetTemplate = strManageReportDetailTargetHtmlTemplate[1];
    } else {
        ManageReportDetailTargetTemplate = "TmplManageTargetDetail_Data"
    }
    var dataArray = [];
    var data = {
        "data": TemplData
    };
    dataArray.push(data);

    $("#tab2_rows_1").empty();
    loadTmpl('#' + ManageReportDetailTargetTemplate).tmpl(dataArray).appendTo('#tab2_rows_1');
    SetManageMonthReprotDetailData(TemplData, 2);
}
function ClickTr(sender, level) {
    var childClass = $(sender).attr("data-value");
    if (level == "Area") {
        var childTrArray = $("." + childClass);
        if ($(sender).hasClass("DetailShow") == true) {
            $(sender).removeClass("DetailShow").addClass("Detailminus");
            for (var i = 0; i < childTrArray.length; i++) {
                $(childTrArray[i]).removeClass("DetailShow").addClass("Detailminus");
                var grandChildClass = $(childTrArray[i]).attr("data-value");
                $("." + grandChildClass).hide();
            }
            $("." + childClass).hide();
        }
        else {
            $(sender).removeClass("Detailminus").addClass("DetailShow");
            for (var i = 0; i < childTrArray.length; i++) {
                $(childTrArray[i]).removeClass("DetailShow").addClass("Detailminus");
                var grandChildClass = $(childTrArray[i]).attr("data-value");
                $("." + grandChildClass).hide();
            }
            $("." + childClass).show();
        }
        //alert($(sender).attr("data-value"));
    }
    else if (level == "LastArea") {
        if ($(sender).hasClass("DetailShow") == true) {
            $(sender).removeClass("DetailShow").addClass("Detailminus");
            $("." + childClass).hide();
        }
        else {
            $(sender).removeClass("Detailminus").addClass("DetailShow");
            $("." + childClass).show();
        }
    }
}
function showManageMonthReprotDetail(companyId, areaName, targetName) {
    var targetData = [];
    for (var i = 0; i < ManageReportDetailData.length; i++) {
        if (ManageReportDetailData[i].IsBlendTarget) {
            //1、取指标
            for (var j = 0; j < ManageReportDetailData[i].ObjValue.length; j++) {
                //targetData.push(ComplateDetailData[i].ObjValue[j]);
                //2、取指标下的完成与未完成信息
                if (ManageReportDetailData[i].ObjValue[j].ObjValue[0].ObjValue[0].Mark == "Area") {
                    var areaObj = Enumerable.From(ManageReportDetailData[i].ObjValue[j].ObjValue[0].ObjValue).Where("$.Name=='" + areaName + "'").ToArray();
                    for (var k = 0; k < areaObj[0].ObjValue.length; k++) {
                        var companyData = Enumerable.From(areaObj[0].ObjValue[k].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                        if (companyData.length > 0) {
                            targetData.push(companyData[0]);
                        }
                    }
                }
                else {
                    var companyData = Enumerable.From(ManageReportDetailData[i].ObjValue[j].ObjValue[0].ObjValue[0].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                    if (companyData.length > 0) {
                        targetData.push(companyData[0]);
                    }
                }
            }
        }
        else {
            //1、取指标
            for (var j = 0; j < ManageReportDetailData[i].ObjValue.length; j++) {
                //targetData.push(ComplateDetailData[i].ObjValue[j]);
                //2、取指标下的完成与未完成信息
                if (ManageReportDetailData[i].ObjValue[j].ObjValue[0].Mark == "Area") {
                    var areaObj = Enumerable.From(ManageReportDetailData[i].ObjValue[j].ObjValue).Where("$.Name=='" + areaName + "'").ToArray();
                    for (var k = 0; k < areaObj[0].ObjValue.length; k++) {
                        var companyData = Enumerable.From(areaObj[0].ObjValue[k].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                        if (companyData.length > 0) {
                            targetData.push(companyData[0]);
                        }
                    }
                }
                else {
                    var companyData = Enumerable.From(ManageReportDetailData[i].ObjValue[j].ObjValue[0].ObjValue).Where("$.CompanyID=='" + companyId + "'").ToArray();
                    if (companyData.length > 0) {
                        targetData.push(companyData[0]);
                    }
                }
            }
        }
    }

    $("#tbCompanyDataDetail").empty();
    var resultData = { "data": targetData };
    loadTmpl('#CompanyDataDetail_tmpl').tmpl(resultData).appendTo('#tbCompanyDataDetail');

    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>指标完成情况</span>'
    });
}

//-------------补回情况------------------------------------------------------------------------------------------------------
function getMonthReportReturnData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetTargetReturnList",
        args: { rpts: WebUtil.jsonToString(ReportInstance), IsLatestVersion: latest },
        successReturn: function (result) {
            ReturnData = result;

            //首先指标先加载
            $("#U1").empty();
            $("#Tbody_Data").empty();
            $("#Tab_ReturnHead").empty();

            loadTmpl_1('#TmplHeadReturn').tmpl().appendTo('#Tab_ReturnHead');  //加载裂头

            if (ReturnData.length == 0) {
                $(".newdiff_retu").show();
                $(".Level1TDSL").attr("colspan", 9);
                $('#Tab_ReturnHead tr th').eq(14).hide();
                return;
            }


            if (ReturnData.length > 1) {  //判断指标有几个分组，如果是2个一上默认选择第一个

                loadTmpl_1('#TmplTargerList').tmpl(ReturnData).appendTo('#U1'); //加载补回指标 Tab

                if (ReturnData[0].TargetGroupCount == 1) {
                    //单个补回 ，（代表：商管，物管）
                    if (ReturnData[0].HtmlTemplate != "") {
                        var tempstr = '#' + ReturnData[0].HtmlTemplate;
                        loadTmpl_1(tempstr).tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                    } else {
                        loadTmpl_1('#TmplTargetRetu_SG_DetailRpt').tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                    }


                    $(".newdiff_retu").hide();
                    $(".Level1TDSL").attr("colspan", 8);
                } else {
                    //混合组合 ，（代表：旅业）
                    loadTmpl_1('#TmplTargetRetu').tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                    $(".newdiff_retu").show();
                    $(".Level1TDSL").attr("colspan", 9);
                }
            } else {
                //完全组合
                loadTmpl_1('#TmplTargetRetu').tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                $(".newdiff_retu").show();
                $(".Level1TDSL").attr("colspan", 9);
            }

            $("#U1 :first a").addClass("active_sub3");
            $(".shangyueleiji").hide();
            $('#CurrentMonthBackDetilDiv').text("本月累计(万元) [+]");

        }
    });

    if (IsNewDataIndex.indexOf("D") < 0) {
        IsNewDataIndex = IsNewDataIndex + "D";
    }
}


//补回情况 tab（补回明细）
function RtunLiaddCss(sender) {
    var m = {};
    $.each(ReturnData, function (n, obj) {
        if (obj.Name == $(sender).text()) {
            m = obj.ObjValue;
            return;
        }
    });
    $("#U1 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });


    $(sender).addClass("active_sub3");
    $('#Tbody_Data').html("");

    if (m[0].TargetGroupCount == 1) {
        //单个补回 ，（代表：商管，物管）
        if (ReturnData[0].HtmlTemplate != "") {
            var tempstr = '#' + ReturnData[0].HtmlTemplate; //获取tmpl的名称
            loadTmpl_1(tempstr).tmpl(m).appendTo('#Tbody_Data');
        } else {
            loadTmpl_1('#TmplTargetRetu_SG_DetailRpt').tmpl(m).appendTo('#Tbody_Data');
        }

        $(".newdiff_retu").hide();
        $(".Level1TDSL").attr("colspan", 8);
    } else {
        //混合组合 ，（代表：旅业）
        loadTmpl_1('#TmplTargetRetu').tmpl(m).appendTo('#Tbody_Data');
        $(".newdiff_retu").show();
        $(".Level1TDSL").attr("colspan", 9);
    }

    //显示影藏
    $(".shangyueleiji").hide();
    $(".TTR2").attr("colspan", 3);
    $(".Special_return").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当出现完成率的时候，差值TD是没有右面的边线的
    $('#CurrentMonthBackDetilDiv').text("本月累计(万元) [+]");

}


//未完成 tab（未完成说明）--------------------------------------------------------------------------------------------
function getMonthReportMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //未完成说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetMissTargetList",
        args: { rpts: WebUtil.jsonToString(ReportInstance), IsLatestVersion: latest },
        successReturn: function (result) {

            MissTargetData = result;

            //var string = WebUtil.jsonToString(MissTargetData);



            //先影藏其它标签
            //首先指标先加载
            $("#U2").empty();
            $("#Tbody_MissTargetData").empty();
            $("#Tab_MissTargetHead").empty();

            var multitarget = false;

            loadTmpl_1('#TmplHeadMiss').tmpl().appendTo('#Tab_MissTargetHead'); //加载列头



            if (MissTargetData.length == 0) {
                $(".newdiff_miss").show();
                $(".Level1TdSp1").attr("colspan", 11);

                $('#Tab_MissTargetHead tr th').eq(15).hide();
               
                return;
            }

            //（拆分单个指标）
            if (MissTargetData.length > 1) {

                loadTmpl_1('#TmplMissTargerList').tmpl(MissTargetData).appendTo('#U2'); //指标标签

                if (MissTargetData[0].TargetGroupCount == 1) {
                    //单个指标（代表：商管，物管）
                    loadTmpl_1('#TmplMissTarget_SG').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                    $(".newdiff_miss").hide();
                    $(".Level1TdSp1").attr("colspan", 10);
                } else {
                    //混合组合指标（代表：旅业）
                    loadTmpl_1('#TmplMissTarget').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                    $(".newdiff_miss").show();
                    $(".Level1TdSp1").attr("colspan", 11);
                }
                multitarget = true;

            } else {
                
                //组合指标
                loadTmpl_1('#TmplMissTarget').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                $(".newdiff_miss").show();
                $(".Level1TdSp1").attr("colspan", 11);
                
            }
            $("#U2 :first a").addClass("active_sub3");

            var obj = $("#Tab_MissTargetHead");
            var tab = $("#Tbody_MissTargetData");

            $(".shangyue").hide();
            $("#Tab_MissTarget").attr({ style: "table-layout: auto" });
            $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");

        }
    });
    if (IsNewDataIndex.indexOf("C") < 0) {
        IsNewDataIndex = IsNewDataIndex + "C";
    }
}


//未完成 tab（未完成说明）--------------------------------------------------------------------------------------------
function getCurrentMonthReportMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //未完成说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetCurrentMissTargetList",
        args: { rpts: WebUtil.jsonToString(ReportInstance), IsLatestVersion: latest },
        successReturn: function (result) {

            CurrentMissTargetData = result;

            //var string = WebUtil.jsonToString(MissTargetData);
            //先影藏其它标签
            //首先指标先加载
            $("#U2_1").empty();
            $("#Tbody_CurrentMissTargetData").empty();
            $("#Tab_CurrentMissTargetHead").empty();

            var multitarget = false;

            loadTmpl_1('#TmplCurrentHeadMiss').tmpl().appendTo('#Tab_CurrentMissTargetHead'); //加载列头

            if (CurrentMissTargetData.length == 0) {
                $(".newdiff_CurrenMiss").show();
                $(".Curr_Level1TdSp1").attr("colspan", 11);
                $('#Tab_CurrentMissTargetHead tr th').eq(15).hide();
                return;
            }



            //（拆分单个指标）
            if (CurrentMissTargetData.length > 1) {

                loadTmpl_1('#TmplCurrentMissTargerList').tmpl(CurrentMissTargetData).appendTo('#U2_1'); //指标标签

                if (CurrentMissTargetData[0].TargetGroupCount == 1) {
                    //单个指标（代表：商管，物管）
                    loadTmpl_1('#TmplCurrentMissTarget_SG').tmpl(CurrentMissTargetData[0]).appendTo('#Tbody_CurrentMissTargetData');
                    $(".newdiff_CurrenMiss").hide();
                    $(".Curr_Level1TdSp1").attr("colspan", 10);
                } else {
                    //混合组合指标（代表：旅业）
                    loadTmpl_1('#TmplCurrentMissTarget').tmpl(CurrentMissTargetData[0]).appendTo('#Tbody_CurrentMissTargetData');
                    $(".newdiff_CurrenMiss").show();
                    $(".Curr_Level1TdSp1").attr("colspan", 11);
                }
                multitarget = true;

            } else {
                //组合指标
                loadTmpl_1('#TmplCurrentMissTarget').tmpl(CurrentMissTargetData[0]).appendTo('#Tbody_CurrentMissTargetData');
                $(".newdiff_CurrenMiss").show();
                $(".Curr_Level1TdSp1").attr("colspan", 11);

            }
            $("#U2_1 :first a").addClass("active_sub3");

            var obj = $("#Tab_CurrentMissTargetHead");
            var tab = $("#Tbody_CurrentMissTargetData");

            $(".leiji").hide();
            $("#Tab_CurrentMissTarget").attr({ style: "table-layout: auto" });
            //$('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");

        }
    });

    if (IsNewDataIndex.indexOf("W") < 0) {
        IsNewDataIndex = IsNewDataIndex + "W";
    }
}


//累计的单个指标筛选
function MissLiaddCss(sender) { //未完成指标筛选（代表：商管体统）
    var m = {};
    $.each(MissTargetData, function (n, obj) {
        if (obj.Name == $(sender).text()) {
            m = obj.ObjValue;
            return;
        }
    });

    $("#U2 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });

    $(sender).addClass("active_sub3");
    $('#Tbody_MissTargetData').html("");

    if (m[0].TargetGroupCount == 1) {
        //单个指标（代表：商管，物管）
        loadTmpl_1('#TmplMissTarget_SG').tmpl(m).appendTo('#Tbody_MissTargetData');
        $(".newdiff_miss").hide();
        $(".Level1TdSp1").attr("colspan", 10);
    } else {
        //混合组合指标（代表：旅业）
        loadTmpl_1('#TmplMissTarget').tmpl(m).appendTo('#Tbody_MissTargetData');
        $(".newdiff_miss").show();
        $(".Level1TdSp1").attr("colspan", 11);
    }

    //显示影藏
    $(".shangyue").hide();
    $(".TT2").attr("colspan", 3);
    $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的

    $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");
}


//当月的单个指标筛选
function MissCurrentLiaddCss(sender) {
    var m = {};
    $.each(CurrentMissTargetData, function (n, obj) {
        if (obj.Name == $(sender).text()) {
            m = obj.ObjValue;
            return;
        }
    });

    $("#U2_1 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });

    $(sender).addClass("active_sub3");

    currentMissTarget = sender;
    $('#Tbody_CurrentMissTargetData').html("");

    if (m[0].TargetGroupCount == 1) {
        //单个指标（代表：商管，物管）
        loadTmpl_1('#TmplCurrentMissTarget_SG').tmpl(m).appendTo('#Tbody_CurrentMissTargetData');

        $(".newdiff_CurrenMiss").hide();  //单个(这里：经营系统：商管和物管)
        $(".Curr_Level1TdSp1").attr("colspan", 10);
    } else {

        //混合组合指标（代表：旅业）
        loadTmpl_1('#TmplCurrentMissTarget').tmpl(m).appendTo('#Tbody_CurrentMissTargetData');

        $(".newdiff_CurrenMiss").show(); //混合 （这里：混合组合：旅业）
        $(".Curr_Level1TdSp1").attr("colspan", 11);
    }
    // loadTmpl_1('#TmplCurrentMissTargetRpt').tmpl(m).appendTo('#Tbody_CurrentMissTargetData');

    //显示影藏
    $(".leiji").hide();
    $(".C_TT2").attr("colspan", 4);
    $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
    //$(".Curr_Level1TdSp1").attr("colspan", 11);
    $('#MonthMissTergetDiv_Current').text("本月发生(万元) [+]");

}




//收缩 累计未完成
function ShouSuo(sender) {

    //未完成
    if (sender == 'YC') {

        $(".shangyue").hide();
        $(".TT2").attr("colspan", 3);
        $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
        $(".Level1TdSp1").attr("colspan", 11);
        $("#Tab_MissTarget").removeAttr("style");
        $('#CurrentMonthMissTergetDiv').text("本月累计(万元)  [+]");

        var obj = $("#Tab_MissTargetHead");
        var tab = $("#Tbody_MissTargetData");

        FloatHeader(obj, tab, false, "MonthRpt"); //浮动表头

    } else if (sender == 'XS') {

        var obj = $("#Tab_MissTargetHead");
        var tab = $("#Tbody_MissTargetData");

        if ($(".newdiff_miss").is(":hidden")) {  //给商管系统用的
            if ($(".Level1TdSp1").attr("colspan").toInt() == 10) {
                $(".shangyue").show();
                $(".TT2").attr("colspan", 4);
                $(".Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
                $(".Level1TdSp1").attr("colspan", 14);

                $("#Tab_MissTarget").attr({ style: "table-layout: auto" });
                $('#CurrentMonthMissTergetDiv').text("本月累计(万元)");


            } else {
                $(".shangyue").hide();
                $(".TT2").attr("colspan", 3);
                $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
                $(".Level1TdSp1").attr("colspan", 10);
                $("#Table1").removeAttr("style");
                $('#CurrentMonthMissTergetDiv').text("本月累计(万元)  [+]");

            }

        } else {
            if ($(".Level1TdSp1").attr("colspan").toInt() == 11) {
                $(".shangyue").show();
                $(".TT2").attr("colspan", 4);
                $(".Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
                $(".Level1TdSp1").attr("colspan", 15);
                $("#Tab_MissTarget").attr({ style: "table-layout: auto" });
                $('#CurrentMonthMissTergetDiv').text("本月累计(万元)");

            } else {
                $(".shangyue").hide();
                $(".Level1TdSp1").attr("colspan", 11);
                $(".TT2").attr("colspan", 3);
                $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
                $("#Table1").removeAttr("style");
                $('#CurrentMonthMissTergetDiv').text("本月累计(万元)  [+]");

            }
        }

        FloatHeader(obj, tab, false, "MonthRpt");

    } else if (sender == 'YCSY') {
        ////补回说明
        $(".shangyueleiji").hide();
        $(".TTR2").attr("colspan", 3);
        $(".Special_return").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的

        $(".Level1TDSL").attr("colspan", 9);
        $("#Tab_Return").removeAttr("style");
        $('#CurrentMonthBackDetilDiv').text("本月累计(万元)  [+]");

        var obj = $("#Tab_ReturnHead");
        var tab = $("#Tbody_Data");
        FloatHeader(obj, tab, false, "MonthRpt");

    } else if (sender == 'XSSY') {

        var obj = $("#Tab_ReturnHead");
        var tab = $("#Tbody_Data");

        if ($(".newdiff_retu").is(":hidden")) {

            if ($(".Level1TDSL").attr("colspan").toInt() == 8) {
                $(".shangyueleiji").show();
                $(".TTR2").attr("colspan", 4);
                $(".Special_return").removeClass("Td_Right").addClass("Td_TopAndBottom");//当出现完成率的时候，差值TD是没有右面的边线的
                $(".Level1TDSL").attr("colspan", 12);
                $('#CurrentMonthBackDetilDiv').text("本月累计(万元)");

            } else {
                $(".shangyueleiji").hide();
                $(".Level1TDSL").attr("colspan", 8);
                $(".TTR2").attr("colspan", 3);
                $(".Special_return").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当出现完成率的时候，差值TD是没有右面的边线的
                $("#Tab_Return").removeAttr("style");
                $('#CurrentMonthBackDetilDiv').text("本月累计(万元)  [+]");

            }

        } else {

            if ($(".Level1TDSL").attr("colspan").toInt() == 9) {
                $(".shangyueleiji").show();
                $(".Level1TDSL").attr("colspan", 13);
                $(".TTR2").attr("colspan", 4);
                $(".Special_return").removeClass("Td_Right").addClass("Td_TopAndBottom");//当出现完成率的时候，差值TD是没有右面的边线的

                $('#CurrentMonthBackDetilDiv').text("本月累计(万元)");



            } else {
                $(".shangyueleiji").hide();
                $(".Level1TDSL").attr("colspan", 9);
                $(".TTR2").attr("colspan", 3);
                $(".Special_return").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当出现完成率的时候，差值TD是没有右面的边线的

                $("#Tab_Return").removeAttr("style");
                $('#CurrentMonthBackDetilDiv').text("本月累计(万元)  [+]");


            }
        }

        FloatHeader(obj, tab, false, "MonthRpt");
    }

}


//收缩 当月未完成
function ShouSuo_Current(sender) {
    if (sender == 'YC') {
        $(".leiji").hide();
        $(".C_TT2").attr("colspan", 3);
        $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
        $(".Curr_Level1TdSp1").attr("colspan", 11);
        $("#Tab_MissTarget").removeAttr("style");
        $('#MonthMissTergetDiv_Current').text("本月发生(万元)  [+]");

    } else if (sender == 'XS') {

        if ($(".newdiff_CurrenMiss").is(":hidden")) {
            if ($(".Curr_Level1TdSp1").attr("colspan").toInt() == 10) {
                $(".leiji").show();
                $(".C_TT2").attr("colspan", 4);
                $(".Curr_Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
                $(".Curr_Level1TdSp1").attr("colspan", 14);
                //$("#Tab_MissTarget").attr({ style: "table-layout: fixed" });
                $('#MonthMissTergetDiv_Current').text("本月发生(万元) [-]");
            } else {
                $(".leiji").hide();
                $(".C_TT2").attr("colspan", 3);
                $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
                $(".Curr_Level1TdSp1").attr("colspan", 10);
                $("#Table1").removeAttr("style");

                $('#MonthMissTergetDiv_Current').text("本月发生(万元)  [+]");
            }
        } else {
            if ($(".Curr_Level1TdSp1").attr("colspan").toInt() == 11) {
                $(".leiji").show();
                $(".C_TT2").attr("colspan", 4);
                $(".Curr_Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
                $(".Curr_Level1TdSp1").attr("colspan", 15);
                // $("#Tab_MissTarget").attr({ style: "table-layout: fixed" });
                $('#MonthMissTergetDiv_Current').text("本月发生(万元) [-]");
            } else {
                $(".leiji").hide();
                $(".C_TT2").attr("colspan", 3);
                $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
                $(".Curr_Level1TdSp1").attr("colspan", 11);
                $("#Table1").removeAttr("style");

                $('#MonthMissTergetDiv_Current').text("本月发生(万元)  [+]");
            }
        }
    }
}



//区分下载报表
function DownExcelReport(sender) {
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


    var send = $(sender).text();
    if ($(sender).text().indexOf("月度经营报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($.trim(send) == "导出完成情况明细") {
        //经营系统的明细下载，分当月和累计排序下载
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetDetail&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&OrderStr=" + MonthReportOrderType + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("累计未完成") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=MissTarget&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("回情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetReturn&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    }
    else if ($(sender).text().indexOf("其它情况") > 1) {
        //其它情况
        var sub_send = $("#Ul5 .active_sub4").text();
        if (sub_send == "完成门店数量") {
            var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetSum&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth;
            window.open(url);
        } else if (sub_send == "完成情况明细") {
            var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetCompleted&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
            window.open(url);
        } else if (sub_send == "补回上月缺口") {
            var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetReturnData&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
            window.open(url);
        } else if (sub_send == "新增未完成门店") {
            var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetAddData&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
            window.open(url);
        }
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
    window.open("/AjaxHander/DownMonthRptFileList.ashx?Param=" + latest + "&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&OrderStr=" + MonthReportOrderType + "&DataSource=" + dataSource);
}

function LoadSearchConditions(strConditions) {
    $('#panel').html(strConditions)
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

            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                return "---";
            } else {
                if (local == "CN") {
                    return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                } else {
                    return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                }
            }
        } else {
            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                return "---";
            } else {
                if (local == "CN") { //如果是CN的显示中文年月
                    return "{1}月{2}".formatBy(year, month, doubleDigit(day));
                } else {
                    return "{0}-{1}-{2}".formatBy(year, doubleDigit(month), doubleDigit(day));
                }
            }
        }


    } catch (e) {
        return "";
    }

    function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }

}


//显示隐藏列表的数据
function ClickCounter(sender, val) {
    if ($(sender).hasClass("DetailShow") == true) {
        $(sender).removeClass("DetailShow").addClass("Detailminus");
        $(sender).nextUntil(".item1").each(function () {
            $(this).show();
        });
        var rowSpanVal = 0;
        val = (val == "" ? "99999BBBB" : val);
        rowSpanVal = $("." + val).attr("rowspan");
        if (rowSpanVal == undefined)
            rowSpanVal = 0;
        $("." + val).attr("rowspan", (rowSpanVal * 1 + $(sender).nextUntil(".item1").length));
    }
    else {
        $(sender).removeClass("Detailminus").addClass("DetailShow");
        $(sender).nextUntil(".item1").each(function () {
            $(this).hide();
        });
        var rowSpanVal = 0;
        val = (val == "" ? "99999BBBB" : val);
        rowSpanVal = $("." + val).attr("rowspan");
        if (rowSpanVal == undefined)
            rowSpanVal = 0;
        $("." + val).attr("rowspan", (rowSpanVal * 1 - $(sender).nextUntil(".item1").length));
    }

}
var VcounterRowID = "CompanyPropertyItem0";
var VcounterRow = 0;
function setMargeCount() {
    VcounterRow = VcounterRow + 1;
    VcounterRowID = VcounterRowID.substring(0, VcounterRowID.length - 1) + VcounterRow;
    ColumnAmount = 0;
    return "";
}
var DataRowsCount = 0;
function SetDataRowsCount(sender) {
    DataRowsCount = sender.length;
    return "";
}

function alertstring(sender) {
    alert(sender);
}


//列表收缩显示/隐藏        
function TrLv1Show(obj) {
    var tr = $(obj).parents("tr:first");
    if ($(obj).hasClass("show") == true) {
        $(obj).removeClass("show").addClass("minus");

        var flag = false;

        //首先标记
        $(tr).nextUntil(".Level1").each(function () {
            if ($(this).context.className.indexOf("Level2") >= 0) {
                flag = true;
            }
        });

        //根据标记来做判断
        $(tr).nextUntil(".Level1").each(function () {
            if (flag) {
                if ($(this).context.className.indexOf("Level2") >= 0) {
                    $(this).show();
                }
            } else { $(this).show(); }
        });
    }
    else {
        $(obj).removeClass("minus").addClass("show");
        $(tr).nextUntil(".Level1").each(function () {
            $(this).hide();
        });
    }
}

function TrLv2Show(obj) {
    var tr = $(obj).parents("tr:first");

    if ($(obj).hasClass("show") == true) {
        $(obj).removeClass("show").addClass("minus");

        $(tr).nextUntil(".Level2").each(function () {

            if ($(this).hasClass("Level1") == true)
            { return; } else {

                if ($(this).context.className.indexOf("Level3") >= 0) {
                    $(this).show();
                }
            }
        });
    }
    else {
        $(obj).removeClass("minus").addClass("show");
        $(tr).nextUntil(".Level2").each(function () {
            if ($(this).hasClass("Level1") == true)
            { return; } else {
                if ($(this).context.className.indexOf("Level3") >= 0) {
                    $(this).hide();
                }
            }

        });
    }
}

//----------------------经营指标完成门店数量情况-----------------------------------------------------
function getDSReportSummaryData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetCompleted",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {

            if (ResultData.length > 0) {

                var tableHead = CreateTableHeadHtml($("#ddlMonth").val());

                var DSSummaryData = {};
                DSSummaryData.ResultData = ResultData;
                DSSummaryData.ResultData.FinMonth = $("#ddlMonth").val();

                //表头
                $("#DSMonthReportSummaryHead").empty();
                $("#DSMonthReportSummaryHead").append(tableHead);
                $("#DSrows").empty();

                //数据绑定
                loadTmpl_1('#DSCompanySummaryTmpl').tmpl(DSSummaryData).appendTo('#DSrows'); //加载数据

                LoadBHother();
            }
        }
    });
    if (IsNewDataIndex.indexOf("E") < 0) {
        IsNewDataIndex = IsNewDataIndex + "E";
    }
}//构建表头


function LoadBHother() {

    $("#Ul5").empty();

    if ($("#ddlSystem option:selected").text() == "百货系统") {
        var jsonBH = [{ 'Name': '完成门店数量' }, { 'Name': '完成情况明细' }, { 'Name': '补回上月缺口' }, { 'Name': '新增未完成门店' }];
    }

    loadTmpl('#BHHeadTemplate').tmpl(jsonBH).appendTo('#Ul5');
    //$("#tab2_rows").empty();
    //loadTmpl('#' + ComplateTargetDetailTemplate).tmpl(sender).appendTo('#tab2_rows');
    $("#Ul5 :first a").addClass("active_sub4");

}

function BHLiaddCss(sender) {
    $("#Ul5 .active_sub4").each(function () {
        $(this).removeClass("active_sub4");
    });

    $(sender).addClass("active_sub4");

    if ($(sender).text() == "完成门店数量") {
        $("#T5,#DownExcel").show();
        $("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#T9").hide();
        getDSReportSummaryData();

    } else if ($(sender).text() == "完成情况明细") {
        //$("#T6,#DownExcel").show();
        //$("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T5,#T7,#T8,#T9").hide();
        $("#T5,#DownExcel").show();
        $("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#T9").hide();
        getDSReprotDetailData();

    } else if ($(sender).text() == "补回上月缺口") {
        //$("#T7,#DownExcel").show();
        //$("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T5,#T6,#T8,#T9").hide();
        $("#T5,#DownExcel").show();
        $("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#T9").hide();
        getDSMonthReportReturnData();

    } else if ($(sender).text() == "新增未完成门店") {
        //$("#T8,#DownExcel").show();
        //$("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T5,#T6,#T7,#T9").hide();
        $("#T5,#DownExcel").show();
        $("#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T4,#T6,#T7,#T8,#T9").hide();
        getMonthReportAddMissTargetData();

    }
}




function CreateTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 5%\">序号</th><th rowspan=\"2\" style=\"width: 15%\">项目</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">" + month + "月(个)</th><th colspan=\"4\" style=\"width: 25%\">" + month + "月累计(个)</th></tr>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
    }
    else {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 5%\">序号</th><th rowspan=\"2\" style=\"width: 15%\">项目</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">1-" + (month - 1) + "月累计(个)</th><th colspan=\"4\" style=\"width: 25%;\" >" + month + "月累计(个)</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">1-" + month + "月累计(个)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th></tr>";
    }
    return strhtml;
}

//---------------------指标完成情况明细---------------------------------------------------------------------
//加载完成情况明细数据
function getDSReprotDetailData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetCompletedDetail",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {

            //$("#DSCompleteDetailHead").empty();
            //$("#DSCompleteDetailHead").append(tableHead);
            $("#DStab2_rows").empty();
            if (ResultData.length > 0) {
                //var strTbody = "";

                var tableHead = CreateDetailTableHeadHtml($("#ddlMonth").val());
                //表头
                $("#DSMonthReportSummaryHead").empty();
                $("#DSMonthReportSummaryHead").append(tableHead);
                $("#DSrows").empty();


                var DSDetailData = {};
                DSDetailData.ResultData = ResultData;
                DSDetailData.FinMonth = $("#ddlMonth").val();

                //数据绑定
                loadTmpl_1('#DSCompanyDetailTmpl').tmpl(DSDetailData).appendTo('#DSrows'); //加载数据
            }
        }
    });
    if (IsNewDataIndex.indexOf("F") < 0) {
        IsNewDataIndex = IsNewDataIndex + "F";
    }
}





//构建表头
function CreateDetailTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">项目</th><th rowspan=\"2\" style=\"width: 10%\">指标</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 40%;\" >" + month + "月(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 40%\">" + month + "月累计(万元)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\" style=\"width: 25%\" >计划</th><th class=\"th_Sub\" style=\"width: 25%\">实际</th><th class=\"th_Sub\" style=\"width: 25%\" >实际与<br/>计划差额</th><th class=\"th_Sub\" style=\"width: 25%\">实际占<br/>计划比</th>";
        strhtml += "     <th class=\"th_Sub\" style=\"width: 25%\">计划</th><th class=\"th_Sub\" style=\"width: 25%\">实际</th><th class=\"th_Sub\" style=\"width: 25%\" >实际与<br/>计划差额</th><th class=\"th_Sub\" style=\"width: 25%\">实际占<br/>计划比</th>";
        strhtml += " </tr>";
    }
    else {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 5%\">项目</th><th rowspan=\"2\" style=\"width: 7%\">指标</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 28%\">" + (month - 1) + "月(万元)</th><th colspan=\"4\" style=\"width: 28%;\" >" + month + "月(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 28%\">1-" + month + "月累计(万元)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与<br/>计划差额</th><th class=\"th_Sub\">实际占<br/>计划比</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与<br/>计划差额</th><th class=\"th_Sub\">实际占<br/>计划比</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与<br/>计划差额</th><th class=\"th_Sub\">实际占<br/>计划比</th>";
        strhtml += " </tr>";
    }
    return strhtml;
}
//-------------补回上月经营指标缺口情况------------------------------------------------------------------------------------------------------

function getDSMonthReportReturnData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetReturnDataList",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {
            var tableHead = CreateReturnTableHeadHtml($("#ddlMonth").val());
            $("#DSMonthReportSummaryHead").empty();
            $("#DSMonthReportSummaryHead").append(tableHead);

            var lastno = 0;
            var no = 0;
            if (ResultData.length > 0) {
                for (var a = 0; a < ResultData.length; a++) {
                    if (ResultData[a].IsAllReturn == true) {
                        lastno = lastno + 1;
                    }
                    else {
                        no = no + 1;
                    }
                }
            }

            var strTrueTbody = "";
            var strFalseTbody = "";
            if ($("#ddlMonth").val() == 1) {
                strTrueTbody += "<tr class=\"Level1\"><td colspan=\"11\" class=\"Td_Merge Level1TDBSL show\" onclick=\"TrLv1Show(this);\"\">一、完全补回上月指标缺口的商家<span style=\"color: red;\">（共" + lastno + "家）</span></td></tr>";
                strFalseTbody += "<tr class=\"Level1\"><td colspan=\"11\"class=\"Td_Merge Level2TDBSL show\" onclick=\"TrLv1Show(this);\" \">二、部分补回上月指标缺口的商家<span style=\"color: red;\">（共" + no + "家）</span></td></tr>";
            }
            else {
                strTrueTbody += "<tr class=\"Level1\"><td colspan=\"14\" class=\"Td_Merge Level1TDBSL show\" onclick=\"TrLv1Show(this);\"\" >一、完全补回上月指标缺口的商家<span style=\"color: red;\">（共" + lastno + "家）</span></td></tr>";
                strFalseTbody += "<tr class=\"Level1\"><td colspan=\"14\" class=\"Td_Merge Level2TDBSL show\" onclick=\"TrLv1Show(this);\"\">二、部分补回上月指标缺口的商家<span style=\"color: red;\">（共" + no + "家）</span></td></tr>";
            }
            $("#DSrows").empty();
            if (ResultData.length > 0) {
                var trueindex = 1;
                var falseIndex = 1;
                $(ResultData).each(function () {
                    if (this.IsAllReturn) {
                        var rcount = this.ReturnDataList.length;
                        var model = this;
                        $(this.ReturnDataList).each(function (i) {
                            lastno = lastno + 1;
                            if (i == 0) {
                                strTrueTbody += "<tr class=\"Level3\" style=\"display: none;\">";
                                strTrueTbody += "<td rowspan=\"" + rcount + "\" style=\"text-align: center !important; vertical-align: middle\"  class=\"Td_Left\">" + trueindex + "</td><td rowspan=\"" + rcount + "\" style=\"text-align: center !important; vertical-align: middle\"  class=\"Td_Left\">" + model.CompanyName + "</td>";
                                strTrueTbody += "<td  class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + this.ReturnTargetName + "</td><td  class=\"Td_Left\">" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strTrueTbody += "<td  class=\"Td_Left\">" + this.LastAccumulativePlan + "</td><td  class=\"Td_Left\">" + this.LastAccumulativeActual + "</td>";
                                    strTrueTbody += "<td  class=\"Td_Left\">" + this.LastAccumulativeDifference + "</td>";
                                }
                                strTrueTbody += "<td  class=\"Td_Left\">" + this.CurrentAccumulativePlan + "</td>";
                                strTrueTbody += "<td  class=\"Td_Left\">" + this.CurrentAccumulativeActual + "</td><td class=\"Td_Left\">" + this.CurrentAccumulativeDifference + "</td>";
                                strTrueTbody += "<td  class=\"Td_Left\">" + this.CurrentAccumulativeRate + "</td>";
                                if (this.CommitDate == "" || this.CommitDate == null) {
                                    strTrueTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strTrueTbody += "<td class=\"Td_Left\">" + this.CommitDate + "</td>";
                                }
                                strTrueTbody += "<td class=\"Td_Left\">" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strTrueTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strTrueTbody += "<td class=\"Td_Left\"><img src=\"../Images/images1/image" + this.Counter + ".png\" /></td>";
                                }
                                strTrueTbody += "</tr>";
                            }
                            else {
                                strTrueTbody += "<tr  class=\"Level3\" style=\"display: none;\">";
                                strTrueTbody += "<td class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + this.ReturnTargetName + "</td><td class=\"Td_Left\">" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strTrueTbody += "<td class=\"Td_Left\">" + this.LastAccumulativePlan + "</td><td class=\"Td_Left\">" + this.LastAccumulativeActual + "</td>";
                                    strTrueTbody += "<td class=\"Td_Left\">" + this.LastAccumulativeDifference + "</td>";
                                }
                                strTrueTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativePlan + "</td>";
                                strTrueTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeActual + "</td><td class=\"Td_Left\">" + this.CurrentAccumulativeDifference + "</td>";
                                strTrueTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeRate + "</td>";
                                if (this.CommitDate == "" || this.CommitDate == null) {
                                    strTrueTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strTrueTbody += "<td class=\"Td_Left\">" + this.CommitDate + "</td>";
                                }
                                strTrueTbody += "<td class=\"Td_Left\">" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strTrueTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strTrueTbody += "<td class=\"Td_Left\"><img src=\"../Images/images1/image" + this.Counter + ".png\" /></td>";
                                }
                                strTrueTbody += "</tr>";
                            }
                        })
                        trueindex++;
                    }
                    else {
                        var rcount = this.ReturnDataList.length;
                        var model = this;
                        $(this.ReturnDataList).each(function (i) {
                            if (i == 0) {
                                strFalseTbody += "<tr  class=\"Level3\" style=\"display: none;\">";
                                strFalseTbody += "<td class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\" rowspan=\"" + rcount + "\">" + falseIndex + "</td><td class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\" rowspan=\"" + rcount + "\" style=\"text-align:left;padding-left:10px; \">" + model.CompanyName + "</td>";
                                strFalseTbody += "<td  class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + this.ReturnTargetName + "</td><td class=\"Td_Left\">" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strFalseTbody += "<td class=\"Td_Left\">" + this.LastAccumulativePlan + "</td><td class=\"Td_Left\">" + this.LastAccumulativeActual + "</td>";
                                    strFalseTbody += "<td class=\"Td_Left\">" + this.LastAccumulativeDifference + "</td>";
                                }
                                strFalseTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativePlan + "</td>";
                                strFalseTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeActual + "</td><td class=\"Td_Left\">" + this.CurrentAccumulativeDifference + "</td>";
                                strFalseTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeRate + "</td>";
                                if (this.CommitDate == "" || this.CommitDate == null) {
                                    strFalseTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strFalseTbody += "<td class=\"Td_Left\">" + this.CommitDate + "</td>";
                                }
                                strFalseTbody += "<td class=\"Td_Left\">" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strFalseTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strFalseTbody += "<td class=\"Td_Left\"><img src=\"../Images/images1/image" + this.Counter + ".png\" /></td>";
                                }
                                strFalseTbody += "</tr>";
                            }
                            else {
                                strFalseTbody += "<tr  class=\"Level3\" style=\"display: none;\">";
                                strFalseTbody += "<td class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + this.ReturnTargetName + "</td><td class=\"Td_Left\">" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strFalseTbody += "<td class=\"Td_Left\">" + this.LastAccumulativePlan + "</td><td class=\"Td_Left\">" + this.LastAccumulativeActual + "</td>";
                                    strFalseTbody += "<td class=\"Td_Left\">" + this.LastAccumulativeDifference + "</td>";
                                }
                                strFalseTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativePlan + "</td>";
                                strFalseTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeActual + "</td><td class=\"Td_Left\">" + this.CurrentAccumulativeDifference + "</td>";
                                strFalseTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeRate + "</td>";
                                if (this.CommitDate == "" || this.CommitDate == null) {
                                    strFalseTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strFalseTbody += "<td class=\"Td_Left\">" + this.CommitDate + "</td>";
                                }
                                strFalseTbody += "<td class=\"Td_Left\">" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strFalseTbody += "<td class=\"Td_Left\">--</td>";
                                }
                                else {
                                    strFalseTbody += "<td class=\"Td_Left\"><img src=\"../Images/images1/image" + this.Counter + ".png\" /></td>";
                                }
                                strFalseTbody += "</tr>";
                            }
                        })
                        falseIndex++;
                    }
                })
                $("#DSrows").empty();
                $("#DSrows").append(strTrueTbody);
                $("#DSrows").append(strFalseTbody);
            }
        }
    });
    if (IsNewDataIndex.indexOf("G") < 0) {
        IsNewDataIndex = IsNewDataIndex + "G";
    }
}

//构建表头
function CreateReturnTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">公司名称</th><th rowspan=\"2\" style=\"width: 6%\">经营指标</th>";
        strhtml += "     <th rowspan=\"2\" style=\"width: 8%\">本月补回<br /> /新增差额</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 32%\">" + month + "月累计(万元)</th><th colspan=\"2\" style=\"width: 16%\">补回说明</th><th rowspan=\"2\" style=\"width: 4%\">警示灯</th></tr>";
        strhtml += "<tr>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">完成率</th>";
        strhtml += "     <th class=\"th_Sub\">要求期限</th><th class=\"th_Sub \">补回情况</th>";
        strhtml += " </tr>";
    }
    else {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">公司名称</th><th rowspan=\"2\" style=\"width: 6%\">经营指标</th>";
        strhtml += "     <th rowspan=\"2\" style=\"width: 8%\">本月补回<br /> /新增差额</th><th colspan=\"3\" style=\"width: 24%;\" >1-" + (month - 1) + "月累计(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 32%\">1-" + month + "月累计(万元)</th><th colspan=\"2\" style=\"width: 16%\">补回说明</th><th rowspan=\"2\" style=\"width: 4%\">警示灯</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">完成率</th>";
        strhtml += "     <th class=\"th_Sub\">要求期限</th><th class=\"th_Sub \">补回情况</th>";
        strhtml += " </tr>";
    }
    return strhtml;
}
//-------------百货系统经营指标新增未完成指标的门店情况--------------------------------------------------------------------------------------------
function getMonthReportAddMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //经营指标新增未完成指标的门店情况
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetAddMissDataList",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {
            var tableHead = CreateAddTableHeadHtml($("#ddlMonth").val());
            $("#DSMonthReportSummaryHead").empty();
            $("#DSMonthReportSummaryHead").append(tableHead);
            $("#DSrows").empty();
            if (ResultData.length > 0) {
                var tarList = new Array();
                var sameObjCount = 0;
                $(ResultData).each(function () {
                    //去除重复元素
                    if (tarList.length > 0) {
                        for (var a = 0; a < tarList.length; a++) {
                            if (this.AddTargetName == tarList[a].AddTargetName) {
                                sameObjCount++;
                            }
                        }
                        if (sameObjCount > 0) {
                            sameObjCount = 0;
                        }
                        else {
                            tarList.push(this);
                        }
                    }
                    else {
                        tarList.push(this);
                    }
                });
                //拼接html代码
                var TargetList = tarList;

                if (TargetList.length > 0) {
                    var strTbody = "";
                    $(TargetList).each(function (inde) {
                        var addtaron = 0;
                        for (var i = 0; i < ResultData.length; i++) {
                            if (ResultData[i].AddTargetName == this.AddTargetName) {
                                addtaron = addtaron + 1;
                            }
                        }
                        if ($("#ddlMonth").val() == 1) {
                            strTbody += "<tr class=\"Level1\"><td colspan=\"11\" onclick=\"TrLv1Show(this);\" class=\"Td_Merge Level1XTDSL show\"\">" + (inde + 1) + "、新增" + this.AddTargetName + "未完成门店<span style=\"color: red;\">（共" + addtaron + "家）</span></td></tr>";
                        }
                        else {
                            strTbody += "<tr class=\"Level1\"><td colspan=\"14\" onclick=\"TrLv1Show(this);\" class=\"Td_Merge Level1XTDSL show\"\">" + (inde + 1) + "、新增" + this.AddTargetName + "未完成门店<span style=\"color: red;\">（共" + addtaron + "家）</span></td></tr>";
                        }
                        var TargetModel = this;
                        var index = 1;
                        $(ResultData).each(function () {
                            if (this.AddTargetName == TargetModel.AddTargetName) {
                                var model = this;

                                var rowspan_Temp = this.ReturnDataList.length;
                                $(this.ReturnDataList).each(function (i) {
                                    if (i == 0) {
                                        strTbody += "<tr class=\"Level3\" style=\"display: none;\">";
                                        strTbody += "<td rowspan=\"" + rowspan_Temp + "\" class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + index + "</td><td rowspan=\"" + rowspan_Temp + "\" class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + model.CompanyName + "</td>";
                                        strTbody += "<td class=\"Td_Left\"  style=\"text-align: center !important; vertical-align: middle\">" + this.ReturnTargetName + "</td><td class=\"Td_Left\">" + this.CurrentReturnAmount + "</td>";
                                        if ($("#ddlMonth").val() > 1) {
                                            strTbody += "<td class=\"Td_Left\">" + this.LastAccumulativePlan + "</td><td class=\"Td_Left\">" + this.LastAccumulativeActual + "</td>";
                                            strTbody += "<td class=\"Td_Left\">" + this.LastAccumulativeDifference + "</td>";
                                        }
                                        strTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativePlan + "</td>";
                                        strTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeActual + "</td><td class=\"Td_Left\">" + this.CurrentAccumulativeDifference + "</td>";
                                        strTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeRate + "</td><td class=\"Td_Left\">" + this.CommitDate + "</td>";
                                        strTbody += "<td class=\"Td_Left\">" + this.ReturnTypeDescrible + "</td>";
                                        if (this.Counter == 0) {
                                            strTbody += "<td class=\"Td_Left\">--</td>";
                                        }
                                        else {
                                            strTbody += "<td class=\"Td_Left\"><img src=\"../Images/images1/image" + this.Counter + ".png\" /></td>";
                                        }
                                        strTbody += "</tr>";
                                    }
                                    else {
                                        strTbody += "<tr class=\"Level3\" style=\"display: none;\">";
                                        strTbody += "<td class=\"Td_Left\"  style=\"text-align: center !important; vertical-align: middle\">" + this.ReturnTargetName + "</td><td class=\"Td_Left\">" + this.CurrentReturnAmount + "</td>";
                                        if ($("#ddlMonth").val() > 1) {
                                            strTbody += "<td class=\"Td_Left\">" + this.LastAccumulativePlan + "</td><td class=\"Td_Left\">" + this.LastAccumulativeActual + "</td>";
                                            strTbody += "<td class=\"Td_Left\">" + this.LastAccumulativeDifference + "</td>";
                                        }
                                        strTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativePlan + "</td>";
                                        strTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeActual + "</td><td class=\"Td_Left\">" + this.CurrentAccumulativeDifference + "</td>";
                                        strTbody += "<td class=\"Td_Left\">" + this.CurrentAccumulativeRate + "</td><td class=\"Td_Left\">" + this.CommitDate + "</td>";
                                        strTbody += "<td class=\"Td_Left\">" + this.ReturnTypeDescrible + "</td>";
                                        if (this.Counter == 0) {
                                            strTbody += "<td class=\"Td_Left\">--</td>";
                                        }
                                        else {
                                            strTbody += "<td class=\"Td_Left\"><img src=\"../Images/images1/image" + this.Counter + ".png\" /></td>";
                                        }
                                        strTbody += "</tr>";
                                    }
                                })
                                index++;
                            }
                        })
                    })
                    $("#DSrows").empty();
                    $("#DSrows").append(strTbody);
                }
            }
        }
    });
    if (IsNewDataIndex.indexOf("H") < 0) {
        IsNewDataIndex = IsNewDataIndex + "H";
    }
}

//构建表头
function CreateAddTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">公司名称</th><th rowspan=\"2\" style=\"width: 6%\">经营指标</th>";
        strhtml += "     <th rowspan=\"2\" style=\"width: 8%\">本月补回<br /> /新增差额</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 32%\">" + month + "月累计(万元)</th><th colspan=\"2\" style=\"width: 16%\">补回说明</th><th rowspan=\"2\" style=\"width: 4%\">警示灯</th></tr>";
        strhtml += "<tr>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">完成率</th>";
        strhtml += "     <th class=\"th_Sub\">要求期限</th><th class=\"th_Sub \">补回情况</th>";
        strhtml += " </tr>";
    }
    else {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">公司名称</th><th rowspan=\"2\" style=\"width: 6%\">经营指标</th>";
        strhtml += "     <th rowspan=\"2\" style=\"width: 8%\">本月补回<br /> /新增差额</th><th colspan=\"3\" style=\"width: 24%;\" >1-" + (month - 1) + "月累计(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 32%\">1-" + month + "月累计(万元)</th><th colspan=\"2\" style=\"width: 16%\">补回说明</th><th rowspan=\"2\" style=\"width: 4%\">警示灯</th></tr>";
        strhtml += "<tr><th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">完成率</th>";
        strhtml += "     <th class=\"th_Sub\">要求期限</th><th class=\"th_Sub \">补回情况</th>";
        strhtml += " </tr>";
    }
    return strhtml;
}

function trim(str) {
    return str.replace(/^(\s|\u00A0)+/, '').replace(/(\s|\u00A0)+$/, '');
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





//去除换行，空格
function _TrimStr(obj)
{
    var str;
    str = obj.replace(new RegExp('^(\s*\\n)*|(\\n\s*)*$'),'');
    ws = /\n/,
    i = str.length;
    while (ws.test(str.charAt(--i)));
    return str.slice(0, i + 1);
}


//判断是否是12月31日，以便前面判断是否隐藏
function IsTimeShow(obj)
{
    try {
        var date = new Date(obj);
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();

        if (month == 12 && day == 31) {
            return true;
        } else
        {
            return false
        }
    } catch (e) {
        return false;
    }
}
//获取月份
function GetMonthStr() {
    return $("#ddlMonth").val()
}
//完成情况明细点击警示灯展开全部列
function unfoldTitle() {
    var liSelect = $("#Ul3 .selected");
    var tipName = "";
    if (liSelect.length > 0) {
        tipName = liSelect.find("span").text();
    }
    if (tipName == "经营报告明细") {
        $("#CompleteDetailHead_1").html("");
        $("#tab2_rows_1").html("");
        var targetName = $("#Ul4_1 .active_sub3")[0] == undefined ? "" : $("#Ul4_1 .active_sub3")[0].innerText;
        var TemplData = {};
        $.each(ManageReportDetailData, function (i, item) {
            if (item.Name == targetName) {
                TemplData = item;
                return;
            }
        });

        //加载表头
        loadTmpl('#TmplCompleteDetail_Head_All').tmpl(TemplData).appendTo('#CompleteDetailHead_1'); //加载列头 

        var dataArray = [];
        var data = { "data": TemplData };
        dataArray.push(data);

        loadTmpl('#TmplManageTargetDetail_Data_All').tmpl(dataArray).appendTo('#tab2_rows_1');
        $("#importedDataTable2_1").css("width", "110%");

        var obj = $("#CompleteDetailHead_1");
        var tab = $("#tab2_rows_1");
        FloatHeader(obj, tab, false, "MonthRpt");
        AddBackGroundColor();
    }
    else if (tipName == "完成情况明细"){
        $("#CompleteDetailHead").html("");
        $("#tab2_rows").html("");
        var targetName = $("#Ul4 .active_sub3")[0] == undefined ? "" : $("#Ul4 .active_sub3")[0].innerText;
        var TemplData = {};
        $.each(ComplateDetailData, function (i, item) {
            if (item.Name == targetName) {
                TemplData = item;
                return;
            }
        });

        //加载表头
        loadTmpl('#TmplCompleteDetail_Head_All').tmpl(TemplData).appendTo('#CompleteDetailHead'); //加载列头 

        var dataArray = [];
        var data = { "data": TemplData };
        dataArray.push(data);

        loadTmpl('#TmplCompleteDetail_Data_All').tmpl(dataArray).appendTo('#tab2_rows');
        $("#importedDataTable2").css("width", "110%");

        var obj = $("#CompleteDetailHead");
        var tab = $("#tab2_rows");
        FloatHeader(obj, tab, false, "MonthRpt");
        AddBackGroundColor();
    }
  
}
//完成情况明细点击警示灯收缩列
function shrinkageTitle() {
    var liSelect = $("#Ul3 .selected");
    var tipName = "";
    if (liSelect.length > 0) {
        tipName = liSelect.find("span").text();
    }
    if (tipName == "经营报告明细") {
        $("#CompleteDetailHead_1").html("");
        $("#tab2_rows_1").html("");

        var targetName = $("#Ul4_1 .active_sub3")[0] == undefined ? "" : $("#Ul4_1 .active_sub3")[0].innerText;
        var TemplData = {};
        $.each(ManageReportDetailData, function (i, item) {
            if (item.Name == targetName) {
                TemplData = item;
                return;
            }
        });
        //加载表头
        loadTmpl('#TmplCompleteDetail_Head').tmpl(TemplData).appendTo('#CompleteDetailHead_1'); //加载列头 

        var dataArray = [];
        var data = { "data": TemplData };
        dataArray.push(data);

        loadTmpl('#TmplManageTargetDetail_Data').tmpl(dataArray).appendTo('#tab2_rows_1');

        $("#importedDataTable2_1").css("width", "100%");
        var obj = $("#CompleteDetailHead_1");
        var tab = $("#tab2_rows_1");
        FloatHeader(obj, tab, false, "MonthRpt");
        AddBackGroundColor();
    }
    else if (tipName == "完成情况明细") {
        $("#CompleteDetailHead").html("");
        $("#tab2_rows").html("");

        var targetName = $("#Ul4 .active_sub3")[0] == undefined ? "" : $("#Ul4 .active_sub3")[0].innerText;
        var TemplData = {};
        $.each(ComplateDetailData, function (i, item) {
            if (item.Name == targetName) {
                TemplData = item;
                return;
            }
        });
        //加载表头
        loadTmpl('#TmplCompleteDetail_Head').tmpl(TemplData).appendTo('#CompleteDetailHead'); //加载列头 

        var dataArray = [];
        var data = { "data": TemplData };
        dataArray.push(data);

        loadTmpl('#TmplCompleteDetail_Data').tmpl(dataArray).appendTo('#tab2_rows');

        $("#importedDataTable2").css("width", "100%");
        var obj = $("#CompleteDetailHead");
        var tab = $("#tab2_rows");
        FloatHeader(obj, tab, false, "MonthRpt");
        AddBackGroundColor();
    }
}
function AddBackGroundColor() {
    if (MonthReportOrderType == "Detail") {
        $(".DetailMonthly").attr("src", "../Images/btn_down02_w.png");
        $(".Detail").attr("src", "../Images/btn_down03_w.png");
        $(".DetailMonthlyCss").addClass("tabOrderBackground");
    } else {
        $(".DetailMonthly").attr("src", "../Images/btn_down03_w.png");
        $(".Detail").attr("src", "../Images/btn_down02_w.png");
        $(".DetailCss").addClass("tabOrderBackground");

    }
}
function GetUnit() {
    return "万元";
}