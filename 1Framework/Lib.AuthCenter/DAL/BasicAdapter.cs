using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wanda.Lib.Data;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.DAL
{
    /*
     设置数据库连接字符串
     */
    class AuthBaseCompositionAdapterT<T> : BaseCompositionAdapterT<T>
         where T : IBaseComposedModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaTargetConnectionString"; }
        }

    }

    class AuthCommonAdapter : CommonAdapter
    {
        protected override string ConnectionName
        {
            get { return "WandaTargetConnectionString"; }
        }
    }

    class AuthBaseAdapterT<T> : BaseAdapterT<T>
     where T : BaseModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaTargetConnectionString"; }
        }
    }
}
