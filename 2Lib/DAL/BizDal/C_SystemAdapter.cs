
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using Wanda.BusinessIndicators.Model;
using System.Data.SqlClient;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// System对象的数据访问适配器
    /// </summary>
    sealed class C_SystemAdapter : AppBaseAdapterT<C_System>
    {

        public IList<C_System> GetSystemListByVersionTime(Guid _cSystemID, DateTime? _VersionTime)
        {
            string sql = ORMapping.GetSelectSql<C_System>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID =@SystemID ";
            sql += "AND VersionStart <=  @VersionTime  and @VersionTime < VersionEnd ";
            sql += " ORDER BY Sequence ASC ";

            SqlParameter SystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, _cSystemID);
            SqlParameter VersionTime = CreateSqlParameter("@VersionTime", System.Data.DbType.DateTime, _VersionTime);

            return ExecuteQuery(sql, SystemID, VersionTime);
        }



        public IList<C_System> GetSystemList(DateTime CurrentDate)
        {
            string sql = ORMapping.GetSelectSql<C_System>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += "AND VersionStart <=  @CurrentDate  and @CurrentDate < VersionEnd ";
            sql += " ORDER BY Sequence ASC ";

            return ExecuteQuery(sql, CreateSqlParameter("@CurrentDate", System.Data.DbType.DateTime, CurrentDate));
        }
        /// <summary>
        /// 取商管和项目公司数据.
        /// </summary>
        /// <returns></returns>
        internal IList<C_System> GetSystemListBySeq() 
        {
            string sql = "SELECT * FROM dbo.C_System WHERE Sequence<200 AND IsDeleted=0 ";
            sql += "  ORDER BY Sequence";

            return ExecuteQuery(sql);
        }

        

        public IList<C_System> GetSystemListByConfigID(Guid cid)
        {
            string sql = " SELECT * FROM dbo.C_System WHERE IsDeleted=0 AND Category IN (SELECT Sequence FROM dbo.Sys_Config WHERE IsDeleted=0 AND ID=@cid) ";
            return ExecuteQuery(sql, CreateSqlParameter("@cid", System.Data.DbType.Guid, cid));
        }

        public IList<C_System> GetSystemListByGrouptype(string GroupType)
        {
            string sql = "SELECT * FROM dbo.C_System WHERE GroupType=@GroupType AND IsDeleted=0 ORDER BY Sequence ASC";
            SqlParameter ptype = CreateSqlParameter("@GroupType", System.Data.DbType.String, GroupType);
            return ExecuteQuery(sql, ptype);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="SystemName"></param>
        /// <returns></returns>
        internal C_System GetSystemByName(string SystemName)
        {
            string sql = ORMapping.GetSelectSql<C_System>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted + " And SystemName=@SystemName ";

            SqlParameter pSystemName = CreateSqlParameter("@SystemName", System.Data.DbType.String, SystemName);

            List<C_System> list = ExecuteQuery(sql, new SqlParameter[] { pSystemName });
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return new C_System();
            }
        }
    }
}

