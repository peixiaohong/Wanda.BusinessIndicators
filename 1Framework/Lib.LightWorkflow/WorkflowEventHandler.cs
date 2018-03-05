using System;
using System.Collections.Generic;
using System.Linq;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Interface;

namespace Wanda.Lib.LightWorkflow
{
    public delegate void WorkflowEventHandler(object sender, WorkflowEventArgs e);
}
