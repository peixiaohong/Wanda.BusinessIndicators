
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using System.Data.Common;
using System.Data;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Company对象的数据访问适配器
    /// </summary>
    sealed class C_CompanyAdapter : AppBaseAdapterT<C_Company>
    {
        public IList<C_Company> GetCompanyListByVersionTime(Guid _companyId, DateTime? _VersionTime)
        {
            string sql = ORMapping.GetSelectSql<C_Company>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID =@CompanyId ";
            sql += " AND VersionStart <=  @VersionTime  and @VersionTime <= VersionEnd ";
            sql += " ORDER BY Sequence ASC ";

            SqlParameter companyId = CreateSqlParameter("@CompanyId", System.Data.DbType.Guid, _companyId);
            SqlParameter VersionTime = CreateSqlParameter("@VersionTime", System.Data.DbType.Guid, _VersionTime);

            return ExecuteQuery(sql, companyId, VersionTime);

        }


        public IList<C_Company> GetCompanyList()
        {
            string sql = ORMapping.GetSelectSql<C_Company>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " ORDER BY Sequence ASC ";
            return ExecuteQuery(sql);
        }

        public IList<C_Company> GetCompanyList(Guid SystemID)
        {
            string sql = ORMapping.GetSelectSql<C_Company>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += "AND Systemid=@SystemID";
            sql += " ORDER BY Sequence ASC ";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);

            return ExecuteQuery(sql, pSystemID);
        }


        /// <summary>
        /// 百货系统中获取上一个月的累计是否达标，需要当月补回的公司信息
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="IsMissTarget"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        internal List<C_Company> GetCompanyListBySystemID(int FinYear, int FinMonth, Guid SystemID, int IsMissTarget, bool IsLatestVersion)
        {
            string sql = string.Empty;
            sql += "select * from C_Company where ID in ( select distinct(ID) from C_Company where ID in (  ";
            if (IsLatestVersion)
            {
                sql += "select CompanyID from [dbo].[B_MonthlyReportDetail]  ";
            }
            else
            {
                sql += "select CompanyID from [dbo].[A_MonthlyReportDetail]  ";
            }
            sql += " where IsMissTarget=@IsMissTarget and SystemID=@SystemID  and TargetID in(  ";
            sql += "select ID from C_Target where SystemID=@SystemID  AND NeedReport=1 AND IsDeleted=0)  ";
            sql += "and FinYear=@FinYear and FinMonth=@FinMonth and IsDeleted=0 )) ";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pIsMissTarget = CreateSqlParameter("@IsMissTarget", System.Data.DbType.Int32, IsMissTarget);
            return ExecuteQuery(sql, new SqlParameter[] { pSystemID, pFinYear, pFinMonth, pIsMissTarget });
        }

        /// <summary>
        /// 百货系统中获取上一个月的累计其中的而一个指标是否达标，需要当月补回的公司信息
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="IsMissTarget"></param>
        /// <param name="TargetID"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        internal List<C_Company> GetCompanyListBySystemIDAndTargetID(int FinYear, int FinMonth, Guid SystemID, int IsMissTarget, Guid TargetID, bool IsLatestVersion)
        {
            string sql = string.Empty;
            sql += "select * from C_Company where ID in ( select distinct(ID) from C_Company where ID in ( ";
            if (IsLatestVersion)
            {
                sql += "select CompanyID from [dbo].[B_MonthlyReportDetail] ";
            }
            else
            {
                sql += "select CompanyID from [dbo].[A_MonthlyReportDetail] ";
            }
            sql += "where IsMissTarget=@IsMissTarget and SystemID=@SystemID  ";
            sql += "and TargetID =@TargetID  ";
            sql += "and FinYear=@FinYear and FinMonth=@FinMonth and IsDeleted=0 ))";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pIsMissTarget = CreateSqlParameter("@IsMissTarget", System.Data.DbType.Int32, IsMissTarget);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, new SqlParameter[] { pSystemID, pFinYear, pFinMonth, pIsMissTarget, pTargetID });
        }


        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        internal int UpdateCompanylLisr(List<C_Company> List)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (List.Count > 0)
            {
                List.ForEach(p =>
                {

                    sb.Append(ORMapping.GetUpdateSql<C_Company>(p, TSqlBuilder.Instance) + sqlSeperator);
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
        /// 获取上报切考核的公司
        /// </summary>
        /// <param name="TargetID"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyByNeedEvaluation(Guid TargetID, Guid SystemID)
        {
            //这里排除掉同比的项
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM  dbo.C_Company WHERE SystemID =@systemID  AND ID NOT IN (  ");
            sb.Append("SELECT CompanyID FROM  dbo.C_ExceptionTarget  WHERE  TargetID=@targetID AND IsDeleted =0   AND ExceptionType <>5  )  ");
            SqlParameter pSystemID = CreateSqlParameter("@systemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pTargetID = CreateSqlParameter("@targetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sb.ToString(), new SqlParameter[] { pSystemID, pTargetID });
        }




        public List<C_Company> GetCompanyTargetList(Guid TargetID, Guid SystemID)
        {
            string sql = string.Empty;
            sql += @"SELECT * FROM dbo.C_Company  WHERE ID NOT IN ( 
SELECT CompanyID FROM dbo.C_ExceptionTarget WHERE TargetID=@TargetID
AND ExceptionType=2 AND IsDeleted=0 ) 
AND C_Company.SystemID=@SystemID
AND C_Company.IsDeleted=0 ORDER BY C_Company.Sequence";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, new SqlParameter[] { pSystemID, pTargetID });
        }


        /// <summary>
        /// 获取所有不可比公司
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public IList<C_Company> NotContrastCompany(Guid SystemID, Guid TargetID)
        {
            string sql = @"SELECT dbo.C_Company.* FROM dbo.C_ExceptionTarget LEFT JOIN dbo.C_Company ON C_Company.ID = C_ExceptionTarget.CompanyID
WHERE C_ExceptionTarget.ExceptionType=5 AND SystemID=@SystemID AND C_Company.IsDeleted=0 AND C_ExceptionTarget.IsDeleted=0
AND TargetID=@TargetID ORDER BY Sequence";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, new SqlParameter[] { pSystemID, pTargetID });
        }
        /// <summary>
        /// 获取所有可比公司
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public IList<C_Company> ContrastCompany(Guid SystemID, Guid TargetID)
        {
            string sql = @"SELECT * FROM dbo.C_Company WHERE SystemID=@SystemID AND IsDeleted=0
 AND ID NOT IN(SELECT CompanyID FROM dbo.C_ExceptionTarget 
 WHERE TargetID=@TargetID AND C_ExceptionTarget.IsDeleted=0 
 AND ( ExceptionType=2 OR ExceptionType=5))  ORDER BY Sequence asc";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, new SqlParameter[] { pSystemID, pTargetID });
        }

        /// <summary>
        /// 获取项目系统下seq小于0的公司  代表这个公司为小计
        /// </summary>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public IList<C_Company> ProCompanyAll(Guid SystemID)
        {
            string sql = @"SELECT  *
FROM    dbo.C_Company
WHERE   SystemID = @SystemID
        AND Sequence < 0
        AND IsDeleted = 0
        AND CompanyName LIKE '%总计%'";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            return ExecuteQuery(sql, pSystemID);
        }

        #region 新增方法

        /// <summary>
        /// 根据项目公司名称查询项目 注意不是模糊查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyInfoByName(string name,Guid systemID) {

            string sql = @"Select * From [dbo].[C_Company] Where [CompanyName]=@CompanyName And [SystemID]=@SystemID And [IsDeleted]=0";
            DbParameter[] parameters = new DbParameter[]
            {
                 CreateSqlParameter("@CompanyName",DbType.String,name),
                 CreateSqlParameter("@SystemID",DbType.Guid,systemID)
            };
            return ExecuteQuery(sql,parameters);
        }


        #endregion
    }
}

