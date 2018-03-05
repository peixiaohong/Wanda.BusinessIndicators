using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Wanda.Lib.LightWorkflow.Entities
{
    public partial class LWF_Process
    {
        [NoMapping]
        public List<ProcessNode> ProcessNodeList
        {
            get;
            set;
        }
    }
}
