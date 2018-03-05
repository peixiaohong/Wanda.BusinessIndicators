using Lib.Data;
/*
 * Create by Hu Wei Zheng
 *          2013-5-2
 */
using System.Collections.Generic;
using Wanda.Lib.AuthCenter.DAL;
using Wanda.Lib.AuthCenter.Model;


namespace Wanda.Lib.AuthCenter.BLL
{
    /// <summary>
    /// BudgetRolling对象的业务逻辑操作
    /// </summary>
    public class WdOrgOperator
    {
        public static readonly WdOrgOperator Instance = new WdOrgOperator();

        public PartlyCollection<WdOrg> GetChildOrg(WdOrg orgs = null)
        {
            PartlyCollection<WdOrg> result = WdOrgAdapter.Instance.GetChildOrg(orgs);
            return result;

        }
        public List<WdOrg> GetOrgList()
        {
            List<WdOrg> result = WdOrgAdapter.Instance.GetOrgList();
            return result;

        }
        /// <summary>
        /// 根据orgID获取用户信息
        /// </summary>
        /// <param name="orgid">wd_org.orgID</param>
        /// <returns></returns>
        public PartlyCollection<WD_User> GetSearchOrgUser(string orgid)
        {
            PartlyCollection<WD_User> list = null;
            list = WdOrgAdapter.Instance.GetSearchOrgUser(orgid);
            return list;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="keyword">用户名称</param>
        /// <param name="deptName">部门名称</param>
        /// <param name="jobName">岗位名称</param>
        /// <returns></returns>
        public PartlyCollection<WD_User> GetSearchOrgUsers(string keyword, string deptName, string jobName)
        {
            PartlyCollection<WD_User> list = null;
            list = WdOrgAdapter.Instance.GetSearchOrgUsers(keyword, deptName, jobName);
            return list;
        }
        /// <summary>
        /// 根据组织机构名称查询组织结构ID（用于导入）
        /// </summary>
        /// <param name="orgName">组织机构名称</param>
        /// <returns>组织机构实体</returns>
        public WdOrg GetOrgByName(string orgName)
        {
            return WdOrgAdapter.Instance.GetOrgByName(orgName);
        }
    }
}

