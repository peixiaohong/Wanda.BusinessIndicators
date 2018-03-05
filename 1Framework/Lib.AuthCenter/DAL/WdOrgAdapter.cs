using System.Collections.Generic;
using Lib.Data;
using System.Data;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.DAL
{
    sealed class WdOrgAdapter : AuthCommonAdapter
    {
        private static WdOrgAdapter _Instance = new WdOrgAdapter();
        public static WdOrgAdapter Instance
        {
            get
            {
                return _Instance;
            }
        }
        public List<WdOrg> GetOrgList()
        {
            List<WdOrg> result = new List<WdOrg>();
            string sqlCommand = string.Empty;

            sqlCommand = "SELECT parentUnitID  ParentID,OrgID, OrgName,ShortName,[order] OrderID FROM dbo.wd_org WHERE  parentStatus = 1 ORDER BY [order]";

            DataSet ds = DbHelper.RunSqlReturnDS(sqlCommand, ConnectionName);
            if (ds != null)
            {
                DataTable table = ds.Tables[0];
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        WdOrg view = new WdOrg();
                        ORMapping.DataRowToObject<WdOrg>(row, view);
                        result.Add(view);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 根据当前的Org信息,获取子节点
        /// </summary>
        /// <param name="orgs">如果参数为null,则返回顶级列表</param>
        /// <returns></returns>
        public PartlyCollection<WdOrg> GetChildOrg(WdOrg orgs = null)
        {
            PartlyCollection<WdOrg> result = new PartlyCollection<WdOrg>();
            string sqlCommand = string.Empty;

            if (orgs == null)
            {
                sqlCommand = "SELECT parentUnitID  ParentID,OrgID, OrgName,ShortName,[order] OrderID, FullPath FROM dbo.wd_org WHERE isRoot = 1  AND  parentStatus = 1 ORDER BY [order]";
            }
            else
            {
                sqlCommand = string.Format("SELECT parentUnitID  ParentID,OrgID, OrgName,ShortName,[order] OrderID FROM dbo.wd_org WHERE parentUnitID = {0}  AND  parentStatus = 1 ORDER BY [order]", orgs.OrgID); ;
            }
            DataSet ds = DbHelper.RunSqlReturnDS(sqlCommand, ConnectionName);
            if (ds != null)
            {
                DataTable table = ds.Tables[0];
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        WdOrg view = new WdOrg();
                        ORMapping.DataRowToObject<WdOrg>(row, view);
                        result.Add(view);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 根据orgID获取用户信息
        /// </summary>
        /// <param name="orgid">wd_org.orgID</param>
        /// <returns></returns>
        public PartlyCollection<WD_User> GetSearchOrgUser(string orgid)
        {
            PartlyCollection<WD_User> result = new PartlyCollection<WD_User>();
            string sqlCommand = string.Empty;
            if (orgid != null)
            {
                sqlCommand = string.Format(@"
                    SELECT b.employeeCode as Wd_UserID,b.username as LoginName,b.employeeName as Name,
                    b.orgName as OrgName,b.orgID as Wd_OrgID,b.jobID as JobID,b.joinUnitDate as StartTime, NULL as EndTime,
                    b.jobName as JobTitle
                    FROM dbo.wd_user_org_rel a
                    INNER JOIN dbo.Wd_User b ON a.username = b.username
                    WHERE b.unitID ='{0}' AND a.username IS NOT NULL and b.employeeStatus='2'", SqlTextHelper.SafeQuote(orgid));
                DataSet ds = DbHelper.RunSqlReturnDS(sqlCommand, ConnectionName);
                if (ds != null)
                {
                    DataTable table = ds.Tables[0];
                    if (table != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            WD_User view = new WD_User();
                            ORMapping.DataRowToObject<WD_User>(row, view);
                            result.Add(view);
                        }
                    }
                }
            }
            return result;
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
            PartlyCollection<WD_User> result = new PartlyCollection<WD_User>();
            string sqlCommand = string.Empty;
            {
                sqlCommand = string.Format(@"SELECT DISTINCT  TOP(100) b.employeeCode as Wd_UserID,b.username as LoginName,b.employeeName as Name,
                                            b.orgName as OrgName,b.orgID as Wd_OrgID,b.jobID as JobID,b.joinUnitDate as StartTime, NULL as EndTime,
                                            b.jobName as JobTitle
                                            FROM dbo.Wd_User b
                                            WHERE (b.username LIKE '%{0}%' or b.employeeName LIKE '%{0}%')
                                            and b.unitName LIKE '%{1}%'
                                            and b.jobName LIKE '%{2}%' and b.employeeStatus='2'",
                             string.IsNullOrEmpty(keyword) == false ? SqlTextHelper.SafeQuote(keyword) : keyword,
                             string.IsNullOrEmpty(deptName) == false ? SqlTextHelper.SafeQuote(deptName) : deptName,
                             string.IsNullOrEmpty(jobName) == false ? SqlTextHelper.SafeQuote(jobName) : jobName);

                DataSet ds = DbHelper.RunSqlReturnDS(sqlCommand, ConnectionName);
                if (ds != null)
                {
                    DataTable table = ds.Tables[0];
                    if (table != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            WD_User view = new WD_User();
                            ORMapping.DataRowToObject<WD_User>(row, view);
                            result.Add(view);
                        }
                    }
                }
            }
            return result;
        }

        public WdOrg GetOrgByName(string orgName)
        {
            WdOrg result = null;
            string sqlCommand = string.Empty;

            sqlCommand = string.Format(@"SELECT parentUnitID  ParentID,OrgID, OrgName,ShortName,[order] OrderID , FullPath
                                         FROM dbo.wd_org WHERE  [orgName] ='{0}' ORDER BY [order]",
                                         SqlTextHelper.SafeQuote(orgName));

            DataSet ds = DbHelper.RunSqlReturnDS(sqlCommand, ConnectionName);
            if (ds != null)
            {
                DataTable table = ds.Tables[0];
                if (table != null && table.Rows.Count > 0)
                {
                    WdOrg view = new WdOrg();
                    ORMapping.DataRowToObject<WdOrg>(table.Rows[0], view);
                    result = view;
                }
            }
            return result;
        }

    }
}
