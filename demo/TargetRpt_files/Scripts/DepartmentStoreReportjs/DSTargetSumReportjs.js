
var Year;
var Month;
var SystemID;
var IsLatestVersion;
var IsNewDataIndex = "";
function SearchData() {
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

//----------------------经营指标完成门店数量情况-----------------------------------------------------
function getDSReportSummaryData() {
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    WebUtil.ajax({
        async: true,
        url: "/DepartmentStoreReportController/GetDSTargetCompleted",
        args: { Year: $("#ddlYear").val(), Month: $("#ddlMonth").val(), IsLatestVersion: latest },
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
}
//构建表头
function CreateTableHeadHtml(month) {
    var strhtml = "";
    if (month == 1) {
        strhtml += "<tr> <th rowspan=\"2\" style=\"width: 5%\">序号</th><th rowspan=\"2\" style=\"width: 15%\">项目</th>";
        strhtml += "<th colspan=\"4\" style=\"width: 25%\">" + month + "月(个)</th><th colspan=\"4\" style=\"width: 25%\">" + month + "月累计(个)</th></tr>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
        strhtml += "     <th class=\"th_Sub\">北区</th><th class=\"th_Sub\">中区</th>";
        strhtml += "     <th class=\"th_Sub\">南区</th><th class=\"th_Sub\">合计</th>";
    }
    else {
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

//区分下载报表
function DownExcelReport() {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var FinMonth = $("#ddlMonth").val();
    var latest = false;
    if ($("#chkIsLastestVersion").attr("checked") == "checked") {
        latest = true;
    }
    var url = "../AjaxHander/DownLoadDSExcel.aspx?ActionType=TargetSum&Param=" + latest + "&FinYear=" + FinYear + "&FinMonth=" + FinMonth;
    window.open(url);
}