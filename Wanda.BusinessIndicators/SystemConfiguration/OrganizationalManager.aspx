<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="OrganizationalManager.aspx.cs" Inherits="Wanda.BusinessIndicators.Web.SystemConfiguration.OrganizationalManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/SystemConfiguration/OrganizationalManager.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main clear">
        <div style="width: 30%; height: 400px; float: left">
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
        <div style="width: 65%; height: 400px; float: left; margin-left: 30px">
            <div class="margin_t10">
                <div class="tabs_m">
                    <ul>
                        <li class="selected m_1"><a class="active_sub2"><span>组织名称</span></a></li>
                    </ul>
                    <div class="organization_btn" style="float: right;">
                        <a class="btn_search" href="javascript:void(0)"><span><i class="fa fa-plus"></i>&nbsp;新增</span></a>
                        <a class="btn_search" href="javascript:void(0)"><span><i class="fa fa-plus"></i>&nbsp;修改</span></a>
                        <a class="btn_search" href="javascript:void(0)"><span><i class="fa fa-plus"></i>&nbsp;删除</span></a>
                    </div>
                </div>
                <div class="tabs_content user-content" style="height: auto">
                    <!--内容-->
                    <div style="margin-top: 5px;">
                        <div class="ztree_defined organization_content">
                            <div>
                                <span>组织名称:</span>
                                <input type="text" name="name" value="" id="organization_name"/>
                            </div>
                            <div>
                                <input type="checkbox" name="name" value="" />
                                <span>是否项目</span>
                            </div>
                            <div class="btnbox">
                                <input class="btn_ok limits_sumbit" type="button" value="确定">
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
