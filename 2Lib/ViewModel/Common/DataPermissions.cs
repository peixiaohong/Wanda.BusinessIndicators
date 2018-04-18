using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.ViewModel.Common
{
    public class DataPermissions
    {
        public DataPermissions()
        {
            this.Nodes = new List<DataPermissions>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 板块ID
        /// </summary>
        public Guid SystemID { get; set; }

        /// <summary>
        /// 中文名称 
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid ParentID { get; set; }

        /// <summary>
        /// 等级  1隆基泰和 2板块 3大区 4区域 5项目
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 是否项目
        /// </summary>
        public bool IsCompany { get; set; }

        public List<DataPermissions> Nodes { get; set; }
    }
}
