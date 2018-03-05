using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface ITodoWorkAdapter
    {
        void Delete(int todoWorkID);
        void DeleteAll(int piid);
        Wanda.Lib.LightWorkflow.Entities.TodoWork Load(int piid, int userID);
        Wanda.Lib.LightWorkflow.Entities.TodoWork Load(int todoWorkID);
        List<TodoWork> LoadListByProcessInstanceID(int processInstanceID);
    }
}
