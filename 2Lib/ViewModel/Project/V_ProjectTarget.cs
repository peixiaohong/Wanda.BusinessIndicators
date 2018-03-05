using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.ViewModel
{
    public class V_ProjectTarget : BaseModel
    {
        /// <summary>
        /// 指标ID
        /// </summary>
        public Guid ProTargetID { get; set; }
        public string ProTargetName { get; set; }
        public Guid ProCompayID { get; set; }

        /// <summary>
        /// 明细表中的ID(唯一标识)
        /// </summary>
        public Guid ProMonthlyReportDetailID { get; set;}
      
        /// <summary>
        /// 年度计划指标
        /// </summary>
        public decimal NPlanAmmountByYear { get; set; }
        /// <summary>
        /// 暂时用用
        /// </summary>
        public decimal NActualAmmountByYear { get; set; } 
        
        public string NActualRateByYear { get; set; }
        /// <summary>
        /// 年度指标完成率
        /// </summary>
        public string NDisplayRateByYear { get; set; }


        public decimal NPlanAmmount { get; set; }
        public decimal NActualAmmount { get; set; }
        public string NActualRate { get; set; }
        public string NDisplayRate { get; set; }



        public decimal NAccumulativePlanAmmount { get; set; }
        public decimal NAccumulativeActualAmmount { get; set; }
        public string NAccumulativeActualRate { get; set; }
        public string NAccumulativeDisplayRate { get; set; }

        /// <summary>
        /// 第一次未完成的时间
        /// </summary>
        public DateTime? FirstMissTargetDate { get; set; }

        /// <summary>
        /// 预警次数
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// 累计未完成
        /// </summary>
        public bool IsMissTarget { get; set; }


        /// <summary>
        /// 当前未完成
        /// </summary>
        public bool IsMissTargetCurrent { get; set; }


        /// <summary>
        /// 项目指标排序
        /// </summary>
        public int ProTargetSequence { get; set; }


    }
}
