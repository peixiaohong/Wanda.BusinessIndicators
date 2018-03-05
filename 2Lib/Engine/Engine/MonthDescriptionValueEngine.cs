using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    public class MonthDescriptionValueEngine
    {
        public MonthDescriptionValueEngine()
        {
            //注册
            AddHandle("*", new DefaultGetMonthDescriptionValue());
           // AddHandle("PRO_Centre_GetMonthDescriptionValue", new Pro_Centre_GetMonthDescriptionValue());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IGetMonthDescriptionValue");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IGetMonthDescriptionValue _interface = MonthDescriptionValueBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static MonthDescriptionValueEngine MonthDescriptionValueService
        {
            get
            {
                return _MonthDescriptionValueService;
            }
        }private static MonthDescriptionValueEngine _MonthDescriptionValueService = new MonthDescriptionValueEngine();

        private Dictionary<string, IGetMonthDescriptionValue> InterfaceInstanceList = new Dictionary<string, IGetMonthDescriptionValue>();

        private IGetMonthDescriptionValue DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IGetMonthDescriptionValue interfaceInstance)
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

        protected IGetMonthDescriptionValue this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IGetMonthDescriptionValue");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IGetMonthDescriptionValue _interface = MonthDescriptionValueBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 根据不同的系统中的配置项,获取不同的月报说明
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public Hashtable GetMonthDescriptionValue(object ViewModel, Guid SystemID)
        {
            string InterfaceName = "*";

            DateTime CurrentDate = DateTime.Now;
            List<MonthlyReportDetail> RptList = (List<MonthlyReportDetail>)ViewModel;
            if (RptList == null || RptList.Count <= 0)
            {
                CurrentDate = RptList[0].CreateTime;
            }

            C_System sys = StaticResource.Instance[SystemID,CurrentDate];

            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("IGetMonthDescriptionValue") != null && sys.Configuration.Element("Interfaces").Elements("IGetMonthDescriptionValue").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("IGetMonthDescriptionValue").ToList())
                    {
                        InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                    }
                }
            }

            return this[InterfaceName].GetMonthDescriptionValue(ViewModel);
        }



    }
}
