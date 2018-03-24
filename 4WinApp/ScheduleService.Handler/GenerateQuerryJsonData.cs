using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.BLL;
using Lib.Config;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Web.Json;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class GenerateQuerryJsonData : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("Service GenerateQuerryJsonData  开始 execute");
            C_ReportTime RptTime = C_ReportTimeOperator.Instance.GetReportTime();

            try
            {
                bool JsonDataSwitch = false;
                bool.TryParse(AppSettingConfig.GetSetting("JsonDataSwitch", ""), out JsonDataSwitch);
                List<B_MonthlyReport> MList = new List<B_MonthlyReport>();

                //开关：true=按照当前上报月 ，查询， 关闭：false=查询所有的数据执行
                if (JsonDataSwitch)
                    MList = B_MonthlyreportOperator.Instance.GetMonthlyReportByApproveList(RptTime.ReportTime.Value.Year, RptTime.ReportTime.Value.Month);
                else
                    MList = B_MonthlyreportOperator.Instance.GetMonthlyreportList().ToList();


                //获取，需要执行的JSon数据 && p.ID == Guid.Parse("114B9692-BC9E-47F0-A6EB-C14450E25C4E")
                var GList = MList.Where(p => string.IsNullOrEmpty(p.DataOptimizationJson)).ToList();

                if (GList != null && GList.Count > 0)
                {
                    GList.ForEach(G =>
                    {
                        try
                        {
                            // 基础信息， 月度报告汇总表
                            ReportInstance rpt = new ReportInstance(G.ID, true, "");
                            
                            List<DictionaryVmodel> listSRDS = ReportInstanceSummaryEngine.ReportInstanceSummaryService.GetSummaryRptDataSource(rpt, false);
                            StringBuilder SumStr = new StringBuilder();
                            SumStr.Append(JsonHelper.Serialize(listSRDS)); // 追加所有的出来的数据

                            string strProp = string.Empty;

                            //判断下，直管公司里是没有属性的
                            if (rpt._System.Category != 4)
                            {
                                //公司的属性List
                                var VpropList = (List<VCompanyProperty>)listSRDS[4].ObjValue;
                                VpropList.ForEach(vp =>
                                {
                                    strProp = strProp + vp.ColumnName + ":";
                                    if (vp.listCP != null && vp.listCP.Count > 0)
                                    {
                                        vp.listCP.ForEach(v =>
                                        {
                                            strProp = strProp + v.ItemCompanyPropertyValue + ",";
                                        });
                                    }
                                    strProp = strProp + ";";
                                });
                            }
                          
                            //月度明细数据
                            ReportInstance rpt_Detail = new ReportInstance(G.ID, true, "");

                            List<DictionaryVmodel> DetailData = ReportInstanceDetailEngine.ReportInstanceDetailService.GetDetailRptDataSource(rpt_Detail, strProp, "Detail", false);
                            StringBuilder DetailStr = new StringBuilder();
                            DetailStr.Append(JsonHelper.Serialize(DetailData)); // 追加所有的出来的数据

                            //累计未完成
                            ReportInstance rpt_MIss = new ReportInstance(G.ID, true, "");
                            List<DictionaryVmodel> MIssData = ReportInstanceMissTargetEngine.ReportInstanceMissTargetService.GetMissTargetRptDataSource(rpt_MIss);
                            StringBuilder MIssStr = new StringBuilder();
                            MIssStr.Append(JsonHelper.Serialize(MIssData)); // 追加所有的出来的数据


                            //当月未完成
                            ReportInstance rpt_CurrenMIss = new ReportInstance(G.ID, true, "");
                            List<DictionaryVmodel> CurrentMIssData = ReportInstanceCurrentMissTargetEngine.ReportInstanceMissTargetService.GetCurrentMissTargetRptDataSource(rpt_CurrenMIss);
                            StringBuilder CurrentMIssStr = new StringBuilder();
                            CurrentMIssStr.Append(JsonHelper.Serialize(CurrentMIssData)); // 追加所有的出来的数据


                            //补回情况
                            C_System sys;
                            if (rpt.ReportDetails != null && rpt.ReportDetails.Count() > 0)
                            {
                                sys = StaticResource.Instance[rpt._System.ID, rpt.ReportDetails[0].CreateTime];
                            }
                            else
                            {
                                sys = StaticResource.Instance[rpt._System.ID, DateTime.Now];
                            }
                            ReportInstance rpt_Return = new ReportInstance(G.ID, true, "");
                            List<DictionaryVmodel> ReturnData = ReportInstanceReturnEngine.ReportInstanceReturnService.GetReturnRptDataSource(rpt_Return, sys); //rpt.GetReturnRptDataSource();
                            StringBuilder ReturnStr = new StringBuilder();
                            ReturnStr.Append(JsonHelper.Serialize(ReturnData)); // 追加所有的出来的数据

                            // 序列化的Json数据保存
                            B_MonthlyReportJsonData JsonData = new B_MonthlyReportJsonData();
                            try
                            {// 获取Json数据
                                JsonData = B_MonthlyReportJsonDataOperator.Instance.GetMonthlyReportJsonData(G.ID);
                            }
                            catch (Exception)
                            {
                                JsonData = null;
                            }

                            if (JsonData != null)
                            {
                                JsonData.ModifierName = "SysService";
                                JsonData.ModifyTime = DateTime.Now;
                                JsonData.QuerrySumJsonData = SumStr.ToString();
                                JsonData.QuerryDetaileJsonData = DetailStr.ToString();
                                JsonData.QuerryMissJsonData = MIssStr.ToString();
                                JsonData.QuerryReturnJsonData = ReturnStr.ToString();
                                JsonData.QuerryCurrentMissJsonData = CurrentMIssStr.ToString();
                                B_MonthlyReportJsonDataOperator.Instance.UpdateMonthlyReportJsonData(JsonData);

                                G.DataOptimizationJson = "100";
                                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(G);

                            }
                            else
                            {
                                JsonData = new B_MonthlyReportJsonData();
                                JsonData.ID = G.ID;
                                JsonData.FinMonth = rpt.FinMonth;
                                JsonData.FinYear = rpt.FinYear;
                                JsonData.PlanType = "M";
                                JsonData.SystemID = rpt._SystemID;
                                JsonData.ModifierName = "SysService";
                                JsonData.ModifyTime = DateTime.Now;
                                JsonData.QuerrySumJsonData = SumStr.ToString();
                                JsonData.QuerryDetaileJsonData = DetailStr.ToString();
                                JsonData.QuerryMissJsonData = MIssStr.ToString();
                                JsonData.QuerryReturnJsonData = ReturnStr.ToString();
                                JsonData.QuerryCurrentMissJsonData = CurrentMIssStr.ToString();
                                B_MonthlyReportJsonDataOperator.Instance.AddMonthlyReportJsonData(JsonData);

                                G.DataOptimizationJson = "100";
                                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(G);
                            }

                        }
                        catch (Exception ep)
                        {
                            G.DataOptimizationJson = "-100";
                            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(G);


                            Common.ScheduleService.Log.Instance.Error("Service GenerateQueryAttachments  错误：" + ep.ToString());
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                Common.ScheduleService.Log.Instance.Error("Service GenerateQueryAttachments  错误：" + ex.ToString());
            }

            Common.ScheduleService.Log.Instance.Info("Service GenerateQuerryJsonData  结束 execute");

        }
    }
}
