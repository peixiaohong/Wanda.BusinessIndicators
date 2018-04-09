var ReportTime;
var NowTime;
var UpdateTime;
var NewDay;
var SysOpendObj;


$(document).ready(function () {

    GetTime();


    WebUtil.ajax({
        async: false,
        url: "/TimeConfiguration/GetReportTime",
        successReturn: function (result) {
            SysOpendObj = result;
            ReportTime = result.ReportTime;
        }
    });

  

    if (SysOpendObj.OpenStatus == "1")
    {
        $("#OpenDayRadio").attr("checked", "checked");
        $("#OpenDayTxt").val(FormatDate(SysOpendObj.SysOpenDay, false));
        $("#ReportDay").val(FormatDate(SysOpendObj.WantTime, false).substring(0, FormatDate(SysOpendObj.WantTime, false).length - 3));

    }else if(SysOpendObj.OpenStatus == "2")
    {
        $("#opentadio").attr("checked", "checked");

        $("#OpenDate").val(FormatDate(SysOpendObj.ReportTime, false).substring(0, FormatDate(SysOpendObj.ReportTime, false).length-3));

    }else
    {
        $("#closeradio").attr("checked", "checked");
    }

    var obj = $("#"+$("input[name='btu']:checked").attr("id"));

    ClickChange(obj)

});
//获取当前月的上个月
function GetTime() {
    var Time = new Date();
    var vYear = Time.getFullYear();
    var vMon = Time.getMonth() +1;
    var day = Time.getDate();
    var clock = day;

    if (vMon == 0) {
        vYear--;
        vMon = 12;
    }

    if (day < 10)
        clock = "0" + day;

    NowTime = vYear + "-" + (vMon < 10 ? "0" + vMon : vMon);

    NewDay = vYear + "-" + (vMon < 10 ? "0" + vMon : vMon) + "-" + (day < 10 ? clock : day);
}

function Save() {

    var value = $("input[name='btu']:checked").val();

    //立即开放的日期
    var txtDate = $("#OpenDate").val();



    //预计某日开放的日期
    var txtDay = $("#OpenDayTxt").val();
    var rptDay = $("#ReportDay").val();

    var SysOpenDay;

    if (value == 1) //预计到某日开放
    {
        if (txtDay != "" && rptDay !="")
        {
            SysOpenDay = txtDay;
            UpdateTime = rptDay + "-01";
        } else {
            alert("请先选择日期!");
            return;
        }

    }else if(value ==2) //立即开放，这个有尼玛蛋子用？？？？？
    {
        if (txtDate != "") {
            UpdateTime = $("#OpenDate").val() + '-01';
        } else {
            alert("请先选择月份!");
            return;
        }

    }else{ //关闭

        UpdateTime = "";
    }



    WebUtil.ajax({
        async: false,
        url: "/TimeConfiguration/UpdateReportTime",
        args: { time: UpdateTime, status: value, openday: SysOpenDay },
        successReturn: function (result) {
            alert("保存成功!");
        }
    });

}


function ClickChange( obj ) {

    var RadioId = $(obj).attr("id");

    if (RadioId == "OpenDayRadio")
    {
        $("#OpenDayTxt,#ReportDay").show();
        $("#OpenDate,#closeTips2,#closeTips3").hide();

    } else if (RadioId == "opentadio") {
        
        $("#OpenDate,#closeTips2").show();
        $("#OpenDayTxt,#closeTips3").hide();
    } else {
        $("#OpenDate,#closeTips2").hide();
        $("#closeTips3").show();
    }

    //$("#OpenDate").val(NowTime);
    //$("#OpenDayTxt").val(NewDay);


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
                    return "{1}月{2}".formatBy(year, month, doubleDigit(day));
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