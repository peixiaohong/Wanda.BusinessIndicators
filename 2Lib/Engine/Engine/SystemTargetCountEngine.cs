using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    public class SystemTargetCountEngine
    {
        public SystemTargetCountEngine()
        {
            //注册
            AddHandle("*", new DefaultSystemTargetCount());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ISystemTargetCount");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ISystemTargetCount _interface = SystemTargetCountBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static SystemTargetCountEngine SystemTargetCountEngineService
        {
            get
            {
                return _SystemTargetCountEngineService;
            }
        }private static SystemTargetCountEngine _SystemTargetCountEngineService = new SystemTargetCountEngine();

        private Dictionary<string, ISystemTargetCount> InterfaceInstanceList = new Dictionary<string, ISystemTargetCount>();

        private ISystemTargetCount DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ISystemTargetCount interfaceInstance)
        {
            if (!InterfaceInstanceList.ContainsKey(InstanceName))
            {
                InterfaceInstanceList.Add(InstanceName, interfaceInstance);
            }
            else
            {
                InterfaceInstanceList[InstanceName] = interfaceInstance;
            }
        }

        protected ISystemTargetCount this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ISystemTargetCount");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ISystemTargetCount _interface = SystemTargetCountBuilder.Instance.DoBuild(newInstance.Reference);
                        if (_interface != null)
                        {
                            AddHandle(newInstance.InterfaceInstanceName, _interface);
                            return _interface;
                        }
                    }
                }
                return DefaultHandle;
            }
        }

        /// <summary>
        /// 依此排除已经符合公式的明细数据
        /// </summary>
        /// <param name="ViewModel">明细数据</param>
        /// <param name="_SystemID"></param>
        /// <param name="TargetIDs"></param>
        /// <returns></returns>
        public object GetSystemTargetCount(List<MonthlyReportDetail> MissTargetList, List<MonthlyReportDetail> LastMissTargetList, List<MonthlyReportDetail> SingleMissTargetList, List<MonthlyReportDetail> DoubleMissTargetList, List<MonthlyReportDetail> FilterMissTargetList, C_System sys)
        {
            string InterfaceName = "*";
           // C_System sys = StaticResource.Instance[_SystemID];
            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("ISystemTargetCount") != null && sys.Configuration.Element("Interfaces").Elements("ISystemTargetCount").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ISystemTargetCount").ToList())
                    {
                        InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                    }
                }
            }
            return this[InterfaceName].GetSystemTargetCount(MissTargetList, LastMissTargetList, SingleMissTargetList, DoubleMissTargetList,FilterMissTargetList);
        }
    }
}
