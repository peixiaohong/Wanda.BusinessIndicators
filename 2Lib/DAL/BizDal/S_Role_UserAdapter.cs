using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    /// <summary>
    /// 角色-用户关系
    /// </summary>
    sealed class S_Role_UserAdapter : AppBaseAdapterT<S_Role_User>
    {
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public int InsertListData(List<S_Role_User> datas)
        {
            return base.InsertList(datas);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public int DeleteListData(List<S_Role_User> datas)
        {
            return base.DeleteList(datas);
        }
    }
}
