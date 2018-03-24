<%@ Page Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="MonthReportConfig.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.MonthReportConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/ecmascript" src="../Scripts/BusinessReport/MonthRptConfig.js"></script>
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
                                <asp:DropDownList ID="ddlSystem" ClientIDMode="Static" runat="server" Style="width: 120px;"  ></asp:DropDownList>
                            </td>
                            <th>上报年份</th>
                            <td>
                                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th>上报月份</th>
                            <td>
                                <asp:DropDownList ID="ddlMonth" ClientIDMode="Static" runat="server" Style="width: 120px;"></asp:DropDownList>
                            </td>
                            <th style="text-align: center">
                                <input id="submana" runat="server" clientidmode="Static" style="width: 80px; text-align: center; height: 27px; display: none" type="button" class="uploadify-button" value="" onclick="AddMessages()" /></th>
                            <td>
                                <asp:CheckBox ID="chkIsLastestVersion" style="display:none;"  ClientIDMode="Static" runat="server" Text="包含审批中"    />
                            </td>
                            <th>
                                <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="SearchData();"><span>查询</span></a>
                            </th>
                        </tr>
                    </tbody>
                </table>
                    </div>
                        <div class="content_b_l">
                        <div class="content_b_r"></div>
                    </div>
        </div>
            <div class="" style="margin-bottom:5px; margin-top:10px; float:right; " >
                     <a class="btn_search" id="A1" href="#" onclick="SaveData();" style="font-weight:700;font-size:14px; " > <span>模版数据保存</span></a>
            </div>
      
          <div class="tabs_content">
          <table id="MonthRptConfig" class="tab_005">
              <thead id="MonthRptConfigHead" >
              </thead>

              <tbody id="MonthRptConfigBody" >
              </tbody>
          </table>
        </div>


    </div>


           

      
        

</asp:Content>
