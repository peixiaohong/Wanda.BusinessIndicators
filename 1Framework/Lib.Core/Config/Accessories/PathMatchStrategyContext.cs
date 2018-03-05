
using System.Collections.Generic;
using Lib.Core;

namespace Lib.Config
{
    /// <summary>
    /// 路径匹配算法计算过程定义 Context
    /// </summary>
    internal class PathMatchStrategyContext : StrategyContextBase<BestPathMatchStrategyBase, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PathMatchStrategyContext() { }


        /// <summary>
        /// 根据抽象算法类型组织计算过程
        /// </summary>
        /// <returns></returns>
        public override string DoAction()
        {
			IList<KeyValuePair<string, string>> data = innerStrategy.Candidates;
			return innerStrategy.Calculate(data);
        }
    }
}
