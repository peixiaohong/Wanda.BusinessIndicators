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
using Lib.Data.AppBase;
using Wanda.Lib.AuthCenter;



namespace Wanda.Lib.AuthCenter.BLL
{
    /// <summary>
    /// Privilege对象的业务逻辑操作
    /// </summary>
    public class PrivilegeGroupOperator : BizOperatorBase<BPrivilegeGroup>
    {
        public static PrivilegeGroupOperator Instance = BizOperatorFactory.Create<PrivilegeGroupOperator>(WebHelper.GetCurrentLoginUser, WebHelper.GetTimeNow);

        private PrivilegeGroupAdapter _privilegeGroupAdapter = AdapterFactory.GetAdapter<PrivilegeGroupAdapter>();

        protected override BaseAdapterT<BPrivilegeGroup> GetAdapter()
        {
            return _privilegeGroupAdapter;
        }
        /// <summary>
        /// 获得全部的权限信息
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 考虑到不会有太多， 所以一次全部返回结果
        /// </remarks>
        public IList<BPrivilegeGroup> GetPrivilegeGroupList()
        {
            IList<BPrivilegeGroup> result = _privilegeGroupAdapter.GetPrivilegeGroupList();
            return result;

        }


        /// <summary>
        /// 新添加权限信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Guid AddPrivilegeGroup(BPrivilegeGroup data)
        {
            ExceptionHelper.TrueThrow<MissingParameterException>(data == null,
                                                          "Argument data is Empty");
            Guid result = base.AddNewModel(data,
                                            PrivilegeGroupUniqueValidator.Instance,
                                            PrivilegeGroupValidator.Instance);
            return result;
        }

        /// <summary>
        /// 根据ID查找权限信息
        /// </summary>
        /// <param name="privilegeGroupID"></param>
        /// <returns></returns>
        public BPrivilegeGroup GetPrivilegeGroup(Guid privilegeGroupID)
        {
            ExceptionHelper.TrueThrow<MissingParameterException>(privilegeGroupID==null || privilegeGroupID==Guid.Empty,
                                                                "Argument privilegeGroupID is Empty");
            return base.GetModelObject(privilegeGroupID);
        }


        /// <summary>
        /// 根据Name查找权限组信息
        /// </summary>
        /// <param name="privilegeGroupID"></param>
        /// <returns></returns>
        public BPrivilegeGroup GetPrivilegeGroupByName(string name)
        {
            ExceptionHelper.TrueThrow<MissingParameterException>(string.IsNullOrEmpty(name),
                                                                "Argument name is Empty");



            BPrivilegeGroup result = GetPrivilegeGroupList()
                .FirstOrDefault(p => string.Compare(name, p.Name, StringComparison.InvariantCultureIgnoreCase) == 0);

            return result;
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns>return data.ID</returns>
        public Guid UpdatePrivilegeGroup(BPrivilegeGroup data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null,
                                                            "Argument data is Empty");
            Guid result = base.UpdateModelObject(data,
                                            PrivilegeGroupUniqueValidator.Instance,
                                            PrivilegeGroupValidator.Instance);
            return result;
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="privilegeGroupID"></param>
        /// <returns></returns>
        public Guid RemovePrivilegeGroup(Guid privilegeGroupID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(privilegeGroupID==null || privilegeGroupID==Guid.Empty,
                                                             "Argument privilegeGroupID is Empty");

            //：权限组删除前，需要清除指定的权限组下的权限
            _privilegeGroupAdapter.ClearPrivileges(privilegeGroupID);
            ////：权限删除前，需要清除指定的权限-人员映射关系
            //_privilegeGroupAdapter.ManagedPrivilegeUsers(privilegeGroupID, null);

            Guid result = base.RemoveObject(privilegeGroupID);
            return result;
        }


        #region Validators

        public class PrivilegeGroupValidator : IValidator<BPrivilegeGroup>
        {
            public static PrivilegeGroupValidator Instance = new PrivilegeGroupValidator();
            public ValidateResult Validate(BPrivilegeGroup data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.LengthControl(data.Name, "Name", 1, 128, ref result);
                return result;
            }
        }
        public class PrivilegeGroupUniqueValidator : IValidator<BPrivilegeGroup>
        {
            public static PrivilegeGroupUniqueValidator Instance = new PrivilegeGroupUniqueValidator();
            public ValidateResult Validate(BPrivilegeGroup data)
            {
                ValidateResult result = new ValidateResult();
                ValidatorHelper.UniqueField(data, "Name", CheckNameUnique, ref result);

                return result;
            }

            private PrivilegeGroupAdapter _privilegeGroupAdapter = AdapterFactory.GetAdapter<PrivilegeGroupAdapter>();

            /// <summary>
            /// 检查是否有重复
            /// </summary>
            /// <param name="name"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            private bool CheckNameUnique(object obj)
            {
                BPrivilegeGroup data = (BPrivilegeGroup)obj;

                IList<BPrivilegeGroup> privilegeGroups = _privilegeGroupAdapter.GetExists(data.Name, data.ID);
                if (privilegeGroups.Count > 0)
                {
                    return false;
                }

                return true;
            }

        }
        #endregion



    }

}

