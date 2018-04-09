

var Year;
var Month;
var SystemID;
var IsLatestVersion;
function SearchData() {
    //补回上月经营指标缺口情况
    getMonthReportReturnData();
    SystemID = $("#ddlSystem").val();
    Year = $("#ddlYear").val();
    Month = $("#ddlMonth").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    IsLatestVersion = latest;
}

//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}

//初始化数据方法
$(document).ready(function () {
    SearchData();
});

//-------------补回上月经营指标缺口情况------------------------------------------------------------------------------------------------------
function getMonthReportReturnData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetReturnDataList",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {
            var tableHead = CreateReturnTableHeadHtml($("#ddlMonth").val());
            $("#DSReturn_Thead").empty();
            $("#DSReturn_Thead").append(tableHead);
            var strTrueTbody = "";
            var strFalseTbody = "";
            if ($("#ddlMonth").val() == 1) {
                strTrueTbody += "<tr><td colspan=\"11\" style=\"text-align:left;font-weight:bolder;padding-left:10px; background-color:#e1eaf5; height:40px;\">一、完全补回上月指标缺口的商家如下：</td></tr>";
                strFalseTbody += "<tr><td colspan=\"11\" style=\"text-align:left;font-weight:bolder;padding-left:10px; background-color:#e1eaf5; height:40px;\">二、部分补回上月指标缺口的商家如下：</td></tr>";
            }
            else {
                strTrueTbody += "<tr><td colspan=\"14\" style=\"text-align:left;font-weight:bolder;padding-left:10px; background-color:#e1eaf5; height:40px;\">一、完全补回上月指标缺口的商家如下：</td></tr>";
                strFalseTbody += "<tr><td colspan=\"14\" style=\"text-align:left;font-weight:bolder;padding-left:10px; background-color:#e1eaf5; height:40px;\">二、部分补回上月指标缺口的商家如下：</td></tr>";
            }
            $("#Tbody_Data").empty();
            if (ResultData.length > 0) {
                var trueindex = 1;
                var falseIndex = 1;
                $(ResultData).each(function () {
                    if (this.IsAllReturn) {
                        var rcount = this.ReturnDataList.length;
                        var model = this;
                        $(this.ReturnDataList).each(function (i) {
                            if (i == 0) {
                                strTrueTbody += "<tr>";
                                strTrueTbody += "<td rowspan=\"" + rcount + "\">" + trueindex + "</td><td rowspan=\"" + rcount + "\" style=\"text-align:left;padding-left:10px; \">" + model.CompanyName + "</td>";
                                strTrueTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.ReturnTargetName + "</td><td>" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strTrueTbody += "<td>" + this.LastAccumulativePlan + "</td><td>" + this.LastAccumulativeActual + "</td>";
                                    strTrueTbody += "<td>" + this.LastAccumulativeDifference + "</td>";
                                }
                                strTrueTbody += "<td>" + this.CurrentAccumulativePlan + "</td>";
                                strTrueTbody += "<td>" + this.CurrentAccumulativeActual + "</td><td>" + this.CurrentAccumulativeDifference + "</td>";
                                strTrueTbody += "<td>" + this.CurrentAccumulativeRate + "</td><td>" + this.CommitDate + "</td>";
                                strTrueTbody += "<td>" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strTrueTbody += "<td></td>";
                                }
                                else {
                                    strTrueTbody += "<td><img src=\"../Images/images1/image" + this.Counter + ".png\" style=\"width:15px; height:15px;line-height: 25px;\" /></td>";
                                }
                                strTrueTbody += "</tr>";
                            }
                            else {
                                strTrueTbody += "<tr>";
                                strTrueTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.ReturnTargetName + "</td><td>" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strTrueTbody += "<td>" + this.LastAccumulativePlan + "</td><td>" + this.LastAccumulativeActual + "</td>";
                                    strTrueTbody += "<td>" + this.LastAccumulativeDifference + "</td>";
                                }
                                strTrueTbody += "<td>" + this.CurrentAccumulativePlan + "</td>";
                                strTrueTbody += "<td>" + this.CurrentAccumulativeActual + "</td><td>" + this.CurrentAccumulativeDifference + "</td>";
                                strTrueTbody += "<td>" + this.CurrentAccumulativeRate + "</td><td>" + this.CommitDate + "</td>";
                                strTrueTbody += "<td>" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strTrueTbody += "<td></td>";
                                }
                                else {
                                    strTrueTbody += "<td><img src=\"../Images/images1/image" + this.Counter + ".png\" style=\"width:15px; height:15px;line-height: 25px;\" /></td>";
                                }
                                strTrueTbody += "</tr>";
                            }
                        })
                        trueindex++;
                    }
                    else {
                        var rcount = this.ReturnDataList.length;
                        var model = this;
                        $(this.ReturnDataList).each(function (i) {
                            if (i == 0) {
                                strFalseTbody += "<tr>";
                                strFalseTbody += "<td rowspan=\"" + rcount + "\">" + falseIndex + "</td><td rowspan=\"" + rcount + "\" style=\"text-align:left;padding-left:10px; \">" + model.CompanyName + "</td>";
                                strFalseTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.ReturnTargetName + "</td><td>" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strFalseTbody += "<td>" + this.LastAccumulativePlan + "</td><td>" + this.LastAccumulativeActual + "</td>";
                                    strFalseTbody += "<td>" + this.LastAccumulativeDifference + "</td>";
                                }
                                strFalseTbody += "<td>" + this.CurrentAccumulativePlan + "</td>";
                                strFalseTbody += "<td>" + this.CurrentAccumulativeActual + "</td><td>" + this.CurrentAccumulativeDifference + "</td>";
                                strFalseTbody += "<td>" + this.CurrentAccumulativeRate + "</td><td>" + this.CommitDate + "</td>";
                                strFalseTbody += "<td>" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strFalseTbody += "<td></td>";
                                }
                                else {
                                    strFalseTbody += "<td><img src=\"../Images/images1/image" + this.Counter + ".png\" style=\"width:15px; height:15px;line-height: 25px;\" /></td>";
                                }
                                strFalseTbody += "</tr>";
                            }
                            else {
                                strFalseTbody += "<tr>";
                                strFalseTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.ReturnTargetName + "</td><td>" + this.CurrentReturnAmount + "</td>";
                                if ($("#ddlMonth").val() > 1) {
                                    strFalseTbody += "<td>" + this.LastAccumulativePlan + "</td><td>" + this.LastAccumulativeActual + "</td>";
                                    strFalseTbody += "<td>" + this.LastAccumulativeDifference + "</td>";
                                }
                                strFalseTbody += "<td>" + this.CurrentAccumulativePlan + "</td>";
                                strFalseTbody += "<td>" + this.CurrentAccumulativeActual + "</td><td>" + this.CurrentAccumulativeDifference + "</td>";
                                strFalseTbody += "<td>" + this.CurrentAccumulativeRate + "</td><td>" + this.CommitDate + "</td>";
                                strFalseTbody += "<td>" + this.ReturnTypeDescrible + "</td>";
                                if (this.Counter == 0) {
                                    strFalseTbody += "<td></td>";
                                }
                                else {
                                    strFalseTbody += "<td><img src=\"../Images/images1/image" + this.Counter + ".png\" style=\"width:15px; height:15px;line-height: 25px;\" /></td>";
                                }
                                strFalseTbody += "</tr>";
                            }
                        })
                        falseIndex++;
                    }
                })
                $("#Tbody_Data").empty();
                $("#Tbody_Data").append(strTrueTbody);
                $("#Tbody_Data").append(strFalseTbody);
            }
        }
    });
}

//构建表头
function CreateReturnTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">公司名称</th><th rowspan=\"2\" style=\"width: 6%\">经营指标</th>";
        strhtml += "     <th rowspan=\"2\" style=\"width: 8%\">本月补回<br /> /新增差额</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 32%\">" + month + "月累计(万元)</th><th colspan=\"2\" style=\"width: 16%\">补回说明</th><th rowspan=\"2\" style=\"width: 4%\">警示灯</th></tr>";
        strhtml += "<tr>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">完成率</th>";
        strhtml += "     <th class=\"th_Sub\">要求期限</th><th class=\"th_Sub \">补回情况</th>";
        strhtml += " </tr>";
    }
    else {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">公司名称</th><th rowspan=\"2\" style=\"width: 6%\">经营指标</th>";
        strhtml += "     <th rowspan=\"2\" style=\"width: 8%\">本月补回<br /> /新增差额</th><th colspan=\"3\" style=\"width: 24%;\" >1-" + (month - 1) + "月累计(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 32%\">1-" + month + "月累计(万元)</th><th colspan=\"2\" style=\"width: 16%\">补回说明</th><th rowspan=\"2\" style=\"width: 4%\">警示灯</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub shangyueleiji\">完成率</th>";
        strhtml += "     <th class=\"th_Sub\">要求期限</th><th class=\"th_Sub \">补回情况</th>";
        strhtml += " </tr>";
    }
    return strhtml;
}

//区分下载报表
function DownExcelReport(sender) {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var FinMonth = $("#ddlMonth").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetReturnData&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
    window.open(url);
}