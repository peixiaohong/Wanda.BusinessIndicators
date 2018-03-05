using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
 

namespace Wanda.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a Company.
	/// </summary>
    [ORTableMapping("dbo.C_Company")]
    public class C_Company : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("CompanyName")]
		public string CompanyName{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty1")]
		public string CompanyProperty1{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty2")]
		public string CompanyProperty2{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty3")]
		public string CompanyProperty3{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty4")]
		public string CompanyProperty4{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty5")]
		public string CompanyProperty5{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty6")]
		public string CompanyProperty6{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty7")]
		public string CompanyProperty7{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty8")]
		public string CompanyProperty8{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty9")]
		public string CompanyProperty9{get;set;}
		 

        
        [ORFieldMapping("CompanyProperty10")]
		public System.Xml.Linq.XElement CompanyProperty10{get;set;}
		 

        
        [ORFieldMapping("Sequence")]
		public int Sequence{get;set;}
		 

        
        [ORFieldMapping("NeedEvaluation")]
		public int NeedEvaluation{get;set;}
		 

        
        [ORFieldMapping("VersionStart")]
		public DateTime VersionStart{get;set;}
		 

        
        [ORFieldMapping("VersionEnd")]
		public DateTime VersionEnd{get;set;}

        [ORFieldMapping("OpeningTime")]
        public DateTime OpeningTime { get; set; }

		#endregion
		
		 
	}




    public class TargetTypeEnum
    {
        public bool IfSelect { get; set; }
        public int TargetType { get; set; }
        public string TargetTypeValue { get; set; }
    }
}

