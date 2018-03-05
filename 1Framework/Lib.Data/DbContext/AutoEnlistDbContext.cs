using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Transactions;
using System.Threading;
using System.Data;

namespace Lib.Data
{
    internal class AutoEnlistDbContext : DeluxeDbContextBase
    {
        #region private classes
        private class GraphWithTransaction : Dictionary<Transaction, Connections>
        {
        }
        #endregion

        #region Private members

        /// <summary>
        /// Current context entity management target (with transaction support).
        /// <remarks>
        ///     the Key type is a System.Transaction.Transaction
        /// </remarks>
        /// </summary>
        private static GraphWithTransaction graphWithTx = null;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">连接名称</param>
        /// <param name="autoClose">是否自动关闭连接</param>
        public AutoEnlistDbContext(string name, bool autoClose)
            : base(name, autoClose)
        {
        }

        #region Protected methods
        /// <summary>
        /// 重写获取当前事物所使用的数据连接的函数
        /// </summary>
        /// <param name="ts">事物对象</param>
        /// <returns>数据连接</returns>
        protected override DbConnection OnGetConnectionWithTransaction(Transaction ts)
        {
            Connections connections = null;

            GraphWithTransaction graph = AutoEnlistDbContext.GraphWithTx;

            lock (graph)
            {
                // current transaction exists only in current HttpContext or Thread
                if (graph.TryGetValue(ts, out connections) == false)
                {
                    connections = new Connections();
                    graph.Add(Transaction.Current, connections);
                }
            }

            ReferenceConnection rConnection = null;

            lock (connections)
            {
                if (connections.TryGetValue(this.Name, out rConnection) == false)
                {
                    rConnection = new ReferenceConnection(this.Name, DbConnectionManager.GetConnection(this.Name));
                    IsConnectionCreator = true;

                    connections.Add(this.Name, rConnection);
                }
                else
                    rConnection.ReferenceCount++;
            }

            return rConnection.Connection;
        }
        /// <summary>
        /// 重新数据库事物结束时触发的事件
        /// </summary>
        /// <param name="args">事件对象</param>
        protected override void OnTransactionCompleted(TransactionEventArgs args)
        {
            GraphWithTransaction graph = GraphWithTx;

            lock (graph)
            {
                Connections connections;

                if (graph.TryGetValue(args.Transaction, out connections))
                {
                    try
                    {
                        lock (connections)
                        {
                            foreach (KeyValuePair<string, ReferenceConnection> item in connections)
                            {
                                DbConnection connection = item.Value.Connection;

                                if (connection.State != ConnectionState.Closed)
                                {
                                    connection.Close();

                                    WriteTraceInfo(connection.DataSource + "." + connection.Database
                                        + "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]",
                                    " Close Connection ");
                                }
                            }
                        }
                    }
                    finally
                    {
                        graph.Remove(args.Transaction);
                    }
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Get connection graph when executing without transaction.
        /// </summary>
        /// <returns></returns>
        private static GraphWithTransaction GraphWithTx
        {
            get
            {
                WriteTraceInfo("GetGraphWithTx ManagedThreadId :"
                    + Thread.CurrentThread.ManagedThreadId.ToString());

                GraphWithTransaction result;

                lock (typeof(GraphWithTransaction))
                {
                    if (AutoEnlistDbContext.graphWithTx == null)
                        AutoEnlistDbContext.graphWithTx = new GraphWithTransaction();

                    result = AutoEnlistDbContext.graphWithTx;
                }

                return result;
            }
        }
        #endregion
    }
}
