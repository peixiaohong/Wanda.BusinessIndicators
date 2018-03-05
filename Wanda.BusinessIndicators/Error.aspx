<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.Error" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>发生错误</title>
    <script>
        function extendInfo() {
            var h = document.getElementById("errorInfo").style.height;
            if (h == "0px") {
                h = "";
            } else {
                h = "0px";
            }
            document.getElementById("errorInfo").style.height = h;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <h4>错误
                </h4>
                <asp:Literal Text="" ID="ErrorTitle" runat="server" />
            </div>
            <div>
                <h4><a href="#" onclick="javascript:extendInfo();">详细信息</a></h4>
                <div id="errorInfo" style="height: 0px; overflow: hidden;">
                    <asp:Literal Text="" ID="ErrorInfo" runat="server" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
