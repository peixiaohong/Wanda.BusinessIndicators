/// <reference path="../AjaxHander/DownLoadDSExcel.aspx" />

//变量误删
var ReturnData = {};
var MissTargetData = {};
var ComplateDetailData = {};
var MonthReportData = {};

var Year;
var Month;
var SystemID;
var IsLatestVersion;
var IsNewDataIndex = "";
function SearchData() {
    var temp;
    temp = $(".m_1 > a");
    $(temp).addClass("selected");
    ChangeTargetDetail(temp, "Search");
    IsNewDataIndex = "";
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
    //经营指标完成门店数量情况
    getDSReportSummaryData();
    SystemID = $("#ddlSystem").val();
    Year = $("#ddlYear").val();
    Month = $("#ddlMonth").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    IsLatestVersion = latest;
});

//切换不同的报表
function ChangeTargetDetail(sender, TabOrSearch) {
    $(".active_sub2").each(function () {
        $(this).removeClass("active_sub2");
        $(this).parent().removeClass("selected");
    });
    $(sender).addClass("active_sub2");
    $(sender).parent().addClass("selected");
    $('#LabelDownload').text("导出" + $(sender).text());
    var CTDSystemID = $("#ddlSystem").val();
    var CTDYear = $("#ddlYear").val();
    var CTDMonth = $("#ddlMonth").val();
    var CTDlatest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        CTDlatest = true;
    }
    var CTDIsLatestVersion = CTDlatest;
    if ($(sender).text() == "经营指标完成门店数量情况" || TabOrSearch == "Search") {
        $('#T1').show();
        $('#T2').hide();
        $("#T3").hide();
        $("#T4").hide();
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, MonthReportData[0], "A") == true) {
            getDSReportSummaryData();
        }
    } else if ($(sender).text() == "完成情况明细" && TabOrSearch != "Search") {
        $('#T1').hide();
        $('#T2').show();
        $("#T3").hide();
        $("#T4").hide();
        //完成情况明细
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ComplateDetailData[0], "B") == true) {
            getDSReprotDetailData();
        }
    } else if ($(sender).text() == "补回上月经营指标缺口情况" && TabOrSearch != "Search") {
        $('#T1').hide();
        $('#T2').hide();
        $("#T3").show();
        $("#T4").hide();
        //补回上月经营指标缺口情况
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "C") == true) {
            getMonthReportReturnData();
        }
    }
    else if ($(sender).text() == "新增经营指标未完成门店情况" && TabOrSearch != "Search") {
        $('#T1').hide();
        $('#T2').hide();
        $("#T3").hide();
        $("#T4").show();
        //新增经营指标未完成门店情况
        if (TransitionCondition(CTDYear, CTDMonth, CTDSystemID, CTDIsLatestVersion, ReturnData[0], "D") == true) {
            getMonthReportAddMissTargetData();
        }
    }
    if (TabOrSearch != "Search") {
        SystemID = $("#ddlSystem").val();
        Year = $("#ddlYear").val();
        Month = $("#ddlMonth").val();
        var latest = false;
        if ($("#chkIsLastestVersion").attr("checked") == "checked") {
            latest = true;
        }
        IsLatestVersion = latest;
    }
}
//
function TransitionCondition(TCYear, TCMonth, TCSystemID, TCIsLatestVersion, resultDate, index) {
    if (resultDate == undefined) {
        return true;
    }
    if (IsNewDataIndex.indexOf(index) < 0) {
        return true;
    }
    if (TCYear == Year && TCMonth == Month && TCSystemID == SystemID && TCIsLatestVersion == IsLatestVersion) {
        return false;
    } else {
        return true;
    }

}

//----------------------经营指标完成门店数量情况-----------------------------------------------------
function getDSReportSummaryData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetCompleted",
        args: {Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {
            var tableHead = CreateTableHeadHtml($("#ddlMonth").val());
            $("#MonthReportSummaryHead").empty();
            $("#MonthReportSummaryHead").append(tableHead);
            $("#rows").empty();
            if (ResultData.length > 0) {
                var strTbody = "";
                $(ResultData).each(function (i) {
                    strTbody += "<tr> <td>" + (i + 1) + "</td><td style=\"text-align:left;padding-left:10px;\">" + this.PorjectName + "</td>";
                    if ($("#ddlMonth").val() > 1) {
                        strTbody += "     <td>" + this.LastNorth + "</td><td>" + this.LastCenter + "</td>";
                        strTbody += "     <td>" + this.LastSouth + "</td><td>" + this.LastTotal + "</td>";
                    }
                    strTbody += "     <td>" + this.CurrentNorth + "</td><td>" + this.CurrentCenter + "</td>";
                    strTbody += "     <td>" + this.CurrentSouth + "</td><td>" + this.CurrentTotal + "</td>";
                    strTbody += "     <td>" + this.ToCurrentNorth + "</td><td>" + this.ToCurrentCenter + "</td>";
                    strTbody += "     <td>" + this.ToCurrentSouth + "</td><td>" + this.ToCurrentTotal + "</td></tr>";
                })
                $("#rows").empty();
                $("#rows").append(strTbody);
            }
        }
    });
    if (IsNewDataIndex.indexOf("A") < 0) {
        IsNewDataIndex = IsNewDataIndex + "A";
    }
}
//构建表头
function CreateTableHeadHtml(month)
{
    var strhtml = "";
    if (month == 1)
    {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 5%\">序号</th><th rowspan=\"2\" style=\"width: 15%\">项目</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">" + month + "月(个)</th><th colspan=\"4\" style=\"width: 25%\">" + month + "月累计(个)</th></tr>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
    }
    else
    {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 5%\">序号</th><th rowspan=\"2\" style=\"width: 15%\">项目</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">1-" + (month - 1) + "月累计(个)</th><th colspan=\"4\" style=\"width: 25%;\" >" + month + "月累计(个)</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">1-" + month + "月累计(个)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th></tr>";
    }
    return strhtml;
}
//---------------------完成情况明细---------------------------------------------------------------------
//加载完成情况明细数据
function getDSReprotDetailData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetCompletedDetail",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {
            var tableHead = CreateDetailTableHeadHtml($("#ddlMonth").val());
            $("#CompleteDetailHead").empty();
            $("#CompleteDetailHead").append(tableHead);
            $("#tab2_rows").empty();
            if (ResultData.length > 0)
            {
                var strTbody = "";
                $(ResultData).each(function () {
                    var rcount = this.DetailList.length;
                    var model = this;
                    $(this.DetailList).each(function (i) {
                        if (i == 0) {
                            strTbody += "<tr>";
                            strTbody += "<td rowspan=\"" + rcount + "\">" + model.ID + "</td><td rowspan=\"" + rcount + "\" style=\"text-align:left;padding-left:10px; \">" + model.AreaName + "</td>";
                            strTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.DetailTargetName + "</td>";
                            if ($("#ddlMonth").val() > 1) {
                                strTbody += "<td>" + this.LastPlan + "</td><td>" + this.LastActual + "</td>";
                                strTbody += "<td>" + this.LastDifference + "</td><td>" + this.LastRate + "</td>";
                            }
                            strTbody += "<td>" + this.CurrentPlan + "</td><td>" + this.CurrentActual + "</td>";
                            strTbody += "<td>" + this.CurrentDifference + "</td><td>" + this.CurrentRate + "</td>";
                            strTbody += "<td>" + this.ToCurrentPlan + "</td><td>" + this.ToCurrentActual + "</td>";
                            strTbody += "<td>" + this.ToCurrentDifference + "</td><td>" + this.ToCurrentRate + "</td>";
                            strTbody += "</tr>";
                        }
                        else {
                            strTbody += "<tr>";
                            strTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.DetailTargetName + "</td>";
                            if ($("#ddlMonth").val() > 1) {
                                strTbody += "<td>" + this.LastPlan + "</td><td>" + this.LastActual + "</td>";
                                strTbody += "<td>" + this.LastDifference + "</td><td>" + this.LastRate + "</td>";
                            }
                            strTbody += "<td>" + this.CurrentPlan + "</td><td>" + this.CurrentActual + "</td>";
                            strTbody += "<td>" + this.CurrentDifference + "</td><td>" + this.CurrentRate + "</td>";
                            strTbody += "<td>" + this.ToCurrentPlan + "</td><td>" + this.ToCurrentActual + "</td>";
                            strTbody += " <td>" + this.ToCurrentDifference + "</td><td>" + this.ToCurrentRate + "</td>";
                            strTbody += "</tr>";
                        }
                    })
                })
                $("#tab2_rows").empty();
                $("#tab2_rows").append(strTbody);
            }
        }
    });
    if (IsNewDataIndex.indexOf("B") < 0) {
        IsNewDataIndex = IsNewDataIndex + "B";
    }
}
//构建表头
function CreateDetailTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1)
    {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">项目</th><th rowspan=\"2\" style=\"width: 10%\">指标</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 40%;\" >" + month + "月(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 40%\">" + month + "月累计(万元)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += " </tr>";
    }
    else
    {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 5%\">项目</th><th rowspan=\"2\" style=\"width: 7%\">指标</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 28%\">" + (month - 1) + "月(万元)</th><th colspan=\"4\" style=\"width: 28%;\" >" + month + "月(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 28%\">1-" + month + "月累计(万元)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += " </tr>";
    }
    return strhtml;
}
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
                strTrueTbody += "<tr><td colspan=\"11\" style=\"text-align:left;font-weight:bolder;padding-left:10px;\">一、完全补回上月指标缺口的商家如下：</td></tr>";
                strFalseTbody += "<tr><td colspan=\"11\" style=\"text-align:left;font-weight:bolder;padding-left:10px;\">二、部分补回上月指标缺口的商家如下：</td></tr>";
            }
            else {
                strTrueTbody += "<tr><td colspan=\"14\" style=\"text-align:left;font-weight:bolder;padding-left:10px;\">一、完全补回上月指标缺口的商家如下：</td></tr>";
                strFalseTbody += "<tr><td colspan=\"14\" style=\"text-align:left;font-weight:bolder;padding-left:10px;\">二、部分补回上月指标缺口的商家如下：</td></tr>";
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
                                strTrueTbody += "<td>" + this.ReturnTypeDescrible + "</td><td>" + this.Counter + "</td>";
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
                                strTrueTbody += "<td>" + this.ReturnTypeDescrible + "</td><td>" + this.Counter + "</td>";
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
                                strFalseTbody += "<td>" + this.ReturnTypeDescrible + "</td><td>" + this.Counter + "</td>";
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
                                strFalseTbody += "<td>" + this.ReturnTypeDescrible + "</td><td>" + this.Counter + "</td>";
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
    if (IsNewDataIndex.indexOf("C") < 0) {
        IsNewDataIndex = IsNewDataIndex + "C";
    }
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
//-------------百货系统经营指标新增未完成指标的门店情况--------------------------------------------------------------------------------------------
function getMonthReportAddMissTargetData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    //经营指标新增未完成指标的门店情况
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetAddMissDataList",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
        successReturn: function (ResultData) {
            var tableHead = CreateAddTableHeadHtml($("#ddlMonth").val());
            $("#DSAdd_Thead").empty();
            $("#DSAdd_Thead").append(tableHead);
            $("#AddTarget_Tbody").empty();
            if (ResultData.length > 0) {
                var tarList = new Array();
                var sameObjCount = 0;
                $(ResultData).each(function () {
                    //去除重复元素
                    if (tarList.length > 0) {
                        for (var a = 0; a < tarList.length; a++) {
                            if (this.AddTargetName == tarList[a].AddTargetName) {
                                sameObjCount++;
                            }
                        }
                        if (sameObjCount > 0) {
                            sameObjCount = 0;
                        }
                        else {
                            tarList.push(this);
                        }
                    }
                    else {
                        tarList.push(this);
                    }
                });
                //拼接html代码
                var TargetList = tarList;
                if (TargetList.length > 0) {
                    var strTbody = "";
                    $(TargetList).each(function (inde) {
                        if ($("#ddlMonth").val() == 1) {
                            strTbody += "<tr><td colspan=\"11\" style=\"text-align:left;font-weight:bolder;padding-left:10px;\">" + (inde+1) + "、新增" + this.AddTargetName + "未完成门店如下：</td></tr>";
                        }
                        else {
                            strTbody += "<tr><td colspan=\"14\" style=\"text-align:left;font-weight:bolder;padding-left:10px;\">" + (inde + 1) + "、新增" + this.AddTargetName + "未完成门店如下：</td></tr>";
                        }
                        var TargetModel = this;
                        var index = 1;
                        $(ResultData).each(function () {
                            if (this.AddTargetName == TargetModel.AddTargetName) {
                                var model = this;
                                $(this.ReturnDataList).each(function (i) {
                                    if (i == 0) {
                                        strTbody += "<tr>";
                                        strTbody += "<td rowspan=\"" + model.ReturnDataList.length + "\">" + index + "</td><td rowspan=\"" + model.ReturnDataList.length + "\" style=\"text-align:left;padding-left:10px; \">" + model.CompanyName + "</td>";
                                        strTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.ReturnTargetName + "</td><td>" + this.CurrentReturnAmount + "</td>";
                                        if ($("#ddlMonth").val() > 1) {
                                            strTbody += "<td>" + this.LastAccumulativePlan + "</td><td>" + this.LastAccumulativeActual + "</td>";
                                            strTbody += "<td>" + this.LastAccumulativeDifference + "</td>";
                                        }
                                        strTbody += "<td>" + this.CurrentAccumulativePlan + "</td>";
                                        strTbody += "<td>" + this.CurrentAccumulativeActual + "</td><td>" + this.CurrentAccumulativeDifference + "</td>";
                                        strTbody += "<td>" + this.CurrentAccumulativeRate + "</td><td>" + this.CommitDate + "</td>";
                                        strTbody += "<td>" + this.ReturnTypeDescrible + "</td><td>" + this.Counter + "</td>";
                                        strTbody += "</tr>";
                                    }
                                    else {
                                        strTbody += "<tr>";
                                        strTbody += "<td style=\"text-align:left;padding-left:10px; \">" + this.ReturnTargetName + "</td><td>" + this.CurrentReturnAmount + "</td>";
                                        if ($("#ddlMonth").val() > 1) {
                                            strTbody += "<td>" + this.LastAccumulativePlan + "</td><td>" + this.LastAccumulativeActual + "</td>";
                                            strTbody += "<td>" + this.LastAccumulativeDifference + "</td>";
                                        }
                                        strTbody += "<td>" + this.CurrentAccumulativePlan + "</td>";
                                        strTbody += "<td>" + this.CurrentAccumulativeActual + "</td><td>" + this.CurrentAccumulativeDifference + "</td>";
                                        strTbody += "<td>" + this.CurrentAccumulativeRate + "</td><td>" + this.CommitDate + "</td>";
                                        strTbody += "<td>" + this.ReturnTypeDescrible + "</td><td>" + this.Counter + "</td>";
                                        strTbody += "</tr>";
                                    }
                                })
                                index++;
                            }
                        })
                    })
                    $("#AddTarget_Tbody").empty();
                    $("#AddTarget_Tbody").append(strTbody);
                }
            }
        }
    });
    if (IsNewDataIndex.indexOf("D") < 0) {
        IsNewDataIndex = IsNewDataIndex + "D";
    }
}
//构建表头
function CreateAddTableHeadHtml(month) {
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
        strhtml += "<tr><th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">完成率</th>";
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
    var txtValue = $(sender).text().replace(" ","");
    if (txtValue == "导出经营指标完成门店数量情况") {
        var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetSum&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth;
        window.open(url);
    } else if (txtValue == "导出完成情况明细") {
        var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetCompleted&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
        window.open(url);
    } else if (txtValue == "导出补回上月经营指标缺口情况") {
        var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetReturnData&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
        window.open(url);
    }
    else if (txtValue == "导出新增经营指标未完成门店情况") {
        var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetAddData&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
        window.open(url);
    }
}