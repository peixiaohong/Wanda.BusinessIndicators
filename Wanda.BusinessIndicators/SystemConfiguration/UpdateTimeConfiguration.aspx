<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateTimeConfiguration.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" Inherits="LJTH.BusinessIndicators.Web.SystemConfiguration.UpdateTimeConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/BusinessReport/TimeConfiguration.js"></script>
    <style type="text/css">
        .auto-style1 {
            width: 10%;
        }

        .auto-style2 {
            height: 70px;
            width: 22%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main">
        <div class="">
            <div class="title">
                <a href="#"><span>上报月调整</span></a>
            </div>
            <!--title-->
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div style="text-align: center">
                <br />
                <br />
                <br />
                <br />
                <br />
                <table class="tab_search" style="width: 45%; text-align: center; border: solid; margin: auto; border-color: #aaa">
                    <tbody>
                        <tr style="height: 60px">
                            <th class="auto-style1"></th>
                            <th></th>
                        </tr>
                        <tr>
                            <th class="auto-style1"></th>
                            <th style="width: 85%; text-align: left"></th>
                        </tr>
                        <tr  >
                            <th >&nbsp;</th>
                            <th style="width: 85%; text-align: left;padding:10px;">
                                <input type="radio" name="btu" id="OpenDayRadio" value="1"  onclick="ClickChange(this)" title="预计到某日开放" />
                                上报月为<input type="text" id="ReportDay" style="width: 100px; margin-right: 10px"  class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM', minDate: '%y-%M' })" />
                                预计到<input type="text" id="OpenDayTxt" style="width: 100px;"  class="Wdate" onclick="WdatePicker({ dateFmt:'yyyy-MM-dd'})" />日开放
                               <%-- <a  id="closeTips1" style="color: red; text-decoration: none">上报月为：自然月的下一个月</a>--%>
                            </th>
                        </tr>
                        <tr >
                            <th >&nbsp;</th>
                            <th style="width: 85%; text-align: left; padding:10px; ">
                                <input type="radio" name="btu" id="opentadio" value="2" onclick="ClickChange(this)"  title="请选择日期!" />立即开放
                                <input type="text" id="OpenDate" style="width: 150px; margin-left: 20px"  class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM'})" />
                                <a  id="closeTips2" style="color: #9d2328; text-decoration: none">上报月为：日期控件选择的月份!</a>
                                </th>
                        </tr>
                        <tr >
                            <th ></th>
                            <th style="width: 85%; text-align: left;padding:10px;">
                                <input type="radio" name="btu" id="closeradio" value="3" onclick="ClickChange(this)" title="默认为自然月的上个月" /> 关闭
                                <a  id="closeTips3" style="color: #9d2328; text-decoration: none">上报月为：自然月的上个月!</a>
                            </th>
                        </tr>

                        <tr>
                            <td ></td>
                            <td style="float:none" >
                                <input type="button" class="uploadify-button" value="保存" style="width: 80px; text-align: center; height: auto; margin-left: 40%; margin-top: 20px" onclick="Save()" />
                            </td>

                        </tr>
                       <tr>
                           <td colspan="2" style="padding:30px" ></td>
                       </tr>
                    </tbody>
                </table>
            </div>

        </div>
    </div>

</asp:Content>
