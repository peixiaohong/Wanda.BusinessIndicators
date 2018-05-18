using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;


namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="VModel">筛选出来的结果</param>
        /// <param name="_SystemID">系统ID</param>
        /// <param name="TargetIDs">指标ID</param>
        /// <returns></returns>
        public object GetSystem(object VModel, Guid _SystemID, List<Guid> TargetIDs)
        {
            List<MonthlyReportDetail> Result = (List<MonthlyReportDetail>)VModel;
            List<MonthlyReportDetail> ResultSum = new List<MonthlyReportDetail>();
            ResultSum = SequenceEngine.SequenceService.GetSequence(_SystemID, "MissTarget", Result.OrderBy(O => O.CompanyID).ToList(), JsonHelper.Serialize(TargetIDs));
            return ResultSum;
        }
    }


    public class System_SG : ISystem
    {
        /// <summary>
        /// 商管专用
        /// </summary>
        /// <param name="VModel">筛选出来的结果</param>
        /// <param name="_SystemID">系统ID</param>
        /// <param name="TargetIDs">指标ID</param>
        /// <returns></returns>
        public object GetSystem(object VModel, Guid _SystemID, List<Guid> TargetIDs)
        {
            List<MonthlyReportDetail> Result = (List<MonthlyReportDetail>)VModel;
            List<MonthlyReportDetail> ResultSum = new List<MonthlyReportDetail>();

            if (Result.Count > 0)
            {
                List<MonthlyReportDetail> ResultTempSum = new List<MonthlyReportDetail>();
                MonthlyReportDetail SumModel = new MonthlyReportDetail();
                SumModel.NAccumulativeDifference = Result.Sum(s => s.NAccumulativeDifference); //差值总和
                SumModel.AddDifference = Result.Sum(s => s.AddDifference);//新增差值
                SumModel.CompanyName = "未完成合计";
                SumModel.NAccumulativePlanAmmount = Result.Sum(s => s.NAccumulativePlanAmmount);
                SumModel.NAccumulativeActualAmmount = Result.Sum(s => s.NAccumulativeActualAmmount);

                if (SumModel.NAccumulativePlanAmmount != 0)
                {
                    SumModel.NAccumulativeDisplayRate = (SumModel.NAccumulativeActualAmmount / SumModel.NAccumulativePlanAmmount).ToString("P");
                }
                else
                {
                    SumModel.NAccumulativeDisplayRate = (SumModel.NAccumulativeActualAmmount / 1).ToString("P");
                }

                
                SumModel.LastNAccumulativeActualAmmount = Result.Sum(s => s.LastNAccumulativeActualAmmount);
                SumModel.LastNAccumulativeDifference = Result.Sum(s => s.LastNAccumulativeDifference);
                SumModel.LastNAccumulativePlanAmmount = Result.Sum(s => s.LastNAccumulativePlanAmmount);
                
                ResultTempSum.Add(SumModel); // 添加
                ResultSum = SequenceEngine.SequenceService.GetSequence(_SystemID, "MissTarget", Result.OrderBy(O => O.CompanyID).ToList(), JsonHelper.Serialize(TargetIDs));
                ResultSum.AddRange(ResultTempSum);
            }
            else
            {
                ResultSum = SequenceEngine.SequenceService.GetSequence(_SystemID, "MissTarget", Result.OrderBy(O => O.CompanyID).ToList(), JsonHelper.Serialize(TargetIDs));
            }
            return ResultSum;
        }
    }


}
