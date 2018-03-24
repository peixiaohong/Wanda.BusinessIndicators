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
    public class ExceptionTargetEvaluationEngine
    {
        public ExceptionTargetEvaluationEngine()
        {
            //注册
            AddHandle("*", new DefaultExceptionTargetEvaluation());

            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IExceptionTargetEvaluation");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IExceptionTargetEvaluation _interface = ExceptionTargetEvaluationBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }
        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ExceptionTargetEvaluationEngine ExceptionTargetEvaluationService
        {
            get
            {
                return _ExceptionTargetEvaluationService;
            }
        }private static ExceptionTargetEvaluationEngine _ExceptionTargetEvaluationService = new ExceptionTargetEvaluationEngine();

        private Dictionary<string, IExceptionTargetEvaluation> InterfaceInstanceList = new Dictionary<string, IExceptionTargetEvaluation>();

        private IExceptionTargetEvaluation DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IExceptionTargetEvaluation interfaceInstance)
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

        protected IExceptionTargetEvaluation this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IExceptionTargetEvaluation");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IExceptionTargetEvaluation _interface = ExceptionTargetEvaluationBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 判断异常指标
        /// </summary>
        /// <param name="RptDetail"></param>
        /// <param name="TemplateType"></param>
        /// <returns></returns>
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, string TemplateType)
        {
            string InterfaceName = "*";
            C_System sys = StaticResource.Instance[RptDetail.SystemID, RptDetail.CreateTime];
            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("IExceptionTargetEvaluation") != null && sys.Configuration.Element("Interfaces").Elements("IExceptionTargetEvaluation").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("IExceptionTargetEvaluation").ToList())
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

        public bool Calculation(Guid SystemID,Guid TargetID,Guid CompanyID, string TemplateType)
        {
            string InterfaceName = "*";
            C_System sys = StaticResource.Instance[SystemID,DateTime.Now];
            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("IExceptionTargetEvaluation") != null && sys.Configuration.Element("Interfaces").Elements("IExceptionTargetEvaluation").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("IExceptionTargetEvaluation").ToList())
                    {
                        //if (e.Attribute("TemplateType").Value.ToLower().Trim() == TemplateType.ToLower().Trim())
                        //{
                        InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                        //}
                    }
                }
            }
            return (bool)this[InterfaceName].Calculation(SystemID, TargetID, CompanyID);
        }
    }
}
