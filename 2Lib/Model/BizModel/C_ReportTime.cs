using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
    [ORTableMapping("dbo.C_ReportTime")]
    public class C_ReportTime : BaseModel
    {
        #region Public Properties

        [ORFieldMapping("ReportTime")]
        public DateTime? ReportTime { get; set; }

        [ORFieldMapping("SysOpenDay")]
        public DateTime? SysOpenDay { get; set; }

        [ORFieldMapping("OpenStatus")]
        public string OpenStatus { get; set; }

        [ORFieldMapping("WantTime")]
        public DateTime? WantTime { get; set; }

        #endregion
    }
}
