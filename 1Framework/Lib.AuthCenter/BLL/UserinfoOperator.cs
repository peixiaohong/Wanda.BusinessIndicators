/*
 * Create by Hu Wei Zheng

 */
using Lib.Core;
using Lib.Data;
using Lib.Data.AppBase;
using Lib.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Wanda.Lib.AuthCenter.DAL;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.AuthCenter.ViewModel;
using Wanda.Lib.Data.AppBase;



namespace Wanda.Lib.AuthCenter.BLL
{
    /// <summary>
    /// Userinfo对象的业务逻辑操作
    /// </summary>
    public class UserinfoOperator : BizOperatorBase<BUserinfo>
    {
        public static UserinfoOperator Instance = BizOperatorFactory.Create<UserinfoOperator>(WebHelper.GetCurrentLoginUser, WebHelper.GetTimeNow);

        private static UserinfoAdapter _userinfoAdapter = AdapterFactory.GetAdapter<UserinfoAdapter>();

        protected override BaseAdapterT<BUserinfo> GetAdapter()
        {
            return _userinfoAdapter;
        }
        private static VUserInfoAdapter _vuserAdapter = AdapterFactory.GetAdapter<VUserInfoAdapter>();
        /// <summary>
        /// 获得所有的用户信息
        /// </summary>
        /// <returns></returns>
        public IList<BUserinfo> GetUserinfoList()
        {
            IList<BUserinfo> result = _userinfoAdapter.GetUserinfoList();
            return result;

        }


        /// <summary>
        /// 检测UserInfo 表中 对应的 WD_User表中的用户状态 返回离职状态的用户信息 
        /// </summary>
        /// <returns></returns>
        public PartlyCollection<VUserInfo> GetResignedEmployees()
        {
            UserInfoFilter filter = new UserInfoFilter();
            filter.IsApplicationUser = 1;
            filter.IsDeleted = 0;
            filter.Status = 3; //离职

            PartlyCollection<VUserInfo> result = _vuserAdapter.GetList(filter);
            return result;
        }


        /// <summary>
        /// 查找用户信息
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PartlyCollection<VUserInfo> GetUserinfoList(UserInfoFilter filter)
        {
            return _vuserAdapter.GetList(filter);
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Guid AddUserinfo(BUserinfo data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null,
                                                          "Argument data is Empty");
            Guid result = base.AddNewModel(data,
                            UserinfoValidator.Instance,
                            UserinfoUniqueValidator.Instance
                );
            return result;
        }

        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="userinfoID"></param>
        /// <returns></returns>
        public BUserinfo GetUserinfo(Guid userinfoID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>((userinfoID==null || userinfoID==Guid.Empty),
                                                                "Argument userinfoID is Empty");
            return base.GetModelObject(userinfoID);
        }

        /// <summary>
        /// 根据万达用户Id查询用户是否存在UserInfo表中
        /// </summary>
        /// <param name="wdUserId"></param>
        /// <returns></returns>
        public BUserinfo GetUserinfoByWDUserID(int wdUserId)
        {
            BUserinfo result = _userinfoAdapter.GetUserInfoByWDUserID(wdUserId);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>return data.ID</returns>
        public Guid UpdateUserinfo(BUserinfo data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null,
                                                            "Argument data is Empty");
            Guid result = base.UpdateModelObject(data,
                            UserinfoValidator.Instance,
                            UserinfoUniqueValidator.Instance);
            return result;
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="userinfoID"></param>
        /// <returns></returns>
        public Guid RemoveUserinfo(Guid userinfoID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userinfoID==null || userinfoID==Guid.Empty,
                                                             "Argument userinfoID is Empty");
            Guid result = base.RemoveObject(userinfoID);
            return result;
        }

        /// <summary>
        /// 找到指定角色下的用户
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<BUserinfo> GetUsersOfSpecificRole(Guid roleID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(roleID==null || roleID==Guid.Empty,
                                                              "Argument roleID is illegal. RoleID=" + roleID);

            IList<BUserinfo> result = _userinfoAdapter.GetUsersOfSpecificRole(roleID);
            return result;
        }


        /// <summary>
        /// 找到拥有指定权限的用户
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        /// <remarks>
        /// 可以考虑使用VUserPrivilege对象重写
        /// </remarks>
        public IList<BUserinfo> GetUsersOfSpecificPrivilege(Guid privilegeID)
        {
            // 
            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID == null || privilegeID == Guid.Empty,
                                                              "Argument privilegeID is Empty");


            HashSet<BUserinfo> result = new HashSet<BUserinfo>(); // 去除重复

            // 用户对应的权限 + 用户所在组对应的权限 

            IList<BRoleinfo> roles = RoleinfoOperator.Instance.GetGrantedRoles(privilegeID);

            foreach (BRoleinfo role in roles)
            {
                IList<BUserinfo> users = GetUsersOfSpecificRole(role.ID);

                foreach (BUserinfo u in users)
                {
                    bool isAdded = result.Add(u);
                }
            }

            // .. Directed Granded

            IList<BUserinfo> users_Directed = GetDirectedGrandedUsersOfPrivilege(privilegeID);

            foreach (BUserinfo u in users_Directed)
            {
                bool isAdded = result.Add(u);
            }

            return result.ToList();
        }

        /// <summary>
        /// 检查用户是否有指定的权限， 如果有， 返回True
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="privilegeID"></param>
        /// <returns></returns>
        /// <remarks>
        /// 注意 Role有禁用的情况， 需要排除
        /// </remarks>
        public bool CheckUserPrivilege(Guid userID, Guid privilegeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userID == null || userID == Guid.Empty,
                                                              "Argument userID is Empty");

            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID == null || privilegeID == Guid.Empty,
                                                              "Argument privilegeID is Empty");

            IList<BRoleinfo> roles = RoleinfoOperator.Instance.GetGrantedRoles(privilegeID);

            BRoleinfo AllUserRole = roles.FirstOrDefault(r => r.ID == BRoleinfo.AllUserRoleID);

            if (AllUserRole != null)
            {
                return true;
            }

            var result = VUserPrivilegeAdapter.Instance.GetUserPrivilege(userID, privilegeID)
                        .Where(p => p.RoleForbidden == false);
            return result.Count() > 0;
        }

        /// <summary>
        /// 管理用户所属于的角色
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public int ManagedUserRoles(Guid userID, List<Guid> roleIDs)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userID == null || userID == Guid.Empty,
                                                           "Argument userID is Empty");

            if (roleIDs == null || roleIDs.Count == 0)
            {
                int result = _userinfoAdapter.ManagedUserRoles(userID, null);
                return result;
            }
            else
            {
                // 找到用户信息
                BUserinfo user = GetUserinfo(userID);

                // 找到角色信息
                Dictionary<Guid, BRoleinfo> roleInfos = RoleinfoOperator.Instance.GetRoleinfoList(roleIDs).ToDictionary(
                          key => key.ID,
                          value => value
                       );

                List<AUserRole> newUserRoleMappings = roleIDs.Select(roleID =>
                   new AUserRole()
                   {
                       UserID = userID,
                       UserName = user.Name,
                       RoleID = roleID,
                       RoleName = roleInfos[roleID].Name,
                       CreatorName = GetCurrentUserName(),
                       ModifierName = GetCurrentUserName(),
                       CreateTime = GetDateTimeNow(),
                       ModifyTime = GetDateTimeNow()
                   }).ToList();


                int result = _userinfoAdapter.ManagedUserRoles(userID, newUserRoleMappings);
                return result;
            }


        }

        /// <summary>
        /// 管理用户直接对应的授权
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="privilegeIDs"></param>
        /// <returns></returns>
        public int ManagedUserDirectedPermissions(Guid userID, List<Guid> privilegeIDs)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userID == null || userID == Guid.Empty,
                                                           "Argument userID is Empty");

            if (privilegeIDs == null || privilegeIDs.Count == 0)
            {
                int result = _userinfoAdapter.ManagedUserPermissions(userID, null);
                return result;
            }
            else
            {
                // 找到用户信息
                BUserinfo user = GetUserinfo(userID);

                // 找到角色信息
                Dictionary<Guid, BPrivilege> privilegeInfos = PrivilegeOperator.Instance.GetPrivilegeList(privilegeIDs).ToDictionary(
                          key => key.ID,
                          value => value
                       );

                List<AUserPrivilege> newUserPrivilegeMappings = privilegeIDs.Select(privilegeID =>
                   new AUserPrivilege()
                   {
                       UserID = userID,
                       UserName = user.Name,
                       PrivilegeID = privilegeID,
                       PrivilegeName = privilegeInfos[privilegeID].Name,
                       CreatorName = GetCurrentUserName()

                   }).ToList();


                int result = _userinfoAdapter.ManagedUserPermissions(userID, newUserPrivilegeMappings);
                return result;
            }


        }

        /// <summary>
        /// 获得直接授权的用户
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <returns></returns>
        public IList<BUserinfo> GetDirectedGrandedUsersOfPrivilege(Guid privilegeID)
        {

            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID==null || privilegeID==Guid.Empty,
                                                              "Argument privilegeID is Empty");

            IList<BUserinfo> result = _userinfoAdapter.GetUsersOfSpecificPrivilege(privilegeID);
            return result;
        }

        #region Validators

        public class UserinfoValidator : IValidator<BUserinfo>
        {
            public static UserinfoValidator Instance = new UserinfoValidator();
            public ValidateResult Validate(BUserinfo data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.LengthControl(data.Name, "Name", 1, 16, ref result);
                return result;
            }
        }
        public class UserinfoUniqueValidator : IValidator<BUserinfo>
        {
            public static UserinfoUniqueValidator Instance = new UserinfoUniqueValidator();
            public ValidateResult Validate(BUserinfo data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.UniqueField(data, "Name_JobTitle", CheckNameUnique, ref result);

                return result;
            }
            private static UserinfoAdapter _UserinfoAdapter = AdapterFactory.GetAdapter<UserinfoAdapter>();

            /// <summary>
            /// 检查是否有重复
            /// </summary>
            /// <param name="name"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            private bool CheckNameUnique(object obj)
            {
                BUserinfo data = (BUserinfo)obj;
                IList<BUserinfo> Userinfos = _UserinfoAdapter.GetExists(data.Name, data.JobTitle, data.ID);
                if (Userinfos.Count > 0)
                {
                    return false;
                }

                return true;
            }

        }
        #endregion

        /// <summary>
        /// 获得多个用户的信息
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        public List<BUserinfo> GetUserinfoList(List<Guid> userIDs)
        {

            ExceptionHelper.TrueThrow<ArgumentNullException>(userIDs == null || userIDs.Count == 0,
                     "至少需要一个参数！");

            List<BUserinfo> result = _userinfoAdapter.GetBatchModelObjects(userIDs).ToList();
            return result;

        }


        public BUserinfo ValidateUser(string strUserName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(strUserName),
         "用户名和密码不允许为空！");

            BUserinfo userInfo = _userinfoAdapter.ValidateUser(strUserName, string.Empty);
            return userInfo;
        }
        public static LoginUserInfo GetGenericIdentityExt(string strUserName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(strUserName),
         "用户名和密码不允许为空！");

            BUserinfo userInfo = _userinfoAdapter.ValidateUser(strUserName, string.Empty);
            LoginUserInfo genericUser = userInfo == null ? null : new LoginUserInfo()
            {
                UserID = userInfo.ID,
                Department = userInfo.Department,
                DisplayName = userInfo.DisplayName,
                //Gender = userInfo.Gender,
                Status = userInfo.Status,
                JobTitle = userInfo.JobTitle,
                LoginName = userInfo.LoginName,
                Name = userInfo.Name,
                Phone = userInfo.Phone,
                WD_UserID = userInfo.WD_UserID
            };
            return genericUser;
        }

        /// <summary>
        /// 根据用户名称找到用户  
        /// </summary> 
        /// <param name="userName"></param>
        /// <returns></returns>
        public BUserinfo GetUserInfoByName(string userName)
        {
            return _userinfoAdapter.GetUserInfoByName(userName);
        }



        /// <summary>
        /// 根据用户ID， 通过查找角色--默认首页的配置，找到用户的默认首页
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="defaultUrlWhenNotFound">缺省的默认首页</param>
        /// <returns></returns>
        public VUserDefaultUrl GetUserDefaultUrl(Guid userID, VUserDefaultUrl defaultUrlWhenNotFound)
        {
            PartlyCollection<VUserDefaultUrl> urls = VUserDefaultUrlAdapter.Instance.GetList(userID);

            if (urls == null || urls.Count == 0)
            {
                return defaultUrlWhenNotFound;
            }

            return urls.OrderBy(p => p.Weight).ThenBy(p => p.ModifyTime).Last();
        }

        public int UpUserInfoJob()
        {
            return _userinfoAdapter.UpdateUserInfoJob();
        }
    }
}

