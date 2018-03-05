using System;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 根据数据库的逻辑名称创建Database实例的工厂类
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// 根据数据库的逻辑名称创建Database实例
        /// </summary>
        /// <param name="name">数据库逻辑名称</param>
        /// <returns>Database实例</returns>
        public static Database Create(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            //// 根据数据驱动名称对应的数据库实体
            //// 如果以后需要开放DeluxeWorks.Db提供自动加载第三方Database类型的话，也仅需要修改这个方法
            //switch (DbConnectionManager.GetDbProviderName(name))
            //{
            //    case "System.Data.SqlClient":
            //        return new SqlDatabase(name);
            //    case "System.Data.OracleClient":
            //        return new OracleDatabase(name);
            //    //case "Oracle.DataAccess.Client":
            //    //    return new ODP.OracleDatabase(name);
            //    default:
            //        throw new NotSupportedException(name + " 所定义的数据驱动不支持");
            //}

            return DbConnectionManager.GetDataProvider(name) as Database;
        }

        /// <summary>
        /// 通过Context获取数据库对象实例
        /// </summary>
        /// <param name="context">调用上下文对象</param>
        /// <returns>Database实例</returns>
        public static Database Create(DbContext context)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(context == null, "context");
            return Create(context.Name);
        }
    }
}
