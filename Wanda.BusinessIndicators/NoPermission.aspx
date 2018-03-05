<%@ Page Language="C#"  MasterPageFile="~/SiteMasterPage/MainMasterPage.Master"  AutoEventWireup="true"   CodeBehind="NoPermission.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.NoPermission"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align:center;margin-top:100px;">
       <h1 style="font-size:28px;">对不起！您没有权限,请与管理员联系</h1>
    </div>

</asp:Content>