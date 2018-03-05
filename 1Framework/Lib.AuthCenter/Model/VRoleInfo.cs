using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Wanda.Lib.Data.AppBase;
using Lib.Xml;

namespace Wanda.Lib.AuthCenter.Model
{
    [Serializable]
    [ORViewMapping(@"
SELECT  a.[ID]
      ,a.[Name]
      ,a.[ScopeID]
      ,cong.Name [ScopeName]
      ,a.[Comment]
      ,a.[IsForbidden]
      ,a.[ModifyTime]
      , (SELECT b.ID '@value', b.name '@text' from PB_Privilege b 
                   inner join PA_RolePrivilege c on b.id=c.privilegeid
                   where c.roleid= a.id and b.isdeleted=0 and c.isdeleted=0
                   FOR XML  PATH('item'), ROOT('root')) as PrivilegesXml
  FROM  [PB_RoleInfo] a 
  inner join c_conglomerate cong on a.scopeid=cong.id
  where a.isdeleted=0 and cong.isdeleted=0",
        "RoleInfoView")
     ]
    public class VRoleInfo : IBaseComposedModel
    {
        [ORFieldMapping("ID")]
        public int RoleID { set; get; }

        [ORFieldMapping("IsForbidden")]
        public bool IsForbidden { set; get; }


        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { set; get; }

        [NoMapping]
        public string StatusText
        {
            get
            {
                return IsForbidden ? "禁用" : "启用";
            }
        }

        [ORFieldMapping("Name")]
        public string RoleName { set; get; }

        [ORFieldMapping("ScopeID")]
        public int ScopeID { set { value = 1; } get { return 1; } }

        [ORFieldMapping("ScopeName")]
        public string ScopeName { set; get; }

        [ORFieldMapping("Comment")]
        public string CommentText { set; get; }


        [ORFieldMapping("PrivilegesXml")]
        [NonSerialized]
        public XElement PrivilegesXml;

        [NoMapping]
        public List<ValueText<int>> PrivilegesList
        {
            get
            {
                return PrivilegesXml.CreateValueTextList<int>();
            }
        }
    }


}
