using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a Organization.
    /// </summary>
    [ORTableMapping("dbo.BOrganization")]
    public class BOrganization : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("Name")]
        public string Name { get; set; }



        [ORFieldMapping("DisplayName")]
        public string DisplayName { get; set; }



        [ORFieldMapping("NcCode")]
        public string NcCode { get; set; }



        [ORFieldMapping("ParentID")]
        public string ParentID { get; set; }



        [ORFieldMapping("OrgType")]
        public int OrgType { get; set; }



        [ORFieldMapping("IsVirtual")]
        public bool IsVirtual { get; set; }



        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }



        [ORFieldMapping("Comment")]
        public string Comment { get; set; }



        [ORFieldMapping("IsLocked")]
        public bool IsLocked { get; set; }


        #endregion


    }
}

