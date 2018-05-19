var SystemID;
var TargetList;
var ExList;
var TargetID;
var ComeFrom;

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
      
        if ($("#SysID").val() != "") {
            $("#ddlSystem").val($("#SysID").val());
        }
        ComeFrom = $("#ComeFrom").val();
        reload()


        var obj = $("#FloatTable2");
        var head = $("#head");
        obj.find("thead").html(head.html());
        var tab = $("#Tbody1");
        FloatHeader(obj, tab);
        //SystemID = "15EE2C18-5C7A-402E-92B8-277CFF78E210";
        var pathname = "/SystemConfiguration/ExceptionTarget.aspx";
        if (location.pathname == pathname) {
            $("#sitmap").html('您当前所在的位置：系统管理<img src="../images/btn08.png">指标信息基本管理<img src="../images/btn08.png">指标管理');
            $("#jMenu").find("li").each(function () {
                var text = $(this).find("span")[0];
                $(this).removeClass("current first");
                if (text && text.innerHTML == "系统管理") {
                    $(this).addClass("current first");
                }
            })
        }

    });
function reload() {
    Load();
    SystemID = $("#ddlSystem").val();
 
    BangList();
    BangTab();
    Change(TargetList[0].ID);
   
    Fake();
}

function BangTab() {
    $("#tabs").html("");
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

}
function BangList() {

    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetExceptionTarget",
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
    var tr3 = "";


    for (var i = 0; i < ExList.length; i++) {
        var a = 1;
        for (var j = 0; j < ExList[i].ExcepListA.length; j++) {
            tr1 += "<tr class=\"A" + ExList[i].TargetID + " " + ExList[i].TargetID + " S\" style=\"display:none\">  <td class=\"Td_Center\">";
            tr1 += " <input type=\"checkbox\" name=\"check1\" value=\"A\"  ID=\"" + ExList[i].ExcepListA[j].ExceptionTargetID + "\" /></td>";
            tr1 += "   <td class=\"Td_Center\">" + a + "</td>";
            tr1 += "   <td class=\"Td_Right\">" + ExList[i].ExcepListA[j].CompanyName + "</td>";
            tr1 += "  <td class=\"Td_Right\"> <select style=\"width: 150px\" onchange=\"SelectChange('1',this.value,'" + ExList[i].ExcepListA[j].ExceptionTargetID + "')\">";
            tr1 += "  <option selected=\"selected\" value=\"1\">上报不考核</option>";
            tr1 += " <option  value=\"2\">不上报不考核</option> <option   value=\"3\">上报考核</option> </select></td>";
            tr1 += "  <td class=\"Td_Right\">" + ExList[i].ExcepListA[j].ModifierName + "</td>";
            tr1 += "<td class=\"Td_Right\">" + FormatTime(ExList[i].ExcepListA[j].ModifyTime) + "</td></tr>";
            a++;
        }
        var b = 1;
        for (var j = 0; j < ExList[i].ExcepListB.length; j++) {
            tr2 += "<tr class=\"B" + ExList[i].TargetID + " " + ExList[i].TargetID + " S\" style=\"display:none\">  <td class=\"Td_Center\">";
            tr2 += " <input type=\"checkbox\" name=\"check1\" value=\"B\" ID=\"" + ExList[i].ExcepListB[j].ExceptionTargetID + "\" /></td>";
            tr2 += "   <td class=\"Td_Center\">" + b + "</td>";

            tr2 += "   <td class=\"Td_Right\">" + ExList[i].ExcepListB[j].CompanyName + "</td>";
            tr2 += "  <td class=\"Td_Right\"> <select style=\"width: 150px\" onchange=\"SelectChange('2',this.value,'" + ExList[i].ExcepListB[j].ExceptionTargetID + "')\">";
            tr2 += "  <option  value=\"1\">上报不考核</option>";
            tr2 += " <option selected=\"selected\" value=\"2\">不上报不考核</option> <option   value=\"3\">上报考核</option> </select></td>";
            tr2 += "  <td class=\"Td_Right\">" + ExList[i].ExcepListB[j].ModifierName + "</td>";
            tr2 += "<td class=\"Td_Right\">" + FormatTime(ExList[i].ExcepListB[j].ModifyTime) + "</tr>";
            b++;
        }
        var c = 1;
        for (var j = 0; j < ExList[i].ExcepListC.length; j++) {
            tr3 += "<tr class=\"C" + ExList[i].TargetID + " " + ExList[i].TargetID + " S\" style=\"display:none\" >  <td class=\"Td_Center\">";
            tr3 += " <input type=\"checkbox\" name=\"check1\"value=\"C\" ID=\"" + ExList[i].ExcepListC[j].CompanyID + "\" /></td>";
            tr3 += "   <td class=\"Td_Center\">" + c + "</td>";

            tr3 += "   <td class=\"Td_Right\">" + ExList[i].ExcepListC[j].CompanyName + "</td>";
            tr3 += "  <td class=\"Td_Right\"> <select style=\"width: 150px\" onchange=\"SelectChange('3',this.value,'" + ExList[i].ExcepListC[j].CompanyID + "')\">";
            tr3 += "  <option  value=\"1\">上报不考核</option>";
            tr3 += " <option  value=\"2\">不上报不考核</option> <option selected=\"selected\"  value=\"3\">上报考核</option> </select></td>";
            tr3 += "  <td class=\"Td_Right\">" + ExList[i].ExcepListC[j].ModifierName + "</td>";
            tr3 += "<td class=\"Td_Right\">" + FormatTime(ExList[i].ExcepListC[j].ModifyTime) + "</tr>";
            c++;
        }
    }
    $("#Tab1").html(tr1);
    $("#Tab2").html(tr2);
    $("#Tab3").html(tr3);
}
function TabChange(select) {
    Load();
    $("." + select + "" + TargetID + "").toggle();
    Fake();
}

function SelectChange(oldtype, newtype, id) {
    var oldtype = oldtype;
    var newtype = newtype;
    var id = id;
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/ChangeExceptionTarget",
        args: { OldType: oldtype, NewType: newtype, ID: id, TargetID: TargetID },
        successReturn: function (result) {
            BangList();
            Change(TargetID);
            if (oldtype=='1') {
                $(".A" + TargetID + "").show();
            }
            else if (oldtype == '2') {
                $(".B" + TargetID + "").show();
            }
            else {
                $(".C" + TargetID + "").show();
            }
           
        }
    });
}

var CheckValue;
var NoRepeatValue;
var ChangeCompanyList;

function ReplaceCompany() {
    ChangeCompanyList = new Array();
    NoRepeatValue = "";
    CheckValue = new Array();
    $("input[name='check1']:checked").each(function () {
        CheckValue.push($(this).val());
        var s = $(this).attr("ID")
        ChangeCompanyList.push($(this).attr("id"));
    });
    NoRepeatValue = uniqueArray(CheckValue);
    if (NoRepeatValue.length==0) {
        alert("请勾选公司!");
    }
    else if (NoRepeatValue.length>1) {
        alert("非同类型公司不能同时转换");
    }
    else {

        art.dialog({
            content: $("#divDetail").html(),
            lock: true,
            id: 'divDetail',
            title: '<span>替换</span>'
        });
    }
} 
function SaveReplace() {
    var SelectValue = $("#ChangSelect").val();
    var IDlist = "";
    for (var i = 0; i < ChangeCompanyList.length; i++) {
        if (i == ChangeCompanyList.length-1) {
            IDlist += ChangeCompanyList[i] ;
        }
        else {
            IDlist += ChangeCompanyList[i] + '|';
        }
      
    }
   if (SelectValue=='0') {
       alert('请选择!');
   }
   else if (SelectValue == NoRepeatValue[0]) {
       alert('请选择不同的类别');
   }
   else {
       WebUtil.ajax({
           async: false,
           url: "/CompanyController/SaveExceptionReplace",
           args: { IDlist: IDlist, Type: NoRepeatValue[0], SelectType: SelectValue, TargetID: TargetID },
           successReturn: function (result) {
               if (result.length == ChangeCompanyList.length) {
                   alert('替换成功!');
                   BangList();
                   Change(TargetID);
                   if (NoRepeatValue[0] == 'A') {
                       $(".A" + TargetID + "").show();
                   }
                   else if (NoRepeatValue[0] == 'B') {
                       $(".B" + TargetID + "").show();
                   }
                   else {
                       $(".C" + TargetID + "").show();
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
function ReturnSystem() {
    location.href = "../SystemConfiguration/" + ComeFrom + ".aspx?ID=" + SystemID + ""
}
function FormatTime(values) {
    if (values != null && values != "0001/1/1 0:00:00") {
        var Time = new Date(values).toDateString("yyyy-MM-dd");
        return Time;
    }
    else {
        return "--";
    }
}