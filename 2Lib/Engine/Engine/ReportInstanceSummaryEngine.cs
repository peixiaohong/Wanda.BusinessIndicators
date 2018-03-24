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
    public class ReportInstanceSummaryEngine
    {
        public ReportInstanceSummaryEngine()
        {
            //注册经营系统
            AddHandle("*", new DefaultReportInstanceSummary());
            //项目公司
            AddHandle("Project", new ReportInstanceSummary_Project());
            //注册集团总部
            AddHandle("Group", new ReportInstanceSummary_Group());
            //注册直属公司
            AddHandle("Directly", new ReportInstanceSummary_Directly());
            
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceSummary");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IReportInstanceSummary _interface = ReportInstanceSummaryBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ReportInstanceSummaryEngine ReportInstanceSummaryService
        {
            get
            {
                return _ReportInstanceSummaryService;
            }
        }private static ReportInstanceSummaryEngine _ReportInstanceSummaryService = new ReportInstanceSummaryEngine();

        private Dictionary<string, IReportInstanceSummary> InterfaceInstanceList = new Dictionary<string, IReportInstanceSummary>();

        private IReportInstanceSummary DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IReportInstanceSummary interfaceInstance)
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

        protected IReportInstanceSummary this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceSummary");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IReportInstanceSummary _interface = ReportInstanceSummaryBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 根据不同系统中的XML,获取不同的补充项
        /// </summary>
        /// <param name="RptModel"></param>
        /// <param name="IsTargetPlan"></param>
        /// <returns></returns>
        public List<DictionaryVmodel> GetSummaryRptDataSource(ReportInstance RptModel,  bool IsTargetPlan)
        {
            //默认是走经营系统
            string InterfaceName = "*";

            C_System sys;

            if (RptModel.ReportDetails != null && RptModel.ReportDetails.Count() > 0)
            {
                B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(RptModel.ReportDetails[0].MonthlyReportID);
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
                    InterfaceName = "Project";
                }
                else if (sys.Category == 3)
                {
                    InterfaceName = "Group";
                }
                else if (sys.Category == 4)
                {
                    InterfaceName = "Directly";
                }
            }
            return this[InterfaceName].GetSummaryRptDataSource(RptModel, IsTargetPlan);
        }
    }
}
