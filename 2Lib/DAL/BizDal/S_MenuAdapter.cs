using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    /// <summary>
    /// 菜单
    /// </summary>
    sealed class S_MenuAdapter : AppBaseAdapterT<S_Menu>
    {
        /// <summary>
        /// 获取单个角色的菜单
        /// </summary>
        /// <param name="ID">角色ID</param>
        /// <returns></returns>
        public List<S_Menu> GetRoleMenus(Guid ID)
        {
            string sql = string.Format(@"Select A.*,
	                                         Case When B.[ID] Is Not Null Then 1
		                                          Else 0 
		                                          End As IsChecked
                                         From [dbo].[S_Menu] As A
                                         Left Join [dbo].[S_RolePermissions] As B On A.[ID]=B.[MenuID] And B.[IsDeleted]=0 And B.[RoleID]='{0}'
                                         Where A.[IsDeleted]=0 
                                         Order By A.[Sequence] Asc ", ID);

            DataTable dt = ExecuteReturnTable(sql);
            return base.DataTableToListT(dt);
        }
    }
}
