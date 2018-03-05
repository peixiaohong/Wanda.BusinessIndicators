using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using System.Configuration;
using Wanda.BusinessIndicators.Common;
using Lib.Core;

namespace Wanda.BusinessIndicators.Engine
{

    /// <summary>
    /// 默认获取月报说明
    /// </summary>
    public class DefaultGetMonthDescriptionValue : IGetMonthDescriptionValue
    {
        public Hashtable GetMonthDescriptionValue(object Vmodel)
        {
            List<MonthlyReportDetail> RptList = (List<MonthlyReportDetail>)Vmodel;

            if (RptList == null || RptList.Count <= 0) return new Hashtable();
            Hashtable hastable = new Hashtable();

            DateTime ReportDate = DateTime.MinValue;

            if (RptList[0].FinMonth == 12)
            {
                DateTime.Parse(Convert.ToInt32(RptList[0].FinYear + 1) + "-01-01").AddDays(-1);
            }
            else
            {
                ReportDate = DateTime.Parse(RptList[0].FinYear + "-" + Convert.ToInt32(RptList[0].FinMonth + 1) + "-01").AddDays(-1);
            }

            DateTime CurrentDate = DateTime.Now;
            C_System sysModel = StaticResource.Instance[RptList[0].SystemID, CurrentDate];

            List<C_Target> targetList = StaticResource.Instance.TargetList[RptList[0].SystemID];

            List<C_Company> compayList = StaticResource.Instance.CompanyList[RptList[0].SystemID];

            List<VCompanyProperty> compayProList = new List<VCompanyProperty>();

            GetMonthDescriptionTool tools = new GetMonthDescriptionTool();

            //百货特殊处理
            if (StaticResource.Instance[RptList[0].SystemID, CurrentDate].ID == System.Configuration.ConfigurationManager.AppSettings["MonthDescription"].ToGuid())
            {
                VCompanyProperty areaModel = new VCompanyProperty();

                XElement element = StaticResource.Instance[RptList[0].SystemID, CurrentDate].Configuration;

                if (element.Elements("ListCompanyProperty").Elements("CompanyProperty") != null)
                {
                    List<XElement> companyPro = element.Elements("ListCompanyProperty").Elements("CompanyProperty").ToList();
                    foreach (XElement pro in companyPro)
                    {
                        compayProList.Add(new VCompanyProperty(pro));
                    }
                    areaModel = compayProList.Where(p => p.CompanyPropertyName == "大区").FirstOrDefault();
                }

                if (areaModel != null)
                {   //循环每个大区
                    foreach (var item in areaModel.listCP)
                    {  //查询大区的相关数据
                        var js = from r in RptList
                                 join co in compayList
                                 on r.CompanyID equals co.ID
                                 where co.CompanyProperty3 == item.ItemCompanyPropertyName
                                 select r;
                        List<MonthlyReportDetail> AreaList = js.ToList();

                        //纯粹只计算各个大区的百货系统
                        tools.ForTaeget(targetList, AreaList, item.ItemCompanyPropertyName, ref hastable);
                    }
                }
                //计算不是大区的值
                tools.ForTaeget(targetList, RptList, ref hastable);
            }
            else
            {
                tools.ForTaeget(targetList, RptList, ref hastable);

                //判断是否是大歌星
                if (StaticResource.Instance[RptList[0].SystemID, CurrentDate].ID == Guid.Parse("BD955BA9-0648-4BB7-A229-43C70F2498E3"))
                {
                    tools.ForCompay(compayList, RptList, ref hastable);
                }
            }

            hastable.Add("当前月", RptList[0].FinMonth);

            return hastable;
        }

    }

    /// <summary>
    /// 公用的方法
    /// </summary>
    public class GetMonthDescriptionTool
    {

        /// <summary>
        /// 循环系统下的指标
        /// </summary>
        /// <param name="targetList">指标列表</param>
        /// <param name="RptList">明细列表</param>
        /// <param name="hastable"></param>
        /// <returns></returns>
        public Hashtable ForTaeget(List<C_Target> targetList, List<MonthlyReportDetail> RptList, ref Hashtable hastable)
        {
            //每个系统相对应的指标

            MonthReportSummaryViewModel monthRptSum = null;
            int i = 0;
            foreach (var item in targetList.OrderBy(o => o.Sequence))
            {
                if (item.NeedReport == true) //指标必须为上报
                {
                    monthRptSum = new MonthReportSummaryViewModel();
                    monthRptSum.ID = i;

                    monthRptSum.FinYear = RptList[0].FinYear;
                    monthRptSum.TargetID = item.ID;
                    monthRptSum.SystemID = item.SystemID;
                    monthRptSum.TargetName = item.TargetName;

                    monthRptSum.NPlanAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NPlanAmmount));
                    monthRptSum.NActualAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NActualAmmount));
                    monthRptSum.NAccumulativePlanAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NAccumulativePlanAmmount));
                    monthRptSum.NAccumulativeActualAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NAccumulativeActualAmmount));
                    monthRptSum.MeasureRate ="0"; // 这个值是为了计算当月或者是累计完成率,如果想计算全年的,请将该字段的值赋值成全年的完成率
                    monthRptSum = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(monthRptSum);

                    if (hastable.ContainsKey(item.TargetName + ".当月计划总数") == false)
                        hastable.Add(item.TargetName + ".当月计划总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NPlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月计划总数"] = ((int)Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NPlanAmmount), 0, MidpointRounding.AwayFromZero)).ToString("N0");


                    if (hastable.ContainsKey(item.TargetName + ".当月实际总数") == false)
                        hastable.Add(item.TargetName + ".当月实际总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount),0,MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月实际总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumActualAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount);
                    decimal SumPlanAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount);
                    if (SumPlanAmmount == 0) SumPlanAmmount = 1;

                    if (hastable.ContainsKey(item.TargetName + ".当月累计完成率") == false)
                        hastable.Add(item.TargetName + ".当月累计完成率", monthRptSum.NActualRate);
                    else
                        hastable[item.TargetName + ".当月累计完成率"] = monthRptSum.NActualRate;

                    if (hastable.ContainsKey(item.TargetName + ".当月未完成公司数量") == false)
                        hastable.Add(item.TargetName + ".当月未完成公司数量", RptList.Where(p => p.TargetID == item.ID && p.IsMissTargetCurrent == true).ToList().Count);
                    else
                        hastable[item.TargetName + ".当月未完成公司数量"] = RptList.Where(p => p.TargetID == item.ID && p.IsMissTargetCurrent == true).ToList().Count;

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(item.TargetName + ".当月亏损总数") == false)
                        hastable.Add(item.TargetName + ".当月亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(item.TargetName + ".当月有差额公司数量") == false)
                        hastable.Add(item.TargetName + ".当月有差额公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.NDifference != 0).Count());
                    else
                        hastable[item.TargetName + ".当月有差额公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.NDifference != 0).Count();

                    //商管专用
                    if (hastable.ContainsKey( item.TargetName + ".当月绝对值亏损总数") == false)
                        hastable.Add(item.TargetName + ".当月绝对值亏损总数", Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月绝对值亏损总数"] = Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0");

                    //商管特有
                    if (hastable.ContainsKey(item.TargetName + ".累计绝对值亏损总数") == false)
                        hastable.Add(item.TargetName + ".累计绝对值亏损总数", Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计绝对值亏损总数"] = Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0");


                    if (hastable.ContainsKey(item.TargetName + ".当月亏损率") == false)
                        hastable.Add(item.TargetName + ".当月亏损率", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月亏损率"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    if (hastable.ContainsKey(item.TargetName + ".累计计划总数") == false)
                        hastable.Add(item.TargetName + ".累计计划总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计计划总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    if (hastable.ContainsKey(item.TargetName + ".累计实际总数") == false)
                        hastable.Add(item.TargetName + ".累计实际总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计实际总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumAccumulativeActualAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount);
                    decimal SumAccumulativePlanAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount);
                    if (SumAccumulativePlanAmmount == 0) SumAccumulativePlanAmmount = 1;

                    if (hastable.ContainsKey(item.TargetName + ".累计累计完成率") == false)
                        hastable.Add(item.TargetName + ".累计累计完成率", monthRptSum.NAccumulativeActualRate);
                    else
                        hastable[item.TargetName + ".累计累计完成率"] = monthRptSum.NAccumulativeActualRate;

                    if (hastable.ContainsKey(item.TargetName + ".累计未完成公司数量") == false)
                        hastable.Add(item.TargetName + ".累计未完成公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.IsMissTarget == true).ToList().Count);
                    else
                        hastable[item.TargetName + ".累计未完成公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.IsMissTarget == true).ToList().Count;

                    if (hastable.ContainsKey(item.TargetName + ".累计亏损总数") == false)
                        hastable.Add(item.TargetName + ".累计亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(item.TargetName + ".累计有差额公司数量") == false)
                        hastable.Add(item.TargetName + ".累计有差额公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.NAccumulativeDifference < 0).Count());
                    else
                        hastable[item.TargetName + ".累计有差额公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.NAccumulativeDifference < 0).Count();

                    if (hastable.ContainsKey(item.TargetName + ".累计亏损率") == false)
                        hastable.Add(item.TargetName + ".累计亏损率", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计亏损率"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                }
            }
            return hastable;
        }


        private Guid GetCompanyID(Guid _SysId)
        {
            List<C_Company> CompanyList = C_CompanyOperator.Instance.GetCompanyList(_SysId).ToList();
            C_Company company = CompanyList.Where(p => p.CompanyName.IndexOf("总计") > 0).FirstOrDefault();

           if (company != null)
           {
               return company.ID;
           }
           else
           {
               return Guid.Empty;
           }

        }

        /// <summary>
        /// 循环系统下的指标（项目系统）
        /// </summary>
        /// <param name="targetList">指标列表</param>
        /// <param name="RptList">明细列表</param>
        /// <param name="hastable"></param>
        /// <returns></returns>
        public Hashtable ForTaeget_Pro(List<C_Target> targetList, List<MonthlyReportDetail> RptList, ref Hashtable hastable)
        {
            //每个系统相对应的指标

            MonthReportSummaryViewModel monthRptSum = null;

            List<A_TargetPlanDetail> targetPDetail = null;

            int i = 0;
            foreach (var item in targetList.OrderBy(o => o.Sequence))
            {
                if (item.NeedReport == true) //指标必须为上报
                {
                    //项目的计划指标需要单独获取
                    Guid companyID = Guid.Empty;

                    double CurrPlanNum = 0;
                    double AccPlanNum = 0;

                    //计划指标ID不是 Null
                    if (RptList[0].TargetPlanID != Guid.Empty)
                    {
                        //获取计划指标
                        targetPDetail = new List<A_TargetPlanDetail>();
                        targetPDetail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(RptList[0].TargetPlanID).ToList();

                        companyID = GetCompanyID(RptList[0].SystemID);

                        //当月项目计划数
                        CurrPlanNum = (double)targetPDetail.Where(p => p.TargetID == item.ID && p.CompanyID == companyID && p.FinYear == RptList[0].FinYear && p.FinMonth == RptList[0].FinMonth).Sum(s => s.Target);

                        //累计项目计划数
                        AccPlanNum = (double)targetPDetail.Where(p => p.TargetID == item.ID && p.CompanyID == companyID && p.FinYear == RptList[0].FinYear && p.FinMonth <= RptList[0].FinMonth).Sum(s => s.Target);

                    }



                    monthRptSum = new MonthReportSummaryViewModel();
                    monthRptSum.ID = i;

                    monthRptSum.FinYear = RptList[0].FinYear;
                    monthRptSum.TargetID = item.ID;
                    monthRptSum.SystemID = item.SystemID;
                    monthRptSum.TargetName = item.TargetName;

                    monthRptSum.NPlanAmmount = CurrPlanNum;
                    monthRptSum.NActualAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NActualAmmount));
                    monthRptSum.NAccumulativePlanAmmount = AccPlanNum;
                    monthRptSum.NAccumulativeActualAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NAccumulativeActualAmmount));
                    monthRptSum.MeasureRate = "0"; // 这个值是为了计算当月或者是累计完成率,如果想计算全年的,请将该字段的值赋值成全年的完成率
                    monthRptSum = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(monthRptSum);

                  

                    if (hastable.ContainsKey(item.TargetName + ".当月计划总数") == false)
                        hastable.Add(item.TargetName + ".当月计划总数", Math.Round(CurrPlanNum, 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月计划总数"] = ((int)Math.Round(CurrPlanNum, 0, MidpointRounding.AwayFromZero)).ToString("N0");
                        


                    if (hastable.ContainsKey(item.TargetName + ".当月实际总数") == false)
                        hastable.Add(item.TargetName + ".当月实际总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月实际总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumActualAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount);
                    decimal SumPlanAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount);
                    if (SumPlanAmmount == 0) SumPlanAmmount = 1;

                    if (hastable.ContainsKey(item.TargetName + ".当月累计完成率") == false)
                        hastable.Add(item.TargetName + ".当月累计完成率", monthRptSum.NActualRate);
                    else
                        hastable[item.TargetName + ".当月累计完成率"] = monthRptSum.NActualRate;

                    if (hastable.ContainsKey(item.TargetName + ".当月未完成公司数量") == false)
                        hastable.Add(item.TargetName + ".当月未完成公司数量", RptList.Where(p => p.TargetID == item.ID && p.IsMissTargetCurrent == true).ToList().Count);
                    else
                        hastable[item.TargetName + ".当月未完成公司数量"] = RptList.Where(p => p.TargetID == item.ID && p.IsMissTargetCurrent == true).ToList().Count;

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(item.TargetName + ".当月亏损总数") == false)
                        hastable.Add(item.TargetName + ".当月亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(item.TargetName + ".当月有差额公司数量") == false)
                        hastable.Add(item.TargetName + ".当月有差额公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.NDifference != 0).Count());
                    else
                        hastable[item.TargetName + ".当月有差额公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.NDifference != 0).Count();

                    //商管专用
                    if (hastable.ContainsKey(item.TargetName + ".当月绝对值亏损总数") == false)
                        hastable.Add(item.TargetName + ".当月绝对值亏损总数", Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月绝对值亏损总数"] = Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0");

                    //商管特有
                    if (hastable.ContainsKey(item.TargetName + ".累计绝对值亏损总数") == false)
                        hastable.Add(item.TargetName + ".累计绝对值亏损总数", Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计绝对值亏损总数"] = Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0");


                    if (hastable.ContainsKey(item.TargetName + ".当月亏损率") == false)
                        hastable.Add(item.TargetName + ".当月亏损率", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".当月亏损率"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    if (hastable.ContainsKey(item.TargetName + ".累计计划总数") == false)
                        hastable.Add(item.TargetName + ".累计计划总数", Math.Round(AccPlanNum, 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计计划总数"] = ((int)Math.Round(AccPlanNum, 0, MidpointRounding.AwayFromZero)).ToString("N0");

                    if (hastable.ContainsKey(item.TargetName + ".累计实际总数") == false)
                        hastable.Add(item.TargetName + ".累计实际总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计实际总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumAccumulativeActualAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount);
                    decimal SumAccumulativePlanAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount);
                    if (SumAccumulativePlanAmmount == 0) SumAccumulativePlanAmmount = 1;

                    if (hastable.ContainsKey(item.TargetName + ".累计累计完成率") == false)
                        hastable.Add(item.TargetName + ".累计累计完成率", monthRptSum.NAccumulativeActualRate);
                    else
                        hastable[item.TargetName + ".累计累计完成率"] = monthRptSum.NAccumulativeActualRate;

                    if (hastable.ContainsKey(item.TargetName + ".累计未完成公司数量") == false)
                        hastable.Add(item.TargetName + ".累计未完成公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.IsMissTarget == true).ToList().Count);
                    else
                        hastable[item.TargetName + ".累计未完成公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.IsMissTarget == true).ToList().Count;

                    if (hastable.ContainsKey(item.TargetName + ".累计亏损总数") == false)
                        hastable.Add(item.TargetName + ".累计亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(item.TargetName + ".累计有差额公司数量") == false)
                        hastable.Add(item.TargetName + ".累计有差额公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.NAccumulativeDifference < 0).Count());
                    else
                        hastable[item.TargetName + ".累计有差额公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.NAccumulativeDifference < 0).Count();

                    if (hastable.ContainsKey(item.TargetName + ".累计亏损率") == false)
                        hastable.Add(item.TargetName + ".累计亏损率", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[item.TargetName + ".累计亏损率"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                }
            }
            return hastable;
        }



        /// <summary>
        /// 循环系统下的指标(百货)
        /// </summary>
        /// <param name="targetList">指标列表</param>
        /// <param name="RptList">明细列表</param>
        /// <param name="hastable"></param>
        /// <returns></returns>
        public Hashtable ForTaeget(List<C_Target> targetList, List<MonthlyReportDetail> RptList, string Area, ref Hashtable hastable)
        {
            //每个系统相对应的指标

            MonthReportSummaryViewModel monthRptSum = null;
            int i = 0;
            foreach (var item in targetList.OrderBy(o => o.Sequence))
            {
                if (item.NeedReport == true) //指标必须为上报
                {
                    monthRptSum = new MonthReportSummaryViewModel();
                    monthRptSum.ID = i;

                    monthRptSum.FinYear = RptList[0].FinYear;
                    monthRptSum.TargetID = item.ID;
                    monthRptSum.SystemID = item.SystemID;
                    monthRptSum.TargetName = item.TargetName;
                    monthRptSum.MeasureRate = "0";// 这个值是为了计算当月或者是累计完成率,如果想计算全年的,请将该字段的值赋值成全年的完成率
                    monthRptSum.NPlanAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NPlanAmmount));
                    monthRptSum.NActualAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NActualAmmount));
                    monthRptSum.NAccumulativePlanAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NAccumulativePlanAmmount));
                    monthRptSum.NAccumulativeActualAmmount = (double)(RptList.Where(p => p.TargetID == item.ID).Sum(e => e.NAccumulativeActualAmmount));
                    monthRptSum = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(monthRptSum);

                    if (hastable.ContainsKey(Area + item.TargetName + ".当月计划总数") == false)
                        hastable.Add(Area + item.TargetName + ".当月计划总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NPlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".当月计划总数"] = ((int)Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NPlanAmmount), 0, MidpointRounding.AwayFromZero)).ToString("N0");


                    if (hastable.ContainsKey(Area + item.TargetName + ".当月实际总数") == false)
                        hastable.Add(Area + item.TargetName + ".当月实际总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".当月实际总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumActualAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount);
                    decimal SumPlanAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NActualAmmount);
                    if (SumPlanAmmount == 0) SumPlanAmmount = 1;

                    if (hastable.ContainsKey(Area + item.TargetName + ".当月累计完成率") == false)
                        hastable.Add(Area + item.TargetName + ".当月累计完成率", monthRptSum.NActualRate);
                    else
                        hastable[Area + item.TargetName + ".当月累计完成率"] = monthRptSum.NActualRate;

                    if (hastable.ContainsKey(Area + item.TargetName + ".当月未完成公司数量") == false)
                        hastable.Add(Area + item.TargetName + ".当月未完成公司数量", RptList.Where(p => p.TargetID == item.ID && p.IsMissTargetCurrent == true).ToList().Count);
                    else
                        hastable[Area + item.TargetName + ".当月未完成公司数量"] = RptList.Where(p => p.TargetID == item.ID && p.IsMissTargetCurrent == true).ToList().Count;

                    if (hastable.ContainsKey(Area + item.TargetName + ".当月亏损总数") == false)
                        hastable.Add(Area + item.TargetName + ".当月亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".当月亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //商管专用
                    if (hastable.ContainsKey(Area + item.TargetName + ".当月有差额公司数量") == false)
                        hastable.Add(Area + item.TargetName + ".当月有差额公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.NDifference < 0).Count());
                    else
                        hastable[Area + item.TargetName + ".当月有差额公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.NDifference < 0).Count();



                    if (hastable.ContainsKey(Area + item.TargetName + ".当月亏损率") == false)
                        hastable.Add(Area + item.TargetName + ".当月亏损率", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".当月亏损率"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //商管专用
                    if (hastable.ContainsKey(Area + item.TargetName + ".当月绝对值亏损总数") == false)
                        hastable.Add(Area + item.TargetName + ".当月绝对值亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".当月绝对值亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");



                    if (hastable.ContainsKey(Area + item.TargetName + ".累计计划总数") == false)
                        hastable.Add(Area + item.TargetName + ".累计计划总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".累计计划总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    if (hastable.ContainsKey(Area + item.TargetName + ".累计实际总数") == false)
                        hastable.Add(Area + item.TargetName + ".累计实际总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".累计实际总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumAccumulativeActualAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeActualAmmount);
                    decimal SumAccumulativePlanAmmount = RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativePlanAmmount);
                    if (SumAccumulativePlanAmmount == 0) SumAccumulativePlanAmmount = 1;

                    if (hastable.ContainsKey(Area + item.TargetName + ".累计累计完成率") == false)
                        hastable.Add(Area + item.TargetName + ".累计累计完成率", monthRptSum.NAccumulativeActualRate);
                    else
                        hastable[Area + item.TargetName + ".累计累计完成率"] = monthRptSum.NAccumulativeActualRate;

                    if (hastable.ContainsKey(Area + item.TargetName + ".累计未完成公司数量") == false)
                        hastable.Add(Area + item.TargetName + ".累计未完成公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.IsMissTarget == true).ToList().Count);
                    else
                        hastable[Area + item.TargetName + ".累计未完成公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.IsMissTarget == true).ToList().Count;

                    if (hastable.ContainsKey(Area + item.TargetName + ".累计亏损总数") == false)
                        hastable.Add(Area + item.TargetName + ".累计亏损总数", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".累计亏损总数"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //商管特有
                    if (hastable.ContainsKey(Area + item.TargetName + ".累计有差额公司数量") == false)
                        hastable.Add(Area + item.TargetName + ".累计有差额公司数量", (int)RptList.Where(p => p.TargetID == item.ID && p.NAccumulativeDifference < 0).Count());
                    else
                        hastable[Area + item.TargetName + ".累计有差额公司数量"] = (int)RptList.Where(p => p.TargetID == item.ID && p.NAccumulativeDifference < 0).Count();

                    if (hastable.ContainsKey(Area + item.TargetName + ".累计亏损率") == false)
                        hastable.Add(Area + item.TargetName + ".累计亏损率", Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".累计亏损率"] = Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //商管特有
                    if (hastable.ContainsKey(Area + item.TargetName + ".累计绝对值亏损总数") == false)
                        hastable.Add(Area + item.TargetName + ".累计绝对值亏损总数", Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0"));
                    else
                        hastable[Area + item.TargetName + ".累计绝对值亏损总数"] = Math.Abs(Math.Round(RptList.Where(p => p.TargetID == item.ID).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero)).ToString("N0");

                }
            }
            return hastable;
        }


        /// <summary>
        /// 循环系统下的公司
        /// </summary>
        /// <param name="compayList">公司</param>
        /// <param name="RptList"></param>
        /// <param name="hastable"></param>
        /// <returns></returns>
        public Hashtable ForCompay(List<C_Company> compayList, List<MonthlyReportDetail> RptList, ref Hashtable hastable)
        {
            int DoubleTarget = 0;
            int SingleTarget1 = 0;
            int SingleTarget2 = 0;
            int missCountt = 0;

            int DoubleTargetAccumulative = 0;
            int SingleTargetAccumulative1 = 0;
            int SingleTargetAccumulative2 = 0;
            int accumulativeMissCount = 0;

            foreach (var item in compayList)
            {
                //当月的
                List<MonthlyReportDetail> tempDetail = RptList.Where(p => p.CompanyID == item.ID && p.IsMissTargetCurrent == true).ToList();

                //判断该公司是否都完成了指标
                if (tempDetail.Count > 0)
                {
                    //双指标未完成,公司未完成指标，有多个
                    int misstargetcount = tempDetail.Count;
                    if (misstargetcount > 1)
                    {
                        DoubleTarget++;
                    }
                    else
                    {
                        //总收入
                        if (tempDetail[0].TargetName == "总收入")
                        {
                            SingleTarget1++;
                        }
                        else
                        {
                            SingleTarget2++;
                        }
                    }

                    missCountt++;
                }

                //累计的
                List<MonthlyReportDetail> tempAccumulativeDetail = RptList.Where(p => p.CompanyID == item.ID && p.IsMissTarget == true).ToList();

                //判断该公司是否都完成了指标
                if (tempAccumulativeDetail.Count > 0)
                {

                    //双指标未完成,公司未完成指标，有多个
                    int misstargetAccCount = tempAccumulativeDetail.Count;
                    if (misstargetAccCount > 1)
                    {
                        DoubleTargetAccumulative++;
                    }
                    else
                    {
                        //总收入
                        if (tempAccumulativeDetail[0].TargetName.Trim() == "总收入")
                        {
                            SingleTargetAccumulative1++;
                        }
                        else
                        {
                            SingleTargetAccumulative2++;
                        }
                    }

                    accumulativeMissCount++;
                }


            }

            //当月
            if (hastable.ContainsKey("当月双指标未完成") == false)
                hastable.Add("当月双指标未完成", DoubleTarget);
            else
                hastable["当月双指标未完成"] = DoubleTarget;

            if (hastable.ContainsKey("当月仅收入未完成") == false)
                hastable.Add("当月仅收入未完成", SingleTarget1);
            else
                hastable["当月仅收入未完成"] = SingleTarget1;

            if (hastable.ContainsKey("当月仅利润未完成") == false)
                hastable.Add("当月仅利润未完成", SingleTarget2);
            else
                hastable["当月仅利润未完成"] = SingleTarget2;

            if (hastable.ContainsKey("当月未完成") == false)
                hastable.Add("当月未完成", missCountt);
            else
                hastable["当月未完成"] = missCountt;


            //累计
            if (hastable.ContainsKey("累计双指标未完成") == false)
                hastable.Add("累计双指标未完成", DoubleTargetAccumulative);
            else
                hastable["累计双指标未完成"] = DoubleTargetAccumulative;

            if (hastable.ContainsKey("累计仅收入未完成") == false)
                hastable.Add("累计仅收入未完成", SingleTargetAccumulative1);
            else
                hastable["累计仅收入未完成"] = SingleTargetAccumulative1;

            if (hastable.ContainsKey("累计仅利润未完成") == false)
                hastable.Add("累计仅利润未完成", SingleTargetAccumulative2);
            else
                hastable["累计仅利润未完成"] = SingleTargetAccumulative2;


            if (hastable.ContainsKey("累计未完成") == false)
                hastable.Add("累计未完成", accumulativeMissCount);
            else
                hastable["累计未完成"] = accumulativeMissCount;


            return hastable;
        }

        /// <summary>
        /// 循环系统下的同一类指标
        /// </summary>
        /// <param name="targetList">指标列表</param>
        /// <param name="RptList">明细列表</param>
        /// <param name="hastable"></param>
        /// <returns></returns>
        public Hashtable ForTaegetType(List<C_Target> targetList, List<MonthlyReportDetail> RptList, ref Hashtable hastable)
        {
            //每个系统相对应的指标

            MonthReportSummaryViewModel monthRptSum = null;
            int i = 0;
            foreach (var item in targetList.OrderBy(o => o.Sequence))
            {
                if (item.NeedReport == true) //指标必须为上报
                {
                    string TargetTypeDescription=EnumHelper.GetEnumDescription(typeof(EnumTargetType), item.TargetType);
                    monthRptSum = new MonthReportSummaryViewModel();
                    monthRptSum.ID = i;

                    monthRptSum.FinYear = RptList[0].FinYear;
                    monthRptSum.TargetID = item.ID;
                    monthRptSum.SystemID = item.SystemID;
                    monthRptSum.TargetName = item.TargetName;
                    monthRptSum.MeasureRate = "0";// 这个值是为了计算当月或者是累计完成率,如果想计算全年的,请将该字段的值赋值成全年的完成率
                    monthRptSum.NPlanAmmount = (double)(RptList.Where(p => p.TargetType == item.TargetType).Sum(e => e.NPlanAmmount));
                    monthRptSum.NActualAmmount = (double)(RptList.Where(p => p.TargetType == item.TargetType).Sum(e => e.NActualAmmount));
                    monthRptSum.NAccumulativePlanAmmount = (double)(RptList.Where(p => p.TargetType == item.TargetType).Sum(e => e.NAccumulativePlanAmmount));
                    monthRptSum.NAccumulativeActualAmmount = (double)(RptList.Where(p => p.TargetType == item.TargetType).Sum(e => e.NAccumulativeActualAmmount));
                    monthRptSum = TargetEvaluationEngine.TargetEvaluationService.SummaryCalculation(monthRptSum);

                    if (hastable.ContainsKey(TargetTypeDescription + ".当月计划总数") == false)
                        hastable.Add(TargetTypeDescription + ".当月计划总数", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NPlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".当月计划总数"] = ((int)Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NPlanAmmount), 0, MidpointRounding.AwayFromZero)).ToString("N0");


                    if (hastable.ContainsKey(TargetTypeDescription + ".当月实际总数") == false)
                        hastable.Add(TargetTypeDescription + ".当月实际总数", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".当月实际总数"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumActualAmmount = RptList.Where(p =>p.TargetType == item.TargetType).Sum(s => s.NActualAmmount);
                    decimal SumPlanAmmount = RptList.Where(p =>p.TargetType == item.TargetType).Sum(s => s.NActualAmmount);
                    if (SumPlanAmmount == 0) SumPlanAmmount = 1;

                    if (hastable.ContainsKey(TargetTypeDescription + ".当月累计完成率") == false)
                        hastable.Add(TargetTypeDescription + ".当月累计完成率", monthRptSum.NActualRate);
                    else
                        hastable[TargetTypeDescription + ".当月累计完成率"] = monthRptSum.NActualRate;

                    if (hastable.ContainsKey(TargetTypeDescription + ".当月未完成公司数量") == false)
                        hastable.Add(TargetTypeDescription + ".当月未完成公司数量", RptList.Where(p =>p.TargetType == item.TargetType && p.IsMissTargetCurrent == true).ToList().Count);
                    else
                        hastable[TargetTypeDescription + ".当月未完成公司数量"] = RptList.Where(p =>p.TargetType == item.TargetType && p.IsMissTargetCurrent == true).ToList().Count;

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(TargetTypeDescription + ".当月亏损总数") == false)
                        hastable.Add(TargetTypeDescription + ".当月亏损总数", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".当月亏损总数"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(TargetTypeDescription + ".当月有差额公司数量") == false)
                        hastable.Add(TargetTypeDescription + ".当月有差额公司数量", (int)RptList.Where(p =>p.TargetType == item.TargetType && p.NDifference != 0).Count());
                    else
                        hastable[TargetTypeDescription + ".当月有差额公司数量"] = (int)RptList.Where(p =>p.TargetType == item.TargetType && p.NDifference != 0).Count();

                    if (hastable.ContainsKey(TargetTypeDescription + ".当月亏损率") == false)
                        hastable.Add(TargetTypeDescription + ".当月亏损率", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".当月亏损率"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    if (hastable.ContainsKey(TargetTypeDescription + ".累计计划总数") == false)
                        hastable.Add(TargetTypeDescription + ".累计计划总数", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativePlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".累计计划总数"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativePlanAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    if (hastable.ContainsKey(TargetTypeDescription + ".累计实际总数") == false)
                        hastable.Add(TargetTypeDescription + ".累计实际总数", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".累计实际总数"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeActualAmmount), 0, MidpointRounding.AwayFromZero).ToString("N0");


                    decimal SumAccumulativeActualAmmount = RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeActualAmmount);
                    decimal SumAccumulativePlanAmmount = RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativePlanAmmount);
                    if (SumAccumulativePlanAmmount == 0) SumAccumulativePlanAmmount = 1;

                    if (hastable.ContainsKey(TargetTypeDescription + ".累计累计完成率") == false)
                        hastable.Add(TargetTypeDescription + ".累计累计完成率", monthRptSum.NAccumulativeActualRate);
                    else
                        hastable[TargetTypeDescription + ".累计累计完成率"] = monthRptSum.NAccumulativeActualRate;

                    if (hastable.ContainsKey(TargetTypeDescription + ".累计未完成公司数量") == false)
                        hastable.Add(TargetTypeDescription + ".累计未完成公司数量", (int)RptList.Where(p =>p.TargetType == item.TargetType && p.IsMissTarget == true).ToList().Count);
                    else
                        hastable[TargetTypeDescription + ".累计未完成公司数量"] = (int)RptList.Where(p =>p.TargetType == item.TargetType && p.IsMissTarget == true).ToList().Count;

                    if (hastable.ContainsKey(TargetTypeDescription + ".累计亏损总数") == false)
                        hastable.Add(TargetTypeDescription + ".累计亏损总数", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".累计亏损总数"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                    //这个是给商管用的标签
                    if (hastable.ContainsKey(TargetTypeDescription + ".累计有差额公司数量") == false)
                        hastable.Add(TargetTypeDescription + ".累计有差额公司数量", (int)RptList.Where(p =>p.TargetType == item.TargetType && p.NAccumulativeDifference < 0).Count());
                    else
                        hastable[TargetTypeDescription + ".累计有差额公司数量"] = (int)RptList.Where(p =>p.TargetType == item.TargetType && p.NAccumulativeDifference < 0).Count();

                    if (hastable.ContainsKey(TargetTypeDescription + ".累计亏损率") == false)
                        hastable.Add(TargetTypeDescription + ".累计亏损率", Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0"));
                    else
                        hastable[TargetTypeDescription + ".累计亏损率"] = Math.Round(RptList.Where(p => p.TargetType == item.TargetType).Sum(s => s.NAccumulativeDifference), 0, MidpointRounding.AwayFromZero).ToString("N0");

                }
            }
            return hastable;
        }
    }


    /// <summary>
    /// 百货的引擎
    /// </summary>
    public class BH_GetMonthDescriptionValue : IGetMonthDescriptionValue
    {
        public Hashtable GetMonthDescriptionValue(object Vmodel)
        {
            List<MonthlyReportDetail> RptList = (List<MonthlyReportDetail>)Vmodel;

            if (RptList == null || RptList.Count <= 0) return new Hashtable();
            Hashtable hastable = new Hashtable();

            DateTime ReportDate = DateTime.MinValue;

            if (RptList[0].FinMonth == 12)
            {
                DateTime.Parse(Convert.ToInt32(RptList[0].FinYear + 1) + "-01-01").AddDays(-1);
            }
            else
            {
                ReportDate = DateTime.Parse(RptList[0].FinYear + "-" + Convert.ToInt32(RptList[0].FinMonth + 1) + "-01").AddDays(-1);
            }

            DateTime CurrentDate = DateTime.Now;

            C_System sysModel = StaticResource.Instance[RptList[0].SystemID, CurrentDate];

            List<C_Target> targetList = StaticResource.Instance.TargetList[RptList[0].SystemID];

            List<C_Company> compayList = StaticResource.Instance.CompanyList[RptList[0].SystemID];

            List<VCompanyProperty> compayProList = new List<VCompanyProperty>();


            VCompanyProperty areaModel = new VCompanyProperty();

            XElement element = StaticResource.Instance[RptList[0].SystemID, CurrentDate].Configuration;

            if (element.Elements("ListCompanyProperty").Elements("CompanyProperty") != null)
            {
                List<XElement> companyPro = element.Elements("ListCompanyProperty").Elements("CompanyProperty").ToList();
                foreach (XElement pro in companyPro)
                {
                    compayProList.Add(new VCompanyProperty(pro));
                }
                areaModel = compayProList.Where(p => p.CompanyPropertyName == "大区").FirstOrDefault();
            }

            GetMonthDescriptionTool tools = new GetMonthDescriptionTool();

            if (areaModel != null)
            {   //循环每个大区
                foreach (var item in areaModel.listCP)
                {  //查询大区的相关数据
                    var js = from r in RptList
                             join co in compayList
                             on r.CompanyID equals co.ID
                             where co.CompanyProperty3 == item.ItemCompanyPropertyName
                             select r;
                    List<MonthlyReportDetail> AreaList = js.ToList();


                    //纯粹只计算各个大区的百货系统
                    tools.ForTaeget(targetList, AreaList, item.ItemCompanyPropertyName, ref hastable);
                }
            }
            //计算不是大区的值
            tools.ForTaeget(targetList, RptList, ref hastable);

            hastable.Add("当前月", RptList[0].FinMonth);

            return hastable;
        }

    }

    /// <summary>
    /// 大歌星的引擎
    /// </summary>
    public class DGX_GetMonthDescriptionValue : IGetMonthDescriptionValue
    {
        public Hashtable GetMonthDescriptionValue(object Vmodel)
        {
            List<MonthlyReportDetail> RptList = (List<MonthlyReportDetail>)Vmodel;

            if (RptList == null || RptList.Count <= 0) return new Hashtable();
            Hashtable hastable = new Hashtable();

            DateTime ReportDate = DateTime.MinValue;

            if (RptList[0].FinMonth == 12)
            {
                DateTime.Parse(Convert.ToInt32(RptList[0].FinYear + 1) + "-01-01").AddDays(-1);
            }
            else
            {
                ReportDate = DateTime.Parse(RptList[0].FinYear + "-" + Convert.ToInt32(RptList[0].FinMonth + 1) + "-01").AddDays(-1);
            }
            DateTime CurrentDate = DateTime.Now;
            C_System sysModel = StaticResource.Instance[RptList[0].SystemID,CurrentDate];

            List<C_Target> targetList = StaticResource.Instance.TargetList[RptList[0].SystemID];

            List<C_Company> compayList = StaticResource.Instance.CompanyList[RptList[0].SystemID];

            List<VCompanyProperty> compayProList = new List<VCompanyProperty>();

            GetMonthDescriptionTool tools = new GetMonthDescriptionTool();

            tools.ForTaeget(targetList, RptList, ref hastable);
            tools.ForCompay(compayList, RptList, ref hastable);

            hastable.Add("当前月", RptList[0].FinMonth);
            return hastable;

        }





    }

    /// <summary>
    /// 项目系统的
    /// </summary>
    public class Pro_Centre_GetMonthDescriptionValue : IGetMonthDescriptionValue
    {
        public Hashtable GetMonthDescriptionValue(object Vmodel)
        {
            List<MonthlyReportDetail> RptList = (List<MonthlyReportDetail>)Vmodel;

            if (RptList == null || RptList.Count <= 0) return new Hashtable();
            Hashtable hastable = new Hashtable();

            DateTime ReportDate = DateTime.MinValue;

            if (RptList[0].FinMonth == 12)
            {
                DateTime.Parse(Convert.ToInt32(RptList[0].FinYear + 1) + "-01-01").AddDays(-1);
            }
            else
            {
                ReportDate = DateTime.Parse(RptList[0].FinYear + "-" + Convert.ToInt32(RptList[0].FinMonth + 1) + "-01").AddDays(-1);
            }
            DateTime CurrentDate = DateTime.Now;
            C_System sysModel = StaticResource.Instance[RptList[0].SystemID,CurrentDate];

            List<C_Target> targetList = StaticResource.Instance.TargetList[RptList[0].SystemID];

            List<C_Company> compayList = StaticResource.Instance.CompanyList[RptList[0].SystemID];

            List<VCompanyProperty> compayProList = new List<VCompanyProperty>();

            GetMonthDescriptionTool tools = new GetMonthDescriptionTool();

            tools.ForTaeget_Pro(targetList, RptList, ref hastable);

            hastable.Add("当前月", RptList[0].FinMonth);
            return hastable;
        }
    }


    /// <summary>
    /// 集团
    /// </summary>
    public class Group_Centre_GetMonthDescriptionValue : IGetMonthDescriptionValue
    {
        public Hashtable GetMonthDescriptionValue(object Vmodel)
        {
            List<MonthlyReportDetail> RptList = (List<MonthlyReportDetail>)Vmodel;

            if (RptList == null || RptList.Count <= 0) return new Hashtable();
            Hashtable hastable = new Hashtable();

            DateTime ReportDate = DateTime.MinValue;

            if (RptList[0].FinMonth == 12)
            {
                DateTime.Parse(Convert.ToInt32(RptList[0].FinYear + 1) + "-01-01").AddDays(-1);
            }
            else
            {
                ReportDate = DateTime.Parse(RptList[0].FinYear + "-" + Convert.ToInt32(RptList[0].FinMonth + 1) + "-01").AddDays(-1);
            }
            DateTime CurrentDate = DateTime.Now;

            C_System sysModel = StaticResource.Instance[RptList[0].SystemID,CurrentDate];

            List<C_Target> targetList = StaticResource.Instance.TargetList[RptList[0].SystemID];

            List<C_Company> compayList = StaticResource.Instance.CompanyList[RptList[0].SystemID];

            List<VCompanyProperty> compayProList = new List<VCompanyProperty>();

            GetMonthDescriptionTool tools = new GetMonthDescriptionTool();

            tools.ForTaegetType(targetList, RptList, ref hastable);

            hastable.Add("当前月", RptList[0].FinMonth);
            return hastable;
        }
    }
}
