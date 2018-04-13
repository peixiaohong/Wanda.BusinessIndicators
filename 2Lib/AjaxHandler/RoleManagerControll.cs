using Lib.Web;
using Lib.Web.MVC.Controller;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Model.BizModel;
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

        [LibAction]
        public object GetRoles()
        {
            try
            {
                var data = S_RoleActionOperator.Instance.GetDatas();
                var success = 1;
                return new {
                    Data = data,
                    Success = success,
                    Message = "查询数据没有问题"
                };
            }
            catch (Exception e)
            {
                return new {
                    Data = "",
                    Success = 0,
                    Message = e.Message
                };
            }
        }

        [LibAction]
        public object InsertRoleData(S_Role data)
        {
            try
            {
                string Message = string.Empty;
                int Success = 0;
                data.ID = Guid.NewGuid();
                data.CreateTime = DateTime.Now;
                data.CreateUserID = 1;
                data.CreatorName = "测试";
                data.EnName = "wu";
                data.IsDeleted = false;
                data.ModifierName = "测试";
                data.ModifyTime = DateTime.Now;
                int number = S_RoleActionOperator.Instance.InsertData(data);
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
    }
}
