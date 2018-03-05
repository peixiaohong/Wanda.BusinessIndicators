using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a MonthlyTargetReport.
	/// </summary>
    [ORTableMapping("dbo.B_MonthlyTargetReport")]
    public class B_MonthlyTargetReport : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("CompanyID")]
		public Guid CompanyID{get;set;}
		 

        
        [ORFieldMapping("ProjectID")]
		public Guid ProjectID{get;set;}
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}
		 

        
        [ORFieldMapping("FinMonth")]
		public int FinMonth{get;set;}
		 

        
        [ORFieldMapping("TargetPlanID")]
		public Guid TargetPlanID{get;set;}
		 

        
        [ORFieldMapping("PlanAmmount")]
		public decimal PlanAmmount{get;set;}
		 

        
        [ORFieldMapping("ActualAmmount")]
		public decimal ActualAmmount{get;set;}
		 

        
        [ORFieldMapping("DisplayRate")]
		public string DisplayRate{get;set;}
		 

        
        [ORFieldMapping("ActualRate")]
		public decimal ActualRate{get;set;}
		 

        
        [ORFieldMapping("PlanDate")]
		public DateTime PlanDate{get;set;}
		 

        
        [ORFieldMapping("LimitingDate")]
		public DateTime LimitingDate{get;set;}
		 

        
        [ORFieldMapping("MissTargetReason")]
		public string MissTargetReason{get;set;}
		 

        
        [ORFieldMapping("ArrearsDetail")]
		public string ArrearsDetail{get;set;}
		 

        
        [ORFieldMapping("Counter")]
		public int Counter{get;set;}
		 

		#endregion
		
		 
	} 
}

