using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    public class LastMonthCompaniesEngine
    {
        public LastMonthCompaniesEngine()
        {
            //注册
            AddHandle("*", new DefaultLastMonthCompanies());
            AddHandle("Directly", new LastMonthCompanies_Directly());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ILastMonthCompanies");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ILastMonthCompanies _interface = LastMonthCompaniesBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static LastMonthCompaniesEngine LastMonthCompaniesService
        {
            get
            {
                return _LastMonthCompaniesService;
            }
        }private static LastMonthCompaniesEngine _LastMonthCompaniesService = new LastMonthCompaniesEngine();

        private Dictionary<string, ILastMonthCompanies> InterfaceInstanceList = new Dictionary<string, ILastMonthCompanies>();

        private ILastMonthCompanies DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ILastMonthCompanies interfaceInstance)
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

        protected ILastMonthCompanies this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ILastMonthCompanies");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ILastMonthCompanies _interface = LastMonthCompaniesBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 根据不同的系统,获取不同的未完成公司ID
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public List<Guid> GetLastMonthCompaniesID(object ViewModel, Guid SystemID)
        {

            string InterfaceName = "*";
            
            DateTime CurrentDate = DateTime.Now;
            List<A_MonthlyReportDetail> RptList = (List<A_MonthlyReportDetail>)ViewModel;
            if (RptList != null && RptList.Count > 0)
            {
                CurrentDate = RptList[0].CreateTime;
            }

            C_System sys = StaticResource.Instance[SystemID,CurrentDate];

            if (sys.Category == 4)
            {
                InterfaceName = "Directly";
            }
            else
            {
                if (sys != null && sys.Configuration.Element("Interfaces") != null)
                {
                    if (sys.Configuration.Element("Interfaces").Elements("ILastMonthCompanies") != null && sys.Configuration.Element("Interfaces").Elements("ILastMonthCompanies").ToList().Count > 0)
                    {
                        foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ILastMonthCompanies").ToList())
                        {
                            InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                        }
                    }
                }
            }
            return this[InterfaceName].GetLastMonthCompaniesID(ViewModel);
        }
    }
}
