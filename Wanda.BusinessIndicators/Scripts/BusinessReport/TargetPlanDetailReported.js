/// <reference path="../../BusinessReport/TargetPlanDetailReportedTmpl.html" />


//变量（误删）
var sysID;
var TargetPlanID;

var TargetPlanDeailData;
var VersionName = "";

//加载模版项
function loadTmplTargetPlanDetail(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetPlanDetailReportedTmpl.html", selector);
}

function ClickItems(sender) {
    //为隐藏域赋值
    if ($("#" + sender + "Span").attr("class") == "txt") {
        $(".flow_nav .current").each(function () {
            $(this).removeClass("current");
        })
        $("#" + sender).addClass("current");
        setArrow_nLeft(sender);
        operateNav(sender);

    }
}

function setArrow_nLeft(sender) {
    switch (sender) {
        case "downLoadTemplate":
            $(".arrow_n").css("left", "80px");
            $("#PromptMessage").html("请下载填报模版。");
            $("#PromptMessage").removeClass("Tishi");
            break;
        case "dataUpload":
            $(".arrow_n").css("left", "250px");
            $("#PromptMessage").html("请使用系统提供的填报模板导入本月数据，如已导入，再次导入将覆盖当前数据。");
            $("#PromptMessage").removeClass("Tishi");
            break;
        //case "missTargetReport":
        //    $(".arrow_n").css("left", "420px");
        //    $("#PromptMessage").html("请填补回情况，未完成原因及措施。");
        //    $("#PromptMessage").removeClass("Tishi");
        //    break;
        //case "monthReport":
        //    $(".arrow_n").css("left", "590px");
        //    $("#PromptMessage").html("请填写月报说明。");
        //    $("#PromptMessage").removeClass("Tishi");
        //    break;
        case "monthReportSubmit":
            $(".arrow_n").css("left", "590px");
            $("#PromptMessage").html("请仔细选择加签，保证分管副总裁审批节点以前审批人完整。");
            $("#PromptMessage").addClass("Tishi");
            break;
        case "monthReportReady":
            $(".arrow_n").css("left", "420px");
            //$("#PromptMessage").html("请保存数据，等待其它上报人员填报。");
            $("#PromptMessage").html("请输入版本类型。");
            $("#PromptMessage").removeClass("Tishi");
            break;

    }
}

function setStlye(sender) {
    var val = sender.split(",");
    for (var i = 0; i < val.length; i++) {
        if (val[i] == 'monthReportReady') {
            var s = val[i].substring(0, val[i].length - 4)
            $("#" + s).addClass("current");
        }
        $("#" + val[i]).removeClass("txtdisable");
        $("#" + val[i]).addClass("txt");
    }
}

$(function setTitle() {
    var j = 0;
    $(".postion_num").each(function (i, item) {

        if ("undefined" != typeof ItemDisplay) {
            if (ItemDisplay + "Title" != item.id) {
                item.innerHTML = j * 1 + 1;
                j++;
            } else {
                $("#" + ItemDisplay).css("display", "none");
            }
        }

    })
    setStlye('monthReportReadySpan,downLoadTemplateSpan');
    ClickItems('monthReportReady');
});


function operateNav(sender) {
    $("#process").hide();
    switch (sender) {
        case "downLoadTemplate":
            $("#DownLoadModel").show();
            $("#UpLoadData,#Down1,#T2,#VersionName").hide();
            if (isCheckPlan())
                setStlye("dataUploadSpan");
            break;
        case "dataUpload":
            if (isCheckPlan()) {
                if (TargetPlanDeailData[0].ObjValue[0].ObjValue.length != 0) {
                    $("#Down1,#T2").show();
                    $("#DownLoadModel,#UpLoadData,#VersionName").hide();
                    var obj = $("#TargetPlanDetailHead");
                    var tab = $("#rows");
                    FloatHeader(obj, tab);
                } else {
                    $("#UpLoadData").show();
                    $("#DownLoadModel,#Down1,#T2,#VersionName").hide();
                }
            }

            break;
        case "monthReportReady": //版本类型
            $("#VersionName").show();
            $("#UpLoadData,#DownLoadModel,#Down1,#T2").hide();
            break;

        case "monthReportSubmit":
            if (isCheckPlan()) {
                $("#process").show();
                //$.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
                //后续加上 BusinessID
                GetProcess($("#HideProcessCode").val(), $("#hideTargetPlanID").val());
                $("#T2,#UpLoadDataDiv,#DownLoadModel,#Down1,#VersionName").hide();
            }

            break;

    }
}

var FlowCode = "";
$(function () {
    sysID = $("#HidSystemID").val();
    var ShowProecessNodeName = false;
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetShowPrcessNodeName",
        args: { strSystemID: sysID, strProcessCode: $("#HideProcessCode").val() },
        successReturn: function (resultData) {
            ShowProecessNodeName = resultData;
            $.unblockUI();
        }
    });
    var otherSetting = {
        OnBeforeExecute: beforeAction,
        OnSaveApplicationData: saveApplicationData,
        OnAfterExecute: afterAction,
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
    };
    bpf_wf_client.initAjaxSetting("process", false, otherSetting);
})

function isCheckPlan() {
    VersionName = $("#txt_VersionName").val();
    if (VersionName == "") {
        alert("请输入版本类型");
        ClickItems("monthReportReady");
        return false;
    }
    $("#hideVersionName").val(VersionName);
    var ret = true;
    WebUtil.ajax({
        async: false,
        url: "/TargetController/isCheckPlan",
        args: { SysID: sysID, Year: $("#HideFinYear").val(), PlanID: TargetPlanID, VersionName: VersionName },
        successReturn: function (resultData) {
            $.unblockUI();
          
            if (!resultData.success) {
                alert('当前版本已纯在审批版本，请勿重复操作！');
                TargetPlanID = resultData.TargetPlanID;
                //$("#txt_VersionName").val("");
                ClickItems("monthReportReady");
                GetTargetPlanDetail();
                ret = false;
            }
        }
    });
    return ret;
}

function GetProcess(key, instanceID) {
    FlowCode = key;
    var businessID = bpf_wf_tool.getQueryString("BusinessID");
    if (businessID != "") {
        bpf_wf_client.getProcess(businessID, function () {

        });
    }
    else {
        bpf_wf_client.exist(instanceID, function () {
            bpf_wf_client.getProcess(instanceID, function () {
                $.unblockUI();
            })
        }, function () {
            bpf_wf_client.createProcess({
                FlowCode: FlowCode,
                BusinessID: instanceID,
                ProcessTitl: $('select#ddlSystem').find('option:selected').text() + $("#HideFinYear").val() + "年计划指标上报",
                FormParams: { ProcessKey: FlowCode }
            });
            $.unblockUI();
        })
    }
}

//开始处理准备FormParam
function beforeAction(args) {
    args.BizContext.FormParams = { ReportName: $('select#ddlSystem').find('option:selected').text() + $("#HideFinYear").val() + "年计划指标上报", ProcessKey: FlowCode }
    return true;//如果流程为同步接入，返回true，则继续往下执行
}
//业务系统保存数据
function saveApplicationData(args) {
}

//流程处理完成，如果执行成功
function afterAction(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle($("#hideTargetPlanID").val(), args);
    }
}

function BusinessDataHandle(instanceID, args) {
    var strPrcessStatus = "";
    if (args.WorkflowContext.ProcessInstance.Status == 2 && args.WorkflowContext.ProcessInstance.RunningNodeID == args.WorkflowContext.ProcessInstance.StartNodeID) {
        strPrcessStatus = "Draft";
    } else if (args.WorkflowContext.ProcessInstance.Status == -1) {
        PrcessStatus = "Cancel";
    } else if (args.WorkflowContext.ProcessInstance.Status == 3) {

        // 审批结束
        if (args.WorkflowContext.CurrentUserNodeID != null && args.WorkflowContext.CurrentUserNodeID != "") {
            var nodeInfo = args.WorkflowContext.NodeInstanceList[args.WorkflowContext.CurrentUserNodeID];
            if (nodeInfo != null && (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7)) {
                strPrcessStatus = "Approved";
            } else {
                strPrcessStatus = null;
            }
        } else {
            strPrcessStatus = null;
        }
    }
    else {
        strPrcessStatus = "Progress";
    }

    $.ajax({
        url: "/AjaxHander/TargetPlanProcessController.ashx",
        type: "post",
        async: true,
        data: {
            BusinessID: instanceID,
            OperatorType: args.OperatorType,
            PrcessStatus: strPrcessStatus
        },
        success: function (result) {
            $.unblockUI();
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


var FinYear;
$(function OnInit() {
    sysID = $("#HidSystemID").val();
    TargetPlanID = $("#hideTargetPlanID").val();

    //获取年份
    if ($("#hideTargetPlanID").val() != "") {
        FinYear = $("#HideFinYear").val();
    }
    else {
        GetReportTime();
    }
    VersionName = $("#hideVersionName").val();
    //获取年份

    $("#txt_VersionName").val(VersionName);

    GetTargetPlanDetail();
});

function GetReportTime() {
    WebUtil.ajax({
        async: false,
        url: "/TimeConfiguration/GetReportDateTime",
        successReturn: function (dateresult) {
            var dt = new Date(dateresult);
            FinYear = dt.getFullYear();

        }
    });
}

function GetTargetPlanDetail() {
    TargetPlanID = $("#hideTargetPlanID").val();
    WebUtil.ajax({
        async: true,
        url: "/TargetPlanDetailController/GetTargetPlanDetail",
        args: { strSystemID: sysID, strFinYear: FinYear, strTargetPlanID: TargetPlanID, IsLatestVersion: true },
        successReturn: SplitData
    });
}
function SplitData(result) {
    var SystemModel;
    TargetPlanDeailData = result;
    $("#rows").empty();
    $("#Ul4").empty();
    $("#TargetPlanDetailHead").empty();
    WebUtil.ajax({
        url: "/TargetController/GetNowSystem",
        args: { SysID: sysID },
        successReturn: function (result) {
            SystemModel = result;
        }
    });


    if (TargetPlanDeailData[0] != null) {
        if (TargetPlanDeailData[0].HtmlTemplate != undefined) {
            strTemplate = TargetPlanDeailData[0].HtmlTemplate.split(',');
        }
        if (strTemplate[0] != "" && strTemplate[0] != undefined) {
            loadTmplTargetPlanDetail('#' + strTemplate[0]).tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
            if (strTemplate[0] == "TargetPlanDetailReportTableHeadTemplate_ToYear") {
                $("#importedDataTable2").css("width", "500px");
            }
        } else {
            loadTmplTargetPlanDetail('#TargetPlanDetailReportTableHeadTemplateforVersion').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
        }
        if (TargetPlanDeailData[0].ObjValue[0].ObjValue.length != 0) {
            setStlye('dataUploadSpan,monthReportSubmitSpan');
            if (SystemModel.Category != 3) {
                var heji = " <li class=\"sd\"><a class=\"active2\" onclick=\"TargetPlanDetailLiaddCss(this);\">汇总</a></li>";
                $("#Ul4").html(heji);
                loadTmplTargetPlanDetail('#TargetPlanDetailReportHeadTemplate').tmpl(TargetPlanDeailData).appendTo('#Ul4');
                LoadTargetPlanDetailData(TargetPlanDeailData[0]);
                AddSumHead(TargetPlanDeailData);
            }
            else {
                loadTmplTargetPlanDetail('#TargetPlanDetailReportHeadTemplate').tmpl(TargetPlanDeailData).appendTo('#Ul4');
                LoadTargetPlanDetailData(TargetPlanDeailData[0]);
                //AddSumHead(TargetPlanDeailData);
            }
            $("#Ul4 :first a").addClass("active_sub3");

            var obj = $("#TargetPlanDetailHead");
            var tab = $("#rows");
            FloatHeader(obj, tab);
        }
    }




}

function LoadTargetPlanDetailData(sender) {
    if (TargetPlanDeailData[0].ObjValue[0].ObjValue.length != 0) {
        if (strTemplate[1] != "" && strTemplate[1] != undefined) {
            loadTmplTargetPlanDetail('#' + strTemplate[1]).tmpl(sender).appendTo("#rows");
        } else {
            loadTmplTargetPlanDetail("#TargetPlanDetailReportTemplate").tmpl(sender).appendTo("#rows");
        }
    }
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




    $("#rows").empty();
    $("#TargetPlanDetailHead").empty();

    if (TemplData.HtmlTemplate != undefined) {
        strTemplate = TemplData.HtmlTemplate.split(',');
    }
    if (strTemplate[0] != "" && strTemplate[0] != undefined) {
        loadTmplTargetPlanDetail('#' + strTemplate[0]).tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
    }
    else if ($(sender).text() == "汇总") {
        AddSumHead(TargetPlanDeailData);
    }
    else {
        loadTmplTargetPlanDetail('#TargetPlanDetailReportTableHeadTemplateforVersion').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
    }

    LoadTargetPlanDetailData(TemplData)
    var obj = $("#TargetPlanDetailHead");
    var tab = $("#rows");
    FloatHeader(obj, tab);
}

function AddSumHead(result) {
    var head = " <tr><th  colspan=\"" + (1 + result.length * 2) + "\">" + VersionName + "</th></tr> ";
    head += " <tr><th style=\"width: 12%\" rowspan=\"2\">月份</th> <th colspan=\"" + result.length + "\">当月数</th> <th colspan=\"" + result.length + "\">累计数</th>";
    head += " </tr><tr id=\"TrTarget\">";
    for (var i = 0; i < result.length; i++) {
        head += "<th>" + result[i].Name + "</th>";
    }
    for (var i = 0; i < result.length; i++) {
        head += "<th>" + result[i].Name + "</th>";
    }
    head += "</tr>";
    $("#TargetPlanDetailHead").html(head);
    BangDetail();
}

function DownLoadTargetPlanExcel(sender) {
    sysID = $("#ddlSystem").val();
    window.open("/AjaxHander/DownExcelTargetPlan.ashx?FileType=Reported&SystemID=" + sysID + "&TargetPlanID=" + $("#hideTargetPlanID").val() + "&FinYear=" + FinYear);
}

$(function () {
    var error = 0;
    fileUpload("file1");
    fileUpload("file_upload");
});

function fileUpload(name) {
    $("#" + name).uploadify({
        'buttonText': '导入数据',
        'width': 100,
        'height': 25,
        'fileTypeDesc': 'office file',
        'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx;',
        'fileSizeLimit': '10240',
        //swf文件路径
        'swf': '../Scripts/UpLoad/uploadify.swf',
        'formData': { 'FileType': 'UpTargetPlanDetail', 'SysId': sysID, 'FinYear': FinYear, 'MonthReportID': TargetPlanID },
        //后台处理页面
        'uploader': '/AjaxHander/UpLoadMonthTargetDetail.ashx',
        'onUploadSuccess': function (file, data, response) {
            error = data;
            if (data == "" || data == null) {

                GetTargetPlanDetail();
                setStlye('monthReportSubmitSpan');
                $("#Down1,#T2").show();
                $("#DownLoadModel,#UpLoadData").hide();
            } else {
                alert(data);
            }
        },
        'onUploadComplete': function () {
            if (error == 0) {
                $("#UpLoadData").hide();
            }
        },
        'onUploadError': function (file, data, response) {
            alert("上传失败，程序出错！");
        }
        , 'onUploadStart': function (file, data, response) {
            $("#" + name).uploadify("settings", "formData", { 'VersionName': $("#hideVersionName").val() });
        }
    });
}

function getVersionName() {
    return VersionName;
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

            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null || obj == "0001/1/1 0:00:00") {
                return "---";
            } else {
                if (local == "CN") {
                    return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                } else {
                    return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                }
            }
        } else {
            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null || obj == "0001/1/1 0:00:00") {
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



//汇总数据
function BangDetail() {
    var SumMonthTargetList;
    var TargetList;
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetTargetList",
        args: { sysID: sysID },
        successReturn: function (result) {
            TargetList = result;
        }
    });
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetSumMonthTargetDetailByTID",
        args: { TargetPlanID: TargetPlanID },
        successReturn: function (view) {
            SumMonthTargetList = view;
        }
    });
    if (SumMonthTargetList != null) {
        var row = "";
        for (var i = 0; i <= 11; i++) {
            row += "<tr> <td class=\"Td_Center\">" + SumMonthTargetList[i].FinMonth + "月</td>";
            for (var j = 0; j < TargetList.length; j++) {
                if (j < SumMonthTargetList[i].TargetDetailList.length) {
                    if (SumMonthTargetList[i].TargetDetailList[j].Target != null) {
                        row += "<td class=\"Td_Right\"title=" + SumMonthTargetList[i].TargetDetailList[j].Target + ">" + MathTarget(SumMonthTargetList[i].TargetDetailList[j].Target) + "</td>";
                    }
                    else {
                        row += "<td class=\"Td_Right\">--</td>";
                    }
                }
                else {
                    row += "<td class=\"Td_Right\">--</td>";

                }
            }
            for (var j = 0; j < TargetList.length; j++) {
                if (j < SumMonthTargetList[i].TargetDetailList.length) {
                    if (SumMonthTargetList[i].TargetDetailList[j].SumTarget != null) {
                        row += "<td class=\"Td_Right\" title=" + SumMonthTargetList[i].TargetDetailList[j].SumTarget + ">" + MathTarget(SumMonthTargetList[i].TargetDetailList[j].SumTarget) + "</td>";
                    }
                    else {
                        row += "<td class=\"Td_Right\">--</td>";
                    }
                }
                else {
                    row += "<td class=\"Td_Right\">--</td>";

                }
            }

            row += "</tr>";
        }
        row += "<tr><th class=\"th_Sub2\">全年</th>";
        for (var i = 0; i < TargetList.length; i++) {
            if (i < SumMonthTargetList[11].TargetDetailList.length) {
                if (SumMonthTargetList[11].TargetDetailList[i].SumTarget != null) {
                    row += "<th class=\"th_Sub2\" style=\"text-align:right\" title=" + SumMonthTargetList[11].TargetDetailList[i].SumTarget + ">" + MathTarget(SumMonthTargetList[11].TargetDetailList[i].SumTarget) + "</th>";
                }
                else {
                    row += "<td class=\"th_Sub2\" style=\"text-align:right\">--</td>";
                }
            }
            else {
                row += "<td class=\"th_Sub2\" style=\"text-align:right\">--</td>";

            }

        }
        for (var i = 0; i < TargetList.length; i++) {
            row += "<th class=\"th_Sub2\"style=\"text-align:right\">--</th>";
        }
        row += "</tr>";
        $("#rows").html(row);
    }


}

function MathTarget(num) {
    var vv = Math.pow(10, 0);
    return Math.round(num * vv) / vv;
}
