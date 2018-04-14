using Lib.Data;
using LJTH.Lib.Data.AppBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Model.Filter
{
    [Serializable]
    public class AllUserPermissionsFilter : PagenationDataFilter
    {
        /// <summary>
        /// 关键字
        /// </summary>
        [FilterFieldAttribute("keyWord")]
        public string keyWord { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [FilterFieldAttribute("RoleID")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// 员工账号
        /// </summary>
        [FilterFieldAttribute("LoginName")]
        public string LoginName { get; set; }
    }
}
