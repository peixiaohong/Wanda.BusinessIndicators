using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a Permission.
    /// </summary>
    [ORTableMapping("dbo.PA_RolePrivilege")]
    public class ARolePrivilege : BaseModel
    {
        #region Public Properties

        [ORFieldMapping("PrivilegeID")]
        public Guid PrivilegeID { get; set; }

        [ORFieldMapping("PrivilegeName")]
        public string PrivilegeName { get; set; }

        [ORFieldMapping("RoleID")]
        public Guid RoleID { get; set; }

        [ORFieldMapping("RoleName")]
        public string RoleName { get; set; }

        [ORFieldMapping("IsGranted")]
        public bool IsGranted { get; set; }

        #endregion
    }
}
