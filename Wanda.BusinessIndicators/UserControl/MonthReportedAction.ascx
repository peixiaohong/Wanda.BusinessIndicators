<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonthReportedAction.ascx.cs" Inherits="Wanda.BusinessIndicators.Web.UserControl.MonthReportedAction" %>

<html>
<head>
    <title>月报上报操作</title>
    <script type="text/javascript">
       
        //初始化数据方法
        function GetMonthlyReportAction() {
            //加载月度报告说明
            WebUtil.ajax({
                async: true,
                url: "/TargetReportedControll/GetMonthlyReportActionList",
                args: { MonthlyReportID: MonthReportID },
                successReturn: SplitDataMonthReport
            });
          
        }
        function SplitDataMonthReport(resultData) {
            $('#MonthlyReportActionHead').empty();
            $('#tab_rows').empty();
            loadTmpl_MonthReportAction('#MonthlyReportActionHeadTmpl').tmpl().appendTo('#MonthlyReportActionHead');
            loadTmpl_MonthReportAction('#MonthlyReportActionTmpl').tmpl(resultData).appendTo('#tab_rows');
        }
        //加载模版项
        function loadTmpl_MonthReportAction(selector) {
            return WebUtil.loadTmpl("../BusinessReport/MonthlyReportActionTmpl.html", selector);
        }
        
    </script>
</head>
<body>
    <div style="display:none" id="MonthReportLog">
        <table class="tab_005" id="importedDataTable2">
            <thead id="MonthlyReportActionHead">
            </thead>
            <tbody id="tab_rows">
            </tbody>
        </table>
    </div>
</body>
</html>
