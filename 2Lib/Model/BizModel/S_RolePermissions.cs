using Lib.Data;
using LJTH.Lib.Data.AppBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Model.BizModel
{
    /// <summary>
    /// 角色权限表
    /// </summary>
    [ORTableMapping("dbo.S_RolePermissions")]
    public class S_RolePermissions : BaseModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ORFieldMapping("RoleID")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        [ORFieldMapping("MenuID")]
        public Guid MenuID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [ORFieldMapping("CreateUserID")]
        public int CreateUserID { get; set; }

    }
}
