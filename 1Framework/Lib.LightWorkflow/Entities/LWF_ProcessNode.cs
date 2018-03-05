using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.LightWorkflow.Entities
{

    /// <summary>
    /// This object represents the properties and methods of a Process.
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.LWF_ProcessNode")]
    public partial class ProcessNode : BaseModel
    {

        #region Public Properties



        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("NodeName")]
        public string NodeName { get; set; }



        [ORFieldMapping("NodeSeq")]
        public int NodeSeq { get; set; }



        [ORFieldMapping("Expression")]
        public string Expression { get; set; }



        [ORFieldMapping("NodeType")]
        public int NodeType { get; set; }



        [ORFieldMapping("IsHandSign")]
        public bool IsHandSign { get; set; }



        [ORFieldMapping("RoleID")]
        public int RoleID { get; set; }

        [NoMapping]
        public string RoleName { get; set; }



        [ORFieldMapping("Description")]
        public string Description { get; set; }



        [ORFieldMapping("ProcessType")]
        public string ProcessType { get; set; }


        #endregion


    } 

}
