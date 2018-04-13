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
        ///// <summary>
        ///// 批量插入数据
        ///// </summary>
        ///// <param name="datas"></param>
        ///// <returns></returns>
        //public int InsertListData(List<S_RolePermissions> datas)
        //{
        //    return base.InsertList(datas);
        //}

        ///// <summary>
        ///// 批量删除
        ///// </summary>
        ///// <param name="datas"></param>
        ///// <returns></returns>
        //public int DeleteListData(List<S_RolePermissions> datas)
        //{
        //    return base.DeleteList(datas);
        //}

        /// <summary>
        /// 获取角色已经授权菜单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<S_RolePermissions> GetListData(Guid ID)
        {
            string sql = string.Format(@"Select A.* From [dbo].[S_RolePermissions] As A
                                         Inner Join [dbo].[S_Menu] As B On A.[MenuID]=B.[ID] And B.[IsDeleted]=0
                                         Where A.[IsDeleted]=0 And A.[RoleID]='{0}'
                                         Order By B.[Sequence] Asc ", ID);

            return ExecuteQuery(sql);
        }
    }
}
