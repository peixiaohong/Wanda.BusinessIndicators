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
                                         Select [A].* From tempData As A
                                         Inner Join [dbo].[S_Org_User] As B On A.[ID]=B.[CompanyID]
                                         Where A.[IsLastNode]=1", id);
            return ExecuteQuery(sql);
        }
    }
}
