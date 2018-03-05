using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Lib.Xml;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.Model
{
    /// <summary>
    /// 用户角色授权视图实体， 
    /// </summary>
    /// <remarks>
    /// 视图实体类， 统一约定以“V”开通
    /// </remarks>
    [ORViewMapping(@"
 select u.id as UserID,
        u.Name as UserName, 
        ur.Roleid, 
        r.Name as RoleName, 
        r.IsForbidden as RoleForbidden, 
        rp.PrivilegeID, 
        p.Name as PrivilegeName , 
        p.PrivilegeType, 
        P.Code,
        (case p.PrivilegeType
                when 'URL' then (select  p2.ID '@value', p2.Name '@text' from [PB_Privilege] p2 
                                  where p2.code like p.code+'#%' 
                              FOR XML  PATH('item'), ROOT('items'))
                else null END ) as Actions
from [PB_Userinfo] u
    left join [PA_UserRole] ur on u.ID=ur.UserID 
    inner join [PB_RoleInfo] r on r.ID=ur.RoleID
    inner join [PA_RolePrivilege] rp on rp.RoleID = r.id
    inner join [PB_Privilege] p on rp.PrivilegeID = p.ID
where u.IsDeleted<1 and r.IsDeleted<1 and ur.IsDeleted<1 and rp.IsDeleted<1 and p.IsDeleted<1 
 ",
        "UserPrivilegeView")]
    public class VUserPrivilege : IBaseComposedModel
    {
 
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
        [ORFieldMapping("RoleID")]
        public string RoleID { get; set; }
        [ORFieldMapping("RoleName")]
        public string RoleName { get; set; }
        [ORFieldMapping("RoleForbidden")]
        public bool RoleForbidden { get; set; }
        [ORFieldMapping("PrivilegeID")]
        public string PrivilegeID { get; set; }
        [ORFieldMapping("PrivilegeName")]
        public string PrivilegeName { get; set; }
        [ORFieldMapping("PrivilegeType")]
        public string PrivilegeType { get; set; }
        [ORFieldMapping("Code")]
        public string Code { get; set; }


        [ORFieldMapping("Actions")]
        [NonSerialized]
        public XElement XmlActions;

        [NoMapping]
        public List<ValueText<int>> Actions { get {
            return XmlActions.CreateValueTextList<int>();
        } }
    }
}
