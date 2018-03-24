<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetList.aspx.cs" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master"   Inherits="LJTH.BusinessIndicators.Web.TargetConfiguration.TargetList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>


        function Add() {
            art.dialog({
                content: $("#divDetail").html(),
                lock: true,
                id: 'divDetail',
                title: '<span>添加指标</span>'
            });
        }
        function saveadd() {
            alert("添加成功!");
            art.dialog({ id: 'divDetail' }).close();
        }
        function Change(adj) {
            art.dialog({
                content: $("#div1").html(),
                lock: true,
                id: 'div1',
                title: '<span>编辑指标</span>'
            });
        }
        function savechange() {
            alert("编辑成功!");
            art.dialog({ id: 'div1' }).close();
        }


    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <div class="" style="text-align: center;">
              <asp:DropDownList runat="server" ID="ddlSystem" ClientIDMode="Static"  AutoPostBack="true" Style="width: 150px; height: 25px;"></asp:DropDownList>
            <div class="uploadify-button value" id="btnvalue" style="width: 80px; margin-top: 5px; float: right; padding: 0" onclick="Add()">
                <span>
                    <label class="DownExcelLabel" id="LabelDownload">添加</label></span>
            </div>
        </div>

    </div>

    <div>
        <table class="tab_005 value" style="margin-top: 5px">
            <thead>
                <tr>
                    <th style="width: 14%">指标名称</th>
                    <th style="width: 10%">是否上报</th>
                    <th style="width: 10%">是否考核</th>
                    <th style="width: 10%">是否含有明细项</th>


                    <th style="width: 10%">类型</th>
                    <th style="width: 8%">版本号</th>
                    <th style="width: 10%">基线</th>
                    <th style="width: 10%">单位</th>
                    <th style="width: 8%">排序</th>
                    <th style="width: 10%">操作</th>
                </tr>
            </thead>
            <tbody id="rows">
                <tr>
                    <td class="Td_Right"><a href="javascript:void(0);" onclick="Change(1)">收入</a> </td>
                    <td class="Td_Right">是</td>
                    <td class="Td_Right">是</td>
                    <td class="Td_Right">是</td>
                

                    <td class="Td_Right">收入类</td>
                    <td class="Td_Right">V1</td>
                    <td class="Td_Right">2015-04</td>
                        <td class="Td_Right">元</td>
                    <td class="Td_Right">1</td>
                    <td class="Td_Center"><a href="javascript:void(0);" onclick="">删除</a>&nbsp;&nbsp; <a href="javascript:void(0);" onclick="Change(1)">操作</a></td>
                </tr>
                <tr>
                    <td class="Td_Right"><a href="javascript:void(0);" onclick="Change(1)">利润</a> </td>
                    <td class="Td_Right">是</td>
                    <td class="Td_Right">否</td>
                    <td class="Td_Right">是</td>
                    

                    <td class="Td_Right">支出类</td>
                    <td class="Td_Right">V1</td>
                    <td class="Td_Right">2015-04</td>
                    <td class="Td_Right">元</td>
                    <td class="Td_Right">2</td>
                    <td class="Td_Center"><a href="javascript:void(0);" onclick="">删除</a>&nbsp;&nbsp; <a href="javascript:void(0);" onclick="Change(1)">操作</a></td>
                </tr>
                <tr>
                    <td class="Td_Right"><a href="javascript:void(0);" onclick="Change(2)">总收入</a></td>
                    <td class="Td_Right">是</td>
                    <td class="Td_Right">是</td>
                    <td class="Td_Right">是</td>
                    

                    <td class="Td_Right">收入类</td>
                    <td class="Td_Right">V2</td>
                    <td class="Td_Right">2015-04</td>
                    <td class="Td_Right">元</td>
                    <td class="Td_Right">3</td>
                    <td class="Td_Center"><a href="javascript:void(0);" onclick="">删除</a>&nbsp;&nbsp; <a href="javascript:void(0);" onclick="Change(2)">操作</a></td>
                </tr>
            </tbody>


        </table>
    </div>

    <div id="divDetail" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <table style="width: 600px;" class="tab02">
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">指标名称:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="name1">
                    <input style="width: 150px;" />
                </td>
            </tr>



            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">指标类型:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">
                        <option selected="selected">请选择</option>
                        <option>成本类</option>
                        <option>净利润类</option>
                        <option>收入类</option>
                        <option>其他</option>
                    </select>
                </td>
            </tr>
            <tr id="HOpenTime">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="Th1">是否考核:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">
                        <option selected="selected">请选择</option>
                        <option>是</option>
                        <option>否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否上报:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">
                        <option selected="selected">请选择</option>
                        <option>是</option>
                        <option>否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否含有明细项:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">
                        <option selected="selected">请选择</option>
                        <option>是</option>
                        <option>否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">单位:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td1">
                    <input style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">排序:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td2">
                    <input style="width: 150px;" />
                </td>
            </tr>
               <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">基线:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td7">
                    <input type="text" id="Text1" style="width: 150px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM' })" value="" />
                </td>
            </tr>
        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'divDetail' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="保存" id="savetrue" onclick="saveadd()" />
        </div>
    </div>
    <div id="div1" class="popup" style="height: auto; display: none; padding-bottom: 10px">
        <table style="width: 600px;" class="tab02">
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">指标名称:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td3">
                    <input style="width: 150px;" value="收入" />
                </td>
            </tr>



            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">指标类型:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">

                        <option>成本类</option>
                        <option>净利润类</option>
                        <option selected="selected">收入类</option>
                        <option>其他</option>
                    </select>
                </td>
            </tr>
            <tr id="Tr1">
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;" id="Th2">是否考核:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">

                        <option selected="selected">是</option>
                        <option>否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否上报:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">

                        <option selected="selected">是</option>
                        <option>否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">是否含有明细项:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;">
                    <select style="width: 150px;">

                        <option selected="selected">是</option>
                        <option>否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">单位:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td4">
                    <input style="width: 150px;" value="元" />
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">排序:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td5">
                    <input style="width: 150px;" value="1" />
                </td>
            </tr>
            <tr>
                <th style="width: 40%; padding: 5px; padding-right: 60px; text-align: right;">基线:</th>
                <td style="width: 60%; padding: 5px; padding-left: 20px; text-align: left;" id="Td6">
                    <input type="text" id="rpt_info_CommitDate" style="width: 150px;" class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy-MM' })" value="2015-03" />
                </td>
            </tr>
        </table>
        <div class="pop_operate">
            <input type="button" class="pop_btn btn_gray" value="取消" onclick="art.dialog({ id: 'div1' }).close();" />
            <input type="button" class="pop_btn btn_blue" value="保存" id="Button1" onclick="savechange()" />
        </div>
    </div>
</asp:Content>
