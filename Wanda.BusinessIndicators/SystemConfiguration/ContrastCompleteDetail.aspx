<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContrastCompleteDetail.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="Wanda.BusinessIndicators.Web.ContrastCompleteDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/ContrastComplete.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <div class="title">
            <a href="#"><span>查询条件</span></a>
        </div>
        <div class="content_t_l">
            <div class="content_t_r"></div>
        </div>
        <div class="content_m" style="text-align: center;">

            <table class="tab_search">
                <tbody>
                    <tr>
                        <th></th>
                        <th>上报年份:</th>
                        <td>
                            <asp:DropDownList ID="ddlYear" runat="server" ClientIDMode="Static" Style="width: 120px;">
                            </asp:DropDownList>
                            &nbsp;</td>
                        <th>上报月份:</th>
                        <td>
                            <asp:DropDownList ID="ddlMonth" runat="server" ClientIDMode="Static" Style="width: 120px;">
                            </asp:DropDownList>
                        </td>
                        <td style="display:none">
                            <input type="checkbox" id="IsPro"  />包含审批中
                        </td>
                        <th>
                            <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" style="cursor: pointer" onclick="GetList()"><span>查询</span></a>
                        </th>

                    </tr>
                </tbody>
            </table>
        </div>
        <div class="content_b_l">
            <div class="content_b_r"></div>
        </div>
    </div>
    <div class="margin_t10">
        <div class="tabs_m">
            <ul id="Ul3">
                <li class="selected m_1"><a class="active_sub2"><span>完成情况同期对比表</span></a></li>
            </ul>
            <%--            <div class="DownExcelDiv" id="DownExcel">
                <span class="DownExcelspan">
                    <div class="uploadify-button" id="file_upload-button1">
                        <span>
                            <label class="DownExcelLabel" id="LabelDownload">导出数据</label></span>
                    </div>
                </span>
            </div>--%>
        </div>
        <div class="uploadify-button" id="file_upload-button1" style="width: 90px; float: right; margin-right: 20px" onclick="DownExcel();">
            <span>
                <label class="DownExcelLabel" id="LabelDownload">导出数据</label></span>
        </div>

    </div>
    <div class="tabs_content">
        <div class="scrolldoorFrame copy">
            <table class="tab_005" id="importedDataTable2">
                <thead id="head">
                    <tr>
                        <th rowspan="2" style="width: 3%">序号</th>
                        <th rowspan="2" style="width: 6%">系统</th>
                        <th rowspan="2" style="width: 9%">类型</th>
                        <th colspan="4">系统整体</th>
                        <th colspan="4">可比门店</th>
                        <th colspan="4">不可比门店</th>
                        <th rowspan="2" style="width: 17%">备注</th>
                        <th rowspan="2" style="width: 5%">编辑</th>
                    </tr>
                    <tr>
                        <th style="width: 5%" class="Last"></th>
                        <th style="width: 5%" class="Now"></th>
                        <th style="width: 5%">差额</th>
                        <th style="width: 5%">增长率</th>
                        <th style="width: 5%" class="Last"></th>
                        <th style="width: 5%" class="Now"></th>
                        <th style="width: 5%">差额</th>
                        <th style="width: 5%">增长率</th>
                        <th style="width: 5%" class="Last"></th>
                        <th style="width: 5%" class="Now"></th>
                        <th style="width: 5%">差额</th>
                        <th style="width: 5%">增长率</th>
                    </tr>

                </thead>
                <tbody id="rows">
                </tbody>

            </table>
        </div>
    </div>
   <div>
        <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
            <table style="width:500px; height:250px;"  class="tab_search">
                <tr>
                    <td style="width:35%" class="Td_Center" >系统名称:</td>
                    <td style="width:65%" class="Td_TrueLeft" id="RemarkSystem" ></td>
                </tr>
                <tr>
                    <td class="Td_Center">指标名称:</td>
                    <td class="Td_TrueLeft" id="RemarkTarget"></td>
                </tr>
                <tr>
                    <td class="Td_Center">备注</td>
                    <td  style="width:30%" class="Td_Center"><textarea id="Remark" cols="55" rows="7"></textarea></td>
                </tr>
            </table>
     
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" AddRemark();" id="savetrue" />
        </div>
    </div>
   </div>
</asp:Content>
