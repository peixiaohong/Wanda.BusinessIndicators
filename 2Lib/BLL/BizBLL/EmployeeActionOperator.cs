using Lib.Data.AppBase;
using LJTH.BusinessIndicators.DAL.BizDal;
using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.BusinessIndicators.Model.Filter;
using LJTH.BusinessIndicators.ViewModel.Employee;
using LJTH.Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.BLL.BizBLL
{
    public class EmployeeActionOperator:BizOperatorBase<Employee>
    {
        #region field
        public static readonly EmployeeActionOperator Instance = PolicyInjection.Create<EmployeeActionOperator>();

        private static EmployeeAdapter _smployeeAdapter = AdapterFactory.GetAdapter<EmployeeAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<Employee> GetAdapter()
        {
            return _smployeeAdapter;
        }
        #endregion

        #region methods

        /// <summary>
        /// 查询员工信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public List<AllUserPermissions> GetAllUser(AllUserPermissionsFilter filter, out int TotalCount)
        {
            return _smployeeAdapter.GetAllUser(filter, out TotalCount);
        }

        /// <summary>
        /// 查询员工信息,角色添加用户用
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public List<AllUserPermissions> GetAddUserInfo(AllUserPermissionsFilter filter, out int TotalCount)
        {
            return _smployeeAdapter.GetAddUserInfo(filter, out TotalCount);
        }
        #endregion
    }
}
