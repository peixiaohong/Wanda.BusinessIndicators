using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a System.
	/// </summary>
    [ORTableMapping("dbo.C_System")]
    public class C_System : BaseModel
	{
	 
		#region Public Properties 
		 
		
        [ORFieldMapping("SystemName")]
		public string SystemName{get;set;}
		 

        [ORFieldMapping("Category")]
        public int Category { get; set; }

        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }



        [ORFieldMapping("Configuration")]
        [NonSerialized]
        public System.Xml.Linq.XElement Configuration;


        [ORFieldMapping("GroupType")]
        public string GroupType { get; set; }

        [ORFieldMapping("VersionStart")]
        public DateTime VersionStart { get; set; }

        [ORFieldMapping("VersionEnd")]
        public DateTime VersionEnd { get; set; }

        #endregion

        [NoMapping]
        public string Config { get; set; }
		

        [NoMapping]
        public List<C_Company> Companies { get; set; }
		 
	} 
}

