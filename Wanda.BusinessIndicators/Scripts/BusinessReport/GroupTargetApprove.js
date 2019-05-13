
//变量误删
var ReturnData = {};
var MissTargetData = {};
var ComplateDetailData = {};
var MonthReportData = {};

var Year;
var Month;
var SystemID;
var MonthReportID;
var IsLatestVersion;

var IsNewDataIndex = "";
var MonthReportOrderType = "Detail";
var CompanyProperty = "";
var IncludeHaveDetail = false;
//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetGroupRptTmpl.html", selector);
}

function FloatHeader(obj, tab, num) {

    var arr = new Array();
    var selfarr = new Array();
    $(window).scroll(function () {
        //var obj = document.getElementById("CompleteDetailHead");

        var st = document.documentElement.scrollTop;
        if (st >= 350) {

            obj.css("position", "fixed");
            obj.css("top", "0px");
            if (arr.length > 0) {
                obj.find("tr:eq(0)").find("th").each(function (i) {
                    $(this).css("width", (selfarr[i] + 1) + "px");
                });
            }
        }
        else {
            obj.css("position", "relative");
            if (arr.length == 0) {
                tab.find("tr:eq(" + num + ")").find("td").each(function () {
                    arr.push($(this).width());
                });

                obj.find("tr:eq(0)").find("th").each(function () {
                    selfarr.push($(this).width());
                });
            }
            else {
                tab.find("tr:eq(" + num + ")").find("td").each(function (i) {
                    $(this).css("width", arr[i] + "px");
                });
            }
        }

        $("#Tab_Return").css("table-layout", "auto");
    });

    $(window).resize(function () {
        var st = document.documentElement.scrollTop;
        if (st >= 350) {

            obj.css("position", "fixed");
            obj.css("top", "0px");
            if (arr.length > 0) {
                obj.find("tr:eq(0)").find("th").each(function (i) {
                    $(this).css("width", (selfarr[i] + 1) + "px");
                });
            }
        }
        else {
            obj.css("position", "relative");
            if (arr.length == 0) {
                tab.find("tr:eq(" + num + ")").find("td").each(function () {
                    arr.push($(this).width());
                });

                obj.find("tr:eq(0)").find("th").each(function () {
                    selfarr.push($(this).width());
                });
            }
            else {
                tab.find("tr:eq(" + num + ")").find("td").each(function (i) {
                    $(this).css("width", arr[i] + "px");
                });
            }
        }

        $("#Tab_Return").css("table-layout", "auto");
    });
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
    //月度经营报告
    SystemID = $("#hideSystemID").attr("value");
    MonthReportID = $("#hideMonthReportID").attr("value");
    Year = $("#hideFinYear").attr("value");
    Month = $("#hideFinMonth").attr("value");
    IsLatestVersion = true;
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    GetGroupTargetApproveProcess(MonthReportID);
    getMonthReportSummaryData(false);
});

$(function () {
    var ShowProecessNodeName = false;
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetShowPrcessNodeName",
        args: { strSystemID: $("#hideSystemID").val(), strProcessCode: $("#HideProcessCode").val() },
        successReturn: function (resultData) {
            ShowProecessNodeName = resultData;
            $.unblockUI();
        }
    });
    var otherSetting = {
        IsShowContextMenu: true,
        PageContextMenu: true,
        EnableDebug: true,
        //ShowNodeName: ShowProecessNodeName == true ? true : false,
        ShowNodeName: true,
        ButtonCssType: "middle",
        CustomerSceneSetting: {
            ShowCc: false,//是否显示抄送
            ShowFowardButton: false,//是否显示转发按钮
            AlwaysReturnToStart: true
        },
        OnAfterExecute: afterAction,
    };
    bpf_wf_client.initAjaxSetting("#process", false, otherSetting);
})
function afterAction(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle($("#hideMonthReportID").attr("value"), args)
      
    }
}
function GetGroupTargetApproveProcess(instanceID) {
    bpf_wf_client.getProcess(instanceID, function () {
        $.unblockUI();
    });
}


function BusinessDataHandle(instanceID, args) {
    var PrcessStatus = "";
    if (args.WorkflowContext.ProcessInstance.Status == 2 && args.WorkflowContext.ProcessInstance.RunningNodeID == args.WorkflowContext.ProcessInstance.StartNodeID) {
        PrcessStatus = "Draft";
    } else if (args.WorkflowContext.ProcessInstance.Status == -1) {
        PrcessStatus = "Cancel";
    } else if (args.WorkflowContext.ProcessInstance.Status == 3) {
        // 审批结束
        if (args.WorkflowContext.CurrentUserNodeID != null && args.WorkflowContext.CurrentUserNodeID != "") {
            var nodeInfo = args.WorkflowContext.NodeInstanceList[args.WorkflowContext.CurrentUserNodeID];
            if (nodeInfo != null && (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7)) {
                PrcessStatus = "Approved";
            } else {
                PrcessStatus = null;
            }
        } else {
            PrcessStatus = null;
        }
    }
    else {
        PrcessStatus = "Progress"; // 审批结束
    }

    if (PrcessStatus != null) {

        $.ajax({
            url: "/AjaxHander/ProcessController.ashx",
            type: "post",
            async: true,
            data: {
                BusinessID: instanceID,
                OperatorType: args.OperatorType,
                PrcessStatus: PrcessStatus
            },
            success: function (result) {
                $.unblockUI();
                window.close();
            },
            error: function () {
                $.unblockUI();
                var errorInfo = "";
                var elem = $(arguments[0].responseText);
                for (var i = 3; i < elem.length; i++) {
                    errorInfo += elem[i].innerHTML;
                }
                WebUtil.alertWarn("对不起！您没有权限提交该流程，请联系管理员");
            }
        });
    } else
    {
        window.close();
    }
}

//function GetGroupTargetApproveProcess(instanceID) {
//    /// <summary>Description</summary>
//    $.ajax({
//        url: "/AjaxHander/ProcessController.ashx",
//        type: "post",
//        async: true,
//        cache: false,
//        data: {
//            CallMethed: "getprocess",
//            BusinessID: instanceID
//        },
//        dataType: "text",
//        success: function (result) {
//            BindGroupTargetApproveProcess(result, instanceID);
//            $.unblockUI();
//        },
//        error: function () {
//            $.unblockUI();
//            var errorInfo = "";
//            var elem = $(arguments[0].responseText);
//            for (var i = 3; i < elem.length; i++) {
//                errorInfo += elem[i].innerHTML;
//            }
//            //debugger; //$(arguments[0].responseText)
//            WebUtil.alertWarn(elem[3].innerText, errorInfo);
//        }
//    });
//}

//function BindGroupTargetApproveProcess(value, instanceID) {
//    wanda_wf.settings.ProcessDataKey = "wanda_wf_hd_process";
//    wanda_wf.settings.PostDataKey = "wanda_wf_opInfo";
//    document.getElementById(wanda_wf.settings.ProcessDataKey).value = value;
//    document.getElementById(wanda_wf.settings.PostDataKey).value = "";
//    wanda_wf.settings.submitFun = function () {
//        $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
//        $.ajax({
//            url: "/AjaxHander/ProcessController.ashx",
//            type: "post",
//            async: true,
//            cache: false,
//            data: {
//                CallMethed: "exec",
//                OperaionInfo: document.getElementById(wanda_wf.settings.PostDataKey).value,
//                Process: document.getElementById(wanda_wf.settings.ProcessDataKey).value,
//                BusinessID: instanceID
//            },
//            dataType: "text",
//            success: function (result) {
//                BindGroupTargetApproveProcess(result, instanceID);
//                $.unblockUI();
//            },
//            error: function () {
//                $.unblockUI();
//                var errorInfo = "";
//                var elem = $(arguments[0].responseText);
//                for (var i = 3; i < elem.length; i++) {
//                    errorInfo += elem[i].innerHTML;
//                }
//                //debugger; //$(arguments[0].responseText)
//                alert(errorInfo);
//            }
//        });
//    }
//    wanda_wf.init();
//}
//切换不同的报表
function ChangeTargetDetail(sender, TabOrSearch) {
    $(".active_sub2").each(function () {

        $(this).removeClass("active_sub2");
        $(this).parent().removeClass("selected");
    });
    $(sender).addClass("active_sub2");
    $(sender).parent().addClass("selected");

    $('#LabelDownload').text("导出" + $(sender).text());

    var CTDSystemID = $("#ddlSystem").val();
    var CTDYear = $("#ddlYear").val();
    var CTDMonth = $("#ddlMonth").val();
    var CTDlatest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        CTDlatest = true;
    }

    var CTDIsLatestVersion = CTDlatest;

    if ($(sender).text() == "月度报告" || TabOrSearch == "Search") {
        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv').show();
        $('#T2').hide();
        //月度报告
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }
    } else if ($(sender).text() == "明细" && TabOrSearch != "Search") {

        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv').hide();
        $('#T2').show();
        var obj = $("#CompleteDetailHead");
        var tab = $("#tab2_rows");
        FloatHeader(obj, tab, false, "MonthRpt");

        //明细
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "B") == true) {
            getMonthReprotDetailData();
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
}

//是否显示查询条件动画方法
function UpDownTableClick() {
    if ($('#imgtableUpDown').attr("src") == "../Images/images1/Down.png") {
        $('#imgtableUpDown').attr("src", "../Images/images1/Up.png");
        $('#ContentPlaceHolder1_panel').slideDown("slow");
    }
    else {
        $('#imgtableUpDown').attr("src", "../Images/images1/Down.png");
        $('#ContentPlaceHolder1_panel').slideUp("slow");
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
    MonthReportData = resultData
    if (resultData) {
        ReportInstance = resultData[0].ObjValue;
        var strSummaryTabel;
        if (resultData[1] != null) {
            $("#txtDes").html("");
            var strTemp = resultData[1].ObjValue;
            //strTemp = strTemp.replace(/\n/g, "<br/>").replace(/ /g, "&nbsp;");
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


function getMonthReportSummaryData(asyncBlock) {
    var block = true;
    if (asyncBlock != undefined) { block = asyncBlock; }

    // if (MonthReportID != "") {
    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        asyncBlock: block,
        url: "/TargetApproveController/GetReportInstance",
        args: { strSystemID: SystemID, strMonthReportID: MonthReportID, IsLatestVersion: IsLatestVersion },
        successReturn: SplitData
    });
    if (IsNewDataIndex.indexOf("A") < 0) {
        IsNewDataIndex = IsNewDataIndex + "A";
    }
    //}
}


//---------------------完成情况明细---------------------------------------------------------------------
//加载完成情况明细数据
function getMonthReprotDetailData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //加载月度报告说明

    WebUtil.ajax({
        async: true,
        url: "/TargetApproveController/GetDetailRptDataSource",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strCompanyProperty: CompanyProperty, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: function (ResultData) {
            ComplateDetailData = ResultData;

            $('#CompleteDetailHead').empty();
            loadTmpl('#GroupMonthReportDetailHeadTmpl').tmpl().appendTo('#CompleteDetailHead');
            $('#tab2_rows').empty();
            loadTmpl('#GroupComplateTargetDetailTemplate').tmpl(ComplateDetailData).appendTo('#tab2_rows');

            ///SetComplateTargetDetailData(ComplateDetailData[0], 1);
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
    SystemID = $("#hideSystemID").attr("value");
    MonthReportID = $("#hideMonthReportID").attr("value");
    var date = new Date;
    Year = date.getFullYear();
    Month = date.getMonth() - 1;
    if ($(sender).text().indexOf("月度报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
    } else if ($(sender).text().indexOf("明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetDetail&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
    }
}

