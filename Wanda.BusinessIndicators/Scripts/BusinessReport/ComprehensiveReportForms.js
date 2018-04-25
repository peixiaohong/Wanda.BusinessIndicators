//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/ComprehensiveReportForms.html?ver=<%=new Random(DateTime.Now.Millisecond).Next(0,10000)%>", selector);
}
//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}

$(function () {
    //页面效果配置
    PageEffect();

    //页面数据初始化加载
    PageLoad();

    //页面事件加载
    EventLoad();
});

//页面效果配置
function PageEffect()
{
    var ie6 = document.all;
    var dv = $('.table-head'), st;
    dv.attr('otop', dv.offset().top); //存储原来的距离顶部的距离
    $(window).scroll(function () {
        st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
        if (st > parseInt(dv.attr('otop'))) {
            if (ie6) {//IE6不支持fixed属性，所以只能靠设置position为absolute和top实现此效果
                dv.css({ position: 'absolute', top: st });
            }
            else if (dv.css('position') != 'fixed') {
                dv.css({ 'position': 'fixed', top: 0 });
            }
        }
        else if (dv.css('position') != 'static') {
            dv.css({ 'position': 'static' });
        }
        $(".table-head").width($(".table-body").width());
    });
}

//页面数据初始化加载
function PageLoad()
{
    //上报系统下拉框数据加载
    SystemInfoLoad();
    //上报年份数据加载
    SelectYearsInfoLoad();
    //页面列表数据加载
    ShowReportDataLoad('00000000-0000-0000-0000-000000000000',0,0);
}


//页面事件加载
function EventLoad()
{
    //查询按钮单击事件
    $(".btn_search").off('click').on('click', function () {
        var systemID = $("#systemInfo").find("option:selected").attr("data-id");
        var year = $("#selectYears").find("option:selected").attr("data-id");
        var month = $("#selectMonth").find("option:selected").attr("data-value");
        ShowReportDataLoad(systemID, year, month);
    });
}


//上报系统下拉框数据加载
function SystemInfoLoad()
{
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetSystemInfo",
        args: {},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                loadTmpl('#systemInfo_Tmpl').tmpl(resultData.Data).appendTo('#systemInfo');
            }
            else {
            }
        }
    });
} 

//上报年份数据加载
function SelectYearsInfoLoad()
{
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetYear",
        args: {},
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                loadTmpl('#selectYears_Tmpl').tmpl(resultData.Data).appendTo('#selectYears');
                //设定当前年默认选中
                $("#selectYears").find("option[data-Id='" + resultData.NowYear + "'").attr("selected", "selected");
                //设定当前月默认选中
                $("#selectMonth").find("option[data-value='" + resultData.NowMonth + "'").attr("selected", "selected");
            }
            else {
            }
        }
    });
}

//页面列表数据加载
function ShowReportDataLoad(systemID, year, month)
{
    //加载动画
    Load();
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetListData",
        args: { systemID: systemID, year: year, month: month },
        successReturn: function (resultData) {
            if (resultData.Success == 1) {
                $('#ShowReportData').empty();
                loadTmpl('#showReportData_Tmpl').tmpl(resultData).appendTo('#ShowReportData');
            }
            else {
            }
            Fake();
        }
    });
}

