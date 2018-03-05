using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using System.ComponentModel;
using Lib.Core;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.Model
{
    /// <summary>             
    /// This object represents the properties and methods of a Privilege.
    /// </summary>
    [ORTableMapping("dbo.PB_Privilege")]
    public class BPrivilege : BaseModel 
    {

        #region Public Properties

        /// <summary>
        /// 如果是Url， 则显示PagePath 和 QueryString， 格式如 page.aspx?key=v1
        /// 如果是Action， 则是PagePath + Action, 格式如 page.aspx?key=v1::action
        /// </summary>

        /// <summary>
        /// 显示名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }


        /// <summary>
        /// 代码
        /// </summary>
        [ORFieldMapping("Code")]
        public string Code { get; set; }

        /// <summary>
        /// Enum： Url=0, Action=1,Industry=2
        /// </summary>
        [ORFieldMapping("PrivilegeType")]
        public string PrivilegeType { get; set; }



        [ORFieldMapping("GroupID")]
        public string GroupID { get; set; }

        [ORFieldMapping("GroupName")]
        public string GroupName { get; set; }


        //[ORFieldMapping("SortNo")]
        //public int SortNo { get; set; }
        #endregion


    }

    public enum PrivilegeType
    {
        [EnumItemDescription("页面地址")]
        Url = 0,

        [EnumItemDescription("页面操作")]
        Action = 1, //Action的代码必须以指定的Url为开头


        [EnumItemDescription("业态")]
        Industry = 2
    }
}

