using System;

namespace Lib.Data
{
    public class SelectSqlClauseBuilderItem : SqlClauseBuilderItemBase
    {
        /// <summary>
        /// 
        /// </summary>
        private string _dataField = string.Empty;

        /// <summary>
        /// Sql语句中的字段名
        /// </summary>
        public string DataField
        {
            get { return this._dataField; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("DataField", "值不能为空");
                }
                this._dataField = value;
            }
        }

        public override string GetDataDesp(ISqlBuilder builder)
        {
            return string.Empty;
        }
    }
}
