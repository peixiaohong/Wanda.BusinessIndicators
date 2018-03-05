using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface IOAMQMessagesAdapter
    {
        void Delete(int messageid);
        //void Insert(Wanda.LightWorkflow.Entities.OAMQMessages oAMQMessages);
        OAMQMessages Load(int messageid);
        List<OAMQMessages> LoadList(int count);
        //void Update(Wanda.LightWorkflow.Entities.OAMQMessages oAMQMessages);
    }
}
