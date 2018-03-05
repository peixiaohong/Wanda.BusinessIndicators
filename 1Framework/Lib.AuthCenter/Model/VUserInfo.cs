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
select  
     isnull(ui.id, 0) as ID,
     (case   when ui.id is null then 0 else 1 end ) as IsApplicationUser,
     ui.IsDeleted,
     a.userName, 
     a.employeeName, 
     a.employeeCode,  
     a.employeeStatus,
     a.employeeStatusName,  
     a.mobile, 
     a.jobName, 
     a.orgID, 
     a.unitName,
     a.userName+'|'+a.employeeName+'|'+a.employeeCode as keyword,
      (select b.Id '@value', b.name + '('+cong.name+')' '@text' from PB_RoleInfo b
                    inner join Pa_userrole  c on b.id=c.roleid
                    inner join c_conglomerate cong on cong.id = b.scopeid
                where c.userid= ui.id and c.isdeleted=0   
						and b.isdeleted=0  and b.IsForbidden=0 and cong.isdeleted=0  
                FOR XML  PATH('item'), ROOT('root')) as RolesString
from wd_user  a
    left join pb_userinfo ui   on ui.wd_userid = a.employeecode   ",
        "UserListView")
     ]
    public class VUserInfo : IBaseComposedModel
    {
        [ORFieldMapping("ID")]
        public int UserID { set; get; }

        [ORFieldMapping("IsDeleted")]
        public bool IsDeleted { set; get; }

        /// <summary>
        /// true表示是系统用户
        /// </summary>
        [ORFieldMapping("IsApplicationUser")]
        public bool IsApplicationUser { set; get; }

        [ORFieldMapping("username")]
        public string UserLoginName { set; get; }

        [ORFieldMapping("employeeCode")]
        public string Wd_UserID { set; get; }

        [ORFieldMapping("employeeName")]
        public string UserName { set; get; }

        [ORFieldMapping("employeeStatus")]
        public int Status { set; get; }


        [ORFieldMapping("employeeStatusName")]
        public string StatusText { set; get; }

        [ORFieldMapping("mobile")]
        public string PhoneNumber { set; get; }

        [ORFieldMapping("JobName")]
        public string JobTitle { set; get; }


        [ORFieldMapping("orgid")]
        public int OrgID { set; get; }

        [ORFieldMapping("unitName")]
        public string Department { set; get; }


        [ORFieldMapping("RolesString")]
        [NonSerialized]
        public XElement RolesString;

        [NoMapping]
        public List<ValueText<int>> RolesList
        {
            get
            {
                return RolesString.CreateValueTextList<int>();
            }
        }
    }


}
