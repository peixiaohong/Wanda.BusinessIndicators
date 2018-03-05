
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
//查询事件
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

//加载模版项-------------------------------------------------------------------------
//详细页面模版
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetProRpt.html", selector);
}

//日志模版
function loadActionTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/MonthlyReportActionTmpl.html", selector);
}

//未完成模版
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/ProTargetReturnTmpl.html", selector);
}

//获取月份
function GetMonthStr() {
    return $("#ddlMonth").val();
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


    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    IsLatestVersion = latest;

    $(".DownExcelDiv").hide();//上来就隐藏
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
        $('#T1,#ApproveAttachDiv,#MonthReportExplainDiv,#T5').show();
        $('#T2,#T3,#T4,#T3_1,.DownExcelDiv,#T5').hide();

        //月度经营报告
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }
    } else

        if ($(sender).text() == "完成情况明细" && TabOrSearch != "Search") {

            $('#T1,#T3,#T4,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5').hide();
            $('#T2,.DownExcelDiv').show();
            var obj = $("#CompleteDetailHead");
            var tab = $("#tab2_rows");
            FloatHeader(obj, tab);

            //完成情况明细
            //GetMonthReportDetailSearchCondition(); 

            if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "B") == true) {
                getMonthReprotDetailData();
            }


        }
        else if ($(sender).text() == "当月未完成" && TabOrSearch != "Search") {
            $("#T4,#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv,#T5").hide();
            $("#T3_1,.DownExcelDiv,#DownExcel").show();

            var obj = $("#Tab_MissTargetHead");
            var tab = $("#Tbody_MissTargetData");

            //未完成说明
            if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, CurrentMissTargetData[0], "F") == true) {
                getCurrentMonthReportMissTargetData();
            }
            FloatHeader(obj, tab, false, "MonthRpt");

        }
        else if ($(sender).text() == "累计未完成" && TabOrSearch != "Search") {
            $("#T4,#T1,#T2,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5").hide();
            $("#T3,.DownExcelDiv,#DownExcel").show();

            var obj = $("#Tab_MissTargetHead");
            var tab = $("#Tbody_MissTargetData");

            //未完成说明
            if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MissTargetData[0], "C") == true) {
                getMonthReportMissTargetData();
            }
            FloatHeader(obj, tab, false, "MonthRpt");

        }
        else if ($(sender).text() == "补回情况明细" && TabOrSearch != "Search") {
            $("#T4,.DownExcelDiv,#DownExcel").show();
            $("#T1,#T2,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T5").hide();

            //$("#Tab_Return").removeAttr("style");
            var obj = $("#Tab_ReturnHead");
            var tab = $("#Tbody_Data");


            //补回情况明细
            if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "D") == true) {
                getMonthReportReturnData();
            }
            FloatHeader(obj, tab, false, "MonthRpt");
        }
        else if ($(sender).text() == "上报日志" && TabOrSearch != "Search") {
            $("#T4,#T1,#T2,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv,#T3,#DownExcel").hide();
            $("#T5,").show();

            var obj = $("#ActionHead");
            var tab = $("#Action_Row");


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
    if ($(sender).text() != "完成情况明细") {
        $('#imgtableUpDown,#SearchPanel').hide();
        $('#imgtableUpDown').attr("src", "../Images/images1/Down.png");
    } else {
        $('#imgtableUpDown').show();
    }
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


//切换不同的标签
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

    if (IsNewDataIndex.indexOf("E") < 0) {
        IsNewDataIndex = IsNewDataIndex + "E";
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

//用正则表达式获取URL参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
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

    // GetMonthReportID(resultData);//获取MonthReportID
    MonthReportData = resultData
    if (resultData) {
        ReportInstance = resultData[0].ObjValue;
        var strSummaryTabel;
        if (resultData[1] != null) { //月报说明
            $("#txtDes").html("");
            var strTemp = resultData[1].ObjValue;
            strTemp = strTemp.replace(/\n/g, "<br/>").replace(/ /g, "&nbsp;");
            $("#txtDes").html(strTemp);
        }

        if (resultData[3] != null) { //附件
            var lstAtt = {};
            lstAtt = resultData[3].ObjValue;
            if (lstAtt != null && lstAtt.length > 0) {
                $('#listAttDiv').empty();
                loadTmpl('#listAtt').tmpl(lstAtt).appendTo('#listAttDiv');
                $("#listAttDiv span:last-child").css({ display: "none" });
            }
        }
        if (resultData[4] != null) { //查询条件
            var SearchList = {};
            SearchList = resultData[4].ObjValue;
            $('#SearchPanel').empty();
            loadTmpl('#MonthlyReportDetailSearhTemplate').tmpl(SearchList).appendTo('#SearchPanel');

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



//月度报告
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

    ShowUpdateDetail();
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
        args: { rpts: WebUtil.jsonToString(ReportInstance), strCompanyProperty: CompanyProperty, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: latest },
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

//获取属性信息
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
                CompanyProperty += "CompanyProperty1" + $("#hid_CompanyProperty1").val() + ";";
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
                CompanyProperty += "CompanyProperty3" + $("#hid_CompanyProperty3").val() + ";";
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
                CompanyProperty += "CompanyProperty4" + $("#hid_CompanyProperty4").val() + ";";
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
                CompanyProperty += "CompanyProperty5" + $("#hid_CompanyProperty5").val() + ";";
        }
    }
}

//属性筛选
function ClickCompanyProperty(sender) {

    //判断数据是否存在
    if (ComplateDetailData[0].ObjValue.length > 4) {
        //首先奖得到的总数据 赋值到一个临时变量里
        var _tempData = JSON.parse(JSON.stringify(ComplateDetailData[0]))

        var m = [];
        if ($(sender).attr("checked") != "checked") {
            //勾选了筛选条件
            $.each(_tempData.ObjValue, function (n, obj) {

                if (obj.ProCompanySequence > 0) {

                    if (obj.ProCompayID != "88888888-8888-8888-8888-888888888888") //排除小计
                    {
                        if (obj.CompayModel.CompanyProperty1 != $(sender).val()) {
                            m.push(obj);
                        }
                    } else { m.push(obj); }

                } else {
                    m.push(obj);
                }
            });


            _tempData.ObjValue = m;
            //重新加载数据
            TargetDetail(_tempData);
        } else {
            //重新加载数据
            TargetDetail(ComplateDetailData[0]);
        }

    }

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

    strComplateMonthReportDetilHtmlTemplate = new Array();
    if (TemplData.HtmlTemplate != undefined) {
        strComplateMonthReportDetilHtmlTemplate = TemplData.HtmlTemplate.split(',');
    }
    //tmpl模板名称
    if (strComplateMonthReportDetilHtmlTemplate[1] != "" && strComplateMonthReportDetilHtmlTemplate[1] != undefined) {
        ComplateTargetDetailTemplate = strComplateMonthReportDetilHtmlTemplate[1];
    } else {
        ComplateTargetDetailTemplate = "ComplateTargetDetailTemplate"
    }


    $("#tab2_rows").empty();
    loadTmpl('#' + ComplateTargetDetailTemplate).tmpl(TemplData).appendTo('#tab2_rows');
    SetComplateTargetDetailData(TemplData, 2);

}

var ComplateTargetDetailTemplate = null;


//加载数据和表头
function SetComplateTargetDetailData(sender, Type) {

    TargetDetail(sender)

}

//加载表头和数据
function TargetDetail(sender) {
    $("#CompleteDetailHead").empty();
    $("#tab2_rows").empty();

    //加载表头
    loadTmpl("#TmplProCompany_Head").tmpl().appendTo("#CompleteDetailHead"); //加载列头 

    if (sender.ObjValue.length > 4) {
        loadTmpl("#TmplProCompanyDetail_Data").tmpl(sender).appendTo("#tab2_rows");
    }
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
var currentDetailTarget = null;
function MonthReportOrder(sender) {
    currentDetailTarget = $("#Ul4 li .active_sub3 ");
    MonthReportOrderType = sender;
    getMonthReprotDetailData();

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
                $('#Tab_ReturnHead tr th').eq(14).hide();
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


//累计未完成 tab（未完成说明）--------------------------------------------------------------------------------------------
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

            var string = WebUtil.jsonToString(MissTargetData);
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


//当月未完成 tab（未完成说明）
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


            //判断指标有几个分组，如果是2个一上默认选择第一个
            if (CurrentMissTargetData.length > 1) {

                loadTmpl_1('#TmplMissTarget_SG').tmpl(CurrentMissTargetData[0]).appendTo('#Tbody_CurrentMissTargetData');
                loadTmpl_1('#TmplCurrentMissTargerList').tmpl(CurrentMissTargetData).appendTo('#U2');
                $(".newdiff_CurrenMiss").hide();
                $(".Curr_Level1TdSp1").attr("colspan", 10);
                multitarget = true;

            } else {
                //单个指标的时候
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

    //if (IsNewDataIndex.indexOf("C") < 0) {
    //    IsNewDataIndex = IsNewDataIndex + "C";
    //}
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

//收缩
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

        FloatHeader(obj, tab, false, "MonthRpt"); //浮动表头

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


//当月未完成
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

    //这里判断数据源是草稿状态，还是审批状态的
    var dataSource = "Draft";
    if ($("#submana").is(":visible")) {
        dataSource = "Progress";
    }


    if ($(sender).text().indexOf("月度经营报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("完成情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetDetail&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("未完成说明") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=MissTarget&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
    } else if ($(sender).text().indexOf("回情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetReturn&SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&DataSource=" + dataSource);
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
    window.open("/AjaxHander/DownMonthRptFileList.ashx?SysId=" + SysId + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsLatestVersion=" + IsLatestVersion + "&OrderStr=" + MonthReportOrderType + "&DataSource=" + dataSource);
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



function unfoldTitle() {
    $("#CompleteDetailHead").html("");
    $("#tab2_rows").html("");

    //加载表头
    loadTmpl('#TmplProCompany_Head_All').tmpl().appendTo('#CompleteDetailHead'); //加载列头 

    if (ComplateDetailData[0].ObjValue.length > 4) {
        loadTmpl('#TmplProCompanyDetail_Data_All').tmpl(ComplateDetailData[0]).appendTo('#tab2_rows');

        $("#importedDataTable2").css("width", "2500");
    }

    var obj = $("#CompleteDetailHead");
    var tab = $("#tab2_rows");
    FloatHeader(obj, tab, false, "MonthRpt");
}


function shrinkageTitle() {
    $("#CompleteDetailHead").html("");
    $("#tab2_rows").html("");

    //加载表头
    loadTmpl('#TmplProCompany_Head').tmpl().appendTo('#CompleteDetailHead'); //加载列头 

    if (ComplateDetailData[0].ObjValue.length > 4) {
        loadTmpl('#TmplProCompanyDetail_Data').tmpl(ComplateDetailData[0]).appendTo('#tab2_rows');

        $("#importedDataTable2").css("width", "100%");
    }
    var obj = $("#CompleteDetailHead");
    var tab = $("#tab2_rows");
    FloatHeader(obj, tab, false, "MonthRpt");
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