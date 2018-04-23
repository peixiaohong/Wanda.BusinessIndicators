using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.BusinessIndicators.ViewModel.Common;
using System;
using System.Collections.Generic;
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
        /// 根据板块ID，获取所有有效的组织架构信息
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public List<S_Organizational> GetAllDataBySystemID(Guid systemID)
        {
            string sql = string.Format(@"Select * From [dbo].[S_Organizational] Where SystemID=@SystemID and [IsDeleted]=0");
            return ExecuteQuery(sql, CreateSqlParameter("@SystemID", System.Data.DbType.Guid, systemID));
        }
        /// <summary>
        /// 获取下层子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<S_Organizational> GetChildDataByID(Guid id)
        {
            string sql = string.Format(@"Select * From [dbo].[S_Organizational] Where [IsDeleted]=0 And [ParentID]='{0}'", id);
            return ExecuteQuery(sql);
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
            string sql = string.Format(@"With noCompanyData
                                        As
                                        (
	                                        Select A.* From [dbo].[S_Organizational] As A
	                                        Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID] And [B].[IsDeleted]=0
	                                        Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
	                                        Where A.[IsDeleted]=0 And A.[SystemID]='{0}' And B.[LoginName]='{1}'
	                                        Union All
                                            Select A.* From [dbo].[S_Organizational] As A 
	                                        Inner Join [noCompanyData] As B On A.[ParentID]=B.[ID]
	                                        Where A.[IsDeleted]=0
                                        )

                                        Select * From [noCompanyData] As A Where A.[IsDeleted]=0 And A.[IsCompany]=0 And A.[Level]>2", systemID, loginName);
            return FormattedData(ExecuteQuery(sql));
        }

        /// <summary>
        /// 根据登陆人拿所有的授权组织
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserAuthorizationOrg(string loginName)
        {
            string sql = string.Format(@"With getAllData
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
		                                          And B.[LoginName]='{0}' And A.[ParentID]='00000000-0000-0000-0000-000000000000'
	                                        Union All
	                                        Select
		                                          A.*
	                                        From  [dbo].[S_Organizational] As A
	                                        Inner Join getAllData As B
			                                           On A.[ParentID]=B.[ID]
	                                        Where A.[IsDeleted]=0
                                           )
                                        Select * From getAllData As A Where A.[IsDeleted]=0;", loginName);
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 根据登陆人拿到所有的板块
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserSystemData(string loginName)
        {

            string sql = string.Format(@"With getSystemData
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
		                                          And B.[LoginName]='{0}' And A.[ParentID]='00000000-0000-0000-0000-000000000000'
	                                        Union All
	                                        Select
		                                          A.*
	                                        From  [dbo].[S_Organizational] As A
	                                        Inner Join getSystemData As B
			                                           On A.[ParentID]=B.[ID]
	                                        Where A.[IsDeleted]=0
                                           )
                                        Select * From getSystemData As A Where A.[IsDeleted]=0 And A.[Level]=2;", loginName);
            return ExecuteQuery(sql);

        }

        /// <summary>
        /// 根据登陆人，板块ID拿到所有的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserCompanyData(Guid systemID, string loginName)
        {
            string sql = string.Format(@"With userCompanyData
                                            As
                                            (
	                                            Select A.* From [dbo].[S_Organizational] As A
	                                            Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID] And [B].[IsDeleted]=0
	                                            Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
	                                            Where A.[IsDeleted]=0 And A.[SystemID]='{0}' And B.[LoginName]='{0}'
	                                            Union All
                                                Select A.* From [dbo].[S_Organizational] As A 
	                                            Inner Join userCompanyData As B On A.[ParentID]=B.[ID]
	                                            Where A.[IsDeleted]=0
                                            )

                                            Select * From userCompanyData As A Where A.[IsDeleted]=0 And A.[IsCompany]=1", systemID, loginName);
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 根据登陆人，获取板块下第一个大区
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserRegional(Guid systemID, string loginName)
        {
            string sql = string.Format(@"With GetOrgRegional
                                        As
                                        (
	                                        Select A.* From [dbo].[S_Organizational] As A
	                                        Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID] And [B].[IsDeleted]=0
	                                        Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
	                                        Where A.[IsDeleted]=0 And A.[SystemID]='{0}' And B.[LoginName]='{1}'
	                                        Union All
                                            Select A.* From [dbo].[S_Organizational] As A 
	                                        Inner Join [GetOrgRegional] As B On A.[ParentID]=B.[ID]
	                                        Where A.[IsDeleted]=0
                                        )

                                        Select * From [GetOrgRegional] As A Where A.[IsDeleted]=0 And A.[IsCompany]=0 And A.[Level]=3", systemID, loginName);
            return ExecuteQuery(sql);
        }

        private List<DataPermissions> FormattedData(List<S_Organizational> data)
        {
            List<DataPermissions> resultData = new List<DataPermissions>();
            foreach (var item in data.Where(i=>i.Level==2))
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

        private List<DataPermissions> FormattedSubData(List<S_Organizational> data,Guid parentID)
        {
            List<DataPermissions> resultData = new List<DataPermissions>();
            foreach (var item in data.Where(i => i.ParentID==parentID))
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

