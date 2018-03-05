using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a SysConfig.
	/// </summary>
    [ORTableMapping("dbo.Sys_Config")]
    public class Sys_Config : BaseModel
	{
	 
		#region Public Properties 
		 
    
        [ORFieldMapping("Biz_Type")]
		public string Biz_Type{get;set;}
		 

        
        [ORFieldMapping("Biz_Value")]
		public string Biz_Value{get;set;}
		 

        
        [ORFieldMapping("ConfigReason")]
        public System.Xml.Linq.XElement ConfigReason { get; set; }
		 

        
        [ORFieldMapping("Sequence")]
		public int Sequence{get;set;}
		 

		#endregion
		
		 
	} 
}

