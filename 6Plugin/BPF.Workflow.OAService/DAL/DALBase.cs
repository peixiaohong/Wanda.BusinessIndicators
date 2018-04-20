using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BPF.OAMQMessages.DAL
{
    public class DALBase
    {
        public enum DataBaseTypes
        { 
            MSSQLServer,
            Oracle
        }
        public DbProviderFactory DbFactory
        {
            get
            {

                if (_DbFactory == null)
                {
                    _DbFactory = GetDataBaseType() == DataBaseTypes.MSSQLServer ? 
                        DbProviderFactories.GetFactory(ConnectionStringSetting.ProviderName) : new OracleClientFactory();
                }
                return _DbFactory;
            }
        }private DbProviderFactory _DbFactory = null;

        public DbConnection DbConnection
        {
            get 
            {
                if (_DbConnection == null)
                {
                    _DbConnection = DbFactory.CreateConnection();
                    _DbConnection.ConnectionString = ConnectionStringSetting.ConnectionString;
                }
                return _DbConnection; 
            }
            set { _DbConnection = value; }
        }private DbConnection _DbConnection = null;

        

        public ConnectionStringSettings ConnectionStringSetting
        {
            get
            {
                if (_ConnectionStringSetting == null)
                {
                    _ConnectionStringSetting = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["defaultDatabase"]];
                    if (_ConnectionStringSetting == null)
                        throw new Exception("没有找到数据库连接字符串配置信息");
                }
                return _ConnectionStringSetting;
            }
        }private ConnectionStringSettings _ConnectionStringSetting = null;

        public DataBaseTypes GetDataBaseType()
        {
            if (ConnectionStringSetting.ProviderName == "System.Data.SqlClient") 
            {
                return DataBaseTypes.MSSQLServer;
            }
            else if (ConnectionStringSetting.ProviderName == "Oracle.ManagedDataAccess.Client")
            {
                return DataBaseTypes.Oracle;
            }
            else 
                throw new Exception("不支持的数据库类型");
        }

        public void AddParameter(System.Data.Common.DbCommand cmd, System.Data.DbType dbType, string name, object value)
        {
            DbParameter parameter = DbFactory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = dbType;
            cmd.Parameters.Add(parameter);
        }

        public class DataTypeHelper
        {
            public static DateTime GetDateTime(object value)
            {
                if (value == null)
                    return DateTime.MinValue;
                DateTime result = DateTime.MinValue;
                if (DateTime.TryParse(value.ToString(), out result)) {
                    return result;
                }
                return DateTime.MinValue;
            }

            public static string GetString(object value)
            {
                return value == null ? string.Empty : value.ToString();
            }

            public static int GetInt(object value)
            {
                if (value == null)
                    return 0;
                int result = 0;
                if (int.TryParse(value.ToString(), out result))
                {
                    return result;
                }
                return 0;
            }

            public static Guid GetGuid(object value)
            { 
                if(value == null)
                {
                return Guid.NewGuid();
                }

                Guid result = Guid.Empty;
                if (Guid.TryParse(value.ToString(), out result))
                {
                    return result;
                }
                return Guid.NewGuid();
            }
        }
    }
}
