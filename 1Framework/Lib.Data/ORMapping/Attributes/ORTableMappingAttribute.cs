using System;

namespace Lib.Data
{
    /// <summary>
    /// 加在类定义之前，用于表示表名的Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = false, Inherited = true)]
    public class ORTableMappingAttribute : System.Attribute
    {
        private string tableName = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="tblName">表名</param>
        public ORTableMappingAttribute(string tblName)
        {
            this.tableName = tblName;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName
        {
            get { return this.tableName; }
            private set { this.tableName = value; }
        }
    }
}
