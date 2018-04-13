<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="RoleManager.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.RoleManager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script  type="text/javascript" src="../Scripts/SystemConfiguration/RoleManager.js?ver=<%=new Random(DateTime.Now.Millisecond).Next(0,10000)%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="">
            <div class="title">
                <a href="javascript:void(0)"><span>查询条件</span></a>
            </div>
            <!--title-->
            <div class="content_t_l">
                <div class="content_t_r"></div>
            </div>
            <div class="content_m">
                <table class="tab_search">
                    <tbody>
                        <tr>
                            <th>角色名称</th>
                            <td>
                                <input type="text"  style="width: 210px;outline:0;" id="QrName">
                            </td>
                            <th style="width: 150px">
                                <a  class="btn_search" href="javascript:void(0)" onclick="QueryRoleData()"><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
                                <a  class="btn_search" href="javascript:void(0)" onclick="SaveRole()"><span><i class="fa fa-plus"></i>&nbsp;添加</span></a>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="content_b_l">
                <div class="content_b_r"></div>
            </div>
        </div>
  
        <div class="margin_t10">
            <div class="tabs_m">
                <ul id="Ul3">
                    <li class="selected m_1"><a class="active_sub2"><span>查询结果</span></a></li>
                </ul>
            </div>
            <div class="tabs_content">
                <!--内容-->
                <div style="margin-top: 5px;">
                    <div class="scrolldoorFrame copy">
                        <table class="tab_005 role-list">
                            <thead style="width: 100%">
                                <tr class="tab_5_row_alt">      
                                    <th style="width: 5%;">序号</th>
                                    <th style="width: 25%;">角色名称</th>         
                                    <th style="width: 40%;">角色描述</th>                
                                    <th style="width: 10%">人员设置</th>         
                                    <th style="width: 10%">设置功能点</th>     
                                    <th style="width: 10%">操作</th>
                                </tr>    
                            </thead>
                            <tbody id="ShowMenuData">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
   <%-- <div>
        <div class="QueryConditions">
            <label>角色名称</label>
            <input type="text" />
            <input type="button" value="查询" />
            <input type="button" class="InsertRoleData_Show" value="新增" />
        </div>
        <div class="">
            <table  border="1" style=" border-collapse: collapse;">
                <thead>
                    <tr>
                        <td>序号</td>
                        <td >角色名称</td>
                        <td >角色描述</td>
                        <td >功能设置</td>
                        <td >人员设置</td>
                        <td >操作</td>
                    </tr>
                </thead>
                <tbody id="ShowMenuData">
                </tbody>
            </table>
        </div>
    </div>

    <div id="AddRole">
        <label>名称</label><input id="CnName" type="text" /><br />
        <label>描述</label><input id="Description" type="text" /><br />
        <input type="button" class="InsertRoleData_OK" value="确定" />
        <input type="button" value="取消" />
    </div>--%>
</asp:Content>
