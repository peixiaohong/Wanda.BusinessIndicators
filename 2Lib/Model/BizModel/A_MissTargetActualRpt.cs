using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a MissTargetActualRpt.
	/// </summary>
    [ORTableMapping("dbo.A_MissTargetActualRpt")]
    public class A_MissTargetActualRpt : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("CompanyID")]
		public Guid CompanyID{get;set;}
		 

        
        [ORFieldMapping("TargetActualRptID")]
		public Guid TargetActualRptID{get;set;}
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}
		 

        
        [ORFieldMapping("FirstMissTargetMonth")]
		public int FirstMissTargetMonth{get;set;}
		 

        
        [ORFieldMapping("Counter")]
		public int Counter{get;set;}
		 

        
        [ORFieldMapping("PlanDate")]
		public DateTime PlanDate{get;set;}
		 

        
        [ORFieldMapping("LimitingDate")]
		public DateTime LimitingDate{get;set;}
		 

        
        [ORFieldMapping("ArrearsDetail")]
		public string ArrearsDetail{get;set;}
		 

        
        [ORFieldMapping("ArrearsStatus")]
		public string ArrearsStatus{get;set;}
		 

		#endregion
		
		 
	} 
}

