using Lib.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using System.Linq;
namespace Wanda.Lib.LightWorkflow.Dal
{
    internal class VPinstaneAndStakeHolderAdapter : LwfBaseCompositionAdapterT<VPInstanceAndStakeHolder>
    {
        //add czq 2013-06-17
        public static VPinstaneAndStakeHolderAdapter Instance = new VPinstaneAndStakeHolderAdapter();

        internal PartlyCollection<VPInstanceAndStakeHolder> LoadList(Filter.WorkFlowFilter filter)
        {
            return base.GetList(filter, filter.SortKey);
        }
    }
}
