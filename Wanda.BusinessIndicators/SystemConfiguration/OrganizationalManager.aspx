<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="OrganizationalManager.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.OrganizationalManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../Scripts/SystemConfiguration/OrganizationalManager.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="main clear">
        <div>
            <div style="width: 20%; height: 400px; float: left">
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
		            <li id="m_add" onclick="CheckOrganization('Add')">新增区域</li>
		            <li id="m_addC" onclick="CheckOrganization('Add','company')">新增项目</li>
		            <li id="m_edit" onclick="CheckOrganization('Edit','company')">修改</li>
		            <li id="m_del" onclick="CheckOrganization('Delete','company')">删除</li>
	            </ul>
            </div>
            <div style="width: 75%; height: 400px; float: left; margin-left: 30px;display:none" class="organization-box">
                <div class="margin_t10">
                    <div class="tabs_m">
                        <ul>
                            <li class="selected m_1"><a class="active_sub2"><span class="organization-title">组织名称</span></a></li>
                        </ul>
                    </div>
                    <div class="tabs_content user-content" style="height: auto">
                        <!--内容-->
                        <div style="margin-top: 5px;">
                            <div class="select_content">123</div>
                            <div class="ztree_defined organization_content">
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
