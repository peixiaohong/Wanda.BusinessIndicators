using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.BLL.BizBLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    [Serializable]
    public class ReportInstanceMissTargetEngine
    {
        public ReportInstanceMissTargetEngine()
        {
            //注册
            AddHandle("*", new DefaultReportInstanceMissTarget());
            AddHandle("Directly", new Directly_ReportInstanceMissTarget());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceMissTarget");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IReportInstanceMissTarget _interface = ReportInstanceMisstargetBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ReportInstanceMissTargetEngine ReportInstanceMissTargetService
        {
            get
            {
                return _ReportInstanceMissTargetService;
            }
        }private static ReportInstanceMissTargetEngine _ReportInstanceMissTargetService = new ReportInstanceMissTargetEngine();

        private Dictionary<string, IReportInstanceMissTarget> InterfaceInstanceList = new Dictionary<string, IReportInstanceMissTarget>();

        private IReportInstanceMissTarget DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IReportInstanceMissTarget interfaceInstance)
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

        protected IReportInstanceMissTarget this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceMissTarget");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IReportInstanceMissTarget _interface = ReportInstanceMisstargetBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 获取未完成数据
        /// </summary>
        /// <param name="RptModel"></param>
        /// <param name="IsReported">是否是从上报页面调用</param>
        /// <returns></returns>
        public List<DictionaryVmodel> GetMissTargetRptDataSource(ReportInstance RptModel)
        {
            //默认是走经营系统
            string InterfaceName = "*";

            C_System sys;

            if (RptModel.ReportDetails != null && RptModel.ReportDetails.Count() > 0)
            {
                B_MonthlyReport bm=B_MonthlyreportOperator.Instance.GetMonthlyreport(RptModel.ReportDetails[0].MonthlyReportID);
                sys = StaticResource.Instance[RptModel._System.ID, bm.CreateTime];
            }
            else
            {
                sys = RptModel._System;
            }

           

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
                    return this[InterfaceName].GetMissTargetRptDataSource(RptModel, sys);
                }

            }
            return this[InterfaceName].GetMissTargetRptDataSource(RptModel, sys);
        }
    }
}
