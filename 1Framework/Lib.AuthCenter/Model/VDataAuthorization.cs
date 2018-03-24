using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    [ORViewMapping(@" 
select p.UserID,
    p.RoleID,
    p.BusinessID ,
    p.BusinessType,
    W.shortName,
    W.orgID,
    p.IsDeleted
     
from PB_DataAuthorization p  
    INNER JOIN Wd_Org W ON p.BusinessID=W.orgID",
                                                
      "UserRoleDataViewmodel")]
    public sealed class VDataAuthorization : IBaseComposedModel
    {
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }
        [ORFieldMapping("RoleID")]
        public int RoleID { get; set; }
        [ORFieldMapping("BusinessType")]
        public string BusinessType { get; set; }
        [ORFieldMapping("BusinessID")]
        public string BusinessID { get; set; }
        [ORFieldMapping("shortName")]
        public string shortName { get; set; }
        [ORFieldMapping("IsDeleted")]
        public bool IsDeleted { get; set; }
        [ORFieldMapping("orgID")]
        public string orgID { get; set; }
    }
}




