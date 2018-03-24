using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;
namespace LJTH.Lib.AuthCenter.Model
{
    [Serializable]
    class WdSimpleUserDataFilter : PagenationDataFilter, IDataFilter
    {  
        /// <summary>
        /// 员工编号
        /// </summary>
        [FilterFieldAttribute("employeeCode", "startwith")]
        public string EmployeeCode { set; get; }

        /// <summary>
        /// 登陆名
        /// </summary>
        [FilterFieldAttribute("username", "startwith")]
        public string LoginName { set; get; }

        /// <summary>
        /// 用户中文名
        /// </summary>
        [FilterFieldAttribute("employeeName", "startwith")]
        public string UserName { set; get; }

         
    }
}
