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
        /// 板块ID
        /// </summary>
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

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
        /// 等级  1隆基泰和 2板块 3大区 4区域 5项目
        /// </summary>
        [ORFieldMapping("Level")]
        public int Level { get; set; }

        /// <summary>
        /// 是否项目
        /// </summary>
        [ORFieldMapping("IsCompany")]
        public bool IsCompany { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [NoMapping]
        public int IsChecked { get; set; }
    }
}
