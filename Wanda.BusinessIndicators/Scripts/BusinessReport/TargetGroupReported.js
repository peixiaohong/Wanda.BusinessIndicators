//变量误删
var ReturnData = {};
var MissTargetData = {};
var ComplateDetailData = {};
var MonthReportData = {};
var ReportInstance;
var sysID;
var FinYear;
var FinMonth;
var MonthReportID;
var Description;
var MonthReportOrderType = "";
var IncludeHaveDetail = true;
var Upload = true;
var ItemDisplay = "missTargetReport";
var ItemDisplay1 = "missCurrentTargetReport"
var MissType ='MissTargetRpt';


//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetGroupReported.html", selector);
}
//加载模版项-------------------------------------------------------------------------
function loadTmpl_1(selector) {

    return WebUtil.loadTmpl("../BusinessReport/TargetReturnTmpl.html", selector);
}



function GetReportInstance() {

    WebUtil.ajax({
        async: true,
        url: "/TargetReportedControll/GetReportInstance",
        args: { strSystemID: sysID, strMonthReportID: MonthReportID, strMonthReportOrderType: MonthReportOrderType, IncludeHaveDetail: IncludeHaveDetail, UploadStr: Upload },
        successReturn: SplitData
    });

}

function SplitData(resultData) {
    if (resultData != null) {
        if (resultData[0] != null) {
            ReportInstance = resultData[0].ObjValue;
        }
        if (resultData[1] != null) {
            ComplateDetailData = resultData[1].ObjValue;

            $('#CompleteDetailHead').empty();
            loadTmpl('#GroupMonthReportDetailHeadTmpl').tmpl().appendTo('#CompleteDetailHead');
            $('#tab2_rows').empty();
            loadTmpl('#GroupComplateTargetDetailTemplate').tmpl(ComplateDetailData).appendTo('#tab2_rows');

            //SetComplateTargetDetailData(ComplateDetailData[0], 1);
            if (ReportInstance.ReportDetails.length > 0) {
                setStlye('missTargetReportSpan,missCurrentTargetReport,monthReportSpan,monthReportReadySpan');
            }
        }
        if (resultData[2] != null) {
            MissTargetData = resultData[2].ObjValue;
            //加载数据
            TmplMissTargetData(MissTargetData, false);
        }
        if (resultData[3] != null) {
            Description = resultData[3].ObjValue;
            $("#MonthGetDescription").html(Description);
        }
    }
}


//页面加载事件
$(function () {


    sysID = $("#ddlSystem").attr("value");
    MonthReportID = $("#hideMonthReportID").attr("value");
    FinYear = $("#hideFinYear").attr("value");
    FinMonth = $("#hideFinMonth").attr("value");
    //获取ReportInstance实例，将此实例传回后台。
    GetReportInstance();

    MissTagetExcelReport();

    //自动保存,光标离开
    $("#MonthGetDescription").blur(function () {
        $("#MonthGetDescription").css("background-color", "#FFFFFF");

        var monthRpt;

        if (ReportInstance.LastestMonthlyReport != undefined) {
            monthRpt = ReportInstance.LastestMonthlyReport;
            monthRpt.Description = $("#MonthGetDescription").html();
        }

        if (Description != $("#MonthGetDescription").html()) {
            // alert("入库");
            WebUtil.ajax({
                async: true,
                url: "/TargetReportedControll/ModifyMonthTRptDescription",
                args: { rpts: WebUtil.jsonToString(monthRpt) },
                successReturn: function (result) {

                }
            });

            Description = $("#MonthGetDescription").html();
        } else {

        }
    });

    $("#MonthGetDescription").focus(function () {
        $("#MonthGetDescription").css("background-color", "#D6D6FF");
    });

})



function MissTagetExcelReport() {
    if (DownLoadTag == "missCurrentTargetReport") {
        MissType = 'CurrentMissTargetRpt';
    } else {
        MissType = 'MissTargetRpt';
    }

    // 未完成数据上传数据
    $('#file_upload').uploadify({
        'buttonText': '导入数据',
        'width': 100,
        'height': 25,
        'successTimeout': 50, 
        'fileTypeDesc': 'office file',
        'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx',
        'fileSizeLimit': '10240',
        'swf': '../Scripts/UpLoad/uploadify.swf',
        'uploader': '../AjaxHander/ExcelReport.ashx?FileType='+ MissType +'&SysId=' + sysID + '&MonthReportID=' + MonthReportID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth,
        'formData': { "action": "未完成指标填写" },
        'onSelect': function (e, queueId, fileObj) { },
        'onUploadSuccess': function (file, data, response) {

            if (data == "") {
                getMonthReportMissTargetData(true);
                setStlye('monthReportSpan,monthReportSubmitSpan');
            } else { alert(data); }
        }
    });
}


//月报说明
function GetMonthGetDescription() {
    WebUtil.ajax({
        async: false,
        url: "/TargetReportedControll/GetMonthTRptDescription",
        args: { rpts: WebUtil.jsonToString(ReportInstance) },
        successReturn: function (result) {
            $("#MonthGetDescription").html(result);
            Description = result;
        }
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


//未完成 tab（未完成说明）--------------------------------------------------------------------------------------------
function getMonthReportMissTargetData(Upload) {

    //未完成说明
    WebUtil.ajax({
        async: false,
        url: "/TargetReportedControll/GetMissTargetList",
        args: { rpts: "", monthRptID: $("#hideMonthReportID").val(), UploadStr: Upload },
        successReturn: function (result) {
            MissTargetData = result;

            //加载数据
            TmplMissTargetData(MissTargetData, false);
        }
    });

}


var isUL = false;

//加载未完成明细Tmpl数据
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
        $(".newdiff_miss").hide(); //商管的
        $(".Level1TdSp1").attr("colspan", 10);

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
    loadTmpl('#' + ComplateTargetDetailTemplate).tmpl(TemplData).appendTo('#tab2_rows');
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
        loadTmpl('#' + strComplateMonthReportDetilHtmlTemplate[0]).tmpl().appendTo('#CompleteDetailHead');

    } else {
        loadTmpl('#CompleteDetailHeadTemplate').tmpl().appendTo('#CompleteDetailHead');
    }


    //tmpl模板名称
    if (strComplateMonthReportDetilHtmlTemplate[2] != "" && strComplateMonthReportDetilHtmlTemplate[2] != undefined) {
        ComplateTargetDetailTemplate = strComplateMonthReportDetilHtmlTemplate[2];
    } else {
        ComplateTargetDetailTemplate = "TargetReportedComplateTargetDetailTemplate"
    }

    if (currentDetailTarget == null) {
        $("#Ul4").empty();
        loadTmpl('#TargetReportedComplateTargetDetailHeadTemplate').tmpl(ComplateDetailData).appendTo('#Ul4');
        $("#tab2_rows").empty();
        loadTmpl('#' + ComplateTargetDetailTemplate).tmpl(sender).appendTo('#tab2_rows');
        $("#Ul4 :first a").addClass("active_sub3");
    } else {
        if (Type == 1) {
            ComplateDetailLiaddCss(currentDetailTarget);
        }
    }

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
function EditMissTargetRpt(sender, obj) {
    GetInfoByID(sender);

    currentMissTarget = $("#U2 li .active_sub3 ");

    if (info != null) {

        if (obj == "all") { // 编辑所有项
            $("#content_edit").empty();
            $("#rpt_info_tmpl").tmpl(info).appendTo($("#content_edit"));
            $("#rpt_info_returntype option[value='" + info.ReturnType + "']").attr("selected", true);
            art.dialog({
                content: $("#divMissTargetRpt").html(),
                lock: true,
                id: 'divMissTargetRpt',
                title: '<span>月度经营上报--编辑</span>'
            });
        } else if (obj == "Reason") { //只编辑未完成原因，措施
            $("#content_edit_Reason").empty();
            $("#rpt_info_Reason_tmpl").tmpl(info).appendTo($("#content_edit_Reason"));
            art.dialog({
                content: $("#divMissTargetRpt_Reason").html(),
                lock: true,
                id: 'divMissTargetRpt_Reason',
                title: '<span>月度经营上报--编辑</span>'
            });
        }
        else if (obj == "return") { //值编辑补回情况
            $("#content_edit_retuen").empty();
            $("#rpt_info_return_tmpl").tmpl(info).appendTo($("#content_edit_retuen"));
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



//未完成编辑
function SaveMissTargetRpt(obj) {
    if (info != null) {

        if (obj == "all") {  //编辑所有的项

            
            info.MIssTargetReason = "\n" + $("#rpt_info_step").val();  //未完成原因
            var pdate;
            //承诺时间
            if ($("#rpt_info_PromissDate").val() != "") {
                if ($("#rpt_info_PromissDate").val().length > 7) {
                    pdate = new Date($("#rpt_info_PromissDate").val().replace("-", "//"));
                } else { pdate = new Date($("#rpt_info_PromissDate").val().replace("-", "//") + "/1 0:00:00"); }

                info.PromissDate = pdate.toDateString();
            } else {
                alert("承诺补回期限为必填项");
                return false;
            }
            info.MIssTargetDescription = "\n" + $("#rpt_info_desc").val(); //采取措施
            info.ReturnDescription = $("#rpt_info_back").val(); //补回情况

            if (info.ReturnType < 5) { //新增 已补回 提前补回 不可编辑
                if ($("#rpt_info_returntype") != undefined) { //补回状态
                    info.ReturnType = $("#rpt_info_returntype option:selected").val();
                }
            }

        } else if (obj == "Reason") { //编辑未完成原因，采取措施

            info.MIssTargetReason = "\n" + $("#rpt_info_Reason_step").val();  //未完成原因
            info.MIssTargetDescription = "\n" + $("#rpt_info_Reason_desc").val(); //采取措施

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

            if (info.ReturnType < 5) { //新增 已补回 提前补回 不可编辑
                if ($("#rpt_info_return_returntype") != undefined) { //补回状态
                    info.ReturnType = $("#rpt_info_return_returntype option:selected").val();
                }
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
        }
    }

    art.dialog({ id: 'divMissTargetRpt' }).close();
    art.dialog({ id: 'divMissTargetRpt_Reason' }).close();
    art.dialog({ id: 'divMissTargetRpt_Retu' }).close();
    //取消按钮事件
    //if (obj == "all") {
    //    art.dialog({ id: 'divMissTargetRpt' }).close();
    //} else if (obj == "Reason") {
    //    art.dialog({ id: 'divMissTargetRpt_Reason' }).close();
    //} else if (obj == "retuen") {
    //    art.dialog({ id:'divMissTargetRpt_retuen'}).close();
    //}
}


//根据ID取json对象
function GetInfoByID(sender) {
    A(MissTargetData, sender);
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


//下载数据
function DownExcel(sender) {
    window.open("/AjaxHander/DownLoadView.ashx?FileType=MissTargetRpt&SysId=" + sysID + "&MonthReportID=" + $("#hideMonthReportID").val() + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth);
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
var lstDetail = null;
function GetMonthReportDetailInfo(details, ID) {
    if (details != null) {
        for (var i = 0; i < details.length; i++) {
            if (details[i].Name == "Group") {
                GetMonthReportDetailInfo(details[i].ObjValue, ID)
            }
            if (details[i].ID == ID || details[i].ObjValue!=null) {
                if (details[i].ID == ID) {
                    if (details[i].Value != null) {
                        lstDetail = details[i];
                        break;
                    } else {
                        lstDetail = details[i];

                        break;
                    }
                } else {
                    GetMonthReportDetailInfo(details[i].ObjValue, ID)
                }
            }
           
        }
    }
}


//查找ReportInstance中编辑的对象并为变量赋值
var lstReportDetail = new Array(); 
function GetReportInstanceMonthReportDetail(details, ID) {
    for (var i = 0; i < details.ReportDetails.length; i++) {
        if (details.ReportDetails[i].CompanyID == ID) {
            lstReportDetail.push(details.ReportDetails[i]);
        }
        if (details.ReportDetails[i].TargetID == ID)
        {
            lstReportDetail.push(details.ReportDetails[i]);
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
    if (lstDetail != null) {
        loadTmpl("#MonthReportDetil_info_tmpl_Group").tmpl(lstDetail).appendTo($("#MonthReportDetailContent_Edit"));
        art.dialog({
            content: $("#MonthReportDetailContent_Edit").html(),
            lock: true,
            id: 'divMonthReportDetail',
            title: '<span>月度经营上报--编辑</span>'
        });
    }
    var InnerCompanyName = $("#SpanComanyName").html();//取出编辑框内的公司名称,如果公司名称为空,则隐藏公司名称
    if (InnerCompanyName == "" || InnerCompanyName == null) {
        $("#ChangeComName").hide();
    }
}


//保存明细项数据
function SaveMonthReportDetail() {

    if (lstDetail == null && lstReportDetail == null) {
        return;
    } else {
        var tempList;
        if (lstDetail.Value != null) {
            for (var i = 0; i < lstDetail.Value.length; i++)
            {
                var TargetPlanAmmount = $("#TargetPlanAmmount" + lstDetail.Value[i].ID).attr("value");
                var TargetAccumulative = $("#TargetAccumulative" + lstDetail.Value[i].ID).attr("value");
                if (!isNaN(TargetPlanAmmount) && !isNaN(TargetAccumulative)
                    && TargetPlanAmmount != "-0" && TargetAccumulative != "-0"
                    && TargetPlanAmmount != "" && TargetAccumulative != ""
                    && TargetPlanAmmount != null && TargetAccumulative != null) {
                    lstDetail.Value[i].NAccumulativePlanAmmount = TargetPlanAmmount;
                    lstDetail.Value[i].NAccumulativeActualAmmount = TargetAccumulative;
                }
            }
            for (var j = 0; i < lstReportDetail.length; i++)
            {
                var TargetPlanAmmount = $("#TargetPlanAmmount" + lstReportDetail[i].ID).attr("value");
                var TargetAccumulative = $("#TargetAccumulative" + lstReportDetail[i].ID).attr("value");
                if (!isNaN(TargetPlanAmmount) && !isNaN(TargetAccumulative)
                    && TargetPlanAmmount != "-0" && TargetAccumulative != "-0"
                    && TargetPlanAmmount != "" && TargetAccumulative != ""
                    && TargetPlanAmmount != null && TargetAccumulative != null) {
                    lstReportDetail[i].NAccumulativePlanAmmount = TargetPlanAmmount;
                    lstReportDetail[i].NAccumulativeActualAmmount = TargetAccumulative;
                } 
            }
        } else {

            for (var i = 0; i < lstDetail.ListGroupTargetDetail.length; i++) {
                var CompanyPlanAmmount = $("#CompanyPlanAmmount" + lstDetail.ListGroupTargetDetail[i].ID).attr("value");
                var CompanyAccumulative = $("#CompanyAccumulative" + lstDetail.ListGroupTargetDetail[i].ID).attr("value");
                if (!isNaN(CompanyPlanAmmount) && !isNaN(CompanyAccumulative)
                    && CompanyPlanAmmount != "-0" && CompanyAccumulative != "-0"
                    && CompanyPlanAmmount != "" && CompanyAccumulative != ""
                    && CompanyPlanAmmount != null && CompanyAccumulative != null) {
                    lstDetail.ListGroupTargetDetail[i].NAccumulativePlanAmmount = CompanyPlanAmmount;
                    lstDetail.ListGroupTargetDetail[i].NAccumulativeActualAmmount = CompanyAccumulative;
                } 
            }

            for (var j = 0; i < lstReportDetail.length; i++) {
                var CompanyPlanAmmount = $("#CompanyPlanAmmount" + lstReportDetail[i].ID).attr("value");
                var CompanyAccumulative = $("#CompanyAccumulative" + lstReportDetail[i].ID).attr("value");
                if (!isNaN(CompanyPlanAmmount) && !isNaN(CompanyAccumulative)
                    && CompanyPlanAmmount != "-0" && CompanyAccumulative != "-0"
                    && CompanyPlanAmmount != "" && CompanyAccumulative != ""
                    && CompanyPlanAmmount != null && CompanyAccumulative !=null) {
                    lstReportDetail[i].NAccumulativePlanAmmount = CompanyPlanAmmount;
                    lstReportDetail[i].NAccumulativeActualAmmount = CompanyAccumulative;
                }
            }

        }
    }
    art.dialog({ id: 'divMonthReportDetail' }).close();
    WebUtil.ajax({
        async: true,
        url: "/TargetReportedControll/UpGroupdateMonthReportDetail",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strMonthReportOrderType: MonthReportOrderType, info: WebUtil.jsonToString(lstDetail), strMonthReportID: MonthReportID, IncludeHaveDetail: IncludeHaveDetail },
        successReturn: SplitData
    });
}




//下载计划指标模板
function DownLoadTargetPlanExcel(sender) {
    window.open("/AjaxHander/DownLoadTargetTemplate.ashx?FileType=DownGroupTargetPlan&SysId=" + sysID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth+"&MonthReportID=" + MonthReportID);

}


function DownExcelMonthReport() {
    window.open("/AjaxHander/DownLoadTargetTemplate.ashx?FileType=DownGroupMonthReport&SysId=" + sysID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth +"&MonthReportID=" + MonthReportID);
}

$(function () {
    var error = 0;
    $('#file1,#file2').uploadify({
        'buttonText': '导入数据',
        'width': 100,
        'height': 25,
        'fileTypeDesc': 'office file',
        'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx;',
        'fileSizeLimit': '10240',
        //swf文件路径
        'swf': '../Scripts/UpLoad/uploadify.swf',
        //后台处理页面
        'uploader': '/AjaxHander/UpLoadMonthTargetDetail.ashx?FileType=UpGroupTargetPlan&SysId=' + sysID + '&MonthReportID=' + MonthReportID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth,
        'onUploadSuccess': function (file, data, response) {
            error = data;
            if (data == "" || data == null) {
                GetReportInstance();
                if (ReportInstance != null) {
                    if (ReportInstance.ReportDetails.length > 0) {
                        $("#T2,#UpLoadDataDiv").show();
                        setStlye('missTargetReportSpan,missCurrentTargetReport,monthReportSpan,monthReportReadySpan');
                    }
                }
            } else {
                alert(data);
            }
        },
        'onUploadComplete': function () {
            if (error == 0) {
                if (ReportInstance != null) {
                    if (ReportInstance.ReportDetails.length > 0) {
                        $("#UpLoadData").hide();
                    }
                }
            }
        },
        'onUploadError': function (file, data, response) {
            alert("上传失败，程序出错！");
        }
    });
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

