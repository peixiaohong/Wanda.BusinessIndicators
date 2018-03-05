using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Wanda.Lib.LightWorkflow.Entities
{
    public partial class ProcessNode
    {
       [NoMapping]
        public bool IsEdit
        {
            get;
            set;
        }
    }
}
