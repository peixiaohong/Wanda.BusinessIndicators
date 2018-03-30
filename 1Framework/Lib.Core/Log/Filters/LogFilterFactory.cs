using System;
using Lib.Core;

namespace Lib.Log
{
    internal class LogFilterFactory
    {
        //private static ReaderWriterLock rwlock = new ReaderWriterLock();
        //private const int _defaultLockTimeout = 3000;

        #region Get FilterPipeline
        public static LogFilterPipeline GetFilterPipeLine()
        {
            return new LogFilterPipeline();
        }

        public static LogFilterPipeline GetFilterPipeLine(LogConfigurationElementCollection filterElements)
        {
            //rwlock.AcquireReaderLock(defaultLockTimeout);
            try
            {
                LogFilterPipeline pipeline = new LogFilterPipeline();

                if (filterElements != null)
                {
                    foreach (LogConfigurationElement ele in filterElements)
                    {
                        ILogFilter filter = GetFilterFromConfig(ele);

                        if (filter != null)
                            pipeline.Add(filter);
                    }
                }

                return pipeline;
            }
            catch (Exception ex)
            {
                throw new LogException("创建FilterPipeline时发生错误：" + ex.Message, ex);
            }
            //finally
            //{
            //    rwlock.ReleaseReaderLock(); 
            //}
        }
        #endregion

        private static ILogFilter GetFilterFromConfig(LogConfigurationElement element)
        {
            return (ILogFilter)TypeCreator.CreateInstance(element.Type, element);
            //return ObjectBuilder.GetInstance<LogFilter>(element.TypeName);
        }
    }
}
