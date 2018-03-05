

//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/MonthReportConfig.html", selector);
}


$(function () {

    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();

    WebUtil.ajax({
        async: false,
        url: "/MonthRptConfigControll/GetSysList",
        args: { SystemID: SysId, Year: FinYear },
        successReturn: function (result) {

            var ObjValue = {};
            ObjValue.SysData = result;

            $("#MonthRptConfigBody").empty();
            $("#MonthRptConfigHead").empty();
            
            loadTmpl('#TmplHeadMonthRptConfig').tmpl().appendTo('#MonthRptConfigHead');

            loadTmpl('#TmplBodyMonthRptConfig').tmpl(ObjValue).appendTo('#MonthRptConfigBody');

        }
    });


});


function SearchData()
{
    var SysId = $("#ddlSystem").val();
    var FinYear = $("#ddlYear").val();

    WebUtil.ajax({
        async: false,
        url: "/MonthRptConfigControll/GetSysList",
        args: { SystemID: SysId, Year: FinYear },
        successReturn: function (result) {

            var ObjValue = {};
            ObjValue.SysData = result;

            $("#MonthRptConfigBody").empty();
            $("#MonthRptConfigHead").empty();

            loadTmpl('#TmplHeadMonthRptConfig').tmpl().appendTo('#MonthRptConfigHead');

            loadTmpl('#TmplBodyMonthRptConfig').tmpl(ObjValue).appendTo('#MonthRptConfigBody');

        }
    });


    
}


function SaveData()
{
    
    //alert($("#SumTmpl").val() + "aa:" + $("#DetailTmpl").val() + "bb:" +  + "CC:" + $("#ReturnTmpl").val());
    var SysId = $("#ddlSystem").val();
    var _SunTmpl =$("#SumTmpl").val();
    var _DetailTmpl = $("#DetailTmpl").val();
    var _MissTargetlTmpl = $("#MissTargetlTmpl").val();
    var _ReturnTmpl =  $("#ReturnTmpl").val();

    WebUtil.ajax({
        async: false,
        url: "/MonthRptConfigControll/SaveSysConfiguration",
        args: { SystemID: SysId, SunTmpl: _SunTmpl, DetailTmpl:_DetailTmpl, MissTargetlTmpl:_MissTargetlTmpl, ReturnTmpl:_ReturnTmpl},
        successReturn: function (result) {

            var ObjValue = {};
            ObjValue.SysData = result;

            $("#MonthRptConfigBody").empty();
            $("#MonthRptConfigHead").empty();

            loadTmpl('#TmplHeadMonthRptConfig').tmpl().appendTo('#MonthRptConfigHead');

            loadTmpl('#TmplBodyMonthRptConfig').tmpl(ObjValue).appendTo('#MonthRptConfigBody');

        }
    });



}