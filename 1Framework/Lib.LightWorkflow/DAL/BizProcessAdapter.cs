using System.Data;
using System.Data.SqlClient;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Interface;

namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class BizProcessAdapter : LwfBaseAdapterT<BBizProcess>
    {
        private static BizProcessAdapter _instance = new BizProcessAdapter();
        public static BizProcessAdapter Instance { get { return _instance; } }
        //逻辑删除
        public int DeleteProcess(BBizProcess Data)
        {
            return base.Update(Data);
        }

    }
}
