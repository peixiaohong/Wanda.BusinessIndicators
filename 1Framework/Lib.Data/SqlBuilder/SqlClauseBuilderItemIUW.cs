using System;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 每一个构造项，包括字段名称和字段的值等内容
    /// </summary>
    [Serializable]
    public class SqlClauseBuilderItemIUW : SqlCaluseBuilderItemWithData
    {
        private string operation = SqlClauseBuilderBase.EqualTo;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SqlClauseBuilderItemIUW()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private string dataField = string.Empty;

        /// <summary>
        /// Sql语句中的字段名
        /// </summary>
        public string DataField
        {
            get { return this.dataField; }
            set
            {
                ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField属性不能为空或空字符串");
                this.dataField = value;
            }
        }

        /// <summary>
        /// 字段和数据之间的操作符
        /// </summary>
        public string Operation
        {
            get { return this.operation; }
            set { this.operation = value; }
        }
    }
}
