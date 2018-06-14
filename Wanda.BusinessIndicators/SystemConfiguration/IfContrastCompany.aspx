<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IfContrastCompany.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="LJTH.BusinessIndicators.Web.IfContrastCompany" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 
    <script src="../Scripts/BusinessReport/ContrastCompany.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="main">
        <div class="" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">

            <asp:DropDownList runat="server" ClientIDMode="Static" ID="DSystemID" AutoPostBack="true" Style="width: 150px; height: 25px;"></asp:DropDownList>
        </div>
        <div>
        </div>
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
<%--                            <div class="uploadify-button" id="Div1" style="width: 90px; float: right;" onclick="ReturnSystem()">
                                <span>
                                    <label class="DownExcelLabel" id="Label1">返回</label></span>
                            </div>--%>
                            <div class="uploadify-button" id="file_upload-button1" style="width: 90px; float: right; margin-right: 20px" onclick="ReplaceCompanyList();">
                                <span>
                                    <label class="DownExcelLabel" id="LabelDownload">批量替换</label></span>
                            </div>

                        </td>
                    </tr>
                </tbody>
            </table>

        </div>


        <div>
            <table class="tab_005" style="width: 100%" id="FloateTable2">
                <thead style="width:100%">
                </thead>
            </table>
            <table class="tab_005" style="width: 100%" id="Table2">
                <thead  id="Tabhead" style="width:100%">
                    <tr>
                        <th style="width: 5%"></th>
                        <th style="width: 5%">序号</th>
                        <th style="width: 20%">公司名称</th>
                        <th style="width: 20%">开店时间</th>
                        <th style="width: 20%">操作人</th>
                        <th style="width: 20%">操作时间</th>
                        <th style="width: 15%">操作</th>

                    </tr>
                </thead>

                <tr class="Level1"  >
                    <td class="Td_Merge Level1TdSp1" colspan="1" style="text-align: center;"> </td>
                    <td class="Td_Merge Level1TdSp1" colspan="6" onclick="TabChange('A')" >不可比公司</td>
                    
                </tr>
                <tbody id="Tab1"><%--可比公司数据--%>
                    
                </tbody>
                
                    <tr class="Level1" >
                        <td class="Td_Merge Level1TdSp1" colspan="1" style="text-align: center;"> </td>
                        <td class="Td_Merge Level1TdSp1" colspan="6" onclick="TabChange('B')">可比公司</td>


                    </tr>
                <tbody id="Tab2"><%--不可比公司数据--%>
                </tbody>
               
                 
            </table>
        </div>


    </div>


    </asp:Content>
