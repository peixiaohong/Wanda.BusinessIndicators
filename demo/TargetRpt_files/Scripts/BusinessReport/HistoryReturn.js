var _SysID;
var  FinYear;


//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../StatisticReport/HistoryReturnTmpl.html", selector);
}

//页面初始化
$(function () {

    _SysID = $("#ddlSystem").val();
    FinYear = $("#ddlYear").val();

    GetDataSoure();
    var obj = $("#HistoryReturnRptHead");
    var tab = $("#HistoryReturnRptBody");
    FloatHeader(obj, tab);

    
})


function GetDataSoure()
{
    WebUtil.ajax({
        async: true,
        url: "/HistoryReturnControll/GetHistoryReturnList",
        args: { SystemID: _SysID, Year: FinYear },
        successReturn: GetHistoryReturn
    });
}

function GetHistoryReturn(result)
{
    var ObjValue = {};
    ObjValue.RData = result;

    $("#HistoryReturnRptBody").empty();
    loadTmpl('#HistoryReturnInfo').tmpl(ObjValue).appendTo('#HistoryReturnRptBody'); //加载信息

}


function SearchData()
{
    _SysID = $("#ddlSystem").val();
    FinYear = $("#ddlYear").val();

    GetDataSoure();
}









//格式化日期类型
function FormatDate(obj, displayTime, local) {
    try {
        var date = new Date(obj);
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var hour = date.getHours();
        var minute = date.getMinutes();
        var second = date.getSeconds();

        if (displayTime) {

            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                return "---";
            } else {
                if (local == "CN") {
                    return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                } else {
                    return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                }
            }
        } else {
            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                return "---";
            } else {
                if (local == "CN") { //如果是CN的显示中文年月
                    return "{1}月".formatBy(year, month, doubleDigit(day));
                } else {
                    return "{0}-{1}-{2}".formatBy(year, doubleDigit(month), doubleDigit(day));
                }
            }
        }

    } catch (e) {
        return "";
    }

    function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }

}



//区分下载报表
function DownExcelReport(sender) {
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();
    var send = $(sender).text();

    if ($.trim(send) == "导出历史要求期限统计汇总报表") {
        window.open("/AjaxHander/DownExcelTemplete.ashx?FileType=HistoryReturn&SysId=" + SysId + "&FinYear=" + FinYear);
    } 
}
