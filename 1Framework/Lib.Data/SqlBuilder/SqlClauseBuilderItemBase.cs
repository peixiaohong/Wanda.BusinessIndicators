using System;

namespace Lib.Data
{
    /// <summary>
    /// 所有Sql语句构造项的基类
    /// </summary>
    public abstract class SqlClauseBuilderItemBase
    {
        /// <summary>
        /// 得到Data的Sql字符串描述
        /// </summary>
        /// <param name="builder">构造器</param>
        /// <returns>返回将data翻译成sql语句的结果</returns>
        public abstract string GetDataDesp(ISqlBuilder builder);
    }
}
