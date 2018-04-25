using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPF.Workflow.Client;


namespace LJTH.BusinessIndicators.Web
{
    public abstract class BasePage : System.Web.UI.Page
    {
        /// <summary>
        /// 当前人
        /// </summary>
        protected string CurrentUser { get { return BPF.Workflow.Client.SDKHelper.GetUserName(Context); } }

        //private List<MemberPropertyResult> _currentUserPropertity = null;

        /// <summary>
        /// 当前人在授权定义的属性
        /// </summary>
        //public List<MemberPropertyResult> CurrentUserPropertity
        //{
        //    get
        //    {
        //        if (_currentUserPropertity == null)
        //            _currentUserPropertity = Wanda.Platform.Permission.ClientComponent
        //            .PermissionCenterProxy.GetUserProperties();
        //        return _currentUserPropertity;
        //    }
        //}
    }

    public static class PermissionHelper
    {
        public static void GetPermission()
        {
            //if (EnablePermission)
            //    //未获取，或者不是同一个人，或者不是同一个应用，则从权限中心获取
            //    if (Wanda.Platform.Permission.ClientComponent.PermissionCenterProxy.GetPCDataStatus() == Wanda.Platform.Permission.ClientComponent.PermissionCenterDataStatus.NotReady
            //        || GetCurrentUser != PermissionCenterProxy.GetUserLoginName()
            //        || PCClientIdentity.CurrentApplicationCode != PermissionCenterProxy.GetApplicationCode())
            //    {
            //        PermissionCenterProxy.GetPermission();
            //    }
        }

        public static List<string> GetStartProcessList()
        {
            //如果要搬走请把GetPermission（） 方法也拷走
            //if (EnablePermission)
            //    return PermissionCenterProxy.GetUserProperties().Where(p => p.PropertyCode == "SystemName").Select(p => p.PropertyValue).ToList();
            //else
            //{
            return null;
            //}
        }
        /// <summary>
        /// 用于获取操作日志
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMonthReportLogList()
        {
            //如果要搬走请把GetPermission（） 方法也拷走
            //if (EnablePermission)
            //    return PermissionCenterProxy.GetUserProperties().Where(p => p.PropertyCode == "MonthReportLog").Select(p => p.PropertyValue).ToList();
            //else
            //{
            return null;
            //}
        }
        /// <summary>
        /// 用于获取操作日志
        /// </summary>
        /// <returns></returns>
        public static List<string> Getsubmanage()
        {
            //如果要搬走请把GetPermission（） 方法也拷走
            //if (EnablePermission)
            //    return PermissionCenterProxy.GetUserProperties().Where(p => p.PropertyCode == "submanage").Select(p => p.PropertyValue).ToList();
            //else
            //{
                return null;
            //}
        }

        //public static List<FunctionalAuthorityResult> GetFuncPermission()
        //{
        //    if (EnablePermission)
        //        return PermissionCenterProxy.GetFuncPermission();
        //    else
        //    {
        //        return null;
        //    }
        //}

        public static bool EnablePermission
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EnablePermission"] == "true";
            }
        }

        /// <summary>
        /// 获取人(请使用这处代码)
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentUser
        {
            get { return BPF.Workflow.Client.SDKHelper.GetUserName(HttpContext.Current); }
        }
    }
}