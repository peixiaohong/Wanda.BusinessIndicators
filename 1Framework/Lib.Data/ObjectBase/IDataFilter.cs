using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{
    /// <summary>
    /// 数据过滤查询
    /// </summary>
    public interface IDataFilter
    {
        // 提供除Attribute外的另一种与查询数据库字段匹配的机制。此机制执行时有限。如果在此字典中找不到对应的属性key， 则查找属性的Attribute 
        // key 为字段的名称， value为数据库字段与操作符
        // 可默认为空
        Dictionary<string, FieldAndOperator> FilterDict { get; }

        /// <summary>
        /// 排序字段
        /// </summary>
        string SortKey { get; set; }
    }


}
