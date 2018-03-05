using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Lib.Data.Properties;

namespace Lib.Data
{
    /// <summary>
    /// SQL Server数据库对象
    /// </summary>
    public class SqlDatabase : Database
    {
        /// <summary>
        /// SQL Server 存储过程参数的标识符号
        /// </summary>
        protected char parameterToken = '@';

        /// <summary>
        /// 通过逻辑名称构造数据库对象实例
        /// </summary>
        /// <param name="name">逻辑名称</param>
        public SqlDatabase(string name)
            : base(name)
        {
            this.factory = SqlClientFactory.Instance;
        }

        /// <summary>
        /// 根据Command对象指向存储过程获取其所需的参数组
        /// </summary>
        /// <param name="discoveryCommand"></param>
        protected override void DeriveParameters(DbCommand discoveryCommand)
        {
            SqlCommandBuilder.DeriveParameters((SqlCommand)discoveryCommand);
        }

        /// <summary>
        /// 获得特定数据库类型下，Parameter在Command中的起始位置。
        /// </summary>
        /// <returns>起始下标</returns>
        protected override int UserParametersStartIndex()
        {
            return 1;
        }

        /// <summary>
        /// 根据数据库类型提供指定的参数名称
        /// </summary>
        /// <remarks>为了真正做到数据库无关，建议所有参数名称均通过该方法进行参数名称匹配</remarks>
        /// <param name="name">应用定义的参数名称</param>
        /// <returns>根据SQL Server命名规则处理后的参数名称</returns>
        public override string BuildParameterName(string name)
        {
            return (name[0] != parameterToken) ? Convert.ToString(parameterToken) + name : name;
        }

        /// <summary>
        /// 判断Command对象所需的参数数量是否与待赋值的数组成员数量匹配
        /// </summary>
        /// <remarks>SQL Server是N+1:N匹配</remarks>
        /// <param name="command">Command对象</param>
        /// <param name="values">待赋值的数组</param>
        /// <returns>是否匹配</returns>
        protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
        {
            return command.Parameters.Count - 1 == values.Length;   // SQL Server默认有一个返回参数
        }

        /// <summary>
        /// 对于存储过程（尤其是Function）返回结果的参数名称
        /// </summary>
        protected override string DefaultReturnValueParameterName
        {
            get
            {
                return "@RETURN_VALUE";
            }
        }

        /// <summary>
        /// 生成一个Parameter对象，同时为期赋值
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            byte precision,
            byte scale,
            string sourceColumn,
            DataRowVersion sourceVersion,
            object value)
        {
            SqlParameter param = parameter as SqlParameter;
            param.ParameterName = parameterName;

            if ((dbType.Equals(DbType.Object)) && (value is byte[]))
            {
                param.SqlDbType = SqlDbType.Image;
            }
            else
            {
                param.DbType = dbType;
            }

            param.Size = size;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.Precision = precision;
            param.Scale = scale;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            param.Value = (value == null) ? DBNull.Value : value;
        }

        /// <summary>
        /// 生成一个Parameter对象
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <param name="nullable"></param>
        /// <param name="sourceColumn"></param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected override void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            SqlParameter param = parameter as SqlParameter;
            param.ParameterName = parameterName;

            if (dbType.Equals(DbType.Object))
            {
                param.SqlDbType = SqlDbType.Image;
            }
            else
            {
                param.DbType = dbType;
            }

            param.Size = size;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
        }

        /// <summary>
        /// 生成一个Parameter对象
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <param name="sourceColumn"></param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected override void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            this.ConfigureParameter(parameter, parameterName, dbType, size, direction, true, sourceColumn);
        }


        #region region Batch Event Mechanism added by wangxiang . May 21, 2008

        /// <summary>
        /// 为DataAdapter更新过程设置事件委托
        /// </summary>
        /// <param name="adapter">Data Adapter</param>
        protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter)
        {
            ((SqlDataAdapter)adapter).RowUpdated +=
                new SqlRowUpdatedEventHandler(this.OnSqlRowUpdated);
        }

        /// <summary>
        /// 对记录更新过程的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rowThatCouldNotBeWritten"></param>
        private void OnSqlRowUpdated(object sender, SqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
        {
            if (rowThatCouldNotBeWritten.RecordsAffected == 0)
            {
                if (rowThatCouldNotBeWritten.Errors != null)
                {
                    rowThatCouldNotBeWritten.Row.RowError = Resource.ExceptionMessageUpdateDataSetRowFailure;
                    rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }

        #endregion
    }
}
