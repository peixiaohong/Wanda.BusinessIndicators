using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    /// <summary>
    /// 角色
    /// </summary>
    sealed class S_RoleAdapter : AppBaseAdapterT<S_Role>
    {
        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InsertData(S_Role data)
        {
            return base.Insert(data);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int UpdateData(S_Role data)
        {
            return base.Update(data);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int DeleteData(S_Role data)
        {
            return base.Remove(data);
        }

        /// <summary>
        /// 获取全部有效的角色【用户角色管理页面】
        /// </summary>
        /// <returns></returns>
        public List<S_Role> GetDatas(string CnName)
        {
            string sql = string.Empty;
            if (CnName == "")
            {
                sql = "Select * From  [dbo].[S_Role] Where [IsDeleted]=0 ORDER BY CreateTime DESC ";
            }
            else
            {
                sql = string.Format("Select * From  [dbo].[S_Role] Where [IsDeleted]=0 and CnName like '%{0}%' ORDER BY CreateTime DESC ", CnName);
            }
            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 获取全部有效角色 【用于用户授权角色页面】
        /// </summary>
        /// <param name="cnName"></param>
        /// <param name="loginName">用户账号必填</param>
        /// <returns></returns>
        public List<S_Role> GetDatasByCnName(string cnName, string loginName)
        {
            string sql = string.Format(@"Select A.[ID],A.[CnName],A.[Description], 
	                                           Case  When Exists(Select 1 From [dbo].[S_Role_User] Where [IsDeleted]=0 And [RoleID]=A.[ID] And [LoginName]='{0}')  Then 1
			                                         Else 0
			                                         End  As IsChecked
                                        From [dbo].[S_Role] As A
                                        Where A.[IsDeleted]=0 ", loginName);
            if (cnName != "")
            {
                sql += string.Format(" And A.[CnName] Like '%{0}%'", cnName);
            }
            return base.DataTableToListT(ExecuteReturnTable(sql));
        }
    }
}

