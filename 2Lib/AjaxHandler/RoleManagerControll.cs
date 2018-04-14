using Lib.Web;
using Lib.Web.MVC.Controller;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Model.BizModel;
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
                #region 添加数据
                if (entity.ID == "00000000-0000-0000-0000-000000000000".ToGuid())
                {
                    entity.ID = Guid.NewGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreateUserID = 1;
                    entity.CreatorName = "测试";
                    entity.EnName = "wu";
                    entity.IsDeleted = false;
                    entity.ModifierName = "测试";
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
                    entity.ModifierName = "";
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
                    Message ="参数丢失"
                };
            }

            try
            {
                List<S_RolePermissions> entitys = JsonConvert.DeserializeObject<List<S_RolePermissions>>(data);
                foreach (var item in entitys)
                {
                    item.CreateTime = DateTime.Now;
                    item.CreateUserID = 0;
                    item.CreatorName = "测试";
                    item.IsDeleted = false;
                    item.ModifierName = "测试";
                    item.ModifyTime = DateTime.Now;
                    item.ID = Guid.NewGuid();
                }
                var resultData = S_RolePermissionsActionOperator.Instance.SaveListData(RoleID.ToGuid(),entitys);
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
    }
}
