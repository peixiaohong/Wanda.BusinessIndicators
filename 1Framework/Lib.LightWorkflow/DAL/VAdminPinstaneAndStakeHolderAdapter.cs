using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.LightWorkflow.Entities;

namespace Wanda.Lib.LightWorkflow.Dal
{
    internal class VAdminPinstaneAndStakeHolderAdapter : LwfBaseCompositionAdapterT<VAdminPInstanceAndStakeHolder>
    {
        //add czq 2013-10-20
        public static VAdminPinstaneAndStakeHolderAdapter Instance = new VAdminPinstaneAndStakeHolderAdapter();

        internal PartlyCollection<VAdminPInstanceAndStakeHolder> LoadList(Filter.WorkFlowFilter filter)
        {
            return base.GetList(filter, "LastUpdatedTime DESC");
        }
    }
}
