<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MonthReportMasterPage.Master" AutoEventWireup="true" CodeBehind="ComprehensiveReportForms.aspx.cs" Inherits="LJTH.BusinessIndicators.Web.BusinessReport.ComprehensiveReportForms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>


    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.all-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.core-3.5.min.js"></script>
    <script type="text/javascript" src="../Scripts/ztree/jquery.ztree.excheck-3.5.min.js"></script>

    <link href="/Styles/ztree/metroStyle/metroStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/BusinessReport/ComprehensiveReportForms.js"></script>
    <script type="text/javascript" src="../Scripts/multiple-select-master/multiple-select.js"></script>
    <link href="../Scripts/multiple-select-master/multiple-select.css" rel="stylesheet" />

    <script type="text/javascript">
        var TreeDataJson = <%=TreeDataJson%>;
    </script>

    <style type="text/css">
        .f {
            border: solid 1px #0463d0;
            width: 120px;
            height: 35px;
            position: relative;
            background-color: transparent;
        }

            .f:before {
                position: absolute;
                top: 0px;
                right: 0;
                left: 0;
                bottom: 0;
                border-bottom: 35px solid #fff;
                border-right: 348px solid transparent;
                content: "";
            }

            .f:after {
                position: absolute;
                left: 0;
                right: 1px;
                top: 1px;
                bottom: 0;
                border-bottom: 35px solid #0463d0;
                border-right: 348px solid transparent;
                content: "";
            }

        .h {
            width: 6%;
            height: 35px;
        }

    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main">

        <div class="title">

            <a href="#"><span>查询条件</span></a>
        </div>
        <div class="content_t_l">
            <div class="content_t_r"></div>
        </div>
        <div class="content_m">
            <table class="tab_search">
                <tbody>
                    <tr>
                        <th>系统名称</th>
                        <td>
                            <input type="text" id="TxtSystem" style="width: 210px;" onclick="showMenu();" />
                        </td>

                        <th>系统指标</th>
                        <td>
                            <select id="ddlTarget" multiple="multiple"></select>
                        </td>
                        <th>数据类型</th>
                        <td>
                            <select id="ddlDataType" style="width: 120px;">
                                <option value="1">实际数 </option>
                                <option value="2">指标数  </option>
                                <option value="3">未完成家数</option>
                                <option value="4">实际与指标数</option>
                            </select>
                        </td>
                        <th>累计/当月</th>
                        <td>
                            <select id="ddlCumulativ_Month" style="width: 120px;">
                                <option value="1">当月数</option>
                                <option value="2">累计数 </option>
                            </select>
                        </td>
                        <th>上报年份</th>
                        <td>
                            <select id="ddlYear" multiple="multiple">
                            </select>
                        </td>


                        <td style="display: none"></td>
                        <th>
                            <a class="btn_search" id="ContentPlaceHolder1_LinkButton1" href="#" onclick="SearchData();"><span>查询</span></a>
                        </th>
                    </tr>
                </tbody>
            </table>
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
                <div class="DownExcelDiv" id="DownExcel">
                    <span class="DownExcelspan">
                        <div class="uploadify-button" id="file_upload-button1" onclick="DownExcelReport('DownExcel');">
                            <span>
                                <label class="DownExcelLabel" id="LabelDownload">导出Excel报表</label></span>
                        </div>
                    </span>
                </div>

            </div>
        </div>
        <div class="tabs_content">
            <div class="scrolldoorFrame copy">
                <ul class="tabs" id="Ul4" style="border-bottom-color: #FFF; margin-top: 5px; height: auto; margin-bottom: 0px;">
                </ul>
                <table class="tab_005" id="CR_table" style="border: 1px solid black;">
                    <thead id="CR_head">
                        <tr>
                            <th colspan="3">
                                <div class="f"></div>
                            </th>

                            <th class="h">1月</th>
                            <th class="h">2月</th>
                            <th class="h">3月</th>
                            <th class="h">4月</th>
                            <th class="h">5月</th>
                            <th class="h">6月</th>
                            <th class="h">7月</th>
                            <th class="h">8月</th>
                            <th class="h">9月</th>
                            <th class="h">10月</th>
                            <th class="h">11月</th>
                            <th class="h">12月</th>
                            <th class="h">合计</th>
                        </tr>
                    </thead>
                    <tbody id="CR_body">
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">

            <textarea id="Remark" cols="55" rows="5"></textarea>

            <div class="pop_operate">
                <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
                <input type="button" class="pop_btn btn_blue" value="确定" onclick=" AddRemark();" id="savetrue" />
            </div>
        </div>
        <div id="div1" class="popup" style="height: auto; display: none; padding-bottom: 10px">

            <table style="width: 600px;" class="tab_005" id="tbEdit">
                <thead>
                    <tr>
                        <th style="width: 20%">操作人</th>
                        <th style="width: 20%">操作</th>
                        <th style="width: 20%">操作时间</th>
                        <th style="width: 40%">操作原因</th>
                    </tr>
                </thead>
                <tbody id="historyaction">
                </tbody>
            </table>
            <div class="pop_operate">
                <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'div1' }).close();" />
            </div>
        </div>
    <div id="menuContent" class="menuContent" style="display: none; position: absolute;">
        <ul id="SysTree" class="ztree_new" style="margin-top: 0; width: 200px; height: 350px; background-color: #fff; border: 1px solid #000; overflow-y: auto;"></ul>
    </div>

</asp:Content>
