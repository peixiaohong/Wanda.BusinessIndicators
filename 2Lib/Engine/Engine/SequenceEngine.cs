using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    internal class SequenceEngine
    {
        public SequenceEngine()
        {
            
            //注册
            AddHandle("*", new DefaultSequence());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ISequence");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ISequence _interface = SequenceBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static SequenceEngine SequenceService
        {
            get
            {
                return _SequenceService;
            }
        }private static SequenceEngine _SequenceService = new SequenceEngine();

        private Dictionary<string, ISequence> InterfaceInstanceList = new Dictionary<string, ISequence>();

        private ISequence DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ISequence interfaceInstance)
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

        protected ISequence this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ISequence");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ISequence _interface = SequenceBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 获取公司排序
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TemplateType"></param>
        /// <param name="RptDetailList"></param>
        /// <param name="Paramters"></param>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetSequence(Guid SystemID, string TemplateType, List<MonthlyReportDetail> RptDetailList, string Paramters=null)
        {
            string InterfaceName = "*";

            DateTime CurrentDate = DateTime.Now;

            if (RptDetailList != null && RptDetailList.Count() > 0)
            { 
                CurrentDate = RptDetailList[0].CreateTime;
            }

            C_System sys = StaticResource.Instance[SystemID, CurrentDate];
            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("ISequence") != null && sys.Configuration.Element("Interfaces").Elements("ISequence").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ISequence").ToList())
                    {
                        if (e.Attribute("TemplateType").Value.ToLower().Trim() == TemplateType.ToLower().Trim())
                        {
                            InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                        }
                    }
                }
            }
            return this[InterfaceName].GetSequence(RptDetailList, Paramters);
        }

    }
}
