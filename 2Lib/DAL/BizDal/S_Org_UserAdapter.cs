using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    sealed class S_Org_UserAdapter : AppBaseAdapterT<S_Org_User>
    {
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int DeleteData(S_Org_User data)
        {
            return base.Remove(data);
        }

        /// <summary>
        /// 根据组织架构ID查询授权数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<S_Org_User> GetDataByOrgID(Guid id)
        {
            string sql = string.Format(@"WITH tempData
                                         AS
                                         (
                                         SELECT * FROM [dbo].[S_Organizational] WHERE id='{0}'
                                         UNION ALL
                                         SELECT [A].* FROM [dbo].[S_Organizational] A
                                         Inner Join tempData B  On A.[ParentID] =B.[ID] 
                                         )
                                         Select [B].* From tempData As A
                                         Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID]
                                         Where A.[IsLastNode]=1", id);
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 获取组织架构【用户设置组织】
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Org_User> GetDataByLoginName(string loginName)
        {
            string sql = string.Format(@"WITH tempData
                                         AS
                                         (
                                         SELECT * FROM [dbo].[S_Organizational] WHERE id='00000000-0000-0000-0000-000000000000' And [IsDeleted]=0
                                         UNION ALL
                                         SELECT [A].* FROM [dbo].[S_Organizational] A
                                         Inner Join tempData B  On A.[ParentID] =B.[ID]  And A.[IsDeleted]=0
                                         )
                                         Select [A].ID,A.[CnName],A.[ParentID],A.[Level],A.[IsLastNode],A.[IsDeleted],
	                                            Case When Exists(Select 1 From [dbo].[S_Org_User] Where [CompanyID]=A.[ID] And [IsDeleted]=0 And [LoginName]='%{0}%') Then 1
			                                         Else 0
			                                         End As IsChecked
                                         From tempData As A  
                                         Where A.[IsDeleted]=0 Order By [A].[Level]
                                         ", loginName);
            return base.DataTableToListT(ExecuteReturnTable(sql));
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDataByLoginName(string loginName)
        {
            string sql = string.Format(@"Delete [dbo].[S_Org_User] Where [LoginName]='{0}'",loginName);
            return ExecuteSql(sql);
        }
    }
}
