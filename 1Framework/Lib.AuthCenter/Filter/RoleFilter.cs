using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    [Serializable]
    public class RoleFilter : PagenationDataFilter, IDataFilter
    {

        [FilterFieldAttribute("ID", DefaultV = 0)]
        public int RoleID { get; set; }

        [FilterFieldAttribute("Name", "like")]
        public string RoleName { get; set; }

        [FilterFieldAttribute("Comment", "like")]
        public string Comment { get; set; }

        [FilterFieldAttribute("IsForbidden", DefaultV = -1)]
        public int IsForbidden { get; set; }

        public RoleFilter()
        {
            IsForbidden = -1;
        }

    }
}
