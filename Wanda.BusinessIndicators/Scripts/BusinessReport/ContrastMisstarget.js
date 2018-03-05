
var FinYear;
var SystemID;
var FinMonth;
var ContrastList;
var SystemID;
var SystemName;
var TargetList;

//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}

$(document).ready(
           function () {
               BangSelect();
               var MyDate = new Date();
               FinYear = $("#ddlYear").val();
               FinMonth = $("#ddlMonth").val();

               if (FinMonth == 1) {
                   $("#ContrastValue option[value='0']").remove();
               }

                Select();
             
           });
function BangSelect() {
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetSystemSeq",
        successReturn: function (result) {
            for (var i = 0; i < result.length; i++) {
                if (i == 0) {
                    $("#ddlSystem").append('<option value="0" selected="selected">全部</option>');

                }

                $("#ddlSystem").append('<option value="' + result[i].ID + '">' + result[i].SystemName + '</option>')

            }

        }
    });

}

function GetMonthlyReport() {
    var m = false;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = true;
    }

    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetContrastMisstarget",
        args: { FinYear: FinYear, FinMonth: FinMonth, SystemID: SystemID, IsPro: m },
        successReturn: function (result) {
            ContrastList = result;

        }
    });
}


function BangList() {
    var table = ""
    for (var i = 0; i < ContrastList.length; i++) {
        for (var j = 0; j < ContrastList[i].ContrastMisstarget.length; j++) {
            table += "<tr>";
            if (j == 0) {
                table += "  <td class=\"Td_Center\" rowspan=\"" + ContrastList[i].ContrastMisstarget.length + "\">" + ContrastList[i].SystemName + "</td>";
            }
            table += " <td class=\"Td_Right\">" + ContrastList[i].ContrastMisstarget[j].TargetName + "</td>";
            table += " <td class=\"Td_Right huan\" >" + ContrastList[i].ContrastMisstarget[j].LastEvaluationCompany + "</td>";
            if (ContrastList[i].ContrastMisstarget[j].LastEvaluationCompany == 0) {
                table += "<td class=\"Td_Right huan dang\" style=\"display:none\" >--</td>";
                table += "<td class=\"Td_Right huan dang\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right huan lei\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right huan lei\" style=\"display:none\">--</td>";
            }
            else {
                table += "<td class=\"Td_Right huan lei\" style=\"display:none\" >" + ContrastList[i].ContrastMisstarget[j].LastIsMissCurrent + "</td>";
                table += "<td class=\"Td_Right huan lei\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].LastProportionCurrent + "</td>";
                table += "<td class=\"Td_Right huan dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].LastIsMissTarget + "</td>";
                table += "<td class=\"Td_Right huan dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].LastProportion + "</td>";

            }

            table += " <td class=\"Td_Right tong\" >" + ContrastList[i].ContrastMisstarget[j].YearEvaluationCompany + "</td>";
            if (ContrastList[i].ContrastMisstarget[j].YearEvaluationCompany == 0) {
                table += "<td class=\"Td_Right tong lei\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right tong lei\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right tong dang\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right tong dang\" style=\"display:none\">--</td>";
            }
            else {
                table += "<td class=\"Td_Right tong lei\" style=\"display:none\" >" + ContrastList[i].ContrastMisstarget[j].YearIsMissCurrent + "</td>";
                table += "<td class=\"Td_Right tong lei\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].YearProportionCurrent + "</td>";
                table += "<td class=\"Td_Right tong dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].YearIsMissTarget + "</td>";
                table += "<td class=\"Td_Right tong dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].YearProportion + "</td>";
            }

            table += " <td class=\"Td_Right \" >" + ContrastList[i].ContrastMisstarget[j].ThisEvaluationCompany + "</td>";
            if (ContrastList[i].ContrastMisstarget[j].ThisEvaluationCompany==0) {
                table += "<td class=\"Td_Right  lei\" style=\"display:none\" >--</td>";
                table += "<td class=\"Td_Right  lei\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right  dang\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right  dang\" style=\"display:none\">--</td>";
            }
            else {
                table += "<td class=\"Td_Right  lei\" style=\"display:none\" >" + ContrastList[i].ContrastMisstarget[j].ThisIsMissCurrent + "</td>";
                table += "<td class=\"Td_Right  lei\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].ThisProportionCurrent + "</td>";
                table += "<td class=\"Td_Right  dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].ThisIsMissTarget + "</td>";
                table += "<td class=\"Td_Right  dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].ThisProportion + "</td>";
            }
            if (ContrastList[i].ContrastMisstarget[j].ThisEvaluationCompany == 0 || ContrastList[i].ContrastMisstarget[j].LastEvaluationCompany==0) {
            
                table += "<td class=\"Td_Right huan lei\" style=\"display:none\" >--</td>";
                table += "<td class=\"Td_Right huan lei\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right huan dang\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right huan dang\" style=\"display:none\">--</td>";
              
            }
            else {
          

                if (ContrastList[i].ContrastMisstarget[j].HuanMissTargetChangeCurrent > 0) {
                    table += "<td class=\"Td_Right huan lei\" style=\"display:none;color:red\" >" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetChangeCurrent + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right huan lei\" style=\"display:none\" >" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetChangeCurrent + "</td>";
                }
                if (parseInt(ContrastList[i].ContrastMisstarget[j].HuanMissTargetPCurrent) > 0) {
                    table += "<td class=\"Td_Right huan lei\" style=\"display:none;color:red\">" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetPCurrent + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right huan lei\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetPCurrent + "</td>";
                }
                if (ContrastList[i].ContrastMisstarget[j].HuanMissTargetChange > 0) {
                    table += "<td class=\"Td_Right huan dang\" style=\"display:none;color:red\">" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetChange + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right huan dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetChange + "</td>";
                }
                if (parseInt(ContrastList[i].ContrastMisstarget[j].HuanMissTargetProportion) > 0) {
                    table += "<td class=\"Td_Right huan dang\" style=\"display:none;color:red\">" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetProportion + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right huan dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].HuanMissTargetProportion + "</td>";
                }
             
            }
            if (ContrastList[i].ContrastMisstarget[j].ThisEvaluationCompany == 0 || ContrastList[i].ContrastMisstarget[j].YearEvaluationCompany==0) {
                table += "<td class=\"Td_Right tong lei\" style=\"display:none\" >--</td>";
                table += "<td class=\"Td_Right tong lei\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right tong dang\" style=\"display:none\">--</td>";
                table += "<td class=\"Td_Right tong dang\" style=\"display:none\">--</td></tr>";
            }
            else {
                if (ContrastList[i].ContrastMisstarget[j].TongMissTargetChangeCurrent > 0) {
                    table += "<td class=\"Td_Right tong lei\" style=\"display:none;color:red\" >" + ContrastList[i].ContrastMisstarget[j].TongMissTargetChangeCurrent + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right tong lei\" style=\"display:none\" >" + ContrastList[i].ContrastMisstarget[j].TongMissTargetChangeCurrent + "</td>";
                }
                if (parseInt(ContrastList[i].ContrastMisstarget[j].TongMissTargetPCurrent) > 0) {
                    table += "<td class=\"Td_Right tong lei\" style=\"display:none;color:red\">" + ContrastList[i].ContrastMisstarget[j].TongMissTargetPCurrent + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right tong lei\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].TongMissTargetPCurrent + "</td>";
                }
                if (ContrastList[i].ContrastMisstarget[j].TongMissTargetChange > 0) {
                    table += "<td class=\"Td_Right tong dang\" style=\"display:none;color:red\">" + ContrastList[i].ContrastMisstarget[j].TongMissTargetChange + "</td>";
                }
                else {
                    table += "<td class=\"Td_Right tong dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].TongMissTargetChange + "</td>";
                }

                if (parseInt(ContrastList[i].ContrastMisstarget[j].TongMissTargetProportion) > 0) {
                    table += "<td class=\"Td_Right tong dang\" style=\"display:none;color:red\">" + ContrastList[i].ContrastMisstarget[j].TongMissTargetProportion + "</td></tr>";
                }
                else {
                    table += "<td class=\"Td_Right tong dang\" style=\"display:none\">" + ContrastList[i].ContrastMisstarget[j].TongMissTargetProportion + "</td></tr>";
                }
            }
        }


    }

    $("#rows").html(table);
}


//时间选择为1月时,
function TimeChange() {
    var s = document.getElementById("ContrastValue").options.length;

    var Month = $("#ddlMonth").val();
    if (Month == '01' || Month == '1') {
        // $("#SelectHuan").hide();
        $("#ContrastValue").val(1);
        $("#ContrastValue option[value='0']").remove();
    }
    else {
        if (s == 1) {
            $("#ContrastValue").prepend("<option value='0'>环比</option>");
        }


    }
}

function Select() {
    Load();


    var SelectChange = $("#ContrastValue").val();
    var ChangeTarget = $("#TargetValue").val();
    //var Time = $("#ddlYear").val();
    //Time = Time.split("-");
    FinMonth = $("#ddlMonth").val();
    FinYear = $("#ddlYear").val();
    $(".InnerTargetValue").html("")
    $("#InnerContrastValue").html("");
    $("#InnerContrastValue2").html("");
    if (ChangeTarget == "0") {
        $(".InnerTargetValue").html("累计指标")
    }
    else {
        $(".InnerTargetValue").html("当月指标")
    }
    if (SelectChange == "0") {

        $("#InnerContrastValue").html("环比分析");
        $("#InnerContrastValue2").html("环比");
        $("#NowTime").html(DeleteZero(FinMonth) + "月");

        $("#LastTime").html(FinMonth - 1 + "月");
    }
    else {
        $("#InnerContrastValue").html("同比分析");
        $("#InnerContrastValue2").html("同比");
        $("#NowTime").html(FinYear + "年" + DeleteZero(FinMonth) + "月");
        $("#LastTime").html(FinYear - 1 + "年" + DeleteZero(FinMonth) + "月");
    }
    SystemID = $("#ddlSystem").val();
    SystemName = $("#ddlSystem").find("option:selected").text();
    GetMonthlyReport();
    BangList();
    $(".tong").show();
    $(".huan").show();
    $(".dang").show();
    $(".lei").show();
    if (SelectChange == "0") {//如果选择环比
        $(".tong").hide();
        if (ChangeTarget == "0") {//选择当月
            $(".lei").hide();
        }
        else {
            $(".dang").hide()
        }
    }
    else {
        $(".huan").hide();
        if (ChangeTarget == "0") {//选择当月
            $(".lei").hide();
        }
        else {
            $(".dang").hide()
        }
    }
    Fake();
}
//去掉月份前面的0
function DeleteZero(adj) {
    var s = adj.replace(/^0/, '');
    return s;
}
function DownExcel() {
    var m = false;
    var s = $("#IsPro").attr("checked");
    if (s == "checked") {
        m = true;
    }
    window.open("/AjaxHander/DownExcelContrastMisstarget.ashx?SysID=" + SystemID + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth + "&IsPro=" + m);
}