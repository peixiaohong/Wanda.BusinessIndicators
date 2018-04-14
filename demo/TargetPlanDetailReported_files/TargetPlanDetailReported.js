/// <reference path="../../BusinessReport/TargetPlanDetailReportedTmpl.html" />
//变量（误删）
var sysID;
var TargetPlanID;

var TargetPlanDeailData;

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
            $("#PromptMessage").html("请使用系统提供的填报模板导入数据，如已导入，再次导入将覆盖当前数据。");
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
            $("#PromptMessage").html("请保存数据，等待其它上报人员填报。");
            $("#PromptMessage").removeClass("Tishi");
            break;

    }
}

function setStlye(sender) {
    var val = sender.split(",");
    for (var i = 0; i < val.length; i++) {
        if (val[i] == 'downLoadTemplateSpan') {
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
    //setStlye('downLoadTemplateSpan,dataUploadSpan');
});


function operateNav(sender) {
    switch (sender) {
        case "downLoadTemplate":
            window.location.href = "TargetPlanDetailReported-1.html";
            break;
        case "dataUpload":
            window.location.href = "TargetPlanDetailReported-2.htm";
            break;
        case "monthReportReady": //保存事件
            window.location.href = "TargetPlanDetailReported-3.htm";
            break;
        case "monthReportSubmit":
            window.location.href = "TargetPlanDetailReported-4.htm";
            break;
        case "inputName":
            window.location.href = "TargetPlanDetailReported.html";
            break;

    }
}

var FlowCode = "";
$(function () {
    var ShowProecessNodeName = false;
    $.blockUI({ message: "<div style='width:200px'><img src='TargetPlanDetailReported_files/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
   
    setTimeout(function () {
        $.unblockUI();
    }, 500);
    //wanda_wf_client.initAjaxSetting("process", false, otherSetting);
})



function GetProcess(key, instanceID) {
    FlowCode = key;
}

//开始处理准备FormParam
function beforeAction(args) {
  
}
//业务系统保存数据
function saveApplicationData(args) {
}

//流程处理完成，如果执行成功
function afterAction(args) {
}

function BusinessDataHandle(instanceID, args) {
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

    GetTargetPlanDetail();
});

function GetReportTime() {
}

function GetTargetPlanDetail() {
   
}
function SplitData(result) {


}

function LoadTargetPlanDetailData(sender) {
   
}


var currentTargetPlanDetail;
function TargetPlanDetailLiaddCss(sender) {
    switch (sender) {
        case 1:
            window.location.href = "TargetPlanDetailReported-2.htm";
            break;
        case 2:
            window.location.href = "TargetPlanDetailReported-2-1.htm";
            break;
        case 3:
            window.location.href = "TargetPlanDetailReported-2-2.htm";
            break;
        case 4:
            window.location.href = "TargetPlanDetailReported-2-3.htm";
            break;
        default:
    }
}

function AddSumHead(result) {
    var head = " <tr><th style=\"width: 12%\" rowspan=\"2\">月份</th> <th colspan=\"" + result.length + "\">当前数</th> <th colspan=\"" + result.length + "\">累计数</th>";
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
    $("#file1,#file_upload").uploadify({
        'buttonText': '导入数据',
        'width': 100,
        'height': 25,
        'fileTypeDesc': 'office file',
        'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx;',
        'fileSizeLimit': '10240',
        //swf文件路径
        'swf': '../Scripts/UpLoad/uploadify.swf',
        //后台处理页面
        'uploader': '/AjaxHander/UpLoadMonthTargetDetail.ashx?FileType=UpTargetPlanDetail&SysId=' + sysID + "&FinYear=" + FinYear + "&MonthReportID=" + TargetPlanID,
        'onUploadSuccess': function (file, data, response) {
            error = data;
            if (data == "" || data == null) {

                GetTargetPlanDetail();
                setStlye('monthReportReadySpan');
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
