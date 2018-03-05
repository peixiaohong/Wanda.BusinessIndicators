<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetConfiguration.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.TargetConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Scripts/Layer/skin/layer.css" rel="stylesheet" />
    <link href="../Scripts/Layer/skin/layer.ext.css" rel="stylesheet" />
    <script src="../Scripts/Layer/layer.min.js"></script>
    <script src="../Scripts/Layer/extend/layer.ext.js"></script>
    <script type="text/javascript" src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script src="../Scripts/BusinessReport/TargetConfiguration.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">
           <select runat="server" clientidmode="Static" style="width:150px;height:25px" id="ddlSystem" onchange="reload()"></select>
        </div>
      
    </div>
    <div id="margin_t10">
<%--        <div class="uploadify-button value" id="btnvalue" style="width: 80px; margin-right: 20px; float: right; padding: 0" onclick="Add()">
            <span>
                <label class="DownExcelLabel" id="LabelDownload">添加</label></span>
        </div>--%>
         <div class="uploadify-button" id="Div2" style="width: 80px;float: right;margin-right:20px; padding: 0" onclick="ExceptionChange()" >
                <span>
                    <label class="DownExcelLabel" id="Label1" >指标管理</label></span>
            </div>
        <table class="tab_005" style="margin-top: 5px">
            <thead>
                <tr>
                    <th style="width: 14%">指标名称</th>
                    <th style="width: 11%">是否上报</th>
                    <th style="width: 11%">是否考核</th>
                    <th style="width: 11%">是否含有明细项</th>
                    <th style="width: 11%">类型</th>
                    <th style="width: 11%">基线</th>
                    <th style="width: 11%">单位</th>
                    <th style="width: 9%">排序</th>
                    <th style="width: 11%">操作</th>
                </tr>
            </thead>
            <tbody id="rows">
            </tbody>
        </table>
    </div>

    <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <table style="width: 600px;" class="tab02">
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">指标名称：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" style="width: 180px;" id="addname" onkeyup="value=value.replace(//s/g,'')" onkeydown="if(event.keyCode==32) return false"  />&nbsp;<a style="color:red">*</a>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否上报：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 182px" id="IfReport">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否考核：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td2">
                    <select style="width: 182px" id="IfEva">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否含有明细项：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td3">
                    <select style="width: 182px" id="IfDetail">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>
               <%--    <tr>
             <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">月度计划指标是否可修改：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td1">
                    <select style="width: 182px" id="Select1">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>
                 <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">累计计划指标是否可修改：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td4">
                    <select style="width: 182px" id="Select2">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>
                 <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否特殊处理差额：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td6">
                    <select style="width: 182px" id="Select3">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>--%>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">类型：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 182px" id="Enums">
                    </select>&nbsp;<a style="color:red">*</a>
                </td>
            </tr>
            <tr id="HOpenTime">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="Th1">基线：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" id="rpt_info_BaseLine" runat="server" clientidmode="Static" style="width: 180px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" />
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">单位：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td5">
                    <input type="text" style="width: 180px;" id="Unit" onkeyup="value=value.replace(//s/g,'')" onkeydown="if(event.keyCode==32) return false"  />
                </td>
            </tr>



            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">排序值：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" style="width: 180px;" id="seq" onkeyup="this.value=this.value.replace(/\D/g,'')" onafterpaste="this.value=this.value.replace(/\D/g,'')" />&nbsp;<a style="color:red">*</a>
                </td>
            </tr>
        <%--       <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否需要阀值：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                     <select style="width: 100px" id="HaveKpi" onchange="ShowKpi()">
                        <option value="1"  >是</option>
                        <option value="0" selected="selected">否</option>
                    </select>
                    <input type="text" style="width: 150px;" id="Kpi" onkeyup="this.value=this.value.replace(/\D/g,'')" onafterpaste="this.value=this.value.replace(/\D/g,'')" value="1" />
                </td>
            </tr>--%>

        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" SaveTarget();" id="savetrue" />
           <%-- <input type="button" class="pop_btn btn_blue" value="确定" onclick=" AddsTarget();" id="addtrue" />--%>
        </div>
    </div>
     <input type="hidden" runat="server" id="SysID" clientidmode="Static" />
</asp:Content>
