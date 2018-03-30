using System;
using System.Text;
using System.Collections;
using Lib.Core.Properties;

namespace Lib.Log
{
    /// <summary>
    /// 文本格式化器
    /// </summary>
    /// <remarks>
    /// LogFormatter的派生类，具体实现文本格式化
    /// </remarks>
    public sealed class TextLogFormatter : LogFormatter
    {
        /// <summary>
        /// 格式模板
        /// </summary>
        private string template;

        /// <summary>
        /// Array of token formatters.
        /// </summary>
        private ArrayList tokenFunctions;

        private const string TimeStampToken = "{timestamp}";
        private const string MessageToken = "{message}";
        private const string PriorityToken = "{priority}";
        private const string EventIdToken = "{eventid}";
        private const string EventTypeToken = "{eventtype}";
        private const string TitleToken = "{title}";
        private const string MachineToken = "{machine}";
        private const string ActivityidToken = "{activity}";
        private const string NewLineToken = "{newline}";
        private const string TabToken = "{tab}";
        private const string StackTraceToken = "{stacktrace}";

        /// <summary>
        ///  缺省构造函数
        /// </summary>
        private TextLogFormatter()
        {
            RegisterTokenFunctions();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">配置节对象</param>
        /// <remarks>
        /// 根据配置信息，创建TextLogFormatter对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
        /// lang="cs" region="Get Formatter Test" tittle="获取Formatter对象"></code>
        /// </remarks>
        public TextLogFormatter(LogConfigurationElement element)
            : base(element.Name)
        {
            if (element.ExtendedAttributes.TryGetValue("template", out this.template) == false)
                this.template = Resource.DefaultTextFormat;

            RegisterTokenFunctions();
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="name">Formatter名称</param>
        /// <param name="template">格式模板</param>
        /// <remarks>
        /// 如果template参数为空，则使用缺省格式模板
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
        /// lang="cs" region="Get Formatter Test" tittle="获取Formatter对象"></code>
        /// </remarks>
        public TextLogFormatter(string name, string template)
            : base(name)
        {
            if (false == string.IsNullOrEmpty(template))
            {
                this.template = template;
            }
            else
            {
                this.template = Resource.DefaultTextFormat;
            }

            RegisterTokenFunctions();
        }

        /// <summary>
        /// 构造函数，使用缺省文本格式
        /// </summary>
        /// <param name="name">Formatter名称</param>
        /// <remarks>
        /// 根据名称和缺省的格式模板创建TextLogFormatter对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
        /// lang="cs" region="Get Formatter Test" tittle="获取Formatter对象"></code>
        /// </remarks>
        public TextLogFormatter(string name)
            : this(name, Resource.DefaultTextFormat)
        {
        }

        /// <summary>
        /// 格式模板
        /// </summary>
        /// <remarks>
        /// 要格式化成的模板字符串
        /// </remarks>
        public string Template
        {
            get
            {
                return this.template;
            }
            set
            {
                this.template = value;
            }
        }

        #region Format Implementation
        /// <summary>
        /// 文本格式化方法
        /// </summary>
        /// <param name="log">待格式化的LogEntity对象</param>
        /// <returns>格式化成的文本串</returns>
        /// <remarks>
        /// 重载方法，实现文本格式化
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\FormatterTest.cs"
        /// lang="cs" region="Format Test" tittle="获取Formatter对象"></code>
        /// </remarks>
        public override string Format(LogEntity log)
        {
            return Format(CreateTemplateBuilder(), log);
        }

        /// <summary>
        /// 文本格式化
        /// </summary>
        /// <param name="templateBuilder">包含格式化模板串的StringBuilder</param>
        /// <param name="log">待格式化的LogEntity对象</param>
        /// <returns>格式化成的文本串</returns>
        private string Format(StringBuilder templateBuilder, LogEntity log)
        {
            templateBuilder.Replace(TextLogFormatter.TimeStampToken, log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"));
            templateBuilder.Replace(TextLogFormatter.TitleToken, log.Title);
            templateBuilder.Replace(TextLogFormatter.MessageToken, log.Message);
            templateBuilder.Replace(TextLogFormatter.EventIdToken, log.EventID.ToString(Resource.Culture));
            templateBuilder.Replace(TextLogFormatter.EventTypeToken, log.LogEventType.ToString());
            templateBuilder.Replace(TextLogFormatter.StackTraceToken, log.StackTrace);
            templateBuilder.Replace(TextLogFormatter.PriorityToken, log.Priority.ToString());
            templateBuilder.Replace(TextLogFormatter.MachineToken, log.MachineName);

            templateBuilder.Replace(TextLogFormatter.ActivityidToken, log.ActivityID.ToString("D", Resource.Culture));

            FormatTokenFunctions(templateBuilder, log);

            templateBuilder.Replace(TextLogFormatter.NewLineToken, Environment.NewLine);
            templateBuilder.Replace(TextLogFormatter.TabToken, "\t");

            return templateBuilder.ToString();
        }

        /// <summary>
        /// 创建模板构建器
        /// </summary>
        /// <returns>包含格式化模板串的StringBuilder</returns>
        private StringBuilder CreateTemplateBuilder()
        {
            StringBuilder templateBuilder =
                            new StringBuilder((this.template == null) || (this.template.Length > 0) ? this.template : Resource.DefaultTextFormat);
            return templateBuilder;
        }
        #endregion

        private void FormatTokenFunctions(StringBuilder templateBuilder, LogEntity log)
        {
            foreach (TokenFunction token in this.tokenFunctions)
            {
                token.Format(templateBuilder, log);
            }
        }

        private void RegisterTokenFunctions()
        {
            this.tokenFunctions = new ArrayList();
            this.tokenFunctions.Add(new DictionaryToken());
            this.tokenFunctions.Add(new KeyValueToken());
            this.tokenFunctions.Add(new TimeStampToken());
            this.tokenFunctions.Add(new ReflectedPropertyToken());
        }
    }
}
