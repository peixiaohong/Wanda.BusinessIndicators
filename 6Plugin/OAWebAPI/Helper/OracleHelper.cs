using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace Plugin.Helper
{
    public static class OracleHelper
    {
        public static DataTable QueryData(string sql)
        {
            DataTable result = new DataTable();
            OracleConnection conn = new OracleConnection(ConfigurationManager.AppSettings["OA.ConnectionString"]);
            conn.Open();
            OracleCommand cmd = new OracleCommand(sql, conn);
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(result);
            return result;
        }
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                list = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = ToEntity<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }
        public static T ToEntity<T>(this DataTable dt)
        {
            T obj = default(T);
            if (dt != null && dt.Rows.Count > 0)
            {
                obj = ToEntity<T>(dt.Rows[0]);
            }
            return obj;
        }
        public static T ToEntity<T>(this DataRow row)
        {
            T obj = default(T);
            string columnName;
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    PropertyInfo prop = obj.GetType().GetProperty(columnName);
                    try
                    {
                        object value = (row[columnName].GetType() == typeof(DBNull))
                        ? null : row[columnName];
                        if (prop.CanWrite)    //判断其是否可写
                            prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return obj;

        }
    }
}
