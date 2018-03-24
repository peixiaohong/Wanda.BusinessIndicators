<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" CodeBehind="TargetReportApproveTime.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.TargetReportApproveTime" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/FloatTable.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/ApproveTime.js"></script>
    <script type="text/javascript" src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">

        <div class="">
            <div class="title">
                <a href="#"><span>查询条件</span></a>
            </div>
            <!--title-->
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div class="content_m">
                <table class="tab_search">
                    <tbody>
                        <tr>
                            <th>上报系统</th>
                            <td>
                                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px;" AutoPostBack="true"></asp:DropDownList>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>上报月份</th>
                            <td>
                                <asp:DropDownList ID="ddlMonth" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>
                                <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" style="cursor: pointer" onclick="BangList()"><span>查询</span></a>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
        </div>


        <!--明细项 开始-->
        <div class="margin_t10">
            <div class="tabs_m">
                <table style="width: 99%">
                    <tr>
                        <td style="width: auto">
                            <ul class="tabs" id="tabs" style="margin-bottom: 0px; height: auto; border-bottom-color: #fff; margin-top: 5px">
                            </ul>
                        </td>
                        <td style="width: auto; float: right">

                            <div class="uploadify-button" style="width: 95px; float: right; display: inline; padding: 0" id="file_upload-button1" onclick="DownExcel();">
                                <span>
                                    <label class="DownExcelLabel" id="LabelDownload">导出数据</label></span>
                            </div>

                            <div style="width: 80px; float: right; display: inline; padding-right: 25px" id="UpLoadData">
                                <span>
                                    <span>
                                        <input type="file" name="upload" id="file1" value="导入补回期限" />
                                    </span>
                                </span>
                            </div>
                            <%--  <div id="UpLoadData" >
                                   
                                </div>--%>

                           

                        </td>
                    </tr>
                </table>

            </div>





            <table class="tab_005" id="Tab_Return">
                <thead id="TbH1">
                    <tr>
                        <th style="width: 10%">序  号</th>
                        <th style="width: 20%">名  称</th>
                        <th style="width: 20%">承诺时间</th>
                        <th style="width: 20%">要求时间</th>
                        <th style="width: 20%">要求说明</th>
                        <th style="width: 10%">警示灯</th>

                    </tr>
                </thead>
                <tbody id="TData1" class="tab_001">
                </tbody>
            </table>
        </div>

    </div>



    <div id="divMonthReportDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">

        <table style="width: 600px;" class="tab02">
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">名&nbsp;&nbsp;&nbsp;称:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="name"></td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">经营指标:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="target"></td>
            </tr>

            <tr style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">补回情况:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="promiss"></td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">承诺时间:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="promissd"></td>
            </tr>
            <tr id="Dateshowselect">
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">要求期限选择方式:</th>
                <td id="SGselect" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="radio" id="radio1" value="月" name="time" checked="checked" onclick="MonthShow();" />按月份
                    <input type="radio" id="radio2" value="日" name="time" onclick="DateShow();" />按日期
                    <input type="radio" id="radio3" value="补回" name="time" onclick="Yaershow()" />年内未补回
                    <span style="color: red" id="redtext">选择日期后,此指标将被设置为完成状态</span>
                </td>
                <td id="Yearselect" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="radio" id="radio4" value="月" name="time" onclick="MonthShow();" />按月份
                    <input type="radio" id="radio5" value="补回"  name="time" onclick="Yaershow()" />年内未补回
                </td>
            </tr>
            <tr id="datetime">
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">要求补回期限:</th>
                <td style="width: 25%; padding: 5px; padding-left: 20px; text-align: left;">
                    <div id="monthshow">
                        <input type="text" id="rpt_info_CommitMonth" runat="server" clientidmode="Static" style="width: 150px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM', minDate: minDatatime })" value="" />
                    </div>
                    <div id="dateshow">
                        <input type="text" id="rpt_info_CommitDate" runat="server" clientidmode="Static" style="width: 150px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: minDatatime })" value="" />
                    </div>
                </td>
            </tr>
            <tr id="YearInt">
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">实际金额</th>
                <td style="width: 25%; padding: 5px; padding-left: 20px; text-align: left;">
                    <div id="yearshow">
                        <input type="text" id="rpt_info_CommitInt" runat="server" clientidmode="Static" style="width: 150px;" onblur="Returned()" value="" /><span style="color: red" id="tip"></span>
                    </div>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 10px; text-align: right;">要求说明:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <textarea cols="60" rows="3" runat="server" clientidmode="Static"  id="rpt_info_CommitReason"></textarea>
                </td>
            </tr>
        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divMonthReportDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" Save();" />
        </div>
        <input type="hidden" id="hiddtime" runat="server" clientidmode="Static" />
    </div>
    <input type="hidden" id="sg" value="<%=MonthSG%>" />
    <input type="hidden" id="sgrent" value="<%=MonthSGRent%>" />
    <input type="hidden" runat="server" clientidmode="Static" id="alljson" />
</asp:Content>
