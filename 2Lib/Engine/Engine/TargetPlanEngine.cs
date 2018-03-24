using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Engine.Engine
{
    [Serializable]
    public class TargetPlanEngine
    {
        public TargetPlanEngine()
        {
            //注册
            AddHandle("*", new DefaultTargetPlan());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ITargetPlanInstance");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ITargetPlanInstance _interface =TargetPlanBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static TargetPlanEngine TargetPlanEngineService
        {
            get
            {
                return _TargetPlanService;
            }
        }private static TargetPlanEngine _TargetPlanService = new TargetPlanEngine();

        private Dictionary<string, ITargetPlanInstance> InterfaceInstanceList = new Dictionary<string, ITargetPlanInstance>();

        private ITargetPlanInstance DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ITargetPlanInstance interfaceInstance)
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

        protected ITargetPlanInstance this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ITargetPlanInstance");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ITargetPlanInstance _interface =TargetPlanBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 获取上报计划数
        /// </summary>
        /// <param name="_SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="TargetPlanID"></param>
        /// <param name="IsLatestVersion"></param>
        /// <returns></returns>
        public object GetTargetPlanSource(Guid _SystemID, int FinYear, Guid TargetPlanID, bool IsLatestVersion)
        {

            string InterfaceName = "*";
            C_System sys = StaticResource.Instance[_SystemID,DateTime.Now];
            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("ITargetPlanInstance") != null && sys.Configuration.Element("Interfaces").Elements("ITargetPlanInstance").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ITargetPlanInstance").ToList())
                    {
                        InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                    }
                }
            }
            return (object)(this[InterfaceName].GetTargetPlanSource(_SystemID, FinYear, TargetPlanID, IsLatestVersion));
        }
    }
}
