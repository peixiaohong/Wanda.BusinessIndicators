using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a MissTargetCategory.
	/// </summary>
    [ORTableMapping("dbo.C_MissTargetCategory")]
    public class C_MissTargetCategory : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("MissTargetCategoryName")]
		public string MissTargetCategoryName{get;set;}
		 

        
        [ORFieldMapping("MissTargetCategoryExpression")]
		public string MissTargetCategoryExpression{get;set;}
		 

        
        [ORFieldMapping("Sequence")]
		public int Sequence{get;set;}
		 

		#endregion
		
		 
	} 
}

