using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Transactions;
using System.Data;

namespace Lib.Data
{
    internal class NotEnlistDbContext : DeluxeDbContextBase
    {
        #region private classes
        protected class DbTransactions : Dictionary<ReferenceConnection, DbTransaction>
        {
        }

        private class GraphWithTransaction : Dictionary<Transaction, DbTransactions>
        {
        }
        #endregion

        /// <summary>
        /// Current context entity management target (with transaction support).
        /// <remarks>
        ///     the Key type is a System.Transaction.Transaction
        /// </remarks>
        /// </summary>
        private static GraphWithTransaction graphWithTx = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">连接名称</param>
        /// <param name="autoClose">是否自动关闭</param>
        public NotEnlistDbContext(string name, bool autoClose)
            : base(name, autoClose)
        {
        }

        #region Protected methods
        protected override DbConnection OnGetConnectionWithTransaction(Transaction ts)
        {
            ReferenceConnection refConnection = GetRefConnectionWithoutTx(this.Name);

            DbConnection connection = refConnection.Connection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DbTransactions localTxs = null;

            lock (GraphWithTx)
            {
                if (GraphWithTx.TryGetValue(ts, out localTxs) == false)
                {
                    localTxs = new DbTransactions();
                    GraphWithTx.Add(ts, localTxs);
                }
            }

            DbTransaction localTx = null;

            lock (localTxs)
            {
                if (localTxs.TryGetValue(refConnection, out localTx) == false)
                {
                    localTx = connection.BeginTransaction();

                    localTxs.Add(refConnection, localTx);
                }
            }

            this.LocalTransaction = localTx;

            return connection;
        }

        protected override void OnTransactionCompleted(TransactionEventArgs args)
        {
            GraphWithTransaction graph = GraphWithTx;

            lock (graph)
            {
                DbTransactions transactions;

                if (graph.TryGetValue(args.Transaction, out transactions))
                {
                    try
                    {
                        lock (transactions)
                        {
                            foreach (KeyValuePair<ReferenceConnection, DbTransaction> item in transactions)
                            {
                                if (args.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                                    item.Value.Commit();
                                else
                                    item.Value.Rollback();

                                ReferenceConnection refConnection = item.Key;

                                if (refConnection.ReferenceCount == 0)
                                {
                                    try
                                    {
                                        refConnection.Connection.Close();
                                    }
                                    finally
                                    {
                                        GraphWithoutTx.Remove(refConnection.Name);
                                    }
                                }

                                WriteTraceInfo(refConnection.Connection.DataSource + "." + refConnection.Connection.Database
                                    + "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]",
                                " Close Connection ");
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

        /*
        protected override GraphWithoutTransaction GraphWithoutTx
        {
            get
            {
                return StaticGraphWithoutTx;
            }
        }

        /// <summary>
        /// Get connection graph according current transaction.
        /// </summary>
        /// <returns></returns>
        private static GraphWithoutTransaction StaticGraphWithoutTx
        {
            get
            {
                GraphWithoutTransaction result;

                lock (typeof(GraphWithoutTransaction))
                {
                    if (NotEnlistDbContext.graphWithoutTx == null)
                    {
                        result = new GraphWithoutTransaction();

                        NotEnlistDbContext.graphWithoutTx = result;
                    }
                    else
                        result = NotEnlistDbContext.graphWithoutTx;
                }

                return result;
            }
        }
         */
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
                    + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());

                GraphWithTransaction result;

                lock (typeof(GraphWithTransaction))
                {
                    if (NotEnlistDbContext.graphWithTx == null)
                        NotEnlistDbContext.graphWithTx = new GraphWithTransaction();

                    result = NotEnlistDbContext.graphWithTx;
                }

                return result;
            }
        }
        #endregion
    }
}
