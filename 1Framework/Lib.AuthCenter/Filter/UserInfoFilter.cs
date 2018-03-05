using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.Model
{
    [Serializable]
    public class UserInfoFilter : PagenationDataFilter, IDataFilter
    {

        [FilterField("ID", "in")]
        public List<int> UserIds { get; set; }


        [FilterField("employeeCode", "in")]
        public List<int> WdUserCodes { get; set; }

        //用户名称
        [FilterField("employeeName", "like")]
        public string Name { get; set; }


        //登录名称
        [FilterField("UserName","like")]
        public string LoginName { get; set; }


        //登录名称
        [FilterField("Keyword", "like")]
        public string Keyword { get; set; }

        //在职状态
        [FilterField("employeeStatus", DefaultV = -1)]
        public int Status { get; set; }


        //单位名称
        [FilterField("unitName", "like")]
        public string Department { get; set; }



        //是否删除
        [FilterField("IsDeleted", DefaultV=-1 )]
        public int IsDeleted { get; set; }


        //是否系统用户
        [FilterField("IsApplicationUser", DefaultV = -1)]
        public int IsApplicationUser { get; set; }

        public UserInfoFilter()
        {
            Status = -1;
            IsDeleted = -1;
            IsApplicationUser = -1;
        }
    }
}
