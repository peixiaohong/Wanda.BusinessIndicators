﻿using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.BusinessIndicators.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    /// <summary>
    /// 组织架构
    /// </summary>
    sealed class S_OrganizationalAdapter : AppBaseAdapterT<S_Organizational>
    {
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int DeleteData(S_Organizational data)
        {
            return base.Remove(data);
        }

        /// <summary>
        /// 获取全部有效的数据
        /// </summary>
        /// <returns></returns>
        public List<S_Organizational> GetAllData()
        {
            string sql = string.Format(@"Select * From [dbo].[S_Organizational] Where [IsDeleted]=0");
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 获取下层子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<S_Organizational> GetChildDataByID(Guid id)
        {
            string sql = "Select * From [dbo].[S_Organizational] Where [IsDeleted]=0 And [ParentID]=@ParentID";
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@ParentID",DbType.Guid,id)
            };
            return ExecuteQuery(sql, parameters);
        }

        /// <summary>
        /// 根据板块拿到所有的大区
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public List<S_Organizational> GetSystem_Regional(Guid systemID)
        {
            string sql = "Select * From [dbo].[S_Organizational] Where [ParentID]=@ParentID And [SystemID]=@SystemID And [IsCompany]=0";
            if (systemID == ("00000000-0000-0000-0000-000000000000".ToGuid()))
            {
                return new List<S_Organizational>();
            }
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@ParentID",DbType.Guid,systemID),
                CreateSqlParameter("@SystemID",DbType.Guid,systemID)
            };
            return ExecuteQuery(sql, parameters);
        }

        /// <summary>
        /// 根据板块ID,大区Id获取项目，如果大区下没有，就拿板块下的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="regionalID"></param>
        /// <returns></returns>
        public List<S_Organizational> GetCompanyByRegionalID(Guid systemID, Guid regionalID)
        {
            List<S_Organizational> entitys = new List<S_Organizational>();
            string sql = @"With tempData
                          As
                          (
	                          Select * From [dbo].[S_Organizational]  Where [ParentID]=@SystemID And [SystemID]=@SystemID And [ID]=@RegionalID And [IsDeleted]=0
	                          Union All
                              Select A.* From [dbo].[S_Organizational] As A 
	                          Inner Join tempData As B On A.[ParentID]=B.[ID]
	                          Where A.[IsDeleted]=0
                          )
                        Select * From tempData As A Where A.[IsDeleted]=0 And A.[IsCompany]=1 And A.[Level]>3";
            DbParameter[] parameters1 = new DbParameter[] 
            {
                CreateSqlParameter("@SystemID",DbType.Guid,systemID),
                CreateSqlParameter("@RegionalID",DbType.Guid,regionalID)
            };
            entitys = ExecuteQuery(sql, parameters1);
            if (entitys != null || entitys.Count > 0)
            {
                return entitys;
            }
            sql = "Select * From [dbo].[S_Organizational] Where [ParentID]=@SystemID And [SystemID]=@SystemID And [IsCompany]=1";
            DbParameter[] parameters2 = new DbParameter[]
            {
                CreateSqlParameter("@SystemID",DbType.Guid,systemID)
            };
            return ExecuteQuery(sql, parameters2);
        }


        /// <summary>
        /// 根据板块ID,名称查询已有数据
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetSystemsubsetCnName(Guid systemID, string cnName)
        {
            string sql = @"With GetSystem_subset
                            As
                            (
	                            Select * From [dbo].[S_Organizational]  Where [SystemID]=@SystemID And Id=@SystemID And [IsDeleted]=0
	                            Union All
                                Select * From [dbo].[S_Organizational] As A
	                            Inner Join [GetSystem_subset] As B On B.[ID]=A.[ParentID] And B.[IsDeleted]=0
	                            Where A.[IsDeleted]=0
                            )
                            Select * From [GetSystem_subset] Where [CnName]=@CnName";
            DbParameter[] parameters = new DbParameter[] 
            {
                CreateSqlParameter("@SystemID",DbType.Guid,systemID),
                CreateSqlParameter("@CnName",DbType.String,cnName)
            };
            return ExecuteQuery(sql, parameters);
        }

        #region 数据权限
        /// <summary>
        /// 根据板块拿授权的区域
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<DataPermissions> GetUserAuthorizationArea(Guid systemID, string loginName)
        {
            string sql =@"With noCompanyData
                          As
                          (
	                          Select A.* From [dbo].[S_Organizational] As A
	                          Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID] And [B].[IsDeleted]=0
	                          Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
	                          Where A.[IsDeleted]=0 And A.[SystemID]=@SystemID And B.[LoginName]=@LoginName
	                          Union All
                              Select A.* From [dbo].[S_Organizational] As A 
	                          Inner Join [noCompanyData] As B On A.[ParentID]=B.[ID]
	                          Where A.[IsDeleted]=0
                          )

                          Select * From [noCompanyData] As A Where A.[IsDeleted]=0 And A.[IsCompany]=0 And A.[Level]>2";
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@SystemID",DbType.Guid,systemID),
                CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return FormattedData(ExecuteQuery(sql, parameters));
        }

        /// <summary>
        /// 根据登陆人拿所有的授权的组织架构
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserAuthorizationOrg(string loginName)
        {
            string sql = @"With getAllData
                             As
                              (Select
		                             A.*
	                           From  [dbo].[S_Organizational] As A
	                           Inner Join [dbo].[S_Org_User] As B
			                              On A.[ID]=B.[CompanyID]
				                             And [B].[IsDeleted]=0
	                           Inner Join [dbo].[Employee] As C
			                              On B.[LoginName]=C.[LoginName]
				                             And C.[IsDeleted]=0
	                           Where A.[IsDeleted]=0
		                             And B.[LoginName]=@LoginName And A.[ParentID]='00000000-0000-0000-0000-000000000000'
	                           Union All
	                           Select
		                             A.*
	                           From  [dbo].[S_Organizational] As A
	                           Inner Join getAllData As B
			                              On A.[ParentID]=B.[ID]
	                           Where A.[IsDeleted]=0
                              )
                           Select * From getAllData As A Where A.[IsDeleted]=0;";
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteQuery(sql,parameters);
        }

        /// <summary>
        /// 根据登陆人拿到所有的授权的板块
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserSystemData(string loginName)
        {

            string sql = @"With getSystemData
                            As
                             (Select
		                            A.*
	                          From  [dbo].[S_Organizational] As A
	                          Inner Join [dbo].[S_Org_User] As B
			                             On A.[ID]=B.[CompanyID]
				                            And [B].[IsDeleted]=0
	                          Inner Join [dbo].[Employee] As C
			                             On B.[LoginName]=C.[LoginName]
				                            And C.[IsDeleted]=0
	                          Where A.[IsDeleted]=0
		                            And B.[LoginName]=@LoginName And A.[ParentID]='00000000-0000-0000-0000-000000000000'
	                          Union All
	                          Select
		                            A.*
	                          From  [dbo].[S_Organizational] As A
	                          Inner Join getSystemData As B
			                             On A.[ParentID]=B.[ID]
	                          Where A.[IsDeleted]=0
                             )
                          Select * From getSystemData As A Where A.[IsDeleted]=0 And A.[Level]=2;";
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteQuery(sql,parameters);

        }

        /// <summary>
        /// 根据登陆人，板块ID拿到所有授权的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserCompanyData(Guid systemID, string loginName)
        {
            string sql = @"With userCompanyData
                           As
                           (
	                           Select A.* From [dbo].[S_Organizational] As A
	                           Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID] And [B].[IsDeleted]=0
	                           Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
	                           Where A.[IsDeleted]=0 And A.[SystemID]=@SystemID And B.[LoginName]=@LoginName
	                           Union All
                               Select A.* From [dbo].[S_Organizational] As A 
	                           Inner Join userCompanyData As B On A.[ParentID]=B.[ID]
	                           Where A.[IsDeleted]=0
                           )
                           Select * From userCompanyData As A Where A.[IsDeleted]=0 And A.[IsCompany]=1";
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@SystemID",DbType.Guid,systemID),
                CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteQuery(sql,parameters);
        }

        /// <summary>
        /// 根据登陆人，获取板块下第一层授权的大区
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserRegional(Guid systemID, string loginName)
        {
            string sql = @"With GetOrgRegional
                           As
                           (
	                           Select A.* From [dbo].[S_Organizational] As A
	                           Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID] And [B].[IsDeleted]=0
	                           Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
	                           Where A.[IsDeleted]=0 And A.[SystemID]=@SystemID And B.[LoginName]=@LoginName
	                           Union All
                               Select A.* From [dbo].[S_Organizational] As A 
	                           Inner Join [GetOrgRegional] As B On A.[ParentID]=B.[ID]
	                           Where A.[IsDeleted]=0
                           )

                           Select * From [GetOrgRegional] As A Where A.[IsDeleted]=0 And A.[IsCompany]=0 And A.[Level]=3";
            DbParameter[] parameters = new DbParameter[]
            {
                CreateSqlParameter("@SystemID",DbType.Guid,systemID),
                CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteQuery(sql,parameters);
        }

        private List<DataPermissions> FormattedData(List<S_Organizational> data)
        {
            List<DataPermissions> resultData = new List<DataPermissions>();
            foreach (var item in data.Where(i => i.Level == 2))
            {
                DataPermissions dp = new DataPermissions();
                dp.ID = item.ID;
                dp.IsCompany = item.IsCompany;
                dp.Level = item.Level;
                dp.ParentID = item.ParentID;
                dp.CnName = item.CnName;
                dp.SystemID = item.SystemID;
                dp.Nodes = FormattedSubData(data, item.ID);
                resultData.Add(dp);
            }
            return resultData;
        }

        private List<DataPermissions> FormattedSubData(List<S_Organizational> data, Guid parentID)
        {
            List<DataPermissions> resultData = new List<DataPermissions>();
            foreach (var item in data.Where(i => i.ParentID == parentID))
            {
                DataPermissions dp = new DataPermissions();
                dp.ID = item.ID;
                dp.IsCompany = item.IsCompany;
                dp.Level = item.Level;
                dp.ParentID = item.ParentID;
                dp.CnName = item.CnName;
                dp.SystemID = item.SystemID;
                dp.Nodes = FormattedSubData(data, item.ID);
                resultData.Add(dp);
            }
            return resultData;
        }
        #endregion
    }
}

