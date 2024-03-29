﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LJTH.Lib.Data;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.DAL
{
    /*
     设置数据库连接字符串
     */
    class AuthBaseCompositionAdapterT<T> : BaseCompositionAdapterT<T>
         where T : IBaseComposedModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }

    }

    class AuthCommonAdapter : CommonAdapter
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }
    }

    class AuthBaseAdapterT<T> : BaseAdapterT<T>
     where T : BaseModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }
    }
}
