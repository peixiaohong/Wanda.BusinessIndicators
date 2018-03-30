using System;
using System.IO;
using System.Diagnostics;

namespace Lib.Log
{
    /// <summary>
    /// 文本文件侦听器（Listerner）
    /// </summary>
    /// <remarks>
    /// FormattedTextWriterTraceListener派生类，增加标头和脚注
    /// </remarks>
    public sealed class FlatFileTraceListener : FormattedTextWriterTraceListener
    {
        private string header = string.Empty;
        private string footer = string.Empty;
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
        public FlatFileTraceListener()
            : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Logging\ListenerTest.cs"
        /// lang="cs" region="FlatFileTraceListener Test" tittle="创建Listener对象"></code>
        /// </remarks>
        public FlatFileTraceListener(LogListenerElement element)
        {
            this.Name = element.Name;

            string filenameFormat = string.Empty;
            if (element.ExtendedAttributes.TryGetValue("filenameFormat", out filenameFormat) == false)
                filenameFormat = element.Name + ".log";

            string filename = RootFileNameAndEnsureTargetFolderExists(filenameFormat);
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TextWriterTraceListener textlistener = new TextWriterTraceListener(fs, this.Name);

            this.SlaveListener = textlistener;

            if (element.ExtendedAttributes.TryGetValue("header", out this.header) == false)
                this.header = string.Empty;
            if (element.ExtendedAttributes.TryGetValue("footer", out this.footer) == false)
                this.footer = string.Empty;

            this.formatterName = element.LogFormatterName;

            //string filename;
            //element.ExtendedAttributes.TryGetValue("fileName", out filename);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 用Formatter初始化一个实例对象
        /// </remarks>
        public FlatFileTraceListener(ILogFormatter formatter)
            : base(formatter)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">FileStream对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(FileStream stream, ILogFormatter formatter)
            : base(stream, formatter)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">FileStream对象</param>
        public FlatFileTraceListener(FileStream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">StreamWriter对象.</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(StreamWriter writer, ILogFormatter formatter)
            : base(writer, formatter)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">StreamWriter对象</param>
        public FlatFileTraceListener(StreamWriter writer)
            : base(writer)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">TextWriter对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(TextWriter writer, ILogFormatter formatter)
            : base(writer, formatter)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">日志文件名</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(string fileName, ILogFormatter formatter)
            : base(fileName, formatter)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">日志文件名</param>
        public FlatFileTraceListener(string fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">日志文件名</param>
        /// <param name="header">日志记录标头</param>
        /// <param name="footer">日志记录脚注</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(string fileName, string header, string footer, ILogFormatter formatter)
            : base(fileName, formatter)
        {
            this.header = header;
            this.footer = footer;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">日志文件名</param>
        /// <param name="header">日志记录标头</param>
        /// <param name="footer">日志记录脚注</param>
        public FlatFileTraceListener(string fileName, string header, string footer)
            : base(fileName)
        {
            this.header = header;
            this.footer = footer;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">FileStream对象</param>
        /// <param name="name">Listener的名称</param>
        /// <param name="formatter">ILogFormatter实例.</param>
        public FlatFileTraceListener(FileStream stream, string name, ILogFormatter formatter)
            : base(stream, formatter)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">FileStream对象</param>
        /// <param name="name">Listener的名称</param>
        public FlatFileTraceListener(FileStream stream, string name)
            : base(stream)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">StreamWriter对象</param>
        /// <param name="name">Listener的名称</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(StreamWriter writer, string name, ILogFormatter formatter)
            : base(writer, formatter)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">StreamWriter对象</param>
        /// <param name="name">Listener的名称</param>
        public FlatFileTraceListener(StreamWriter writer, string name)
            : base(writer)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">日志文件名</param>
        /// <param name="name">Listener的名称</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FlatFileTraceListener(string fileName, string name, ILogFormatter formatter)
            : base(fileName, formatter)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">日志文件名</param>
        /// <param name="name">Listener的名称</param>
        public FlatFileTraceListener(string fileName, string name)
            : base(fileName)
        {
            this.Name = name;
        }

        /// <summary>
        /// 重载方法，写入文件
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
            if (this.header.Length > 0)
                WriteLine(this.header);

            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;

                if (this.Formatter != null)
                {
                    base.WriteLine(this.Formatter.Format(logData));
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

            if (this.footer.Length > 0)
                WriteLine(this.footer);
        }

        /// <summary>
        /// FlatFileTraceListener支持的Attibutes
        /// </summary>
        protected override string[] GetSupportedAttributes()
        {
            return new string[4] { "formatter", "fileName", "header", "footer" };
        }
    }
}
