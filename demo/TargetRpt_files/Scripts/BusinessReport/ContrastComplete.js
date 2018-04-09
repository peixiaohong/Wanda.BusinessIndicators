var DetailList;
var FinYear;
var FinMonth;
var AddSysId;
var AddTarId;

$(document).ready(function () {

    GetList();

});

function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}



function GetList() {
    //Load();
    var m = true;
    var s=  $("#IsPro").attr("checked");
    if (s == "checked") {
        m = false;
    }

    FinYear = $("#ddlYear").val();
    FinMonth = $("#ddlMonth").val();
    $(".Last").html("");
    $(".Now").html("");
    $(".Last").html(FinYear - 1 + "年" + FinMonth + "月");
    $(".Now").html(FinYear + "年" + FinMonth + "月");
    WebUtil.ajax({
        async: true,
        url: "/CompanyController/GetContrastDetail",
        args: { FinYear: FinYear, FinMonth: FinMonth, IsPro: m },
        successReturn: function (result) {
            DetailList = result;
            BangList();
        }
    });
    //Fake();


    //var obj3 = $("#head");
    //var tab3 = $("#rows");
    //FloatHeader(obj3, tab3);

}


function BangList() {
    $("#rows").html("");
    var rows = "";
    var a = 1;
    for (var i = 0; i < DetailList.length; i++) {
        for (var j = 0; j < DetailList[i].ContrastDetailMl.length; j++) {
            rows += "<tr>";
            if (j == 0) {
                rows += "<td rowspan=" + DetailList[i].ContrastDetailMl.length + " class=\"Td_Center\">" + a + "</td>";
                rows += "  <td rowspan=" + DetailList[i].ContrastDetailMl.length + " class=\"Td_Center\">";
                rows += "<a style=\"color:blue;text-decoration: underline; cursor:pointer\"  onclick=\"CheckList('" + DetailList[i].systemID + "')\">" + DetailList[i].SystemName + "</a></td>";
            }
            //系统整体
            rows += "<td class=\"Td_Center\">" + DetailList[i].ContrastDetailMl[j].TargetName + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].LastAllTotal + ">" + Math.round(DetailList[i].ContrastDetailMl[j].LastAllTotal) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].NowAllTotal + ">" + Math.round(DetailList[i].ContrastDetailMl[j].NowAllTotal) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].Difference + ">" + Math.round(DetailList[i].ContrastDetailMl[j].Difference) + "</td>";
            if (Analysis(DetailList[i].ContrastDetailMl[j].Mounting) == true && DetailList[i].ContrastDetailMl[j].Mounting != "--") {
                rows += "<td class=\"Td_Center\" style=\"color: red\">" + DetailList[i].ContrastDetailMl[j].Mounting + "</td>";
            }
            else {
                rows += "<td class=\"Td_Center\">" + DetailList[i].ContrastDetailMl[j].Mounting + "</td>";
            }

            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].PossibleContrastLast + ">" + Math.round(DetailList[i].ContrastDetailMl[j].PossibleContrastLast) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].PossibleContrastNow + ">" + Math.round(DetailList[i].ContrastDetailMl[j].PossibleContrastNow) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].PossibleDifference + ">" + Math.round(DetailList[i].ContrastDetailMl[j].PossibleDifference) + "</td>";
            if (Analysis(DetailList[i].ContrastDetailMl[j].PossibleMounting) == true && DetailList[i].ContrastDetailMl[j].PossibleMounting != "--") {
                rows += "<td class=\"Td_Center\" style=\"color: red\">" + DetailList[i].ContrastDetailMl[j].PossibleMounting + "</td>";
            }
            else {
                rows += "<td class=\"Td_Center\">" + DetailList[i].ContrastDetailMl[j].PossibleMounting + "</td>";
            }


            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].NotContrastLast + ">" + Math.round(DetailList[i].ContrastDetailMl[j].NotContrastLast) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].NotContrastNow + ">" + Math.round(DetailList[i].ContrastDetailMl[j].NotContrastNow) + "</td>";
            rows += "<td class=\"Td_Center\" title=" + DetailList[i].ContrastDetailMl[j].NotDifference + ">" + Math.round(DetailList[i].ContrastDetailMl[j].NotDifference) + "</td>";
            if (Analysis(DetailList[i].ContrastDetailMl[j].NotMounting) == true && DetailList[i].ContrastDetailMl[j].NotMounting != "--") {
                rows += "<td class=\"Td_Center\" style=\"color: red\">" + DetailList[i].ContrastDetailMl[j].NotMounting + "</td>";
            }
            else {
                rows += "<td class=\"Td_Center\">" + DetailList[i].ContrastDetailMl[j].NotMounting + "</td>";
            }


            rows += "<td class=\"Td_TrueLeft\">" + DetailList[i].ContrastDetailMl[j].Remark + "</td>";
            rows += "<td class=\"Td_Center\"><a href=\"#\" onclick=\"ChangRemark('" + DetailList[i].ContrastDetailMl[j].SystemID + "','" + DetailList[i].ContrastDetailMl[j].TargetID + "')\">"; ;
            rows += "编辑</a></td>";
            
            rows += "</tr>";

        }
        a = a + 1;
    }
    $("#rows").html(rows);
}


//页面跳转方法
function CheckList(SystemID) {
    var m = true;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = false;
    }
    window.open("../SystemConfiguration/ContrastDetail.aspx?SystemID=" + SystemID + "&FinMonth=" + FinMonth + "&FinYear=" + FinYear + "&IsPro=" + m + "");
}

function Analysis(value) {
    var str = false;
    if (value.indexOf("增亏") >= 0) {
        value = true;
    }
    else {
        if (value.indexOf("减亏") < 0) {
            if (value.indexOf("-") >= 0) {
                str = true;
            }
        }
    }
    return str
}




function ChangRemark(SystemID, TargetID) {
    var m = true;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = false;
    }
    AddSysId = SystemID;
    AddTarId = TargetID;
    $("#Remark").val("");
    $("#RemarkSystem").html("");
    $("#RemarkTarget").html("");
    WebUtil.ajax({
        async: true,
        url: "/CompanyController/GetContrastRemark",
        args: { SystemID: AddSysId, TargetID: AddTarId, FinMonth: FinMonth, FinYear: FinYear, IsPro: m },
        successReturn: function (result) {
          
            $("#Remark").val(result.EvaluationRemark);
            $("#RemarkSystem").html(result.SystemName);
            $("#RemarkTarget").html(result.TargetName);
           
        }
    });
    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>编辑备注</span>'
    });
}


function AddRemark() {
    var m = true;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = false;
    }
    var RemarkDetail = $("#Remark").val();
    WebUtil.ajax({
        async: true,
        url: "/CompanyController/UpdateContrastRemark",
        args: { SystemID: AddSysId, TargetID: AddTarId, FinMonth: FinMonth, FinYear: FinYear, Remark: RemarkDetail, IsPro: m },
        successReturn: function (result) {
            if (result!=null) {
               
                GetList();
                art.dialog({ id: 'divDetail' }).close();
            }
        }
    });
}


function DownExcel() {
    var m = true;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = false;
    }
    window.open("/AjaxHander/DownExcelContrastComplete.ashx?FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsPro=" + m+"");

}