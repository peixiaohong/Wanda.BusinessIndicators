var FinYear;
var FinMonth;
var MonthlyReportID;
var SysID;
var DetailList
//加载动画开始



$(document).ready(function () {


    if ($("#FinYear").val() != "") {
        FinYear = $("#FinYear").val();
        FinMonth = $("#FinMonth").val();
        SysID = $("#SysID").val();
        $("#ddlYear").val(FinYear);
        $("#ddlSystem").val(SysID);
        $("#ddlMonth").val(FinMonth);
    }
    else {
        FinYear = $("#ddlYear").val();
        FinMonth = $("#ddlMonth").val();
        SysID = $("#ddlSystem").val();
    }
    GetMonthlyReportID();
    var m = true;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = false;
    }

    $("#LastYear").html("" + FinYear - 1 + "年" + "1-" + FinMonth + "月");
    $("#ThisYear").html("" + FinYear + "年" + "1-" + FinMonth + "月");
    if (MonthlyReportID != "") {
        WebUtil.ajax({
            async: false,
            url: "/TargetController/GetContrastList",
            args: { MonthlyReportID: MonthlyReportID, FinYear: FinYear, FinMonth: FinMonth, SystemID: SysID, IfPro: m },
            successReturn: function (result) {
                DetailList = result;
                BangList();
                Change(DetailList[0].TargetID);
            }
        });
    }
   
   

});

function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}

function GetList() {
  
    FinYear = $("#ddlYear").val();
    FinMonth = $("#ddlMonth").val();
    SysID = $("#ddlSystem").val();
    GetMonthlyReportID();
    //var m = false;
    //var s = $("#IsPro").attr("checked");
    //if (s == "checked") {
        m = true;
    //}
   
    $("#LastYear").html("" + FinYear - 1 + "年" + "1-" + FinMonth + "月");
    $("#ThisYear").html("" + FinYear + "年" + "1-" + FinMonth + "月");
        WebUtil.ajax({
            async: false,
            url: "/TargetController/GetContrastList",
            args: { MonthlyReportID: MonthlyReportID, FinYear: FinYear, FinMonth: FinMonth, SystemID: SysID, IfPro: m },
            successReturn: function (result) {
                DetailList = result;
                BangList();
                Change(DetailList[0].TargetID);
            }
        });

}

function GetMonthlyReportID() {
    WebUtil.ajax({
        async: false,
        url: "/MonthlyReportController/GetNewMonthlyreport",
        args: { FinYear: FinYear, FinMonth: FinMonth, SystemID: SysID },
        successReturn: function (result) {
          
                MonthlyReportID = result;
        
        }
    });

}

function BangList() {
    var tabs = "";
    var rows = "";
    $("#tabs").html("");
    $("#rows").html("");
  
    for (var i = 0; i < DetailList.length; i++) {
        var a = 1;
        var b = 1;
        tabs += "<li class=\"sd\" style=\"DISPLAY: list-item\">";
        tabs += "<a class=\"active3\" onclick=\"Change('" + DetailList[i].TargetID + "')\" id=\"tab" + DetailList[i].TargetID + "\">" + DetailList[i].TargetName + "</a>";
        tabs += "</li>";

        rows += " <tr class=\"tr" + DetailList[i].TargetID + "\" onclick=\"TabChange('kb" + DetailList[i].TargetID + "')\" >";
        rows += "<td class=\"Td_Merge\" colspan=\"2\" style=\"text-align: center;\">可比门店小计</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].ContractLastTotal + ">" + uformat(Math.round(DetailList[i].ContractLastTotal)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].ContractNowTotal + ">" + uformat(Math.round(DetailList[i].ContractNowTotal)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].ContractDifference + ">" + uformat(Math.round(DetailList[i].ContractDifference)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" >" + DetailList[i].ContractMounting + "</td>";
        rows += "</tr>";
        for (var j = 0; j < DetailList[i].ContrastList.length; j++) {
            rows += "<tr class=\"kb" + DetailList[i].TargetID + "\">";
            rows += "<td class=\"Td_Center\">" + a + "</td>";
            rows += "<td class=\"Td_Center\">" + DetailList[i].ContrastList[j].CompanyName + "</td>";
            rows += "<td class=\"Td_Center\"title=" + DetailList[i].ContrastList[j].LastAllTotal + ">" + uformat(Math.round(DetailList[i].ContrastList[j].LastAllTotal)) + "</td>";
            rows += "<td class=\"Td_Center\"title=" + DetailList[i].ContrastList[j].NowAllTotal + ">" + uformat(Math.round(DetailList[i].ContrastList[j].NowAllTotal)) + "</td>";
            rows += "<td class=\"Td_Center\"title=" + DetailList[i].ContrastList[j].Difference + ">" +uformat( Math.round(DetailList[i].ContrastList[j].Difference)) + "</td>";
            rows += "<td class=\"Td_Center\">" + DetailList[i].ContrastList[j].Mounting + "</td>";
            rows += "</tr>";
            a = a + 1;
        }
        rows += " <tr class=\"tr" + DetailList[i].TargetID + "\" onclick=\"TabChange('bkb" + DetailList[i].TargetID + "')\" >";
        rows += "<td class=\"Td_Merge\" colspan=\"2\" style=\"text-align: center;\">不可比门店小计</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].NotContractLastTotal + ">" + uformat(Math.round(DetailList[i].NotContractLastTotal)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].NotContractNowTotal + ">" + uformat(Math.round(DetailList[i].NotContractNowTotal)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].NotContractDifference + ">" + uformat(Math.round(DetailList[i].NotContractDifference) )+ "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" >" + DetailList[i].NotContractMounting + "</td>";
        rows += "</tr>";

        for (var h = 0; h < DetailList[i].NotContrastList.length; h++) {
            rows += "<tr class=\"bkb" + DetailList[i].TargetID + "\">";
            rows += "<td class=\"Td_Center\">" + b + "</td>";
            rows += "<td class=\"Td_Center\">" + DetailList[i].NotContrastList[h].CompanyName + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].NotContrastList[h].LastAllTotal + ">" + uformat(Math.round(DetailList[i].NotContrastList[h].LastAllTotal)) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].NotContrastList[h].NowAllTotal + ">" + uformat(Math.round(DetailList[i].NotContrastList[h].NowAllTotal)) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].NotContrastList[h].Difference + ">" + uformat(Math.round(DetailList[i].NotContrastList[h].Difference) )+ "</td>";
            rows += "<td class=\"Td_Center\">" + DetailList[i].NotContrastList[h].Mounting + "</td>";
            rows += "</tr>";
            b = b + 1;

        }
        rows += " <tr  class=\"tr" + DetailList[i].TargetID + "\" >";
        rows += "<td class=\"Td_Merge\" colspan=\"2\" style=\"text-align: center;\">合计</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].LastTotal + ">" +uformat( Math.round(DetailList[i].LastTotal)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].NowTotal + ">" + uformat(Math.round(DetailList[i].NowTotal)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" title=" + DetailList[i].Difference + ">" + uformat(Math.round(DetailList[i].Difference)) + "</td>";
        rows += "<td class=\"Td_Merge\" style=\"text-align: center;\" >" + DetailList[i].Mounting + "</td>";
        rows += "</tr>";

    }
    $("#tabs").html(tabs);
    $("#rows").html(rows);
}
function uformat(num) {
    return (num.toFixed(0) + '').replace(/\d{1,3}(?=(\d{3})+(\.\d*)?$)/g, '$&,');
}



function Change(adj) {
    for (var i = 0; i < DetailList.length; i++) {
        $("#tab" + DetailList[i].TargetID + "").attr("class", "active3");
        $(".tr" + DetailList[i].TargetID + "").hide();
        $(".kb" + DetailList[i].TargetID + "").hide();
        $(".bkb" + DetailList[i].TargetID + "").hide();
    }
    $("#tab" + adj + "").attr("class", "active3 active_sub3");
    $(".tr" + adj + "").show();

}

function TabChange(adj) {
    Load();
    $("." + adj + "").toggle();
    Fake();
}

//页面跳转方法
function ReturnR(SystemID) {
    location.href = "../SystemConfiguration/ContrastCompleteDetail.aspx";
}


function DownExcel() {
    var m = true;
    //var s = $("#IsPro").attr("checked");
    //if (s == "checked") {
    //    m = false;
    //}

    window.open("/AjaxHander/DownExcelContrastDetail.ashx?SysID=" + SysID + "&MonthlyReportID=" + MonthlyReportID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsPro=" + m + "");
}