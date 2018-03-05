using Lib.Data;
using System;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IProcessNodeInstanceAdapter
    {
        void Delete(int niid);
        void DeleteList(int  processInstanceId);
        //void Insert(Wanda.LightWorkflow.Entities.ProcessNodeInstance lWF_ProcessNodeInstance);
        Wanda.Lib.LightWorkflow.Entities.ProcessNodeInstance Load(int niid);
        PartlyCollection<ProcessNodeInstance> LoadList(int processInstanceId);
        //void Update(Wanda.LightWorkflow.Entities.ProcessNodeInstance lWF_ProcessNodeInstance);
    }
}
