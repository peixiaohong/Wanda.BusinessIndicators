using System;
using System.Data;
using System.Data.Common;
using Lib.Cache;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 存储过程参数缓冲
    /// </summary>
    public class DbParameterCache
    {
        #region Internal Type CacheMechanism
        private sealed class CacheMechanism : PortableCacheQueue<string, IDataParameter[]>
        {
            public static readonly CacheMechanism Instance = CacheManager.GetInstance<CacheMechanism>();

            private CacheMechanism() { }
        }
        #endregion

        private CacheMechanism cache = CacheMechanism.Instance;

        /// <summary>
        /// 将缓冲好的参数组绑定到指定的Command对象上
        /// </summary>
        /// <param name="command">Command对象</param>
        /// <param name="database">数据库实体</param>
        public void SetParameters(DbCommand command, Database database)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(command == null, "command");

            ExceptionHelper.TrueThrow<ArgumentNullException>(database == null, "database");

            if (cache.ContainsKey(CreateHashKey(command)))
            {
                foreach (IDataParameter parameter in GetCommandParameters(command))
                    command.Parameters.Add(parameter);
            }
            else
            {
                database.DiscoverParameters(command);   // 将参数发现机制通过IOC交给实体Database类完成
                cache.Add(CreateHashKey(command), Clone(command));
            }
        }

        /// <summary>
        /// 清理缓冲
        /// </summary>
        protected internal void Clear()
        {
            cache.Clear();
        }

        private static string CreateHashKey(IDbCommand command)
        {
            return command.Connection.ConnectionString + ":" + command.CommandText;
        }

        /// <summary>
        /// 提供某组参数的一个副本
        /// </summary>
        private static IDataParameter[] Clone(IDbCommand command)
        {
            IDataParameterCollection parameters = command.Parameters;
            IDataParameter[] parameterArray = new IDataParameter[parameters.Count];
            parameters.CopyTo(parameterArray, 0);
            return Clone(parameterArray);
        }

        /// <summary>
        /// 提供某组参数的一个副本
        /// </summary>
        /// <param name="originalParameters">参数数组</param>
        private static IDataParameter[] Clone(IDataParameter[] originalParameters)
        {
            IDataParameter[] result = new IDataParameter[originalParameters.Length];
            for (int i = 0; i < originalParameters.Length; i++)
                result[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
            return result;
        }

        /// <summary>
        /// 获取一组缓冲的参数
        /// </summary>
        /// <param name="command">Command对象</param>
        private IDataParameter[] GetCommandParameters(IDbCommand command)
        {
            IDataParameter[] cachedParameters = cache[CreateHashKey(command)];
            return Clone(cachedParameters);
        }
    }
}
