using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a MonthlyReport.
	/// </summary>
    [ORTableMapping("dbo.A_MonthlyReport")]
    public class A_MonthlyReport : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("FinMonth")]
		public int FinMonth{get;set;}
		 

        
        [ORFieldMapping("Description")]
		public string Description{get;set;}

        [ORFieldMapping("status")]
        public int Status { get; set; }


        /// <summary>
        /// 批次ID
        /// </summary>
        [ORFieldMapping("SystemBatchID")]
        public Guid SystemBatchID { get; set; }

        /// <summary>
        /// 大区ID
        /// </summary>
        [ORFieldMapping("AreaID")]
        public Guid AreaID { get; set; }



        /// <summary>
        /// 指标计划ID
        /// </summary>
        [ORFieldMapping("TargetPlanID")]
        public Guid TargetPlanID { get; set; }

        #endregion


    }
}

