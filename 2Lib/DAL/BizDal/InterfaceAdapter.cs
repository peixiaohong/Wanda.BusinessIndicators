using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    sealed internal class InterfaceAdapter : AppBaseAdapterT<InterfaceDefinition>
    {
        public List<InterfaceDefinition> LoadList()
        {
            return base.Load(p => p.AppendItem("IsDeleted", "0"));
        }
    }

    sealed internal class InterfaceInstanceAdapter : AppBaseAdapterT<InterfaceInstance>
    {
        public List<InterfaceInstance> LoadList()
        {
            return base.Load(p => p.AppendItem("IsDeleted", "0"));
        }

        public List<InterfaceInstance> LoadListByInterfaceName(string InterfaceName)
        {
            return base.Load(p => { p.AppendItem("IsDeleted", "0"); p.AppendItem("InterfaceName", InterfaceName); });
        }
    }
}
