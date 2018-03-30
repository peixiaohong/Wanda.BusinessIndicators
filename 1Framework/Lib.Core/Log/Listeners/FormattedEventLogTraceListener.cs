using System;
using System.Diagnostics;

namespace Lib.Log
{
    /// <summary>
    /// EventLog侦听器（Listerner）
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase派生类，将日志记入EventLog中
    /// </remarks>
    public sealed class FormattedEventLogTraceListener : FormattedTraceListenerWrapperBase
    {
        private const string DefaultLogName = "";
        private const string DefaultMachineName = ".";
        private const string DefaultSource = "";

        //私有字段
        private string logName = DefaultLogName;
        private string source = DefaultSource;

        private string formatterName = string.Empty;

        /// <summary>
        /// 文本格式化器
        /// </summary>
        /// <remarks>
        /// 该Listener下所配的格式化器
        /// </remarks>
        public override ILogFormatter Formatter
        {
            get
            {
                ILogFormatter formatter = base.Formatter;
                if (false == string.IsNullOrEmpty(this.formatterName))// != string.Empty)
                {
                    LogConfigurationElement formatterelement = LoggingSection.GetConfig().LogFormatterElements[this.formatterName];

                    formatter = LogFormatterFactory.GetFormatter(formatterelement);
                }

                return formatter;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <remarks>
        /// 根据配置信息创建FormattedEventLogTraceListener对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\ListenerTest.cs"
        /// lang="cs" region="EventLogTraceListener Test" tittle="创建Listener对象"></code>
        /// </remarks>
        public FormattedEventLogTraceListener(LogListenerElement element)
        {
            this.formatterName = element.LogFormatterName;
            this.Name = element.Name;

            if (element.ExtendedAttributes.TryGetValue("logName", out this.logName) == false)
                this.logName = FormattedEventLogTraceListener.DefaultLogName;

            if (element.ExtendedAttributes.TryGetValue("source", out this.source) == false)
                this.source = string.IsNullOrEmpty(this.logName) ? FormattedEventLogTraceListener.DefaultSource : this.logName;

            this.SlaveListener = new EventLogTraceListener();
        }

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        /// <remarks>
        /// 用EventLogTraceListener初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener()
            : base(new EventLogTraceListener())
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formater">ILogFormatter实例</param>
        /// <remarks>
        /// 用EventLogTraceListener和Formatter初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener(ILogFormatter formater)
            : base(new EventLogTraceListener(), formater)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventLog">EventLog对象</param>
        /// <remarks>
        /// 用EventLogTraceListener(EventLog)初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog)
            : base(new EventLogTraceListener(eventLog))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventLog">EventLog对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 用EventLogTraceListener(EventLog)和Formatter初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog, ILogFormatter formatter)
            : base(new EventLogTraceListener(eventLog), formatter)
        {
        }

        /// <summary>
        /// 构造函数，用EventLogTraceListener(EventLog)和Formatter初始化一个实例对象
        /// </summary>
        /// <param name="source">EventLog中的事件来源</param>
        /// <param name="logName">EventLog中的日志名称</param>
        /// <param name="machineName">记录事件日志的机器名称</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FormattedEventLogTraceListener(string source, string logName, string machineName, ILogFormatter formatter)
            : base(new EventLogTraceListener(new EventLog(logName, NormalizeMachineName(machineName), source)), formatter)
        {
        }

        /// <summary>
        /// 是否线程安全，对EventLogTraceListener为true
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 重载方法，写入文件
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="logEventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="eventID">事件的数值标识符</param>
        /// <param name="data">要记录的日志数据</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, object data)
        {
            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;

                //取LogEntity对象的Source属性，如果为空则取来源于配置的缺省值
                string sourceName = string.IsNullOrEmpty(logData.Source) ? this.source : logData.Source;

                string registerLogName = RegisterSourceToLogName(sourceName, this.logName);

                EventLog eventlog = new EventLog(registerLogName, FormattedEventLogTraceListener.DefaultMachineName, sourceName);

                //if (eventlog.OverflowAction == OverflowAction.OverwriteOlder && eventlog.MinimumRetentionDays > 3)
                //    eventlog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);

                //if (eventlog.MaximumKilobytes < 1024)
                //    eventlog.MaximumKilobytes = 2048;

                (this.SlaveListener as EventLogTraceListener).EventLog = eventlog;
            }

            base.TraceData(eventCache, source, logEventType, eventID, data);
        }

        #region 备用的扩充构造函数
        ///// <summary>
        ///// 构造函数，用EventLogTraceListener(string)初始化一个实例对象
        ///// <see cref="EventLogTraceListener"/> initialized with a source name.
        ///// </summary>
        ///// <param name="source">The source name for the wrapped listener.</param>
        //public FormattedEventLogTraceListener(string source)
        //    : base(new EventLogTraceListener(source))
        //{
        //}

        ///// <summary>
        ///// 构造函数，用EventLogTraceListener(string)和Formatter初始化一个实例对象
        ///// </summary>
        ///// <param name="source">The source name for the wrapped listener.</param>
        ///// <param name="formatter">The formatter for the wrapper.</param>
        //public FormattedEventLogTraceListener(string source, ILogFormatter formatter)
        //    : base(new EventLogTraceListener(source), formatter)
        //{
        //}

        ///// <summary>
        ///// 构造函数，用EventLogTraceListener(EventLog)和Formatter初始化一个实例对象
        ///// </summary>
        ///// <param name="source">The source name for the wrapped listener.</param>
        ///// <param name="logName">The name of the event log.</param>
        ///// <param name="formatter">The formatter for the wrapper.</param>
        //public FormattedEventLogTraceListener(string source, string logName, ILogFormatter formatter)
        //    : base(new EventLogTraceListener(new EventLog(logName, DefaultMachineName, source)), formatter)
        //{
        //}
        #endregion

        private static string NormalizeMachineName(string machineName)
        {
            return string.IsNullOrEmpty(machineName) ? FormattedEventLogTraceListener.DefaultMachineName : machineName;
        }

        /// <summary>
        /// 注册日志来源和日志名称的映射关系
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="logName">日志名称</param>
        private string RegisterSourceToLogName(string source, string logName)
        {
            string registeredLogName = logName;

            EventSourceCreationData creationData = new EventSourceCreationData(source, logName);

            if (EventLog.SourceExists(source))
            {
                string originalLogName = EventLog.LogNameFromSourceName(source, FormattedEventLogTraceListener.DefaultMachineName);

                //source注册的日志名称和指定的logName不一致，且不等于source自身
                //（事件日志源“System”等于日志名称，不能删除。[System.InvalidOperationException]）
                if (string.Compare(logName, originalLogName, true) != 0 && string.Compare(source, originalLogName, true) != 0)
                {
                    //删除现有的关联重新注册
                    EventLog.DeleteEventSource(source, FormattedEventLogTraceListener.DefaultMachineName);
                    EventLog.CreateEventSource(creationData);
                }
                else
                    registeredLogName = originalLogName;
            }
            else
                //source未在该服务器上注册过
                EventLog.CreateEventSource(creationData);

            return registeredLogName;
        }

    }
}
