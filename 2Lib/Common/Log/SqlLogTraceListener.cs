using Lib.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;


namespace LJTH.BusinessIndicators.Common
{
    public class SqlLogTraceListener : FormattedTraceListenerBase
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if ((this.Filter == null) || this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {

                if (data is LogEntity)
                {
                    LogEntity logData = data as LogEntity;

                    LogSqlAdapter.AddLog(logData, "anonymous", connectionString); // anonymous 没用， 现在使用的是Entity的CreatorName

                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
            }
        }

        private string connectionString = "";


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Logging\ListenerTest.cs"
        /// lang="cs" region="FlatFileTraceListener Test" tittle="创建Listener对象"></code>
        /// </remarks>
        public SqlLogTraceListener(LogListenerElement element)
        {
            this.Name = element.Name;

            if (element.ExtendedAttributes.TryGetValue("connectionString", out this.connectionString) == false)
            {
                this.connectionString = string.Empty;
            }

        }



        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            throw new NotImplementedException();
        }
    }
}
