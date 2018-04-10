﻿
//变量误删
var ReturnData = {};
var MissTargetData = {};
var CurrentMissTargetData = {};
var ComplateDetailData = {};
var MonthReportData = {};
var ReportInstance;
var sysID; //系统ID
var FinYear;
var FinMonth;
var MonthReportID; //月报ID
var _BusinessID; //流程ID
var Description;
var MonthReportOrderType = "Detail";
var IncludeHaveDetail = true;
var Upload = true;
var MissType ='MissTargetRpt';


//加载模版项
function loadTmpl_2(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetProRpt.html", selector);
}
//加载模版项-------------------------------------------------------------------------
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/ProTargetReturnTmpl.html?v=2", selector);
}



function ComplateDetailLiaddCss2(sender) {
    var TemplData = {};

    $("#Ul6 .active_sub3").each(function () {
        $(this).removeClass("active_sub3");
    });
    $(sender).addClass("active_sub3");

    currentDetailTarget = $("#Ul6 li .active_sub3 ");
    if ($(sender).attr("data-value") == "完成情况明细-利润额") {
        $("#importedDataTable5").show();
        $("#importedDataTable4").hide();
    }
    else if ($(sender).attr("data-value") == "完成情况明细-销售+回款") {
        $("#importedDataTable5").hide();
        $("#importedDataTable4").show();
    }
}

//页面加载事件
$(function () {

    sysID = $("#ddlSystem").val();
    MonthReportID = $("#hideMonthReportID").val();
    FinYear = $("#hideFinYear").val();
    FinMonth = $("#hideFinMonth").val();
    _BusinessID = $("#HiddenBatch").val();

    //针对没有批次的数据
    if ($("#HiddenBatch").val() == "") {
        _BusinessID = MonthReportID;
    }

    //获取ReportInstance实例，将此实例传回后台。
    GetReportInstance();

    MissTagetExcelReport();

    //自动保存,光标离开
    $("#MonthGetDescription").blur(function () {
        $("#MonthGetDescription").css("background-color", "#FFFFFF");

        var monthRpt;

        if (ReportInstance.LastestMonthlyReport != undefined) {
            monthRpt = ReportInstance.LastestMonthlyReport;
            monthRpt.Description = $("#MonthGetDescription").val();
        }

        if (Description != $("#MonthGetDescription").val()) {
            //// alert("入库");
            //WebUtil.ajax({
            //    async: true,
            //    url: "/TargetReportedControll/ModifyMonthTRptDescription",
            //    args: { rpts: WebUtil.jsonToString(monthRpt) },
            //    successReturn: function (result) {

            //    }
            //});

            Description = $("#MonthGetDescription").val();
        } else {

        }
    });

    $("#MonthGetDescription").focus(function () {
        $("#MonthGetDescription").css("background-color", "#D6D6FF");
    });

})


function MissTagetExcelReport() {
    //if (DownLoadTag == "missCurrentTargetReport") {
    //    MissType = 'CurrentMissTargetRpt';
    //} else {
    //    MissType = 'MissTargetRpt';
    //}

    //// 未完成数据上传数据
    //$('#file_upload').uploadify({
    //    'buttonText': '导入数据',
    //    'width': 100,
    //    'height': 25,
    //    'successTimeout': 50, 
    //    'fileTypeDesc': 'office file',
    //    'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx',
    //    'fileSizeLimit': '10240',
    //    'swf': '../Scripts/UpLoad/uploadify.swf',
    //    'uploader': '../AjaxHander/ExcelReport.ashx?FileType='+ MissType +'&SysId=' + sysID + '&MonthReportID=' + MonthReportID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth,
    //    'formData': { "action": "未完成指标填写" },
    //    'onSelect': function (e, queueId, fileObj) { },
    //    'onUploadSuccess': function (file, data, response) {
    //        if (data == "") {
    //            getMonthReportMissTargetData(true);
    //            setStlye('monthReportSpan,monthReportReadySpan');
    //        } else { alert(data); }
    //    }
    //});

}




//初始化数据
function GetReportInstance() {

    //WebUtil.ajax({
    //    async: true,
    //    url: "/TargetReportedControll/GetReportInstance",
    //    args: { strSystemID: sysID, strMonthReportID: MonthReportID, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail, UploadStr: Upload },
    //    successReturn: SplitData
    //});

}

function SplitData(resultData) {
    if (resultData != null) {
        if (resultData[0] != null) {
            ReportInstance = resultData[0].ObjValue;
        }
        if (resultData[1] != null) {
            ComplateDetailData = resultData[1].ObjValue;

            TargetDetail();
            if (ReportInstance.ReportDetails.length > 0) {
                setStlye('dataUploadSpan,downLoadTemplateSpan,missTargetReportSpan,missCurrentTargetReportSpan,monthReportSpan,monthReportReadySpan');
            }
        }
        if (resultData[2] != null) {
            MissTargetData = resultData[2].ObjValue;
            //加载数据
            TmplMissTargetData(MissTargetData, false);
        }
        if (resultData[3] != null) {
            Description = resultData[3].ObjValue;
            $("#MonthGetDescription").val(Description);
        }
        if (resultData[4] != null) //当月数据
        {
            CurrentMissTargetData = resultData[4].ObjValue;
            TmplCurrentMissTargetData(resultData[4].ObjValue, false);
        }

    }
}



//加载表头和数据
function TargetDetail() {
    $("#CompleteDetailHead").empty();
    $("#tab2_rows").empty();

    ////加载表头
    //loadTmpl_2('#TmplProCompany_South_Head').tmpl().appendTo('#CompleteDetailHead'); //加载列头 

    //if (ComplateDetailData[0].ObjValue.length > 4) {
    //    loadTmpl_2('#TmplProCompanyDetail_South_Data').tmpl(ComplateDetailData[0]).appendTo('#tab2_rows');
    //}

}

//获取月份
function GetMonthStr() {
    return $("#hideFinMonth").val()
}


//月报说明
function GetMonthGetDescription() {
   

}


//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}


//未完成 tab（未完成说明）--------------------------------------------------------------------------------------------
function getMonthReportMissTargetData(Upload) {

    //未完成说明
    WebUtil.ajax({
        async: false,
        url: "/TargetReportedControll/GetMissTargetList_Reported",
        args: { rpts: "", monthRptID: $("#hideMonthReportID").val(), UploadStr: Upload },
        successReturn: function (result) {
            //MissTargetData = result;

            ////加载数据
            //TmplMissTargetData(MissTargetData, false);

            if (result != null && result != undefined) {
                if (result[0] != null && result[0] != undefined) {
                    MissTargetData = result[0].ObjValue;
                    TmplMissTargetData(MissTargetData, false);
                }

                if (result[1] != null && result[1] != undefined) {
                    CurrentMissTargetData = result[1].ObjValue;
                    TmplCurrentMissTargetData(CurrentMissTargetData, false);
                }
            }   



        }
    });

}


var isUL = false;
//加载未完成明细Tmpl数据(leji)
function TmplMissTargetData(MissTargetObj, isUL) { //MissTargetObj :未完成数据源， isUL：UL标签是否重新加载
    //先影藏其它标签
    //首先指标先加载

    $("#Tbody_MissTargetData").empty();
    $("#Tab_MissTargetHead").empty();

    loadTmpl_1('#TmplHeadMiss').tmpl().appendTo('#Tab_MissTargetHead'); //加载列头
    if (isUL == false) {
        $("#U2").empty();
        loadTmpl_1('#TmplMissTargerList').tmpl(MissTargetData).appendTo('#U2');
    }
    //判断指标有几个分组，如果是2个一上默认选择第一个

    if (MissTargetObj.length > 1) {
        loadTmpl_1('#TmplMissTargetRpt').tmpl(MissTargetObj[0]).appendTo('#Tbody_MissTargetData');


        if (MissTargetObj[0].TargetGroupCount == 1) {
            $(".newdiff_miss").hide(); //单个(这里：经营系统：商管和物管)
            $(".Level1TdSp1").attr("colspan", 10);
        } else {
            $(".newdiff_miss").show(); //混合 （这里：混合组合：旅业）
            $(".Level1TdSp1").attr("colspan", 11);
        }

        //$(".newdiff_miss").hide(); //商管的
        //$(".Level1TdSp1").attr("colspan", 10);

        if (currentMissTarget != null) {
            MissLiaddCss(currentMissTarget)
        }
        else {
            $("#U2 :first a").addClass("active_sub3");
        }

    } else {
        loadTmpl_1('#TmplMissTargetRpt').tmpl(MissTargetObj).appendTo('#Tbody_MissTargetData');
        $(".newdiff_miss").show();
        $(".active3").hide();//把指标标签影藏
        $(".Level1TdSp1").attr("colspan", 11);
    }



    //显示影藏
    $(".shangyue").hide();
    //$(".Level1TdSp1").attr("colspan", 11);
    $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");
}

var currentMissTarget = null;//在未完成编辑的时候，通过指标筛选时，停留在当前指标

//单个指标筛选
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

    currentMissTarget = sender;
    $('#Tbody_MissTargetData').html("");

    loadTmpl_1('#TmplMissTargetRpt').tmpl(m).appendTo('#Tbody_MissTargetData');

    //显示影藏
    $(".shangyue").hide();
    $(".TT2").attr("colspan", 3);
    $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
    $(".Level1TdSp1").attr("colspan", 10);
    $('#CurrentMonthMissTergetDiv').text("本月累计(万元) [+]");

}


var IsCUL = false;
//加载未完成明细Tmpl数据 （当月的）
function TmplCurrentMissTargetData(MissTargetObj, IsCUL) { //MissTargetObj :未完成数据源， IsCUL：UL标签是否重新加载
    //先影藏其它标签
    //首先指标先加载

    $("#Tbody_CurrentMissTargetData").empty();
    $("#Tab_CurrentMissTargetHead").empty();

    loadTmpl_1('#TmplCurrentHeadMiss').tmpl().appendTo('#Tab_CurrentMissTargetHead'); //加载列头
    if (IsCUL == false) {
        $("#U2_1").empty();
        loadTmpl_1('#TmplCurrentMissTargerList').tmpl(CurrentMissTargetData).appendTo('#U2_1');
    }
    //判断指标有几个分组，如果是2个一上默认选择第一个

    if (MissTargetObj.length > 1) { //完全分解指标
        loadTmpl_1('#TmplCurrentMissTargetRpt').tmpl(MissTargetObj[0]).appendTo('#Tbody_CurrentMissTargetData');

        if (MissTargetObj[0].TargetGroupCount == 1) {
            $(".newdiff_CurrenMiss").hide(); //单个(这里：经营系统：商管和物管)
            $(".Curr_Level1TdSp1").attr("colspan", 10);
        } else {
            $(".newdiff_CurrenMiss").show(); //混合 （这里：混合组合：旅业）
            $(".Curr_Level1TdSp1").attr("colspan", 11);
        }

        if (currentMissTarget != null) {
            MissCurrentLiaddCss(currentMissTarget)
        }
        else {
            $("#U2_1 :first a").addClass("active_sub3");
            $(".C_TT2").attr("colspan", 4);
        }

    } else { //完全组合指标
        loadTmpl_1('#TmplCurrentMissTargetRpt').tmpl(MissTargetObj).appendTo('#Tbody_CurrentMissTargetData');
        $(".newdiff_CurrenMiss").show();
        $(".active3").hide();//把指标标签影藏
        $(".Curr_Level1TdSp1").attr("colspan", 11);
    }

    //混合组合指标

    //显示影藏
    $(".leiji").hide();

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
        $(".newdiff_CurrenMiss").hide();  //单个(这里：经营系统：商管和物管)
        $(".Curr_Level1TdSp1").attr("colspan", 10);
    } else {
        $(".newdiff_CurrenMiss").show(); //混合 （这里：混合组合：旅业）
        $(".Curr_Level1TdSp1").attr("colspan", 11);
    }
    loadTmpl_1('#TmplCurrentMissTargetRpt').tmpl(m).appendTo('#Tbody_CurrentMissTargetData');

    //显示影藏
    $(".leiji").hide();
    $(".C_TT2").attr("colspan", 4);
    $(".Curr_Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
    //$(".Curr_Level1TdSp1").attr("colspan", 11);
    $('#MonthMissTergetDiv_Current').text("本月发生(万元) [+]");

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
    currentDetailTarget = sender;
    //$("#Ul4 li .active_sub3 ");

    strComplateMonthReportDetilHtmlTemplate = new Array();
    if (TemplData.HtmlTemplate != undefined) {
        strComplateMonthReportDetilHtmlTemplate = TemplData.HtmlTemplate.split(',');
    }
    //tmpl模板名称
    if (strComplateMonthReportDetilHtmlTemplate[2] != "" && strComplateMonthReportDetilHtmlTemplate[2] != undefined) {
        ComplateTargetDetailTemplate = strComplateMonthReportDetilHtmlTemplate[2];
    } else {
        ComplateTargetDetailTemplate = "TargetReportedComplateTargetDetailTemplate"
    }

    $("#tab2_rows").empty();
    loadTmpl_2('#' + ComplateTargetDetailTemplate).tmpl(TemplData).appendTo('#tab2_rows');
    SetComplateTargetDetailData(TemplData, 2);

}
var ComplateTargetDetailTemplate = null;

function SetComplateTargetDetailData(sender, Type) {
    strComplateMonthReportDetilHtmlTemplate = new Array();
    if (sender.HtmlTemplate != undefined) {
        strComplateMonthReportDetilHtmlTemplate = sender.HtmlTemplate.split(',');
    }
    $("#CompleteDetailHead").empty();
    if (strComplateMonthReportDetilHtmlTemplate[0] != "" && strComplateMonthReportDetilHtmlTemplate[0] != undefined) {
        loadTmpl_2('#' + strComplateMonthReportDetilHtmlTemplate[0]).tmpl().appendTo('#CompleteDetailHead');

    } else {
        loadTmpl_2('#CompleteDetailHeadTemplate').tmpl().appendTo('#CompleteDetailHead');
    }


    //tmpl模板名称
    if (strComplateMonthReportDetilHtmlTemplate[2] != "" && strComplateMonthReportDetilHtmlTemplate[2] != undefined) {
        ComplateTargetDetailTemplate = strComplateMonthReportDetilHtmlTemplate[2];
    } else {
        ComplateTargetDetailTemplate = "TargetReportedComplateTargetDetailTemplate"
    }

    if (currentDetailTarget == null) {
        $("#Ul4").empty();
        loadTmpl_2('#TargetReportedComplateTargetDetailHeadTemplate').tmpl(ComplateDetailData).appendTo('#Ul4');
        $("#tab2_rows").empty();
        loadTmpl_2('#' + ComplateTargetDetailTemplate).tmpl(sender).appendTo('#tab2_rows');
        $("#Ul4 :first a").addClass("active_sub3");
    } else {
        if (Type == 1) {
            ComplateDetailLiaddCss(currentDetailTarget);
        }
    }

    if (MonthReportOrderType == "Detail") {
        $(".DetailMonthly").attr("src", "../Images/btn_down02_w.png");
        $(".Detail").attr("src", "../Images/btn_down03_w.png");
        $(".DetailCss").addClass("tabOrderBackground");
    } else {
        $(".DetailMonthly").attr("src", "../Images/btn_down03_w.png");
        $(".Detail").attr("src", "../Images/btn_down02_w.png");
        $(".DetailMonthlyCss").addClass("tabOrderBackground");
    }



}



//汇总数据
function SummaryComplateDetailData(ObjValue, ColumnName) {
    var count = 0;
    $.each(ObjValue, function (i, item) {
        count += item[ColumnName];
    });
    if (!isNaN(count)) {
        return count.toFixed(2);
    } else {
        return 0;
    }
}

//编辑未完成信息 -- All
function EditMissTargetRpt(sender, obj, tag) {

    info = null;
    currentInfo = null;

    GetInfoByID(sender, tag);

    currentMissTarget = $("#U2 li .active_sub3 ");

    if (info != null) {

        if (tag == "current") {

            // 这里的编辑项---当月未完成
            if (obj == "all") { // 编辑所有项
                $("#content_edit").empty();
                loadTmpl_1("#rpt_current_info_tmpl").tmpl(info).appendTo($("#content_edit"));

                art.dialog({
                    content: $("#divMissTargetRpt").html(),
                    lock: true,
                    id: 'divMissTargetRpt',
                    title: '<span>月度经营上报--编辑</span>'
                });
            } else if (obj == "Reason") { //只编辑未完成原因，措施
                $("#content_edit_Reason").empty();
                loadTmpl_1("#rpt_current_info_Reason_tmpl").tmpl(info).appendTo($("#content_edit_Reason"));
                art.dialog({
                    content: $("#divMissTargetRpt_Reason").html(),
                    lock: true,
                    id: 'divMissTargetRpt_Reason',
                    title: '<span>月度经营上报--编辑</span>'
                });
            } else if (obj == "return") { //值编辑补回情况
                $("#content_edit_retuen").empty();
                loadTmpl_1("#rpt_current_info_return_tmpl").tmpl(info).appendTo($("#content_edit_retuen"));
                $("#rpt_info_return_returntype option[value='" + info.ReturnType_Sub + "']").attr("selected", true);
                art.dialog({
                    content: $("#divMissTargetRpt_Retu").html(),
                    lock: true,
                    id: 'divMissTargetRpt_Retu',
                    title: '<span>月度经营上报--编辑</span>'
                });
            }

        } else
        {
            if (obj == "all") { // 编辑所有项
                $("#content_edit").empty();
                loadTmpl_1("#rpt_info_tmpl").tmpl(info).appendTo($("#content_edit"));
                $("#rpt_info_returntype option[value='" + info.ReturnType + "']").attr("selected", true);
                art.dialog({
                    content: $("#divMissTargetRpt").html(),
                    lock: true,
                    id: 'divMissTargetRpt',
                    title: '<span>月度经营上报--编辑</span>'
                });
            } else if (obj == "Reason") { //只编辑未完成原因，措施
                $("#content_edit_Reason").empty();
                loadTmpl_1("#rpt_info_Reason_tmpl").tmpl(info).appendTo($("#content_edit_Reason"));
                art.dialog({
                    content: $("#divMissTargetRpt_Reason").html(),
                    lock: true,
                    id: 'divMissTargetRpt_Reason',
                    title: '<span>月度经营上报--编辑</span>'
                });
            }
            else if (obj == "return") { //值编辑补回情况
                $("#content_edit_retuen").empty();
                loadTmpl_1("#rpt_info_return_tmpl").tmpl(info).appendTo($("#content_edit_retuen"));
                $("#rpt_info_return_returntype option[value='" + info.ReturnType + "']").attr("selected", true);
                art.dialog({
                    content: $("#divMissTargetRpt_Retu").html(),
                    lock: true,
                    id: 'divMissTargetRpt_Retu',
                    title: '<span>月度经营上报--编辑</span>'
                });
            }
        }

       
    }
}

//编辑未完成，补回情况的“子状态”
function SetReturnDesc(Obj) {

    var PromissDate;
    var pdate;
    //承诺时间
    if ($("#rpt_info_PromissDate").val() != undefined || $("#rpt_info_return_PromissDate").val() != undefined) {
        if ($("#rpt_info_PromissDate").val() != undefined) {
            PromissDate = $("#rpt_info_PromissDate").val();
        } else {
            PromissDate = $("#rpt_info_return_PromissDate").val();
        }
    }

    if (PromissDate.length > 7) {
        pdate = new Date(PromissDate.replace("-", "//"));
    } else { pdate = new Date(PromissDate.replace("-", "//") + "/1 0:00:00"); }

    var TempMonth = pdate.getMonth() + 1 + "月"; //承诺的的月份
    var currObj = $(Obj).find("option:selected"); //子情况的选项
    var returnStr = currObj.text();

    var myDate = new Date();

    switch (currObj.val()) {
        case "T1": //补回中
            $("#rpt_info_return_back,#rpt_info_back").val("");
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val(info.CommitDate);
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,#rpt_info_PromissDate_day,#rpt_info_return_PromiseTarget").hide(); //隐藏承诺时间好
            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });
            break;
        case "T2": //承诺提前至X月份补回
            //returnStr = returnStr.replace("X月", TempMonth);
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val("");

            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,").show(); //隐藏承诺时间好
            $("#rpt_info_PromissDate_day,#rpt_info_return_PromiseTarget").hide();
            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });
            break;
        case "T3": //预计无法按期补回，承诺X月份补回
            var currMonth = myDate.getMonth() + 1;

            var currDate = myDate.getFullYear() + "-" + currMonth;
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val("");
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").show(); //隐藏承诺时间好
            $("#rpt_info_PromissDate_day,#rpt_info_return_PromiseTarget").hide();

            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });

            break;
        case "T4": //预计年内无法补回（必保全年完成)
            var currDate = myDate.getFullYear() + "-12-31";
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val(currDate); //时间

            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,#rpt_info_PromissDate_day").hide(); //隐藏承诺时间好
            $("#rpt_info_PromissDate_day").hide();

            $("#rpt_info_return_PromiseTarget").show(); // 显示必保的输入框

            $("#rpt_info_back,#rpt_info_return_back").removeAttr('readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "" });


            returnStr = "预计年内无法补回必保全年完成【】万（指标 " + info.AnnualTargetPlanValue.thousandize0OrEmpty(0) + "万，差额" + info.NAccumulativeDifference.thousandize0OrEmpty(0) + "万）";

            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
            break;
        case "T41": //完成承诺数据,实际全年完成[](指标【】万，差额【】万)
            var currDate = myDate.getFullYear() + "-12-31";
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val(currDate); //时间
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,#rpt_info_PromissDate_day,#rpt_info_return_PromiseTarget").hide(); //隐藏承诺时间好
            $("#rpt_info_return_PromiseTarget").val("");


            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });

            var CE = (info.NAccumulativeActualAmmount - info.AnnualTargetPlanValue).thousandize0OrEmpty(0);
            returnStr = "完成承诺数据,实际全年完成" + info.NAccumulativeActualAmmount.thousandize0OrEmpty(0) + "万(指标" + info.AnnualTargetPlanValue.thousandize0OrEmpty(0) + "万，差额" + CE + "万)"

            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);


            break;

        case "T42": //未完成承诺数据,实际全年完成[](指标【】万，差额【】万)
            var currDate = myDate.getFullYear() + "-12-31";
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val(currDate); //时间
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,#rpt_info_PromissDate_day,#rpt_info_return_PromiseTarget").hide(); //隐藏承诺时间好
            $("#rpt_info_return_PromiseTarget").val("");

            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });
            var CE = (info.NAccumulativeActualAmmount - info.AnnualTargetPlanValue).thousandize0OrEmpty(0);
            returnStr = "未完成承诺数据,实际全年完成" + info.NAccumulativeActualAmmount.thousandize0OrEmpty(0) + "万(指标" + info.AnnualTargetPlanValue.thousandize0OrEmpty(0) + "万，差额" + CE + "万)"

            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);


            break;

        case "T5"://X月X日已补回
            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,#rpt_info_return_PromiseTarget").hide(); //隐藏承诺时间好
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });
            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
            $("#rpt_info_PromissDate_day").val("");
            $("#rpt_info_PromissDate_day").show();
            break;
        case "T6"://其它情况

            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").val("");
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate").show();
            $("#rpt_info_PromissDate_day,#rpt_info_return_PromiseTarget").hide();
            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
            $("#rpt_info_back,#rpt_info_return_back").removeAttr('readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "" });
            break;
        case "TT"://请选择
            $("#rpt_info_PromissDate,#rpt_info_return_PromissDate,#rpt_info_PromissDate_day").hide(); //隐藏承诺时间好
            $("#rpt_info_back,#rpt_info_return_back").val("");
            $("#rpt_info_back,#rpt_info_return_back").attr("readonly", 'readonly');
            $("#rpt_info_back,#rpt_info_return_back").css({ "backgroundColor": "#ececec" });
            break;

    }

}


//必保金额，差额
function ChangePromiseTarget() {

    //if ($v.isRequired($("#rpt_info_return_PromiseTarget").val()) == false )
    //{ return alert("必填项！") };

    if ($v.isNumber($("#rpt_info_return_PromiseTarget").val()) == false) {
        return alert("请输入数字");
    }

    var CE = ($("#rpt_info_return_PromiseTarget").val() - info.AnnualTargetPlanValue).thousandize0OrEmpty(0);
    var BB = Number($("#rpt_info_return_PromiseTarget").val());

    var returnStr = "预计年内无法补回必保全年完成 " + BB.thousandize0OrEmpty(0) + "万（指标 " + info.AnnualTargetPlanValue.thousandize0OrEmpty(0) + "万，差额" + CE + "万）";

    $("#rpt_info_back,#rpt_info_return_back").val(returnStr);

}


//日期选择控件
function ChangelFunc(objStr) {

    var c = $dp.cal;
    var TempMonth; //承诺的的月份

    var Obj = $("#" + objStr);

    var currObj = $(Obj).find("option:selected");
    var returnStr = currObj.text();

    if (c !== undefined) {
        if (currObj.val() == "T5") {
            TempMonth = c.newdate.M + "月" + c.newdate.d + "日";
            returnStr = returnStr.replace("X月X日", TempMonth); //文字
            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
        } else {
            TempMonth = c.newdate.M + "月" //选择后的几月
            returnStr = returnStr.replace("X月", TempMonth); //文字
            $("#rpt_info_back,#rpt_info_return_back").val(returnStr);
        }
    }


}


//未完成编辑
function SaveMissTargetRpt(obj) {
    //info ：代表的是累计，currentInfo：代表的是当月 
    if (info != null && currentInfo != null) {

        if (obj == "all") {  //编辑所有的项

            currentInfo.CurrentMIssTargetReason = info.CurrentMIssTargetReason =  info.MIssTargetReason =  $("#rpt_info_step").val();  //未完成原因
            var pdate;

            //补回子情况
            var returnTypeSub = $("#rpt_info_returntype option:selected").val(); //补回子情况

            if (returnTypeSub != "TT") {

                currentInfo.ReturnType_Sub = info.ReturnType_Sub = returnTypeSub;
            } else {
                alert("请先选择子状况");
                return false;
            }

            var PromissDate;

            if (returnTypeSub == "T5") //如果子情况是“X月X日已经补回”
            {
                PromissDate = $("#rpt_info_PromissDate_day").val(); // 显示“天”的展示
                if (PromissDate != "") {
                    currentInfo.PromissDate =  info.PromissDate = new Date(PromissDate.replace("-", "//").replace("-", "//")).toDateString();
                }
                else {
                    alert("承诺补回期限为必填项");
                    return false;
                }
            }
            else {

                //预计年内无法补回
                if (returnTypeSub == "T4") {
                    if ($("#rpt_info_return_PromiseTarget").val() == "") {
                        alert("请填写全年必保金额");
                        return false;
                    }
                    if ($v.isNumber($("#rpt_info_return_PromiseTarget").val()) == false) {
                        alert("请输入数字");
                        return false;
                    }

                }

                PromissDate = $("#rpt_info_PromissDate").val();  //显示“月”的展示
                //承诺时间
                if (PromissDate != "") {
                    if (PromissDate.length > 7) {
                        pdate = new Date(PromissDate.replace("-", "//"));
                    } else { pdate = new Date(PromissDate.replace("-", "//") + "/1 0:00:00"); }

                    currentInfo.PromissDate =  info.PromissDate = pdate.toDateString();
                } else {
                    alert("承诺补回期限为必填项");
                    return false;
                }
            }
            currentInfo.CurrentMIssTargetDescription = info.CurrentMIssTargetDescription =  info.MIssTargetDescription = "\n" + $("#rpt_info_desc").val(); //采取措施
            currentInfo.ReturnDescription = info.ReturnDescription = $("#rpt_info_back").val(); //补回情况



        } else if (obj == "Reason") { //编辑未完成原因，采取措施

            currentInfo.CurrentMIssTargetReason = info.CurrentMIssTargetReason = info.MIssTargetReason = $("#rpt_info_Reason_step").val();  //未完成原因
            currentInfo.CurrentMIssTargetDescription = info.CurrentMIssTargetDescription =  info.MIssTargetDescription = "\n" + $("#rpt_info_Reason_desc").val(); //采取措施

        } else if (obj == "retuen") {  //编辑补回情况
            var pdate;
            //承诺时间
            if ($("#rpt_info_return_PromissDate").val() != "") {
                if ($("#rpt_info_return_PromissDate").val().length > 7) {
                    pdate = new Date($("#rpt_info_return_PromissDate").val().replace("-", "//"));
                } else { pdate = new Date($("#rpt_info_return_PromissDate").val().replace("-", "//") + "/1 0:00:00"); }
                currentInfo.PromissDate =  info.PromissDate = pdate.toDateString(); //承诺时间
            } else {
                alert("承诺补回期限为必填项");
                return false;
            }

            currentInfo.ReturnDescription = info.ReturnDescription = $("#rpt_info_return_back").val(); //补回情况

            var returnTypeSub = $("#rpt_info_return_returntype option:selected").val(); //补回子情况

            if (returnTypeSub != "TT") {
                if (returnTypeSub == "T4") {
                    if ($("#rpt_info_return_PromiseTarget").val() == "") {
                        alert("请填写全年必保金额");
                        return false;
                    }

                    if ($v.isNumber($("#rpt_info_return_PromiseTarget").val()) == false) {
                        alert("请输入数字");
                        return false;
                    }
                }
                currentInfo.ReturnType_Sub = info.ReturnType_Sub = returnTypeSub;
            } else {

                alert("请先选择子状况");
                return false;
            }

        }

        WebUtil.ajax({
            async: true,
            url: "/TargetReportedControll/ModifyMissTargetRptInfo",
            args: { info: WebUtil.jsonToString(info), IncludeHaveDetail: IncludeHaveDetail },
            successReturn: function (result) {

            }
        });

        //重新绑定对象
        if (MissTargetData != undefined) {
            //加载数据
            TmplMissTargetData(MissTargetData, true);
            TmplCurrentMissTargetData(CurrentMissTargetData, true);
        }

    } else
    {


        if (obj == "all") {  //编辑所有的项
            
            if (DownLoadTag == "missTargetReport") // 代表的是累计的
            {
                info.MIssTargetReason = "\n" + $("#rpt_info_step").val();  //未完成原因
                var pdate;
                //补回子情况
                var returnTypeSub = $("#rpt_info_returntype option:selected").val(); //补回子情况
                if (returnTypeSub != "TT") {

                    info.ReturnType_Sub = returnTypeSub;
                } else {
                    alert("请先选择子状况");
                    return false;
                }
                var PromissDate;
                if (returnTypeSub == "T5") //如果子情况是“X月X日已经补回”
                {
                    PromissDate = $("#rpt_info_PromissDate_day").val(); // 显示“天”的展示
                    if (PromissDate != "") {
                        info.PromissDate = new Date(PromissDate.replace("-", "//").replace("-", "//")).toDateString();
                    }
                    else {
                        alert("承诺补回期限为必填项");
                        return false;
                    }
                }
                else {

                    //预计年内无法补回
                    if (returnTypeSub == "T4") {
                        if ($("#rpt_info_return_PromiseTarget").val() == "") {
                            alert("请填写全年必保金额");
                            return false;
                        }
                        if ($v.isNumber($("#rpt_info_return_PromiseTarget").val()) == false) {
                            alert("请输入数字");
                            return false;
                        }

                    }

                    PromissDate = $("#rpt_info_PromissDate").val();  //显示“月”的展示
                    //承诺时间
                    if (PromissDate != "") {
                        if (PromissDate.length > 7) {
                            pdate = new Date(PromissDate.replace("-", "//"));
                        } else { pdate = new Date(PromissDate.replace("-", "//") + "/1 0:00:00"); }

                        info.PromissDate = pdate.toDateString();
                    } else {
                        alert("承诺补回期限为必填项");
                        return false;
                    }
                }
                info.MIssTargetDescription = "\n" + $("#rpt_info_desc").val(); //采取措施
                info.ReturnDescription = $("#rpt_info_back").val(); //补回情况
            } else
            {
                //这是代表的当月的编辑
                info.CurrentMIssTargetReason = $("#rpt_info_step").val();  //未完成原因
                info.CurrentMIssTargetDescription = "\n" + $("#rpt_info_desc").val(); //采取措施

            }


        } else if (obj == "Reason") { //编辑未完成原因，采取措施
            if (DownLoadTag == "missTargetReport") // 代表的是累计的
            {
                info.MIssTargetReason =  $("#rpt_info_Reason_step").val();  //未完成原因
                info.MIssTargetDescription = "\n" + $("#rpt_info_Reason_desc").val(); //采取措施
            } else
            {

                //不得已啊，谁叫这两个Tmpl加载的时候穿插了。
                if ($("#rpt_info_Reason_step").val() == undefined) {
                    //这是代表的当月的编辑
                    info.CurrentMIssTargetReason = $("#rpt_info_step").val();  //未完成原因
                    info.CurrentMIssTargetDescription = "\n" + $("#rpt_info_desc").val(); //采取措施
                } else
                {
                    info.CurrentMIssTargetReason = $("#rpt_info_Reason_step").val();  //未完成原因
                    info.CurrentMIssTargetDescription = "\n" + $("#rpt_info_Reason_desc").val(); //采取措施
                }
            }
        } else if (obj == "retuen") {  //编辑补回情况
            var pdate;
            //承诺时间
            if ($("#rpt_info_return_PromissDate").val() != "") {
                if ($("#rpt_info_return_PromissDate").val().length > 7) {
                    pdate = new Date($("#rpt_info_return_PromissDate").val().replace("-", "//"));
                } else { pdate = new Date($("#rpt_info_return_PromissDate").val().replace("-", "//") + "/1 0:00:00"); }
                info.PromissDate = pdate.toDateString(); //承诺时间
            } else {
                alert("承诺补回期限为必填项");
                return false;
            }

            info.ReturnDescription = $("#rpt_info_return_back").val(); //补回情况

            var returnTypeSub = $("#rpt_info_return_returntype option:selected").val(); //补回子情况

            if (returnTypeSub != "TT") {
                if (returnTypeSub == "T4") {
                    if ($("#rpt_info_return_PromiseTarget").val() == "") {
                        alert("请填写全年必保金额");
                        return false;
                    }

                    if ($v.isNumber($("#rpt_info_return_PromiseTarget").val()) == false) {
                        alert("请输入数字");
                        return false;
                    }
                }
                info.ReturnType_Sub = returnTypeSub;
            } else {

                alert("请先选择子状况");
                return false;
            }

        }

        WebUtil.ajax({
            async: true,
            url: "/TargetReportedControll/ModifyMissTargetRptInfo",
            args: { info: WebUtil.jsonToString(info), IncludeHaveDetail: IncludeHaveDetail },
            successReturn: function (result) {

            }
        });

        //重新绑定对象
        if (MissTargetData != undefined) {
            //加载数据
            TmplMissTargetData(MissTargetData, true);
            TmplCurrentMissTargetData(CurrentMissTargetData, true);
        }

    }

    art.dialog({ id: 'divMissTargetRpt' }).close();
    art.dialog({ id: 'divMissTargetRpt_Reason' }).close();
    art.dialog({ id: 'divMissTargetRpt_Retu' }).close();

    var obj = $("#Tab_MissTargetHead");
    var tab = $("#Tbody_MissTargetData");
    FloatHeader(obj, tab, false, "Reported")

}

//根据ID取json对象
function GetInfoByID(sender, tag) {

    if (tag == 'current') // 如果tag的标签是 ‘current’代表的是当前月，反之则是累计的
    {
        A(CurrentMissTargetData, sender);
    } else {
        A(MissTargetData, sender); //如果是累计的时候编辑，同时改变当前月的数据
        B(CurrentMissTargetData, sender);
    }

    return info.ID;
}

var info = null;

function A(o, id) {
    for (var i = 0; i < o.length; i++) {
        if (o[i].Mark != "Data") {
            A(o[i].ObjValue, id);
        }
        else {
            for (var j = 0; j < o[i].ObjValue.length; j++) {
                if (o[i].ObjValue[j].ID == id) {
                    info = o[i].ObjValue[j];
                    break;
                }
            }
        }
    }
}

var currentInfo = null;
//这里主要编辑 当月未完成的
function B(o, id) {
    for (var i = 0; i < o.length; i++) {
        if (o[i].Mark != "Data") {
            B(o[i].ObjValue, id);
        }
        else {
            for (var j = 0; j < o[i].ObjValue.length; j++) {
                if (o[i].ObjValue[j].ID == id) {
                    currentInfo = o[i].ObjValue[j];
                    break;
                }
            }
        }
    }
}




//收缩
function ShouSuo(sender) {
    if (sender == 'YC') {
        $(".shangyue").hide();
        $(".TT2").attr("colspan", 3);
        $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
        $(".Level1TdSp1").attr("colspan", 11);
        $("#Tab_MissTarget").removeAttr("style");

        $('#CurrentMonthMissTergetDiv').text("本月累计(万元)  [+]");

    } else if (sender == 'XS') {

        if ($(".newdiff_miss").is(":hidden")) {
            if ($(".Level1TdSp1").attr("colspan").toInt() == 10) {
                $(".shangyue").show();
                $(".TT2").attr("colspan", 4);
                $(".Special").removeClass("Td_Right").addClass("Td_TopAndBottom"); //当出现完成率的时候，差值TD是没有右面的边线的
                $(".Level1TdSp1").attr("colspan", 14);
                //$("#Tab_MissTarget").attr({ style: "table-layout: fixed" });
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
                // $("#Tab_MissTarget").attr({ style: "table-layout: fixed" });
                $('#CurrentMonthMissTergetDiv').text("本月累计(万元)");
            } else {
                $(".shangyue").hide();
                $(".TT2").attr("colspan", 3);
                $(".Special").removeClass("Td_TopAndBottom").addClass("Td_Right"); //当影藏完成率的时候，差值TD是有右面的边线的
                $(".Level1TdSp1").attr("colspan", 11);
                $("#Table1").removeAttr("style");

                $('#CurrentMonthMissTergetDiv').text("本月累计(万元)  [+]");
            }
        }
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


//下载数据
function DownExcel(sender) {
    //window.open("/AjaxHander/DownLoadView.ashx?FileType=MissTargetRpt&SysId=" + sysID + "&MonthReportID=" + $("#hideMonthReportID").val() + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth);

    //下载不同的未完成明细数据
    if (DownLoadTag == "missTargetReport") {
        window.open("/AjaxHander/DownLoadView.ashx?FileType=MissTargetRpt&SysId=" + sysID + "&MonthReportID=" + $("#hideMonthReportID").val() + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth);
    }
    else if (DownLoadTag == "missCurrentTargetReport") {
        window.open("/AjaxHander/DownLoadView.ashx?FileType=missCurrentTargetReport&SysId=" + sysID + "&MonthReportID=" + $("#hideMonthReportID").val() + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth);
    }



}

//完成情况明细-------------------------------------------------------------------------------

//加载完成情况明细数据
function getMonthReprotDetailData() {

    //加载月度报告说明
    WebUtil.ajax({
        async: true,
        url: "/TargetReportedControll/GetTargetDetailList",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strCompanyProperty: "", strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: function (ResultData) {
            if (ResultData) {
                ComplateDetailData = ResultData;
                SetComplateTargetDetailData(ComplateDetailData[0], 1);

            }
        }
    });

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


//查找要编辑的对象
function SearchMonthReportDetail(sender) {
    GetMonthReportDetailInfo(ComplateDetailData, sender);
    GetReportInstanceMonthReportDetail(ReportInstance, sender);
}


//查找ComplateDetailData的对象并为变量赋值
var detail = null;
function GetMonthReportDetailInfo(details, ID) {
    if (details != null) {
        for (var i = 0; i < details.length; i++) {
            if (details[i].Mark != "Data") {
                GetMonthReportDetailInfo(details[i].ObjValue, ID);
            }
            else {
                if (details[i].ObjValue != null) {
                    for (var j = 0; j < details[i].ObjValue.length; j++) {
                        if (details[i].ObjValue[j].ProCompayID == ID) {
                            detail = details[i].ObjValue[j];
                            break;
                        }
                    }
                }
            }
        }
    }
}


//查找ReportInstance中编辑的对象并为变量赋值
var reportDetail = null;
function GetReportInstanceMonthReportDetail(details, ID) {
    for (var i = 0; i < details.ReportDetails.length; i++) {
        if (details.ReportDetails[i].ID == ID) {
            reportDetail = details.ReportDetails[i];
            break;
        }
    }
}

//编辑完成情况明细信息
var tempEditType = 'single';//编辑类型

function EditMonthReportDetail(sender, EditType) {
    tempEditType = EditType;
    SearchMonthReportDetail(sender);
    currentDetailTarget = $("#Ul4 li .active_sub3 ");
    $("#MonthReportDetailContent_Edit").empty();
    if (detail != null) {
        loadTmpl_2("#MonthReportDetil_info_tmpl_Pro").tmpl(detail).appendTo($("#MonthReportDetailContent_Edit"));

        art.dialog({
            content: $("#MonthReportDetailContent_Edit").html(),
            lock: true,
            id: 'divMonthReportDetail',
            title: '<span>月度经营上报--编辑</span>'
        });
    }
}


//保存明细项数据
function SaveMonthReportDetail() {

    if (detail == null && reportDetail == null) {
        return;
    }

    if (tempEditType == 'all') {

        //循环该公司的 所有指标
        for (var i = 0; i < detail.ProjectTargets.length; i++) {
            //CurrentAmmount${ProTargetID} ,当月的实际值
            //Accumulative${ProTargetID} ,当月的累计值

            var Txt_Current_ID = "#CurrentAmmount" + detail.ProjectTargets[i].ProTargetID;
            var Txt_Accumulative_ID = "#Accumulative" + detail.ProjectTargets[i].ProTargetID;

            if ($v.isRequired($(Txt_Current_ID).val()) == false || $v.isRequired($(Txt_Accumulative_ID).val()) == false)
            { return alert("必填项！") };

            if ($v.isNumber($(Txt_Current_ID).val()) == false || $v.isNumber($(Txt_Accumulative_ID).val()) == false) {
                return alert("请输入数字");
            }

            detail.ProjectTargets[i].NActualAmmount = $(Txt_Current_ID).val();
            detail.ProjectTargets[i].NAccumulativeActualAmmount = $(Txt_Accumulative_ID).val();
        }
    }
    art.dialog({ id: 'divMonthReportDetail' }).close();
    WebUtil.ajax({
        async: true,
        url: "/TargetReportedControll/UpdateProMonthReportDetail",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strMonthReportOrderType: MonthReportOrderType, info: WebUtil.jsonToString(detail), strMonthReportID: MonthReportID, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: SplitData
    });
}


//为完成情况明细排序（本月排序和年累计排序）
var currentDetailTarget = null;
function MonthReportOrder(sender) {
    if (sender == "Detail") {
        $("#imgDetailMonthly").attr("src", "../Images/btn_down02_w.png")
        $("#imgDetail").attr("src", "../Images/btn_down03_w.png")
    } else {
        $("#imgDetailMonthly").attr("src", "../Images/btn_down03_w.png")
        $("#imgDetail").attr("src", "../Images/btn_down02_w.png")
    }
    currentDetailTarget = $("#Ul4 li .active_sub3 ");
    MonthReportOrderType = sender;
    getMonthReprotDetailData();
}




//下载计划指标模板
function DownLoadTargetPlanExcel(sender) {
    window.open("/AjaxHander/DownLoadTargetTemplate.ashx?FileType=DownProTargetPlan&SysId=" + sysID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth);
}

function DownExcelMonthReport() {
    window.open("/AjaxHander/DownLoadTargetTemplate.ashx?FileType=DownProMonthReport&SysId=" + sysID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&MonthReportID=" + MonthReportID);
}


$(function () {
    var error = 0; // 这里是2个上传控件，但是功能是一样的
  
});


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

//行 显示影藏
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

//行 显示影藏
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







function unfoldTitle() {
    $("#CompleteDetailHead").html("");
    $("#tab2_rows").html("");

    //加载表头
    loadTmpl_2('#TmplProCompany_Head_All').tmpl().appendTo('#CompleteDetailHead'); //加载列头 

    if (ComplateDetailData[0].ObjValue.length > 4) {
        loadTmpl_2('#ProCompanyDetail_Data_All').tmpl(ComplateDetailData[0]).appendTo('#tab2_rows');

        $("#importedDataTable2").css("width", "2500");
    }

    var obj = $("#CompleteDetailHead");
    var tab = $("#tab2_rows");
    FloatHeader(obj, tab, false);
}


function shrinkageTitle() {
    $("#CompleteDetailHead").empty();
    $("#tab2_rows").empty();

    //加载表头
    loadTmpl_2("#TmplProCompany_South_Head").tmpl().appendTo("#CompleteDetailHead"); //加载列头 

    if (ComplateDetailData[0].ObjValue.length > 4) {
        loadTmpl_2("#TmplProCompanyDetail_South_Data").tmpl(ComplateDetailData[0]).appendTo("#tab2_rows");

        $("#importedDataTable2").css("width", "100%");
    }


    var obj = $("#CompleteDetailHead");
    var tab = $("#tab2_rows");
    FloatHeader(obj, tab, false);
}