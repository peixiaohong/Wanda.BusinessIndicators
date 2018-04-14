$(function () {
    //var businessId = utils.getQueryString("businessID");
    //Task.Init(businessId);
});
var Task = {
    Init: function (businessId) {
        Task.LoadData(businessId, Task.BuildData);
    },
    BuildData: function (data) {
        $(".tc-title").text(data.TaskTitle);
        $(".tc-createdate").text(utils.ChangeDateFormat(data.AssignDate, true));
        $(".tc-assignMember").text(data.AssignName);
        $(".tc-request").text(data.TaskRemark);
        if (data.TaskAttachments.length > 0) {
            $(".tc-attachments-title").removeClass("hidden");
            $(".tc-attachments").removeClass("hidden");
            $.each(data.TaskAttachments, function (i, d) {
                var $html = Task.BuildAttachItem(d);
                $(".tc-attachments").append($html);
            });
        } else {
            $(".tc-attachments-title").addClass("hidden");
            $(".tc-attachments").addClass("hidden");
        }
        if (data.TaskReportAttachments.length > 0) {
            $(".tc-report-attachment-container").removeClass("hidden");
            $.each(data.TaskReportAttachments, function (i, d) {
                var $html = Task.BuildTaskReportAttachItem(d);
                $(".tc-task-report-attachments").append($html);
            });
        }
        if (data.TaskAttachment != null) {
            $(".tc-taskAttachment-title").removeClass("hidden");
            $(".tc-taskAttachment-container").removeClass("hidden");
            //附件名称：${TaskFileName}&#10;上传时间：${OverTime}&#10;上传人员：${TaskOverName}
            var fileName = '附件名称：{0}\r\n上传时间：{1}\r\n上传人员：{2}（{3}）\r\n点击预览'.formatBy(data.TaskAttachment.FileName, utils.ChangeDateFormat(data.TaskAttachment.CreateDate, true), data.TaskAttachment.CreateName, data.TaskAttachment.CreateLoginName);
            $(".tc-taskAttachment-fileName").attr("title", fileName);
            $(".tc-download-size").text(data.TaskAttachment.FileSize);
            $(".tc-taskAttachment-fileName").click(function () {
                //commons.DownloadAttachment(data.TaskAttachment.ID);
                commons.OWA_PreviewAttachment(data.TaskAttachment.ID);
            });
        }
        $(".tc-report-remark").text(data.TaskReportRemark);
        Task.BuildTaskReportAttachItemLength();


        $("body>#bpf-wfclient-button-content").remove();
        if (!data.IsNeedApprove) {
            var $container = $("#SJSJ_LC").closest(".tc-approve");
            var $wfButtons = $container.find("#bpf-wfclient-button-content");
            var $wfButtonClone = $wfButtons.clone();
            $wfButtonClone.appendTo("body");
            $container.hide();
            $wfButtonClone.find(".bpf-wfclient-button-btn").click(function () {
                var op = $(this).attr("optype");
                $("[optype='" + op + "']", $("#bpf-wfclient-button-content")).click();
            });
        } else {
            var $container = $("#SJSJ_LC").closest(".approval-process");
            $container.show();
        }

    },
    BeforeAction: function (args, func, data) {
        WFParam_SJSJ.DyRoleList = [data.ReceiveLoginName];
        WFParam_SJSJ.ApprovalRoleList = [data.AssignLoginName];
        WFOperator_SJSJ.StartPage.BeforeAction(args, func, {
            BusinessID: data.BusinessID,
            TaskName: data.TaskTitle
        });
    },
    CommonSave: function (action, args, func) {
        //var businessID = utils.getQueryString("businessID");
        //var url = api_url + 'api/todos/workflow/' + action + '/' + businessID;
        //utils.ajax({
        //    type: 'POST',
        //    url: url,
        //    args: { BusinessID: businessID, TaskReportRemark: $(".tc-report-remark").val() },
        //    success: function (data) {
        //        func();
        //    }
        //});
    },
    Save: function (args, func) {
        Task.CommonSave("save", args, func);
    },
    Approve: function (args) {
        Task.CommonSave("approve", args, function () {
            window.close();
        });
    },
    Reject: function (args) {
        Task.CommonSave("reject", args, function () {
            WFOperator_SJSJ.AfterActionRedirect(args);
        });
    },
    AfterAction: function (argsT) {
        
        //TODO审批通过时修改数据状态，修改成功后请调用WFOperator_SJSJ.AfterActionRedirect(args);做跳转
        WFOperator_SJSJ.ApprovePage.AfterAction(argsT,
        {
            Approval: function (args) { Task.Approve(args) },
            Return: function (args) { Task.Reject(args) },
            Redirect: function (args) { window.close() }
        });


    },
    LoadData: function (businessId, callback) {
        //var url = api_url + 'api/todos/load/' + businessId;
        //utils.ajax({
        //    type: 'GET',
        //    url: url,
        //    success: function (data) {
        //        //工作流操作开始
        //        WFOperator_SJSJ.InitSetting({
        //            OnAfterExecute: Task.AfterAction//执行后调用（进行回滚或其它操作（例如跳转））
        //        , IsView: utils.getQueryString("v").length > 0 ? true : false
        //        });
        //        if (data.BusinessID != "") {
        //            WFOperator_SJSJ.GetProcess({ BusinessID: data.BusinessID, CheckUserInProcess: utils.getQueryString("v").length > 0 ? false : true });
        //        }
        //        callback(data);
        //    }
        //});
    },
    BuildAttachItem: function (attach) {
        var $html = $($("#script_attachment").html());
        $html.find(".tc-attach-item").text(attach.FileName).click(function () {
            commons.DownloadAttachment(attach.ID);
        });
        return $html;
    },
    BuildTaskReportAttachItem: function (attach) {
        var $html = $($("#script_report_attachment").html());
        $html.find(".tc-report-attachment").text(attach.FileName).click(function () {
            commons.DownloadAttachment(attach.ID);
        });
        return $html;
    },
    BuildTaskReportAttachItemLength: function () {
        var attachlength = $(".tc-task-report-attachments .tc-report-attachment").length;
        if (attachlength > 0) {
            $(".tc-task-report-attachlength").text("{0} 个附件".formatBy(attachlength));
        }
    }
};