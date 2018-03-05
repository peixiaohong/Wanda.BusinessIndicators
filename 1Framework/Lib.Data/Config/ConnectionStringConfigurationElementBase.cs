using System;
using System.Configuration;
using Lib.Config;
using Lib.Core;
using Lib.Data.Properties;

namespace Lib.Data
{
    class ConnectionStringConfigurationElementBase : NamedConfigurationElement
    {
        #region Private Fields
        protected string dbEventArgsTypeName = string.Empty;
        protected bool isParsed = false;
        protected ConnectionStringElement settings;
        #endregion

        #region Helper Methods
        protected virtual void ParseBuilders()
        {
            #region Parse <connectionString>
            if ((Builders != null) && (Builders.Count > 0))
            {
                foreach (BuilderConfigurationElement elementBuilder in Builders)
                {
                    ConnectionStringBuilderBase builder = (ConnectionStringBuilderBase)TypeCreator.CreateInstance(elementBuilder.Type);
                    switch (elementBuilder.AttributeName)
                    {
                        case "connectionString":
                            settings.ConnectionString = builder.BuildUp(settings.ConnectionString);
                            break;
                        case "name":
                            settings.Name = builder.BuildUp(settings.Name);
                            break;
                        case "providerName":
                            settings.ProviderName = builder.BuildUp(settings.ProviderName);
                            break;
                    }
                }
            }
            #endregion
        }

        protected virtual void ParseEventArgs()
        {
            #region Parse DbEventArgs
            if (EventArgs != null)
            {
                Type dbEventArgsType = Type.GetType(EventArgs.Type);
                ExceptionHelper.TrueThrow<ArgumentException>(
                    dbEventArgsType != null && !typeof(DbEventArgs).IsAssignableFrom(dbEventArgsType),
                    string.Format(Resource.DbEventTypeIsNotValidException, EventArgs.Type));
                dbEventArgsTypeName = EventArgs.Type;
            }
            #endregion
        }

        protected virtual void ParseOriginalSettings()
        {
            #region Get Original Configuration
            settings.Name = (string)this["name"];
            settings.ConnectionString = (string)this["connectionString"];
            settings.ProviderName = (string)this["providerName"];
            #endregion
        }

        protected virtual void Parse()
        {
            this.settings = new ConnectionStringElement();
            ParseOriginalSettings();
            ParseBuilders();
            ParseEventArgs();
            isParsed = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 以标准方式提供连接配置信息
        /// </summary>
        public virtual ConnectionStringElement Settings
        {
            get
            {
                if (false == isParsed)
                    Parse();
                return settings;
            }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public override string Name
        {
            get
            {
                if (false == isParsed)
                    Parse();
                return settings.Name;
            }
        }

        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get
            {
                if (false == isParsed)
                    Parse();
                return settings.ConnectionString;
            }
        }

        [ConfigurationProperty("providerName")]
        public string ProviderName
        {
            get
            {
                if (false == isParsed)
                    Parse();
                return settings.ProviderName;
            }
        }

        [ConfigurationProperty("commandTimeout", IsRequired = false, DefaultValue = "00:00:30")]
        public TimeSpan CommandTimeout
        {
            get
            {
                return (TimeSpan)this["commandTimeout"];
            }
        }

        public string DbEventArgsTypeName { get { return dbEventArgsTypeName; } }

        [ConfigurationProperty("builders", IsRequired = false)]
        public BuildersConfiguratonElementCollection Builders
        {
            get
            {
                return base["builders"] as BuildersConfiguratonElementCollection;
            }
        }

        [ConfigurationProperty("eventArgs", IsRequired = false)]
        public DbEventArgsConfigurationElement EventArgs
        {
            get
            {
                return base["eventArgs"] as DbEventArgsConfigurationElement;
            }
        }
        #endregion
    }
}
