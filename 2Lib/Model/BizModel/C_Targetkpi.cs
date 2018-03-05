using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
using System.Xml.Linq;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a TargetKpi.
	/// </summary>
    [ORTableMapping("dbo.C_TargetKpi")]
    public class C_TargetKpi:BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}
		 

        
        [ORFieldMapping("TargetName")]
		public string TargetName{get;set;}
		 

        
        [ORFieldMapping("NeedEvaluation")]
		public bool NeedEvaluation{get;set;}
		 

        
        [ORFieldMapping("TargetType")]
		public string TargetType{get;set;}
		 

        
        [ORFieldMapping("MeasureRate")]
		public decimal MeasureRate{get;set;}


        [ORFieldMapping("MeasureRateUnit")]
        public string MeasureRateUnit { get; set; }
        
        [ORFieldMapping("Sequence")]
		public int Sequence{get;set;}
		 

        
        [ORFieldMapping("VersionStart")]
		public DateTime VersionStart{get;set;}
		 

        
        [ORFieldMapping("VersionEnd")]
		public DateTime VersionEnd{get;set;}
		 

        
        [ORFieldMapping("Configuration")]
		public XElement Configuration{get;set;}
		 

		#endregion
		
		 
	} 
}

