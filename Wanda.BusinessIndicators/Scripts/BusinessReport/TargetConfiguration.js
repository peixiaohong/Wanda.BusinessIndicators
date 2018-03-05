var SysID;//读取页面系统ID
var TargetModel;

$(document).ready(
    function () {
        if ($("#SysID").val() != "") {
            $("#ddlSystem").val($("#SysID").val());
        }

        reload();
    });

function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/TargetConfiguration.html", selector);
}

//页面加载
function reload() {

    SysID = $("#ddlSystem").val();

    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetTargetList",
        args: { SysID: SysID },
        successReturn: function (tarlist) {

            $("#rows").empty();
            for (var i = 0; i < tarlist.length; i++) {
                loadTmpl('#TargetList').tmpl(tarlist[i]).appendTo('#rows');
            }
        }
    });
}

function SetTime(time, bool) {
    var Newtime = '';
    if (time != "0001/1/1 0:00:00") {
        var date = new Date(time);
        var NewYear = date.getFullYear();
        var NewMonth = date.getMonth() + 1;
        var NewDay = date.getDate();
        Newtime = NewYear + "-" + NewMonth + "-" + NewDay;
    }
    else {
        if (bool == true) {
            Newtime = '---';
        }
        else {
            Newtime = '';
        }
    }
    return Newtime;
}

function Add() {
    $("#savetrue").hide();
    $("#addtrue").show();
    $("#Kpi").hide();
    $("#addname").val("");
    $("#Unit").val("");
    $("#seq").val("");
    $("#rpt_info_BaseLine").val("");
    BangEums();
    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>添加指标</span>'
    });
}

function BangEums(obj) {
    $("#Enums").empty();
    WebUtil.ajax({
        async: false,
        args: { adj: obj },
        url: "/CompanyController/GetTargetTypeEnum",
        successReturn: function (Enums) {
            for (var i = 0; i < Enums.length; i++) {
                loadTmpl('#TargetTypeEnums').tmpl(Enums[i]).appendTo('#Enums');
            }
        }
    });


}
function AddsTarget() {

    var TarName = $("#addname").val();
    var TargtTypeSelect = $("#Enums").val();
    var BaseLine = $("#rpt_info_BaseLine").val();
    if (BaseLine == "") {
        BaseLine = null;
    }
    var Unit = $("#Unit").val();
    if (Unit == "") {
        Unit = null;
    }
    var seq = $("#seq").val();
    var Report = true;
    var Eva = true;
    var Detail = true;
    if ($("#IfReport").val() == '0') {
        Report = false;
    }
    if ($("#IfEva").val() == '0') {
        Eva = false;
    }
    if ($("#IfDetail").val() == '0') {
        Detail = false;
    }
    if (TarName == "") {
        alert("指标名称不能为空!");
    }
    else if (TargtTypeSelect == 0) {
        alert("请选择指标类型!");
    }
    else if (seq == "") {
        alert("排序值不能为空!");
    }
    else {
        var jsoninfo = "";
        jsoninfo += "TargetName:" + TarName + ",";
        jsoninfo += "HaveDetail:" + Detail + ",";
        jsoninfo += "NeedEvaluation:" + Eva + ",";
        jsoninfo += "NeedReport:" + Report + ",";
        jsoninfo += "TargetType:" + TargtTypeSelect + ",";
        jsoninfo += "Unit:" + Unit + ",";
        jsoninfo += "Sequence:" + seq + ",";
        jsoninfo += "BaseLine:" + BaseLine + "";
        WebUtil.ajax({
            async: false,
            args: { info: jsoninfo, SystemID: SysID },
            url: "/CompanyController/AddTarget",
            successReturn: function (ID) {
                reload();
                alert("保存成功!");
                art.dialog({ id: 'divDetail' }).close();
            }
        });
    }
}



function Change(ID) {
    $("#savetrue").show();
    $("#addtrue").hide();
    WebUtil.ajax({
        async: false,
        args: { TargetID: ID },
        url: "/CompanyController/GetTarget",
        successReturn: function (result) {
            TargetModel = result;
            BangEums(result.TargetType);
            $("#addname").val(result.TargetName);
            if (result.NeedReport == false) {
                $("#IfReport").val("0");
            }
            if (result.NeedEvaluation == false) {
                $("#IfEva").val("0");
            }
            if (result.HaveDetail == false) {
                $("#IfDetail").val("0");
            }
            $("#rpt_info_BaseLine").val(SetTime(result.BaseLine, false));
            $("#Unit").val(result.Unit);
            $("#seq").val(result.Sequence);
        }
    });
    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>编辑指标</span>'
    });
}
function SaveTarget() {
    var BaseLine = $("#rpt_info_BaseLine").val();
    if (BaseLine == "") {
        BaseLine = "0001/1/1 0:00:00";
    }
    var Unit = $("#Unit").val();
    if (Unit == "") {
        Unit = null;
    }
    var Report = true;
    var Eva = true;
    var Detail = true;
    if ($("#IfReport").val() == '0') {
        Report = false;
    }
    if ($("#IfEva").val() == '0') {
        Eva = false;
    }
    if ($("#IfDetail").val() == '0') {
        Detail = false;
    }
    if ($("#addname").val() == "") {
        alert("指标名称不能为空!");
    }
    else if ($("#Enums").val() == 0) {
        alert("请选择指标类型!");
    }
    else if ($("#seq").val() == "") {
        alert("排序值不能为空!");
    }
    else {
        TargetModel.TargetName = $("#addname").val();
        TargetModel.HaveDetail = Detail;
        TargetModel.NeedEvaluation = Eva;
        TargetModel.NeedReport = Report;
        TargetModel.TargetType = $("#Enums").val();
        TargetModel.BaseLine = BaseLine;
        TargetModel.Unit = Unit;
        TargetModel.Sequence = $("#seq").val();
        WebUtil.ajax({
            async: false,
            args: { info: WebUtil.jsonToString(TargetModel), ID: TargetModel.ID },
            url: "/CompanyController/UpdateTarget",
            successReturn: function (result) {
                reload();
                alert("保存成功!");
                art.dialog({ id: 'divDetail' }).close();
            }
        });
    }
}
function ShowKpi() {
    var value = $("#HaveKpi").val();
    if (value == '1') {
        $("#Kpi").show();
    }
    else {
        $("#Kpi").hide();
    }

}

function Delete(ID) {
    WebUtil.ajax({
        async: false,
        args: { ID: ID },
        url: "/CompanyController/DeleteTarget",
        successReturn: function (id) {
            reload();
            alert("删除成功!");
        }
    });
}
function ExceptionChange() {
    location.href = "../SystemConfiguration/ExceptionTarget.aspx?ID=" + SysID + "&ComeFrom=TargetConfiguration";
}