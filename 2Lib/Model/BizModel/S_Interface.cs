using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a Interface.
	/// </summary>
    [ORTableMapping("dbo.S_Interface")]
    public class S_Interface : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("InterfaceName")]
		public string InterfaceName{get;set;}
		 

        
        [ORFieldMapping("Description")]
		public string Description{get;set;}
		 

        
        [ORFieldMapping("IsDefault")]
		public bool IsDefault{get;set;}
		 


		 


		 
		#endregion
		
		 
	} 
}

