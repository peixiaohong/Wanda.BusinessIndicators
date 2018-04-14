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
    /// 角色-用户关系表
    /// </summary>
    [ORTableMapping("dbo.S_Role_User")]
    public class S_Role_User : BaseModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ORFieldMapping("RoleID")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [ORFieldMapping("LoginName")]
        public string LoginName { get; set; }
    }
}
