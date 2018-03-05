using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a InterfaceInstance.
	/// </summary>
    [ORTableMapping("dbo.S_InterfaceInstance")]
    public class S_InterfaceInstance : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("InterfaceID")]
		public string InterfaceID{get;set;}
		 

        
        [ORFieldMapping("InterfaceName")]
		public string InterfaceName{get;set;}
		 

        
        [ORFieldMapping("InterfaceInstanceName")]
		public string InterfaceInstanceName{get;set;}
		 

        
        [ORFieldMapping("Reference")]
		public string Reference{get;set;}
		 

        
        [ORFieldMapping("Description")]
		public string Description{get;set;}
		 

        

		 
		#endregion
		
		 
	} 
}

