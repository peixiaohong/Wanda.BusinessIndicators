using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a TargetMapping.
	/// </summary>
    [ORTableMapping("dbo.C_TargetMapping")]
    public class C_TargetMapping : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("CompanyID")]
		public Guid CompanyID{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}
		 

        
        [ORFieldMapping("TargetName")]
		public string TargetName{get;set;}
		 

        
        [ORFieldMapping("NeedEvaluation")]
		public bool NeedEvaluation{get;set;}
		 

        
        [ORFieldMapping("TargetType")]
		public string TargetType{get;set;}
		 

		#endregion
		
		 
	} 
}

