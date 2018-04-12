using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    /// <summary>
    /// 角色权限
    /// </summary>
    sealed class S_RolePermissionsAdapter:AppBaseAdapterT<S_RolePermissions>
    {
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public int InsertListData(List<S_RolePermissions> datas)
        {
            return base.InsertList(datas);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public int DeleteListData(List<S_RolePermissions> datas)
        {
            return base.DeleteList(datas);
        }
    }
}
