using Lib.Data;
using System;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IProcessAdapter
    {
        void Delete(int pid);
        PartlyCollection<Process> GetAllActivedProcesses();
        //void Insert(Wanda.LightWorkflow.Entities.Process lWF_Process);
        Wanda.Lib.LightWorkflow.Entities.Process Load(int pid);
        Wanda.Lib.LightWorkflow.Entities.Process LoadByCode(string processCode,int CongID);
        //Wanda.Lib.LightWorkflow.Entities.Process LoadByCode(string processCode);
    }
}
