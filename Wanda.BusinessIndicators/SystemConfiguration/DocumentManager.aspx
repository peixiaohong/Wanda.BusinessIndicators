<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" CodeBehind="DocumentManager.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.DocumentManager" %>

<%@ Register Src="~/UserControl/DocMutipleUpload.ascx" TagPrefix="uc1" TagName="DocMutipleUpload" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.exedit-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/DocumentTree.js"></script>
    <link href="../Styles/css/NavStlye.css" type="text/css" rel="stylesheet" />

    <link href="../ProcessResource/css/wfStyle-201.88.css" rel="stylesheet" />

    <style type="text/css">
        .Search {
            padding-left: 20px;
            display: block;
            background-image: url("../../images/search.png");
            background-attachment: scroll;
            background-repeat: no-repeat;
            background-position-x: 0px;
            background-position-y: 3px;
            background-color: transparent;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="title">
        <a href="#"><span>文档查询</span></a>
    </div>

    <div class="content_t_l">
        <div class="content_t_r"></div>
    </div>
    <div class="content_m" style="padding-left: 0px !important; padding-right: 0px !important;">
        <table style="width: 100%">
            <tr>
                <td style="width: auto; display: block" id="TreeTD">
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
                <td style="width: 0%">
                    <img src="../Images/images1/Left.png" id="HideTree" onclick="HideZTree()" title="隐藏树桩列表" style="display: none; cursor: pointer" />
                    <img src="../Images/images1/Right.png" id="ShowTree" style="cursor: pointer" onclick="ShowZTree()" title="显示树桩列表" />
                </td>
                <td style="margin: 0; padding: 0">
                    <div class="rightCon" style="padding-top: 0px; margin: 0px;">
                        <div style="margin-top: 0; margin-bottom: 0px; text-align: left" class="con">


                            <div id="DivRight" style="padding: 4px 8px 0 8px; height: 90px; text-align: left; width: 890px; float: left">
                                <div class="tabs_m" style="overflow: hidden">
                                    <ul id="Ul3">
                                    </ul>
                                </div>

                                <div style="padding-bottom: 10px; padding-top: 27px; text-align: left; display: inline;">

                                    <div style="border: 2px solid #ccc;border-bottom:0; min-height: 20px; padding-bottom: 5px; width: 950px; margin-top: 1px; top: 2px" id="DivSelect1">
                                    </div>

                                     <div style="border: 2px solid #ccc;border-bottom:0;border-top:0; min-height: 20px; padding-bottom: 5px; width: 950px; margin-top: 1px; top: 2px" id="DivSelect2">
                                    </div>
                                     <div style="border: 2px solid #ccc;border-bottom:0;border-top:0; min-height: 20px; padding-bottom: 5px; width: 950px; margin-top: 1px; top: 2px" id="DivSelect3">
                                    </div>
                                    <div id="DivYear" style="border: 2px solid #ccc; border-top:0; min-height: 20px; width: 950px">            
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="rightCon" style="padding-bottom: 5px; padding-top: 47px; margin-left: 8px; display: inline;">
                        <div id="Div4" style="padding-bottom: 1px;">
                            <input type="text" id="Text1" style="width: 790px; height: 22px; vertical-align: top; line-height: 22px; color: gray" value="输入文档名称" onfocus="if (value =='输入文档名称'){value ='';this.style.color='black'}" onclick="Click()" onblur="if (value ==''){value='输入文档名称'; this.style.color='gray'}" />
                            <a class="btn_search" id="A1" href="#" style="margin-left: 20px;color:#cb5c61;" onclick="SelectData();"><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
                        </div>
                    </div>
                    <div class="rightCon" style="padding-top: 0; margin: 0px;">
                        <div style="margin-top: 0; margin-bottom: 0px;" class="con">
                            <div class="menutree">

                                <div id="Div1" style="padding: 4px 8px 0 8px; height: 400px; overflow: auto; overflow-x: hidden;">
                                    <uc1:DocMutipleUpload runat="server" ID="DocMutipleUpload" AttachmentType="文档分类上传" />
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
        </div>
    </div>
</asp:Content>
