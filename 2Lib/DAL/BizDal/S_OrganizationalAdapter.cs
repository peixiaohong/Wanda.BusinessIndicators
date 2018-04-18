using LJTH.BusinessIndicators.Model.BizModel;
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
        /// 获取下层子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<S_Organizational> GetChildDataByID(Guid id)
        {
            string sql = string.Format(@"Select * From [dbo].[S_Organizational] Where [IsDeleted]=0 And [ParentID]='{0}'", id);
            return ExecuteQuery(sql);
        }
    }
}
