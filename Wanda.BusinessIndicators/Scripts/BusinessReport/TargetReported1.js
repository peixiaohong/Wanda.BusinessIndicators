var DownLoadTag = ""; // 这个变量给 经营的模版系统用
//var ZG_DownLoadTag = "";// 这个变量给 直管模版的系统用
//var Pro_DownLoadTag = ""; // 这个变量给 项目模版的系统用

function operateNav(sender) {
    $("#process").hide();
    switch (sender) {
        case "downLoadTemplate"://下载模版
            $("#DownLoadModel").show();
            $("#UpLoadData,#UpLoadDataDiv,#T2,#T3,#T3_1,#Down1,#T4,#RptAttachments,#divMonthLyReportAction").hide();
            break;
        case "dataUpload"://完成明细
            $("#DownLoadModel,#T3,#T3_1,#Down1,#T4,#RptAttachments,#divMonthLyReportAction").hide();
            if (ReportInstance != null) {
                if (ReportInstance.ReportDetails.length > 0) {
                    $("#T2,#UpLoadDataDiv").show();
                    $("#UpLoadData").hide();
                    var obj = $("#CompleteDetailHead");
                    var tab = $("#tab2_rows");
                    FloatHeader(obj, tab, true);

                } else {
                    $("#UpLoadData").show();
                    $("#T2,#UpLoadDataDiv").hide();
                }
            } else {
                $("#UpLoadData").show();
                $("#T2,#UpLoadDataDiv").hide();
            }

            break;
        case "missTargetReport"://未完成（累计）
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#UpLoadData,#T4,#T3_1,#RptAttachments,#divMonthLyReportAction").hide();
            $("#T3,#Down1").show();
            var obj = $("#Tab_MissTargetHead");
            var tab = $("#Tbody_MissTargetData");
           // FloatHeader(obj, tab, false);
            DownLoadTag = "missTargetReport"; // 判断下载的模版
            MissTagetExcelReport();

            break;
        case "missCurrentTargetReport": //未完成（当月）
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#UpLoadData,#T4,#T3,#Down1,#RptAttachments,#divMonthLyReportAction,#Down1").hide();
            $("#T3_1,#Down1").show();
            var obj = $("#Tab_CurrentMissTargetHead");
            var tab = $("#Tbody_CurrentMissTargetData");
           // FloatHeader(obj, tab, false);

            DownLoadTag = "missCurrentTargetReport"; // 判断下载的模版

            MissTagetExcelReport();

            break;

        case "monthReport":
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3,#T3_1,#Down1,#divMonthLyReportAction").hide();
            $("#T4,#RptAttachments").show();
            break;
        case "monthReportReady": //保存事件
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3,#T3_1,#Down1,#T4,#RptAttachments,#UpLoadData").hide();
            SaveMonthlyReportLog("1");
          
            $("#divMonthLyReportAction").show();//显示日志列表
            setStlye('missTargetReportSpan,monthReportSpan,monthReportReadySpan,monthReportSubmitSpan');
            
            break;
        case "monthReportSubmit":
            $("#process").show();
            $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
            //后续加上 BusinessID
            //GetProcess($("#HideProcessCode").val(), $("#hideMonthReportID").val());

            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3,#T3_1,#Down1,#T4,#RptAttachments,#divMonthLyReportAction").hide();
            break;

    }
}


var FlowCode = "";
$(function () {
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
        ShowNodeName: ShowProecessNodeName ==  false,
        ButtonCssType: "middle",
        CustomerSceneSetting: {
            ShowCc: false,//是否显示抄送
            ShowFowardButton: false,//是否显示转发按钮
            AlwaysReturnToStart: true
        },
    };
    //wanda_wf_client.initAjaxSetting("process", false, otherSetting);
})


var Month;
function GetProcess(key, instanceID) {
    Month = $("#hideFinMonth").val() * 1 > 9 ? $("#hideFinMonth").val() : "0" + $("#hideFinMonth").val();
    FlowCode = key;
    var businessID = wanda_wf_tool.getQueryString("BusinessID");
    if (businessID != "") {
        wanda_wf_client.getProcess(businessID, function () {
            $.unblockUI();
        });
    }
    else {
        wanda_wf_client.exist(instanceID, function () {
            wanda_wf_client.getProcess(instanceID, function () {
                $.unblockUI();
            })
        }, function () {
            wanda_wf_client.createProcess({
                FlowCode: FlowCode,
                BusinessID: instanceID,
                ProcessTitle: $('select#ddlSystem').find('option:selected').text() + $("#hideFinYear").val() + "年" + Month + "月度报告",
                FormParams: { ProcessKey: FlowCode }
            });
            $.unblockUI();
        })
    }
}

//开始处理准备FormParam
function beforeAction(args) {
    args.BizContext.FormParams = { ReportName: $('select#ddlSystem').find('option:selected').text() + $("#hideFinYear").val() + "年" + Month + "月度报告", ProcessKey: FlowCode }
    return true;//如果流程为同步接入，返回true，则继续往下执行
}
//业务系统保存数据
function saveApplicationData(args) {
}

//流程处理完成，如果执行成功
function afterAction(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle($("#hideMonthReportID").val(), args)
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
        //审批中的
        strPrcessStatus = "Progress";
    }


    if (strPrcessStatus != null) {
        $.ajax({
            url: "/AjaxHander/ProcessController.ashx",
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

}




function SaveMonthlyReportLog(sender) {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    $.ajax({
        url: "/AjaxHander/MonthlyReportLog.ashx",
        type: "post",
        async: true,
        cache: false,
        data: {
            ActionType: sender,
            SysId: $("#ddlSystem").attr("value"),
            FinYear: $("#hideFinYear").attr("value"),
            FinMonth: $("#hideFinMonth").attr("value"),
            MonthReportID: $("#hideMonthReportID").attr("value")
        },
        dataType: "text",
        success: function (result) {
            GetMonthlyReportAction();//加载操作日志

            GetReportInstance();// 保存后刷新下整体数据
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
        }
    });
}




//显示隐藏列表的数据
function ClickCounter(sender, val) {
    try {
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
    } catch (e) {

    }
}

