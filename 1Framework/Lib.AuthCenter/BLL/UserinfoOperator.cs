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
using LJTH.Lib.AuthCenter.DAL;
using LJTH.Lib.AuthCenter.Model;
using LJTH.Lib.AuthCenter.ViewModel;
using LJTH.Lib.Data.AppBase;



namespace LJTH.Lib.AuthCenter.BLL
{
    /// <summary>
    /// Userinfo对象的业务逻辑操作
    /// </summary>
    public class UserinfoOperator //: BizOperatorBase<BUserinfo>
    {
        public static UserinfoOperator Instance = new UserinfoOperator();

        private static UserinfoAdapter _userinfoAdapter = new UserinfoAdapter();
       
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
        /// 根据用户名称找到用户  
        /// </summary> 
        /// <param name="userName"></param>
        /// <returns></returns>
        public LoginUserInfo GetUserInfoByName(string userName)
        {
            return _userinfoAdapter.GetUserInfoByName(userName);
        }
    }
}

