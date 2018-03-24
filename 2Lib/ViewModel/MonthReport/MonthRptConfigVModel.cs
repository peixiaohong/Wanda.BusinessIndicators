using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.ViewModel.MonthReport
{
    public class MonthRptConfigVModel : BaseModel, IBaseComposedModel
    {
        [ORFieldMapping("SystemName")]
        public string SystemName { get; set; }

        [ORFieldMapping("Category")]
        public int Category { get; set; }

        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }

        [ORFieldMapping("Configuration")]
        [NonSerialized]
        public System.Xml.Linq.XElement Configuration;

        [NoMapping]
        [NonSerialized]
        public System.Xml.Linq.XElement SummaryTemplate;

        [NoMapping]
        [NonSerialized]
        public System.Xml.Linq.XElement DetailTemplate;

        [NoMapping]
        [NonSerialized]
        public System.Xml.Linq.XElement MissTargetTemplate;

        [NoMapping]
        [NonSerialized]
        public System.Xml.Linq.XElement ReturnTemplate;

    }
}
