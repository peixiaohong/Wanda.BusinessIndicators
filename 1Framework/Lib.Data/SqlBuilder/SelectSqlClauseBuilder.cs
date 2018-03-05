using System;
using Lib.Core;
using System.Text;

namespace Lib.Data
{
    public class SelectSqlClauseBuilder : SqlClauseBuilderBase
    {
        public string TableName { get; set; }

        public void AppendItem(string fieldName)
        {
            SelectSqlClauseBuilderItem item = new SelectSqlClauseBuilderItem();

            if (string.IsNullOrEmpty(TableName))
            {
                item.DataField = fieldName;
            }
            else
            {
                item.DataField = string.Format("{0}.{1}", TableName, fieldName);
            }

            List.Add(item);
        }




        public override string ToSqlString(ISqlBuilder sqlBuilder)
        {
            ExceptionHelper.TrueThrow(sqlBuilder == null, "{0} 不能为空", sqlBuilder);

            StringBuilder builder = new StringBuilder();

            foreach (SelectSqlClauseBuilderItem item in List)
            {
                if (builder.Length > 0)
                    builder.Append(",");

                builder.Append(item.DataField);

                string sqlValue = item.GetDataDesp(sqlBuilder);
                if (!string.IsNullOrEmpty(sqlValue))
                {
                    builder.Append(" ");
                    builder.Append(sqlValue);
                }
            }

            return builder.ToString();
        }
    }
}
