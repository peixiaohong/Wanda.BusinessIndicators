using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.BusinessIndicators.Model.Filter;
using LJTH.BusinessIndicators.ViewModel.Employee;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    sealed class EmployeeAdapter : AppBaseAdapterT<Employee>
    {
        /// <summary>
        /// 查询员工信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public List<AllUserPermissions> GetAllUser(AllUserPermissionsFilter filter,out int TotalCount)
        {
            //string sql = string.Format("exec Proc_GetContractList @ContractCode,@ContractName,@PartyAUnitName,@DataStatus,@ContractTypeCode,@ScalingName,@PartyBUnitName,@BusinessKey,@ProjectID,@pageSize,@pageNumber,@PageSource");
            string sql = string.Format("Exec [dbo].[Pro_GetUserPersionInfo] @LoginName,@RoleID,@KeyWord,@PageIndex, @PageSize");

            DbParameter[] parameters = new DbParameter[]{
                CreateSqlParameter("@LoginName",DbType.String,filter.LoginName),
                CreateSqlParameter("@RoleID",DbType.Guid,filter.RoleID),
                CreateSqlParameter("@KeyWord",DbType.String,filter.keyWord),
                CreateSqlParameter("@PageIndex",DbType.Int32,filter.PageIndex),
                CreateSqlParameter("@PageSize",DbType.Int32,filter.PageSize)
            };
            DataSet ds = ExecuteReturnDataSet(sql, parameters);
            List<AllUserPermissions> list = new List<AllUserPermissions>();
            TotalCount = 0;
            if (ds != null)
            {
                DataTable returnTable = ds.Tables[0];
                string tempName = string.Empty;
                if (returnTable != null)
                {
                    foreach (DataRow dr in returnTable.Rows)
                    {
                        AllUserPermissions obj = new AllUserPermissions();
                        System.Reflection.PropertyInfo[] propertys = obj.GetType().GetProperties();
                        foreach (PropertyInfo pi in propertys)
                        {
                            tempName = pi.Name;
                            if (returnTable.Columns.Contains(tempName))
                            {
                                if (!pi.CanWrite) continue;
                                object value = dr[tempName];
                                if (value != DBNull.Value)
                                {
                                    pi.SetValue(obj, value, null);
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }
                TotalCount = int.Parse(ds.Tables[1].Rows[0][0].ToString());
            }
            return list;
        }

        /// <summary>
        /// 查询员工信息,角色添加用户用
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public List<AllUserPermissions> GetAddUserInfo(AllUserPermissionsFilter filter, out int TotalCount)
        {
            //string sql = string.Format("exec Proc_GetContractList @ContractCode,@ContractName,@PartyAUnitName,@DataStatus,@ContractTypeCode,@ScalingName,@PartyBUnitName,@BusinessKey,@ProjectID,@pageSize,@pageNumber,@PageSource");
            string sql = string.Format("Exec [dbo].[Pro_GetUserInfo] @RoleID,@KeyWord,@PageIndex, @PageSize");

            DbParameter[] parameters = new DbParameter[]{
                CreateSqlParameter("@LoginName",DbType.String,filter.LoginName),
                CreateSqlParameter("@RoleID",DbType.Guid,filter.RoleID),
                CreateSqlParameter("@KeyWord",DbType.String,filter.keyWord),
                CreateSqlParameter("@PageIndex",DbType.Int32,filter.PageIndex),
                CreateSqlParameter("@PageSize",DbType.Int32,filter.PageSize)
            };
            DataSet ds = ExecuteReturnDataSet(sql, parameters);
            List<AllUserPermissions> list = new List<AllUserPermissions>();
            TotalCount = 0;
            if (ds != null)
            {
                DataTable returnTable = ds.Tables[0];
                string tempName = string.Empty;
                if (returnTable != null)
                {
                    foreach (DataRow dr in returnTable.Rows)
                    {
                        AllUserPermissions obj = new AllUserPermissions();
                        System.Reflection.PropertyInfo[] propertys = obj.GetType().GetProperties();
                        foreach (PropertyInfo pi in propertys)
                        {
                            tempName = pi.Name;
                            if (returnTable.Columns.Contains(tempName))
                            {
                                if (!pi.CanWrite) continue;
                                object value = dr[tempName];
                                if (value != DBNull.Value)
                                {
                                    pi.SetValue(obj, value, null);
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }
                TotalCount = int.Parse(ds.Tables[1].Rows[0][0].ToString());
            }
            return list;
        }
    }
}
