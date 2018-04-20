using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a MonthlyReport.
    /// </summary>
    [ORTableMapping("dbo.B_MonthlyReport")]
    public class B_MonthlyReport : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }



        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }



        [ORFieldMapping("FinMonth")]
        public int FinMonth { get; set; }



        [ORFieldMapping("Description")]
        public string Description { get; set; }

        [ORFieldMapping("status")]
        public int Status { get; set; }

        /// <summary>
        /// ������״̬
        /// </summary>
        [ORFieldMapping("WFStatus")]
        public string WFStatus { get; set; }


        /// <summary>
        /// ʵʱ�ϱ��������
        /// </summary>
        [ORFieldMapping("ReportApprove")]
        public string ReportApprove { get; set; }
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
        /// �ж��Ʒ�����
        /// </summary>
        [ORFieldMapping("CreatorID")]
        public int CreatorID{get;set;}

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// �����Ż����洢��ReportInstance��Json����
        /// </summary>
        [ORFieldMapping("DataOptimizationJson")]
        public String DataOptimizationJson { get; set; }


        /// <summary>
        /// ����ID
        /// </summary>
        //[ORFieldMapping("AreaID")]
        //public Guid AreaID { get; set; }

    #endregion


    }
}

