
//变量误删
var ReturnData = {};
var MissTargetData = {};
var CurrentMissTargetData = {};
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
    return WebUtil.loadTmpl("../BusinessReport/TargetDirectlyRpt.html", selector);
}

//加载模版项-------------------------------------------------------------------------
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/MissTargerDirectlyRpt.html", selector);
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
    GetDirectlyTargetApproveProcess(MonthReportID);
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
        ShowNodeName: ShowProecessNodeName == true ? true : false,
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
function GetDirectlyTargetApproveProcess(instanceID) {
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
        PrcessStatus = "Progress";  //审批中
    }


    if (PrcessStatus != null) {
        $.ajax({
            url: "/AjaxHander/ProcessController.ashx",
            type: "post",
            async: true,
            data: {
                BusinessID: instanceID,
                OperatorType: args.OperatorType,
                PrcessStatus: PrcessStatus,
                ExecuteType: "afterAction",
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



function ChangeTargetDetail(sender, TabOrSearch) {
    $(".active_sub2").each(function () {

        $(this).removeClass("active_sub2");
        $(this).parent().removeClass("selected");
    });
    $(sender).addClass("active_sub2");
    $(sender).parent().addClass("selected");
    $('#LabelDownload').text("导出" + $(sender).text());
    if ($(sender).text() == "月度经营报告") {
        $('#T1,#MonthReportExplainDiv,#ApproveAttachDiv').show();
        $('#T2,#T4,#T3,#T3_1').hide();
     
        //月度经营报告
        if (TransitionCondition(MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }
    } else if ($(sender).text() == "完成情况明细") {
        $('#T1,#T3,#T3_1,#T4,#MonthReportExplainDiv,#ApproveAttachDiv').hide();
        $('#T2').show();
        //var obj = $("#CompleteDetailHead");
        //var tab = $("#tab2_rows");
        //FloatHeader(obj, tab, false, "MonthRpt");
       
        //完成情况明细
        if (TransitionCondition(ComplateDetailData[0], "B") == true) {
            getMonthReprotDetailData();
        }

    }
    else if ($(sender).text() == "当月未完成") {
        $("#T4,#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv").hide();
        $("#T3_1").show();

        //var obj = $("#Tab_MissTargetHead");
        //var tab = $("#Tbody_MissTargetData");
        //FloatHeader(obj, tab, false, "MonthRpt");
        
        //未完成说明
        if (TransitionCondition(CurrentMissTargetData[0], "F") == true) {
            getCurrentMonthReportMissTargetData();
        }
    }else if ($(sender).text() == "累计未完成") {
        $("#T4,#T1,#T2,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv").hide();
        $("#T3").show();

        //var obj = $("#Tab_MissTargetHead");
        //var tab = $("#Tbody_MissTargetData");
        //FloatHeader(obj, tab, false, "MonthRpt");
        

        //未完成说明
        if (TransitionCondition(MissTargetData[0], "C") == true) {
            getMonthReportMissTargetData();
        }
    } else if ($(sender).text() == "补回情况明细") {
        $("#T4").show();
        $("#T1,#T2,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv").hide();

        //补回情况明细
        if (TransitionCondition(ReturnData[0], "D") == true) {
            getMonthReportReturnData();
        }
    }

    if ($(sender).text() != "完成情况明细") {
        $('#imgtableUpDown').hide();
        $('#imgtableUpDown').attr("src", "../Images/images1/Down.png");
        $('#ContentPlaceHolder1_panel').hide();
    } else {
        $('#imgtableUpDown').show();
    }
}
//
function TransitionCondition(resultDate, index) {
    if (resultDate == undefined) {
        return true;
    }
    if (IsNewDataIndex.indexOf(index) < 0) {
        return true;
    }
}

////是否显示查询条件动画方法
//function UpDownTableClick() {
//    if ($('#imgtableUpDown').attr("src") == "../Images/images1/Down.png") {
//        $('#imgtableUpDown').attr("src", "../Images/images1/Up.png");
//        $('#ContentPlaceHolder1_panel').slideDown("slow");
//    }
//    else {
//        $('#imgtableUpDown').attr("src", "../Images/images1/Down.png");
//        $('#ContentPlaceHolder1_panel').slideUp("slow");
//    }
//}


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
            return  obj[0].TargetGroupCount;
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


function getMonthReportSummaryData(asyncBlock) {

    var block = true;
    if (asyncBlock != undefined) { block = asyncBlock; }
    // if (MonthReportID != "") {
    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        asyncBlock:block,
        url: "/TargetApproveController/GetReportInstance",
        args: { strSystemID: SystemID, strMonthReportID: MonthReportID, IsLatestVersion: IsLatestVersion },
        successReturn: SplitData
    });
    if (IsNewDataIndex.indexOf("A") < 0) {
        IsNewDataIndex = IsNewDataIndex + "A";
    }
    //}
}


//-------------补回情况------------------------------------------------------------------------------------------------------
function getMonthReportReturnData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/TargetApproveController/GetTargetReturnList",
        args: { rpts: WebUtil.jsonToString(ReportInstance) },
        successReturn: function (result) {
            ReturnData = result;
            //先影藏其它标签
            // $("#T3").hide();

            //首先指标先加载
            $("#U1").empty();
            $("#Tbody_Data").empty();
            $("#Tab_ReturnHead").empty();

            loadTmpl_1('#TmplHeadReturn').tmpl().appendTo('#Tab_ReturnHead');  //加载裂头

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
            var obj = $("#Tab_FloatReturn");
            var head = $('#Tab_ReturnHead');
            var tab = $("#Tbody_Data");
            obj.find("thead").html(head.html());
            FloatHeader(obj, tab);
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
    //$("#Tab_Return").attr({ style: "table-layout: fixed" });
    $('#CurrentMonthBackDetilDiv').text("本月累计(万元) [+]");

}


//未完成 tab（未完成说明 累计的）--------------------------------------------------------------------------------------------
function getMonthReportMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //未完成说明
    WebUtil.ajax({
        async: true,
        url: "/TargetApproveController/GetMissTargetList",
        args: { rpts: WebUtil.jsonToString(ReportInstance) },
        successReturn: function (result) {
            MissTargetData = result;
            //先影藏其它标签//首先指标先加载
            $("#U2").empty();
            $("#Tbody_MissTargetData").empty();
            $("#Tab_MissTargetHead").empty();

            var multitarget = false;
            //判断指标有几个分组，如果是2个一上默认选择第一个
            if (MissTargetData.length > 1) {

                loadTmpl_1('#TmplHeadMiss_SG').tmpl().appendTo('#Tab_MissTargetHead');

                loadTmpl_1('#TmplMissTarget_SG').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                loadTmpl_1('#TmplMissTargerList').tmpl(MissTargetData).appendTo('#U2');
                $(".newdiff_miss").hide();
                $(".Level1TdSp1").attr("colspan", 10);
                multitarget = true;

            } else {

                loadTmpl_1('#TmplHeadMiss').tmpl().appendTo('#Tab_MissTargetHead');
                //单个指标的时候
                loadTmpl_1('#TmplMissTarget').tmpl(MissTargetData[0]).appendTo('#Tbody_MissTargetData');
                $(".newdiff_miss").show();
                $(".Level1TdSp1").attr("colspan", 11);

            }
            $("#U2 :first a").addClass("active_sub3");
            $(".shangyue").hide();
            $("#Tab_MissTarget").attr({ style: "table-layout: fixed" });
            $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");
            var obj = $("#Tab_MissFloatTarget");
            var head = $('#Tab_MissTargetHead');
            var tab = $("#Tbody_MissTargetData");
            obj.find("thead").html(head.html());
            FloatHeader(obj, tab);
        }
    });
    //给第一指标添加背景颜色

    if (IsNewDataIndex.indexOf("C") < 0) {
        IsNewDataIndex = IsNewDataIndex + "C";
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
            var string = WebUtil.jsonToString(CurrentMissTargetData);
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
                loadTmpl_1('#TmplCurrentMissTarget_Directly').tmpl(CurrentMissTargetData[0]).appendTo('#Tbody_CurrentMissTargetData');
                $(".newdiff_CurrenMiss").show();
                $(".Curr_Level1TdSp1").attr("colspan", 11);

            }
            $("#U2_1 :first a").addClass("active_sub3");

            //var obj = $("#Tab_CurrentMissTargetHead");
            //var tab = $("#Tbody_CurrentMissTargetData");
            var obj = $("#Tab_CurrentMissFloatTarget");
            var head = $('#Tab_CurrentMissTargetHead');
            var tab = $("#Tbody_CurrentMissTargetData");
            obj.find("thead").html(head.html());
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


function MissLiaddCss(sender) {
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
    SystemID = $("#hideSystemID").attr("value");
    MonthReportID = $("#hideMonthReportID").attr("value");
    var date = new Date;
    Year = date.getFullYear();
    Month = date.getMonth() - 1;
    if ($(sender).text().indexOf("月度经营报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID
           + "&Approve=true");
    } else if ($(sender).text().indexOf("累计未完成") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=MissTarget&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID
           + "&Approve=true");
    } else if ($(sender).text().indexOf("当月未完成") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=CurrentMissTarget&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID
           + "&Approve=true");
    } else if ($(sender).text().indexOf("回情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetReturn&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID
           + "&Approve=true");
    }

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
            { return; } else
            {
                if ($(this).context.className.indexOf("Level3") >= 0) {
                    $(this).hide();
                }
            }

        });
    }
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