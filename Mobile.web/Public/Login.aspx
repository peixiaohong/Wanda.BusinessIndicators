<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/Empty.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Mobile.web.Public.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
      <div class="main">
        <div class="banner">
            <img src="../Assets/images/62.jpg" alt="#"/>
        </div>
        <div class="form-part">

            <asp:TextBox runat="server" placeholder="用户名" ID="txtUserName" Text="zhengguilong"></asp:TextBox>
            <asp:TextBox runat="server" placeholder="密码" ID="txtUserPassword" Text="1" TextMode="Password"></asp:TextBox>
            <select name="language">
                <option value="1">简体中文</option>
            </select>
            <div class="forget-pwd" style="display: none"><a href="">忘记密码</a> </div>
            <asp:Button ID="btnLogin" runat="server" Text=" " OnClick="btnLogin_Click" />
        </div>
    </div>
</asp:Content>
