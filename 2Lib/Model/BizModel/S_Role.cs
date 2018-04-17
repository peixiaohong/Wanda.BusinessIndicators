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
    /// 角色表
    /// </summary>
    [ORTableMapping("dbo.S_Role")]
    public class S_Role : BaseModel
    {
        /// <summary>
        /// 中文名称
        /// </summary>
        [ORFieldMapping("CnName")]
        public string CnName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [ORFieldMapping("EnName")]
        public string EnName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [ORFieldMapping("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [NoMapping]
        public bool IsChecked { get; set; }
    }
}
