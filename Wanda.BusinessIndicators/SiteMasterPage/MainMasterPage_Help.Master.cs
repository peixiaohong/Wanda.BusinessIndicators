﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using LJTH.BusinessIndicators.Common;

namespace LJTH.BusinessIndicators.Web
{
    public partial class MainMasterPage_Help : System.Web.UI.MasterPage
    {
        #region LoginInfo
        //protected bool HasLogined { get { return WebHelper.GetCurrentUser() != null; } }
        //protected bool ShowLogOut = bool.Parse(AppSettingConfig.GetSetting("ShowLogOut", "False"));
        //protected string LoginUserName { get { return WebHelper.GetCurrentUser().Name; } }

        protected bool HasLogined = true;
        protected bool ShowLogOut = true;
        protected string LoginUserName = "管理员";
        #endregion

        protected List<NavSiteMapNode> NavigatorNodes = new List<NavSiteMapNode>();

        /// <summary>
        /// 表示是否要强制菜单项为显示状态, 默认为受访问权限限制不显示
        /// </summary>
        private bool forceMenuShow = false;

        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="schemaid"></param>



        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hideLoginUser.Value = Common.WebHelper.GetCurrentLoginUser();

            //去掉Csr攻击
            //WebHelper.CheckCsrf();
            //if (AppSettingConfig.GetSetting("forceMenuShow", "false").ToLower() == "true")
            //{
            //forceMenuShow = true;
            //}
            NavigatorNodes = GetCachedNavNodes();
            bangsitmap();
        }

        private List<NavSiteMapNode> GetCachedNavNodes()
        {
            string cacheKey = "List_NavSiteMapNode_"; //+ WebHelper.GetCurrentUser().UserID;
            List<NavSiteMapNode> result = null;

            //  result = (List<NavSiteMapNode>) HttpContext.Current.Cache[cacheKey];
            if (result == null)
            {
                string mapFile = MapPath("~/sitemasterpage/navigator.sitemap");
                SiteMapParser parser = new SiteMapParser(mapFile);
                NavSiteMap siteMap = parser.GetCachedMap();
                result = GetNavNodesAfterCheck(siteMap);

                CacheDependency fileDependency = new CacheDependency(Server.MapPath(WebHelper.AuthCacheDependencyFile));
                HttpContext.Current.Cache.Insert(cacheKey, result, fileDependency);
            }

            return result;
        }

        protected void bangsitmap()
        {

            string first = string.Empty;
            string two = string.Empty;
            string url = Request.FilePath;
            //url = ".." + url;
            for (int i = 0; i < NavigatorNodes.Count; i++)
            {

                if (url == NavigatorNodes[i].Url)
                {
                    first = NavigatorNodes[i].Title;
                    //first = NavigatorNodes[i].Title+"<img src=\"../images/btn08.png\" />";
                }
                else
                {
                    if (NavigatorNodes[i].Nodes.Count > 0)
                    {
                        for (int a = 0; a < NavigatorNodes[i].Nodes.Count; a++)
                        {

                            if (url == NavigatorNodes[i].Nodes[a].Url)
                            {
                                first = NavigatorNodes[i].Title + "<img src=\"../images/btn08.png\" />";
                                //two = NavigatorNodes[i].Nodes[a].Title + "<img src=\"../images/btn08.png\" />";
                                two = NavigatorNodes[i].Nodes[a].Title;
                            }
                        }
                    }
                }
            }
            sitmap.InnerHtml = "您当前所在的位置：" + first + two;


        }
        private List<NavSiteMapNode> GetNavNodesAfterCheck(NavSiteMap siteMap)
        {

            #region 修改Enable属性， 在前端渲染时根据Enable属性判断是否需要显示
            List<NavSiteMapNode> result = new List<NavSiteMapNode>();
            // 如果第二级没有可以显示， 则第一级不显示
            foreach (NavSiteMapNode level1_item in siteMap.Nodes)
            {
                ////功能权限                
                if (!level1_item.Enable)
                {
                    continue;
                }

                NavSiteMapNode menuNode_level1 = new NavSiteMapNode(level1_item);
                if (this.Request.Url.AbsoluteUri.ToLower().Contains(menuNode_level1.Url.ToLower()))
                {
                    menuNode_level1.Selected = true;
                }

                menuNode_level1.Enable = false; //初始化， 根据是否有子级菜单决定是否显示

                foreach (NavSiteMapNode level2_item in level1_item.Nodes)
                {

                    if (level2_item.Enable == false)
                    {
                        continue;
                    }
                    NavSiteMapNode menuNode_level2 = new NavSiteMapNode(level2_item);

                    foreach (NavSiteMapNode level3_item in level2_item.Nodes)
                    {
                        if (level3_item.Enable == false)
                        {
                            continue;
                        }
                        NavSiteMapNode menuNode_level3 = new NavSiteMapNode(level3_item);

                        menuNode_level3.Enable = forceMenuShow || GetEnable(level3_item);
                        if (menuNode_level3.Enable)
                        {
                            if (this.Request.Url.AbsoluteUri.ToLower().Contains(menuNode_level3.Url.ToLower()))
                            {
                                menuNode_level2.Selected = true;
                            }
                            menuNode_level2.Nodes.Add(menuNode_level3);
                            menuNode_level2.Enable = true;
                        }
                    }

                    menuNode_level2.Enable = forceMenuShow || GetEnable(level2_item);

                    if (menuNode_level2.Enable)
                    {
                        if ((level2_item.Nodes.Count > 0 && menuNode_level2.Nodes.Count > 0) || level2_item.Nodes.Count == 0)
                        {
                            if (this.Request.Url.AbsoluteUri.ToLower().Contains(menuNode_level2.Url.ToLower()) || menuNode_level2.Selected)
                            {
                                menuNode_level1.Selected = true;
                            }
                            menuNode_level1.Nodes.Add(menuNode_level2);
                            menuNode_level1.Enable = true;
                        }
                    }
                }

                if (!menuNode_level1.Enable && menuNode_level1.Url != "#" && GetEnable(menuNode_level1))
                {
                    menuNode_level1.Enable = true;
                }

                if (menuNode_level1.Enable)
                {
                    result.Add(menuNode_level1);
                }
            }
            #endregion

            return result;
        }

        private bool GetEnable(NavSiteMapNode node)
        {
            // 在此做用户的权限判断
            //var auth = PermissionHelper.GetFuncPermission();            ////功能权限
            //if (PermissionHelper.EnablePermission)
            //    if (Request.Url.AbsolutePath == node.Url)
            //    {
            //        if (auth != null && !auth.Any(p => p.FuncCode == "GQ_" + node.ResourceKey))
            //        {
            //            //throw new Exception("无效的资源访问");
            //            Response.Redirect("../NoPermission.aspx");
            //        }
            //    }
            //Uri uri;
            //if (node.Url.StartsWith("/"))
            //{
            //    uri = new Uri("http://domain" + node.Url);
            //}
            //else if (node.Url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    uri = new Uri(node.Url);
            //}
            //else if (node.Url.StartsWith("#"))
            //{
            //    return true;
            //}
            //else
            //{
            //    throw new ApplicationException("无效的导航配置节点。 Node.url=" + node.Url);
            //}
            //if (PermissionHelper.EnablePermission)
            //    return auth != null && auth.Count > 0 && auth.Any(p => p.FuncCode == "GQ_" + node.ResourceKey);
            return true;
        }

        //private bool _enabledSSO = true;

        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    string enabledSSO = System.Configuration.ConfigurationManager.AppSettings["EnabledSSO"] ?? "true";
        //    bool.TryParse(enabledSSO, out _enabledSSO);

        //    if (Page.User.Identity.IsAuthenticated && !_enabledSSO)
        //    {
        //        //退出登录
        //        System.Web.Security.FormsAuthentication.SignOut();
        //        Response.Redirect(System.Web.Security.FormsAuthentication.LoginUrl);
        //    }
        //}

    }
}