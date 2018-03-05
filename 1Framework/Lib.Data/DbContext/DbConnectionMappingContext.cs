using System;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 数据库联接名称的映射。在上下文中维护一个连接名称的映射字典。
    /// </summary>
    public class DbConnectionMappingContext : IDisposable
    {
        private string sourceConnectionName = string.Empty;
        private string destinationConnectionName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string SourceConnectionName
        {
            get { return sourceConnectionName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DestinationConnectionName
        {
            get { return destinationConnectionName; }
        }

        private DbConnectionMappingContext()
        {
        }

        /// <summary>
        /// 创建连接名称的对应关系
        /// </summary>
        /// <param name="srcConnectionName"></param>
        /// <param name="destConnectionName"></param>
        /// <returns></returns>
        public static DbConnectionMappingContext CreateMapping(string srcConnectionName, string destConnectionName)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(srcConnectionName, "srcConnectionName");
            ExceptionHelper.CheckStringIsNullOrEmpty(destConnectionName, "destConnectionName");

            ExceptionHelper.TrueThrow(DbConnectionMappingContextCache.Instance.ContainsKey(srcConnectionName),
                "已经为连接名称{0}建立了映射关系", srcConnectionName);

            DbConnectionMappingContext context = new DbConnectionMappingContext();

            context.sourceConnectionName = srcConnectionName;
            context.destinationConnectionName = destConnectionName;

            DbConnectionMappingContextCache.Instance.Add(context.sourceConnectionName, context);

            return context;
        }

        /// <summary>
        /// 得到映射后的连接名称
        /// </summary>
        /// <param name="srcConnectionName"></param>
        /// <returns></returns>
        public static string GetMappedConnectionName(string srcConnectionName)
        {
            DbConnectionMappingContext context = null;
            string result = srcConnectionName;

            if (DbConnectionMappingContextCache.Instance.TryGetValue(srcConnectionName, out context))
                result = context.destinationConnectionName;

            return result;
        }
        #region IDisposable Members

        /// <summary>
        /// 释放对应关系
        /// </summary>
        public void Dispose()
        {
            DbConnectionMappingContext context = null;

            if (DbConnectionMappingContextCache.Instance.TryGetValue(this.sourceConnectionName, out context))
            {
                DbConnectionMappingContextCache.Instance.Remove(this.sourceConnectionName);
            }
        }

        #endregion
    }
}
