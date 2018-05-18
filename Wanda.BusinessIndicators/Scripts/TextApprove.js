$(function () {
    $.ajax({
        url: "/AjaxHander/ProcessController.ashx",
        type: "post",
        async: true,
        data: {
            BusinessID: "07b07780-5b83-4389-86f1-4bb347c28030",
            ExecuteType: "afterAction",
            OperatorType: "1",
            PrcessStatus: "Approved",
            strProType: ""
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
})