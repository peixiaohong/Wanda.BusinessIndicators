using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Filter;

namespace Wanda.Lib.LightWorkflow.Dal
{
    internal class VOAMQListAdapter : LwfBaseCompositionAdapterT<VOAMQMeaages>
    {
        //add czq 2013-06-17
        public static VOAMQListAdapter Instance = new VOAMQListAdapter();

        internal PartlyCollection<VOAMQMeaages> LoadList(OAMQFilter filter)
        {
            return base.GetList(filter, "Sendertime DESC");
        }
    }
}
