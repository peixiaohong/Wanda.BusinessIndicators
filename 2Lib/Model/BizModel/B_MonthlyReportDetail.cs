using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;
 

namespace LJTH.BusinessIndicators.Model
{
	/// <summary>
	/// This object represents the properties and methods of a MonthlyReportDetail.
	/// </summary>
    [ORTableMapping("dbo.B_MonthlyReportDetail")]
    public class B_MonthlyReportDetail : BaseModel
	{
	 
		#region Public Properties 
	      
        [ORFieldMapping("SystemID")]
		public Guid SystemID{get;set;}
		 

        
        [ORFieldMapping("FinYear")]
		public int FinYear{get;set;}
		 

        
        [ORFieldMapping("FinMonth")]
		public int FinMonth{get;set;}
		 

        
        [ORFieldMapping("TargetID")]
		public Guid TargetID{get;set;}

        [NoMapping]
        public string TargetName { get; set; }

        
        [ORFieldMapping("CompanyID")]
		public Guid CompanyID{get;set;}
		 

        
        [ORFieldMapping("TargetPlanID")]
		public Guid TargetPlanID{get;set;}
		 

        
        [ORFieldMapping("OPlanAmmount")]
		public decimal OPlanAmmount{get;set;}
		 

        
        [ORFieldMapping("OActualAmmount")]
		public decimal OActualAmmount{get;set;}
		 

        
        [ORFieldMapping("OActualRate")]
        public string OActualRate { get; set; }
		 

        
        [ORFieldMapping("ODisplayRate")]
		public string ODisplayRate{get;set;}
		 

        
        [ORFieldMapping("NPlanAmmount")]
		public decimal NPlanAmmount{get;set;}
		 

        
        [ORFieldMapping("NActualAmmount")]
		public decimal NActualAmmount{get;set;}

     
        
        [ORFieldMapping("NActualRate")]
        public string NActualRate { get; set; }
		 

        
        [ORFieldMapping("NDisplayRate")]
		public string NDisplayRate{get;set;}
		 

        
        [ORFieldMapping("OAccumulativePlanAmmount")]
		public decimal OAccumulativePlanAmmount{get;set;}
		 

        
        [ORFieldMapping("OAccumulativeActualAmmount")]
		public decimal OAccumulativeActualAmmount{get;set;}
		 

        
        [ORFieldMapping("OAccumulativeActualRate")]
        public string OAccumulativeActualRate { get; set; }
		 

        
        [ORFieldMapping("OAcccumulativeDisplayRate")]
		public string OAcccumulativeDisplayRate{get;set;}
		 

        
        [ORFieldMapping("NAccumulativePlanAmmount")]
		public decimal NAccumulativePlanAmmount{get;set;}
		 

        
        [ORFieldMapping("NAccumulativeActualAmmount")]
		public decimal NAccumulativeActualAmmount{get;set;}


        
        [ORFieldMapping("NAccumulativeActualRate")]
        public string NAccumulativeActualRate { get; set; }
		 

        
        [ORFieldMapping("NAccumulativeDisplayRate")]
        public string NAccumulativeDisplayRate { get; set; }
		 

        
        [ORFieldMapping("IsMissTarget")]
		public bool IsMissTarget{get;set;}

        /// <summary>
        /// ��ǰ��δ��ɱ�ʶ
        /// </summary>
        [ORFieldMapping("IsMissTargetCurrent")]
        public bool IsMissTargetCurrent { get; set; }


        
        [ORFieldMapping("Counter")]
		public int Counter{get;set;}
		 

        
        [ORFieldMapping("FirstMissTargetDate")]
		public DateTime? FirstMissTargetDate{get;set;}
		 

        
        [ORFieldMapping("PromissDate")]
		public DateTime? PromissDate{get;set;}
		 

        /// <summary>
        /// Ҫ��ʱ��
        /// </summary>
        [ORFieldMapping("CommitDate")]
		public DateTime? CommitDate{get;set;}
		 

        /// <summary>
        /// δ���˵��
        /// </summary>
        [ORFieldMapping("MIssTargetReason")]
		public string MIssTargetReason{get;set;}
		 

        /// <summary>
        /// ��ȡ��ʩ
        /// </summary>
        [ORFieldMapping("MIssTargetDescription")]
		public string MIssTargetDescription{get;set;}




        /// <summary>
        /// ����δ���˵��
        /// </summary>
        [ORFieldMapping("CurrentMIssTargetReason")]
        public string CurrentMIssTargetReason { get; set; }


        /// <summary>
        /// ���²�ȡ��ʩ
        /// </summary>
        [ORFieldMapping("CurrentMIssTargetDescription")]
        public string CurrentMIssTargetDescription { get; set; }





        [ORFieldMapping("ReturnType")]
		public int ReturnType{get;set;}


        /// <summary>
        /// ��ֵ(��)
        /// </summary>
        [ORFieldMapping("NDifference")]
        public decimal NDifference { get; set; }

        /// <summary>
        /// ��ֵ(��)
        /// </summary>
        [ORFieldMapping("ODifference")]
        public decimal ODifference { get; set; }

        /// <summary>
        /// �ۼƲ�ֵ(��)
        /// </summary>
        [ORFieldMapping("NAccumulativeDifference")]
        public decimal NAccumulativeDifference { get; set; }

        /// <summary>
        /// �ۼƲ�ֵ(��)
        /// </summary>
        [ORFieldMapping("OAccumulativeDifference")]
        public decimal OAccumulativeDifference { get; set; }

        /// <summary>
        /// �¶��ϱ�ID 
        /// </summary>
        [ORFieldMapping("MonthlyReportID")]
        public Guid MonthlyReportID { get; set; }


        /// <summary>
        /// ���µĿ���ָ��
        /// </summary>
        [ORFieldMapping("MeasureRate")]
        public decimal MeasureRate { get; set; }


        /// <summary>
        /// �������
        /// </summary>
        [ORFieldMapping("ReturnDescription")]
        public string ReturnDescription { get; set; }

        /// <summary>
        /// �Ƿ���ҳ����ʾ
        /// </summary>
        [ORFieldMapping("Display")]
        public bool Display { get; set; }
		#endregion

        [ORFieldMapping("CompanyProperty1")]
        public string CompanyProperty1 { get; set; }

        /// <summary>
        /// �Ƿ��ǵ���Ҫ��ʱ��
        /// </summary>
        [ORFieldMapping("IsCommitDate")]
        public int IsCommitDate { get; set; }


        /// <summary>
        /// Ҫ��˵��(��û��Ҫ��ʱ���ʱ����ʾ)
        /// </summary>
        [ORFieldMapping("CommitReason")]
        public string CommitReason { get; set; }


        /// <summary>
        /// ����δ���Ҫ��ʱ�� //---ҵ��������õ��ֶ�
        /// </summary>
        [ORFieldMapping("CurrentMonthCommitDate")]
        public DateTime? CurrentMonthCommitDate { get; set; }


        /// <summary>
        /// ����δ���Ҫ��˵�� //---ҵ��������õ��ֶ�
        /// </summary>
        [ORFieldMapping("CurrentMonthCommitReason")]
        public string CurrentMonthCommitReason { get; set; }


        /// <summary>
        /// ����״̬����״̬��
        /// </summary>
        [ORFieldMapping("ReturnType_Sub")]
        public string ReturnType_Sub { get; set; }


        /// <summary>
        /// ���¼����Counter
        /// </summary>
        [ORFieldMapping("NewCounter")]
        public int NewCounter { get; set; }


        /// <summary>
        /// ��˾����Json
        /// </summary>
        [ORFieldMapping("CompanyProperty")]
        public string CompanyProperty{get;set;}

	} 
}

