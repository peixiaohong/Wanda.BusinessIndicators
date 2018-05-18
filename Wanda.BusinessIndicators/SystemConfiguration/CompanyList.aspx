<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyList.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="LJTH.BusinessIndicators.Web.CompanyList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"> 

    <link href="../Scripts/Layer/skin/layer.css" rel="stylesheet" />
    <link href="../Scripts/Layer/skin/layer.ext.css" rel="stylesheet" />
    <script src="../Scripts/Layer/layer.min.js"></script>
    <script src="../Scripts/Layer/extend/layer.ext.js"></script>
  
    <script type="text/javascript" src="../Scripts/BusinessReport/CompanyList.js"></script>
    <script type="text/javascript" src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Scripts/UpLoad/jquery.uploadify.min.js"></script>
    <script type="text/C#">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="" style="border-bottom-color: #ececec; text-align: center; border-bottom-style: solid;">
            <asp:DropDownList runat="server" ID="ddlSystem" ClientIDMode="Static" AutoPostBack="true" Style="width: 150px; height: 25px;"></asp:DropDownList>

        </div>
        <div>
        </div>
    </div>

    <div id="margin_t10">
        <div class="tabs_m">
            <table style="width: 99%">
                <tr>
                    <td style="width: auto">
                        <ul class="tabs" id="tabhead" style="MARGIN-BOTTOM: 0px; HEIGHT: auto; BORDER-BOTTOM-COLOR: #fff; MARGIN-TOP: 5px">
                            <%-- <li class="sd" id="li1">
                                <a class="active3 active_sub3" onclick="ChangeTab(1)" id="a1" runat="server" clientidmode="Static">2</a>
                            </li>
                            <li class="sd" id="li2">
                                <a class="active3" id="a2" onclick="ChangeTab(2)" runat="server" clientidmode="Static">3</a>
                            </li>
                            <li class="sd" id="li3">
                                <a class="active3" id="a3" onclick="ChangeTab(3)" runat="server" clientidmode="Static">4</a>
                                <input type="hidden" runat="server" clientidmode="Static" id="Lihidden" />
                            </li>
                            <li class="sd" id="li4">
                                <a class="active3" id="a4" onclick="ChangeTab(4)" runat="server" clientidmode="Static">5</a>
                            </li>--%>
                        </ul>
                    </td>
                    <td style="width: auto; float: right">
                           <div class="uploadify-button" style="width: 80px; display: inline; float: right; padding: 0" id="Div2" onclick="TargetConfig();">
                            <span>
                                <label class="DownExcelLabel" id="Label2">指标管理</label></span>
                        </div>
                        <div class="uploadify-button" style="width: 80px; display: inline; float: right; padding: 0;margin-right:13px" id="file_upload-button1" onclick="AddCompany();">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">添加公司</label></span>
                        </div>
                        <div class="uploadify-button" style="width: 80px; float: right; display: inline; margin-right: 13px" id="Div1" onclick="DownExcel()">
                            <span>
                                <label class="DownExcelLabel" id="Label1">导出数据</label></span>
                        </div>

                        <div style="width: 80px; float: right; display: inline; padding-right: 25px;margin-right:13px;" id="UpLoadData">
                            <span>
                                <span>
                                    <input type="file" name="upload" id="file1" value="导入数据" />
                                </span>
                            </span>
                        </div>

                    </td>
                </tr>
            </table>

        </div>
        <div id="tab">
            
        </div>
    </div>


    <input type="hidden" runat="server" clientidmode="Static" id="HiddenSystemJson" />


    <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <table style="width: 600px;" class="tab02" id="tbDetail">
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">公司名称：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="name1">
                    <input type="text" style="width: 180px;" id="addname" onkeyup="value=value.replace(//s/g,'')" onkeydown="if(event.keyCode==32) return false" />
                </td>
            </tr>
            <tr id="HCompanyProperty1" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty1"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty1" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty2" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty2"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty2" style="width: 180px;"></select>
                    <input id="CompanyProperty2input"onkeyup="value=value.replace(//s/g,'')" onkeydown="if(event.keyCode==32) return false" style="display:none;width:180px" />
                </td>
            </tr>
            <tr id="HCompanyProperty3" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty3"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty3" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty4" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty4"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty4" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty5" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty5"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty5" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty6" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty6"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty6" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty7" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty7"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty7" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty8" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty8"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty8" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HCompanyProperty9" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NCompanyProperty9"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TCompanyProperty9" style="width: 180px;"></select>
                </td>
            </tr>

            <tr id="HOpenTime">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="Th1">开业时间：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" id="rpt_info_CommitDate" runat="server" clientidmode="Static" style="width: 150px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" />
                </td>
            </tr>
               
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">排序值：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" style="width: 50px;" id="seq" onkeyup="this.value=this.value.replace(/\D/g,'')" onafterpaste="this.value=this.value.replace(/\D/g,'')" />
                </td>
            </tr>
            <tr><th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">所属指标：</th>

                 <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="TargetList">
                    
                </td>
            </tr>
        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />

            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" AddsCompany();" id="addtrue" />
        </div>
    </div>


     <div id="EditDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <table style="width: 600px;" class="tab02" id ="tbEdit">
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">公司名称：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td1">
                    <input type="text" style="width: 180px;" id="EditName" onkeyup="value=value.replace(//s/g,'')" onkeydown="if(event.keyCode==32) return false" />
                </td>
            </tr>
            <tr id="HBCompanyProperty1" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty1"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty1" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty2" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty2"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty2" style="width: 180px;"></select>
                    <input id="BCompanyProperty2input"onkeyup="value=value.replace(//s/g,'')" onkeydown="if(event.keyCode==32) return false" style="display:none;width:180px" />
                </td>
            </tr>
            <tr id="HBCompanyProperty3" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty3"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty3" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty4" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty4"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty4" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty5" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty5"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty5" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty6" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty6"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty6" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty7" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty7"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty7" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty8" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty8"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty8" style="width: 180px;"></select>
                </td>
            </tr>
            <tr id="HBCompanyProperty9" style="display: none">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="NBCompanyProperty9"></th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select id="TBCompanyProperty9" style="width: 180px;"></select>
                </td>
            </tr>

            <tr id="HBCompanyProperty10">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="ThTime">开业时间：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" id="Selecttime" runat="server" clientidmode="Static" style="width: 150px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd' })" value="" />
                </td>
            </tr>
               
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">排序值：</th>
                <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <input type="text" style="width: 50px;" id="ThSeq" onkeyup="this.value=this.value.replace(/\D/g,'')" onafterpaste="this.value=this.value.replace(/\D/g,'')" />
                </td>
            </tr>
            <tr><th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">所属指标：</th>

                 <td colspan="3" style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="ThTar">
                    
                </td>
            </tr>
        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'EditDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="确定" onclick=" SaveCompany();" id="Button1" />
        </div>
    </div>
        <input type="hidden" runat="server" id="SysID" clientidmode="Static" />
</asp:Content>
