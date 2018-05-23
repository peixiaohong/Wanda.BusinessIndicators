
var Year;
var Month;
var SystemID;
var minDatatime;
var lihidden;
var all
var TargetName;
var detail;

var Monthsg;//从webconfig中读取的商管ID
var Monthsgrent;//从webconfig中读取的商管租金收缴率的ID

var TargetList;
var NowMissList;//当前指标下的所有未完成
var NowTargetID;//当前所选指标ID;
var ID;

function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/CompanyTmpl.html", selector);
}
$(document).ready(
           function () {
               Monthsg = $("#sg").val();
               Monthsgrent = $("#sgrent").val();
               Monthsg = Monthsg.toLowerCase();
               Monthsgrent = Monthsgrent.toLowerCase();
               $("#redtext").hide();

               $("#monthshow").show();
               $("#dateshow").hide();
               BangList();
           });
function BangList() {
    SystemID = $("#ddlSystem").val();
    Year = $("#ddlYear").val();
    Month = $("#ddlMonth").val();
    if (SystemID != Monthsg) {
        $("#SGselect").hide();
        $("#Yearselect").show();
        $("#datetime").hide();
        $("#YearInt").show();
    } else {
        $("#Yearselect").hide();
    }
    GetTab();
    if (TargetList.length > 0) {
        ChangeTab(TargetList[0].ID);
    }

}

function GetTab() {
    TargetList = "";
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetTargetListNew",
        args: { SystemID: SystemID, Year: Year, Month: Month },
        successReturn: function (result) {
            TargetList = result;
            if (TargetList.length > 0) {
                BangTab(result);
            }
        }
    });
}

/// 加载指标tab
function BangTab(result) {
    $("#tabs").html("");

    //loadTmpl('#TargetReportTab').tmpl(TargetList).appendTo('#tabs');

    var tab = "";

    for (var i = 0; i < TargetList.length; i++) {
        tab += " <li class=\"sd\">";
        tab += "  <a class=\"active3\" id=\"Ta" + TargetList[i].ID + "\" onclick=\"ChangeTab('" + TargetList[i].ID + "')\">" + TargetList[i].TargetName + "</a>";
        tab += "</li>";
    }
    $("#tabs").html(tab);
}



function ChangeTab(result) {
    NowTargetID = result;

    ///控制tab页样式
    for (var i = 0; i < TargetList.length; i++) {
        if (result == TargetList[i].ID) {
         
            document.getElementById('Ta' + TargetList[i].ID + '').className = "active3 active_sub3";

            //$("#Ta" + TargetList[i].ID + "").addClass("selected");
            $("#target").html(TargetList[i].TargetName);
        }
        else {
            document.getElementById('Ta' + TargetList[i].ID + '').className = "active3";
            //$("#Ta" + TargetList[i].ID + "").attr("class", "active3");
        }
    }


    //加载数据
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetVMissDetailByTargetID",
        args: { SystemID: SystemID, TargetID: result, Year: Year, Month: Month },
        successReturn: function (result) {
            NowMissList = result;
            //loadTmpl('#TargetReportBody').tmpl(result).appendTo('#TData1');//客户版本不支持tmpl
            AddView(result);
            //var obj = $("#TbH1");
            //var tab = $("#TData1");
            //FloatHeader(obj, tab);
            
        }
    });

}

function AddView(result) {
    $("#TData1").html("");
    var detail = "";
    for (var i = 0; i < result.length; i++) {
        detail += "<tr class=\"list_tr\">";
        detail += "<td class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">" + result[i].TargetType + "</td>";
        detail += " <td class=\"Td_Left\">";
        detail += "<a style=\"text-decoration: underline; color: blue; font-size: 13px; cursor: pointer\"onclick=\"change('" + result[i].ID + "')\">";
        detail += "" + result[i].CompanyName + "</a></td>";
        detail += " <td class=\"Td_Left\">";
        if (result[i].PromissDate == null || result[i].PromissDate == "") {

            //if (result[i].ReturnType==1) {
            //    detail +=  FormatTime(result[i].CurrentMonthCommitDate) + "</td>";
            //}
            //else {
            detail += "--</td>";
            //}

        }
        else {
            detail += "" + FormatTime(result[i].PromissDate) + "</td>";
        }
        detail += " <td class=\"Td_Left\">";
        if (result[i].CurrentMonthCommitDate == null || result[i].CurrentMonthCommitDate == "") {
            detail += "--</td>";
        }
        else {
            detail += "" + FormatTime(result[i].CurrentMonthCommitDate) + "</td>";
        }
        detail += "<td class=\"Td_Left\" style=\"text-align: left !important\">" + result[i].CurrentMonthCommitReason + "</td>";
        detail += "<td class=\"Td_Left\" style=\"text-align: center !important; vertical-align: middle\">";
        detail += "<img class=\"none" + result[i].Counter + "\" src=\"../Images/images1/image" + result[i].Counter + ".png\" /></td>";
        detail += "</tr>";

    }
    $("#TData1").html(detail);
    var obj = $("#Tab_FloatReturn");
    var head = $("#TbH1");
    obj.find("thead").html(head.html());
    var tab = $("#TData1");
    FloatHeader(obj, tab);
}
var arrresult = [];
var Companyid = "";
var PlanAmmout = "";
var ActualAmmout = "";
var AnnualTarget = "";
var TargetPlanList;
function change(CompanyID) {
    var arr = [];
    var targetlist = [];
    for (var i = 0; i < NowMissList.length; i++) {
        if (NowMissList[i].ID == CompanyID) {
            arr.push(NowMissList[i]);
        }
    }
    var dedate = new Date();
    var year = dedate.getFullYear();
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetTargetPlan",
        args: { TargetPlanID: arr[0].TargetPlanID, FinYear: year },
        successReturn: function (TargetList) {
            TargetPlanList = TargetList;
        }
    });
    for (var i = 0; i < TargetPlanList.length; i++) {
        if (TargetPlanList[i].CompanyID == arr[0].CompanyID && TargetPlanList[i].TargetID == arr[0].TargetID) {
            targetlist.push(TargetPlanList[i]);
        }
    }
    $("#name").html(arr[0].CompanyName);
    if (arr[0].PromissDate != null) {
        $("#promissd").html(FormatTime(arr[0].PromissDate) + "月份");
    }
    else {
        $("#promissd").html("---");
    }

    if (arr[0].CurrentMonthCommitDate != null) {
        $("#rpt_info_CommitMonth").val(FormatTime(arr[0].CurrentMonthCommitDate));
        $("#rpt_info_CommitDate").val(FormatDate(arr[0].CurrentMonthCommitDate));
    } else if ($("#rpt_info_CommitMonth").val() !=null) {
        $("#rpt_info_CommitMonth").val("");
    } else if ($("#rpt_info_CommitDate").val() != null) {
        $("#rpt_info_CommitDate").val();
    }

    var dates = FormatDate(arr[0].CurrentMonthCommitDate);//取时间
    if (dates != "---" && dates != "" && dates != null) {
        var Isdate = dates.split("-");
        var IsMonthDay = Isdate[1] +"-"+Isdate[2];
    }
    
    if (SystemID != Monthsg) {
        if (IsMonthDay == "12-31") {
            $("#radio5").attr("checked", "true");
            $("#radio4").removeAttr("checked");
            $("input[type='radio']:checked").val();
            $("#datetime").hide();
            $("#YearInt").show();
            //$("#rpt_info_CommitReason").attr("readonly", "readonly");
            $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
            $("#rpt_info_CommitReason").html(arr[0].CurrentMonthCommitReason);
        } else {
            $("#radio4").attr("checked", "true");
            $("#radio5").removeAttr("checked");
            $("input[type='radio']:checked").val();
            $("#datetime").show();
            $("#YearInt").hide();
            //$("#rpt_info_CommitReason").attr("readonly", "readonly");
            $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
            $("#rpt_info_CommitReason").val("");
            $("#rpt_info_CommitReason").html("");
        }
    } else {
        if (IsMonthDay == "12-31") {
            $("#radio3").attr("checked", "true");
            $("#rpt_info_CommitReason").html(arr[0].CurrentMonthCommitReason);
            Yaershow();
        } else {
            if (arr[0].IsMissTarget == false) {
                $("#radio2").attr("checked", "true");
                DateShow();
            }
            else {
                $("#radio1").attr("checked", "true");
                
                MonthShow();
            }
        }
    }
    
    ID = arr[0].ID;
    Companyid = ID;
    AnnualTarget = Math.round(targetlist[0].Target);
    PlanAmmout = arr[0].NAccumulativePlanAmmount;
    ActualAmmout = arr[0].NAccumulativeActualAmmount;
    art.dialog({
        content: $("#divMonthReportDetail").html(),
        lock: true,
        id: 'divMonthReportDetail',
        title: '<span>要求时间--编辑</span>'
    });
}
function Save() {
    //选择是按月保存还是按日保存
    var SelectDate = $("input[name='time']:checked").val();
    if (SelectDate == "月") {
        SaveMonth();
    }
    else if (SelectDate == "日") {
        SaveDate();
    } else if (SelectDate == "补回") {
        if (SystemID != Monthsg) {
            SaveAmmout();
        } else {
            SaveAmmout_SG();
        }
    }
    ChangeTab(NowTargetID);
}
function SaveAmmout() {
    GetMonthly();
    if (detail != null) {
        var Number = $("#rpt_info_CommitInt").val();//判断是否为数字。
        //解决在IE8及以下.trim()方法不支持的问题。
        if (typeof String.prototype.trim !== 'function') {
            String.prototype.trim = function () {
                return this.replace(/^\s+|\s+$/g, '');
            }
        }
        if (Number.trim() == "" || $v.isNumber(Number) == false ) {
            $("#tip").html("必须输入数字，且不能有空格!");
            return false;
        }
        var dedate = new Date();
        var year = dedate.getFullYear();
        var month = 12;
        var day = 31;
        var date = year + "-" + month + "-" + day;
        var CommitReasonInfo = $("#rpt_info_CommitReason").val();//取填写的信息
        detail.CurrentMonthCommitReason = jsTrim(CommitReasonInfo);
        detail.CurrentMonthCommitDate = date;
        WebUtil.ajax({
            async: false,
            url: "/MonthlyReportController/UpdateTargetDetail",
            args: { info: WebUtil.jsonToString(detail)},
            successReturn: function (result) {
                if (result==0) {
                    //alert("保存成功！");
                    art.dialog({ id: 'divMonthReportDetail' }).close();
                  
                }
                else {
                    alert("保存失败，请重试！");
                }
            
            }
        });

    }
    else {
        alert("请填写实际金额");
    }

}

function SaveAmmout_SG() {
    GetMonthly();
    if (detail != null) {
        var Number = $("#rpt_info_CommitInt").val();//判断是否为数字。
        //解决在IE8及以下.trim()方法不支持的问题。
        if (typeof String.prototype.trim !== 'function') {
            String.prototype.trim = function () {
                return this.replace(/^\s+|\s+$/g, '');
            }
        }
        if (Number.trim() == "" || $v.isNumber(Number) == false ) {
            $("#tip").html("必须输入数字，且不能有空格!");
            return false;
        }
        var dedate = new Date();
        var year = dedate.getFullYear();
        var month = 12;
        var day = 31;
        var date = year + "-" + month + "-" + day;
        var CommitReasonInfo = $("#rpt_info_CommitReason").val();//取填写的信息
        detail.CurrentMonthCommitReason = jsTrim(CommitReasonInfo);
        detail.CurrentMonthCommitDate = date;
        WebUtil.ajax({
            async: false,
            url: "/MonthlyReportController/UpdateTargetReturn",
            args: { info: WebUtil.jsonToString(detail), isday: "false" },
            successReturn: function (result) {
                if (result == 0) {
                    //alert("保存成功！");
                    art.dialog({ id: 'divMonthReportDetail' }).close();

                }
                else {
                    alert("保存失败，请重试！");
                }

            }
        });

    }
    else {
        alert("请填写实际金额");
    }

}

function SaveDate() {
    GetMonthly();
    if (detail != null) {
        var dedate = $("#rpt_info_CommitDate").val();//取所选择的时间
        var CommitReasonInfo = $("#rpt_info_CommitReason").val();//取填写的信息
        if (dedate != "---" && dedate != "" && dedate != null) {
            var dearr = dedate.split("-");
            detail.CurrentMonthCommitReason = jsTrim(CommitReasonInfo);
            detail.CurrentMonthCommitDate = dedate;
            var UpdateCommitDate = dearr[0] + "-" + dearr[1];
            if (Month < 10) {
                minDatatime = Year + "-0" + Month;
            }
            else {
                minDatatime = Year + "-" + Month;
            }

            if (minDatatime == UpdateCommitDate) {
                detail.IsCommitDate = 1;
            }
            else {
                detail.IsCommitDate = 0;
            }
            detail.IsMissTarget = false;
            WebUtil.ajax({
                async: false,
                url: "/MonthlyReportController/UpdateTargetReturn",
                args: { info: WebUtil.jsonToString(detail), isday: "true" },
                successReturn: function (result) {
                    if (result == 0) {
                        //alert("保存成功！");
                        art.dialog({ id: 'divMonthReportDetail' }).close();

                    }
                    else {
                        alert("保存失败，请重试！");
                    }
                }
            });

        }
        else {
            alert("请选择要求时间");
        }
    }


}

function GetMonthly() {
    WebUtil.ajax({
        async: false,
        url: "/MonthlyReportController/GetMonthLyRModel",
        args: { ID: ID },
        successReturn: function (result) {
            detail = result;
        }
    });
}

function SaveMonth() {
    GetMonthly();
    if (detail != null) {
        var dedate = $("#rpt_info_CommitMonth").val();//取所选择的时间
        var CommitReasonInfo = $("#rpt_info_CommitReason").html();//取填写的信息
        if (dedate != "---" && dedate != "" && dedate != null) {
            var dearr = dedate.split("-");
            var daterr;
            if (dearr[1] == 01 || dearr[1] == 03 || dearr[1] == 05 || dearr[1] == 07 || dearr[1] == 08 || dearr[1] == 10 || dearr[1] == 12) {
                if (dearr[1] == 12) {
                    daterr = dearr[0] + "-" + dearr[1] + "-30 0:00:00";
                } else {
                    daterr = dearr[0] + "-" + dearr[1] + "-31 0:00:00";
                }
                
            }
            else if (dearr[1] == 04 || dearr[1] == 06 || dearr[1] == 09 || dearr[1] == 11) {
                daterr = dearr[0] + "-" + dearr[1] + "-30 0:00:00";
            }
            else if (dearr[1] == 02) {
                if (dearr[0] % 4 == 0) {
                    daterr = dearr[0] + "-" + dearr[1] + "-29 0:00:00";
                }
                else {
                    daterr = dearr[0] + "-" + dearr[1] + "-28 0:00:00";
                }
            }//将选取的时间拼完整  拼接成:yyyy-MM-DD 0:00:00格式

            detail.CurrentMonthCommitReason = jsTrim(CommitReasonInfo);
            detail.CurrentMonthCommitDate = daterr;
            var UpdateCommitDate = dearr[0] + "-" + dearr[1];
            if (Month < 10) {
                minDatatime = Year + "-0" + Month;
            }
            else {
                minDatatime = Year + "-" + Month;
            }

            if (minDatatime == UpdateCommitDate) {
                detail.IsCommitDate = 1;
            }
            else {
                detail.IsCommitDate = 0;
            }
            detail.IsMissTarget = true;
            WebUtil.ajax({
                async: false,
                url: "/MonthlyReportController/UpdateTargetReturn",
                args: { info: WebUtil.jsonToString(detail), isday: "false" },
                successReturn: function (result) {
                    if (result == 0) {
                        //alert("保存成功！");
                        art.dialog({ id: 'divMonthReportDetail' }).close();

                    }
                    else {
                        alert("保存失败，请重试！");
                    }
                }
            });
            art.dialog({ id: 'divMonthReportDetail' }).close();
        }
        else {
            alert("请选择要求时间");
        }
    }


}


function DownExcel() {
    window.open("/AjaxHander/DownExcelCommitDate.ashx?SysId=" + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month);
}
function jsTrim(str) {
    return str.replace(/\ /g, "");
}

$(function () {
    var error = 0;
    $('#file1').uploadify({
        'buttonText': '导入数据',
        'width': 80,
        'height': 25,
        'fileTypeDesc': 'office file',
        'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx;',
        'fileSizeLimit': '10240',
        //swf文件路径
        'swf': '../Scripts/UpLoad/uploadify.swf',
        //后台处理页面
        'uploader': '/AjaxHander/UpLoadExcelCommitDate.ashx?SysId=' + SystemID + "&FinYear=" + Year + "&FinMonth=" + Month,
        'onUploadSuccess': function (file, data, response) {
            error = data;
            if (data == "" || data == null) {
                document.getElementById("ContentPlaceHolder1_LinkButton1").click();
                alert("保存成功");
            } else {
                alert(data);
            }
        },
        'onUploadError': function (file, data, response) {
            alert("上传失败，程序出错！");
        }
    })
})

function MonthShow() {
    if (SystemID != Monthsg) {
        $("#radio5").removeAttr("checked");
        $("#radio4").attr("checked", "true");
        $("input[type='radio']:checked").val();
        $("#YearInt").hide();
        $("#datetime").show();
        //$("#rpt_info_CommitReason").attr("readonly", "readonly");
        $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
        $("#rpt_info_CommitReason").val("");
    } else {
        $("#radio1").attr("checked", "true");
        $("input[type='radio']:checked").val();
        $("#dateshow").hide();
        $("#YearInt").hide();
        $("#datetime").show();
        $("#monthshow").show();
        $("#redtext").hide();
        //$("#rpt_info_CommitReason").attr("readonly", "readonly");
        $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
        $("#rpt_info_CommitReason").val("");
    }
}
function DateShow() {
    $("#radio2").attr("checked", "true");
    $("input[type='radio']:checked").val();
    $("#monthshow").hide();
    $("#YearInt").hide();
    $("#datetime").show();
    $("#dateshow").show();
    $("#redtext").show();
   // $("#rpt_info_CommitReason").attr("readonly", "readonly");
    $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
    $("#rpt_info_CommitReason").val("");
}
function Yaershow() {
    if (SystemID != Monthsg) {
        $("#radio4").removeAttr("checked");
        $("#radio5").attr("checked", "true");
        $("input[type='radio']:checked").val();
        $("#datetime").hide();
        $("#YearInt").show();
        //$("#rpt_info_CommitReason").attr("readonly", "readonly");
        $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
        $("#rpt_info_CommitReason").val("预计年内无法补回必保全年完成【】万(指标" + AnnualTarget + "万，差额【】万)");
    } else {
        $("#radio3").attr("checked", "true");
        $("input[type='radio']:checked").val();
        $("#monthshow").hide();
        $("#dateshow").hide();
        $("#datetime").hide();
        $("#redtext").hide();
        $("#YearInt").show();
        //$("#rpt_info_CommitReason").attr("readonly", "readonly");
        $("#rpt_info_CommitReason").css({ "backgroundColor": "#ececec" });
        $("#rpt_info_CommitReason").val("预计年内无法补回必保全年完成【】万(指标" + AnnualTarget + "万，差额【】万)");

    }
}

function Returned() {
    var $txt = $("#rpt_info_CommitInt").val();
   // 解决在IE8及以下.trim()方法不支持的问题。
    if (typeof String.prototype.trim !== 'function') {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, '');
        }
    }
    
    if ($txt.trim() == "" || $v.isNumber($("#rpt_info_CommitInt").val()) == false) {
        $("#tip").html("必须输入数字，且不能有空格!");
    } else {
        if ($txt) {
            $("#rpt_info_CommitReason").val("预计年内无法补回必保全年完成" + $txt + "万(指标" + AnnualTarget + "万，差额" + ($txt - AnnualTarget) + "万)");
        } else {
            $("#rpt_info_CommitReason").html();
        }
        $("#tip").html("");
    }
}

function FormatTime(values) {
    if (values != null && values != "0001/1/1 0:00:00") {
        var Y = new Date(values).getFullYear();
        var X = new Date(values).getMonth() + 1;
        if (X < 10) {
            return Y + "-0" + X;
        }
        else {
            return Y + "-" + X;
        }
    }
    else {
        return "--";
    }
}

function FormatDate(values) {
    var day = new Date(values);
    var Year = 0;
    var Month = 0;
    var Day = 0;
    var CurrentDate = "";
    //初始化时间
    //Year= day.getYear();//有火狐下2008年显示108的bug
    Year = day.getFullYear();//ie火狐下都可以
    Month = day.getMonth() + 1;
    Day = day.getDate();
    //Hour = day.getHours();
    // Minute = day.getMinutes();
    // Second = day.getSeconds();
    CurrentDate += Year + "-";
    if (Month >= 10) {
        CurrentDate += Month + "-";
    }
    else {
        CurrentDate += "0" + Month + "-";
    }
    if (Day >= 10) {
        CurrentDate += Day;
    }
    else {
        CurrentDate += "0" + Day;
    }
    return CurrentDate;
}