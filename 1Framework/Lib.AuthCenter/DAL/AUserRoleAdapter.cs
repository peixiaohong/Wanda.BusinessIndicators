using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.AuthCenter.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.DAL
{
    /// <summary>
    /// AUserRole对象的数据访问适配器
    /// </summary>
    sealed class AUserRoleAdapter : AuthBaseAdapterT<AUserRole>, IUsage
    {
        public static readonly AUserRoleAdapter Instance = new AUserRoleAdapter();
        public int UsageCount(Guid ID)
         {
             int result = 0;
             return result;
         }
    }
}

