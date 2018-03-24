using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    internal class BizContextEngine
    {
        public BizContextEngine()
        {
            //注册
            AddHandle("*", new DefaultBizContext());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IBizContext");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IBizContext _interface = BizContextBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static BizContextEngine BizContextService
        {
            get
            {
                return _BizContextService;
            }
        }private static BizContextEngine _BizContextService = new BizContextEngine();

        private Dictionary<string, IBizContext> InterfaceInstanceList = new Dictionary<string, IBizContext>();

        private IBizContext DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IBizContext interfaceInstance)
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

        protected IBizContext this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IBizContext");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IBizContext _interface = BizContextBuilder.Instance.DoBuild(newInstance.Reference);
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

        public Hashtable GetBizContext(object ViewModel,string ViewModelName)
        {
            return this[ViewModelName].GetBizContext(ViewModel);
        }
    }
}
