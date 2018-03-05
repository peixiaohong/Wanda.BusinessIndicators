using System;

namespace Lib.Data
{
    /// <summary>
    /// 抽象来自于配置文件或者配置数据库的连接串对象
    /// </summary>
    public class ConnectionStringElement
    {
        /// <summary>
        /// 连接串逻辑名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 数据驱动名称
        /// </summary>
        public string ProviderName;

        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnectionString;

        /// <summary>
        /// 数据访问事件对象类型
        /// </summary>
        public string EventArgsType;

        /// <summary>
        /// Command执行的超时时间
        /// </summary>
        public TimeSpan CommandTimeout = TimeSpan.FromSeconds(30);
    }
}
