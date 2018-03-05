using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;


namespace Wanda.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a TargetPlanDetail.
    /// </summary>
    [ORTableMapping("dbo.A_TargetPlanDetail")]
    public class A_TargetPlanDetail : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }



        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }



        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }



        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }



        [ORFieldMapping("Target")]
        public decimal Target { get; set; }



        [ORFieldMapping("VersionStart")]
        public DateTime VersionStart { get; set; }



        [ORFieldMapping("Versionend")]
        public DateTime Versionend { get; set; }

        [ORFieldMapping("TargetPlanID")]
        public Guid TargetPlanID { get; set; }
        #endregion

        /// <summary>
        /// 同步数据--过来的与上月的差额 == 当月数据
        /// </summary>
        [NoMapping]
        public decimal JQNDifference { get; set; }

        [NoMapping]
        public string CompanyName { get; set; }

        [NoMapping]
        public string TargetName { get; set; }


    }
    //public class TargetDetail
    //{
    //    public int FinMonth { get; set; }

    //    public List<TargetDetailList> TargetDetailList { get; set; }
    //}

    //public class TargetDetailList
    //{
    //    public Guid TargetID { get; set; }

    //    public string TargetName { get; set; }

    //    public decimal? Target { get; set; }

    //    public decimal? SumTarget { get; set; }
    //}

}

