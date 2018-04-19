var DownLoadTag = "";// 这个变量给 经营的模版系统用

function operateNav(sender) {
    $("#process").hide();
    switch (sender) {
        case "downLoadTemplate":
            $("#DownLoadModel").show();
            $("#UpLoadData,#UpLoadDataDiv,#T2,#T3,#T3_1,#T5,#Down1,#T4,#RptAttachments").hide();
            break;
        case "dataUpload":
            $("#DownLoadModel,#T3,#T3_1,#Down1,#T4,#RptAttachments,#T5").hide();
            if (ReportInstance != null) {
                if (ReportInstance.ReportDetails.length > 0) {
                    $("#T2,#UpLoadDataDiv").show();
                    $("#UpLoadData").hide();
                    var obj = $("#CompleteDetailHead");
                    var tab = $("#tab2_rows");
                    FloatHeader(obj, tab);
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
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3_1,#UpLoadData,#T4,#RptAttachments,#T5").hide();
            $("#T3,#Down1").show();

            DownLoadTag = "missTargetReport"; // 判断下载的模版
            MissTagetExcelReport();

            var obj = $("#Tab_MissTargetHead");
            var tab = $("#Tbody_MissTargetData");
            FloatHeader(obj, tab);
            break;
        case "missCurrentTargetReport": //未完成（当月）
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#UpLoadData,#T4,#T3,#T3_1,#Down1,#RptAttachments,#divMonthLyReportAction,#Down1").hide();
            $("#T3_1,#Down1").show();

            DownLoadTag = "missCurrentTargetReport"; // 判断下载的模版
            MissTagetExcelReport();

            var obj = $("#Tab_CurrentMissTargetHead");
            var tab = $("#Tbody_CurrentMissTargetData");
            // FloatHeader(obj, tab, false);
            break;

        case "monthReport":
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3,#T3_1,#Down1,#T5").hide();
            $("#T4,#RptAttachments").show();
            break;

        case "monthReportReady": //保存事件
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3,#T3_1,#Down1,#T4,#RptAttachments,#UpLoadData").hide();
            $("#T5").show();

            SaveMonthlyReportLog(1);
            $("#divMonthLyReportAction").show();//显示日志列表
            setStlye('missTargetReportSpan,monthReportSpan,monthReportReadySpan,monthReportSubmitSpan');
            

           // alert("保存事件");

            break;
        case "monthReportSubmit":
            $("#process").show();
            $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
            //GetProcess($("#HideProcessCode").val(), MonthReportID);
            $("#T2,#UpLoadDataDiv,#DownLoadModel,#T3,#Down1,#T4,#RptAttachments,#T5").hide();
            break;

    }
}

//旧的代码 作废
function ProSystemSave()
{
    WebUtil.ajax({
        async: true,
        url: "/TargetReportedControll/ProSystemSave",
        args: { rpts: WebUtil.jsonToString(ReportInstance), strMonthReportID: MonthReportID },
        successReturn: function (result) {
            $("#Tab_SaveReadyHead").empty();
            loadTmpl("#TmplProSystemSave").tmpl().appendTo("#Tab_SaveReadyHead"); //加载表头

            $("#Tab_SaveReadyData").empty();

            var save = {
                "proYear":FinYear,
                "proMonth":FinMonth,
                "ObjValue": result
            };

            var _isReady = true;
            $.each(result, function (n, obj) {
                if (obj.IsReady == false) {
                    _isReady = false;
                    return;
                }
            });

            loadTmpl("#TmplProSystemSaveData").tmpl(save).appendTo("#Tab_SaveReadyData");

            if (_isReady)
            {
                setStlye('missTargetReportSpan,monthReportSpan,monthReportReadySpan,monthReportSubmitSpan');
            }

        }
    });
    
}



//保存日志
function SaveMonthlyReportLog(sender) {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
    $.ajax({
        url: "/AjaxHander/MonthlyReportLog.ashx",
        type: "post",
        async: true,
        cache: false,
        data: {
            ActionType: sender,
            SysId: $("#ddlSystem").val(),
            FinYear: $("#hideFinYear").val(),
            FinMonth: $("#hideFinMonth").val(),
            MonthReportID: $("#hideMonthReportID").val()
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
    var Opinions = $("#HideOpinions").val();
    var otherSetting = {
        OnBeforeExecute: beforeAction,
        OnSaveApplicationData: saveApplicationData,
        OnAfterExecute: afterAction,
        IsShowContextMenu: true,
        PageContextMenu: true,
        EnableDebug: true,
        ShowNodeName: ShowProecessNodeName == true ? true : false,
        ButtonCssType: "middle",
        CustomerProcessLog: Opinions != "" ? JSON.parse(Opinions) : {},
        CustomerSceneSetting: {
            ShowCc: false,//是否显示抄送
            ShowFowardButton: false,//是否显示转发按钮
            AlwaysReturnToStart:true
        },
    };
    //bpf_wf_client.initAjaxSetting("process", false, otherSetting);
})


var Month
function GetProcess(key, instanceID) {
    Month = $("#hideFinMonth").val() * 1 > 9 ? $("#hideFinMonth").val() : "0" + $("#hideFinMonth").val();
    FlowCode = key;
    var businessID = wanda_wf_tool.getQueryString("BusinessID");
    if (businessID != "") {
        bpf_wf_client.getProcess(businessID, function () {
            $.unblockUI();
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
                ProcessTitle: ($('select#ddlSystem').find('option:selected').text() != "境外项目" ? "项目系统" : $('select#ddlSystem').find('option:selected').text()) + $("#hideFinYear").val() + "年" + Month + "月度报告",
                FormParams: { ProcessKey: FlowCode }
                
            });
            $.unblockUI();
        })
            
        
        $.unblockUI();
    }
}

//开始处理准备FormParam
function beforeAction(args) {
    BusinessDataHandle("beforeAction", MonthReportID, args)
    args.BizContext.FormParams = { ReportName: ($('select#ddlSystem').find('option:selected').text() != "境外项目" ? "项目系统" : $('select#ddlSystem').find('option:selected').text()) + $("#hideFinYear").val() + "年" + Month + "月度报告", ProcessKey: FlowCode }
    return true;//如果流程为同步接入，返回true，则继续往下执行
}
//业务系统保存数据
function saveApplicationData(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle("saveApplicationData", MonthReportID, args)
    }
}

//流程处理完成，如果执行成功
function afterAction(args) {
    if (args.WorkflowContext.StatusCode == 0) {
        BusinessDataHandle("afterAction", MonthReportID, args)
        window.location.href = "ProTargetApprove.aspx?BusinessID=" + MonthReportID;
    }
}

function BusinessDataHandle(ExecuteType, instanceID, args) {
    $.ajax({
        url: "/AjaxHander/ProProcessController.ashx",
        type: "post",
        async: false,
        cache: false,
        data: {
            BusinessID: instanceID,
            ExecuteType: ExecuteType,
            OperatorType: args.OperatorType
        },
        dataType: "text",
        success: function (result) {
        
        }, 
        error: function () {
            $.unblockUI();
            var errorInfo = "";
            var elem = $(arguments[0].responseText);
            for (var i = 3; i < elem.length; i++) {
                errorInfo += elem[i].innerHTML;
            }
            //alert(errorInfo);
            //debugger; //$(arguments[0].responseText)
            WebUtil.alertWarn("对不起！您没有权限提交该流程，请联系管理员");
        }
    })
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

