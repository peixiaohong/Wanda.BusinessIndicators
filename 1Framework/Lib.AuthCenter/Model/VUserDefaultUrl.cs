using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    [ORViewMapping(@"
select distinct 
    u.ID as userID, 
    url.UrlName,
    url.UrlAbsolutePath, 
    rd.Weight, 
    rd.QueryString,
    rd.CreateTime as CreateTime,
    rd.ModifyTime as ModifyTime
from PB_USERINFO u
    inner join PA_USERROLE ur  on u.ID=ur.UserID
    inner join PB_ROLEINFO r on ur.RoleID = r.ID
    inner join PA_ROLEDEFAULTURL rd on rd.RoleID = r.ID
    inner join PC_URLINFO url on url.ID=rd.UrlID

where u.IsDeleted=0 and
 ur.IsDeleted=0 and
  r.IsDeleted=0 and
   rd.IsDeleted=0 and
  url.IsDeleted=0 and r.IsForbidden=0
", "VUserDefaultUrl")]
    public class VUserDefaultUrl : IBaseComposedModel
    {

 
      

        [ORFieldMapping("userID")]
        public int UserID { get; set; }


        [ORFieldMapping("UrlName")]
        public string UrlName { get; set; }

        [ORFieldMapping("UrlAbsolutePath")]
        public string UrlAbsolutePath { get; set; }

        [ORFieldMapping("QueryString")]
        public string QueryString { get; set; }

        [ORFieldMapping("Weight")]
        public int Weight { get; set; }

        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { get; set; }
    }
}
