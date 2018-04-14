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
        /// 获取全部有效的角色
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
    }
}

