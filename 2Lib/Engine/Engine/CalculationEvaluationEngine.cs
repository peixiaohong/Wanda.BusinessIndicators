using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;
using System.Xml.Linq;

namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    public class CalculationEvaluationEngine
    {

        public CalculationEvaluationEngine()
        {

            //注册
            AddHandle("*", new DefaultCalculationEvation());
            AddHandle("Directly", new DirectlyCalculationEvation());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ICalculationEvaluation");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ICalculationEvaluation _interface = CalculationEvaluationBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static CalculationEvaluationEngine CalculationEvaluationService
        {
            get
            {
                return _CalculationEvaluationService;
            }
        }private static CalculationEvaluationEngine _CalculationEvaluationService = new CalculationEvaluationEngine();

        private Dictionary<string, ICalculationEvaluation> InterfaceInstanceList = new Dictionary<string, ICalculationEvaluation>();

        private ICalculationEvaluation DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ICalculationEvaluation interfaceInstance)
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

        protected ICalculationEvaluation this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ICalculationEvaluation");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ICalculationEvaluation _interface = CalculationEvaluationBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// <param name="RptDetail"></param>
        /// <param name="TemplateType"></param>
        /// <returns></returns>
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, string TemplateType)
        {
            string InterfaceName = "*";
            C_System sys = new C_System();
            if (RptDetail.CreateTime!=null&&RptDetail.CreateTime!=DateTime.MinValue&&RptDetail.CreateTime.ToString()!= "0001/1/1 0:00:00")
            {
                 sys = StaticResource.Instance[RptDetail.SystemID, RptDetail.CreateTime];
            }
            else
            {
                 sys = StaticResource.Instance[RptDetail.SystemID, DateTime.Now];
            }
            //最后是系统设置
            if (sys!=null)
            {
                if (sys.Category == 4)
                {
                    InterfaceName = "Directly";
                }
            }
            //然后是系统的配置
            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("ICalculationEvaluation") != null && sys.Configuration.Element("Interfaces").Elements("ICalculationEvaluation").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ICalculationEvaluation").ToList())
                    {
                        //if (e.Attribute("TemplateType").Value.ToLower().Trim() == TemplateType.ToLower().Trim())
                        //{
                        InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                        //}
                    }
                }
            }

            //优先级是指标
            C_Target target = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(p => p.ID == RptDetail.TargetID);
            if (target != null && target.Configuration.Element("Interfaces") != null)
            {
                if (target.Configuration.Element("Interfaces").Elements("ICalculationEvaluation") != null && target.Configuration.Element("Interfaces").Elements("ICalculationEvaluation").ToList().Count > 0)
                {
                    foreach (XElement e in target.Configuration.Element("Interfaces").Elements("ICalculationEvaluation").ToList())
                    {
                        //if (e.Attribute("TemplateType").Value.ToLower().Trim() == TemplateType.ToLower().Trim())
                        //{
                        InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                        //}
                    }
                }
            }

            return (B_MonthlyReportDetail)this[InterfaceName].Calculation(RptDetail);
        }

    }
}
