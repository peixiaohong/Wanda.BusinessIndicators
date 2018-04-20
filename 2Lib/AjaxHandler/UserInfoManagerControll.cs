using Lib.Web;
using Lib.Web.MVC.Controller;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model.BizModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class UserInfoManagerControll : BaseController
    {
        #region 角色设置

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="cnName"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [LibAction]
        public object GetUserRoles(string cnName, string loginName)
        {
            string message = string.Empty;
            int success = 0;
            if (loginName == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数：用户账号信息没有传递"
                };
            }
            try
            {
                var data = S_RoleActionOperator.Instance.GetDatasByCnName(cnName, loginName);
                success = 1;
                message = "查询成功";
                return new
                {
                    Data = data,
                    Success = success,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 保存用户-角色的关系
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        [LibAction]
        public object SaveUsesRoles(string loginName, string roleIDs)
        {
            if (loginName == "" || roleIDs == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数：用户账号信息没有传递；角色ID传递为null"
                };
            }
            string message = string.Empty;
            int success = 0;
            try
            {
                string[] roleIds = roleIDs.Split(',');
                if (roleIds.Length < 1)
                {
                    return new
                    {
                        Data = "",
                        Success = 0,
                        Message = "参数：角色ID传递为null"
                    };
                }
                var resutlDeleteUser_Role = S_Role_UserActionOperator.Instance.DeleteDatasByLoginName(loginName);
                List<S_Role_User> entitys = new List<S_Role_User>();
                foreach (var item in roleIds)
                {
                    S_Role_User su = new S_Role_User()
                    {
                        ID = Guid.NewGuid(),
                        LoginName = loginName,
                        RoleID = item.ToGuid(),
                        IsDeleted = false,
                        CreateTime = DateTime.Now,
                        CreatorName = base.CurrentUserName,
                        ModifierName = base.CurrentUserName,
                        ModifyTime = DateTime.Now
                    };
                    entitys.Add(su);
                }
                int number = S_Role_UserActionOperator.Instance.InsertListData(entitys);
                //清除缓存
                WebHelper.InvalidAuthCache();
                if (number > 0)
                {
                    success = 1;
                    message = "设置角色成功";
                }
                else
                {
                    success = 0;
                    message = "设置角色失败";
                }
                return new
                {
                    Data = "",
                    Success = success,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        #endregion 角色设置

        #region 组织架构设置

        /// <summary>
        /// 数据获取
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [LibAction]
        public object GetUserOrgs(string loginName)
        {
            if (loginName == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数：用户账号信息没有传递"
                };
            }
            string message = string.Empty;
            int success = 0;
            try
            {
                var resutlData = S_OrganizationalActionOperator.Instance.GetDataByLoginName(loginName);
                success = 1;
                message = "查询数据库成功";
                return new
                {
                    Data = resutlData,
                    Success = success,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 保存用户的组织权限
        /// </summary>
        /// <param name="data"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [LibAction]
        public object SaveUser_org(string data, string loginName)
        {
            string message = string.Empty;
            int success = 0;
            if (loginName == "" || data == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数：用户账号信息没有传递"
                };
            }
            try
            {
                var resutlDelteUser_org = S_Org_UserActionOperator.Instance.DeleteDataByLoginName(loginName);
                List<S_Org_User> entitys = JsonConvert.DeserializeObject<List<S_Org_User>>(data);
                foreach (var item in entitys)
                {
                    item.CreateTime = item.ModifyTime = DateTime.Now;
                    item.CreatorName = item.ModifierName = base.CurrentUserName;
                    item.IsDeleted = false;
                    item.ID = Guid.NewGuid();
                }
                var number=S_Org_UserActionOperator.Instance.InsertListData(entitys);
                //清除缓存
                WebHelper.InvalidAuthCache();
                if (number > 0)
                {
                    success = 1;
                    message = "设置组织成功";
                }
                else
                {
                    success = 0;
                    message = "设置组织失败";
                }
                return new
                {
                    Data = "",
                    Success = success,
                    Message = message
                };
            }
            catch (Exception ex)
            {

                return new
                {
                    Data = "",
                    Success = 0,
                    Message = ex.Message
                };
            }
        }
        #endregion
    }
}