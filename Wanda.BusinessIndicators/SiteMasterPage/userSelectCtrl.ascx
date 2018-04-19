<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="userSelectCtrl.ascx.cs" Inherits="WebApplication.WebPortal.SiteMaster.userSelectCtrl" %>
<%--<link href="/Assets/vendors/userselect/css/common.userselect.ctrl<%=HttpContext.Current.Request.Browser.IsMobileDevice?".mobile":"" %>.css" rel="stylesheet" />
<script src="/Assets/vendors/userselect/js/common.userselect.ctrl<%=HttpContext.Current.Request.Browser.IsMobileDevice?".mobile":"" %>.js"></script>--%>



<link href="http://192.168.50.72:81/UserSelectService/css/bpf-sdk-core.css" rel="stylesheet" />
<link href="http://192.168.50.72:81/UserSelectService/css/bpf-userselect-<%=HttpContext.Current.Request.Browser.IsMobileDevice?"mobile":"pc" %>.css" rel="stylesheet" />
<script src="http://192.168.50.72:81/UserSelectService/js/bpf-sdk-core.js"></script>
<script src="http://192.168.50.72:81/UserSelectService/js/bpf-userselect-client.js?v=2"></script>
<script src="http://192.168.50.72:81/UserSelectService/js/bpf-userselect-<%=HttpContext.Current.Request.Browser.IsMobileDevice?"mobile":"pc" %>.js?v=2"></script>
<script src="http://192.168.50.72:81/UserSelectService/js/bpf-userselect-lang-zh-cn.js?v=2"></script>