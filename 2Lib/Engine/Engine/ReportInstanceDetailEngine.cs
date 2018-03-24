using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    public class ReportInstanceDetailEngine
    {
        public ReportInstanceDetailEngine()
        {
            //注册
            AddHandle("*", new DefaultReportInstanceDetail());
            AddHandle("ProRptDetail", new ReportInstanceProDetail());
            AddHandle("GroupRptDetail", new ReportInstanceGroupDetail());
            AddHandle("DirectlyRptDetail", new ReportInstanceDirectlyDetail());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceDetail");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IReportInstanceDetail _interface = ReportInstanceDetailBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static ReportInstanceDetailEngine ReportInstanceDetailService
        {
            get
            {
                return _ReportInstanceDetailService;
            }
        }private static ReportInstanceDetailEngine _ReportInstanceDetailService = new ReportInstanceDetailEngine();

        private Dictionary<string, IReportInstanceDetail> InterfaceInstanceList = new Dictionary<string, IReportInstanceDetail>();

        private IReportInstanceDetail DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, IReportInstanceDetail interfaceInstance)
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

        protected IReportInstanceDetail this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IReportInstanceDetail");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        IReportInstanceDetail _interface = ReportInstanceDetailBuilder.Instance.DoBuild(newInstance.Reference);
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
        /// 获取明细指标数据
        /// </summary>
        /// <param name="RptModel">ReportInstance:通用类型数据</param>
        /// <param name="strCompanyProperty">公司属性：在查询经营系统中用到，项目系统中暂时没用到, （这里占用该字段来标注，引擎的调用方式：当strCompanyProperty=“Reported”时，表示是在指标上报页面调用的，反之则是在查询的时候调用）</param>
        /// <param name="strOrderType">排序字段：在经营系统中用到，在项目系统中被当作IsPlan字段来用</param>
        /// <param name="IncludeHaveDetail">是否包含明细：在经营系统中用到，在项目系统中被当作IsLatestVersion用</param>
        /// <returns></returns>
        public List<DictionaryVmodel> GetDetailRptDataSource(ReportInstance RptModel, string strCompanyProperty, string strOrderType, bool IncludeHaveDetail)
        {
            //默认是走经营系统
            string InterfaceName = "*";
            C_System sys = RptModel._System;
            if (sys != null)
            {
                if (sys.Category == 2)
                {
                    InterfaceName = "ProRptDetail";
                    return this[InterfaceName].GetDetailRptDataSource(RptModel, strCompanyProperty, strOrderType, IncludeHaveDetail);
                }
                else if (sys.Category == 3)
                {
                    InterfaceName = "GroupRptDetail";
                    return this[InterfaceName].GetDetailRptDataSource(RptModel, strCompanyProperty, strOrderType, IncludeHaveDetail);
                }
                else if (sys.Category == 4)
                {
                    InterfaceName = "DirectlyRptDetail";
                    return this[InterfaceName].GetDetailRptDataSource(RptModel, strCompanyProperty, strOrderType, IncludeHaveDetail);
                }

            }


            return this[InterfaceName].GetDetailRptDataSource(RptModel, strCompanyProperty, strOrderType, IncludeHaveDetail);
        }

    }
}
