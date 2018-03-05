using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lib.Core
{
    /// <summary>
    /// 强类型的枚举项描述信息集合
    /// </summary>
    /// <remarks>
    /// 强类型的枚举项描述信息集合,该信息是只读的
    /// </remarks>
    public sealed class EnumItemDescriptionList : ReadOnlyCollection<EnumItemDescription>
    {
        /// <summary>
        /// 带枚举项描述信息类参数的构造函数
        /// </summary>
        /// <param name="list">枚举项描述信息类的实例</param>
        /// <remarks>带枚举项描述信息类参数的构造函数
        /// </remarks>
        public EnumItemDescriptionList(IList<EnumItemDescription> list)
            : base(list)
        {
        }

        /// <summary>
        /// 获得指定位置元素的枚举项描述信息
        /// </summary>
        /// <param name="i">第i个元素</param>
        /// <returns>枚举项描述信息类的实例</returns>
        /// <remarks>该属性是只读的
        /// </remarks>
        public new EnumItemDescription this[int i]
        {
            get { return base[i]; }
        }
    }
}
