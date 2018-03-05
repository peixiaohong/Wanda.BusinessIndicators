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
    /// Roleinfo对象的业务逻辑操作
    /// </summary>
    public class RoleinfoOperator : BizOperatorBase<BRoleinfo>
    {

        public static RoleinfoOperator Instance = BizOperatorFactory.Create<RoleinfoOperator>(WebHelper.GetCurrentLoginUser, WebHelper.GetTimeNow);


        private RoleinfoAdapter _roleinfoAdapter = AdapterFactory.GetAdapter<RoleinfoAdapter>();

        protected override BaseAdapterT<BRoleinfo> GetAdapter()
        {
            return _roleinfoAdapter;
        }

        /// <summary>
        /// 返回所有的 角色列表
        /// </summary>
        /// <returns></returns>
        public IList<BRoleinfo> GetRoleinfoList()
        {
            IList<BRoleinfo> result = _roleinfoAdapter.GetRoleinfoList();
            return result;
        }

        /// <summary>
        /// 根据分组获取角色列表 li jing guang 2013-06-07
        /// </summary>
        /// <param name="groupId">分组Id</param>
        /// <returns></returns>
        public IList<BRoleinfo> GetRoleinfoListByGroupId(Guid groupId)
        {
            IList<BRoleinfo> result = _roleinfoAdapter.GetRoleinfoListByGroupId(groupId);
            return result;
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PartlyCollection<VRoleInfo> GetRoleinfoList(RoleFilter filter)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(filter == null, "过滤参数对象不能为空");

            PartlyCollection<VRoleInfo> result = VRoleAdapter.Instance.GetList(filter, "ModifyTime desc");

            return result;
        }
        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Guid AddRoleinfo(BRoleinfo data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null,
                                                          "Argument data is Empty");

            Guid result = base.AddNewModel(data,
                RoleinfoValidator.Instance, RoleinfoUniqueValidator.Instance
                );
            return result;
        }

        /// <summary>
        /// 返回指定的角色信息
        /// </summary>
        /// <param name="roleinfoID"></param>
        /// <returns></returns>
        public BRoleinfo GetRoleinfo(Guid roleinfoID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(roleinfoID==null || roleinfoID==Guid.Empty,
                                                                "Argument roleinfoID is Empty");
            return base.GetModelObject(roleinfoID);
        }



        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns>return data.ID</returns>
        public Guid UpdateRoleinfo(BRoleinfo data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null,
                                                            "Argument data is Empty");

            BRoleinfo original = GetRoleinfo(data.ID);
            
            

            // 只有名称改变的时候， 才需要判断重名。 
            if (original.Name == data.Name)
            {
                Guid result = base.UpdateModelObject(data);
                return result;
            }
            else
            {
                Guid result = base.UpdateModelObject(data,
                    RoleinfoValidator.Instance, RoleinfoUniqueValidator.Instance
                    );
                return result;
            }
        }

        /// <summary>
        /// 删除指定的角色
        /// </summary>
        /// <param name="roleinfoID"></param>
        /// <returns></returns>
        public Guid RemoveRoleinfo(Guid roleinfoID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(roleinfoID==null || roleinfoID==Guid.Empty,
                                                             "Argument roleinfoID is Empty");
            //：角色删除前，需要清除指定的角色-授权映射关系
            _roleinfoAdapter.ManagedRolePermissions(roleinfoID, null);
            //：角色删除前，需要清除指定的角色-人员映射关系
            _roleinfoAdapter.ManagedRoleUsers(roleinfoID, null);

            Guid result = base.RemoveObject(roleinfoID);
            return result;
        }

        /// <summary>
        /// 返回用户所属的角色列表
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<BRoleinfo> GetBelongsToRoles(Guid userID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(userID==null || userID==Guid.Empty,
                                                           "Argument userID is Empty");
            List<BRoleinfo> result = _roleinfoAdapter.GetBelongsToRoles(userID).ToList();
            return result;
        }

        /// <summary>
        /// 获得指定授权匹配的角色列表
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <returns></returns>
        public IList<BRoleinfo> GetGrantedRoles(Guid privilegeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeID ==null || privilegeID==Guid.Empty,
                "Argument privilegeID is Empty");
            IList<BRoleinfo> result = _roleinfoAdapter.GetGrantedRoles(privilegeID);
            return result;
        }


        /// <summary>
        /// 更新角色用户列表
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        public int ManagedRoleUsers(Guid roleID, List<Guid> userIDs)
        {

            ExceptionHelper.TrueThrow<ArgumentNullException>(roleID == null || roleID == Guid.Empty,
                                                     "Argument roleID is Empty");

            if (userIDs == null || userIDs.Count == 0)
            {
                int result = _roleinfoAdapter.ManagedRoleUsers(roleID, null);
                return result;
            }
            else
            {
                // 找到角色信息
                BRoleinfo role = GetRoleinfo(roleID);

                // 找到用户信息
                Dictionary<Guid, BUserinfo> users = UserinfoOperator.Instance.GetUserinfoList(userIDs).ToDictionary(
                          key => key.ID,
                          value => value
                       );

                List<AUserRole> newUserRoleMappings = userIDs.Select(userID =>
                   new AUserRole()
                   {
                       UserID = userID,
                       UserName = users[userID].Name,
                       RoleID = roleID,
                       RoleName = role.Name,
                       CreatorName = GetCurrentUserName()
                   }).ToList();

                int result = _roleinfoAdapter.ManagedRoleUsers(roleID, newUserRoleMappings);
                return result;
            }
        }



        public int ManagedRolePermissions(Guid roleID, List<Guid> privilegeIDs)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(roleID==null || roleID==Guid.Empty,
                                                     "Argument roleID is Empty");

            if (privilegeIDs == null || privilegeIDs.Count == 0)
            {
                int result = _roleinfoAdapter.ManagedRolePermissions(roleID, null);
                return result;
            }
            else
            {   // 找到角色信息
                BRoleinfo role = GetRoleinfo(roleID);

                // 找到用户信息
                Dictionary<Guid, BPrivilege> privileges = PrivilegeOperator.Instance.GetPrivilegeList(privilegeIDs).ToDictionary(
                          key => key.ID,
                          value => value
                       );

                List<ARolePrivilege> newRolePrivilegeMappings = privilegeIDs.Select(privilegeID =>
                   new ARolePrivilege()
                   {
                       PrivilegeID = privilegeID,
                       PrivilegeName = privileges[privilegeID].Name,
                       RoleID = roleID,
                       RoleName = role.Name,
                       IsGranted = false,
                       CreatorName = GetCurrentUserName(),
                       ModifierName = privileges[privilegeID].ModifierName,
                       ModifyTime = privileges[privilegeID].ModifyTime
                   }).ToList();


                int result = _roleinfoAdapter.ManagedRolePermissions(roleID, newRolePrivilegeMappings);
                return result;
            }
        }
        #region Validators

        public class RoleinfoValidator : IValidator<BRoleinfo>
        {
            public static RoleinfoValidator Instance = new RoleinfoValidator();
            public ValidateResult Validate(BRoleinfo data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.LengthControl(data.Name, "Name", 1, 16, ref result);
                return result;
            }
        }
        public class RoleinfoUniqueValidator : IValidator<BRoleinfo>
        {
            public static RoleinfoUniqueValidator Instance = new RoleinfoUniqueValidator();
            public ValidateResult Validate(BRoleinfo data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.UniqueField(data, "Name", CheckNameUnique, ref result);

                return result;
            }

            private RoleinfoAdapter _roleinfoAdapter = AdapterFactory.GetAdapter<RoleinfoAdapter>();

            /// <summary>
            /// 检查是否有重复
            /// </summary>
            /// <param name="name"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            private bool CheckNameUnique(object obj)
            {
                BRoleinfo data = (BRoleinfo)obj;

                IList<BRoleinfo> Roleinfos = _roleinfoAdapter.GetExists(data.Name, data.ID, data.ScopeID);
                if (Roleinfos.Count > 0)
                {
                    return false;
                }

                return true;
            }

        }
        #endregion



        internal List<BRoleinfo> GetRoleinfoList(List<Guid> roleIDs)
        {
            List<BRoleinfo> result = base.GetBatchModelObjects(roleIDs.ToArray()).ToList();
            return result;
        }


        /// <summary>
        /// 根据角色名称获得角色对象
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="CongID"></param>
        /// <returns></returns>
        public BRoleinfo GetRoleinfoByName(string roleName)
        {
            RoleFilter filter = new RoleFilter();
            filter.RoleName = roleName;
            BRoleinfo result = _roleinfoAdapter.GetRoleList(filter).FirstOrDefault();
            return result;
        }
    }
}

