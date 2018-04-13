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
            
        }
    }
}
