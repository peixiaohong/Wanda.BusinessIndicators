using Lib.Log;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Wanda.BusinessIndicators.Common
{

    public class AutoLogCallHandler : ICallHandler
    {

        public AutoLogCallHandler() { }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            StringBuilder sb = null;
            ParameterInfo pi = null;

            string methodName = input.MethodBase.Name;
            WriteLog(string.Format("Enter method {0}(); Order:{1} ", methodName, Order));


            if (input.Arguments != null && input.Arguments.Count > 0)
            {
                sb = new StringBuilder();
                for (int i = 0; i < input.Arguments.Count; i++)
                {
                    pi = input.Arguments.GetParameterInfo(i);
                    sb.Append(pi.Name).Append(" : ").Append(input.Arguments[i]).AppendLine();
                }
                WriteLog(sb.ToString());
            }


            Stopwatch sw = new Stopwatch();
            sw.Start();

            IMethodReturn result = getNext()(input, getNext);
            //如果发生异常则，result.Exception != null
            if (result.Exception != null)
            {
                WriteException(result.Exception);
                //必须将异常处理掉，否则无法继续执行
                //result.Exception = null;
            }

            sw.Stop();

            WriteLog(string.Format("Exit method {0}(). Elapsed time: {1}.", methodName, sw.Elapsed));

            return result;
        }

        private void WriteLog(string message)
        {
            LogEntity log = new LogEntity(message);

            log.Priority = LogPriority.BelowNormal;
            log.Source = LoggerName;
            log.Title = "Method Log";
            log.LogEventType = TraceEventType.Information;

            logger.Write(log);
        }

        private void WriteException(System.Exception ex)
        {
            LogEntity log = new LogEntity(ex);
            log.Priority = LogPriority.AboveNormal;
            log.Source = LoggerName;
            log.Title = "Method Exception";
            log.LogEventType = TraceEventType.Error;
            logger.Write(log);
        }

        public int Order { get; set; }

        private static string LoggerName = "AutoLog";

        private Logger logger = LogHelper.AutoLogger;
    }


    public class AutoLogCallHandlerAttribute : HandlerAttribute
    {
        /*
         必须在配置文件中增加Autolog的Log Listener， 如
  <LoggingSettings  >
    <Loggers>
      <add name="AutoLog" enable="true">
        <Listeners>
          <add name="sqlListener" type="Wanda.HR.Common.Log.SqlLogTraceListener, Wanda.HR.Common" connectionString="WandaBudgetConnectionString"></add>
        </Listeners>
      </add>
    </Loggers>
  </LoggingSettings>
         */
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new AutoLogCallHandler() { Order = this.Order  };
        }
    }


}
