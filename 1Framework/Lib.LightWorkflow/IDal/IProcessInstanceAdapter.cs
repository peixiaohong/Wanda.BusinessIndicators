using System;
using System.Collections.Generic;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IProcessInstanceAdapter
    {
        void Delete(int piid);
        //void Insert(Wanda.LightWorkflow.Entities.ProcessInstance lWF_ProcessInstance);
        ProcessInstance Load(int piid);
        ProcessInstance LoadByBizProcessID(int bizProcessID);
        List<ProcessInstance> LoadList(List<string> bizProcessIDList);
        List<ProcessInstance> LoadList(Wanda.Lib.LightWorkflow.CommonConsts.ProcessInstanceStatus? status, string userID, string projectID, string processCode);
        List<ProcessInstance> LoadListByCreateUserID(string userID, Dictionary<string, string> whereString);
        List<ProcessInstance> LoadListByRelatedUserID(string userID, Dictionary<string, string> whereString);
        //void Update(Wanda.LightWorkflow.Entities.ProcessInstance lWF_ProcessInstance);
    }
}
