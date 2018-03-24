using Lib.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.Lib.AuthCenter.ViewModel
{
    public class LoginUser
    {
        public string LoginUserID { get; set; }
        public string LoginUserName { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime LoginTime { get; set; }
    }

    public class LoginUserInfo
    {
        public Guid UserID { get; set; }

        public int WD_UserID { get; set; }

        public string Name { get; set; }

        public string LoginName { get; set; }

        public string DisplayName { get; set; }

        public string Department { get; set; }

        public string JobTitle { get; set; }

        public string Gender { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }
    }

    public class LoginUserInfoCache : CacheQueue<String, LoginUserInfo>
    {
        public static readonly LoginUserInfoCache Instance = CacheManager.GetInstance<LoginUserInfoCache>();

        private LoginUserInfoCache()
        {
        }
    }
}
