using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 元连接串对象的配置元素
    /// </summary>
    class MetaConnectionStringConfigurationElement : ConnectionStringConfigurationElementBase
    {
        /// <summary>
        /// 标示当前Builder是针对Meta串还是数据库中具体某个连接串
        /// </summary>
        private const string MetaSectionPrefix = "META:";
        private const string ItemSectionPrefix = "ITEM:";
        private const string BothSectionPrefix = "BOTH:";

        /// <summary>
        /// 保存所有从Meta库获得的连接串
        /// </summary>
        private static IDictionary<string, ConnectionStringElement> metaConnectionSettings = null;

        /// <summary>
        /// 是否对Meta库的连接串解析过
        /// </summary>
        private static bool isMetaConnectionParsed = false;

        private void ParseSettingBuildersByPrefix(string prefix, ref ConnectionStringElement setting)
        {
            if ((Builders != null) && (Builders.Count > 0))
            {
                foreach (BuilderConfigurationElement elementBuilder in Builders)
                {
                    if ((elementBuilder.Name.StartsWith(prefix)) || (elementBuilder.Name.StartsWith(BothSectionPrefix)))
                    {
                        ConnectionStringBuilderBase builder = (ConnectionStringBuilderBase)TypeCreator.CreateInstance(elementBuilder.Type);
                        switch (elementBuilder.AttributeName)
                        {
                            case "connectionString":
                                setting.ConnectionString = builder.BuildUp(setting.ConnectionString); break;
                            case "name":
                                setting.Name = builder.BuildUp(setting.Name); break;
                            case "providerName":
                                setting.ProviderName = builder.BuildUp(setting.ProviderName); break;
                        }
                    }
                }
            }
        }

        private DataTable LoadMetaConnections()
        {
            DataTable resultTable = null;

            //沈峥添加，如果ProvicerName为空，则不进行查询
            if (string.IsNullOrEmpty(settings.ProviderName) == false)
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);

                //这里应该有问题，袁勇记录 20080320
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = settings.ConnectionString;
                    DbCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM CONNECTIONS";

                    using (DbDataAdapter adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        DataSet result = new DataSet();
                        adapter.Fill(result);
                        if ((result == null) || (result.Tables.Count == 0) || (result.Tables[0].Rows.Count == 0))
                            return null;

                        resultTable = result.Tables[0];
                    }
                }
            }

            return resultTable;
        }

        private void TranslateMetaConnections()
        {
            DataTable result = LoadMetaConnections();
            if (result == null)
                return;

            metaConnectionSettings = new Dictionary<string, ConnectionStringElement>();

            foreach (DataRow row in result.Rows)
            {
                ConnectionStringElement setting = new ConnectionStringElement();
                setting.Name = row["NAME"] as string;
                setting.ConnectionString = row["CONNECTION_STRING"] as string;
                setting.ProviderName = row["PROVIDER_NAME"] as string;
                setting.EventArgsType = row["EVENT_ARGUMENT_TYPE"] as string;

                ParseSettingBuildersByPrefix(ItemSectionPrefix, ref setting);
                metaConnectionSettings.Add(setting.Name, setting);
            }
        }

        protected override void ParseBuilders()
        {
            if (isMetaConnectionParsed)
                return;

            isMetaConnectionParsed = true;

            // 配置文件的解析好像成问题，保险的办法就是在这里try一下重新解析
            if (settings == null)
            {
                settings = new ConnectionStringElement();
                settings.Name = (string)this["name"];
                settings.ConnectionString = (string)this["connectionString"];
                settings.ProviderName = (string)this["providerName"];
            }

            ParseSettingBuildersByPrefix(MetaSectionPrefix, ref settings);
            TranslateMetaConnections();
        }

        /// <summary>
        /// 因为这部分定义在数据库内部，所以不需要解析
        /// </summary>
        protected override void ParseEventArgs()
        {
        }

        /// <summary>
        /// 根据元库的配置获取连接串
        /// </summary>
        /// <param name="name">连接串名称</param>
        /// <returns>连接串对象</returns>
        public ConnectionStringElement GetConnectionStringElement(string name)
        {
            ParseBuilders();
            if ((metaConnectionSettings == null) || (metaConnectionSettings.Count == 0))
                return null;
            ConnectionStringElement element;
            if (false == metaConnectionSettings.TryGetValue(name, out element))
                return null;
            else
                return element;
        }
    }
}
