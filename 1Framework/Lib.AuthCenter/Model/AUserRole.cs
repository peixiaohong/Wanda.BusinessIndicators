using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;



namespace Wanda.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a UserRole.
    /// </summary>
    [ORTableMapping("dbo.PA_UserRole")]
    public class AUserRole : BaseModel
    {
         
        #region Public Properties



        [ORFieldMapping("RoleID")]
        public Guid RoleID { get; set; }

        [ORFieldMapping("RoleName")]
        public string RoleName { get; set; }


        [ORFieldMapping("UserID")]
        public Guid UserID { get; set; }

        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

       

        #endregion


    }
}

