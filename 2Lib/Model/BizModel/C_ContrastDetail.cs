using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.Model
{
    [ORTableMapping("dbo.C_ContrastDetail")]
    public class C_ContrastDetail : BaseModel
    {

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }

        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }

        /// <summary>
        /// 系统ID
        /// </summary>
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }


        /// <summary>
        /// 系统名称
        /// </summary>
        [ORFieldMapping("SystemName")]
        public string SystemName { get; set; }


        /// <summary>
        /// 指标名称
        /// </summary>
        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }
        /// <summary>
        /// 指标ID
        /// </summary>
        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }
        #region 系统整体
        /// <summary>
        /// 去年系统总计
        /// </summary>
        [ORFieldMapping("LastAllTotal")]
        public decimal LastAllTotal { get; set; }

        /// <summary>
        /// 今年系统总计
        /// </summary>
        [ORFieldMapping("NowAllTotal")]
        public decimal NowAllTotal { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        [ORFieldMapping("Difference")]
        public decimal Difference { get; set; }

        /// <summary>
        /// 增长率
        /// </summary>
        [ORFieldMapping("Mounting")]
        public string Mounting { get; set; }

        #endregion

        #region 不可比部分
        /// <summary>
        /// 去年不可比总计
        /// </summary>
        [ORFieldMapping("NotContrastLast")]
        public decimal NotContrastLast { get; set; }

        /// <summary>
        /// 今年不可比总计
        /// </summary>
        [ORFieldMapping("NotContrastNow")]
        public decimal NotContrastNow { get; set; }


        /// <summary>
        /// 不可比差额
        /// </summary>
        [ORFieldMapping("NotDifference")]
        public decimal NotDifference { get; set; }

        /// <summary>
        /// 不可比增长率
        /// </summary>
        [ORFieldMapping("NotMounting")]
        public string NotMounting { get; set; }
        #endregion

        #region 可比部分
        /// <summary>
        /// 去年可比总计
        /// </summary>
        [ORFieldMapping("PossibleContrastLast")]
        public decimal PossibleContrastLast { get; set; }

        /// <summary>
        /// 今年可比总计
        /// </summary>
        [ORFieldMapping("PossibleContrastNow")]
        public decimal PossibleContrastNow { get; set; }


        /// <summary>
        /// 可比差额
        /// </summary>
        [ORFieldMapping("PossibleDifference")]
        public decimal PossibleDifference { get; set; }

        /// <summary>
        /// 可比增长率
        /// </summary>
        [ORFieldMapping("PossibleMounting")]
        public string PossibleMounting { get; set; }


        #endregion
        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("Remark")]
        public string Remark { get; set; }

        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }

    }



    public class ContrastDetailList
    {
        public string SystemName { get; set; }
        public Guid systemID { get; set; }
        public Guid MonthlyReportID { get; set; }
        public List<C_ContrastDetail> ContrastDetailMl { get; set; }
    }


    public class ContrastCompanyList
    {
        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("ID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("OpeningTime")]
        public DateTime OpeningTime { get; set; }
        /// <summary>
        /// 去年系统总计
        /// </summary>
        [ORFieldMapping("LastTotal")]
        public decimal LastAllTotal { get; set; }

        /// <summary>
        /// 今年系统总计
        /// </summary>
        [ORFieldMapping("NowTotal")]
        public decimal NowAllTotal { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        [ORFieldMapping("Difference")]
        public decimal Difference { get; set; }

        /// <summary>
        /// 增长率
        /// </summary>
        [ORFieldMapping("Mounting")]
        public string Mounting { get; set; }
    }

    public class ContarstTargetList
    {
        public string TargetName { get; set; }
        public Guid TargetID { get; set; }
        public List<ContrastCompanyList> List { get; set; }
    }
    public class ContarstTargetDetailList
    {
        public string TargetName { get; set; }
        public Guid TargetID { get; set; }
        /// <summary>
        /// 可比门店明细
        /// </summary>
        public List<ContrastCompanyList> ContrastList { get; set; }
        /// <summary>
        /// 不可比门店明细
        /// </summary>
        public List<ContrastCompanyList> NotContrastList { get; set; }


        #region 可比门店数据

        public decimal ContractLastTotal { get; set; }

        public decimal ContractNowTotal { get; set; }

        public decimal ContractDifference { get; set; }

        public string ContractMounting { get; set; }

        #endregion

        #region 不可比门店数据
        public decimal NotContractLastTotal { get; set; }

        public decimal NotContractNowTotal { get; set; }

        public decimal NotContractDifference { get; set; }

        public string NotContractMounting { get; set; }
        #endregion

        #region 合计
        public decimal LastTotal { get; set; }

        public decimal NowTotal { get; set; }

        public decimal Difference { get; set; }

        public string Mounting { get; set; }
        #endregion
    }

}
