using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanda.Lib.Data.AppBase
{
    /// <summary>
    /// 带层叠关系的对象， 类似树形结构的节点（Node）
    /// </summary>
    public interface ICascadeObject
    {
       // List<ICascadeObject> GetChildren();

        void RemoveChildren();
    }
}
