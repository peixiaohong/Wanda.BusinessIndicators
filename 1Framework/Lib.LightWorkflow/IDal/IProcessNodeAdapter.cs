using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IProcessNodeAdapter
    {
        void Delete(int nid);
        //void Insert(Wanda.LightWorkflow.Entities.ProcessNode lWF_ProcessNode);
        Wanda.Lib.LightWorkflow.Entities.ProcessNode Load(int nid);
        List<ProcessNode> LoadList(int processID, string processType);
        //void Update(Wanda.LightWorkflow.Entities.ProcessNode lWF_ProcessNode);
    }
}
