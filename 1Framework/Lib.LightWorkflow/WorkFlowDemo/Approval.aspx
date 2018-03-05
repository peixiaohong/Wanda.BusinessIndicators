<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="WorkFlowDemo.Approval" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title class="itemTitle">年度预算指标审批</title>
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
    <form runat="server">
    <div class="box">
        <div id="headerContent" class="header">
        </div>
        <div id="main">
            <div id="leftContent"></div>
            <div id="breadpathContent" class="breadpathContent">
            </div>

            <div class="preview">
                <!--<ul class="launchProcess">
                    <li><a ><span>上报内容</span></a></li>
                    <li><a ><span>上报说明</span></a></li>
                    <li><a ><span>上报数据</span></a></li>
                    <li><a ><span>上报流程</span></a></li>
                    <li><a ><span>审批意见</span></a></li>
                </ul>-->
                <div id="propose_cont" class="launchProcess_cont">
                    <ul class="propose_cont_title">
                        <li><a><span>上报基本信息</span></a></li>
                    </ul>
                    <div class="propose_content">
                        <div class="method_div">
                            <table class="tab_4">
                                <tbody>
                                    <tr>
                                        <th>上报范围</th>
                                        <td>
                                            <select style="width: 160px" disabled="disabled">
                                                <option selected="selected">集团总部</option>
                                                <option>商业地产总部</option>
                                                <option>文化集团总部</option>
                                            </select></td>
                                    </tr>
                                    <tr>
                                        <th>预算年度</th>
                                        <td>
                                            <select style="width: 160px" disabled="disabled">
                                                <option selected="selected">2013年</option>
                                                <option>2014年</option>
                                                <option>2015年</option>
                                                <option>2016年</option>
                                                <option>2017年</option>
                                            </select></td>

                                    </tr>
                                    <tr>
                                        <th>上报申请人</th>
                                        <td><span runat="server" id="userN">孙胜强</span></td>
                                    </tr>
                                    <tr>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div id="propose_data" class="launchProcess_cont">
                    <ul class="propose_cont_title">
                        <li><a><span>上报说明</span></a></li>
                    </ul>
                    <div class="propose_content">

                        <div class="method_div">
                            <div style="line-height: 2.5;">请填写上报说明：</div>
                            <textarea rows="5" style="width: 100%" disabled="disabled" id="note" runat="server"></textarea>
                        </div>
                    </div>
                </div>
                <%--<div id="propose_wf" class="launchProcess_cont" style="visibility:hidden">
                    <ul class="propose_cont_title">
                        <li><a><span>上报预算指标</span></a></li>
                    </ul>

                    <div class="propose_content" id="uploadedData">
                        <div class="method_div">
                            <div>
                                <table  class="tab_3">
                                    <tr>
                                        <th>上报预算指标文件</th>
                                        <td>
                                            <input type="text" /><a href="#" class="btn" style="margin-left: 14px">浏览</a> </td>
                                        <td>上报的数据将覆盖更新当前预算。<a href="../material/预算编制模板.xlsx">请点此下载数据文件<img src="../Images/xls.gif" /></a>！</td>
                                    </tr>
                                </table>

                            </div>
                        </div>

                        <div class="method_div" style="display: none">
                            <table class="tab_5" id="importedDataTable">
                                <tr>
                                    <th style="width: 32px">序号</th>
                                    <th><span class="itemTitle">项目</span>(万元)</th>
                                    <th>1月</th>
                                    <th>2月</th>
                                    <th>3月</th>
                                    <th>4月</th>
                                    <th>5月</th>
                                    <th>6月</th>
                                    <th>7月</th>
                                    <th>8月</th>
                                    <th>9月</th>
                                    <th>10月</th>
                                    <th>11月</th>
                                    <th>12月</th>
                                    <th>小计</th>

                                </tr>
                                <tr class="sumrow sum">
                                    <td class="align_center">1</td>
                                    <td>合计</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td></td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td></td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">2</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td></td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td></td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">3</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">4&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">5</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">6</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">7</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="align_center">8</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>

                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                            </table>
                            <div id="pagerContent">
                            </div>
                        </div>
                    </div>
                </div>--%>
                <div id="Div2" class="launchProcess_cont">
                    <ul class="propose_cont_title">
                        <li><a><span>上报文件列表</span></a></li>
                    </ul>
                    <div class="propose_content">
                        <div class="method_div">
                            <table class="tab_5">
                                <tbody>
                                    <tr class="tab_5_row_alt">
                                        <th>编号</th>
                                        <th style="width: 450px;">附件名称</th>
                                        <th>上传用户</th>
                                        <th>保存时间</th>
                                        <th>操作</th>
                                    </tr>
                                    <tr>
                                        <td class="align_center">1</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>


                <div id="Div1" class="launchProcess_cont">
                    <ul class="propose_cont_title">
                        <li><a><span>审批意见</span></a></li>
                    </ul>
                    <div class="propose_content">

                        <div class="method_div">
                            <div style="line-height: 2.5;">请填写审批意见：</div>
                            <textarea rows="5" style="width: 100%" runat="server" id="approvalNotes"></textarea>
                        </div>
                    </div>
                </div>
                <div id="propose_enclosure" class="launchProcess_cont">
                    <ul class="propose_cont_title">
                        <li><a><span>上报决策文件</span></a></li>
                    </ul>
                    <ul class="add_fujian">
                        <li><a href="javascript:void(0)" class="btn" onclick="$('#enclosure_div').show()">上传</a></li>
                    </ul>
                    <div class="propose_content">
                        <div class="method_div">
                            <table class="tab_5">
                                <tbody>
                                    <tr class="tab_5_row_alt">
                                        <th>编号</th>
                                        <th style="width: 450px;">附件名称</th>
                                        <th>上传用户</th>
                                        <th>保存时间</th>
                                        <th>操作</th>
                                    </tr>
                                    <tr>
                                        <td class="align_center">1</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div id="wf_comment" class="launchProcess_cont">
                    <ul class="propose_cont_title">
                        <li><a><span>审批情况</span></a></li>
                    </ul>
                    <div class="propose_content">
                        <div class="content2" id="approvalBox">

                            <div class="method_div">
                                <table class="tab_2 ">
                                    <tbody>
                                        <tr style="height: 84px;">
                                            <th>预算上报审批流</th>
                                            <td colspan="2">
                                                <ul class="approval">
                                                    <li title="passed" class="passed">孙胜强</li>
                                                    <li class="arrow">&nbsp;</li>
                                                    <li title="current" class="current"><span>集团财务</span></li>
                                                    <li class="arrow">&nbsp;</li>
                                                    <li>集团财务领导</li>
                                                    <!--<li class="arrow">&nbsp;</li>
                                                <li title="end" class="end">&nbsp;</li>-->
                                                </ul>

                                            </td>
                                        </tr>

                                        <tr class="tab_2_row_alt">
                                            <td>审批人</td>
                                            <td style="width: 96px;">审批时间</td>
                                            <td style="">审批意见</td>
                                        </tr>
                                        <asp:Repeater ID="Rpt" runat="server">
                                            <ItemTemplate>
                                                <tr style="background: rgb(255, 255, 255);">
                                                    <td class="align_center"><%#Eval("UserName")%></td>
                                                    <td><%#Eval("CompleteTime")%></td>
                                                    <td><%#Eval("ApprovalNote") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                                <div style="text-align: right;">
                                    <a id="approval" class="issuebtn" runat="server" onserverclick="Apply_Click">审批</a>
                                    <a id="Reject" class="issuebtn" runat="server" onserverclick="Reject_Click">退回</a>
                                    <a href="javascript:void(0)" class="issuebtn">取消</a>
                                </div>
                            </div>
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

            var rowNames = ["长白山公司股权收入", "黑土公司股权收入", "绿城股权收入", "中海地产股权收入", "长江实业股权收入"];
            $("#importedDataTable tr:gt(0)").each(function (i, row) {
                $(row).find("td:eq(0)").text(i + 1);
                if (rowNames[i]) {
                    $(row).find("td:eq(1)").text(rowNames[i]);
                    var sum = 0;

                    $(row).find("td:gt(1)").each(function (j, cell) {

                        $(cell).addClass("ra");

                        if (Math.random() < 0.3 && j < 12) {
                            var value = (Math.random() * 100);
                            $(cell).text(value.toFixed(2));
                            sum += value;
                        } else if (j == 12) {
                            $(cell).text(sum.toFixed(2));
                        }
                    });
                }
            });
        });
    </script>
</body>
</html>
