
//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/ComprehensiveReportForms.html", selector);
}


$(function () {
    InitYear();

    GetReportTime();

});



function InitYear() {
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetYear",
        successReturn: function (dateresult) {

            loadTmpl("#Tmpl_ddlYearInfo").tmpl(dateresult).appendTo("#ddlYears");
        }
    });

}


function GetReportTime() {
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetReportTime",
        successReturn: function (dateresult) {

            var Report = dateresult;
            var MMM = new Date(dateresult.ReportTime).getMonth() + 1;

            var YYY = new Date(dateresult.ReportTime).getFullYear();

            $("#ddlYears").val(YYY);

            $("#ddlMonths").val(MMM);

        }
    });
}


function DownExcelReport(sender) {
 
    
    var FinMonth = $("#ddlMonths").val();
    var G_FinYear = $("#ddlYears").val();
    
    var _F = WebUtil.jsonToString([G_FinYear]);

    if (sender == 'Movie')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Movie&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
    else if (sender == 'Children')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Children&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
    else if (sender == 'Business')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Business&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
    else if (sender == 'Culture')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Culture&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
}