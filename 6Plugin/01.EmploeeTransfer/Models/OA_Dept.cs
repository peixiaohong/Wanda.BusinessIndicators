using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.EmploeeTransfer.Models
{
    public class OA_Dept
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DESCRIPT { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>
        public int PARENT_ID { get; set; }
        /// <summary>
        /// 真实上级部门
        /// </summary>
        public int REAL_PARENT_ID { get; set; }
        /// <summary>
        /// 类型 1部门 2分公司
        /// </summary>
        public int MDM_ID { get; set; }
        /// <summary>
        /// 分公司ID
        /// </summary>
        public int COMPANY_ID { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int ORDERLY { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UPDATE_TIME { get; set; }
        /// <summary>
        /// 是否已删除
        /// </summary>
        public int IS_DELETE { get; set; }
        /// <summary>
        /// 部门完整路径
        /// </summary>
        public string DeptFullPath { get; set; }
        /// <summary>
        /// 部门完整名称
        /// </summary>
        public string DeptFullName { get; set; }
    }
}
