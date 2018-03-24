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
using Lib.Core;
namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    public class TargetEvaluationEngine
    {
        public TargetEvaluationEngine()
        {

            //注册
            //正常计算
            AddHandle("*", new DefaultTargetEvaluation());
            AddHandle("2", new ProfitTargetEvaluation());
            AddHandle("3", new CostTargetEvaluation());
            AddHandle("Project", new ProjectTargetEvaluation()); //项目系统
            AddHandle("Group", new GroupjectTargetEvaluation()); //集团总部系统

            //重新计算
            AddHandle("R1", new DefaultResetTargetEvaluation());
            AddHandle("R4", new DefaultResetTargetEvaluation());
            AddHandle("R2", new ResetProfitTargetEvaluation());
            AddHandle("R3", new ResetCostTargetEvaluation());
            AddHandle("RProject", new ResetProjectTargetEvaluation()); //项目系统
            AddHandle("RGroup", new ResetGroupjectTargetEvaluation()); //集团总部系统

            //月度报告
            AddHandle("SUM_1", new DefaultSummaryTargetEvaluation());
            AddHandle("SUM_4", new DefaultSummaryTargetEvaluation());
            AddHandle("SUM_2", new ProfitSummaryTargetEvaluation());
            AddHandle("SUM_3", new CostSummaryTargetEvaluation());
            AddHandle("SUM_Group", new GroupSummaryTargetEvaluation());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ITargetEvaluation");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                ITargetEvaluation _interface = TargetEvaluationBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static TargetEvaluationEngine TargetEvaluationService
        {
            get
            {
                return _TargetEvaluationService;
            }
        }private static TargetEvaluationEngine _TargetEvaluationService = new TargetEvaluationEngine();

        private Dictionary<string, ITargetEvaluation> InterfaceInstanceList = new Dictionary<string, ITargetEvaluation>();

        private ITargetEvaluation DefaultHandle
        {
            get
            {
                return InterfaceInstanceList["*"];
            }
        }

        protected void AddHandle(string InstanceName, ITargetEvaluation interfaceInstance)
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

        protected ITargetEvaluation this[string key]
        {
            get
            {
                if (InterfaceInstanceList.ContainsKey(key))
                {
                    return InterfaceInstanceList[key];
                }
                else
                {
                    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IExcelParse");
                    InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                    if (newInstance != null)
                    {
                        ITargetEvaluation _interface = TargetEvaluationBuilder.Instance.DoBuild(newInstance.Reference);
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



        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
            bool IsBaseline = false;
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            //是否能够改动计划，如果能改，取Excel数据，不能改取系统计划数(在Taget Configuration配置)
            List<A_TargetPlanDetail> listCurrentMonthData = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).ToList().Where(p => p.FinMonth == rpt.FinMonth && p.CompanyID == rpt.CompanyID).ToList();
            if (listCurrentMonthData.Count > 0)
            {
                A_TargetPlanDetail currentMonthData = listCurrentMonthData[0];
                bool IsModifiy = false;
                if (currentMonthData.Target != RptDetail.OPlanAmmount)
                {
                    //获取当前指标
                    List<C_Target> listTarget = StaticResource.Instance.GetTargetList(RptDetail.SystemID, RptDetail.CreateTime).ToList().Where(p => p.ID == RptDetail.TargetID).ToList();
                    if (listTarget.Count > 0)
                    {
                        //获取当前指标的Configuration
                        IList<System.Xml.Linq.XElement> xmlConfiguration = listTarget[0].Configuration.Elements("IsModifyTargetPlanDetail").ToList();
                        if (xmlConfiguration.Count > 0)
                        {
                            //判断当前指标是否可以修改
                            if (xmlConfiguration.ToList()[0].GetAttributeValue("IsModifiy", "") == "True")
                            {
                                IsModifiy = true;
                            }
                        }
                    }

                }
                //如果IsModifiy为True，则计划数可以修改，否则反之。
                if (IsModifiy == false)
                {
                    RptDetail.OPlanAmmount = currentMonthData.Target;
                }
            }




            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);
            if (lastMonthData != null)
            {
                rpt.NAccumulativeActualAmmount = lastMonthData.NAccumulativeActualAmmount + rpt.NActualAmmount;
                rpt.OAccumulativeActualAmmount = lastMonthData.OAccumulativeActualAmmount + rpt.OActualAmmount;
                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    rpt.NAccumulativePlanAmmount = lastMonthData.NAccumulativePlanAmmount + rpt.NPlanAmmount;
                    rpt.OAccumulativePlanAmmount = lastMonthData.OAccumulativePlanAmmount + rpt.OPlanAmmount;
                    rpt.CommitDate = lastMonthData.CurrentMonthCommitDate;
                    rpt.CommitReason = lastMonthData.CurrentMonthCommitReason;
                }
                else
                {

                    IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                    rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                    rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                    
                }

            }
            else
            {
                if (WithCounter)
                {
                    rpt.NAccumulativeActualAmmount = rpt.NActualAmmount;
                    rpt.NAccumulativePlanAmmount = rpt.NPlanAmmount;
                    rpt.OAccumulativeActualAmmount = rpt.OActualAmmount;
                    rpt.OAccumulativePlanAmmount = rpt.OPlanAmmount;
                }
            }


            //特殊处理差额，针对指标
            XElement element = null;
            element = T.Configuration;
            XElement subElement = null;

            bool IsDifferenceException = false;

            if ( element.Element("IsDifferenceExceptionTarget") != null)
            {
                subElement = element.Elements("IsDifferenceExceptionTarget").ToList()[0];
                IsDifferenceException = subElement.GetAttributeValue("value", false);
            }

            if (!IsDifferenceException)
            {
                rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
                rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;

                rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
                rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;
            }
          
            rpt = (B_MonthlyReportDetail)this[T.TargetType.ToString()].Calculation(RptDetail);

            if (T.NeedEvaluation)
            {

                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况

                //上月累计未完成

                if (!IsBaseline)  //从1月开始重新计算
                {
                    if (lastMonthData != null && (lastMonthData.IsMissTarget || lastMonthData.NAccumulativeDifference < 0))
                    {
                        if (rpt.IsMissTarget || rpt.NAccumulativeDifference < 0)
                        {
                            if (lastMonthData.CurrentMonthCommitDate >= ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.Returning;
                                if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear) )
                                {
                                    rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                }
                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.NotReturn;
                            }
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = lastMonthData.Counter + 1;
                            }
                        }
                        else
                        {
                            if (lastMonthData.CurrentMonthCommitDate >= ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.AccomplishInadvance;
                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.Accomplish;
                            }
                            if (WithCounter)
                            {
                                rpt.Counter = 0;
                                rpt.FirstMissTargetDate = null;
                            }
                        }

                        //要求期限，是本月
                        if (lastMonthData.CurrentMonthCommitDate < ReportDate && lastMonthData.CurrentMonthCommitDate >= ReportDate.AddMonths(-1))
                        {
                            rpt.IsCommitDate = 1;
                        }
                        else
                        {
                            rpt.IsCommitDate = 0;
                        }

                    }
                    //无上月数据(1月)或上月累计完成
                    else
                    {
                        if (rpt.IsMissTarget || rpt.NAccumulativeDifference < 0)
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                //无上月数据(1月)或上月累计完成
                //重新计算
                else
                {
                    if (rpt.IsMissTarget || rpt.NAccumulativeDifference < 0)
                    {
                        rpt.ReturnType = (int)EnumReturnType.New;
                        if (rpt.IsMissTarget && WithCounter)
                        {
                            rpt.Counter = 1;
                            rpt.FirstMissTargetDate = ReportDate;
                        }
                    }
                }
            }
            else
            {
                rpt.IsMissTarget = false;
                rpt.IsMissTargetCurrent = false;
            }

            return rpt;
        }

        public MonthReportSummaryViewModel SummaryCalculation(MonthReportSummaryViewModel model)
        {
            C_Target T = StaticResource.Instance.TargetList[model.SystemID].Find(t => t.ID == model.TargetID);
            if (StaticResource.Instance.SystemList.Where(p=>p.ID==model.SystemID).FirstOrDefault().Category == 3)
            {
                return (MonthReportSummaryViewModel)this["SUM_Group"].Calculation(model);
            }
            
            return (MonthReportSummaryViewModel)this["SUM_" + T.TargetType.ToString()].Calculation(model);
        }


        /// <summary>
        /// 正常计算
        /// </summary>
        /// <param name="RptDetail"></param>
        /// <param name="TemplateType"></param>
        /// <returns></returns>
        public object Calculation(B_MonthlyReportDetail RptDetail, string TemplateType)
        {
            string InterfaceName = "*";

            C_System sys = new C_System();
            if (RptDetail.CreateTime!=null&&RptDetail.CreateTime!=DateTime.MinValue)
            {
                sys = StaticResource.Instance[RptDetail.SystemID, RptDetail.CreateTime];
            }
            else
            {
                sys = StaticResource.Instance[RptDetail.SystemID,DateTime.Now];
            }
            

            if (sys.Category == 2) //如果系统是项目系统
            {
                InterfaceName = "Project"; //项目公司有单独的计算
            }
            else if (sys.Category == 3)
            {
                InterfaceName = "Group"; //集团总部有单独的计算
            }
            else
            {
                C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
                if (T != null)
                {
                    InterfaceName = T.TargetType.ToString();
                }
            }

            if (sys != null && sys.Configuration.Element("Interfaces") != null)
            {
                if (sys.Configuration.Element("Interfaces").Elements("ITargetCalculation") != null && sys.Configuration.Element("Interfaces").Elements("ITargetCalculation").ToList().Count > 0)
                {
                    foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ITargetCalculation").ToList())
                    {
                        if (e.Attribute("TemplateType").Value.ToLower().Trim() == TemplateType.ToLower().Trim())
                        {
                            InterfaceName = e.Attribute("InterfaceName").Value.Trim();
                        }
                    }
                }
            }

            return this[InterfaceName].Calculation(RptDetail);
        }


        /// <summary>
        /// A表重新计算
        /// </summary>
        /// <param name="RptDetail"></param>
        /// <param name="TemplateType"></param>
        /// <returns></returns>
        public object Calculation(A_MonthlyReportDetail RptDetail, string TemplateType)
        {
            string InterfaceName = "*";

            C_System sys = StaticResource.Instance[RptDetail.SystemID, DateTime.Now];

            if (sys.Category == 2) //如果系统是项目系统
            {
                InterfaceName = "RProject"; //项目公司有单独的计算
            }
            else if (sys.Category == 3)
            {
                InterfaceName = "RGroup"; //集团总部有单独的计算
            }
            else
            {
                C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
                if (T != null)
                {
                    InterfaceName ="R"+T.TargetType.ToString();
                }
            }

            return this[InterfaceName].Calculation(RptDetail);
        }


    }


}