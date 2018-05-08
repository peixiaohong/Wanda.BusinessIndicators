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
    public class DefaultCalculationEvation : ICalculationEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.NewCounter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);

            //成本类指标
            bool CostTarget = false;
            if (T.TargetType == 3) CostTarget = true;

            bool IsBaseline = false;
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            #region //是否能够改动计划，如果能改，取Excel数据，不能改取系统计划数(在Taget Configuration配置)
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
            #endregion


            //这里获取上月的实际上报数，和年计划指标的计划数
            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);

            if (lastMonthData != null)
            {
                //实际上报数
                rpt.NAccumulativeActualAmmount = lastMonthData.NAccumulativeActualAmmount + rpt.NActualAmmount;
                rpt.OAccumulativeActualAmmount = lastMonthData.OAccumulativeActualAmmount + rpt.OActualAmmount;

                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    rpt.CommitDate = lastMonthData.CurrentMonthCommitDate;
                    rpt.CommitReason = lastMonthData.CurrentMonthCommitReason;
                }

                //这里总是从最新的指标计划获取
                IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);

            }
            else
            {
                //当没有上月数据的时候
                if (WithCounter)
                {

                    //这里总是从最新的指标计划获取
                    IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                    rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target); //计划值从这里取到
                    rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);

                    rpt.NAccumulativeActualAmmount = rpt.NActualAmmount;
                    rpt.OAccumulativeActualAmmount = rpt.OActualAmmount;

                }
            }

            rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
            rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;
            rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
            rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;

            rpt = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail, "");

            if (T.NeedEvaluation)
            {

                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况

                //上月累计未完成

                if (!IsBaseline)  //从1月开始重新计算
                {
                    #region 是否有上月数据完成，或者是没有上月数据

                    if (lastMonthData != null && lastMonthData.NewCounter != 0)
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.Returning;

                                // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                {
                                    rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                } // Update 2015-5-29
                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.NotReturn;
                            }
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = lastMonthData.NewCounter + 1;
                                rpt.NewCounter = lastMonthData.NewCounter + 1;
                            }
                        }
                        else
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
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
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;
                            }
                        }

                        rpt.IsCommitDate = 0;

                    }
                    #endregion
                    //无上月数据(1月)或上月累计完成
                    else
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                //无上月数据(1月)或上月累计完成
                //重新计算
                else
                {
                    if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                    {
                        rpt.ReturnType = (int)EnumReturnType.New;
                        if (rpt.IsMissTarget && WithCounter)
                        {
                            rpt.Counter = 1;
                            rpt.NewCounter = 1;
                            rpt.FirstMissTargetDate = ReportDate;
                        }
                    }
                }
            }
            else
            {
                rpt.IsMissTargetCurrent = false;
                rpt.IsMissTarget = false;
            }

            //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
            RptDetail.Display = true;
            //if (RptDetail.NActualAmmount == 0 && RptDetail.NPlanAmmount == 0 && RptDetail.NAccumulativePlanAmmount == 0 && RptDetail.NAccumulativeActualAmmount == 0)
            //{
            //    RptDetail.Display = false;
            //}
            //判断异常指标
            rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");

            return rpt;
        }

    }


    /// <summary>
    /// 经营系统特殊处理（商管系统）
    /// </summary>
    public class CalculationEvation_SG : ICalculationEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.NewCounter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
            //成本类指标
            bool CostTarget = false;
            if (T.TargetType == 3) CostTarget = true;

            bool IsBaseline = false;
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);
            if (lastMonthData != null)
            {
                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    rpt.CommitDate = lastMonthData.CurrentMonthCommitDate;
                    rpt.CommitReason = lastMonthData.CurrentMonthCommitReason;
                }
            }


            //特殊处理差额，针对指标
            XElement element = null;
            element = T.Configuration;
            XElement subElement = null;

            bool IsDifferenceException = false;

            if (element.Elements("IsDifferenceExceptionTarget").ToList().Count > 0)
            {
                subElement = element.Elements("IsDifferenceExceptionTarget").ToList()[0];
                IsDifferenceException = subElement.GetAttributeValue("value", false);
            }


            rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
            rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;
            rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
            rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;


            if (IsDifferenceException) //异常
            {
                if (rpt.NAccumulativeDifference > 0) rpt.NAccumulativeDifference = 0;

                if (rpt.OAccumulativeDifference > 0) rpt.OAccumulativeDifference = 0;

                if (rpt.NDifference > 0) rpt.NDifference = 0;

                if (rpt.ODifference > 0) rpt.ODifference = 0;
            }

            rpt = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail, "");

            if (T.NeedEvaluation)
            {
                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况
                //上月累计未完成
                if (!IsBaseline)  //从1月开始重新计算
                {
                    if (lastMonthData != null && (lastMonthData.IsMissTarget || (!CostTarget && lastMonthData.NAccumulativeDifference < 0)))
                    {
                        #region  检测出含有上月数据

                        bool IsDelayComplete = false;
                        if (lastMonthData.IsDelayComplete == true)
                            IsDelayComplete = lastMonthData.IsDelayComplete;


                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            if (IsDelayComplete) //
                            {
                                #region 特殊处理 延迟完成

                                rpt.ReturnType = (int)EnumReturnType.New;

                                if (rpt.IsMissTarget && WithCounter)
                                {
                                    rpt.NewCounter = 1;
                                    rpt.Counter = 1;
                                }
                                #endregion
                            }
                            else
                            {
                                #region 正常处理

                                if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                                {
                                    rpt.ReturnType = (int)EnumReturnType.Returning;

                                    // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                    if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                    {
                                        rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                    } // Update 2015-5-29
                                }
                                else
                                {
                                    rpt.ReturnType = (int)EnumReturnType.NotReturn;
                                }
                                if (rpt.IsMissTarget && WithCounter)
                                {
                                    rpt.Counter = lastMonthData.NewCounter + 1;
                                    rpt.NewCounter = lastMonthData.NewCounter + 1;
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            if (IsDelayComplete)
                            {
                                #region 特殊处理 延迟完成

                                rpt.Counter = 0;
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;

                                #endregion
                            }
                            else
                            {
                                #region 正常处理

                                if (lastMonthData.CurrentMonthCommitDate > ReportDate)
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
                                    rpt.NewCounter = 0;
                                    rpt.FirstMissTargetDate = null;
                                }

                                #endregion
                            }
                        }

                        rpt.IsCommitDate = 0;

                        #endregion
                    }
                    else
                    {
                        #region 无上月数据(1月)或上月累计完成

                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }

                        #endregion
                    }
                }
                else
                {
                    #region  重新计算（指定任意月为起始月份）

                    if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                    {
                        rpt.ReturnType = (int)EnumReturnType.New;
                        if (rpt.IsMissTarget && WithCounter)
                        {
                            rpt.Counter = 1;
                            rpt.NewCounter = 1;
                            rpt.FirstMissTargetDate = ReportDate;
                        }
                    }

                    #endregion
                }
            }
            else
            {
                rpt.IsMissTargetCurrent = false;
                rpt.IsMissTarget = false;
            }

            //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
            RptDetail.Display = true;
            //if (RptDetail.NActualAmmount == 0 && RptDetail.NPlanAmmount == 0 && RptDetail.NAccumulativePlanAmmount == 0 && RptDetail.NAccumulativeActualAmmount == 0)
            //{
            //    RptDetail.Display = false;
            //}

            //判断异常指标
            rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");
            return rpt;
        }
    }

    /// <summary>
    /// 经营系统特殊处理（物管系统）
    /// </summary>
    public class CalculationEvation_WG : ICalculationEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.NewCounter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
            //成本类指标
            bool CostTarget = false;
            if (T.TargetType == 3) CostTarget = true;

            bool IsBaseline = false;
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            #region (已废除)//是否能够改动计划，如果能改，取Excel数据，不能改取系统计划数(在Taget Configuration配置)
            //List<A_TargetPlanDetail> listCurrentMonthData = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).ToList().Where(p => p.FinMonth == rpt.FinMonth && p.CompanyID == rpt.CompanyID).ToList();
            //if (listCurrentMonthData.Count > 0)
            //{
            //    A_TargetPlanDetail currentMonthData = listCurrentMonthData[0];
            //    bool IsModifiy = false;
            //    if (currentMonthData.Target != RptDetail.OPlanAmmount)
            //    {
            //        //获取当前指标
            //        List<C_Target> listTarget = StaticResource.Instance.GetTargetList(RptDetail.SystemID).ToList().Where(p => p.ID == RptDetail.TargetID).ToList();
            //        if (listTarget.Count > 0)
            //        {
            //            //获取当前指标的Configuration
            //            IList<System.Xml.Linq.XElement> xmlConfiguration = listTarget[0].Configuration.Elements("IsModifyTargetPlanDetail").ToList();
            //            if (xmlConfiguration.Count > 0)
            //            {
            //                //判断当前指标是否可以修改
            //                if (xmlConfiguration.ToList()[0].GetAttributeValue("IsModifiy", "").ToLower() == "True")
            //                {
            //                    IsModifiy = true;
            //                }
            //            }
            //        }

            //    }
            //    //如果IsModifiy为True，则计划数可以修改，否则反之。
            //    if (IsModifiy == false)
            //    {
            //        RptDetail.OPlanAmmount = currentMonthData.Target;
            //    }
            //}
            #endregion

            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);
            if (lastMonthData != null)
            {
                //rpt.NAccumulativeActualAmmount = lastMonthData.NAccumulativeActualAmmount + rpt.NActualAmmount;
                //rpt.OAccumulativeActualAmmount = lastMonthData.OAccumulativeActualAmmount + rpt.OActualAmmount;
                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    rpt.CommitDate = lastMonthData.CurrentMonthCommitDate;
                    rpt.CommitReason = lastMonthData.CurrentMonthCommitReason;
                }

                IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
            }
            else
            {
                if (WithCounter)
                {
                    //这里总是从最新的指标计划获取
                    IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                    rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target); //计划值从这里取到
                    rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);

                    rpt.NAccumulativeActualAmmount = rpt.NActualAmmount;
                    rpt.OAccumulativeActualAmmount = rpt.OActualAmmount;

                }
            }
            rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
            rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;
            rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
            rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;

            rpt = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail, "");

            if (T.NeedEvaluation)
            {

                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况

                //上月累计未完成
                if (!IsBaseline)  //从1月开始重新计算
                {
                    if (lastMonthData != null && (lastMonthData.IsMissTarget || (!CostTarget && lastMonthData.NAccumulativeDifference < 0)))
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.Returning;

                                // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                {
                                    rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                } // Update 2015-5-29
                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.NotReturn;
                            }
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = lastMonthData.NewCounter + 1;
                                rpt.NewCounter = lastMonthData.NewCounter + 1;
                            }
                        }
                        else
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
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
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;
                            }
                        }

                        rpt.IsCommitDate = 0;

                    }
                    //无上月数据(1月)或上月累计完成
                    else
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                //无上月数据(1月)或上月累计完成
                //重新计算
                else
                {
                    if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                    {
                        rpt.ReturnType = (int)EnumReturnType.New;
                        if (rpt.IsMissTarget && WithCounter)
                        {
                            rpt.Counter = 1;
                            rpt.NewCounter = 1;
                            rpt.FirstMissTargetDate = ReportDate;
                        }
                    }
                }
            }
            else
            {
                rpt.IsMissTargetCurrent = false;
                rpt.IsMissTarget = false;
            }

            //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
            RptDetail.Display = true;
            //if (RptDetail.NActualAmmount == 0 && RptDetail.NPlanAmmount == 0 && RptDetail.NAccumulativePlanAmmount == 0 && RptDetail.NAccumulativeActualAmmount == 0)
            //{
            //    RptDetail.Display = false;
            //}
            //判断异常指标
            rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");

            return rpt;
        }
    }


    /// <summary>
    /// 项目公司的计算
    /// </summary>
    public class ProCalculationEvation : ICalculationEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.NewCounter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
            //成本类指标
            bool CostTarget = false;
            if (T.TargetType == 3) CostTarget = true;

            bool IsBaseline = false;
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            #region 是否能够改动计划，如果能改，取Excel数据，不能改取系统计划数(在Taget Configuration配置)

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

            #endregion

            #region 计算本月累计实际数，这里有两种情况：1 本月 + 上月累计  2 直接从Excel中读取得到的本月累计数


            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);

            if (lastMonthData != null)
            {
                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    //如果中途指标更换, 则重新计算更换后的指标
                    rpt.CommitDate = lastMonthData.CurrentMonthCommitDate;
                    rpt.CommitReason = lastMonthData.CurrentMonthCommitReason;
                }

                //如果中途指标更换, 则重新计算更换后的指标
                IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
            }

            rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
            rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;
            rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
            rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;

            #endregion

            rpt = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail, "");

            #region 指标的补回情况和考核值设置

            if (T.NeedEvaluation)
            {

                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况

                //上月累计未完成

                if (!IsBaseline)  //从1月开始重新计算
                {
                    if (lastMonthData != null && lastMonthData.NewCounter != 0)
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.Returning;

                                // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                {
                                    rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                }

                                // Update 2015-5-29

                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.NotReturn;
                            }
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = lastMonthData.NewCounter + 1;
                                rpt.NewCounter = lastMonthData.NewCounter + 1;
                            }
                        }
                        else
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
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
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;
                            }
                        }

                        rpt.IsCommitDate = 0;

                    }
                    //无上月数据(1月)或上月累计完成
                    else
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                //无上月数据(1月)或上月累计完成
                //重新计算
                else
                {
                    if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                    {
                        rpt.ReturnType = (int)EnumReturnType.New;
                        if (rpt.IsMissTarget && WithCounter)
                        {
                            rpt.Counter = 1;
                            rpt.NewCounter = 1;
                            rpt.FirstMissTargetDate = ReportDate;
                        }
                    }
                }
            }
            else //如果指标不考核，那么就不计算未完成
            {
                rpt.IsMissTarget = false;
                rpt.IsMissTargetCurrent = false;
            }


            //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
            RptDetail.Display = true;
            //if (RptDetail.NActualAmmount == 0 && RptDetail.NPlanAmmount == 0 && RptDetail.NAccumulativePlanAmmount == 0 && RptDetail.NAccumulativeActualAmmount == 0)
            //{
            //    RptDetail.Display = false;
            //}

            #endregion

            //判断异常指标
            rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");

            return rpt;
        }
    }



    /// <summary>
    /// 集团总部的计算
    /// </summary>
    public class GroupCalculationEvation : ICalculationEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.NewCounter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
            //成本类指标
            bool CostTarget = false;
            if (T.TargetType == 3) CostTarget = true;

            bool IsBaseline = false;
            bool IsModifiy = false;
            IList<System.Xml.Linq.XElement> xmlConfiguration = T.Configuration.Elements("IsModifyTargetPlanDetail").ToList();
            if (xmlConfiguration.Count > 0)
            {
                //判断当前指标是否可以修改
                if (xmlConfiguration.ToList()[0].GetAttributeValue("IsModify", "") == "True")
                {
                    IsModifiy = true;
                }
            }
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            #region 是否能够改动计划，如果能改，取Excel数据，不能改取系统计划数(在Taget Configuration配置)

            List<A_TargetPlanDetail> listCurrentMonthData = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).ToList().Where(p => p.FinMonth == rpt.FinMonth && p.CompanyID == rpt.CompanyID).ToList();
            if (listCurrentMonthData.Count > 0)
            {
                A_TargetPlanDetail currentMonthData = listCurrentMonthData[0];
                //如果IsModifiy为True，则计划数可以修改，否则反之。
                if (IsModifiy == false)
                {
                    RptDetail.OPlanAmmount = currentMonthData.Target;
                }
            }

            #endregion

            #region 本月累计实际数(集团总部实际数直接从Excel中获取）


            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);

            if (lastMonthData != null)
            {
                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    rpt.CommitDate = lastMonthData.CurrentMonthCommitDate; //获取要求补回时间
                    rpt.CommitReason = lastMonthData.CurrentMonthCommitReason;
                }
                else
                {
                    //如果中途指标更换, 则重新计算更换后的指标
                    if (IsModifiy == false)
                    {
                        IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                        rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                        rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                    }
                }
            }

            rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
            rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;
            rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
            rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;

            #endregion

            rpt = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail, "");

            #region 指标的补回情况和考核值设置

            if (T.NeedEvaluation)
            {

                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况

                //上月累计未完成

                if (!IsBaseline)  //从1月开始重新计算
                {
                    if (lastMonthData != null && lastMonthData.NewCounter != 0)
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.Returning;

                                // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                {
                                    rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                } // Update 2015-5-29
                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.NotReturn;
                            }
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = lastMonthData.Counter + 1;
                                rpt.NewCounter = lastMonthData.NewCounter + 1;
                            }
                        }
                        else
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
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
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;
                            }
                        }

                        rpt.IsCommitDate = 0;

                    }
                    //无上月数据(1月)或上月累计完成
                    else
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                //无上月数据(1月)或上月累计完成
                //重新计算
                else
                {
                    if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                    {
                        rpt.ReturnType = (int)EnumReturnType.New;
                        if (rpt.IsMissTarget && WithCounter)
                        {
                            rpt.Counter = 1;
                            rpt.NewCounter = 1;
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

            //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
            RptDetail.Display = true;
            //if (RptDetail.NActualAmmount == 0 && RptDetail.NPlanAmmount == 0 && RptDetail.NAccumulativePlanAmmount == 0 && RptDetail.NAccumulativeActualAmmount == 0)
            //{
            //    RptDetail.Display = false;
            //}

            #endregion

            //判断异常指标
            rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");

            return rpt;
        }
    }


    /// <summary>
    /// 直管公司的计算
    /// </summary>
    public class DirectlyCalculationEvation : ICalculationEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            B_MonthlyReportDetail rpt = RptDetail;
            rpt.ReturnType = 0;
            rpt.Counter = 0;
            rpt.NewCounter = 0;
            rpt.IsMissTarget = false;
            rpt.IsMissTargetCurrent = false;

            C_Target T = StaticResource.Instance.TargetList[RptDetail.SystemID].Find(t => t.ID == RptDetail.TargetID);
            //成本类指标
            bool CostTarget = false;
            if (T.TargetType == 3) CostTarget = true;

            bool IsBaseline = false;
            if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
            {
                IsBaseline = true;
            }

            #region //是否能够改动计划，如果能改，取Excel数据，不能改取系统计划数(在Taget Configuration配置)
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
            #endregion

            A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(rpt.SystemID, rpt.CompanyID, rpt.TargetID, rpt.FinYear, rpt.FinMonth - 1);
            if (lastMonthData != null)
            {
                rpt.NAccumulativeActualAmmount = lastMonthData.NAccumulativeActualAmmount + rpt.NActualAmmount;
                rpt.OAccumulativeActualAmmount = lastMonthData.OAccumulativeActualAmmount + rpt.OActualAmmount;
                //重新考核需要特殊处理，直接获取累计指标，无承诺补回时间
                if (!IsBaseline)
                {
                    //rpt.NAccumulativePlanAmmount = lastMonthData.NAccumulativePlanAmmount + rpt.NPlanAmmount;
                    //rpt.OAccumulativePlanAmmount = lastMonthData.OAccumulativePlanAmmount + rpt.OPlanAmmount;

                    IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                    rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target);
                    rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);

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

                    //这里总是从最新的指标计划获取
                    IList<A_TargetPlanDetail> ATPD = StaticResource.Instance.GetTargetPlanList(rpt.SystemID, rpt.FinYear).FindAll(P => P.CompanyID == rpt.CompanyID && P.TargetID == rpt.TargetID && P.FinMonth <= rpt.FinMonth);
                    rpt.NAccumulativePlanAmmount = ATPD.Sum(p => p.Target); //计划值从这里取到
                    rpt.OAccumulativePlanAmmount = ATPD.Sum(p => p.Target);

                    rpt.NAccumulativeActualAmmount = rpt.NActualAmmount;
                    rpt.OAccumulativeActualAmmount = rpt.OActualAmmount;
                }
            }
            rpt.NAccumulativeDifference = rpt.NAccumulativeActualAmmount - rpt.NAccumulativePlanAmmount;
            rpt.OAccumulativeDifference = rpt.OAccumulativeActualAmmount - rpt.OAccumulativePlanAmmount;
            rpt.NDifference = rpt.NActualAmmount - rpt.NPlanAmmount;
            rpt.ODifference = rpt.OActualAmmount - rpt.OPlanAmmount;

            rpt = (B_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail, "");

            if (T.NeedEvaluation)
            {

                DateTime ReportDate = DateTime.MinValue;
                DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                //累计未完成情况

                //上月累计未完成

                if (!IsBaseline)  //从1月开始重新计算
                {
                    if (lastMonthData != null && (lastMonthData.IsMissTarget))
                    {
                        if (rpt.IsMissTarget)
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                            {
                                rpt.ReturnType = (int)EnumReturnType.Returning;

                                // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                {
                                    rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                } // Update 2015-5-29

                            }
                            else
                            {
                                rpt.ReturnType = (int)EnumReturnType.NotReturn;
                            }
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = lastMonthData.NewCounter + 1;
                                rpt.NewCounter = lastMonthData.NewCounter + 1;
                            }
                        }
                        else
                        {
                            if (lastMonthData.CurrentMonthCommitDate > ReportDate)
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
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;
                            }
                        }

                        rpt.IsCommitDate = 0;

                    }
                    //无上月数据(1月)或上月累计完成
                    else
                    {
                        if (rpt.IsMissTarget)
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget && WithCounter)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                //无上月数据(1月)或上月累计完成
                //重新计算
                else
                {
                    if (rpt.IsMissTarget)
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
                rpt.IsMissTargetCurrent = false;
                rpt.IsMissTarget = false;
            }

            //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
            RptDetail.Display = true;
            //if (RptDetail.NActualAmmount == 0 && RptDetail.NPlanAmmount == 0 && RptDetail.NAccumulativePlanAmmount == 0 && RptDetail.NAccumulativeActualAmmount == 0)
            //{
            //    RptDetail.Display = false;
            //}
            //判断异常指标
            rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");

            return rpt;
        }
    }



    /// <summary>
    /// 重新计算A表的数据
    /// </summary>
    public class ResetCalculationEvation : IResetCalculationEvaluation
    {

        /// <summary>
        /// 重新计算A表的数据
        /// </summary>
        /// <param name="RptDetailList">List传入时，需要按年月排序，从1月开始 </param>
        /// <returns></returns>
        public List<A_MonthlyReportDetail> ResetCalculation(List<A_MonthlyReportDetail> RptDetailList, List<A_MonthlyReportDetail> AllRptDetailList)
        {

            RptDetailList.ForEach(p =>
            {

                A_MonthlyReportDetail rpt = p;
                rpt.ReturnType = 0;
                rpt.Counter = 0;
                rpt.NewCounter = 0;
                rpt.IsMissTarget = false;
                rpt.IsMissTargetCurrent = false;

                C_Target T = StaticResource.Instance.TargetList[p.SystemID].Find(t => t.ID == p.TargetID);

                //成本类指标
                bool CostTarget = false;
                if (T.TargetType == 3) CostTarget = true;

                bool IsBaseline = false;
                if (T.BaseLine.Year == rpt.FinYear && T.BaseLine.Month == rpt.FinMonth)
                {
                    IsBaseline = true;
                }

                // 这里做个简单的计算指标，只计算是否IsMissTarget
                rpt = (A_MonthlyReportDetail)TargetEvaluationEngine.TargetEvaluationService.Calculation(rpt, "");


                if (T.NeedEvaluation)
                {
                    DateTime ReportDate = DateTime.MinValue;
                    DateTime.TryParse(rpt.FinYear + "-" + rpt.FinMonth + "-1 00:00:00", out ReportDate);
                    ReportDate = ReportDate.AddMonths(1).AddMinutes(-5);
                    //累计未完成情况

                    //上月累计未完成
                    A_MonthlyReportDetail lastMonthData = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetail(p.SystemID, p.CompanyID, p.TargetID, p.FinYear, p.FinMonth - 1);

                    if (!IsBaseline)  //从1月开始重新计算
                    {
                        #region 上月累计未完成计算


                        if (lastMonthData != null)
                        {
                            if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                            {
                                if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                                {
                                    rpt.ReturnType = (int)EnumReturnType.Returning;

                                    // Update 2015-5-29 当是年内无法补回时，每个月自动将上月的说明带入
                                    if (lastMonthData.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(EnumReturnType_Sub), (int)EnumReturnType_Sub.Sub_UnableReturnByYear))
                                    {
                                        rpt.ReturnDescription = lastMonthData.ReturnDescription;
                                    } // Update 2015-5-29
                                }
                                else
                                {
                                    rpt.ReturnType = (int)EnumReturnType.NotReturn;
                                }
                                if (rpt.IsMissTarget)
                                {
                                    rpt.Counter = lastMonthData.NewCounter + 1;
                                    rpt.NewCounter = lastMonthData.NewCounter + 1;
                                }
                            }
                            else
                            {
                                if (lastMonthData.CurrentMonthCommitDate > ReportDate)
                                {
                                    rpt.ReturnType = (int)EnumReturnType.AccomplishInadvance;
                                }
                                else
                                {
                                    rpt.ReturnType = (int)EnumReturnType.Accomplish;
                                }

                                rpt.Counter = 0;
                                rpt.NewCounter = 0;
                                rpt.FirstMissTargetDate = null;
                            }

                            rpt.IsCommitDate = 0;

                        }
                        //无上月数据(1月)或上月累计完成
                        else
                        {
                            if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                            {
                                rpt.ReturnType = (int)EnumReturnType.New;
                                if (rpt.IsMissTarget)
                                {
                                    rpt.Counter = 1;
                                    rpt.NewCounter = 1;
                                    rpt.FirstMissTargetDate = ReportDate;
                                }
                            }
                        }

                        #endregion
                    }
                    //无上月数据(1月)或上月累计完成
                    //重新计算
                    else
                    {
                        if (rpt.IsMissTarget || (!CostTarget && rpt.NAccumulativeDifference < 0))
                        {
                            rpt.ReturnType = (int)EnumReturnType.New;
                            if (rpt.IsMissTarget)
                            {
                                rpt.Counter = 1;
                                rpt.NewCounter = 1;
                                rpt.FirstMissTargetDate = ReportDate;
                            }
                        }
                    }
                }
                else
                {
                    rpt.IsMissTargetCurrent = false;
                    rpt.IsMissTarget = false;
                }

                //如果计划数和实际数都为0，则页面不显示。（display控制页面是否显示当前数据）
                p.Display = true;
                if (p.NActualAmmount == 0 && p.NPlanAmmount == 0 && p.NAccumulativePlanAmmount == 0 && p.NAccumulativeActualAmmount == 0)
                {
                    p.Display = false;
                }
                //判断异常指标
                //rpt = ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(rpt, "");


            });

            return RptDetailList;
        }

    }


}
