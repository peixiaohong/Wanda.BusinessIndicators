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

        /// <summary>
        /// 获取人已经授权的菜单
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Menu> GetLoinNameMenu(string loginName)
        {
            string sql = string.Format(@"With menu_TempData
                                       As
                                       (
                                       Select E.[ID],E.[CnName],E.[EnName],E.[ParentMenuID],E.[Sequence],E.[Level],E.[Url],E.[ResourceKey]
                                       From [dbo].[Employee]  As A
                                       Inner Join [dbo].[S_Role_User] As B On A.[LoginName]=B.[LoginName] And [B].[IsDeleted]=0
                                       Inner Join  [dbo].[S_Role] As C On B.[RoleID]=C.[ID] And C.[IsDeleted]=0
                                       Inner Join [dbo].[S_RolePermissions] As D On C.Id=D.[RoleID] And D.[IsDeleted]=0
                                       Inner Join [dbo].[S_Menu] As E On D.[MenuID]=E.[ID] 
                                       Where A.[IsDeleted]=0  And A.[LoginName]='zhengguilong'
                                       Union All
                                       Select A.[ID],A.[CnName],A.[EnName],A.[ParentMenuID],A.[Sequence],A.[Level],A.[Url],A.[ResourceKey] From [dbo].[S_Menu] As A 
                                       Inner Join menu_TempData As B On B.[ParentMenuID]=A.[ID]
                                       Where A.[IsDeleted]=0
                                       )
                                       Select Distinct * From menu_TempData Order By [Sequence] Asc", loginName);
            return ExecuteQuery(sql);
        }
    }
}
