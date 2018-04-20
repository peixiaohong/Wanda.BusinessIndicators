<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="UserInfoManager.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.UserInfoManager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/SystemConfiguration/UserInfoManager.js"></script>
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
                            <th>关键字</th>
                            <td>
                                <input type="text"  style="width: 210px;outline:0;" id="UsersName">
                            </td>
                            <th style="width: 200px">
                                <a  class="btn_search QueryUsers_Button" href="javascript:void(0)"><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
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
                                    <th style="width: 10%;">员工名称</th>         
                                    <th style="width: 10%;">用户名称</th>                
                                    <th style="width: 10%">岗位</th>         
                                    <th style="width: 40%">部门</th>     
                                     <th style="width: 10%">所属角色</th>  
                                     <th style="width: 10%">所属架构</th>     
                                </tr>    
                            </thead>
                            <tbody id="UsersMenuData">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="user-model">
        <div class="main user-main">
        <div>
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
                                <input type="text"  style="width: 210px;outline:0;" id="UsersRoleName">
                            </td>
                            <th style="width: 150px">
                                <a  class="btn_search QueryConditions_Button" href="javascript:void(0)"><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
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
                <ul>
                    <li class="selected m_1"><a class="active_sub2"><span>查询结果</span></a></li>
                </ul>
            </div>
            <div class="tabs_content user-content">
                <!--内容-->
                <div style="margin-top: 5px;">
                    <div class="scrolldoorFrame copy">
                        <table class="tab_005 role-list">
                            <thead style="width: 100%">
                                <tr class="tab_5_row_alt">      
                                    <th style="width: 5%;">序号</th>
                                    <th style="width: 20%;">角色名称</th>         
                                    <th style="width: 20%;">角色描述</th>                 
                                </tr>    
                            </thead>
                            <tbody id="UsersRolesData">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="btnbox">
                <input class="btn_ok set_role_sumbit" type="button" value="确定">
                <input class="btn_no set_role_cancel" type="button" value="取消">

            </div>
        </div>
    </div>
    </div>
</asp:Content>

