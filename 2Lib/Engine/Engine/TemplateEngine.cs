using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    internal class TemplateEngine
    {
        public TemplateEngine()
        {
            //注册
            AddHandle("*", new DefaultGetTemplate());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IGetTemplate");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IGetTemplate _interface = TemplateBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static TemplateEngine TemplateService
        {
            get
            {
                return _TemplateService;
            }
        }private static TemplateEngine _TemplateService = new TemplateEngine();

        private Dictionary<string, IGetTemplate> InterfaceInstanceList = new Dictionary<string, IGetTemplate>();

        private IGetTemplate DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IGetTemplate interfaceInstance)
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

        protected IGetTemplate this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IExcelParse");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IGetTemplate _interface = TemplateBuilder.Instance.DoBuild(newInstance.Reference);
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

        public string GetTemplate(Guid SystemID)
        {
            return null;
        }
    }
}
