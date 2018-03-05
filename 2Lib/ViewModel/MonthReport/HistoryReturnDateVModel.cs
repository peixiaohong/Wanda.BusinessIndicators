using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.ViewModel
{
    public class HistoryReturnDateVModel : BaseModel, IBaseComposedModel
    {
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }
        

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        /// <summary>
        /// 一月
        /// </summary>
        [ORFieldMapping("January")]
        public int January{ get; set; }

        /// <summary>
        /// 二月
        /// </summary>
        [ORFieldMapping("February")]
        public int February{ get; set; }

        /// <summary>
        /// 三月
        /// </summary>
        [ORFieldMapping("March")]
        public int March{ get; set; }

        /// <summary>
        /// 四月
        /// </summary>
        [ORFieldMapping("April")]
        public int April { get; set; }

        /// <summary>
        /// 五月
        /// </summary>
        [ORFieldMapping("May")]
        public int May { get; set; }

        /// <summary>
        /// 六月
        /// </summary>
        [ORFieldMapping("June")]
        public int June { get; set; }

        /// <summary>
        /// 七月
        /// </summary>
        [ORFieldMapping("July")]
        public int July { get; set; }

        /// <summary>
        /// 八月
        /// </summary>
        [ORFieldMapping("August")]
        public int August { get; set; }

        /// <summary>
        /// 九月
        /// </summary>
        [ORFieldMapping("September")]
        public int September { get; set; }

        /// <summary>
        /// 十月
        /// </summary>
        [ORFieldMapping("October ")]
        public int October { get; set; }

        /// <summary>
        /// 十一
        /// </summary>
        [ORFieldMapping("November")]
        public int November { get; set; }

        /// <summary>
        /// 十二
        /// </summary>
        [ORFieldMapping("December")]
        public int December { get; set; }


        /// <summary>
        /// 一月要求补回期限
        /// </summary>
        [ORFieldMapping("January_CommitDate")]
        public DateTime? January_CommitDate { get; set; }

        /// <summary>
        /// 二月要求补回期限
        /// </summary>
        [ORFieldMapping("February_CommitDate")]
        public DateTime? February_CommitDate { get; set; }

        /// <summary>
        /// 三月要求补回期限
        /// </summary>
        [ORFieldMapping("March_CommitDate")]
        public DateTime? March_CommitDate { get; set; }

        /// <summary>
        /// 四月要求补回期限
        /// </summary>
        [ORFieldMapping("April_CommitDate")]
        public DateTime? April_CommitDate { get; set; }

        /// <summary>
        /// 五月要求补回期限
        /// </summary>
        [ORFieldMapping("May_CommitDate")]
        public DateTime? May_CommitDate { get; set; }

        /// <summary>
        /// 六月要求补回期限
        /// </summary>
        [ORFieldMapping("June_CommitDate")]
        public DateTime? June_CommitDate { get; set; }

        /// <summary>
        /// 七月要求补回期限
        /// </summary>
        [ORFieldMapping("July_CommitDate")]
        public DateTime? July_CommitDate { get; set; }

        /// <summary>
        /// 八月要求补回期限
        /// </summary>
        [ORFieldMapping("August_CommitDate")]
        public DateTime? August_CommitDate { get; set; }

        /// <summary>
        /// 九月要求补回期限
        /// </summary>
        [ORFieldMapping("September_CommitDate")]
        public DateTime? September_CommitDate { get; set; }

        /// <summary>
        /// 十月要求补回期限
        /// </summary>
        [ORFieldMapping("October_CommitDate")]
        public DateTime? October_CommitDate { get; set; }

        /// <summary>
        /// 十一要求补回期限
        /// </summary>
        [ORFieldMapping("November_CommitDate")]
        public DateTime? November_CommitDate { get; set; }

        /// <summary>
        /// 十二
        /// </summary>
        [ORFieldMapping("December_CommitDate")]
        public DateTime? December_CommitDate { get; set; }

    }


    


    public class ComprehensiveReportVModel : BaseModel, IBaseComposedModel
    {

        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }

        [ORFieldMapping("SystemName")]
        public string SystemName { get; set; }

        [ORFieldMapping("TType")]
        public int TType { get; set; }

        /// <summary>
        /// 指标的排序
        /// </summary>
        [ORFieldMapping("T_Sequence")]
        public int T_Sequence { get; set; }


        /// <summary>
        /// 系统排序
        /// </summary>
        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }

        [ORFieldMapping("1")]
        public decimal _1 { get; set; }
        
        [ORFieldMapping("2")]
        public decimal _2 { get; set; }

        [ORFieldMapping("3")]
        public decimal _3 { get; set; }

        [ORFieldMapping("4")]
        public decimal _4 { get; set; }

        [ORFieldMapping("5")]
        public decimal _5 { get; set; }

        [ORFieldMapping("6")]
        public decimal _6 { get; set; }

        [ORFieldMapping("7")]
        public decimal _7 { get; set; }

        [ORFieldMapping("8")]
        public decimal _8 { get; set; }

        [ORFieldMapping("9")]
        public decimal _9 { get; set; }

        [ORFieldMapping("10")]
        public decimal _10 { get; set; }

        [ORFieldMapping("11")]
        public decimal _11 { get; set; }


        [ORFieldMapping("12")]
        public decimal _12 { get; set; }

        [ORFieldMapping("Sun_12")]
        public decimal Sun_12 { get; set; }

        [NoMapping]
        public int SrowSpan_str { get; set; }
        [NoMapping]
        public int TrowSpan_str { get; set; }
      
    }


    public class ComprehensiveReportGroupModel {

        public Guid SystemID { get; set; }

        public int sys_count { get; set; }

    }

}
