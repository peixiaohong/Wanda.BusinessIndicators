using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class V_SubReport
    {
        public V_SubReport()
        { 
            
        }

        public V_SubReport(Guid SystemID, string SystemName, Guid ReportID, bool IsReady)
        {
            this.SystemID = SystemID;
            this.SystemName = SystemName;
            this.ReportID = ReportID;
            this.IsReady = IsReady;
        }

        /// <summary>
        /// 系统ID
        /// </summary>
        public Guid SystemID { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 月报ID
        /// </summary>
        public Guid ReportID  { get; set; }

        /// <summary>
        /// 是否准备
        /// </summary>
        public bool IsReady { get; set; }

    }
}
