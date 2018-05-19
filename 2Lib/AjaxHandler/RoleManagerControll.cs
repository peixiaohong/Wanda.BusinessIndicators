using Lib.Web;
using Lib.Web.MVC.Controller;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.BusinessIndicators.Model.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class RoleManagerControll : BaseController
    {
        [LibAction]
        public string Test()
        {
            return "测试成功";
        }

        #region 角色管理List页面
        /// <summary>
        /// 无条件加载页面
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public object GetRoles(string CnName)
        {
            try
            {
                var data = S_RoleActionOperator.Instance.GetDatas(CnName);
                var success = 1;
                return new
                {
                    Data = data,
                    Success = success,
                    Message = "查询数据没有问题"
                };
            }
            catch (Exception e)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="CnName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        [LibAction]
        public object SaveRole(string RoleData)
        {
            string Message = string.Empty;
            int Success = 0;
            try
            {
                S_Role entity = JsonConvert.DeserializeObject<S_Role>(RoleData);

                // 数据验证
                if (entity.CnName == "" && entity.Description == "")
                {
                    return new {
                        Data = "",
                        Success = 0,
                        Message = "参数传递失败"
                    };
                }
                List<S_Role> oldRoles = S_RoleActionOperator.Instance.GetDatasByCnName(entity.CnName);
                #region 添加数据
                if (entity.ID == "00000000-0000-0000-0000-000000000000".ToGuid())
                {
                    if (oldRoles != null && oldRoles.Count > 0)
                    {
                        return new
                        {
                            Data = "",
                            Success = 0,
                            Message = "保存的角色名：【" + entity.CnName + "】已经存在，不允许角色名称重复"
                        };
                    }

                    entity.ID = Guid.NewGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreatorName =base.CurrentUserName;
                    entity.EnName = "";
                    entity.IsDeleted = false;
                    entity.ModifierName = base.CurrentUserName;
                    entity.ModifyTime = DateTime.Now;
                    int number = S_RoleActionOperator.Instance.InsertData(entity);
                    if (number > 0)
                    {
                        Success = 1;
                        Message = "添加成功";
                    }
                    else
                    {
                        Success = 0;
                        Message = "添加失败";
                    }
                }
                #endregion
                #region 修改
                else
                {
                    if (oldRoles != null && oldRoles.Where(or => or.ID != entity.ID).Count() > 0)
                    {
                        return new
                        {
                            Data = "",
                            Success = 0,
                            Message = "更改的角色名：【" + entity.CnName + "】已经存在，不允许角色名称重复"
                        };
                    }
                    entity.ModifierName = base.CurrentUserName;
                    entity.ModifyTime = DateTime.Now;
                    Guid number = S_RoleActionOperator.Instance.UpdateData(entity);
                    Success = 1;
                    Message = "修改成功";
                }
                #endregion
                return new
                {
                    Data = "",
                    Success = Success,
                    Message = Message
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
        /// 删除单条数据（逻辑删除）
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [LibAction]
        public object DeleteRoleData(string ID)
        {
            string Message = string.Empty;
            int Success = 0;
            try
            {
                #region 添加数据
                Guid number = S_RoleActionOperator.Instance.RemoveObject(ID.ToGuid());
                //清除缓存
                WebHelper.InvalidAuthCache();
                Success = 1;
                Message = "删除成功";
                #endregion
                return new
                {
                    Data = "",
                    Success = Success,
                    Message = Message
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
        /// 获取单个角色
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [LibAction]
        public object GetRoleDataByID(string ID)
        {
            string Message = string.Empty;
            int Success = 0;
            try
            {
                var data = S_RoleActionOperator.Instance.GetRoleByID(ID.ToGuid());
                Success = 1;
                Message = "查询成功";
                return new
                {
                    Data = data,
                    Success = Success,
                    Message = Message
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

        #region 功能点

        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [LibAction]
        public Object GetMenuDatas(string ID)
        {
            string Message = string.Empty;
            int Success = 0;
            try
            {
                var data = S_MenuActionOperator.Instance.GetRoleMenus(ID.ToGuid());
                Success = 1;
                Message = "查询成功";
                return new
                {
                    Data = data,
                    Success = Success,
                    Message = Message
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

        [LibAction]
        public Object SaveRolePermissions(string RoleID, string data)
        {

            string Message = string.Empty;
            int Success = 0;

            if (RoleID == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数丢失"
                };
            }

            try
            {
                List<S_RolePermissions> entitys = JsonConvert.DeserializeObject<List<S_RolePermissions>>(data);
                foreach (var item in entitys)
                {
                    item.CreateTime = DateTime.Now;
                    item.CreatorName = base.CurrentUserName;
                    item.IsDeleted = false;
                    item.ModifierName =base.CurrentUserName;
                    item.ModifyTime = DateTime.Now;
                    item.ID = Guid.NewGuid();
                }
                var resultData = S_RolePermissionsActionOperator.Instance.SaveListData(RoleID.ToGuid(), entitys);
                //清除缓存
                WebHelper.InvalidAuthCache();
                if (resultData > 0)
                {
                    Success = 1;
                    Message = "保存成功";
                }
                else
                {
                    Success = 0;
                    Message = "保存失败";
                }
                return new
                {
                    Data = resultData,
                    Success = Success,
                    Message = Message
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

        #region 人员设置

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [LibAction]
        public object GetAllUser(string data)
        {
            string Message = string.Empty;
            int Success = 0;
            int TotalCount = 0;

            try
            {
                AllUserPermissionsFilter filter = JsonConvert.DeserializeObject<AllUserPermissionsFilter>(data);
                var resultData = EmployeeActionOperator.Instance.GetAllUser(filter, out TotalCount);
                Success = 1;
                Message = "查询成功";
                return new
                {
                    Data = resultData,
                    TotalCount = TotalCount,
                    Success = Success,
                    Message = Message
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    TotalCount = TotalCount,
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取点击新增用户的时候用户数据
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        [LibAction]
        public object GetAddUserInfo(string data)
        {
            string Message = string.Empty;
            int Success = 0;
            int TotalCount = 0;

            try
            {
                AllUserPermissionsFilter filter = JsonConvert.DeserializeObject<AllUserPermissionsFilter>(data);
                var resultData = EmployeeActionOperator.Instance.GetAddUserInfo(filter, out TotalCount);
                Success = 1;
                Message = "查询成功";
                return new
                {
                    Data = resultData,
                    TotalCount = TotalCount,
                    Success = Success,
                    Message = Message
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    TotalCount = TotalCount,
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 保存角色—用户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [LibAction]
        public object SaveUser_Role(string PageLoginNames, string RoleID)
        {
            string Message = string.Empty;
            int Success = 0;

            if (RoleID == "" || PageLoginNames == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数丢失"
                };
            }

            try
            {
                List<string> oldLoginNames = S_Role_UserActionOperator.Instance.GetDataByRoleID(RoleID.ToGuid());
                List<S_Role_User> entitys = new List<S_Role_User>();
                string[] LoginNames = PageLoginNames.Split(',');
                if (LoginNames.Count() < 1)
                {
                    return new
                    {
                        Data = "",
                        Success = 0,
                        Message = "参数丢失"
                    };
                }
                foreach (var item in LoginNames)
                {
                    if (oldLoginNames.Count > 0)
                    {
                        if (oldLoginNames.Contains(item))
                        {
                            continue;
                        }
                    }
                    S_Role_User su = new S_Role_User();
                    su.RoleID = RoleID.ToGuid();
                    su.LoginName = item;
                    su.CreateTime = DateTime.Now;
                    su.CreatorName = base.CurrentUserName;
                    su.IsDeleted = false;
                    su.ModifierName = base.CurrentUserName;
                    su.ModifyTime = DateTime.Now;
                    su.ID = Guid.NewGuid();
                    entitys.Add(su);
                }
                int resultData = S_Role_UserActionOperator.Instance.InsertListData(entitys);
                //清除缓存
                WebHelper.InvalidAuthCache();
                if (resultData > 0)
                {
                    Success = 1;
                    Message = "保存成功";
                }
                else
                {
                    Success = 0;
                    Message = "保存失败";
                }
                return new
                {
                    Data = "",
                    Success = Success,
                    Message = Message
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
        /// 删除用户和角色的关联【一条数据】
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="LoginName"></param>
        /// <returns></returns>
        [LibAction]
        public object DeleteRole_User(string RoleID,string LoginName)
        {

            string Message = string.Empty;
            int Success = 0;
            try
            {
                if (RoleID == "" || LoginName == "")
                {
                    return new
                    {
                        Data = "",
                        Success = 0,
                        Message = "参数不完整"
                    };
                }
                
                var resultData = S_Role_UserActionOperator.Instance.DelteDataByRoleID_LoginName(RoleID.ToGuid(),LoginName);
                //清除缓存
                WebHelper.InvalidAuthCache();
                if (resultData > 0)
                {
                    Success = 1;
                    Message = "删除成功";
                }
                else
                {
                    Success = 0;
                    Message = "删除失败";
                }
                return new
                {
                    Data = resultData,
                    Success = Success,
                    Message = Message
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
