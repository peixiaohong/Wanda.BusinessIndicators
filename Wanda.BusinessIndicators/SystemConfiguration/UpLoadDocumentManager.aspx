<%@ Page Language="C#" AutoEventWireup="true"  EnableEventValidation="false" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" CodeBehind="UpLoadDocumentManager.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.UpLoadDocumentManager" %>

<%@ Register Src="~/UserControl/DocMutipleUpload.ascx" TagPrefix="uc1" TagName="DocMutipleUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.exedit-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script src="../Scripts/BusinessReport/UpLoadDocumentManager.js"></script>
    <link href="../Styles/css/NavStlye.css" type="text/css" rel="stylesheet" />

    <link href="../ProcessResource/css/wfStyle-201.88.css" rel="stylesheet" />


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="title">
        <a href="#"><span>文档编辑上传</span></a>
    </div>

    <div class="content_t_l">
        <div class="content_t_r"></div>
    </div>
    <div class="content_m" style="padding-left: 0px !important; padding-right: 0px !important;">
        <table style="width: 100%">
            <tr>
                <td style="width: 20%;" id="TreeTD">
                    <div class="leftCon" style="padding-top: 0px; margin: 0px;">
                        <div style="padding-top: 0" class="leftCon">
                            <div style="margin-top: 0; margin-bottom: 0px;" class="con">
                                <div class="menutree">
                                    <div style="border-bottom: 1px solid #deedf2; padding: 8px 0; display: none">

                                        <asp:DropDownList runat="server" ClientIDMode="Static" ID="ddlSystem" AutoPostBack="true" Style="width: 180px; display: none;" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged"></asp:DropDownList>

                                        <input type="text" id="TxtDocSearch" style="width: 45%; height: 22px; vertical-align: top; line-height: 22px; color: gray" value="输入文档名称" onfocus="if (value =='输入文档名称'){value ='';this.style.color='black'}" onblur="if (value ==''){value='输入文档名称'; this.style.color='gray'}" />
                                        <a class="btn_search" id="btnSearch" href="#" onclick="SearchData();"><span class="Search">查 询</span></a>

                                    </div>
                                    <div id="DivLeft" style="padding: 4px 0 0 4px; height: 544px; overflow: auto; overflow-x: hidden;">
                                        <ul id="DocTree" class="ztree_new">
                                        </ul>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </td>

                <td style="margin: 0; padding: 0">
                    <div class="rightCon" style="padding-top: 0px; margin: 0px;">
                        <div style="margin-top: 0; margin-bottom: 0px; text-align: left" class="con">


                            <div id="DivRight" style="padding: 4px 8px 0 8px; height: 90px; text-align: left; width:98%;  float: left">
                                <table class="tab_search">
                                    <tr>
                                        <th style="width:10%">文件类型:</th>
                                        <td style="width:10%">
                                              <asp:DropDownList ID="ValueA" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ValueA_SelectedIndexChanged" ></asp:DropDownList>
                                        </td>

                                        <th style="width:10%">产业集团:</th>
                                        <td style="width:10%">
                                            <asp:DropDownList ID="ValueB" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ValueB_SelectedIndexChanged">  <asp:ListItem Value="0" Selected="True">请选择</asp:ListItem></asp:DropDownList></td>

                                        <th style="width:10%">系统:</th>
                                        <td style="width:10%"><asp:DropDownList ID="ValueC" runat="server" ClientIDMode="Static" AutoPostBack="true"  OnSelectedIndexChanged="ValueC_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Selected="True">请选择</asp:ListItem>
                                            </asp:DropDownList>
                                            
                                           </td>
                                        <th style="width:10%">公司</th>
                                        <td style="width:10%">
                                            <asp:DropDownList ID="ValueD" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ValueD_SelectedIndexChanged">
                                                 <asp:ListItem Value="0" Selected="True">请选择</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                         <th style="width:10%">年份:</th>
                                        <td style="width:10%"> <asp:DropDownList ID="FinsYear" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="FinYear_SelectedIndexChanged"></asp:DropDownList>
                                        </td>
                                    </tr>

                                </table>
                            </div>
                        </div>
                    </div>

                    <div class="rightCon" style="padding-top: 0; margin: 0px;">
                        <div style="margin-top: 0; margin-bottom: 0px;" class="con">
                            <div class="menutree">

                                <div id="Div1" style="padding: 4px 8px 0 8px; height: 450px; overflow: auto; overflow-x: hidden;">
                                    <uc1:DocMutipleUpload runat="server" ID="DocMutipleUpload" ClientIDMode="Static" AttachmentType="文档分类上传" />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div class="content_b_l">
        <div class="content_b_r">
            <asp:HiddenField runat="server" ID="hideTerrNodeId" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="hidefile" ClientIDMode="Static" Value="0" /> 
        </div>
    </div>
</asp:Content>

