using Lib.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.DAL
{
    class AppBaseCompositionAdapterT<T> : BaseCompositionAdapterT<T>
         where T : IBaseComposedModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }
    }

    class AppCommonAdapter : CommonAdapter
    {
        protected override string ConnectionName
        {
            get { return ""; }
        }
    }

    class AppBaseAdapterT<T> : BaseAdapterT<T>
     where T : BaseModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }
    }

     
}
