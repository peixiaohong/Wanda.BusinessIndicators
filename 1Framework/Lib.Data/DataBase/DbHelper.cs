using System;
using System.Data;
using System.Transactions;
using System.Xml;
using System.Data.Common;

namespace Lib.Data
{
    public static class DbHelper
    {
        #region  数据访问函数

        public static DbContext GetDBContext(string connectionName)
        {
            return DbContext.GetContext(connectionName);
        }

        public static Database GetDBDatabase(string connectionName)
        {
            return DatabaseFactory.Create(connectionName);
        }

        public static void RunSql(Action<Database> action, string connectionName)
        {
            using (DbContext context = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(connectionName);

                action(db);
            }
        }

        public static object RunSqlReturnScalar(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteScalar(CommandType.Text, strSql);
            }
        }


        /// <summary>
        /// 使用参数执行sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="connectionName"></param>
        /// <param name="dbParams"></param>
        /// <returns></returns>
        public static object RunSqlReturnScalar(string strSql, string connectionName, params DbParameter[] dbParams)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                DbCommand dbCmd = db.GetSqlStringCommand(strSql);
                foreach (DbParameter dbParam in dbParams)
                {
                    db.AddInParameter(dbCmd, dbParam.ParameterName, dbParam.DbType, dbParam.Value);
                }

                return db.ExecuteScalar(dbCmd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static int RunSql(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);
                return db.ExecuteNonQuery(CommandType.Text, strSql);
            }
        }

        /// <summary>
        /// 使用参数执行sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="connectionName"></param>
        /// <param name="dbParams"></param>
        /// <returns></returns>
        public static int RunSql(string strSql, string connectionName, params DbParameter[] dbParams)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                DbCommand dbCmd = db.GetSqlStringCommand(strSql);
                foreach (DbParameter dbParam in dbParams)
                {
                    db.AddInParameter(dbCmd, dbParam.ParameterName, dbParam.DbType, dbParam.Value);
                }

                return db.ExecuteNonQuery(dbCmd);
            }
        }

        public static int RunSqlWithTransaction(string strSql, string connectionName)
        {
            using (TransactionScope ts = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                int result = RunSql(strSql, connectionName);

                ts.Complete();

                return result;
            }
        }

        public static int RunSqlWithTransaction(string strSql, string connectionName, params DbParameter[] dbParams)
        {
            using (TransactionScope ts = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                int result = RunSql(strSql, connectionName, dbParams);

                ts.Complete();

                return result;
            }
        }

        public static IDataReader RunSqlReturnDR(string strSql, string connectionName)
        {
            DbContext dbi = DbContext.GetContext(connectionName);
            Database db = DatabaseFactory.Create(dbi);

            return db.ExecuteReader(CommandType.Text, strSql);
        }

        public static IDataReader RunSqlReturnDR(string strSql, string connectionName, params DbParameter[] dbParams)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                DbCommand dbCmd = db.GetSqlStringCommand(strSql);
                foreach (DbParameter dbParam in dbParams)
                {
                    db.AddInParameter(dbCmd, dbParam.ParameterName, dbParam.DbType, dbParam.Value);
                }

                return db.ExecuteReader(dbCmd);
            }
        }

        public static DataSet RunSqlReturnDS(string strSql, string connectionName)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteDataSet(CommandType.Text, strSql);
            }
        }

        public static DataSet RunSqlReturnDS(string strSql, string connectionName, params DbParameter[] dbParams)
        {
            using (DbContext dbi = DbContext.GetContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                DbCommand dbCmd = db.GetSqlStringCommand(strSql);
                foreach (DbParameter dbParam in dbParams)
                {
                    db.AddInParameter(dbCmd, dbParam.ParameterName, dbParam.DbType, dbParam.Value);
                }

                return db.ExecuteDataSet(dbCmd);
            }
        }

        public static XmlDocument RunSQLReturnXmlDoc(string strSql, string connectionName)
        {
            string xmlStr = string.Empty;

            if (strSql != string.Empty)
            {
                strSql += " FOR XML AUTO, ELEMENTS";
            }

            object obj = RunSqlReturnScalar(strSql, connectionName);

            if (obj != null)

                xmlStr = obj.ToString();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xmlStr);

            return xmlDoc;
        }

        public static DataSet RunSPReturnDS(string spName, string connectionName, params object[] parameterValues)
        {
            using (DbContext dbi = GetDBContext(connectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                return db.ExecuteDataSet(spName, parameterValues);
            }
        }

        public static IDataReader RunSPReturnDR(string spName, string connectionName, params object[] parameterValues)
        {
            //using (DbContext dbi = GetDBContext(connectionName))
            //{
            DbContext dbi = GetDBContext(connectionName);
            Database db = DatabaseFactory.Create(dbi);

            return db.ExecuteReader(spName, parameterValues);
            //}
        }

        #endregion
    }
}
