<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" MasterPageFile="~/SiteMasterPage/Empty.Master" Inherits="WebApplication.WebPortal.Public.Login" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="content">
    <div class="main">
        <div class="banner">
            <img src="/Images/ljth/62.jpg" alt="#">
        </div>
        <div class="form-part">

            <asp:TextBox runat="server" placeholder="用户名" ID="txtUserName" Text=""></asp:TextBox>
            <asp:TextBox runat="server" placeholder="密码" ID="txtUserPassword" Text="" TextMode="Password"></asp:TextBox>
            <select name="language">
                <option value="1">简体中文</option>
            </select>
            <div class="forget-pwd" style="display: none"><a href="#">忘记密码</a> </div>
            <asp:Button ID="btnLogin" runat="server" Text=" " OnClick="btnLogin_Click" />
        </div>
    </div>
</asp:Content>
