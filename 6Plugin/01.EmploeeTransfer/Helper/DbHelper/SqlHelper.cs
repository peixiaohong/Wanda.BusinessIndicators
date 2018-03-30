using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.EmployeeTransfer.Helper.DbHelper
{
    public static class SqlHelper
    {
        public static DataTable QueryData(string sql)
        {
            DataTable result = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["System.ConnectionString"]))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(result);
            }
            return result;
        }
        public static int ExecuteSql(string sql)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["System.ConnectionString"]))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
    }
}
