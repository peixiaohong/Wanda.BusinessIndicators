
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;


namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// SysConfig对象的数据访问适配器
    /// </summary>
    sealed class SysConfigAdapter : AppBaseAdapterT<Sys_Config>
    {

        public IList<Sys_Config> GetSysConfigList()
        {
            string sql = ORMapping.GetSelectSql<Sys_Config>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public IList<Sys_Config> GetSysConfigListByType(string type)
        {
            string sql = " SELECT * FROM Sys_Config WHERE IsDeleted=0 AND Biz_Type=@type ";
            return ExecuteQuery(sql, CreateSqlParameter("@type", System.Data.DbType.String, type));
        }

    }
}

