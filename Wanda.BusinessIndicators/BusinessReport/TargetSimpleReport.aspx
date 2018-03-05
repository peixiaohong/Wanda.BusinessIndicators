<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="TargetSimpleReport.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.ProtoType.TargetSimpleReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../Scripts/UpLoad/uploadify.css" />
    <link href="../styles/mini.css" rel="stylesheet" />
    <link href="../Styles/popup.css" rel="stylesheet" />
    <link href="../Styles/simplereport.css" rel="stylesheet" />
    <%--<script type="text/javascript" src="../Scripts/jquery.tmpl.js"></script>--%>
    <script type="text/javascript">
        $(document).ready(function () {
            SeachSystem();
            BindSystem();
            
        });
        //绑定上报系统
        function BindSystem() {
            WebUtil.ajax({
                url: "/TargetSimpleReportController/GetSystemListByConfigID",
                args: { cid: $("#Syscategory option:selected").val() },
                successReturn: function (rlist) {
                    if (rlist != undefined && rlist.length > 0) {
                        $("#Select1SYs").empty();
                        $("#Select1SYs").show();
                        $("#txtSystem").val("");
                        $("#hid_systemid").val("");
                        for (var i = 0; i < rlist.length; i++) {
                            $("#Select1SYs").append("<option value='" + rlist[i].ID + "'>" + rlist[i].SystemName + "</option>");
                        }
                    }
                }
            });
            $("#txtSystem").MultDropList("Select1SYs", "s", null, false);
        }
        var O = new Object();
        //查询
        function SeachSystem() {
            var ids = $("#hid_systemid").val();
            if (ids == "") {
                //alert("请选择上报系统！");
            }
            else {
                var List = new Array();
                var arr = ids.split(",");
                for (var i = 0; i < arr.length; i++) {
                    var obj = new Object();
                    obj.Index = i + 1;
                    obj.ID = arr[i];
                    obj.Name = $("#Select1SYs option[value='" + arr[i] + "']").text();
                    obj.Time = $("#CheckYear option:selected").val() + "-" + $("#CheckMonth option:selected").val();
                    obj.Month = $("#CheckMonth option:selected").val();
                    obj.Year = $("#CheckYear option:selected").val();
                    List.push(obj);
                }
                O = new Object();
                O.List = List;
                $("#tbody_item").empty();
                $("#item_info_tmpl").tmpl(O).appendTo($("#tbody_item"));
            }
        }
        //下载数据
        function DownExcel() {
            if (O.List == undefined) {
                alert("当前无数据！");
            }
            else {
                var SimpleSearh="";
                for (var i = 0; i < O.List.length; i++)
                {
                    O.List[i].Unit = $("#list_tr_" + (i + 1)).find("td:last").find("input:checked").val();
                    SimpleSearh = SimpleSearh+O.List[i].ID + ":" + $("#list_tr_" + (i + 1)).find("td:last").find("input:checked").val() + ","
                }
                window.open("/AjaxHander/DownLoadView.ashx?FileType=TargetSimpleRpt&FinYear=" + $("#CheckYear option:selected").val() + "&FinMonth=" + $("#CheckMonth option:selected").val() + "&Simple=" + SimpleSearh);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="Pagebox" style="min-height: 550px; _height: 550px;">
        <div id="mainRight">
            <div class="preview" style="min-height: 500px;">
                <div class="searchDiv" id="searchDiv" style="position: relative; margin-left: 0px; margin-right: 0px; top: 0px;">
                    <ul class="scrollUl" style="background-color: rgb(72, 147, 249);">
                        <li class="select"><a href="#"><span>查询条件</span></a></li>
                    </ul>
                    <table class="tab_search" style="border: 1px solid #ccc;min-width:1000px;">
                        <tbody>
                            <tr>
                                <th>系统范围</th>
                                <td>
                                    <asp:DropDownList runat="server" ID="Syscategory" DataTextField="Biz_Value" DataValueField="ID" ClientIDMode="Static" onchange="BindSystem();"></asp:DropDownList>
                                </td>
                                <th>上报系统</th>
                                <td>
                                    <input type="text" id="txtSystem" style="width: 216px;" />
                                    <input id="hid_systemid" type="hidden" value="" />
                                    <select id="Select1SYs" style="width: 220px; height: 100px; display:none">
                                    </select>
                                </td>
                                <th>考核年份</th>
                                <td>
                                    <asp:DropDownList runat="server" ID="CheckYear" ClientIDMode="Static" Style="width: 120px;">
                                    </asp:DropDownList>

                                </td>
                                <th>考核月份</th>
                                <td>
                                    <asp:DropDownList runat="server" ID="CheckMonth" ClientIDMode="Static" Style="width: 120px;">
                                    </asp:DropDownList>
                                </td>
                                <th>
                                    <div style="padding-bottom: 5px; text-align: right; padding-top: 5px; padding-left: 15px; padding-right: 25px"><a class="btn_search" id="link_search" href="javascript:void(0)" onclick="SeachSystem();"><span>查询</span></a> </div>
                                </th>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cls"></div>

                </div>
                <div style="border-bottom-color: #0869f6; border-bottom-style: solid; float: initial; display: none;">
                    <div class="content_title1_Left" style="width: 100%; height: 20px;">
                        <span>简报
                        </span>
                    </div>

                </div>
                <!--导出月度经营报告 -->
                <div class="searchDiv" style="padding-top: 5px; padding-bottom: 5px; position: relative; top: 5px;">
                    <span style="font-size: 13px; height: 30px; line-height: 60px; display: block; position: absolute; top: 0px; right: 0; z-index: 100;">
                        <div class="uploadify-button " id="file_upload-button1" style="height: 25px; line-height: 25px; text-indent: 0px; width: auto; padding: 0px 5px 0px 5px; cursor: pointer;" onclick="DownExcel();">
                            <span class="uploadify-button-text">
                                <label id="LabelDownload">导出PPT版简报</label></span>
                        </div>
                    </span>
                </div>

                <!--导出月度经营报告 -->
                <!--简报开始-->
                <div class="searchDiv" style="padding-top: 5px; margin-left: 0px; margin-right: 0px; margin-top: 20px;min-width:800px;">
                    <div class="scrolldoorFrame copy">
                        <table id="table1" class="tab_5 table_tree" id="currentBudget" style="table-layout: fixed">
                            <thead>
                                <tr>
                                    <th style="width: 5%">序号</th>
                                    <th style="width: 45%">报告主体</th>
                                    <th style="width: 20%">报告期间</th>
                                    <th style="width: 20%">报告单位</th>
                                </tr>
                            </thead>
                            <tbody id="tbody_item">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--简报结束-->
            </div>
        </div>
    </div>
    <script type="text/x-jquery-tmpl" id="item_info_tmpl">
        {{each List}}
        <tr class="Level3" style="height: 35px;" id="list_tr_${Index}">
            <td style="text-align: left; text-align: center">${Index}</td>
            <td style="text-align: left; padding-left: 20px">
                <input type="hidden" value="${ID}" />
                <label>${Name}</label>
            </td>

            <td style="text-overflow: ellipsis; white-space: nowrap; overflow: hidden; text-align: center">${Time}</td>
            <td style="text-overflow: ellipsis; white-space: nowrap; overflow: hidden; text-align: center">
                <input type="radio" id="millon_${Index}" name="unit_${Index}" value="万元" />
                <label for="millon_${Index}">万元</label>&nbsp;
                                                <input type="radio" checked="checked" id="billion_${Index}" name="unit_${Index}" value="亿元" />
                <label for="billion_${Index}">亿元</label>&nbsp;
            </td>
        </tr>
        {{/each}}
    </script>
</asp:Content>
