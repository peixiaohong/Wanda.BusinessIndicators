<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TodoWorkList.aspx.cs" Inherits="WorkFlowDemo.TodoWorkList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title class="itemTitle">年度预算指标上报</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Content-Language" content="zh-cn" />

    <link href="Styles/style02.css" rel="stylesheet" />
    <link href="styles/popup.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../Scripts/base.js"></script>
    <script src="Scripts/json/budgetitems.js"></script>

    <script src="Scripts/jquery.zxxbox.3.0.js"></script>
    <link href="Styles/jquery.zxxbox.style.css" rel="stylesheet" />

    <!--    <script src="../Scripts/jquery.colorbox.js"></script>
    <link href="../Styles/colorbox.css" rel="stylesheet" />-->
    <style type="text/css">
        table.tab_5 tbody td {
            text-align: center;
        }

        table.tab_5 textarea {
            width: 100%;
        }

        .method_div {
            margin: 20px 20px 8px 20px;
        }

        table.tab_4 th {
            width: 20%;
            text-align: right;
            padding-right: 5px;
        }
    </style>
</head>
<body>
    <form id="Form1" runat="server">
        <div class="box">
            <div id="headerContent" class="header">
            </div>
            <div id="main">
                <div id="leftContent"></div>
                <div id="breadpathContent" class="breadpathContent">
                </div>
                <div class="preview">
                    <div id="propose_enclosure" class="launchProcess_cont">
                        <ul class="propose_cont_title">
                            <li><a><span>待办信息</span></a></li>
                        </ul>
                        <div class="propose_content">
                            <div class="method_div">
                                <table class="tab_5">
                                    <tbody>
                                        <tr class="tab_5_row_alt">
                                            <th>编号</th>
                                            <th style="width: 450px;">任务名称</th>
                                        </tr>
                                        <asp:Repeater ID="Rpt" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="align_center"><%#Container.ItemIndex + 1%></td>
                                                    <td><a href="Approval.aspx?uid=<%#Eval("UserID") %>&bid=<%#Eval("BizID") %>"><%#Eval("Name") %></a></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>


                </div>

                <!-- end of preview-->
            </div>
        </div>
        <!--box-->
        <div id="footerContent">
        </div>
    </form>
    <script type="text/javascript">


        $(function () {
            loadHeader("预算指标管理");
            loadFooter(); loadLeft();
            loadPagenation();
            loadBreadPath(["预算指标管理", "年度预算指标上报"]);

        });
    </script>
</body>
</html>



