using System;
using System.Diagnostics;

namespace Lib.Log
{
    /// <summary>
    /// 抽象基类，Logging组件内置Listener的基类
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerBase的派生类，内部包装了TraceListener对象
    /// </remarks>
    public abstract class FormattedTraceListenerWrapperBase : FormattedTraceListenerBase
    {
        private TraceListener slaveListener;

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public FormattedTraceListenerWrapperBase()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="slaveListener">被包装的TraceListener对象</param>
        public FormattedTraceListenerWrapperBase(TraceListener slaveListener)
        {
            this.slaveListener = slaveListener;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="slaveListener">被包装的TraceListener对象</param>
        /// <param name="formater">ILogFormatter实例</param>
        public FormattedTraceListenerWrapperBase(TraceListener slaveListener, ILogFormatter formater)
            : base(formater)
        {
            this.slaveListener = slaveListener;
        }

        /// <summary>
        /// 重载方法，写入数据
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="logEventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="eventID">事件的数值标识符</param>
        /// <param name="data">要记录的日志数据</param>
        /// <remarks>
        /// 调用包装的TraceListener类的对应TraceData方法
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, params object[] data)
        {
            this.slaveListener.TraceData(eventCache, source, logEventType, eventID, data);
        }

        /// <summary>
        /// 重载方法，写入数据
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="logEventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="eventID">事件的数值标识符</param>
        /// <param name="data">要记录的日志数据</param>
        /// <remarks>
        /// 只有Formatter属性不为空时，才格式化
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, object data)
        {
            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;
                if (this.Formatter != null)
                {
                    this.slaveListener.TraceData(eventCache, source, logEventType, eventID, this.Formatter.Format(logData));
                }
                else
                {
                    this.slaveListener.TraceData(eventCache, source, logEventType, eventID, logData);
                }
            }
            else
            {
                this.slaveListener.TraceData(eventCache, source, logEventType, eventID, data);
            }
        }

        /// <summary>
        /// 重载方法，写入数据
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="logEventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="id">事件的数值标识符</param>
        /// <param name="message">要记录的日志数据</param>
        /// <remarks>
        /// 调用包装的TraceListener类的对应TraceEvent方法
        /// </remarks>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType logEventType, int id, string message)
        {
            this.slaveListener.TraceEvent(eventCache, source, logEventType, id, message);
        }

        /// <summary>
        /// 重载方法，写入数据
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="logEventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="id">事件的数值标识符</param>
        /// <param name="format">格式串</param>
        /// <param name="args">要记录的日志数据</param>
        /// <remarks>
        /// 调用包装的TraceListener类的对应TraceEvent方法
        /// </remarks>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType logEventType, int id, string format, params object[] args)
        {
            this.slaveListener.TraceEvent(eventCache, source, logEventType, id, format, args);
        }

        /// <summary>
        /// 重载方法，写入字符串
        /// </summary>
        /// <param name="message">待写入的字符串</param>
        /// <remarks>
        /// 调用包装的TraceListener类的对应Write方法
        /// </remarks>
        public override void Write(string message)
        {
            this.slaveListener.Write(message);
        }

        /// <summary>
        /// 重载方法，写入一行字符串
        /// </summary>
        /// <param name="message">待写入的字符串</param>
        /// <remarks>
        /// 调用包装的TraceListener类的对应WriteLine方法
        /// </remarks>
        public override void WriteLine(string message)
        {
            this.slaveListener.WriteLine(message);
        }

        /// <summary>
        /// 刷新Listener的缓冲区
        /// </summary>
        /// <remarks>
        /// 调用包装的TraceListener类的对应Flush方法
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void Flush()
        {
            this.slaveListener.Flush();
        }

        /// <summary>
        /// 关闭Listener
        /// </summary>
        /// <remarks>
        /// 调用包装的TraceListener类的对应Close方法
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void Close()
        {
            this.slaveListener.Close();
        }

        internal TraceListener SlaveListener
        {
            get
            {
                return this.slaveListener;
            }
            set
            {
                this.slaveListener = value;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放包装的资源</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.slaveListener.Dispose();
            }
        }
    }

}
