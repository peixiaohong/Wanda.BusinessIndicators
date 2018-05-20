
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using System.Linq;
using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data;
using LJTH.BusinessIndicators.ViewModel;
using System.Reflection;
using System.Configuration;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Monthlyreportdetail对象的数据访问适配器
    /// </summary>
    sealed class B_MonthlyreportdetailAdapter : AppBaseAdapterT<B_MonthlyReportDetail>
    {

        public IList<B_MonthlyReportDetail> GetMonthlyreportdetailList()
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }


        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int UpdateMonthlyreportdetailLisr(List<B_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {

                    sb.Append(ORMapping.GetUpdateSql<B_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
                });

                if (sb.Length > 0)
                {
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        i = ExecuteSql(sb.ToString());
                        scope.Complete();
                    }
                }

                return i;
            }
            else
            {
                return i;
            }
        }

        /// <summary>
        /// 添加数据到列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int AddMonthlyreportdetailLisr(List<B_MonthlyReportDetail> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {
                    sb.Append(ORMapping.GetInsertSql<B_MonthlyReportDetail>(p, TSqlBuilder.Instance) + sqlSeperator);
                });

                if (sb.Length > 0)
                {
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        i = ExecuteSql(sb.ToString());
                        scope.Complete();
                    }
                }

                return i;
            }
            else
            {
                return i;
            }
        }

        internal IList<B_MonthlyReportDetail> GetMonthlyreportdetailList(Guid MonthlyReportID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND MonthlyReportID=@MonthlyReportID";

            SqlParameter pMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);

            return ExecuteQuery(sql, pMonthlyReportID);
        }

        internal IList<B_MonthlyReportDetail> GetMonthlyreportdetailList(Guid systemId, int year, int month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID and FinYear=@FinYear and FinMonth=@FinMonth";

            SqlParameter[] parameters = {
                new SqlParameter{ ParameterName= "@SystemID", Value=systemId },
                new SqlParameter{ ParameterName="@FinYear",Value=year},
                new SqlParameter{ ParameterName="@FinMonth",Value=month}
            };


            return ExecuteQuery(sql, parameters);
        }


        /// <summary>
        /// 判断明细表中是否含有数据
        /// </summary>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        internal bool GetMonthlyReportDetailCount(Guid MonthlyReportID)
        {

            string sql = @" SELECT  TOP 1 ID FROM  dbo.B_MonthlyReportDetail "; //WHERE MonthlyReportID='291E72F7-3176-496B-99C0-D6A15F8F7795'

            sql += " WHERE " + base.NotDeleted;
            sql += " AND MonthlyReportID=@MonthlyReportID";
            SqlParameter pTMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTMonthlyReportID);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// A表获取数据，复制到B表
        /// </summary>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        internal List<B_MonthlyReportDetail> GetMonthlyReportDetail_ByAToB(int FinYear, int FinMonth, Guid SystemID, Guid AreaID, Guid MonthlyReportID, Guid TargetPlanID)
        {
            string sql = "GetMonthlyReportDetail_ByAToB ";

            SqlParameter p1 = CreateSqlParameter("@FinYear", DbType.Int32, FinYear);
            SqlParameter p2 = CreateSqlParameter("@FinMonth", DbType.Int32, FinMonth);
            SqlParameter p3 = CreateSqlParameter("@SystemID", DbType.Guid, SystemID);
            SqlParameter p4 = CreateSqlParameter("@AreaID", DbType.Guid, AreaID);
            SqlParameter p5 = CreateSqlParameter("@MonthlyReportID", DbType.Guid, MonthlyReportID);
            SqlParameter p6 = CreateSqlParameter("@TargetPlanID", DbType.Guid, TargetPlanID);
            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3, p4, p5, p6);

            List<B_MonthlyReportDetail> data = new List<B_MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                B_MonthlyReportDetail item = new B_MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;
        }

        /// <summary>
        /// 从B表的审批中数据，复制一份出来
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        internal List<B_MonthlyReportDetail> GetMonthlyReportDetail_ByBToB(int FinYear, int FinMonth, Guid SystemID, Guid OldMonthlyReportID, Guid NewMonthlyReportID)
        {

            string sql = "GetMonthlyReportDetail_ByBToB ";
            SqlParameter p1 = CreateSqlParameter("@FinYear", DbType.Int32, FinYear);
            SqlParameter p2 = CreateSqlParameter("@FinMonth", DbType.Int32, FinMonth);
            SqlParameter p3 = CreateSqlParameter("@SystemID", DbType.Guid, SystemID);
            SqlParameter p4 = CreateSqlParameter("@OldMonthlyReportID", DbType.Guid, OldMonthlyReportID);
            SqlParameter p5 = CreateSqlParameter("@NewMonthlyReportID", DbType.Guid, NewMonthlyReportID);

            DataSet ds = DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2, p3, p4, p5);

            List<B_MonthlyReportDetail> data = new List<B_MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                B_MonthlyReportDetail item = new B_MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;

        }

        internal IList<MonthlyReportVM> GetBVMonthlyReport(Guid MonthlyReportID)
        {
            string sql = @"
SELECT  TargetID ,
        B_MonthlyReportDetail.SystemID ,
        CompanyID ,
        CompanyName ,
        B_MonthlyReportDetail.CompanyProperty1 ,
        IsMissTarget ,
        Counter ,
        IsMissTargetCurrent ,
        NAccumulativeActualAmmount
FROM    dbo.B_MonthlyReportDetail
        LEFT JOIN dbo.C_Company ON C_Company.ID = B_MonthlyReportDetail.CompanyID
WHERE   MonthlyReportID = @MonthlyReportID
        AND B_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0";
            SqlParameter pTMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTMonthlyReportID);
            List<MonthlyReportVM> data = new List<MonthlyReportVM>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportVM item = new MonthlyReportVM();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }

        internal List<MonthlyReportDetail> GetMonthlyReportDetailList(Guid SystemID, int Year, int Month, Guid MonthlyReportID, Guid TargetPlanID, bool IsAll)
        {
            DataSet ds = new DataSet();
            string sql = "GetMonthlyReportDetailList ";
            if (!IsAll)
            {
                sql = "GetMonthlyReportDetailListMonthID";
                ds = DbHelper.RunSPReturnDS(sql, ConnectionName, CreateSqlParameter("@MonthlyReportID", DbType.Guid, MonthlyReportID));
            }
            else
            {
                ds = DbHelper.RunSPReturnDS(sql, ConnectionName, CreateSqlParameter("@SystemID", DbType.Guid, SystemID), CreateSqlParameter("@FinYear", DbType.Int32, Year), CreateSqlParameter("@FinMonth", DbType.Int32, Month), CreateSqlParameter("@TargetPlanID", DbType.Guid, TargetPlanID));
            }
            List<MonthlyReportDetail> data = new List<MonthlyReportDetail>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                MonthlyReportDetail item = new MonthlyReportDetail();
                ORMapping.DataRowToObject(row, item);
                item.NDisplayRateByYear = item.NPlanAmmountByYear == 0 ? "--" : Math.Round((item.NAccumulativeActualAmmount / item.NPlanAmmountByYear), 5, MidpointRounding.AwayFromZero).ToString("P1");
                data.Add(item);
            });
            return data;
        }

        /// <summary>
        /// 万达电影固定报表，国内影城-票房收入指标查询
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<B_MonthlyReportDetail> GetMonthTargetDetailCNPF(Guid SystemID, int FinYear, int FinMonth, Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportDetail>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth AND TargetID=@TargetID";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int64, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int64, FinMonth);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);

            return ExecuteQuery(sql, pSystemID, pFinYear, pFinMonth, pTargetID);
        }


        #region 批量插入数据

        internal int BulkAddMonthlyReportDetailLisr(List<B_MonthlyReportDetail> list)
        {
            int result = list.Count();
            try
            {
                string createTableSql = "	SELECT * FROM B_MonthlyReportDetail WHERE 1=2";
                string sql = string.Format(@"DELETE
                        FROM    B_MonthlyReportDetail
                        WHERE   MonthlyReportID='{0}';", list[0].MonthlyReportID);
                list.ConvertAll(v => v.ID = System.Guid.NewGuid());
                list.ConvertAll(v => v.ModifyTime = DateTime.Now);
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    var con = new SqlConnection(DbConnectionManager.GetConnectionString(base.ConnectionName));
                    con.Open();
                    //创建表结构
                    SqlCommand createTableCmd = new SqlCommand(createTableSql, con);
                    SqlDataAdapter sdap = new SqlDataAdapter();
                    sdap.SelectCommand = createTableCmd;
                    DataTable dt = new DataTable();
                    sdap.Fill(dt);

                    //获取数据
                    ConvertToTable(dt, list);

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(con);
                    bulkCopy.DestinationTableName = "B_MonthlyReportDetail";
                    if (dt != null && dt.Rows.Count != 0)
                        bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.WriteToServer(dt);
                    con.Close();
                    scope.Complete();
                }


            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }

        private DataTable ConvertToTable(DataTable dt, List<B_MonthlyReportDetail> list)
        {
            PropertyInfo[] props = typeof(B_MonthlyReportDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo pi;
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn c in dt.Columns)
                {
                    pi = props.Where(v => v.Name == c.ColumnName).FirstOrDefault();
                    if (pi != null)
                    {
                        dr[c.ColumnName] = pi.GetValue(item) == null ? DBNull.Value : pi.GetValue(item);
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// </summary>
        private DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        #endregion
    }
}

