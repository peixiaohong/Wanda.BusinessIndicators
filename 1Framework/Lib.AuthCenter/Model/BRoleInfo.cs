using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;



namespace Wanda.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a Roleinfo.
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.PB_RoleInfo")]
    public class BRoleinfo : BaseModel
    {
         

        #region Public Properties



        [ORFieldMapping("Name")]
        public string Name { get; set; }



        [ORFieldMapping("Comment")]
        public string Comment { get; set; }



        [ORFieldMapping("ScopeID")]
        public Guid ScopeID { set { value = Guid.Empty; } get { return Guid.Empty; } }



        [ORFieldMapping("IsForbidden")]
        public bool IsForbidden { get; set; }


        #endregion

        public static Guid AllUserRoleID = Guid.Empty;
         
    }
}

