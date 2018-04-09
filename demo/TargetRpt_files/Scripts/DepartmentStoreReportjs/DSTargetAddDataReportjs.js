
var Year;
var Month;
var SystemID;
var IsLatestVersion;
function SearchData() {
    //新增未完成指标的门店情况
    getMonthReportAddMissTargetData();
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
                            strTbody += "<tr><td colspan=\"11\"  style=\"text-align:left;font-weight:bolder;padding-left:10px; background-color:#e1eaf5; height:40px;\">" + (inde + 1) + "、新增" + this.AddTargetName + "未完成门店如下：</td></tr>";
                        }
                        else {
                            strTbody += "<tr><td colspan=\"14\"  style=\"text-align:left;font-weight:bolder;padding-left:10px; background-color:#e1eaf5;height:40px;\">" + (inde + 1) + "、新增" + this.AddTargetName + "未完成门店如下：</td></tr>";
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
                                        strTbody += "<td>" + this.ReturnTypeDescrible + "</td>";
                                        if (this.Counter == 0) {
                                            strTbody += "<td></td>";
                                        }
                                        else {
                                            strTbody += "<td><img src=\"../Images/images1/image" + this.Counter + ".png\" style=\"width:15px; height:15px;line-height: 25px;\" /></td>";
                                        }
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
                                        strTbody += "<td>" + this.ReturnTypeDescrible + "</td>";
                                        if (this.Counter == 0) {
                                            strTbody += "<td></td>";
                                        }
                                        else {
                                            strTbody += "<td><img src=\"../Images/images1/image" + this.Counter + ".png\" style=\"width:15px; height:15px;line-height: 25px;\" /></td>";
                                        }
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
    var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetAddData&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
    window.open(url);

}