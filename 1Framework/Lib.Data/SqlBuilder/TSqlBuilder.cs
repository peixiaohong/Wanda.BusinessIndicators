using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace Lib.Data
{
    /// <summary>
    /// 基于T-SQL的ISqlBuilder的实现类
    /// </summary>
    public class TSqlBuilder : SqlBuilderBase
    {
        /// <summary>
        /// TSqlBuilder的实例
        /// </summary>
        public static readonly TSqlBuilder Instance = new TSqlBuilder();

        private TSqlBuilder()
        {
        }

        /// <summary>
        /// 获取数据库同步时间函数
        /// </summary>
        public override string DBCurrentTimeFunction
        {
            get
            {
                return "GETDATE()";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullStr"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string DBNullToString(string data, string nullStr, bool addQuotation)
        {
            return string.Format("ISNULL({0}, {1})", AddQuotation(data, addQuotation), nullStr);
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStatementBegin
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStatementEnd
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStatementSeperator
        {
            get
            {
                return "\n";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStrConcatSymbol
        {
            get
            {
                return "+";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override string FormatDateTime(DateTime dt)
        {
            return this.AddQuotation(dt.ToString("yyyyMMdd HH:mm:ss.fff"), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string GetDBByteLengthFunction(string data, bool addQuotation)
        {
            return string.Format("DATALENGTH({0})", AddQuotation(data, addQuotation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string GetDBStringLengthFunction(string data, bool addQuotation)
        {
            return string.Format("LEN({0})", AddQuotation(data, addQuotation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string GetDBSubStringStr(string data, int start, int length, bool addQuotation)
        {
            return string.Format("SUBSTRING({0}, {1}, {2})", AddQuotation(data, addQuotation), start, length);
        }

        public override DbParameter CreateDbParameter()
        {
            return new SqlParameter();
        }

        public override DbParameter CreateDbParameter(string paramName, DbType paramType)
        {
            return new SqlParameter(paramName, paramType);
        }
    }
}
