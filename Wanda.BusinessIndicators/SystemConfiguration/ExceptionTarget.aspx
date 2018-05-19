<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceptionTarget.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.ExceptionTarget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/ExceptionTarget.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">

        <select runat="server" clientidmode="Static" style="width: 150px; height: 25px" id="ddlSystem" onchange="reload()"></select>
    </div>
    <div class="margin_t10">
        <div class="tabs_m">
            <table style="width: 99%">
                <tbody>
                    <tr>
                        <td>
                            <ul id="tabs" class="tabs" style="MARGIN-BOTTOM: 0px; HEIGHT: auto; BORDER-BOTTOM-COLOR: #fff; MARGIN-TOP: 5px">
                            </ul>
                        </td>
                        <td>

                            <div class="uploadify-button" id="file_upload-button1" style="width: 90px; float: right; margin-right: 20px" onclick="ReplaceCompany();">
                                <span>
                                    <label class="DownExcelLabel" id="LabelDownload">替换</label></span>
                            </div>

                        </td>
                    </tr>
                </tbody>
            </table>

        </div>


        <div>
            <table class="tab_005" style="width: 100%" id="FloatTable2">
                <thead>
                </thead>           

            </table>
            <table class="tab_005" style="width: 100%" id="Table2">
                <thead id="head" style="width: 100%">
                    <tr>
                        <th style="width: 5%"></th>
                        <th style="width: 5%">序号</th>
                        <th style="width: 20%">公司名称</th>
                        <th style="width: 20%">模式</th>
                        <th style="width: 25%">操作人</th>
                        <th style="width: 25%">操作时间</th>

                    </tr>
                </thead>

                <tbody id="Tbody1">
                    <%--上报不考核数据--%>

                    <tr class="Level1" onclick="TabChange('A')">
                        <td class="Td_Merge Level1TdSp1"></td>
                        <td class="Td_Merge Level1TdSp1"></td>
                        <td class="Td_Merge Level1TdSp1" style="text-align: center;">上报不考核</td>
                        <td class="Td_Merge Level1TdSp1"></td>
                        <td class="Td_Merge Level1TdSp1"></td>
                        <td class="Td_Merge Level1TdSp1"></td>
                    </tr>
                </tbody>
                <tbody id="Tab1">
                    <%--上报不考核数据--%>
                </tbody>

                <tr class="Level1" onclick="TabChange('B')">
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1" style="text-align: center;">不上报不考核</td>
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1"></td>

                </tr>
                <tbody id="Tab2">
                    <%--不上报不考核数据--%>
                </tbody>

                <tr class="Level1" onclick="TabChange('C')">
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1" style="text-align: center;">上报考核</td>
                    <td class="Td_Merge Level1TdSp1"></td>
                    <td class="Td_Merge Level1TdSp1"></td>

                    <td class="Td_Merge Level1TdSp1"></td>
                </tr>


                <tbody id="Tab3">
                    <%--上报考核数据--%>
                </tbody>
            </table>
        </div>


    </div>

    <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <table style="width: 400px;" class="tab02">
            <tr>
                <td style="color: red">非同类型公司不能同时转换</td>
            </tr>
            <tr>
                <td>
                    <select style="width: 150px" id="ChangSelect">
                        <option value="0">--请选择--</option>
                        <option value="A">上报不考核</option>
                        <option value="B">不上报不考核</option>
                        <option value="C">上报考核</option>
                    </select></td>

            </tr>

        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="保存" id="savetrue" onclick="SaveReplace()" />
        </div>
    </div>



    <input type="hidden" runat="server" id="SysID" clientidmode="Static" />

    <asp:HiddenField ClientIDMode="Static" runat="server" ID="ComeFrom" />
</asp:Content>
