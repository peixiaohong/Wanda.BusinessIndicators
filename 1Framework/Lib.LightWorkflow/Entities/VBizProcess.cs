using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Entities
{
    /// <summary>
    /// This object represents the properties and methods of a BizProcess.
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.B_BizProcess")]
    public class VBizProcess : IBaseComposedModel
    {

        #region Public Properties



        [ORFieldMapping("TaskID")]
        public int TaskID { get; set; }



        [ORFieldMapping("ProcessType")]
        public short ProcessType { get; set; }



        [ORFieldMapping("Title")]
        public string Title { get; set; }



        [ORFieldMapping("ParentID")]
        public int ParentID { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("IsCurrentVersion")]
        public bool IsCurrentVersion { get; set; }



        [ORFieldMapping("AlarmAction")]
        public short AlarmAction { get; set; }



        [ORFieldMapping("AlarmPlanSplitDate")]
        public DateTime AlarmPlanSplitDate { get; set; }


        #endregion


    }
}
