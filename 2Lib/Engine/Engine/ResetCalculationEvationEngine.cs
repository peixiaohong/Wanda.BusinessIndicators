using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    public class ResetCalculationEvationEngine
    {
        public ResetCalculationEvationEngine()
        {

            //注册
            AddHandle("*", new ResetCalculationEvation());

            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IResetCalculationEvaluation");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IResetCalculationEvaluation _interface = ResetCalculationEvationBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ResetCalculationEvationEngine ResetCalculationEvaluationService
        {
            get
            {
                return _ResetCalculationEvaluationService;
            }
        }private static ResetCalculationEvationEngine _ResetCalculationEvaluationService = new ResetCalculationEvationEngine();

        private Dictionary<string, IResetCalculationEvaluation> InterfaceInstanceList = new Dictionary<string, IResetCalculationEvaluation>();

        private IResetCalculationEvaluation DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IResetCalculationEvaluation interfaceInstance)
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

        protected IResetCalculationEvaluation this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IResetCalculationEvaluation");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IResetCalculationEvaluation _interface = ResetCalculationEvationBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 计算引擎（优先级是在指标上配置，然后是在系统上配置）
        /// </summary>
        /// <param name="RptDetail">当前月 </param>
        /// <param name="TemplateType"> 全年的数据</param>
        /// <returns></returns>
        public List<A_MonthlyReportDetail> ResetCalculation(List<A_MonthlyReportDetail> RptDetailList, List<A_MonthlyReportDetail> ALLRptDetailList)
        {
            string InterfaceName = "*";

            return (List<A_MonthlyReportDetail>)this[InterfaceName].ResetCalculation(RptDetailList, ALLRptDetailList);
        }


    }
}
