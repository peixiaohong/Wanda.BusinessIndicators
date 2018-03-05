/*
 * Create by Hu Wei Zheng

 */
using Lib.Core;
using Lib.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Wanda.Lib.AuthCenter.DAL;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;
using System.Web;
using Wanda.Lib.AuthCenter.ViewModel;
using Lib.Data.AppBase;
using Lib.Data;


namespace Wanda.Lib.AuthCenter.BLL
{
    /// <summary>
    /// BPrivilege对象的业务逻辑操作
    /// </summary>
    public class PrivilegeOperator : BizOperatorBase<BPrivilege>
    {
        public static PrivilegeOperator Instance = BizOperatorFactory.Create<PrivilegeOperator>(WebHelper.GetCurrentLoginUser, WebHelper.GetTimeNow);

        private PrivilegeAdapter _privilegeAdapter = AdapterFactory.GetAdapter<PrivilegeAdapter>();

        protected override BaseAdapterT<BPrivilege> GetAdapter()
        {
            return _privilegeAdapter;
        }
        /// <summary>
        /// 获得全部的权限信息
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 考虑到不会有太多， 所以一次全部返回结果
        /// </remarks>
        public List<BPrivilege> GetPrivilegeList()
        {
            List<BPrivilege> result = _privilegeAdapter.GetPrivilegeList();
            return result;

        }

        /// <summary>
        /// 根据类型Id获取权限 
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeListByTypeId(int typeId)
        {
            List<BPrivilege> result = _privilegeAdapter.GetPrivilegeListByTypeId(typeId);
            return result;
        }

        public IList<BPrivilege> GetPrivilegeListByGroupName(string groupname)
        {
            IList<BPrivilege> result = _privilegeAdapter.GetPrivilegeByGroupID(groupname);
            return result;
        }

        public IList<BPrivilege> GetPrivilegeListAction()
        {
            List<BPrivilege> result = _privilegeAdapter.GetPrivilegeListAction();
            return result;
        }

        //public IList<BPrivilege> GetPrivilegesByUrl(string absoluteUrlPath)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 分页
        ///// </summary>
        ///// <param name="pageSize"></param>
        ///// <param name="filter"></param>
        ///// <returns></returns>
        //public PartlyCollection<VPrivilege> GetPrivilegeListByPage(PrivilegeFilter filter)
        //{
        //    ExceptionHelper.TrueThrow<ArgumentNullException>(filter == null, "过滤参数对象不能为空");

        //    PartlyCollection<VPrivilege> result = null;//  _privilegeAdapter.GetResearchResult(filter);

        //    return result;
        //}


        /// <summary>
        /// 新添加权限信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Guid AddPrivilege(BPrivilege data)
        {
            ExceptionHelper.TrueThrow<MissingParameterException>(data == null,
                                                          "Argument data is Empty");
            Guid result = base.AddNewModel(data,
                                            new BPrivilegeUniqueValidator(),
                                           new BPrivilegeValidator());
            return result;
        }

        /// <summary>
        /// 根据ID查找权限信息
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <returns></returns>
        public BPrivilege GetPrivilege(Guid privilegeID)
        {
            ExceptionHelper.TrueThrow<MissingParameterException>(privilegeID ==null || privilegeID==Guid.Empty,
                                                                "Argument privilegeID is Empty");
            return base.GetModelObject(privilegeID);
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns>return data.ID</returns>
        public Guid UpdatePrivilege(BPrivilege data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null,
                                                            "Argument data is Empty");
            Guid result = base.UpdateModelObject(data,
                                            new BPrivilegeUniqueValidator(),
                                           new BPrivilegeValidator());
            return result;
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <returns></returns>
        public Guid RemovePrivilege(Guid privilegeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID==null || privilegeID==Guid.Empty,
                                                             "Argument privilegeID is Empty");

            //：权限删除前，需要清除指定的权限-角色映射关系
            _privilegeAdapter.ManagedPrivilegeRoles(privilegeID, null);
            //：权限删除前，需要清除指定的权限-人员映射关系
            _privilegeAdapter.ManagedPrivilegeUsers(privilegeID, null);

            Guid result = base.RemoveObject(privilegeID);
            return result;
        }

        public IList<BPrivilege> GetPrivilegesByUser(Guid userID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userID ==null || userID==Guid.Empty,
                                                        "Argument userID is Empty");

            // 用户对应的权限 + 用户所在组对应的权限 
            HashSet<BPrivilege> result = new HashSet<BPrivilege>();

            foreach (BRoleinfo role in RoleinfoOperator.Instance.GetBelongsToRoles(userID))
            {
                foreach (BPrivilege p in PrivilegeOperator.Instance.GetPrivilegesByRole(role.ID))
                {
                    bool isAdded = result.Add(p);
                }
            }

            // Directed
            foreach (BPrivilege p in GetDirectedPrivilegesByUser(userID))
            {
                bool isAdded = result.Add(p);
            }

            return result.ToList();
        }

        internal IList<BPrivilege> GetDirectedPrivilegesByUser(Guid userID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userID ==null || userID==Guid.Empty,
                                                        "Argument userID is Empty");

            IList<BPrivilege> result = _privilegeAdapter.GetDirectedPrivilegesOfUser(userID);

            return result;
        }

        /// <summary>
        /// 返回角色对应的权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<BPrivilege> GetPrivilegesByRole(Guid roleID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(roleID == null || roleID == Guid.Empty,
                                                       "Argument roleID is Empty");
            IList<BPrivilege> result = _privilegeAdapter.GetPrivilegesOfRole(roleID);
            //IList<BPrivilege> result = _privilegeAdapter.GetPrivilegesOfRole(1);
            return result;
        }



        /// <summary>
        /// 管理、更新权限所属的角色
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public int ManagedPrivilegeRoles(Guid privilegeID, List<Guid> roleIDs)
        {

            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID==null || privilegeID==Guid.Empty,
                                                        "Argument privilegeID is Empty");

            if (roleIDs == null || roleIDs.Count == 0)
            {
                int result = _privilegeAdapter.ManagedPrivilegeRoles(privilegeID, null);
                return result;
            }
            else
            {
                // 找到权限信息
                BPrivilege privilege = GetPrivilege(privilegeID);

                // 找到角色信息
                Dictionary<Guid, BRoleinfo> roles = RoleinfoOperator.Instance.GetRoleinfoList(roleIDs).ToDictionary(
                          key => key.ID,
                          value => value
                       );

                List<ARolePrivilege> newPrivilegeRoleMappings = roleIDs.Select(roleID =>
                   new ARolePrivilege()
                   {
                       PrivilegeID = privilegeID,
                       PrivilegeName = privilege.Name,
                       RoleID = roleID,
                       IsGranted = false,
                       RoleName = roles[roleID].Name,
                       CreatorName = GetCurrentUserName()
                   }).ToList();

                int result = _privilegeAdapter.ManagedPrivilegeRoles(privilegeID, newPrivilegeRoleMappings);
                return result;
            }
        }


        /// <summary>
        /// 管理、更新权限所属的用户
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public int ManagedPrivilegeUsers(Guid privilegeID, List<Guid> userIDs)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID==null || privilegeID==Guid.Empty,
                                                       "Argument privilegeID is Empty");

            if (userIDs == null || userIDs.Count == 0)
            {
                int result = _privilegeAdapter.ManagedPrivilegeUsers(privilegeID, null);
                return result;
            }
            else
            {
                // 找到权限信息
                BPrivilege privilege = GetPrivilege(privilegeID);

                // 找到用户信息
                Dictionary<Guid, BUserinfo> users = UserinfoOperator.Instance.GetUserinfoList(userIDs).ToDictionary(
                          key => key.ID,
                          value => value
                       );

                List<AUserPrivilege> newPrivilegeUserMappings = userIDs.Select(userID =>
                   new AUserPrivilege()
                   {
                       PrivilegeID = privilegeID,
                       PrivilegeName = privilege.Name,
                       UserID = userID,
                       UserName = users[userID].Name,
                       CreatorName = GetCurrentUserName()
                   }).ToList();

                int result = _privilegeAdapter.ManagedPrivilegeUsers(privilegeID, newPrivilegeUserMappings);
                return result;
            }
        }


        /// <summary>
        /// 根据分组ID获取权限列表  li jing guang 2013-05-13
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeByGroupID(string groupName)
        {
            return _privilegeAdapter.GetPrivilegeByGroupID(groupName);
        }
        /// <summary>
        /// 根据id查询操作权限
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeByID(string code)
        {
            return _privilegeAdapter.GetPrivilegeByID(code);
        }
        /// <summary>
        /// 获得多个权限信息
        /// </summary>
        /// <param name="privilegeIDs"></param>
        /// <returns></returns>
        internal List<BPrivilege> GetPrivilegeList(List<Guid> privilegeIDs)
        {
            List<BPrivilege> result = (PartlyCollection<BPrivilege>)base.GetBatchModelObjects(privilegeIDs.ToArray()).ToList();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeByUrlPath(string urlPath)
        {
            List<BPrivilege> result = _privilegeAdapter.GetPrivilegeByCode(urlPath, PrivilegeType.Url);
            return result;
        }

        public List<BPrivilege> GetPrivilegeByAction(string urlPath, string action)
        {
            List<BPrivilege> result = _privilegeAdapter.GetPrivilegeByCode(urlPath, PrivilegeType.Action);
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BPrivilege GetBestMatchedPrivilege(string absoluteUrlPath, string queryString)
        {

            if (string.IsNullOrEmpty(absoluteUrlPath))
            {
                throw new ArgumentNullException("参数absoluteUrlPath为空！");
            }

            if (absoluteUrlPath.StartsWith("/") == false)
            {
                throw new ArgumentException("参数absoluteUrlPath需以‘/’开头！");
            }
            queryString = queryString.TrimStart("?".ToCharArray()); //如果开头有‘？’,则去掉
            Dictionary<string, string> nameValueCollection = ParseQueryString(queryString);
            List<BPrivilege> alikeOnes = _privilegeAdapter.GetPrivilegeByCode(absoluteUrlPath, PrivilegeType.Url);
            if (alikeOnes == null || alikeOnes.Count == 0)
            {
                return null;
            }

            // BPrivilege result = GetMatchedPrivilege(alikeOnes, nameValueCollection);
            return alikeOnes.FirstOrDefault();
        }

        /// <summary>
        /// 查询用户的操作权限
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="type">权限类型</param>
        /// <param name="code">页面权限</param>
        /// <returns>权限集合</returns>
        public List<VUserPrivilege> GetPrivilegeByType(string userID, string type, string code)
        {
            List<VUserPrivilege> privilegeList = VUserPrivilegeAdapter.Instance.GetByUserIDAndType(userID, type).ToList();
            if (null != privilegeList)
                privilegeList = privilegeList.Where(p => p.Code.StartsWith(code)).ToList();
            return privilegeList;
        }

        #region GetBestMatchedPrivilege Invoking Methods
        /*
         * 由于页面Privilege可以带不同的QueryString 参数, 所以存在一个页面对应多个BPrivilege的情况
         * 而QueryString又不能规定死顺序, 所以需要分析出来, 再注意判断是否吻合
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParseQueryString(string queryString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(queryString))
            {
                return result;
            }

            string[] keyValues = queryString.Split("&".ToCharArray());
            foreach (string keyValue in keyValues)
            {
                string[] parts = keyValue.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string key = parts[0];
                string value = string.Empty;
                if (parts.Length > 1)
                {
                    value = HttpUtility.UrlDecode(parts[1]);
                }
                result[key] = value;
            }
            return result;
        }

        /// <summary>
        /// 找到最适合的"页面privilege"
        /// </summary>
        /// <param name="alikeOnes"></param>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        private BPrivilege GetMatchedPrivilege(List<BPrivilege> alikeOnes, Dictionary<string, string> nameValueCollection)
        {
            if (alikeOnes == null || alikeOnes.Count == 0)
            {
                throw new ArgumentNullException("权限项参数为空！");
            }
            if (alikeOnes.Count == 1)
            {
                return alikeOnes.First();
            }

            Tuple<int, BPrivilege> matched = Tuple.Create<int, BPrivilege>(0, null);

            foreach (BPrivilege item in alikeOnes)
            {
                int matchedRank = GetMatchedRank(item, nameValueCollection);
                if (matchedRank > matched.Item1)
                {
                    matched = Tuple.Create(matchedRank, item);
                }
            }

            return matched.Item2;
        }

        /// <summary>
        /// 根据Url的QueryString判断匹配的程度
        /// </summary>
        /// <param name="privilege"></param>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        private int GetMatchedRank(BPrivilege privilege, Dictionary<string, string> nameValueCollection)
        {
            string[] parts = privilege.Code.Split("?".ToCharArray());
            if (parts.Length < 2)
            {
                return 0;
            }

            string[] appendices = parts[1].Split("&".ToCharArray());

            int result = 0;
            foreach (var appendix in appendices)
            {
                string pairString = HttpUtility.UrlDecode(appendix);
                string[] keyValues = pairString.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (keyValues.Length < 2)
                {
                    continue;
                }

                string key = keyValues[0];
                string value = keyValues[1];

                if (nameValueCollection.Keys.Contains(key) == false)
                {
                    continue;
                }

                string queryStringValue = HttpUtility.UrlDecode(nameValueCollection[key]);
                if (queryStringValue.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    result++;
                }
            }
            if (result < appendices.Length)
            {
                result = 0; //不符合最低要求
            }
            return result;
        }
        #endregion

        #region Validators

        public class BPrivilegeValidator : IValidator<BPrivilege>
        {
            public ValidateResult Validate(BPrivilege data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.LengthControl(data.Name, "Name", 1, 128, ref result);
                //ValidatorHelper.LengthControl(data.GroupID, "GroupID", 1, 36, ref result);
                return result;
            }
        }
        public class BPrivilegeUniqueValidator : IValidator<BPrivilege>
        {
            public ValidateResult Validate(BPrivilege data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.UniqueField(data, "Group_Name", CheckNameUnique, ref result);

                return result;
            }

            private static PrivilegeAdapter _privilegeAdapter = AdapterFactory.GetAdapter<PrivilegeAdapter>();

            /// <summary>
            /// 检查是否有重复
            /// </summary>
            /// <param name="name"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            private bool CheckNameUnique(object obj)
            {
                BPrivilege data = (BPrivilege)obj;

                IList<BPrivilege> privileges = null;//_privilegeAdapter.GetExists(data.Name, data.GroupID, data.ID);
                if (privileges.Count > 0)
                {
                    return false;
                }

                return true;
            }

        }
        #endregion





    }

}

