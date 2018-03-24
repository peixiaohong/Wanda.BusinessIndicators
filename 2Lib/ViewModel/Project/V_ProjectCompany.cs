using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class V_ProjectCompany : BaseModel
    {
        /// <summary>
        /// 项目公司ID(唯一标识)
        /// </summary>
        public Guid ProCompayID { get; set; }

        /// <summary>
        /// 项目公司名称
        /// </summary>
        public string ProCompayName { get; set; }

        public Guid SystemID { get; set; }
       
        public int FinYear { get; set; }
        public int FinMonth { get; set; }

        /// <summary>
        /// 项目公司属性
        /// </summary>
        public string ProCompanyProperty1 { get; set; }

        /// <summary>
        /// 项目公司排序
        /// </summary>
        public int ProCompanySequence { get; set; }

        /// <summary>
        /// 通行行数
        /// </summary>
        public int ProRowSpan { get; set; }

        /// <summary>
        /// 该字段只是针对Excel中尾盘的分组，不用于其它地方
        /// </summary>
        public int ExcelGroupRow { get; set; }

        /// <summary>
        /// 公司的实体
        /// </summary>
        public C_Company CompayModel { get; set; }

        /// <summary>
        /// 项目公司的指标列表
        /// </summary>
        public List<V_ProjectTarget> ProjectTargets { get; set; }

        /// <summary>
        /// 项目公司序号，（因分组，所以需要重新计算）
        /// </summary>
        public int ProCompanyNumber { get; set; }

        /// <summary>
        /// 项目公司数据类型：Data:标识从数据库中获取， XML:标识从XML中设置Modle中拼装生成, Remain:标识尾盘
        /// </summary>
        public string ProDataType { get; set; }

    }
}
