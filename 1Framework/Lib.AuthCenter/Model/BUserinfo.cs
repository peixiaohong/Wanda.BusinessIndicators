using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;



namespace Wanda.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a Userinfo.
    /// </summary>
    [ORTableMapping("dbo.PB_UserInfo")]
    public  class BUserinfo : BaseModel
    {
        #region Public Properties

        [ORFieldMapping("Name")]
        public string Name { get; set; }

        [ORFieldMapping("WD_UserID")]
        public int WD_UserID { get; set; }


        [ORFieldMapping("LoginName")]
        public string LoginName { get; set; }

        [ORFieldMapping("DisplayName")]
        public string DisplayName { get; set; }



        [ORFieldMapping("Department")]
        public string Department { get; set; }

        [ORFieldMapping("JobTitle")]
        public string JobTitle { get; set; }

        //[ORFieldMapping("Gender")]
        //public string Gender { get; set; }


        [ORFieldMapping("Phone")]
        public string Phone { get; set; }


        /// <summary>
        /// 需要解释Status的值
        /// </summary>
        [ORFieldMapping("Status")]
        public string Status { get; set; }

        [ORFieldMapping("GroupName")]
        public string GroupName { get; set; }

        #endregion
    }
}

