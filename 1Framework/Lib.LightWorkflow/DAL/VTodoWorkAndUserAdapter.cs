using Lib.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Filter;
using System.Linq;
namespace Wanda.Lib.LightWorkflow.Dal
{
    internal class VTodoWorkAndUserAdapter : LwfBaseCompositionAdapterT<VTodoWorkAndUser>
    {
        //add czq 2013-06-21
        public static VTodoWorkAndUserAdapter Instance = new VTodoWorkAndUserAdapter();


        internal PartlyCollection<VTodoWorkAndUser> LoadList(TodoWorkFilter filter)
        {
            return base.GetList(filter,filter.SortKey);
        }
    }
}
