using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.Model
{
    [ORTableMapping("dbo.PB_DataAuthorization")]

     [Serializable]
     public class BDataAuthorization: BaseModel
    {
        #region Public Properties

        [ORFieldMapping("UserID")]
        public string UserID { get; set; }

        [ORFieldMapping("RoleID")]
        public int RoleID { get; set; }

        [ORFieldMapping("BusinessType")]
        public string BusinessType { get; set; }

        [ORFieldMapping("BusinessID")]
        public string BusinessID { get; set; }

        #endregion

    }
}

    

 
        