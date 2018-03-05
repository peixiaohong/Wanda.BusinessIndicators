using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.BLL.BizBLL;


namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    public  class CompanyExceptionTargetEngine
    {
        public CompanyExceptionTargetEngine()
        {
         //注册
            AddHandle("*", new DefaultCompanyExceptionTarget());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ICompanyExceptionTarget");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ICompanyExceptionTarget _interface = CompanyExceptionTargetBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }
        

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static CompanyExceptionTargetEngine CompanyExceptionTargetEngineService
        {
            get
            {
                return _CompanyExceptionTargetEngineService;
            }
        }private static CompanyExceptionTargetEngine _CompanyExceptionTargetEngineService = new CompanyExceptionTargetEngine();

        private Dictionary<string, ICompanyExceptionTarget> InterfaceInstanceList = new Dictionary<string, ICompanyExceptionTarget>();

        private ICompanyExceptionTarget DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ICompanyExceptionTarget interfaceInstance)
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

        protected ICompanyExceptionTarget this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ICompanyExceptionTarget");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ICompanyExceptionTarget _interface = CompanyExceptionTargetBuilder.Instance.DoBuild(newInstance.Reference);
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


        public int GetCompanyExceptionTarget( C_Company  Model  )
        {

            string InterfaceName = "*";

            return this[InterfaceName].GetCompanyExceptionTarget(Model);

        }


    }
}
