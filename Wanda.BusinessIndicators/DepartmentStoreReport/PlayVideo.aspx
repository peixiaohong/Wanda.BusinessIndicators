<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PlayVideo.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage_Help.Master" Inherits="Wanda.BusinessIndicators.Web.DepartmentStoreReport.PlayVideo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <%-- <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.exedit-3.5.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
    <script type="text/javascript" src="../Scripts/BusinessReport/DocumentTree.js"></script>--%>
  <%--  <link href="../Styles/css/NavStlye.css" type="text/css" rel="stylesheet" />--%>
    <link rel="stylesheet" href="assets/hivideo.css" />

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

    <style>
    .div{ width:100%;height:100% }
     #Content1 {
    height: 100%;
}
     #container { /* this is the div you want to fill the window */
    min-height: 100%;
}
     

      #vid{
          width:100%;height:590px;
      }
    </style>
  

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <div class="div">

<%--       <video ishivideo="true" autoplay="true" isrotate="false" autoHide="true">
    <source src="/Excel/Help/最新月报上报视频.mp4" type="video/mp4">
</video>--%>
        <video id="vid" controls="controls" style="margin-top:30px">
  <%--<source src="movie.ogg" type="video/ogg">--%>
  <source  src="../../Excel/Help/最新月报上报视频.mp4" type="video/mp4">
</video>


    </div>





</asp:Content>
