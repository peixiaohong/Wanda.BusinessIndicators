<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="MXLTest.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.MXLTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function XMLTranslation() {
            var xml = $("#returnxml").val();
            if (xml != null && xml!="") {
                xml = xml.replace(/\&amp;/g, "&");
                xml = xml.replace(/\&lt;/g, "<"); 
                xml = xml.replace(/\&gt;/g, ">");
                xml = xml.replace(/\&quot;/g, "\"");
                xml = xml.replace(/\&apos;/g, "'");
            }
            $("#returnxml").val(xml);
        }
        function StringTranslation() {
            var xml = $("#returnxml").val();
            var re = /&/g;
            if (xml != null && xml != "") {
                xml = xml.replace(re, "&");
                xml = xml.replace(/\</g, "&lt;");
                xml = xml.replace(/\>/g, "&gt;");
                xml = xml.replace(/\"/g, "&quot;");
                xml = xml.replace(/\'/g, "&apos;");
            }
            $("#returnxml").val(xml);
        }
  
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main" style="text-align: center">
        <div class="" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">
            <asp:DropDownList runat="server" ID="ddlSystem" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged" ClientIDMode="Static" AutoPostBack="true" Style="width: 150px; height: 25px;"></asp:DropDownList>
            <span style="color:red" id="romerror" runat="server" clientidmode="Static"></span>
        </div>
        <div style="text-align: center">
            <span style="text-align:center"></span>
            <textarea cols="200" rows="28" id="returnxml" runat="server"  clientidmode="Static"></textarea>
          
        </div>
        <span style="color:red" id="texterror" runat="server" clientidmode="Static"></span>
        <div style="text-align: center">
            <input style="display:none" type="button" class="uploadify-button" value="转义为字符串" onclick="XMLTranslation()" />
            <input style="display:none" type="button" class="uploadify-button" value="转义为XML" onclick="StringTranslation()" />
              <asp:Button ID="Button1" runat="server" ClientIDMode="Static" CssClass="uploadify-button" OnClick="Unnamed1_Click" Text="验证" />
            <asp:Button ID="Save" runat="server" ClientIDMode="Static"  CssClass="uploadify-button" OnClick="Save_Click" Text="保存" />
        </div>
    </div>
</asp:Content>
