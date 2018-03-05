using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IApprovalLogAdapter
    {
        void Delete(int apid);
        //  void Insert(Wanda.LightWorkflow.Entities.ApprovalLog lWF_ApprovalLog);
        Wanda.Lib.LightWorkflow.Entities.ApprovalLog Load(int apid);
        //void Update(Wanda.LightWorkflow.Entities.ApprovalLog lWF_ApprovalLog);
        PartlyCollection<ApprovalLog> LoadList(int processInstanceId);
    }
}
