/*
 * Create by Hu Wei Zheng

 */
using System;
using Lib.Data;
using System.Collections.Generic;
using LJTH.Lib.AuthCenter.Model;
using System.Text;
using System.Data; 
using LJTH.Lib.Data.AppBase;


namespace LJTH.Lib.AuthCenter.DAL
{
    /// <summary>
    /// Privilege对象的数据访问适配器
    /// </summary>
    sealed class PrivilegeGroupAdapter : AuthBaseAdapterT<BPrivilegeGroup>, IUsage
    {
       
        /// <summary>
        /// 获得所有的权限组列表
        /// </summary>
        /// <returns></returns>
        public IList<BPrivilegeGroup> GetPrivilegeGroupList()
        {
            string sql = ORMapping.GetSelectSql<BPrivilegeGroup>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        ///// <summary>
        ///// 获得所有的权限组列表
        ///// </summary>
        ///// <returns></returns>
        //public IList<BPrivilegeGroup> GetPrivilegeGroupList()
        //{
        //    string sql = ORMapping.GetSelectSql<BPrivilegeGroup>(TSqlBuilder.Instance);

        //    sql += " WHERE " + base.NotDeleted;

        //    return ExecuteQuery(sql);
        //}


        /// <summary>
        /// 清除一个权限组下的所有权限项 
        /// </summary>
        /// <param name="BPrivilegeGroupID"></param>
        /// <returns></returns>
        internal void ClearPrivileges(Guid BPrivilegeGroupID)
        {
            string sql = string.Format("update {0} from {0} set ISDELETE=1 WHERE GROUPID='{1}'",
                    ORMapping.GetTableName(typeof(BPrivilege)),
                     BPrivilegeGroupID);
            DbHelper.RunSql(sql, this.ConnectionName);
        }



        /// <summary>
        /// 关联关系的数量
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int UsageCount(Guid ID)
        {
            int result = 0;

            string sql = string.Format(@"
                SELECT COUNT(*) FROM {0} A 
                WHERE A.GroupID='{1}' AND A.ISDELETED<1",
                ORMapping.GetTableName(typeof(BPrivilege)),ID);
            result = (int)DbHelper.RunSqlReturnScalar(sql, ConnectionName);
            return result;
        }



        internal IList<BPrivilegeGroup> GetExists(string name, Guid id)
        {
            var list = this.Load(p =>
           {
               p.AppendItem("Name",  SqlTextHelper.SafeQuote(name));
               if (id != null && id!=Guid.Empty)
               {
                   p.AppendItem("ID", id, "<>");
               }
               p.AppendItem("ISDELETED", "1", "<>");

           });
            return list;
        }

    }
}

