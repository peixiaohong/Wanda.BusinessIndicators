using Lib.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using Wanda.Lib.AuthCenter.DAL;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.AuthCenter.ViewModel;
using Wanda.Lib.Data.AppBase;
using Lib.Data;

namespace Wanda.Lib.AuthCenter.BLL
{
    public class WD_UserOperator
    {
        public static WD_UserOperator Instance = new WD_UserOperator(); //BizOperatorFactory.Create<WD_UserOperator>();
        private static WD_UserAdapter _wdUserinfoAdapter = WD_UserAdapter.Instance;




        /// <summary>
        /// 获取用户详细信息根据用户Id
        /// </summary>
        /// <param name="wdUserId"></param>
        /// <returns></returns>
        public BUserinfo GetWDUserInfoByID(int wdUserId)
        {
            return _wdUserinfoAdapter.GetWDUserInfoByID(wdUserId);
        }

        /// <summary>
        /// 获取用户详细信息根据用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public BUserinfo GetWDUserInfoByUserName(string userName)
        {
            return _wdUserinfoAdapter.GetWDUserInfoByUserName(userName);
        }
        /// <summary>
        /// 模糊查询用户
        /// </summary>
        /// <param name="Con"></param>
        /// <returns></returns>
        //public PartlyCollection<WD_User> FindUser(string Con)
        //{
        //    return _userinfoAdapter.FindUser(Con);
        //}

        /// <summary>
        /// 分别按照姓名、登陆名和员工号进行匹配
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<VWdSimpleUserInfo> GetUserInfo(string keyword, int count)
        {
            WdSimpleUserDataFilter filter1 = new WdSimpleUserDataFilter { LoginName = keyword, PageIndex = 1, PageSize = count };
            WdSimpleUserDataFilter filter2 = new WdSimpleUserDataFilter { UserName = keyword, PageIndex = 1, PageSize = count };
            WdSimpleUserDataFilter filter3 = new WdSimpleUserDataFilter { EmployeeCode = keyword, PageIndex = 1, PageSize = count };

            List<VWdSimpleUserInfo> result = new List<VWdSimpleUserInfo>();
            result.AddRange(VSimleWdUserInfoAdapter.Instance.GetList(filter1));

            if (result.Count < count)
                result.AddRange(VSimleWdUserInfoAdapter.Instance.GetList(filter2));

            if (result.Count < count)
                result.AddRange(VSimleWdUserInfoAdapter.Instance.GetList(filter3));
            return result;


        }
    }
}
