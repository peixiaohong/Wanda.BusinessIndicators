using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.ViewModel.Employee
{
    public class AllUserPermissions
    {
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 登陆账号
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string DeptFullName { get; set; }

        /// <summary>
        /// 角色名称集合
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 部门名称集合
        /// </summary>
        public string OrgName { get; set; }
    }
}
