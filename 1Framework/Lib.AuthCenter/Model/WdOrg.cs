using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;

namespace LJTH.Lib.AuthCenter.Model
{
    [ORTableMapping("dbo.wd_org")]
    public  class WdOrg
    {
        /// <summary>
        /// 如果父编号为NULL,则为0
        /// </summary>
        [ORFieldMapping("ParentID")]
        public int ParentID { get; set; }

        [ORFieldMapping("OrgID")]
        public int OrgID { get; set; }

        [ORFieldMapping("OrgCode")]
        public string OrgCode { get; set; }

        [ORFieldMapping("OrgName")]
        public string OrgName { get; set; }

        [ORFieldMapping("ShortName")]
        public string ShortName { get; set; }

        [ORFieldMapping("OrderID")]
        public int OrderID { get; set; }


        [ORFieldMapping("FullPath")]
        public string FullPath { get; set; }
    }
}
