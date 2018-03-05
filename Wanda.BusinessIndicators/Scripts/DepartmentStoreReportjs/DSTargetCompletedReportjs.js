
var Year;
var Month;
var SystemID;
var IsLatestVersion;
function SearchData() {
    //经营指标完成门店数量情况
    getDSReprotDetailData();
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
            if (ResultData.length > 0) {
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
}
//构建表头
function CreateDetailTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 4%\">序号</th><th rowspan=\"2\" style=\"width: 6%\">项目</th><th rowspan=\"2\" style=\"width: 10%\">指标</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 40%;\" >" + month + "月(万元)</th>";
        strhtml += "     <th colspan=\"4\" style=\"width: 40%\">" + month + "月累计(万元)</th></tr>";
        strhtml += "<tr> <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += "     <th class=\"th_Sub\">计划</th><th class=\"th_Sub\">实际</th><th class=\"th_Sub\">实际与计划差额</th><th class=\"th_Sub\">实际占计划比</th>";
        strhtml += " </tr>";
    }
    else {
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

//区分下载报表
function DownExcelReport(sender) {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var FinMonth = $("#ddlMonth").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetCompleted&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth
    window.open(url);
}