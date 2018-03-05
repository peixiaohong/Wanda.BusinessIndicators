using System;
using System.Configuration;
using System.Data.Common;
using Lib.Core;
using Lib.Data.Properties;

namespace Lib.Data
{
    /// <summary>
    /// 解析配置文件，获取相应得ConnectionString及ProviderName，进而获取数据库连接和Provider信息的管理类
    /// </summary>
    public static class DbConnectionManager
    {
        /// <summary>
        /// 获取指定连接的ConnectionString
        /// </summary>
        /// <param name="name">数据库逻辑名称</param>
        /// <returns>ConnectionString</returns>
        public static string GetConnectionString(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            return GetConnection(name).ConnectionString;
        }

        /// <summary>
        /// 数据库联接是否定义
        /// </summary>
        /// <param name="name"></param>
        public static bool ConnectionNameIsConfiged(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            ConnectionStringElement settings = GetConfiguration(name);

            return settings != null;
        }

        private static ConnectionStringElement GetConfiguration(string name)
        {
            ConnectionManagerConfigurationSection section =
                ConnectionManagerConfigurationSection.GetConfig();

            ConnectionStringConfigurationElement configElement = section.ConnectionStrings[name];
            ConnectionStringElement element = null;

            // 本地配置文件的连接串采用优先级高于Meta库中的
            if (configElement != null)
            {
                element = new ConnectionStringElement();
                element.ConnectionString = configElement.ConnectionString;
                element.ProviderName = configElement.ProviderName;
                element.EventArgsType = configElement.EventArgs.Type;
                element.Name = configElement.Name;
                element.CommandTimeout = configElement.CommandTimeout;
            }
            else
            {
                // 备选通道是从Meta库获取
                MetaConnectionStringConfigurationElement metaElement = section.MetaConnectionString;
                if (metaElement != null)
                    element = metaElement.GetConnectionStringElement(name);
            }

            return element;
        }

        /// <summary>
        /// 根据数据库逻辑名称获得连接实例
        /// </summary>
        /// <param name="name">数据库逻辑名称</param>
        /// <returns>数据库连接实例</returns>
        internal static DbConnection GetConnection(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            ConnectionStringElement settings = GetConfiguration(name);

            ExceptionHelper.FalseThrow<ConfigurationException>(settings != null, Resource.CanNotFindConnectionName, name);

            DbConnection dbConnection = DbProviderFactories.GetFactory(settings.ProviderName).CreateConnection();
            dbConnection.ConnectionString = settings.ConnectionString;

            return dbConnection;
        }

        /// <summary>
        /// 获取指定连接的ProviderName
        /// </summary>
        /// <param name="name">数据库逻辑名称</param>
        /// <returns>ProviderName</returns>
        //internal static string GetDbProviderName(string name) Modify By Yuanyong 20080320
        private static string GetDbProviderName(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            return GetConfiguration(name).ProviderName;
        }

        /// <summary>
        /// 获取指定连接的Database对象
        /// </summary>
        /// <param name="name">数据库逻辑名称</param>
        /// <returns>连接的Database对象</returns>
        internal static Database GetDataProvider(string name)
        {
            string providerName = GetDbProviderName(name);

            ConnectionManagerConfigurationSection section = ConnectionManagerConfigurationSection.GetConfig();

            DataProviderConfigurationElement configProvider = section.DataProviders[providerName];

            //沈峥添加(2008-08-26)，检查dataProvider配置是否正确
            ExceptionHelper.FalseThrow<ConfigurationException>(configProvider != null, Resource.CanNotFindProviderName, name, providerName);

            return (Database)configProvider.CreateInstance(name);
        }

        /// <summary>
        /// 获取指定连接的Command执行超时时间
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static TimeSpan GetCommandTimeout(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            return GetConfiguration(name).CommandTimeout;
        }

        /// <summary>
        /// 获取指定连接的DbProviderFactory对象(Added by Shen Zheng
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static DbProviderFactory GetDbProviderFactory(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            ConnectionStringElement elem = GetConfiguration(name);

            ExceptionHelper.FalseThrow<ConfigurationException>(elem != null, Resource.CanNotFindConnectionName, name);

            return DbProviderFactories.GetFactory(elem.ProviderName);
        }

        // Del By Yuanyong 20080320
        ///// <summary>
        ///// 获取指定连接的DbProviderFactory
        ///// </summary>
        ///// <param name="name">数据库逻辑名称</param>
        ///// <returns>DbProviderFactory实例</returns>
        //internal static DbProviderFactory GetDbProviderFactory(string name)
        //{
        //    ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");
        //    return DbProviderFactories.GetFactory(GetDbProviderName(name));
        //}

        /// <summary>
        /// 根据配置信息获得指定逻辑数据库的自定义事件参数类型
        /// </summary>
        /// <param name="name">数据库逻辑名称</param>
        /// <returns>事件参数类型</returns>
        internal static DbEventArgs GetEventArgsType(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            string typeName = GetConfiguration(name).EventArgsType;

            return string.IsNullOrEmpty(typeName) ? null : (DbEventArgs)TypeCreator.CreateInstance(typeName);
        }
    }
}
