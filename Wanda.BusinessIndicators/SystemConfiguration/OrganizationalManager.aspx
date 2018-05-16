<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="OrganizationalManager.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.OrganizationalManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../Scripts/SystemConfiguration/OrganizationalManager.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main clear">
        <div>
            <div style="width: 20%; min-height: 400px; float: left">
                <div class="margin_t10">
                    <div class="tabs_m">
                        <ul>
                            <li class="selected m_1"><a class="active_sub2"><span>组织架构</span></a></li>
                        </ul>
                    </div>
                    <div class="tabs_content user-content" style="height: auto">
                        <!--内容-->
                        <div style="margin-top: 5px;">
                            <ul id="tree" class="ztree ztree_defined"></ul>
                        </div>
                    </div>
                </div>
            </div>
            <div id="rMenu">
                <ul>
                    <li id="m_add" onclick="CheckOrganization('Add','company')">新增区域</li>
                    <li id="m_addC" onclick="CheckOrganization('Add')">新增项目</li>
                    <li id="m_edit" onclick="CheckOrganization('Edit','company')">修改</li>
                    <li id="m_del" onclick="CheckOrganization('Delete','company')">删除</li>
                </ul>
            </div>
            <div style="width: 75%; min-height: 400px; float: left; margin-left: 30px; display: none" class="organization-box">
                <div class="select_content" style="margin-top:15px">
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
                                        <th>项目名称</th>
                                        <td>
                                            <input type="text" style="width: 210px; outline: 0;" id="companyName">
                                        </td>
                                        <th style="width: 200px">
                                            <a class="btn_search company_search" href="javascript:void(0)"><span><i class="fa fa-search"></i>&nbsp;查询</span></a>
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
                                                <th style="width: 10%;"></th>
                                                <th style="width: 10%;">序号</th>
                                                <th style="width: 80%;">项目名称</th>
                                            </tr>
                                        </thead>
                                        <tbody id="CompanyMenuData">
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="padding-bottom: 30px; padding-top: 10px;">
                        <div id="pager" style='text-align: right; font-size: 12px; float: right;'></div>
                    </div>
                     <div class="btnbox">
                        <input class="btn_ok company_sumbit" type="button" value="确定">
                    </div>
                </div>
                <div class="margin_t10 organization_content">
                    <div class="tabs_m">
                        <ul>
                            <li class="selected m_1"><a class="active_sub2"><span class="organization-title">组织名称</span></a></li>
                        </ul>
                    </div>
                    <div class="tabs_content user-content" style="height: auto">
                        <!--内容-->
                        <div style="margin-top: 5px;">
                            <div class="ztree_defined">
                                <div>
                                    <div class="organization_name">
                                        <span>板块名称:</span>
                                        <input type="text" name="name" value="" readonly="readonly" />
                                    </div>
                                    <ul class="organization_edit">
                                        <li>
                                            <span>组织名称:</span>
                                            <input type="text" name="name" value="" class="organization_edit_name" />
                                        </li>
                                        <li class="btnbox">
                                            <input class="btn_ok add_submit" type="button" value="确定">
                                            <input class="btn_ok edit_submit" type="button" value="确定">
                                        </li>
                                    </ul>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
