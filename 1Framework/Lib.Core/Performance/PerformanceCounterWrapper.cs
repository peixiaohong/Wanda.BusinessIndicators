using System;
using System.Diagnostics;
using Lib.Core;

namespace Lib.Performance
{
    /// <summary>
    /// 性能计数器的包装类，如果初始化性能计数器失败，可以忽略掉所有操作
    /// </summary>
    public class PerformanceCounterWrapper
    {
        private PerformanceCounter counter = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="data">性能计数器的初始化参数</param>
        public PerformanceCounterWrapper(PerformanceCounterInitData data)
        {
            InitPerformanceCounter(data);
        }

        /// <summary>
        /// 性能计数器加一
        /// </summary>
        /// <returns>加一之后的计数器值</returns>
        public long Increment()
        {
            long result = 0;

            if (this.counter != null)
                result = this.counter.Increment();

            return result;
        }

        /// <summary>
        /// 性能计数器增加一个整数
        /// </summary>
        /// <param name="value">需要增加的整数</param>
        /// <returns>增加之后的计数器值</returns>
        public long IncrementBy(long value)
        {
            long result = 0;

            if (this.counter != null)
                result = this.counter.IncrementBy(value);

            return result;
        }

        /// <summary>
        /// 性能计数器值减一
        /// </summary>
        /// <returns>减一之后的计数器值</returns>
        public long Decrement()
        {
            long result = 0;

            if (this.counter != null)
                this.counter.Decrement();

            return result;
        }

        /// <summary>
        /// 获取性能计数器下一个数值
        /// </summary>
        /// <returns></returns>
        public float NextValue()
        {
            float result = 0;

            if (this.counter != null)
                result = this.counter.NextValue();

            return result;
        }

        /// <summary>
        /// 获取性能计数器下一个采样
        /// </summary>
        /// <returns></returns>
        public CounterSample NextSample()
        {
            CounterSample sample = new CounterSample();

            if (this.counter != null)
                sample = this.counter.NextSample();

            return sample;
        }

        /// <summary>
        /// 读取和设置性能计数器的原始值
        /// </summary>
        public long RawValue
        {
            get
            {
                long result = 0;

                if (this.counter != null)
                    result = this.counter.RawValue;

                return result;
            }
            set
            {
                if (this.counter != null)
                    this.counter.RawValue = value;
            }
        }

        private void InitPerformanceCounter(PerformanceCounterInitData data)
        {
            PerformanceCounter pc;

            ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");
            ExceptionHelper.CheckStringIsNullOrEmpty(data.CategoryName, "CategoryName");
            ExceptionHelper.CheckStringIsNullOrEmpty(data.CounterName, "CounterName");

            try
            {
                if (string.IsNullOrEmpty(data.MachineName))
                    pc = new PerformanceCounter(data.CategoryName, data.CounterName, data.InstanceName, data.Readonly);
                else
                    pc = new PerformanceCounter(data.CategoryName, data.CounterName, data.InstanceName, data.MachineName);

                this.counter = pc;
            }
            catch (System.Exception)
            {
            }
        }
    }
}
