var SystemID;
var FinYear;
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
      
      
        GetTargetList();
      
       
    });


function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/TargetConfiguration.html", selector);
}
function GetTargetList() {
    Load();
    FinYear = $("#ddlYear").val();
    SystemID = $("#ddlSystem").val();
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetTargetHistory",
        args: { SystemID: SystemID, Year: FinYear },
        successReturn: function (result) {
            TargetList = result;
            BangTable();
        }
    });
    Fake();
}

function BangTable() {
    $("#rows").html("");
    if (TargetList.length>0) {
        loadTmpl('#TargetPlanHistory').tmpl(TargetList).appendTo('#rows');
    }
}
function DownLoad(TargetPlanID) {
    window.open("/AjaxHander/DownExcelTargetHistory.ashx?SysId=" + SystemID + "&FinYear=" + FinYear + "&TargetPlanID=" + TargetPlanID);
}
function FormatTime(value) {
    if (value != null) {
        var Time = new Date(value).toDateString();
        //var NewDate = Time.getYear() + "-" + Time.getMonth() + "-" + Time.getDay();
        return Time;
    }
    else {
        return "--";
    }
}

