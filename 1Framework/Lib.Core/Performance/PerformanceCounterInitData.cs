
namespace Lib.Performance
{
    /// <summary>
    /// 初始化性能计数器时的参数
    /// </summary>
    public class PerformanceCounterInitData
    {
        private string categoryName = string.Empty;
        private string counterName = string.Empty;
        private string instanceName = string.Empty;
        private bool isReadonly = false;
        private string machineName = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        public PerformanceCounterInitData()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="categoryName">性能计数器的类别</param>
        /// <param name="counterName">性能计数器的名字</param>
        public PerformanceCounterInitData(string categoryName, string counterName)
            : this(categoryName, counterName, string.Empty)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="perCategoryName">性能计数器的类别</param>
        /// <param name="perCounterName">性能计数器的名字</param>
        /// <param name="perInstanceName">实例名称</param>
        public PerformanceCounterInitData(string perCategoryName, string perCounterName, string perInstanceName)
        {
            this.categoryName = perCategoryName;
            this.counterName = perCounterName;
            this.instanceName = perInstanceName;
        }

        /// <summary>
        /// 性能计数器的类别
        /// </summary>
        public string CategoryName
        {
            get { return this.categoryName; }
            set { this.categoryName = value; }
        }

        /// <summary>
        /// 性能计数器的名称
        /// </summary>
        public string CounterName
        {
            get { return this.counterName; }
            set { this.counterName = value; }
        }

        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceName
        {
            get { return this.instanceName; }
            set { this.instanceName = value; }
        }

        /// <summary>
        /// 是否是只读
        /// </summary>
        public bool Readonly
        {
            get { return this.isReadonly; }
            set { this.isReadonly = value; }
        }

        /// <summary>
        /// 性能计数器所在的机器名
        /// </summary>
        public string MachineName
        {
            get { return this.machineName; }
            set { this.machineName = value; }
        }
    }
}
