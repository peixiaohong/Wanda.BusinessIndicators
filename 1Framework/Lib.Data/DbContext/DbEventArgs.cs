using System;

namespace Lib.Data
{
    /// <summary>
    /// 数据访问过程的委托方法定义。
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="e">事件参数</param>
    public delegate void DbEventHandler(object sender, DbEventArgs e);


    /// <summary>
    /// 数据访问过程的事件参数
    /// </summary>
    public class DbEventArgs : EventArgs
    {
        private object executor;

        /// <summary>
        /// 根据访问方法的不同，可能是Command或者DataAdapter对象，由具体的Database对象负责绑定。
        /// </summary>
        public virtual object Executor
        {
            get
            {
                return this.executor;
            }
            set
            {
                this.executor = value;
            }
        }
    }
}
