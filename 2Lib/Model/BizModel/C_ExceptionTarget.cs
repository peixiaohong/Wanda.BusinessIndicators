using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a ExceptionTarget.
	/// </summary>
    [ORTableMapping("dbo.C_ExceptionTarget")]
    public class C_ExceptionTarget : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("CompanyID")]
		public Guid CompanyID{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}
		 

        
        [ORFieldMapping("ExceptionType")]
		public int ExceptionType{get;set;}
		 
		#endregion
		
		 
	} 
}

