using System;
using System.Collections.Generic;
using System.Threading;
using Lib.Core;

namespace Lib.Log
{
    internal class TraceListenerFactory
    {
        private static ReaderWriterLock rwLock = new ReaderWriterLock();
        private const int DefaultLockTimeout = 3000;

        #region Get Listeners
        public static List<FormattedTraceListenerBase> GetListeners()
        {
            List<FormattedTraceListenerBase> listeners = new List<FormattedTraceListenerBase>();

            return listeners;
        }

        public static List<FormattedTraceListenerBase> GetListeners(LogListenerElementCollection listenerElements)
        {
            TraceListenerFactory.rwLock.AcquireWriterLock(TraceListenerFactory.DefaultLockTimeout);
            try
            {
                List<FormattedTraceListenerBase> listeners = new List<FormattedTraceListenerBase>();

                if (listenerElements != null)
                {
                    foreach (LogListenerElement listenerelement in listenerElements)
                    {
                        FormattedTraceListenerBase listener = GetListenerFromConfig(listenerelement);

                        if (listener != null)
                            listeners.Add(listener);
                    }
                }

                return listeners;
            }
            catch (Exception ex)
            {
                throw new LogException("创建Listeners时发生错误：" + ex.Message, ex);
            }
            finally
            {
                TraceListenerFactory.rwLock.ReleaseWriterLock();
            }
        }
        #endregion

        private static FormattedTraceListenerBase GetListenerFromConfig(LogListenerElement element)
        {
            return (FormattedTraceListenerBase)TypeCreator.CreateInstance(element.Type, element);
            //return ObjectBuilder.GetInstance<TraceListener>(element.TypeName);
        }
    }
}
