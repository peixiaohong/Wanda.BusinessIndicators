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
    /// 组织架构-用户权限表
    /// </summary>
    [ORTableMapping("dbo.S_Org_User")]
    public class S_Org_User : BaseModel
    {
        /// <summary>
        /// 板块ID
        /// </summary>
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreateUserID { get; set; }
    }
}
