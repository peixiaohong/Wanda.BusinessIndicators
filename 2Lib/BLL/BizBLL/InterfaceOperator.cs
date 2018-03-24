using Lib.Data.AppBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.DAL.BizDal;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.BLL.BizBLL
{
    public class InterfaceOperator : BizOperatorBase<InterfaceDefinition>
    {
        public static readonly InterfaceOperator Instance = BizOperatorFactory.Create<InterfaceOperator>();
        private static InterfaceAdapter dInterfaceAdapter = AdapterFactory.GetAdapter<InterfaceAdapter>();

        protected override BaseAdapterT<InterfaceDefinition> GetAdapter()
        {
            return dInterfaceAdapter;
        }

        public List<InterfaceDefinition> LoadList()
        {
            return dInterfaceAdapter.LoadList();
        }
    }

    public class InterfaceInstanceOperator : BizOperatorBase<InterfaceInstance>
    {
        public static readonly InterfaceInstanceOperator Instance = BizOperatorFactory.Create<InterfaceInstanceOperator>();
        private static InterfaceInstanceAdapter iInterfaceInstanceAdapter = AdapterFactory.GetAdapter<InterfaceInstanceAdapter>();

        protected override BaseAdapterT<InterfaceInstance> GetAdapter()
        {
            return iInterfaceInstanceAdapter;
        }

        public List<InterfaceInstance> LoadList()
        {
            return iInterfaceInstanceAdapter.LoadList();
        }
        public List<InterfaceInstance> LoadListByName(string InterfaceName)
        {
            return iInterfaceInstanceAdapter.LoadListByInterfaceName(InterfaceName);
        }
    }

}
