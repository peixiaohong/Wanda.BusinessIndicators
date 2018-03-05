using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Model;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class CalculateMisssTargetScope : Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("计算未完成指标考核范围Stare!");

            //获取需要的项目系统
            List<C_System> sysList = C_SystemOperator.Instance.GetSystemListBySeq().ToList();

            //获取上报年
            C_ReportTime RTime = C_ReportTimeOperator.Instance.GetReportTime();

            sysList.ForEach(S =>
            {
                Common.ScheduleService.Log.Instance.Info("循环系统 Stare! " + S.SystemName );

                //筛选指标类型是 收入和净利润 (这里还包含了，旅业的客流量) ,不包含成本类的指标
                List<C_Target> targetList = StaticResource.Instance.GetTargetList(S.ID, DateTime.Now).Where(T => T.NeedEvaluation == true && (T.TargetType != (int)EnumTargetType.Cost)).ToList();
                    
                targetList.ForEach(t =>
                {
                    Common.ScheduleService.Log.Instance.Info("循环系统下的考核指标 Stare! " + t.TargetName.ToString() );
                    //获取上报年计划指标 （参数，公司ID， 指标ID， 系统ID）
                    List<A_TargetPlanDetail> targetPlanList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(S.ID, RTime.ReportTime.Value.Year).ToList();

                    for (int i = 1; i <= 12; i++)
                    { 
                        Common.ScheduleService.Log.Instance.Info("循环系统下的考核指标的 全年12月 Stare! " + i.ToString() + "月");

                        #region  循环12月，在里面查询公司
                        
                        //每个月,上报的月报单独指标 A表
                        List<A_MonthlyReportDetail> currentMRDList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(S.ID, RTime.ReportTime.Value.Year, i).Where(M => M.TargetID == t.ID).ToList();


                        //每个月，上报的月报单独指标， B表
                        List<B_MonthlyReportDetail> currentB_MRDList = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(S.ID,RTime.ReportTime.Value.Year,i,Guid.Empty).Where(M => M.TargetID == t.ID).ToList();

                        //未完成数量
                        int MissTargetA = 0;
                        int MissTargetB = 0;

                        MissTargetA = currentMRDList.Where(p => p.IsMissTarget == true).ToList().Count();

                        MissTargetB = currentB_MRDList.Where(p => p.IsMissTarget == true).ToList().Count();

                        //每个月的单独计划指标公司
                        List<A_TargetPlanDetail> currentTargetPlanList = targetPlanList.Where(tp => tp.FinMonth <= i && tp.TargetID == t.ID).ToList();


                        //获取上报且考核的公司
                        List<C_Company> companyList = C_CompanyOperator.Instance.GetCompanyListByNeedEvaluation(t.ID, S.ID).ToList();


                        //该list 作为记数用 A表的 
                        List<C_Company> companyListCount = new List<C_Company>();

                        //该list 作为记数用 B表的 
                        List<C_Company> companyB_ListCount = new List<C_Company>();


                        //这里循环有效的考核范围公司
                        companyList.ForEach(c =>
                        {
                            //首先判断当月公司是否存在,存在代表：大于0
                            int isCompanyCount = currentTargetPlanList.Where(P => P.CompanyID == c.ID).Count();

                            //获取当月指标等于0的个数
                            int tempCount = currentTargetPlanList.Where(P => P.CompanyID == c.ID && P.Target == 0).Count();

                            //记录A表数据
                            int isMonthRotCount = currentMRDList.Where(MR => MR.CompanyID == c.ID).Count();

                            //记录B表数据
                            int isB_MonthRotCount = currentB_MRDList.Where(MR => MR.CompanyID == c.ID).Count();


                            A_MonthlyReportDetail MonthRpt = currentMRDList.Where(MR => MR.CompanyID == c.ID).FirstOrDefault();

                            B_MonthlyReportDetail B_MonthRpt = currentB_MRDList.Where(MR => MR.CompanyID == c.ID).FirstOrDefault();

                            //A表数据的List
                            if (isMonthRotCount > 0 && isCompanyCount>0 && tempCount != i)
                            {
                                //这里排除总部
                                if (c.CompanyName.IndexOf("总部") == -1 && MonthRpt.CompanyProperty1 !="筹备门店")
                                {
                                    companyListCount.Add(c);
                                }
                            }

                            //B表数据的List
                            if (isB_MonthRotCount > 0 && isCompanyCount > 0 && tempCount != i)
                            {
                                //这里排除总部
                                if (c.CompanyName.IndexOf("总部") == -1 && B_MonthRpt.CompanyProperty1 != "筹备门店")
                                {
                                    companyB_ListCount.Add(c);
                                }
                            }

                        }); //End companyList

                        try
                        {
                            //首先获取数据 ， 这里区分A表 和B表的数据 （A表数据） 
                            R_MissTargetEvaluationScope UpdateModel = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(S.ID, t.ID, RTime.ReportTime.Value.Year, i, "NeedEvaluation");

                            if (UpdateModel != null)
                            {
                                //修改
                                if (companyListCount!=null)
                                {
                                    UpdateModel.EvaluationNumber = companyListCount.Count;
                                }
                                else
                                {
                                    UpdateModel.EvaluationNumber = 0;
                                }
                                UpdateModel.MissTargetNumber = MissTargetA;
                                UpdateModel.ModifierName = "Sys";
                                UpdateModel.ModifyTime = DateTime.Now;
                                R_MissTargetEvaluationScopeOperator.Instance.UpdateMissTargetEvaluationScope(UpdateModel);
                            }
                            else
                            {
                                //新增
                                R_MissTargetEvaluationScope ScopeModel = new R_MissTargetEvaluationScope();
                                if (companyListCount != null)
                                {
                                    ScopeModel.EvaluationNumber = companyListCount.Count;
                                }
                                else
                                {
                                    ScopeModel.EvaluationNumber = 0;
                                }
                                ScopeModel.MissTargetNumber = MissTargetA;
                                ScopeModel.FinMonth = i;
                                ScopeModel.FinYear = RTime.ReportTime.Value.Year;
                                ScopeModel.SystemID = S.ID;
                                ScopeModel.TargetID = t.ID;
                                ScopeModel.EvaluationType = "NeedEvaluation";
                                ScopeModel.CreateTime = DateTime.Now;
                                ScopeModel.CreatorName = "Sys";
                                
                                //12个月的，每个月，添加一次
                                R_MissTargetEvaluationScopeOperator.Instance.AddMissTargetEvaluationScope(ScopeModel);
                            }

                            //-----------------------------------------------------------------------------------------------------------------

                            //这里代表B表的数据
                            R_MissTargetEvaluationScope B_UpdateModel = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(S.ID, t.ID, RTime.ReportTime.Value.Year, i, "BNeedEvaluation");

                            if (UpdateModel != null)
                            {
                                //修改
                                if (companyB_ListCount!=null)
                                {
                                    B_UpdateModel.EvaluationNumber = companyB_ListCount.Count;
                                }
                                else
                                {
                                    B_UpdateModel.EvaluationNumber = 0;
                                }
                                B_UpdateModel.MissTargetNumber = MissTargetB;
                                B_UpdateModel.ModifierName = "Sys";
                                B_UpdateModel.ModifyTime = DateTime.Now;
                                R_MissTargetEvaluationScopeOperator.Instance.UpdateMissTargetEvaluationScope(B_UpdateModel);
                            }
                            else
                            {
                                //新增
                                R_MissTargetEvaluationScope ScopeModel = new R_MissTargetEvaluationScope();
                                if (companyB_ListCount != null)
                                {
                                    ScopeModel.EvaluationNumber = companyB_ListCount.Count;
                                }
                                else
                                {
                                    ScopeModel.EvaluationNumber = 0;
                                }
                                ScopeModel.MissTargetNumber = MissTargetB;
                                ScopeModel.FinMonth = i;
                                ScopeModel.FinYear = RTime.ReportTime.Value.Year;
                                ScopeModel.SystemID = S.ID;
                                ScopeModel.TargetID = t.ID;
                                ScopeModel.EvaluationType = "BNeedEvaluation";
                                ScopeModel.CreateTime = DateTime.Now;
                                ScopeModel.CreatorName = "Sys";

                                //12个月的，每个月，添加一次
                                R_MissTargetEvaluationScopeOperator.Instance.AddMissTargetEvaluationScope(ScopeModel);
                            }
                            
                        }
                        catch (Exception Exp)
                        {
                            Common.ScheduleService.Log.Instance.Error("添加错误 TargetID" + t.ID + "SystemID" + S.ID + "FinMonth" + i + "FinYear" + RTime.ReportTime.Value.Year);
                        }

                        #endregion

                    }

                    Common.ScheduleService.Log.Instance.Info("循环系统下的考核指标 End!" + t.TargetName.ToString());
                }); //End targetList

                Common.ScheduleService.Log.Instance.Info("循环系统 End!" + S.SystemName.ToString());
            });// End sysList




            Common.ScheduleService.Log.Instance.Info("计算未完成指标考核范围End!");


        }

    }
}
