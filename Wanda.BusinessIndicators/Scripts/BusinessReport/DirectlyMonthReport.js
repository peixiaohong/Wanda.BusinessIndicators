
//变量误删
var ReturnData = {};
var MissTargetData = {};
var CurrentMissTargetData = {};
var ComplateDetailData = {};
var MonthReportData = {};
var MonthReportActionData = {};


var Year;
var Month;
var SystemID;
var TargetPlanID;
var IsLatestVersion;
var IsNewDataIndex = "";
var MonthReportOrderType = "Detail";
var IncludeHaveDetail = false;
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
    MonthReportOrderType = "Detail";
    currentDetailTarget = null;

}

//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetDirectlyRpt.html", selector);
}

//加载模版项-------------------------------------------------------------------------
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/MissTargerDirectlyRpt.html", selector);
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
        }else
        {
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
    $("#sitmap").html("您当前所在的位置：月度经营报告");
    $("#jMenu").find("li").each(function () {
        var text = $(this).find("span")[0];
        $(this).removeClass("current first");
        if (text && text.innerHTML == "月度经营报告") {
            $(this).addClass("current first");
        }
    })

    TargetPlanID = $("#ddlVersionType").val();
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

    FileChangeClick();

});

function f_search() {
    IsNewDataIndex = "";
    ChangeTargetDetail($(".defaultTarget"), "Tab");
}

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
        $('#T2,#T3,#T3_1,#T4,#T5').hide();

        //月度经营报告
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }
    }
    else if ($(sender).text() == "完成情况明细" && TabOrSearch != "Search") {

        $('#T1,#T3,#T3_1,#T4,#MonthReportExplainDiv,#ApproveAttachDiv,#T5').hide();
        $('#T2,#DownExcel').show();
        //var obj = $("#CompleteDetailHead");
        //var tab = $("#tab2_rows");
        //FloatHeader(obj, tab, false, "MonthRpt");

        //完成情况明细
        GetMonthReportDetailSearchCondition();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "B") == true) {
            getMonthReprotDetailData();
        }
    }
    else if ($(sender).text() == "当月未完成" && TabOrSearch != "Search") {
        $("#T4,#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T5").hide();
        $("#T3_1,#DownExcel").show();

        //var obj = $("#Tab_MissTargetHead");
        //var tab = $("#Tbody_MissTargetData");
        //FloatHeader(obj, tab, false, "MonthRpt");
        //未完成说明
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, CurrentMissTargetData[0], "F") == true) {
            getCurrentMonthReportMissTargetData();
        }

    }
    else if ($(sender).text() == "累计未完成" && TabOrSearch != "Search") {
        $("#T4,#T1,#T2,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5").hide();
        $("#T3,#DownExcel").show();

        //var obj = $("#Tab_MissTargetHead");
        //var tab = $("#Tbody_MissTargetData");
        //FloatHeader(obj, tab, false, "MonthRpt");
        //未完成说明
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MissTargetData[0], "C") == true) {
            getMonthReportMissTargetData();
        }

    }
    else if ($(sender).text() == "补回情况明细" && TabOrSearch != "Search") {
        $("#T4,#DownExcel").show();
        $("#T1,#T2,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5").hide();

        //$("#Tab_Return").removeAttr("style");
        //var obj = $("#Tab_ReturnHead");
        //var tab = $("#Tbody_Data");


        //补回情况明细
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "D") == true) {
            getMonthReportReturnData();
        }
        // FloatHeader(obj, tab, 2);
    }
    else if ($(sender).text() == "上报日志" && TabOrSearch != "Search") {
        $("#T4,#T1,#T2,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T3,#DownExcel").hide();
        $("#T5").show();

        var obj = $("#ActionHead");
        var tab = $("#Action_Row");
        //FloatHeader(obj, tab);
        //未完成说明
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportActionData[0], "E") == true) {
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
    //if (TCYear == Year && TCMonth == Month && TCSystemID == SystemID && TCIsLatestVersion == IsLatestVersion) {
    //    return false;
    //}
    else {
        return true;
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
            return obj[0].TargetGroupCount;
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
    //GetMonthReportID(resultData);
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
                loadTmpl('#MonthReportSummaryHeadTemplate_DZSW').tmpl().appendTo('#MonthReportSummaryHead');

            }

            $('#rows').empty();
            if (strSummaryTabel[1] != "" && strSummaryTabel[1] != undefined) {
                loadTmpl('#' + strSummaryTabel[1]).tmpl(resultData[2].ObjValue).appendTo('#rows');
            } else {
                loadTmpl('#MonthReportSummaryTemplate_0').tmpl(resultData[2].ObjValue).appendTo('#rows');
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
}
if (IsNewDataIndex.indexOf("E") < 0) {
    IsNewDataIndex = IsNewDataIndex + "E";
}



function getMonthReportSummaryData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
        IsLatestVersion = latest;
    }

    //这里判断数据源是草稿状态，还是审批状态的
    var dataSource = "Progress";
    //草稿状态下数据不显示
    //if ($("#submana").is(":visible")) {
    //    dataSource = "Progress";
    //}

    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetReportInstance",
        args: { SystemID: $("#ddlSystem").val(), Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), TargetPlanID: $("#ddlVersionType").val(), IsLatestVersion: latest, DataSource: dataSource, IsAll: true },
        successReturn: SplitData
    });
    if (IsNewDataIndex.indexOf("A") < 0) {
        IsNewDataIndex = IsNewDataIndex + "A";
    }
}

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
            //先影藏其它标签
            // $("#T3").hide();

            //首先指标先加载
            $("#U1").empty();
            $("#Tbody_Data").empty();
            $("#Tab_ReturnHead").empty();

            loadTmpl_1('#TmplHeadReturn').tmpl().appendTo('#Tab_ReturnHead');  //加载裂头
            if (ReturnData.length == 0) {
                $(".newdiff_retu").show();
                $(".Level1TDSL").attr("colspan", 9);
                $('#Tab_ReturnHead tr th').eq(11).hide();
                return;
            }

            if (ReturnData.length > 1) {  //判断指标有几个分组，如果是2个一上默认选择第一个
                loadTmpl_1('#TmplTargerList').tmpl(ReturnData).appendTo('#U1');
                loadTmpl_1('#TmplTargetRetu_SG').tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                $(".newdiff_retu").hide();
                $(".Level1TDSL").attr("colspan", 8);
            } else {
                loadTmpl_1('#TmplTargetRetu').tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                $(".newdiff_retu").show();
                $(".Level1TDSL").attr("colspan", 9);
            }

            $("#U1 :first a").addClass("active_sub3");

            $(".shangyueleiji").hide();

            var obj = $("#Tab_FloatReturn");
            var head = $("#Tab_ReturnHead");
            obj.find("thead").html(head.html());
            var tab = $("#Tbody_Data");
            FloatHeader(obj, tab);

            //$("#Tab_Return").attr({ style: "table-layout: fixed" });
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
    loadTmpl_1('#TmplTargetRetu_SG').tmpl(m).appendTo('#Tbody_Data');

    //显示影藏
    $(".shangyueleiji").hide();
    $(".Level1TDSL").attr("colspan", 8);
    $(".TTR2").attr("colspan", 3);
    $(".Special_return").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当出现完成率的时候，差值TD是没有右面的边线的

    $('#CurrentMonthBackDetilDiv').text("本月累计(万元) [+]");

}


//未完成 tab（未完成说明  累计的 ）--------------------------------------------------------------------------------------------
function getMonthReportMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //未完成说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetMissTargetList",
        args: { rpts: WebUtil.jsonToString(ReportInstance) },
        successReturn: function (result) {

            MissTargetData = result;

            var string = WebUtil.jsonToString(MissTargetData);
            //先影藏其它标签
            //首先指标先加载
            $("#U2").empty();
            $("#Tbody_MissTargetData").empty();
            $("#Tab_MissTargetHead").empty();

            var multitarget = false;

            loadTmpl_1('#TmplHeadMiss').tmpl().appendTo('#Tab_MissTargetHead'); //加载列头

            //判断指标有几个分组，如果是2个一上默认选择第一个
            if (MissTargetData.length > 1) {

                loadTmpl_1('#TmplMissTarget_SG').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                loadTmpl_1('#TmplMissTargerList').tmpl(MissTargetData).appendTo('#U2');
                $(".newdiff_miss").hide();
                $(".Level1TdSp1").attr("colspan", 10);
                multitarget = true;

            } else {
                //单个指标的时候
                loadTmpl_1('#TmplMissTarget').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                $(".newdiff_miss").show();
                $(".Level1TdSp1").attr("colspan", 11);

            }
            $("#U2 :first a").addClass("active_sub3");

            //var obj = $("#Tab_MissTargetHead");
            //var tab = $("#Tbody_MissTargetData");


            var obj = $("#Tab_MissFloatTarget");
            var head = $("#Tab_MissTargetHead");
            obj.find("thead").html(head.html());
            var tab = $("#Tbody_MissTargetData");
            FloatHeader(obj, tab);

            $(".shangyue").hide();
            $("#Tab_MissTarget").attr({ style: "table-layout: auto" });
            $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");

        }
    });
    if (IsNewDataIndex.indexOf("C") < 0) {
        IsNewDataIndex = IsNewDataIndex + "C";
    }
}

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

    loadTmpl_1('#TmplMissTarget_SG').tmpl(m).appendTo('#Tbody_MissTargetData');

    //显示影藏
    $(".shangyue").hide();
    $(".Level1TdSp1").attr("colspan", 10);
    $(".TT2").attr("colspan", 3);
    $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
    //$("#Tab_MissTarget").attr({ style: "table-layout: fixed" });
    $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");
}


//收缩 累计
function ShouSuo(sender) {

    //未完成
    if (sender == "YC") {
        $(".shangyue").hide();
        $(".TT2").attr("colspan", 3);
        $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
        $(".Level1TdSp1").attr("colspan", 10);
        $("#Table1").removeAttr("style");
        $('#CurrentMonthMissTergetDiv').text("本月累计(万元)  [+]");
    } else if (sender == 'XS') {

        $(".shangyue").show();
        $(".TT2").attr("colspan", 4);
        $(".Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
        $(".Level1TdSp1").attr("colspan", 15);
        $('#CurrentMonthMissTergetDiv').text("本月累计(万元)");

    } else if (sender == 'YCSY') {
        ////补回说明
        $(".shangyueleiji").hide();
        $(".TTR2").attr("colspan", 3);
        $(".Special_return").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的

        $(".Level1TDSL").attr("colspan", 9);
        $("#Tab_Return").removeAttr("style");
        $('#CurrentMonthBackDetilDiv').text("本月累计(万元)  [+]");

    } else if (sender == 'XSSY') {

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
    }

}


//未完成 tab（未完成说明  当月的 ）--------------------------------------------------------------------------------------------
function getCurrentMonthReportMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //未完成说明
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetCurrentMissTargetList",
        args: { rpts: WebUtil.jsonToString(ReportInstance) },
        successReturn: function (result) {

            CurrentMissTargetData = result;
           
            //先影藏其它标签
            //首先指标先加载
            $("#U2_1").empty();
            $("#Tbody_CurrentMissTargetData").empty();
            $("#Tab_CurrentMissTargetHead").empty();

            var multitarget = false;

            loadTmpl_1('#TmplCurrentHeadMiss').tmpl().appendTo('#Tab_CurrentMissTargetHead'); //加载列头

            //判断指标有几个分组，如果是2个一上默认选择第一个
            if (CurrentMissTargetData.length > 1) {

                loadTmpl_1('#TmplMissTarget_SG').tmpl(CurrentMissTargetData[0]).appendTo('#Tbody_CurrentMissTargetData');
                loadTmpl_1('#TmplCurrentMissTargerList').tmpl(CurrentMissTargetData).appendTo('#U2');
                $(".newdiff_CurrenMiss").hide();
                $(".Curr_Level1TdSp1").attr("colspan", 10);
                multitarget = true;

            } else {
                //单个指标的时候
                loadTmpl_1('#TmplCurrentMissTarget_Directly').tmpl(CurrentMissTargetData[0]).appendTo("#Tbody_CurrentMissTargetData");
                $(".newdiff_CurrenMiss").show();
                $(".Curr_Level1TdSp1").attr("colspan", 11);

            }
            $("#U2_1 :first a").addClass("active_sub3");

            //var obj = $("#Tab_CurrentMissTargetHead");
            //var tab = $("#Tbody_CurrentMissTargetData");

            var obj = $("#Tab_CurrentMissFloatTarget");
            var head = $("#Tab_CurrentMissTargetHead");
            obj.find("thead").html(head.html());
            var tab = $("#Tbody_CurrentMissTargetData");
            FloatHeader(obj, tab);

            $(".leiji").hide();
            $("#Tab_CurrentMissTarget").attr({ style: "table-layout: auto" });
            //$('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");

        }
    });

    //if (IsNewDataIndex.indexOf("C") < 0) {
    //    IsNewDataIndex = IsNewDataIndex + "C";
    //}
}




////收缩 当月未完成
function ShouSuo_Current(sender) {

    if (sender == "YC") {
        $(".leiji").hide();
        $(".C_TT2").attr("colspan", 3);
        $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
        $(".Curr_Level1TdSp1").attr("colspan", 10);
        $("#Table1").removeAttr("style");
        $('#MonthMissTergetDiv_Current').text("当月发生(万元) [+]");
    } else {

        if ($(".leiji").is(":hidden")) {
            $(".leiji").show();
            $(".C_TT2").attr("colspan", 4);
            $(".Curr_Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
            $(".Curr_Level1TdSp1").attr("colspan", 15);
            $('#MonthMissTergetDiv_Current').text("当月发生(万元) [-]");
        } else {
            $(".leiji").hide();
            $(".C_TT2").attr("colspan", 3);
            $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
            $(".Curr_Level1TdSp1").attr("colspan", 10);
            $("#Table1").removeAttr("style");
            $('#MonthMissTergetDiv_Current').text("当月发生(万元) [+]");
        }
    }

}


//区分下载报表
function DownExcelReport(sender) {
    SystemID = $("#ddlSystem").val();
    Year = $("#ddlYear").val();
    Month = $("#ddlMonth").val();

    //这里判断数据源是草稿状态，还是审批状态的
    var dataSource = "Progress";

    //不显示草稿状态下数据
    //if ($("#submana").is(":visible")) {
    //    dataSource = "Progress";
    //}


    if ($(sender).text().indexOf("月度经营报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("累计未完成") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=MissTarget&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("回情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetReturn&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    }

}

//打包下载报表
function DownExcelReportList(sender) {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var FinMonth = $("#ddlMonth").val();
    var TargetPlanID = $("#ddlVersionType").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }

    //这里数据源是审批状态的
    var dataSource = "Progress";

    window.open("/AjaxHander/DownMonthRptFileList.ashx?SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource + "&IsAll=true&TargetPlanID=" + TargetPlanID);
}

//用正则表达式获取URL参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
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
        rowSpanVal = $("." + val).attr("rowspan");
        if (rowSpanVal == undefined)
            rowSpanVal = 0;
        $("." + val).attr("rowspan", (rowSpanVal * 1 - $(sender).nextUntil(".item1").length));
    }

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
function _TrimStr(obj) {
    var str;
    str = obj.replace(new RegExp('^(\s*\\n)*|(\\n\s*)*$'), '');
    ws = /\n/,
        i = str.length;
    while (ws.test(str.charAt(--i)));
    return str.slice(0, i + 1);
}


//判断是否是12月31日，以便前面判断是否隐藏
function IsTimeShow(obj) {
    try {
        var date = new Date(obj);
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();

        if (month == 12 && day == 31) {
            return true;
        } else {
            return false
        }
    } catch (e) {
        return false;
    }
}
//搜索条件年份、月份、是否包含审批中添加切换版本类型事件
function FileChangeClick() {
    $("#ddlYear").change(function () {
        CommonGetTargetVersionType($("#ddlSystem").val(), $("#ddlYear").val(), $("#ddlMonth").val())
    });
    $("#ddlMonth").change(function () {
        CommonGetTargetVersionType($("#ddlSystem").val(), $("#ddlYear").val(), $("#ddlMonth").val())
    });
}
function CommonGetTargetVersionType(sid, y, m) {
    WebUtil.ajax({
        async: true,
        url: "/MonthlyReportController/GetTargetVersionType",
        args: { SystemID: sid, FinYear: y, FinMonth: m},
        successReturn: function (result) {
            $("#ddlVersionType").empty();
            if (result != undefined && result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    $("#ddlVersionType").append("<option value='" + result[i].ID + "'>" + result[i].VersionName + "</option>");
                }
            }
        }
    });
}
