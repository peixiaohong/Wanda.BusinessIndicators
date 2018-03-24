using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    public class ReportInstanceCurrentMissTargetEngine
    {
        public ReportInstanceCurrentMissTargetEngine()
        {
            //注册
            AddHandle("*", new DefaultReportInstanceCurrentMissTarget());
            AddHandle("Directly", new Directly_ReportInstanceCurrentMissTarget());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceCurrentMissTarget");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IReportInstanceCurrentMissTarget _interface = ReportInstanceCurrentMisstargetBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ReportInstanceCurrentMissTargetEngine ReportInstanceMissTargetService
        {
            get
            {
                return _ReportInstanceCurrentMissTargetService;
            }
        }private static ReportInstanceCurrentMissTargetEngine _ReportInstanceCurrentMissTargetService = new ReportInstanceCurrentMissTargetEngine();

        private Dictionary<string, IReportInstanceCurrentMissTarget> InterfaceInstanceList = new Dictionary<string, IReportInstanceCurrentMissTarget>();

        private IReportInstanceCurrentMissTarget DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IReportInstanceCurrentMissTarget interfaceInstance)
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

        protected IReportInstanceCurrentMissTarget this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceCurrentMissTarget");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IReportInstanceCurrentMissTarget _interface = ReportInstanceCurrentMisstargetBuilder.Instance.DoBuild(newInstance.Reference);
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
        public List<DictionaryVmodel> GetCurrentMissTargetRptDataSource(ReportInstance RptModel)
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
                    return this[InterfaceName].GetCurrentMissTargetRptDataSource(RptModel, sys);
                }

            }
            return this[InterfaceName].GetCurrentMissTargetRptDataSource(RptModel, sys);
        }
    }
}
