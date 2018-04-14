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
    /// 菜单表
    /// </summary>
    [ORTableMapping("dbo.S_Menu")]
    public class S_Menu: BaseModel
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
        /// 父级GUID
        /// </summary>
        [ORFieldMapping("ParentMenuID")]
        public Guid ParentMenuID { get; set; }

        /// <summary>
        /// 等级 1板块 2大区 3区域 4项目
        /// </summary>
        [ORFieldMapping("Level")]
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sequence")]
        public string Sequence { get; set; }

        /// <summary>
        /// Url地址
        /// </summary>
        [ORFieldMapping("Url")]
        public string Url { get; set; }

        /// <summary>
        /// 唯一key
        /// </summary>
        [ORFieldMapping("ResourceKey")]
        public string ResourceKey { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [ORFieldMapping("CreateUserID")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [NoMapping]
        public int IsChecked { get; set; }
    }
}
