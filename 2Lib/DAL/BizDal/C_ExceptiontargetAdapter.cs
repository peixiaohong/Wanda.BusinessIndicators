using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;


namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Exceptiontarget对象的数据访问适配器
    /// </summary>
    sealed class C_ExceptiontargetAdapter : AppBaseAdapterT<C_ExceptionTarget>
	{

        public IList<C_ExceptionTarget> GetExceptiontargetList()
        {
            string sql = ORMapping.GetSelectSql<C_ExceptionTarget>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 获取ExceptionType小于3的数据
        /// </summary>
        /// <returns></returns>
        public IList<C_ExceptionTarget> GetExceptionTList()
        {
            string sql = ORMapping.GetSelectSql<C_ExceptionTarget>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted + " AND  ExceptionType<3";

            return ExecuteQuery(sql);
        }

        public IList<C_ExceptionTarget> GetExceptiontargetList(Guid CompanyID, Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<C_ExceptionTarget>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += " AND CompanyID=@CompanyID ";
            sql += " AND TargetID=@TargetID AND ExceptionType<3 ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pCompanyID, pTargetID);
        }

        public IList<C_ExceptionTarget> GetNotContrastList( Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<C_ExceptionTarget>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;


            sql += " AND TargetID=@TargetID AND ExceptionType=5 ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pTargetID);
        }


        public IList<C_ExceptionTarget> GetExcTargetListBytargetID(Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<C_ExceptionTarget>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += " AND TargetID=@TargetID AND ExceptionType<3 ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pTargetID);
        }
        /// <summary>
        /// 获取不上报不考核数据
        /// </summary>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public IList<C_ExceptionTarget> GetNotUpdateListBytargetID(Guid TargetID)
        {
            string sql = ORMapping.GetSelectSql<C_ExceptionTarget>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += " AND TargetID=@TargetID AND ExceptionType=2 ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            return ExecuteQuery(sql, pTargetID);
        }

        public IList<ExceptionTargetVModel> GetExctargetListByComList(Guid CompanyID)
        {
            string sql = @"SELECT TargetID,TargetName,C_ExceptionTarget.ID FROM
 dbo.C_ExceptionTarget LEFT JOIN dbo.C_Target ON C_Target.ID = C_ExceptionTarget.TargetID
 WHERE CompanyID=@CompanyID 
 AND C_Target.IsDeleted=0 AND ExceptionType=2
 AND C_ExceptionTarget.IsDeleted=0";

            SqlParameter pCompanyID = CreateSqlParameter("@CompanyID", System.Data.DbType.Guid, CompanyID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pCompanyID);
            List<ExceptionTargetVModel> data = new List<ExceptionTargetVModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                ExceptionTargetVModel item = new ExceptionTargetVModel();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;

        }

        public IList<ExceptionCompanyVModel> GetExctargetListByTarList(Guid TargetID, int ExceptionType)
        {
            string sql = @"SELECT C_Company.ID AS CompanyID , C_ExceptionTarget.ID AS ExceptionTargetID,CompanyName,
 C_ExceptionTarget.ModifierName,C_ExceptionTarget.ModifyTime,C_ExceptionTarget.CreateTime,ExceptionType,OpeningTime 
 FROM dbo.C_ExceptionTarget left JOIN dbo.C_Company ON dbo.C_Company.ID = C_ExceptionTarget.CompanyID
 WHERE  C_Company.IsDeleted=0 AND ExceptionType=@ExceptionType
 AND C_ExceptionTarget.IsDeleted=0
 AND TargetID=@TargetID
 ORDER BY C_ExceptionTarget.ModifyTime";

            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pExceptionType = CreateSqlParameter("@ExceptionType", System.Data.DbType.Int32, ExceptionType);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTargetID,pExceptionType);
            List<ExceptionCompanyVModel> data = new List<ExceptionCompanyVModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                ExceptionCompanyVModel item = new ExceptionCompanyVModel();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;

        }


        public IList<LastExceptionCompanyVModel> GetLastExctargetListByTarList(Guid TargetID, Guid SystemID)
        {
            string sql = @"SELECT * FROM dbo.C_Company WHERE SystemID=@SystemID AND IsDeleted=0
 AND ID NOT IN(SELECT CompanyID FROM dbo.C_ExceptionTarget 
 WHERE TargetID=@TargetID AND C_ExceptionTarget.IsDeleted=0 
 AND ExceptionType<3) ORDER BY Sequence asc";

            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTargetID, pSystemID);
            List<LastExceptionCompanyVModel> data = new List<LastExceptionCompanyVModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                LastExceptionCompanyVModel item = new LastExceptionCompanyVModel();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;

        }
        public IList<LastExceptionCompanyVModel> GetNoExceptionReplaceList(Guid TargetID, Guid SystemID)
        {
            string sql = @"SELECT * FROM dbo.C_Company WHERE SystemID=@SystemID AND IsDeleted=0
 AND ID NOT IN(SELECT CompanyID FROM dbo.C_ExceptionTarget 
 WHERE TargetID=@TargetID AND C_ExceptionTarget.IsDeleted=0 
 AND ( ExceptionType=2 OR ExceptionType=5)) ORDER BY Sequence asc";

            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pTargetID, pSystemID);
            List<LastExceptionCompanyVModel> data = new List<LastExceptionCompanyVModel>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                LastExceptionCompanyVModel item = new LastExceptionCompanyVModel();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });

            return data;

        }


        /// <summary>
        /// 获取所有公司数量o
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="ID">如果为去年的 则SystemID 否则为MonthlyReportID</param>
        /// <returns></returns>
        public IList<ContrastAllCompanyVM> GetContrastAllCompany(int FinYear, int FinMonth, Guid ID,Guid TargetID,bool IfNow)
        {
            string sql = "";
            //如果IfNow是True  则查询B表数据
            if (IfNow==true)
            {
                sql = @"  SELECT CompanyID,
        CompanyName
 FROM   dbo.B_MonthlyReportDetail
        LEFT JOIN dbo.C_Company ON C_Company.ID = B_MonthlyReportDetail.CompanyID
 WHERE  B_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0
        AND B_MonthlyReportDetail.MonthlyReportID = @ID
        AND TargetID = @TargetID
        AND CompanyName NOT LIKE '%总部%'";
            }
            else
            {
                sql = @"SELECT CompanyID ,
        CompanyName
FROM    dbo.A_MonthlyReportDetail
        LEFT JOIN dbo.C_Company ON C_Company.ID = A_MonthlyReportDetail.CompanyID
WHERE   A_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0
        AND FinYear=@FinYear AND FinMonth=@FinMonth
        AND A_MonthlyReportDetail.SystemID = @ID
        AND TargetID = @TargetID
        AND CompanyName NOT LIKE '%总部%'";
            }

            SqlParameter pMonthlyReportID = CreateSqlParameter("@ID", System.Data.DbType.Guid, ID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pTarget = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);

            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pMonthlyReportID, pFinYear, pFinMonth, pTarget);
            List<ContrastAllCompanyVM> data = new List<ContrastAllCompanyVM>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                ContrastAllCompanyVM item = new ContrastAllCompanyVM();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }

        /// <summary>
        /// 获取可比公司数量o
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public IList<ContrastAllCompanyVM> GetContrastCompanyTotal(int FinYear, int FinMonth, Guid ID,Guid TargetID,bool IfNow)
        {
            string sql = "";
            if (IfNow==true)
            {
                sql = @"SELECT  CompanyID  ,
        CompanyName
FROM    dbo.C_Company
        RIGHT JOIN dbo.B_MonthlyReportDetail ON B_MonthlyReportDetail.CompanyID = C_Company.ID
WHERE   B_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0
        AND MonthlyReportID = @ID
		AND TargetID=@TargetID
        AND C_Company.ID NOT IN ( SELECT    CompanyID
                                  FROM      dbo.C_ExceptionTarget
                                  WHERE     C_ExceptionTarget.IsDeleted = 0
                                            AND (ExceptionType = 5 OR  ExceptionType = 2) AND TargetID=@TargetID )
	    AND CompanyName NOT LIKE'%总部%'";
            }
            else
            {
                sql = @" SELECT  CompanyID ,
        CompanyName
FROM    dbo.C_Company
        RIGHT JOIN dbo.A_MonthlyReportDetail ON A_MonthlyReportDetail.CompanyID = C_Company.ID
WHERE   A_MonthlyReportDetail.IsDeleted = 0
        AND C_Company.IsDeleted = 0
        AND FinMonth = @FinMonth
        AND FinYear = @FinYear
        AND A_MonthlyReportDetail.SystemID = @ID
		AND TargetID=@TargetID
        AND C_Company.ID NOT IN ( SELECT    CompanyID
                                  FROM      dbo.C_ExceptionTarget
                                  WHERE     C_ExceptionTarget.IsDeleted = 0
                                            AND (ExceptionType = 5 OR  ExceptionType = 2) AND TargetID=@TargetID)
        AND CompanyName NOT LIKE'%总部%'";
            }
 

            SqlParameter pSystemID = CreateSqlParameter("@ID", System.Data.DbType.Guid, ID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pTarget = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pSystemID, pFinYear, pFinMonth, pTarget);
            List<ContrastAllCompanyVM> data = new List<ContrastAllCompanyVM>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                ContrastAllCompanyVM item = new ContrastAllCompanyVM();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }

        /// <summary>
        /// 获取不可比公司数量o
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="ID">如果IFB==true 则SystemID 否则为MonthlyReportID</param>
        /// <returns></returns>
        public IList<ContrastAllCompanyVM> GetNoContrastCompanyTotal(int FinYear, int FinMonth, Guid ID,Guid TargetID,bool IFB)
        {
            string sql = "";
            if (IFB == true)
            {
                sql = @"SELECT   CompanyID  ,
        CompanyName,NAccumulativeActualAmmount,OpeningTime
        FROM    ( SELECT    B_MonthlyReportDetail.CompanyID ,
                    B_MonthlyReportDetail.TargetID ,
                    NAccumulativeActualAmmount
          FROM      dbo.C_ExceptionTarget
                    RIGHT JOIN dbo.B_MonthlyReportDetail ON B_MonthlyReportDetail.CompanyID = C_ExceptionTarget.CompanyID
          WHERE     ExceptionType = 5
                    AND B_MonthlyReportDetail.IsDeleted = 0
                    AND C_ExceptionTarget.IsDeleted = 0
                    AND MonthlyReportID = @ID
					AND B_MonthlyReportDetail.TargetID=@TargetID
					AND C_ExceptionTarget.TargetID=@TargetID
        ) AA
        LEFT JOIN C_Company ON AA.CompanyID = C_Company.ID
		WHERE  CompanyName NOT LIKE '%总部%'
        ORDER BY OpeningTime";
            }
            else
            {
                sql = @"SELECT  CompanyID ,
        CompanyName ,
        NAccumulativeActualAmmount ,
        OpeningTime
FROM    ( SELECT    *
          FROM      dbo.A_MonthlyReportDetail
          WHERE     SystemID = @ID
                    AND TargetID = @TargetID
                    AND FinYear = @FinYear
                    AND FinMonth = @FinMonth
                    AND IsDeleted = 0
        ) aa
        RIGHT JOIN ( SELECT CompanyName ,
                            C_Company.ID ,
                            OpeningTime
                     FROM   dbo.C_ExceptionTarget
                            LEFT JOIN dbo.C_Company ON C_Company.ID = C_ExceptionTarget.CompanyID
                     WHERE  TargetID = @TargetID
                            AND ExceptionType = 5
                            AND C_Company.IsDeleted = 0
                            AND C_ExceptionTarget.IsDeleted = 0
                   ) bb ON bb.ID = aa.CompanyID
WHERE   NAccumulativeActualAmmount != 0
        AND CompanyName NOT LIKE '%总部%'
ORDER BY OpeningTime
";
            }
            SqlParameter pSystemID = CreateSqlParameter("@ID", System.Data.DbType.Guid, ID);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);
            SqlParameter pTargetID = CreateSqlParameter("@TargetID", System.Data.DbType.Guid, TargetID);
            var ds = DbHelper.RunSqlReturnDS(sql, ConnectionName, pSystemID, pFinYear, pFinMonth, pTargetID);
            List<ContrastAllCompanyVM> data = new List<ContrastAllCompanyVM>();
            ds.Tables[0].Rows.Cast<System.Data.DataRow>().ForEach(row =>
            {
                ContrastAllCompanyVM item = new ContrastAllCompanyVM();
                ORMapping.DataRowToObject(row, item);
                data.Add(item);
            });
            return data;
        }
     





	}
}

