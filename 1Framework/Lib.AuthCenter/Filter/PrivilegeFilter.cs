using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    /// <summary>
    ///权限条件查询 
    /// </summary>
    [Serializable]
    public class PrivilegeFilter : PagenationDataFilter, IDataFilter
    {

        [FilterFieldAttribute("ID")]
        public string PrivilegeID { get; set; }

        [FilterFieldAttribute("Name", " like ")]
        public string PrivilegeName { get; set; }

        [FilterFieldAttribute("PrivilegeType")]
        public string PrivilegeType { get; set; }

        [FilterFieldAttribute("GroupName")]
        public string GroupName { get; set; }

        public PrivilegeFilter()
        {
        }
    }

}
