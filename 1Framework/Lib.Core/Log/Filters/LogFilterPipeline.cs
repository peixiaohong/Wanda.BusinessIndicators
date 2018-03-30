using System;
using System.Collections.Generic;
using Lib.Core;

namespace Lib.Log
{
    /// <summary>
    /// 实现ILogFilter的管道（Pipeline）
    /// </summary>
    /// <remarks>
    /// 内置LogFilter集合对象
    /// </remarks>
    internal class LogFilterPipeline : FilterPipelineBase<ILogFilter, LogEntity>
    {
        internal LogFilterPipeline(List<ILogFilter> filters)
        {
            this.pipeline = filters;
        }

        internal LogFilterPipeline()
        {
            this.pipeline = new List<ILogFilter>();
        }

        /// <summary>
        /// 向Pipeline中添加ILogFilter对象
        /// </summary>
        /// <param name="filter">ILogFilter对象</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="增删LogFillter对象"></code>
        /// </remarks>
        public override void Add(ILogFilter filter)
        {
            lock (pipeline.GetType())
            {
                pipeline.Add(filter);
            }
        }

        /// <summary>
        /// 从Pipeline中移除ILogFilter实例
        /// </summary>
        /// <param name="filter">ILogFilter实例</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="增删LogFillter对象"></code>
        /// </remarks>
        public override void Remove(ILogFilter filter)
        {
            lock (pipeline.GetType())
            {
                pipeline.Remove(filter);
            }
        }

        #region 集合类的属性
        /// <summary>
        /// 根据索引返回单个的ILogFilter实例
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>单个的ILogFilter实例</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="增删LogFillter对象"></code>
        /// </remarks>
        public ILogFilter this[int index]
        {
            get
            {
                return pipeline[index];
            }
        }

        /// <summary>
        /// Pipeline中ILogFilter实例的个数
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="增删LogFillter对象"></code>
        /// </remarks>
        public int Length
        {
            get
            {
                return pipeline.Count;
            }
        }
        #endregion

        /// <summary>
        /// 实现日志过滤，判断是否通过LogFilterPipeline
        /// </summary>
        /// <param name="log">日志对象</param>
        /// <returns>布尔值，true：通过，false：不通过</returns>
        /// <remarks>
        /// Pipeline的Filter之间是“与”的关系，只有所有的Filter都通过，才算通过
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" tittle="写日志记录"></code>
        /// </remarks>
        public override bool IsMatch(LogEntity log)
        {
            bool passFilters = true;

            if (this.pipeline != null)
            {
                foreach (ILogFilter filter in pipeline)
                {
                    try
                    {
                        bool passed = filter.IsMatch(log);
                        passFilters &= passed;

                        if (false == passFilters)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw new LogException(string.Format("LogFilter:{0}过滤日志时失败，请检查其配置或实现是否正确", filter.Name), ex);
                    }
                }
            }
            return passFilters;
        }
    }
}
