using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a WD_User.
    /// </summary>
    [ORTableMapping("dbo.WD_User")]
    [Serializable]
    public class WD_User  
    {
        #region Public Properties

        [ORFieldMapping("Wd_UserID")]
        public int Wd_UserID { get; set; }
        [ORFieldMapping("Name")]
        public string Name { get; set; }
        [ORFieldMapping("LoginName")]
        public string LoginName { get; set; }
        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }
        [ORFieldMapping("EndTime")]
        public DateTime EndTime { get; set; }
        [ORFieldMapping("Wd_OrgID")]
        public int Wd_OrgID { get; set; }
        [ORFieldMapping("OrgName")]
        public string OrgName { get; set; }
        [ORFieldMapping("JobID")]
        public int Wd_posID { get; set; }
        [ORFieldMapping("JobTitle")]
        public string PosName { get; set; }
        #endregion
    }
}
