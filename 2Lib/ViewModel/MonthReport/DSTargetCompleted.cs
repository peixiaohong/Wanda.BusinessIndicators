using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.ViewModel
{
    #region//百货系统经营指标完成门店数量情况所用的实体类

    public class DSTargetCompleted
    {
        public string CompanyName { get; set; }
        public string AreaName { get; set; }
        public int LastCount { get; set; }
        public int CurrentCount { get; set; }
        public int ToCurrentCount { get; set; }
    }

    public class ShowDSTargetCompleted
    {
        public string PorjectName { get; set; }
        public int LastNorth { get; set; }
        public int LastCenter { get; set; }
        public int LastSouth { get; set; }
        public int LastTotal { get; set; }
        public int CurrentNorth { get; set; }
        public int CurrentCenter { get; set; }
        public int CurrentSouth { get; set; }
        public int CurrentTotal { get; set; }
        public int ToCurrentNorth { get; set; }
        public int ToCurrentCenter { get; set; }
        public int ToCurrentSouth { get; set; }
        public int ToCurrentTotal { get; set; }
    }

    #endregion

    
    #region //百货系统经营指标完成情况对比所用的实体类

    public class ShowDSTargetArea
    {
        public int ID { get; set; }
        public string AreaName { get; set; }
        public List<ShowDSTargetCompletedDetail> DetailList { get; set; }
    }

    public class ShowDSTargetCompletedDetail
    {
        public int DetailAreaID { get; set; }
        public string DetailTargetName { get; set; }
        public decimal LastPlan { get; set; }
        public decimal LastActual { get; set; }
        public decimal LastDifference { get; set; }
        public string LastRate { get; set; }
        public decimal CurrentPlan { get; set; }
        public decimal CurrentActual { get; set; }
        public decimal CurrentDifference { get; set; }
        public string CurrentRate { get; set; }
        public decimal ToCurrentPlan { get; set; }
        public decimal ToCurrentActual { get; set; }
        public decimal ToCurrentDifference { get; set; }
        public string ToCurrentRate { get; set; }
    }

    #endregion 
    
    
    #region //百货系统经营指标补回/新增情况所用的实体类

    public class DSTargetReturnDataCompany
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }//(补回的时候使用的)
        public bool IsAllReturn { get; set; }//(补回的时候使用的)
        public string AddTargetName { get; set; }//(新增的时候使用的)
        public List<DSTargetReturnData> ReturnDataList { get; set; }
    }

    public class DSTargetReturnData
    {
        public int CompanyID { get; set; }
        public string ReturnTargetName { get; set; }
        public decimal CurrentReturnAmount { get; set; }
        public decimal LastAccumulativePlan { get; set; }
        public decimal LastAccumulativeActual { get; set; }
        public decimal LastAccumulativeDifference { get; set; }
        public decimal CurrentAccumulativePlan { get; set; }
        public decimal CurrentAccumulativeActual { get; set; }
        public decimal CurrentAccumulativeDifference { get; set; }
        public string CurrentAccumulativeRate { get; set; }
        public string CommitDate { get; set; }
        public int ReturnType { get; set; }
        public string ReturnTypeDescrible { get; set; }
        public int Counter { get; set; }
    }

    #endregion
}
