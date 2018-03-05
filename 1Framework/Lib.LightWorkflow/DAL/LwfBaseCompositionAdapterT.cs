using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Dal
{
 

    internal class LwfBaseCompositionAdapterT<T> : BaseCompositionAdapterT<T>
       where T : IBaseComposedModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaKpiConnectionString"; }
        }
    }

    internal class LwfCommonAdapter : CommonAdapter
    {
        protected override string ConnectionName
        {
            get { return "WandaKpiConnectionString"; }
        }
    }

    internal class LwfBaseAdapterT<T> : BaseAdapterT<T>
     where T : BaseModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaKpiConnectionString"; }
        }
    }

}
