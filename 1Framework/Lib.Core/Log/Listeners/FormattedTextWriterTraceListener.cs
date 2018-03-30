using System;
using System.Diagnostics;
using System.IO;

namespace Lib.Log
{
    /// <summary>
    /// 可格式化的文本编写侦听器（Listerner）
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase的派生类，包装TextWriterTraceListener类
    /// </remarks>
    public class FormattedTextWriterTraceListener : FormattedTraceListenerWrapperBase
    {
        private const string DefaultLogFileName = "log.txt";

        private string logFileName = string.Empty;
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
        /// 缺省构造函数
        /// </summary>
        public FormattedTextWriterTraceListener()
            : base(new TextWriterTraceListener())
        {
        }


        ///Add by JerryShi for create log by text writer

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <remarks>
        /// 根据配置信息创建FormattedEventLogTraceListener对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\ListenerTest.cs"
        /// lang="cs" region="EventLogTraceListener Test" tittle="创建Listener对象"></code>
        /// </remarks>
        public FormattedTextWriterTraceListener(LogListenerElement element)
        {
            this.formatterName = element.LogFormatterName;
            this.Name = element.Name;

            if (element.ExtendedAttributes.TryGetValue("logFileName", out this.logFileName) == false)
                this.logFileName = FormattedTextWriterTraceListener.DefaultLogFileName;

            this.logFileName = RootFileNameAndEnsureTargetFolderExists(this.logFileName);

            FileStream fs = new FileStream(this.logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TextWriterTraceListener textlistener = new TextWriterTraceListener(fs, this.Name);

            this.SlaveListener = textlistener;
            //if (element.ExtendedAttributes.TryGetValue("source", out this.source) == false)
            //    this.source = string.IsNullOrEmpty(this.logName) ? FormattedEventLogTraceListener.DefaultSource : this.logName;

            //this.SlaveListener = new TextWriterTraceListener();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatter">ILogFormatter实例</param>
        public FormattedTextWriterTraceListener(ILogFormatter formatter)
            : this()
        {
            this.Formatter = formatter;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">日志文件名</param>
        /// <remarks>
        /// 将日志记录写入filename指定的文件中
        /// </remarks>
        public FormattedTextWriterTraceListener(string filename)
        {
            this.logFileName = RootFileNameAndEnsureTargetFolderExists(filename);
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TextWriterTraceListener textlistener = new TextWriterTraceListener(fs, this.Name);

            this.SlaveListener = textlistener;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">日志文件名</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 将日志记录，由formatter格式化后写入filename指定的文件中
        /// </remarks>
        public FormattedTextWriterTraceListener(string filename, ILogFormatter formatter)
            : this(filename)
        {
            this.Formatter = formatter;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">Stream对象</param>
        /// <remarks>
        /// 将日志记录，写入stream流中
        /// </remarks>
        public FormattedTextWriterTraceListener(Stream stream)
        {
            TextWriterTraceListener textlistener = new TextWriterTraceListener(stream, this.Name);

            this.SlaveListener = textlistener;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">Stream对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 将日志记录，由formatter格式化后写入stream流中
        /// </remarks>
        public FormattedTextWriterTraceListener(Stream stream, ILogFormatter formatter)
            : this(stream)
        {
            this.Formatter = formatter;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">TextWriter对象</param>
        /// <remarks>
        /// 将日志记录，写入writer中
        /// </remarks>
        public FormattedTextWriterTraceListener(TextWriter writer)
            : base(new TextWriterTraceListener(writer))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">TextWriter对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 将日志记录，由formatter格式化后写入writer中
        /// </remarks>
        public FormattedTextWriterTraceListener(TextWriter writer, ILogFormatter formatter)
            : this(writer)
        {
            this.Formatter = formatter;
        }

        /// <summary>
        /// 是否线程安全，对TextWriterTraceListener为true
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 重载方法，写入数据
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="eventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="id">事件的数值标识符</param>
        /// <param name="data">要记录的日志数据</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;
                if (this.Formatter != null)
                {
                    base.Write(this.Formatter.Format(logData));
                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, logData);
                }

            }
            else
            {
                base.TraceData(eventCache, source, eventType, id, data);
            }
        }

        /// <summary>
        ///支持的属性
        /// </summary>
        /// <returns>属性数组</returns>
        protected override string[] GetSupportedAttributes()
        {
            return new string[2] { "formatter", "logFileName" };
        }

        /// <summary>
        /// 文件路径处理
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>标准的完整文件路径</returns>
        protected static string RootFileNameAndEnsureTargetFolderExists(string fileName)
        {
            //string rootedFileName = 
            //if (false == Path.IsPathRooted(rootedFileName))
            //{
            string rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(fileName, DateTime.Now)); //fileName格式如 sysLog{0:yyyyMMdd}
            // }

            string directory = Path.GetDirectoryName(rootedFileName);
            if (false == string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return rootedFileName;
        }
    }
}
