using System;
using System.Data;
using System.Data.Common;

namespace Lib.Data
{
    /// <summary>
    /// ISqlBuilder的实现基类
    /// </summary>
    public abstract class SqlBuilderBase : ISqlBuilder
    {
        #region ISqlBuilder 成员
        /// <summary>
        /// 检查并修改引号标记
        /// </summary>
        /// <param name="data">需要检查的字符串</param>
        /// <param name="addQuotation">是否添加</param>
        /// <returns>返回检查后的字符串</returns>
        public virtual string CheckQuotationMark(string data, bool addQuotation)
        {
            string result = data.Replace("'", "''");

            if (addQuotation)
                result = "'" + result + "'";

            return result;
        }

 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string GetDBStringLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string GetDBByteLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string GetDBSubStringStr(string data, int start, int length, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract string FormatDateTime(DateTime dt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullStr"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string DBNullToString(string data, string nullStr, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBCurrentTimeFunction
        { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStrConcatSymbol
        { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStatementBegin
        { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStatementEnd
        { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStatementSeperator
        { get; }

        public abstract DbParameter CreateDbParameter();

        public abstract DbParameter CreateDbParameter(string paramName, DbType paramType);

        #endregion

        /// <summary>
        /// 添加引号
        /// </summary>
        /// <param name="data">需要操作的字符串</param>
        /// <param name="addQuotation">添加引号</param>
        /// <returns></returns>
        protected virtual string AddQuotation(string data, bool addQuotation)
        {
            string result = data;

            if (addQuotation)
                result = CheckQuotationMark(data, addQuotation);

            return result;
        }
    }
}
