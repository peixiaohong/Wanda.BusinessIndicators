using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{
    [Serializable]
    [ORTableMapping("dbo.LWF_Process")]
    public partial class Process : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("ProcessCode")]
        public string ProcessCode { get; set; }



        [ORFieldMapping("ProcessName")]
        public string ProcessName { get; set; }



        [ORFieldMapping("IsActived")]
        public bool IsActived { get; set; }



        [ORFieldMapping("Description")]
        public string Description { get; set; }

        [ORFieldMapping("CongID")]
        public int CongID { get { return 1; } set { value = 1; } }

        [NoMapping]
        public string CongName { get; set; }


        #endregion


    } 
}
