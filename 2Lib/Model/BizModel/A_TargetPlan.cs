using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a TargetPlan.
	/// </summary>
    [ORTableMapping("dbo.A_TargetPlan")]
    public class A_TargetPlan : BaseModel
	{
	 
		#region Public Properties 
		 
		
        
        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("Description")]
		public string Description{get;set;}
		 

        
        [ORFieldMapping("VersionStart")]
		public DateTime VersionStart{get;set;}
		 

        
        [ORFieldMapping("Versionend")]
		public DateTime Versionend{get;set;}

        [ORFieldMapping("status")]
        public int Status { get; set; }

        /// <summary>
        /// ������״̬
        /// </summary>
        [ORFieldMapping("WFStatus")]
        public string WFStatus { get; set; }


        /// <summary>
        /// ��ǰ�ύ��
        /// </summary>
        [ORFieldMapping("ProcessOwn")]
        public string ProcessOwn { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [ORFieldMapping("SystemBatchID")]
        public Guid SystemBatchID { get; set; }

        /// <summary>
        /// �汾����
        /// </summary>
        [ORFieldMapping("VensionName")]
        public string VensionName { get; set; }


        /// <summary>
        /// �Ƿ�Ĭ�ϰ汾
        /// </summary>
        [ORFieldMapping("VensionDefault")]
        public int VensionDefault { get; set; }
        #endregion


    } 
}

