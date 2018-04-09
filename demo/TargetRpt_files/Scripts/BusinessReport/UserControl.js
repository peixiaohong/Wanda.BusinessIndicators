function ClickItems(sender) {
    //为隐藏域赋值
    if ($("#" + sender + "Span").attr("class") == "txt") {

        if (setArrow_nLeft(sender)) {
            document.getElementById("ContentPlaceHolder1_UserControl_ItemStateHidden").value = sender;
            $(".flow_nav .current").each(function () {
                $(this).removeClass("current");
            })

            $("#" + sender).addClass("current");

            operateNav(sender);
        }

    }
}


//为了添加提示信息
function setArrow_nLeft(sender) {
    switch (sender) {
        case "downLoadTemplate":
            $(".arrow_n").css("left", "80px");
            $("#PromptMessage").html("请下载填报模版。");
            $("#PromptMessage").removeClass("Tishi");
            return true;
            break;
        case "dataUpload":
            $(".arrow_n").css("left", "250px");
            $("#PromptMessage").html("请使用系统提供的填报模板导入本月数据，如已导入，再次导入将覆盖当前数据。");
            $("#PromptMessage").removeClass("Tishi");
            return true;
            break;
        case "missTargetReport":
            $(".arrow_n").css("left", "420px");
            $("#PromptMessage").html("请填补回情况，未完成原因及措施。");
            $("#PromptMessage").removeClass("Tishi");
            return true;
            break;

        case "missCurrentTargetReport":
            $(".arrow_n").css("left", "590px");
            $("#PromptMessage").html("请先将累计未完成的补回情况填写完后，再填写当月的补回情况，未完成原因及措施。");
            $("#PromptMessage").addClass("Tishi");
            return true;
            break;

        
        case "monthReportReady":
            if (confirm("请您确认是否保存月度上报")) {
                $(".arrow_n").css("left", "760px");
                $("#PromptMessage").html("请保存数据，等待其它上报人员填报。");
                $("#PromptMessage").removeClass("Tishi");
                return true;
            } else {
                return false;
            }
            break;
        case "monthReport":
            $(".arrow_n").css("left", "930px");
            $("#PromptMessage").html("请填写月报说明。");
            $("#PromptMessage").removeClass("Tishi");

            return true;
            break;

        case "monthReportSubmit":
            if (confirm("请您确认是否提交月度上报")) {
                $(".arrow_n").css("left", "1060");
                $("#PromptMessage").html("请仔细选择加签，保证分管副总裁审批节点以前审批人完整。");
                $("#PromptMessage").addClass("Tishi");
                return true;
            } else {
                return false;
            }
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
            if (ItemDisplay + "Title" != item.id && ItemDisplay1 + "Title" != item.id  ) {
                item.innerHTML = j * 1 + 1;
                j++;
            } else {
                $("#" + ItemDisplay).css("display", "none"); // 这个是累计未完成
                $("#" + ItemDisplay1).css("display", "none"); // 这个是当月未完成
            }
        }

    })
});

