<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="RoleManager.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.RoleManager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script  type="text/javascript" src="../Scripts/SystemConfiguration/RoleManager.js?ver=<%=new Random(DateTime.Now.Millisecond).Next(0,10000)%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
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
    </div>
</asp:Content>
