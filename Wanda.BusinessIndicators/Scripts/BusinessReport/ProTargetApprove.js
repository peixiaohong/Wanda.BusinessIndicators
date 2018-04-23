
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
var _BusinessID;

var IsNewDataIndex = "";
var MonthReportOrderType = "Detail";
var CompanyProperty = "";
var IncludeHaveDetail = false;
var ProType = "";

//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetProRpt.html", selector);
}

//加载模版项-------------------------------------------------------------------------
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/ProTargetReturnTmpl.html", selector);
}

//初始化数据方法
$(document).ready(function () {
    //月度经营报告
    SystemID = $("#hideSystemID").attr("value"); //系统ID
    MonthReportID = $("#hideMonthReportID").attr("value"); //月报ID
    _BusinessID = $("#HiddBusinessID").val(); //流程ID
    ProType = $("#HideProType").val(); //判断是否是批次

    var date = new Date;
    Year = date.getFullYear();
    Month = date.getMonth() - 1;
    IsLatestVersion = true;

    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    GetProcess(_BusinessID);


    //月度经营报告
    getMonthReportSummaryData(false);
    $(".DownExcelDiv").hide();//上来就隐藏

});


function AddStr(val) {
    return val + "%";
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
//不同的系统转换不同的表头
function ChangeTabelHead(TabelHeadID, val) {
    var tabelHead;
    if (val == "XM") {

    } else if (val == "ZG") {

    } else if (val == "JT") {

    } else {
        if (TabelHeadID == "CompleteDetailHead") {
            tabelHead = "<tr class='tab_5_row_alt'><th colspan='3' rowspan='2' style='width:17%'>公司名称</th><th colspan='4' style='width:40%'> <a href='javascript:MonthReportOrder(&quot;DetailMonthly&quot;)' style='text-decoration:none;color:#ffffff' >本月发生(万元) <img ID='imgDetailMonthly'  src='../Images/btn_down02_w.png' /> </a></th><th colspan='4' style='width:35%'><a href='javascript:MonthReportOrder(&quot;Detail&quot;)' style='text-decoration:none;color:#ffffff'>本年累计(万元) <img id='imgDetail'  src='../Images/btn_down03_w.png' /> </a></th><th rowspan='2' style='13%'>警示灯</th></tr><tr class='tab_5_row_alt'><th class='th_Sub'>计划</th><th class='th_Sub'>实际</th><th class='th_Sub'>差额</th><th class='th_Sub2'>完成率</th><th class='th_Sub1'>计划</th><th class='th_Sub'>实际</th><th class='th_Sub'>差额</th><th class='th_Sub'>完成率</th></tr>";
            $("#" + TabelHeadID).empty();
            $("#" + TabelHeadID).append(tabelHead);
        } else {
            $("#" + TabelHeadID).empty();
            tabelHead = "<tr><th rowspan='2' style='width: 4%'>序号</th><th rowspan='2'>项目</th><th rowspan='2' style='width: 10%'>年度指标</th><th colspan='3' style='width: 35%'>本月发生(万元)</th><th colspan='3' style='width: 35%'>本年累计(万元)</th></tr><tr><th class='th_Sub'>计划</th><th class='th_Sub'>实际</th><th class='th_Sub2'>完成率</th><th class='th_Sub1'>计划</th><th class='th_Sub'>实际</th><th class='th_Sub'>完成率</th></tr>";
            $("#" + TabelHeadID).append(tabelHead);
        }
    }
}



//获取月份
function GetMonthStr() {
    return $("#HiddenMonth").val();
}


$(function () {
    var ShowProecessNodeName = false;
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetShowPrcessNodeName",
        args: { strSystemID: $("#ddlSystem").val(), strProcessCode: $("#HideProcessCode").val() },
        successReturn: function (resultData) {
            ShowProecessNodeName = resultData;
            $.unblockUI();
        }
    });
    var Opinions = $("#HideOpinions").val();
    var otherSetting = {
        IsShowContextMenu: true,
        PageContextMenu: true,
        EnableDebug: true,
        ShowNodeName: ShowProecessNodeName == true ? true : false,
        ButtonCssType: "middle",
        CustomerProcessLog: Opinions != "" ? JSON.parse(Opinions) : {},
        CustomerSceneSetting: {
            ShowCc: false,//是否显示抄送
            ShowFowardButton: false,//是否显示转发按钮
            AlwaysReturnToStart: true
        },
        OnAfterExecute: afterAction,
        OnSaveApplicationData: saveApplicationData
    };
    bpf_wf_client.initAjaxSetting("#process", false, otherSetting);
})

//业务系统保存数据
function saveApplicationData(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle("saveApplicationData", _BusinessID, args)
    }
}

function afterAction(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle("afterAction", _BusinessID, args)

        history.go(0);
    } 
}
function GetProcess(instanceID) {
    bpf_wf_client.getProcess(instanceID, function () {
        $.unblockUI();
    });
}


function BusinessDataHandle(ExecuteType,instanceID, args) {

    $.ajax({
        url: "/AjaxHander/ProProcessController.ashx",
        type: "post",
        async: false,
        data: {
            ExecuteType:ExecuteType,
            BusinessID: instanceID,
            OperatorType: args.OperatorType,
            strProType: ProType
        },
        success: function (result) {
            $.unblockUI();

            if (ExecuteType == "afterAction")
            {
                window.close();
            }
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
}


//切换不同的报表
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
        $(".DownExcelDiv").hide();
        //月度经营报告
        if (TransitionCondition(MonthReportData[0], "A") == true) {
            getMonthReportSummaryData();
        }

    } else if ($(sender).text() == "完成情况明细") {
        $('#T1,#T3,#T4,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv').hide();
        $('#T2').show();
        var obj = $("#CompleteDetailHead");
        var tab = $("#tab2_rows");
        FloatHeader(obj, tab);
        //完成情况明细
        if (TransitionCondition(ComplateDetailData[0], "B") == true) {
            getMonthReprotDetailData();
        }
        $(".DownExcelDiv").show();//到明细显示

    }
    else if ($(sender).text() == "当月未完成") {
        $("#T4,#T1,#T2,#T3,#MonthReportExplainDiv,#ApproveAttachDiv").hide();
        $("#T3_1").show();

        var obj = $("#Tab_MissTargetHead");
        var tab = $("#Tbody_MissTargetData");

        //未完成说明
        if (TransitionCondition(CurrentMissTargetData[0], "F") == true) {
            getCurrentMonthReportMissTargetData();
        }
        $(".DownExcelDiv").show();//到明细显示
        FloatHeader(obj, tab, false, "MonthRpt");

    } else if ($(sender).text() == "累计未完成") {
        $("#T4,#T1,#T2,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv").hide();
        $("#T3").show();

        var obj = $("#Tab_MissTargetHead");
        var tab = $("#Tbody_MissTargetData");

        //未完成说明
        if (TransitionCondition(MissTargetData[0], "C") == true) {
            getMonthReportMissTargetData();
        }
        $(".DownExcelDiv").show();//到明细显示
        FloatHeader(obj, tab, false, "MonthRpt");

    } else if ($(sender).text() == "补回情况明细") {
        $("#T4").show();
        $("#T1,#T2,#T3,#T3_1,#MonthReportExplainDiv,#ApproveAttachDiv").hide();

        var obj = $("#Tab_ReturnHead");
        var tab = $("#Tbody_Data");

        //补回情况明细
        if (TransitionCondition(ReturnData[0], "D") == true) {
            getMonthReportReturnData();
        }
        $(".DownExcelDiv").show();//到明细显示
        FloatHeader(obj, tab, false, "MonthRpt");
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
            strTemp = strTemp.replace(/\n/g, "<br/>").replace(/ /g, "&nbsp;");
            $("#txtDes").html(strTemp);
        }

        if (resultData[3] != null) {
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
            $('#ContentPlaceHolder1_panel').empty();
            loadTmpl('#MonthlyReportDetailSearhTemplate').tmpl(SearchList).appendTo('#ContentPlaceHolder1_panel');

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


function getMonthReportSummaryData(asyncBlock) {
    var block = true;
    if (asyncBlock != undefined) { block = asyncBlock; }
    WebUtil.ajax({
        async: true,
        asyncBlock: block,
        url: "/TargetApproveController/GetProReportInstance",
        args: { strSystemID: SystemID, strMonthReportID: MonthReportID, strProType: ProType, strBusinessID: $("#HiddBusinessID").val(), IsLatestVersion: IsLatestVersion },
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
    //加载月度报告说明

    WebUtil.ajax({
        async: true,
        url: "/TargetApproveController/GetDetailRptDataSource",
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

    var chk = $("#CompanyProperty1>.SearchCheckBox");
    //给多选框赋值为False
    $(chk).prop("checked", false);
    ClickCompanyProperty(chk);

   // TargetDetail(sender)

}

//加载表头和数据
function TargetDetail(sender) {
    $("#CompleteDetailHead").empty();
    $("#tab2_rows").empty();

    //加载表头
    loadTmpl("#TmplProCompany_Head").tmpl().appendTo("#CompleteDetailHead"); //加载列头 

    if (sender.ObjValue.length > 4)
    {
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
        url: "/TargetApproveController/GetTargetReturnList",
        args: { rpts: WebUtil.jsonToString(ReportInstance) },
        successReturn: function (result) {
            ReturnData = result;
            //先影藏其它标签
            //首先指标先加载
            $("#U1").empty();
            $("#Tbody_Data").empty();
            $("#Tab_ReturnHead").empty();


            //判断指标有几个分组，如果是2个一上默认选择第一个
            if (ReturnData.length > 1) {

                loadTmpl_1('#TmplTargerList').tmpl(ReturnData).appendTo('#U1');

                loadTmpl_1('#TmplHeadReturn_SG').tmpl().appendTo('#Tab_ReturnHead');

                loadTmpl_1('#TmplTargetRetu_SG').tmpl(ReturnData[0]).appendTo('#Tbody_Data');

                $(".newdiff_retu").hide();
                $(".Level1TDSL").attr("colspan", 8);
            } else {

                loadTmpl_1('#TmplHeadReturn').tmpl().appendTo('#Tab_ReturnHead');

                loadTmpl_1('#TmplTargetRetu').tmpl(ReturnData[0]).appendTo('#Tbody_Data');
                $(".newdiff_retu").show();
                $(".Level1TDSL").attr("colspan", 9);
            }

            $("#U1 :first a").addClass("active_sub3");
            $(".shangyueleiji").hide();
            $("#Tab_Return").attr({ style: "table-layout: fixed" });
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
    //$("#Tab_Return").attr({ style: "table-layout: fixed" });
    $('#CurrentMonthBackDetilDiv').text("本月累计(万元) [+]");

}


//未完成 tab（累计未完成说明）--------------------------------------------------------------------------------------------
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

        }
    });
    //给第一指标添加背景颜色

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

    if ($(sender).text().indexOf("月度经营报告") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetSummary&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
    } else if ($(sender).text().indexOf("完成情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetDetail&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
    } else if ($(sender).text().indexOf("累计未完成") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=MissTarget&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
    } else if ($(sender).text().indexOf("当月未完成") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=CurrentMissTarget&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
    }else if ($(sender).text().indexOf("回情况明细") > 1) {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=TargetReturn&SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month + "&MonthlyReportID=" + MonthReportID);
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