using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a Target.
    /// </summary>
    [ORTableMapping("dbo.C_Target")]
    public class C_Target : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }



        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }



        [ORFieldMapping("HaveDetail")]
        public bool HaveDetail { get; set; }



        [ORFieldMapping("DetailLevel")]
        public string DetailLevel { get; set; }



        [ORFieldMapping("NeedEvaluation")]
        public bool NeedEvaluation { get; set; }



        [ORFieldMapping("TargetType")]
        public int TargetType { get; set; }

        [NoMapping]
        public string TargetTypeValue { get; set; }

        [NoMapping]
        public bool IsCheck { get; set; }




        [ORFieldMapping("MeasureRate")]
        public decimal? MeasureRate { get; set; }



        [ORFieldMapping("VersionStart")]
        public DateTime VersionStart { get; set; }



        [ORFieldMapping("VersionEnd")]
        public DateTime VersionEnd { get; set; }



        [ORFieldMapping("SystemType")]
        public int SystemType { get; set; }



        [ORFieldMapping("Configuration")]
        public System.Xml.Linq.XElement Configuration { get; set; }




        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }


        [ORFieldMapping("NeedReport")]
        public bool NeedReport { get; set; }

        [ORFieldMapping("BaseLine")]
        public DateTime BaseLine { get; set; }

        [ORFieldMapping("Unit")]
        public string Unit { get; set; }
        #endregion


    }
}

