using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{

    /// <summary>
    /// 供用户输入控件查询使用
    /// </summary>
    [ORViewMapping(@"
select employeeName,
    employeeCode,
    unitName,
    username
from Wd_User a 
    where a.employeeStatus <>3
", "VWdSimpleUserInfo")]
    public class VWdSimpleUserInfo : IBaseComposedModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [ORFieldMapping("employeeCode")]
        public string UserID { set; get; }

        /// <summary>
        /// 员工编号
        /// </summary>
        [ORFieldMapping("employeeCode")]
        public string EmployeeCode { set; get; }

        /// <summary>
        /// 登陆名
        /// </summary>
        [ORFieldMapping("username")]
        public string LoginName { set; get; }

        /// <summary>
        /// 用户中文名
        /// </summary>
        [ORFieldMapping("employeeName")]
        public string Name { set; get; }


        /// <summary>
        /// 所在单位名称
        /// </summary>
        [ORFieldMapping("unitName")]
        public string DeptName { set; get; }
    }

}
