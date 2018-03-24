using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using System.ComponentModel;
using Lib.Core;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.Model
{
    /// <summary>
    /// This object represents the properties and methods of a Privilege.
    /// </summary>
    [ORTableMapping("dbo.PB_PrivilegeGroup")]
    public class BPrivilegeGroup : BaseModel
    {

        #region Public Properties

      

        /// <summary>
        /// 显示名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }


      

        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }

        #endregion


    }

    
}

