using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a MonthlyReportDetail.
	/// </summary>
    [ORTableMapping("dbo.B_MonthlyReportDetail")]
    public class B_MonthlyReportDetail : BaseModel
	{
	 
		#region Public Properties 
	      
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("FinMonth")]
		public int FinMonth{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}

        [NoMapping]
        public string TargetName { get; set; }

        
        [ORFieldMapping("CompanyID")]
		public Guid CompanyID{get;set;}
		 

        
        [ORFieldMapping("TargetPlanID")]
		public Guid TargetPlanID{get;set;}
		 

        
        [ORFieldMapping("OPlanAmmount")]
		public decimal OPlanAmmount{get;set;}
		 

        
        [ORFieldMapping("OActualAmmount")]
		public decimal OActualAmmount{get;set;}
		 

        
        [ORFieldMapping("OActualRate")]
        public string OActualRate { get; set; }
		 

        
        [ORFieldMapping("ODisplayRate")]
		public string ODisplayRate{get;set;}
		 

        
        [ORFieldMapping("NPlanAmmount")]
		public decimal NPlanAmmount{get;set;}
		 

        
        [ORFieldMapping("NActualAmmount")]
		public decimal NActualAmmount{get;set;}

     
        
        [ORFieldMapping("NActualRate")]
        public string NActualRate { get; set; }
		 

        
        [ORFieldMapping("NDisplayRate")]
		public string NDisplayRate{get;set;}
		 

        
        [ORFieldMapping("OAccumulativePlanAmmount")]
		public decimal OAccumulativePlanAmmount{get;set;}
		 

        
        [ORFieldMapping("OAccumulativeActualAmmount")]
		public decimal OAccumulativeActualAmmount{get;set;}
		 

        
        [ORFieldMapping("OAccumulativeActualRate")]
        public string OAccumulativeActualRate { get; set; }
		 

        
        [ORFieldMapping("OAcccumulativeDisplayRate")]
		public string OAcccumulativeDisplayRate{get;set;}
		 

        
        [ORFieldMapping("NAccumulativePlanAmmount")]
		public decimal NAccumulativePlanAmmount{get;set;}
		 

        
        [ORFieldMapping("NAccumulativeActualAmmount")]
		public decimal NAccumulativeActualAmmount{get;set;}


        
        [ORFieldMapping("NAccumulativeActualRate")]
        public string NAccumulativeActualRate { get; set; }
		 

        
        [ORFieldMapping("NAccumulativeDisplayRate")]
        public string NAccumulativeDisplayRate { get; set; }
		 

        
        [ORFieldMapping("IsMissTarget")]
		public bool IsMissTarget{get;set;}

        /// <summary>
        /// 当前月未完成标识
        /// </summary>
        [ORFieldMapping("IsMissTargetCurrent")]
        public bool IsMissTargetCurrent { get; set; }


        
        [ORFieldMapping("Counter")]
		public int Counter{get;set;}
		 

        
        [ORFieldMapping("FirstMissTargetDate")]
		public DateTime? FirstMissTargetDate{get;set;}
		 

        
        [ORFieldMapping("PromissDate")]
		public DateTime? PromissDate{get;set;}
		 

        /// <summary>
        /// 要求时间
        /// </summary>
        [ORFieldMapping("CommitDate")]
		public DateTime? CommitDate{get;set;}
		 

        /// <summary>
        /// 未完成说明
        /// </summary>
        [ORFieldMapping("MIssTargetReason")]
		public string MIssTargetReason{get;set;}
		 

        /// <summary>
        /// 采取措施
        /// </summary>
        [ORFieldMapping("MIssTargetDescription")]
		public string MIssTargetDescription{get;set;}




        /// <summary>
        /// 当月未完成说明
        /// </summary>
        [ORFieldMapping("CurrentMIssTargetReason")]
        public string CurrentMIssTargetReason { get; set; }


        /// <summary>
        /// 当月采取措施
        /// </summary>
        [ORFieldMapping("CurrentMIssTargetDescription")]
        public string CurrentMIssTargetDescription { get; set; }





        [ORFieldMapping("ReturnType")]
		public int ReturnType{get;set;}


        /// <summary>
        /// 差值(新)
        /// </summary>
        [ORFieldMapping("NDifference")]
        public decimal NDifference { get; set; }

        /// <summary>
        /// 差值(旧)
        /// </summary>
        [ORFieldMapping("ODifference")]
        public decimal ODifference { get; set; }

        /// <summary>
        /// 累计差值(新)
        /// </summary>
        [ORFieldMapping("NAccumulativeDifference")]
        public decimal NAccumulativeDifference { get; set; }

        /// <summary>
        /// 累计差值(旧)
        /// </summary>
        [ORFieldMapping("OAccumulativeDifference")]
        public decimal OAccumulativeDifference { get; set; }

        /// <summary>
        /// 月度上报ID 
        /// </summary>
        [ORFieldMapping("MonthlyReportID")]
        public Guid MonthlyReportID { get; set; }


        /// <summary>
        /// 当月的考核指数
        /// </summary>
        [ORFieldMapping("MeasureRate")]
        public decimal MeasureRate { get; set; }


        /// <summary>
        /// 补回情况
        /// </summary>
        [ORFieldMapping("ReturnDescription")]
        public string ReturnDescription { get; set; }

        /// <summary>
        /// 是否在页面显示
        /// </summary>
        [ORFieldMapping("Display")]
        public bool Display { get; set; }
		#endregion

        [ORFieldMapping("CompanyProperty1")]
        public string CompanyProperty1 { get; set; }

        /// <summary>
        /// 是否是当月要求时间
        /// </summary>
        [ORFieldMapping("IsCommitDate")]
        public int IsCommitDate { get; set; }


        /// <summary>
        /// 要求说明(在没有要求时间的时候，显示)
        /// </summary>
        [ORFieldMapping("CommitReason")]
        public string CommitReason { get; set; }


        /// <summary>
        /// 本月未完成要求时间 //---业务管理设置的字段
        /// </summary>
        [ORFieldMapping("CurrentMonthCommitDate")]
        public DateTime? CurrentMonthCommitDate { get; set; }


        /// <summary>
        /// 本月未完成要求说明 //---业务管理设置的字段
        /// </summary>
        [ORFieldMapping("CurrentMonthCommitReason")]
        public string CurrentMonthCommitReason { get; set; }


        /// <summary>
        /// 补回状态（子状态）
        /// </summary>
        [ORFieldMapping("ReturnType_Sub")]
        public string ReturnType_Sub { get; set; }


        /// <summary>
        /// 重新计算的Counter
        /// </summary>
        [ORFieldMapping("NewCounter")]
        public int NewCounter { get; set; }


        /// <summary>
        /// 公司属性Json
        /// </summary>
        [ORFieldMapping("CompanyProperty")]
        public string CompanyProperty{get;set;}

	} 
}

