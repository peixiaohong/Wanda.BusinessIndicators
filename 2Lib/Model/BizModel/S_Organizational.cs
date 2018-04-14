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
    /// 组织架构表
    /// </summary>
    [ORTableMapping("dbo.S_Organizational")]
    public class S_Organizational : BaseModel
    {
        /// <summary>
        /// 中文名称 
        /// </summary>
        [ORFieldMapping("CnName")]
        public string CnName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code")]
        public string Code { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [ORFieldMapping("ParentID")]
        public Guid ParentID { get; set; }

        /// <summary>
        /// 等级 1板块 2大区 3区域 4项目
        /// </summary>
        [ORFieldMapping("Level")]
        public int Level { get; set; }
    }
}
