
var SystemID;
//var FinYear;
var TargetPlanID;
var TargetPlanDeailData;



var SumMonthTargetList;
var FinYear;
var TargetList;
var SumTargetPlanList;





//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/TargetPlanDetailReportedTmpl.html", selector);
}

$(document).ready(function () {
    SystemID = $("#hideSystemID").val();
    TargetPlanID = $("#hideTargetPlanID").val();
    FinYear = $("#hideFinYear").val();
   // GetValue();
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    GetTargetPlanDetailApproveProcess(TargetPlanID);

    if (SysDataJson.Category == 3) {
        GetTargetPlanDetail();
    } else
    {
        GetSumList();
    }
    
});


/// ---- 新的写法
function GetSumList() {
    $("#tabs").html("<li class=\"sd\" style=\"DISPLAY: list-item\"><a class=\"active3 active_sub3\" onclick=\"Change('sum','')\" id=\"tabsum\">汇总</a></li>");
    $("#rows_new").html("");
 
    WebUtil.ajax({
        async: true,
        url: "/TargetPlanDetailController/GetVerTargetList",
        args: { strTargetPlanID: TargetPlanID },
        successReturn: function (result) {
            TargetList = result; //绑定 指标

            loadTmpl('#TabsTarget').tmpl(TargetList).appendTo('#Ul4');

            BangHead(result); // 汇总的表头

            BangDetail(SystemID); //汇总的数据    

            SumTargetPlanList = ""; // 将明细数据，清空
        
            //  GetSumTargetPlanList(); // 指标下的明细数据 
            $("#SumTable").show();
            $("#TargetTable").hide();

           // Fake();
            var obj = $("#importedDataFloatTable2");
            var head = $("#Head");
            obj.find("thead").html(head.html());
            var tab = $("#rows");
            FloatHeader(obj, tab);

        }
    });


    //var obj = $("#Thead1");
    //var tab = $("#TrTargetTable");
    //FloatHeader(obj, tab);

    
}

function GetSumTargetPlanList() {
    WebUtil.ajax({
        async: false,
        url: "/TargetPlanDetailController/GetSumTargetDetail",
        args: { strFinYear: FinYear, strSystemID: SystemID, strTargetPlanID: TargetPlanID },
        successReturn: function (result) {
            SumTargetPlanList = result;
        }
    });
}

function Change(adj, id) {
    //Load();
    document.getElementById('tabsum').className = "active3";

    for (var i = 0; i < TargetList.length; i++) {
        if (id == TargetList[i].ID) {
            document.getElementById('tab' + TargetList[i].ID + '').className = "active3 active_sub3";
        }
        else {
            document.getElementById('tab' + TargetList[i].ID + '').className = "active3";
        }

    }
    if (adj == 'sum') {
        document.getElementById('tabsum').className = "active3 active_sub3";
        $("#SumTable").show();
        $("#TargetTable").hide();
        $("#file_upload-button1").show();

        var obj = $("#importedDataFloatTable2");
        var head = $("#Head");
        obj.find("thead").html(head.html());
        var tab = $("#rows");
        FloatHeader(obj, tab);
    }
    else {
        $("#SumTable").hide();
        $("#TargetTable").show();
        BangSumTargetPlanList(id);
        $("#file_upload-button1").show();
    
    }
    //Fake();
}
function BangSumTargetPlanList(id) {
    $("#TrTargetTable").html("");
    $("#SumTrTargetTable").html("");

    if (SumTargetPlanList != null && SumTargetPlanList != undefined && SumTargetPlanList.length > 0) {
        //判断当前是否有数据
        for (var i = 0; i < SumTargetPlanList.length; i++) {
            if (SumTargetPlanList[i].TargetID == id) {
                // loadTmpl('#TrSumTargetSum').tmpl(SumTargetPlanList[i]).appendTo('#SumTrTargetTable');
                loadTmpl('#TrSumTargetPlan').tmpl(SumTargetPlanList[i].TargetPlanDetailList).appendTo('#TrTargetTable');
            }
        }
    }
    else {
        WebUtil.ajax({
            async: true,
            url: "/TargetPlanDetailController/GetSumTargetDetail",
            args: { strFinYear: FinYear, strSystemID: SystemID, strTargetPlanID: TargetPlanID},
            successReturn: function (result) {
                SumTargetPlanList = result;
                for (var i = 0; i < SumTargetPlanList.length; i++) {
                    if (SumTargetPlanList[i].TargetID == id) {
                        // loadTmpl('#TrSumTargetSum').tmpl(SumTargetPlanList[i]).appendTo('#SumTrTargetTable');
                        loadTmpl('#TrSumTargetPlan').tmpl(SumTargetPlanList[i].TargetPlanDetailList).appendTo('#TrTargetTable');
                    }
                }
            }
        });

    }
    var obj = $("#importedDataFloatTable2");
    var head = $("#Thead1");
    obj.find("thead").html(head.html());
    var tab = $("#SumTrTargetTable");
    FloatHeader(obj, tab);
}

function BangHead(result) {
    $("#Dsum").attr("colspan", result.length);
    $("#Asum").attr("colspan", result.length);
    $("#Vsum").attr("colspan", result.length*2+1);
    $("#TrTarget").html("");
    for (var i = 0; i < 2; i++) {
        loadTmpl('#TrTarget').tmpl(result).appendTo('#TrTarget');
    }
}

//--新的写法








//-------------------------旧数据的获取方法

function GetTargetPlanDetail() {
    WebUtil.ajax({
        async: false,
        url: "/TargetPlanDetailController/GetTargetPlanDetail",
        args: { strSystemID: SystemID, strFinYear: FinYear, strTargetPlanID: TargetPlanID, IsLatestVersion: true },
        successReturn: SplitData
    });
}
function SplitData(result) {
    var SystemModel;
    WebUtil.ajax({
        url: "/TargetController/GetNowSystem",
        args: { SysID: SystemID },
        successReturn: function (sys) {
            SystemModel = sys;
        }
    });
    TargetPlanDeailData=result;
    if (TargetPlanDeailData[0] != null) {
        if (TargetPlanDeailData[0].HtmlTemplate != undefined) {
            strTemplate = TargetPlanDeailData[0].HtmlTemplate.split(',');
        }
        if (strTemplate[0] != "" && strTemplate[0] != undefined) {
            loadTmpl('#' + strTemplate[0]).tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
            if (strTemplate[0] == "TargetPlanDetailReportTableHeadTemplate_ToYear") {
                $("#importedDataTable2").css("width", "500px");
            }
        } else {
            loadTmpl('#TargetPlanDetailReportTableHeadTemplateforVersion').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
        }
        if (TargetPlanDeailData[0].ObjValue[0].ObjValue != null) {
            if (SystemModel.Category != 3) {
                var heji = " <li class=\"sd\"><a class=\"active2\" onclick=\"TargetPlanDetailLiaddCss(this);\">汇总</a></li>";
                $("#Ul4").html(heji);
                loadTmpl('#TargetPlanDetailReportHeadTemplate').tmpl(TargetPlanDeailData).appendTo('#Ul4');
                LoadTargetPlanDetailData(TargetPlanDeailData[0]);
                AddSumHead(TargetPlanDeailData);
            }
            else {
                $("#Ul4").empty();
                loadTmpl('#TargetPlanDetailReportHeadTemplate').tmpl(TargetPlanDeailData).appendTo('#Ul4');
                LoadTargetPlanDetailData(TargetPlanDeailData[0]);
             
            }
            $("#Ul4 :first a").addClass("active_sub3");

            //var obj = $("#TargetPlanDetailHead");
            //var tab = $("#rows_old");

            //FloatHeader(obj, tab);
            
        }
    }
}

function AddSumHead(result) {
    var head = " <tr><th style=\"width: 12%\" rowspan=\"2\">月份</th> <th colspan=\"" + result.length + "\">当月数</th> <th colspan=\"" + result.length + "\">累计数</th>";
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

function LoadTargetPlanDetailData(sender) {
    if (TargetPlanDeailData[0].ObjValue[0].ObjValue.length != 0) {
        if (strTemplate[1] != "" && strTemplate[1] != undefined) {
            loadTmpl('#' + strTemplate[1]).tmpl(sender).appendTo("#rows_old");
        } else {
            loadTmpl("#TargetPlanDetailReportTemplate").tmpl(sender).appendTo("#rows_old");
        }
        var obj = $("#importedDataFloatTable2");
        var head = $("#TargetPlanDetailHead");
        obj.find("thead").html(head.html());
        var tab = $("#rows_old");
        FloatHeader(obj, tab);
    }
}

function SearchData() {
    GetTargetPlanDetail();
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
  


    //var obj = $("#TargetPlanDetailHead");
    //var tab = $("#rows_old");

    //FloatHeader(obj, tab);
 
    $("#rows_old").empty();
    $("#TargetPlanDetailHead").empty();

    if (TemplData.HtmlTemplate != undefined) {
        strTemplate = TemplData.HtmlTemplate.split(',');
    }
    if (strTemplate[0] != "" && strTemplate[0] != undefined) {
        loadTmpl('#' + strTemplate[0]).tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
    }
    else if ($(sender).text() == "汇总") {
        AddSumHead(TargetPlanDeailData);
    } else {
        loadTmpl('#TargetPlanDetailReportTableHeadTemplateforVersion').tmpl().appendTo('#TargetPlanDetailHead'); //加载列头
    }

    LoadTargetPlanDetailData(TemplData)
}

//------------------------旧的数据的获取方法


function DownExcelReport(sender) {
    var latest = true; //修改下载
    window.open("/AjaxHander/DownExcelTargetPlan.ashx?FileType=Reported&SystemID=" + SystemID + "&TargetPlanID=" + TargetPlanID + "&FinYear=" + FinYear + "&IsLatestVersion=" + latest);
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
    var otherSetting = {
        IsShowContextMenu: true,
        PageContextMenu: true,
        EnableDebug: true,
        ShowNodeName: ShowProecessNodeName == true ? true : false,
        ButtonCssType: "middle",
        OnAfterExecute: afterAction,
        CustomerSceneSetting: {
            ShowCc: false,//是否显示抄送
            ShowFowardButton: false,//是否显示转发按钮
            AlwaysReturnToStart: true
        },
    };
    bpf_wf_client.initAjaxSetting("#process", false, otherSetting);
})
function afterAction(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle($("#hideTargetPlanID").val(), args)
    } else
    {
        window.close();
    }
}
function GetTargetPlanDetailApproveProcess(instanceID) {
    bpf_wf_client.getProcess(instanceID, function () {
        $.unblockUI();
    });
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
            if (nodeInfo != null && (nodeInfo.NodeType == 0 ||nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7)) {
                strPrcessStatus = "Approved";


            } else {
                strPrcessStatus = null;
            }
        } else {
            strPrcessStatus = null;
        }

    }
    else {
        //审批中的
        strPrcessStatus = "Progress";
    }
    $.ajax({
        url: "/AjaxHander/TargetPlanProcessController.ashx",
        type: "post",
        async: true,
        data: {
            BusinessID: instanceID,
            OperatorType: args.OperatorType,
            PrcessStatus: strPrcessStatus,

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
}

function AddSumHead(result) {
    var head = " <tr><th  colspan=\"" + (1 + result.length * 2) + "\">" + $("#hideVersionName").val() + "</th></tr> ";
     head += " <tr><th style=\"width: 12%\" rowspan=\"2\">月份</th> <th colspan=\"" + result.length + "\">当前数</th> <th colspan=\"" + result.length + "\">累计数</th>";
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


//汇总数据
function BangDetail() {
    var SumMonthTargetList;
    var TargetList;
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetTargetList",
        args: { sysID: SystemID },
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
            for (var n = 0; n < TargetList.length; n++) {
                for (var j = 0; j < SumMonthTargetList[i].TargetDetailList.length; j++) {
                    if (TargetList[n].ID == SumMonthTargetList[i].TargetDetailList[j].TargetID) {
                        if (SumMonthTargetList[i].TargetDetailList[j].Target != null) {
                            row += "<td class=\"Td_Right\"title=" + SumMonthTargetList[i].TargetDetailList[j].Target + ">" + MathTarget(SumMonthTargetList[i].TargetDetailList[j].Target) + "</td>";
                        }
                        else {
                            row += "<td class=\"Td_Right\">--</td>";
                        }

                    }
                }
            }
            for (var n = 0; n < TargetList.length; n++) {
                for (var j = 0; j < SumMonthTargetList[i].TargetDetailList.length; j++) {
                    if (TargetList[n].ID == SumMonthTargetList[i].TargetDetailList[j].TargetID) {
                        if (SumMonthTargetList[i].TargetDetailList[j].SumTarget != null) {
                            row += "<td class=\"Td_Right\" title=" + SumMonthTargetList[i].TargetDetailList[j].SumTarget + ">" + MathTarget(SumMonthTargetList[i].TargetDetailList[j].SumTarget) + "</td>";
                        }
                        else {
                            row += "<td class=\"Td_Right\">--</td>";
                        }
                    }
                }
            }
            row += "</tr>";
        }
        row += "<tr><th class=\"th_Sub2\">全年</th>";
        for (var i = 0; i < TargetList.length; i++) {
            for (var j = 0; j < SumMonthTargetList[11].TargetDetailList.length; j++) {
                if (TargetList[i].ID == SumMonthTargetList[11].TargetDetailList[j].TargetID) {
                    if (SumMonthTargetList[11].TargetDetailList[j].SumTarget != null) {
                        row += "<th class=\"th_Sub2\" style=\"text-align:right\" title=" + SumMonthTargetList[11].TargetDetailList[j].SumTarget + ">" + MathTarget(SumMonthTargetList[11].TargetDetailList[j].SumTarget) + "</th>";
                    }
                    else {
                        row += "<td class=\"th_Sub2\" style=\"text-align:right\">--</td>";
                    }

                }
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
