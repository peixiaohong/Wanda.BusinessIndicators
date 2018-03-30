
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.EmployeeTransfer.Helper.DbHelper
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
    }
}
