using System;
using System.Data;
using System.Data.Common;
using Lib.Core;
using Lib.Data.Properties;

namespace Lib.Data
{
    /// <summary>
    /// 抽象数据库实体类
    /// </summary>
    public abstract class Database
    {
        #region Protected Fields
        /// <summary>
        /// 当前数据库对象的逻辑名称
        /// </summary>
        private string name;
        #endregion

        #region Private Fields
        /// <summary>
        /// 数据库引擎工厂
        /// </summary>
        protected DbProviderFactory factory;//Modify By Yuanyong 20080320

        private bool hasCustomizedDbEventArgs;      // 是否为这个逻辑数据库配置了事件机制
        private static DbParameterCache cache = new DbParameterCache(); // 线程安全的
        #endregion

        #region Private Consts
        /// <summary>
        /// 系统默认输出DataSet中的数据表名称
        /// </summary>
        /// <remarks>
        /// 系统默认输出DataSet中的数据表名称(常量)，用于处理ExecuteDataSet和LoadDataSet中的TableName命名处理
        /// </remarks>
        protected const string SystemCreatedTableNameRoot = "Table";
        #endregion

        #region Public Fields
        /// <summary>
        /// 数据库逻辑名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 数据库调用执行之前的事件
        /// </summary>
        public event DbEventHandler BeforeExecution;
        /// <summary>
        /// 数据库调用执行之后的事件
        /// </summary>
        public event DbEventHandler AfterExecution;

        // 数据库调用出现异常的事件并没有列出，考虑到执行效率，整个Database机制对错误处理的策略是“直接抛出”
        #endregion

        #region Constructor
        /// <summary>
        /// 通过逻辑名称构造数据库对象实例
        /// </summary>
        /// <param name="name"></param>
        protected Database(string name)
        {
            this.name = name;
            //this.factory = DbConnectionManager.GetDbProviderFactory(this.name); Del By Yuanyong 20080320
            this.hasCustomizedDbEventArgs = DbConnectionManager.GetEventArgsType(this.name) == null ? false : true;
        }
        #endregion

        #region LoadDataSet
        #region Del
        ///// <summary>
        ///// 向DataSet中填充SQL查询返回的结果
        ///// </summary>
        ///// <param name="commandType">Command类型</param>
        ///// <param name="commandText">Command命令内容</param>
        ///// <param name="dataSet">待填充的DataSet</param>
        //public virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet)
        //{
        //    LoadDataSet(commandType, commandText, dataSet, SystemCreatedTableNameRoot);
        //}

        ///// <summary>
        ///// 向DataSet中填充SQL返回的结果
        ///// </summary>
        ///// <param name="commandType">Command类型</param>
        ///// <param name="commandText">Command命令内容</param>
        ///// <param name="dataSet">待填充的DataSet</param>
        ///// <param name="tableName">查询结果的DataTable名称</param>
        //public virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string tableName)
        //{
        //    LoadDataSet(commandType, commandText, dataSet, new string[] { tableName });
        //}
        #endregion
        /// <summary>
        /// 向DataSet中填充SQL返回的结果
        /// </summary>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        public virtual void LoadDataSet(CommandType commandType,
            string commandText,
            DataSet dataSet,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, pageNo, pageSize, tableNames);
            }
        }
        /// <summary>
        /// 向DataSet中填充SQL返回的结果
        /// </summary>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        public virtual void LoadDataSet(CommandType commandType,
            string commandText,
            DataSet dataSet,
            params string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }
        /// <summary>
        /// 向DataSet中填充存储过程返回的结果
        /// </summary>
        /// <remarks>如果是Oracle查询，需要在定义存储过程的时候把REF CURSOR放在其他参数的后面</remarks>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableName">查询结果的DataTable名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        public virtual void LoadDataSet(string storedProcedureName,
            DataSet dataSet,
            string tableName,
            params object[] parameterValues)
        {
            LoadDataSet(storedProcedureName, dataSet, new string[] { tableName }, parameterValues);
        }
        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <remarks>如果是Oracle查询，需要在定义存储过程的时候把REF CURSOR放在其他参数的后面</remarks>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        public virtual void LoadDataSet(string storedProcedureName,
            DataSet dataSet,
            string[] tableNames,
            params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }
        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        public virtual void LoadDataSet(DbCommand command,
            DataSet dataSet,
            params string[] tableNames)
        {
            //DoLoadDataSet(command, dataSet, tableNames);
            this.LoadDataSet(command, dataSet, 0, 0, tableNames);
        }
        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        public virtual void LoadDataSet(DbCommand command,
            DataSet dataSet,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            this.DoLoadDataSet(command, dataSet, pageNo, pageSize, tableNames);
        }
        #region del by yuanyong
        ///// <summary>
        ///// 向DataSet中填充Command返回的结果
        ///// </summary>
        ///// <param name="command">Command实例</param>
        ///// <param name="dataSet">待填充的DataSet</param>
        ///// <param name="tableName">查询结果的DataTable名称</param>
        //public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        //{
        //    LoadDataSet(command, dataSet, new string[] { tableName });
        //}

        ///// <summary>
        ///// 向DataSet中填充Command返回的结果
        ///// </summary>
        ///// <param name="command">Command实例</param>
        ///// <param name="dataSet">待填充的DataSet</param>
        //public virtual void LoadDataSet(DbCommand command, DataSet dataSet)
        //{
        //    LoadDataSet(command, dataSet, SystemCreatedTableNameRoot);
        //}
        #endregion
        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// 返回存储过程查询结果
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>查询结果</returns>
        public virtual DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            return ExecuteDataSet(storedProcedureName, new string[] { SystemCreatedTableNameRoot }, parameterValues);
        }
        #region Del
        ///// <summary>
        ///// 返回存储过程查询结果
        ///// </summary>
        ///// <param name="storedProcedureName">存储过程名称</param>
        ///// <param name="tableName">查询结果的DataTable名称</param>
        ///// <param name="parameterValues">存储过程参数赋值</param>
        ///// <returns>查询结果</returns>
        //public virtual DataSet ExecuteDataSet(string storedProcedureName, string tableName, params object[] parameterValues)
        //{
        //    return ExecuteDataSet(storedProcedureName, new string[] { tableName }, parameterValues);
        //}
        #endregion
        /// <summary>
        /// 返回存储过程查询结果
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>查询结果</returns>
        public virtual DataSet ExecuteDataSet(string storedProcedureName, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteDataSet(command, tableNames);
            }
        }
        #region del
        ///// <summary>
        ///// 返回查询结果
        ///// </summary>
        ///// <param name="commandType">Command类型</param>
        ///// <param name="commandText">Command命令内容</param>
        ///// <returns>查询结果</returns>
        //public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText)
        //{
        //    return ExecuteDataSet(commandType, commandText, SystemCreatedTableNameRoot);
        //}

        ///// <summary>
        ///// 返回查询结果
        ///// </summary>
        ///// <param name="commandType">Command类型</param>
        ///// <param name="commandText">Command命令内容</param>
        ///// <param name="tableName">查询结果的DataTable名称</param>
        ///// <returns>查询结果</returns>
        //public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText, string tableName)
        //{
        //    return ExecuteDataSet(commandType, commandText, new string[]{tableName});
        //}
        #endregion
        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText, params string[] tableNames)
        {
            //using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            //{
            //    return ExecuteDataSet(command, tableNames);
            //}
            return ExecuteDataSet(commandType, commandText, 0, 0, tableNames);
        }
        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="command">Command实例</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command, params string[] tableNames)
        {
            //DataSet dataSet = new DataSet();
            //dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;

            //LoadDataSet(command, dataSet, tableNames);
            //return dataSet;
            return ExecuteDataSet(command, 0, 0, tableNames);
        }
        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        public virtual DataSet ExecuteDataSet(CommandType commandType,
            string commandText,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command, pageNo, pageSize, tableNames);
            }
        }
        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="command">Command实例</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command, int pageNo, int pageSize, params string[] tableNames)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;

            LoadDataSet(command, dataSet, pageNo, pageSize, tableNames);
            return dataSet;
        }
        #region Del
        ///// <summary>
        ///// 返回查询结果
        ///// </summary>
        ///// <param name="command">Command实例</param>
        ///// <param name="tableName">查询结果的DataTable名称</param>
        ///// <returns>查询结果</returns>
        //public virtual DataSet ExecuteDataSet(DbCommand command, string tableName)
        //{
        //    return ExecuteDataSet(command, new string[] { tableName });
        //}

        ///// <summary>
        ///// 返回查询结果
        ///// </summary>
        ///// <param name="command">Command实例</param>
        ///// <returns>查询结果</returns>
        //public virtual DataSet ExecuteDataSet(DbCommand command)
        //{
        //    return ExecuteDataSet(command, SystemCreatedTableNameRoot);
        //}
        #endregion
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行Command返回单值
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <returns>单值</returns>
        public virtual object ExecuteScalar(DbCommand command)
        {
            return DoExecuteScalar(command);
        }

        /// <summary>
        /// 执行存储过程返回单值
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>单值</returns>
        public virtual object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// 执行指定查询返回单值
        /// </summary>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">SQL语句或者SPName，与commandType匹配使用</param>
        /// <returns>单值</returns>
        public virtual object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        #endregion

        #region ExecuteReader
        /// <summary>
        /// 返回一个DataReader对象
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>对于存储过程方式返回DataReader需包括有返回查询结果的情况</item>
        ///     <item>需要外部应用显示关闭Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="command">命令实例</param>
        /// <returns>DataReader对象</returns>
        public virtual DbDataReader ExecuteReader(DbCommand command)
        {
            return DoExecuteReader(command);
        }

        /// <summary>
        /// 返回一个DataReader对象
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>对于存储过程方式返回DataReader需包括有返回查询结果的情况</item>
        ///     <item>需要外部应用显示关闭Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>DataReader对象</returns>
        public DbDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteReader(command);
            }
        }

        /// <summary>
        /// 返回一个DataReader对象
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>对于存储过程方式返回DataReader需包括有返回查询结果的情况</item>
        ///     <item>需要外部应用显示关闭Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>DataReader对象</returns>
        public DbDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command);
            }
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行数据更新操作(DML)
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <returns>受影响的行数</returns>
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// 执行数据更新操作(DML)
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>受影响的行数</returns>
        public virtual int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = InitStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 执行数据更新操作(DML)
        /// </summary>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>受影响的行数</returns>
        public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }
        #endregion

        #region UpdateDataSet 面向批量处理增加的方法 added by wangxiang . May 21, 2008

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="dataSet">待更新的数据</param>
        /// <param name="tableName">需要更新的数据表名称</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataSet dataSet, string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return DoUpdateDataSet(behavior, dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="dataSet">待更新的数据</param>
        /// <param name="tableName">需要更新的数据表名称</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand)
        {
            return DoUpdateDataSet(behavior, dataSet, tableName, insertCommand, updateCommand, deleteCommand, null);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="dataSet">待更新的数据</param>
        /// <param name="tableName">需要更新的数据表名称</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return UpdateDataSet(UpdateBehavior.Transactional, dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="dataSet">待更新的数据</param>
        /// <param name="tableName">需要更新的数据表名称</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand)
        {
            return UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, null);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return DoUpdateDataSet(behavior, table, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(UpdateBehavior behavior,
            DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand)
        {
            return DoUpdateDataSet(behavior, table, insertCommand, updateCommand, deleteCommand, null);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns></returns>
        public int UpdateDataSet(DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            return UpdateDataSet(UpdateBehavior.Transactional, table, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <returns></returns>
        public int UpdateDataSet(DataTable table, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            return UpdateDataSet(table, insertCommand, updateCommand, deleteCommand, null);
        }

        #region 衍生的方法

        #region BatchInsert

        /// <summary>
        /// 批量Insert
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns>影响行数</returns>
        public int BatchInsert(UpdateBehavior behavior, DataTable table, DbCommand insertCommand, int? updateBatchSize)
        {
            return UpdateDataSet(behavior, table, insertCommand, null, null, updateBatchSize);
        }

        /// <summary>
        /// 批量Insert
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns>影响行数</returns>
        public int BatchInsert(DataTable table, DbCommand insertCommand, int? updateBatchSize)
        {
            return UpdateDataSet(table, insertCommand, null, null, updateBatchSize);
        }

        /// <summary>
        /// 批量Insert
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <returns>影响行数</returns>
        public int BatchInsert(UpdateBehavior behavior, DataTable table, DbCommand insertCommand)
        {
            return UpdateDataSet(behavior, table, insertCommand, null, null, null);
        }

        /// <summary>
        /// 批量Insert
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <returns>影响行数</returns>
        public int BatchInsert(DataTable table, DbCommand insertCommand)
        {
            return UpdateDataSet(table, insertCommand, null, null, null);
        }

        #endregion

        #region BatchUpdate

        /// <summary>
        /// 批量Update
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns>影响行数</returns>
        public int BatchUpdate(UpdateBehavior behavior, DataTable table, DbCommand updateCommand, int? updateBatchSize)
        {
            return UpdateDataSet(behavior, table, null, updateCommand, null, updateBatchSize);
        }

        /// <summary>
        /// 批量Update
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns>影响行数</returns>
        public int BatchUpdate(DataTable table, DbCommand updateCommand, int? updateBatchSize)
        {
            return UpdateDataSet(table, null, updateCommand, null, updateBatchSize);
        }

        /// <summary>
        /// 批量Update
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <returns>影响行数</returns>
        public int BatchUpdate(UpdateBehavior behavior, DataTable table, DbCommand updateCommand)
        {
            return UpdateDataSet(behavior, table, null, updateCommand, null, null);
        }

        /// <summary>
        /// 批量Update
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <returns>影响行数</returns>
        public int BatchUpdate(DataTable table, DbCommand updateCommand)
        {
            return UpdateDataSet(table, null, updateCommand, null, null);
        }

        #endregion

        #region BatchDelete

        /// <summary>
        /// 批量Delete
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(UpdateBehavior behavior, DataTable table, DbCommand BatchDelete, int? updateBatchSize)
        {
            return UpdateDataSet(behavior, table, null, null, BatchDelete, updateBatchSize);
        }

        /// <summary>
        /// 批量Delete
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(DataTable table, DbCommand BatchDelete, int? updateBatchSize)
        {
            return UpdateDataSet(table, null, null, BatchDelete, updateBatchSize);
        }

        /// <summary>
        /// 批量Delete
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(UpdateBehavior behavior, DataTable table, DbCommand BatchDelete)
        {
            return UpdateDataSet(behavior, table, null, null, BatchDelete, null);
        }

        /// <summary>
        /// 批量Delete
        /// </summary>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="BatchDelete">BatchDeleteDbCommand</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(DataTable table, DbCommand BatchDelete)
        {
            return UpdateDataSet(table, null, null, BatchDelete, null);
        }

        #endregion

        #endregion

        #endregion

        #region Command and Stored Procedure Mechanism
        /// <summary>
        /// 根据Command对象指向存储过程获取其所需的参数组
        /// <remarks>
        ///     将参数发现机制以IOC方式交给实体Database类完成
        /// </remarks>
        /// </summary>
        protected abstract void DeriveParameters(DbCommand discoveryCommand);

        /// <summary>
        /// 对于存储过程（尤其是Function），一般ExecuteScalar的返回结果保存在RETURN_VALUE中，
        /// 但不同数据库该参数命名不同，因此需要由各数据库实例自己实现该属性
        /// </summary>
        protected abstract string DefaultReturnValueParameterName { get; }

        /// <summary>
        /// 根据Command对象指向存储过程获取其所需的参数组
        /// </summary>
        internal void DiscoverParameters(DbCommand command)
        {
            //if ((command == null) || (command.CommandType != CommandType.StoredProcedure)) return;
            if (command != null && command.CommandType == CommandType.StoredProcedure)
            {
                using (DbContext context = DbContext.GetContext(this.name))
                {
                    command.Connection = context.Connection;
                    using (DbCommand discoveryCommand = CreateCommandByCommandType(command))
                    {
                        //OpenConnection(command);

                        discoveryCommand.Connection = command.Connection;
                        DeriveParameters(discoveryCommand);
                        foreach (IDataParameter parameter in discoveryCommand.Parameters)
                        {
                            IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                            command.Parameters.Add(cloneParameter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取执行指定存储过程需要的Command实例
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">对应的一组参数赋值</param>
        /// <returns>Command实例</returns>
        /// <remarks>获取执行指定存储过程需要的Command实例，此时要求其参数匹配齐全否则内部的参数检查将报出异常。</remarks>
        ///// <param name="connection">数据库Connection实例</param>
        //protected virtual DbCommand GetStoredProcedureCommand(string storedProcedureName, DbConnection connection, params object[] parameterValues)
        public virtual DbCommand InitStoredProcedureCommand(string storedProcedureName, params object[] parameterValues)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(storedProcedureName), "storedProcedureName");

            DbCommand command = CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);

            cache.SetParameters(command, this);

            ExceptionHelper.FalseThrow<InvalidOperationException>(SameNumberOfParametersAndValues(command, parameterValues), "parameterValues");

            AssignParameterValues(command, parameterValues);

            return command;
        }

        /// <summary>
        /// 创建一个存储过程Command对象
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <returns>制定的Command对象</returns>
        /// <remarks>创建一个存储过程Command对象，仅仅用于创建而不做任何参数检查处理。</remarks>
        public virtual DbCommand CreateStoredProcedureCommand(string storedProcedureName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(storedProcedureName), "storedProcedureName");
            //if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentNullException("storedProcedureName");
            return CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
        }

        /// <summary>
        /// 获的一个可以执行SQL语句的Command对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>Command对象</returns>
        public DbCommand GetSqlStringCommand(string sql)
        {
            return CreateCommandByCommandType(CommandType.Text, sql);
        }

        /// <summary>
        /// 判断Command对象所需的参数数量是否与待赋值的数组成员数量匹配
        /// </summary>
        /// <param name="command">Command对象</param>
        /// <param name="values">待赋值的数组</param>
        /// <returns>是否匹配</returns>
        protected virtual bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
        {
            return command.Parameters.Count == values.Length;
        }

        #endregion

        #region Direct Execution Methods
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="dataSet">待更新的数据</param>
        /// <param name="tableName">需要更新的数据表名称</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns></returns>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        private int DoUpdateDataSet(UpdateBehavior behavior,
            DataSet dataSet,
            string tableName,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            return DoUpdateDataSet(behavior, dataSet.Tables[tableName], insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="behavior">更新行为</param>
        /// <param name="table">需要更新的数据表</param>
        /// <param name="insertCommand">增加数据DbCommand</param>
        /// <param name="updateCommand">更新数据DbCommand</param>
        /// <param name="deleteCommand">删除数据DbCommand</param>
        /// <param name="updateBatchSize">每批更新的数据量</param>
        /// <returns></returns>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        private int DoUpdateDataSet(UpdateBehavior behavior,
            DataTable table,
            DbCommand insertCommand,
            DbCommand updateCommand,
            DbCommand deleteCommand,
            int? updateBatchSize)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            if (insertCommand == null && updateCommand == null && deleteCommand == null)
            {
                throw new ArgumentNullException("command");
            }

            using (DbDataAdapter adapter = GetDataAdapter(behavior))
            {
                IDbDataAdapter explicitAdapter = (IDbDataAdapter)adapter;

                if (insertCommand != null)
                {
                    explicitAdapter.InsertCommand = insertCommand;
                }
                if (updateCommand != null)
                {
                    explicitAdapter.UpdateCommand = updateCommand;
                }
                if (deleteCommand != null)
                {
                    explicitAdapter.DeleteCommand = deleteCommand;
                }

                if (updateBatchSize != null)
                {
                    adapter.UpdateBatchSize = (int)updateBatchSize;
                    if (insertCommand != null)
                        adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
                    if (updateCommand != null)
                        adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
                    if (deleteCommand != null)
                        adapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;
                }

                int rows = adapter.Update(table);
                return rows;
            }
        }

        /// <summary>
        /// 执行Command返回单值
        /// </summary>
        /// <param name="command">Command实例</param>
        /// <returns>单值</returns>
        private object DoExecuteScalar(DbCommand command)
        {
            //OpenConnection(command);

            ExceptionHelper.TrueThrow<ArgumentException>(command.CommandType == CommandType.TableDirect,
                Resource.ExecuteScalarNotSupportTableDirectException);

            using (DbContext context = DbContext.GetContext(this.name))
            {
                command.Connection = context.Connection;

                DoDbEvent(command, DbEventType.BeforeExecution);
                object returnValue = command.ExecuteScalar();
                if (command.CommandType != CommandType.Text)
                {
                    // 由于 SQL Server 在Stored Procedure和Function返回值处理方式上存在不同，因此增加了适应性修改
                    returnValue = (returnValue == null) ? command.Parameters[DefaultReturnValueParameterName].Value : returnValue;
                }
                DoDbEvent(command, DbEventType.AfterExecution);

                return returnValue;
            }
        }

        private int DoExecuteNonQuery(DbCommand command)
        {
            //OpenConnection(command);
            using (DbContext context = DbContext.GetContext(this.name))
            {
                command.Connection = context.Connection;

                DoDbEvent(command, DbEventType.BeforeExecution);
                int rowsAffected = command.ExecuteNonQuery();
                DoDbEvent(command, DbEventType.AfterExecution);

                return rowsAffected;
            }
        }

        private DbDataReader DoExecuteReader(DbCommand command)
        {
            if (null == command.Connection)
                command.Connection = DbContext.GetContext(this.name, false).Connection;
            if (ConnectionState.Open != command.Connection.State)
                command.Connection.Open();

            DoDbEvent(command, DbEventType.BeforeExecution);
            DbDataReader reader = command.ExecuteReader();
            DoDbEvent(command, DbEventType.AfterExecution);

            return reader;
        }

        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        protected void DoLoadDataSet(DbCommand command, DataSet dataSet, int pageNo, int pageSize, string[] tableNames)
        {
            for (int i = 0; i < tableNames.Length; i++)
                ExceptionHelper.CheckStringIsNullOrEmpty(tableNames[i], "tableNames[" + i + "]");

            //OpenConnection(command);
            using (DbContext context = DbContext.GetContext(this.name))
            {
                command.Connection = context.Connection;

                using (DbDataAdapter adapter = this.factory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemCreatedTableName = (i == 0) ? SystemCreatedTableNameRoot : SystemCreatedTableNameRoot + i;
                        adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                    }

                    DoDbEvent(command, DbEventType.BeforeExecution);
                    if (pageSize == 0)
                        adapter.Fill(dataSet);
                    else
                    {
                        string srcTableName = tableNames.Length > 0 ? tableNames[0] : "Table";
                        adapter.Fill(dataSet, pageNo * pageSize, pageSize, srcTableName);
                    }
                    DoDbEvent(command, DbEventType.AfterExecution);
                }
            }
        }

        ///// <summary>
        ///// 将Command对象中设置的Connection对象Open
        ///// </summary>
        ///// <param name="command">待Open的Connection所在的Command对象</param>
        ///// <remarks>将Command对象中设置的Connection对象Open。这里要求处理Command对象以及command.Connection是否存在。</remarks>
        //protected static void OpenConnection(DbCommand command)
        //{
        //    ExceptionHelper.TrueThrow<ArgumentNullException>(command == null, "command");
        //    ExceptionHelper.TrueThrow<ArgumentNullException>(command.Connection == null, "DbCommand未设置DbConnection");

        //    if (command.Connection.State != ConnectionState.Open)
        //    {
        //        Trace.WriteLine(command.Connection.DataSource + "." + command.Connection.Database
        //            + "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]", 
        //            " Open Connection ");
        //        command.Connection.Open();
        //    }
        //}
        #endregion

        #region Parameter Mechanism
        /// <summary>
        /// 根据数据库类型提供指定的参数名称
        /// <remarks>
        ///     为了真正做到数据库无关，建议所有参数名称均通过该方法进行参数名称匹配。
        ///     例如存储过程参数entryId, 在Oracle中采用entryId，而在SQL Server中采用@entryId
        /// </remarks>
        /// </summary>
        /// <param name="parameterName">应用定义的参数名称</param>
        /// <returns>根据不同数据库命名规则处理后的参数名称</returns>
        public virtual string BuildParameterName(string parameterName)
        {
            return parameterName;
        }

        /// <summary>
        /// 为一个Parameter对象赋值
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="value">值</param>
        /// <param name="parameterName">数据库参数名称</param>
        public virtual void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            if (value is DbParameter)
            {
                command.Parameters[BuildParameterName(parameterName)] = (DbParameter)value;
            }
            else
            {
                command.Parameters[BuildParameterName(parameterName)].Value = (value == null) ? DBNull.Value : value;
            }
        }

        /// <summary>
        /// 获取一个Parameter对象的值
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="parameterName">数据库参数名称</param>
        public virtual object GetParameterValue(DbCommand command, string parameterName)
        {
            return command.Parameters[BuildParameterName(parameterName)].Value;
        }

        /// <summary>
        /// 依次为Command对象的每个Parameter赋值
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="values">需要添加的一组值</param>
        protected virtual void AssignParameterValues(DbCommand command, object[] values)
        {
            int parameterIndexShift = UserParametersStartIndex();
            for (int i = 0; i < values.Length; i++)
            {
                IDataParameter parameter = command.Parameters[i + parameterIndexShift];
                SetParameterValue(command, parameter.ParameterName, values[i]);
            }
        }

        /// <summary>
        /// 生成一个Parameter对象，同时为其赋值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数对应的数据类型</param>
        /// <param name="size">参数数据长度要求</param>
        /// <param name="direction">参数输入输出类型（枚举）</param>
        /// <param name="nullable">参数是否允许为空</param>
        /// <param name="precision">参数精确值</param>
        /// <param name="scale">参数大小</param>
        /// <param name="sourceColumn">对应的Source的Column</param>
        /// <param name="sourceVersion">对应SourceClumn的版本号</param>
        /// <param name="value">参数具体数据值</param>
        /// <returns>准备好参数内容的DBParameter对象</returns>
        /// <remarks>生成一个Parameter对象，同时为期赋值</remarks>
        protected DbParameter CreateParameter(string parameterName,
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
            DbParameter parameter = CreateParameter(parameterName);
            ConfigureParameter(parameter, parameterName, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return parameter;
        }
        /// <summary>
        /// 生成一个Parameter对象
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数对应的数据类型</param>
        /// <param name="size">参数数据长度要求</param>
        /// <param name="direction">参数输入输出类型（枚举）</param>
        /// <param name="nullable">参数是否允许为空</param>
        /// <param name="sourceColumn">对应的Source的Column</param>
        /// <returns>生成一个Parameter对象，同时为期赋值</returns>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected DbParameter CreateParameter(string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            DbParameter parameter = CreateParameter(parameterName);
            ConfigureParameter(parameter, parameterName, dbType, size, direction, nullable, sourceColumn);
            return parameter;
        }

        /// <summary>
        /// 生成一个Parameter对象
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">执行类型</param>
        /// <param name="size">大小</param>
        /// <param name="direction">参数类型</param>
        /// <param name="sourceColumn">类名称</param>
        /// <returns>生成一个Parameter对象，同时为期赋值</returns>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected DbParameter CreateParameter(string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            DbParameter parameter = CreateParameter(parameterName);
            ConfigureParameter(parameter, parameter.ParameterName, dbType, size, direction, sourceColumn);
            return parameter;
        }

        /// <summary>
        /// 生成一个Parameter对象
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>生成一个Parameter对象，同时为期赋值</returns>
        protected DbParameter CreateParameter(string parameterName)
        {
            DbParameter parameter = this.factory.CreateParameter();
            parameter.ParameterName = BuildParameterName(parameterName);
            return parameter;
        }

        /// <summary>
        /// 获得特定数据库类型下，Parameter在Command中的起始位置。
        /// </summary>
        /// <returns>起始下标</returns>
        protected virtual int UserParametersStartIndex()
        {
            return 0;
        }
        /// <summary>
        /// 根据指定的内容为Prameter赋值
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">操作类型</param>
        /// <param name="size">大小</param>
        /// <param name="direction">参数类型</param>
        /// <param name="nullable">是否可空</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected virtual void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Direction = direction;
            parameter.IsNullable = nullable;
            parameter.SourceColumn = sourceColumn;
        }

        /// <summary>
        /// 根据指定的内容为Prameter赋值
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">操作类型</param>
        /// <param name="size">大小</param>
        /// <param name="direction">参数类型</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected virtual void ConfigureParameter(DbParameter parameter,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            ConfigureParameter(parameter, parameterName, dbType, size, direction, true, sourceColumn);
        }

        /// <summary>
        /// 根据指定的内容为Prameter赋值
        /// </summary>
        /// <param name="dbType">操作类型</param>
        /// <param name="direction">参数输入输出类型</param>
        /// <param name="nullable">是否可空</param>
        /// <param name="parameter">参数</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="precision">精度</param>
        /// <param name="scale">大小</param>
        /// <param name="size">长度</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <param name="sourceVersion">版本</param>
        /// <param name="value">值</param>
        protected virtual void ConfigureParameter(DbParameter parameter,
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
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = (value == null) ? DBNull.Value : value;
            //parameter.Size = size;//modify by ccic\yuanyong 20080409
            parameter.Size = (size == 0 && dbType == DbType.String) ? value.ToString().Length : size;
            parameter.Direction = direction;
            parameter.IsNullable = nullable;
            parameter.SourceColumn = sourceColumn;
            parameter.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// 增加一个Parameter
        /// </summary>
        /// <param name="dbType">操作类型</param>
        /// <param name="direction">参数输入输出类型</param>
        /// <param name="nullable">是否可空</param>
        /// <param name="command">Sql语句</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="precision">精度</param>
        /// <param name="scale">大小</param>
        /// <param name="size">长度</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <param name="sourceVersion">版本</param>
        /// <param name="value">值</param>
        public virtual void AddParameter(DbCommand command,
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
            DbParameter parameter = CreateParameter(parameterName, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }
        /// <summary>
        /// 增加一个Parameter
        /// </summary>
        /// <param name="dbType">操作类型</param>
        /// <param name="direction">参数输入输出类型</param>
        /// <param name="command">Sql语句</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <param name="sourceVersion">版本</param>
        /// <param name="value">值</param>
        public void AddParameter(DbCommand command,
            string parameterName,
            DbType dbType,
            ParameterDirection direction,
            string sourceColumn,
            DataRowVersion sourceVersion,
            object value)
        {
            AddParameter(command, parameterName, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }
        /// <summary>
        /// 增加一个Parameter
        /// </summary>
        /// <param name="command">Sql语句</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">操作类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型</param>
        /// <param name="nullable">是否可空</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        public virtual void AddParameter(DbCommand command,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool nullable,
            string sourceColumn)
        {
            DbParameter parameter = CreateParameter(parameterName, dbType, size, direction, nullable, sourceColumn);

            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// 增加一个Parameter
        /// </summary>
        /// <param name="command">Sql语句</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">操作类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型</param>
        /// <param name="sourceColumn">数据项名称</param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        public virtual void AddParameter(DbCommand command,
            string parameterName,
            DbType dbType,
            int size,
            ParameterDirection direction,
            string sourceColumn)
        {
            DbParameter parameter = CreateParameter(parameterName, dbType, size, direction, sourceColumn);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// 增加一个Out Parameter
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterName">数据库参数名称</param>
        /// <param name="size">长度</param>
        public void AddOutParameter(DbCommand command, string parameterName, DbType dbType, int size)
        {
            AddParameter(command, parameterName, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// 增加一个In Parameter
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterName">数据库参数名称</param>
        public void AddInParameter(DbCommand command, string parameterName, DbType dbType)
        {
            AddInParameter(command, parameterName, dbType, null);
        }
        /// <summary>
        /// 增加一个In Parameter
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterName">数据库参数名称</param>
        /// <param name="value">值</param>
        public void AddInParameter(DbCommand command, string parameterName, DbType dbType, object value)
        {
            AddParameter(command, parameterName, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }
        /// <summary>
        /// 增加一个In Parameter
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterName">数据库参数名称</param>
        /// <param name="sourceColumn">数据库数据项名称</param>
        /// <param name="sourceVersion">类型</param>
        public void AddInParameter(DbCommand command, string parameterName,
            DbType dbType,
            string sourceColumn,
            DataRowVersion sourceVersion)
        {
            AddParameter(command, parameterName, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// 通过事件暴露内部的命令对象
        /// </summary>
        /// <remarks>由于大多数应用不会使用这个特性，出于性能优化的考虑，先把Delegate放在前面判断</remarks>
        /// <param name="command">内部的命令对象</param>
        /// <param name="eventType">数据访问调用时机类型</param>
        protected void DoDbEvent(IDbCommand command, DbEventType eventType)
        {
            if (false == this.hasCustomizedDbEventArgs)
                return;

            if ((BeforeExecution != null) && (eventType == DbEventType.BeforeExecution))
                TriggerDbEvent(command, BeforeExecution);
            else if ((AfterExecution != null) && (eventType == DbEventType.AfterExecution))
                TriggerDbEvent(command, AfterExecution);
        }

        private void TriggerDbEvent(object executor, DbEventHandler targetHandler)
        {
            DbEventArgs args = DbConnectionManager.GetEventArgsType(name);
            args.Executor = executor;
            targetHandler(this, args);     // 触发外部事件
        }
        #endregion

        #region Protected Helper Methods
        /// <summary>
        /// 根据更新特征构造Data Adapter
        /// </summary>
        /// <param name="updateBehavior">
        /// </param>        
        /// <returns>Data Adapter适配器</returns>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>

        protected DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior)
        {
            DbDataAdapter adapter = factory.CreateDataAdapter();

            if (updateBehavior == UpdateBehavior.Continue)
            {
                this.SetUpRowUpdatedEvent(adapter);
            }

            return adapter;
        }

        /// <summary>
        /// 生成简单的DbCommand对象 
        /// <remarks>
        ///     相当于Provider Identpendent的new()出一个Command对象
        /// </remarks>
        /// </summary>
        /// <param name="commandText">Command执行语句</param>
        /// <param name="commandType">Command类型</param>
        /// <returns>Command对象</returns>
        protected DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(commandText), "commandText");

            using (DbContext context = DbContext.GetContext(this.name))
            {
                DbCommand command = this.factory.CreateCommand();

                command.CommandTimeout = (int)DbConnectionManager.GetCommandTimeout(this.name).TotalSeconds;
                command.CommandType = commandType;
                command.CommandText = commandText;
                command.Connection = context.Connection;
                command.Transaction = context.LocalTransaction;

                return command;
            }
        }
        /// <summary>
        /// 生成简单的DbCommand对象 
        /// <remarks>
        ///     相当于Provider Identpendent的new()出一个Command对象
        /// </remarks>
        /// </summary>
        /// <param name="originalCommand">需要执行的数据库语句</param>
        /// <returns>Command对象</returns>
        protected DbCommand CreateCommandByCommandType(IDbCommand originalCommand)
        {
            return CreateCommandByCommandType(originalCommand.CommandType, originalCommand.CommandText);
        }
        /// <summary>
        /// 为DataAdapter更新过程设置事件委托
        /// </summary>
        /// <param name="adapter">Data Adapter</param>
        /// <remarks>
        ///     面向批量处理增加的方法
        ///     added by wangxiang . May 21, 2008
        /// </remarks>
        protected virtual void SetUpRowUpdatedEvent(DbDataAdapter adapter)
        {
        }

        #endregion
    }
}
