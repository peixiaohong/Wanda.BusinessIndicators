using System;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IStakeHolderAdapter
    {
        void Delete(int shid);
        //void Insert(Wanda.LightWorkflow.Entities.StakeHolder lWF_StakeHolder);
        Wanda.Lib.LightWorkflow.Entities.StakeHolder Load(int piid, int userID);
        Wanda.Lib.LightWorkflow.Entities.StakeHolder Load(int shid);
        System.Collections.Generic.List<Wanda.Lib.LightWorkflow.Entities.StakeHolder> LoadList(int processInstanceId);
        //void Update(Wanda.LightWorkflow.Entities.StakeHolder lWF_StakeHolder);
    }
}
