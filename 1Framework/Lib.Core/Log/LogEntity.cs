using System;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections.Generic;
using Lib.Core;
using System.Security;
using System.Security.Permissions;

namespace Lib.Log
{
    /// <summary>
    /// 日志记录
    /// </summary>
    /// <remarks>
    /// 日志记录类，一条记录对应一个实例
    /// </remarks>
    [Serializable]
    [XmlRoot("LogEntity")]
    public sealed class LogEntity : ICloneable
    {
        private LogPriority defaultPriority = LogPriority.Normal;
        private TraceEventType logEventType = TraceEventType.Information;
        private int defaultEventId = 0;
        private Guid activityID = Guid.Empty;
        private string defaultTitle = string.Empty;
        private string message = string.Empty;
        private string stackTrace = string.Empty;
        private string source = string.Empty;
        private DateTime timeStamp = DateTime.MinValue;
        private string machineName = string.Empty;
        private IDictionary<string, object> extendedProperties = null;

        /// <summary>
        /// 构造函数，根据消息，创建日志记录对象
        /// </summary>
        /// <param name="message">日志记录消息</param>
        /// <remarks>
        /// message参数不能为空，否则抛异常
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="根据消息创建日志记录对象"></code>
        /// </remarks>
        public LogEntity(string message)
        {
            ExceptionHelper.TrueThrow<LogException>(string.IsNullOrEmpty(message), "日志记录信息不能为空");

            this.message = message;
            BuildUpInitialProperties();
        }

        /// <summary>
        /// 构造函数，根据异常创建日志记录对象
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <remarks>
        /// ex参数不能为空，否则抛异常
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="根据异常对象创建日志记录对象"></code>
        /// </remarks>
        public LogEntity(Exception ex)
        {
            ExceptionHelper.TrueThrow<LogException>(ex == null, "传递的异常对象为空");

            BuildUpInitialProperties();

            Exception realEx = ExceptionHelper.GetRealException(ex);

            this.defaultTitle = ex.Message;
            this.message = realEx.Message;
            this.stackTrace = realEx.StackTrace;
            this.source = realEx.Source;
            this.logEventType = TraceEventType.Error;
        }

        /// <summary>
        /// 构造函数，根据参数构造LogEntity对象
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="message">日志消息</param>
        /// <param name="eventID">日志事件ID</param>
        /// <remarks>
        /// 根据所传递的参数构造LogEntity对象，其他属性为缺省值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="根据参数构造LogEntity对象"></code>
        /// </remarks>
        public LogEntity(string title, string message, int eventID)
            : this(title, message, eventID, LogPriority.Normal, TraceEventType.Information, string.Empty, string.Empty, null)
        {

        }

        /// <summary>
        /// 构造函数，根据参数构造LogEntity对象
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="message">日志消息</param>
        /// <param name="eventID">日志事件ID</param>
        /// <param name="priority">日志记录优先级</param>
        /// <param name="logEventType">日志事件类型</param>
        /// <param name="source">日志来源</param>
        /// <param name="stackTrace">调用栈</param>
        /// <param name="propterties">扩展信息</param>
        /// <remarks>
        /// 根据所传递的参数构造LogEntity对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="根据参数构造LogEntity对象"></code>
        /// </remarks>
        public LogEntity(string title, string message, int eventID, LogPriority priority, TraceEventType logEventType, string source,
            string stackTrace, IDictionary<string, Object> propterties)
        {
            ExceptionHelper.TrueThrow<LogException>(string.IsNullOrEmpty(message), "日志记录信息不能为空");

            this.message = message;
            this.defaultTitle = title;
            this.defaultEventId = eventID;
            this.logEventType = logEventType;
            this.defaultPriority = priority;
            this.source = source;
            this.stackTrace = stackTrace;
            this.extendedProperties = propterties;

            BuildUpInitialProperties();
        }

        #region 公共属性
        /// <summary>
        /// 日志事件ID
        /// </summary>
        public int EventID
        {
            get
            {
                return this.defaultEventId;
            }
            set
            {
                this.defaultEventId = value;
            }
        }

        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.defaultTitle;
            }
            set
            {
                this.defaultTitle = value;
            }
        }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        /// <summary>
        /// 调用栈
        /// </summary>
        public string StackTrace
        {
            get
            {
                //return Environment.StackTrace;
                return this.stackTrace;
            }
            set
            {
                this.stackTrace = value;
            }
        }

        /// <summary>
        /// 日志来源
        /// </summary>
        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return this.timeStamp;
            }
            set
            {
                this.timeStamp = value;
            }
        }

        /// <summary>
        /// 机器名
        /// </summary>
        public string MachineName
        {
            get
            {
                return this.machineName;
            }
            set
            {
                this.machineName = value;
            }
        }

        public string CreatorName { get; set; }
        /// <summary>
        /// 活动（操作）ID
        /// </summary>
        public Guid ActivityID
        {
            get
            {
                return this.activityID;
            }
            set
            {
                this.activityID = value;
            }
        }

        /// <summary>
        /// 日志记录优先级
        /// </summary>
        public LogPriority Priority
        {
            get
            {
                return this.defaultPriority;
            }
            set
            {
                this.defaultPriority = value;
            }
        }

        /// <summary>
        /// 日志事件类型
        /// </summary>
        public TraceEventType LogEventType
        {
            get
            {
                return this.logEventType;
            }
            set
            {
                this.logEventType = value;
            }
        }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public IDictionary<string, object> ExtendedProperties
        {
            get
            {
                if (this.extendedProperties == null)
                    this.extendedProperties = new Dictionary<string, object>();

                return this.extendedProperties;
            }
            //set
            //{
            //    extendedProperties = value;
            //}
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// 实现ICloneable接口
        /// </summary>
        /// <returns>
        /// 克隆出的LogEntity对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="LogEntityClone Test" tittle="克隆LogEntity对象"></code>
        /// </returns>
        public object Clone()
        {
            LogEntity log = new LogEntity(this.Message);

            log.EventID = this.EventID;
            log.Title = this.Title;
            log.Priority = this.Priority;
            log.LogEventType = this.LogEventType;
            log.MachineName = this.MachineName;
            log.StackTrace = this.StackTrace;
            log.ActivityID = this.ActivityID;
            log.Source = this.Source;

            if (this.extendedProperties != null)
                log.extendedProperties = new Dictionary<string, object>(this.extendedProperties);

            return log;
        }

        #endregion

        private void BuildUpInitialProperties()
        {
            this.timeStamp = DateTime.Now;

            if (IsTracingAvailable)
            {
                try
                {
                    this.ActivityID = Trace.CorrelationManager.ActivityId;
                }
                catch (Exception)
                {
                    this.ActivityID = Guid.Empty;
                }
            }

            try
            {
                this.machineName = Environment.MachineName;
            }
            catch (Exception ex)
            {
                this.machineName = String.Format(Lib.Core.Properties.Resource.Culture,
                    Lib.Core.Properties.Resource.InitialPropertyError, ex.Message);
            }
        }

        private bool IsTracingAvailable
        {
            get
            {
                bool tracingAvailable = false;

                try
                {
                    tracingAvailable = SecurityManager.IsGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
                }
                catch (SecurityException)
                {
                }

                return tracingAvailable;
            }
        }
    }
}
