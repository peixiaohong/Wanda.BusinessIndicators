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
    [ORTableMapping("dbo.PA_UserPrivilege")]
    public class AUserPrivilege : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("PrivilegeID")]
        public Guid PrivilegeID { get; set; }

        [ORFieldMapping("PrivilegeName")]
        public string PrivilegeName { get; set; }


        [ORFieldMapping("UserID")]
        public Guid UserID { get; set; }

        [ORFieldMapping("UserName")]
        public string UserName { get; set; }


        [ORFieldMapping("ScopeID")]
        public string ScopeID { get; set; }


       

        #endregion


    }
}

