using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    public class ReportInstanceReturnEngine
    {
        public ReportInstanceReturnEngine()
        {
            //注册
            AddHandle("*", new DefaultReportInstanceReturn());
            AddHandle("Directly", new DirectlyReportInstanceReturn());
            
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceReturn");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IReportInstanceReturn _interface = ReportInstanceReturnBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ReportInstanceReturnEngine ReportInstanceReturnService
        {
            get
            {
                return _ReportInstanceReturnService;
            }
        }private static ReportInstanceReturnEngine _ReportInstanceReturnService = new ReportInstanceReturnEngine();

        private Dictionary<string, IReportInstanceReturn> InterfaceInstanceList = new Dictionary<string, IReportInstanceReturn>();

        private IReportInstanceReturn DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IReportInstanceReturn interfaceInstance)
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

        protected IReportInstanceReturn this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceReturn");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IReportInstanceReturn _interface = ReportInstanceReturnBuilder.Instance.DoBuild(newInstance.Reference);
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

        public List<DictionaryVmodel> GetReturnRptDataSource(ReportInstance RptModel,C_System sys)
        {
            //默认是走经营系统
            string InterfaceName = "*";
 
            if (sys != null)
            {
                if (sys.Category == 2)
                { 
                    
                }
                else if (sys.Category == 3)
                {

                }
                else if (sys.Category == 4)
                {
                    InterfaceName = "Directly";
                    return this[InterfaceName].GetReturnRptDataSource(RptModel, sys);
                }
             
            }


            return this[InterfaceName].GetReturnRptDataSource(RptModel, sys);
        }
    }
}
