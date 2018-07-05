var SystemID;
var TargetList;
var TargetID;

function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/TargetCollectDisplayTmpl.html", selector);
}
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
        Load();
        SystemID = $("#DSystemID").val();
        //SystemID = "15EE2C18-5C7A-402E-92B8-277CFF78E210";
        BangTab();
        BangList();
        Change(TargetList[0].ID);
        var obj = $("#FloateTable2");
        var head = $("#Tabhead");
        obj.find("thead").html(head.html());
        var tab = $("#Tab2");
        FloatHeaderWidth(obj, head);
        FloatHeader(obj, tab);

        var pathname = "/SystemConfiguration/IfContrastCompany.aspx";
        if (location.pathname == pathname) {
            $("#sitmap").html('您当前所在的位置：系统管理<img src="../images/btn08.png">公司属性管理<img src="../images/btn08.png">可比属性管理');
            $("#jMenu").find("li").each(function () {
                var text = $(this).find("span")[0];
                $(this).removeClass("current first");
                if (text && text.innerHTML == "系统管理") {
                    $(this).addClass("current first");
                }
            })
        }

        Fake();
    });

function BangTab() {
    $("#tabs").val("");
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetTargetList",
        args: { SysID: SystemID },
        successReturn: function (result) {
            TargetList = result;
            loadTmpl('#ExceptionTarget').tmpl(TargetList).appendTo('#tabs');

        }
    });
}

function Change(odj) {
    for (var i = 0; i < TargetList.length; i++) {
        $("#tab" + TargetList[i].ID + "").attr("class", "active3");
    }
    $("#tab" + odj + "").attr("class", "active3 active_sub3");
    TargetID = odj;
    $(".S").hide();
    TabChange("B");
    //$(".N").show();
}

function BangList() {

    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetExceptionReplace",
        args: { SystemID: SystemID },
        successReturn: function (result) {
            ExList = result;
            BangTable();
        }
    });
}

function BangTable() {
    var tr1 = ""
    var tr2 = ""
    for (var i = 0; i < ExList.length; i++) {
        for (var j = 0; j < ExList[i].ExcepListA.length; j++) {
            tr1 += "<tr class=\"A" + ExList[i].TargetID + " " + ExList[i].TargetID + " S\" style=\"display:none\">  <td class=\"Td_Center\">";
            tr1 += " <input type=\"checkbox\" name=\"check1\" value=\"A\"  ID=\"" + ExList[i].ExcepListA[j].ExceptionTargetID + "\" /> </td>";
            tr1 += "   <td class=\"Td_Center\">" + (j + 1) + " </td>";
            tr1 += "   <td class=\"Td_Right\">" + ExList[i].ExcepListA[j].CompanyName + "</td>";
            tr1 += "   <td class=\"Td_Right\">" + FormatTime(ExList[i].ExcepListA[j].OpeningTime) + "</td>";
            tr1 += "   <td class=\"Td_Right\">" + ExList[i].ExcepListA[j].ModifierName + "</td>";
            tr1 += "   <td class=\"Td_Right\">" + FormatTime(ExList[i].ExcepListA[j].ModifyTime) + "</td>";
            tr1 += "    <td class=\"Td_Center\"><a onclick=\"ReplaceCompany('A','" + ExList[i].ExcepListA[j].ExceptionTargetID + "')\" href=\"#\">替换</a></td>";
        }
        for (var j = 0; j < ExList[i].ExcepListB.length; j++) {
            tr2 += "<tr class=\"B" + ExList[i].TargetID + " " + ExList[i].TargetID + " S\" style=\"display:none\" >  <td class=\"Td_Center\">";
            tr2 += " <input type=\"checkbox\" name=\"check1\"value=\"B\" ID=\"" + ExList[i].ExcepListB[j].CompanyID + "\" /> </td>";
            tr2 += "   <td class=\"Td_Center\">" + (j + 1) + " </td>";
            tr2 += "   <td class=\"Td_Right\">" + ExList[i].ExcepListB[j].CompanyName + "</td>";
            tr2 += "   <td class=\"Td_Right\">" + FormatTime(ExList[i].ExcepListB[j].OpeningTime) + "</td>";
            tr2 += "   <td class=\"Td_Right\">--</td>";
            tr2 += "   <td class=\"Td_Right\">--</td>";
            tr2 += "    <td class=\"Td_Center\"><a onclick=\"ReplaceCompany('B','" + ExList[i].ExcepListB[j].CompanyID + "')\" href=\"#\">替换</a></td>";
        }
    }
    $("#Tab1").html(tr1);
    $("#Tab2").html(tr2);
}

function ReplaceCompany(adj, ID) {
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/SaveCompareException",
        args: { SelectType: adj, ID: ID, TargetID: TargetID },
        successReturn: function (result) {
            BangList();
            Change(TargetID);
            if (adj == 'A') {
                $(".A" + TargetID + "").show();
            }
            else {
                $(".B" + TargetID + "").show();
            }
        }
    });
}
function TabChange(select) {
    Load();
    //if (select == 'A') {
    //    var obj = $("#Tabhead");
    //    var tab = $("#Tab2");
    //    FloatHeader(obj, tab, false, "Reported");
    //}
    //else {
    //    var obj = $("#Tabhead");
    //    var tab = $("#Tab1");
    //    FloatHeader(obj, tab, false, "Reported");
    //}
    $("." + select + "" + TargetID + "").toggle();
    Fake();
}
function FormatTime(value) {
    if (value != null) {
        var Time = new Date(value);
        // var NewDate = Time.getFullYear() + "-" + Time.toLocaleDateString() + "-" + Time.getDay();
        var NewDate = Time.toDateString();
        return NewDate;
    }
    else {
        return "--";
    }
}

var CheckValue;
var NoRepeatValue;
var ChangeCompanyList;
function ReplaceCompanyList() {
    ChangeCompanyList = new Array();
    NoRepeatValue = "";
    CheckValue = new Array();
    $("input[name='check1']:checked").each(function () {
        CheckValue.push($(this).val());
        var s = $(this).attr("ID")
        ChangeCompanyList.push($(this).attr("id"));
    });
    NoRepeatValue = uniqueArray(CheckValue);
    if (NoRepeatValue.length == 0) {
        alert("请勾选公司!");
    }
    else if (NoRepeatValue.length > 1) {
        alert("非同类型公司不能同时转换");
    }
    else {
        var IDlist = "";
        for (var i = 0; i < ChangeCompanyList.length; i++) {
            if (i == ChangeCompanyList.length - 1) {
                IDlist += ChangeCompanyList[i];
            }
            else {
                IDlist += ChangeCompanyList[i] + '|';
            }
        }
        WebUtil.ajax({
            async: false,
            url: "/CompanyController/SaveCompareExceptionList",
            args: { IDlist: IDlist, SelectType: NoRepeatValue[0], TargetID: TargetID },
            successReturn: function (result) {
                if (result.length == ChangeCompanyList.length) {
                    alert('替换成功!');
                    BangList();
                    Change(TargetID);
                    if (NoRepeatValue[0] == 'A') {
                        $(".A" + TargetID + "").show();
                    }
                    else {
                        $(".B" + TargetID + "").show();
                    }

                    art.dialog({ id: 'divDetail' }).close();
                }
            }
        });
    }
}
//去重复
function uniqueArray(a) {
    var hash = {},
    len = a.length,
    result = [];

    for (var i = 0; i < len; i++) {
        if (!hash[a[i]]) {
            hash[a[i]] = true;
            result.push(a[i]);
        }
    }
    return result;
}


