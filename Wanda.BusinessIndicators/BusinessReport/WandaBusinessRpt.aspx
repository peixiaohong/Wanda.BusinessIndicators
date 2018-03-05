<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="WandaBusinessRpt.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.BusinessReport.WandaBusinessRpt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
         <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>
     <script type="text/javascript" src="../Scripts/BusinessReport/FixedRpt.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main">

        <div class="title">

            <a href="#"><span>查询条件</span></a>
        </div>
        <div class="content_t_l">
            <div class="content_t_r"></div>
        </div>
        <div class="content_m" style="overflow: hidden">
            <div style="float: left; width: 30%;">
                <span>上报年</span>
                <select id="ddlYears" style="width: 120px;">
                </select>
            </div>
            <div style="width: 30%; float: left">
                <span>上报月份</span>

                <select class="DownExcelDiv" style="width: 100px; margin-left: 20px;" id="ddlMonths">
                    <option value="1">1月</option>
                    <option value="2">2月</option>
                    <option value="3">3月</option>
                    <option value="4">4月</option>
                    <option value="5">5月</option>
                    <option value="6">6月</option>
                    <option value="7">7月</option>
                    <option value="8">8月</option>
                    <option value="9">9月</option>
                    <option value="10">10月</option>
                    <option value="11">11月</option>
                    <option value="12">12月</option>
                </select>
            </div>
            <div class="DownExcelDiv" id="DownBusiness" style="width: 200px; float: left;">
                <span class="DownExcelspan">
                    <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport('Business');" style="margin-bottom: 5px; position: relative; top: -5px;">
                        <span>
                            <label class="DownExcelLabel" id="LabelDownload">导出万达商业整体固定报表</label>

                        </span>
                    </div>
                </span>
            </div>

        </div>
        <div class="content_b_l">
            <div class="content_b_r"></div>
        </div>
        <!--明细项 开始-->
        <!--明细项 开始-->
        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2"><span>综合查询</span></a></li>
                </ul>

            </div>
        </div>
        <div class="tabs_content">
            <div class="scrolldoorFrame copy">
                <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                </ul>
            </div>
        </div>
    </div>


</asp:Content>
